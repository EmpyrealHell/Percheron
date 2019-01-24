using System;
using System.Collections.Generic;
using System.Composition;
using System.Text;

namespace Percheron.Interfaces.Plugin
{
    public interface IPluginMetadata
    {
        string Name { get; set; }
    }

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PluginMetadataAttribute : Attribute, IPluginMetadata
    {
        public string Name { get; set; }

        public PluginMetadataAttribute(string name) : base()
        {
            this.Name = name;
        }
    }

    public class PluginMetadata : IPluginMetadata
    {
        public string Name { get; set; }
    }
}
