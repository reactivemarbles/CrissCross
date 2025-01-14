// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Windows;
using System.Windows.Markup;

namespace CrissCross.WPF;

/// <summary>
/// CrissCross Dictionary.
/// </summary>
/// <seealso cref="ResourceDictionary" />
[Localizability(LocalizationCategory.Ignore)]
[Ambient]
[UsableDuringInitialization(true)]
public class CrissCrossWpfDictionary : ResourceDictionary
{
    private const string DictionaryUri = "pack://application:,,,/CrissCross.WPF;component/Themes/Generic.xaml";

    /// <summary>
    /// Initializes a new instance of the <see cref="CrissCrossWpfDictionary"/> class.
    /// Default constructor defining <see cref="ResourceDictionary.Source"/> of the <c>CrissCross</c> Themes dictionary.
    /// </summary>
    public CrissCrossWpfDictionary() => Source = new Uri(DictionaryUri, UriKind.Absolute);
}
