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

            // Validasi Username
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.")
                .MinimumLength(6).WithMessage("Username must be at least 6 characters.") 
                .MaximumLength(20).WithMessage("Username must be at most 20 characters.")
                .MustAsync(async (username, cancellation) =>
                {
                    return !await _context.Users.AnyAsync(u => u.Username == username, cancellation);
                }).WithMessage("Username is already registered.");

            // Validasi Email
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email format is not valid.")
                .MustAsync(async (email, cancellation) =>
                {
                    return !await _context.Users.AnyAsync(u => u.Email == email, cancellation);
                }).WithMessage("Email is already registered.");

            // Validasi Password
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
                .MaximumLength(20).WithMessage("Password must be at most 20 characters.");

            // Validasi basic
            //RuleFor(x => x.Username)
            //    .NotEmpty().WithMessage("Username is required.")
            //    .MinimumLength(6).WithMessage("Username must be at least 6 characters.")
            //    .MaximumLength(20).WithMessage("Username must be at most 20 characters.");

            //RuleFor(x => x.Email)
            //    .NotEmpty().WithMessage("Email is required.")
            //    .EmailAddress().WithMessage("Email format is not valid.");

            //RuleFor(x => x.Password)
            //    .NotEmpty().WithMessage("Password is required.")
            //    .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
            //    .MaximumLength(20).WithMessage("Password must be at most 20 characters.");

            //// Validasi tambahan untuk profile
            //RuleFor(x => x.Name)
            //    .NotEmpty().WithMessage("Name is required.")
            //    .MaximumLength(50).WithMessage("Name must be at most 50 characters.");

            //RuleFor(x => x.Address)
            //    .NotEmpty().WithMessage("Address is required.")
            //    .MaximumLength(200).WithMessage("Address must be at most 200 characters.");

            //RuleFor(x => x.PhoneNumber)
            //    .NotEmpty().WithMessage("Phone number is required.")
            //    .Matches(@"^\+?[\d\s\-\(\)]+$").WithMessage("Phone number format is not valid.");

            //RuleFor(x => x.Image)
            //    .NotEmpty().WithMessage("Image is required.");
        

        }
    }
}
