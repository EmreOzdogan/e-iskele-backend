using System;
using FluentValidation;

namespace EIskele.Application.Reservations;

public class CreateReservationRequestValidator : AbstractValidator<CreateReservationRequest>
{
    public CreateReservationRequestValidator()
    {
        RuleFor(x => x.BoatId)
            .NotEmpty()
            .WithMessage("Tekne seçimi zorunludur.");

        RuleFor(x => x.TourPackageId)
            .NotEmpty()
            .WithMessage("Paket seçimi zorunludur.");

        RuleFor(x => x.StartDateTime)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Geçmiş tarih için rezervasyon oluşturulamaz.");
            
        RuleFor(x => x.EndDateTime)
            .GreaterThan(x => x.StartDateTime)
            .WithMessage("Bitiş tarihi, başlangıç tarihinden sonra olmalıdır.");

        RuleFor(x => x.GuestCount)
            .GreaterThan(0)
            .WithMessage("Kişi sayısı en az 1 olmalıdır.");
    }
}
