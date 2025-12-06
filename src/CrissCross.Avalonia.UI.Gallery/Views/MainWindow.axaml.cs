// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using CrissCross.Avalonia.UI.Gallery.ViewModels;
using ReactiveUI;
using Splat;

namespace CrissCross.Avalonia.UI.Gallery.Views;

/// <summary>
/// Main window for the gallery application.
/// </summary>
public partial class MainWindow : NavigationWindow<MainViewModel>
{
    private const string NavHostName = "mainNavHost";

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        // Set the window name for navigation registration BEFORE InitializeComponent
        Name = NavHostName;

        InitializeComponent();

        // Set the DataContext to the MainViewModel
        DataContext = AppLocator.Current.GetService<MainViewModel>();

        // Use WhenActivated to navigate after the window is fully loaded
        // This matches the pattern used in the working CrissCross.Avalonia.Test app
        this.WhenActivated(_ =>
        {
            // Navigate to home page initially
            this.NavigateToView<HomePageViewModel>();
        });
    }

    /// <summary>
    /// Registers the content presenter.
    /// Override to create custom layout with navigation menu alongside the NavigationFrame.
    /// </summary>
    /// <param name="presenter">The presenter.</param>
    /// <returns>A bool indicating whether registration was successful.</returns>
    protected override bool RegisterContentPresenter(ContentPresenter presenter)
    {
        if (presenter == null)
        {
            return false;
        }

        // Create custom layout with navigation menu and navigation frame
        if (presenter.Name == "PART_ContentPresenter" && presenter.Content == null)
        {
            // Create the main layout grid using CrissCross Grid
            var mainGrid = new UI.Controls.Grid();
            mainGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            mainGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));

            // Title Bar using CrissCross Border
            var titleBorder = new UI.Controls.Border
            {
                Background = Brush.Parse("#2D2D30"),
                Padding = new Thickness(16, 12)
            };
            var titleText = new UI.Controls.TextBlock
            {
                Text = "CrissCross Avalonia UI Gallery",
                FontSize = 24,
                FontWeight = FontWeight.Bold,
                Foreground = Brushes.White
            };
            titleBorder.Child = titleText;
            mainGrid.Children.Add(titleBorder);
            Grid.SetRow(titleBorder, 0);

            // Content area grid (navigation menu + content) using CrissCross Grid
            var contentGrid = new UI.Controls.Grid();
            contentGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(250)));
            contentGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            mainGrid.Children.Add(contentGrid);
            Grid.SetRow(contentGrid, 1);

            // Navigation Menu using CrissCross Border
            var navBorder = new UI.Controls.Border
            {
                Background = Brush.Parse("#252526"),
                BorderBrush = Brush.Parse("#3F3F46"),
                BorderThickness = new Thickness(0, 0, 1, 0)
            };
            var navScrollViewer = new ScrollViewer();
            var navStack = new UI.Controls.StackPanel { Margin = new Thickness(8) };

            // Navigation header using CrissCross TextBlock
            var navHeader = new UI.Controls.TextBlock
            {
                Text = "Control Categories",
                FontSize = 16,
                FontWeight = FontWeight.Bold,
                Foreground = Brushes.White,
                Margin = new Thickness(8, 8, 8, 16)
            };
            navStack.Children.Add(navHeader);

            // Home button using CrissCross Button
            AddNavigationButton(navStack, "🏠 Home", "GotoHome");

            // Basic Controls Expander using CrissCross Expander
            var basicExpander = new UI.Controls.Expander
            {
                Header = "Basic Controls",
                IsExpanded = true,
                Margin = new Thickness(0, 8, 0, 8)
            };
            var basicStack = new UI.Controls.StackPanel { Margin = new Thickness(16, 8, 0, 8) };
            AddNavigationButton(basicStack, "Button", "GotoButtons");
            AddNavigationButton(basicStack, "CheckBox", "GotoCheckBox");
            AddNavigationButton(basicStack, "RadioButton", "GotoRadioButton");
            basicExpander.Content = basicStack;
            navStack.Children.Add(basicExpander);

            // Input Controls Expander using CrissCross Expander
            var inputExpander = new UI.Controls.Expander
            {
                Header = "Input Controls",
                IsExpanded = true,
                Margin = new Thickness(0, 0, 0, 8)
            };
            var inputStack = new UI.Controls.StackPanel { Margin = new Thickness(16, 8, 0, 8) };
            AddNavigationButton(inputStack, "TextBox", "GotoInput");
            AddNavigationButton(inputStack, "ComboBox", "GotoComboBox");
            AddNavigationButton(inputStack, "Slider", "GotoSlider");
            inputExpander.Content = inputStack;
            navStack.Children.Add(inputExpander);

            // Date & Time Expander using CrissCross Expander
            var dateExpander = new UI.Controls.Expander
            {
                Header = "Date & Time",
                IsExpanded = true,
                Margin = new Thickness(0, 0, 0, 8)
            };
            var dateStack = new UI.Controls.StackPanel { Margin = new Thickness(16, 8, 0, 8) };
            AddNavigationButton(dateStack, "DatePicker", "GotoDatePicker");
            dateExpander.Content = dateStack;
            navStack.Children.Add(dateExpander);

            // Color Expander using CrissCross Expander
            var colorExpander = new UI.Controls.Expander
            {
                Header = "Color Controls",
                IsExpanded = true,
                Margin = new Thickness(0, 0, 0, 8)
            };
            var colorStack = new UI.Controls.StackPanel { Margin = new Thickness(16, 8, 0, 8) };
            AddNavigationButton(colorStack, "ColorPicker", "GotoColorPicker");
            colorExpander.Content = colorStack;
            navStack.Children.Add(colorExpander);

            // Progress Expander using CrissCross Expander
            var progressExpander = new UI.Controls.Expander
            {
                Header = "Progress",
                Margin = new Thickness(0, 0, 0, 8)
            };
            var progressStack = new UI.Controls.StackPanel { Margin = new Thickness(16, 8, 0, 8) };
            AddNavigationButton(progressStack, "ProgressBar", "GotoProgress");
            progressExpander.Content = progressStack;
            navStack.Children.Add(progressExpander);

            navScrollViewer.Content = navStack;
            navBorder.Child = navScrollViewer;
            contentGrid.Children.Add(navBorder);
            Grid.SetColumn(navBorder, 0);

            // Content Display Area with NavigationFrame from base class using CrissCross Border
            var contentBorder = new UI.Controls.Border
            {
                Background = Brush.Parse("#1E1E1E"),
                Padding = new Thickness(0),
                Child = NavigationFrame
            };
            contentGrid.Children.Add(contentBorder);
            Grid.SetColumn(contentBorder, 1);

            presenter.Content = mainGrid;
        }

        return base.RegisterContentPresenter(presenter);
    }

    private static void AddNavigationButton(UI.Controls.StackPanel stack, string content, string commandBinding)
    {
        var button = new UI.Controls.Button
        {
            Content = content,
            Margin = new Thickness(0, 4),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Left,
            Padding = new Thickness(12, 8)
        };
        button.Bind(Button.CommandProperty, new Binding(commandBinding));
        stack.Children.Add(button);
    }
}
