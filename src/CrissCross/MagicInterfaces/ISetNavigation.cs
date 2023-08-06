// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace CrissCross
{
    /// <summary>
    /// Enables setting of ViewModelRoutedViewHost.
    /// </summary>
    public interface ISetNavigation
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string? Name { get; }
    }
}
