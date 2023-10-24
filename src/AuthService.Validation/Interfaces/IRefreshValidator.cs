using FluentValidation;
using HerzenHelper.AuthService.Models.Dto.Requests;
using HerzenHelper.Core.Attributes;

namespace HerzenHelper.AuthService.Validation.Interfaces
{
  [AutoInject]
  public interface IRefreshValidator : IValidator<RefreshRequest>
  {
  }
}
