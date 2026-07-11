// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Avalonia.UI.Controls;

namespace CrissCross.Avalonia.UI;

/// <summary>A service that provides methods related to displaying the <see cref="Snackbar"/>.</summary>
public class SnackbarService : ISnackbarService
{
    /// <summary>Provides the _presenter member.</summary>
    private SnackbarPresenter? _presenter;

    /// <summary>Provides the _snackbar member.</summary>
    private Snackbar? _snackbar;

    /// <inheritdoc />
    public TimeSpan DefaultTimeOut { get; set; } = TimeSpan.FromSeconds(5);

    /// <inheritdoc />
    public void SetSnackbarPresenter(SnackbarPresenter contentPresenter) => _presenter = contentPresenter;

    /// <inheritdoc />
    public SnackbarPresenter GetSnackbarPresenter() =>
        _presenter ?? throw new InvalidOperationException("The SnackbarPresenter has not been set.");

    /// <inheritdoc />
    public void Show(
        string title,
        string message,
        ControlAppearance appearance,
        IconElement? icon,
        TimeSpan timeout)
    {
        if (_presenter is null)
        {
            throw new ArgumentNullException("The SnackbarPresenter didn't set previously.");
        }

        _snackbar ??= new Snackbar(_presenter);

        _snackbar.Title = title;
        _snackbar.Content = message;
        _snackbar.Appearance = appearance;
        _snackbar.Icon = icon;
        _snackbar.Timeout = timeout.TotalSeconds == 0 ? DefaultTimeOut : timeout;

        _snackbar.Show(true);
    }
}
