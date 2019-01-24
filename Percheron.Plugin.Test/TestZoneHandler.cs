using Percheron.Interfaces.Chat;
using Percheron.Interfaces.Plugin;
using System;
using System.Collections.Generic;
using System.Text;

namespace Percheron.Plugin.Test
{
    public class TestZoneHandler : IZoneHandler
    {
        public IEnumerable<PluginOption> Options { get; private set; }

        public TestZoneHandler()
        {
            this.Options = new List<PluginOption>();
        }

        public void Initialize(ITwitchClient client, ChatZone zone)
        {
            throw new NotImplementedException();
        }
    }
}
