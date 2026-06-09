using EIskele.Application.Users.DTOs;
using FluentValidation;

namespace EIskele.Application.Users.Validators;

public class AdminUserListQueryValidator : AbstractValidator<AdminUserListQuery>
{
    public AdminUserListQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Sayfa numarası 0'dan büyük olmalıdır.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Sayfa boyutu 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(100).WithMessage("Sayfa boyutu en fazla 100 olabilir.");
            
        RuleFor(x => x.CreatedDateStart)
            .LessThanOrEqualTo(x => x.CreatedDateEnd)
            .When(x => x.CreatedDateStart.HasValue && x.CreatedDateEnd.HasValue)
            .WithMessage("Başlangıç tarihi bitiş tarihinden büyük olamaz.");
    }
}
