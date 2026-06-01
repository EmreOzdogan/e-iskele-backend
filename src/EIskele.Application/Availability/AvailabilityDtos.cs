using System;

namespace EIskele.Application.Availability;

public class CheckAvailabilityRequest
{
    public Guid BoatId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class BlockDatesRequest
{
    public Guid BoatId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class AvailabilitySlotResponse
{
    public Guid Id { get; set; }
    public Guid BoatId { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public string Status { get; set; } = string.Empty;
}
