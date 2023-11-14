using System;
using System.Threading.Tasks;
using UniversityHelper.AuthService.Models.Dto.Enums;
using UniversityHelper.AuthService.Token.Interfaces;
using UniversityHelper.Core.BrokerSupport.Broker;
using UniversityHelper.Core.BrokerSupport.Middlewares.Token;
using MassTransit;

namespace UniversityHelper.AuthService.Broker.Consumers;

public class CheckTokenConsumer : IConsumer<ICheckTokenRequest>
{
  private readonly ITokenValidator _tokenValidator;

  public CheckTokenConsumer(ITokenValidator tokenValidator)
  {
    _tokenValidator = tokenValidator;
  }

  public async Task Consume(ConsumeContext<ICheckTokenRequest> context)
  {
    var response = OperationResultWrapper.CreateResponse(GetValidationResult, context.Message);

    await context.RespondAsync<IOperationResult<Guid>>(response);
  }

  private Guid GetValidationResult(ICheckTokenRequest request)
  {
    return _tokenValidator.Validate(request.Token, TokenType.Access);
  }
}
