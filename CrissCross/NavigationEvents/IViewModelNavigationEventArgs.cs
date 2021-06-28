using ReactiveUI;

namespace CrissCross
{
    /// <summary>
    /// I View Model Navigation EventArgs.
    /// </summary>
    /// <seealso cref="AICS.Windows.IViewModelNavigationBaseEventArgs" />
    public interface IViewModelNavigationEventArgs : IViewModelNavigationBaseEventArgs
    {
        /// <summary>
        /// Gets the type of the navigation.
        /// </summary>
        /// <value>
        /// The type of the navigation.
        /// </value>
        NavigationType NavigationType { get; }

        /// <summary>
        /// Gets or sets the view.
        /// </summary>
        /// <value>
        /// The view.
        /// </value>
        IViewFor? View { get; set; }

        /// <summary>
        /// Gets or sets the name of the host.
        /// </summary>
        /// <value>
        /// The name of the host.
        /// </value>
        string HostName { get; set; }
    }
}