// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
[assembly: XmlnsDefinition("https://github.com/reactivemarbles/CrissCross", "CrissCross.Reactive")]
[assembly: XmlnsDefinition("https://github.com/reactivemarbles/CrissCross", "CrissCross.Reactive.MAUI")]
#else
[assembly: XmlnsDefinition("https://github.com/reactivemarbles/CrissCross", "CrissCross")]
[assembly: XmlnsDefinition("https://github.com/reactivemarbles/CrissCross", "CrissCross.MAUI")]
#endif
[assembly: Microsoft.Maui.Controls.XmlnsPrefix("https://github.com/reactivemarbles/CrissCross", "rxNav")]
