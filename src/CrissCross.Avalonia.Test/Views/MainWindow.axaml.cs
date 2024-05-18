// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using ReactiveUI;

namespace CrissCross.Avalonia.Test.Views;

/// <summary>
/// MainWindow.
/// </summary>
/// <seealso cref="Window" />
public partial class MainWindow : NavigationWindow<MainWindowViewModel>
{
    private Button? _NavBack;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        this.WhenActivated(d =>
        {
            this.NavigateToView<MainViewModel>();
            if (_NavBack != null)
            {
                _NavBack.Command = ReactiveCommand.Create(() => this.NavigateBack(), this.CanNavigateBack()).DisposeWith(d);
            }
        });
    }

    /// <summary>
    /// Registers the content presenter.
    /// </summary>
    /// <param name="presenter">The presenter.</param>
    /// <returns>
    /// A bool.
    /// </returns>
    protected override bool RegisterContentPresenter(ContentPresenter presenter)
    {
        if (presenter == null)
        {
            return false;
        }

        // Override the default content presenter with a grid containing a back button and the navigation frame
        if (presenter.Name == "PART_ContentPresenter" && presenter.Content == null)
        {
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            _NavBack = new Button
            {
                Content = "Back",
                Name = "NavBack",
                Height = 30
            };
            grid.Children.Add(_NavBack);
            Grid.SetColumn(_NavBack, 0);
            grid.Children.Add(NavigationFrame!);
            Grid.SetColumn(NavigationFrame!, 1);
            presenter.Content = grid;
        }

        return base.RegisterContentPresenter(presenter);
    }
}
