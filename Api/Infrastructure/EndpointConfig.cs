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
            endpointConfiguration.EnableInstallers();

            var transport = endpointConfiguration.UseTransport<MsmqTransport>();

            endpointConfiguration.LimitMessageProcessingConcurrencyTo(int.Parse(ConfigurationManager.AppSettings["Messaging.MaximumConcurrencyLevel"]));
            endpointConfiguration.Recoverability().Immediate(cfg => cfg.NumberOfRetries(int.Parse(ConfigurationManager.AppSettings["Messaging.MaxRetries"])));
            endpointConfiguration.Recoverability().Delayed(cfg => cfg.NumberOfRetries(0));

            endpointConfiguration.DisableFeature<TimeoutManager>();
            
            endpointConfiguration.UseContainer<WindsorBuilder>(customizations => customizations.ExistingContainer(container));

            endpointConfiguration.UsePersistence<NHibernatePersistence>();

            var commandConvention = new CommandConvention();
            var conventions = endpointConfiguration.Conventions();

            conventions.DefiningCommandsAs(type =>
                typeof(ICommand).IsAssignableFrom(type) || commandConvention.IsMatch(type));

            LogManager.Use<SerilogFactory>();

            endpointConfiguration.SendOnly();

            var startableEndpoint = await Endpoint.Create(endpointConfiguration);
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
