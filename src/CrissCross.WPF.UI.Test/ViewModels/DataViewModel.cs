// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
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
        var random = new Random();
        var colorCollection = new List<DataColor>();

        for (var i = 0; i < 8192; i++)
        {
            colorCollection.Add(
                new DataColor
                {
                    Color = new SolidColorBrush(
                        Color.FromArgb(
                            (byte)200,
                            (byte)random.Next(0, 250),
                            (byte)random.Next(0, 250),
                            (byte)random.Next(0, 250)))
                });
        }

        Colors = colorCollection;

        _isInitialized = true;
    }
}
