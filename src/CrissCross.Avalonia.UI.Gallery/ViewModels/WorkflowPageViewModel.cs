// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Avalonia.UI.Gallery.ViewModels;

/// <summary>View model for the workflow and feedback controls gallery page.</summary>
public sealed class WorkflowPageViewModel : RxObject
{
    /// <summary>Initializes a new instance of the <see cref="WorkflowPageViewModel"/> class.</summary>
    public WorkflowPageViewModel() => this.BuildComplete(() => DisplayName = "Workflow and feedback");
}
