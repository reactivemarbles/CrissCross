// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;

namespace CrissCross.Avalonia.UI.Gallery.ViewModels;

/// <summary>Reactive manual-QA view model for the BBCodeBlock control.</summary>
public sealed class BBCodeBlockPageViewModel : RxObject
{
    /// <summary>Initializes a new instance of the <see cref="BBCodeBlockPageViewModel"/> class.</summary>
    public BBCodeBlockPageViewModel()
    {
        DisplayName = "BBCodeBlock";
        LinkCommand = ReactiveCommand.Create<string>(HandleLink);
    }

    /// <summary>Gets the command invoked by <c>cmd:</c> links.</summary>
    public ReactiveCommand<string, Unit> LinkCommand { get; }

    /// <summary>Gets the most recently received command payload.</summary>
    public string LastCommand
    {
        get => field;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    } = "Select the in-document command link to verify command routing.";

    /// <summary>Records the command-link payload for visual verification.</summary>
    /// <param name="payload">The command payload.</param>
    private void HandleLink(string payload) => LastCommand = $"Command link invoked with payload: {payload}";
}
