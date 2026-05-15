// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross;

/// <summary>
/// Describes the editor surface used by a descriptor-driven data filter field.
/// </summary>
public enum FilterEditorKind
{
    /// <summary>
    /// A free text editor.
    /// </summary>
    Text,

    /// <summary>
    /// A numeric editor.
    /// </summary>
    Number,

    /// <summary>
    /// A boolean editor.
    /// </summary>
    Boolean,

    /// <summary>
    /// An explicit choice editor.
    /// </summary>
    Enum,

    /// <summary>
    /// A date-only editor.
    /// </summary>
    Date,

    /// <summary>
    /// A date and time editor.
    /// </summary>
    DateTime,

    /// <summary>
    /// A date and time range editor.
    /// </summary>
    DateRange,

    /// <summary>
    /// A consumer-provided editor template.
    /// </summary>
    Custom
}
