using Percheron.Interfaces.Chat;
using Percheron.Interfaces.Plugin;
using System;
using System.Collections.Generic;
using System.Composition;

namespace Percheron.Plugin.Test
{
    [PluginMetadata("Test")]
    public class TestPlugin : IPlugin
    {
        public IEnumerable<PluginOption> Options { get; private set; }

        public TestPlugin()
        {
            var options = new List<PluginOption>();
            options.Add(new BooleanOption("DoThing", "Do the thing?", true));
            this.Options = options;
        }

        public void Initialize()
        {
        }

        public IZoneHandler Attach(ChatZone zone)
        {
            return new TestZoneHandler();
        }
    }
}
