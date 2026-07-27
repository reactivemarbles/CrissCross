// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Avalonia.UI.Gallery.ViewModels;

/// <summary>Describes the concrete gallery page or application host that covers a control family.</summary>
/// <param name="Family">The public control family name.</param>
/// <param name="Example">The discoverable gallery page or host name.</param>
/// <param name="Notes">The topology or composition note for the example.</param>
public sealed record ControlCoverageItem(string Family, string Example, string Notes);
