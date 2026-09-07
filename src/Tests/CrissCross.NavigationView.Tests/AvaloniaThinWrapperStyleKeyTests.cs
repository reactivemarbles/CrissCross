// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Primitives;
using CrissCrossContextMenu = CrissCross.Avalonia.UI.Controls.ContextMenu;
using CrissCrossDynamicScrollBar = CrissCross.Avalonia.UI.Controls.DynamicScrollBar;
using CrissCrossDynamicScrollViewer = CrissCross.Avalonia.UI.Controls.DynamicScrollViewer;
using CrissCrossExpander = CrissCross.Avalonia.UI.Controls.Expander;
using CrissCrossGifImage = CrissCross.Avalonia.UI.Controls.GifImage;
using CrissCrossIconElement = CrissCross.Avalonia.UI.Controls.IconElement;
using CrissCrossImage = CrissCross.Avalonia.UI.Controls.Image;
using CrissCrossItemsControl = CrissCross.Avalonia.UI.Controls.ItemsControl;
using CrissCrossLabel = CrissCross.Avalonia.UI.Controls.Label;
using CrissCrossListBox = CrissCross.Avalonia.UI.Controls.ListBox;
using CrissCrossListView = CrissCross.Avalonia.UI.Controls.ListView;
using CrissCrossMenu = CrissCross.Avalonia.UI.Controls.Menu;
using CrissCrossMenuItem = CrissCross.Avalonia.UI.Controls.MenuItem;
using CrissCrossScrollBar = CrissCross.Avalonia.UI.Controls.ScrollBar;
using CrissCrossScrollViewer = CrissCross.Avalonia.UI.Controls.ScrollViewer;
using CrissCrossSeparator = CrissCross.Avalonia.UI.Controls.Separator;
using CrissCrossTabControl = CrissCross.Avalonia.UI.Controls.TabControl;
using CrissCrossTextBlock = CrissCross.Avalonia.UI.Controls.TextBlock;
using CrissCrossTreeView = CrissCross.Avalonia.UI.Controls.TreeView;
using CrissCrossVirtualizingGridView = CrissCross.Avalonia.UI.Controls.VirtualizingGridView;
using CrissCrossVirtualizingItemsControl = CrissCross.Avalonia.UI.Controls.VirtualizingItemsControl;
using CrissCrossWindow = CrissCross.Avalonia.UI.Controls.Window;
using NativeScrollBar = Avalonia.Controls.Primitives.ScrollBar;

namespace CrissCross.NavigationView.Tests;

/// <summary>Verifies thin Avalonia UI wrappers keep their native control themes.</summary>
[TUnit.Core.Executors.TestExecutor<AvaloniaUiTestExecutor>]
public sealed class AvaloniaThinWrapperStyleKeyTests
{
    /// <summary>Provides the Avalonia UI project directory name.</summary>
    private const string AvaloniaUiProject = "CrissCross.Avalonia.UI";

    /// <summary>Provides the themes directory name.</summary>
    private const string ThemesDirectory = "Themes";

    /// <summary>Provides the CheckBox theme filename.</summary>
    private const string CheckBoxThemeFile = "CheckBox.axaml";

    /// <summary>Provides the Button theme filename.</summary>
    private const string ButtonThemeFile = "Button.axaml";

    /// <summary>Provides the RadioButton theme filename.</summary>
    private const string RadioButtonThemeFile = "RadioButton.axaml";

    /// <summary>Provides the semantic primary foreground brush key.</summary>
    private const string PrimaryForegroundBinding = "TextFillColorPrimaryBrush";

    /// <summary>Provides the disabled foreground brush key.</summary>
    private const string DisabledForegroundBinding = "TextFillColorDisabledBrush";

    /// <summary>Provides the button disabled foreground brush key.</summary>
    private const string ButtonDisabledForegroundBinding = "ButtonForegroundDisabled";

    /// <summary>Provides the attached text foreground binding used by generated string content.</summary>
    private const string ContentPresenterForegroundBinding = "TextElement.Foreground=\"{TemplateBinding Foreground}\"";

    /// <summary>Provides the checked radio indicator brush binding.</summary>
    private const string RadioButtonCheckedIndicatorBinding = "RadioButtonOuterEllipseCheckedStroke";

    /// <summary>Provides the transparent control fill brush binding.</summary>
    private const string TransparentControlFillBinding = "ControlAltFillColorTransparentBrush";

    /// <summary>Provides the disabled control stroke brush binding.</summary>
    private const string DisabledControlStrokeBinding = "ControlStrongStrokeColorDisabledBrush";

    /// <summary>Provides the obsolete checked radio fill key that is not in the theme dictionaries.</summary>
    private const string UndefinedRadioButtonCheckedFillBinding = "RadioButtonOuterEllipseFillChecked";

    /// <summary>Provides the obsolete checked radio stroke key that is not in the theme dictionaries.</summary>
    private const string UndefinedRadioButtonCheckedStrokeBinding = "RadioButtonOuterEllipseStrokeChecked";

    /// <summary>Provides the obsolete pointer-over radio stroke key that is not in the theme dictionaries.</summary>
    private const string UndefinedRadioButtonPointerOverStrokeBinding = "RadioButtonOuterEllipseStrokePointerOver";

    /// <summary>Provides the legacy theme foreground brush key that does not exist in the Avalonia semantic palette.</summary>
    private const string LegacyThemeForegroundBinding = "ThemeForegroundBrush";

    /// <summary>Provides the source root used to locate Avalonia UI theme files.</summary>
    private static readonly string SourceRoot = LocateSourceRoot();

    /// <summary>Verifies the Expander retains native header/content rendering and its own style key.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Expander_WhenHeaderAndContentAreSet_RendersTheNativeTemplate()
    {
        var content = new Button { Content = "Button" };
        var expander = new CrissCrossExpander { Header = "Basic Controls", Content = content, IsExpanded = true };

        var window = new Window { Content = expander };
        try
        {
            window.Show();
            window.UpdateLayout();
            await Assert.That(expander.Header).IsEqualTo("Basic Controls");
            await Assert.That(expander.Content).IsEqualTo(content);
            await Assert.That(expander.StyleKey).IsEqualTo(typeof(CrissCrossExpander));
            await Assert.That(expander.Theme?.TargetType).IsEqualTo(typeof(Expander));
            await Assert.That(expander.Template).IsNotNull();
            await Assert.That(content.IsEffectivelyVisible).IsTrue();
            await Assert.That(TopLevel.GetTopLevel(content)).IsSameReferenceAs(window);
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>Verifies wrappers keep their custom style keys and reuse native control themes.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public Task ThinWrappers_WhenAttached_ReuseNativeTemplatesWithCustomStyleKeys() => AvaloniaTestUiThread.RunAsync(static async () =>
    {
        var cases = new[]
        {
            new StyleKeyCase(typeof(CrissCrossContextMenu), typeof(ContextMenu)),
            new StyleKeyCase(typeof(CrissCrossDynamicScrollBar), typeof(NativeScrollBar)),
            new StyleKeyCase(typeof(CrissCrossDynamicScrollViewer), typeof(ScrollViewer)),
            new StyleKeyCase(typeof(CrissCrossGifImage), typeof(Image)),
            new StyleKeyCase(typeof(CrissCrossImage), typeof(Image)),
            new StyleKeyCase(typeof(CrissCrossItemsControl), typeof(ItemsControl)),
            new StyleKeyCase(typeof(CrissCrossLabel), typeof(TextBlock)),
            new StyleKeyCase(typeof(CrissCrossListBox), typeof(ListBox)),
            new StyleKeyCase(typeof(CrissCrossListView), typeof(ListBox)),
            new StyleKeyCase(typeof(CrissCrossMenu), typeof(Menu)),
            new StyleKeyCase(typeof(CrissCrossMenuItem), typeof(MenuItem)),
            new StyleKeyCase(typeof(CrissCrossScrollBar), typeof(NativeScrollBar)),
            new StyleKeyCase(typeof(CrissCrossScrollViewer), typeof(ScrollViewer)),
            new StyleKeyCase(typeof(CrissCrossSeparator), typeof(Separator)),
            new StyleKeyCase(typeof(CrissCrossTabControl), typeof(TabControl)),
            new StyleKeyCase(typeof(CrissCrossTextBlock), typeof(TextBlock)),
            new StyleKeyCase(typeof(CrissCrossTreeView), typeof(TreeView)),
            new StyleKeyCase(typeof(CrissCrossVirtualizingGridView), typeof(ListBox)),
            new StyleKeyCase(typeof(CrissCrossVirtualizingItemsControl), typeof(ItemsControl)),
            new StyleKeyCase(typeof(CrissCrossWindow), typeof(Window)),
        };

        foreach (var testCase in cases)
        {
            var wrapper = (Control)Activator.CreateInstance(testCase.WrapperType)!;
            var window = wrapper as Window ?? new Window { Content = wrapper };
            try
            {
                window.Show();
                window.UpdateLayout();
                await Assert.That(wrapper.StyleKey).IsEqualTo(testCase.WrapperType);
                if (wrapper is TemplatedControl templated)
                {
                    await Assert.That(templated.Theme?.TargetType).IsEqualTo(testCase.NativeThemeTarget);
                    await Assert.That(templated.Template).IsNotNull();
                }
            }
            finally
            {
                window.Close();
            }
        }
    });

    /// <summary>Verifies CheckBox and RadioButton content text binds to semantic foreground resources.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task SelectionControlThemes_WhenStringContentIsUsed_BindContentPresenterForegroundToSemanticTextBrushes()
    {
        var checkBoxTheme = await File.ReadAllTextAsync(GetAvaloniaThemePath(CheckBoxThemeFile));
        var radioButtonTheme = await File.ReadAllTextAsync(GetAvaloniaThemePath(RadioButtonThemeFile));

        await Assert.That(checkBoxTheme).Contains(PrimaryForegroundBinding);
        await Assert.That(checkBoxTheme).Contains(ContentPresenterForegroundBinding);
        await Assert.That(checkBoxTheme).Contains(DisabledForegroundBinding);
        await Assert.That(checkBoxTheme).DoesNotContain(LegacyThemeForegroundBinding);

        await Assert.That(radioButtonTheme).Contains(PrimaryForegroundBinding);
        await Assert.That(radioButtonTheme).Contains(ContentPresenterForegroundBinding);
        await Assert.That(radioButtonTheme).Contains(DisabledForegroundBinding);
    }

    /// <summary>Verifies RadioButton checked and disabled indicators use defined semantic brushes.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RadioButtonTheme_WhenCheckedOrDisabled_UsesDefinedVisibleIndicatorBrushes()
    {
        var radioButtonTheme = await File.ReadAllTextAsync(GetAvaloniaThemePath(RadioButtonThemeFile));

        await Assert.That(radioButtonTheme).Contains(RadioButtonCheckedIndicatorBinding);
        await Assert.That(radioButtonTheme).Contains(TransparentControlFillBinding);
        await Assert.That(radioButtonTheme).Contains(DisabledControlStrokeBinding);
        await Assert.That(radioButtonTheme).DoesNotContain(UndefinedRadioButtonCheckedFillBinding);
        await Assert.That(radioButtonTheme).DoesNotContain(UndefinedRadioButtonCheckedStrokeBinding);
        await Assert.That(radioButtonTheme).DoesNotContain(UndefinedRadioButtonPointerOverStrokeBinding);
    }

    /// <summary>Verifies icon elements inherit text foreground for button-hosted icons.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task IconElementForeground_WhenHostedInButton_UsesInheritedTextForeground()
    {
        var buttonTheme = await File.ReadAllTextAsync(GetAvaloniaThemePath(ButtonThemeFile));

        await Assert.That(CrissCrossIconElement.ForegroundProperty.Name).IsEqualTo(nameof(TextElement.Foreground));
        await Assert.That(CrissCrossIconElement.ForegroundProperty.Inherits).IsTrue();
        await Assert.That(buttonTheme).Contains(ContentPresenterForegroundBinding);
        await Assert.That(buttonTheme).Contains(ButtonDisabledForegroundBinding);
    }

    /// <summary>Gets the path to an Avalonia theme file.</summary>
    /// <param name="themeFileName">The theme file name.</param>
    /// <returns>The full theme path.</returns>
    private static string GetAvaloniaThemePath(string themeFileName) =>
        Path.Combine(SourceRoot, AvaloniaUiProject, ThemesDirectory, themeFileName);

    /// <summary>Locates the repository source root.</summary>
    /// <returns>The source root path.</returns>
    private static string LocateSourceRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, AvaloniaUiProject);
            if (Directory.Exists(candidate))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException($"Could not locate {AvaloniaUiProject} from {AppContext.BaseDirectory}.");
    }

    /// <summary>Provides a wrapper type and its expected native Avalonia theme target.</summary>
    /// <param name="WrapperType">The CrissCross wrapper type.</param>
    /// <param name="NativeThemeTarget">The native control theme target type.</param>
    private sealed record StyleKeyCase(Type WrapperType, Type NativeThemeTarget);
}
