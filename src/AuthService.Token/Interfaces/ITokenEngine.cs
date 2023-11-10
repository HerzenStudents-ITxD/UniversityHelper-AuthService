using System;
using UniversityHelper.AuthService.Models.Dto.Enums;

namespace UniversityHelper.AuthService.Token.Interfaces
{
  public interface ITokenEngine
  {
    /// <summary>
    /// Create new refresh or access token based on user id.
    /// </summary>
    /// <param name="userId">Specified user ID</param>
    /// <param name="tokenType">Token type (Access, Refresh)</param>
    /// <returns>Token based on userId</returns>
    string Create(Guid userId, TokenType tokenType, out double tokenExpiresIn);
  }
}
