using System;
using EIskele.Application.Packages;
using EIskele.Persistence;

namespace EIskele.Infrastructure.Services;

public partial class TourPackageService : ITourPackageService
{
    private readonly EIskeleDbContext _context;

    public TourPackageService(EIskeleDbContext context)
    {
        _context = context;
    }
}
