// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;
using System.Runtime.InteropServices;
using CrissCross.Avalonia.UI.Controls.Automation.Windows;

namespace CrissCross.NavigationView.Tests;

/// <summary>Verifies the native UI Automation interface and marshaling contracts.</summary>
public sealed class WindowsAutomationAbiTests
{
    /// <summary>The native rectangle byte size.</summary>
    private const int RectangleSize = 32;

    /// <summary>The native point byte size.</summary>
    private const int PointSize = 16;

    /// <summary>The native rectangle's left edge.</summary>
    private const double RectangleLeft = 1;

    /// <summary>The native rectangle's top edge.</summary>
    private const double RectangleTop = 2;

    /// <summary>The native rectangle's width.</summary>
    private const double RectangleWidth = 3;

    /// <summary>The native rectangle's height.</summary>
    private const double RectangleHeight = 4;

    /// <summary>Verifies interface identifiers against the Windows SDK IDL.</summary>
    /// <param name="type">The managed interface.</param>
    /// <param name="guid">The native interface identifier.</param>
    /// <returns>The asynchronous assertion task.</returns>
    [Test]
    [Arguments(typeof(IWindowsRawElementProviderSimple), "d6dd68d1-86fd-4332-8666-9abedea2d24c")]
    [Arguments(typeof(IWindowsRawElementProviderFragment), "f7063da8-8359-439c-9297-bbc5299a7d87")]
    [Arguments(typeof(IWindowsRawElementProviderFragmentRoot), "620ce2a5-ab8f-40a9-86cb-de3c75599b58")]
    [Arguments(typeof(IWindowsTextProvider), "3589c92c-63f3-4367-99bb-ada653b77cf2")]
    [Arguments(typeof(IWindowsTextRangeProvider), "5347ad7b-c355-46f8-aff5-909033582f63")]
    [Arguments(typeof(IWindowsInvokeProvider), "54fcb24b-e18e-47a2-b4d3-eccbe77599a2")]
    [Arguments(typeof(IWindowsValueProvider), "c7935180-6fb3-4201-b174-7df73adbf64a")]
    [Arguments(typeof(IWindowsToggleProvider), "56d00bd0-c4f4-433c-a836-1a52a57e0892")]
    [Arguments(typeof(IWindowsRangeValueProvider), "36dc7aef-33e6-4691-afe1-2be7274b3d33")]
    [Arguments(typeof(IWindowsSelectionProvider), "fb8b03af-3bdf-48d4-bd36-1a65793be168")]
    [Arguments(typeof(IWindowsSelectionItemProvider), "2acad808-b2d4-452d-a407-91ff1ad167b2")]
    [Arguments(typeof(IWindowsExpandCollapseProvider), "d847d3a5-cab0-4a98-8c32-ecb45c59ad24")]
    [Arguments(typeof(IWindowsScrollProvider), "b38b8077-1fc3-42a5-8cae-d40c2215055a")]
    [Arguments(typeof(IWindowsScrollItemProvider), "2360c714-4bf1-4b26-ba65-9b21316127eb")]
    [Arguments(typeof(IWindowsGridProvider), "b17d6187-0907-464b-a168-0ef17a1572b1")]
    [Arguments(typeof(IWindowsGridItemProvider), "d02541f1-fb81-4d64-ae32-f520f8a6dbd1")]
    [Arguments(typeof(IWindowsTableProvider), "9c860395-97b3-490a-b52a-858cc22af166")]
    [Arguments(typeof(IWindowsTableItemProvider), "b9734fa6-771f-4d78-9c90-2517999349cd")]
    public async Task InterfaceGuid_MatchesWindowsSdk(Type type, string guid)
    {
        await Assert.That(type.GUID).IsEqualTo(new(guid));
        await Assert.That(type.IsPublic).IsTrue();
        var attribute = (InterfaceTypeAttribute?)Attribute.GetCustomAttribute(type, typeof(InterfaceTypeAttribute));
        await Assert.That(attribute?.Value).IsEqualTo(ComInterfaceType.InterfaceIsIUnknown);
    }

    /// <summary>Verifies geometry contains native doubles in sequential order.</summary>
    /// <returns>The asynchronous assertion task.</returns>
    [Test]
    public async Task Geometry_HasNativeLayout()
    {
        await Assert.That(Marshal.SizeOf<WindowsAutomationRect>()).IsEqualTo(RectangleSize);
        await Assert.That(Marshal.SizeOf<WindowsAutomationPoint>()).IsEqualTo(PointSize);
        var rectangle = new WindowsAutomationRect(RectangleLeft, RectangleTop, RectangleWidth, RectangleHeight);
        await Assert.That(rectangle.Width).IsEqualTo(RectangleWidth);
        await Assert.That(rectangle.Height).IsEqualTo(RectangleHeight);
    }

    /// <summary>Verifies UIA BOOL uses four bytes and rectangle arrays use VT_R8.</summary>
    /// <returns>The asynchronous assertion task.</returns>
    [Test]
    public async Task TextRange_UsesNativeBoolAndDoubleSafeArray()
    {
        var textRangeProviderType = typeof(IWindowsTextRangeProvider);
        var compare = textRangeProviderType.GetMethod(nameof(IWindowsTextRangeProvider.Compare))
            ?? throw new InvalidOperationException("The native comparison method is missing.");
        var compareMarshal = GetSingleMarshalAsAttribute(compare.ReturnParameter);
        await Assert.That(compareMarshal.Value).IsEqualTo(UnmanagedType.Bool);
        var rectangles = textRangeProviderType.GetMethod(nameof(IWindowsTextRangeProvider.GetBoundingRectangles))
            ?? throw new InvalidOperationException("The native rectangle method is missing.");
        var rectanglesMarshal = GetSingleMarshalAsAttribute(rectangles.ReturnParameter);
        await Assert.That(rectanglesMarshal.Value).IsEqualTo(UnmanagedType.SafeArray);
        await Assert.That(rectanglesMarshal.SafeArraySubType).IsEqualTo(VarEnum.VT_R8);
    }

    /// <summary>Verifies provider and range arrays use IUnknown rather than VARIANT.</summary>
    /// <param name="type">The interface.</param>
    /// <param name="methodName">The array method.</param>
    /// <returns>The asynchronous assertion task.</returns>
    [Test]
    [Arguments(typeof(IWindowsTextProvider), nameof(IWindowsTextProvider.GetSelection))]
    [Arguments(typeof(IWindowsRawElementProviderFragment), nameof(IWindowsRawElementProviderFragment.GetEmbeddedFragmentRoots))]
    [Arguments(typeof(IWindowsSelectionProvider), nameof(IWindowsSelectionProvider.GetSelection))]
    public async Task ProviderArrays_UseUnknownSafeArrays(Type type, string methodName)
    {
        var method = type.GetMethod(methodName) ?? throw new InvalidOperationException("The native array method is missing.");
        var marshal = GetSingleMarshalAsAttribute(method.ReturnParameter);
        await Assert.That(marshal.Value).IsEqualTo(UnmanagedType.SafeArray);
        await Assert.That(marshal.SafeArraySubType).IsEqualTo(VarEnum.VT_UNKNOWN);
    }

    /// <summary>Requires exactly one native marshaling contract.</summary>
    /// <param name="parameter">The native parameter or return value.</param>
    /// <returns>The marshaling contract.</returns>
    private static MarshalAsAttribute GetSingleMarshalAsAttribute(ParameterInfo parameter)
    {
        MarshalAsAttribute? result = null;
        foreach (var attribute in parameter.GetCustomAttributes<MarshalAsAttribute>(false))
        {
            if (result is not null)
            {
                throw new InvalidOperationException("The native marshaling attribute is repeated.");
            }

            result = attribute;
        }

        return result ?? throw new InvalidOperationException("The native marshaling attribute is missing.");
    }
}
