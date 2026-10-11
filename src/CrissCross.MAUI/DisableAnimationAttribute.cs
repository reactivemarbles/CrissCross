// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace CrissCross.Reactive.MAUI;
#else
namespace CrissCross.MAUI;
#endif

/// <summary>Disables animation when a page is navigated by <see cref="NavigationShell"/>.</summary>
[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public sealed class DisableAnimationAttribute : Attribute;
