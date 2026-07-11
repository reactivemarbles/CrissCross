// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>Represents NavigationView.</summary>
/// <seealso cref="System.Windows.Controls.Control" />
/// <seealso cref="INavigationView" />
[TemplatePart(
    Name = TemplateElementNavigationViewContentPresenter,
    Type = typeof(NavigationViewContentPresenter))]
[TemplatePart(
    Name = TemplateElementMenuItemsItemsControl,
    Type = typeof(System.Windows.Controls.ItemsControl))]
[TemplatePart(
    Name = TemplateElementFooterMenuItemsItemsControl,
    Type = typeof(System.Windows.Controls.ItemsControl))]
[TemplatePart(Name = TemplateElementBackButton, Type = typeof(System.Windows.Controls.Button))]
[TemplatePart(Name = TemplateElementToggleButton, Type = typeof(System.Windows.Controls.Button))]
[TemplatePart(
    Name = TemplateElementAutoSuggestBoxSymbolButton,
    Type = typeof(System.Windows.Controls.Button))]
public partial class NavigationView
{
    /// <summary>Template element represented by the <c>PART_MenuItemsItemsControl</c> name.</summary>
    private const string TemplateElementNavigationViewContentPresenter =
        "PART_NavigationViewContentPresenter";

    /// <summary>Template element represented by the <c>PART_MenuItemsItemsControl</c> name.</summary>
    private const string TemplateElementMenuItemsItemsControl = "PART_MenuItemsItemsControl";

    /// <summary>Template element represented by the <c>PART_FooterMenuItemsItemsControl</c> name.</summary>
    private const string TemplateElementFooterMenuItemsItemsControl = "PART_FooterMenuItemsItemsControl";

    /// <summary>Template element represented by the <c>PART_BackButton</c> name.</summary>
    private const string TemplateElementBackButton = "PART_BackButton";

    /// <summary>Template element represented by the <c>PART_ToggleButton</c> name.</summary>
    private const string TemplateElementToggleButton = "PART_ToggleButton";

    /// <summary>Template element represented by the <c>PART_AutoSuggestBoxSymbolButton</c> name.</summary>
    private const string TemplateElementAutoSuggestBoxSymbolButton = "PART_AutoSuggestBoxSymbolButton";

    /// <summary>Gets or sets the control responsible for rendering the content.</summary>
    protected NavigationViewContentPresenter NavigationViewContentPresenter { get; set; } = null!;

    /// <summary>Gets or sets the menu item control located in the pane.</summary>
    protected System.Windows.Controls.ItemsControl MenuItemsItemsControl { get; set; } = null!;

    /// <summary>Gets or sets the footer menu item control located in the pane.</summary>
    protected System.Windows.Controls.ItemsControl FooterMenuItemsItemsControl { get; set; } = null!;

    /// <summary>Gets or sets the control located at the top of the pane with left arrow icon.</summary>
    protected System.Windows.Controls.Button? BackButton { get; set; }

    /// <summary>Gets or sets the control located at the top of the pane with hamburger icon.</summary>
    protected System.Windows.Controls.Button? ToggleButton { get; set; }

    /// <summary>Gets or sets the control that is visitable if PaneDisplayMode="Left" and in compact state.</summary>
    protected System.Windows.Controls.Button? AutoSuggestBoxSymbolButton { get; set; }

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        NavigationViewContentPresenter = GetTemplateChild<NavigationViewContentPresenter>(
            TemplateElementNavigationViewContentPresenter);
        MenuItemsItemsControl = GetTemplateChild<System.Windows.Controls.ItemsControl>(
            TemplateElementMenuItemsItemsControl);
        FooterMenuItemsItemsControl = GetTemplateChild<System.Windows.Controls.ItemsControl>(
            TemplateElementFooterMenuItemsItemsControl);

        MenuItemsItemsControl.ItemsSource = MenuItems;
        FooterMenuItemsItemsControl.ItemsSource = FooterMenuItems;

        if (NavigationViewContentPresenter is not null)
        {
            NavigationViewContentPresenter.Navigated -= OnNavigationViewContentPresenterNavigated;
            NavigationViewContentPresenter.Navigated += OnNavigationViewContentPresenterNavigated;
        }

        if (
            GetTemplateChild(TemplateElementAutoSuggestBoxSymbolButton)
            is System.Windows.Controls.Button autoSuggestBoxSymbolButton)
        {
            AutoSuggestBoxSymbolButton = autoSuggestBoxSymbolButton;

            AutoSuggestBoxSymbolButton.Click -= AutoSuggestBoxSymbolButtonOnClick;
            AutoSuggestBoxSymbolButton.Click += AutoSuggestBoxSymbolButtonOnClick;
        }

        if (GetTemplateChild(TemplateElementBackButton) is System.Windows.Controls.Button backButton)
        {
            BackButton = backButton;

            BackButton.Click -= OnBackButtonClick;
            BackButton.Click += OnBackButtonClick;
        }

        if (GetTemplateChild(TemplateElementToggleButton) is not System.Windows.Controls.Button toggleButton)
        {
            return;
        }

        ToggleButton = toggleButton;

        ToggleButton.Click -= OnToggleButtonClick;
        ToggleButton.Click += OnToggleButtonClick;
    }

    /// <summary>Gets the template child.</summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="name">The name.</param>
    /// <returns>An instance of T.</returns>
    protected T GetTemplateChild<T>(string name)
        where T : DependencyObject
    {
        if (GetTemplateChild(name) is not T dependencyObject)
        {
            throw new ArgumentNullException(name);
        }

        return dependencyObject;
    }
}
