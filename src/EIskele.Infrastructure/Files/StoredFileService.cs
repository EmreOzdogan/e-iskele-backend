using System;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Files;
using EIskele.Application.Common.Results;
using EIskele.Domain.Entities;
using EIskele.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EIskele.Infrastructure.Files;

public class StoredFileService : IStoredFileService
{
    private readonly EIskeleDbContext _dbContext;

    public StoredFileService(EIskeleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<StoredFile>> GetFileRecordAsync(Guid fileId, Guid currentUserId, bool isAdmin, CancellationToken cancellationToken = default)
    {
        var file = await _dbContext.Set<StoredFile>().FindAsync(new object[] { fileId }, cancellationToken);
        
        if (file == null || file.IsDeleted)
        {
            return Result<StoredFile>.Failure("FILE_NOT_FOUND", "Dosya bulunamadı veya silinmiş.");
        }

        if (!file.IsPublic)
        {
            if (currentUserId == Guid.Empty)
            {
                return Result<StoredFile>.Failure("UNAUTHORIZED", "Bu dosyayı görüntülemek için giriş yapmalısınız.");
            }

            if (!isAdmin && file.OwnerUserId != currentUserId)
            {
                return Result<StoredFile>.Failure("FORBIDDEN", "Bu dosyayı görüntüleme yetkiniz yok.");
            }
        }

        return Result<StoredFile>.Success(file);
    }
}
