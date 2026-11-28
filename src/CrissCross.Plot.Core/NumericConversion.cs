// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace CrissCross.Plot;

/// <summary>
/// This class contains type-specific methods to convert between generic values and doubles
/// optimized for performance using platform-specific features.
/// </summary>
[SuppressMessage("RoslynCommonAnalyzers", "SST1704", Justification = "NumericConversion is a public API name and must remain for compatibility.")]
public static class NumericConversion
{
    /// <summary>Gets the method implementation options used by numeric conversion methods.</summary>
    internal const MethodImplOptions ImplOptions =
#if NETCOREAPP
        MethodImplOptions.AggressiveOptimization
        | MethodImplOptions.AggressiveInlining;
#else
        MethodImplOptions.AggressiveInlining;
#endif

    /// <summary>The earliest year supported by plot date conversions.</summary>
    private const int MinimumDateTimeYear = 100;

    /// <summary>The first year outside the supported plot date range.</summary>
    private const int MaximumDateTimeYear = 10_000;

    /// <summary>The earliest supported OLE Automation date coordinate.</summary>
    private const double MinimumOADate = -657_435;

    /// <summary>The latest supported OLE Automation date coordinate.</summary>
    private const double MaximumOADate = 2_958_466;

    /// <summary>The highest representable year used for date bounds.</summary>
    private const int MaximumPlotDateTimeYear = 9_999;

    /// <summary>Converts an array of generic values to a double array.</summary>
    /// <typeparam name="T">The type of values in the array.</typeparam>
    /// <param name="values">The values to convert.</param>
    /// <returns>A double array containing the converted values.</returns>
    [MethodImpl(ImplOptions)]
    public static double[] GenericToDoubleArray<T>(T[] values)
    {
        double[] values2 = new double[values.Length];

        for (int i = 0; i < values.Length; i++)
        {
            values2[i] = GenericToDouble(values, i);
        }

        return values2;
    }

    /// <summary>Converts an enumerable of generic values to a double array.</summary>
    /// <typeparam name="T">The type of values in the sequence.</typeparam>
    /// <param name="values">The values to convert.</param>
    /// <returns>A double array containing the converted values.</returns>
    [MethodImpl(ImplOptions)]
    public static double[] GenericToDoubleArray<T>(IEnumerable<T> values)
    {
        ArgumentChecks.ThrowIfNull(values, nameof(values));

        List<double> converted = [];
        foreach (T value in values)
        {
            T item = value;
            converted.Add(GenericToDouble(ref item));
        }

        return converted.ToArray();
    }

    /// <summary>Returns the double value of a <typeparamref name="T"/> using a conversion technique optimized for the platform.</summary>
    /// <typeparam name="T">The type of value to convert.</typeparam>
    /// <param name="value">The value to convert.</param>
    /// <returns>The converted double value.</returns>
    [MethodImpl(ImplOptions)]
    public static double GenericToDouble<T>(ref T value) => value switch
    {
        double vDouble => vDouble,
        float vSingle => Convert.ToDouble(vSingle),
        int vInt32 => Convert.ToDouble(vInt32),
        uint vUint32 => Convert.ToDouble(vUint32),
        long vInt64 => Convert.ToDouble(vInt64),
        ulong vUint64 => Convert.ToDouble(vUint64),
        short vInt16 => Convert.ToDouble(vInt16),
        ushort vUint16 => Convert.ToDouble(vUint16),
        decimal vDecimal => Convert.ToDouble(vDecimal),
        byte vByte => Convert.ToDouble(vByte),
        DateTime vDateTime => ToNumber(vDateTime),
        _ => Convert.ToDouble(value),
    };

    /// <summary>Returns the double value at the specified position in a list.</summary>
    /// <typeparam name="T">The type of value in the list.</typeparam>
    /// <param name="list">The list containing the value.</param>
    /// <param name="i">The position of the value.</param>
    /// <returns>The converted double value.</returns>
    [MethodImpl(ImplOptions)]
    public static double GenericToDouble<T>(IReadOnlyList<T> list, int i)
    {
        var v = list[i];
        return GenericToDouble(ref v);
    }

    /// <summary>Returns the double value at the specified position in an array.</summary>
    /// <typeparam name="T">The type of value in the array.</typeparam>
    /// <param name="array">The array containing the value.</param>
    /// <param name="i">The position of the value.</param>
    /// <returns>The converted double value.</returns>
    [MethodImpl(ImplOptions)]
    public static double GenericToDouble<T>(T[] array, int i)
    {
        var v = array[i];
        return GenericToDouble(ref v);
    }

    /// <summary>Converts a pair of generic values to coordinates.</summary>
    /// <typeparam name="T1">The type of the x value.</typeparam>
    /// <typeparam name="T2">The type of the y value.</typeparam>
    /// <param name="x">The x value to convert.</param>
    /// <param name="y">The y value to convert.</param>
    /// <returns>The converted coordinates.</returns>
    [MethodImpl(ImplOptions)]
    public static Coordinates GenericToCoordinates<T1, T2>(ref T1 x, ref T2 y) => new(GenericToDouble(ref x), GenericToDouble(ref y));

    /// <summary>Converts matching sequences of generic x and y values to coordinates.</summary>
    /// <typeparam name="T1">The type of the x values.</typeparam>
    /// <typeparam name="T2">The type of the y values.</typeparam>
    /// <param name="xs">The x values to convert.</param>
    /// <param name="ys">The y values to convert.</param>
    /// <returns>An array of converted coordinates.</returns>
    /// <exception cref="ArgumentException">The sequences do not contain the same number of values.</exception>
    [MethodImpl(ImplOptions)]
    public static Coordinates[] GenericToCoordinates<T1, T2>(IEnumerable<T1> xs, IEnumerable<T2> ys)
    {
        double[] xs2 = GenericToDoubleArray(xs);
        double[] ys2 = GenericToDoubleArray(ys);
        if (xs2.Length != ys2.Length)
        {
            throw new ArgumentException($"{nameof(xs)} and {nameof(ys)} must have equal length");
        }

        Coordinates[] coordinates = new Coordinates[xs2.Length];
        for (int i = 0; i < coordinates.Length; i++)
        {
            coordinates[i] = new(xs2[i], ys2[i]);
        }

        return coordinates;
    }

    /// <summary>Creates a <typeparamref name="T"/> for a given double <paramref name="value"/> using a conversion technique optimized for the platform.</summary>
    /// <typeparam name="T">The type of value to create.</typeparam>
    /// <param name="value">The double value to convert.</param>
    /// <param name="v">The converted value.</param>
    [MethodImpl(ImplOptions)]
    public static void DoubleToGeneric<T>(double value, out T v) => v = Type.GetTypeCode(typeof(T)) switch
    {
            TypeCode.Double => (T)(object)value,
            TypeCode.Single => (T)(object)Convert.ToSingle(value),
            TypeCode.Int32 => (T)(object)Convert.ToInt32(value),
            TypeCode.UInt32 => (T)(object)Convert.ToUInt32(value),
            TypeCode.Int64 => (T)(object)Convert.ToInt64(value),
            TypeCode.UInt64 => (T)(object)Convert.ToUInt64(value),
            TypeCode.Int16 => (T)(object)Convert.ToInt16(value),
            TypeCode.UInt16 => (T)(object)Convert.ToUInt16(value),
            TypeCode.Decimal => (T)(object)Convert.ToDecimal(value),
            TypeCode.Byte => (T)(object)Convert.ToByte(value),
            TypeCode.DateTime => (T)(object)DateTime.FromOADate(value),
                _ => (T)Convert.ChangeType(value, typeof(T)),
            };

            /// <summary>Creates a <typeparamref name="T"/> for a given double value.</summary>
    /// <typeparam name="T">The type of value to create.</typeparam>
    /// <param name="value">The double value to convert.</param>
    /// <returns>The converted value.</returns>
    [SuppressMessage("RoslynCommonAnalyzers", "SST2307", Justification = "The caller selects the target type through the required generic type argument.")]
    [MethodImpl(ImplOptions)]
    public static T DoubleToGeneric<T>(double value)
    {
        DoubleToGeneric(value, out T converted);
        return converted;
    }

    /// <summary>Extension members for <c>double[]</c>.</summary>
    /// <param name="input">The array to convert.</param>
    extension(double[] input)
    {
        /// <summary>Converts every double in the array to a generic type.</summary>
        /// <typeparam name="T">The target element type.</typeparam>
        /// <returns>A generic array containing the converted values.</returns>
        [SuppressMessage("RoslynCommonAnalyzers", "SST2307", Justification = "The caller selects the target type through the required generic type argument.")]
        public T[] DoubleToGeneric<T>()
        {
            T[] result = new T[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                DoubleToGeneric(input[i], out result[i]);
            }

            return result;
        }

        /// <summary>Converts every double in the array to a generic type.</summary>
        /// <typeparam name="T">The target element type.</typeparam>
        /// <returns>A generic array containing the converted values.</returns>
        [SuppressMessage("RoslynCommonAnalyzers", "SST2307", Justification = "The caller selects the target type through the required generic type argument.")]
        public T[] ToGenericArray<T>()
        {
            T[] result = new T[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                DoubleToGeneric(input[i], out result[i]);
            }

            return result;
        }
    }

    /// <summary>Adds two byte values.</summary>
    /// <param name="a">The first value.</param>
    /// <param name="b">The second value.</param>
    /// <returns>The addition result, cast to a byte.</returns>
    public static byte AddBytes(byte a, byte b) => (byte)(a + b);

    /// <summary>Multiplies two byte values.</summary>
    /// <param name="a">The first value.</param>
    /// <param name="b">The second value.</param>
    /// <returns>The multiplication result, cast to a byte.</returns>
    public static byte Multiply(byte a, byte b) => (byte)(a * b);

    /// <summary>Subtracts two byte values.</summary>
    /// <param name="a">The first value.</param>
    /// <param name="b">The second value.</param>
    /// <returns>The subtraction result, cast to a byte.</returns>
    public static byte SubtractBytes(byte a, byte b) => (byte)(a - b);

    /// <summary>Determines whether one byte is less than or equal to another.</summary>
    /// <param name="a">The first value.</param>
    /// <param name="b">The second value.</param>
    /// <returns><see langword="true"/> if <paramref name="a"/> is less than or equal to <paramref name="b"/>; otherwise <see langword="false"/>.</returns>
    public static bool LessThanOrEqualBytes(byte a, byte b) => a <= b;

    /// <summary>Creates a function that adds two values of type <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <returns>A function that adds two values.</returns>
    [SuppressMessage("RoslynCommonAnalyzers", "SST2307", Justification = "The caller selects the operator value type through the required generic type argument.")]
    public static Func<T, T, T> CreateAddFunction<T>()
    {
        ParameterExpression paramA = Expression.Parameter(typeof(T), "a");
        ParameterExpression paramB = Expression.Parameter(typeof(T), "b");

        BinaryExpression body = Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Byte => Expression.Add(paramA, paramB, typeof(NumericConversion).GetMethod(nameof(NumericConversion.AddBytes))),
            _ => Expression.Add(paramA, paramB),
        };

        return Expression.Lambda<Func<T, T, T>>(body, paramA, paramB).Compile();
    }

    /// <summary>Creates a function that multiplies two values of type <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <returns>A function that multiplies two values.</returns>
    [SuppressMessage("RoslynCommonAnalyzers", "SST2307", Justification = "The caller selects the operator value type through the required generic type argument.")]
    public static Func<T, T, T> CreateMultFunction<T>()
    {
        ParameterExpression paramA = Expression.Parameter(typeof(T), "a");
        ParameterExpression paramB = Expression.Parameter(typeof(T), "b");

        BinaryExpression body = Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Byte => Expression.Multiply(paramA, paramB, typeof(NumericConversion).GetMethod(nameof(NumericConversion.Multiply))),
            _ => Expression.Multiply(paramA, paramB),
        };

        return Expression.Lambda<Func<T, T, T>>(body, paramA, paramB).Compile();
    }

    /// <summary>Creates a function that subtracts two values of type <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <returns>A function that subtracts two values.</returns>
    [SuppressMessage("RoslynCommonAnalyzers", "SST2307", Justification = "The caller selects the operator value type through the required generic type argument.")]
    public static Func<T, T, T> CreateSubtractFunction<T>()
    {
        ParameterExpression paramA = Expression.Parameter(typeof(T), "a");
        ParameterExpression paramB = Expression.Parameter(typeof(T), "b");

        BinaryExpression body = Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Byte => Expression.Subtract(paramA, paramB, typeof(NumericConversion).GetMethod(nameof(NumericConversion.SubtractBytes))),
            _ => Expression.Subtract(paramA, paramB),
        };

        return Expression.Lambda<Func<T, T, T>>(body, paramA, paramB).Compile();
    }

    /// <summary>Creates a function that compares two values of type <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <returns>A function that determines whether the first value is less than or equal to the second.</returns>
    [SuppressMessage("RoslynCommonAnalyzers", "SST2307", Justification = "The caller selects the operator value type through the required generic type argument.")]
    public static Func<T, T, bool> CreateLessThanOrEqualFunction<T>()
    {
        ParameterExpression paramA = Expression.Parameter(typeof(T), "a");
        ParameterExpression paramB = Expression.Parameter(typeof(T), "b");

        BinaryExpression body = Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Byte => Expression.LessThanOrEqual(paramA, paramB, false, typeof(NumericConversion).GetMethod(nameof(NumericConversion.LessThanOrEqualBytes))),
            _ => Expression.LessThanOrEqual(paramA, paramB),
        };

        return Expression.Lambda<Func<T, T, bool>>(body, paramA, paramB).Compile();
    }

    /// <summary>
    /// NOTE: Implementing Clamp() is for .NET versions less than 6 which do not have Math.Clamp() available.
    /// Type-specific overloads are included below to enhance performance for common datatypes.
    /// </summary>
    /// <typeparam name="T">The comparable value type.</typeparam>
    /// <param name="input">The value to clamp.</param>
    /// <param name="min">The inclusive minimum.</param>
    /// <param name="max">The inclusive maximum.</param>
    /// <returns>The clamped value.</returns>
    public static T Clamp<T>(T input, T min, T max)
        where T : IComparable
    {
        if (input.CompareTo(min) < 0)
        {
            return min;
        }

        return input.CompareTo(max) > 0 ? max : input;
    }

    /// <summary>Restricts a signed byte to an inclusive range.</summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The inclusive minimum.</param>
    /// <param name="max">The inclusive maximum.</param>
    /// <returns>The clamped value.</returns>
    [MethodImpl(ImplOptions)]
    public static sbyte Clamp(sbyte value, sbyte min, sbyte max)
    {
        if (value < min)
        {
            return min;
        }

        return value > max ? max : value;
    }

    /// <summary>Restricts a byte to an inclusive range.</summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The inclusive minimum.</param>
    /// <param name="max">The inclusive maximum.</param>
    /// <returns>The clamped value.</returns>
    [MethodImpl(ImplOptions)]
    public static byte Clamp(byte value, byte min, byte max)
    {
        if (value < min)
        {
            return min;
        }

        return value > max ? max : value;
    }

    /// <summary>Restricts a 16-bit signed integer to an inclusive range.</summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The inclusive minimum.</param>
    /// <param name="max">The inclusive maximum.</param>
    /// <returns>The clamped value.</returns>
    [MethodImpl(ImplOptions)]
    public static short Clamp(short value, short min, short max)
    {
        if (value < min)
        {
            return min;
        }

        return value > max ? max : value;
    }

    /// <summary>Restricts a 16-bit unsigned integer to an inclusive range.</summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The inclusive minimum.</param>
    /// <param name="max">The inclusive maximum.</param>
    /// <returns>The clamped value.</returns>
    [MethodImpl(ImplOptions)]
    public static ushort Clamp(ushort value, ushort min, ushort max)
    {
        if (value < min)
        {
            return min;
        }

        return value > max ? max : value;
    }

    /// <summary>Restricts a 32-bit signed integer to an inclusive range.</summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The inclusive minimum.</param>
    /// <param name="max">The inclusive maximum.</param>
    /// <returns>The clamped value.</returns>
    [MethodImpl(ImplOptions)]
    public static int Clamp(int value, int min, int max)
    {
        if (value < min)
        {
            return min;
        }

        return value > max ? max : value;
    }

    /// <summary>Restricts a 32-bit unsigned integer to an inclusive range.</summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The inclusive minimum.</param>
    /// <param name="max">The inclusive maximum.</param>
    /// <returns>The clamped value.</returns>
    [MethodImpl(ImplOptions)]
    public static uint Clamp(uint value, uint min, uint max)
    {
        if (value < min)
        {
            return min;
        }

        return value > max ? max : value;
    }

    /// <summary>Restricts a 64-bit signed integer to an inclusive range.</summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The inclusive minimum.</param>
    /// <param name="max">The inclusive maximum.</param>
    /// <returns>The clamped value.</returns>
    [MethodImpl(ImplOptions)]
    public static long Clamp(long value, long min, long max)
    {
        if (value < min)
        {
            return min;
        }

        return value > max ? max : value;
    }

    /// <summary>Restricts a 64-bit unsigned integer to an inclusive range.</summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The inclusive minimum.</param>
    /// <param name="max">The inclusive maximum.</param>
    /// <returns>The clamped value.</returns>
    [MethodImpl(ImplOptions)]
    public static ulong Clamp(ulong value, ulong min, ulong max)
    {
        if (value < min)
        {
            return min;
        }

        return value > max ? max : value;
    }

    /// <summary>Restricts a single-precision value to an inclusive range.</summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The inclusive minimum.</param>
    /// <param name="max">The inclusive maximum.</param>
    /// <returns>The clamped value.</returns>
    [MethodImpl(ImplOptions)]
    public static float Clamp(float value, float min, float max)
    {
        if (value < min)
        {
            return min;
        }

        return value > max ? max : value;
    }

    /// <summary>Restricts a double-precision value to an inclusive range.</summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The inclusive minimum.</param>
    /// <param name="max">The inclusive maximum.</param>
    /// <returns>The clamped value.</returns>
    [MethodImpl(ImplOptions)]
    public static double Clamp(double value, double min, double max)
    {
        if (value < min)
        {
            return min;
        }

        return value > max ? max : value;
    }

    /// <summary>Restricts a decimal value to an inclusive range.</summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The inclusive minimum.</param>
    /// <param name="max">The inclusive maximum.</param>
    /// <returns>The clamped value.</returns>
    [MethodImpl(ImplOptions)]
    public static decimal Clamp(decimal value, decimal min, decimal max)
    {
        if (value < min)
        {
            return min;
        }

        return value > max ? max : value;
    }

    /// <summary>Determines whether both values are finite real numbers.</summary>
    /// <param name="x">The first value.</param>
    /// <param name="y">The second value.</param>
    /// <returns><see langword="true"/> if both values are finite; otherwise <see langword="false"/>.</returns>
    public static bool AreReal(double x, double y)
    {
        // implemented here because older versions of .NET do not support double.IsReal()
        return IsReal(x) && IsReal(y);
    }

    /// <summary>Determines whether a double-precision value is finite.</summary>
    /// <param name="x">The value to inspect.</param>
    /// <returns><see langword="true"/> if the value is finite; otherwise <see langword="false"/>.</returns>
    public static bool IsReal(double x)
    {
        // implemented here because older versions of .NET do not support double.IsReal()
        return !double.IsNaN(x) && !double.IsInfinity(x);
    }

    /// <summary>Determines whether a single-precision value is finite.</summary>
    /// <param name="x">The value to inspect.</param>
    /// <returns><see langword="true"/> if the value is finite; otherwise <see langword="false"/>.</returns>
    public static bool IsReal(float x)
    {
        // implemented here because older versions of .NET do not support double.IsReal()
        return !float.IsNaN(x) && !float.IsInfinity(x);
    }

    /// <summary>Converts between <see cref="DateTime"/> values and coordinates represented as doubles.</summary>
    /// <remarks>
    /// Converting between doubles and OLE Automation dates has limitations, including a restricted range.
    /// Keeping this conversion centralized allows its implementation to change without updating every call site.
    /// </remarks>
    /// <param name="value">The coordinate value to convert.</param>
    /// <returns>The corresponding date and time.</returns>
    [SuppressMessage("RoslynCommonAnalyzers", "SST2016", Justification = "DateTime is part of the public API and conversion behavior must remain compatible.")]
    public static DateTime ToDateTime(double value)
    {
        if (value <= MinimumOADate)
        {
            return new(MinimumDateTimeYear, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
        }

        return value >= MaximumOADate
            ? new(9_999, 1, 1, 0, 0, 0, DateTimeKind.Unspecified)
            : DateTime.FromOADate(value);
    }

    /// <summary>Converts a DateTime into a number that can be plotted on a numeric axis.</summary>
    /// <param name="value">The date and time to convert.</param>
    /// <returns>The OLE Automation date, adjusted to the supported plotting range.</returns>
    [SuppressMessage("RoslynCommonAnalyzers", "SST2016", Justification = "DateTime is part of the public API and conversion behavior must remain compatible.")]
    public static double ToNumber(DateTime value)
    {
        if (value.Year < MinimumDateTimeYear)
        {
            return new DateTime(MinimumDateTimeYear, 1, 1, 0, 0, 0, DateTimeKind.Unspecified).ToOADate();
        }

        return value.Year >= MaximumDateTimeYear
            ? new DateTime(MaximumDateTimeYear, 1, 1, 0, 0, 0, DateTimeKind.Unspecified).ToOADate()
            : value.ToOADate();
    }

    /// <summary>Returns the next representable double value by incrementing the 64-bit representation.</summary>
    /// <param name="value">The value to increment.</param>
    /// <returns>The next representable double value.</returns>
    public static double IncrementLargeDouble(double value)
    {
        long bits = BitConverter.DoubleToInt64Bits(value);
        long nextBits = bits + 1;
        return BitConverter.Int64BitsToDouble(nextBits);
    }

    /// <summary>Returns the previous representable double value by decrementing the 64-bit representation.</summary>
    /// <param name="value">The value to decrement.</param>
    /// <returns>The previous representable double value.</returns>
    public static double DecrementLargeDouble(double value)
    {
        long bits = BitConverter.DoubleToInt64Bits(value);
        long nextBits = bits - 1;
        return BitConverter.Int64BitsToDouble(nextBits);
    }
}
