﻿namespace UniversityHelper.AuthService.Models.Dto.Requests;

public record RefreshRequest
{
  public string RefreshToken { get; set; }
}
