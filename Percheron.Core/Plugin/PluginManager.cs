using Percheron.Interfaces.Plugin;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Percheron.Core.Plugin
{
    public class PluginManager
    {
        public IEnumerable<ExportFactory<IPlugin, PluginMetadata>> Plugins { get; private set; }

        public void Load()
        {
            var conventions = new ConventionBuilder();
            conventions.ForTypesDerivedFrom<IPlugin>()
                .Export<IPlugin>()
                .Shared();
            conventions.ForTypesDerivedFrom<IPluginMetadata>()
                .Export()
                .Shared();

            var assemblies = Directory.GetFiles(Environment.CurrentDirectory + "/plugins", "*.dll")
                .Select(path => Assembly.LoadFile(path))
                .Where(x => x != null);

            var configuration = new ContainerConfiguration()
                .WithAssemblies(assemblies, conventions)
                .WithAssembly(this.GetType().Assembly);

            var container = configuration.CreateContainer();
            this.Plugins = container.GetExports<ExportFactory<IPlugin, PluginMetadata>>();
            foreach (var plugin in this.Plugins)
            {
                Console.WriteLine(plugin.Metadata.Name);
                plugin.CreateExport().Value.Initialize();
            }
        }
    }
}
