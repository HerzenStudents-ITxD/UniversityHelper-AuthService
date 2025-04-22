using UniversityHelper.Core.BrokerSupport.Attributes;
using UniversityHelper.Core.BrokerSupport.Configurations;
using UniversityHelper.Models.Broker.Requests.User;

namespace UniversityHelper.AuthService.Models.Dto.Configurations;

public class RabbitMqConfig : BaseRabbitMqConfig
{
  [AutoInjectRequest(typeof(IGetUserCredentialsRequest))]
  public string GetUserCredentialsEndpoint { get; set; }

  public string GetTokenEndpoint { get; set; }
}
