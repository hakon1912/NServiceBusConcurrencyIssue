using System.Configuration;
using Castle.Windsor;
using NServiceBus;
using NServiceBus.Features;
using NServiceBus.Logging;
using NServiceBus.Serilog;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Serilog;
using IfInsurance.Messages.Commands;

namespace Api.Infrastructure
{
    public class EndpointConfig 
    {
        public async Task Configure(IWindsorContainer container)
        {
            LogConfig.ConfigureLogging();

            var endpointConfiguration = new EndpointConfiguration(ConfigurationManager.AppSettings["Environment.ShortName"] + "_claims_invoicereceiver");
            
            // You should not always invoke the installers
            //endpointConfiguration.EnableInstallers();

            var transport = endpointConfiguration.UseTransport<MsmqTransport>();


            // Use the routing API
            var routing = transport.Routing();
            routing.RouteToEndpoint(typeof(SomeCommand), "Messaging.Endpoint.Claims.InvoiceReceiver");

            endpointConfiguration.LimitMessageProcessingConcurrencyTo(int.Parse(ConfigurationManager.AppSettings["Messaging.MaximumConcurrencyLevel"]));
            endpointConfiguration.Recoverability().Immediate(cfg => cfg.NumberOfRetries(int.Parse(ConfigurationManager.AppSettings["Messaging.MaxRetries"])));
            endpointConfiguration.Recoverability().Delayed(cfg => cfg.NumberOfRetries(0));

            endpointConfiguration.DisableFeature<TimeoutManager>();
            
            endpointConfiguration.UseContainer<WindsorBuilder>(customizations => customizations.ExistingContainer(container));

            endpointConfiguration.UsePersistence<NHibernatePersistence>();

            // Use this to configure an error queue, but as this is a send only endpoint it should not be required
            endpointConfiguration.SendFailedMessagesTo(ConfigurationManager.AppSettings["Messaging.ErrorQueue"]);

            var commandConvention = new CommandConvention();
            var conventions = endpointConfiguration.Conventions();

            conventions.DefiningCommandsAs(type => typeof(ICommand).IsAssignableFrom(type) || commandConvention.IsMatch(type));

            LogManager.Use<SerilogFactory>();

            endpointConfiguration.SendOnly();

            var startableEndpoint = await Endpoint.Create(endpointConfiguration).ConfigureAwait(false);
            var endpointInstance = await startableEndpoint.Start().ConfigureAwait(false);

            Log.Information("Endpointconfig started");

            //await endpointInstance.Send(new ProcessFinvoiceCommand());

            container.Register(Component.For<IEndpointInstance>().Instance(endpointInstance));
        }

        private System.Reflection.Assembly[] AssembliesToScan
        {
            get
            {
                return new[]
                {
                    typeof (IHandleMessages<>).Assembly,
                    typeof (NHibernatePersistence).Assembly,
                    typeof (SomeCommand).Assembly,
                    System.Reflection.Assembly.GetExecutingAssembly()
                };
            }
        }
    }
}
