using Owin;
using System.Web.Http;
using Api.Infrastructure;
using System.Web.Http.Dispatcher;
using System.Threading.Tasks;

namespace Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            app.UseWebApi(config);

            config.MapHttpAttributeRoutes();

            ConfigureJsonFormatting(config);

            var container = new Bootstrapper().CreateAndConfigureContainer();
            var t = Task.Run(() => new EndpointConfig().Configure(container));
            Task.WaitAll(t);

            config.Services.Replace(typeof(IHttpControllerActivator),
                new WindsorCompositionRoot(container));
        }

        private void ConfigureJsonFormatting(HttpConfiguration config)
        {
            var settings = config.Formatters.JsonFormatter.SerializerSettings;
            settings.Formatting = Newtonsoft.Json.Formatting.Indented;
            settings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        }
    }
}
