// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.NavigationModes.Example;

/// <summary>Provides a concrete view model used by each navigation flow.</summary>
/// <param name="source">Identifies how the view model was created.</param>
public sealed class CustomerPageViewModel(string source) : RxObject, ICustomerPageViewModel
{
    /// <summary>Gets how this view model was created.</summary>
    public string Source { get; } = source;

    /// <inheritdoc />
    public string PageKind => "customer";
}
