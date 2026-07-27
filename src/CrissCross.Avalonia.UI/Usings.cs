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
global using ReactiveUI;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI;
#else
namespace CrissCross.Avalonia.UI;
#endif

/// <summary>Anchors the project's shared global imports in a documented source type.</summary>
internal static class Usings
{
    /// <summary>The package family that owns these shared imports.</summary>
    internal const string PackageFamily = "CrissCross.Avalonia.UI";
}
