// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Versioning;
using ReactiveUI;

namespace CrissCross.WinForms.Test;

/// <summary>SecondForm member.</summary>
/// <seealso cref="Form" />
public partial class SecondForm : NavigationForm
{
    /// <summary>Initializes a new instance of the <see cref="SecondForm"/> class.</summary>
    [RequiresPreviewFeatures]
    public SecondForm()
    {
        InitializeComponent();
        _ = this.WhenSetup().Subscribe(_ =>
        {
            NavBack.Command = ReactiveCommand.Create(() => this.NavigateBack(), CanNavigateBack);
            this.NavigateToView<FirstViewModel>();
        });
    }
}
