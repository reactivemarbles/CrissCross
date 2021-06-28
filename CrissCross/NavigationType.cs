namespace CrissCross
{
    /// <summary>
    /// Identifies the types of navigation that are supported.
    /// </summary>
    public enum NavigationType
    {
        /// <summary>
        /// Navigating to new content.
        /// </summary>
        New = 0,

        /// <summary>
        /// Navigating back in the back navigation history.
        /// </summary>
        Back = 1,

        /// <summary>
        /// Reloading the current content.
        /// </summary>
        Refresh = 2
    }
}