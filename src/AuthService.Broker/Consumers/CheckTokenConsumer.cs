using System;
using System.Threading.Tasks;
using HerzenHelper.AuthService.Models.Dto.Enums;
using HerzenHelper.AuthService.Token.Interfaces;
using HerzenHelper.Core.BrokerSupport.Broker;
using HerzenHelper.Core.BrokerSupport.Middlewares.Token;
using MassTransit;

namespace HerzenHelper.AuthService.Broker.Consumers
{
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
}
