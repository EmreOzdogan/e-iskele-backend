using FluentValidation;

namespace EIskele.Application.Captains.Validators;

public class CaptainApplicationRequestValidator : AbstractValidator<CaptainApplicationRequest>
{
    public CaptainApplicationRequestValidator()
    {
        RuleFor(x => x.ApplicationType)
            .NotEmpty()
            .WithMessage("Başvuru tipi boş bırakılamaz.")
            .Must(x => x == "individual" || x == "company")
            .WithMessage("Başvuru tipi 'individual' veya 'company' olmalıdır.");

        RuleFor(x => x.Individual)
            .NotNull()
            .When(x => x.ApplicationType == "individual")
            .WithMessage("Bireysel başvuru için birey bilgileri zorunludur.");

        RuleFor(x => x.Individual!)
            .SetValidator(new ApplicationIndividualDtoValidator())
            .When(x => x.ApplicationType == "individual" && x.Individual != null);

        RuleFor(x => x.Company)
            .NotNull()
            .When(x => x.ApplicationType == "company")
            .WithMessage("Kurumsal başvuru için firma bilgileri zorunludur.");

        RuleFor(x => x.Company!)
            .SetValidator(new ApplicationCompanyDtoValidator())
            .When(x => x.ApplicationType == "company" && x.Company != null);

        RuleFor(x => x.Boat)
            .NotNull()
            .WithMessage("Tekne bilgileri zorunludur.");

        RuleFor(x => x.Boat)
            .SetValidator(new ApplicationBoatDtoValidator())
            .When(x => x.Boat != null);
    }
}

public class ApplicationIndividualDtoValidator : AbstractValidator<ApplicationIndividualDto>
{
    public ApplicationIndividualDtoValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Ad soyad boş bırakılamaz.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta adresi boş bırakılamaz.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Telefon numarası boş bırakılamaz.");

        RuleFor(x => x.IdentityNumber)
            .NotEmpty().WithMessage("TC Kimlik numarası boş bırakılamaz.")
            .Length(11).WithMessage("TC Kimlik numarası 11 haneli olmalıdır.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre boş bırakılamaz.")
            .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.");
    }
}

public class ApplicationCompanyDtoValidator : AbstractValidator<ApplicationCompanyDto>
{
    public ApplicationCompanyDtoValidator()
    {
        RuleFor(x => x.CompanyTitle)
            .NotEmpty().WithMessage("Firma ünvanı boş bırakılamaz.");

        RuleFor(x => x.AuthorizedPersonFullName)
            .NotEmpty().WithMessage("Yetkili kişi adı soyadı boş bırakılamaz.");

        RuleFor(x => x.TaxOffice)
            .NotEmpty().WithMessage("Vergi dairesi boş bırakılamaz.");

        RuleFor(x => x.TaxNumber)
            .NotEmpty().WithMessage("Vergi numarası boş bırakılamaz.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta adresi boş bırakılamaz.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Telefon numarası boş bırakılamaz.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre boş bırakılamaz.")
            .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.");
    }
}

public class ApplicationBoatDtoValidator : AbstractValidator<ApplicationBoatDto>
{
    public ApplicationBoatDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tekne adı boş bırakılamaz.");

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Tekne kapasitesi en az 1 kişi olmalıdır.");

        RuleFor(x => x.LocationId)
            .NotEmpty().WithMessage("Lokasyon seçimi zorunludur.");
    }
}
