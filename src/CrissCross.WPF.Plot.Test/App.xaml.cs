// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;

namespace CrissCross.WPF.Plot.Test;

/// <summary>Interaction logic for App.xaml.</summary>
public partial class App
{
    /// <summary>Initializes a new instance of the <see cref="App"/> class and configures the application to use ReactiveUI with WPF support.</summary>
    /// <remarks>This constructor sets up the application's dependency injection and platform integration
    /// using the ReactiveUI framework for WPF. It should be called once at application startup.</remarks>
    public App() => RxAppBuilder.CreateReactiveUIBuilder().WithWpf().BuildApp();
}
