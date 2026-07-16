// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Media;
using CrissCross.WPF.UI.Test.Models;
using ReactiveUI;

namespace CrissCross.WPF.UI.Test.ViewModels;

/// <summary>DataViewModel member.</summary>
/// <seealso cref="RxObject" />
/// <seealso cref="INavigationAware" />
public class DataViewModel : RxObject, INavigationAware
{
    /// <summary>The number of colors generated for the sample.</summary>
    private const int GeneratedColorCount = 8192;

    /// <summary>The alpha channel used by generated colors.</summary>
    private const byte GeneratedColorAlpha = 200;

    /// <summary>The exclusive upper bound for generated color channels.</summary>
    private const int GeneratedChannelUpperBound = 250;

    /// <summary>Tracks whether sample data has been generated.</summary>
    private bool _isInitialized;

    /// <summary>Gets or sets the colors.</summary>
    /// <value>
    /// The colors.
    /// </value>
    public IEnumerable<DataColor>? Colors
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Method triggered when the class is navigated.</summary>
    public void OnNavigatedTo()
    {
        if (_isInitialized)
        {
            return;
        }

        InitializeViewModel();
    }

    /// <summary>Method triggered when the navigation leaves the current class.</summary>
    public void OnNavigatedFrom() { }

    /// <summary>Generates the sample color collection.</summary>
    private void InitializeViewModel()
    {
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        var colorCollection = new List<DataColor>();

        byte GetRandomByte(int min, int max)
        {
            // Generate a random byte in [min, max)
            var buffer = new byte[4];
            rng.GetBytes(buffer);
            int value = BitConverter.ToInt32(buffer, 0) & int.MaxValue;
            return (byte)(min + (value % (max - min)));
        }

        for (var i = 0; i < GeneratedColorCount; i++)
        {
            colorCollection.Add(
                new DataColor
                {
                    Color = new SolidColorBrush(
                        Color.FromArgb(
                            GeneratedColorAlpha,
                            GetRandomByte(0, GeneratedChannelUpperBound),
                            GetRandomByte(0, GeneratedChannelUpperBound),
                            GetRandomByte(0, GeneratedChannelUpperBound))),
                });
        }

        Colors = colorCollection;

        _isInitialized = true;
    }
}
