// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Resources;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Provides the ResourceAccessor member.</summary>
/// <param name="controlType">The controlType value.</param>
internal sealed class ResourceAccessor(Type controlType)
{
    /// <summary>Resource key for .</summary>
    internal const string BasicRatingString = "BasicRatingString";

    /// <summary>Resource key for CommunityRatingString.</summary>
    internal const string CommunityRatingString = "CommunityRatingString";

    /// <summary>Resource key for RatingsControlName.</summary>
    internal const string RatingsControlName = "RatingsControlName";

    /// <summary>Resource key for RatingControlName.</summary>
    internal const string RatingControlName = "RatingControlName";

    /// <summary>Resource key for RatingUnset.</summary>
    internal const string RatingUnset = "RatingUnset";

    /// <summary>Resource key for NavigationButtonClosedName.</summary>
    internal const string NavigationButtonClosedName = "NavigationButtonClosedName";

    /// <summary>Resource key for NavigationButtonOpenName.</summary>
    internal const string NavigationButtonOpenName = "NavigationButtonOpenName";

    /// <summary>Resource key for NavigationViewItemDefaultControlName.</summary>
    internal const string NavigationViewItemDefaultControlName = "NavigationViewItemDefaultControlName";

    /// <summary>Resource key for NavigationBackButtonName.</summary>
    internal const string NavigationBackButtonName = "NavigationBackButtonName";

    /// <summary>Resource key for NavigationBackButtonToolTip.</summary>
    internal const string NavigationBackButtonToolTip = "NavigationBackButtonToolTip";

    /// <summary>Resource key for NavigationCloseButtonName.</summary>
    internal const string NavigationCloseButtonName = "NavigationCloseButtonName";

    /// <summary>Resource key for NavigationOverflowButtonName.</summary>
    internal const string NavigationOverflowButtonName = "NavigationOverflowButtonName";

    /// <summary>Resource key for NavigationOverflowButtonText.</summary>
    internal const string NavigationOverflowButtonText = "NavigationOverflowButtonText";

    /// <summary>Resource key for NavigationOverflowButtonToolTip.</summary>
    internal const string NavigationOverflowButtonToolTip = "NavigationOverflowButtonToolTip";

    /// <summary>Resource key for SettingsButtonName.</summary>
    internal const string SettingsButtonName = "SettingsButtonName";

    /// <summary>Resource key for NavigationViewSearchButtonName.</summary>
    internal const string NavigationViewSearchButtonName = "NavigationViewSearchButtonName";

    /// <summary>Resource key for TextAlphaLabel.</summary>
    internal const string TextAlphaLabel = "TextAlphaLabel";

    /// <summary>Resource key for AutomationNameAlphaSlider.</summary>
    internal const string AutomationNameAlphaSlider = "AutomationNameAlphaSlider";

    /// <summary>Resource key for AutomationNameAlphaTextBox.</summary>
    internal const string AutomationNameAlphaTextBox = "AutomationNameAlphaTextBox";

    /// <summary>Resource key for AutomationNameHueSlider.</summary>
    internal const string AutomationNameHueSlider = "AutomationNameHueSlider";

    /// <summary>Resource key for AutomationNameSaturationSlider.</summary>
    internal const string AutomationNameSaturationSlider = "AutomationNameSaturationSlider";

    /// <summary>Resource key for AutomationNameValueSlider.</summary>
    internal const string AutomationNameValueSlider = "AutomationNameValueSlider";

    /// <summary>Resource key for TextBlueLabel.</summary>
    internal const string TextBlueLabel = "TextBlueLabel";

    /// <summary>Resource key for AutomationNameBlueTextBox.</summary>
    internal const string AutomationNameBlueTextBox = "AutomationNameBlueTextBox";

    /// <summary>Resource key for AutomationNameColorModelComboBox.</summary>
    internal const string AutomationNameColorModelComboBox = "AutomationNameColorModelComboBox";

    /// <summary>Resource key for AutomationNameColorSpectrum.</summary>
    internal const string AutomationNameColorSpectrum = "AutomationNameColorSpectrum";

    /// <summary>Resource key for TextGreenLabel.</summary>
    internal const string TextGreenLabel = "TextGreenLabel";

    /// <summary>Resource key for AutomationNameGreenTextBox.</summary>
    internal const string AutomationNameGreenTextBox = "AutomationNameGreenTextBox";

    /// <summary>Resource key for HelpTextColorSpectrum.</summary>
    internal const string HelpTextColorSpectrum = "HelpTextColorSpectrum";

    /// <summary>Resource key for AutomationNameHexTextBox.</summary>
    internal const string AutomationNameHexTextBox = "AutomationNameHexTextBox";

    /// <summary>Resource key for ContentHSVComboBoxItem.</summary>
    internal const string ContentHSVComboBoxItem = "ContentHSVComboBoxItem";

    /// <summary>Resource key for TextHueLabel.</summary>
    internal const string TextHueLabel = "TextHueLabel";

    /// <summary>Resource key for AutomationNameHueTextBox.</summary>
    internal const string AutomationNameHueTextBox = "AutomationNameHueTextBox";

    /// <summary>Resource key for LocalizedControlTypeColorSpectrum.</summary>
    internal const string LocalizedControlTypeColorSpectrum = "LocalizedControlTypeColorSpectrum";

    /// <summary>Resource key for TextRedLabel.</summary>
    internal const string TextRedLabel = "TextRedLabel";

    /// <summary>Resource key for AutomationNameRedTextBox.</summary>
    internal const string AutomationNameRedTextBox = "AutomationNameRedTextBox";

    /// <summary>Resource key for ContentRGBComboBoxItem.</summary>
    internal const string ContentRGBComboBoxItem = "ContentRGBComboBoxItem";

    /// <summary>Resource key for TextSaturationLabel.</summary>
    internal const string TextSaturationLabel = "TextSaturationLabel";

    /// <summary>Resource key for AutomationNameSaturationTextBox.</summary>
    internal const string AutomationNameSaturationTextBox = "AutomationNameSaturationTextBox";

    /// <summary>Resource key for TextValueLabel.</summary>
    internal const string TextValueLabel = "TextValueLabel";

    /// <summary>Resource key for ValueStringColorSpectrumWithColorName.</summary>
    internal const string ValueStringColorSpectrumWithColorName = "ValueStringColorSpectrumWithColorName";

    /// <summary>Resource key for ValueStringColorSpectrumWithoutColorName.</summary>
    internal const string ValueStringColorSpectrumWithoutColorName = "ValueStringColorSpectrumWithoutColorName";

    /// <summary>Resource key for ValueStringHueSliderWithColorName.</summary>
    internal const string ValueStringHueSliderWithColorName = "ValueStringHueSliderWithColorName";

    /// <summary>Resource key for ValueStringHueSliderWithoutColorName.</summary>
    internal const string ValueStringHueSliderWithoutColorName = "ValueStringHueSliderWithoutColorName";

    /// <summary>Resource key for ValueStringSaturationSliderWithColorName.</summary>
    internal const string ValueStringSaturationSliderWithColorName = "ValueStringSaturationSliderWithColorName";

    /// <summary>Resource key for ValueStringSaturationSliderWithoutColorName.</summary>
    internal const string ValueStringSaturationSliderWithoutColorName = "ValueStringSaturationSliderWithoutColorName";

    /// <summary>Resource key for ValueStringValueSliderWithColorName.</summary>
    internal const string ValueStringValueSliderWithColorName = "ValueStringValueSliderWithColorName";

    /// <summary>Resource key for ValueStringValueSliderWithoutColorName.</summary>
    internal const string ValueStringValueSliderWithoutColorName = "ValueStringValueSliderWithoutColorName";

    /// <summary>Resource key for AutomationNameValueTextBox.</summary>
    internal const string AutomationNameValueTextBox = "AutomationNameValueTextBox";

    /// <summary>Resource key for ToolTipStringAlphaSlider.</summary>
    internal const string ToolTipStringAlphaSlider = "ToolTipStringAlphaSlider";

    /// <summary>Resource key for ToolTipStringHueSliderWithColorName.</summary>
    internal const string ToolTipStringHueSliderWithColorName = "ToolTipStringHueSliderWithColorName";

    /// <summary>Resource key for ToolTipStringHueSliderWithoutColorName.</summary>
    internal const string ToolTipStringHueSliderWithoutColorName = "ToolTipStringHueSliderWithoutColorName";

    /// <summary>Resource key for ToolTipStringSaturationSliderWithColorName.</summary>
    internal const string ToolTipStringSaturationSliderWithColorName = "ToolTipStringSaturationSliderWithColorName";

    /// <summary>Resource key for ToolTipStringSaturationSliderWithoutColorName.</summary>
    internal const string ToolTipStringSaturationSliderWithoutColorName = "ToolTipStringSaturationSliderWithoutColorName";

    /// <summary>Resource key for ToolTipStringValueSliderWithColorName.</summary>
    internal const string ToolTipStringValueSliderWithColorName = "ToolTipStringValueSliderWithColorName";

    /// <summary>Resource key for ToolTipStringValueSliderWithoutColorName.</summary>
    internal const string ToolTipStringValueSliderWithoutColorName = "ToolTipStringValueSliderWithoutColorName";

    /// <summary>Resource key for AutomationNameMoreButtonCollapsed.</summary>
    internal const string AutomationNameMoreButtonCollapsed = "AutomationNameMoreButtonCollapsed";

    /// <summary>Resource key for AutomationNameMoreButtonExpanded.</summary>
    internal const string AutomationNameMoreButtonExpanded = "AutomationNameMoreButtonExpanded";

    /// <summary>Resource key for HelpTextMoreButton.</summary>
    internal const string HelpTextMoreButton = "HelpTextMoreButton";

    /// <summary>Resource key for TextMoreButtonLabelCollapsed.</summary>
    internal const string TextMoreButtonLabelCollapsed = "TextMoreButtonLabelCollapsed";

    /// <summary>Resource key for TextMoreButtonLabelExpanded.</summary>
    internal const string TextMoreButtonLabelExpanded = "TextMoreButtonLabelExpanded";

    /// <summary>Resource key for BadgeItemPlural1.</summary>
    internal const string BadgeItemPlural1 = "BadgeItemPlural1";

    /// <summary>Resource key for BadgeItemPlural2.</summary>
    internal const string BadgeItemPlural2 = "BadgeItemPlural2";

    /// <summary>Resource key for BadgeItemPlural3.</summary>
    internal const string BadgeItemPlural3 = "BadgeItemPlural3";

    /// <summary>Resource key for BadgeItemPlural4.</summary>
    internal const string BadgeItemPlural4 = "BadgeItemPlural4";

    /// <summary>Resource key for BadgeItemPlural5.</summary>
    internal const string BadgeItemPlural5 = "BadgeItemPlural5";

    /// <summary>Resource key for BadgeItemPlural6.</summary>
    internal const string BadgeItemPlural6 = "BadgeItemPlural6";

    /// <summary>Resource key for BadgeItemPlural7.</summary>
    internal const string BadgeItemPlural7 = "BadgeItemPlural7";

    /// <summary>Resource key for BadgeItemSingular.</summary>
    internal const string BadgeItemSingular = "BadgeItemSingular";

    /// <summary>Resource key for BadgeItemTextOverride.</summary>
    internal const string BadgeItemTextOverride = "BadgeItemTextOverride";

    /// <summary>Resource key for BadgeIcon.</summary>
    internal const string BadgeIcon = "BadgeIcon";

    /// <summary>Resource key for BadgeIconTextOverride.</summary>
    internal const string BadgeIconTextOverride = "BadgeIconTextOverride";

    /// <summary>Resource key for PersonName.</summary>
    internal const string PersonName = "PersonName";

    /// <summary>Resource key for GroupName.</summary>
    internal const string GroupName = "GroupName";

    /// <summary>Resource key for CancelDraggingString.</summary>
    internal const string CancelDraggingString = "CancelDraggingString";

    /// <summary>Resource key for DefaultItemString.</summary>
    internal const string DefaultItemString = "DefaultItemString";

    /// <summary>Resource key for DropIntoNodeString.</summary>
    internal const string DropIntoNodeString = "DropIntoNodeString";

    /// <summary>Resource key for FallBackPlaceString.</summary>
    internal const string FallBackPlaceString = "FallBackPlaceString";

    /// <summary>Resource key for PagerControlPageTextName.</summary>
    internal const string PagerControlPageTextName = "PagerControlPageText";

    /// <summary>Resource key for PagerControlPrefixTextName.</summary>
    internal const string PagerControlPrefixTextName = "PagerControlPrefixText";

    /// <summary>Resource key for PagerControlSuffixTextName.</summary>
    internal const string PagerControlSuffixTextName = "PagerControlSuffixText";

    /// <summary>Resource key for PagerControlFirstPageButtonTextName.</summary>
    internal const string PagerControlFirstPageButtonTextName = "PagerControlFirstPageButtonText";

    /// <summary>Resource key for PagerControlPreviousPageButtonTextName.</summary>
    internal const string PagerControlPreviousPageButtonTextName = "PagerControlPreviousPageButtonText";

    /// <summary>Resource key for PagerControlNextPageButtonTextName.</summary>
    internal const string PagerControlNextPageButtonTextName = "PagerControlNextPageButtonText";

    /// <summary>Resource key for PagerControlLastPageButtonTextName.</summary>
    internal const string PagerControlLastPageButtonTextName = "PagerControlLastPageButtonText";

    /// <summary>Resource key for PipsPagerNameText.</summary>
    internal const string PipsPagerNameText = "PipsPagerNameText";

    /// <summary>Resource key for PipsPagerNextPageButtonText.</summary>
    internal const string PipsPagerNextPageButtonText = "PipsPagerNextPageButtonText";

    /// <summary>Resource key for PipsPagerPreviousPageButtonText.</summary>
    internal const string PipsPagerPreviousPageButtonText = "PipsPagerPreviousPageButtonText";

    /// <summary>Resource key for PipsPagerPageText.</summary>
    internal const string PipsPagerPageText = "PipsPagerPageText";

    /// <summary>Resource key for PlaceAfterString.</summary>
    internal const string PlaceAfterString = "PlaceAfterString";

    /// <summary>Resource key for PlaceBeforeString.</summary>
    internal const string PlaceBeforeString = "PlaceBeforeString";

    /// <summary>Resource key for PlaceBetweenString.</summary>
    internal const string PlaceBetweenString = "PlaceBetweenString";

    /// <summary>Resource key for ProgressRingName.</summary>
    internal const string ProgressRingName = "ProgressRingName";

    /// <summary>Resource key for ProgressRingIndeterminateStatus.</summary>
    internal const string ProgressRingIndeterminateStatus = "ProgressRingIndeterminateStatus";

    /// <summary>Resource key for ProgressBarIndeterminateStatus.</summary>
    internal const string ProgressBarIndeterminateStatus = "ProgressBarIndeterminateStatus";

    /// <summary>Resource key for ProgressBarPausedStatus.</summary>
    internal const string ProgressBarPausedStatus = "ProgressBarPausedStatus";

    /// <summary>Resource key for ProgressBarErrorStatus.</summary>
    internal const string ProgressBarErrorStatus = "ProgressBarErrorStatus";

    /// <summary>Resource key for RatingLocalizedControlType.</summary>
    internal const string RatingLocalizedControlType = "RatingLocalizedControlType";

    /// <summary>Resource key for SplitButtonSecondaryButtonName.</summary>
    internal const string SplitButtonSecondaryButtonName = "SplitButtonSecondaryButtonName";

    /// <summary>Resource key for ProofingMenuItemLabel.</summary>
    internal const string ProofingMenuItemLabel = "ProofingMenuItemLabel";

    /// <summary>Resource key for TextCommandLabelCut.</summary>
    internal const string TextCommandLabelCut = "TextCommandLabelCut";

    /// <summary>Resource key for TextCommandLabelCopy.</summary>
    internal const string TextCommandLabelCopy = "TextCommandLabelCopy";

    /// <summary>Resource key for TextCommandLabelPaste.</summary>
    internal const string TextCommandLabelPaste = "TextCommandLabelPaste";

    /// <summary>Resource key for TextCommandLabelSelectAll.</summary>
    internal const string TextCommandLabelSelectAll = "TextCommandLabelSelectAll";

    /// <summary>Resource key for TextCommandLabelBold.</summary>
    internal const string TextCommandLabelBold = "TextCommandLabelBold";

    /// <summary>Resource key for TextCommandLabelItalic.</summary>
    internal const string TextCommandLabelItalic = "TextCommandLabelItalic";

    /// <summary>Resource key for TextCommandLabelUnderline.</summary>
    internal const string TextCommandLabelUnderline = "TextCommandLabelUnderline";

    /// <summary>Resource key for TextCommandLabelUndo.</summary>
    internal const string TextCommandLabelUndo = "TextCommandLabelUndo";

    /// <summary>Resource key for TextCommandLabelRedo.</summary>
    internal const string TextCommandLabelRedo = "TextCommandLabelRedo";

    /// <summary>Resource key for TextCommandDescriptionCut.</summary>
    internal const string TextCommandDescriptionCut = "TextCommandDescriptionCut";

    /// <summary>Resource key for TextCommandDescriptionCopy.</summary>
    internal const string TextCommandDescriptionCopy = "TextCommandDescriptionCopy";

    /// <summary>Resource key for TextCommandDescriptionPaste.</summary>
    internal const string TextCommandDescriptionPaste = "TextCommandDescriptionPaste";

    /// <summary>Resource key for TextCommandDescriptionSelectAll.</summary>
    internal const string TextCommandDescriptionSelectAll = "TextCommandDescriptionSelectAll";

    /// <summary>Resource key for TextCommandDescriptionBold.</summary>
    internal const string TextCommandDescriptionBold = "TextCommandDescriptionBold";

    /// <summary>Resource key for TextCommandDescriptionItalic.</summary>
    internal const string TextCommandDescriptionItalic = "TextCommandDescriptionItalic";

    /// <summary>Resource key for TextCommandDescriptionUnderline.</summary>
    internal const string TextCommandDescriptionUnderline = "TextCommandDescriptionUnderline";

    /// <summary>Resource key for TextCommandDescriptionUndo.</summary>
    internal const string TextCommandDescriptionUndo = "TextCommandDescriptionUndo";

    /// <summary>Resource key for TextCommandDescriptionRedo.</summary>
    internal const string TextCommandDescriptionRedo = "TextCommandDescriptionRedo";

    /// <summary>Resource key for TextCommandKeyboardAcceleratorKeyCut.</summary>
    internal const string TextCommandKeyboardAcceleratorKeyCut = "TextCommandKeyboardAcceleratorKeyCut";

    /// <summary>Resource key for TextCommandKeyboardAcceleratorKeyCopy.</summary>
    internal const string TextCommandKeyboardAcceleratorKeyCopy = "TextCommandKeyboardAcceleratorKeyCopy";

    /// <summary>Resource key for TextCommandKeyboardAcceleratorKeyPaste.</summary>
    internal const string TextCommandKeyboardAcceleratorKeyPaste = "TextCommandKeyboardAcceleratorKeyPaste";

    /// <summary>Resource key for TextCommandKeyboardAcceleratorKeySelectAll.</summary>
    internal const string TextCommandKeyboardAcceleratorKeySelectAll = "TextCommandKeyboardAcceleratorKeySelectAll";

    /// <summary>Resource key for TextCommandKeyboardAcceleratorKeyBold.</summary>
    internal const string TextCommandKeyboardAcceleratorKeyBold = "TextCommandKeyboardAcceleratorKeyBold";

    /// <summary>Resource key for TextCommandKeyboardAcceleratorKeyItalic.</summary>
    internal const string TextCommandKeyboardAcceleratorKeyItalic = "TextCommandKeyboardAcceleratorKeyItalic";

    /// <summary>Resource key for TextCommandKeyboardAcceleratorKeyUnderline.</summary>
    internal const string TextCommandKeyboardAcceleratorKeyUnderline = "TextCommandKeyboardAcceleratorKeyUnderline";

    /// <summary>Resource key for TextCommandKeyboardAcceleratorKeyUndo.</summary>
    internal const string TextCommandKeyboardAcceleratorKeyUndo = "TextCommandKeyboardAcceleratorKeyUndo";

    /// <summary>Resource key for TextCommandKeyboardAcceleratorKeyRedo.</summary>
    internal const string TextCommandKeyboardAcceleratorKeyRedo = "TextCommandKeyboardAcceleratorKeyRedo";

    /// <summary>Resource key for TeachingTipAlternateCloseButtonName.</summary>
    internal const string TeachingTipAlternateCloseButtonName = "TeachingTipAlternateCloseButtonName";

    /// <summary>Resource key for TeachingTipAlternateCloseButtonTooltip.</summary>
    internal const string TeachingTipAlternateCloseButtonTooltip = "TeachingTipAlternateCloseButtonTooltip";

    /// <summary>Resource key for TeachingTipCustomLandmarkName.</summary>
    internal const string TeachingTipCustomLandmarkName = "TeachingTipCustomLandmarkName";

    /// <summary>Resource key for TeachingTipNotification.</summary>
    internal const string TeachingTipNotification = "TeachingTipNotification";

    /// <summary>Resource key for TeachingTipNotificationWithoutAppName.</summary>
    internal const string TeachingTipNotificationWithoutAppName = "TeachingTipNotificationWithoutAppName";

    /// <summary>Resource key for TabViewAddButtonName.</summary>
    internal const string TabViewAddButtonName = "TabViewAddButtonName";

    /// <summary>Resource key for TabViewAddButtonTooltip.</summary>
    internal const string TabViewAddButtonTooltip = "TabViewAddButtonTooltip";

    /// <summary>Resource key for TabViewCloseButtonName.</summary>
    internal const string TabViewCloseButtonName = "TabViewCloseButtonName";

    /// <summary>Resource key for TabViewCloseButtonTooltip.</summary>
    internal const string TabViewCloseButtonTooltip = "TabViewCloseButtonTooltip";

    /// <summary>Resource key for TabViewCloseButtonTooltipWithKA.</summary>
    internal const string TabViewCloseButtonTooltipWithKA = "TabViewCloseButtonTooltipWithKA";

    /// <summary>Resource key for TabViewScrollDecreaseButtonTooltip.</summary>
    internal const string TabViewScrollDecreaseButtonTooltip = "TabViewScrollDecreaseButtonTooltip";

    /// <summary>Resource key for TabViewScrollIncreaseButtonTooltip.</summary>
    internal const string TabViewScrollIncreaseButtonTooltip = "TabViewScrollIncreaseButtonTooltip";

    /// <summary>Resource key for NumberBoxUpSpinButtonName.</summary>
    internal const string NumberBoxUpSpinButtonName = "NumberBoxUpSpinButtonName";

    /// <summary>Resource key for NumberBoxDownSpinButtonName.</summary>
    internal const string NumberBoxDownSpinButtonName = "NumberBoxDownSpinButtonName";

    /// <summary>Resource key for ExpanderDefaultControlName.</summary>
    internal const string ExpanderDefaultControlName = "ExpanderDefaultControlName";

    /// <summary>Resource key for InfoBarCloseButtonName.</summary>
    internal const string InfoBarCloseButtonName = "InfoBarCloseButtonName";

    /// <summary>Resource key for InfoBarOpenedNotification.</summary>
    internal const string InfoBarOpenedNotification = "InfoBarOpenedNotification";

    /// <summary>Resource key for InfoBarClosedNotification.</summary>
    internal const string InfoBarClosedNotification = "InfoBarClosedNotification";

    /// <summary>Resource key for InfoBarCustomLandmarkName.</summary>
    internal const string InfoBarCustomLandmarkName = "InfoBarCustomLandmarkName";

    /// <summary>Resource key for InfoBarCloseButtonTooltip.</summary>
    internal const string InfoBarCloseButtonTooltip = "InfoBarCloseButtonTooltip";

    /// <summary>Resource key for NoiseAsset256X256Png.</summary>
    internal const string NoiseAsset256X256Png = "NoiseAsset_256X256_PNG";

    /// <summary>Stores the _controlType value.</summary>
    private readonly Type _controlType = controlType ?? throw new ArgumentNullException(nameof(controlType));

    /// <summary>Stores the _resourceManager value.</summary>
    private ResourceManager? _resourceManager;

    /// <summary>Provides the GetLocalizedStringResource member.</summary>
    /// <param name="resourceName">The resourceName value.</param>
    /// <returns>The result.</returns>
    internal string? GetLocalizedStringResource(string resourceName)
    {
        if (_resourceManager is null)
        {
            var assembly = _controlType.Assembly;
            var assemblyName = assembly.GetName().Name;
            var controlName = _controlType.Name;
            var baseName = $"{assemblyName}.Controls.{controlName}.Strings.Resources";
            _resourceManager = new(baseName, assembly);
        }

        return _resourceManager.GetString(resourceName);
    }
}
