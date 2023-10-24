using FluentValidation;
using HerzenHelper.AuthService.Models.Dto.Requests;
using HerzenHelper.AuthService.Validation.Interfaces;

namespace HerzenHelper.AuthService.Validation
{
  public class RefreshValidator : AbstractValidator<RefreshRequest>, IRefreshValidator
  {
    public RefreshValidator()
    {
      RuleFor(request => request.RefreshToken.Trim())
        .NotEmpty()
        .WithMessage("Token must not be empty.");
    }
  }
}
