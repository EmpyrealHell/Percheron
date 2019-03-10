using System;
using System.Collections.Generic;
using System.Text;

namespace Percheron.Interfaces.Plugin.Option
{
    public class PluginOption
    {
        public string Name { get; set; }
        public string Prompt { get; set; }
        public string Description { get; set; }
        public object Value { get; set; }

        public PluginOption(string name, string prompt, string description, object value)
        {
            this.Name = name;
            this.Prompt = prompt;
            this.Description = description;
            this.Value = value;
        }
    }
}
