// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Markup;

namespace CrissCross.WPF.UI.Markup;

/// <summary>Provides a dictionary implementation that contains controls resources used by components and other elements of a WPF application.</summary>
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
public class ControlsDictionary : ResourceDictionary
{
    /// <summary>Provides the DictionaryUri member.</summary>
    private const string DictionaryUri = "pack://application:,,,/CrissCross.WPF.UI;component/Resources/CrissCross.Ui.xaml";

    /// <summary>Initializes a new instance of the <see cref="ControlsDictionary"/> class. Default constructor defining <see cref="ResourceDictionary.Source"/> of the <c>CrissCross UI</c> controls dictionary.</summary>
    public ControlsDictionary() => Source = new(DictionaryUri, UriKind.Absolute);
}
