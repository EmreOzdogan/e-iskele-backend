using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EIskele.Application.Common.Notifications;
using EIskele.Domain.Entities;
using EIskele.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EIskele.Infrastructure.Notifications;

public class NotificationService : INotificationService
{
    private readonly IEnumerable<INotificationSender> _senders;
    private readonly EIskeleDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public NotificationService(
        IEnumerable<INotificationSender> senders, 
        EIskeleDbContext dbContext,
        UserManager<ApplicationUser> userManager)
    {
        _senders = senders;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<NotificationResult> SendAsync(NotificationRequest request, CancellationToken cancellationToken = default)
    {
        var template = await _dbContext.NotificationTemplates
            .FirstOrDefaultAsync(t => t.Code == request.TemplateCode && t.Channel == request.Channel && t.IsActive, cancellationToken);

        if (template == null)
        {
            return new NotificationResult { Success = false, ErrorMessage = "Şablon bulunamadı veya pasif." };
        }

        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
        {
            return new NotificationResult { Success = false, ErrorMessage = "Kullanıcı bulunamadı." };
        }

        // Email senaryosu için şimdilik doğrudan User.Email kullanıyoruz.
        // İleride Sms için User.PhoneNumber vb. çekilebilir.
        var toAddress = user.Email;
        if (string.IsNullOrEmpty(toAddress))
        {
            return new NotificationResult { Success = false, ErrorMessage = "Kullanıcının gönderim adresi eksik." };
        }

        var subject = ReplaceParameters(template.SubjectTemplate, request.Parameters);
        var body = ReplaceParameters(template.BodyTemplate, request.Parameters);

        var sender = _senders.FirstOrDefault(s => s.Channel.Equals(request.Channel, StringComparison.OrdinalIgnoreCase));
        
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Channel = request.Channel,
            Type = request.TemplateCode,
            Subject = subject,
            Body = body,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Notifications.Add(notification);

        if (sender == null)
        {
            notification.Status = "Failed";
            notification.ErrorMessage = "İlgili kanal için gönderici bulunamadı.";
            await _dbContext.SaveChangesAsync(cancellationToken);
            return new NotificationResult { Success = false, ErrorMessage = notification.ErrorMessage };
        }

        try
        {
            var isSent = await sender.SendAsync(toAddress, subject, body, cancellationToken);
            if (isSent)
            {
                notification.Status = "Sent";
                notification.SentAt = DateTime.UtcNow;
            }
            else
            {
                notification.Status = "Failed";
                notification.ErrorMessage = "Gönderici servisi başarısız döndü.";
            }
        }
        catch (Exception ex)
        {
            notification.Status = "Failed";
            notification.ErrorMessage = ex.Message;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new NotificationResult 
        { 
            Success = notification.Status == "Sent", 
            ErrorMessage = notification.ErrorMessage 
        };
    }

    private string ReplaceParameters(string template, Dictionary<string, string> parameters)
    {
        var result = template;
        if (parameters != null)
        {
            foreach (var kvp in parameters)
            {
                result = result.Replace($"{{{{{kvp.Key}}}}}", kvp.Value); // Örn: {{Name}}
            }
        }
        return result;
    }
}
