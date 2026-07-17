// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Storage;
#else
namespace CrissCross.WPF.UI.Storage;
#endif

/// <summary>Represents IStore.</summary>
public interface IStore
{
    /// <summary>Lists the ids.</summary>
    /// <returns>A string array.</returns>
    IEnumerable<string> ListIds();

    /// <summary>Sets the data.</summary>
    /// <param name="id">The id value.</param>
    /// <param name="values">The values value.</param>
    void SetData(string id, IDictionary<string, object?> values);

    /// <summary>Gets the data.</summary>
    /// <param name="id">The id value.</param>
    /// <returns>A string array.</returns>
    IDictionary<string, object?> GetData(string id);

    /// <summary>Clears the data.</summary>
    /// <param name="id">The id value.</param>
    void ClearData(string id);

    /// <summary>Clears all.</summary>
    void ClearAll();
}
