// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.WPF.UI.Designer;
#else
using CrissCross.WPF.UI.Designer;
#endif

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Internal activator for creating content instances of the navigation view items.</summary>
internal static class NavigationViewActivator
{
    /// <summary>Creates new instance of type derived from <see cref="FrameworkElement"/>.</summary>
    /// <param name="pageType"><see cref="FrameworkElement"/> to instantiate.</param>
    /// <param name="dataContext">Additional context to set.</param>
    /// <returns>Instance of the <see cref="FrameworkElement"/> object or <see langword="null"/>.</returns>
    internal static FrameworkElement? CreateInstance(Type pageType, object? dataContext = null)
    {
        if (!typeof(FrameworkElement).IsAssignableFrom(pageType))
        {
            throw new InvalidCastException(
                $"PageType of {typeof(INavigationViewItem)} must be derived from "
                    + $"{typeof(FrameworkElement)}. {pageType} is not.");
        }

        if (DesignerHelper.IsInDesignMode)
        {
            return new Page { Content = new TextBlock { Text = "Pages are not rendered while using the Designer. Edit the page template directly.", }, };
        }

        FrameworkElement? instance;

#if NET472_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        if (TryCreateFromServiceProvider(pageType, dataContext, out instance))
        {
            return instance;
        }

        if (ControlsServices.ControlsServiceProvider is null && dataContext is not null)
#else
        if (dataContext is not null)
#endif
        {
            instance = InvokeElementConstructor(pageType, dataContext);

            if (instance is not null)
            {
                return instance;
            }
        }

        var emptyConstructor =
            FindParameterlessConstructor(pageType)
            ?? throw new InvalidOperationException(
                $"The {pageType} page does not have a parameterless constructor. If you are using "
                    + $"{typeof(IPageService)}, do not navigate initially or use Cache or Precache.");

        instance = emptyConstructor.Invoke(null) as FrameworkElement;
        SetDataContext(instance, dataContext);

        return instance;
    }

#if NET472_OR_GREATER || NETCOREAPP3_0_OR_GREATER

    /// <summary>Tries to create the page using the configured service provider.</summary>
    /// <param name="pageType">The page type.</param>
    /// <param name="dataContext">The data context.</param>
    /// <param name="instance">The created instance.</param>
    /// <returns><c>true</c> if an instance was created; otherwise, <c>false</c>.</returns>
    private static bool TryCreateFromServiceProvider(Type pageType, object? dataContext, out FrameworkElement? instance)
    {
        instance = null;
        if (ControlsServices.ControlsServiceProvider is null)
        {
            return false;
        }

        var pageConstructors = pageType.GetConstructors();
        var parameterlessCount = 0;
        foreach (var constructor in pageConstructors)
        {
            if (constructor.GetParameters().Length == 0)
            {
                parameterlessCount++;
            }
        }

        var parameterfullCount = pageConstructors.Length - parameterlessCount;

        if (parameterlessCount == 1)
        {
            instance = FindParameterlessConstructor(pageType)?.Invoke(null) as FrameworkElement;
            return true;
        }

        if (parameterlessCount != 0 || parameterfullCount <= 0)
        {
            return false;
        }

        var selectedCtor =
            FitBestConstructor(pageConstructors, dataContext)
            ?? throw new InvalidOperationException(CreateMissingConstructorMessage(pageType));

        instance = InvokeElementConstructor(selectedCtor, dataContext);
        SetDataContext(instance, dataContext);
        return true;
    }

    /// <summary>Creates an actionable diagnostic when page activation cannot select a constructor.</summary>
    /// <param name="pageType">The page type being activated.</param>
    /// <returns>The activation error message.</returns>
    private static string CreateMissingConstructorMessage(Type pageType) =>
        $"""
         The {pageType} page does not have a parameterless constructor, or the required services
         have not been configured for dependency injection. Use the static {nameof(ControlsServices)} class
         to initialize the GUI library with your service provider. If you are using {typeof(IPageService)},
         do not navigate initially or use Cache or Precache.
         """;

    /// <summary>Provides the ResolveConstructorParameter member.</summary>
    /// <param name="parameterType">The parameter type.</param>
    /// <param name="dataContext">The dataContext value.</param>
    /// <returns>The result.</returns>
    private static object? ResolveConstructorParameter(Type parameterType, object? dataContext) => dataContext is not null && dataContext.GetType() == parameterType
        ? dataContext
        : ControlsServices.ControlsServiceProvider?.GetService(parameterType);

    /// <summary>Picks a constructor which has the most satisfiable arguments count.</summary>
    /// <param name="parameterfullCtors">The parameterfullCtors value.</param>
    /// <param name="dataContext">The dataContext value.</param>
    /// <returns>The result.</returns>
    private static ConstructorInfo? FitBestConstructor(ConstructorInfo[] parameterfullCtors, object? dataContext)
    {
        ConstructorInfo? selectedConstructor = null;
        var selectedScore = int.MaxValue;

        foreach (var constructor in parameterfullCtors)
        {
            var parameters = constructor.GetParameters();
            var fullyResolved = true;
            foreach (var parameter in parameters)
            {
                if (ResolveConstructorParameter(parameter.ParameterType, dataContext) is null)
                {
                    fullyResolved = false;
                    break;
                }
            }

            if (fullyResolved && parameters.Length > 0 && parameters.Length < selectedScore)
            {
                selectedConstructor = constructor;
                selectedScore = parameters.Length;
            }
        }

        return selectedConstructor;
    }

    /// <summary>Provides the InvokeElementConstructor member.</summary>
    /// <param name="ctor">The ctor value.</param>
    /// <param name="dataContext">The dataContext value.</param>
    /// <returns>The result.</returns>
    private static FrameworkElement? InvokeElementConstructor(ConstructorInfo ctor, object? dataContext)
    {
        var parameters = ctor.GetParameters();
        var arguments = new object?[parameters.Length];
        for (var index = 0; index < parameters.Length; index++)
        {
            arguments[index] = ResolveConstructorParameter(parameters[index].ParameterType, dataContext);
        }

        return ctor.Invoke(arguments) as FrameworkElement;
    }
#endif

    /// <summary>Provides the InvokeElementConstructor member.</summary>
    /// <param name="pageType">The page type.</param>
    /// <param name="dataContext">The dataContext value.</param>
    /// <returns>The result.</returns>
    private static FrameworkElement? InvokeElementConstructor(Type pageType, object? dataContext)
    {
        var ctor = dataContext is null
            ? pageType.GetConstructor(Type.EmptyTypes)
            : pageType.GetConstructor([dataContext!.GetType()]);

        return ctor is null ? null : ctor.Invoke([dataContext]) as FrameworkElement;
    }

    /// <summary>Provides the FindParameterlessConstructor member.</summary>
    /// <param name="pageType">The page type.</param>
    /// <returns>The result.</returns>
    private static ConstructorInfo? FindParameterlessConstructor(Type? pageType) =>
        pageType?.GetConstructor(Type.EmptyTypes);

    /// <summary>Provides the SetDataContext member.</summary>
    /// <param name="element">The element value.</param>
    /// <param name="dataContext">The dataContext value.</param>
    private static void SetDataContext(FrameworkElement? element, object? dataContext)
    {
        if (element is null || dataContext is null)
        {
            return;
        }

        element.DataContext = dataContext;
    }
}
