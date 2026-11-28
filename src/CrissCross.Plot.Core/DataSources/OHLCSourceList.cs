// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.DataSources;

/// <summary>Provides OHLC data from a list.</summary>
/// <param name="prices">The list of OHLC values.</param>
public class OHLCSourceList(List<OHLC> prices) : OHLCSourceBase
{
    /// <summary>The list of OHLC values used by this data source.</summary>
    private readonly List<OHLC> _prices = prices;

    /// <summary>Gets the number of OHLC values in the source.</summary>
    public override int Count => _prices.Count;

    /// <summary>Gets the OHLC values in this source.</summary>
    /// <returns>The underlying list of OHLC values.</returns>
    public override IReadOnlyList<OHLC> GetOHLCs() => _prices;
}
