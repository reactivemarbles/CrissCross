// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Avalonia.UI.Controls;

namespace CrissCross.Avalonia.UI;

/// <summary>
/// A service that provides methods related to displaying the <see cref="Snackbar"/>.
/// </summary>
public class SnackbarService : ISnackbarService
{
    private SnackbarPresenter? _presenter;

    private Snackbar? _snackbar;

    /// <inheritdoc />
    public TimeSpan DefaultTimeOut { get; set; } = TimeSpan.FromSeconds(5);

    /// <inheritdoc />
    public void SetSnackbarPresenter(SnackbarPresenter contentPresenter) => _presenter = contentPresenter;

    /// <inheritdoc />
    public SnackbarPresenter GetSnackbarPresenter()
    {
        if (_presenter is null)
        {
            throw new ArgumentNullException("The SnackbarPresenter didn't set previously.");
        }

        return _presenter;
    }

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
