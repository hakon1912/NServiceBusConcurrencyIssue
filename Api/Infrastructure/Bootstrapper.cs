using Castle.Windsor;
using Castle.Windsor.Installer;
using Serilog;

namespace Api.Infrastructure
{
    public class Bootstrapper
    {
        public IWindsorContainer CreateAndConfigureContainer()
        {
            var container = new WindsorContainer();
            container.Install(FromAssembly.This());

            return container;
        }
    }
}