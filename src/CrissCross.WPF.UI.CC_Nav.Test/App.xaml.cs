// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using ReactiveUI.Builder;

namespace CrissCross.WPF.UI.CC_Nav.Test
{
    /// <summary>
    /// Interaction logic for App.xaml.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class and configures the application to use ReactiveUI with WPF.
        /// </summary>
        /// <remarks>This constructor sets up the application's reactive infrastructure by integrating
        /// ReactiveUI with the WPF platform. Use this constructor when creating a WPF application that leverages
        /// ReactiveUI for MVVM and reactive programming patterns.</remarks>
        public App() => RxAppBuilder.CreateReactiveUIBuilder().WithWpf().BuildApp();
    }
}
