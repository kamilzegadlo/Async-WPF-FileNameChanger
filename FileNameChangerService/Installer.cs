using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;

namespace FileNameChanger.Service
{
    public class Installer : IWindsorInstaller
    {
        public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            container
                .Register(Component.For<IFileNameChangerService>().ImplementedBy<FileNameChangerService>().LifestyleTransient());
        }
    }
}
