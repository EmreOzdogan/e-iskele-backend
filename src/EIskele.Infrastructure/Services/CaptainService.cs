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

    public CaptainService(EIskeleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<CaptainApplicationResponse>> ApplyAsync(Guid userId, CaptainApplicationRequest request, CancellationToken cancellationToken = default)
    {
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
            ApplicationType = request.ApplicationType,
            IdentityNumber = request.Individual?.IdentityNumber ?? string.Empty,
            LicenseNumber = string.Empty, // Gelecekte belge okuma ile dolabilir
            Status = "UnderReview",
            AccountStatus = "Active",
            Address = request.ApplicationType == "company" ? request.Company?.Address ?? "" : request.Individual?.Address ?? "",
            Iban = request.Payout.Iban
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

        return Result<CaptainApplicationResponse>.Success(new CaptainApplicationResponse
        {
            ApplicationId = captain.Id,
            ApplicationNo = captain.ApplicationNo,
            Status = captain.Status
        });
    }

    public async Task<Result> ApproveApplicationAsync(Guid applicationId, CancellationToken cancellationToken = default)
    {
        var captain = await _dbContext.Captains.FindAsync(new object[] { applicationId }, cancellationToken);
        if (captain == null)
        {
            return Result.Failure("NOT_FOUND", "Başvuru bulunamadı.");
        }

        if (captain.Status == "Approved")
        {
            return Result.Failure("CAPTAIN.ALREADY_APPROVED", "Bu başvuru zaten onaylanmış.");
        }

        captain.Status = "Approved";
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

        captain.Status = "Rejected";
        captain.AdminNote = reason;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
