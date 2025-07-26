using FluentValidation;
using LuxoraStore.Model.DTO;

namespace LuxoraStore.Validators
{
    public class LoginValidator : AbstractValidator<LoginDTO>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required")
                .MaximumLength(20).WithMessage("Password must be at least 20 characters");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MaximumLength(15).WithMessage("Password must be at least 15 characters");
        }
    }
}
