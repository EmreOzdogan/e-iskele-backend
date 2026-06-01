using System;

namespace EIskele.Application.Reservations;

public record CreateReservationRequest(Guid CustomerId, Guid BoatId, Guid TourPackageId, DateTime StartDateTime, DateTime EndDateTime, int GuestCount);
