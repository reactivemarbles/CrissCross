// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI
{
    /// <summary>
    /// Single Assign Single Assign.
    /// </summary>
    /// <typeparam name="T">The Type.</typeparam>
    public class SingleAssign<T>
    {
        private bool _assigned;

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public T? Value { get; private set; }

        /// <summary>
        /// Assigns the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Assign(T? value)
        {
            if (_assigned)
            {
                return;
            }

            Value = value;
            _assigned = true;
        }
    }
}
