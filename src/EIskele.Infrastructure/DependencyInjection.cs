using EIskele.Application.Common.Audit;
using EIskele.Infrastructure.Audit;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.AspNetCore.Authorization;
using EIskele.Application.Common.Security;
using EIskele.Infrastructure.Security;

namespace EIskele.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IAuditLogService, AuditLogService>();
        services.AddScoped<EIskele.Application.Common.Files.IFileStorageService, EIskele.Infrastructure.Files.LocalFileStorageService>();
        
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

        services.AddScoped<EIskele.Infrastructure.Notifications.INotificationSender, EIskele.Infrastructure.Notifications.EmailNotificationSender>();
        services.AddScoped<EIskele.Application.Common.Notifications.INotificationService, EIskele.Infrastructure.Notifications.NotificationService>();

        services.AddScoped<EIskele.Application.Common.Settings.ISettingsService, EIskele.Infrastructure.Settings.SettingsService>();

        // Email Infrastructure
        services.AddSingleton<EIskele.Infrastructure.Emails.Services.IEmailTemplateRenderer, EIskele.Infrastructure.Emails.Services.RazorEmailTemplateRenderer>();
        services.AddScoped<EIskele.Infrastructure.Emails.Services.IEmailSender, EIskele.Infrastructure.Emails.Services.SmtpEmailSender>();
        // Application Core Services
        services.AddScoped<EIskele.Application.Auth.IAuthService, EIskele.Infrastructure.Services.AuthService>();
        services.AddScoped<EIskele.Application.Captains.ICaptainService, EIskele.Infrastructure.Services.CaptainService>();
        services.AddScoped<EIskele.Application.Boats.IBoatService, EIskele.Infrastructure.Services.BoatService>();
        services.AddScoped<EIskele.Application.Reservations.IReservationService, EIskele.Infrastructure.Services.ReservationService>();
        services.AddScoped<EIskele.Application.Availability.IAvailabilityService, EIskele.Infrastructure.Services.AvailabilityService>();
        services.AddScoped<EIskele.Application.Availability.ICaptainCalendarService, EIskele.Infrastructure.Services.CaptainCalendarService>();
        services.AddScoped<EIskele.Application.Users.Services.IAdminUserService, EIskele.Infrastructure.Services.AdminUserService>();
        services.AddScoped<EIskele.Application.Common.Locations.ILocationService, EIskele.Infrastructure.Locations.LocationService>();
        services.AddScoped<EIskele.Application.Packages.ITourPackageService, EIskele.Infrastructure.Services.TourPackageService>();
        services.AddScoped<EIskele.Application.Payments.IPaymentService, EIskele.Infrastructure.Services.PaymentService>();
        services.AddScoped<EIskele.Application.Dashboard.IDashboardService, EIskele.Infrastructure.Services.DashboardService>();
        services.AddScoped<EIskele.Application.Reservations.ICaptainReservationService, EIskele.Infrastructure.Services.CaptainReservationService>();
        services.AddScoped<EIskele.Application.Reviews.ICaptainReviewService, EIskele.Infrastructure.Services.CaptainReviewService>();
        services.AddScoped<EIskele.Application.Earnings.ICaptainEarningService, EIskele.Infrastructure.Services.CaptainEarningService>();
        services.AddScoped<EIskele.Application.Settings.ICaptainSettingsService, EIskele.Infrastructure.Services.CaptainSettingsService>();
        services.AddScoped<EIskele.Application.Dashboard.ICaptainDashboardService, EIskele.Infrastructure.Services.CaptainDashboardService>();
        services.AddScoped<EIskele.Application.Layout.ICaptainLayoutService, EIskele.Infrastructure.Services.CaptainLayoutService>();
        services.AddScoped<EIskele.Application.Captains.ICaptainDocumentsService, EIskele.Infrastructure.Services.CaptainDocumentsService>();

        return services;
    }
}
