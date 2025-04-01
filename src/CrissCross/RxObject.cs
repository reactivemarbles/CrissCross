// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using ReactiveUI;

namespace CrissCross;

/// <summary>
/// Rx Object.
/// </summary>
/// <seealso cref="ReactiveUI.ReactiveObject" />
/// <seealso cref="CrissCross.IRxObject" />
/// <seealso cref="ReactiveObject" />
/// <seealso cref="IRxObject" />
#if NET8_0_OR_GREATER
[RequiresDynamicCode("The method uses reflection and will not work in AOT environments.")]
[RequiresUnreferencedCode("The method uses reflection and will not work in AOT environments.")]
#endif
public abstract class RxObject : ReactiveObject, IRxObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RxObject"/> class.
    /// </summary>
    protected RxObject()
    {
    }

    /// <summary>
    /// Gets the URL path segment.
    /// </summary>
    /// <value>
    /// The URL path segment.
    /// </value>
    public string? Name => GetType().FullName;

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    /// <value>
    /// The display name.
    /// </value>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Gets a value indicating whether this instance is disposed.
    /// </summary>
    /// <value><c>true</c> if this instance is disposed; otherwise, <c>false</c>.</value>
    public bool IsDisposed => Disposables.IsDisposed;

    /// <summary>
    /// Gets the disposables.
    /// </summary>
    /// <value>
    /// The disposables.
    /// </value>
    protected CompositeDisposable Disposables { get; } = [];

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting
    /// unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public virtual void WhenNavigatedFrom(IViewModelNavigationEventArgs e)
    {
    }

    /// <inheritdoc/>
    public virtual void WhenNavigatedTo(IViewModelNavigationEventArgs e, CompositeDisposable disposables)
    {
    }

    /// <inheritdoc/>
    public virtual void WhenNavigating(IViewModelNavigatingEventArgs e)
    {
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing">
    /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
    /// unmanaged resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (Disposables?.IsDisposed == false && disposing)
        {
            Disposables?.Dispose();
        }
    }
}
