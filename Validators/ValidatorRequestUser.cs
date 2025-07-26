using FluentValidation;
using LuxoraStore.Model;
using LuxoraStore.Model.DTO;
using Microsoft.EntityFrameworkCore;

namespace LuxoraStore.Validators
{
    public class ValidatorRequestUser : AbstractValidator<UserDTO>
    {
        private readonly ApplicationContext _context;
        public ValidatorRequestUser(ApplicationContext context)
        {
            _context = context;

            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
                .MaximumLength(20).WithMessage("Username must be at most 20 characters.")
                .MustAsync(async (username, cancellation) =>
                {
                    return !await _context.Users.AnyAsync(u => u.Username == username);
                }).WithMessage("Username is already registered.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email format is not valid.")
                .MustAsync(async (email, cancellation) =>
                {
                    return !await _context.Users.AnyAsync(u => u.Email == email);
                }).WithMessage("Email is already registered.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
                .MaximumLength(20).WithMessage("The maximum password must be 20 characters.");

        }
    }
}
