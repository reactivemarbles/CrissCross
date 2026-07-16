// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Media.Animation;

namespace CrissCross.WPF.UI.Animations;

/// <summary>Provides tools for <see cref="FrameworkElement"/> animation.</summary>
/// <example>
/// <code lang="csharp">
/// TransitionAnimationProvider.ApplyTransition(MyFrameworkElement, Transition.FadeIn, 500);
/// </code>
/// </example>
public static class TransitionAnimationProvider
{
    /// <summary>Provides the default deceleration ratio.</summary>
    private const double DefaultDecelerationRatio = 0.7D;

    /// <summary>Provides the minimum supported transition duration in milliseconds.</summary>
    private const int MinimumDurationMilliseconds = 10;

    /// <summary>Provides the maximum supported transition duration in milliseconds.</summary>
    private const int MaximumDurationMilliseconds = 10_000;

    /// <summary>Provides the vertical slide distance in device-independent pixels.</summary>
    private const double VerticalSlideOffset = 30D;

    /// <summary>Provides the horizontal slide distance in device-independent pixels.</summary>
    private const double HorizontalSlideOffset = 50D;

    /// <summary>Provides the centered transform-origin coordinate.</summary>
    private const double TransformOriginCenter = 0.5D;

    /// <summary>Attempts to apply an animation effect while adding content to the frame.</summary>
    /// <param name="element">Currently rendered element.</param>
    /// <param name="type">Selected transition type.</param>
    /// <param name="duration">Transition duration.</param>
    /// <returns>Returns <see langword="true"/> if the transition was applied. Otherwise <see
    /// langword="false"/>.</returns>
    public static bool ApplyTransition(object element, Transition type, int duration)
    {
        if (type == Transition.None)
        {
            return false;
        }

        // Disable transitions for non-accelerated devices.
        if (!HardwareAcceleration.IsSupported(RenderingTier.PartialAcceleration))
        {
            return false;
        }

        if (element is not UIElement uiElement)
        {
            return false;
        }

        if (duration < MinimumDurationMilliseconds)
        {
            return false;
        }

        if (duration > MaximumDurationMilliseconds)
        {
            duration = MaximumDurationMilliseconds;
        }

        var timespanDuration = new Duration(TimeSpan.FromMilliseconds(duration));

        switch (type)
        {
            case Transition.FadeIn:
            {
                FadeInTransition(uiElement, timespanDuration);
                break;
            }

            case Transition.FadeInWithSlide:
            {
                FadeInWithSlideTransition(uiElement, timespanDuration);
                break;
            }

            case Transition.SlideBottom:
            {
                SlideBottomTransition(uiElement, timespanDuration);
                break;
            }

            case Transition.SlideRight:
            {
                SlideRightTransition(uiElement, timespanDuration);
                break;
            }

            case Transition.SlideLeft:
            {
                SlideLeftTransition(uiElement, timespanDuration);
                break;
            }

            default:
                return false;
        }

        return true;
    }

    /// <summary>Provides the FadeInTransition member.</summary>
    /// <param name="animatedUiElement">The animatedUiElement value.</param>
    /// <param name="duration">The duration value.</param>
    private static void FadeInTransition(UIElement animatedUiElement, Duration duration)
    {
        var opacityDoubleAnimation = new DoubleAnimation
        {
            Duration = duration,
            DecelerationRatio = DefaultDecelerationRatio,
            From = 0.0,
            To = 1.0,
        };

        animatedUiElement.BeginAnimation(UIElement.OpacityProperty, opacityDoubleAnimation);
    }

    /// <summary>Provides the FadeInWithSlideTransition member.</summary>
    /// <param name="animatedUiElement">The animatedUiElement value.</param>
    /// <param name="duration">The duration value.</param>
    private static void FadeInWithSlideTransition(UIElement animatedUiElement, Duration duration)
    {
        var translateDoubleAnimation = new DoubleAnimation
        {
            Duration = duration,
            DecelerationRatio = DefaultDecelerationRatio,
            From = VerticalSlideOffset,
            To = 0,
        };

        if (animatedUiElement.RenderTransform is not TranslateTransform)
        {
            animatedUiElement.SetCurrentValue(UIElement.RenderTransformProperty, new TranslateTransform(0, 0));
        }

        if (!animatedUiElement.RenderTransformOrigin.Equals(new Point(TransformOriginCenter, TransformOriginCenter)))
        {
            animatedUiElement.SetCurrentValue(
                UIElement.RenderTransformOriginProperty,
                new Point(TransformOriginCenter, TransformOriginCenter));
        }

        animatedUiElement.RenderTransform.BeginAnimation(TranslateTransform.YProperty, translateDoubleAnimation);

        var opacityDoubleAnimation = new DoubleAnimation
        {
            Duration = duration,
            DecelerationRatio = DefaultDecelerationRatio,
            From = 0.0,
            To = 1.0,
        };

        animatedUiElement.BeginAnimation(UIElement.OpacityProperty, opacityDoubleAnimation);
    }

    /// <summary>Provides the SlideBottomTransition member.</summary>
    /// <param name="animatedUiElement">The animatedUiElement value.</param>
    /// <param name="duration">The duration value.</param>
    private static void SlideBottomTransition(UIElement animatedUiElement, Duration duration)
    {
        var translateDoubleAnimation = new DoubleAnimation
        {
            Duration = duration,
            DecelerationRatio = DefaultDecelerationRatio,
            From = VerticalSlideOffset,
            To = 0,
        };

        if (animatedUiElement.RenderTransform is not TranslateTransform)
        {
            animatedUiElement.SetCurrentValue(UIElement.RenderTransformProperty, new TranslateTransform(0, 0));
        }

        if (!animatedUiElement.RenderTransformOrigin.Equals(new Point(TransformOriginCenter, TransformOriginCenter)))
        {
            animatedUiElement.SetCurrentValue(
                UIElement.RenderTransformOriginProperty,
                new Point(TransformOriginCenter, TransformOriginCenter));
        }

        animatedUiElement.RenderTransform.BeginAnimation(TranslateTransform.YProperty, translateDoubleAnimation);
    }

    /// <summary>Provides the SlideRightTransition member.</summary>
    /// <param name="animatedUiElement">The animatedUiElement value.</param>
    /// <param name="duration">The duration value.</param>
    private static void SlideRightTransition(UIElement animatedUiElement, Duration duration)
    {
        var translateDoubleAnimation = new DoubleAnimation
        {
            Duration = duration,
            DecelerationRatio = DefaultDecelerationRatio,
            From = HorizontalSlideOffset,
            To = 0,
        };

        if (animatedUiElement.RenderTransform is not TranslateTransform)
        {
            animatedUiElement.SetCurrentValue(UIElement.RenderTransformProperty, new TranslateTransform(0, 0));
        }

        if (!animatedUiElement.RenderTransformOrigin.Equals(new Point(TransformOriginCenter, TransformOriginCenter)))
        {
            animatedUiElement.SetCurrentValue(
                UIElement.RenderTransformOriginProperty,
                new Point(TransformOriginCenter, TransformOriginCenter));
        }

        animatedUiElement.RenderTransform.BeginAnimation(TranslateTransform.XProperty, translateDoubleAnimation);
    }

    /// <summary>Provides the SlideLeftTransition member.</summary>
    /// <param name="animatedUiElement">The animatedUiElement value.</param>
    /// <param name="duration">The duration value.</param>
    private static void SlideLeftTransition(UIElement animatedUiElement, Duration duration)
    {
        var translateDoubleAnimation = new DoubleAnimation
        {
            Duration = duration,
            DecelerationRatio = DefaultDecelerationRatio,
            From = -HorizontalSlideOffset,
            To = 0,
        };

        if (animatedUiElement.RenderTransform is not TranslateTransform)
        {
            animatedUiElement.SetCurrentValue(UIElement.RenderTransformProperty, new TranslateTransform(0, 0));
        }

        if (!animatedUiElement.RenderTransformOrigin.Equals(new Point(TransformOriginCenter, TransformOriginCenter)))
        {
            animatedUiElement.SetCurrentValue(
                UIElement.RenderTransformOriginProperty,
                new Point(TransformOriginCenter, TransformOriginCenter));
        }

        animatedUiElement.RenderTransform.BeginAnimation(TranslateTransform.XProperty, translateDoubleAnimation);
    }
}
