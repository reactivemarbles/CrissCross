// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;

namespace CrissCross.Tests;

/// <summary>Tests for Navigation Event Args classes.</summary>
public class NavigationEventArgsTests
{
    /// <summary>Provides the ViewModelNavigationEventArgs_Constructor_SetsProperties member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelNavigationEventArgs_Constructor_SetsProperties()
    {
        // Arrange
        var from = new TestRxObject();
        var to = new TestRxObject();
        const NavigationType navType = NavigationType.New;
        var view = new TestView();
        const string hostName = "TestHost";
        var parameter = new object();

        // Act
        var eventArgs = new ViewModelNavigationEventArgs(from, to, navType, view, hostName, parameter);

        // Assert
        await Assert.That(eventArgs.From).IsEqualTo(from);
        await Assert.That(eventArgs.To).IsEqualTo(to);
        await Assert.That(eventArgs.NavigationType).IsEqualTo(navType);
        await Assert.That(eventArgs.View).IsEqualTo(view);
        await Assert.That(eventArgs.HostName).IsEqualTo(hostName);
        await Assert.That(eventArgs.NavigationParameter).IsEqualTo(parameter);
    }

    /// <summary>Provides the ViewModelNavigationEventArgs_Constructor_AllowsNullValues member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelNavigationEventArgs_Constructor_AllowsNullValues()
    {
        // Arrange & Act
        var eventArgs = new ViewModelNavigationEventArgs(null, null, NavigationType.New, null, null, null);

        // Assert
        await Assert.That(eventArgs.From).IsNull();
        await Assert.That(eventArgs.To).IsNull();
        await Assert.That(eventArgs.View).IsNull();
        await Assert.That(eventArgs.HostName).IsNull();
        await Assert.That(eventArgs.NavigationParameter).IsNull();
    }

    /// <summary>Provides the ViewModelNavigationEventArgs_HostName_CanBeModified member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelNavigationEventArgs_HostName_CanBeModified()
    {
        // Arrange
        var eventArgs = new ViewModelNavigationEventArgs(null, null, NavigationType.New, null, "Original", null);

        // Act
        eventArgs.HostName = "Modified";

        // Assert
        await Assert.That(eventArgs.HostName).IsEqualTo("Modified");
    }

    /// <summary>Provides the ViewModelNavigationEventArgs_View_CanBeModified member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelNavigationEventArgs_View_CanBeModified()
    {
        // Arrange
        var view1 = new TestView();
        var view2 = new TestView();
        var eventArgs = new ViewModelNavigationEventArgs(null, null, NavigationType.New, view1, null, null);

        // Act
        eventArgs.View = view2;

        // Assert
        await Assert.That(eventArgs.View).IsEqualTo(view2);
    }

    /// <summary>Provides the ViewModelNavigationEventArgs_NavigationType_New_IsSet member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelNavigationEventArgs_NavigationType_New_IsSet()
    {
        // Arrange & Act
        var eventArgs = new ViewModelNavigationEventArgs(null, null, NavigationType.New, null, null, null);

        // Assert
        await Assert.That(eventArgs.NavigationType).IsEqualTo(NavigationType.New);
    }

    /// <summary>Provides the ViewModelNavigationEventArgs_NavigationType_Back_IsSet member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelNavigationEventArgs_NavigationType_Back_IsSet()
    {
        // Arrange & Act
        var eventArgs = new ViewModelNavigationEventArgs(null, null, NavigationType.Back, null, null, null);

        // Assert
        await Assert.That(eventArgs.NavigationType).IsEqualTo(NavigationType.Back);
    }

    /// <summary>Provides the ViewModelNavigationEventArgs_NavigationType_Refresh_IsSet member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelNavigationEventArgs_NavigationType_Refresh_IsSet()
    {
        // Arrange & Act
        var eventArgs = new ViewModelNavigationEventArgs(null, null, NavigationType.Refresh, null, null, null);

        // Assert
        await Assert.That(eventArgs.NavigationType).IsEqualTo(NavigationType.Refresh);
    }

    /// <summary>Provides the ViewModelNavigatingEventArgs_Constructor_SetsProperties member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelNavigatingEventArgs_Constructor_SetsProperties()
    {
        // Arrange
        var from = new TestRxObject();
        var to = new TestRxObject();
        const NavigationType navType = NavigationType.New;
        var view = new TestView();
        const string hostName = "TestHost";
        var parameter = new object();

        // Act
        var eventArgs = new ViewModelNavigatingEventArgs(from, to, navType, view, hostName, parameter);

        // Assert
        await Assert.That(eventArgs.From).IsEqualTo(from);
        await Assert.That(eventArgs.To).IsEqualTo(to);
        await Assert.That(eventArgs.NavigationType).IsEqualTo(navType);
        await Assert.That(eventArgs.View).IsEqualTo(view);
        await Assert.That(eventArgs.HostName).IsEqualTo(hostName);
        await Assert.That(eventArgs.NavigationParameter).IsEqualTo(parameter);
    }

    /// <summary>Provides the ViewModelNavigatingEventArgs_Cancel_IsFalseByDefault member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelNavigatingEventArgs_Cancel_IsFalseByDefault()
    {
        // Arrange & Act
        var eventArgs = new ViewModelNavigatingEventArgs(null, null, NavigationType.New, null, null, null);

        // Assert
        await Assert.That(eventArgs.Cancel).IsFalse();
    }

    /// <summary>Provides the ViewModelNavigatingEventArgs_Cancel_CanBeSet member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelNavigatingEventArgs_Cancel_CanBeSet()
    {
        // Arrange
        var eventArgs = new ViewModelNavigatingEventArgs(null, null, NavigationType.New, null, null, null);

        // Act
        eventArgs.Cancel = true;

        // Assert
        await Assert.That(eventArgs.Cancel).IsTrue();
    }

    /// <summary>Provides the ViewModelNavigatingEventArgs_InheritsFromViewModelNavigationEventArgs member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelNavigatingEventArgs_InheritsFromViewModelNavigationEventArgs()
    {
        // Arrange & Act
        var eventArgs = new ViewModelNavigatingEventArgs(null, null, NavigationType.New, null, null, null);

        // Assert
        await Assert.That(eventArgs).IsAssignableTo<ViewModelNavigationEventArgs>();
    }

    /// <summary>Provides the ViewModelNavigatingEventArgs_ImplementsIViewModelNavigatingEventArgs member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelNavigatingEventArgs_ImplementsIViewModelNavigatingEventArgs()
    {
        // Arrange & Act
        var eventArgs = new ViewModelNavigatingEventArgs(null, null, NavigationType.New, null, null, null);

        // Assert
        await Assert.That(eventArgs).IsAssignableTo<IViewModelNavigatingEventArgs>();
    }

    /// <summary>Provides the ViewModelNavigationEventArgs_ImplementsIViewModelNavigationEventArgs member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelNavigationEventArgs_ImplementsIViewModelNavigationEventArgs()
    {
        // Arrange & Act
        var eventArgs = new ViewModelNavigationEventArgs(null, null, NavigationType.New, null, null, null);

        // Assert
        await Assert.That(eventArgs).IsAssignableTo<IViewModelNavigationEventArgs>();
    }

    /// <summary>Provides the ViewModelNavigationBaseEventArgs_ImplementsIViewModelNavigationBaseEventArgs member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelNavigationBaseEventArgs_ImplementsIViewModelNavigationBaseEventArgs()
    {
        // Arrange & Act
        var eventArgs = new ViewModelNavigationEventArgs(null, null, NavigationType.New, null, null, null);

        // Assert
        await Assert.That(eventArgs).IsAssignableTo<IViewModelNavigationBaseEventArgs>();
    }

    /// <summary>Provides the NavigationType_Enum_HasCorrectValues member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigationType_Enum_HasCorrectValues()
    {
        // Assert
        await Assert.That((int)NavigationType.New).IsEqualTo(0);
        await Assert.That((int)NavigationType.Back).IsEqualTo(1);
        await Assert.That((int)NavigationType.Refresh).IsEqualTo(2);
    }

    /// <summary>Provides the ViewModelNavigatingEventArgs_AllowsNullParameters member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelNavigatingEventArgs_AllowsNullParameters()
    {
        // Arrange & Act
        var eventArgs = new ViewModelNavigatingEventArgs(null, null, NavigationType.New, null, null, null);

        // Assert
        await Assert.That(eventArgs.From).IsNull();
        await Assert.That(eventArgs.To).IsNull();
        await Assert.That(eventArgs.View).IsNull();
        await Assert.That(eventArgs.HostName).IsNull();
        await Assert.That(eventArgs.NavigationParameter).IsNull();
    }

    /// <summary>Provides the ViewModelNavigatingEventArgs_WithParameter_PreservesParameter member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelNavigatingEventArgs_WithParameter_PreservesParameter()
    {
        // Arrange
        var parameter = new { Value = "Test" };

        // Act
        var eventArgs = new ViewModelNavigatingEventArgs(null, null, NavigationType.New, null, null, parameter);

        // Assert
        await Assert.That(eventArgs.NavigationParameter).IsEqualTo(parameter);
    }

    /// <summary>Provides the TestRxObject member.</summary>
    private sealed class TestRxObject : RxObject;

    /// <summary>Provides the TestView member.</summary>
    private sealed class TestView : IViewFor
    {
        /// <summary>Initializes a new instance of the <see cref="TestView"/> class.</summary>
        /// <param name="viewModel">The viewModel value.</param>
        public TestView(object? viewModel = null)
        {
            ViewModel = viewModel;
        }

        /// <summary>Gets or sets the value.</summary>
        public object? ViewModel { get; set; }
    }
}
