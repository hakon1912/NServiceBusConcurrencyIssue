using System.Configuration;
using NServiceBus.Config;
using NServiceBus.Config.ConfigurationSource;

namespace Api.Infrastructure
{
    // Should not be used anymore


    //public class MessageForwardingInCaseOfFaultConfigConfig : IProvideConfiguration<MessageForwardingInCaseOfFaultConfig>
    //{
    //    public MessageForwardingInCaseOfFaultConfig GetConfiguration()
    //    {
    //        var config =
    //            (MessageForwardingInCaseOfFaultConfig)ConfigurationManager.GetSection(typeof(MessageForwardingInCaseOfFaultConfig).Name) ??
    //            new MessageForwardingInCaseOfFaultConfig();

    //        config.ErrorQueue = ConfigurationManager.AppSettings["Messaging.ErrorQueue"];

    //        return config;
    //    }
    //}
}