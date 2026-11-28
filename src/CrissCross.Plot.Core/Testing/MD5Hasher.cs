// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace CrissCross.Plot.Testing;

/// <summary>Creates MD5 digests for compatibility scenarios.</summary>
public static class MD5Hasher
{
    /// <summary>The number of hexadecimal characters used for each byte.</summary>
    private const int HexCharactersPerByte = 2;

    /// <summary>Returns the lowercase hexadecimal MD5 digest of the specified input.</summary>
    /// <param name="input">The string to hash.</param>
    /// <returns>The lowercase hexadecimal MD5 digest.</returns>
    [SuppressMessage(
        "Security",
        "CA5351:Do Not Use Broken Cryptographic Algorithms",
        Justification = "This explicitly named compatibility helper returns MD5 digests and must not be used for security; changing algorithms would break its documented output.")]
    public static string GetHash(string input)
    {
#if NET8_0_OR_GREATER
        byte[] data = MD5.HashData(Encoding.UTF8.GetBytes(input));
#else
        using MD5 md5Hash = MD5.Create();
        byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
#endif
        StringBuilder hash = new(data.Length * HexCharactersPerByte);
        foreach (byte value in data)
        {
            _ = hash.Append(value.ToString("x2", CultureInfo.InvariantCulture));
        }

        return hash.ToString();
    }
}
