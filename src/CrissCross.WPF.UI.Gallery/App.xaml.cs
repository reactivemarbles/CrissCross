// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;

namespace CrissCross.WPF.UI.Gallery;

/// <summary>Interaction logic for App.xaml.</summary>
public partial class App
{
    /// <summary>Initializes a new instance of the <see cref="App"/> class.</summary>
    /// <remarks>Configures ReactiveUI dependency injection and WPF integration during application startup.</remarks>
    public App() => RxAppBuilder.CreateReactiveUIBuilder().WithWpf().BuildApp();
}
