using UniversityHelper.AuthService.Business.Commands.Interfaces;
using UniversityHelper.AuthService.Models.Dto.Requests;
using UniversityHelper.AuthService.Models.Dto.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace UniversityHelper.AuthService.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController
{
  [HttpPost("login")]
  public async Task<LoginResult> LoginUser(
      [FromServices] ILoginCommand command,
      [FromBody] LoginRequest userCredentials)
  {
    return await command.Execute(userCredentials);
  }

  [HttpPost("refresh")]
  public LoginResult RefreshToken(
      [FromServices] IRefreshTokenCommand command,
      [FromBody] RefreshRequest refreshToken)
  {
    return command.Execute(refreshToken);
  }
}
