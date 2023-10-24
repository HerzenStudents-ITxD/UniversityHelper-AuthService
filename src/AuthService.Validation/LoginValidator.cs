using FluentValidation;
using HerzenHelper.AuthService.Models.Dto.Requests;
using HerzenHelper.AuthService.Validation.Interfaces;

namespace HerzenHelper.AuthService.Validation
{
  public class LoginValidator : AbstractValidator<LoginRequest>, ILoginValidator
  {
    public LoginValidator()
    {
      RuleFor(user => user.LoginData)
        .NotEmpty()
        .WithMessage("Login data must not be empty.");

      RuleFor(user => user.Password)
        .NotEmpty()
        .WithMessage("Password must not be empty.");
    }
  }
}
