// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross;

/// <summary>
/// Describes the comparison operator represented by a search or filter token.
/// </summary>
public enum FilterOperator
{
    /// <summary>
    /// The field value equals the token value.
    /// </summary>
    Equals,

    /// <summary>
    /// The field value does not equal the token value.
    /// </summary>
    NotEquals,

    /// <summary>
    /// The field value contains the token value.
    /// </summary>
    Contains,

    /// <summary>
    /// The field value starts with the token value.
    /// </summary>
    StartsWith,

    /// <summary>
    /// The field value ends with the token value.
    /// </summary>
    EndsWith,

    /// <summary>
    /// The field value is greater than the token value.
    /// </summary>
    GreaterThan,

    /// <summary>
    /// The field value is greater than or equal to the token value.
    /// </summary>
    GreaterThanOrEqual,

    /// <summary>
    /// The field value is less than the token value.
    /// </summary>
    LessThan,

    /// <summary>
    /// The field value is less than or equal to the token value.
    /// </summary>
    LessThanOrEqual,

    /// <summary>
    /// The field value falls between the token value range.
    /// </summary>
    Between
}
