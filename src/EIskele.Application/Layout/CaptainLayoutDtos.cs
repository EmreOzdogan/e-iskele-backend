using System;
using System.Collections.Generic;

namespace EIskele.Application.Layout;

public class CaptainLayoutDataDto
{
    public CaptainProfileLayoutDto Captain { get; set; } = new();
    public CaptainCountersDto Counters { get; set; } = new();
    public List<CaptainNotificationDto> Notifications { get; set; } = new();
}

public class CaptainProfileLayoutDto
{
    public string FullName { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public string VerificationStatus { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string PrimaryBoatName { get; set; } = string.Empty;
    public string AccountStatus { get; set; } = string.Empty;
}

public class CaptainCountersDto
{
    public int PendingReservations { get; set; }
    public int DocumentActions { get; set; }
    public int UnansweredReviews { get; set; }
    public int SupportWaitingCaptain { get; set; }
    public int UnreadNotifications { get; set; }
}

public class CaptainNotificationDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string CreatedAtText { get; set; } = string.Empty;
    public string ActionPath { get; set; } = string.Empty;
    public bool Read { get; set; }
}
