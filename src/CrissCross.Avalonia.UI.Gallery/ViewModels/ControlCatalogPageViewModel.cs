// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace CrissCross.Avalonia.UI.Gallery.ViewModels;

/// <summary>View model for the data-driven control and host coverage catalog.</summary>
public sealed class ControlCatalogPageViewModel : RxObject
{
    /// <summary>Catalog page label used by standalone-control entries.</summary>
    private const string CatalogExample = "Control catalog";

    /// <summary>Workflow page label used by composed workflow-control entries.</summary>
    private const string WorkflowExample = "Workflow & Feedback";

    /// <summary>Application navigation-host label used by navigation-control entries.</summary>
    private const string NavigationHostExample = "MainWindow navigation shell";

    /// <summary>Application window-host label used by dialog and chrome-control entries.</summary>
    private const string ApplicationHostExample = "MainWindow application host";

    /// <summary>Initializes a new instance of the <see cref="ControlCatalogPageViewModel"/> class.</summary>
    public ControlCatalogPageViewModel() => DisplayName = CatalogExample;

    /// <summary>Gets the control-family coverage entries that require a composed or application-level example.</summary>
    public IReadOnlyList<ControlCoverageItem> Coverage { get; } =
    [
        new("Alarms", WorkflowExample, "Composed alarm state in the workflow feedback sample."),
        new("Anchor", CatalogExample, "Standalone themed hyperlink primitive."),
        new("AppBar", NavigationHostExample, "Application command surface hosted by the gallery window."),
        new("Arc", CatalogExample, "Standalone vector primitive used by gauge-style visuals."),
        new("BreadcrumbBar", CatalogExample, "Navigation breadcrumb composition example."),
        new("ChipGroup", WorkflowExample, "Composes the Chip examples into a selectable group."),
        new("ContentDialog", ApplicationHostExample, "Requires the top-level window and the content-dialog service."),
        new("ContextMenu", CatalogExample, "Attached-menu host example; requires an owning control."),
        new("DataFilterPanel", "Reactive Feature Playground", "Shares reactive query state with FilterBar and DataPager."),
        new("DataGrid", CatalogExample, "Collection host for tabular data templates."),
        new("DynamicScrollBar", CatalogExample, "Scroll infrastructure hosted by a scroll viewer."),
        new("DynamicScrollViewer", CatalogExample, "Scroll infrastructure hosted by catalog content."),
        new("EmptyState", CatalogExample, "Standalone empty-content presentation."),
        new("Expander", CatalogExample, "Standalone expandable content presentation."),
        new("Flyout", CatalogExample, "Attached flyout host example; requires an owning control."),
        new("Frame", NavigationHostExample, "Navigation content host used by the gallery shell."),
        new("Gauges", "Progress", "CircularGauge sample exercises the shared gauge theme."),
        new("GifImage", CatalogExample, "Image-derived control hosted by the catalog."),
        new("GridView", CatalogExample, "Grid-oriented list host example."),
        new("GroupBox", CatalogExample, "Standalone labelled content group."),
        new("IconElement", "Buttons", "AppBarButton and SymbolIcon provide the concrete icon host."),
        new("IconSource", "Buttons", "Icon source is consumed by the AppBarButton icon host."),
        new("Image", CatalogExample, "Standalone image presentation."),
        new("ItemsControl", CatalogExample, "Collection host used by the catalog itself."),
        new("Label", CatalogExample, "Standalone text label presentation."),
        new("ListBox", CatalogExample, "Selectable item collection example."),
        new("ListView", CatalogExample, "View-state-aware item collection example."),
        new("LoadingScreen", "Progress", "Overlay sample in the progress page."),
        new("Menu", CatalogExample, "Menu composition with MenuItem."),
        new("MessageBox", ApplicationHostExample, "Window-backed dialog; must be owned and shown by the top-level host."),
        new("MessageBoxAsync", ApplicationHostExample, "Async window-backed dialog service owned by the top-level host."),
        new("NavigationControls", NavigationHostExample, "Back/forward controls require a navigation host."),
        new("NavigationUserControl", NavigationHostExample, "Navigation-aware content is activated inside the navigation host."),
        new("NavigationView", NavigationHostExample, "Navigation host/control requires application-level routing."),
        new("NumericPushButton", CatalogExample, "Standalone numeric command button."),
        new("Page", NavigationHostExample, "Page lifecycle requires a navigation host."),
        new("PropertyGridLite", CatalogExample, "Property inspection host for supplied property rows."),
        new("ScrollBar", CatalogExample, "Scroll infrastructure hosted by a scroll viewer."),
        new("ScrollViewer", CatalogExample, "Hosts the catalog’s vertically scrollable content."),
        new("StatusBar", CatalogExample, "Standalone status content layout."),
        new("TabControl", CatalogExample, "Tabbed content host example."),
        new("TabView", CatalogExample, "Fluent tabbed content host example."),
        new("TitleBar", ApplicationHostExample, "Top-level title chrome requires a window host."),
        new("ToolBar", CatalogExample, "Standalone command-strip layout."),
        new("ToolTip", CatalogExample, "Attached tooltip host example; requires an owning control."),
        new("TreeGrid", CatalogExample, "Hierarchical tabular data host."),
        new("TreeView", CatalogExample, "Hierarchical item collection host."),
        new("ValidationSummary", WorkflowExample, "Form validation presentation companion to ReactiveFormField."),
        new("VirtualizingGridView", CatalogExample, "Virtualized grid collection host."),
        new("VirtualizingItemsControl", CatalogExample, "Virtualized item collection host."),
        new("VirtualizingWrapPanel", CatalogExample, "Virtualizing layout panel hosted by a collection."),
    ];
}
