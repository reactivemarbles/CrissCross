// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross;
using ReactiveUI;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace CrissCross.Tests;

/// <summary>
/// Tests for Navigation Event Args classes.
/// </summary>
public class NavigationEventArgsTests
{
    private class TestRxObject : RxObject;

    private class TestView : IViewFor
    {
        public TestView(object? viewModel = null)
        {
            ViewModel = viewModel;
        }

        public object? ViewModel { get; set; }
    }

    [Test]
    public async Task ViewModelNavigationEventArgs_Constructor_SetsProperties()
    {
        // Arrange
        var from = new TestRxObject();
        var to = new TestRxObject();
        var navType = NavigationType.New;
        var view = new TestView();
        var hostName = "TestHost";
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

    [Test]
    public async Task ViewModelNavigationEventArgs_NavigationType_New_IsSet()
    {
        // Arrange & Act
        var eventArgs = new ViewModelNavigationEventArgs(null, null, NavigationType.New, null, null, null);

        // Assert
        await Assert.That(eventArgs.NavigationType).IsEqualTo(NavigationType.New);
    }

    [Test]
    public async Task ViewModelNavigationEventArgs_NavigationType_Back_IsSet()
    {
        // Arrange & Act
        var eventArgs = new ViewModelNavigationEventArgs(null, null, NavigationType.Back, null, null, null);

        // Assert
        await Assert.That(eventArgs.NavigationType).IsEqualTo(NavigationType.Back);
    }

    [Test]
    public async Task ViewModelNavigationEventArgs_NavigationType_Refresh_IsSet()
    {
        // Arrange & Act
        var eventArgs = new ViewModelNavigationEventArgs(null, null, NavigationType.Refresh, null, null, null);

        // Assert
        await Assert.That(eventArgs.NavigationType).IsEqualTo(NavigationType.Refresh);
    }

    [Test]
    public async Task ViewModelNavigatingEventArgs_Constructor_SetsProperties()
    {
        // Arrange
        var from = new TestRxObject();
        var to = new TestRxObject();
        var navType = NavigationType.New;
        var view = new TestView();
        var hostName = "TestHost";
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

    [Test]
    public async Task ViewModelNavigatingEventArgs_Cancel_IsFalseByDefault()
    {
        // Arrange & Act
        var eventArgs = new ViewModelNavigatingEventArgs(null, null, NavigationType.New, null, null, null);

        // Assert
        await Assert.That(eventArgs.Cancel).IsFalse();
    }

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

    [Test]
    public async Task ViewModelNavigatingEventArgs_InheritsFromViewModelNavigationEventArgs()
    {
        // Arrange & Act
        var eventArgs = new ViewModelNavigatingEventArgs(null, null, NavigationType.New, null, null, null);

        // Assert
        await Assert.That(eventArgs).IsAssignableTo<ViewModelNavigationEventArgs>();
    }

    [Test]
    public async Task ViewModelNavigatingEventArgs_ImplementsIViewModelNavigatingEventArgs()
    {
        // Arrange & Act
        var eventArgs = new ViewModelNavigatingEventArgs(null, null, NavigationType.New, null, null, null);

        // Assert
        await Assert.That(eventArgs).IsAssignableTo<IViewModelNavigatingEventArgs>();
    }

    [Test]
    public async Task ViewModelNavigationEventArgs_ImplementsIViewModelNavigationEventArgs()
    {
        // Arrange & Act
        var eventArgs = new ViewModelNavigationEventArgs(null, null, NavigationType.New, null, null, null);

        // Assert
        await Assert.That(eventArgs).IsAssignableTo<IViewModelNavigationEventArgs>();
    }

    [Test]
    public async Task ViewModelNavigationBaseEventArgs_ImplementsIViewModelNavigationBaseEventArgs()
    {
        // Arrange & Act
        var eventArgs = new ViewModelNavigationEventArgs(null, null, NavigationType.New, null, null, null);

        // Assert
        await Assert.That(eventArgs).IsAssignableTo<IViewModelNavigationBaseEventArgs>();
    }

    [Test]
    public async Task NavigationType_Enum_HasCorrectValues()
    {
        // Assert
        await Assert.That((int)NavigationType.New).IsEqualTo(0);
        await Assert.That((int)NavigationType.Back).IsEqualTo(1);
        await Assert.That((int)NavigationType.Refresh).IsEqualTo(2);
    }

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
}
