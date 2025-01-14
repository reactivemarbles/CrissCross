// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;

namespace CrissCross.WPF.UI;

/// <summary>
/// NotifyableColor.
/// </summary>
/// <seealso cref="RxObject" />
/// <remarks>
/// Initializes a new instance of the <see cref="NotifyableColor"/> class.
/// </remarks>
/// <param name="colorStateStorage">The color state storage.</param>
public class NotifyableColor(IColorStateStorage colorStateStorage) : RxObject
{
    /// <summary>
    /// Gets or sets a.
    /// </summary>
    /// <value>
    /// a.
    /// </value>
    public double A
    {
        get => colorStateStorage.ColorState.A * 255;
        set
        {
            var state = colorStateStorage.ColorState;
            state.A = value / 255;
            colorStateStorage.ColorState = state;
        }
    }

    /// <summary>
    /// Gets or sets the RGB r.
    /// </summary>
    /// <value>
    /// The RGB r.
    /// </value>
    public double RGB_R
    {
        get => colorStateStorage.ColorState.RGB_R * 255;
        set
        {
            var state = colorStateStorage.ColorState;
            state.RGB_R = value / 255;
            colorStateStorage.ColorState = state;
        }
    }

    /// <summary>
    /// Gets or sets the RGB g.
    /// </summary>
    /// <value>
    /// The RGB g.
    /// </value>
    public double RGB_G
    {
        get => colorStateStorage.ColorState.RGB_G * 255;
        set
        {
            var state = colorStateStorage.ColorState;
            state.RGB_G = value / 255;
            colorStateStorage.ColorState = state;
        }
    }

    /// <summary>
    /// Gets or sets the RGB b.
    /// </summary>
    /// <value>
    /// The RGB b.
    /// </value>
    public double RGB_B
    {
        get => colorStateStorage.ColorState.RGB_B * 255;
        set
        {
            var state = colorStateStorage.ColorState;
            state.RGB_B = value / 255;
            colorStateStorage.ColorState = state;
        }
    }

    /// <summary>
    /// Gets or sets the HSV h.
    /// </summary>
    /// <value>
    /// The HSV h.
    /// </value>
    public double HSV_H
    {
        get => colorStateStorage.ColorState.HSV_H;
        set
        {
            var state = colorStateStorage.ColorState;
            state.HSV_H = value;
            colorStateStorage.ColorState = state;
        }
    }

    /// <summary>
    /// Gets or sets the HSV s.
    /// </summary>
    /// <value>
    /// The HSV s.
    /// </value>
    public double HSV_S
    {
        get => colorStateStorage.ColorState.HSV_S * 100;
        set
        {
            var state = colorStateStorage.ColorState;
            state.HSV_S = value / 100;
            colorStateStorage.ColorState = state;
        }
    }

    /// <summary>
    /// Gets or sets the HSV v.
    /// </summary>
    /// <value>
    /// The HSV v.
    /// </value>
    public double HSV_V
    {
        get => colorStateStorage.ColorState.HSV_V * 100;
        set
        {
            var state = colorStateStorage.ColorState;
            state.HSV_V = value / 100;
            colorStateStorage.ColorState = state;
        }
    }

    /// <summary>
    /// Gets or sets the HSL h.
    /// </summary>
    /// <value>
    /// The HSL h.
    /// </value>
    public double HSL_H
    {
        get => colorStateStorage.ColorState.HSL_H;
        set
        {
            var state = colorStateStorage.ColorState;
            state.HSL_H = value;
            colorStateStorage.ColorState = state;
        }
    }

    /// <summary>
    /// Gets or sets the HSL s.
    /// </summary>
    /// <value>
    /// The HSL s.
    /// </value>
    public double HSL_S
    {
        get => colorStateStorage.ColorState.HSL_S * 100;
        set
        {
            var state = colorStateStorage.ColorState;
            state.HSL_S = value / 100;
            colorStateStorage.ColorState = state;
        }
    }

    /// <summary>
    /// Gets or sets the HSL l.
    /// </summary>
    /// <value>
    /// The HSL l.
    /// </value>
    public double HSL_L
    {
        get => colorStateStorage.ColorState.HSL_L * 100;
        set
        {
            var state = colorStateStorage.ColorState;
            state.HSL_L = value / 100;
            colorStateStorage.ColorState = state;
        }
    }

    /// <summary>
    /// Updates the everything.
    /// </summary>
    /// <param name="oldValue">The old value.</param>
    public void UpdateEverything(ColorState oldValue)
    {
        var currentValue = colorStateStorage.ColorState;
        if (currentValue.A != oldValue.A)
        {
            this.RaisePropertyChanged(nameof(A));
        }

        if (currentValue.RGB_R != oldValue.RGB_R)
        {
            this.RaisePropertyChanged(nameof(RGB_R));
        }

        if (currentValue.RGB_G != oldValue.RGB_G)
        {
            this.RaisePropertyChanged(nameof(RGB_G));
        }

        if (currentValue.RGB_B != oldValue.RGB_B)
        {
            this.RaisePropertyChanged(nameof(RGB_B));
        }

        if (currentValue.HSV_H != oldValue.HSV_H)
        {
            this.RaisePropertyChanged(nameof(HSV_H));
        }

        if (currentValue.HSV_S != oldValue.HSV_S)
        {
            this.RaisePropertyChanged(nameof(HSV_S));
        }

        if (currentValue.HSV_V != oldValue.HSV_V)
        {
            this.RaisePropertyChanged(nameof(HSV_V));
        }

        if (currentValue.HSL_H != oldValue.HSL_H)
        {
            this.RaisePropertyChanged(nameof(HSL_H));
        }

        if (currentValue.HSL_S != oldValue.HSL_S)
        {
            this.RaisePropertyChanged(nameof(HSL_S));
        }

        if (currentValue.HSL_L != oldValue.HSL_L)
        {
            this.RaisePropertyChanged(nameof(HSL_L));
        }
    }
}
