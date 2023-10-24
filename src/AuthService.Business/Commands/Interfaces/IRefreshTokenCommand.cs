using HerzenHelper.AuthService.Models.Dto.Requests;
using HerzenHelper.AuthService.Models.Dto.Responses;
using HerzenHelper.Core.Attributes;

namespace HerzenHelper.AuthService.Business.Commands.Interfaces
{
  [AutoInject]
  public interface IRefreshTokenCommand
  {
    LoginResult Execute(RefreshRequest request);
  }
}
