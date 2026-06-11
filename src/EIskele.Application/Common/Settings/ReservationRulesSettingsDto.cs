using System;

namespace EIskele.Application.Common.Settings;

public class ReservationRulesSettingsDto
{
    public string DefaultReservationApprovalType { get; set; } = "captainApproval";
    public int CaptainApprovalTimeoutHours { get; set; } = 12;
    public int MinimumReservationLeadTimeHours { get; set; } = 24;
    public int MaxAdvanceReservationDays { get; set; } = 90;
    public int CancellationAllowedHoursBeforeTour { get; set; } = 48;
    public bool WeatherPostponeEnabled { get; set; } = true;
    public bool GuestCountValidationEnabled { get; set; } = true;
    public bool PreventOverlappingReservations { get; set; } = true;
}
