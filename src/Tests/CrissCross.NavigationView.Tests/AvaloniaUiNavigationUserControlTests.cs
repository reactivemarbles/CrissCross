// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Controls;
using ReactiveUI;
using Splat;
using UiNavigationUserControl = CrissCross.Avalonia.UI.Controls.NavigationUserControl;

namespace CrissCross.NavigationView.Tests;

/// <summary>Tests the Avalonia UI navigation user control wrapper.</summary>
[TUnit.Core.Executors.TestExecutor<AvaloniaUiTestExecutor>]
public sealed class AvaloniaUiNavigationUserControlTests
{
    /// <summary>The explicit host name used by the UI navigation control.</summary>
    private const string HostName = "ui-navigation-host";

    /// <summary>The replacement host name used by the UI navigation control.</summary>
    private const string UpdatedHostName = "ui-navigation-host-updated";

    /// <summary>The expected history count after two navigation requests.</summary>
    private const int ExpectedNavigationHistoryCount = 2;

    /// <summary>Verifies initialization creates and registers the composed navigation frame.</summary>
    /// <returns>The asynchronous test operation.</returns>
    [Test]
    public async Task OnInitialized_WhenHostNameIsSet_ConfiguresNavigationFrame()
    {
        using var control = new TestNavigationUserControl { HostName = HostName, NavigateBackIsEnabled = false };

        control.InitializeForTest();

        await Assert.That(control.NavigationFrame).IsNotNull();
        await Assert.That(control.NavigationFrame!.HostName).IsEqualTo(HostName);
        await Assert.That(control.NavigationFrame.Name).IsEqualTo(HostName);
        await Assert.That(control.NavigationFrame.NavigateBackIsEnabled).IsFalse();
        await Assert.That(control.Content).IsSameReferenceAs(control.NavigationFrame);
        await Assert.That(((IUseNavigation)control).Name).IsEqualTo(HostName);
    }

    /// <summary>Verifies content supplied before initialization is preserved inside the composed frame.</summary>
    /// <returns>The asynchronous test operation.</returns>
    [Test]
    public async Task OnInitialized_WhenConsumerContentExists_MovesContentIntoNavigationFrame()
    {
        var consumerContent = new Button { Content = "Existing content" };
        using var control = new TestNavigationUserControl { HostName = HostName, Content = consumerContent };

        control.InitializeForTest();

        await Assert.That(control.NavigationFrame).IsNotNull();
        await Assert.That(control.Content).IsSameReferenceAs(control.NavigationFrame);
        await Assert.That(control.NavigationFrame!.Content).IsSameReferenceAs(consumerContent);
        await Assert.That(consumerContent.Parent).IsSameReferenceAs(control.NavigationFrame);
    }

    /// <summary>Verifies default host names are unique for multiple control instances.</summary>
    /// <returns>The asynchronous test operation.</returns>
    [Test]
    public async Task OnInitialized_WhenTwoControlsUseDefaults_GeneratesUniqueHostNames()
    {
        using var first = new TestNavigationUserControl();
        using var second = new TestNavigationUserControl();

        first.InitializeForTest();
        second.InitializeForTest();

        await Assert.That(first.NavigationFrame).IsNotNull();
        await Assert.That(second.NavigationFrame).IsNotNull();
        await Assert.That(first.NavigationFrame!.HostName).IsNotNull();
        await Assert.That(second.NavigationFrame!.HostName).IsNotNull();
        await Assert.That(first.NavigationFrame.HostName).IsNotEqualTo(second.NavigationFrame.HostName);
        await Assert.That(first.Content).IsSameReferenceAs(first.NavigationFrame);
        await Assert.That(second.Content).IsSameReferenceAs(second.NavigationFrame);
    }

    /// <summary>Verifies content supplied after initialization is still hosted by the navigation frame.</summary>
    /// <returns>The asynchronous test operation.</returns>
    [Test]
    public async Task Content_WhenSetAfterInitialization_RemainsHostedByNavigationFrame()
    {
        var consumerContent = new Button { Content = "Updated content" };
        using var control = new TestNavigationUserControl { HostName = HostName };

        control.InitializeForTest();
        control.Content = consumerContent;

        await Assert.That(control.Content).IsSameReferenceAs(control.NavigationFrame);
        await Assert.That(control.NavigationFrame!.Content).IsSameReferenceAs(consumerContent);
        await Assert.That(consumerContent.Parent).IsSameReferenceAs(control.NavigationFrame);
    }

    /// <summary>Verifies host properties update the composed frame after initialization.</summary>
    /// <returns>The asynchronous test operation.</returns>
    [Test]
    public async Task HostProperties_WhenChangedAfterInitialization_UpdateNavigationFrame()
    {
        using var control = new TestNavigationUserControl { HostName = HostName };

        control.InitializeForTest();
        control.HostName = UpdatedHostName;
        control.NavigateBackIsEnabled = false;

        await Assert.That(control.NavigationFrame!.HostName).IsEqualTo(UpdatedHostName);
        await Assert.That(control.NavigationFrame.Name).IsEqualTo(HostName);
        await Assert.That(control.NavigationFrame.NavigateBackIsEnabled).IsFalse();
        await Assert.That(((IUseNavigation)control).Name).IsEqualTo(UpdatedHostName);
    }

    /// <summary>Verifies attachment and logical-host changes preserve the styled frame name.</summary>
    /// <returns>The asynchronous test operation.</returns>
    [Test]
    public async Task AttachAndReattach_PreserveTheStyledFrameName()
    {
        var content = new Button { Content = "Navigation host ready" };
        using var control = new UiNavigationUserControl { HostName = HostName, Content = content };
        var window = new Window { Content = control };
        try
        {
            window.Show();
            window.UpdateLayout();
            control.HostName = UpdatedHostName;
            window.Content = null;
            window.Content = control;
            window.UpdateLayout();
            await Assert.That(control.NavigationFrame!.Name).IsEqualTo(HostName);
            await Assert.That(control.NavigationFrame.HostName).IsEqualTo(UpdatedHostName);
            await Assert.That(control.NavigationFrame.Parent).IsSameReferenceAs(control);
            await Assert.That(control.NavigationFrame.Content).IsSameReferenceAs(content);
            await Assert.That(TopLevel.GetTopLevel(content)).IsSameReferenceAs(window);
            await Assert.That(content.IsEffectivelyVisible).IsTrue();
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>Verifies the UI navigation control can navigate through the public navigation registry.</summary>
    /// <returns>The asynchronous test operation.</returns>
    [Test]
    public async Task NavigateToView_WhenViewLocatorResolvesViews_UpdatesComposedFrame()
    {
        var previousMainThreadScheduler = RxSchedulers.MainThreadScheduler;
        RxSchedulers.MainThreadScheduler = ImmediateScheduler.Instance;
        using var control = new TestNavigationUserControl { HostName = HostName };

        try
        {
            AppLocator.CurrentMutable.UnregisterAll<FirstViewModel>();
            AppLocator.CurrentMutable.UnregisterAll<SecondViewModel>();
            AppLocator.CurrentMutable.RegisterConstant(new FirstViewModel());
            AppLocator.CurrentMutable.RegisterConstant(new SecondViewModel());
            control.InitializeForTest();
            control.NavigationFrame!.ViewLocator = new TestViewLocator();

            control.NavigateToView(new NavigationKeyRequest<FirstViewModel>());
            control.NavigateToView(new NavigationKeyRequest<SecondViewModel>());

            await Assert.That(control.NavigationFrame.NavigationStack.Count).IsEqualTo(ExpectedNavigationHistoryCount);
            await Assert.That(control.NavigationFrame.Content).IsTypeOf<SecondView>();
            await Assert.That(((SecondView)control.NavigationFrame.Content!).ViewModel).IsNotNull();
            await Assert.That(control.NavigationFrame.CanNavigateBack).IsTrue();
        }
        finally
        {
            RxSchedulers.MainThreadScheduler = previousMainThreadScheduler;
            AppLocator.CurrentMutable.UnregisterAll<FirstViewModel>();
            AppLocator.CurrentMutable.UnregisterAll<SecondViewModel>();
        }
    }

    /// <summary>Verifies the typed UI control uses the non-generic XAML view-model property.</summary>
    /// <returns>The asynchronous test operation.</returns>
    [Test]
    public async Task GenericControl_UsesNonGenericViewModelProperty()
    {
        using var control = new CrissCross.Avalonia.UI.Controls.NavigationUserControl<FirstViewModel>();
        var viewModel = new FirstViewModel();

        control.ViewModel = viewModel;

        await Assert.That(UiNavigationUserControl.ViewModelProperty.OwnerType).IsEqualTo(typeof(UiNavigationUserControl));
        await Assert.That(control.GetValue(UiNavigationUserControl.ViewModelProperty)).IsSameReferenceAs(viewModel);
        await Assert.That(((IViewFor)control).ViewModel).IsSameReferenceAs(viewModel);
        await Assert.That(control.BindingRoot).IsSameReferenceAs(viewModel);
    }

    /// <summary>Verifies disposing the wrapper disposes and clears the composed frame reference.</summary>
    /// <returns>The asynchronous test operation.</returns>
    [Test]
    public async Task Dispose_WhenNavigationFrameExists_ClearsFrameReference()
    {
        var control = new TestNavigationUserControl { HostName = HostName };

        control.InitializeForTest();
        var frame = control.NavigationFrame;

        control.Dispose();

        await Assert.That(frame).IsNotNull();
        await Assert.That(control.NavigationFrame).IsNull();
    }

    /// <summary>First test view model.</summary>
    public sealed class FirstViewModel : RxObject;

    /// <summary>Second test view model.</summary>
    public sealed class SecondViewModel : RxObject;

    /// <summary>First test view.</summary>
    private sealed class FirstView : TextBlock, IViewFor<FirstViewModel>
    {
        /// <summary>Gets or sets the strongly typed view model.</summary>
        public FirstViewModel? ViewModel { get; set; }

        /// <inheritdoc/>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (FirstViewModel?)value;
        }
    }

    /// <summary>Second test view.</summary>
    private sealed class SecondView : TextBlock, IViewFor<SecondViewModel>
    {
        /// <summary>Gets or sets the strongly typed view model.</summary>
        public SecondViewModel? ViewModel { get; set; }

        /// <inheritdoc/>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (SecondViewModel?)value;
        }
    }

    /// <summary>View locator used by navigation tests.</summary>
    private sealed class TestViewLocator : IViewLocator
    {
        /// <inheritdoc/>
        public IViewFor<TViewModel> ResolveView<TViewModel>()
            where TViewModel : class => ResolveView<TViewModel>(contract: null);

        /// <inheritdoc/>
        public IViewFor<TViewModel> ResolveView<TViewModel>(string? contract)
            where TViewModel : class
        {
            if (typeof(TViewModel) == typeof(FirstViewModel))
            {
                return (IViewFor<TViewModel>)(object)new FirstView();
            }

            if (typeof(TViewModel) == typeof(SecondViewModel))
            {
                return (IViewFor<TViewModel>)(object)new SecondView();
            }

            throw new InvalidOperationException($"Unsupported view model type {typeof(TViewModel).FullName}.");
        }

        /// <inheritdoc/>
        public IViewFor? ResolveView(object? instance) => ResolveView(instance, contract: null);

        /// <inheritdoc/>
        public IViewFor? ResolveView(object? instance, string? contract) => instance switch
        {
            FirstViewModel => new FirstView(),
            SecondViewModel => new SecondView(),
            _ => null,
        };
    }

    /// <summary>Testable UI navigation control.</summary>
    private sealed class TestNavigationUserControl : UiNavigationUserControl
    {
        /// <summary>Initializes the navigation control for tests.</summary>
        public void InitializeForTest() => OnInitialized();
    }
}
