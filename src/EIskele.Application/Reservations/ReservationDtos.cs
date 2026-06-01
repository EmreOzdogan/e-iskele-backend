using System;

namespace EIskele.Application.Reservations;

public class ReservationResponse
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
}
