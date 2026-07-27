// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;

namespace CrissCross.WPF.UI.Gallery.ViewModels;

/// <summary>Provides the reactive state for the curated WPF control catalog.</summary>
public class ControlCatalogViewModel : RxObject
{
    /// <summary>Initializes a new instance of the <see cref="ControlCatalogViewModel"/> class.</summary>
    public ControlCatalogViewModel()
    {
        RefreshCommand = ReactiveCommand.Create(Refresh);
        Refresh();
    }

    /// <summary>Gets the command that refreshes the deterministic catalog status.</summary>
    public ReactiveCommand<Unit, Unit> RefreshCommand { get; }

    /// <summary>Gets the status shown by the catalog's interactive command example.</summary>
    public string StatusText
    {
        get => field;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    /// <summary>Refreshes the deterministic catalog status.</summary>
    private void Refresh() => StatusText = "Catalog controls are ready for interaction.";
}
