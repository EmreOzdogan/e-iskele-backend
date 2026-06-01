namespace EIskele.Domain.Enums;

public enum ReservationStatus
{
    Pending = 0,
    WaitingCaptainApproval = 1,
    Approved = 2,
    PaymentPending = 3,
    Paid = 4,
    Completed = 5,
    Cancelled = 6,
    Rejected = 7,
    NoShow = 8,
    PostponedDueToWeather = 9
}
