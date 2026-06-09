using EIskele.Application.Users.DTOs;
using FluentValidation;

namespace EIskele.Application.Users.Validators;

public class UpdateUserStatusRequestValidator : AbstractValidator<UpdateUserStatusRequest>
{
    public UpdateUserStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Geçersiz kullanıcı durumu.");
            
        RuleFor(x => x.Reason)
            .MaximumLength(500).WithMessage("Neden açıklaması en fazla 500 karakter olabilir.");
    }
}
