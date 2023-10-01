using AuthServer.Core.DTOs;
using FluentValidation;

namespace AuthServer.API.Validations
{
    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator()
        {
            RuleFor(r => r.Email).NotEmpty().WithMessage("Email is required.").EmailAddress().WithMessage("Email is wrong.");
            RuleFor(r => r.Password).NotEmpty().WithMessage("Password is required.");
            RuleFor(r => r.UserName).NotEmpty().WithMessage("Username is required.");
        }
    }
}
