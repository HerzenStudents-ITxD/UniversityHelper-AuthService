﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation.Results;
using UniversityHelper.AuthService.Business.Commands;
using UniversityHelper.AuthService.Business.Commands.Interfaces;
using UniversityHelper.AuthService.Business.Helpers;
using UniversityHelper.AuthService.Models.Dto.Requests;
using UniversityHelper.AuthService.Token.Interfaces;
using UniversityHelper.AuthService.Validation.Interfaces;
using UniversityHelper.Core.BrokerSupport.Broker;
using UniversityHelper.Models.Broker.Requests.User;
using UniversityHelper.Models.Broker.Responses.User;
using MassTransit;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace UniversityHelper.AuthService.Business.UnitTests;

public class UserCredentialsResponse : IGetUserCredentialsResponse
{
  public Guid UserId { get; set; }
  public string PasswordHash { get; set; }
  public string Salt { get; set; }
  public string UserLogin { get; set; }
}

public class UserLoginCommandTests
{
  #region private fields
  private Mock<ITokenEngine> tokenEngineMock;
  private Mock<ILoginValidator> loginValidatorMock;
  private Mock<ValidationResult> validationResultIsValidMock;
  private Mock<IRequestClient<IGetUserCredentialsRequest>> requestBrokerMock;
  private Mock<IOperationResult<IGetUserCredentialsResponse>> operationResultMock;
  private Mock<IGetUserCredentialsResponse> brokerResponseMock;

  private string salt;
  private AutoMocker autoMocker;
  private ILoginCommand command;

  private LoginRequest newUserCredentials;
  private ValidationResult validationResultError;

  #endregion

  #region Setup
  [OneTimeSetUp]
  public void OneTimeSetUp()
  {
    salt = "Example_Salt1";

    loginValidatorMock = new Mock<ILoginValidator>();

    newUserCredentials = new LoginRequest
    {
      LoginData = "User_login_example",
      Password = "Example_1234"
    };

    validationResultError = new ValidationResult(
        new List<ValidationFailure>
        {
                  new ValidationFailure("error", "something", null)
        });

    validationResultIsValidMock = new Mock<ValidationResult>();

    validationResultIsValidMock
        .Setup(x => x.IsValid)
        .Returns(true);

    BrokerSetUp();

    tokenEngineMock = new Mock<ITokenEngine>();

    autoMocker = new AutoMocker();
    command = autoMocker.CreateInstance<LoginCommand>();
  }

  public void BrokerSetUp()
  {
    var passwordHash = PasswordHelper.GetPasswordHash(
        newUserCredentials.LoginData,
        salt,
        newUserCredentials.Password);

    var responseBrokerMock = new Mock<Response<IOperationResult<IGetUserCredentialsResponse>>>();
    requestBrokerMock = new Mock<IRequestClient<IGetUserCredentialsRequest>>();

    brokerResponseMock = new Mock<IGetUserCredentialsResponse>();
    brokerResponseMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
    brokerResponseMock.Setup(x => x.PasswordHash).Returns(passwordHash);
    brokerResponseMock.Setup(x => x.Salt).Returns(salt);
    brokerResponseMock.Setup(x => x.UserLogin).Returns(newUserCredentials.LoginData);

    operationResultMock = new Mock<IOperationResult<IGetUserCredentialsResponse>>();
    operationResultMock.Setup(x => x.Body).Returns(brokerResponseMock.Object);
    operationResultMock.Setup(x => x.IsSuccess).Returns(true);
    operationResultMock.Setup(x => x.Errors).Returns(new List<string>());

    requestBrokerMock.Setup(
        x => x.GetResponse<IOperationResult<IGetUserCredentialsResponse>>(
            It.IsAny<object>(), default, default))
        .Returns(Task.FromResult(responseBrokerMock.Object));

    responseBrokerMock
        .SetupGet(x => x.Message)
        .Returns(operationResultMock.Object);
  }
  #endregion

  #region Successful test
  //[Test]
  //public void ShouldReturnUserIdAndJwtWhenPasswordsAndEmailHasMatch()
  //{
  //    string JwtToken = "Example_jwt";

  //    var expectedLoginResponse = new LoginResult
  //    {
  //        UserId = brokerResponseMock.Object.UserId,
  //        Token = JwtToken
  //    };

  //    loginValidatorMock
  //       .Setup(x => x.Validate(It.IsAny<IValidationContext>()))
  //       .Returns(validationResultIsValidMock.Object);

  //    tokenEngineMock
  //        .Setup(X => X.Create(brokerResponseMock.Object.UserId))
  //        .Returns(JwtToken);

  //    SerializerAssert.AreEqual(expectedLoginResponse, command.Execute(newUserCredentials).Result);
  //}
  #endregion

  #region Fail tests
  //[Test]
  //public void ShouldThrowExceptionWhenPasswordsHasNotMatch()
  //{
  //    loginValidatorMock
  //       .Setup(x => x.Validate(It.IsAny<IValidationContext>()))
  //       .Returns(validationResultIsValidMock.Object);

  //    newUserCredentials.Password = "Example";

  //    Assert.ThrowsAsync<ForbiddenException>(() => command.Execute(newUserCredentials));
  //}

  //[Test]
  //public void ShouldThrowExceptionWhenUserEmailHasNotMatchInDb()
  //{
  //    operationResultMock.Setup(x => x.Body).Returns((IUserCredentialsResponse)null);
  //    operationResultMock.Setup(x => x.IsSuccess).Returns(false);
  //    operationResultMock.Setup(x => x.Errors).Returns(new List<string> { "User email not found" });

  //    loginValidatorMock
  //       .Setup(x => x.Validate(It.IsAny<IValidationContext>()))
  //       .Returns(validationResultIsValidMock.Object);

  //    Assert.ThrowsAsync<NotFoundException>(
  //        () => command.Execute(newUserCredentials),
  //        "User email not found");
  //}

  //[Test]
  //public void ShouldThrowExceptionWhenUserLoginInfoNotValid()
  //{
  //    newUserCredentials.LoginData = string.Empty;

  //    loginValidatorMock
  //       .Setup(x => x.Validate(It.IsAny<IValidationContext>()))
  //       .Returns(validationResultError);

  //    Assert.ThrowsAsync<ValidationException>(() => command.Execute(newUserCredentials));
  //}
  #endregion
}
