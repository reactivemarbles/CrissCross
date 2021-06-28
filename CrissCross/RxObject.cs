using ReactiveUI;
using System;
using System.Reactive.Disposables;

namespace CrissCross
{
    /// <summary>
    /// Rx Object.
    /// </summary>
    /// <seealso cref="ReactiveObject" />
    /// <seealso cref="IRxObject" />
    public abstract class RxObject : ReactiveObject, IRxObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RxObject"/> class.
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        protected RxObject()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RxObject"/> class.
        /// </summary>
        /// <param name="hostScreen">The host screen.</param>
        protected RxObject(IScreen? hostScreen = null)
        {
            HostScreen = hostScreen!;
        }

        /// <summary>
        /// Gets the disposables.
        /// </summary>
        /// <value>
        /// The disposables.
        /// </value>
        protected CompositeDisposable Disposables { get; } = new CompositeDisposable();

        /// <summary>
        /// Gets the URL path segment.
        /// </summary>
        /// <value>
        /// The URL path segment.
        /// </value>
        public string? Name => GetType().FullName;

        /// <summary>
        /// Gets the host screen.
        /// </summary>
        /// <value>
        /// The host screen.
        /// </value>
        public IScreen HostScreen { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value><c>true</c> if this instance is disposed; otherwise, <c>false</c>.</value>
        public bool IsDisposed => Disposables.IsDisposed;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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

        public virtual void WhenNavigatedFrom(IViewModelNavigationEventArgs e)
        {
        }

        public virtual void WhenNavigatedTo(IViewModelNavigationEventArgs e, CompositeDisposable disposables)
        {
        }

        public virtual void WhenNavigating(IViewModelNavigatingEventArgs e)
        {
        }
    }
}