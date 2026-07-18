// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;
using WpfButton = System.Windows.Controls.Button;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Adds icon content to a standard button.</summary>
public class AppBarButton : WpfButton
{
    /// <summary>The background glyph font size property.</summary>
    public static readonly DependencyProperty BackgroundGlyphFontSizeProperty = DependencyProperty.Register(
        nameof(BackgroundGlyphFontSize),
        typeof(double),
        typeof(AppBarButton),
        new PropertyMetadata(33D));

    /// <summary>The background glyph property.</summary>
    public static readonly DependencyProperty BackgroundGlyphProperty = DependencyProperty.Register(
        nameof(BackgroundGlyph),
        typeof(string),
        typeof(AppBarButton),
        new PropertyMetadata(string.Empty));

    /// <summary>Identifies the Ellipse Diameter property.</summary>
    public static readonly DependencyProperty ElipseDiameterProperty = DependencyProperty.Register(
        nameof(ElipseDiameter),
        typeof(double),
        typeof(AppBarButton),
        new PropertyMetadata(40D));

    /// <summary>The foreground glyph color property.</summary>
    public static readonly DependencyProperty ForegroundGlyphColorProperty = DependencyProperty.Register(
        nameof(ForegroundGlyphColor),
        typeof(Brush),
        typeof(AppBarButton),
        new PropertyMetadata(Brushes.LightGray));

    /// <summary>The foreground glyph font size property.</summary>
    public static readonly DependencyProperty ForegroundGlyphFontSizeProperty = DependencyProperty.Register(
        nameof(ForegroundGlyphFontSize),
        typeof(double),
        typeof(AppBarButton),
        new PropertyMetadata(33D));

    /// <summary>The foreground glyph property.</summary>
    public static readonly DependencyProperty ForegroundGlyphProperty = DependencyProperty.Register(
        nameof(ForegroundGlyph),
        typeof(string),
        typeof(AppBarButton),
        new PropertyMetadata(string.Empty));

    /// <summary>Identifies the IconData property.</summary>
    public static readonly DependencyProperty IconDataProperty = DependencyProperty.Register(
        nameof(IconData),
        typeof(Geometry),
        typeof(AppBarButton));

    /// <summary>Identifies the IconHeight property.</summary>
    public static readonly DependencyProperty IconHeightProperty = DependencyProperty.Register(
        nameof(IconHeight),
        typeof(double),
        typeof(AppBarButton),
        new PropertyMetadata(25D));

    /// <summary>Identifies the StandardIcon property.</summary>
    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        nameof(Icon),
        typeof(AppBarIcons),
        typeof(AppBarButton),
        new PropertyMetadata(AppBarIcons.None, SetIcon));

    /// <summary>Identifies the IconWidth property.</summary>
    public static readonly DependencyProperty IconWidthProperty = DependencyProperty.Register(
        nameof(IconWidth),
        typeof(double),
        typeof(AppBarButton),
        new PropertyMetadata(25D));

    /// <summary>Loads and caches the embedded icon geometries used by app bar buttons.</summary>
    private static readonly AppBarIconProvider IconProvider = new(typeof(AppBarButton).Assembly);

    /// <summary>Initializes static members of the <see cref="AppBarButton"/> class.</summary>
    static AppBarButton() =>
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(AppBarButton),
            new FrameworkPropertyMetadata(typeof(AppBarButton)));

    /// <summary>Gets or sets the background glyph.</summary>
    /// <value>The background glyph.</value>
    [Description("Gets/Sets the Background Glyph value")]
    [Category("Common")]
    public string BackgroundGlyph
    {
        get => (string)GetValue(BackgroundGlyphProperty);
        set => SetValue(BackgroundGlyphProperty, value);
    }

    /// <summary>Gets or sets the size of the background glyph font.</summary>
    /// <value>The size of the background glyph font.</value>
    [Description("Gets/Sets the Background Glyph FontSize value")]
    [Category("Common")]
    public double BackgroundGlyphFontSize
    {
        get => (double)GetValue(BackgroundGlyphFontSizeProperty);
        set => SetValue(BackgroundGlyphFontSizeProperty, value);
    }

    /// <summary>Gets or sets the Ellipse Diameter.</summary>
    /// <value>The Ellipse Diameter.</value>
    [Description("Gets/Sets the Ellipse Diameter value")]
    [Category("Layout")]
    public double ElipseDiameter
    {
        get => (double)GetValue(ElipseDiameterProperty);
        set => SetValue(ElipseDiameterProperty, value);
    }

    /// <summary>Gets or sets the foreground glyph.</summary>
    /// <value>The foreground glyph.</value>
    [Description("Gets/Sets the Foreground Glyph value")]
    [Category("Common")]
    public string ForegroundGlyph
    {
        get => (string)GetValue(ForegroundGlyphProperty);
        set => SetValue(ForegroundGlyphProperty, value);
    }

    /// <summary>Gets or sets the color of the foreground glyph.</summary>
    /// <value>The color of the foreground glyph.</value>
    [Description("Gets/Sets the Foreground Glyph Color value")]
    [Category("Brush")]
    public Brush ForegroundGlyphColor
    {
        get => (Brush)GetValue(ForegroundGlyphColorProperty);
        set => SetValue(ForegroundGlyphColorProperty, value);
    }

    /// <summary>Gets or sets the size of the foreground glyph font.</summary>
    /// <value>The size of the foreground glyph font.</value>
    [Description("Gets/Sets the Foreground Glyph FontSize value")]
    [Category("Common")]
    public double ForegroundGlyphFontSize
    {
        get => (double)GetValue(ForegroundGlyphFontSizeProperty);
        set => SetValue(ForegroundGlyphFontSizeProperty, value);
    }

    /// <summary>Gets or sets the icon path data from a list of default Icons.</summary>
    /// <value>The icon path data.</value>
    [Description("Gets/Sets the Icon value")]
    [Category("Common")]
    public AppBarIcons Icon
    {
        get
        {
            var icon = GetValue(IconProperty);
            return icon is null ? default : (AppBarIcons)icon;
        }
        set => SetValue(IconProperty, value);
    }

    /// <summary>Gets or sets the icon path data.</summary>
    /// <value>The icon path data.</value>
    [Description("Gets/Sets the IconData value")]
    [Category("Common")]
    public Geometry? IconData
    {
        get
        {
            var iconData = GetValue(IconDataProperty);
            return iconData is null ? default : (Geometry)iconData;
        }
        set => SetValue(IconDataProperty, value);
    }

    /// <summary>Gets or sets the icon height.</summary>
    /// <value>The icon height.</value>
    [Description("Gets/Sets the Icon Height value")]
    [Category("Layout")]
    public double IconHeight
    {
        get => (double)GetValue(IconHeightProperty);
        set => SetValue(IconHeightProperty, value);
    }

    /// <summary>Gets or sets the icon width.</summary>
    /// <value>The icon width.</value>
    [Description("Gets/Sets the Icon Width value")]
    [Category("Layout")]
    public double IconWidth
    {
        get => (double)GetValue(IconWidthProperty);
        set => SetValue(IconWidthProperty, value);
    }

    /// <summary>Sets the icon.</summary>
    /// <param name="d">The d.</param>
    /// <param name="e">
    /// The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.
    /// </param>
    private static void SetIcon(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not AppBarButton appBarButton)
        {
            return;
        }

        var icon = (AppBarIcons)e.NewValue;
        appBarButton.IconData = icon == AppBarIcons.None ? default : IconProvider.GetIcon(icon);
    }

    /// <summary>Creates complete WPF geometries from the icon resource shapes.</summary>
    private static class AppBarIconGeometryFactory
    {
        /// <summary>Creates bounds from WPF shape attributes.</summary>
        /// <param name="element">The shape element.</param>
        /// <returns>The shape bounds.</returns>
        public static Rect CreateBounds(XElement element) =>
            new(
                GetAttributeValue(element, "Canvas.Left"),
                GetAttributeValue(element, "Canvas.Top"),
                GetAttributeValue(element, nameof(Width)),
                GetAttributeValue(element, nameof(Height)));

        /// <summary>Adds parsed path data to a geometry group.</summary>
        /// <param name="geometry">The destination geometry group.</param>
        /// <param name="data">The serialized path data.</param>
        public static void AddPathData(GeometryGroup geometry, string? data)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                return;
            }

            geometry.Children.Add(Geometry.Parse(data));
        }

        /// <summary>Validates and freezes a loaded icon geometry.</summary>
        /// <param name="geometry">The geometry to complete.</param>
        /// <param name="resourceName">The source resource name.</param>
        /// <returns>The frozen geometry.</returns>
        public static Geometry CompleteGeometry(GeometryGroup geometry, string resourceName)
        {
            if (geometry.Children.Count == 0)
            {
                throw new InvalidDataException(
                    $"The embedded icon resource '{resourceName}' does not contain supported geometry.");
            }

            Geometry combinedGeometry = geometry.Children[0];
            for (var index = 1; index < geometry.Children.Count; index++)
            {
                combinedGeometry = Geometry.Combine(
                    combinedGeometry,
                    geometry.Children[index],
                    GeometryCombineMode.Union,
                    Transform.Identity);
            }

            combinedGeometry.Freeze();
            return combinedGeometry;
        }

        /// <summary>Reads a numeric XAML attribute, using zero when it is absent.</summary>
        /// <param name="element">The element containing the attribute.</param>
        /// <param name="attributeName">The attribute name.</param>
        /// <returns>The numeric attribute value.</returns>
        private static double GetAttributeValue(XElement element, string attributeName)
        {
            var value = (string?)element.Attribute(attributeName);
            return string.IsNullOrWhiteSpace(value)
                ? 0D
                : double.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture);
        }
    }

    /// <summary>Loads icon geometries from the resources embedded with the control.</summary>
    private sealed class AppBarIconProvider
    {
        /// <summary>The prefix used for legacy AppBar resource names.</summary>
        private const string AppBarPrefix = "Ab_";

        /// <summary>The prefix used for Material Design resource names.</summary>
        private const string MaterialDesignPrefix = "Md_";

        /// <summary>The common manifest resource prefix for migrated AppBarButton assets.</summary>
#if REACTIVE_SHIM
        private const string ResourcePrefix = "CrissCross.Reactive.WPF.UI.Controls.AppBarButton.Assets";
#else
        private const string ResourcePrefix = "CrissCross.WPF.UI.Controls.AppBarButton.Assets";
#endif

        /// <summary>Stores the assembly containing the embedded icon resources.</summary>
        private readonly Assembly _assembly;

        /// <summary>Caches parsed and frozen geometries by icon.</summary>
        private readonly ConcurrentDictionary<AppBarIcons, Geometry> _cache = new();

        /// <summary>Initializes a new instance of the <see cref="AppBarIconProvider"/> class.</summary>
        /// <param name="assembly">The assembly containing the icon resources.</param>
        public AppBarIconProvider(Assembly assembly) => _assembly = assembly;

        /// <summary>Gets the geometry for an icon.</summary>
        /// <param name="icon">The icon to load.</param>
        /// <returns>The cached icon geometry.</returns>
        public Geometry GetIcon(AppBarIcons icon) => _cache.GetOrAdd(icon, LoadIcon);

        /// <summary>Loads and parses an icon geometry.</summary>
        /// <param name="icon">The icon to load.</param>
        /// <returns>The parsed icon geometry.</returns>
        private Geometry LoadIcon(AppBarIcons icon)
        {
            var iconName = icon.ToString();
            if (iconName.StartsWith(AppBarPrefix, StringComparison.Ordinal))
            {
                return LoadAppBarIcon(iconName);
            }

            if (iconName.StartsWith(MaterialDesignPrefix, StringComparison.Ordinal))
            {
                return LoadMaterialDesignIcon(iconName);
            }

            throw new ArgumentOutOfRangeException(
                nameof(icon),
                icon,
                "The icon does not have a supported resource prefix.");
        }

        /// <summary>Loads a legacy AppBar icon, including multi-path and basic shape assets.</summary>
        /// <param name="iconName">The enum member name.</param>
        /// <returns>The icon geometry.</returns>
        private Geometry LoadAppBarIcon(string iconName)
        {
            var shortIconName = iconName[AppBarPrefix.Length..].Replace('_', '.');
            var resourceName = $"{ResourcePrefix}.AppBar.appbar.{shortIconName}.xaml";
            var document = LoadDocument(resourceName);
            XNamespace presentation = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
            var geometry = new GeometryGroup();

            foreach (var path in document.Descendants(presentation + "Path"))
            {
                AppBarIconGeometryFactory.AddPathData(geometry, (string?)path.Attribute("Data"));
            }

            foreach (var ellipse in document.Descendants(presentation + "Ellipse"))
            {
                geometry.Children.Add(new EllipseGeometry(AppBarIconGeometryFactory.CreateBounds(ellipse)));
            }

            foreach (var rectangle in document.Descendants(presentation + "Rectangle"))
            {
                geometry.Children.Add(new RectangleGeometry(AppBarIconGeometryFactory.CreateBounds(rectangle)));
            }

            return AppBarIconGeometryFactory.CompleteGeometry(geometry, resourceName);
        }

        /// <summary>Loads a Material Design SVG icon, including every path in the asset.</summary>
        /// <param name="iconName">The enum member name.</param>
        /// <returns>The icon geometry.</returns>
        private Geometry LoadMaterialDesignIcon(string iconName)
        {
            var shortIconName = iconName[MaterialDesignPrefix.Length..].Replace('_', '-');
            var resourceName = $"{ResourcePrefix}.svg.{shortIconName}.svg";
            var document = LoadDocument(resourceName);
            XNamespace svg = "http://www.w3.org/2000/svg";
            var geometry = new GeometryGroup();

            foreach (var path in document.Descendants(svg + "path"))
            {
                AppBarIconGeometryFactory.AddPathData(geometry, (string?)path.Attribute("d"));
            }

            return AppBarIconGeometryFactory.CompleteGeometry(geometry, resourceName);
        }

        /// <summary>Loads an embedded XML icon document.</summary>
        /// <param name="resourceName">The manifest resource name.</param>
        /// <returns>The loaded XML document.</returns>
        private XDocument LoadDocument(string resourceName)
        {
            using var stream =
                _assembly.GetManifestResourceStream(resourceName)
                ?? throw new InvalidOperationException(
                    $"The embedded icon resource '{resourceName}' could not be found.");

            return XDocument.Load(stream);
        }
    }
}
