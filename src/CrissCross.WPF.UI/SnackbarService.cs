// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>A service that provides methods related to displaying the <see cref="Snackbar"/>.</summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class SnackbarService : ISnackbarService
{
    /// <summary>Provides the default snackbar timeout in seconds.</summary>
    private const double DefaultTimeoutSeconds = 5D;

    /// <summary>Stores the _presenter value.</summary>
    private SnackbarPresenter? _presenter;

    /// <summary>Stores the _snackbar value.</summary>
    private Snackbar? _snackbar;

    /// <inheritdoc />
    public TimeSpan DefaultTimeOut { get; set; } = TimeSpan.FromSeconds(DefaultTimeoutSeconds);

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;

    /// <inheritdoc />
    public void SetSnackbarPresenter(SnackbarPresenter contentPresenter) => _presenter = contentPresenter;

    /// <inheritdoc />
    public SnackbarPresenter GetSnackbarPresenter() =>
        _presenter ?? throw new InvalidOperationException("The SnackbarPresenter was not set previously.");

    /// <inheritdoc />
    public void Show(string title, string message, ControlAppearance appearance, IconElement? icon, TimeSpan timeout)
    {
        var presenter =
            _presenter ?? throw new InvalidOperationException("The SnackbarPresenter was not set previously.");

        _snackbar ??= new Snackbar(presenter);

        _snackbar.SetCurrentValue(Snackbar.TitleProperty, title);
        _snackbar.SetCurrentValue(System.Windows.Controls.ContentControl.ContentProperty, message);
        _snackbar.SetCurrentValue(Snackbar.AppearanceProperty, appearance);
        _snackbar.SetCurrentValue(Snackbar.IconProperty, icon);
        _snackbar.SetCurrentValue(Snackbar.TimeoutProperty, timeout.TotalSeconds == 0 ? DefaultTimeOut : timeout);

        _snackbar.Show(true);
    }
}
