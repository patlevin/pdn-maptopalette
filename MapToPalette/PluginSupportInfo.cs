using PaintDotNet;
using System;
using System.Reflection;

namespace MapToPalette
{
    /// <summary>
    /// Plugin support information data
    /// </summary>
    public class PluginSupportInfo : IPluginSupportInfo
    {
        /// <summary>
        /// Display name of the plugin
        /// </summary>
        public string DisplayName => this.GetCustomAttribute<AssemblyProductAttribute>().Product;

        /// <summary>
        /// Plugin author
        /// </summary>
        public string Author => this.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright;

        /// <summary>
        /// Plugin copyright information
        /// </summary>
        public string Copyright => this.GetCustomAttribute<AssemblyDescriptionAttribute>().Description;

        /// <summary>
        /// Plugin version
        /// </summary>
        public Version Version => GetType().Assembly.GetName().Version;

        /// <summary>
        /// Plugin URI
        /// </summary>
        public Uri WebsiteUri => new Uri("http://www.getpaint.net/redirect/plugins.html");
    }
}
