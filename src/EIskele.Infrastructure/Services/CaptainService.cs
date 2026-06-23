using System;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Captains;
using EIskele.Application.Common.Results;
using EIskele.Domain.Entities;
using EIskele.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EIskele.Infrastructure.Services;

public partial class CaptainService : ICaptainService
{
    private readonly EIskeleDbContext _dbContext;
    private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
    private readonly EIskele.Application.Common.Notifications.INotificationService _notificationService;
    private readonly EIskele.Application.Common.Files.IFileStorageService _fileStorageService;

    public CaptainService(
        EIskeleDbContext dbContext, 
        Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager,
        EIskele.Application.Common.Notifications.INotificationService notificationService,
        EIskele.Application.Common.Files.IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _notificationService = notificationService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<CaptainApplicationResponse>> ApplyAsync(Guid userId, CaptainApplicationRequest request, CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
        {
            var email = request.ApplicationType == "company" ? request.Company?.Email : request.Individual?.Email;
            var phone = request.ApplicationType == "company" ? request.Company?.Phone : request.Individual?.Phone;
            var fullName = request.ApplicationType == "company" ? request.Company?.AuthorizedPersonFullName : request.Individual?.FullName;
            var password = request.ApplicationType == "company" ? request.Company?.Password : request.Individual?.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return Result<CaptainApplicationResponse>.Failure("VALIDATION_ERROR", "E-posta ve şifre zorunludur.");
            }

            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                return Result<CaptainApplicationResponse>.Failure("USER_EXISTS", "Bu e-posta adresi ile zaten bir kullanıcı mevcut.");
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = fullName?.Split(' ').FirstOrDefault() ?? "Kaptan",
                LastName = string.Join(" ", fullName?.Split(' ').Skip(1) ?? Array.Empty<string>()),
                PhoneNumber = phone ?? ""
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result<CaptainApplicationResponse>.Failure("USER_CREATE_ERROR", $"Kullanıcı oluşturulamadı: {errors}");
            }

            await _userManager.AddToRoleAsync(user, "Captain");
            userId = user.Id;
        }

        var existingCaptain = await _dbContext.Captains.FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
        if (existingCaptain != null)
        {
            return Result<CaptainApplicationResponse>.Failure("CAPTAIN.ALREADY_APPLIED", "Bu kullanıcı zaten kaptanlık başvurusunda bulunmuş.");
        }

        var captainId = Guid.NewGuid();
        var applicationNo = $"KPT-{DateTime.UtcNow.Year}-{new Random().Next(1000, 9999)}";

        var captain = new Captain
        {
            Id = captainId,
            UserId = userId,
            ApplicationNo = applicationNo,
            ApplicationType = request.ApplicationType.ToLower() == "company" ? EIskele.Domain.Enums.CaptainApplicationType.Company : EIskele.Domain.Enums.CaptainApplicationType.Individual,
            IdentityNumber = request.Individual?.IdentityNumber ?? string.Empty,
            LicenseNumber = string.Empty, // Gelecekte belge okuma ile dolabilir
            Status = EIskele.Domain.Enums.CaptainStatus.UnderReview,
            AccountStatus = EIskele.Domain.Enums.CaptainAccountStatus.Active,
            Address = request.ApplicationType == "company" ? request.Company?.Address ?? "" : request.Individual?.Address ?? "",
            Iban = request.Payout?.Iban ?? string.Empty
        };

        if (request.ApplicationType == "company" && request.Company != null)
        {
            var company = new Company
            {
                Id = Guid.NewGuid(),
                CaptainId = captainId,
                CompanyName = request.Company.CompanyTitle,
                AuthorizedPersonName = request.Company.AuthorizedPersonFullName,
                TaxOffice = request.Company.TaxOffice,
                TaxNumber = request.Company.TaxNumber,
                Address = request.Company.Address
            };
            _dbContext.Companies.Add(company);
        }

        var boat = new Boat
        {
            Id = Guid.NewGuid(),
            CaptainId = captainId,
            LocationId = request.Boat.LocationId,
            HarborId = request.Boat.HarborId == Guid.Empty ? null : request.Boat.HarborId,
            Name = request.Boat.Name,
            Capacity = request.Boat.Capacity,
            Status = EIskele.Domain.Enums.BoatStatus.Draft
        };
        _dbContext.Boats.Add(boat);

        // Upload edilmiş dosyaları Captain kaydına bağlayalım
        foreach (var docKV in request.Documents)
        {
            var storedFile = await _dbContext.StoredFiles.FindAsync(new object[] { docKV.Value }, cancellationToken);
            if (storedFile != null)
            {
                storedFile.RelatedEntityId = captainId.ToString();
                // Opsiyonel: storedFile.RelatedEntityType = "Captain";
            }
        }

        _dbContext.Captains.Add(captain);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Kaptan başvurusu alındı bildirimi gönder (Email)
        var captainName = request.ApplicationType == "company" 
            ? request.Company?.AuthorizedPersonFullName ?? "Kaptan"
            : request.Individual?.FullName ?? "Kaptan";

        var notificationRequest = new EIskele.Application.Common.Notifications.NotificationRequest
        {
            UserId = userId,
            TemplateCode = "CAPTAIN_APPLICATION_RECEIVED",
            Channel = "Email",
            Parameters = new System.Collections.Generic.Dictionary<string, string>
            {
                { "ApplicationNo", captain.ApplicationNo },
                { "CaptainName", captainName }
            }
        };
        
        await _notificationService.SendAsync(notificationRequest, cancellationToken);

        return Result<CaptainApplicationResponse>.Success(new CaptainApplicationResponse
        {
            ApplicationId = captain.Id,
            ApplicationNo = captain.ApplicationNo,
            Status = captain.Status.ToString()
        });
    }

    public async Task<Result> ApproveApplicationAsync(Guid applicationId, CancellationToken cancellationToken = default)
    {
        var captain = await _dbContext.Captains.FindAsync(new object[] { applicationId }, cancellationToken);
        if (captain == null)
        {
            return Result.Failure("NOT_FOUND", "Başvuru bulunamadı.");
        }

        if (captain.Status == EIskele.Domain.Enums.CaptainStatus.Approved)
        {
            return Result.Failure("CAPTAIN.ALREADY_APPROVED", "Bu başvuru zaten onaylanmış.");
        }

        captain.Status = EIskele.Domain.Enums.CaptainStatus.Approved;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> RejectApplicationAsync(Guid applicationId, string reason, CancellationToken cancellationToken = default)
    {
        var captain = await _dbContext.Captains.FindAsync(new object[] { applicationId }, cancellationToken);
        if (captain == null)
        {
            return Result.Failure("NOT_FOUND", "Başvuru bulunamadı.");
        }

        captain.Status = EIskele.Domain.Enums.CaptainStatus.Rejected;
        captain.AdminNote = reason;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
