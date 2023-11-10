using FluentValidation;
using UniversityHelper.AuthService.Models.Dto.Requests;
using UniversityHelper.AuthService.Validation.Interfaces;

namespace UniversityHelper.AuthService.Validation
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
