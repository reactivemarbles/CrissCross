namespace CrissCross
{
    /// <summary>
    /// IView Model Navigating EventArgs.
    /// </summary>
    /// <seealso cref="AICS.Windows.IViewModelNavigationEventArgs" />
    public interface IViewModelNavigatingEventArgs : IViewModelNavigationEventArgs
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IViewModelNavigatingEventArgs"/> is cancel.
        /// </summary>
        /// <value>
        ///   <c>true</c> if cancel; otherwise, <c>false</c>.
        /// </value>
        bool Cancel { get; set; }
    }
}