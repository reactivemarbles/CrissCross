// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Windows;
using System.Windows.Markup;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF;
#else
namespace CrissCross.WPF;
#endif

/// <summary>CrissCross Dictionary.</summary>
/// <seealso cref="ResourceDictionary" />
[Localizability(LocalizationCategory.Ignore)]
[Ambient]
[UsableDuringInitialization(true)]
public class CrissCrossWpfDictionary : ResourceDictionary
{
    /// <summary>Defines the packaged generic resource dictionary URI.</summary>
#if REACTIVE_SHIM
    private const string DictionaryUri =
        "pack://application:,,,/CrissCross.WPF.Reactive;component/Themes/Generic.xaml";
#else
    private const string DictionaryUri = "pack://application:,,,/CrissCross.WPF;component/Themes/Generic.xaml";
#endif

    /// <summary>Initializes a new instance of the <see cref="CrissCrossWpfDictionary"/> class.</summary>
    public CrissCrossWpfDictionary() => Source = new(DictionaryUri, UriKind.Absolute);
}
