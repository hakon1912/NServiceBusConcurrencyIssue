using System.Configuration;
using IfInsurance.Messages.Commands;
using NServiceBus.Config;
using NServiceBus.Config.ConfigurationSource;

namespace Api.Infrastructure
{
    // SHoudl not be used, should use the code API


    //public class UnicastBusConfigProvider : IProvideConfiguration<UnicastBusConfig>
    //{
    //    public UnicastBusConfig GetConfiguration()
    //    {
    //        UnicastBusConfig config = (UnicastBusConfig)ConfigurationManager.GetSection(typeof(UnicastBusConfig).Name) ?? new UnicastBusConfig();

    //        config.MessageEndpointMappings.Add(new MessageEndpointMapping
    //        {
    //            Endpoint = ConfigurationManager.AppSettings["Messaging.Endpoint.Claims.InvoiceReceiver"],
    //            Messages = typeof(SomeCommand).Assembly.GetName().Name
    //        });

    //        return config;
    //    }
    //}
}