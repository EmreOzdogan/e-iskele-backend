using System;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Data;
using EIskele.Application.Common.Results;
using EIskele.Application.Common.Settings;
using EIskele.Application.Reservations;
using EIskele.Domain.Enums;
using EIskele.Infrastructure.Emails.Services;
using EIskele.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EIskele.Infrastructure.Services;

public class AdminReservationCommandService : IAdminReservationCommandService
{
    private readonly EIskeleDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISmtpEmailSettingsService _smtpEmailSettingsService;
    private readonly IEmailSender _emailSender;

    public AdminReservationCommandService(
        EIskeleDbContext dbContext,
        IUnitOfWork unitOfWork,
        ISmtpEmailSettingsService smtpEmailSettingsService,
        IEmailSender emailSender)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _smtpEmailSettingsService = smtpEmailSettingsService;
        _emailSender = emailSender;
    }

    public async Task<Result> AdminCancelReservationAsync(Guid id, string reason, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var reservation = await _dbContext.Reservations
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
                
            if (reservation == null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure("NOT_FOUND", "Rezervasyon bulunamadı.");
            }

            reservation.Status = ReservationStatus.Cancelled;
            
            // Send email
            if (reservation.Customer != null && !string.IsNullOrWhiteSpace(reservation.Customer.Email))
            {
                var smtpSettingsResult = await _smtpEmailSettingsService.GetSmtpEmailSettingsAsync(cancellationToken);
                if (smtpSettingsResult.IsSuccess && smtpSettingsResult.Value != null && smtpSettingsResult.Value.SmtpEnabled)
                {
                    var htmlBody = $@"
                        <h2>Rezervasyonunuz İptal Edildi</h2>
                        <p>Sayın {reservation.Customer.FirstName},</p>
                        <p><strong>{reservation.ReservationNo}</strong> numaralı rezervasyonunuz iptal edilmiştir.</p>
                        <p><strong>İptal Sebebi:</strong> {reason}</p>
                        <p>İyi günler dileriz.</p>";

                    try
                    {
                        await _emailSender.SendAsync(
                            reservation.Customer.Email, 
                            "e-iskele: Rezervasyonunuz İptal Edildi", 
                            htmlBody, 
                            smtpSettingsResult.Value, 
                            null, 
                            cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Email send failed: {ex.Message}");
                    }
                }
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return Result.Success();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task<Result> AdminPostponeReservationAsync(Guid id, DateTime newDate, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var reservation = await _dbContext.Reservations
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
                
            if (reservation == null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure("NOT_FOUND", "Rezervasyon bulunamadı.");
            }

            var duration = reservation.EndDateTime - reservation.StartDateTime;
            reservation.StartDateTime = newDate;
            reservation.EndDateTime = newDate.Add(duration);
            reservation.Status = ReservationStatus.PostponedDueToWeather; 
            
            // Send email
            if (reservation.Customer != null && !string.IsNullOrWhiteSpace(reservation.Customer.Email))
            {
                var smtpSettingsResult = await _smtpEmailSettingsService.GetSmtpEmailSettingsAsync(cancellationToken);
                if (smtpSettingsResult.IsSuccess && smtpSettingsResult.Value != null && smtpSettingsResult.Value.SmtpEnabled)
                {
                    var htmlBody = $@"
                        <h2>Rezervasyonunuz Ertelendi</h2>
                        <p>Sayın {reservation.Customer.FirstName},</p>
                        <p><strong>{reservation.ReservationNo}</strong> numaralı rezervasyonunuzun tarihi güncellenmiştir.</p>
                        <p><strong>Yeni Tarih:</strong> {newDate.ToString("dd.MM.yyyy HH:mm")}</p>
                        <p>İyi günler dileriz.</p>";

                    try
                    {
                        await _emailSender.SendAsync(
                            reservation.Customer.Email, 
                            "e-iskele: Rezervasyonunuz Ertelendi", 
                            htmlBody, 
                            smtpSettingsResult.Value, 
                            null, 
                            cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Email send failed: {ex.Message}");
                    }
                }
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return Result.Success();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
