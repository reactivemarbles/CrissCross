// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;
using NavigationControl = CrissCross.Avalonia.NavigationUserControl;
using NavigationWindow = CrissCross.Avalonia.NavigationWindow;

namespace CrissCross.NavigationView.Tests;

/// <summary>Verifies XAML property ownership is shared across closed generic navigation types.</summary>
[TUnit.Core.Executors.TestExecutor<AvaloniaUiTestExecutor>]
public sealed class AvaloniaViewModelPropertyTests
{
    /// <summary>Verifies independent generic controls share one non-generic property registration.</summary>
    /// <returns>The asynchronous test operation.</returns>
    [Test]
    public async Task GenericControlsUseNonGenericPropertyOwner()
    {
        var first = new CrissCross.Avalonia.NavigationUserControl<FirstViewModel>();
        var second = new CrissCross.Avalonia.NavigationUserControl<SecondViewModel>();
        var firstModel = new FirstViewModel();
        var secondModel = new SecondViewModel();
        first.ViewModel = firstModel;
        _ = second.SetValue(NavigationControl.ViewModelProperty, secondModel);

        await Assert.That(NavigationControl.ViewModelProperty.OwnerType).IsEqualTo(typeof(NavigationControl));
        await Assert.That(ReferenceEquals(first.GetValue(NavigationControl.ViewModelProperty), firstModel)).IsTrue();
        await Assert.That(ReferenceEquals(second.ViewModel, secondModel)).IsTrue();
        await Assert.That(ReferenceEquals(((IViewFor)first).ViewModel, firstModel)).IsTrue();
        ((IViewFor)first).ViewModel = null;
        await Assert.That(first.ViewModel).IsNull();
        await Assert.That(ReferenceEquals(second.ViewModel, secondModel)).IsTrue();
    }

    /// <summary>Verifies different generic window types register one XAML-addressable property.</summary>
    /// <returns>The asynchronous test operation.</returns>
    [Test]
    public Task GenericWindowsUseNonGenericPropertyOwner() => AvaloniaTestUiThread.RunAsync(static async () =>
    {
        var first = new CrissCross.Avalonia.NavigationWindow<FirstViewModel>();
        var second = new CrissCross.Avalonia.NavigationWindow<SecondViewModel>();
        var firstModel = new FirstViewModel();
        var secondModel = new SecondViewModel();
        first.ViewModel = firstModel;
        _ = second.SetValue(NavigationWindow.ViewModelProperty, secondModel);

        await Assert.That(NavigationWindow.ViewModelProperty.OwnerType).IsEqualTo(typeof(NavigationWindow));
        await Assert.That(ReferenceEquals(first.GetValue(NavigationWindow.ViewModelProperty), firstModel)).IsTrue();
        await Assert.That(ReferenceEquals(second.ViewModel, secondModel)).IsTrue();
        await Assert.That(ReferenceEquals(((IViewFor)first).ViewModel, firstModel)).IsTrue();
        ((IViewFor)first).ViewModel = null;
        await Assert.That(first.ViewModel).IsNull();
        first.Close();
        second.Close();
    });

    /// <summary>Provides the first strongly typed navigation model.</summary>
    public sealed class FirstViewModel : RxObject;

    /// <summary>Provides the second strongly typed navigation model.</summary>
    public sealed class SecondViewModel : RxObject;
}
