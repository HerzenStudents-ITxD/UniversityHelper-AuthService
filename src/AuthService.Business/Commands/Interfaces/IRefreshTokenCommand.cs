using UniversityHelper.AuthService.Models.Dto.Requests;
using UniversityHelper.AuthService.Models.Dto.Responses;
using UniversityHelper.Core.Attributes;

namespace UniversityHelper.AuthService.Business.Commands.Interfaces;

[AutoInject]
public interface IRefreshTokenCommand
{
  LoginResult Execute(RefreshRequest request);
}
