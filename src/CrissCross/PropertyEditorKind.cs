// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross;

/// <summary>
/// Describes an explicit editor surface for descriptor-driven property inspectors.
/// </summary>
public enum PropertyEditorKind
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
    /// A color editor.
    /// </summary>
    Color,

    /// <summary>
    /// A date-only editor.
    /// </summary>
    Date,

    /// <summary>
    /// A date and time editor.
    /// </summary>
    DateTime,

    /// <summary>
    /// A command/action editor.
    /// </summary>
    Command,

    /// <summary>
    /// A consumer-provided editor template.
    /// </summary>
    Custom
}
