// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>Contains dual-picker initialization behavior.</summary>
public partial class DualPickerControlBase
{
    /// <inheritdoc />
    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);
        _secondColorDecorator = new(this);
        _hintColorDecorator = new(this);
        SecondColor = new(_secondColorDecorator);
        SecondColor.PropertyChanged += (_, _) => UpdateSecondaryColor();
        HintNotifyableColor = new(_hintColorDecorator);
        HintNotifyableColor.PropertyChanged += (_, _) => UpdateHintColor();
    }

    /// <summary>Updates the secondary color.</summary>
    private void UpdateSecondaryColor()
    {
        if (_ignoreSecondaryColorChange)
        {
            return;
        }

        _ignoreSecondaryColorPropertyChange = true;
        SecondaryColor = System.Windows.Media.Color.FromArgb(
            (byte)Math.Round(SecondColor.A),
            (byte)Math.Round(SecondColor.RGB_R),
            (byte)Math.Round(SecondColor.RGB_G),
            (byte)Math.Round(SecondColor.RGB_B));
        _ignoreSecondaryColorPropertyChange = false;
    }

    /// <summary>Updates the hint color.</summary>
    private void UpdateHintColor()
    {
        if (_ignoreHintNotifyableColorChange)
        {
            return;
        }

        _ignoreHintColorPropertyChange = true;
        HintColor = System.Windows.Media.Color.FromArgb(
            (byte)Math.Round(HintNotifyableColor.A),
            (byte)Math.Round(HintNotifyableColor.RGB_R),
            (byte)Math.Round(HintNotifyableColor.RGB_G),
            (byte)Math.Round(HintNotifyableColor.RGB_B));
        _ignoreHintColorPropertyChange = false;
    }
}
