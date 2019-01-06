using Percheron.Interfaces.Plugin;
using System;
using System.Collections.Generic;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Text;

namespace Percheron.Core
{
    public class PluginManager
    {
        public IEnumerable<IPlugin> Plugins { get; private set; }

        public void Load()
        {
            var conventions = new ConventionBuilder();
            conventions.ForTypesDerivedFrom<IPlugin>()
                .Export<IPlugin>()
                .Shared();

            var assemblies = Directory.GetFiles("./plugins", "*.dll")
                .Select(path => AssemblyLoadContext.Default.LoadFromAssemblyPath(path))
                .Where(x => x != null);

            var configuration = new ContainerConfiguration()
                .WithAssemblies(assemblies, conventions)
                .WithAssembly(this.GetType().Assembly);

            var container = configuration.CreateContainer();
            this.Plugins = container.GetExports<IPlugin>();
        }
    }
}
