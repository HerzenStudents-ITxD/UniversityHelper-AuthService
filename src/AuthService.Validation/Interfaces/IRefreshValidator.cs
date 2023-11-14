using FluentValidation;
using UniversityHelper.AuthService.Models.Dto.Requests;
using UniversityHelper.Core.Attributes;

namespace UniversityHelper.AuthService.Validation.Interfaces;

[AutoInject]
public interface IRefreshValidator : IValidator<RefreshRequest>
{
}
