using FluentValidation;

namespace EIskele.Application.Boats.Validators;

public class CreateCaptainBoatRequestValidator : AbstractValidator<CreateCaptainBoatRequest>
{
    public CreateCaptainBoatRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Tekne adı boş bırakılamaz.")
            .MinimumLength(3)
            .WithMessage("Tekne adı en az 3 karakter olmalıdır.");

        RuleFor(x => x.Capacity)
            .GreaterThan(0)
            .WithMessage("Tekne kapasitesi en az 1 kişi olmalıdır.");

        RuleFor(x => x.LocationId)
            .NotEmpty()
            .WithMessage("Lokasyon seçimi zorunludur.");
            
        RuleFor(x => x.BoatType)
            .NotEmpty()
            .WithMessage("Tekne tipi boş bırakılamaz.");
            
        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .WithMessage("Açıklama 2000 karakterden uzun olamaz.");
    }
}
