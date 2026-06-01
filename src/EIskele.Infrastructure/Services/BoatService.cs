using System;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Boats;
using EIskele.Application.Common.Results;
using EIskele.Domain.Entities;
using EIskele.Domain.Enums;
using EIskele.Persistence;

namespace EIskele.Infrastructure.Services;

public class BoatService : IBoatService
{
    private readonly EIskeleDbContext _dbContext;

    public BoatService(EIskeleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<BoatResponse>> CreateBoatAsync(CreateBoatRequest request, CancellationToken cancellationToken = default)
    {
        var boat = new Boat
        {
            Id = Guid.NewGuid(),
            CaptainId = request.CaptainId,
            LocationId = request.LocationId,
            Name = request.Name,
            Capacity = request.Capacity,
            Status = BoatStatus.Draft
        };

        _dbContext.Boats.Add(boat);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<BoatResponse>.Success(new BoatResponse
        {
            Id = boat.Id,
            Status = boat.Status.ToString()
        });
    }

    public async Task<Result> SubmitForReviewAsync(Guid boatId, CancellationToken cancellationToken = default)
    {
        var boat = await _dbContext.Boats.FindAsync(new object[] { boatId }, cancellationToken);
        if (boat == null)
        {
            return Result.Failure("NOT_FOUND", "Tekne bulunamadı.");
        }

        if (boat.Status != BoatStatus.Draft && boat.Status != BoatStatus.Rejected)
        {
            return Result.Failure("BOAT.INVALID_STATUS", "Sadece taslak veya reddedilmiş tekneler incelemeye gönderilebilir.");
        }

        boat.Status = BoatStatus.UnderReview;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ApproveBoatAsync(Guid boatId, CancellationToken cancellationToken = default)
    {
        var boat = await _dbContext.Boats.FindAsync(new object[] { boatId }, cancellationToken);
        if (boat == null)
        {
            return Result.Failure("NOT_FOUND", "Tekne bulunamadı.");
        }

        if (boat.Status != BoatStatus.UnderReview)
        {
            return Result.Failure("BOAT.NOT_UNDER_REVIEW", "Tekne inceleme aşamasında değil.");
        }

        boat.Status = BoatStatus.Published;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<TourPackageResponse>> AddTourPackageAsync(CreateTourPackageRequest request, CancellationToken cancellationToken = default)
    {
        var boat = await _dbContext.Boats.FindAsync(new object[] { request.BoatId }, cancellationToken);
        if (boat == null)
        {
            return Result<TourPackageResponse>.Failure("NOT_FOUND", "Tekne bulunamadı.");
        }

        if (request.Price <= 0)
        {
            return Result<TourPackageResponse>.Failure("PACKAGE.INVALID_PRICE", "Paket fiyatı sıfır veya negatif olamaz.");
        }

        if (request.MinCapacity > request.MaxCapacity)
        {
            return Result<TourPackageResponse>.Failure("PACKAGE.INVALID_CAPACITY", "Minimum kapasite, maksimum kapasiteden büyük olamaz.");
        }

        if (request.MaxCapacity > boat.Capacity)
        {
            return Result<TourPackageResponse>.Failure("PACKAGE.CAPACITY_EXCEEDED", "Paket kapasitesi tekne kapasitesini aşamaz.");
        }

        var package = new TourPackage
        {
            Id = Guid.NewGuid(),
            BoatId = request.BoatId,
            Name = request.Name,
            Price = request.Price,
            MinCapacity = request.MinCapacity,
            MaxCapacity = request.MaxCapacity,
            IsActive = true,
            ApprovalType = ReservationApprovalType.AutoApprove
        };

        _dbContext.TourPackages.Add(package);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<TourPackageResponse>.Success(new TourPackageResponse
        {
            Id = package.Id
        });
    }
}
