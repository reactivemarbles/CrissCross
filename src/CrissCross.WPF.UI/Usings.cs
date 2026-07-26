// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

global using System;
global using System.Collections.Generic;
global using System.ComponentModel;
global using System.Diagnostics;
global using System.Globalization;
global using System.IO;
global using System.Linq;
global using System.Linq.Expressions;
global using System.Net;
global using System.Reflection;
global using System.Runtime.CompilerServices;
global using System.Text;
global using System.Threading;
global using System.Threading.Tasks;
global using System.Windows;
global using System.Windows.Interop;
global using System.Windows.Media;
#if REACTIVELIST_REACTIVE
global using CrissCross.Reactive.WPF.UI.Appearance;
global using CrissCross.Reactive.WPF.UI.Configuration;
global using CrissCross.Reactive.WPF.UI.Configuration.Attributes;
global using CrissCross.Reactive.WPF.UI.Controls;
global using CrissCross.Reactive.WPF.UI.Hardware;
global using CrissCross.Reactive.WPF.UI.Interop;
global using CrissCross.Reactive.WPF.UI.Storage;
global using CrissCross.Reactive.WPF.UI.TaskBar;
#else
global using CrissCross.WPF.UI.Appearance;
global using CrissCross.WPF.UI.Configuration;
global using CrissCross.WPF.UI.Configuration.Attributes;
global using CrissCross.WPF.UI.Controls;
global using CrissCross.WPF.UI.Hardware;
global using CrissCross.WPF.UI.Interop;
global using CrissCross.WPF.UI.Storage;
global using CrissCross.WPF.UI.TaskBar;
#endif

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>Anchors the global imports in a source type.</summary>
internal static class Usings
{
    /// <summary>Anchors the source file in the compilation.</summary>
    internal static void Anchor() => GC.KeepAlive(typeof(Usings));
}
