using Core.Library.ArivuTharavuThalam.DataCaching;
using Core.Library.ArivuTharavuThalam.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Core.API.AdditionalService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class PingController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public PingController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet("Environment")]
        public JsonResult GetEnvironment()
        {
            IDictionary<string, string> keyValuePairs = new Dictionary<string, string>();

            keyValuePairs.Add("ProcessName", Process.GetCurrentProcess().ProcessName);
            keyValuePairs.Add("HostTime", DateTime.Now.ToString());
            keyValuePairs.Add("ASPNETCORE_ENVIRONMENT", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));

            string value = JsonConvert.SerializeObject(keyValuePairs);

            Log.Information(value);

            return new JsonResult(keyValuePairs);
        }

        [HttpGet("Config")]
        public JsonResult GetConfig()
        {
            IDictionary<string, string> keyValuePairs = new Dictionary<string, string>
            {
                { "Curren Application Insights Type", configuration.CurrenApplicationInsightsType() },
                { "Current DB Connection", configuration.GetConnectionString(configuration.CurrentDBContext()) }
            };

            //IDENTITY SERVER SETTINGS
            var identityServerSettingsOption = configuration.GetSection("IdentityServerSettings");
            var identityServerSettings = identityServerSettingsOption.Get<IdentityServerSettings>();
            keyValuePairs.Add("Curren IdentityServer Type", identityServerSettings.Audience);


            //CURRENT CACHING TYPE
            switch (configuration.CurrenCachingType())
            {
                case "Empty":
                default:
                    keyValuePairs.Add("Curren Caching Type", "Empty");
                    break;

                case "RedisCache":
                    var redisCacheSettingOption = configuration.GetSection("RedisCacheSetting");
                    var redisCacheSetting = redisCacheSettingOption.Get<RedisCacheSetting>();

                    keyValuePairs.Add("Curren Caching Type", redisCacheSetting.ConnectionStrings + " - " + redisCacheSetting.ApplicationInstanceName);

                    break;
            }

            //CURRENT MESSAGING TYPE
            switch (configuration.CurrentMessageType())
            {
                case "Empty":
                default:
                    keyValuePairs.Add("Curren Messaging Type", "Empty");
                    break;

                case "AzureQueue":

                    var azureQueuesMessageSettingsOption = configuration.GetSection("AzureQueuesMessageSettings");
                    var azureQueuesMessageSettings = azureQueuesMessageSettingsOption.Get<AzureQueuesMessageSettings>();
                    keyValuePairs.Add("Curren Messaging Type", azureQueuesMessageSettings.ConnectionStrings);
                    break;

                case "AzureTopic":

                    var azureTopicMessageSettingsOption = configuration.GetSection("AzureQueuesMessageSettings");
                    var azureTopicMessageSettings = azureTopicMessageSettingsOption.Get<AzureQueuesMessageSettings>();
                    keyValuePairs.Add("Curren Messaging Type", azureTopicMessageSettings.ConnectionStrings);
                    break;

                case "RabbitMQ":

                    var rabbitMQServiceSettingsOption = configuration.GetSection("RabbitMQServiceSettings");
                    var rabbitMQServiceSettings = rabbitMQServiceSettingsOption.Get<RabbitMQServiceSettings>();
                    keyValuePairs.Add("Curren Messaging Type", rabbitMQServiceSettings.HostName + ":" + rabbitMQServiceSettings.HostPort);
                    break;

                case "RedisQueues":

                    var redisQueuesMessageSettingsOption = configuration.GetSection("RedisQueuesMessageSettings");
                    var redisQueuesMessageSettings = redisQueuesMessageSettingsOption.Get<RedisQueuesMessageSettings>();
                    keyValuePairs.Add("Curren Messaging Type", redisQueuesMessageSettings.ConnectionStrings);
                    break;
            }

            return new JsonResult(keyValuePairs);
        }
    }
}