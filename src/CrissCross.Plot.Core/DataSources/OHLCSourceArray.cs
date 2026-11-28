// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.DataSources;

/// <summary>Provides OHLC data from an array.</summary>
/// <param name="prices">The array of OHLC values.</param>
public class OHLCSourceArray(OHLC[] prices) : OHLCSourceBase
{
    /// <summary>The array of OHLC values used by this data source.</summary>
    private readonly OHLC[] _prices = prices;

    /// <summary>Gets the number of OHLC values in the source.</summary>
    public override int Count => _prices.Length;

    /// <summary>Gets the OHLC values in this source.</summary>
    /// <returns>The underlying array of OHLC values.</returns>
    public override IReadOnlyList<OHLC> GetOHLCs() => _prices;
}
