using HerzenHelper.Core.BrokerSupport.Attributes;
using HerzenHelper.Core.BrokerSupport.Configurations;
using HerzenHelper.Models.Broker.Requests.User;

namespace HerzenHelper.AuthService.Models.Dto.Configurations
{
  public class RabbitMqConfig : BaseRabbitMqConfig
  {
    [AutoInjectRequest(typeof(IGetUserCredentialsRequest))]
    public string GetUserCredentialsEndpoint { get; set; }

    public string GetTokenEndpoint { get; set; }
  }
}
