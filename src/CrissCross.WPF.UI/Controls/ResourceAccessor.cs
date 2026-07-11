// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Resources;

namespace CrissCross.WPF.UI.Controls;

/// <summary>Provides the ResourceAccessor member.</summary>
/// <param name="controlType">The controlType value.</param>
internal sealed class ResourceAccessor(Type controlType)
{
    /// <summary>Resource key for .</summary>
    public const string BasicRatingString = "BasicRatingString";

    /// <summary>Resource key for CommunityRatingString.</summary>
    public const string CommunityRatingString = "CommunityRatingString";

    /// <summary>Resource key for RatingsControlName.</summary>
    public const string RatingsControlName = "RatingsControlName";

    /// <summary>Resource key for RatingControlName.</summary>
    public const string RatingControlName = "RatingControlName";

    /// <summary>Resource key for RatingUnset.</summary>
    public const string RatingUnset = "RatingUnset";

    /// <summary>Resource key for NavigationButtonClosedName.</summary>
    public const string NavigationButtonClosedName = "NavigationButtonClosedName";

    /// <summary>Resource key for NavigationButtonOpenName.</summary>
    public const string NavigationButtonOpenName = "NavigationButtonOpenName";

    /// <summary>Resource key for NavigationViewItemDefaultControlName.</summary>
    public const string NavigationViewItemDefaultControlName = "NavigationViewItemDefaultControlName";

    /// <summary>Resource key for NavigationBackButtonName.</summary>
    public const string NavigationBackButtonName = "NavigationBackButtonName";

    /// <summary>Resource key for NavigationBackButtonToolTip.</summary>
    public const string NavigationBackButtonToolTip = "NavigationBackButtonToolTip";

    /// <summary>Resource key for NavigationCloseButtonName.</summary>
    public const string NavigationCloseButtonName = "NavigationCloseButtonName";

    /// <summary>Resource key for NavigationOverflowButtonName.</summary>
    public const string NavigationOverflowButtonName = "NavigationOverflowButtonName";

    /// <summary>Resource key for NavigationOverflowButtonText.</summary>
    public const string NavigationOverflowButtonText = "NavigationOverflowButtonText";

    /// <summary>Resource key for NavigationOverflowButtonToolTip.</summary>
    public const string NavigationOverflowButtonToolTip = "NavigationOverflowButtonToolTip";

    /// <summary>Resource key for SettingsButtonName.</summary>
    public const string SettingsButtonName = "SettingsButtonName";

    /// <summary>Resource key for NavigationViewSearchButtonName.</summary>
    public const string NavigationViewSearchButtonName = "NavigationViewSearchButtonName";

    /// <summary>Resource key for TextAlphaLabel.</summary>
    public const string TextAlphaLabel = "TextAlphaLabel";

    /// <summary>Resource key for AutomationNameAlphaSlider.</summary>
    public const string AutomationNameAlphaSlider = "AutomationNameAlphaSlider";

    /// <summary>Resource key for AutomationNameAlphaTextBox.</summary>
    public const string AutomationNameAlphaTextBox = "AutomationNameAlphaTextBox";

    /// <summary>Resource key for AutomationNameHueSlider.</summary>
    public const string AutomationNameHueSlider = "AutomationNameHueSlider";

    /// <summary>Resource key for AutomationNameSaturationSlider.</summary>
    public const string AutomationNameSaturationSlider = "AutomationNameSaturationSlider";

    /// <summary>Resource key for AutomationNameValueSlider.</summary>
    public const string AutomationNameValueSlider = "AutomationNameValueSlider";

    /// <summary>Resource key for TextBlueLabel.</summary>
    public const string TextBlueLabel = "TextBlueLabel";

    /// <summary>Resource key for AutomationNameBlueTextBox.</summary>
    public const string AutomationNameBlueTextBox = "AutomationNameBlueTextBox";

    /// <summary>Resource key for AutomationNameColorModelComboBox.</summary>
    public const string AutomationNameColorModelComboBox = "AutomationNameColorModelComboBox";

    /// <summary>Resource key for AutomationNameColorSpectrum.</summary>
    public const string AutomationNameColorSpectrum = "AutomationNameColorSpectrum";

    /// <summary>Resource key for TextGreenLabel.</summary>
    public const string TextGreenLabel = "TextGreenLabel";

    /// <summary>Resource key for AutomationNameGreenTextBox.</summary>
    public const string AutomationNameGreenTextBox = "AutomationNameGreenTextBox";

    /// <summary>Resource key for HelpTextColorSpectrum.</summary>
    public const string HelpTextColorSpectrum = "HelpTextColorSpectrum";

    /// <summary>Resource key for AutomationNameHexTextBox.</summary>
    public const string AutomationNameHexTextBox = "AutomationNameHexTextBox";

    /// <summary>Resource key for ContentHSVComboBoxItem.</summary>
    public const string ContentHSVComboBoxItem = "ContentHSVComboBoxItem";

    /// <summary>Resource key for TextHueLabel.</summary>
    public const string TextHueLabel = "TextHueLabel";

    /// <summary>Resource key for AutomationNameHueTextBox.</summary>
    public const string AutomationNameHueTextBox = "AutomationNameHueTextBox";

    /// <summary>Resource key for LocalizedControlTypeColorSpectrum.</summary>
    public const string LocalizedControlTypeColorSpectrum = "LocalizedControlTypeColorSpectrum";

    /// <summary>Resource key for TextRedLabel.</summary>
    public const string TextRedLabel = "TextRedLabel";

    /// <summary>Resource key for AutomationNameRedTextBox.</summary>
    public const string AutomationNameRedTextBox = "AutomationNameRedTextBox";

    /// <summary>Resource key for ContentRGBComboBoxItem.</summary>
    public const string ContentRGBComboBoxItem = "ContentRGBComboBoxItem";

    /// <summary>Resource key for TextSaturationLabel.</summary>
    public const string TextSaturationLabel = "TextSaturationLabel";

    /// <summary>Resource key for AutomationNameSaturationTextBox.</summary>
    public const string AutomationNameSaturationTextBox = "AutomationNameSaturationTextBox";

    /// <summary>Resource key for TextValueLabel.</summary>
    public const string TextValueLabel = "TextValueLabel";

    /// <summary>Resource key for ValueStringColorSpectrumWithColorName.</summary>
    public const string ValueStringColorSpectrumWithColorName = "ValueStringColorSpectrumWithColorName";

    /// <summary>Resource key for ValueStringColorSpectrumWithoutColorName.</summary>
    public const string ValueStringColorSpectrumWithoutColorName = "ValueStringColorSpectrumWithoutColorName";

    /// <summary>Resource key for ValueStringHueSliderWithColorName.</summary>
    public const string ValueStringHueSliderWithColorName = "ValueStringHueSliderWithColorName";

    /// <summary>Resource key for ValueStringHueSliderWithoutColorName.</summary>
    public const string ValueStringHueSliderWithoutColorName = "ValueStringHueSliderWithoutColorName";

    /// <summary>Resource key for ValueStringSaturationSliderWithColorName.</summary>
    public const string ValueStringSaturationSliderWithColorName = "ValueStringSaturationSliderWithColorName";

    /// <summary>Resource key for ValueStringSaturationSliderWithoutColorName.</summary>
    public const string ValueStringSaturationSliderWithoutColorName = "ValueStringSaturationSliderWithoutColorName";

    /// <summary>Resource key for ValueStringValueSliderWithColorName.</summary>
    public const string ValueStringValueSliderWithColorName = "ValueStringValueSliderWithColorName";

    /// <summary>Resource key for ValueStringValueSliderWithoutColorName.</summary>
    public const string ValueStringValueSliderWithoutColorName = "ValueStringValueSliderWithoutColorName";

    /// <summary>Resource key for AutomationNameValueTextBox.</summary>
    public const string AutomationNameValueTextBox = "AutomationNameValueTextBox";

    /// <summary>Resource key for ToolTipStringAlphaSlider.</summary>
    public const string ToolTipStringAlphaSlider = "ToolTipStringAlphaSlider";

    /// <summary>Resource key for ToolTipStringHueSliderWithColorName.</summary>
    public const string ToolTipStringHueSliderWithColorName = "ToolTipStringHueSliderWithColorName";

    /// <summary>Resource key for ToolTipStringHueSliderWithoutColorName.</summary>
    public const string ToolTipStringHueSliderWithoutColorName = "ToolTipStringHueSliderWithoutColorName";

    /// <summary>Resource key for ToolTipStringSaturationSliderWithColorName.</summary>
    public const string ToolTipStringSaturationSliderWithColorName = "ToolTipStringSaturationSliderWithColorName";

    /// <summary>Resource key for ToolTipStringSaturationSliderWithoutColorName.</summary>
    public const string ToolTipStringSaturationSliderWithoutColorName = "ToolTipStringSaturationSliderWithoutColorName";

    /// <summary>Resource key for ToolTipStringValueSliderWithColorName.</summary>
    public const string ToolTipStringValueSliderWithColorName = "ToolTipStringValueSliderWithColorName";

    /// <summary>Resource key for ToolTipStringValueSliderWithoutColorName.</summary>
    public const string ToolTipStringValueSliderWithoutColorName = "ToolTipStringValueSliderWithoutColorName";

    /// <summary>Resource key for AutomationNameMoreButtonCollapsed.</summary>
    public const string AutomationNameMoreButtonCollapsed = "AutomationNameMoreButtonCollapsed";

    /// <summary>Resource key for AutomationNameMoreButtonExpanded.</summary>
    public const string AutomationNameMoreButtonExpanded = "AutomationNameMoreButtonExpanded";

    /// <summary>Resource key for HelpTextMoreButton.</summary>
    public const string HelpTextMoreButton = "HelpTextMoreButton";

    /// <summary>Resource key for TextMoreButtonLabelCollapsed.</summary>
    public const string TextMoreButtonLabelCollapsed = "TextMoreButtonLabelCollapsed";

    /// <summary>Resource key for TextMoreButtonLabelExpanded.</summary>
    public const string TextMoreButtonLabelExpanded = "TextMoreButtonLabelExpanded";

    /// <summary>Resource key for BadgeItemPlural1.</summary>
    public const string BadgeItemPlural1 = "BadgeItemPlural1";

    /// <summary>Resource key for BadgeItemPlural2.</summary>
    public const string BadgeItemPlural2 = "BadgeItemPlural2";

    /// <summary>Resource key for BadgeItemPlural3.</summary>
    public const string BadgeItemPlural3 = "BadgeItemPlural3";

    /// <summary>Resource key for BadgeItemPlural4.</summary>
    public const string BadgeItemPlural4 = "BadgeItemPlural4";

    /// <summary>Resource key for BadgeItemPlural5.</summary>
    public const string BadgeItemPlural5 = "BadgeItemPlural5";

    /// <summary>Resource key for BadgeItemPlural6.</summary>
    public const string BadgeItemPlural6 = "BadgeItemPlural6";

    /// <summary>Resource key for BadgeItemPlural7.</summary>
    public const string BadgeItemPlural7 = "BadgeItemPlural7";

    /// <summary>Resource key for BadgeItemSingular.</summary>
    public const string BadgeItemSingular = "BadgeItemSingular";

    /// <summary>Resource key for BadgeItemTextOverride.</summary>
    public const string BadgeItemTextOverride = "BadgeItemTextOverride";

    /// <summary>Resource key for BadgeIcon.</summary>
    public const string BadgeIcon = "BadgeIcon";

    /// <summary>Resource key for BadgeIconTextOverride.</summary>
    public const string BadgeIconTextOverride = "BadgeIconTextOverride";

    /// <summary>Resource key for PersonName.</summary>
    public const string PersonName = "PersonName";

    /// <summary>Resource key for GroupName.</summary>
    public const string GroupName = "GroupName";

    /// <summary>Resource key for CancelDraggingString.</summary>
    public const string CancelDraggingString = "CancelDraggingString";

    /// <summary>Resource key for DefaultItemString.</summary>
    public const string DefaultItemString = "DefaultItemString";

    /// <summary>Resource key for DropIntoNodeString.</summary>
    public const string DropIntoNodeString = "DropIntoNodeString";

    /// <summary>Resource key for FallBackPlaceString.</summary>
    public const string FallBackPlaceString = "FallBackPlaceString";

    /// <summary>Resource key for PagerControlPageTextName.</summary>
    public const string PagerControlPageTextName = "PagerControlPageText";

    /// <summary>Resource key for PagerControlPrefixTextName.</summary>
    public const string PagerControlPrefixTextName = "PagerControlPrefixText";

    /// <summary>Resource key for PagerControlSuffixTextName.</summary>
    public const string PagerControlSuffixTextName = "PagerControlSuffixText";

    /// <summary>Resource key for PagerControlFirstPageButtonTextName.</summary>
    public const string PagerControlFirstPageButtonTextName = "PagerControlFirstPageButtonText";

    /// <summary>Resource key for PagerControlPreviousPageButtonTextName.</summary>
    public const string PagerControlPreviousPageButtonTextName = "PagerControlPreviousPageButtonText";

    /// <summary>Resource key for PagerControlNextPageButtonTextName.</summary>
    public const string PagerControlNextPageButtonTextName = "PagerControlNextPageButtonText";

    /// <summary>Resource key for PagerControlLastPageButtonTextName.</summary>
    public const string PagerControlLastPageButtonTextName = "PagerControlLastPageButtonText";

    /// <summary>Resource key for PipsPagerNameText.</summary>
    public const string PipsPagerNameText = "PipsPagerNameText";

    /// <summary>Resource key for PipsPagerNextPageButtonText.</summary>
    public const string PipsPagerNextPageButtonText = "PipsPagerNextPageButtonText";

    /// <summary>Resource key for PipsPagerPreviousPageButtonText.</summary>
    public const string PipsPagerPreviousPageButtonText = "PipsPagerPreviousPageButtonText";

    /// <summary>Resource key for PipsPagerPageText.</summary>
    public const string PipsPagerPageText = "PipsPagerPageText";

    /// <summary>Resource key for PlaceAfterString.</summary>
    public const string PlaceAfterString = "PlaceAfterString";

    /// <summary>Resource key for PlaceBeforeString.</summary>
    public const string PlaceBeforeString = "PlaceBeforeString";

    /// <summary>Resource key for PlaceBetweenString.</summary>
    public const string PlaceBetweenString = "PlaceBetweenString";

    /// <summary>Resource key for ProgressRingName.</summary>
    public const string ProgressRingName = "ProgressRingName";

    /// <summary>Resource key for ProgressRingIndeterminateStatus.</summary>
    public const string ProgressRingIndeterminateStatus = "ProgressRingIndeterminateStatus";

    /// <summary>Resource key for ProgressBarIndeterminateStatus.</summary>
    public const string ProgressBarIndeterminateStatus = "ProgressBarIndeterminateStatus";

    /// <summary>Resource key for ProgressBarPausedStatus.</summary>
    public const string ProgressBarPausedStatus = "ProgressBarPausedStatus";

    /// <summary>Resource key for ProgressBarErrorStatus.</summary>
    public const string ProgressBarErrorStatus = "ProgressBarErrorStatus";

    /// <summary>Resource key for RatingLocalizedControlType.</summary>
    public const string RatingLocalizedControlType = "RatingLocalizedControlType";

    /// <summary>Resource key for SplitButtonSecondaryButtonName.</summary>
    public const string SplitButtonSecondaryButtonName = "SplitButtonSecondaryButtonName";

    /// <summary>Resource key for ProofingMenuItemLabel.</summary>
    public const string ProofingMenuItemLabel = "ProofingMenuItemLabel";

    /// <summary>Resource key for TextCommandLabelCut.</summary>
    public const string TextCommandLabelCut = "TextCommandLabelCut";

    /// <summary>Resource key for TextCommandLabelCopy.</summary>
    public const string TextCommandLabelCopy = "TextCommandLabelCopy";

    /// <summary>Resource key for TextCommandLabelPaste.</summary>
    public const string TextCommandLabelPaste = "TextCommandLabelPaste";

    /// <summary>Resource key for TextCommandLabelSelectAll.</summary>
    public const string TextCommandLabelSelectAll = "TextCommandLabelSelectAll";

    /// <summary>Resource key for TextCommandLabelBold.</summary>
    public const string TextCommandLabelBold = "TextCommandLabelBold";

    /// <summary>Resource key for TextCommandLabelItalic.</summary>
    public const string TextCommandLabelItalic = "TextCommandLabelItalic";

    /// <summary>Resource key for TextCommandLabelUnderline.</summary>
    public const string TextCommandLabelUnderline = "TextCommandLabelUnderline";

    /// <summary>Resource key for TextCommandLabelUndo.</summary>
    public const string TextCommandLabelUndo = "TextCommandLabelUndo";

    /// <summary>Resource key for TextCommandLabelRedo.</summary>
    public const string TextCommandLabelRedo = "TextCommandLabelRedo";

    /// <summary>Resource key for TextCommandDescriptionCut.</summary>
    public const string TextCommandDescriptionCut = "TextCommandDescriptionCut";

    /// <summary>Resource key for TextCommandDescriptionCopy.</summary>
    public const string TextCommandDescriptionCopy = "TextCommandDescriptionCopy";

    /// <summary>Resource key for TextCommandDescriptionPaste.</summary>
    public const string TextCommandDescriptionPaste = "TextCommandDescriptionPaste";

    /// <summary>Resource key for TextCommandDescriptionSelectAll.</summary>
    public const string TextCommandDescriptionSelectAll = "TextCommandDescriptionSelectAll";

    /// <summary>Resource key for TextCommandDescriptionBold.</summary>
    public const string TextCommandDescriptionBold = "TextCommandDescriptionBold";

    /// <summary>Resource key for TextCommandDescriptionItalic.</summary>
    public const string TextCommandDescriptionItalic = "TextCommandDescriptionItalic";

    /// <summary>Resource key for TextCommandDescriptionUnderline.</summary>
    public const string TextCommandDescriptionUnderline = "TextCommandDescriptionUnderline";

    /// <summary>Resource key for TextCommandDescriptionUndo.</summary>
    public const string TextCommandDescriptionUndo = "TextCommandDescriptionUndo";

    /// <summary>Resource key for TextCommandDescriptionRedo.</summary>
    public const string TextCommandDescriptionRedo = "TextCommandDescriptionRedo";

    /// <summary>Resource key for TextCommandKeyboardAcceleratorKeyCut.</summary>
    public const string TextCommandKeyboardAcceleratorKeyCut = "TextCommandKeyboardAcceleratorKeyCut";

    /// <summary>Resource key for TextCommandKeyboardAcceleratorKeyCopy.</summary>
    public const string TextCommandKeyboardAcceleratorKeyCopy = "TextCommandKeyboardAcceleratorKeyCopy";

    /// <summary>Resource key for TextCommandKeyboardAcceleratorKeyPaste.</summary>
    public const string TextCommandKeyboardAcceleratorKeyPaste = "TextCommandKeyboardAcceleratorKeyPaste";

    /// <summary>Resource key for TextCommandKeyboardAcceleratorKeySelectAll.</summary>
    public const string TextCommandKeyboardAcceleratorKeySelectAll = "TextCommandKeyboardAcceleratorKeySelectAll";

    /// <summary>Resource key for TextCommandKeyboardAcceleratorKeyBold.</summary>
    public const string TextCommandKeyboardAcceleratorKeyBold = "TextCommandKeyboardAcceleratorKeyBold";

    /// <summary>Resource key for TextCommandKeyboardAcceleratorKeyItalic.</summary>
    public const string TextCommandKeyboardAcceleratorKeyItalic = "TextCommandKeyboardAcceleratorKeyItalic";

    /// <summary>Resource key for TextCommandKeyboardAcceleratorKeyUnderline.</summary>
    public const string TextCommandKeyboardAcceleratorKeyUnderline = "TextCommandKeyboardAcceleratorKeyUnderline";

    /// <summary>Resource key for TextCommandKeyboardAcceleratorKeyUndo.</summary>
    public const string TextCommandKeyboardAcceleratorKeyUndo = "TextCommandKeyboardAcceleratorKeyUndo";

    /// <summary>Resource key for TextCommandKeyboardAcceleratorKeyRedo.</summary>
    public const string TextCommandKeyboardAcceleratorKeyRedo = "TextCommandKeyboardAcceleratorKeyRedo";

    /// <summary>Resource key for TeachingTipAlternateCloseButtonName.</summary>
    public const string TeachingTipAlternateCloseButtonName = "TeachingTipAlternateCloseButtonName";

    /// <summary>Resource key for TeachingTipAlternateCloseButtonTooltip.</summary>
    public const string TeachingTipAlternateCloseButtonTooltip = "TeachingTipAlternateCloseButtonTooltip";

    /// <summary>Resource key for TeachingTipCustomLandmarkName.</summary>
    public const string TeachingTipCustomLandmarkName = "TeachingTipCustomLandmarkName";

    /// <summary>Resource key for TeachingTipNotification.</summary>
    public const string TeachingTipNotification = "TeachingTipNotification";

    /// <summary>Resource key for TeachingTipNotificationWithoutAppName.</summary>
    public const string TeachingTipNotificationWithoutAppName = "TeachingTipNotificationWithoutAppName";

    /// <summary>Resource key for TabViewAddButtonName.</summary>
    public const string TabViewAddButtonName = "TabViewAddButtonName";

    /// <summary>Resource key for TabViewAddButtonTooltip.</summary>
    public const string TabViewAddButtonTooltip = "TabViewAddButtonTooltip";

    /// <summary>Resource key for TabViewCloseButtonName.</summary>
    public const string TabViewCloseButtonName = "TabViewCloseButtonName";

    /// <summary>Resource key for TabViewCloseButtonTooltip.</summary>
    public const string TabViewCloseButtonTooltip = "TabViewCloseButtonTooltip";

    /// <summary>Resource key for TabViewCloseButtonTooltipWithKA.</summary>
    public const string TabViewCloseButtonTooltipWithKA = "TabViewCloseButtonTooltipWithKA";

    /// <summary>Resource key for TabViewScrollDecreaseButtonTooltip.</summary>
    public const string TabViewScrollDecreaseButtonTooltip = "TabViewScrollDecreaseButtonTooltip";

    /// <summary>Resource key for TabViewScrollIncreaseButtonTooltip.</summary>
    public const string TabViewScrollIncreaseButtonTooltip = "TabViewScrollIncreaseButtonTooltip";

    /// <summary>Resource key for NumberBoxUpSpinButtonName.</summary>
    public const string NumberBoxUpSpinButtonName = "NumberBoxUpSpinButtonName";

    /// <summary>Resource key for NumberBoxDownSpinButtonName.</summary>
    public const string NumberBoxDownSpinButtonName = "NumberBoxDownSpinButtonName";

    /// <summary>Resource key for ExpanderDefaultControlName.</summary>
    public const string ExpanderDefaultControlName = "ExpanderDefaultControlName";

    /// <summary>Resource key for InfoBarCloseButtonName.</summary>
    public const string InfoBarCloseButtonName = "InfoBarCloseButtonName";

    /// <summary>Resource key for InfoBarOpenedNotification.</summary>
    public const string InfoBarOpenedNotification = "InfoBarOpenedNotification";

    /// <summary>Resource key for InfoBarClosedNotification.</summary>
    public const string InfoBarClosedNotification = "InfoBarClosedNotification";

    /// <summary>Resource key for InfoBarCustomLandmarkName.</summary>
    public const string InfoBarCustomLandmarkName = "InfoBarCustomLandmarkName";

    /// <summary>Resource key for InfoBarCloseButtonTooltip.</summary>
    public const string InfoBarCloseButtonTooltip = "InfoBarCloseButtonTooltip";

    /// <summary>Resource key for NoiseAsset256X256Png.</summary>
    public const string NoiseAsset256X256Png = "NoiseAsset_256X256_PNG";

    /// <summary>Stores the _controlType value.</summary>
    private readonly Type _controlType = controlType ?? throw new ArgumentNullException(nameof(controlType));

    /// <summary>Stores the _resourceManager value.</summary>
    private ResourceManager? _resourceManager;

    /// <summary>Provides the GetLocalizedStringResource member.</summary>
    /// <param name="resourceName">The resourceName value.</param>
    /// <returns>The result.</returns>
    public string? GetLocalizedStringResource(string resourceName)
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
