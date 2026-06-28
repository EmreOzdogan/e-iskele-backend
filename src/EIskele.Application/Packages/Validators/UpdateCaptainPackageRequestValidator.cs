using FluentValidation;

namespace EIskele.Application.Packages.Validators;

public class UpdateCaptainPackageRequestValidator : AbstractValidator<UpdateCaptainPackageRequest>
{
    public UpdateCaptainPackageRequestValidator()
    {
        RuleFor(x => x.BoatId)
            .NotEmpty()
            .WithMessage("Tekne seçimi zorunludur.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Paket adı boş bırakılamaz.")
            .MinimumLength(3)
            .WithMessage("Paket adı en az 3 karakter olmalıdır.");

        RuleFor(x => x.TourType)
            .NotEmpty()
            .WithMessage("Tur tipi belirtilmelidir.");

        RuleFor(x => x.ReservationModel)
            .NotEmpty()
            .WithMessage("Rezervasyon modeli seçilmelidir.");

        RuleFor(x => x.MinGuests)
            .GreaterThan(0)
            .WithMessage("Minimum kişi sayısı 0'dan büyük olmalıdır.");

        RuleFor(x => x.MaxGuests)
            .GreaterThanOrEqualTo(x => x.MinGuests)
            .WithMessage("Maksimum kişi sayısı, minimum kişi sayısından küçük olamaz.");

        RuleFor(x => x.PerPersonPrice)
            .GreaterThan(0)
            .When(x => x.PriceType == "PerPerson")
            .WithMessage("Kişi başı fiyatı 0'dan büyük olmalıdır.");

        RuleFor(x => x.WholeBoatPrice)
            .GreaterThan(0)
            .When(x => x.PriceType == "WholeBoat")
            .WithMessage("Tüm tekne fiyatı 0'dan büyük olmalıdır.");

        RuleFor(x => x.DepositRate)
            .InclusiveBetween(0, 100)
            .When(x => x.HasDeposit)
            .WithMessage("Kapora oranı %0 ile %100 arasında olmalıdır.");
    }
}
