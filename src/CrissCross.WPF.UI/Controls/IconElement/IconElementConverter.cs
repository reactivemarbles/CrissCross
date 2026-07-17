// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.WPF.UI.Extensions;
#else
using CrissCross.WPF.UI.Extensions;
#endif

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Tries to convert SymbolRegular and <seealso cref="SymbolFilled"/> to SymbolRegular.</summary>
public class IconElementConverter : TypeConverter
{
    /// <summary>Returns whether this converter can convert an object of the given type to the type of this converter,
    /// using the specified context.</summary>
    /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format
    /// context.</param>
    /// <param name="sourceType">A <see cref="T:System.Type" /> that represents the type you want to convert
    /// from.</param>
    /// <returns>
    /// true if this converter can perform the conversion; otherwise, false.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(SymbolRegular) || sourceType == typeof(SymbolFilled);
    }

    /// <summary>Provides the CanConvertTo member.</summary>
    /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format
    /// context.</param>
    /// <param name="destinationType">A <see cref="T:System.Type" /> that represents the type you want to convert
    /// to.</param>
    /// <returns>
    /// true if this converter can perform the conversion; otherwise, false.
    /// </returns>
    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType) => false;

    /// <summary>Provides the ConvertFrom member.</summary>
    /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format
    /// context.</param>
    /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo" /> to use as the current
    /// culture.</param>
    /// <param name="value">The <see cref="T:System.Object" /> to convert.</param>
    /// <returns>
    /// An <see cref="T:System.Object" /> that represents the converted value.
    /// </returns>
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object? value) =>
        value switch
        {
            SymbolRegular symbolRegular => new SymbolIcon(symbolRegular),
            SymbolFilled symbolFilled => new SymbolIcon(symbolFilled.Swap(), SymbolIcon.DefaultFontSize, true),
            _ => null,
        };

    /// <summary>Provides the ConvertTo member.</summary>
    /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format
    /// context.</param>
    /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo" />. If null is passed, the current
    /// culture is assumed.</param>
    /// <param name="value">The <see cref="T:System.Object" /> to convert.</param>
    /// <param name="destinationType">The <see cref="T:System.Type" /> to convert the <paramref name="value" />
    /// parameter to.</param>
    /// <returns>
    /// An <see cref="T:System.Object" /> that represents the converted value.
    /// </returns>
    public override object ConvertTo(
        ITypeDescriptorContext? context,
        CultureInfo? culture,
        object? value,
        Type destinationType) => throw GetConvertFromException(value);
}
