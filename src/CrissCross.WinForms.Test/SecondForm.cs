// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;

namespace CrissCross.WinForms.Test;

/// <summary>SecondForm member.</summary>
/// <seealso cref="Form" />
public partial class SecondForm : NavigationForm
{
    /// <summary>Initializes a new instance of the <see cref="SecondForm"/> class.</summary>
    public SecondForm()
    {
        InitializeComponent();
    }

    /// <inheritdoc />
    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        NavBack.Command = ReactiveCommand.Create(() => this.NavigateBack(), CanNavigateBack);
        this.NavigateToView(new NavigationKeyRequest<FirstViewModel>());
    }
}
