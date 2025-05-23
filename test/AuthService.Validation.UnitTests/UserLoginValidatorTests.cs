﻿using FluentValidation.TestHelper;
using UniversityHelper.AuthService.Models.Dto.Requests;
using UniversityHelper.AuthService.Validation.Interfaces;
using NUnit.Framework;

namespace UniversityHelper.AuthService.Validation.UnitTests;

public class LoginValidatorTests
{
  private ILoginValidator validator;

  [OneTimeSetUp]
  public void SetUp()
  {
    validator = new LoginValidator();
  }

  [Test]
  public void GoodLoginRequestTest()
  {
    var request = new LoginRequest
    {
      LoginData = "admin",
      Password = "admin"
    };
    validator.TestValidate(request).ShouldNotHaveAnyValidationErrors();

    request.LoginData = " admin ";
    request.Password = " admin ";
    validator.TestValidate(request).ShouldNotHaveAnyValidationErrors();
  }

  [Test]
  public void BadLoginRequestTest()
  {
    var request = new LoginRequest
    {
      LoginData = "Login",
      Password = ""
    };
    validator.TestValidate(request).ShouldHaveAnyValidationError();

    request.Password = " ";
    validator.TestValidate(request).ShouldHaveAnyValidationError();

    request.LoginData = " ";
    validator.TestValidate(request).ShouldHaveAnyValidationError();

    request.LoginData = "";
    validator.TestValidate(request).ShouldHaveAnyValidationError();

    request.Password = "Password";
    validator.TestValidate(request).ShouldHaveAnyValidationError();
  }
}
