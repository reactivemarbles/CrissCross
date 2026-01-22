// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Media;
using CrissCross.WPF.UI.Test.Models;
using ReactiveUI;

namespace CrissCross.WPF.UI.Test.ViewModels;

/// <summary>
/// DataViewModel.
/// </summary>
/// <seealso cref="RxObject" />
/// <seealso cref="INavigationAware" />
public class DataViewModel : RxObject, INavigationAware
{
    private bool _isInitialized;
    private IEnumerable<DataColor>? _colors;

    /// <summary>
    /// Gets or sets the colors.
    /// </summary>
    /// <value>
    /// The colors.
    /// </value>
    public IEnumerable<DataColor>? Colors
    {
        get => _colors;
        set => this.RaiseAndSetIfChanged(ref _colors, value);
    }

    /// <summary>
    /// Method triggered when the class is navigated.
    /// </summary>
    public void OnNavigatedTo()
    {
        if (!_isInitialized)
        {
            InitializeViewModel();
        }
    }

    /// <summary>
    /// Method triggered when the navigation leaves the current class.
    /// </summary>
    public void OnNavigatedFrom()
    {
    }

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

        for (var i = 0; i < 8192; i++)
        {
            colorCollection.Add(
                new DataColor
                {
                    Color = new SolidColorBrush(
                        Color.FromArgb(
                            (byte)200,
                            GetRandomByte(0, 250),
                            GetRandomByte(0, 250),
                            GetRandomByte(0, 250)))
                });
        }

        Colors = colorCollection;

        _isInitialized = true;
    }
}
