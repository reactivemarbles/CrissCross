using ReactiveUI;
using System.Reactive.Disposables;

namespace CrissCross
{
    /// <summary>
    /// INotifiy Routable ViewModel.
    /// </summary>
    /// <seealso cref="IRoutableViewModel" />
    /// <seealso cref="IUseHostedNavigation" />
    public interface INotifiyRoutableViewModel : IReactiveObject, IUseHostedNavigation
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string? Name { get; }

        /// <summary>
        /// Raises the <see cref="E:NavigatedFrom"/> event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="IViewModelNavigationEventArgs"/> instance containing the event data.
        /// </param>
        void WhenNavigatedFrom(IViewModelNavigationEventArgs e);

        /// <summary>
        /// Raises the <see cref="E:NavigatedTo"/> event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="IViewModelNavigationEventArgs"/> instance containing the event data.
        /// </param>
        /// <param name="disposables">The disposables.</param>
        void WhenNavigatedTo(IViewModelNavigationEventArgs e, CompositeDisposable disposables);

        /// <summary>
        /// Raises the <see cref="E:Navigating"/> event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="IViewModelNavigatingEventArgs"/> instance containing the event data.
        /// </param>
        void WhenNavigating(IViewModelNavigatingEventArgs e);
    }
}