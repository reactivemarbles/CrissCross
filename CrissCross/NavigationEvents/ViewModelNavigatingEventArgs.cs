﻿using ReactiveUI;
using System.Runtime.Serialization;

namespace CrissCross
{
    /// <summary>
    /// View Model Navigating Event Args.
    /// </summary>
    /// <seealso cref="AICS.Windows.ViewModelNavigationEventArgs" />
    /// <seealso cref="AICS.Windows.IViewModelNavigatingEventArgs" />
    [DataContract]
    public class ViewModelNavigatingEventArgs : ViewModelNavigationEventArgs, IViewModelNavigatingEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelNavigatingEventArgs" /> class.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="navType">Type of the nav.</param>
        /// <param name="view">The view.</param>
        /// <param name="hostName">The hostName.</param>
        /// <param name="parmeter">The parmeter.</param>
        public ViewModelNavigatingEventArgs(IRxObject? from, IRxObject? to, NavigationType navType, IViewFor? view, string hostName, object? parmeter = null)
            : base(from, to, navType, view, hostName, parmeter)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ViewModelNavigatingEventArgs"/>
        /// is canceled.
        /// </summary>
        /// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
        [DataMember]
        public bool Cancel { get; set; }
    }
}