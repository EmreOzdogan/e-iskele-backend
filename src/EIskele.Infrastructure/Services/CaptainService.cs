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

    public async Task<Result<CaptainApplicationResponse>> ApplyAsync(CaptainApplicationRequest request, CancellationToken cancellationToken = default)
    {
        var existingCaptain = await _dbContext.Captains.FirstOrDefaultAsync(c => c.UserId == request.UserId, cancellationToken);
        if (existingCaptain != null)
        {
            return Result<CaptainApplicationResponse>.Failure("CAPTAIN.ALREADY_APPLIED", "Bu kullanıcı zaten kaptanlık başvurusunda bulunmuş.");
        }

        var captain = new Captain
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            LicenseNumber = request.LicenseNumber,
            Status = "UnderReview",
            Bio = request.Notes ?? string.Empty
        };

        _dbContext.Captains.Add(captain);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<CaptainApplicationResponse>.Success(new CaptainApplicationResponse
        {
            ApplicationId = captain.Id,
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
