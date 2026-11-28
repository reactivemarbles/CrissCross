// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using System.Reflection;

namespace CrissCross.Plot;

/// <summary>Provides version information for the CrissCross.Plot assembly.</summary>
public static class Version
{
    /// <summary>Gets the major version number.</summary>
    public static int Major => int.Parse(VersionString.Split('-')[0].Split('.')[0]);

    /// <summary>Gets the minor version number.</summary>
    public static int Minor => int.Parse(VersionString.Split('-')[0].Split('.')[1]);

    /// <summary>Gets the build version number.</summary>
    public static int Build => int.Parse(VersionString.Split('-')[0].Split('.')[2]);

    /// <summary>Gets the version formatted like "1.2.3-beta".</summary>
    public static string VersionString { get; } = GetVersionString();

    /// <summary>Gets the version formatted like "ScottPlot 1.2.3-beta".</summary>
    public static string LongString { get; } = $"ScottPlot {GetVersionString()}";

    /// <summary>Throws if this version does not match the expected major version.</summary>
    /// <param name="major">The expected major version.</param>
    public static void ShouldBe(int major)
    {
        if (major == Major)
        {
            return;
        }

        throw new InvalidOperationException($"ScottPlot was expected to be {major}.x.x but is actually {VersionString}");
    }

    /// <summary>Throws if this version does not match the expected major and minor versions.</summary>
    /// <param name="major">The expected major version.</param>
    /// <param name="minor">The expected minor version.</param>
    public static void ShouldBe(int major, int minor)
    {
        if ((major == Major) && (minor == Minor))
        {
            return;
        }

        throw new InvalidOperationException($"ScottPlot was expected to be {major}.{minor}.x but is actually {VersionString}");
    }

    /// <summary>Throws if this version does not match the expected major, minor, and build versions.</summary>
    /// <param name="major">The expected major version.</param>
    /// <param name="minor">The expected minor version.</param>
    /// <param name="build">The expected build version.</param>
    public static void ShouldBe(int major, int minor, int build)
    {
        if ((major == Major) && (minor == Minor) && (build == Build))
        {
            return;
        }

        throw new InvalidOperationException($"ScottPlot was expected to be {major}.{minor}.{build} but is actually {VersionString}");
    }

    /// <summary>Reads the informational version from the assembly metadata.</summary>
    /// <returns>The informational version without build metadata.</returns>
    private static string GetVersionString()
    {
        string v = Assembly.GetAssembly(typeof(Plot))!
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()!
            .InformationalVersion;

        int metadataIndex = v.IndexOf('+');
        return metadataIndex >= 0 ? v.Remove(metadataIndex) : v;
    }
}
