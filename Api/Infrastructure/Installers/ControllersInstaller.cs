using System.Web.Http.Controllers;
using Castle.MicroKernel.Registration;

namespace Api.Infrastructure.Installers
{
    public class ControllersInstaller : IWindsorInstaller
    {
        public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            container.Register(Classes.FromThisAssembly()
                                      .BasedOn<IHttpController>()
                                      .LifestyleTransient());
        }
    }
}