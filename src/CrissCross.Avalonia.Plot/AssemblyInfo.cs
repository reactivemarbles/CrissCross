// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Metadata;

#if REACTIVELIST_REACTIVE
[assembly: XmlnsDefinition("https://github.com/reactivemarbles/CrissCross.ui", "CrissCross.Reactive.Avalonia.Plot")]
#else
[assembly: XmlnsDefinition("https://github.com/reactivemarbles/CrissCross.ui", "CrissCross.Avalonia.Plot")]
#endif
[assembly: XmlnsPrefix("https://github.com/reactivemarbles/CrissCross.ui", "ccui")]
