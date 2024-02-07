// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Configuration
{
    /// <summary>
    /// Event args for a tracking operation. Enables the handler to cancel the operation and modify the data that will be persisted/applied.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="PropertyOperationData"/> class.
    /// Creates a new instance of PropertyData.
    /// </remarks>
    /// <param name="property">The property that is being persisted or applied to.</param>
    /// <param name="value">The value that is being persited or applied.</param>
    public class PropertyOperationData(string property, object? value)
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PropertyOperationData"/> is cancel.
        /// </summary>
        /// <value>
        ///   <c>true</c> if cancel; otherwise, <c>false</c>.
        /// </value>
        public bool Cancel { get; set; }

        /// <summary>
        /// Gets the property that is being persisted or applied to.
        /// </summary>
        /// <value>
        /// The property.
        /// </value>
        public string Property { get; } = property;

        /// <summary>
        /// Gets or sets the value that is being persited or applied. Has a setter to support converting/mapping/limiting values when applying/persisting.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public object? Value { get; set; } = value;
    }
}
