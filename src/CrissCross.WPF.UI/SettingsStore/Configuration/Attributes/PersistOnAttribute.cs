// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Configuration.Attributes;
#else
namespace CrissCross.WPF.UI.Configuration.Attributes;
#endif

/// <summary>Represents PersistOnAttribute.</summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Event)]
public sealed class PersistOnAttribute : Attribute;
