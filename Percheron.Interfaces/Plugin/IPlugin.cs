using Percheron.Interfaces.Chat;
using System;
using System.Collections.Generic;
using System.Text;

namespace Percheron.Interfaces.Plugin
{
    public interface IPlugin
    {
        /// <summary>
        /// Perfoms initial setup of the plugin when the application is launched. The options will be loaded before this method is called, so that property should be set first.
        /// </summary>
        void Initialize();
        /// <summary>
        /// Provides a zone handler to attach to a zone for processing chat messages.
        /// </summary>
        /// <param name="zone">The chat zone this handler will be attached to.</param>
        /// <returns>The handler to use for processing messages from this zone. The same handler can be returned to multiple zones to more easily share data between them.</returns>
        IZoneHandler Attach(ChatZone zone);
    }
}
