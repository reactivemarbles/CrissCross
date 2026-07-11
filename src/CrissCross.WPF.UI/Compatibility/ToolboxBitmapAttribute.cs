// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
#if NET8_0_OR_GREATER && !NET10_0_OR_GREATER

namespace System.Drawing;

/// <summary>Provides a compile-time replacement for <c>ToolboxBitmapAttribute</c> on modern .NET WPF targets.</summary>
[AttributeUsage(AttributeTargets.Class)]
internal sealed class ToolboxBitmapAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="ToolboxBitmapAttribute"/> class.</summary>
    /// <param name="type">The type associated with the bitmap resource.</param>
    /// <param name="name">The bitmap resource name.</param>
    public ToolboxBitmapAttribute(Type type, string name)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Type = type;
        Name = name;
    }

    /// <summary>Gets the type associated with the bitmap resource.</summary>
    public Type Type { get; }

    /// <summary>Gets the bitmap resource name.</summary>
    public string Name { get; }
}

#endif
