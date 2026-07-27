// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Markup;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Markup;
#else
namespace CrissCross.WPF.UI.Markup;
#endif

/// <summary>Provides a dictionary implementation that contains controls resources used by components and other elements
/// of a WPF application.</summary>
/// <example>
/// <code lang="xml">
/// &lt;Application
///     xmlns:ui="https://github.com/reactivemarbles/CrissCross.ui"&gt;
///     &lt;Application.Resources&gt;
///         &lt;ResourceDictionary&gt;
///             &lt;ResourceDictionary.MergedDictionaries&gt;
///                 &lt;ui:ControlsDictionary /&gt;
///             &lt;/ResourceDictionary.MergedDictionaries&gt;
///         &lt;/ResourceDictionary&gt;
///     &lt;/Application.Resources&gt;
/// &lt;/Application&gt;
/// </code>
/// </example>
[Localizability(LocalizationCategory.Ignore)]
[Ambient]
[UsableDuringInitialization(true)]
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class ControlsDictionary : ResourceDictionary
{
    /// <summary>Provides the DictionaryUri member.</summary>
#if REACTIVE_SHIM
    private const string DictionaryUri =
        "pack://application:,,,/CrissCross.WPF.UI.Reactive;component/Resources/CrissCross.Ui.xaml";
#else
    private const string DictionaryUri =
        "pack://application:,,,/CrissCross.WPF.UI;component/Resources/CrissCross.Ui.xaml";
#endif

    /// <summary>Initializes a new instance of the <see cref="ControlsDictionary"/> class.</summary>
    public ControlsDictionary() => Source = new(DictionaryUri, UriKind.Absolute);

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;
}
