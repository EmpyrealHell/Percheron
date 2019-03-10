using System;
using System.Collections.Generic;
using System.Text;

namespace Percheron.Interfaces.Plugin.Option
{
    public class BooleanOption : PluginOption
    {
        public BooleanOption(string name, string prompt, string description, bool value) : base(name, prompt, description, value)
        {
        }
    }
}
