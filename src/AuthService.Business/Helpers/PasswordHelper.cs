using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

[assembly: InternalsVisibleTo("UniversityHelper.AuthService.Business.UnitTests")]
namespace UniversityHelper.AuthService.Business.Helpers
{
  internal static class PasswordHelper
  {
    private const string INTERNAL_SALT = "UniversityHelper.SALT3";

    internal static string GetPasswordHash(string userLogin, string salt, string userPassword)
    {
      return Encoding.UTF8.GetString(new SHA512Managed().ComputeHash(
              Encoding.UTF8.GetBytes($"{salt}{userLogin}{userPassword}{INTERNAL_SALT}")));
    }
  }
}
