// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using CrissCross.Avalonia.UI.Gallery.ViewModels;
using Splat;

namespace CrissCross.Avalonia.UI.Gallery.Views;

/// <summary>Main window for the gallery application.</summary>
public partial class MainWindow : NavigationWindow<MainViewModel>
{
    /// <summary>Provides the NavHostName member.</summary>
    private const string NavHostName = "mainNavHost";

    /// <summary>Width of the navigation pane.</summary>
    private const double NavigationPaneWidth = 250;

    /// <summary>Large title font size.</summary>
    private const double TitleFontSize = 24;

    /// <summary>Navigation section header font size.</summary>
    private const double NavigationHeaderFontSize = 16;

    /// <summary>Large horizontal padding for title and buttons.</summary>
    private const double LargeHorizontalSpacing = 16;

    /// <summary>Medium spacing used by the navigation layout.</summary>
    private const double MediumSpacing = 8;

    /// <summary>Small spacing used by compact navigation buttons.</summary>
    private const double SmallSpacing = 4;

    /// <summary>Right border thickness for the navigation pane.</summary>
    private const double NavigationBorderThickness = 1;

    /// <summary>Title bar vertical padding.</summary>
    private const double TitleVerticalPadding = 12;

    /// <summary>Initializes a new instance of the <see cref="MainWindow"/> class.</summary>
    public MainWindow()
    {
        // Set the window name for navigation registration BEFORE InitializeComponent
        Name = NavHostName;

        InitializeComponent();

        if (NavigationFrame is not null)
        {
            NavigationFrame.Name = NavHostName;
            NavigationFrame.HostName = NavHostName;
            this.SetMainNavigationHost(NavigationFrame);
        }

        // Set the DataContext to the MainViewModel
        DataContext = AppLocator.Current.GetService<MainViewModel>();
    }

    /// <inheritdoc/>
    protected override void OnOpened(System.EventArgs e)
    {
        base.OnOpened(e);
        _ = this.WhenSetup()
            .Where(x => x)
            .Take(1)
            .Subscribe(_ => this.NavigateToView(new NavigationKeyRequest<HomePageViewModel>()));
    }

    /// <summary>Registers the content presenter for the navigation layout.</summary>
    /// <param name="presenter">The presenter.</param>
    /// <returns>A bool indicating whether registration was successful.</returns>
    protected override bool RegisterContentPresenter(ContentPresenter presenter)
    {
        if (presenter is null)
        {
            return false;
        }

        // Create custom layout with navigation menu and navigation frame
        if (presenter.Name == "PART_ContentPresenter" && presenter.Content is null)
        {
            if (NavigationFrame is not null)
            {
                NavigationFrame.Name = NavHostName;
                NavigationFrame.HostName = NavHostName;
                this.SetMainNavigationHost(NavigationFrame);
            }

            // Create the main layout grid using CrissCross Grid
            var mainGrid = new UI.Controls.Grid();
            mainGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            mainGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));

            // Title Bar using CrissCross Border
            var titleBorder = new UI.Controls.Border { Padding = new(LargeHorizontalSpacing, TitleVerticalPadding) };
            titleBorder.Classes.Add("gallery-title");
            var titleText = new UI.Controls.TextBlock
            {
                Text = "CrissCross Avalonia UI Gallery",
                FontSize = TitleFontSize,
                FontWeight = FontWeight.Bold,
            };
            titleText.Classes.Add("gallery-shell-text");
            titleBorder.Child = titleText;
            mainGrid.Children.Add(titleBorder);
            Grid.SetRow(titleBorder, 0);

            // Content area grid (navigation menu + content) using CrissCross Grid
            var contentGrid = new UI.Controls.Grid();
            contentGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(NavigationPaneWidth)));
            contentGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            mainGrid.Children.Add(contentGrid);
            Grid.SetRow(contentGrid, 1);

            var navBorder = CreateNavigationBorder();
            contentGrid.Children.Add(navBorder);
            Grid.SetColumn(navBorder, 0);

            // Content Display Area with NavigationFrame from base class using CrissCross Border
            var contentBorder = new UI.Controls.Border { Padding = new(0), Child = NavigationFrame };
            contentBorder.Classes.Add("gallery-content");
            contentGrid.Children.Add(contentBorder);
            Grid.SetColumn(contentBorder, 1);

            presenter.Content = mainGrid;
        }

        return base.RegisterContentPresenter(presenter);
    }

    /// <summary>Creates the gallery navigation pane.</summary>
    /// <returns>The navigation pane border.</returns>
    private static UI.Controls.Border CreateNavigationBorder()
    {
        var navStack = new UI.Controls.StackPanel { Margin = new(MediumSpacing) };
        var navHeader = new UI.Controls.TextBlock
        {
            Text = "Control Categories",
            FontSize = NavigationHeaderFontSize,
            FontWeight = FontWeight.Bold,
            Margin = new(MediumSpacing, MediumSpacing, MediumSpacing, LargeHorizontalSpacing),
        };
        navHeader.Classes.Add("gallery-shell-text");
        navStack.Children.Add(navHeader);
        AddNavigationButton(navStack, "🏠 Home", "GotoHome");
        navStack.Children.Add(
            CreateNavigationExpander(
                "Basic Controls",
                includeTopMargin: true,
                [("Button", "GotoButtons"), ("CheckBox", "GotoCheckBox"), ("RadioButton", "GotoRadioButton")]));
        navStack.Children.Add(
            CreateNavigationExpander(
                "Input Controls",
                includeTopMargin: false,
                [("TextBox", "GotoInput"), ("ComboBox", "GotoComboBox"), ("Slider", "GotoSlider")]));
        navStack.Children.Add(
            CreateNavigationExpander("Date & Time", includeTopMargin: false, [("DatePicker", "GotoDatePicker")]));
        navStack.Children.Add(
            CreateNavigationExpander("Color Controls", includeTopMargin: false, [("ColorPicker", "GotoColorPicker")]));
        navStack.Children.Add(
            CreateNavigationExpander("Progress", includeTopMargin: false, [("ProgressBar", "GotoProgress")]));
        AddNavigationButton(navStack, "✨ Reactive Feature Playground", "GotoFeaturePlayground");

        var border = new UI.Controls.Border
        {
            BorderThickness = new(0, 0, NavigationBorderThickness, 0),
            Child = new ScrollViewer { Content = navStack },
        };
        border.Classes.Add("gallery-navigation");
        return border;
    }

    /// <summary>Creates a navigation category and its bound navigation buttons.</summary>
    /// <param name="header">The category header.</param>
    /// <param name="includeTopMargin">Whether to include spacing above the category.</param>
    /// <param name="items">The button labels and command bindings.</param>
    /// <returns>The configured navigation expander.</returns>
    private static UI.Controls.Expander CreateNavigationExpander(
        string header,
        bool includeTopMargin,
        (string Content, string CommandBinding)[] items)
    {
        var stack = new UI.Controls.StackPanel
        {
            Margin = new(LargeHorizontalSpacing, MediumSpacing, 0, MediumSpacing),
        };
        foreach (var item in items)
        {
            AddNavigationButton(stack, item.Content, item.CommandBinding);
        }

        return new()
        {
            Header = header,
            IsExpanded = true,
            Margin = new(0, includeTopMargin ? MediumSpacing : 0, 0, MediumSpacing),
            Content = stack,
        };
    }

    /// <summary>Provides the AddNavigationButton member.</summary>
    /// <param name="stack">The stack value.</param>
    /// <param name="content">The content value.</param>
    /// <param name="commandBinding">The commandBinding value.</param>
    private static void AddNavigationButton(UI.Controls.StackPanel stack, string content, string commandBinding)
    {
        var button = new UI.Controls.Button
        {
            Content = content,
            Margin = new(0, SmallSpacing),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Left,
            Padding = new(TitleVerticalPadding, MediumSpacing),
        };
        _ = button.Bind(Button.CommandProperty, new Binding(commandBinding));
        stack.Children.Add(button);
    }
}
