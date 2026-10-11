// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace CrissCross.Reactive.Maui.UI.Controls;
#else
namespace CrissCross.Maui.UI.Controls;
#endif

/// <summary>Represents an actionable card surface with optional trailing chevron semantics.</summary>
public class CardAction : Button
{
    /// <summary>Bindable property for <see cref="IsChevronVisible"/>.</summary>
    public static readonly BindableProperty IsChevronVisibleProperty = BindableProperty.Create(nameof(IsChevronVisible), typeof(bool), typeof(CardAction), true);

    /// <summary>Gets or sets a value indicating whether a template should show the trailing chevron.</summary>
    public bool IsChevronVisible
    {
        get => (bool)GetValue(IsChevronVisibleProperty);
        set => SetValue(IsChevronVisibleProperty, value);
    }
}
