// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Markup;

#if REACTIVELIST_REACTIVE
[assembly: XmlnsDefinition("https://github.com/reactivemarbles/CrissCross.ui", "CrissCross.Reactive.WPF.Plot")]
#else
[assembly: XmlnsDefinition("https://github.com/reactivemarbles/CrissCross.ui", "CrissCross.WPF.Plot")]
#endif
[assembly: XmlnsPrefix("https://github.com/reactivemarbles/CrissCross.ui", "ccui")]
[assembly: ThemeInfo(ResourceDictionaryLocation.SourceAssembly, ResourceDictionaryLocation.SourceAssembly)]
