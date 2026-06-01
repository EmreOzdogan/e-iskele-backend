namespace EIskele.Application.Common.Errors;

public static class ErrorCodes
{
    // Auth
    public const string AuthInvalidCredentials = "AUTH_INVALID_CREDENTIALS";
    public const string AuthTokenExpired = "AUTH_TOKEN_EXPIRED";
    
    // Boat
    public const string BoatNotFound = "BOAT_NOT_FOUND";
    public const string BoatNotApproved = "BOAT_NOT_APPROVED";

    // Reservation
    public const string ReservationSlotConflict = "RESERVATION_SLOT_CONFLICT";
    public const string ReservationDateInPast = "RESERVATION_DATE_IN_PAST";
    public const string ReservationCapacityExceeded = "RESERVATION_CAPACITY_EXCEEDED";
    
    // Validations & Server
    public const string ValidationError = "VALIDATION_ERROR";
    public const string InternalServerError = "INTERNAL_SERVER_ERROR";
}
