﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MapToPalette.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("MapToPalette.Properties.Strings", typeof(Strings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dithering Amount.
        /// </summary>
        internal static string DitheringAmount {
            get {
                return ResourceManager.GetString("DitheringAmount", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dithering Method.
        /// </summary>
        internal static string DitheringMethod {
            get {
                return ResourceManager.GetString("DitheringMethod", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to None.
        /// </summary>
        internal static string DitheringMethodNone {
            get {
                return ResourceManager.GetString("DitheringMethodNone", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Map to Palette.
        /// </summary>
        internal static string EffectName {
            get {
                return ResourceManager.GetString("EffectName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Color.
        /// </summary>
        internal static string EffectSubMenuName {
            get {
                return ResourceManager.GetString("EffectSubMenuName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Map to Palette
        ///----------------
        ///Map image colors to the current palette. The first and last palette entry
        ///selections determine the range of colors to be used. Note that &quot;1&quot; is
        ///the first color.
        ///
        ///The &quot;Keep Opacity&quot;-checkbox controls how alpha-values are treated.
        ///By default the option is activated and only red, green, and blue are
        ///matched against the image colors. When deactivated, the alpha
        ///(opacity) value of the palette color is applied to the image as well,
        ///potentially turning transparent regions  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string HelpText {
            get {
                return ResourceManager.GetString("HelpText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Keep opacity .
        /// </summary>
        internal static string IgnoreAlpha {
            get {
                return ResourceManager.GetString("IgnoreAlpha", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Last Palette Index.
        /// </summary>
        internal static string PaletteEndIndex {
            get {
                return ResourceManager.GetString("PaletteEndIndex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to First Palette Index.
        /// </summary>
        internal static string PaletteStartIndex {
            get {
                return ResourceManager.GetString("PaletteStartIndex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap PluginIcon {
            get {
                object obj = ResourceManager.GetObject("PluginIcon", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
    }
}
