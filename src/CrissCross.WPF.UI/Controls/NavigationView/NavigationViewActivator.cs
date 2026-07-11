// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.UI.Designer;

namespace CrissCross.WPF.UI.Controls;

/// <summary>Internal activator for creating content instances of the navigation view items.</summary>
internal static class NavigationViewActivator
{
    /// <summary>Creates new instance of type derived from <see cref="FrameworkElement"/>.</summary>
    /// <param name="pageType"><see cref="FrameworkElement"/> to instantiate.</param>
    /// <param name="dataContext">Additional context to set.</param>
    /// <returns>Instance of the <see cref="FrameworkElement"/> object or <see langword="null"/>.</returns>
    public static FrameworkElement? CreateInstance(Type pageType, object? dataContext = null)
    {
        if (!typeof(FrameworkElement).IsAssignableFrom(pageType))
        {
            throw new InvalidCastException(
                $"PageType of the ${typeof(INavigationViewItem)} must be derived from {typeof(FrameworkElement)}. {pageType} is not.");
        }

        if (DesignerHelper.IsInDesignMode)
        {
            return new Page
            {
                Content = new TextBlock
                {
                    Text = "Pages are not rendered while using the Designer. Edit the page template directly."
                }
            };
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

        var emptyConstructor = FindParameterlessConstructor(pageType) ?? throw new InvalidOperationException(
                $"The {pageType} page does not have a parameterless constructor. If you are using {typeof(IPageService)} do not navigate initially and don't use Cache or Precache.");

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
        var parameterlessCount = pageConstructors.Count(ctor => ctor.GetParameters().Length == 0);
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

        var selectedCtor = FitBestConstructor(pageConstructors, dataContext) ?? throw new InvalidOperationException(
                $"The {pageType} page does not have a parameterless constructor or the required services have not been configured for dependency injection. Use the static {nameof(ControlsServices)} class to initialize the GUI library with your service provider. If you are using {typeof(IPageService)} do not navigate initially and don't use Cache or Precache.");

        instance = InvokeElementConstructor(selectedCtor, dataContext);
        SetDataContext(instance, dataContext);
        return true;
    }

    /// <summary>Provides the ResolveConstructorParameter member.</summary>
    /// <param name="parameterType">The parameter type.</param>
    /// <param name="dataContext">The dataContext value.</param>
    /// <returns>The result.</returns>
    private static object? ResolveConstructorParameter(Type parameterType, object? dataContext)
    {
        return dataContext is not null && dataContext.GetType() == parameterType ? dataContext : ControlsServices.ControlsServiceProvider?.GetService(parameterType);
    }

    /// <summary>Picks a constructor which has the most satisfiable arguments count.</summary>
    /// <param name="parameterfullCtors">The parameterfullCtors value.</param>
    /// <param name="dataContext">The dataContext value.</param>
    /// <returns>The result.</returns>
    private static ConstructorInfo? FitBestConstructor(
        ConstructorInfo[] parameterfullCtors,
        object? dataContext)
    {
        return parameterfullCtors
            .Select(ctor =>
            {
                var parameters = ctor.GetParameters();
                var argumentResolution = parameters.Select(prm =>
                {
                    var resolved = ResolveConstructorParameter(prm.ParameterType, dataContext);
                    return resolved is not null;
                });
                var fullyResolved = argumentResolution.All(resolved => resolved);
                var score = fullyResolved ? parameters.Length : 0;

                return (Constructor: ctor, Score: score);
            })
            .Where(cs => cs.Score > 0)
            .OrderBy(cs => cs.Score)
            .Select(cs => cs.Constructor)
            .FirstOrDefault();
    }

    /// <summary>Provides the InvokeElementConstructor member.</summary>
    /// <param name="ctor">The ctor value.</param>
    /// <param name="dataContext">The dataContext value.</param>
    /// <returns>The result.</returns>
    private static FrameworkElement? InvokeElementConstructor(ConstructorInfo ctor, object? dataContext)
    {
        var args = ctor.GetParameters()
            .Select(prm => ResolveConstructorParameter(prm.ParameterType, dataContext));

        return ctor.Invoke(args.ToArray()) as FrameworkElement;
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
    private static ConstructorInfo? FindParameterlessConstructor(Type? pageType) => pageType?.GetConstructor(Type.EmptyTypes);

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
