// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.UI.Gallery.ViewModels;
using CrissCross.WPF.UI.Gallery.Views;
using IViewFor = ReactiveUI.Binding.IViewFor;

namespace CrissCross.WPF.UI.Gallery.Tests;

/// <summary>Verifies the dependency property contract formerly generated for gallery views.</summary>
public sealed class GalleryViewModelContractTests
{
    /// <summary>Verifies that typed and untyped assignments share the dependency property and binding root.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ViewModel_WhenReplaced_PreservesDependencyPropertyAndInterfaceContract()
    {
        var snapshot = await RunOnStaThreadAsync(static () =>
        {
            var view = new ColorControlsView();
            var first = new ColorControlsViewModel();
            var replacement = new ColorControlsViewModel();
            var context = new object();
            IViewFor untyped = view;
            var initiallyNull = untyped.ViewModel is null;
            view.DataContext = context;
            view.ViewModel = first;
            var typedAssignment = ReferenceEquals(view.GetValue(ColorControlsView.ViewModelProperty), first)
                && ReferenceEquals(view.BindingRoot, first)
                && ReferenceEquals(untyped.ViewModel, first);
            untyped.ViewModel = replacement;
            var untypedAssignment = ReferenceEquals(view.ViewModel, replacement)
                && ReferenceEquals(view.BindingRoot, replacement);
            untyped.ViewModel = null;
            return (
                InitiallyNull: initiallyNull,
                TypedAssignment: typedAssignment,
                UntypedAssignment: untypedAssignment,
                Cleared: view.GetValue(ColorControlsView.ViewModelProperty) is null,
                ContextPreserved: ReferenceEquals(view.DataContext, context));
        });

        await Assert.That(snapshot.InitiallyNull).IsTrue();
        await Assert.That(snapshot.TypedAssignment).IsTrue();
        await Assert.That(snapshot.UntypedAssignment).IsTrue();
        await Assert.That(snapshot.Cleared).IsTrue();
        await Assert.That(snapshot.ContextPreserved).IsTrue();
    }

    /// <summary>Runs dependency property operations on an STA thread suitable for WPF.</summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="action">The WPF action.</param>
    /// <returns>A task that completes with the action result.</returns>
    private static Task<TResult> RunOnStaThreadAsync<TResult>(Func<TResult> action) =>
        WpfTestDispatcher.RunAsync(action);
}
