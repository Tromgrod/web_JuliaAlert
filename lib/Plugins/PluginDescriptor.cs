using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace LIB.Plugins
{
    using LIB.Infrastructure;

    public sealed class PluginDescriptor : IComparable<PluginDescriptor>
    {
        public PluginDescriptor()
        {
            this.SupportedVersions = new List<string>();
            this.LimitedToClients = new List<int>();
        }


        public PluginDescriptor(Assembly referencedAssembly, FileInfo originalAssemblyFile,
            Type pluginType)
            : this()
        {
            this.ReferencedAssembly = referencedAssembly;
            this.OriginalAssemblyFile = originalAssemblyFile;
            this.PluginType = pluginType;
        }
        /// <summary>
        /// Plugin type
        /// </summary>
        public string PluginFileName { get; set; }

        /// <summary>
        /// Plugin type
        /// </summary>
        public Type PluginType { get; set; }

        /// <summary>
        /// The assembly that has been shadow copied that is active in the application
        /// </summary>
        public Assembly ReferencedAssembly { get; internal set; }

        /// <summary>
        /// The original assembly file that a shadow copy was made from it
        /// </summary>
        public FileInfo OriginalAssemblyFile { get; internal set; }

        /// <summary>
        /// Gets or sets the plugin group
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Gets or sets the friendly name
        /// </summary>
        public string FriendlyName { get; set; }

        /// <summary>
        /// Gets or sets the system name
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// Gets or sets the version
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the supported versions of nopCommerce
        /// </summary>
        public IList<string> SupportedVersions { get; set; }

        /// <summary>
        /// Gets or sets the author
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the list of store identifiers in which this plugin is available. If empty, then this plugin is available in all stores
        /// </summary>
        public IList<int> LimitedToClients { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether plugin is installed
        /// </summary>
        public bool Installed { get; set; }

        public T Instance<T>() where T : class, IPlugin
        {
            object instance;
            if (!EngineContext.Current.ContainerManager.TryResolve(PluginType, null, out instance))
            {
                //not resolved
                instance = EngineContext.Current.ContainerManager.ResolveUnregistered(PluginType);
            }
            var typedInstance = instance as T;
            if (typedInstance != null)
                typedInstance.PluginDescriptor = this;
            return typedInstance;
        }

        public IPlugin Instance()
        {
            return Instance<IPlugin>();
        }

        public int CompareTo(PluginDescriptor other)
        {
            if (DisplayOrder != other.DisplayOrder)
                return DisplayOrder.CompareTo(other.DisplayOrder);
            else
                return FriendlyName.CompareTo(other.FriendlyName);
        }

        public override string ToString()
        {
            return FriendlyName;
        }

        public override bool Equals(object obj)
        {
            var other = obj as PluginDescriptor;
            return other != null && 
                SystemName != null &&
                SystemName.Equals(other.SystemName);
        }

        public override int GetHashCode()
        {
            return SystemName.GetHashCode();
        }
    }
}
