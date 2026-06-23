using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Auth;
using EIskele.Application.Common.Results;
using EIskele.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EIskele.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly EIskele.Application.Common.Settings.ISecuritySettingsService _securitySettingsService;

    public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, EIskele.Application.Common.Settings.ISecuritySettingsService securitySettingsService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _securitySettingsService = securitySettingsService;
    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return Result<AuthResponse>.Failure("AUTH.EMAIL_EXISTS", "Bu e-posta adresi ile kayıtlı bir kullanıcı zaten var.");
        }

        var securitySettingsResult = await _securitySettingsService.GetSecuritySettingsAsync(cancellationToken);
        var securitySettings = securitySettingsResult.IsSuccess ? securitySettingsResult.Value : new EIskele.Application.Common.Settings.SecuritySettingsDto();

        if (request.Password.Length < securitySettings.PasswordMinimumLength)
        {
            return Result<AuthResponse>.Failure("AUTH.PASSWORD_TOO_SHORT", $"Şifre en az {securitySettings.PasswordMinimumLength} karakter olmalıdır.");
        }

        if (securitySettings.RequirePasswordComplexity)
        {
            bool hasUpper = request.Password.Any(char.IsUpper);
            bool hasLower = request.Password.Any(char.IsLower);
            bool hasDigit = request.Password.Any(char.IsDigit);
            bool hasSpecial = request.Password.Any(ch => !char.IsLetterOrDigit(ch));

            if (!hasUpper || !hasLower || !hasDigit || !hasSpecial)
            {
                return Result<AuthResponse>.Failure("AUTH.PASSWORD_COMPLEXITY", "Şifre en az bir büyük harf, bir küçük harf, bir rakam ve bir özel karakter içermelidir.");
            }
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result<AuthResponse>.Failure("AUTH.REGISTER_FAILED", $"Kayıt işlemi başarısız: {errors}");
        }

        // Varsayılan rol atama (Örn: Customer)
        await _userManager.AddToRoleAsync(user, "Customer");

        return await LoginAsync(new LoginRequest { Email = request.Email, Password = request.Password }, cancellationToken);
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Result<AuthResponse>.Failure("AUTH.INVALID_CREDENTIALS", "Geçersiz e-posta veya şifre.");
        }

        var securitySettingsResult = await _securitySettingsService.GetSecuritySettingsAsync(cancellationToken);
        var securitySettings = securitySettingsResult.IsSuccess ? securitySettingsResult.Value : new EIskele.Application.Common.Settings.SecuritySettingsDto();

        if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow)
        {
            return Result<AuthResponse>.Failure("AUTH.ACCOUNT_LOCKED", "Hesabınız çok fazla başarısız deneme nedeniyle kilitlenmiştir.");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        
        if (!result.Succeeded)
        {
            user.AccessFailedCount++;
            if (user.AccessFailedCount >= securitySettings.MaxFailedLoginAttempts)
            {
                user.LockoutEnd = DateTimeOffset.UtcNow.AddMinutes(securitySettings.AccountLockoutMinutes);
                user.AccessFailedCount = 0;
                await _userManager.UpdateAsync(user);
                return Result<AuthResponse>.Failure("AUTH.ACCOUNT_LOCKED", "Hesabınız çok fazla başarısız deneme nedeniyle kilitlenmiştir.");
            }
            await _userManager.UpdateAsync(user);
            return Result<AuthResponse>.Failure("AUTH.INVALID_CREDENTIALS", "Geçersiz e-posta veya şifre.");
        }

        // Başarılı girişte sayacı ve kilidi sıfırla
        user.AccessFailedCount = 0;
        user.LockoutEnd = null;
        await _userManager.UpdateAsync(user);

        var roles = await _userManager.GetRolesAsync(user);
        int sessionMinutes = securitySettings.AdminSessionMinutes > 0 ? securitySettings.AdminSessionMinutes : _configuration.GetValue<int>("Jwt:AccessTokenMinutes", 60);

        var tokenString = await GenerateJwtTokenAsync(user, roles, sessionMinutes);

        var response = new AuthResponse
        {
            AccessToken = tokenString,
            RefreshToken = "not-implemented-yet",
            ExpiresIn = sessionMinutes * 60
        };

        return Result<AuthResponse>.Success(response);
    }

    public Task<Result<AuthResponse>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result<AuthResponse>.Failure("NOT_IMPLEMENTED", "Metod henüz kodlanmadı."));
    }

    private Task<string> GenerateJwtTokenAsync(ApplicationUser user, IList<string> roles, int sessionMinutes)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim("FirstName", user.FirstName),
            new Claim("LastName", user.LastName)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(sessionMinutes),
            signingCredentials: creds
        );

        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }
}
