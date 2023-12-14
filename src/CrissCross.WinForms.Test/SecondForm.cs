// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.Versioning;
using ReactiveUI;

namespace CrissCross.WinForms.Test;

/// <summary>
/// SecondForm.
/// </summary>
/// <seealso cref="Form" />
public partial class SecondForm : NavigationForm
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SecondForm"/> class.
    /// </summary>
    [RequiresPreviewFeatures]
    public SecondForm()
    {
        InitializeComponent();
        this.WhenSetup().Subscribe(_ =>
        {
            NavBack.Command = ReactiveCommand.Create(() => this.NavigateBack(), CanNavigateBack);
            this.NavigateToView<FirstViewModel>();
        });
    }
}
