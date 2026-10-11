// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls.Automation.Windows;
#else
namespace CrissCross.Avalonia.UI.Controls.Automation.Windows;
#endif

/// <summary>Maps public managed automation properties to Windows identifiers.</summary>
internal sealed partial class WindowsAutomationNode
{
    /// <summary>The Windows heading level base identifier.</summary>
    private const int HeadingLevelBase = 80_050;

    /// <summary>The largest supported Windows heading level.</summary>
    private const int MaximumHeadingLevel = 9;

    /// <summary>The managed property identifiers emitted by Avalonia peers.</summary>
    private static readonly Dictionary<AutomationProperty, int> PropertyIds = new()
    {
        [AutomationElementIdentifiers.AutomationIdProperty] = PropertyId.AutomationId,
        [AutomationElementIdentifiers.BoundingRectangleProperty] = PropertyId.BoundingRectangle,
        [AutomationElementIdentifiers.ClassNameProperty] = PropertyId.ClassName,
        [AutomationElementIdentifiers.NameProperty] = PropertyId.Name,
        [AutomationElementIdentifiers.HelpTextProperty] = PropertyId.HelpText,
        [AutomationElementIdentifiers.LandmarkTypeProperty] = PropertyId.LandmarkType,
        [AutomationElementIdentifiers.HeadingLevelProperty] = PropertyId.HeadingLevel,
        [AutomationElementIdentifiers.ItemStatusProperty] = PropertyId.ItemStatus,
        [ValuePatternIdentifiers.ValueProperty] = PropertyId.ValueValue,
        [ValuePatternIdentifiers.IsReadOnlyProperty] = PropertyId.ValueIsReadOnly,
        [ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty] = PropertyId.ExpandCollapseState,
        [TogglePatternIdentifiers.ToggleStateProperty] = PropertyId.ToggleState,
        [RangeValuePatternIdentifiers.ValueProperty] = PropertyId.RangeValueValue,
        [RangeValuePatternIdentifiers.IsReadOnlyProperty] = PropertyId.RangeValueIsReadOnly,
        [RangeValuePatternIdentifiers.MinimumProperty] = PropertyId.RangeValueMinimum,
        [RangeValuePatternIdentifiers.MaximumProperty] = PropertyId.RangeValueMaximum,
        [ScrollPatternIdentifiers.HorizontalScrollPercentProperty] = PropertyId.ScrollHorizontalScrollPercent,
        [ScrollPatternIdentifiers.HorizontalViewSizeProperty] = PropertyId.ScrollHorizontalViewSize,
        [ScrollPatternIdentifiers.VerticalScrollPercentProperty] = PropertyId.ScrollVerticalScrollPercent,
        [ScrollPatternIdentifiers.VerticalViewSizeProperty] = PropertyId.ScrollVerticalViewSize,
        [ScrollPatternIdentifiers.HorizontallyScrollableProperty] = PropertyId.ScrollHorizontallyScrollable,
        [ScrollPatternIdentifiers.VerticallyScrollableProperty] = PropertyId.ScrollVerticallyScrollable,
        [SelectionPatternIdentifiers.SelectionProperty] = PropertyId.SelectionSelection,
        [SelectionPatternIdentifiers.CanSelectMultipleProperty] = PropertyId.SelectionCanSelectMultiple,
        [SelectionPatternIdentifiers.IsSelectionRequiredProperty] = PropertyId.SelectionIsSelectionRequired,
        [SelectionItemPatternIdentifiers.IsSelectedProperty] = PropertyId.SelectionItemIsSelected,
        [SelectionItemPatternIdentifiers.SelectionContainerProperty] = PropertyId.SelectionItemSelectionContainer,
    };

    /// <summary>Maps managed control types to the native Windows identifiers.</summary>
    /// <param name="type">The managed control type.</param>
    /// <returns>The native control type identifier.</returns>
    private static int ControlTypeId(AutomationControlType type) => type switch
    {
        AutomationControlType.None or AutomationControlType.Expander or AutomationControlType.Group => ControlId.Group,
        AutomationControlType.ComboBoxItem or AutomationControlType.ListItem => ControlId.ListItem,
        AutomationControlType.ScrollViewer or AutomationControlType.Pane => ControlId.Pane,
        AutomationControlType.Button => ControlId.Button,
        AutomationControlType.Calendar => ControlId.Calendar,
        AutomationControlType.CheckBox => ControlId.CheckBox,
        AutomationControlType.ComboBox => ControlId.ComboBox,
        AutomationControlType.Edit => ControlId.Edit,
        AutomationControlType.Hyperlink => ControlId.Hyperlink,
        AutomationControlType.Image => ControlId.Image,
        AutomationControlType.List => ControlId.List,
        AutomationControlType.Menu => ControlId.Menu,
        AutomationControlType.MenuBar => ControlId.MenuBar,
        AutomationControlType.MenuItem => ControlId.MenuItem,
        AutomationControlType.ProgressBar => ControlId.ProgressBar,
        AutomationControlType.RadioButton => ControlId.RadioButton,
        AutomationControlType.ScrollBar => ControlId.ScrollBar,
        AutomationControlType.Slider => ControlId.Slider,
        AutomationControlType.Spinner => ControlId.Spinner,
        AutomationControlType.StatusBar => ControlId.StatusBar,
        AutomationControlType.Tab => ControlId.Tab,
        AutomationControlType.TabItem => ControlId.TabItem,
        AutomationControlType.Text => ControlId.Text,
        AutomationControlType.ToolBar => ControlId.ToolBar,
        AutomationControlType.ToolTip => ControlId.ToolTip,
        AutomationControlType.Tree => ControlId.Tree,
        AutomationControlType.TreeItem => ControlId.TreeItem,
        AutomationControlType.Thumb => ControlId.Thumb,
        AutomationControlType.DataGrid => ControlId.DataGrid,
        AutomationControlType.DataItem => ControlId.DataItem,
        AutomationControlType.Document => ControlId.Document,
        AutomationControlType.SplitButton => ControlId.SplitButton,
        AutomationControlType.Window => ControlId.Window,
        AutomationControlType.Header => ControlId.Header,
        AutomationControlType.HeaderItem => ControlId.HeaderItem,
        AutomationControlType.Table => ControlId.Table,
        AutomationControlType.TitleBar => ControlId.TitleBar,
        AutomationControlType.Separator => ControlId.Separator,
        _ => ControlId.Custom,
    };

    /// <summary>Maps managed landmark types to native identifiers.</summary>
    /// <param name="type">The managed landmark type.</param>
    /// <returns>The native landmark type.</returns>
    private static int? LandmarkTypeId(AutomationLandmarkType? type) => type switch
    {
        AutomationLandmarkType.Form => LandmarkId.Form,
        AutomationLandmarkType.Main => LandmarkId.Main,
        AutomationLandmarkType.Navigation => LandmarkId.Navigation,
        AutomationLandmarkType.Search => LandmarkId.Search,
        null => null,
        _ => LandmarkId.Custom,
    };

    /// <summary>Converts native bounds to a property rectangle array.</summary>
    /// <param name="rectangle">The native bounds.</param>
    /// <returns>The four rectangle components.</returns>
    private static double[] RectangleArray(in WindowsAutomationRect rectangle) => [rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height];

    /// <summary>Reads a native property on the UI thread.</summary>
    /// <param name="identifier">The native identifier.</param>
    /// <returns>The property value.</returns>
    private object? ReadProperty(int identifier) => identifier switch
    {
        PropertyId.RuntimeId => GetRuntimeId(),
        PropertyId.BoundingRectangle => RectangleArray(Context.ToScreenRectangle(Peer.GetBoundingRectangle())),
        PropertyId.ProcessId => Environment.ProcessId,
        PropertyId.ControlType => ControlTypeId(Peer.GetAutomationControlType()),
        PropertyId.LocalizedControlType => Peer.GetLocalizedControlType(),
        PropertyId.Name => Peer.GetName(),
        PropertyId.AcceleratorKey => Peer.GetAcceleratorKey(),
        PropertyId.AccessKey => Peer.GetAccessKey(),
        PropertyId.HasKeyboardFocus => Peer.HasKeyboardFocus(),
        PropertyId.IsKeyboardFocusable => Peer.IsKeyboardFocusable(),
        PropertyId.IsEnabled => Peer.IsEnabled(),
        PropertyId.AutomationId => Peer.GetAutomationId(),
        PropertyId.ClassName => Peer.GetClassName(),
        PropertyId.HelpText => Peer.GetHelpText(),
        PropertyId.ClickablePoint => GetClickablePoint(),
        PropertyId.Culture => System.Globalization.CultureInfo.CurrentCulture.LCID,
        PropertyId.IsControlElement => Peer.IsControlElement(),
        PropertyId.IsContentElement => Peer.IsContentElement(),
        PropertyId.LabeledBy => Peer.GetLabeledBy() is { } label ? Context.Node(label) : null,
        PropertyId.IsPassword => Peer is TextBoxAutomationPeer && Peer.GetProvider<IValueProvider>() is null,
        PropertyId.ItemType => Peer.GetItemType(),
        PropertyId.IsOffscreen => Peer.IsOffscreen(),
        PropertyId.FrameworkId => "Avalonia",
        PropertyId.ItemStatus => Peer.GetItemStatus(),
        PropertyId.LiveSetting => (int)Peer.GetLiveSetting(),
        PropertyId.HeadingLevel => Peer.GetHeadingLevel() is >= 1 and <= MaximumHeadingLevel ? HeadingLevelBase + Peer.GetHeadingLevel() : HeadingLevelBase,
        PropertyId.LandmarkType => LandmarkTypeId(Peer.GetLandmarkType()),
        PropertyId.LocalizedLandmarkType => Peer.GetLandmarkType()?.ToString().ToLowerInvariant(),
        _ => ReadPatternProperty(identifier),
    };

    /// <summary>Reads value pattern properties.</summary>
    /// <param name="identifier">The native property identifier.</param>
    /// <returns>The property value.</returns>
    private object? ReadPatternProperty(int identifier) => identifier switch
    {
        PropertyId.ValueValue => Peer.GetProvider<IValueProvider>()?.Value,
        PropertyId.ValueIsReadOnly => Peer.GetProvider<IValueProvider>()?.IsReadOnly,
        PropertyId.RangeValueValue => Peer.GetProvider<IRangeValueProvider>()?.Value,
        PropertyId.RangeValueIsReadOnly => Peer.GetProvider<IRangeValueProvider>()?.IsReadOnly,
        PropertyId.RangeValueMinimum => Peer.GetProvider<IRangeValueProvider>()?.Minimum,
        PropertyId.RangeValueMaximum => Peer.GetProvider<IRangeValueProvider>()?.Maximum,
        PropertyId.RangeValueLargeChange => Peer.GetProvider<IRangeValueProvider>()?.LargeChange,
        PropertyId.RangeValueSmallChange => Peer.GetProvider<IRangeValueProvider>()?.SmallChange,
        _ => ReadScrollProperty(identifier),
    };

    /// <summary>Reads scroll pattern properties.</summary>
    /// <param name="identifier">The native property identifier.</param>
    /// <returns>The property value.</returns>
    private object? ReadScrollProperty(int identifier) => identifier switch
    {
        PropertyId.ScrollHorizontalScrollPercent => Peer.GetProvider<IScrollProvider>()?.HorizontalScrollPercent,
        PropertyId.ScrollHorizontalViewSize => Peer.GetProvider<IScrollProvider>()?.HorizontalViewSize,
        PropertyId.ScrollVerticalScrollPercent => Peer.GetProvider<IScrollProvider>()?.VerticalScrollPercent,
        PropertyId.ScrollVerticalViewSize => Peer.GetProvider<IScrollProvider>()?.VerticalViewSize,
        PropertyId.ScrollHorizontallyScrollable => Peer.GetProvider<IScrollProvider>()?.HorizontallyScrollable,
        PropertyId.ScrollVerticallyScrollable => Peer.GetProvider<IScrollProvider>()?.VerticallyScrollable,
        _ => ReadSelectionProperty(identifier),
    };

    /// <summary>Reads selection pattern properties.</summary>
    /// <param name="identifier">The native property identifier.</param>
    /// <returns>The property value.</returns>
    private object? ReadSelectionProperty(int identifier) => identifier switch
    {
        PropertyId.SelectionSelection => ConvertProperty(Peer.GetProvider<ISelectionProvider>()?.GetSelection()),
        PropertyId.SelectionCanSelectMultiple => Peer.GetProvider<ISelectionProvider>()?.CanSelectMultiple,
        PropertyId.SelectionIsSelectionRequired => Peer.GetProvider<ISelectionProvider>()?.IsSelectionRequired,
        PropertyId.SelectionItemIsSelected => Peer.GetProvider<ISelectionItemProvider>()?.IsSelected,
        PropertyId.SelectionItemSelectionContainer => ConvertProperty(Peer.GetProvider<ISelectionItemProvider>()?.SelectionContainer),
        PropertyId.ExpandCollapseState => ConvertProperty(Peer.GetProvider<IExpandCollapseProvider>()?.ExpandCollapseState),
        PropertyId.ToggleState => ConvertProperty(Peer.GetProvider<IToggleProvider>()?.ToggleState),
        _ => ReadAvailability(identifier),
    };

    /// <summary>Reads pattern availability flags.</summary>
    /// <param name="identifier">The native availability property identifier.</param>
    /// <returns>The availability flag, or null for unrelated properties.</returns>
    private object? ReadAvailability(int identifier) => identifier switch
    {
        AvailabilityId.ExpandCollapse => GetPatternProvider(PatternId.ExpandCollapse) is not null,
        AvailabilityId.Invoke => GetPatternProvider(PatternId.Invoke) is not null,
        AvailabilityId.RangeValue => GetPatternProvider(PatternId.RangeValue) is not null,
        AvailabilityId.Scroll => GetPatternProvider(PatternId.Scroll) is not null,
        AvailabilityId.ScrollItem => GetPatternProvider(PatternId.ScrollItem) is not null,
        AvailabilityId.SelectionItem => GetPatternProvider(PatternId.SelectionItem) is not null,
        AvailabilityId.Selection => GetPatternProvider(PatternId.Selection) is not null,
        AvailabilityId.Text => GetPatternProvider(TextPatternId) is not null,
        AvailabilityId.Toggle => GetPatternProvider(PatternId.Toggle) is not null,
        AvailabilityId.Value => GetPatternProvider(PatternId.Value) is not null,
        _ => null,
    };

    /// <summary>Converts property values to native automation values.</summary>
    /// <param name="value">The managed value.</param>
    /// <returns>The native value.</returns>
    private object? ConvertProperty(object? value) => value switch
    {
        AutomationPeer peer => Context.Node(peer),
        AutomationLandmarkType landmark => LandmarkTypeId(landmark),
        ExpandCollapseState state => (int)state,
        ToggleState state => (int)state,
        global::Avalonia.Rect rectangle => RectangleArray(Context.ToScreenRectangle(rectangle)),
        IReadOnlyList<AutomationPeer> peers => ConvertPeers(peers),
        _ => value,
    };

    /// <summary>Gets the center of the peer's screen bounds.</summary>
    /// <returns>The native clickable point components.</returns>
    private double[] GetClickablePoint()
    {
        const double CenterFactor = 0.5;
        var rectangle = Context.ToScreenRectangle(Peer.GetBoundingRectangle());
        return [rectangle.X + (rectangle.Width * CenterFactor), rectangle.Y + (rectangle.Height * CenterFactor)];
    }

    /// <summary>Converts a peer list to native simple providers.</summary>
    /// <param name="peers">The managed peers.</param>
    /// <returns>The native providers.</returns>
    private IWindowsRawElementProviderSimple[] ConvertPeers(IReadOnlyList<AutomationPeer> peers)
    {
        var result = new IWindowsRawElementProviderSimple[peers.Count];
        for (var index = 0; index < result.Length; index++)
        {
            result[index] = Context.Node(peers[index]);
        }

        return result;
    }

    /// <summary>The native AvailabilityId identifiers.</summary>
    private static class AvailabilityId
    {
        /// <summary>The ExpandCollapse identifier.</summary>
        internal const int ExpandCollapse = 30_028;

        /// <summary>The Invoke identifier.</summary>
        internal const int Invoke = 30_031;

        /// <summary>The RangeValue identifier.</summary>
        internal const int RangeValue = 30_033;

        /// <summary>The Scroll identifier.</summary>
        internal const int Scroll = 30_034;

        /// <summary>The ScrollItem identifier.</summary>
        internal const int ScrollItem = 30_035;

        /// <summary>The SelectionItem identifier.</summary>
        internal const int SelectionItem = 30_036;

        /// <summary>The Selection identifier.</summary>
        internal const int Selection = 30_037;

        /// <summary>The Text identifier.</summary>
        internal const int Text = 30_040;

        /// <summary>The Toggle identifier.</summary>
        internal const int Toggle = 30_041;

        /// <summary>The Value identifier.</summary>
        internal const int Value = 30_043;
    }

    /// <summary>The native PatternId identifiers.</summary>
    private static class PatternId
    {
        /// <summary>The Invoke identifier.</summary>
        internal const int Invoke = 10_000;

        /// <summary>The Selection identifier.</summary>
        internal const int Selection = 10_001;

        /// <summary>The Value identifier.</summary>
        internal const int Value = 10_002;

        /// <summary>The RangeValue identifier.</summary>
        internal const int RangeValue = 10_003;

        /// <summary>The Scroll identifier.</summary>
        internal const int Scroll = 10_004;

        /// <summary>The ExpandCollapse identifier.</summary>
        internal const int ExpandCollapse = 10_005;

        /// <summary>The SelectionItem identifier.</summary>
        internal const int SelectionItem = 10_010;

        /// <summary>The Toggle identifier.</summary>
        internal const int Toggle = 10_015;

        /// <summary>The ScrollItem identifier.</summary>
        internal const int ScrollItem = 10_017;
    }

    /// <summary>The native ControlId identifiers.</summary>
    private static class ControlId
    {
        /// <summary>The Button identifier.</summary>
        internal const int Button = 50_000;

        /// <summary>The Calendar identifier.</summary>
        internal const int Calendar = 50_001;

        /// <summary>The CheckBox identifier.</summary>
        internal const int CheckBox = 50_002;

        /// <summary>The ComboBox identifier.</summary>
        internal const int ComboBox = 50_003;

        /// <summary>The Edit identifier.</summary>
        internal const int Edit = 50_004;

        /// <summary>The Hyperlink identifier.</summary>
        internal const int Hyperlink = 50_005;

        /// <summary>The Image identifier.</summary>
        internal const int Image = 50_006;

        /// <summary>The ListItem identifier.</summary>
        internal const int ListItem = 50_007;

        /// <summary>The List identifier.</summary>
        internal const int List = 50_008;

        /// <summary>The Menu identifier.</summary>
        internal const int Menu = 50_009;

        /// <summary>The MenuBar identifier.</summary>
        internal const int MenuBar = 50_010;

        /// <summary>The MenuItem identifier.</summary>
        internal const int MenuItem = 50_011;

        /// <summary>The ProgressBar identifier.</summary>
        internal const int ProgressBar = 50_012;

        /// <summary>The RadioButton identifier.</summary>
        internal const int RadioButton = 50_013;

        /// <summary>The ScrollBar identifier.</summary>
        internal const int ScrollBar = 50_014;

        /// <summary>The Slider identifier.</summary>
        internal const int Slider = 50_015;

        /// <summary>The Spinner identifier.</summary>
        internal const int Spinner = 50_016;

        /// <summary>The StatusBar identifier.</summary>
        internal const int StatusBar = 50_017;

        /// <summary>The Tab identifier.</summary>
        internal const int Tab = 50_018;

        /// <summary>The TabItem identifier.</summary>
        internal const int TabItem = 50_019;

        /// <summary>The Text identifier.</summary>
        internal const int Text = 50_020;

        /// <summary>The ToolBar identifier.</summary>
        internal const int ToolBar = 50_021;

        /// <summary>The ToolTip identifier.</summary>
        internal const int ToolTip = 50_022;

        /// <summary>The Tree identifier.</summary>
        internal const int Tree = 50_023;

        /// <summary>The TreeItem identifier.</summary>
        internal const int TreeItem = 50_024;

        /// <summary>The Custom identifier.</summary>
        internal const int Custom = 50_025;

        /// <summary>The Group identifier.</summary>
        internal const int Group = 50_026;

        /// <summary>The Thumb identifier.</summary>
        internal const int Thumb = 50_027;

        /// <summary>The DataGrid identifier.</summary>
        internal const int DataGrid = 50_028;

        /// <summary>The DataItem identifier.</summary>
        internal const int DataItem = 50_029;

        /// <summary>The Document identifier.</summary>
        internal const int Document = 50_030;

        /// <summary>The SplitButton identifier.</summary>
        internal const int SplitButton = 50_031;

        /// <summary>The Window identifier.</summary>
        internal const int Window = 50_032;

        /// <summary>The Pane identifier.</summary>
        internal const int Pane = 50_033;

        /// <summary>The Header identifier.</summary>
        internal const int Header = 50_034;

        /// <summary>The HeaderItem identifier.</summary>
        internal const int HeaderItem = 50_035;

        /// <summary>The Table identifier.</summary>
        internal const int Table = 50_036;

        /// <summary>The TitleBar identifier.</summary>
        internal const int TitleBar = 50_037;

        /// <summary>The Separator identifier.</summary>
        internal const int Separator = 50_038;
    }

    /// <summary>The native LandmarkId identifiers.</summary>
    private static class LandmarkId
    {
        /// <summary>The Custom identifier.</summary>
        internal const int Custom = 80_000;

        /// <summary>The Form identifier.</summary>
        internal const int Form = 80_001;

        /// <summary>The Main identifier.</summary>
        internal const int Main = 80_002;

        /// <summary>The Navigation identifier.</summary>
        internal const int Navigation = 80_003;

        /// <summary>The Search identifier.</summary>
        internal const int Search = 80_004;
    }

    /// <summary>The Windows UI Automation property identifiers.</summary>
    private static class PropertyId
    {
        /// <summary>The RuntimeId property.</summary>
        internal const int RuntimeId = 30_000;

        /// <summary>The BoundingRectangle property.</summary>
        internal const int BoundingRectangle = 30_001;

        /// <summary>The ProcessId property.</summary>
        internal const int ProcessId = 30_002;

        /// <summary>The ControlType property.</summary>
        internal const int ControlType = 30_003;

        /// <summary>The LocalizedControlType property.</summary>
        internal const int LocalizedControlType = 30_004;

        /// <summary>The Name property.</summary>
        internal const int Name = 30_005;

        /// <summary>The AcceleratorKey property.</summary>
        internal const int AcceleratorKey = 30_006;

        /// <summary>The AccessKey property.</summary>
        internal const int AccessKey = 30_007;

        /// <summary>The HasKeyboardFocus property.</summary>
        internal const int HasKeyboardFocus = 30_008;

        /// <summary>The IsKeyboardFocusable property.</summary>
        internal const int IsKeyboardFocusable = 30_009;

        /// <summary>The IsEnabled property.</summary>
        internal const int IsEnabled = 30_010;

        /// <summary>The AutomationId property.</summary>
        internal const int AutomationId = 30_011;

        /// <summary>The ClassName property.</summary>
        internal const int ClassName = 30_012;

        /// <summary>The HelpText property.</summary>
        internal const int HelpText = 30_013;

        /// <summary>The ClickablePoint property.</summary>
        internal const int ClickablePoint = 30_014;

        /// <summary>The Culture property.</summary>
        internal const int Culture = 30_015;

        /// <summary>The IsControlElement property.</summary>
        internal const int IsControlElement = 30_016;

        /// <summary>The IsContentElement property.</summary>
        internal const int IsContentElement = 30_017;

        /// <summary>The LabeledBy property.</summary>
        internal const int LabeledBy = 30_018;

        /// <summary>The IsPassword property.</summary>
        internal const int IsPassword = 30_019;

        /// <summary>The ItemType property.</summary>
        internal const int ItemType = 30_021;

        /// <summary>The IsOffscreen property.</summary>
        internal const int IsOffscreen = 30_022;

        /// <summary>The FrameworkId property.</summary>
        internal const int FrameworkId = 30_024;

        /// <summary>The ItemStatus property.</summary>
        internal const int ItemStatus = 30_026;

        /// <summary>The ValueValue property.</summary>
        internal const int ValueValue = 30_045;

        /// <summary>The ValueIsReadOnly property.</summary>
        internal const int ValueIsReadOnly = 30_046;

        /// <summary>The RangeValueValue property.</summary>
        internal const int RangeValueValue = 30_047;

        /// <summary>The RangeValueIsReadOnly property.</summary>
        internal const int RangeValueIsReadOnly = 30_048;

        /// <summary>The RangeValueMinimum property.</summary>
        internal const int RangeValueMinimum = 30_049;

        /// <summary>The RangeValueMaximum property.</summary>
        internal const int RangeValueMaximum = 30_050;

        /// <summary>The RangeValueLargeChange property.</summary>
        internal const int RangeValueLargeChange = 30_051;

        /// <summary>The RangeValueSmallChange property.</summary>
        internal const int RangeValueSmallChange = 30_052;

        /// <summary>The ScrollHorizontalScrollPercent property.</summary>
        internal const int ScrollHorizontalScrollPercent = 30_053;

        /// <summary>The ScrollHorizontalViewSize property.</summary>
        internal const int ScrollHorizontalViewSize = 30_054;

        /// <summary>The ScrollVerticalScrollPercent property.</summary>
        internal const int ScrollVerticalScrollPercent = 30_055;

        /// <summary>The ScrollVerticalViewSize property.</summary>
        internal const int ScrollVerticalViewSize = 30_056;

        /// <summary>The ScrollHorizontallyScrollable property.</summary>
        internal const int ScrollHorizontallyScrollable = 30_057;

        /// <summary>The ScrollVerticallyScrollable property.</summary>
        internal const int ScrollVerticallyScrollable = 30_058;

        /// <summary>The SelectionSelection property.</summary>
        internal const int SelectionSelection = 30_059;

        /// <summary>The SelectionCanSelectMultiple property.</summary>
        internal const int SelectionCanSelectMultiple = 30_060;

        /// <summary>The SelectionIsSelectionRequired property.</summary>
        internal const int SelectionIsSelectionRequired = 30_061;

        /// <summary>The ExpandCollapseState property.</summary>
        internal const int ExpandCollapseState = 30_070;

        /// <summary>The SelectionItemIsSelected property.</summary>
        internal const int SelectionItemIsSelected = 30_079;

        /// <summary>The SelectionItemSelectionContainer property.</summary>
        internal const int SelectionItemSelectionContainer = 30_080;

        /// <summary>The ToggleState property.</summary>
        internal const int ToggleState = 30_086;

        /// <summary>The LiveSetting property.</summary>
        internal const int LiveSetting = 30_135;

        /// <summary>The LandmarkType property.</summary>
        internal const int LandmarkType = 30_157;

        /// <summary>The LocalizedLandmarkType property.</summary>
        internal const int LocalizedLandmarkType = 30_158;

        /// <summary>The HeadingLevel property.</summary>
        internal const int HeadingLevel = 30_173;
    }
}
