// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.UI.Controls;

namespace CrissCross.WPF.UI.Gallery.Tests;

/// <summary>Verifies mixed scripts preserve the initials character-set precedence.</summary>
public sealed class PersonPictureInitialsTests
{
    /// <summary>Verifies glyph and symbolic characters prevent unsafe truncation even when followed by Latin characters.</summary>
    /// <param name="displayName">The display name to project.</param>
    /// <param name="expectedInitials">The safe initials or null for the generic picture glyph.</param>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [Arguments("John Smith", "JS")]
    [Arguments("中A", null)]
    [Arguments("اA", null)]
    [Arguments("Aا", null)]
    public async Task DisplayName_WithMixedScripts_PreservesCharacterPrecedence(string displayName, string? expectedInitials)
    {
        var completion = new TaskCompletionSource<string?>(TaskCreationOptions.RunContinuationsAsynchronously);
        var thread = new Thread(() =>
        {
            try
            {
                using var picture = new PersonPicture { DisplayName = displayName };
                completion.SetResult(picture.TemplateSettings.ActualInitials);
            }
            catch (Exception exception)
            {
                completion.SetException(exception);
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        await Assert.That(await completion.Task).IsEqualTo(expectedInitials);
    }
}
