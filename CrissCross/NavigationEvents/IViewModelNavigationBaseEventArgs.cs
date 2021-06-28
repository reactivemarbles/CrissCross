namespace CrissCross
{
    /// <summary>
    /// IView Model Navigation Base Event Args.
    /// </summary>
    public interface IViewModelNavigationBaseEventArgs
    {
        /// <summary>
        /// Gets from.
        /// </summary>
        /// <value>
        /// From.
        /// </value>
        IRxObject? From { get; }

        /// <summary>
        /// Gets the navigation parameter.
        /// </summary>
        /// <value>
        /// The navigation parameter.
        /// </value>
        object? NavigationParameter { get; }

        /// <summary>
        /// Gets to.
        /// </summary>
        /// <value>
        /// To.
        /// </value>
        IRxObject? To { get; }
    }
}