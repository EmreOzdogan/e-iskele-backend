using EIskele.Application.Users.DTOs;
using FluentValidation;

namespace EIskele.Application.Users.Validators;

public class UpdateUserRolesRequestValidator : AbstractValidator<UpdateUserRolesRequest>
{
    public UpdateUserRolesRequestValidator()
    {
        RuleFor(x => x.Roles)
            .NotNull().WithMessage("Rol listesi boş olamaz.")
            .Must(roles => roles.Count > 0).WithMessage("En az bir rol seçilmelidir.");
    }
}
