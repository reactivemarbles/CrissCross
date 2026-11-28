// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Interactivity;

/// <summary>Describes what may need to happen after a user action response has completed executing.</summary>
public readonly record struct ResponseInfo
{
    /// <summary>Gets a response indicating that no additional action is required.</summary>
    public static ResponseInfo NoActionRequired => new();

    /// <summary>Gets a response requesting a render after processing completes.</summary>
    public static ResponseInfo Refresh => new() { RefreshNeeded = true };

    /// <summary>Gets request a render after all responses have finished executing.</summary>
    public bool RefreshNeeded { get; init; }

    /// <summary>
    /// Gets if true, all other responses will not be executed until the response
    /// that returned this result returns a new result with this flag false.
    /// </summary>
    public bool IsPrimary { get; init; }

    /// <summary>Returns a description of the requested response actions.</summary>
    /// <returns>A string describing this response.</returns>
    public override string ToString()
    {
        string message = "PlotResponseResult";
        if (RefreshNeeded)
        {
            message += " [REFRESH]";
        }

        if (IsPrimary)
        {
            message += " [PRIMARY]";
        }

        return message;
    }
}
