// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using ReactiveUI;

namespace CrissCross.Avalonia.UI.Gallery.ViewModels;

/// <summary>ViewModel for the buttons page.</summary>
public class ButtonsPageViewModel : RxObject
{
    /// <summary>Initializes a new instance of the <see cref="ButtonsPageViewModel"/> class.</summary>
    public ButtonsPageViewModel() =>
        this.BuildComplete(() =>
        {
            DisplayName = "Buttons";
            AppBarCommand = ReactiveCommand.Create<string>(action => LastAppBarAction = $"{action} selected");
        });

    /// <summary>Gets the command shared by the AppBarButton examples.</summary>
    public ICommand? AppBarCommand { get; private set; }

    /// <summary>Gets the most recent AppBarButton action.</summary>
    public string LastAppBarAction
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    } = "Choose an AppBarButton action.";
}
