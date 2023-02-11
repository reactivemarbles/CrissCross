// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace CrissCross.MAUI.Test
{
    /// <summary>
    /// MainPage.
    /// </summary>
    /// <seealso cref="Microsoft.Maui.Controls.ContentPage" />
    public partial class MainPage
    {
        private int _count;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            _count++;

            if (_count == 1)
            {
                CounterBtn.Text = $"Clicked {_count} time";
            }
            else
            {
                CounterBtn.Text = $"Clicked {_count} times";
            }

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }
}
