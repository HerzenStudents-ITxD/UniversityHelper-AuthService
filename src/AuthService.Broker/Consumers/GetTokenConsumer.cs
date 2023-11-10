using System.Threading.Tasks;
using UniversityHelper.AuthService.Models.Dto.Enums;
using UniversityHelper.AuthService.Token.Interfaces;
using UniversityHelper.Core.BrokerSupport.Broker;
using UniversityHelper.Models.Broker.Requests.Auth;
using UniversityHelper.Models.Broker.Responses.Auth;
using MassTransit;

namespace UniversityHelper.AuthService.Broker.Consumers
{
  public class GetTokenConsumer : IConsumer<IGetTokenRequest>
  {
    private readonly ITokenEngine _tokenEngine;

    public GetTokenConsumer(ITokenEngine tokenEngine)
    {
      _tokenEngine = tokenEngine;
    }

    public async Task Consume(ConsumeContext<IGetTokenRequest> context)
    {
      var response = OperationResultWrapper.CreateResponse(GetTokenResult, context.Message);

      await context.RespondAsync<IOperationResult<IGetTokenResponse>>(response);
    }

    private object GetTokenResult(IGetTokenRequest request)
    {
      return IGetTokenResponse.CreateObj(
        _tokenEngine.Create(request.UserId, TokenType.Access, out double accessTokenExpiresIn),
        _tokenEngine.Create(request.UserId, TokenType.Refresh, out double refreshTokenExpiresIn),
        accessTokenExpiresIn,
        refreshTokenExpiresIn);
    }
  }
}
