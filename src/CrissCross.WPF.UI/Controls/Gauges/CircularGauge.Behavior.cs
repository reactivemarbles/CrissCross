// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Path = System.Windows.Shapes.Path;

namespace CrissCross.WPF.UI.Controls;

/// <summary>Contains CircularGauge template behavior and value handling.</summary>
public sealed partial class CircularGauge
{
    /// <summary>Load the visualization template.</summary>
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        // Get reference to known elements on the control template
        _rootGrid = GetTemplateChild("LayoutRoot") as Grid;
        _pointer = GetTemplateChild(Pointer) as Path;
        _pointerCap = GetTemplateChild("PointerCap") as Ellipse;
        _lightIndicator = GetTemplateChild("RangeIndicatorLight") as Ellipse;

        // Draw scale and range indicator
        DrawScale();
        DrawRangeIndicator();

        // Set Z index of pointer and pointer cap to a really high number so that it stays on top
        // of the scale and the range indicator
        Panel.SetZIndex(_pointer, PointerZIndex);
        Panel.SetZIndex(_pointerCap, PointerCapZIndex);

        // Reset Pointer
        if (!ResetPointerOnStartUp)
        {
            return;
        }

        MovePointer(ScaleStartAngle);
    }

    /// <summary>Raises the <see cref="E:ValueChanged"/> event.</summary>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    public void OnValueChanged(DependencyPropertyChangedEventArgs e)
    {
        // Validate and set the new value
        var newValue = (double)e.NewValue;

        if (newValue > MaxValue)
        {
            newValue = MaxValue;
        }
        else if (newValue < MinValue)
        {
            newValue = MinValue;
        }

        if (_pointer is null)
        {
            return;
        }

        var range = MaxValue - MinValue;
        var proccessValue = (newValue - MinValue) / range;
        var newcurrRealworldunit = proccessValue * ScaleSweepAngle;

        if (newcurrRealworldunit.Equals(double.NaN))
        {
            newcurrRealworldunit = 0;
        }

        // Animate the pointer from the old value to the new value
        AnimatePointer(ScaleStartAngle + _oldcurrRealworldunit, ScaleStartAngle + newcurrRealworldunit);
        _oldcurrRealworldunit = newcurrRealworldunit;
    }

    /// <summary>Refreshes the dial range.</summary>
    public void RefreshDialRange()
    {
        try
        {
            foreach (var item in _ht.Values)
            {
                if (item is TextBlock textBlock)
                {
                    _rootGrid?.Children.Remove(textBlock);
                }

                if (item is Rectangle element)
                {
                    _rootGrid?.Children.Remove(element);
                }
            }

            _ht.Clear();
            DrawScale();
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
        }
    }

    /// <summary>Refreshes the range indicator.</summary>
    public void RefreshRangeIndicator()
    {
        try
        {
            if (_rangeIndicator1 is not null && _rootGrid is not null)
            {
                _rootGrid.Children.Remove(_rangeIndicator1);
            }

            if (_rangeIndicator2 is not null && _rootGrid is not null)
            {
                _rootGrid.Children.Remove(_rangeIndicator2);
            }

            if (_rangeIndicator3 is not null && _rootGrid is not null)
            {
                _rootGrid.Children.Remove(_rangeIndicator3);
            }

            DrawRangeIndicator();
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
        }
    }

    /// <summary>Handles major value changes.</summary>
    /// <param name="d">The dependency object.</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    private static void AMajorValueHasChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        _ = e;
        if (d is not CircularGauge gauge)
        {
            return;
        }

        gauge.RefreshDialRange();
        gauge.RefreshRangeIndicator();
        lock (gauge._lockObject)
        {
            var g = gauge.Value;
            gauge.AnimatePointer(gauge.ScaleStartAngle + gauge._oldcurrRealworldunit, gauge.ScaleStartAngle);
            gauge._oldcurrRealworldunit = gauge.ScaleStartAngle;

            gauge.Value = gauge.MinValue;
            gauge.Value = g;
        }
    }

    /// <summary>Provides the GetRangeIndicatorGradEffect member.</summary>
    /// <param name="gradientColor">The gradientColor value.</param>
    /// <returns>The result.</returns>
    private static LinearGradientBrush GetRangeIndicatorGradEffect(Brush gradientColor)
    {
        var gradient = new LinearGradientBrush { StartPoint = new(0, 0), EndPoint = new(1, 1) };

        var color1 = new GradientStop
        {
            Offset = RangeGradientStartOffset,
            Color = Equals(gradientColor, Brushes.Transparent) ? Colors.Transparent : Colors.LightGray,
        };
        gradient.GradientStops.Add(color1);
        gradient.GradientStops.Add(
            new GradientStop { Color = ((SolidColorBrush)gradientColor).Color, Offset = RangeGradientMiddleOffset });
        gradient.GradientStops.Add(
            new GradientStop { Color = ((SolidColorBrush)gradientColor).Color, Offset = RangeGradientEndOffset });
        return gradient;
    }

    /// <summary>Provides the OnDetectValueOrErrorPropertyChanged member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static async void OnDetectValueOrErrorPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var gauge = d as CircularGauge;
        if ((bool)e.NewValue)
        {
            // Start checking
            await gauge!.DetectectingThread();
        }
        else
        {
            // Stop checking
            gauge?._detectValuesChanging = false;
        }
    }

    /// <summary>Provides the OnOptimalRangeEndValuePropertyChanged member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void OnOptimalRangeEndValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // Get access to the instance of CircularGaugeConrol whose property value changed
        if (d is not CircularGauge gauge)
        {
            return;
        }

        if ((double)e.NewValue > gauge.MaxValue)
        {
            gauge.OptimalRangeEndValue = gauge.MaxValue;
        }

        if ((double)e.NewValue < gauge.OptimalRangeStartValue)
        {
            gauge.OptimalRangeEndValue = Math.Min(gauge.OptimalRangeStartValue, gauge.MaxValue);
        }

        gauge.RefreshRangeIndicator();
    }

    /// <summary>Provides the OnOptimalRangeStartValuePropertyChanged member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void OnOptimalRangeStartValuePropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        // Get access to the instance of CircularGaugeConrol whose property value changed
        if (d is not CircularGauge gauge)
        {
            return;
        }

        if ((double)e.NewValue < gauge.MinValue)
        {
            gauge.OptimalRangeStartValue = gauge.MinValue;
        }
        else if ((double)e.NewValue > gauge.OptimalRangeEndValue)
        {
            gauge.OptimalRangeStartValue = gauge.OptimalRangeEndValue;
        }
        else
        {
            gauge.OptimalRangeStartValue = (double)e.NewValue;
        }

        gauge.RefreshRangeIndicator();
    }

    /// <summary>Provides the OnValuePropertyChanged member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // Get access to the instance of CircularGauge whose property value changed
        if (d is not CircularGauge gauge)
        {
            return;
        }

        lock (gauge._lockObject)
        {
            gauge._valueChanged = true;
            gauge.OnValueChanged(e);
        }
    }

    /// <summary>Provides the ScalePropertyChanged member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void ScalePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        _ = e;
        if (d is not CircularGauge gauge)
        {
            return;
        }

        gauge.RefreshDialRange();
    }

    /// <summary>Provides the AnimatePointer member.</summary>
    /// <param name="oldValueAngle">The oldValueAngle value.</param>
    /// <param name="newValueAngle">The newValueAngle value.</param>
    private void AnimatePointer(double oldValueAngle, double newValueAngle)
    {
        if (_pointer is null || DoubleComparison.AreClose(newValueAngle, oldValueAngle))
        {
            return;
        }

        var da = new DoubleAnimation
        {
            From = oldValueAngle,
            To = newValueAngle,
            Duration = new(TimeSpan.FromMilliseconds(Math.Abs(oldValueAngle - newValueAngle) * AnimatingSpeedFactor)),
        };

        var sb = new Storyboard();
        sb.Completed += Sb_Completed;
        sb.Children.Add(da);
        Storyboard.SetTarget(da, _pointer);
        Storyboard.SetTargetProperty(
            da,
            new PropertyPath("(Path.RenderTransform).(TransformGroup.Children)[0].(RotateTransform.Angle)"));
        sb.Begin();
    }

    /// <summary>Provides the DetectectingThread member.</summary>
    /// <returns>The result.</returns>
    private async Task DetectectingThread()
    {
        _detectValuesChanging = true;
        var count = 0;
        while (_detectValuesChanging)
        {
            if (_valueChanged)
            {
                count = 0;
                _valueChanged = false;
                ShowError = Visibility.Collapsed;
            }
            else
            {
                count++;
                if (count >= DetectionTimeOut * DetectionPollsPerTimeoutUnit)
                {
                    count = 0;
                    ShowError = Visibility.Visible;
                }
            }

            await Task.Delay(DetectionPollIntervalMilliseconds).ConfigureAwait(false);
        }
    }
}
