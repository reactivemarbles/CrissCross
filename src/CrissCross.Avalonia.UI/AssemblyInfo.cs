// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using Avalonia.Metadata;

[assembly: InternalsVisibleTo("CrissCross.NavigationView.Tests")]
#if REACTIVELIST_REACTIVE
[assembly: XmlnsDefinition(
    "https://github.com/reactivemarbles/CrissCross.Avalonia.UI",
    "CrissCross.Reactive.Avalonia.UI")]
[assembly: XmlnsDefinition(
    "https://github.com/reactivemarbles/CrissCross.Avalonia.UI",
    "CrissCross.Reactive.Avalonia.UI.Controls")]
[assembly: XmlnsDefinition(
    "https://github.com/reactivemarbles/CrissCross.Avalonia.UI",
    "CrissCross.Reactive.Avalonia.UI.Appearance")]
#else
[assembly: XmlnsDefinition("https://github.com/reactivemarbles/CrissCross.Avalonia.UI", "CrissCross.Avalonia.UI")]
[assembly: XmlnsDefinition(
    "https://github.com/reactivemarbles/CrissCross.Avalonia.UI",
    "CrissCross.Avalonia.UI.Controls")]
[assembly: XmlnsDefinition(
    "https://github.com/reactivemarbles/CrissCross.Avalonia.UI",
    "CrissCross.Avalonia.UI.Appearance")]
#endif
