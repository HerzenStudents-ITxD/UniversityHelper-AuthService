using UniversityHelper.Core.BrokerSupport.Attributes;
using UniversityHelper.Core.BrokerSupport.Configurations;
using UniversityHelper.Models.Broker.Requests.User;
using UniversityHelper.Core.BrokerSupport.Middlewares.Token;

namespace UniversityHelper.AuthService.Models.Dto.Configurations;

public class RabbitMqConfig : BaseRabbitMqConfig
{
  [AutoInjectRequest(typeof(IGetUserCredentialsRequest))]
  public string GetUserCredentialsEndpoint { get; set; }

  public string GetTokenEndpoint { get; set; }

  [AutoInjectRequest(typeof(ICheckTokenRequest))]
  public string ValidateTokenEndpoint { get; set; }
}
