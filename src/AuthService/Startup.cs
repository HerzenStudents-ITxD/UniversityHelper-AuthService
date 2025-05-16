using System;
using System.Collections.Generic;
using System.Linq;
using HealthChecks.UI.Client;
using UniversityHelper.AuthService.Broker.Consumers;
using UniversityHelper.AuthService.Models.Dto.Configurations;
using UniversityHelper.AuthService.Token;
using UniversityHelper.AuthService.Token.Interfaces;
using UniversityHelper.Core.BrokerSupport.Configurations;
using UniversityHelper.Core.BrokerSupport.Extensions;
using UniversityHelper.Core.Configurations;
using UniversityHelper.Core.Extensions;
using UniversityHelper.Core.Middlewares.ApiInformation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using UniversityHelper.Core.BrokerSupport.Helpers;

namespace UniversityHelper.AuthService;

public class Startup : BaseApiInfo
{
  public const string CorsPolicyName = "LtDoCorsPolicy";

  private readonly BaseServiceInfoConfig _serviceInfoConfig;
  private readonly RabbitMqConfig _rabbitMqConfig;

  public IConfiguration Configuration { get; }

  #region private methods

  private void ConfigureJwt(IServiceCollection services)
  {
    var signingKey = new SigningSymmetricKey();
    var signingDecodingKey = (IJwtSigningDecodingKey)signingKey;

    services.AddSingleton<IJwtSigningEncodingKey>(signingKey);
    services.AddSingleton<IJwtSigningDecodingKey>(signingKey);

    services.AddTransient<ITokenEngine, TokenEngine>();
    services.AddTransient<ITokenValidator, TokenValidator>();

    services.Configure<TokenSettings>(Configuration.GetSection("TokenSettings"));

    services
      .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddJwtBearer(options =>
      {
        options.RequireHttpsMetadata = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
          ValidateIssuer = true,
          ValidIssuer = Configuration.GetSection("TokenSettings:TokenIssuer").Value,
          ValidateAudience = true,
          ValidAudience = Configuration.GetSection("TokenSettings:TokenAudience").Value,
          ValidateLifetime = true,
          IssuerSigningKey = signingDecodingKey.GetKey(),
          ValidateIssuerSigningKey = true
        };
      });
  }

  private void ConfigureMassTransit(IServiceCollection services)
  {
    (string username, string password) = RabbitMqCredentialsHelper
      .Get(_rabbitMqConfig, _serviceInfoConfig);

    services.AddMassTransit(x =>
    {
      // Register consumers
      x.AddConsumer<CheckTokenConsumer>();
      x.AddConsumer<GetTokenConsumer>();

      x.UsingRabbitMq((context, cfg) =>
      {
        cfg.Host(_rabbitMqConfig.Host, "/", host =>
        {
          host.Username(username);
          host.Password(password);
        });

        // Configure endpoints
        cfg.ReceiveEndpoint(_rabbitMqConfig.ValidateTokenEndpoint, e =>
        {
          e.ConfigureConsumer<CheckTokenConsumer>(context);
        });

        cfg.ReceiveEndpoint(_rabbitMqConfig.GetTokenEndpoint, e =>
        {
          e.ConfigureConsumer<GetTokenConsumer>(context);
        });
      });
    });

    services.AddMassTransitHostedService();
  }

  #endregion

  #region public methods

  public Startup(IConfiguration configuration)
  {
    Configuration = configuration;

    _serviceInfoConfig = Configuration
      .GetSection(BaseServiceInfoConfig.SectionName)
      .Get<BaseServiceInfoConfig>();

    _rabbitMqConfig = Configuration
      .GetSection(BaseRabbitMqConfig.SectionName)
      .Get<RabbitMqConfig>();

    Version = "2.0.2.0";
    Description = "AuthService is an API intended to work with user authentication, create token for user.";
    StartTime = DateTime.UtcNow;
    ApiName = $"UniversityHelper - {_serviceInfoConfig.Name}";
  }

  public void ConfigureServices(IServiceCollection services)
  {
    services.AddCors(options =>
    {
      options.AddPolicy(
        CorsPolicyName,
        builder =>
        {
          builder
            .WithOrigins(
              "http://localhost:5173",
              "http://localhost:4200",
              "http://localhost:4500")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
    });

    services.AddHttpContextAccessor();

    services.Configure<BaseRabbitMqConfig>(Configuration.GetSection(BaseRabbitMqConfig.SectionName));
    services.Configure<BaseServiceInfoConfig>(Configuration.GetSection(BaseServiceInfoConfig.SectionName));
    services.Configure<ForwardedHeadersOptions>(options =>
    {
      options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    });

    services.AddBusinessObjects();

    services
      .AddHealthChecks()
      .AddRabbitMqCheck();

    services.AddControllers();
    
    ConfigureMassTransit(services);
    ConfigureJwt(services);

    services.AddSwaggerGen(options =>
    {
      options.SwaggerDoc($"{Version}", new OpenApiInfo
      {
        Version = Version,
        Title = _serviceInfoConfig.Name,
        Description = Description
      });

      options.EnableAnnotations();
    });
  }

  public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
  {
    app.UseForwardedHeaders();

    app.UseExceptionsHandler(loggerFactory);

    app.UseRouting();

    app.UseCors(CorsPolicyName);

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseApiInformation();

    app.UseEndpoints(endpoints =>
    {
      endpoints.MapControllers().RequireCors(CorsPolicyName);

      endpoints.MapHealthChecks($"/{_serviceInfoConfig.Id}/hc", new HealthCheckOptions
      {
        ResultStatusCodes = new Dictionary<HealthStatus, int>
        {
          { HealthStatus.Unhealthy, 200 },
          { HealthStatus.Healthy, 200 },
          { HealthStatus.Degraded, 200 },
        },
        Predicate = check => check.Name != "masstransit-bus",
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
      });
    });

    app.UseSwagger()
      .UseSwaggerUI(options =>
      {
        options.SwaggerEndpoint($"/swagger/{Version}/swagger.json", $"{Version}");
      });
  }

  #endregion
}
