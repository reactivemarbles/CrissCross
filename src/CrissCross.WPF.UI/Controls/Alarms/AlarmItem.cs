// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Represents one alarm item for the <see cref="Alarms"/> control.
/// </summary>
public sealed class AlarmItem
{
    /// <summary>
    /// Gets or sets a value indicating whether the alarm is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the alarm can be closed.
    /// </summary>
    public bool IsClosable { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the acknowledge action is visible.
    /// </summary>
    public bool IsAcknowledgeVisible { get; set; } = true;

    /// <summary>
    /// Gets or sets the alarm message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the alarm severity.
    /// </summary>
    public InfoBarSeverity Severity { get; set; } = InfoBarSeverity.Error;

    /// <summary>
    /// Gets or sets the acknowledge button label.
    /// </summary>
    public string AcknowledgeText { get; set; } = "Acknowledge";
}
