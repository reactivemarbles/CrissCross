// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Represents a control that displays a calendar and allows users to select dates or date ranges.</summary>
/// <remarks>The Calendar control provides a visual interface for date selection, supporting single or multiple
/// date selection modes. It can be used for scenarios such as scheduling, date picking, or displaying events. The
/// control supports customization of appearance and behavior through various properties and events.</remarks>
public class Calendar : global::Avalonia.Controls.Calendar;
