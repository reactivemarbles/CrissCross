// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace CrissCross;

/// <summary>Represents an invalid bidirectional navigation registration.</summary>
public sealed class NavigationRegistrationException : InvalidOperationException
{
    /// <summary>Initializes a new instance of the <see cref="NavigationRegistrationException"/> class.</summary>
    public NavigationRegistrationException()
    {
        SourceKind = NavigationSourceKind.ViewModel;
        ServiceType = typeof(object);
    }

    /// <summary>Initializes a new instance of the <see cref="NavigationRegistrationException"/> class.</summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public NavigationRegistrationException(string? message)
        : base(message)
    {
        SourceKind = NavigationSourceKind.ViewModel;
        ServiceType = typeof(object);
    }

    /// <summary>Initializes a new instance of the <see cref="NavigationRegistrationException"/> class.</summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public NavigationRegistrationException(string? message, Exception? innerException)
        : base(message, innerException)
    {
        SourceKind = NavigationSourceKind.ViewModel;
        ServiceType = typeof(object);
    }

    /// <summary>Initializes a new instance of the <see cref="NavigationRegistrationException"/> class.</summary>
    /// <param name="sourceKind">The duplicated source side.</param>
    /// <param name="serviceType">The duplicated source key.</param>
    /// <param name="contract">The duplicated navigation contract.</param>
    public NavigationRegistrationException(NavigationSourceKind sourceKind, Type serviceType, string? contract)
        : base($"A {sourceKind} navigation registration already exists for '{serviceType?.FullName}' with contract '{NavigationContract.ToDisplay(contract)}'.")
    {
        SourceKind = sourceKind;
        ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
        Contract = NavigationContract.Normalize(contract);
    }

    /// <summary>Gets the duplicated source side.</summary>
    public NavigationSourceKind SourceKind { get; }

    /// <summary>Gets the duplicated source key.</summary>
    public Type ServiceType { get; }

    /// <summary>Gets the duplicated normalized navigation contract.</summary>
    public string? Contract { get; }
}
