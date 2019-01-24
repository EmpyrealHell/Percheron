using Percheron.Interfaces.Chat;
using System;
using System.Collections.Generic;
using System.Text;

namespace Percheron.Interfaces.Plugin
{
    public interface IZoneHandler
    {
        /// <summary>
        /// Initializes the zone handler. The options will be loaded before this method is called, so that property should be set first.
        /// </summary>
        /// <param name="client">The client that handles messages for this zone.</param>
        /// <param name="zone">The chat zone this handler will process messages for.</param>
        void Initialize(ITwitchClient client, ChatZone zone);
    }
}
