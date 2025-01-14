// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Markup;

/// <summary>
/// Collection of theme resources.
/// </summary>
/// <example>
/// <code lang="xml">
/// &lt;ui:TextBox Foreground={ui:ThemeResource TextFillColorSecondaryBrush} /&gt;
/// </code>
/// </example>
public enum ThemeResource
{
    /// <summary>
    /// Unspecified theme resource.
    /// </summary>
    Unknown,

    /// <summary>
    /// The system accent color.
    /// </summary>
    SystemAccentColor,

    /// <summary>
    /// The system accent color primary.
    /// </summary>
    SystemAccentColorPrimary,

    /// <summary>
    /// The system accent color secondary.
    /// </summary>
    SystemAccentColorSecondary,

    /// <summary>
    /// The system accent color tertiary.
    /// </summary>
    SystemAccentColorTertiary,

    /// <summary>
    /// The system accent color primary brush.
    /// </summary>
    SystemAccentColorPrimaryBrush,

    /// <summary>
    /// The system accent color secondary brush.
    /// </summary>
    SystemAccentColorSecondaryBrush,

    /// <summary>
    /// The system accent color tertiary brush.
    /// </summary>
    SystemAccentColorTertiaryBrush,

    /// <summary>
    /// The accent text fill color primary brush.
    /// </summary>
    AccentTextFillColorPrimaryBrush,

    /// <summary>
    /// The accent text fill color secondary brush.
    /// </summary>
    AccentTextFillColorSecondaryBrush,

    /// <summary>
    /// The accent text fill color tertiary brush.
    /// </summary>
    AccentTextFillColorTertiaryBrush,

    /// <summary>
    /// The application background color.
    /// </summary>
    ApplicationBackgroundColor,

    /// <summary>
    /// The application background brush.
    /// </summary>
    ApplicationBackgroundBrush,

    /// <summary>
    /// The keyboard focus border color.
    /// </summary>
    KeyboardFocusBorderColor,

    /// <summary>
    /// The keyboard focus border color brush.
    /// </summary>
    KeyboardFocusBorderColorBrush,

    /// <summary>
    /// The text fill color primary.
    /// </summary>
    TextFillColorPrimary,

    /// <summary>
    /// The text fill color secondary.
    /// </summary>
    TextFillColorSecondary,

    /// <summary>
    /// The text fill color tertiary.
    /// </summary>
    TextFillColorTertiary,

    /// <summary>
    /// The text fill color disabled.
    /// </summary>
    TextFillColorDisabled,

    /// <summary>
    /// The text placeholder color.
    /// </summary>
    TextPlaceholderColor,

    /// <summary>
    /// The text fill color inverse.
    /// </summary>
    TextFillColorInverse,

    /// <summary>
    /// The accent text fill color disabled.
    /// </summary>
    AccentTextFillColorDisabled,

    /// <summary>
    /// The text on accent fill color selected text.
    /// </summary>
    TextOnAccentFillColorSelectedText,

    /// <summary>
    /// The text on accent fill color primary.
    /// </summary>
    TextOnAccentFillColorPrimary,

    /// <summary>
    /// The text on accent fill color secondary.
    /// </summary>
    TextOnAccentFillColorSecondary,

    /// <summary>
    /// The text on accent fill color disabled.
    /// </summary>
    TextOnAccentFillColorDisabled,

    /// <summary>
    /// The control fill color default.
    /// </summary>
    ControlFillColorDefault,

    /// <summary>
    /// The control fill color secondary.
    /// </summary>
    ControlFillColorSecondary,

    /// <summary>
    /// The control fill color tertiary.
    /// </summary>
    ControlFillColorTertiary,

    /// <summary>
    /// The control fill color disabled.
    /// </summary>
    ControlFillColorDisabled,

    /// <summary>
    /// The control fill color transparent.
    /// </summary>
    ControlFillColorTransparent,

    /// <summary>
    /// The control fill color input active.
    /// </summary>
    ControlFillColorInputActive,

    /// <summary>
    /// The control strong fill color default.
    /// </summary>
    ControlStrongFillColorDefault,

    /// <summary>
    /// The control strong fill color disabled.
    /// </summary>
    ControlStrongFillColorDisabled,

    /// <summary>
    /// The control solid fill color default.
    /// </summary>
    ControlSolidFillColorDefault,

    /// <summary>
    /// The subtle fill color transparent.
    /// </summary>
    SubtleFillColorTransparent,

    /// <summary>
    /// The subtle fill color secondary.
    /// </summary>
    SubtleFillColorSecondary,

    /// <summary>
    /// The subtle fill color tertiary.
    /// </summary>
    SubtleFillColorTertiary,

    /// <summary>
    /// The subtle fill color disabled.
    /// </summary>
    SubtleFillColorDisabled,

    /// <summary>
    /// The control alt fill color transparent.
    /// </summary>
    ControlAltFillColorTransparent,

    /// <summary>
    /// The control alt fill color secondary.
    /// </summary>
    ControlAltFillColorSecondary,

    /// <summary>
    /// The control alt fill color tertiary.
    /// </summary>
    ControlAltFillColorTertiary,

    /// <summary>
    /// The control alt fill color quarternary.
    /// </summary>
    ControlAltFillColorQuarternary,

    /// <summary>
    /// The control alt fill color disabled.
    /// </summary>
    ControlAltFillColorDisabled,

    /// <summary>
    /// The control on image fill color default.
    /// </summary>
    ControlOnImageFillColorDefault,

    /// <summary>
    /// The control on image fill color secondary.
    /// </summary>
    ControlOnImageFillColorSecondary,

    /// <summary>
    /// The control on image fill color tertiary.
    /// </summary>
    ControlOnImageFillColorTertiary,

    /// <summary>
    /// The control on image fill color disabled.
    /// </summary>
    ControlOnImageFillColorDisabled,

    /// <summary>
    /// The accent fill color disabled.
    /// </summary>
    AccentFillColorDisabled,

    /// <summary>
    /// The control stroke color default.
    /// </summary>
    ControlStrokeColorDefault,

    /// <summary>
    /// The control stroke color secondary.
    /// </summary>
    ControlStrokeColorSecondary,

    /// <summary>
    /// The control stroke color tertiary.
    /// </summary>
    ControlStrokeColorTertiary,

    /// <summary>
    /// The control stroke color on accent default.
    /// </summary>
    ControlStrokeColorOnAccentDefault,

    /// <summary>
    /// The control stroke color on accent secondary.
    /// </summary>
    ControlStrokeColorOnAccentSecondary,

    /// <summary>
    /// The control stroke color on accent tertiary.
    /// </summary>
    ControlStrokeColorOnAccentTertiary,

    /// <summary>
    /// The control stroke color on accent disabled.
    /// </summary>
    ControlStrokeColorOnAccentDisabled,

    /// <summary>
    /// The control stroke color for strong fill when on image.
    /// </summary>
    ControlStrokeColorForStrongFillWhenOnImage,

    /// <summary>
    /// The card stroke color default.
    /// </summary>
    CardStrokeColorDefault,

    /// <summary>
    /// The card stroke color default solid.
    /// </summary>
    CardStrokeColorDefaultSolid,

    /// <summary>
    /// The control strong stroke color default.
    /// </summary>
    ControlStrongStrokeColorDefault,

    /// <summary>
    /// The control strong stroke color disabled.
    /// </summary>
    ControlStrongStrokeColorDisabled,

    /// <summary>
    /// The surface stroke color default.
    /// </summary>
    SurfaceStrokeColorDefault,

    /// <summary>
    /// The surface stroke color flyout.
    /// </summary>
    SurfaceStrokeColorFlyout,

    /// <summary>
    /// The surface stroke color inverse.
    /// </summary>
    SurfaceStrokeColorInverse,

    /// <summary>
    /// The divider stroke color default.
    /// </summary>
    DividerStrokeColorDefault,

    /// <summary>
    /// The focus stroke color outer.
    /// </summary>
    FocusStrokeColorOuter,

    /// <summary>
    /// The focus stroke color inner.
    /// </summary>
    FocusStrokeColorInner,

    /// <summary>
    /// The card background fill color default.
    /// </summary>
    CardBackgroundFillColorDefault,

    /// <summary>
    /// The card background fill color secondary.
    /// </summary>
    CardBackgroundFillColorSecondary,

    /// <summary>
    /// The smoke fill color default.
    /// </summary>
    SmokeFillColorDefault,

    /// <summary>
    /// The layer fill color default.
    /// </summary>
    LayerFillColorDefault,

    /// <summary>
    /// The layer fill color alt.
    /// </summary>
    LayerFillColorAlt,

    /// <summary>
    /// The layer on acrylic fill color default.
    /// </summary>
    LayerOnAcrylicFillColorDefault,

    /// <summary>
    /// The layer on accent acrylic fill color default.
    /// </summary>
    LayerOnAccentAcrylicFillColorDefault,

    /// <summary>
    /// The layer on mica base alt fill color default.
    /// </summary>
    LayerOnMicaBaseAltFillColorDefault,

    /// <summary>
    /// The layer on mica base alt fill color secondary.
    /// </summary>
    LayerOnMicaBaseAltFillColorSecondary,

    /// <summary>
    /// The layer on mica base alt fill color tertiary.
    /// </summary>
    LayerOnMicaBaseAltFillColorTertiary,

    /// <summary>
    /// The layer on mica base alt fill color transparent.
    /// </summary>
    LayerOnMicaBaseAltFillColorTransparent,

    /// <summary>
    /// The solid background fill color base.
    /// </summary>
    SolidBackgroundFillColorBase,

    /// <summary>
    /// The solid background fill color secondary.
    /// </summary>
    SolidBackgroundFillColorSecondary,

    /// <summary>
    /// The solid background fill color tertiary.
    /// </summary>
    SolidBackgroundFillColorTertiary,

    /// <summary>
    /// The solid background fill color quarternary.
    /// </summary>
    SolidBackgroundFillColorQuarternary,

    /// <summary>
    /// The solid background fill color transparent.
    /// </summary>
    SolidBackgroundFillColorTransparent,

    /// <summary>
    /// The solid background fill color base alt.
    /// </summary>
    SolidBackgroundFillColorBaseAlt,

    /// <summary>
    /// The system fill color success.
    /// </summary>
    SystemFillColorSuccess,

    /// <summary>
    /// The system fill color caution.
    /// </summary>
    SystemFillColorCaution,

    /// <summary>
    /// The system fill color critical.
    /// </summary>
    SystemFillColorCritical,

    /// <summary>
    /// The system fill color neutral.
    /// </summary>
    SystemFillColorNeutral,

    /// <summary>
    /// The system fill color solid neutral.
    /// </summary>
    SystemFillColorSolidNeutral,

    /// <summary>
    /// The system fill color attention background.
    /// </summary>
    SystemFillColorAttentionBackground,

    /// <summary>
    /// The system fill color success background.
    /// </summary>
    SystemFillColorSuccessBackground,

    /// <summary>
    /// The system fill color caution background.
    /// </summary>
    SystemFillColorCautionBackground,

    /// <summary>
    /// The system fill color critical background.
    /// </summary>
    SystemFillColorCriticalBackground,

    /// <summary>
    /// The system fill color neutral background.
    /// </summary>
    SystemFillColorNeutralBackground,

    /// <summary>
    /// The system fill color solid attention background.
    /// </summary>
    SystemFillColorSolidAttentionBackground,

    /// <summary>
    /// The system fill color solid neutral background.
    /// </summary>
    SystemFillColorSolidNeutralBackground,

    /// <summary>
    /// The text fill color primary brush.
    /// </summary>
    TextFillColorPrimaryBrush,

    /// <summary>
    /// The text fill color secondary brush.
    /// </summary>
    TextFillColorSecondaryBrush,

    /// <summary>
    /// The text fill color tertiary brush.
    /// </summary>
    TextFillColorTertiaryBrush,

    /// <summary>
    /// The text fill color disabled brush.
    /// </summary>
    TextFillColorDisabledBrush,

    /// <summary>
    /// The text placeholder color brush.
    /// </summary>
    TextPlaceholderColorBrush,

    /// <summary>
    /// The text fill color inverse brush.
    /// </summary>
    TextFillColorInverseBrush,

    /// <summary>
    /// The accent text fill color disabled brush.
    /// </summary>
    AccentTextFillColorDisabledBrush,

    /// <summary>
    /// The text on accent fill color selected text brush.
    /// </summary>
    TextOnAccentFillColorSelectedTextBrush,

    /// <summary>
    /// The text on accent fill color primary brush.
    /// </summary>
    TextOnAccentFillColorPrimaryBrush,

    /// <summary>
    /// The text on accent fill color secondary brush.
    /// </summary>
    TextOnAccentFillColorSecondaryBrush,

    /// <summary>
    /// The text on accent fill color disabled brush.
    /// </summary>
    TextOnAccentFillColorDisabledBrush,

    /// <summary>
    /// The control fill color default brush.
    /// </summary>
    ControlFillColorDefaultBrush,

    /// <summary>
    /// The control fill color secondary brush.
    /// </summary>
    ControlFillColorSecondaryBrush,

    /// <summary>
    /// The control fill color tertiary brush.
    /// </summary>
    ControlFillColorTertiaryBrush,

    /// <summary>
    /// The control fill color disabled brush.
    /// </summary>
    ControlFillColorDisabledBrush,

    /// <summary>
    /// The control fill color transparent brush.
    /// </summary>
    ControlFillColorTransparentBrush,

    /// <summary>
    /// The control fill color input active brush.
    /// </summary>
    ControlFillColorInputActiveBrush,

    /// <summary>
    /// The control strong fill color default brush.
    /// </summary>
    ControlStrongFillColorDefaultBrush,

    /// <summary>
    /// The control strong fill color disabled brush.
    /// </summary>
    ControlStrongFillColorDisabledBrush,

    /// <summary>
    /// The control solid fill color default brush.
    /// </summary>
    ControlSolidFillColorDefaultBrush,

    /// <summary>
    /// The subtle fill color transparent brush.
    /// </summary>
    SubtleFillColorTransparentBrush,

    /// <summary>
    /// The subtle fill color secondary brush.
    /// </summary>
    SubtleFillColorSecondaryBrush,

    /// <summary>
    /// The subtle fill color tertiary brush.
    /// </summary>
    SubtleFillColorTertiaryBrush,

    /// <summary>
    /// The subtle fill color disabled brush.
    /// </summary>
    SubtleFillColorDisabledBrush,

    /// <summary>
    /// The control alt fill color transparent brush.
    /// </summary>
    ControlAltFillColorTransparentBrush,

    /// <summary>
    /// The control alt fill color secondary brush.
    /// </summary>
    ControlAltFillColorSecondaryBrush,

    /// <summary>
    /// The control alt fill color tertiary brush.
    /// </summary>
    ControlAltFillColorTertiaryBrush,

    /// <summary>
    /// The control alt fill color quarternary brush.
    /// </summary>
    ControlAltFillColorQuarternaryBrush,

    /// <summary>
    /// The control alt fill color disabled brush.
    /// </summary>
    ControlAltFillColorDisabledBrush,

    /// <summary>
    /// The control on image fill color default brush.
    /// </summary>
    ControlOnImageFillColorDefaultBrush,

    /// <summary>
    /// The control on image fill color secondary brush.
    /// </summary>
    ControlOnImageFillColorSecondaryBrush,

    /// <summary>
    /// The control on image fill color tertiary brush.
    /// </summary>
    ControlOnImageFillColorTertiaryBrush,

    /// <summary>
    /// The control on image fill color disabled brush.
    /// </summary>
    ControlOnImageFillColorDisabledBrush,

    /// <summary>
    /// The accent fill color disabled brush.
    /// </summary>
    AccentFillColorDisabledBrush,

    /// <summary>
    /// The control stroke color default brush.
    /// </summary>
    ControlStrokeColorDefaultBrush,

    /// <summary>
    /// The control stroke color secondary brush.
    /// </summary>
    ControlStrokeColorSecondaryBrush,

    /// <summary>
    /// The control stroke color tertiary brush.
    /// </summary>
    ControlStrokeColorTertiaryBrush,

    /// <summary>
    /// The control stroke color on accent default brush.
    /// </summary>
    ControlStrokeColorOnAccentDefaultBrush,

    /// <summary>
    /// The control stroke color on accent secondary brush.
    /// </summary>
    ControlStrokeColorOnAccentSecondaryBrush,

    /// <summary>
    /// The control stroke color on accent tertiary brush.
    /// </summary>
    ControlStrokeColorOnAccentTertiaryBrush,

    /// <summary>
    /// The control stroke color on accent disabled brush.
    /// </summary>
    ControlStrokeColorOnAccentDisabledBrush,

    /// <summary>
    /// The control stroke color for strong fill when on image brush.
    /// </summary>
    ControlStrokeColorForStrongFillWhenOnImageBrush,

    /// <summary>
    /// The card stroke color default brush.
    /// </summary>
    CardStrokeColorDefaultBrush,

    /// <summary>
    /// The card stroke color default solid brush.
    /// </summary>
    CardStrokeColorDefaultSolidBrush,

    /// <summary>
    /// The control strong stroke color default brush.
    /// </summary>
    ControlStrongStrokeColorDefaultBrush,

    /// <summary>
    /// The control strong stroke color disabled brush.
    /// </summary>
    ControlStrongStrokeColorDisabledBrush,

    /// <summary>
    /// The surface stroke color default brush.
    /// </summary>
    SurfaceStrokeColorDefaultBrush,

    /// <summary>
    /// The surface stroke color flyout brush.
    /// </summary>
    SurfaceStrokeColorFlyoutBrush,

    /// <summary>
    /// The surface stroke color inverse brush.
    /// </summary>
    SurfaceStrokeColorInverseBrush,

    /// <summary>
    /// The divider stroke color default brush.
    /// </summary>
    DividerStrokeColorDefaultBrush,

    /// <summary>
    /// The focus stroke color outer brush.
    /// </summary>
    FocusStrokeColorOuterBrush,

    /// <summary>
    /// The focus stroke color inner brush.
    /// </summary>
    FocusStrokeColorInnerBrush,

    /// <summary>
    /// The card background fill color default brush.
    /// </summary>
    CardBackgroundFillColorDefaultBrush,

    /// <summary>
    /// The card background fill color secondary brush.
    /// </summary>
    CardBackgroundFillColorSecondaryBrush,

    /// <summary>
    /// The smoke fill color default brush.
    /// </summary>
    SmokeFillColorDefaultBrush,

    /// <summary>
    /// The layer fill color default brush.
    /// </summary>
    LayerFillColorDefaultBrush,

    /// <summary>
    /// The layer fill color alt brush.
    /// </summary>
    LayerFillColorAltBrush,

    /// <summary>
    /// The layer on acrylic fill color default brush.
    /// </summary>
    LayerOnAcrylicFillColorDefaultBrush,

    /// <summary>
    /// The layer on accent acrylic fill color default brush.
    /// </summary>
    LayerOnAccentAcrylicFillColorDefaultBrush,

    /// <summary>
    /// The layer on mica base alt fill color default brush.
    /// </summary>
    LayerOnMicaBaseAltFillColorDefaultBrush,

    /// <summary>
    /// The layer on mica base alt fill color secondary brush.
    /// </summary>
    LayerOnMicaBaseAltFillColorSecondaryBrush,

    /// <summary>
    /// The layer on mica base alt fill color tertiary brush.
    /// </summary>
    LayerOnMicaBaseAltFillColorTertiaryBrush,

    /// <summary>
    /// The layer on mica base alt fill color transparent brush.
    /// </summary>
    LayerOnMicaBaseAltFillColorTransparentBrush,

    /// <summary>
    /// The solid background fill color base brush.
    /// </summary>
    SolidBackgroundFillColorBaseBrush,

    /// <summary>
    /// The solid background fill color secondary brush.
    /// </summary>
    SolidBackgroundFillColorSecondaryBrush,

    /// <summary>
    /// The solid background fill color tertiary brush.
    /// </summary>
    SolidBackgroundFillColorTertiaryBrush,

    /// <summary>
    /// The solid background fill color quarternary brush.
    /// </summary>
    SolidBackgroundFillColorQuarternaryBrush,

    /// <summary>
    /// The solid background fill color base alt brush.
    /// </summary>
    SolidBackgroundFillColorBaseAltBrush,

    /// <summary>
    /// The system fill color success brush.
    /// </summary>
    SystemFillColorSuccessBrush,

    /// <summary>
    /// The system fill color caution brush.
    /// </summary>
    SystemFillColorCautionBrush,

    /// <summary>
    /// The system fill color critical brush.
    /// </summary>
    SystemFillColorCriticalBrush,

    /// <summary>
    /// The system fill color neutral brush.
    /// </summary>
    SystemFillColorNeutralBrush,

    /// <summary>
    /// The system fill color solid neutral brush.
    /// </summary>
    SystemFillColorSolidNeutralBrush,

    /// <summary>
    /// The system fill color attention background brush.
    /// </summary>
    SystemFillColorAttentionBackgroundBrush,

    /// <summary>
    /// The system fill color success background brush.
    /// </summary>
    SystemFillColorSuccessBackgroundBrush,

    /// <summary>
    /// The system fill color caution background brush.
    /// </summary>
    SystemFillColorCautionBackgroundBrush,

    /// <summary>
    /// The system fill color critical background brush.
    /// </summary>
    SystemFillColorCriticalBackgroundBrush,

    /// <summary>
    /// The system fill color neutral background brush.
    /// </summary>
    SystemFillColorNeutralBackgroundBrush,

    /// <summary>
    /// The system fill color solid attention background brush.
    /// </summary>
    SystemFillColorSolidAttentionBackgroundBrush,

    /// <summary>
    /// The system fill color solid neutral background brush.
    /// </summary>
    SystemFillColorSolidNeutralBackgroundBrush,

    /// <summary>
    /// Gradient <see cref="Brush"/>.
    /// </summary>
    ControlElevationBorderBrush,

    /// <summary>
    /// Gradient <see cref="Brush"/>.
    /// </summary>
    CircleElevationBorderBrush,

    /// <summary>
    /// Gradient <see cref="Brush"/>.
    /// </summary>
    AccentControlElevationBorderBrush
}
