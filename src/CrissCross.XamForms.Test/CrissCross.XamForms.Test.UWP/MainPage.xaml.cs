// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace CrissCross.XamForms.Test.UWP
{
    /// <summary>
    /// MainPage.
    /// </summary>
    /// <seealso cref="Xamarin.Forms.Platform.UWP.WindowsPage" />
    public sealed partial class MainPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            LoadApplication(new Test.App());
        }
    }
}
