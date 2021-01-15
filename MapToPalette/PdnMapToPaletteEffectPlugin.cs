using PaintDotNet;
using PaintDotNet.Effects;
using PaintDotNet.IndirectUI;
using PaintDotNet.PropertySystem;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace MapToPalette
{
    /// <summary>
    /// Paint.NET Effect Plugin implementation
    /// </summary>
    [PluginSupportInfo(typeof(PluginSupportInfo), DisplayName = "Map to Palette")]
    public class PdnMapToPaletteEffectPlugin : PropertyBasedEffect
    {
        /// <summary>
        /// Localisable effect name
        /// </summary>
        public static string EffectName => Properties.Strings.EffectName;

        /// <summary>
        /// Localisable effect submenu name
        /// </summary>
        public static string EffectSubMenuName => Properties.Strings.EffectSubMenuName;

        /// <summary>
        /// Effect icon
        /// </summary>
        public static Image Icon => Properties.Strings.PluginIcon;

        /// <summary>
        /// Dialog property identifiers
        /// </summary>
        public enum PropertyNames
        {
            /// <summary>First palette index</summary>
            PaletteStartIndex,
            /// <summary>Last palette index</summary>
            PaletteEndIndex,
            /// <summary>Ignore palette colour opacity</summary>
            IgnoreAlpha,
            /// <summary>Dithering method</summary>
            DitheringMethod,
            /// <summary>Dithering amount</summary>
            DitheringAmount
        }

        /// <summary>
        /// Selection of dithering methods (only error diffusion)
        /// </summary>
        public enum DitheringMethods
        {
            /// <summary>Disable colour dithering</summary>
            None,
            /// <summary>Use Floyd-Steinberg kernel</summary>
            FloydSteinberg,
            /// <summary>Use Jarvis kernel</summary>
            JarvisJudiceNinke,
            /// <summary>Use Stucki kernel</summary>
            Stucki,
            /// <summary>Use Burkes kernel</summary>
            Burkes,
            /// <summary>Use Sierra kernel</summary>
            Sierra
        }

        /// <summary>
        /// Initialise the plugin
        /// </summary>
        public PdnMapToPaletteEffectPlugin()
            : base(EffectName, Icon, EffectSubMenuName, new EffectOptions { Flags = EffectFlags.Configurable })
        { 
        }

        /// <summary>
        /// Return the plugin configuration dialog properties
        /// </summary>
        /// <returns>Dialog properties and property-rules</returns>
        protected override PropertyCollection OnCreatePropertyCollection()
        {
            var props = new Property[]
            {
                new Int32Property(PropertyNames.PaletteStartIndex, 1, 1, 96),
                new Int32Property(PropertyNames.PaletteEndIndex, 96, 1, 96),
                new BooleanProperty(PropertyNames.IgnoreAlpha, true),
                StaticListChoiceProperty.CreateForEnum<DitheringMethods>(PropertyNames.DitheringMethod, 0, false),
                new DoubleProperty(PropertyNames.DitheringAmount, 0.3, 0.0, 1.0)
            };

            var rules = new PropertyCollectionRule[]
            {
                new SoftMutuallyBoundMinMaxRule<int, Int32Property>(
                    PropertyNames.PaletteStartIndex, PropertyNames.PaletteEndIndex),
                new ReadOnlyBoundToValueRule<object, StaticListChoiceProperty>(
                    PropertyNames.DitheringAmount, PropertyNames.DitheringMethod, DitheringMethods.None, false),
                new ReadOnlyBoundToValueRule<double, DoubleProperty>(
                    PropertyNames.DitheringMethod, PropertyNames.DitheringAmount, 0.0, false)
            };

            return new PropertyCollection(props, rules);
        }

        /// <summary>
        /// Return property information
        /// </summary>
        /// <param name="props">UI properties</param>
        /// <returns>Additional UI configuration for each property</returns>
        protected override ControlInfo OnCreateConfigUI(PropertyCollection props)
        {
            var configUI = CreateDefaultConfigUI(props);

            configUI.SetPropertyControlValue(
                PropertyNames.PaletteStartIndex,
                ControlInfoPropertyNames.DisplayName,
                Properties.Strings.PaletteStartIndex);
            configUI.SetPropertyControlValue(
                PropertyNames.PaletteStartIndex, ControlInfoPropertyNames.SliderSmallChange, 1);
            configUI.SetPropertyControlValue(
                PropertyNames.PaletteStartIndex, ControlInfoPropertyNames.SliderLargeChange, 15);

            configUI.SetPropertyControlValue(
                PropertyNames.PaletteEndIndex, ControlInfoPropertyNames.DisplayName,
                Properties.Strings.PaletteEndIndex);
            configUI.SetPropertyControlValue(
                PropertyNames.PaletteEndIndex, ControlInfoPropertyNames.SliderSmallChange, 1);
            configUI.SetPropertyControlValue(
                PropertyNames.PaletteEndIndex, ControlInfoPropertyNames.SliderLargeChange, 15);

            configUI.SetPropertyControlValue(
                PropertyNames.IgnoreAlpha,
                ControlInfoPropertyNames.DisplayName,
                Properties.Strings.IgnoreAlpha);

            configUI.SetPropertyControlValue(
                PropertyNames.DitheringMethod,
                ControlInfoPropertyNames.DisplayName,
                Properties.Strings.DitheringMethod);
            var ditheringControl = configUI.FindControlForPropertyName(PropertyNames.DitheringMethod);
            ditheringControl.SetValueDisplayName(DitheringMethods.None, Properties.Strings.DitheringMethodNone);
            ditheringControl.SetValueDisplayName(DitheringMethods.FloydSteinberg, "Floyd-Steinberg");
            ditheringControl.SetValueDisplayName(DitheringMethods.JarvisJudiceNinke, "Jarvis-Judice-Ninke");
            ditheringControl.SetValueDisplayName(DitheringMethods.Stucki, "Stucki");
            ditheringControl.SetValueDisplayName(DitheringMethods.Burkes, "Burkes");
            ditheringControl.SetValueDisplayName(DitheringMethods.Sierra, "Sierra");

            configUI.SetPropertyControlValue(
                PropertyNames.DitheringAmount,
                ControlInfoPropertyNames.DisplayName,
                Properties.Strings.DitheringAmount);
            configUI.SetPropertyControlValue(
                PropertyNames.DitheringAmount, ControlInfoPropertyNames.SliderSmallChange, 0.1);
            configUI.SetPropertyControlValue(
                PropertyNames.DitheringAmount, ControlInfoPropertyNames.SliderLargeChange, 0.25);
            configUI.SetPropertyControlValue(
                PropertyNames.DitheringAmount, ControlInfoPropertyNames.UpDownIncrement, 0.05);

            return configUI;
        }

        /// <summary>
        /// Apply the selected property values
        /// </summary>
        /// <param name="newToken">UI property data</param>
        /// <param name="dstArgs">Destination information</param>
        /// <param name="srcArgs">Source information</param>
        protected override void OnSetRenderInfo(PropertyBasedEffectConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            var start = newToken.GetProperty<Int32Property>(PropertyNames.PaletteStartIndex).Value;
            var end = newToken.GetProperty<Int32Property>(PropertyNames.PaletteEndIndex).Value;
            var palettes = Services.GetService<PaintDotNet.AppModel.IPalettesService>();
            paletteBgra = palettes.CurrentPalette.Skip(start - 1).Take(end - start + 1).ToArray();

            dithering = (DitheringMethods)((int)newToken.GetProperty<StaticListChoiceProperty>(
                PropertyNames.DitheringMethod).Value);

            ditheringAmount = (float)newToken.GetProperty<DoubleProperty>(
                PropertyNames.DitheringAmount).Value;

            keepOpacity = newToken.GetProperty<BooleanProperty>(PropertyNames.IgnoreAlpha).Value;

            base.OnSetRenderInfo(newToken, dstArgs, srcArgs);
        }

        /// <summary>
        /// Apply additional dialog properties (help and title)
        /// </summary>
        /// <param name="props">Property collection</param>
        protected override void OnCustomizeConfigUIWindowProperties(PropertyCollection props)
        {
            base.OnCustomizeConfigUIWindowProperties(props);
            props[ControlInfoPropertyNames.WindowTitle].Value = EffectName;
            props[ControlInfoPropertyNames.WindowHelpContentType].Value = WindowHelpContentType.PlainText;
            props[ControlInfoPropertyNames.WindowHelpContent].Value = Properties.Strings.HelpText;
        }

        /// <summary>
        /// Render the effect
        /// </summary>
        /// <param name="renderRects">Regions to render</param>
        /// <param name="startIndex">Index o the first region</param>
        /// <param name="length">Number of regions</param>
        protected override void OnRender(Rectangle[] renderRects, int startIndex, int length)
        {
            if (ditheringAmount > 0.0 && dithering != DitheringMethods.None)
            {
                for (int i = 0; i < length; ++i)
                {
                    DoRenderDithered(DstArgs.Surface, SrcArgs.Surface, renderRects[startIndex + i]);
                }
            } else
            {
                for (int i = 0; i < length; ++i)
                {
                    DoRender(DstArgs.Surface, SrcArgs.Surface, renderRects[startIndex + i]);
                }
            }
        }

        // Render the effect wihout dithering
        private void DoRender(Surface dest, Surface source, Rectangle roi)
        {
            var doMatch = ColourMatcher;
            for (int y = roi.Top; y < roi.Bottom; ++y)
            {
                for (int x = roi.Left; x < roi.Right; ++x)
                {
                    var colour = source[x, y];
                    var mapped = doMatch(colour);
                    dest[x, y] = mapped;
                }
            }
        }

        // Render the effect using the selected error diffusion method
        private void DoRenderDithered(Surface dest, Surface source, Rectangle roi)
        {
            var dither = new ErrorDiffusion(kernels[dithering], ditheringAmount, roi.Width);
            var doMatch = ColourMatcher;
            for (int y = roi.Top; y < roi.Bottom; ++y)
            {
                for (int x = roi.Left, col = 0; x < roi.Right; ++x, ++col)
                {
                    var colour = source[x, y];
                    var mapped = dither.GetFinalColour(col, colour, doMatch);
                    dest[x, y] = mapped;
                }
                dither.MoveToNextLine();
            }
        }

        // Select a colour match function depending on opacity settings
        private Func<ColorBgra, ColorBgra> ColourMatcher
            => (keepOpacity ? MatchNoOpacity : (Func<ColorBgra, ColorBgra>)Match).Memoise();

        // Match to palette inculding alpha channel
        private ColorBgra Match(ColorBgra colour)
            => paletteBgra.MinBy(c => c.SquaredDistance(colour));

        // Match only colour to palette, keep opacity as-is
        private ColorBgra MatchNoOpacity(ColorBgra colour)
            => paletteBgra.MinBy(entry => entry.SquaredDistance(colour)).NewAlpha(colour.A);

        // Current palette - colours from start to end index
        private IList<ColorBgra> paletteBgra = new ColorBgra[]
        {
            ColorBgra.Black, ColorBgra.White
        };

        // Selected dithering method
        private DitheringMethods dithering;

        // Dithering amount (0 to 1)
        private float ditheringAmount;

        // Keep current opacity and ignore palette alpha
        private bool keepOpacity;

        // Map from dithering method to dithering kernel
        private static readonly IDictionary<DitheringMethods, DitherKernel> kernels =
            new Dictionary<DitheringMethods, DitherKernel>
            {
                { DitheringMethods.None, null },
                { DitheringMethods.FloydSteinberg, DitherKernel.FloydSteinberg },
                { DitheringMethods.JarvisJudiceNinke, DitherKernel.JarvisJudiceNinke },
                { DitheringMethods.Burkes, DitherKernel.Burkes },
                { DitheringMethods.Stucki, DitherKernel.Stucki },
                { DitheringMethods.Sierra, DitherKernel.Sierra }
            };
    }
}
