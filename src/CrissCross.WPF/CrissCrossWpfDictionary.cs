// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Windows;
using System.Windows.Markup;

namespace CrissCross.WPF
{
    /// <summary>
    /// CrissCross Dictionary.
    /// </summary>
    /// <seealso cref="System.Windows.ResourceDictionary" />
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
}
