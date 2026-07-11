// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace CP.Reactive.Collections;

/// <summary>Observable collection compatibility type for small chart-related collections.</summary>
/// <typeparam name="T">The item type.</typeparam>
public class QuaternaryList<T> : ReactiveList<T>
{
    /// <summary>Initializes a new instance of the <see cref="QuaternaryList{T}"/> class.</summary>
    public QuaternaryList()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="QuaternaryList{T}"/> class.</summary>
    /// <param name="collection">The initial items.</param>
    public QuaternaryList(IEnumerable<T> collection)
        : base(collection)
    {
    }
}
