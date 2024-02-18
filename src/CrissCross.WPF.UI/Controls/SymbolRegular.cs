// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

////   This file has been borrowed from Wpf-UI.

//// This Source Code Form is subject to the terms of the MIT License.
//// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
//// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
//// All Rights Reserved.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Represents a list of regular Fluent System Icons <c>v.1.1.226</c>.
/// <para>May be converted to <see langword="char"/> using <c>GetGlyph()</c> or to <see langword="string"/> using <c>GetString()</c>.</para>
/// </summary>
#pragma warning disable CS1591
public enum SymbolRegular
{
    /// <summary>
    /// Actually, this icon is not empty, but makes it easier to navigate.
    /// </summary>
    Empty = 0x0,
    /// <summary>
    /// The access time20.
    /// </summary>
    AccessTime20 = 0xE000,
    /// <summary>
    /// The accessibility32.
    /// </summary>
    Accessibility32 = 0xE001,
    /// <summary>
    /// The accessibility48.
    /// </summary>
    Accessibility48 = 0xE002,
    /// <summary>
    /// The accessibility checkmark20.
    /// </summary>
    AccessibilityCheckmark20 = 0xE003,
    /// <summary>
    /// The accessibility checkmark24.
    /// </summary>
    AccessibilityCheckmark24 = 0xE004,
    /// <summary>
    /// The add circle16.
    /// </summary>
    AddCircle16 = 0xE005,
    /// <summary>
    /// The add circle32.
    /// </summary>
    AddCircle32 = 0xE006,
    /// <summary>
    /// The add square20.
    /// </summary>
    AddSquare20 = 0xE007,
    /// <summary>
    /// The add square multiple16.
    /// </summary>
    AddSquareMultiple16 = 0xE008,
    /// <summary>
    /// The add square multiple20.
    /// </summary>
    AddSquareMultiple20 = 0xE009,
    /// <summary>
    /// The add subtract circle16.
    /// </summary>
    AddSubtractCircle16 = 0xE00A,
    /// <summary>
    /// The add subtract circle20.
    /// </summary>
    AddSubtractCircle20 = 0xE00B,
    /// <summary>
    /// The add subtract circle24.
    /// </summary>
    AddSubtractCircle24 = 0xE00C,
    /// <summary>
    /// The add subtract circle28.
    /// </summary>
    AddSubtractCircle28 = 0xE00D,
    /// <summary>
    /// The add subtract circle48.
    /// </summary>
    AddSubtractCircle48 = 0xE00E,
    /// <summary>
    /// The album20.
    /// </summary>
    Album20 = 0xE00F,
    /// <summary>
    /// The album24.
    /// </summary>
    Album24 = 0xE010,
    /// <summary>
    /// The album add20.
    /// </summary>
    AlbumAdd20 = 0xE011,
    /// <summary>
    /// The album add24.
    /// </summary>
    AlbumAdd24 = 0xE012,
    /// <summary>
    /// The alert12.
    /// </summary>
    Alert12 = 0xE013,
    /// <summary>
    /// The alert16.
    /// </summary>
    Alert16 = 0xE014,
    /// <summary>
    /// The alert32.
    /// </summary>
    Alert32 = 0xE015,
    /// <summary>
    /// The alert48.
    /// </summary>
    Alert48 = 0xE016,
    /// <summary>
    /// The alert badge16.
    /// </summary>
    AlertBadge16 = 0xE017,
    /// <summary>
    /// The alert badge20.
    /// </summary>
    AlertBadge20 = 0xE018,
    /// <summary>
    /// The alert badge24.
    /// </summary>
    AlertBadge24 = 0xE019,
    /// <summary>
    /// The alert on20.
    /// </summary>
    AlertOn20 = 0xE01A,
    /// <summary>
    /// The alert snooze12.
    /// </summary>
    AlertSnooze12 = 0xE01B,
    /// <summary>
    /// The alert snooze16.
    /// </summary>
    AlertSnooze16 = 0xE01C,
    /// <summary>
    /// The alert urgent16.
    /// </summary>
    AlertUrgent16 = 0xE01D,
    /// <summary>
    /// The align bottom16.
    /// </summary>
    AlignBottom16 = 0xE01E,
    /// <summary>
    /// The align bottom20.
    /// </summary>
    AlignBottom20 = 0xE01F,
    /// <summary>
    /// The align bottom24.
    /// </summary>
    AlignBottom24 = 0xE020,
    /// <summary>
    /// The align bottom28.
    /// </summary>
    AlignBottom28 = 0xE021,
    /// <summary>
    /// The align bottom32.
    /// </summary>
    AlignBottom32 = 0xE022,
    /// <summary>
    /// The align bottom48.
    /// </summary>
    AlignBottom48 = 0xE023,
    /// <summary>
    /// The align center horizontal16.
    /// </summary>
    AlignCenterHorizontal16 = 0xE024,
    /// <summary>
    /// The align center horizontal20.
    /// </summary>
    AlignCenterHorizontal20 = 0xE025,
    /// <summary>
    /// The align center horizontal24.
    /// </summary>
    AlignCenterHorizontal24 = 0xE026,
    /// <summary>
    /// The align center horizontal28.
    /// </summary>
    AlignCenterHorizontal28 = 0xE027,
    /// <summary>
    /// The align center horizontal32.
    /// </summary>
    AlignCenterHorizontal32 = 0xE028,
    /// <summary>
    /// The align center horizontal48.
    /// </summary>
    AlignCenterHorizontal48 = 0xE029,
    /// <summary>
    /// The align center vertical16.
    /// </summary>
    AlignCenterVertical16 = 0xE02A,
    /// <summary>
    /// The align center vertical20.
    /// </summary>
    AlignCenterVertical20 = 0xE02B,
    /// <summary>
    /// The align center vertical24.
    /// </summary>
    AlignCenterVertical24 = 0xE02C,
    /// <summary>
    /// The align center vertical28.
    /// </summary>
    AlignCenterVertical28 = 0xE02D,
    /// <summary>
    /// The align center vertical32.
    /// </summary>
    AlignCenterVertical32 = 0xE02E,
    /// <summary>
    /// The align center vertical48.
    /// </summary>
    AlignCenterVertical48 = 0xE02F,
    /// <summary>
    /// The align end horizontal20.
    /// </summary>
    AlignEndHorizontal20 = 0xE030,
    /// <summary>
    /// The align end vertical20.
    /// </summary>
    AlignEndVertical20 = 0xE031,
    /// <summary>
    /// The align left16.
    /// </summary>
    AlignLeft16 = 0xE032,
    /// <summary>
    /// The align left20.
    /// </summary>
    AlignLeft20 = 0xE033,
    /// <summary>
    /// The align left24.
    /// </summary>
    AlignLeft24 = 0xE034,
    /// <summary>
    /// The align left28.
    /// </summary>
    AlignLeft28 = 0xE035,
    /// <summary>
    /// The align left32.
    /// </summary>
    AlignLeft32 = 0xE036,
    /// <summary>
    /// The align left48.
    /// </summary>
    AlignLeft48 = 0xE037,
    /// <summary>
    /// The align right16.
    /// </summary>
    AlignRight16 = 0xE038,
    /// <summary>
    /// The align right20.
    /// </summary>
    AlignRight20 = 0xE039,
    /// <summary>
    /// The align right24.
    /// </summary>
    AlignRight24 = 0xE03A,
    /// <summary>
    /// The align right28.
    /// </summary>
    AlignRight28 = 0xE03B,
    /// <summary>
    /// The align right32.
    /// </summary>
    AlignRight32 = 0xE03C,
    /// <summary>
    /// The align right48.
    /// </summary>
    AlignRight48 = 0xE03D,
    /// <summary>
    /// The align space around horizontal20.
    /// </summary>
    AlignSpaceAroundHorizontal20 = 0xE03E,
    /// <summary>
    /// The align space around vertical20.
    /// </summary>
    AlignSpaceAroundVertical20 = 0xE03F,
    /// <summary>
    /// The align space between horizontal20.
    /// </summary>
    AlignSpaceBetweenHorizontal20 = 0xE040,
    /// <summary>
    /// The align space between vertical20.
    /// </summary>
    AlignSpaceBetweenVertical20 = 0xE041,
    /// <summary>
    /// The align space evenly horizontal20.
    /// </summary>
    AlignSpaceEvenlyHorizontal20 = 0xE042,
    /// <summary>
    /// The align space evenly vertical20.
    /// </summary>
    AlignSpaceEvenlyVertical20 = 0xE043,
    /// <summary>
    /// The align space fit vertical20.
    /// </summary>
    AlignSpaceFitVertical20 = 0xE044,
    /// <summary>
    /// The align start horizontal20.
    /// </summary>
    AlignStartHorizontal20 = 0xE045,
    /// <summary>
    /// The align start vertical20.
    /// </summary>
    AlignStartVertical20 = 0xE046,
    /// <summary>
    /// The align stretch horizontal20.
    /// </summary>
    AlignStretchHorizontal20 = 0xE047,
    /// <summary>
    /// The align stretch vertical20.
    /// </summary>
    AlignStretchVertical20 = 0xE048,
    /// <summary>
    /// The align top16.
    /// </summary>
    AlignTop16 = 0xE049,
    /// <summary>
    /// The align top20.
    /// </summary>
    AlignTop20 = 0xE04A,
    /// <summary>
    /// The align top24.
    /// </summary>
    AlignTop24 = 0xE04B,
    /// <summary>
    /// The align top28.
    /// </summary>
    AlignTop28 = 0xE04C,
    /// <summary>
    /// The align top32.
    /// </summary>
    AlignTop32 = 0xE04D,
    /// <summary>
    /// The align top48.
    /// </summary>
    AlignTop48 = 0xE04E,
    /// <summary>
    /// The animal dog16.
    /// </summary>
    AnimalDog16 = 0xE04F,
    /// <summary>
    /// The animal rabbit16.
    /// </summary>
    AnimalRabbit16 = 0xE050,
    /// <summary>
    /// The animal rabbit20.
    /// </summary>
    AnimalRabbit20 = 0xE051,
    /// <summary>
    /// The animal rabbit24.
    /// </summary>
    AnimalRabbit24 = 0xE052,
    /// <summary>
    /// The animal rabbit28.
    /// </summary>
    AnimalRabbit28 = 0xE053,
    /// <summary>
    /// The animal turtle16.
    /// </summary>
    AnimalTurtle16 = 0xE054,
    /// <summary>
    /// The animal turtle20.
    /// </summary>
    AnimalTurtle20 = 0xE055,
    /// <summary>
    /// The animal turtle24.
    /// </summary>
    AnimalTurtle24 = 0xE056,
    /// <summary>
    /// The animal turtle28.
    /// </summary>
    AnimalTurtle28 = 0xE057,
    /// <summary>
    /// The application folder16.
    /// </summary>
    AppFolder16 = 0xE058,
    /// <summary>
    /// The application folder28.
    /// </summary>
    AppFolder28 = 0xE059,
    /// <summary>
    /// The application folder32.
    /// </summary>
    AppFolder32 = 0xE05A,
    /// <summary>
    /// The application folder48.
    /// </summary>
    AppFolder48 = 0xE05B,
    /// <summary>
    /// The application generic20.
    /// </summary>
    AppGeneric20 = 0xE05C,
    /// <summary>
    /// The application recent20.
    /// </summary>
    AppRecent20 = 0xE05D,
    /// <summary>
    /// The application title20.
    /// </summary>
    AppTitle20 = 0xE05E,
    /// <summary>
    /// The approvals app16.
    /// </summary>
    ApprovalsApp16 = 0xE05F,
    /// <summary>
    /// The approvals app20.
    /// </summary>
    ApprovalsApp20 = 0xE060,
    /// <summary>
    /// The approvals app32.
    /// </summary>
    ApprovalsApp32 = 0xE061,
    /// <summary>
    /// The apps add in16.
    /// </summary>
    AppsAddIn16 = 0xE062,
    /// <summary>
    /// The apps add in28.
    /// </summary>
    AppsAddIn28 = 0xE063,
    /// <summary>
    /// The apps list detail20.
    /// </summary>
    AppsListDetail20 = 0xE064,
    /// <summary>
    /// The apps list detail24.
    /// </summary>
    AppsListDetail24 = 0xE065,
    /// <summary>
    /// The archive32.
    /// </summary>
    Archive32 = 0xE066,
    /// <summary>
    /// The archive arrow back16.
    /// </summary>
    ArchiveArrowBack16 = 0xE067,
    /// <summary>
    /// The archive arrow back20.
    /// </summary>
    ArchiveArrowBack20 = 0xE068,
    /// <summary>
    /// The archive arrow back24.
    /// </summary>
    ArchiveArrowBack24 = 0xE069,
    /// <summary>
    /// The archive arrow back28.
    /// </summary>
    ArchiveArrowBack28 = 0xE06A,
    /// <summary>
    /// The archive arrow back32.
    /// </summary>
    ArchiveArrowBack32 = 0xE06B,
    /// <summary>
    /// The archive arrow back48.
    /// </summary>
    ArchiveArrowBack48 = 0xE06C,
    /// <summary>
    /// The archive multiple16.
    /// </summary>
    ArchiveMultiple16 = 0xE06D,
    /// <summary>
    /// The archive multiple20.
    /// </summary>
    ArchiveMultiple20 = 0xE06E,
    /// <summary>
    /// The archive multiple24.
    /// </summary>
    ArchiveMultiple24 = 0xE06F,
    /// <summary>
    /// The archive settings20.
    /// </summary>
    ArchiveSettings20 = 0xE070,
    /// <summary>
    /// The archive settings24.
    /// </summary>
    ArchiveSettings24 = 0xE071,
    /// <summary>
    /// The archive settings28.
    /// </summary>
    ArchiveSettings28 = 0xE072,
    /// <summary>
    /// The arrow autofit content20.
    /// </summary>
    ArrowAutofitContent20 = 0xE073,
    /// <summary>
    /// The arrow autofit content24.
    /// </summary>
    ArrowAutofitContent24 = 0xE074,
    /// <summary>
    /// The arrow autofit down20.
    /// </summary>
    ArrowAutofitDown20 = 0xE075,
    /// <summary>
    /// The arrow autofit down24.
    /// </summary>
    ArrowAutofitDown24 = 0xE076,
    /// <summary>
    /// The arrow autofit height20.
    /// </summary>
    ArrowAutofitHeight20 = 0xE077,
    /// <summary>
    /// The arrow autofit height dotted20.
    /// </summary>
    ArrowAutofitHeightDotted20 = 0xE078,
    /// <summary>
    /// The arrow autofit height dotted24.
    /// </summary>
    ArrowAutofitHeightDotted24 = 0xE079,
    /// <summary>
    /// The arrow autofit up20.
    /// </summary>
    ArrowAutofitUp20 = 0xE07A,
    /// <summary>
    /// The arrow autofit up24.
    /// </summary>
    ArrowAutofitUp24 = 0xE07B,
    /// <summary>
    /// The arrow autofit width20.
    /// </summary>
    ArrowAutofitWidth20 = 0xE07C,
    /// <summary>
    /// The arrow autofit width dotted20.
    /// </summary>
    ArrowAutofitWidthDotted20 = 0xE07D,
    /// <summary>
    /// The arrow autofit width dotted24.
    /// </summary>
    ArrowAutofitWidthDotted24 = 0xE07E,
    /// <summary>
    /// The arrow between down20.
    /// </summary>
    ArrowBetweenDown20 = 0xE07F,
    /// <summary>
    /// The arrow between down24.
    /// </summary>
    ArrowBetweenDown24 = 0xE080,
    /// <summary>
    /// The arrow between up20.
    /// </summary>
    ArrowBetweenUp20 = 0xE081,
    /// <summary>
    /// The arrow bidirectional up down12.
    /// </summary>
    ArrowBidirectionalUpDown12 = 0xE082,
    /// <summary>
    /// The arrow bidirectional up down16.
    /// </summary>
    ArrowBidirectionalUpDown16 = 0xE083,
    /// <summary>
    /// The arrow bidirectional up down20.
    /// </summary>
    ArrowBidirectionalUpDown20 = 0xE084,
    /// <summary>
    /// The arrow bidirectional up down24.
    /// </summary>
    ArrowBidirectionalUpDown24 = 0xE085,
    /// <summary>
    /// The arrow bounce16.
    /// </summary>
    ArrowBounce16 = 0xE086,
    /// <summary>
    /// The arrow bounce20.
    /// </summary>
    ArrowBounce20 = 0xE087,
    /// <summary>
    /// The arrow bounce24.
    /// </summary>
    ArrowBounce24 = 0xE088,
    /// <summary>
    /// The arrow circle down12.
    /// </summary>
    ArrowCircleDown12 = 0xE089,
    /// <summary>
    /// The arrow circle down16.
    /// </summary>
    ArrowCircleDown16 = 0xE08A,
    /// <summary>
    /// The arrow circle down28.
    /// </summary>
    ArrowCircleDown28 = 0xE08B,
    /// <summary>
    /// The arrow circle down32.
    /// </summary>
    ArrowCircleDown32 = 0xE08C,
    /// <summary>
    /// The arrow circle down48.
    /// </summary>
    ArrowCircleDown48 = 0xE08D,
    /// <summary>
    /// The arrow circle down right16.
    /// </summary>
    ArrowCircleDownRight16 = 0xE08E,
    /// <summary>
    /// The arrow circle down right20.
    /// </summary>
    ArrowCircleDownRight20 = 0xE08F,
    /// <summary>
    /// The arrow circle down right24.
    /// </summary>
    ArrowCircleDownRight24 = 0xE090,
    /// <summary>
    /// The arrow circle down up20.
    /// </summary>
    ArrowCircleDownUp20 = 0xE091,
    /// <summary>
    /// The arrow circle left12.
    /// </summary>
    ArrowCircleLeft12 = 0xE092,
    /// <summary>
    /// The arrow circle left16.
    /// </summary>
    ArrowCircleLeft16 = 0xE093,
    /// <summary>
    /// The arrow circle left20.
    /// </summary>
    ArrowCircleLeft20 = 0xE094,
    /// <summary>
    /// The arrow circle left24.
    /// </summary>
    ArrowCircleLeft24 = 0xE095,
    /// <summary>
    /// The arrow circle left28.
    /// </summary>
    ArrowCircleLeft28 = 0xE096,
    /// <summary>
    /// The arrow circle left32.
    /// </summary>
    ArrowCircleLeft32 = 0xE097,
    /// <summary>
    /// The arrow circle left48.
    /// </summary>
    ArrowCircleLeft48 = 0xE098,
    /// <summary>
    /// The arrow circle right12.
    /// </summary>
    ArrowCircleRight12 = 0xE099,
    /// <summary>
    /// The arrow circle right16.
    /// </summary>
    ArrowCircleRight16 = 0xE09A,
    /// <summary>
    /// The arrow circle right20.
    /// </summary>
    ArrowCircleRight20 = 0xE09B,
    /// <summary>
    /// The arrow circle right24.
    /// </summary>
    ArrowCircleRight24 = 0xE09C,
    /// <summary>
    /// The arrow circle right28.
    /// </summary>
    ArrowCircleRight28 = 0xE09D,
    /// <summary>
    /// The arrow circle right32.
    /// </summary>
    ArrowCircleRight32 = 0xE09E,
    /// <summary>
    /// The arrow circle right48.
    /// </summary>
    ArrowCircleRight48 = 0xE09F,
    /// <summary>
    /// The arrow circle up12.
    /// </summary>
    ArrowCircleUp12 = 0xE0A0,
    /// <summary>
    /// The arrow circle up16.
    /// </summary>
    ArrowCircleUp16 = 0xE0A1,
    /// <summary>
    /// The arrow circle up20.
    /// </summary>
    ArrowCircleUp20 = 0xE0A2,
    /// <summary>
    /// The arrow circle up24.
    /// </summary>
    ArrowCircleUp24 = 0xE0A3,
    /// <summary>
    /// The arrow circle up28.
    /// </summary>
    ArrowCircleUp28 = 0xE0A4,
    /// <summary>
    /// The arrow circle up32.
    /// </summary>
    ArrowCircleUp32 = 0xE0A5,
    /// <summary>
    /// The arrow circle up48.
    /// </summary>
    ArrowCircleUp48 = 0xE0A6,
    /// <summary>
    /// The arrow circle up left20.
    /// </summary>
    ArrowCircleUpLeft20 = 0xE0A7,
    /// <summary>
    /// The arrow circle up left24.
    /// </summary>
    ArrowCircleUpLeft24 = 0xE0A8,
    /// <summary>
    /// The arrow clockwise12.
    /// </summary>
    ArrowClockwise12 = 0xE0A9,
    /// <summary>
    /// The arrow clockwise16.
    /// </summary>
    ArrowClockwise16 = 0xE0AA,
    /// <summary>
    /// The arrow clockwise28.
    /// </summary>
    ArrowClockwise28 = 0xE0AB,
    /// <summary>
    /// The arrow clockwise32.
    /// </summary>
    ArrowClockwise32 = 0xE0AC,
    /// <summary>
    /// The arrow clockwise48.
    /// </summary>
    ArrowClockwise48 = 0xE0AD,
    /// <summary>
    /// The arrow clockwise dashes20.
    /// </summary>
    ArrowClockwiseDashes20 = 0xE0AE,
    /// <summary>
    /// The arrow clockwise dashes24.
    /// </summary>
    ArrowClockwiseDashes24 = 0xE0AF,
    /// <summary>
    /// The arrow collapse all20.
    /// </summary>
    ArrowCollapseAll20 = 0xE0B0,
    /// <summary>
    /// The arrow collapse all24.
    /// </summary>
    ArrowCollapseAll24 = 0xE0B1,
    /// <summary>
    /// The arrow counterclockwise12.
    /// </summary>
    ArrowCounterclockwise12 = 0xE0B2,
    /// <summary>
    /// The arrow counterclockwise16.
    /// </summary>
    ArrowCounterclockwise16 = 0xE0B3,
    /// <summary>
    /// The arrow counterclockwise32.
    /// </summary>
    ArrowCounterclockwise32 = 0xE0B4,
    /// <summary>
    /// The arrow counterclockwise48.
    /// </summary>
    ArrowCounterclockwise48 = 0xE0B5,
    /// <summary>
    /// The arrow counterclockwise dashes20.
    /// </summary>
    ArrowCounterclockwiseDashes20 = 0xE0B6,
    /// <summary>
    /// The arrow counterclockwise dashes24.
    /// </summary>
    ArrowCounterclockwiseDashes24 = 0xE0B7,
    /// <summary>
    /// The arrow curve down left16.
    /// </summary>
    ArrowCurveDownLeft16 = 0xE0B8,
    /// <summary>
    /// The arrow curve down left24.
    /// </summary>
    ArrowCurveDownLeft24 = 0xE0B9,
    /// <summary>
    /// The arrow curve down left28.
    /// </summary>
    ArrowCurveDownLeft28 = 0xE0BA,
    /// <summary>
    /// The arrow down left20.
    /// </summary>
    ArrowDownLeft20 = 0xE0BB,
    /// <summary>
    /// The arrow down left32.
    /// </summary>
    ArrowDownLeft32 = 0xE0BC,
    /// <summary>
    /// The arrow down left48.
    /// </summary>
    ArrowDownLeft48 = 0xE0BD,
    /// <summary>
    /// The arrow eject20.
    /// </summary>
    ArrowEject20 = 0xE0BE,
    /// <summary>
    /// The arrow enter20.
    /// </summary>
    ArrowEnter20 = 0xE0BF,
    /// <summary>
    /// The arrow enter left20.
    /// </summary>
    ArrowEnterLeft20 = 0xE0C0,
    /// <summary>
    /// The arrow enter left24.
    /// </summary>
    ArrowEnterLeft24 = 0xE0C1,
    /// <summary>
    /// The arrow enter up20.
    /// </summary>
    ArrowEnterUp20 = 0xE0C2,
    /// <summary>
    /// The arrow enter up24.
    /// </summary>
    ArrowEnterUp24 = 0xE0C3,
    /// <summary>
    /// The arrow exit20.
    /// </summary>
    ArrowExit20 = 0xE0C4,
    /// <summary>
    /// The arrow expand20.
    /// </summary>
    ArrowExpand20 = 0xE0C5,
    /// <summary>
    /// The arrow export LTR16.
    /// </summary>
    ArrowExportLtr16 = 0xE0C6,
    /// <summary>
    /// The arrow export LTR20.
    /// </summary>
    ArrowExportLtr20 = 0xE0C7,
    /// <summary>
    /// The arrow export LTR24.
    /// </summary>
    ArrowExportLtr24 = 0xE0C8,
    /// <summary>
    /// The arrow export RTL16.
    /// </summary>
    ArrowExportRtl16 = 0xE0C9,
    /// <summary>
    /// The arrow export RTL24.
    /// </summary>
    ArrowExportRtl24 = 0xE0CA,
    /// <summary>
    /// The arrow export up20.
    /// </summary>
    ArrowExportUp20 = 0xE0CB,
    /// <summary>
    /// The arrow export up24.
    /// </summary>
    ArrowExportUp24 = 0xE0CC,
    /// <summary>
    /// The arrow fit20.
    /// </summary>
    ArrowFit20 = 0xE0CD,
    /// <summary>
    /// The arrow fit in16.
    /// </summary>
    ArrowFitIn16 = 0xE0CE,
    /// <summary>
    /// The arrow fit in20.
    /// </summary>
    ArrowFitIn20 = 0xE0CF,
    /// <summary>
    /// The arrow forward28.
    /// </summary>
    ArrowForward28 = 0xE0D0,
    /// <summary>
    /// The arrow forward48.
    /// </summary>
    ArrowForward48 = 0xE0D1,
    /// <summary>
    /// The arrow forward down lightning20.
    /// </summary>
    ArrowForwardDownLightning20 = 0xE0D2,
    /// <summary>
    /// The arrow forward down lightning24.
    /// </summary>
    ArrowForwardDownLightning24 = 0xE0D3,
    /// <summary>
    /// The arrow forward down person20.
    /// </summary>
    ArrowForwardDownPerson20 = 0xE0D4,
    /// <summary>
    /// The arrow forward down person24.
    /// </summary>
    ArrowForwardDownPerson24 = 0xE0D5,
    /// <summary>
    /// The arrow join20.
    /// </summary>
    ArrowJoin20 = 0xE0D6,
    /// <summary>
    /// The arrow left12.
    /// </summary>
    ArrowLeft12 = 0xE0D7,
    /// <summary>
    /// The arrow maximize32.
    /// </summary>
    ArrowMaximize32 = 0xE0D8,
    /// <summary>
    /// The arrow maximize48.
    /// </summary>
    ArrowMaximize48 = 0xE0D9,
    /// <summary>
    /// The arrow maximize vertical48.
    /// </summary>
    ArrowMaximizeVertical48 = 0xE0DA,
    /// <summary>
    /// The arrow minimize vertical20.
    /// </summary>
    ArrowMinimizeVertical20 = 0xE0DB,
    /// <summary>
    /// The arrow move inward20.
    /// </summary>
    ArrowMoveInward20 = 0xE0DC,
    /// <summary>
    /// The arrow next12.
    /// </summary>
    ArrowNext12 = 0xE0DD,
    /// <summary>
    /// The arrow outline up right20.
    /// </summary>
    ArrowOutlineUpRight20 = 0xE0DE,
    /// <summary>
    /// The arrow outline up right24.
    /// </summary>
    ArrowOutlineUpRight24 = 0xE0DF,
    /// <summary>
    /// The arrow outline up right32.
    /// </summary>
    ArrowOutlineUpRight32 = 0xE0E0,
    /// <summary>
    /// The arrow outline up right48.
    /// </summary>
    ArrowOutlineUpRight48 = 0xE0E1,
    /// <summary>
    /// The arrow paragraph20.
    /// </summary>
    ArrowParagraph20 = 0xE0E2,
    /// <summary>
    /// The arrow previous12.
    /// </summary>
    ArrowPrevious12 = 0xE0E3,
    /// <summary>
    /// The arrow redo16.
    /// </summary>
    ArrowRedo16 = 0xE0E4,
    /// <summary>
    /// The arrow redo28.
    /// </summary>
    ArrowRedo28 = 0xE0E5,
    /// <summary>
    /// The arrow reply28.
    /// </summary>
    ArrowReply28 = 0xE0E6,
    /// <summary>
    /// The arrow reply all28.
    /// </summary>
    ArrowReplyAll28 = 0xE0E7,
    /// <summary>
    /// The arrow reset32.
    /// </summary>
    ArrowReset32 = 0xE0E8,
    /// <summary>
    /// The arrow reset48.
    /// </summary>
    ArrowReset48 = 0xE0E9,
    /// <summary>
    /// The arrow right12.
    /// </summary>
    ArrowRight12 = 0xE0EA,
    /// <summary>
    /// The arrow right16.
    /// </summary>
    ArrowRight16 = 0xE0EB,
    /// <summary>
    /// The arrow rotate clockwise16.
    /// </summary>
    ArrowRotateClockwise16 = 0xE0EC,
    /// <summary>
    /// The airplane landing16.
    /// </summary>
    AirplaneLanding16 = 0xE0ED,
    /// <summary>
    /// The airplane landing20.
    /// </summary>
    AirplaneLanding20 = 0xE0EE,
    /// <summary>
    /// The airplane landing24.
    /// </summary>
    AirplaneLanding24 = 0xE0EF,
    /// <summary>
    /// The align space evenly horizontal24.
    /// </summary>
    AlignSpaceEvenlyHorizontal24 = 0xE0F0,
    /// <summary>
    /// The arrow sort down lines20.
    /// </summary>
    ArrowSortDownLines20 = 0xE0F1,
    /// <summary>
    /// The arrow sort down lines24.
    /// </summary>
    ArrowSortDownLines24 = 0xE0F2,
    /// <summary>
    /// The arrow split16.
    /// </summary>
    ArrowSplit16 = 0xE0F3,
    /// <summary>
    /// The arrow split20.
    /// </summary>
    ArrowSplit20 = 0xE0F4,
    /// <summary>
    /// The arrow split24.
    /// </summary>
    ArrowSplit24 = 0xE0F5,
    /// <summary>
    /// The arrow square down20.
    /// </summary>
    ArrowSquareDown20 = 0xE0F6,
    /// <summary>
    /// The arrow square down24.
    /// </summary>
    ArrowSquareDown24 = 0xE0F7,
    /// <summary>
    /// The arrow step back16.
    /// </summary>
    ArrowStepBack16 = 0xE0F8,
    /// <summary>
    /// The arrow step back20.
    /// </summary>
    ArrowStepBack20 = 0xE0F9,
    /// <summary>
    /// The arrow step in12.
    /// </summary>
    ArrowStepIn12 = 0xE0FA,
    /// <summary>
    /// The arrow step in16.
    /// </summary>
    ArrowStepIn16 = 0xE0FB,
    /// <summary>
    /// The arrow step in20.
    /// </summary>
    ArrowStepIn20 = 0xE0FC,
    /// <summary>
    /// The arrow step in24.
    /// </summary>
    ArrowStepIn24 = 0xE0FD,
    /// <summary>
    /// The arrow step in28.
    /// </summary>
    ArrowStepIn28 = 0xE0FE,
    /// <summary>
    /// The arrow step in left12.
    /// </summary>
    ArrowStepInLeft12 = 0xE0FF,
    /// <summary>
    /// The arrow step in left16.
    /// </summary>
    ArrowStepInLeft16 = 0xE100,
    /// <summary>
    /// The arrow step in left20.
    /// </summary>
    ArrowStepInLeft20 = 0xE101,
    /// <summary>
    /// The arrow step in left24.
    /// </summary>
    ArrowStepInLeft24 = 0xE102,
    /// <summary>
    /// The arrow step in left28.
    /// </summary>
    ArrowStepInLeft28 = 0xE103,
    /// <summary>
    /// The arrow step in right12.
    /// </summary>
    ArrowStepInRight12 = 0xE104,
    /// <summary>
    /// The arrow step in right16.
    /// </summary>
    ArrowStepInRight16 = 0xE105,
    /// <summary>
    /// The arrow step in right20.
    /// </summary>
    ArrowStepInRight20 = 0xE106,
    /// <summary>
    /// The arrow step in right24.
    /// </summary>
    ArrowStepInRight24 = 0xE107,
    /// <summary>
    /// The arrow step in right28.
    /// </summary>
    ArrowStepInRight28 = 0xE108,
    /// <summary>
    /// The arrow step out12.
    /// </summary>
    ArrowStepOut12 = 0xE109,
    /// <summary>
    /// The arrow step out16.
    /// </summary>
    ArrowStepOut16 = 0xE10A,
    /// <summary>
    /// The arrow step out20.
    /// </summary>
    ArrowStepOut20 = 0xE10B,
    /// <summary>
    /// The arrow step out24.
    /// </summary>
    ArrowStepOut24 = 0xE10C,
    /// <summary>
    /// The arrow step out28.
    /// </summary>
    ArrowStepOut28 = 0xE10D,
    /// <summary>
    /// The arrow step over16.
    /// </summary>
    ArrowStepOver16 = 0xE10E,
    /// <summary>
    /// The arrow step over20.
    /// </summary>
    ArrowStepOver20 = 0xE10F,
    /// <summary>
    /// The arrow sync16.
    /// </summary>
    ArrowSync16 = 0xE110,
    /// <summary>
    /// The arrow synchronize checkmark20.
    /// </summary>
    ArrowSyncCheckmark20 = 0xE111,
    /// <summary>
    /// The arrow synchronize checkmark24.
    /// </summary>
    ArrowSyncCheckmark24 = 0xE112,
    /// <summary>
    /// The arrow synchronize dismiss20.
    /// </summary>
    ArrowSyncDismiss20 = 0xE113,
    /// <summary>
    /// The arrow synchronize dismiss24.
    /// </summary>
    ArrowSyncDismiss24 = 0xE114,
    /// <summary>
    /// The arrow synchronize off16.
    /// </summary>
    ArrowSyncOff16 = 0xE115,
    /// <summary>
    /// The arrow synchronize off20.
    /// </summary>
    ArrowSyncOff20 = 0xE116,
    /// <summary>
    /// The arrow trending checkmark20.
    /// </summary>
    ArrowTrendingCheckmark20 = 0xE117,
    /// <summary>
    /// The arrow trending checkmark24.
    /// </summary>
    ArrowTrendingCheckmark24 = 0xE118,
    /// <summary>
    /// The arrow trending down16.
    /// </summary>
    ArrowTrendingDown16 = 0xE119,
    /// <summary>
    /// The arrow trending down20.
    /// </summary>
    ArrowTrendingDown20 = 0xE11A,
    /// <summary>
    /// The arrow trending down24.
    /// </summary>
    ArrowTrendingDown24 = 0xE11B,
    /// <summary>
    /// The arrow trending lines20.
    /// </summary>
    ArrowTrendingLines20 = 0xE11C,
    /// <summary>
    /// The arrow trending lines24.
    /// </summary>
    ArrowTrendingLines24 = 0xE11D,
    /// <summary>
    /// The arrow trending settings20.
    /// </summary>
    ArrowTrendingSettings20 = 0xE11E,
    /// <summary>
    /// The arrow trending settings24.
    /// </summary>
    ArrowTrendingSettings24 = 0xE11F,
    /// <summary>
    /// The arrow trending text20.
    /// </summary>
    ArrowTrendingText20 = 0xE120,
    /// <summary>
    /// The arrow trending text24.
    /// </summary>
    ArrowTrendingText24 = 0xE121,
    /// <summary>
    /// The arrow trending wrench20.
    /// </summary>
    ArrowTrendingWrench20 = 0xE122,
    /// <summary>
    /// The arrow trending wrench24.
    /// </summary>
    ArrowTrendingWrench24 = 0xE123,
    /// <summary>
    /// The arrow turn bidirectional down right20.
    /// </summary>
    ArrowTurnBidirectionalDownRight20 = 0xE124,
    /// <summary>
    /// The arrow turn right20.
    /// </summary>
    ArrowTurnRight20 = 0xE125,
    /// <summary>
    /// The arrow undo16.
    /// </summary>
    ArrowUndo16 = 0xE126,
    /// <summary>
    /// The arrow undo28.
    /// </summary>
    ArrowUndo28 = 0xE127,
    /// <summary>
    /// The arrow undo32.
    /// </summary>
    ArrowUndo32 = 0xE128,
    /// <summary>
    /// The arrow undo48.
    /// </summary>
    ArrowUndo48 = 0xE129,
    /// <summary>
    /// The arrow up12.
    /// </summary>
    ArrowUp12 = 0xE12A,
    /// <summary>
    /// The arrow up left16.
    /// </summary>
    ArrowUpLeft16 = 0xE12B,
    /// <summary>
    /// The arrow up left20.
    /// </summary>
    ArrowUpLeft20 = 0xE12C,
    /// <summary>
    /// The arrow up left48.
    /// </summary>
    ArrowUpLeft48 = 0xE12D,
    /// <summary>
    /// The arrow up right20.
    /// </summary>
    ArrowUpRight20 = 0xE12E,
    /// <summary>
    /// The arrow up right32.
    /// </summary>
    ArrowUpRight32 = 0xE12F,
    /// <summary>
    /// The arrow up right48.
    /// </summary>
    ArrowUpRight48 = 0xE130,
    /// <summary>
    /// The arrow upload16.
    /// </summary>
    ArrowUpload16 = 0xE131,
    /// <summary>
    /// The arrow wrap20.
    /// </summary>
    ArrowWrap20 = 0xE132,
    /// <summary>
    /// The arrow wrap off20.
    /// </summary>
    ArrowWrapOff20 = 0xE133,
    /// <summary>
    /// The arrows bidirectional20.
    /// </summary>
    ArrowsBidirectional20 = 0xE134,
    /// <summary>
    /// The attach12.
    /// </summary>
    Attach12 = 0xE135,
    /// <summary>
    /// The attach text20.
    /// </summary>
    AttachText20 = 0xE136,
    /// <summary>
    /// The automatic fit height20.
    /// </summary>
    AutoFitHeight20 = 0xE137,
    /// <summary>
    /// The automatic fit height24.
    /// </summary>
    AutoFitHeight24 = 0xE138,
    /// <summary>
    /// The automatic fit width20.
    /// </summary>
    AutoFitWidth20 = 0xE139,
    /// <summary>
    /// The automatic fit width24.
    /// </summary>
    AutoFitWidth24 = 0xE13A,
    /// <summary>
    /// The autocorrect20.
    /// </summary>
    Autocorrect20 = 0xE13B,
    /// <summary>
    /// The backpack32.
    /// </summary>
    Backpack32 = 0xE13C,
    /// <summary>
    /// The backpack add20.
    /// </summary>
    BackpackAdd20 = 0xE13D,
    /// <summary>
    /// The backpack add24.
    /// </summary>
    BackpackAdd24 = 0xE13E,
    /// <summary>
    /// The backpack add28.
    /// </summary>
    BackpackAdd28 = 0xE13F,
    /// <summary>
    /// The backpack add48.
    /// </summary>
    BackpackAdd48 = 0xE140,
    /// <summary>
    /// The badge20.
    /// </summary>
    Badge20 = 0xE141,
    /// <summary>
    /// The balloon12.
    /// </summary>
    Balloon12 = 0xE142,
    /// <summary>
    /// The battery1020.
    /// </summary>
    Battery1020 = 0xE143,
    /// <summary>
    /// The battery1024.
    /// </summary>
    Battery1024 = 0xE144,
    /// <summary>
    /// The battery checkmark20.
    /// </summary>
    BatteryCheckmark20 = 0xE145,
    /// <summary>
    /// The battery checkmark24.
    /// </summary>
    BatteryCheckmark24 = 0xE146,
    /// <summary>
    /// The battery warning20.
    /// </summary>
    BatteryWarning20 = 0xE147,
    /// <summary>
    /// The beach16.
    /// </summary>
    Beach16 = 0xE148,
    /// <summary>
    /// The beach20.
    /// </summary>
    Beach20 = 0xE149,
    /// <summary>
    /// The beach24.
    /// </summary>
    Beach24 = 0xE14A,
    /// <summary>
    /// The beach28.
    /// </summary>
    Beach28 = 0xE14B,
    /// <summary>
    /// The beach32.
    /// </summary>
    Beach32 = 0xE14C,
    /// <summary>
    /// The beach48.
    /// </summary>
    Beach48 = 0xE14D,
    /// <summary>
    /// The bezier curve square12.
    /// </summary>
    BezierCurveSquare12 = 0xE14E,
    /// <summary>
    /// The bezier curve square20.
    /// </summary>
    BezierCurveSquare20 = 0xE14F,
    /// <summary>
    /// The bin full20.
    /// </summary>
    BinFull20 = 0xE150,
    /// <summary>
    /// The bin full24.
    /// </summary>
    BinFull24 = 0xE151,
    /// <summary>
    /// The bluetooth connected20.
    /// </summary>
    BluetoothConnected20 = 0xE152,
    /// <summary>
    /// The bluetooth disabled20.
    /// </summary>
    BluetoothDisabled20 = 0xE153,
    /// <summary>
    /// The bluetooth searching20.
    /// </summary>
    BluetoothSearching20 = 0xE154,
    /// <summary>
    /// The board16.
    /// </summary>
    Board16 = 0xE155,
    /// <summary>
    /// The board20.
    /// </summary>
    Board20 = 0xE156,
    /// <summary>
    /// The board28.
    /// </summary>
    Board28 = 0xE157,
    /// <summary>
    /// The board games20.
    /// </summary>
    BoardGames20 = 0xE158,
    /// <summary>
    /// The board heart16.
    /// </summary>
    BoardHeart16 = 0xE159,
    /// <summary>
    /// The board heart20.
    /// </summary>
    BoardHeart20 = 0xE15A,
    /// <summary>
    /// The board heart24.
    /// </summary>
    BoardHeart24 = 0xE15B,
    /// <summary>
    /// The board split16.
    /// </summary>
    BoardSplit16 = 0xE15C,
    /// <summary>
    /// The board split20.
    /// </summary>
    BoardSplit20 = 0xE15D,
    /// <summary>
    /// The board split24.
    /// </summary>
    BoardSplit24 = 0xE15E,
    /// <summary>
    /// The board split28.
    /// </summary>
    BoardSplit28 = 0xE15F,
    /// <summary>
    /// The board split48.
    /// </summary>
    BoardSplit48 = 0xE160,
    /// <summary>
    /// The book add24.
    /// </summary>
    BookAdd24 = 0xE161,
    /// <summary>
    /// The book arrow clockwise20.
    /// </summary>
    BookArrowClockwise20 = 0xE162,
    /// <summary>
    /// The book arrow clockwise24.
    /// </summary>
    BookArrowClockwise24 = 0xE163,
    /// <summary>
    /// The book clock20.
    /// </summary>
    BookClock20 = 0xE164,
    /// <summary>
    /// The book clock24.
    /// </summary>
    BookClock24 = 0xE165,
    /// <summary>
    /// The book coins20.
    /// </summary>
    BookCoins20 = 0xE166,
    /// <summary>
    /// The book coins24.
    /// </summary>
    BookCoins24 = 0xE167,
    /// <summary>
    /// The book compass20.
    /// </summary>
    BookCompass20 = 0xE168,
    /// <summary>
    /// The book compass24.
    /// </summary>
    BookCompass24 = 0xE169,
    /// <summary>
    /// The book contacts20.
    /// </summary>
    BookContacts20 = 0xE16A,
    /// <summary>
    /// The book contacts24.
    /// </summary>
    BookContacts24 = 0xE16B,
    /// <summary>
    /// The book contacts28.
    /// </summary>
    BookContacts28 = 0xE16C,
    /// <summary>
    /// The book contacts32.
    /// </summary>
    BookContacts32 = 0xE16D,
    /// <summary>
    /// The book database20.
    /// </summary>
    BookDatabase20 = 0xE16E,
    /// <summary>
    /// The book database24.
    /// </summary>
    BookDatabase24 = 0xE16F,
    /// <summary>
    /// The book exclamation mark20.
    /// </summary>
    BookExclamationMark20 = 0xE170,
    /// <summary>
    /// The book exclamation mark24.
    /// </summary>
    BookExclamationMark24 = 0xE171,
    /// <summary>
    /// The book globe20.
    /// </summary>
    BookGlobe20 = 0xE172,
    /// <summary>
    /// The book information20.
    /// </summary>
    BookInformation20 = 0xE173,
    /// <summary>
    /// The book information24.
    /// </summary>
    BookInformation24 = 0xE174,
    /// <summary>
    /// The book letter20.
    /// </summary>
    BookLetter20 = 0xE175,
    /// <summary>
    /// The book letter24.
    /// </summary>
    BookLetter24 = 0xE176,
    /// <summary>
    /// The book open16.
    /// </summary>
    BookOpen16 = 0xE177,
    /// <summary>
    /// The book open20.
    /// </summary>
    BookOpen20 = 0xE178,
    /// <summary>
    /// The book open24.
    /// </summary>
    BookOpen24 = 0xE179,
    /// <summary>
    /// The book open28.
    /// </summary>
    BookOpen28 = 0xE17A,
    /// <summary>
    /// The book open32.
    /// </summary>
    BookOpen32 = 0xE17B,
    /// <summary>
    /// The book open48.
    /// </summary>
    BookOpen48 = 0xE17C,
    /// <summary>
    /// The book open globe20.
    /// </summary>
    BookOpenGlobe20 = 0xE17D,
    /// <summary>
    /// The book open globe24.
    /// </summary>
    BookOpenGlobe24 = 0xE17E,
    /// <summary>
    /// The book open microphone20.
    /// </summary>
    BookOpenMicrophone20 = 0xE17F,
    /// <summary>
    /// The book open microphone24.
    /// </summary>
    BookOpenMicrophone24 = 0xE180,
    /// <summary>
    /// The book open microphone28.
    /// </summary>
    BookOpenMicrophone28 = 0xE181,
    /// <summary>
    /// The book open microphone32.
    /// </summary>
    BookOpenMicrophone32 = 0xE182,
    /// <summary>
    /// The book open microphone48.
    /// </summary>
    BookOpenMicrophone48 = 0xE183,
    /// <summary>
    /// The book pulse20.
    /// </summary>
    BookPulse20 = 0xE184,
    /// <summary>
    /// The book pulse24.
    /// </summary>
    BookPulse24 = 0xE185,
    /// <summary>
    /// The book question mark20.
    /// </summary>
    BookQuestionMark20 = 0xE186,
    /// <summary>
    /// The book question mark24.
    /// </summary>
    BookQuestionMark24 = 0xE187,
    /// <summary>
    /// The book question mark RTL20.
    /// </summary>
    BookQuestionMarkRtl20 = 0xE188,
    /// <summary>
    /// The book search20.
    /// </summary>
    BookSearch20 = 0xE189,
    /// <summary>
    /// The book search24.
    /// </summary>
    BookSearch24 = 0xE18A,
    /// <summary>
    /// The book star20.
    /// </summary>
    BookStar20 = 0xE18B,
    /// <summary>
    /// The book star24.
    /// </summary>
    BookStar24 = 0xE18C,
    /// <summary>
    /// The book template20.
    /// </summary>
    BookTemplate20 = 0xE18D,
    /// <summary>
    /// The book theta20.
    /// </summary>
    BookTheta20 = 0xE18E,
    /// <summary>
    /// The book theta24.
    /// </summary>
    BookTheta24 = 0xE18F,
    /// <summary>
    /// The book toolbox24.
    /// </summary>
    BookToolbox24 = 0xE190,
    /// <summary>
    /// The bookmark32.
    /// </summary>
    Bookmark32 = 0xE191,
    /// <summary>
    /// The bookmark multiple16.
    /// </summary>
    BookmarkMultiple16 = 0xE192,
    /// <summary>
    /// The bookmark multiple20.
    /// </summary>
    BookmarkMultiple20 = 0xE193,
    /// <summary>
    /// The bookmark multiple24.
    /// </summary>
    BookmarkMultiple24 = 0xE194,
    /// <summary>
    /// The bookmark multiple28.
    /// </summary>
    BookmarkMultiple28 = 0xE195,
    /// <summary>
    /// The bookmark multiple32.
    /// </summary>
    BookmarkMultiple32 = 0xE196,
    /// <summary>
    /// The bookmark multiple48.
    /// </summary>
    BookmarkMultiple48 = 0xE197,
    /// <summary>
    /// The bookmark off20.
    /// </summary>
    BookmarkOff20 = 0xE198,
    /// <summary>
    /// The bookmark search20.
    /// </summary>
    BookmarkSearch20 = 0xE199,
    /// <summary>
    /// The bookmark search24.
    /// </summary>
    BookmarkSearch24 = 0xE19A,
    /// <summary>
    /// The border all16.
    /// </summary>
    BorderAll16 = 0xE19B,
    /// <summary>
    /// The border all20.
    /// </summary>
    BorderAll20 = 0xE19C,
    /// <summary>
    /// The border all24.
    /// </summary>
    BorderAll24 = 0xE19D,
    /// <summary>
    /// The border bottom20.
    /// </summary>
    BorderBottom20 = 0xE19E,
    /// <summary>
    /// The border bottom24.
    /// </summary>
    BorderBottom24 = 0xE19F,
    /// <summary>
    /// The border bottom double20.
    /// </summary>
    BorderBottomDouble20 = 0xE1A0,
    /// <summary>
    /// The border bottom double24.
    /// </summary>
    BorderBottomDouble24 = 0xE1A1,
    /// <summary>
    /// The border bottom thick20.
    /// </summary>
    BorderBottomThick20 = 0xE1A2,
    /// <summary>
    /// The border bottom thick24.
    /// </summary>
    BorderBottomThick24 = 0xE1A3,
    /// <summary>
    /// The border left20.
    /// </summary>
    BorderLeft20 = 0xE1A4,
    /// <summary>
    /// The border left24.
    /// </summary>
    BorderLeft24 = 0xE1A5,
    /// <summary>
    /// The border left right20.
    /// </summary>
    BorderLeftRight20 = 0xE1A6,
    /// <summary>
    /// The border left right24.
    /// </summary>
    BorderLeftRight24 = 0xE1A7,
    /// <summary>
    /// The border none20.
    /// </summary>
    BorderNone20 = 0xE1A8,
    /// <summary>
    /// The border none24.
    /// </summary>
    BorderNone24 = 0xE1A9,
    /// <summary>
    /// The border outside20.
    /// </summary>
    BorderOutside20 = 0xE1AA,
    /// <summary>
    /// The border outside24.
    /// </summary>
    BorderOutside24 = 0xE1AB,
    /// <summary>
    /// The border outside thick20.
    /// </summary>
    BorderOutsideThick20 = 0xE1AC,
    /// <summary>
    /// The border outside thick24.
    /// </summary>
    BorderOutsideThick24 = 0xE1AD,
    /// <summary>
    /// The border right20.
    /// </summary>
    BorderRight20 = 0xE1AE,
    /// <summary>
    /// The border right24.
    /// </summary>
    BorderRight24 = 0xE1AF,
    /// <summary>
    /// The border top20.
    /// </summary>
    BorderTop20 = 0xE1B0,
    /// <summary>
    /// The border top24.
    /// </summary>
    BorderTop24 = 0xE1B1,
    /// <summary>
    /// The border top bottom20.
    /// </summary>
    BorderTopBottom20 = 0xE1B2,
    /// <summary>
    /// The border top bottom24.
    /// </summary>
    BorderTopBottom24 = 0xE1B3,
    /// <summary>
    /// The border top bottom double20.
    /// </summary>
    BorderTopBottomDouble20 = 0xE1B4,
    /// <summary>
    /// The border top bottom double24.
    /// </summary>
    BorderTopBottomDouble24 = 0xE1B5,
    /// <summary>
    /// The border top bottom thick20.
    /// </summary>
    BorderTopBottomThick20 = 0xE1B6,
    /// <summary>
    /// The border top bottom thick24.
    /// </summary>
    BorderTopBottomThick24 = 0xE1B7,
    /// <summary>
    /// The bot20.
    /// </summary>
    Bot20 = 0xE1B8,
    /// <summary>
    /// The bot add20.
    /// </summary>
    BotAdd20 = 0xE1B9,
    /// <summary>
    /// The box16.
    /// </summary>
    Box16 = 0xE1BA,
    /// <summary>
    /// The box20.
    /// </summary>
    Box20 = 0xE1BB,
    /// <summary>
    /// The box24.
    /// </summary>
    Box24 = 0xE1BC,
    /// <summary>
    /// The box arrow left20.
    /// </summary>
    BoxArrowLeft20 = 0xE1BD,
    /// <summary>
    /// The box arrow left24.
    /// </summary>
    BoxArrowLeft24 = 0xE1BE,
    /// <summary>
    /// The box arrow up20.
    /// </summary>
    BoxArrowUp20 = 0xE1BF,
    /// <summary>
    /// The box arrow up24.
    /// </summary>
    BoxArrowUp24 = 0xE1C0,
    /// <summary>
    /// The box checkmark20.
    /// </summary>
    BoxCheckmark20 = 0xE1C1,
    /// <summary>
    /// The box checkmark24.
    /// </summary>
    BoxCheckmark24 = 0xE1C2,
    /// <summary>
    /// The box dismiss20.
    /// </summary>
    BoxDismiss20 = 0xE1C3,
    /// <summary>
    /// The box dismiss24.
    /// </summary>
    BoxDismiss24 = 0xE1C4,
    /// <summary>
    /// The box edit20.
    /// </summary>
    BoxEdit20 = 0xE1C5,
    /// <summary>
    /// The box edit24.
    /// </summary>
    BoxEdit24 = 0xE1C6,
    /// <summary>
    /// The box multiple20.
    /// </summary>
    BoxMultiple20 = 0xE1C7,
    /// <summary>
    /// The box multiple24.
    /// </summary>
    BoxMultiple24 = 0xE1C8,
    /// <summary>
    /// The box multiple arrow left20.
    /// </summary>
    BoxMultipleArrowLeft20 = 0xE1C9,
    /// <summary>
    /// The box multiple arrow left24.
    /// </summary>
    BoxMultipleArrowLeft24 = 0xE1CA,
    /// <summary>
    /// The box multiple arrow right20.
    /// </summary>
    BoxMultipleArrowRight20 = 0xE1CB,
    /// <summary>
    /// The box multiple arrow right24.
    /// </summary>
    BoxMultipleArrowRight24 = 0xE1CC,
    /// <summary>
    /// The box multiple checkmark20.
    /// </summary>
    BoxMultipleCheckmark20 = 0xE1CD,
    /// <summary>
    /// The box multiple checkmark24.
    /// </summary>
    BoxMultipleCheckmark24 = 0xE1CE,
    /// <summary>
    /// The box multiple search20.
    /// </summary>
    BoxMultipleSearch20 = 0xE1CF,
    /// <summary>
    /// The box multiple search24.
    /// </summary>
    BoxMultipleSearch24 = 0xE1D0,
    /// <summary>
    /// The box search20.
    /// </summary>
    BoxSearch20 = 0xE1D1,
    /// <summary>
    /// The box search24.
    /// </summary>
    BoxSearch24 = 0xE1D2,
    /// <summary>
    /// The box toolbox20.
    /// </summary>
    BoxToolbox20 = 0xE1D3,
    /// <summary>
    /// The box toolbox24.
    /// </summary>
    BoxToolbox24 = 0xE1D4,
    /// <summary>
    /// The braces20.
    /// </summary>
    Braces20 = 0xE1D5,
    /// <summary>
    /// The braces24.
    /// </summary>
    Braces24 = 0xE1D6,
    /// <summary>
    /// The braces variable20.
    /// </summary>
    BracesVariable20 = 0xE1D7,
    /// <summary>
    /// The braces variable24.
    /// </summary>
    BracesVariable24 = 0xE1D8,
    /// <summary>
    /// The branch20.
    /// </summary>
    Branch20 = 0xE1D9,
    /// <summary>
    /// The branch compare16.
    /// </summary>
    BranchCompare16 = 0xE1DA,
    /// <summary>
    /// The branch compare20.
    /// </summary>
    BranchCompare20 = 0xE1DB,
    /// <summary>
    /// The branch compare24.
    /// </summary>
    BranchCompare24 = 0xE1DC,
    /// <summary>
    /// The branch fork16.
    /// </summary>
    BranchFork16 = 0xE1DD,
    /// <summary>
    /// The branch fork20.
    /// </summary>
    BranchFork20 = 0xE1DE,
    /// <summary>
    /// The branch fork24.
    /// </summary>
    BranchFork24 = 0xE1DF,
    /// <summary>
    /// The branch fork hint20.
    /// </summary>
    BranchForkHint20 = 0xE1E0,
    /// <summary>
    /// The branch fork hint24.
    /// </summary>
    BranchForkHint24 = 0xE1E1,
    /// <summary>
    /// The branch fork link20.
    /// </summary>
    BranchForkLink20 = 0xE1E2,
    /// <summary>
    /// The branch fork link24.
    /// </summary>
    BranchForkLink24 = 0xE1E3,
    /// <summary>
    /// The branch request20.
    /// </summary>
    BranchRequest20 = 0xE1E4,
    /// <summary>
    /// The breakout room20.
    /// </summary>
    BreakoutRoom20 = 0xE1E5,
    /// <summary>
    /// The breakout room24.
    /// </summary>
    BreakoutRoom24 = 0xE1E6,
    /// <summary>
    /// The breakout room28.
    /// </summary>
    BreakoutRoom28 = 0xE1E7,
    /// <summary>
    /// The briefcase12.
    /// </summary>
    Briefcase12 = 0xE1E8,
    /// <summary>
    /// The briefcase16.
    /// </summary>
    Briefcase16 = 0xE1E9,
    /// <summary>
    /// The briefcase28.
    /// </summary>
    Briefcase28 = 0xE1EA,
    /// <summary>
    /// The briefcase32.
    /// </summary>
    Briefcase32 = 0xE1EB,
    /// <summary>
    /// The briefcase48.
    /// </summary>
    Briefcase48 = 0xE1EC,
    /// <summary>
    /// The briefcase medical16.
    /// </summary>
    BriefcaseMedical16 = 0xE1ED,
    /// <summary>
    /// The briefcase medical24.
    /// </summary>
    BriefcaseMedical24 = 0xE1EE,
    /// <summary>
    /// The briefcase medical32.
    /// </summary>
    BriefcaseMedical32 = 0xE1EF,
    /// <summary>
    /// The briefcase off16.
    /// </summary>
    BriefcaseOff16 = 0xE1F0,
    /// <summary>
    /// The briefcase off20.
    /// </summary>
    BriefcaseOff20 = 0xE1F1,
    /// <summary>
    /// The briefcase off24.
    /// </summary>
    BriefcaseOff24 = 0xE1F2,
    /// <summary>
    /// The briefcase off28.
    /// </summary>
    BriefcaseOff28 = 0xE1F3,
    /// <summary>
    /// The briefcase off32.
    /// </summary>
    BriefcaseOff32 = 0xE1F4,
    /// <summary>
    /// The briefcase off48.
    /// </summary>
    BriefcaseOff48 = 0xE1F5,
    /// <summary>
    /// The brightness high16.
    /// </summary>
    BrightnessHigh16 = 0xE1F6,
    /// <summary>
    /// The brightness high20.
    /// </summary>
    BrightnessHigh20 = 0xE1F7,
    /// <summary>
    /// The brightness high24.
    /// </summary>
    BrightnessHigh24 = 0xE1F8,
    /// <summary>
    /// The brightness high28.
    /// </summary>
    BrightnessHigh28 = 0xE1F9,
    /// <summary>
    /// The brightness high32.
    /// </summary>
    BrightnessHigh32 = 0xE1FA,
    /// <summary>
    /// The brightness high48.
    /// </summary>
    BrightnessHigh48 = 0xE1FB,
    /// <summary>
    /// The brightness low16.
    /// </summary>
    BrightnessLow16 = 0xE1FC,
    /// <summary>
    /// The brightness low20.
    /// </summary>
    BrightnessLow20 = 0xE1FD,
    /// <summary>
    /// The brightness low24.
    /// </summary>
    BrightnessLow24 = 0xE1FE,
    /// <summary>
    /// The brightness low28.
    /// </summary>
    BrightnessLow28 = 0xE1FF,
    /// <summary>
    /// The brightness low32.
    /// </summary>
    BrightnessLow32 = 0xE200,
    /// <summary>
    /// The brightness low48.
    /// </summary>
    BrightnessLow48 = 0xE201,
    /// <summary>
    /// The broad activity feed16.
    /// </summary>
    BroadActivityFeed16 = 0xE202,
    /// <summary>
    /// The broad activity feed20.
    /// </summary>
    BroadActivityFeed20 = 0xE203,
    /// <summary>
    /// The broom28.
    /// </summary>
    Broom28 = 0xE204,
    /// <summary>
    /// The bug16.
    /// </summary>
    Bug16 = 0xE205,
    /// <summary>
    /// The bug20.
    /// </summary>
    Bug20 = 0xE206,
    /// <summary>
    /// The bug24.
    /// </summary>
    Bug24 = 0xE207,
    /// <summary>
    /// The bug arrow counterclockwise20.
    /// </summary>
    BugArrowCounterclockwise20 = 0xE208,
    /// <summary>
    /// The bug prohibited20.
    /// </summary>
    BugProhibited20 = 0xE209,
    /// <summary>
    /// The building16.
    /// </summary>
    Building16 = 0xE20A,
    /// <summary>
    /// The building20.
    /// </summary>
    Building20 = 0xE20B,
    /// <summary>
    /// The building bank16.
    /// </summary>
    BuildingBank16 = 0xE20C,
    /// <summary>
    /// The building bank20.
    /// </summary>
    BuildingBank20 = 0xE20D,
    /// <summary>
    /// The building bank24.
    /// </summary>
    BuildingBank24 = 0xE20E,
    /// <summary>
    /// The building bank28.
    /// </summary>
    BuildingBank28 = 0xE20F,
    /// <summary>
    /// The building bank48.
    /// </summary>
    BuildingBank48 = 0xE210,
    /// <summary>
    /// The building bank link16.
    /// </summary>
    BuildingBankLink16 = 0xE211,
    /// <summary>
    /// The building bank link20.
    /// </summary>
    BuildingBankLink20 = 0xE212,
    /// <summary>
    /// The building bank link24.
    /// </summary>
    BuildingBankLink24 = 0xE213,
    /// <summary>
    /// The building bank link28.
    /// </summary>
    BuildingBankLink28 = 0xE214,
    /// <summary>
    /// The building bank link48.
    /// </summary>
    BuildingBankLink48 = 0xE215,
    /// <summary>
    /// The building factory16.
    /// </summary>
    BuildingFactory16 = 0xE216,
    /// <summary>
    /// The building factory20.
    /// </summary>
    BuildingFactory20 = 0xE217,
    /// <summary>
    /// The building factory24.
    /// </summary>
    BuildingFactory24 = 0xE218,
    /// <summary>
    /// The building factory28.
    /// </summary>
    BuildingFactory28 = 0xE219,
    /// <summary>
    /// The building factory32.
    /// </summary>
    BuildingFactory32 = 0xE21A,
    /// <summary>
    /// The building factory48.
    /// </summary>
    BuildingFactory48 = 0xE21B,
    /// <summary>
    /// The building government20.
    /// </summary>
    BuildingGovernment20 = 0xE21C,
    /// <summary>
    /// The building government24.
    /// </summary>
    BuildingGovernment24 = 0xE21D,
    /// <summary>
    /// The building government32.
    /// </summary>
    BuildingGovernment32 = 0xE21E,
    /// <summary>
    /// The building home16.
    /// </summary>
    BuildingHome16 = 0xE21F,
    /// <summary>
    /// The building home20.
    /// </summary>
    BuildingHome20 = 0xE220,
    /// <summary>
    /// The building home24.
    /// </summary>
    BuildingHome24 = 0xE221,
    /// <summary>
    /// The building lighthouse20.
    /// </summary>
    BuildingLighthouse20 = 0xE222,
    /// <summary>
    /// The building multiple20.
    /// </summary>
    BuildingMultiple20 = 0xE223,
    /// <summary>
    /// The building multiple24.
    /// </summary>
    BuildingMultiple24 = 0xE224,
    /// <summary>
    /// The building retail20.
    /// </summary>
    BuildingRetail20 = 0xE225,
    /// <summary>
    /// The building retail money20.
    /// </summary>
    BuildingRetailMoney20 = 0xE226,
    /// <summary>
    /// The building retail money24.
    /// </summary>
    BuildingRetailMoney24 = 0xE227,
    /// <summary>
    /// The building retail more20.
    /// </summary>
    BuildingRetailMore20 = 0xE228,
    /// <summary>
    /// The building retail shield20.
    /// </summary>
    BuildingRetailShield20 = 0xE229,
    /// <summary>
    /// The building retail shield24.
    /// </summary>
    BuildingRetailShield24 = 0xE22A,
    /// <summary>
    /// The building retail toolbox20.
    /// </summary>
    BuildingRetailToolbox20 = 0xE22B,
    /// <summary>
    /// The building retail toolbox24.
    /// </summary>
    BuildingRetailToolbox24 = 0xE22C,
    /// <summary>
    /// The building shop16.
    /// </summary>
    BuildingShop16 = 0xE22D,
    /// <summary>
    /// The building shop20.
    /// </summary>
    BuildingShop20 = 0xE22E,
    /// <summary>
    /// The building shop24.
    /// </summary>
    BuildingShop24 = 0xE22F,
    /// <summary>
    /// The building skyscraper16.
    /// </summary>
    BuildingSkyscraper16 = 0xE230,
    /// <summary>
    /// The building skyscraper20.
    /// </summary>
    BuildingSkyscraper20 = 0xE231,
    /// <summary>
    /// The building skyscraper24.
    /// </summary>
    BuildingSkyscraper24 = 0xE232,
    /// <summary>
    /// The calculator24.
    /// </summary>
    Calculator24 = 0xE233,
    /// <summary>
    /// The calculator arrow clockwise20.
    /// </summary>
    CalculatorArrowClockwise20 = 0xE234,
    /// <summary>
    /// The calculator arrow clockwise24.
    /// </summary>
    CalculatorArrowClockwise24 = 0xE235,
    /// <summary>
    /// The calculator multiple20.
    /// </summary>
    CalculatorMultiple20 = 0xE236,
    /// <summary>
    /// The calculator multiple24.
    /// </summary>
    CalculatorMultiple24 = 0xE237,
    /// <summary>
    /// The calendar3 day16.
    /// </summary>
    Calendar3Day16 = 0xE238,
    /// <summary>
    /// The calendar add16.
    /// </summary>
    CalendarAdd16 = 0xE239,
    /// <summary>
    /// The calendar add28.
    /// </summary>
    CalendarAdd28 = 0xE23A,
    /// <summary>
    /// The calendar arrow down20.
    /// </summary>
    CalendarArrowDown20 = 0xE23B,
    /// <summary>
    /// The calendar arrow down24.
    /// </summary>
    CalendarArrowDown24 = 0xE23C,
    /// <summary>
    /// The calendar arrow right16.
    /// </summary>
    CalendarArrowRight16 = 0xE23D,
    /// <summary>
    /// The calendar arrow right24.
    /// </summary>
    CalendarArrowRight24 = 0xE23E,
    /// <summary>
    /// The calendar assistant16.
    /// </summary>
    CalendarAssistant16 = 0xE23F,
    /// <summary>
    /// The calendar cancel16.
    /// </summary>
    CalendarCancel16 = 0xE240,
    /// <summary>
    /// The calendar chat20.
    /// </summary>
    CalendarChat20 = 0xE241,
    /// <summary>
    /// The calendar chat24.
    /// </summary>
    CalendarChat24 = 0xE242,
    /// <summary>
    /// The calendar clock16.
    /// </summary>
    CalendarClock16 = 0xE243,
    /// <summary>
    /// The calendar day16.
    /// </summary>
    CalendarDay16 = 0xE244,
    /// <summary>
    /// The calendar edit16.
    /// </summary>
    CalendarEdit16 = 0xE245,
    /// <summary>
    /// The calendar edit20.
    /// </summary>
    CalendarEdit20 = 0xE246,
    /// <summary>
    /// The calendar edit24.
    /// </summary>
    CalendarEdit24 = 0xE247,
    /// <summary>
    /// The calendar empty32.
    /// </summary>
    CalendarEmpty32 = 0xE248,
    /// <summary>
    /// The calendar error20.
    /// </summary>
    CalendarError20 = 0xE249,
    /// <summary>
    /// The calendar error24.
    /// </summary>
    CalendarError24 = 0xE24A,
    /// <summary>
    /// The calendar info20.
    /// </summary>
    CalendarInfo20 = 0xE24B,
    /// <summary>
    /// The calendar LTR12.
    /// </summary>
    CalendarLtr12 = 0xE24C,
    /// <summary>
    /// The calendar LTR16.
    /// </summary>
    CalendarLtr16 = 0xE24D,
    /// <summary>
    /// The calendar LTR20.
    /// </summary>
    CalendarLtr20 = 0xE24E,
    /// <summary>
    /// The calendar LTR24.
    /// </summary>
    CalendarLtr24 = 0xE24F,
    /// <summary>
    /// The calendar LTR28.
    /// </summary>
    CalendarLtr28 = 0xE250,
    /// <summary>
    /// The calendar LTR32.
    /// </summary>
    CalendarLtr32 = 0xE251,
    /// <summary>
    /// The calendar LTR48.
    /// </summary>
    CalendarLtr48 = 0xE252,
    /// <summary>
    /// The calendar mail16.
    /// </summary>
    CalendarMail16 = 0xE253,
    /// <summary>
    /// The calendar mail20.
    /// </summary>
    CalendarMail20 = 0xE254,
    /// <summary>
    /// The calendar mention20.
    /// </summary>
    CalendarMention20 = 0xE255,
    /// <summary>
    /// The calendar multiple28.
    /// </summary>
    CalendarMultiple28 = 0xE256,
    /// <summary>
    /// The calendar multiple32.
    /// </summary>
    CalendarMultiple32 = 0xE257,
    /// <summary>
    /// The calendar pattern16.
    /// </summary>
    CalendarPattern16 = 0xE258,
    /// <summary>
    /// The calendar pattern20.
    /// </summary>
    CalendarPattern20 = 0xE259,
    /// <summary>
    /// The calendar person16.
    /// </summary>
    CalendarPerson16 = 0xE25A,
    /// <summary>
    /// The calendar person24.
    /// </summary>
    CalendarPerson24 = 0xE25B,
    /// <summary>
    /// The calendar phone16.
    /// </summary>
    CalendarPhone16 = 0xE25C,
    /// <summary>
    /// The calendar phone20.
    /// </summary>
    CalendarPhone20 = 0xE25D,
    /// <summary>
    /// The calendar question mark16.
    /// </summary>
    CalendarQuestionMark16 = 0xE25E,
    /// <summary>
    /// The calendar question mark20.
    /// </summary>
    CalendarQuestionMark20 = 0xE25F,
    /// <summary>
    /// The calendar question mark24.
    /// </summary>
    CalendarQuestionMark24 = 0xE260,
    /// <summary>
    /// The calendar RTL12.
    /// </summary>
    CalendarRtl12 = 0xE261,
    /// <summary>
    /// The calendar RTL16.
    /// </summary>
    CalendarRtl16 = 0xE262,
    /// <summary>
    /// The calendar RTL20.
    /// </summary>
    CalendarRtl20 = 0xE263,
    /// <summary>
    /// The calendar RTL24.
    /// </summary>
    CalendarRtl24 = 0xE264,
    /// <summary>
    /// The calendar RTL28.
    /// </summary>
    CalendarRtl28 = 0xE265,
    /// <summary>
    /// The calendar RTL32.
    /// </summary>
    CalendarRtl32 = 0xE266,
    /// <summary>
    /// The calendar RTL48.
    /// </summary>
    CalendarRtl48 = 0xE267,
    /// <summary>
    /// The calendar search20.
    /// </summary>
    CalendarSearch20 = 0xE268,
    /// <summary>
    /// The calendar settings16.
    /// </summary>
    CalendarSettings16 = 0xE269,
    /// <summary>
    /// The calendar star16.
    /// </summary>
    CalendarStar16 = 0xE26A,
    /// <summary>
    /// The calendar toolbox20.
    /// </summary>
    CalendarToolbox20 = 0xE26B,
    /// <summary>
    /// The calendar toolbox24.
    /// </summary>
    CalendarToolbox24 = 0xE26C,
    /// <summary>
    /// The calendar week numbers20.
    /// </summary>
    CalendarWeekNumbers20 = 0xE26D,
    /// <summary>
    /// The calendar work week28.
    /// </summary>
    CalendarWorkWeek28 = 0xE26E,
    /// <summary>
    /// The call16.
    /// </summary>
    Call16 = 0xE26F,
    /// <summary>
    /// The call20.
    /// </summary>
    Call20 = 0xE270,
    /// <summary>
    /// The call24.
    /// </summary>
    Call24 = 0xE271,
    /// <summary>
    /// The call28.
    /// </summary>
    Call28 = 0xE272,
    /// <summary>
    /// The call32.
    /// </summary>
    Call32 = 0xE273,
    /// <summary>
    /// The call48.
    /// </summary>
    Call48 = 0xE274,
    /// <summary>
    /// The call add16.
    /// </summary>
    CallAdd16 = 0xE275,
    /// <summary>
    /// The call add20.
    /// </summary>
    CallAdd20 = 0xE276,
    /// <summary>
    /// The call checkmark24.
    /// </summary>
    CallCheckmark24 = 0xE277,
    /// <summary>
    /// The call connecting20.
    /// </summary>
    CallConnecting20 = 0xE278,
    /// <summary>
    /// The call dismiss16.
    /// </summary>
    CallDismiss16 = 0xE279,
    /// <summary>
    /// The call end16.
    /// </summary>
    CallEnd16 = 0xE27A,
    /// <summary>
    /// The call exclamation20.
    /// </summary>
    CallExclamation20 = 0xE27B,
    /// <summary>
    /// The call forward16.
    /// </summary>
    CallForward16 = 0xE27C,
    /// <summary>
    /// The call forward20.
    /// </summary>
    CallForward20 = 0xE27D,
    /// <summary>
    /// The call forward28.
    /// </summary>
    CallForward28 = 0xE27E,
    /// <summary>
    /// The call forward48.
    /// </summary>
    CallForward48 = 0xE27F,
    /// <summary>
    /// The call inbound20.
    /// </summary>
    CallInbound20 = 0xE280,
    /// <summary>
    /// The call inbound28.
    /// </summary>
    CallInbound28 = 0xE281,
    /// <summary>
    /// The call inbound48.
    /// </summary>
    CallInbound48 = 0xE282,
    /// <summary>
    /// The call missed20.
    /// </summary>
    CallMissed20 = 0xE283,
    /// <summary>
    /// The call missed28.
    /// </summary>
    CallMissed28 = 0xE284,
    /// <summary>
    /// The call missed48.
    /// </summary>
    CallMissed48 = 0xE285,
    /// <summary>
    /// The call outbound20.
    /// </summary>
    CallOutbound20 = 0xE286,
    /// <summary>
    /// The call outbound28.
    /// </summary>
    CallOutbound28 = 0xE287,
    /// <summary>
    /// The call outbound48.
    /// </summary>
    CallOutbound48 = 0xE288,
    /// <summary>
    /// The call park16.
    /// </summary>
    CallPark16 = 0xE289,
    /// <summary>
    /// The call park20.
    /// </summary>
    CallPark20 = 0xE28A,
    /// <summary>
    /// The call park28.
    /// </summary>
    CallPark28 = 0xE28B,
    /// <summary>
    /// The call park48.
    /// </summary>
    CallPark48 = 0xE28C,
    /// <summary>
    /// The call prohibited16.
    /// </summary>
    CallProhibited16 = 0xE28D,
    /// <summary>
    /// The call prohibited20.
    /// </summary>
    CallProhibited20 = 0xE28E,
    /// <summary>
    /// The call prohibited24.
    /// </summary>
    CallProhibited24 = 0xE28F,
    /// <summary>
    /// The call prohibited28.
    /// </summary>
    CallProhibited28 = 0xE290,
    /// <summary>
    /// The call prohibited48.
    /// </summary>
    CallProhibited48 = 0xE291,
    /// <summary>
    /// The call transfer16.
    /// </summary>
    CallTransfer16 = 0xE292,
    /// <summary>
    /// The call transfer20.
    /// </summary>
    CallTransfer20 = 0xE293,
    /// <summary>
    /// The call warning16.
    /// </summary>
    CallWarning16 = 0xE294,
    /// <summary>
    /// The call warning20.
    /// </summary>
    CallWarning20 = 0xE295,
    /// <summary>
    /// The calligraphy pen checkmark20.
    /// </summary>
    CalligraphyPenCheckmark20 = 0xE296,
    /// <summary>
    /// The calligraphy pen error20.
    /// </summary>
    CalligraphyPenError20 = 0xE297,
    /// <summary>
    /// The calligraphy pen question mark20.
    /// </summary>
    CalligraphyPenQuestionMark20 = 0xE298,
    /// <summary>
    /// The camera16.
    /// </summary>
    Camera16 = 0xE299,
    /// <summary>
    /// The camera dome16.
    /// </summary>
    CameraDome16 = 0xE29A,
    /// <summary>
    /// The camera dome20.
    /// </summary>
    CameraDome20 = 0xE29B,
    /// <summary>
    /// The camera dome24.
    /// </summary>
    CameraDome24 = 0xE29C,
    /// <summary>
    /// The camera dome28.
    /// </summary>
    CameraDome28 = 0xE29D,
    /// <summary>
    /// The camera dome48.
    /// </summary>
    CameraDome48 = 0xE29E,
    /// <summary>
    /// The camera edit20.
    /// </summary>
    CameraEdit20 = 0xE29F,
    /// <summary>
    /// The camera off20.
    /// </summary>
    CameraOff20 = 0xE2A0,
    /// <summary>
    /// The camera off24.
    /// </summary>
    CameraOff24 = 0xE2A1,
    /// <summary>
    /// The camera switch20.
    /// </summary>
    CameraSwitch20 = 0xE2A2,
    /// <summary>
    /// The caret down right12.
    /// </summary>
    CaretDownRight12 = 0xE2A3,
    /// <summary>
    /// The caret down right16.
    /// </summary>
    CaretDownRight16 = 0xE2A4,
    /// <summary>
    /// The caret down right20.
    /// </summary>
    CaretDownRight20 = 0xE2A5,
    /// <summary>
    /// The caret down right24.
    /// </summary>
    CaretDownRight24 = 0xE2A6,
    /// <summary>
    /// The caret up12.
    /// </summary>
    CaretUp12 = 0xE2A7,
    /// <summary>
    /// The caret up16.
    /// </summary>
    CaretUp16 = 0xE2A8,
    /// <summary>
    /// The caret up20.
    /// </summary>
    CaretUp20 = 0xE2A9,
    /// <summary>
    /// The caret up24.
    /// </summary>
    CaretUp24 = 0xE2AA,
    /// <summary>
    /// The cart16.
    /// </summary>
    Cart16 = 0xE2AB,
    /// <summary>
    /// The cart20.
    /// </summary>
    Cart20 = 0xE2AC,
    /// <summary>
    /// The catch up16.
    /// </summary>
    CatchUp16 = 0xE2AD,
    /// <summary>
    /// The catch up20.
    /// </summary>
    CatchUp20 = 0xE2AE,
    /// <summary>
    /// The catch up24.
    /// </summary>
    CatchUp24 = 0xE2AF,
    /// <summary>
    /// The cellular3g20.
    /// </summary>
    Cellular3g20 = 0xE2B0,
    /// <summary>
    /// The cellular4g20.
    /// </summary>
    Cellular4g20 = 0xE2B1,
    /// <summary>
    /// The cellular5g20.
    /// </summary>
    Cellular5g20 = 0xE2B2,
    /// <summary>
    /// The cellular5g24.
    /// </summary>
    Cellular5g24 = 0xE2B3,
    /// <summary>
    /// The cellular off20.
    /// </summary>
    CellularOff20 = 0xE2B4,
    /// <summary>
    /// The cellular off24.
    /// </summary>
    CellularOff24 = 0xE2B5,
    /// <summary>
    /// The cellular warning20.
    /// </summary>
    CellularWarning20 = 0xE2B6,
    /// <summary>
    /// The cellular warning24.
    /// </summary>
    CellularWarning24 = 0xE2B7,
    /// <summary>
    /// The center horizontal20.
    /// </summary>
    CenterHorizontal20 = 0xE2B8,
    /// <summary>
    /// The center horizontal24.
    /// </summary>
    CenterHorizontal24 = 0xE2B9,
    /// <summary>
    /// The center vertical20.
    /// </summary>
    CenterVertical20 = 0xE2BA,
    /// <summary>
    /// The center vertical24.
    /// </summary>
    CenterVertical24 = 0xE2BB,
    /// <summary>
    /// The channel28.
    /// </summary>
    Channel28 = 0xE2BC,
    /// <summary>
    /// The channel48.
    /// </summary>
    Channel48 = 0xE2BD,
    /// <summary>
    /// The channel add16.
    /// </summary>
    ChannelAdd16 = 0xE2BE,
    /// <summary>
    /// The channel add20.
    /// </summary>
    ChannelAdd20 = 0xE2BF,
    /// <summary>
    /// The channel add24.
    /// </summary>
    ChannelAdd24 = 0xE2C0,
    /// <summary>
    /// The channel add28.
    /// </summary>
    ChannelAdd28 = 0xE2C1,
    /// <summary>
    /// The channel add48.
    /// </summary>
    ChannelAdd48 = 0xE2C2,
    /// <summary>
    /// The channel alert16.
    /// </summary>
    ChannelAlert16 = 0xE2C3,
    /// <summary>
    /// The channel alert20.
    /// </summary>
    ChannelAlert20 = 0xE2C4,
    /// <summary>
    /// The channel alert24.
    /// </summary>
    ChannelAlert24 = 0xE2C5,
    /// <summary>
    /// The channel alert28.
    /// </summary>
    ChannelAlert28 = 0xE2C6,
    /// <summary>
    /// The channel alert48.
    /// </summary>
    ChannelAlert48 = 0xE2C7,
    /// <summary>
    /// The channel arrow left16.
    /// </summary>
    ChannelArrowLeft16 = 0xE2C8,
    /// <summary>
    /// The channel arrow left20.
    /// </summary>
    ChannelArrowLeft20 = 0xE2C9,
    /// <summary>
    /// The channel arrow left24.
    /// </summary>
    ChannelArrowLeft24 = 0xE2CA,
    /// <summary>
    /// The channel arrow left28.
    /// </summary>
    ChannelArrowLeft28 = 0xE2CB,
    /// <summary>
    /// The channel arrow left48.
    /// </summary>
    ChannelArrowLeft48 = 0xE2CC,
    /// <summary>
    /// The channel dismiss16.
    /// </summary>
    ChannelDismiss16 = 0xE2CD,
    /// <summary>
    /// The channel dismiss20.
    /// </summary>
    ChannelDismiss20 = 0xE2CE,
    /// <summary>
    /// The channel dismiss24.
    /// </summary>
    ChannelDismiss24 = 0xE2CF,
    /// <summary>
    /// The channel dismiss28.
    /// </summary>
    ChannelDismiss28 = 0xE2D0,
    /// <summary>
    /// The channel dismiss48.
    /// </summary>
    ChannelDismiss48 = 0xE2D1,
    /// <summary>
    /// The channel share12.
    /// </summary>
    ChannelShare12 = 0xE2D2,
    /// <summary>
    /// The channel share16.
    /// </summary>
    ChannelShare16 = 0xE2D3,
    /// <summary>
    /// The channel share20.
    /// </summary>
    ChannelShare20 = 0xE2D4,
    /// <summary>
    /// The channel share24.
    /// </summary>
    ChannelShare24 = 0xE2D5,
    /// <summary>
    /// The channel share28.
    /// </summary>
    ChannelShare28 = 0xE2D6,
    /// <summary>
    /// The channel share48.
    /// </summary>
    ChannelShare48 = 0xE2D7,
    /// <summary>
    /// The channel subtract16.
    /// </summary>
    ChannelSubtract16 = 0xE2D8,
    /// <summary>
    /// The channel subtract20.
    /// </summary>
    ChannelSubtract20 = 0xE2D9,
    /// <summary>
    /// The channel subtract24.
    /// </summary>
    ChannelSubtract24 = 0xE2DA,
    /// <summary>
    /// The channel subtract28.
    /// </summary>
    ChannelSubtract28 = 0xE2DB,
    /// <summary>
    /// The channel subtract48.
    /// </summary>
    ChannelSubtract48 = 0xE2DC,
    /// <summary>
    /// The chart multiple20.
    /// </summary>
    ChartMultiple20 = 0xE2DD,
    /// <summary>
    /// The chart multiple24.
    /// </summary>
    ChartMultiple24 = 0xE2DE,
    /// <summary>
    /// The chart person20.
    /// </summary>
    ChartPerson20 = 0xE2DF,
    /// <summary>
    /// The chart person24.
    /// </summary>
    ChartPerson24 = 0xE2E0,
    /// <summary>
    /// The chart person28.
    /// </summary>
    ChartPerson28 = 0xE2E1,
    /// <summary>
    /// The chart person48.
    /// </summary>
    ChartPerson48 = 0xE2E2,
    /// <summary>
    /// The chat12.
    /// </summary>
    Chat12 = 0xE2E3,
    /// <summary>
    /// The chat16.
    /// </summary>
    Chat16 = 0xE2E4,
    /// <summary>
    /// The chat32.
    /// </summary>
    Chat32 = 0xE2E5,
    /// <summary>
    /// The chat48.
    /// </summary>
    Chat48 = 0xE2E6,
    /// <summary>
    /// The chat arrow back16.
    /// </summary>
    ChatArrowBack16 = 0xE2E7,
    /// <summary>
    /// The chat arrow back20.
    /// </summary>
    ChatArrowBack20 = 0xE2E8,
    /// <summary>
    /// The chat arrow double back16.
    /// </summary>
    ChatArrowDoubleBack16 = 0xE2E9,
    /// <summary>
    /// The chat arrow double back20.
    /// </summary>
    ChatArrowDoubleBack20 = 0xE2EA,
    /// <summary>
    /// The chat bubbles question20.
    /// </summary>
    ChatBubblesQuestion20 = 0xE2EB,
    /// <summary>
    /// The chat dismiss16.
    /// </summary>
    ChatDismiss16 = 0xE2EC,
    /// <summary>
    /// The chat dismiss20.
    /// </summary>
    ChatDismiss20 = 0xE2ED,
    /// <summary>
    /// The chat dismiss24.
    /// </summary>
    ChatDismiss24 = 0xE2EE,
    /// <summary>
    /// The chat mail20.
    /// </summary>
    ChatMail20 = 0xE2EF,
    /// <summary>
    /// The chat off20.
    /// </summary>
    ChatOff20 = 0xE2F0,
    /// <summary>
    /// The chat video20.
    /// </summary>
    ChatVideo20 = 0xE2F1,
    /// <summary>
    /// The chat video24.
    /// </summary>
    ChatVideo24 = 0xE2F2,
    /// <summary>
    /// The chat warning16.
    /// </summary>
    ChatWarning16 = 0xE2F3,
    /// <summary>
    /// The chat warning20.
    /// </summary>
    ChatWarning20 = 0xE2F4,
    /// <summary>
    /// The check24.
    /// </summary>
    Check24 = 0xE2F5,
    /// <summary>
    /// The checkbox120.
    /// </summary>
    Checkbox120 = 0xE2F6,
    /// <summary>
    /// The checkbox124.
    /// </summary>
    Checkbox124 = 0xE2F7,
    /// <summary>
    /// The checkbox220.
    /// </summary>
    Checkbox220 = 0xE2F8,
    /// <summary>
    /// The checkbox224.
    /// </summary>
    Checkbox224 = 0xE2F9,
    /// <summary>
    /// The checkbox arrow right20.
    /// </summary>
    CheckboxArrowRight20 = 0xE2FA,
    /// <summary>
    /// The checkbox arrow right24.
    /// </summary>
    CheckboxArrowRight24 = 0xE2FB,
    /// <summary>
    /// The checkbox checked sync20.
    /// </summary>
    CheckboxCheckedSync20 = 0xE2FC,
    /// <summary>
    /// The checkbox indeterminate16.
    /// </summary>
    CheckboxIndeterminate16 = 0xE2FD,
    /// <summary>
    /// The checkbox indeterminate20.
    /// </summary>
    CheckboxIndeterminate20 = 0xE2FE,
    /// <summary>
    /// The checkbox indeterminate24.
    /// </summary>
    CheckboxIndeterminate24 = 0xE2FF,
    /// <summary>
    /// The checkbox person16.
    /// </summary>
    CheckboxPerson16 = 0xE300,
    /// <summary>
    /// The checkbox person20.
    /// </summary>
    CheckboxPerson20 = 0xE301,
    /// <summary>
    /// The checkbox person24.
    /// </summary>
    CheckboxPerson24 = 0xE302,
    /// <summary>
    /// The checkbox warning20.
    /// </summary>
    CheckboxWarning20 = 0xE303,
    /// <summary>
    /// The checkbox warning24.
    /// </summary>
    CheckboxWarning24 = 0xE304,
    /// <summary>
    /// The checkmark16.
    /// </summary>
    Checkmark16 = 0xE305,
    /// <summary>
    /// The checkmark48.
    /// </summary>
    Checkmark48 = 0xE306,
    /// <summary>
    /// The checkmark circle12.
    /// </summary>
    CheckmarkCircle12 = 0xE307,
    /// <summary>
    /// The checkmark note20.
    /// </summary>
    CheckmarkNote20 = 0xE308,
    /// <summary>
    /// The checkmark square20.
    /// </summary>
    CheckmarkSquare20 = 0xE309,
    /// <summary>
    /// The checkmark starburst20.
    /// </summary>
    CheckmarkStarburst20 = 0xE30A,
    /// <summary>
    /// The checkmark starburst24.
    /// </summary>
    CheckmarkStarburst24 = 0xE30B,
    /// <summary>
    /// The chess20.
    /// </summary>
    Chess20 = 0xE30C,
    /// <summary>
    /// The chevron circle down12.
    /// </summary>
    ChevronCircleDown12 = 0xE30D,
    /// <summary>
    /// The chevron circle down16.
    /// </summary>
    ChevronCircleDown16 = 0xE30E,
    /// <summary>
    /// The chevron circle down20.
    /// </summary>
    ChevronCircleDown20 = 0xE30F,
    /// <summary>
    /// The chevron circle down24.
    /// </summary>
    ChevronCircleDown24 = 0xE310,
    /// <summary>
    /// The chevron circle down28.
    /// </summary>
    ChevronCircleDown28 = 0xE311,
    /// <summary>
    /// The chevron circle down32.
    /// </summary>
    ChevronCircleDown32 = 0xE312,
    /// <summary>
    /// The chevron circle down48.
    /// </summary>
    ChevronCircleDown48 = 0xE313,
    /// <summary>
    /// The chevron circle left12.
    /// </summary>
    ChevronCircleLeft12 = 0xE314,
    /// <summary>
    /// The chevron circle left16.
    /// </summary>
    ChevronCircleLeft16 = 0xE315,
    /// <summary>
    /// The chevron circle left20.
    /// </summary>
    ChevronCircleLeft20 = 0xE316,
    /// <summary>
    /// The chevron circle left24.
    /// </summary>
    ChevronCircleLeft24 = 0xE317,
    /// <summary>
    /// The chevron circle left28.
    /// </summary>
    ChevronCircleLeft28 = 0xE318,
    /// <summary>
    /// The chevron circle left32.
    /// </summary>
    ChevronCircleLeft32 = 0xE319,
    /// <summary>
    /// The chevron circle left48.
    /// </summary>
    ChevronCircleLeft48 = 0xE31A,
    /// <summary>
    /// The chevron circle right12.
    /// </summary>
    ChevronCircleRight12 = 0xE31B,
    /// <summary>
    /// The chevron circle right16.
    /// </summary>
    ChevronCircleRight16 = 0xE31C,
    /// <summary>
    /// The chevron circle right20.
    /// </summary>
    ChevronCircleRight20 = 0xE31D,
    /// <summary>
    /// The chevron circle right24.
    /// </summary>
    ChevronCircleRight24 = 0xE31E,
    /// <summary>
    /// The chevron circle right28.
    /// </summary>
    ChevronCircleRight28 = 0xE31F,
    /// <summary>
    /// The chevron circle right32.
    /// </summary>
    ChevronCircleRight32 = 0xE320,
    /// <summary>
    /// The chevron circle right48.
    /// </summary>
    ChevronCircleRight48 = 0xE321,
    /// <summary>
    /// The chevron circle up12.
    /// </summary>
    ChevronCircleUp12 = 0xE322,
    /// <summary>
    /// The chevron circle up16.
    /// </summary>
    ChevronCircleUp16 = 0xE323,
    /// <summary>
    /// The chevron circle up20.
    /// </summary>
    ChevronCircleUp20 = 0xE324,
    /// <summary>
    /// The chevron circle up24.
    /// </summary>
    ChevronCircleUp24 = 0xE325,
    /// <summary>
    /// The chevron circle up28.
    /// </summary>
    ChevronCircleUp28 = 0xE326,
    /// <summary>
    /// The chevron circle up32.
    /// </summary>
    ChevronCircleUp32 = 0xE327,
    /// <summary>
    /// The chevron circle up48.
    /// </summary>
    ChevronCircleUp48 = 0xE328,
    /// <summary>
    /// The chevron double down20.
    /// </summary>
    ChevronDoubleDown20 = 0xE329,
    /// <summary>
    /// The chevron double left20.
    /// </summary>
    ChevronDoubleLeft20 = 0xE32A,
    /// <summary>
    /// The chevron double right20.
    /// </summary>
    ChevronDoubleRight20 = 0xE32B,
    /// <summary>
    /// The chevron double up16.
    /// </summary>
    ChevronDoubleUp16 = 0xE32C,
    /// <summary>
    /// The chevron double up20.
    /// </summary>
    ChevronDoubleUp20 = 0xE32D,
    /// <summary>
    /// The chevron up down16.
    /// </summary>
    ChevronUpDown16 = 0xE32E,
    /// <summary>
    /// The chevron up down20.
    /// </summary>
    ChevronUpDown20 = 0xE32F,
    /// <summary>
    /// The chevron up down24.
    /// </summary>
    ChevronUpDown24 = 0xE330,
    /// <summary>
    /// The circle12.
    /// </summary>
    Circle12 = 0xE331,
    /// <summary>
    /// The circle32.
    /// </summary>
    Circle32 = 0xE332,
    /// <summary>
    /// The circle48.
    /// </summary>
    Circle48 = 0xE333,
    /// <summary>
    /// The circle edit20.
    /// </summary>
    CircleEdit20 = 0xE334,
    /// <summary>
    /// The circle edit24.
    /// </summary>
    CircleEdit24 = 0xE335,
    /// <summary>
    /// The circle eraser20.
    /// </summary>
    CircleEraser20 = 0xE336,
    /// <summary>
    /// The circle half fill12.
    /// </summary>
    CircleHalfFill12 = 0xE337,
    /// <summary>
    /// The circle image20.
    /// </summary>
    CircleImage20 = 0xE338,
    /// <summary>
    /// The circle line12.
    /// </summary>
    CircleLine12 = 0xE339,
    /// <summary>
    /// The circle line20.
    /// </summary>
    CircleLine20 = 0xE33A,
    /// <summary>
    /// The circle multiple subtract checkmark20.
    /// </summary>
    CircleMultipleSubtractCheckmark20 = 0xE33B,
    /// <summary>
    /// The circle off16.
    /// </summary>
    CircleOff16 = 0xE33C,
    /// <summary>
    /// The circle off20.
    /// </summary>
    CircleOff20 = 0xE33D,
    /// <summary>
    /// The circle small20.
    /// </summary>
    CircleSmall20 = 0xE33E,
    /// <summary>
    /// The class20.
    /// </summary>
    Class20 = 0xE33F,
    /// <summary>
    /// The clear formatting16.
    /// </summary>
    ClearFormatting16 = 0xE340,
    /// <summary>
    /// The clear formatting20.
    /// </summary>
    ClearFormatting20 = 0xE341,
    /// <summary>
    /// The clipboard16.
    /// </summary>
    Clipboard16 = 0xE342,
    /// <summary>
    /// The clipboard32.
    /// </summary>
    Clipboard32 = 0xE343,
    /// <summary>
    /// The clipboard arrow right16.
    /// </summary>
    ClipboardArrowRight16 = 0xE344,
    /// <summary>
    /// The clipboard arrow right20.
    /// </summary>
    ClipboardArrowRight20 = 0xE345,
    /// <summary>
    /// The clipboard arrow right24.
    /// </summary>
    ClipboardArrowRight24 = 0xE346,
    /// <summary>
    /// The clipboard bullet list LTR16.
    /// </summary>
    ClipboardBulletListLtr16 = 0xE347,
    /// <summary>
    /// The clipboard bullet list LTR20.
    /// </summary>
    ClipboardBulletListLtr20 = 0xE348,
    /// <summary>
    /// The clipboard bullet list RTL16.
    /// </summary>
    ClipboardBulletListRtl16 = 0xE349,
    /// <summary>
    /// The clipboard bullet list RTL20.
    /// </summary>
    ClipboardBulletListRtl20 = 0xE34A,
    /// <summary>
    /// The clipboard checkmark20.
    /// </summary>
    ClipboardCheckmark20 = 0xE34B,
    /// <summary>
    /// The clipboard checkmark24.
    /// </summary>
    ClipboardCheckmark24 = 0xE34C,
    /// <summary>
    /// The clipboard clock20.
    /// </summary>
    ClipboardClock20 = 0xE34D,
    /// <summary>
    /// The clipboard clock24.
    /// </summary>
    ClipboardClock24 = 0xE34E,
    /// <summary>
    /// The clipboard data bar20.
    /// </summary>
    ClipboardDataBar20 = 0xE34F,
    /// <summary>
    /// The clipboard data bar24.
    /// </summary>
    ClipboardDataBar24 = 0xE350,
    /// <summary>
    /// The clipboard data bar32.
    /// </summary>
    ClipboardDataBar32 = 0xE351,
    /// <summary>
    /// The clipboard edit20.
    /// </summary>
    ClipboardEdit20 = 0xE352,
    /// <summary>
    /// The clipboard error20.
    /// </summary>
    ClipboardError20 = 0xE353,
    /// <summary>
    /// The clipboard error24.
    /// </summary>
    ClipboardError24 = 0xE354,
    /// <summary>
    /// The clipboard heart24.
    /// </summary>
    ClipboardHeart24 = 0xE355,
    /// <summary>
    /// The clipboard image20.
    /// </summary>
    ClipboardImage20 = 0xE356,
    /// <summary>
    /// The clipboard image24.
    /// </summary>
    ClipboardImage24 = 0xE357,
    /// <summary>
    /// The clipboard more20.
    /// </summary>
    ClipboardMore20 = 0xE358,
    /// <summary>
    /// The clipboard note20.
    /// </summary>
    ClipboardNote20 = 0xE359,
    /// <summary>
    /// The clipboard paste16.
    /// </summary>
    ClipboardPaste16 = 0xE35A,
    /// <summary>
    /// The clipboard pulse24.
    /// </summary>
    ClipboardPulse24 = 0xE35B,
    /// <summary>
    /// The clipboard settings24.
    /// </summary>
    ClipboardSettings24 = 0xE35C,
    /// <summary>
    /// The clipboard task20.
    /// </summary>
    ClipboardTask20 = 0xE35D,
    /// <summary>
    /// The clipboard task24.
    /// </summary>
    ClipboardTask24 = 0xE35E,
    /// <summary>
    /// The clipboard task add20.
    /// </summary>
    ClipboardTaskAdd20 = 0xE35F,
    /// <summary>
    /// The clipboard task add24.
    /// </summary>
    ClipboardTaskAdd24 = 0xE360,
    /// <summary>
    /// The clipboard task list LTR20.
    /// </summary>
    ClipboardTaskListLtr20 = 0xE361,
    /// <summary>
    /// The clipboard task list LTR24.
    /// </summary>
    ClipboardTaskListLtr24 = 0xE362,
    /// <summary>
    /// The clipboard task list RTL20.
    /// </summary>
    ClipboardTaskListRtl20 = 0xE363,
    /// <summary>
    /// The clipboard task list RTL24.
    /// </summary>
    ClipboardTaskListRtl24 = 0xE364,
    /// <summary>
    /// The clipboard text32.
    /// </summary>
    ClipboardText32 = 0xE365,
    /// <summary>
    /// The clipboard text edit20.
    /// </summary>
    ClipboardTextEdit20 = 0xE366,
    /// <summary>
    /// The clipboard text edit24.
    /// </summary>
    ClipboardTextEdit24 = 0xE367,
    /// <summary>
    /// The clipboard text edit32.
    /// </summary>
    ClipboardTextEdit32 = 0xE368,
    /// <summary>
    /// The clipboard text LTR20.
    /// </summary>
    ClipboardTextLtr20 = 0xE369,
    /// <summary>
    /// The clipboard text LTR24.
    /// </summary>
    ClipboardTextLtr24 = 0xE36A,
    /// <summary>
    /// The clipboard text LTR32.
    /// </summary>
    ClipboardTextLtr32 = 0xE36B,
    /// <summary>
    /// The clipboard text RTL20.
    /// </summary>
    ClipboardTextRtl20 = 0xE36C,
    /// <summary>
    /// The clipboard text RTL24.
    /// </summary>
    ClipboardTextRtl24 = 0xE36D,
    /// <summary>
    /// The clock32.
    /// </summary>
    Clock32 = 0xE36E,
    /// <summary>
    /// The clock alarm16.
    /// </summary>
    ClockAlarm16 = 0xE36F,
    /// <summary>
    /// The clock alarm32.
    /// </summary>
    ClockAlarm32 = 0xE370,
    /// <summary>
    /// The clock arrow download24.
    /// </summary>
    ClockArrowDownload24 = 0xE371,
    /// <summary>
    /// The clock dismiss20.
    /// </summary>
    ClockDismiss20 = 0xE372,
    /// <summary>
    /// The clock dismiss24.
    /// </summary>
    ClockDismiss24 = 0xE373,
    /// <summary>
    /// The clock pause20.
    /// </summary>
    ClockPause20 = 0xE374,
    /// <summary>
    /// The clock pause24.
    /// </summary>
    ClockPause24 = 0xE375,
    /// <summary>
    /// The clock toolbox20.
    /// </summary>
    ClockToolbox20 = 0xE376,
    /// <summary>
    /// The clock toolbox24.
    /// </summary>
    ClockToolbox24 = 0xE377,
    /// <summary>
    /// The closed caption16.
    /// </summary>
    ClosedCaption16 = 0xE378,
    /// <summary>
    /// The closed caption20.
    /// </summary>
    ClosedCaption20 = 0xE379,
    /// <summary>
    /// The closed caption28.
    /// </summary>
    ClosedCaption28 = 0xE37A,
    /// <summary>
    /// The closed caption32.
    /// </summary>
    ClosedCaption32 = 0xE37B,
    /// <summary>
    /// The closed caption48.
    /// </summary>
    ClosedCaption48 = 0xE37C,
    /// <summary>
    /// The closed caption off16.
    /// </summary>
    ClosedCaptionOff16 = 0xE37D,
    /// <summary>
    /// The closed caption off20.
    /// </summary>
    ClosedCaptionOff20 = 0xE37E,
    /// <summary>
    /// The closed caption off24.
    /// </summary>
    ClosedCaptionOff24 = 0xE37F,
    /// <summary>
    /// The closed caption off28.
    /// </summary>
    ClosedCaptionOff28 = 0xE380,
    /// <summary>
    /// The closed caption off48.
    /// </summary>
    ClosedCaptionOff48 = 0xE381,
    /// <summary>
    /// The cloud16.
    /// </summary>
    Cloud16 = 0xE382,
    /// <summary>
    /// The cloud28.
    /// </summary>
    Cloud28 = 0xE383,
    /// <summary>
    /// The cloud32.
    /// </summary>
    Cloud32 = 0xE384,
    /// <summary>
    /// The cloud add20.
    /// </summary>
    CloudAdd20 = 0xE385,
    /// <summary>
    /// The cloud archive16.
    /// </summary>
    CloudArchive16 = 0xE386,
    /// <summary>
    /// The cloud archive20.
    /// </summary>
    CloudArchive20 = 0xE387,
    /// <summary>
    /// The cloud archive24.
    /// </summary>
    CloudArchive24 = 0xE388,
    /// <summary>
    /// The cloud archive28.
    /// </summary>
    CloudArchive28 = 0xE389,
    /// <summary>
    /// The cloud archive32.
    /// </summary>
    CloudArchive32 = 0xE38A,
    /// <summary>
    /// The cloud archive48.
    /// </summary>
    CloudArchive48 = 0xE38B,
    /// <summary>
    /// The cloud arrow down16.
    /// </summary>
    CloudArrowDown16 = 0xE38C,
    /// <summary>
    /// The cloud arrow down20.
    /// </summary>
    CloudArrowDown20 = 0xE38D,
    /// <summary>
    /// The cloud arrow down24.
    /// </summary>
    CloudArrowDown24 = 0xE38E,
    /// <summary>
    /// The cloud arrow down28.
    /// </summary>
    CloudArrowDown28 = 0xE38F,
    /// <summary>
    /// The cloud arrow down32.
    /// </summary>
    CloudArrowDown32 = 0xE390,
    /// <summary>
    /// The cloud arrow down48.
    /// </summary>
    CloudArrowDown48 = 0xE391,
    /// <summary>
    /// The cloud arrow up16.
    /// </summary>
    CloudArrowUp16 = 0xE392,
    /// <summary>
    /// The cloud arrow up20.
    /// </summary>
    CloudArrowUp20 = 0xE393,
    /// <summary>
    /// The cloud arrow up24.
    /// </summary>
    CloudArrowUp24 = 0xE394,
    /// <summary>
    /// The cloud arrow up28.
    /// </summary>
    CloudArrowUp28 = 0xE395,
    /// <summary>
    /// The cloud arrow up32.
    /// </summary>
    CloudArrowUp32 = 0xE396,
    /// <summary>
    /// The cloud arrow up48.
    /// </summary>
    CloudArrowUp48 = 0xE397,
    /// <summary>
    /// The cloud checkmark16.
    /// </summary>
    CloudCheckmark16 = 0xE398,
    /// <summary>
    /// The cloud checkmark20.
    /// </summary>
    CloudCheckmark20 = 0xE399,
    /// <summary>
    /// The cloud checkmark24.
    /// </summary>
    CloudCheckmark24 = 0xE39A,
    /// <summary>
    /// The cloud checkmark28.
    /// </summary>
    CloudCheckmark28 = 0xE39B,
    /// <summary>
    /// The cloud checkmark32.
    /// </summary>
    CloudCheckmark32 = 0xE39C,
    /// <summary>
    /// The cloud checkmark48.
    /// </summary>
    CloudCheckmark48 = 0xE39D,
    /// <summary>
    /// The cloud dismiss16.
    /// </summary>
    CloudDismiss16 = 0xE39E,
    /// <summary>
    /// The cloud dismiss20.
    /// </summary>
    CloudDismiss20 = 0xE39F,
    /// <summary>
    /// The cloud dismiss24.
    /// </summary>
    CloudDismiss24 = 0xE3A0,
    /// <summary>
    /// The cloud dismiss28.
    /// </summary>
    CloudDismiss28 = 0xE3A1,
    /// <summary>
    /// The cloud dismiss32.
    /// </summary>
    CloudDismiss32 = 0xE3A2,
    /// <summary>
    /// The cloud dismiss48.
    /// </summary>
    CloudDismiss48 = 0xE3A3,
    /// <summary>
    /// The cloud edit20.
    /// </summary>
    CloudEdit20 = 0xE3A4,
    /// <summary>
    /// The cloud flow24.
    /// </summary>
    CloudFlow24 = 0xE3A5,
    /// <summary>
    /// The cloud link20.
    /// </summary>
    CloudLink20 = 0xE3A6,
    /// <summary>
    /// The cloud off16.
    /// </summary>
    CloudOff16 = 0xE3A7,
    /// <summary>
    /// The cloud off20.
    /// </summary>
    CloudOff20 = 0xE3A8,
    /// <summary>
    /// The cloud off28.
    /// </summary>
    CloudOff28 = 0xE3A9,
    /// <summary>
    /// The cloud off32.
    /// </summary>
    CloudOff32 = 0xE3AA,
    /// <summary>
    /// The cloud swap20.
    /// </summary>
    CloudSwap20 = 0xE3AB,
    /// <summary>
    /// The cloud swap24.
    /// </summary>
    CloudSwap24 = 0xE3AC,
    /// <summary>
    /// The cloud sync16.
    /// </summary>
    CloudSync16 = 0xE3AD,
    /// <summary>
    /// The cloud sync20.
    /// </summary>
    CloudSync20 = 0xE3AE,
    /// <summary>
    /// The cloud sync24.
    /// </summary>
    CloudSync24 = 0xE3AF,
    /// <summary>
    /// The cloud sync28.
    /// </summary>
    CloudSync28 = 0xE3B0,
    /// <summary>
    /// The cloud sync32.
    /// </summary>
    CloudSync32 = 0xE3B1,
    /// <summary>
    /// The cloud sync48.
    /// </summary>
    CloudSync48 = 0xE3B2,
    /// <summary>
    /// The cloud words16.
    /// </summary>
    CloudWords16 = 0xE3B3,
    /// <summary>
    /// The cloud words20.
    /// </summary>
    CloudWords20 = 0xE3B4,
    /// <summary>
    /// The cloud words24.
    /// </summary>
    CloudWords24 = 0xE3B5,
    /// <summary>
    /// The cloud words28.
    /// </summary>
    CloudWords28 = 0xE3B6,
    /// <summary>
    /// The cloud words32.
    /// </summary>
    CloudWords32 = 0xE3B7,
    /// <summary>
    /// The cloud words48.
    /// </summary>
    CloudWords48 = 0xE3B8,
    /// <summary>
    /// The code circle20.
    /// </summary>
    CodeCircle20 = 0xE3B9,
    /// <summary>
    /// The code text20.
    /// </summary>
    CodeText20 = 0xE3BA,
    /// <summary>
    /// The code text edit20.
    /// </summary>
    CodeTextEdit20 = 0xE3BB,
    /// <summary>
    /// The color16.
    /// </summary>
    Color16 = 0xE3BC,
    /// <summary>
    /// The color fill16.
    /// </summary>
    ColorFill16 = 0xE3BF,
    /// <summary>
    /// The color fill28.
    /// </summary>
    ColorFill28 = 0xE3C0,
    /// <summary>
    /// The color line16.
    /// </summary>
    ColorLine16 = 0xE3C5,
    /// <summary>
    /// The column20.
    /// </summary>
    Column20 = 0xE3C9,
    /// <summary>
    /// The column arrow right20.
    /// </summary>
    ColumnArrowRight20 = 0xE3CA,
    /// <summary>
    /// The column double compare20.
    /// </summary>
    ColumnDoubleCompare20 = 0xE3CB,
    /// <summary>
    /// The column edit20.
    /// </summary>
    ColumnEdit20 = 0xE3CC,
    /// <summary>
    /// The column edit24.
    /// </summary>
    ColumnEdit24 = 0xE3CD,
    /// <summary>
    /// The column triple20.
    /// </summary>
    ColumnTriple20 = 0xE3CE,
    /// <summary>
    /// The column triple edit20.
    /// </summary>
    ColumnTripleEdit20 = 0xE3CF,
    /// <summary>
    /// The column triple edit24.
    /// </summary>
    ColumnTripleEdit24 = 0xE3D0,
    /// <summary>
    /// The comma20.
    /// </summary>
    Comma20 = 0xE3D1,
    /// <summary>
    /// The comma24.
    /// </summary>
    Comma24 = 0xE3D2,
    /// <summary>
    /// The comment12.
    /// </summary>
    Comment12 = 0xE3D3,
    /// <summary>
    /// The comment28.
    /// </summary>
    Comment28 = 0xE3D4,
    /// <summary>
    /// The comment48.
    /// </summary>
    Comment48 = 0xE3D5,
    /// <summary>
    /// The comment add12.
    /// </summary>
    CommentAdd12 = 0xE3D6,
    /// <summary>
    /// The comment add16.
    /// </summary>
    CommentAdd16 = 0xE3D7,
    /// <summary>
    /// The comment add20.
    /// </summary>
    CommentAdd20 = 0xE3D8,
    /// <summary>
    /// The comment add28.
    /// </summary>
    CommentAdd28 = 0xE3D9,
    /// <summary>
    /// The comment add48.
    /// </summary>
    CommentAdd48 = 0xE3DA,
    /// <summary>
    /// The comment arrow left12.
    /// </summary>
    CommentArrowLeft12 = 0xE3DB,
    /// <summary>
    /// The comment arrow left16.
    /// </summary>
    CommentArrowLeft16 = 0xE3DC,
    /// <summary>
    /// The comment arrow left20.
    /// </summary>
    CommentArrowLeft20 = 0xE3DD,
    /// <summary>
    /// The comment arrow left24.
    /// </summary>
    CommentArrowLeft24 = 0xE3DE,
    /// <summary>
    /// The comment arrow left28.
    /// </summary>
    CommentArrowLeft28 = 0xE3DF,
    /// <summary>
    /// The comment arrow left48.
    /// </summary>
    CommentArrowLeft48 = 0xE3E0,
    /// <summary>
    /// The comment arrow right12.
    /// </summary>
    CommentArrowRight12 = 0xE3E1,
    /// <summary>
    /// The comment arrow right16.
    /// </summary>
    CommentArrowRight16 = 0xE3E2,
    /// <summary>
    /// The comment arrow right20.
    /// </summary>
    CommentArrowRight20 = 0xE3E3,
    /// <summary>
    /// The comment arrow right24.
    /// </summary>
    CommentArrowRight24 = 0xE3E4,
    /// <summary>
    /// The comment arrow right28.
    /// </summary>
    CommentArrowRight28 = 0xE3E5,
    /// <summary>
    /// The comment arrow right48.
    /// </summary>
    CommentArrowRight48 = 0xE3E6,
    /// <summary>
    /// The comment checkmark12.
    /// </summary>
    CommentCheckmark12 = 0xE3E7,
    /// <summary>
    /// The comment checkmark16.
    /// </summary>
    CommentCheckmark16 = 0xE3E8,
    /// <summary>
    /// The comment checkmark20.
    /// </summary>
    CommentCheckmark20 = 0xE3E9,
    /// <summary>
    /// The comment checkmark24.
    /// </summary>
    CommentCheckmark24 = 0xE3EA,
    /// <summary>
    /// The comment checkmark28.
    /// </summary>
    CommentCheckmark28 = 0xE3EB,
    /// <summary>
    /// The comment checkmark48.
    /// </summary>
    CommentCheckmark48 = 0xE3EC,
    /// <summary>
    /// The comment dismiss20.
    /// </summary>
    CommentDismiss20 = 0xE3ED,
    /// <summary>
    /// The comment dismiss24.
    /// </summary>
    CommentDismiss24 = 0xE3EE,
    /// <summary>
    /// The comment edit20.
    /// </summary>
    CommentEdit20 = 0xE3EF,
    /// <summary>
    /// The comment edit24.
    /// </summary>
    CommentEdit24 = 0xE3F0,
    /// <summary>
    /// The comment error20.
    /// </summary>
    CommentError20 = 0xE3F1,
    /// <summary>
    /// The comment error24.
    /// </summary>
    CommentError24 = 0xE3F2,
    /// <summary>
    /// The comment multiple28.
    /// </summary>
    CommentMultiple28 = 0xE3F3,
    /// <summary>
    /// The comment multiple32.
    /// </summary>
    CommentMultiple32 = 0xE3F4,
    /// <summary>
    /// The comment multiple checkmark16.
    /// </summary>
    CommentMultipleCheckmark16 = 0xE3F5,
    /// <summary>
    /// The comment multiple checkmark20.
    /// </summary>
    CommentMultipleCheckmark20 = 0xE3F6,
    /// <summary>
    /// The comment multiple checkmark24.
    /// </summary>
    CommentMultipleCheckmark24 = 0xE3F7,
    /// <summary>
    /// The comment multiple checkmark28.
    /// </summary>
    CommentMultipleCheckmark28 = 0xE3F8,
    /// <summary>
    /// The comment multiple link16.
    /// </summary>
    CommentMultipleLink16 = 0xE3F9,
    /// <summary>
    /// The comment multiple link20.
    /// </summary>
    CommentMultipleLink20 = 0xE3FA,
    /// <summary>
    /// The comment multiple link24.
    /// </summary>
    CommentMultipleLink24 = 0xE3FB,
    /// <summary>
    /// The comment multiple link28.
    /// </summary>
    CommentMultipleLink28 = 0xE3FC,
    /// <summary>
    /// The comment multiple link32.
    /// </summary>
    CommentMultipleLink32 = 0xE3FD,
    /// <summary>
    /// The comment note20.
    /// </summary>
    CommentNote20 = 0xE3FE,
    /// <summary>
    /// The comment note24.
    /// </summary>
    CommentNote24 = 0xE3FF,
    /// <summary>
    /// The comment off16.
    /// </summary>
    CommentOff16 = 0xE400,
    /// <summary>
    /// The comment off20.
    /// </summary>
    CommentOff20 = 0xE401,
    /// <summary>
    /// The comment off24.
    /// </summary>
    CommentOff24 = 0xE402,
    /// <summary>
    /// The comment off28.
    /// </summary>
    CommentOff28 = 0xE403,
    /// <summary>
    /// The comment off48.
    /// </summary>
    CommentOff48 = 0xE404,
    /// <summary>
    /// The communication person20.
    /// </summary>
    CommunicationPerson20 = 0xE405,
    /// <summary>
    /// The communication person24.
    /// </summary>
    CommunicationPerson24 = 0xE406,
    /// <summary>
    /// The component2 double tap swipe down24.
    /// </summary>
    Component2DoubleTapSwipeDown24 = 0xE407,
    /// <summary>
    /// The component2 double tap swipe up24.
    /// </summary>
    Component2DoubleTapSwipeUp24 = 0xE408,
    /// <summary>
    /// The contact card28.
    /// </summary>
    ContactCard28 = 0xE409,
    /// <summary>
    /// The contact card32.
    /// </summary>
    ContactCard32 = 0xE40A,
    /// <summary>
    /// The contact card48.
    /// </summary>
    ContactCard48 = 0xE40B,
    /// <summary>
    /// The contact card group16.
    /// </summary>
    ContactCardGroup16 = 0xE40C,
    /// <summary>
    /// The contact card group20.
    /// </summary>
    ContactCardGroup20 = 0xE40D,
    /// <summary>
    /// The contact card group28.
    /// </summary>
    ContactCardGroup28 = 0xE40E,
    /// <summary>
    /// The contact card group48.
    /// </summary>
    ContactCardGroup48 = 0xE40F,
    /// <summary>
    /// The contact card link20.
    /// </summary>
    ContactCardLink20 = 0xE410,
    /// <summary>
    /// The contact card ribbon16.
    /// </summary>
    ContactCardRibbon16 = 0xE411,
    /// <summary>
    /// The contact card ribbon20.
    /// </summary>
    ContactCardRibbon20 = 0xE412,
    /// <summary>
    /// The contact card ribbon24.
    /// </summary>
    ContactCardRibbon24 = 0xE413,
    /// <summary>
    /// The contact card ribbon28.
    /// </summary>
    ContactCardRibbon28 = 0xE414,
    /// <summary>
    /// The contact card ribbon32.
    /// </summary>
    ContactCardRibbon32 = 0xE415,
    /// <summary>
    /// The contact card ribbon48.
    /// </summary>
    ContactCardRibbon48 = 0xE416,
    /// <summary>
    /// The content settings32.
    /// </summary>
    ContentSettings32 = 0xE417,
    /// <summary>
    /// The content view20.
    /// </summary>
    ContentView20 = 0xE418,
    /// <summary>
    /// The content view32.
    /// </summary>
    ContentView32 = 0xE419,
    /// <summary>
    /// The content view gallery20.
    /// </summary>
    ContentViewGallery20 = 0xE41A,
    /// <summary>
    /// The control button20.
    /// </summary>
    ControlButton20 = 0xE41B,
    /// <summary>
    /// The control button24.
    /// </summary>
    ControlButton24 = 0xE41C,
    /// <summary>
    /// The convert range20.
    /// </summary>
    ConvertRange20 = 0xE41D,
    /// <summary>
    /// The convert range24.
    /// </summary>
    ConvertRange24 = 0xE41E,
    /// <summary>
    /// The copy add20.
    /// </summary>
    CopyAdd20 = 0xE41F,
    /// <summary>
    /// The copy add24.
    /// </summary>
    CopyAdd24 = 0xE420,
    /// <summary>
    /// The copy arrow right16.
    /// </summary>
    CopyArrowRight16 = 0xE421,
    /// <summary>
    /// The copy arrow right20.
    /// </summary>
    CopyArrowRight20 = 0xE422,
    /// <summary>
    /// The copy arrow right24.
    /// </summary>
    CopyArrowRight24 = 0xE423,
    /// <summary>
    /// The copy select20.
    /// </summary>
    CopySelect20 = 0xE424,
    /// <summary>
    /// The couch12.
    /// </summary>
    Couch12 = 0xE425,
    /// <summary>
    /// The couch20.
    /// </summary>
    Couch20 = 0xE426,
    /// <summary>
    /// The couch24.
    /// </summary>
    Couch24 = 0xE427,
    /// <summary>
    /// The credit card person20.
    /// </summary>
    CreditCardPerson20 = 0xE428,
    /// <summary>
    /// The credit card person24.
    /// </summary>
    CreditCardPerson24 = 0xE429,
    /// <summary>
    /// The credit card toolbox24.
    /// </summary>
    CreditCardToolbox24 = 0xE42A,
    /// <summary>
    /// The crop20.
    /// </summary>
    Crop20 = 0xE42B,
    /// <summary>
    /// The crop interim20.
    /// </summary>
    CropInterim20 = 0xE42C,
    /// <summary>
    /// The crop interim off20.
    /// </summary>
    CropInterimOff20 = 0xE42D,
    /// <summary>
    /// The cube12.
    /// </summary>
    Cube12 = 0xE42E,
    /// <summary>
    /// The cube add20.
    /// </summary>
    CubeAdd20 = 0xE42F,
    /// <summary>
    /// The cube arrow curve down20.
    /// </summary>
    CubeArrowCurveDown20 = 0xE430,
    /// <summary>
    /// The cube link20.
    /// </summary>
    CubeLink20 = 0xE431,
    /// <summary>
    /// The cube multiple20.
    /// </summary>
    CubeMultiple20 = 0xE432,
    /// <summary>
    /// The cube multiple24.
    /// </summary>
    CubeMultiple24 = 0xE433,
    /// <summary>
    /// The cube quick16.
    /// </summary>
    CubeQuick16 = 0xE434,
    /// <summary>
    /// The cube quick20.
    /// </summary>
    CubeQuick20 = 0xE435,
    /// <summary>
    /// The cube quick24.
    /// </summary>
    CubeQuick24 = 0xE436,
    /// <summary>
    /// The cube quick28.
    /// </summary>
    CubeQuick28 = 0xE437,
    /// <summary>
    /// The cube rotate20.
    /// </summary>
    CubeRotate20 = 0xE438,
    /// <summary>
    /// The cube sync20.
    /// </summary>
    CubeSync20 = 0xE439,
    /// <summary>
    /// The cube sync24.
    /// </summary>
    CubeSync24 = 0xE43A,
    /// <summary>
    /// The cube tree20.
    /// </summary>
    CubeTree20 = 0xE43B,
    /// <summary>
    /// The cube tree24.
    /// </summary>
    CubeTree24 = 0xE43C,
    /// <summary>
    /// The currency dollar euro16.
    /// </summary>
    CurrencyDollarEuro16 = 0xE43D,
    /// <summary>
    /// The currency dollar euro20.
    /// </summary>
    CurrencyDollarEuro20 = 0xE43E,
    /// <summary>
    /// The currency dollar euro24.
    /// </summary>
    CurrencyDollarEuro24 = 0xE43F,
    /// <summary>
    /// The currency dollar rupee16.
    /// </summary>
    CurrencyDollarRupee16 = 0xE440,
    /// <summary>
    /// The currency dollar rupee20.
    /// </summary>
    CurrencyDollarRupee20 = 0xE441,
    /// <summary>
    /// The currency dollar rupee24.
    /// </summary>
    CurrencyDollarRupee24 = 0xE442,
    /// <summary>
    /// The cursor20.
    /// </summary>
    Cursor20 = 0xE443,
    /// <summary>
    /// The cursor24.
    /// </summary>
    Cursor24 = 0xE444,
    /// <summary>
    /// The cursor click20.
    /// </summary>
    CursorClick20 = 0xE445,
    /// <summary>
    /// The cursor click24.
    /// </summary>
    CursorClick24 = 0xE446,
    /// <summary>
    /// The cursor hover16.
    /// </summary>
    CursorHover16 = 0xE447,
    /// <summary>
    /// The cursor hover20.
    /// </summary>
    CursorHover20 = 0xE448,
    /// <summary>
    /// The cursor hover24.
    /// </summary>
    CursorHover24 = 0xE449,
    /// <summary>
    /// The cursor hover28.
    /// </summary>
    CursorHover28 = 0xE44A,
    /// <summary>
    /// The cursor hover32.
    /// </summary>
    CursorHover32 = 0xE44B,
    /// <summary>
    /// The cursor hover48.
    /// </summary>
    CursorHover48 = 0xE44C,
    /// <summary>
    /// The cursor hover off16.
    /// </summary>
    CursorHoverOff16 = 0xE44D,
    /// <summary>
    /// The cursor hover off20.
    /// </summary>
    CursorHoverOff20 = 0xE44E,
    /// <summary>
    /// The cursor hover off24.
    /// </summary>
    CursorHoverOff24 = 0xE44F,
    /// <summary>
    /// The cursor hover off28.
    /// </summary>
    CursorHoverOff28 = 0xE450,
    /// <summary>
    /// The cursor hover off48.
    /// </summary>
    CursorHoverOff48 = 0xE451,
    /// <summary>
    /// The dark theme20.
    /// </summary>
    DarkTheme20 = 0xE452,
    /// <summary>
    /// The data area20.
    /// </summary>
    DataArea20 = 0xE453,
    /// <summary>
    /// The data bar vertical add20.
    /// </summary>
    DataBarVerticalAdd20 = 0xE454,
    /// <summary>
    /// The data bar vertical add24.
    /// </summary>
    DataBarVerticalAdd24 = 0xE455,
    /// <summary>
    /// The data funnel20.
    /// </summary>
    DataFunnel20 = 0xE456,
    /// <summary>
    /// The data histogram20.
    /// </summary>
    DataHistogram20 = 0xE457,
    /// <summary>
    /// The data line20.
    /// </summary>
    DataLine20 = 0xE458,
    /// <summary>
    /// The data scatter20.
    /// </summary>
    DataScatter20 = 0xE459,
    /// <summary>
    /// The data sunburst20.
    /// </summary>
    DataSunburst20 = 0xE45A,
    /// <summary>
    /// The data treemap20.
    /// </summary>
    DataTreemap20 = 0xE45B,
    /// <summary>
    /// The data trending16.
    /// </summary>
    DataTrending16 = 0xE45C,
    /// <summary>
    /// The data trending20.
    /// </summary>
    DataTrending20 = 0xE45D,
    /// <summary>
    /// The data trending24.
    /// </summary>
    DataTrending24 = 0xE45E,
    /// <summary>
    /// The data usage20.
    /// </summary>
    DataUsage20 = 0xE45F,
    /// <summary>
    /// The data usage edit24.
    /// </summary>
    DataUsageEdit24 = 0xE460,
    /// <summary>
    /// The data usage settings20.
    /// </summary>
    DataUsageSettings20 = 0xE461,
    /// <summary>
    /// The data usage toolbox20.
    /// </summary>
    DataUsageToolbox20 = 0xE462,
    /// <summary>
    /// The data usage toolbox24.
    /// </summary>
    DataUsageToolbox24 = 0xE463,
    /// <summary>
    /// The data waterfall20.
    /// </summary>
    DataWaterfall20 = 0xE464,
    /// <summary>
    /// The data whisker20.
    /// </summary>
    DataWhisker20 = 0xE465,
    /// <summary>
    /// The database20.
    /// </summary>
    Database20 = 0xE466,
    /// <summary>
    /// The database24.
    /// </summary>
    Database24 = 0xE467,
    /// <summary>
    /// The database arrow down20.
    /// </summary>
    DatabaseArrowDown20 = 0xE468,
    /// <summary>
    /// The database arrow right20.
    /// </summary>
    DatabaseArrowRight20 = 0xE469,
    /// <summary>
    /// The database arrow up20.
    /// </summary>
    DatabaseArrowUp20 = 0xE46A,
    /// <summary>
    /// The database lightning20.
    /// </summary>
    DatabaseLightning20 = 0xE46B,
    /// <summary>
    /// The database link20.
    /// </summary>
    DatabaseLink20 = 0xE46C,
    /// <summary>
    /// The database link24.
    /// </summary>
    DatabaseLink24 = 0xE46D,
    /// <summary>
    /// The database multiple20.
    /// </summary>
    DatabaseMultiple20 = 0xE46E,
    /// <summary>
    /// The database person20.
    /// </summary>
    DatabasePerson20 = 0xE46F,
    /// <summary>
    /// The database person24.
    /// </summary>
    DatabasePerson24 = 0xE470,
    /// <summary>
    /// The database plug connected20.
    /// </summary>
    DatabasePlugConnected20 = 0xE471,
    /// <summary>
    /// The database search20.
    /// </summary>
    DatabaseSearch20 = 0xE472,
    /// <summary>
    /// The database search24.
    /// </summary>
    DatabaseSearch24 = 0xE473,
    /// <summary>
    /// The database switch20.
    /// </summary>
    DatabaseSwitch20 = 0xE474,
    /// <summary>
    /// The database warning20.
    /// </summary>
    DatabaseWarning20 = 0xE475,
    /// <summary>
    /// The database window20.
    /// </summary>
    DatabaseWindow20 = 0xE476,
    /// <summary>
    /// The decimal arrow left20.
    /// </summary>
    DecimalArrowLeft20 = 0xE477,
    /// <summary>
    /// The decimal arrow left24.
    /// </summary>
    DecimalArrowLeft24 = 0xE478,
    /// <summary>
    /// The decimal arrow right20.
    /// </summary>
    DecimalArrowRight20 = 0xE479,
    /// <summary>
    /// The decimal arrow right24.
    /// </summary>
    DecimalArrowRight24 = 0xE47A,
    /// <summary>
    /// The delete16.
    /// </summary>
    Delete16 = 0xE47B,
    /// <summary>
    /// The delete arrow back16.
    /// </summary>
    DeleteArrowBack16 = 0xE47C,
    /// <summary>
    /// The delete arrow back20.
    /// </summary>
    DeleteArrowBack20 = 0xE47D,
    /// <summary>
    /// The delete dismiss20.
    /// </summary>
    DeleteDismiss20 = 0xE47E,
    /// <summary>
    /// The delete dismiss24.
    /// </summary>
    DeleteDismiss24 = 0xE47F,
    /// <summary>
    /// The delete dismiss28.
    /// </summary>
    DeleteDismiss28 = 0xE480,
    /// <summary>
    /// The delete lines20.
    /// </summary>
    DeleteLines20 = 0xE481,
    /// <summary>
    /// The dentist12.
    /// </summary>
    Dentist12 = 0xE482,
    /// <summary>
    /// The dentist16.
    /// </summary>
    Dentist16 = 0xE483,
    /// <summary>
    /// The dentist20.
    /// </summary>
    Dentist20 = 0xE484,
    /// <summary>
    /// The dentist28.
    /// </summary>
    Dentist28 = 0xE485,
    /// <summary>
    /// The dentist48.
    /// </summary>
    Dentist48 = 0xE486,
    /// <summary>
    /// The desktop32.
    /// </summary>
    Desktop32 = 0xE487,
    /// <summary>
    /// The desktop arrow right16.
    /// </summary>
    DesktopArrowRight16 = 0xE488,
    /// <summary>
    /// The desktop arrow right20.
    /// </summary>
    DesktopArrowRight20 = 0xE489,
    /// <summary>
    /// The desktop arrow right24.
    /// </summary>
    DesktopArrowRight24 = 0xE48A,
    /// <summary>
    /// The desktop cursor16.
    /// </summary>
    DesktopCursor16 = 0xE48B,
    /// <summary>
    /// The desktop cursor20.
    /// </summary>
    DesktopCursor20 = 0xE48C,
    /// <summary>
    /// The desktop cursor24.
    /// </summary>
    DesktopCursor24 = 0xE48D,
    /// <summary>
    /// The desktop cursor28.
    /// </summary>
    DesktopCursor28 = 0xE48E,
    /// <summary>
    /// The desktop edit16.
    /// </summary>
    DesktopEdit16 = 0xE48F,
    /// <summary>
    /// The desktop edit20.
    /// </summary>
    DesktopEdit20 = 0xE490,
    /// <summary>
    /// The desktop edit24.
    /// </summary>
    DesktopEdit24 = 0xE491,
    /// <summary>
    /// The desktop flow20.
    /// </summary>
    DesktopFlow20 = 0xE492,
    /// <summary>
    /// The desktop flow24.
    /// </summary>
    DesktopFlow24 = 0xE493,
    /// <summary>
    /// The desktop keyboard16.
    /// </summary>
    DesktopKeyboard16 = 0xE494,
    /// <summary>
    /// The desktop keyboard20.
    /// </summary>
    DesktopKeyboard20 = 0xE495,
    /// <summary>
    /// The desktop keyboard24.
    /// </summary>
    DesktopKeyboard24 = 0xE496,
    /// <summary>
    /// The desktop keyboard28.
    /// </summary>
    DesktopKeyboard28 = 0xE497,
    /// <summary>
    /// The desktop mac16.
    /// </summary>
    DesktopMac16 = 0xE498,
    /// <summary>
    /// The desktop mac20.
    /// </summary>
    DesktopMac20 = 0xE499,
    /// <summary>
    /// The desktop mac24.
    /// </summary>
    DesktopMac24 = 0xE49A,
    /// <summary>
    /// The desktop mac32.
    /// </summary>
    DesktopMac32 = 0xE49B,
    /// <summary>
    /// The desktop pulse16.
    /// </summary>
    DesktopPulse16 = 0xE49C,
    /// <summary>
    /// The desktop pulse20.
    /// </summary>
    DesktopPulse20 = 0xE49D,
    /// <summary>
    /// The desktop pulse24.
    /// </summary>
    DesktopPulse24 = 0xE49E,
    /// <summary>
    /// The desktop pulse28.
    /// </summary>
    DesktopPulse28 = 0xE49F,
    /// <summary>
    /// The desktop pulse32.
    /// </summary>
    DesktopPulse32 = 0xE4A0,
    /// <summary>
    /// The desktop pulse48.
    /// </summary>
    DesktopPulse48 = 0xE4A1,
    /// <summary>
    /// The desktop signal20.
    /// </summary>
    DesktopSignal20 = 0xE4A2,
    /// <summary>
    /// The desktop signal24.
    /// </summary>
    DesktopSignal24 = 0xE4A3,
    /// <summary>
    /// The desktop speaker20.
    /// </summary>
    DesktopSpeaker20 = 0xE4A4,
    /// <summary>
    /// The desktop speaker24.
    /// </summary>
    DesktopSpeaker24 = 0xE4A5,
    /// <summary>
    /// The desktop speaker off20.
    /// </summary>
    DesktopSpeakerOff20 = 0xE4A6,
    /// <summary>
    /// The desktop speaker off24.
    /// </summary>
    DesktopSpeakerOff24 = 0xE4A7,
    /// <summary>
    /// The desktop sync20.
    /// </summary>
    DesktopSync20 = 0xE4A8,
    /// <summary>
    /// The desktop sync24.
    /// </summary>
    DesktopSync24 = 0xE4A9,
    /// <summary>
    /// The desktop toolbox20.
    /// </summary>
    DesktopToolbox20 = 0xE4AA,
    /// <summary>
    /// The desktop toolbox24.
    /// </summary>
    DesktopToolbox24 = 0xE4AB,
    /// <summary>
    /// The developer board20.
    /// </summary>
    DeveloperBoard20 = 0xE4AC,
    /// <summary>
    /// The developer board lightning20.
    /// </summary>
    DeveloperBoardLightning20 = 0xE4AD,
    /// <summary>
    /// The developer board lightning toolbox20.
    /// </summary>
    DeveloperBoardLightningToolbox20 = 0xE4AE,
    /// <summary>
    /// The developer board search20.
    /// </summary>
    DeveloperBoardSearch20 = 0xE4AF,
    /// <summary>
    /// The developer board search24.
    /// </summary>
    DeveloperBoardSearch24 = 0xE4B0,
    /// <summary>
    /// The device eq20.
    /// </summary>
    DeviceEq20 = 0xE4B1,
    /// <summary>
    /// The device meeting room20.
    /// </summary>
    DeviceMeetingRoom20 = 0xE4B2,
    /// <summary>
    /// The device meeting room remote20.
    /// </summary>
    DeviceMeetingRoomRemote20 = 0xE4B3,
    /// <summary>
    /// The diagram20.
    /// </summary>
    Diagram20 = 0xE4B4,
    /// <summary>
    /// The diagram24.
    /// </summary>
    Diagram24 = 0xE4B5,
    /// <summary>
    /// The dialpad28.
    /// </summary>
    Dialpad28 = 0xE4B6,
    /// <summary>
    /// The dialpad32.
    /// </summary>
    Dialpad32 = 0xE4B7,
    /// <summary>
    /// The dialpad48.
    /// </summary>
    Dialpad48 = 0xE4B8,
    /// <summary>
    /// The dialpad off20.
    /// </summary>
    DialpadOff20 = 0xE4B9,
    /// <summary>
    /// The diamond16.
    /// </summary>
    Diamond16 = 0xE4BA,
    /// <summary>
    /// The diamond20.
    /// </summary>
    Diamond20 = 0xE4BB,
    /// <summary>
    /// The diamond24.
    /// </summary>
    Diamond24 = 0xE4BC,
    /// <summary>
    /// The diamond28.
    /// </summary>
    Diamond28 = 0xE4BD,
    /// <summary>
    /// The diamond32.
    /// </summary>
    Diamond32 = 0xE4BE,
    /// <summary>
    /// The diamond48.
    /// </summary>
    Diamond48 = 0xE4BF,
    /// <summary>
    /// The directions16.
    /// </summary>
    Directions16 = 0xE4C0,
    /// <summary>
    /// The dismiss circle12.
    /// </summary>
    DismissCircle12 = 0xE4C1,
    /// <summary>
    /// The dismiss circle28.
    /// </summary>
    DismissCircle28 = 0xE4C2,
    /// <summary>
    /// The dismiss circle32.
    /// </summary>
    DismissCircle32 = 0xE4C3,
    /// <summary>
    /// The dismiss square20.
    /// </summary>
    DismissSquare20 = 0xE4C4,
    /// <summary>
    /// The dismiss square24.
    /// </summary>
    DismissSquare24 = 0xE4C5,
    /// <summary>
    /// The dismiss square multiple16.
    /// </summary>
    DismissSquareMultiple16 = 0xE4C6,
    /// <summary>
    /// The dismiss square multiple20.
    /// </summary>
    DismissSquareMultiple20 = 0xE4C7,
    /// <summary>
    /// The diversity20.
    /// </summary>
    Diversity20 = 0xE4C8,
    /// <summary>
    /// The diversity24.
    /// </summary>
    Diversity24 = 0xE4C9,
    /// <summary>
    /// The diversity28.
    /// </summary>
    Diversity28 = 0xE4CA,
    /// <summary>
    /// The diversity48.
    /// </summary>
    Diversity48 = 0xE4CB,
    /// <summary>
    /// The divider short16.
    /// </summary>
    DividerShort16 = 0xE4CC,
    /// <summary>
    /// The divider short20.
    /// </summary>
    DividerShort20 = 0xE4CD,
    /// <summary>
    /// The divider tall16.
    /// </summary>
    DividerTall16 = 0xE4CE,
    /// <summary>
    /// The divider tall20.
    /// </summary>
    DividerTall20 = 0xE4CF,
    /// <summary>
    /// The dock20.
    /// </summary>
    Dock20 = 0xE4D0,
    /// <summary>
    /// The dock row20.
    /// </summary>
    DockRow20 = 0xE4D1,
    /// <summary>
    /// The doctor12.
    /// </summary>
    Doctor12 = 0xE4D2,
    /// <summary>
    /// The doctor16.
    /// </summary>
    Doctor16 = 0xE4D3,
    /// <summary>
    /// The doctor20.
    /// </summary>
    Doctor20 = 0xE4D4,
    /// <summary>
    /// The doctor28.
    /// </summary>
    Doctor28 = 0xE4D5,
    /// <summary>
    /// The doctor48.
    /// </summary>
    Doctor48 = 0xE4D6,
    /// <summary>
    /// The document16.
    /// </summary>
    Document16 = 0xE4D7,
    /// <summary>
    /// The document32.
    /// </summary>
    Document32 = 0xE4D8,
    /// <summary>
    /// The document48.
    /// </summary>
    Document48 = 0xE4D9,
    /// <summary>
    /// The document add16.
    /// </summary>
    DocumentAdd16 = 0xE4DA,
    /// <summary>
    /// The document add20.
    /// </summary>
    DocumentAdd20 = 0xE4DB,
    /// <summary>
    /// The document add24.
    /// </summary>
    DocumentAdd24 = 0xE4DC,
    /// <summary>
    /// The document add28.
    /// </summary>
    DocumentAdd28 = 0xE4DD,
    /// <summary>
    /// The document add48.
    /// </summary>
    DocumentAdd48 = 0xE4DE,
    /// <summary>
    /// The document arrow down16.
    /// </summary>
    DocumentArrowDown16 = 0xE4DF,
    /// <summary>
    /// The document arrow down20.
    /// </summary>
    DocumentArrowDown20 = 0xE4E0,
    /// <summary>
    /// The document arrow left16.
    /// </summary>
    DocumentArrowLeft16 = 0xE4E1,
    /// <summary>
    /// The document arrow left20.
    /// </summary>
    DocumentArrowLeft20 = 0xE4E2,
    /// <summary>
    /// The document arrow left24.
    /// </summary>
    DocumentArrowLeft24 = 0xE4E3,
    /// <summary>
    /// The document arrow left28.
    /// </summary>
    DocumentArrowLeft28 = 0xE4E4,
    /// <summary>
    /// The document arrow left48.
    /// </summary>
    DocumentArrowLeft48 = 0xE4E5,
    /// <summary>
    /// The document arrow right20.
    /// </summary>
    DocumentArrowRight20 = 0xE4E6,
    /// <summary>
    /// The document arrow right24.
    /// </summary>
    DocumentArrowRight24 = 0xE4E7,
    /// <summary>
    /// The document arrow up20.
    /// </summary>
    DocumentArrowUp20 = 0xE4E8,
    /// <summary>
    /// The document bullet list clock20.
    /// </summary>
    DocumentBulletListClock20 = 0xE4E9,
    /// <summary>
    /// The document bullet list clock24.
    /// </summary>
    DocumentBulletListClock24 = 0xE4EA,
    /// <summary>
    /// The document bullet list multiple20.
    /// </summary>
    DocumentBulletListMultiple20 = 0xE4EB,
    /// <summary>
    /// The document bullet list multiple24.
    /// </summary>
    DocumentBulletListMultiple24 = 0xE4EC,
    /// <summary>
    /// The document bullet list off20.
    /// </summary>
    DocumentBulletListOff20 = 0xE4ED,
    /// <summary>
    /// The document bullet list off24.
    /// </summary>
    DocumentBulletListOff24 = 0xE4EE,
    /// <summary>
    /// The document catch up16.
    /// </summary>
    DocumentCatchUp16 = 0xE4EF,
    /// <summary>
    /// The document catch up20.
    /// </summary>
    DocumentCatchUp20 = 0xE4F0,
    /// <summary>
    /// The document checkmark20.
    /// </summary>
    DocumentCheckmark20 = 0xE4F1,
    /// <summary>
    /// The document checkmark24.
    /// </summary>
    DocumentCheckmark24 = 0xE4F2,
    /// <summary>
    /// The document chevron double20.
    /// </summary>
    DocumentChevronDouble20 = 0xE4F3,
    /// <summary>
    /// The document chevron double24.
    /// </summary>
    DocumentChevronDouble24 = 0xE4F4,
    /// <summary>
    /// The document CSS20.
    /// </summary>
    DocumentCss20 = 0xE4F5,
    /// <summary>
    /// The document CSS24.
    /// </summary>
    DocumentCss24 = 0xE4F6,
    /// <summary>
    /// The document data20.
    /// </summary>
    DocumentData20 = 0xE4F7,
    /// <summary>
    /// The document data24.
    /// </summary>
    DocumentData24 = 0xE4F8,
    /// <summary>
    /// The document dismiss16.
    /// </summary>
    DocumentDismiss16 = 0xE4F9,
    /// <summary>
    /// The document flowchart20.
    /// </summary>
    DocumentFlowchart20 = 0xE4FA,
    /// <summary>
    /// The document flowchart24.
    /// </summary>
    DocumentFlowchart24 = 0xE4FB,
    /// <summary>
    /// The document footer16.
    /// </summary>
    DocumentFooter16 = 0xE4FC,
    /// <summary>
    /// The document footer20.
    /// </summary>
    DocumentFooter20 = 0xE4FD,
    /// <summary>
    /// The document footer dismiss20.
    /// </summary>
    DocumentFooterDismiss20 = 0xE4FE,
    /// <summary>
    /// The document footer dismiss24.
    /// </summary>
    DocumentFooterDismiss24 = 0xE4FF,
    /// <summary>
    /// The document header16.
    /// </summary>
    DocumentHeader16 = 0xE500,
    /// <summary>
    /// The document header20.
    /// </summary>
    DocumentHeader20 = 0xE501,
    /// <summary>
    /// The document header arrow down16.
    /// </summary>
    DocumentHeaderArrowDown16 = 0xE502,
    /// <summary>
    /// The document header arrow down20.
    /// </summary>
    DocumentHeaderArrowDown20 = 0xE503,
    /// <summary>
    /// The document header arrow down24.
    /// </summary>
    DocumentHeaderArrowDown24 = 0xE504,
    /// <summary>
    /// The document header dismiss20.
    /// </summary>
    DocumentHeaderDismiss20 = 0xE505,
    /// <summary>
    /// The document header dismiss24.
    /// </summary>
    DocumentHeaderDismiss24 = 0xE506,
    /// <summary>
    /// The document header footer16.
    /// </summary>
    DocumentHeaderFooter16 = 0xE507,
    /// <summary>
    /// The document heart20.
    /// </summary>
    DocumentHeart20 = 0xE508,
    /// <summary>
    /// The document heart24.
    /// </summary>
    DocumentHeart24 = 0xE509,
    /// <summary>
    /// The document heart pulse20.
    /// </summary>
    DocumentHeartPulse20 = 0xE50A,
    /// <summary>
    /// The document heart pulse24.
    /// </summary>
    DocumentHeartPulse24 = 0xE50B,
    /// <summary>
    /// The document javascript20.
    /// </summary>
    DocumentJavascript20 = 0xE50C,
    /// <summary>
    /// The document javascript24.
    /// </summary>
    DocumentJavascript24 = 0xE50D,
    /// <summary>
    /// The document landscape data20.
    /// </summary>
    DocumentLandscapeData20 = 0xE50E,
    /// <summary>
    /// The document landscape data24.
    /// </summary>
    DocumentLandscapeData24 = 0xE50F,
    /// <summary>
    /// The document landscape split20.
    /// </summary>
    DocumentLandscapeSplit20 = 0xE510,
    /// <summary>
    /// The document landscape split24.
    /// </summary>
    DocumentLandscapeSplit24 = 0xE511,
    /// <summary>
    /// The document landscape split hint20.
    /// </summary>
    DocumentLandscapeSplitHint20 = 0xE512,
    /// <summary>
    /// The document link16.
    /// </summary>
    DocumentLink16 = 0xE513,
    /// <summary>
    /// The document lock16.
    /// </summary>
    DocumentLock16 = 0xE514,
    /// <summary>
    /// The document lock20.
    /// </summary>
    DocumentLock20 = 0xE515,
    /// <summary>
    /// The document lock24.
    /// </summary>
    DocumentLock24 = 0xE516,
    /// <summary>
    /// The document lock28.
    /// </summary>
    DocumentLock28 = 0xE517,
    /// <summary>
    /// The document lock32.
    /// </summary>
    DocumentLock32 = 0xE518,
    /// <summary>
    /// The document lock48.
    /// </summary>
    DocumentLock48 = 0xE519,
    /// <summary>
    /// The document mention16.
    /// </summary>
    DocumentMention16 = 0xE51A,
    /// <summary>
    /// The document mention20.
    /// </summary>
    DocumentMention20 = 0xE51B,
    /// <summary>
    /// The document mention24.
    /// </summary>
    DocumentMention24 = 0xE51C,
    /// <summary>
    /// The document mention28.
    /// </summary>
    DocumentMention28 = 0xE51D,
    /// <summary>
    /// The document mention48.
    /// </summary>
    DocumentMention48 = 0xE51E,
    /// <summary>
    /// The document multiple16.
    /// </summary>
    DocumentMultiple16 = 0xE51F,
    /// <summary>
    /// The document multiple20.
    /// </summary>
    DocumentMultiple20 = 0xE520,
    /// <summary>
    /// The document multiple24.
    /// </summary>
    DocumentMultiple24 = 0xE521,
    /// <summary>
    /// The document multiple percent20.
    /// </summary>
    DocumentMultiplePercent20 = 0xE522,
    /// <summary>
    /// The document multiple percent24.
    /// </summary>
    DocumentMultiplePercent24 = 0xE523,
    /// <summary>
    /// The document multiple prohibited20.
    /// </summary>
    DocumentMultipleProhibited20 = 0xE524,
    /// <summary>
    /// The document multiple prohibited24.
    /// </summary>
    DocumentMultipleProhibited24 = 0xE525,
    /// <summary>
    /// The document multiple sync20.
    /// </summary>
    DocumentMultipleSync20 = 0xE526,
    /// <summary>
    /// The document page break20.
    /// </summary>
    DocumentPageBreak20 = 0xE527,
    /// <summary>
    /// The document PDF32.
    /// </summary>
    DocumentPdf32 = 0xE528,
    /// <summary>
    /// The document percent20.
    /// </summary>
    DocumentPercent20 = 0xE529,
    /// <summary>
    /// The document percent24.
    /// </summary>
    DocumentPercent24 = 0xE52A,
    /// <summary>
    /// The document person20.
    /// </summary>
    DocumentPerson20 = 0xE52B,
    /// <summary>
    /// The document pill20.
    /// </summary>
    DocumentPill20 = 0xE52C,
    /// <summary>
    /// The document pill24.
    /// </summary>
    DocumentPill24 = 0xE52D,
    /// <summary>
    /// The document prohibited20.
    /// </summary>
    DocumentProhibited20 = 0xE52E,
    /// <summary>
    /// The document prohibited24.
    /// </summary>
    DocumentProhibited24 = 0xE52F,
    /// <summary>
    /// The document question mark16.
    /// </summary>
    DocumentQuestionMark16 = 0xE530,
    /// <summary>
    /// The document question mark20.
    /// </summary>
    DocumentQuestionMark20 = 0xE531,
    /// <summary>
    /// The document question mark24.
    /// </summary>
    DocumentQuestionMark24 = 0xE532,
    /// <summary>
    /// The document queue20.
    /// </summary>
    DocumentQueue20 = 0xE533,
    /// <summary>
    /// The document queue24.
    /// </summary>
    DocumentQueue24 = 0xE534,
    /// <summary>
    /// The document queue add20.
    /// </summary>
    DocumentQueueAdd20 = 0xE535,
    /// <summary>
    /// The document queue add24.
    /// </summary>
    DocumentQueueAdd24 = 0xE536,
    /// <summary>
    /// The document queue multiple20.
    /// </summary>
    DocumentQueueMultiple20 = 0xE537,
    /// <summary>
    /// The document queue multiple24.
    /// </summary>
    DocumentQueueMultiple24 = 0xE538,
    /// <summary>
    /// The document ribbon16.
    /// </summary>
    DocumentRibbon16 = 0xE539,
    /// <summary>
    /// The document ribbon20.
    /// </summary>
    DocumentRibbon20 = 0xE53A,
    /// <summary>
    /// The document ribbon24.
    /// </summary>
    DocumentRibbon24 = 0xE53B,
    /// <summary>
    /// The document ribbon28.
    /// </summary>
    DocumentRibbon28 = 0xE53C,
    /// <summary>
    /// The document ribbon32.
    /// </summary>
    DocumentRibbon32 = 0xE53D,
    /// <summary>
    /// The document ribbon48.
    /// </summary>
    DocumentRibbon48 = 0xE53E,
    /// <summary>
    /// The document save20.
    /// </summary>
    DocumentSave20 = 0xE53F,
    /// <summary>
    /// The document save24.
    /// </summary>
    DocumentSave24 = 0xE540,
    /// <summary>
    /// The document search16.
    /// </summary>
    DocumentSearch16 = 0xE541,
    /// <summary>
    /// The document settings20.
    /// </summary>
    DocumentSettings20 = 0xE542,
    /// <summary>
    /// The document split hint16.
    /// </summary>
    DocumentSplitHint16 = 0xE543,
    /// <summary>
    /// The document split hint20.
    /// </summary>
    DocumentSplitHint20 = 0xE544,
    /// <summary>
    /// The document split hint off16.
    /// </summary>
    DocumentSplitHintOff16 = 0xE545,
    /// <summary>
    /// The document split hint off20.
    /// </summary>
    DocumentSplitHintOff20 = 0xE546,
    /// <summary>
    /// The document sync16.
    /// </summary>
    DocumentSync16 = 0xE547,
    /// <summary>
    /// The document sync20.
    /// </summary>
    DocumentSync20 = 0xE548,
    /// <summary>
    /// The document sync24.
    /// </summary>
    DocumentSync24 = 0xE549,
    /// <summary>
    /// The document table16.
    /// </summary>
    DocumentTable16 = 0xE54A,
    /// <summary>
    /// The document table20.
    /// </summary>
    DocumentTable20 = 0xE54B,
    /// <summary>
    /// The document table24.
    /// </summary>
    DocumentTable24 = 0xE54C,
    /// <summary>
    /// The document table arrow right20.
    /// </summary>
    DocumentTableArrowRight20 = 0xE54D,
    /// <summary>
    /// The document table arrow right24.
    /// </summary>
    DocumentTableArrowRight24 = 0xE54E,
    /// <summary>
    /// The document table checkmark20.
    /// </summary>
    DocumentTableCheckmark20 = 0xE54F,
    /// <summary>
    /// The document table checkmark24.
    /// </summary>
    DocumentTableCheckmark24 = 0xE550,
    /// <summary>
    /// The document table cube20.
    /// </summary>
    DocumentTableCube20 = 0xE551,
    /// <summary>
    /// The document table cube24.
    /// </summary>
    DocumentTableCube24 = 0xE552,
    /// <summary>
    /// The document table search20.
    /// </summary>
    DocumentTableSearch20 = 0xE553,
    /// <summary>
    /// The document table search24.
    /// </summary>
    DocumentTableSearch24 = 0xE554,
    /// <summary>
    /// The document table truck20.
    /// </summary>
    DocumentTableTruck20 = 0xE555,
    /// <summary>
    /// The document table truck24.
    /// </summary>
    DocumentTableTruck24 = 0xE556,
    /// <summary>
    /// The document text20.
    /// </summary>
    DocumentText20 = 0xE557,
    /// <summary>
    /// The document text24.
    /// </summary>
    DocumentText24 = 0xE558,
    /// <summary>
    /// The document text clock20.
    /// </summary>
    DocumentTextClock20 = 0xE559,
    /// <summary>
    /// The document text clock24.
    /// </summary>
    DocumentTextClock24 = 0xE55A,
    /// <summary>
    /// The document text extract20.
    /// </summary>
    DocumentTextExtract20 = 0xE55B,
    /// <summary>
    /// The document text extract24.
    /// </summary>
    DocumentTextExtract24 = 0xE55C,
    /// <summary>
    /// The document text link20.
    /// </summary>
    DocumentTextLink20 = 0xE55D,
    /// <summary>
    /// The document text link24.
    /// </summary>
    DocumentTextLink24 = 0xE55E,
    /// <summary>
    /// The document text toolbox20.
    /// </summary>
    DocumentTextToolbox20 = 0xE55F,
    /// <summary>
    /// The document text toolbox24.
    /// </summary>
    DocumentTextToolbox24 = 0xE560,
    /// <summary>
    /// The door16.
    /// </summary>
    Door16 = 0xE561,
    /// <summary>
    /// The door20.
    /// </summary>
    Door20 = 0xE562,
    /// <summary>
    /// The door28.
    /// </summary>
    Door28 = 0xE563,
    /// <summary>
    /// The door arrow left16.
    /// </summary>
    DoorArrowLeft16 = 0xE564,
    /// <summary>
    /// The door arrow left20.
    /// </summary>
    DoorArrowLeft20 = 0xE565,
    /// <summary>
    /// The door arrow left24.
    /// </summary>
    DoorArrowLeft24 = 0xE566,
    /// <summary>
    /// The door arrow right16.
    /// </summary>
    DoorArrowRight16 = 0xE567,
    /// <summary>
    /// The door arrow right20.
    /// </summary>
    DoorArrowRight20 = 0xE568,
    /// <summary>
    /// The door arrow right28.
    /// </summary>
    DoorArrowRight28 = 0xE569,
    /// <summary>
    /// The door tag20.
    /// </summary>
    DoorTag20 = 0xE56A,
    /// <summary>
    /// The door tag24.
    /// </summary>
    DoorTag24 = 0xE56B,
    /// <summary>
    /// The double swipe down20.
    /// </summary>
    DoubleSwipeDown20 = 0xE56C,
    /// <summary>
    /// The double swipe up20.
    /// </summary>
    DoubleSwipeUp20 = 0xE56D,
    /// <summary>
    /// The double tap swipe down20.
    /// </summary>
    DoubleTapSwipeDown20 = 0xE56E,
    /// <summary>
    /// The double tap swipe down24.
    /// </summary>
    DoubleTapSwipeDown24 = 0xE56F,
    /// <summary>
    /// The double tap swipe up20.
    /// </summary>
    DoubleTapSwipeUp20 = 0xE570,
    /// <summary>
    /// The double tap swipe up24.
    /// </summary>
    DoubleTapSwipeUp24 = 0xE571,
    /// <summary>
    /// The drag20.
    /// </summary>
    Drag20 = 0xE572,
    /// <summary>
    /// The draw image20.
    /// </summary>
    DrawImage20 = 0xE573,
    /// <summary>
    /// The draw image24.
    /// </summary>
    DrawImage24 = 0xE574,
    /// <summary>
    /// The draw shape20.
    /// </summary>
    DrawShape20 = 0xE575,
    /// <summary>
    /// The draw shape24.
    /// </summary>
    DrawShape24 = 0xE576,
    /// <summary>
    /// The draw text20.
    /// </summary>
    DrawText20 = 0xE577,
    /// <summary>
    /// The draw text24.
    /// </summary>
    DrawText24 = 0xE578,
    /// <summary>
    /// The drawer add20.
    /// </summary>
    DrawerAdd20 = 0xE579,
    /// <summary>
    /// The drawer add24.
    /// </summary>
    DrawerAdd24 = 0xE57A,
    /// <summary>
    /// The drawer arrow download20.
    /// </summary>
    DrawerArrowDownload20 = 0xE57B,
    /// <summary>
    /// The drawer arrow download24.
    /// </summary>
    DrawerArrowDownload24 = 0xE57C,
    /// <summary>
    /// The drawer dismiss20.
    /// </summary>
    DrawerDismiss20 = 0xE57D,
    /// <summary>
    /// The drawer dismiss24.
    /// </summary>
    DrawerDismiss24 = 0xE57E,
    /// <summary>
    /// The drawer play20.
    /// </summary>
    DrawerPlay20 = 0xE57F,
    /// <summary>
    /// The drawer play24.
    /// </summary>
    DrawerPlay24 = 0xE580,
    /// <summary>
    /// The drawer subtract20.
    /// </summary>
    DrawerSubtract20 = 0xE581,
    /// <summary>
    /// The drawer subtract24.
    /// </summary>
    DrawerSubtract24 = 0xE582,
    /// <summary>
    /// The drink beer16.
    /// </summary>
    DrinkBeer16 = 0xE583,
    /// <summary>
    /// The drink beer20.
    /// </summary>
    DrinkBeer20 = 0xE584,
    /// <summary>
    /// The drink coffee16.
    /// </summary>
    DrinkCoffee16 = 0xE585,
    /// <summary>
    /// The drink margarita16.
    /// </summary>
    DrinkMargarita16 = 0xE586,
    /// <summary>
    /// The drink margarita20.
    /// </summary>
    DrinkMargarita20 = 0xE587,
    /// <summary>
    /// The drink to go20.
    /// </summary>
    DrinkToGo20 = 0xE588,
    /// <summary>
    /// The drink to go24.
    /// </summary>
    DrinkToGo24 = 0xE589,
    /// <summary>
    /// The drink wine16.
    /// </summary>
    DrinkWine16 = 0xE58A,
    /// <summary>
    /// The drink wine20.
    /// </summary>
    DrinkWine20 = 0xE58B,
    /// <summary>
    /// The drive train20.
    /// </summary>
    DriveTrain20 = 0xE58C,
    /// <summary>
    /// The drive train24.
    /// </summary>
    DriveTrain24 = 0xE58D,
    /// <summary>
    /// The drop12.
    /// </summary>
    Drop12 = 0xE58E,
    /// <summary>
    /// The drop16.
    /// </summary>
    Drop16 = 0xE58F,
    /// <summary>
    /// The drop20.
    /// </summary>
    Drop20 = 0xE590,
    /// <summary>
    /// The drop24.
    /// </summary>
    Drop24 = 0xE591,
    /// <summary>
    /// The drop28.
    /// </summary>
    Drop28 = 0xE592,
    /// <summary>
    /// The drop48.
    /// </summary>
    Drop48 = 0xE593,
    /// <summary>
    /// The dual screen20.
    /// </summary>
    DualScreen20 = 0xE594,
    /// <summary>
    /// The dual screen add20.
    /// </summary>
    DualScreenAdd20 = 0xE595,
    /// <summary>
    /// The dual screen arrow right20.
    /// </summary>
    DualScreenArrowRight20 = 0xE596,
    /// <summary>
    /// The dual screen arrow up20.
    /// </summary>
    DualScreenArrowUp20 = 0xE597,
    /// <summary>
    /// The dual screen arrow up24.
    /// </summary>
    DualScreenArrowUp24 = 0xE598,
    /// <summary>
    /// The dual screen clock20.
    /// </summary>
    DualScreenClock20 = 0xE599,
    /// <summary>
    /// The dual screen closed alert20.
    /// </summary>
    DualScreenClosedAlert20 = 0xE59A,
    /// <summary>
    /// The dual screen closed alert24.
    /// </summary>
    DualScreenClosedAlert24 = 0xE59B,
    /// <summary>
    /// The dual screen desktop20.
    /// </summary>
    DualScreenDesktop20 = 0xE59C,
    /// <summary>
    /// The dual screen dismiss20.
    /// </summary>
    DualScreenDismiss20 = 0xE59D,
    /// <summary>
    /// The dual screen dismiss24.
    /// </summary>
    DualScreenDismiss24 = 0xE59E,
    /// <summary>
    /// The dual screen group20.
    /// </summary>
    DualScreenGroup20 = 0xE59F,
    /// <summary>
    /// The dual screen header20.
    /// </summary>
    DualScreenHeader20 = 0xE5A0,
    /// <summary>
    /// The dual screen lock20.
    /// </summary>
    DualScreenLock20 = 0xE5A1,
    /// <summary>
    /// The dual screen mirror20.
    /// </summary>
    DualScreenMirror20 = 0xE5A2,
    /// <summary>
    /// The dual screen pagination20.
    /// </summary>
    DualScreenPagination20 = 0xE5A3,
    /// <summary>
    /// The dual screen settings20.
    /// </summary>
    DualScreenSettings20 = 0xE5A4,
    /// <summary>
    /// The dual screen span20.
    /// </summary>
    DualScreenSpan20 = 0xE5A5,
    /// <summary>
    /// The dual screen span24.
    /// </summary>
    DualScreenSpan24 = 0xE5A6,
    /// <summary>
    /// The dual screen speaker20.
    /// </summary>
    DualScreenSpeaker20 = 0xE5A7,
    /// <summary>
    /// The dual screen speaker24.
    /// </summary>
    DualScreenSpeaker24 = 0xE5A8,
    /// <summary>
    /// The dual screen status bar20.
    /// </summary>
    DualScreenStatusBar20 = 0xE5A9,
    /// <summary>
    /// The dual screen tablet20.
    /// </summary>
    DualScreenTablet20 = 0xE5AA,
    /// <summary>
    /// The dual screen update20.
    /// </summary>
    DualScreenUpdate20 = 0xE5AB,
    /// <summary>
    /// The dual screen vertical scroll20.
    /// </summary>
    DualScreenVerticalScroll20 = 0xE5AC,
    /// <summary>
    /// The dual screen vibrate20.
    /// </summary>
    DualScreenVibrate20 = 0xE5AD,
    /// <summary>
    /// The dumbbell16.
    /// </summary>
    Dumbbell16 = 0xE5AE,
    /// <summary>
    /// The dumbbell20.
    /// </summary>
    Dumbbell20 = 0xE5AF,
    /// <summary>
    /// The dumbbell24.
    /// </summary>
    Dumbbell24 = 0xE5B0,
    /// <summary>
    /// The dumbbell28.
    /// </summary>
    Dumbbell28 = 0xE5B1,
    /// <summary>
    /// The edit28.
    /// </summary>
    Edit28 = 0xE5B2,
    /// <summary>
    /// The edit32.
    /// </summary>
    Edit32 = 0xE5B3,
    /// <summary>
    /// The edit48.
    /// </summary>
    Edit48 = 0xE5B4,
    /// <summary>
    /// The edit arrow back20.
    /// </summary>
    EditArrowBack20 = 0xE5B5,
    /// <summary>
    /// The edit off16.
    /// </summary>
    EditOff16 = 0xE5B6,
    /// <summary>
    /// The edit off20.
    /// </summary>
    EditOff20 = 0xE5B7,
    /// <summary>
    /// The edit off24.
    /// </summary>
    EditOff24 = 0xE5B8,
    /// <summary>
    /// The edit off28.
    /// </summary>
    EditOff28 = 0xE5B9,
    /// <summary>
    /// The edit off32.
    /// </summary>
    EditOff32 = 0xE5BA,
    /// <summary>
    /// The edit off48.
    /// </summary>
    EditOff48 = 0xE5BB,
    /// <summary>
    /// The edit prohibited16.
    /// </summary>
    EditProhibited16 = 0xE5BC,
    /// <summary>
    /// The edit prohibited20.
    /// </summary>
    EditProhibited20 = 0xE5BD,
    /// <summary>
    /// The edit prohibited24.
    /// </summary>
    EditProhibited24 = 0xE5BE,
    /// <summary>
    /// The edit prohibited28.
    /// </summary>
    EditProhibited28 = 0xE5BF,
    /// <summary>
    /// The edit prohibited32.
    /// </summary>
    EditProhibited32 = 0xE5C0,
    /// <summary>
    /// The edit prohibited48.
    /// </summary>
    EditProhibited48 = 0xE5C1,
    /// <summary>
    /// The edit settings20.
    /// </summary>
    EditSettings20 = 0xE5C2,
    /// <summary>
    /// The edit settings24.
    /// </summary>
    EditSettings24 = 0xE5C3,
    /// <summary>
    /// The emoji28.
    /// </summary>
    Emoji28 = 0xE5C4,
    /// <summary>
    /// The emoji32.
    /// </summary>
    Emoji32 = 0xE5C5,
    /// <summary>
    /// The emoji48.
    /// </summary>
    Emoji48 = 0xE5C6,
    /// <summary>
    /// The emoji add16.
    /// </summary>
    EmojiAdd16 = 0xE5C7,
    /// <summary>
    /// The emoji add20.
    /// </summary>
    EmojiAdd20 = 0xE5C8,
    /// <summary>
    /// The emoji edit16.
    /// </summary>
    EmojiEdit16 = 0xE5C9,
    /// <summary>
    /// The emoji edit20.
    /// </summary>
    EmojiEdit20 = 0xE5CA,
    /// <summary>
    /// The emoji edit24.
    /// </summary>
    EmojiEdit24 = 0xE5CB,
    /// <summary>
    /// The emoji edit28.
    /// </summary>
    EmojiEdit28 = 0xE5CC,
    /// <summary>
    /// The emoji edit48.
    /// </summary>
    EmojiEdit48 = 0xE5CD,
    /// <summary>
    /// The emoji hand20.
    /// </summary>
    EmojiHand20 = 0xE5CE,
    /// <summary>
    /// The emoji hand24.
    /// </summary>
    EmojiHand24 = 0xE5CF,
    /// <summary>
    /// The emoji hand28.
    /// </summary>
    EmojiHand28 = 0xE5D0,
    /// <summary>
    /// The emoji laugh16.
    /// </summary>
    EmojiLaugh16 = 0xE5D1,
    /// <summary>
    /// The emoji multiple20.
    /// </summary>
    EmojiMultiple20 = 0xE5D2,
    /// <summary>
    /// The emoji multiple24.
    /// </summary>
    EmojiMultiple24 = 0xE5D3,
    /// <summary>
    /// The emoji sad16.
    /// </summary>
    EmojiSad16 = 0xE5D4,
    /// <summary>
    /// The emoji sad slight20.
    /// </summary>
    EmojiSadSlight20 = 0xE5D5,
    /// <summary>
    /// The emoji sad slight24.
    /// </summary>
    EmojiSadSlight24 = 0xE5D6,
    /// <summary>
    /// The emoji smile slight20.
    /// </summary>
    EmojiSmileSlight20 = 0xE5D7,
    /// <summary>
    /// The emoji smile slight24.
    /// </summary>
    EmojiSmileSlight24 = 0xE5D8,
    /// <summary>
    /// The emoji sparkle16.
    /// </summary>
    EmojiSparkle16 = 0xE5D9,
    /// <summary>
    /// The emoji sparkle20.
    /// </summary>
    EmojiSparkle20 = 0xE5DA,
    /// <summary>
    /// The emoji sparkle24.
    /// </summary>
    EmojiSparkle24 = 0xE5DB,
    /// <summary>
    /// The emoji sparkle28.
    /// </summary>
    EmojiSparkle28 = 0xE5DC,
    /// <summary>
    /// The emoji sparkle32.
    /// </summary>
    EmojiSparkle32 = 0xE5DD,
    /// <summary>
    /// The emoji sparkle48.
    /// </summary>
    EmojiSparkle48 = 0xE5DE,
    /// <summary>
    /// The engine20.
    /// </summary>
    Engine20 = 0xE5DF,
    /// <summary>
    /// The engine24.
    /// </summary>
    Engine24 = 0xE5E0,
    /// <summary>
    /// The equal circle20.
    /// </summary>
    EqualCircle20 = 0xE5E1,
    /// <summary>
    /// The equal circle24.
    /// </summary>
    EqualCircle24 = 0xE5E2,
    /// <summary>
    /// The equal off24.
    /// </summary>
    EqualOff24 = 0xE5E3,
    /// <summary>
    /// The eraser20.
    /// </summary>
    Eraser20 = 0xE5E4,
    /// <summary>
    /// The eraser24.
    /// </summary>
    Eraser24 = 0xE5E5,
    /// <summary>
    /// The eraser medium20.
    /// </summary>
    EraserMedium20 = 0xE5E6,
    /// <summary>
    /// The eraser medium24.
    /// </summary>
    EraserMedium24 = 0xE5E7,
    /// <summary>
    /// The eraser segment20.
    /// </summary>
    EraserSegment20 = 0xE5E8,
    /// <summary>
    /// The eraser segment24.
    /// </summary>
    EraserSegment24 = 0xE5E9,
    /// <summary>
    /// The eraser small20.
    /// </summary>
    EraserSmall20 = 0xE5EA,
    /// <summary>
    /// The eraser small24.
    /// </summary>
    EraserSmall24 = 0xE5EB,
    /// <summary>
    /// The eraser tool20.
    /// </summary>
    EraserTool20 = 0xE5EC,
    /// <summary>
    /// The error circle12.
    /// </summary>
    ErrorCircle12 = 0xE5ED,
    /// <summary>
    /// The error circle settings20.
    /// </summary>
    ErrorCircleSettings20 = 0xE5EE,
    /// <summary>
    /// The extended dock20.
    /// </summary>
    ExtendedDock20 = 0xE5EF,
    /// <summary>
    /// The eye12.
    /// </summary>
    Eye12 = 0xE5F0,
    /// <summary>
    /// The eye16.
    /// </summary>
    Eye16 = 0xE5F1,
    /// <summary>
    /// The eye20.
    /// </summary>
    Eye20 = 0xE5F2,
    /// <summary>
    /// The eye24.
    /// </summary>
    Eye24 = 0xE5F3,
    /// <summary>
    /// The eye off16.
    /// </summary>
    EyeOff16 = 0xE5F4,
    /// <summary>
    /// The eye off20.
    /// </summary>
    EyeOff20 = 0xE5F5,
    /// <summary>
    /// The eye off24.
    /// </summary>
    EyeOff24 = 0xE5F6,
    /// <summary>
    /// The eye tracking16.
    /// </summary>
    EyeTracking16 = 0xE5F7,
    /// <summary>
    /// The eye tracking20.
    /// </summary>
    EyeTracking20 = 0xE5F8,
    /// <summary>
    /// The eye tracking24.
    /// </summary>
    EyeTracking24 = 0xE5F9,
    /// <summary>
    /// The eye tracking off16.
    /// </summary>
    EyeTrackingOff16 = 0xE5FA,
    /// <summary>
    /// The eye tracking off20.
    /// </summary>
    EyeTrackingOff20 = 0xE5FB,
    /// <summary>
    /// The eye tracking off24.
    /// </summary>
    EyeTrackingOff24 = 0xE5FC,
    /// <summary>
    /// The eyedropper20.
    /// </summary>
    Eyedropper20 = 0xE5FD,
    /// <summary>
    /// The eyedropper24.
    /// </summary>
    Eyedropper24 = 0xE5FE,
    /// <summary>
    /// The eyedropper off20.
    /// </summary>
    EyedropperOff20 = 0xE5FF,
    /// <summary>
    /// The eyedropper off24.
    /// </summary>
    EyedropperOff24 = 0xE600,
    /// <summary>
    /// The f stop16.
    /// </summary>
    FStop16 = 0xE601,
    /// <summary>
    /// The f stop20.
    /// </summary>
    FStop20 = 0xE602,
    /// <summary>
    /// The f stop24.
    /// </summary>
    FStop24 = 0xE603,
    /// <summary>
    /// The f stop28.
    /// </summary>
    FStop28 = 0xE604,
    /// <summary>
    /// The fast acceleration20.
    /// </summary>
    FastAcceleration20 = 0xE605,
    /// <summary>
    /// The fast forward16.
    /// </summary>
    FastForward16 = 0xE606,
    /// <summary>
    /// The fast forward28.
    /// </summary>
    FastForward28 = 0xE607,
    /// <summary>
    /// The fax20.
    /// </summary>
    Fax20 = 0xE608,
    /// <summary>
    /// The filter12.
    /// </summary>
    Filter12 = 0xE609,
    /// <summary>
    /// The filter16.
    /// </summary>
    Filter16 = 0xE60A,
    /// <summary>
    /// The filter add20.
    /// </summary>
    FilterAdd20 = 0xE60B,
    /// <summary>
    /// The filter dismiss16.
    /// </summary>
    FilterDismiss16 = 0xE60C,
    /// <summary>
    /// The filter dismiss20.
    /// </summary>
    FilterDismiss20 = 0xE60D,
    /// <summary>
    /// The filter dismiss24.
    /// </summary>
    FilterDismiss24 = 0xE60E,
    /// <summary>
    /// The filter sync20.
    /// </summary>
    FilterSync20 = 0xE60F,
    /// <summary>
    /// The filter sync24.
    /// </summary>
    FilterSync24 = 0xE610,
    /// <summary>
    /// The fingerprint20.
    /// </summary>
    Fingerprint20 = 0xE611,
    /// <summary>
    /// The fingerprint48.
    /// </summary>
    Fingerprint48 = 0xE612,
    /// <summary>
    /// The fixed width20.
    /// </summary>
    FixedWidth20 = 0xE613,
    /// <summary>
    /// The fixed width24.
    /// </summary>
    FixedWidth24 = 0xE614,
    /// <summary>
    /// The flag off16.
    /// </summary>
    FlagOff16 = 0xE615,
    /// <summary>
    /// The flag off20.
    /// </summary>
    FlagOff20 = 0xE616,
    /// <summary>
    /// The flash16.
    /// </summary>
    Flash16 = 0xE617,
    /// <summary>
    /// The flash20.
    /// </summary>
    Flash20 = 0xE618,
    /// <summary>
    /// The flash24.
    /// </summary>
    Flash24 = 0xE619,
    /// <summary>
    /// The flash28.
    /// </summary>
    Flash28 = 0xE61A,
    /// <summary>
    /// The flash add20.
    /// </summary>
    FlashAdd20 = 0xE61B,
    /// <summary>
    /// The flash auto20.
    /// </summary>
    FlashAuto20 = 0xE61C,
    /// <summary>
    /// The flash checkmark16.
    /// </summary>
    FlashCheckmark16 = 0xE61D,
    /// <summary>
    /// The flash checkmark20.
    /// </summary>
    FlashCheckmark20 = 0xE61E,
    /// <summary>
    /// The flash checkmark24.
    /// </summary>
    FlashCheckmark24 = 0xE61F,
    /// <summary>
    /// The flash checkmark28.
    /// </summary>
    FlashCheckmark28 = 0xE620,
    /// <summary>
    /// The flash flow16.
    /// </summary>
    FlashFlow16 = 0xE621,
    /// <summary>
    /// The flash flow20.
    /// </summary>
    FlashFlow20 = 0xE622,
    /// <summary>
    /// The flash flow24.
    /// </summary>
    FlashFlow24 = 0xE623,
    /// <summary>
    /// The flash off20.
    /// </summary>
    FlashOff20 = 0xE624,
    /// <summary>
    /// The flash play20.
    /// </summary>
    FlashPlay20 = 0xE625,
    /// <summary>
    /// The flash settings20.
    /// </summary>
    FlashSettings20 = 0xE626,
    /// <summary>
    /// The flash settings24.
    /// </summary>
    FlashSettings24 = 0xE627,
    /// <summary>
    /// The flashlight16.
    /// </summary>
    Flashlight16 = 0xE628,
    /// <summary>
    /// The flashlight20.
    /// </summary>
    Flashlight20 = 0xE629,
    /// <summary>
    /// The flashlight off20.
    /// </summary>
    FlashlightOff20 = 0xE62A,
    /// <summary>
    /// The flip horizontal16.
    /// </summary>
    FlipHorizontal16 = 0xE62B,
    /// <summary>
    /// The flip horizontal20.
    /// </summary>
    FlipHorizontal20 = 0xE62C,
    /// <summary>
    /// The flip horizontal24.
    /// </summary>
    FlipHorizontal24 = 0xE62D,
    /// <summary>
    /// The flip horizontal28.
    /// </summary>
    FlipHorizontal28 = 0xE62E,
    /// <summary>
    /// The flip horizontal32.
    /// </summary>
    FlipHorizontal32 = 0xE62F,
    /// <summary>
    /// The flip horizontal48.
    /// </summary>
    FlipHorizontal48 = 0xE630,
    /// <summary>
    /// The flip vertical16.
    /// </summary>
    FlipVertical16 = 0xE631,
    /// <summary>
    /// The flip vertical20.
    /// </summary>
    FlipVertical20 = 0xE632,
    /// <summary>
    /// The flip vertical24.
    /// </summary>
    FlipVertical24 = 0xE633,
    /// <summary>
    /// The flip vertical28.
    /// </summary>
    FlipVertical28 = 0xE634,
    /// <summary>
    /// The flip vertical32.
    /// </summary>
    FlipVertical32 = 0xE635,
    /// <summary>
    /// The flip vertical48.
    /// </summary>
    FlipVertical48 = 0xE636,
    /// <summary>
    /// The flow20.
    /// </summary>
    Flow20 = 0xE637,
    /// <summary>
    /// The flowchart20.
    /// </summary>
    Flowchart20 = 0xE638,
    /// <summary>
    /// The flowchart24.
    /// </summary>
    Flowchart24 = 0xE639,
    /// <summary>
    /// The flowchart circle20.
    /// </summary>
    FlowchartCircle20 = 0xE63A,
    /// <summary>
    /// The flowchart circle24.
    /// </summary>
    FlowchartCircle24 = 0xE63B,
    /// <summary>
    /// The fluent20.
    /// </summary>
    Fluent20 = 0xE63C,
    /// <summary>
    /// The fluent24.
    /// </summary>
    Fluent24 = 0xE63D,
    /// <summary>
    /// The fluent32.
    /// </summary>
    Fluent32 = 0xE63E,
    /// <summary>
    /// The fluent48.
    /// </summary>
    Fluent48 = 0xE63F,
    /// <summary>
    /// The fluid16.
    /// </summary>
    Fluid16 = 0xE640,
    /// <summary>
    /// The fluid20.
    /// </summary>
    Fluid20 = 0xE641,
    /// <summary>
    /// The fluid24.
    /// </summary>
    Fluid24 = 0xE642,
    /// <summary>
    /// The folder16.
    /// </summary>
    Folder16 = 0xE643,
    /// <summary>
    /// The folder32.
    /// </summary>
    Folder32 = 0xE644,
    /// <summary>
    /// The folder add16.
    /// </summary>
    FolderAdd16 = 0xE645,
    /// <summary>
    /// The folder arrow left16.
    /// </summary>
    FolderArrowLeft16 = 0xE646,
    /// <summary>
    /// The folder arrow left20.
    /// </summary>
    FolderArrowLeft20 = 0xE647,
    /// <summary>
    /// The folder arrow left24.
    /// </summary>
    FolderArrowLeft24 = 0xE648,
    /// <summary>
    /// The folder arrow left28.
    /// </summary>
    FolderArrowLeft28 = 0xE649,
    /// <summary>
    /// The folder arrow left32.
    /// </summary>
    FolderArrowLeft32 = 0xE64A,
    /// <summary>
    /// The folder arrow right16.
    /// </summary>
    FolderArrowRight16 = 0xE64B,
    /// <summary>
    /// The folder arrow right20.
    /// </summary>
    FolderArrowRight20 = 0xE64C,
    /// <summary>
    /// The folder arrow right24.
    /// </summary>
    FolderArrowRight24 = 0xE64D,
    /// <summary>
    /// The folder arrow right28.
    /// </summary>
    FolderArrowRight28 = 0xE64E,
    /// <summary>
    /// The folder arrow right48.
    /// </summary>
    FolderArrowRight48 = 0xE64F,
    /// <summary>
    /// The folder arrow up16.
    /// </summary>
    FolderArrowUp16 = 0xE650,
    /// <summary>
    /// The folder arrow up20.
    /// </summary>
    FolderArrowUp20 = 0xE651,
    /// <summary>
    /// The folder arrow up24.
    /// </summary>
    FolderArrowUp24 = 0xE652,
    /// <summary>
    /// The folder arrow up28.
    /// </summary>
    FolderArrowUp28 = 0xE653,
    /// <summary>
    /// The folder arrow up48.
    /// </summary>
    FolderArrowUp48 = 0xE654,
    /// <summary>
    /// The folder globe20.
    /// </summary>
    FolderGlobe20 = 0xE655,
    /// <summary>
    /// The folder mail16.
    /// </summary>
    FolderMail16 = 0xE656,
    /// <summary>
    /// The folder mail20.
    /// </summary>
    FolderMail20 = 0xE657,
    /// <summary>
    /// The folder mail24.
    /// </summary>
    FolderMail24 = 0xE658,
    /// <summary>
    /// The folder mail28.
    /// </summary>
    FolderMail28 = 0xE659,
    /// <summary>
    /// The folder person20.
    /// </summary>
    FolderPerson20 = 0xE65A,
    /// <summary>
    /// The folder prohibited16.
    /// </summary>
    FolderProhibited16 = 0xE65B,
    /// <summary>
    /// The folder prohibited20.
    /// </summary>
    FolderProhibited20 = 0xE65C,
    /// <summary>
    /// The folder prohibited24.
    /// </summary>
    FolderProhibited24 = 0xE65D,
    /// <summary>
    /// The folder prohibited28.
    /// </summary>
    FolderProhibited28 = 0xE65E,
    /// <summary>
    /// The folder prohibited48.
    /// </summary>
    FolderProhibited48 = 0xE65F,
    /// <summary>
    /// The folder swap16.
    /// </summary>
    FolderSwap16 = 0xE660,
    /// <summary>
    /// The folder swap20.
    /// </summary>
    FolderSwap20 = 0xE661,
    /// <summary>
    /// The folder swap24.
    /// </summary>
    FolderSwap24 = 0xE662,
    /// <summary>
    /// The folder sync16.
    /// </summary>
    FolderSync16 = 0xE663,
    /// <summary>
    /// The folder sync20.
    /// </summary>
    FolderSync20 = 0xE664,
    /// <summary>
    /// The folder sync24.
    /// </summary>
    FolderSync24 = 0xE665,
    /// <summary>
    /// The food16.
    /// </summary>
    Food16 = 0xE666,
    /// <summary>
    /// The food apple20.
    /// </summary>
    FoodApple20 = 0xE667,
    /// <summary>
    /// The food apple24.
    /// </summary>
    FoodApple24 = 0xE668,
    /// <summary>
    /// The food cake12.
    /// </summary>
    FoodCake12 = 0xE669,
    /// <summary>
    /// The food cake16.
    /// </summary>
    FoodCake16 = 0xE66A,
    /// <summary>
    /// The food cake20.
    /// </summary>
    FoodCake20 = 0xE66B,
    /// <summary>
    /// The food egg16.
    /// </summary>
    FoodEgg16 = 0xE66C,
    /// <summary>
    /// The food egg20.
    /// </summary>
    FoodEgg20 = 0xE66D,
    /// <summary>
    /// The food grains20.
    /// </summary>
    FoodGrains20 = 0xE66E,
    /// <summary>
    /// The food grains24.
    /// </summary>
    FoodGrains24 = 0xE66F,
    /// <summary>
    /// The food pizza20.
    /// </summary>
    FoodPizza20 = 0xE670,
    /// <summary>
    /// The food pizza24.
    /// </summary>
    FoodPizza24 = 0xE671,
    /// <summary>
    /// The food toast16.
    /// </summary>
    FoodToast16 = 0xE672,
    /// <summary>
    /// The food toast20.
    /// </summary>
    FoodToast20 = 0xE673,
    /// <summary>
    /// The form new20.
    /// </summary>
    FormNew20 = 0xE674,
    /// <summary>
    /// The FPS12020.
    /// </summary>
    Fps12020 = 0xE675,
    /// <summary>
    /// The FPS12024.
    /// </summary>
    Fps12024 = 0xE676,
    /// <summary>
    /// The FPS24020.
    /// </summary>
    Fps24020 = 0xE677,
    /// <summary>
    /// The FPS3016.
    /// </summary>
    Fps3016 = 0xE678,
    /// <summary>
    /// The FPS3020.
    /// </summary>
    Fps3020 = 0xE679,
    /// <summary>
    /// The FPS3024.
    /// </summary>
    Fps3024 = 0xE67A,
    /// <summary>
    /// The FPS3028.
    /// </summary>
    Fps3028 = 0xE67B,
    /// <summary>
    /// The FPS3048.
    /// </summary>
    Fps3048 = 0xE67C,
    /// <summary>
    /// The FPS6016.
    /// </summary>
    Fps6016 = 0xE67D,
    /// <summary>
    /// The FPS6020.
    /// </summary>
    Fps6020 = 0xE67E,
    /// <summary>
    /// The FPS6024.
    /// </summary>
    Fps6024 = 0xE67F,
    /// <summary>
    /// The FPS6028.
    /// </summary>
    Fps6028 = 0xE680,
    /// <summary>
    /// The FPS6048.
    /// </summary>
    Fps6048 = 0xE681,
    /// <summary>
    /// The FPS96020.
    /// </summary>
    Fps96020 = 0xE682,
    /// <summary>
    /// The full screen maximize16.
    /// </summary>
    FullScreenMaximize16 = 0xE683,
    /// <summary>
    /// The full screen maximize20.
    /// </summary>
    FullScreenMaximize20 = 0xE684,
    /// <summary>
    /// The full screen maximize24.
    /// </summary>
    FullScreenMaximize24 = 0xE685,
    /// <summary>
    /// The full screen minimize16.
    /// </summary>
    FullScreenMinimize16 = 0xE686,
    /// <summary>
    /// The full screen minimize20.
    /// </summary>
    FullScreenMinimize20 = 0xE687,
    /// <summary>
    /// The full screen minimize24.
    /// </summary>
    FullScreenMinimize24 = 0xE688,
    /// <summary>
    /// The games16.
    /// </summary>
    Games16 = 0xE689,
    /// <summary>
    /// The games20.
    /// </summary>
    Games20 = 0xE68A,
    /// <summary>
    /// The games28.
    /// </summary>
    Games28 = 0xE68B,
    /// <summary>
    /// The games32.
    /// </summary>
    Games32 = 0xE68C,
    /// <summary>
    /// The games48.
    /// </summary>
    Games48 = 0xE68D,
    /// <summary>
    /// The gantt chart20.
    /// </summary>
    GanttChart20 = 0xE68E,
    /// <summary>
    /// The gantt chart24.
    /// </summary>
    GanttChart24 = 0xE68F,
    /// <summary>
    /// The gas20.
    /// </summary>
    Gas20 = 0xE690,
    /// <summary>
    /// The gas24.
    /// </summary>
    Gas24 = 0xE691,
    /// <summary>
    /// The gas pump20.
    /// </summary>
    GasPump20 = 0xE692,
    /// <summary>
    /// The gas pump24.
    /// </summary>
    GasPump24 = 0xE693,
    /// <summary>
    /// The gather20.
    /// </summary>
    Gather20 = 0xE694,
    /// <summary>
    /// The gauge add20.
    /// </summary>
    GaugeAdd20 = 0xE695,
    /// <summary>
    /// The gavel20.
    /// </summary>
    Gavel20 = 0xE696,
    /// <summary>
    /// The gavel24.
    /// </summary>
    Gavel24 = 0xE697,
    /// <summary>
    /// The gavel32.
    /// </summary>
    Gavel32 = 0xE698,
    /// <summary>
    /// The gesture20.
    /// </summary>
    Gesture20 = 0xE699,
    /// <summary>
    /// The gif16.
    /// </summary>
    Gif16 = 0xE69A,
    /// <summary>
    /// The gift16.
    /// </summary>
    Gift16 = 0xE69B,
    /// <summary>
    /// The gift card24.
    /// </summary>
    GiftCard24 = 0xE69C,
    /// <summary>
    /// The gift card add24.
    /// </summary>
    GiftCardAdd24 = 0xE69D,
    /// <summary>
    /// The gift card arrow right20.
    /// </summary>
    GiftCardArrowRight20 = 0xE69E,
    /// <summary>
    /// The gift card arrow right24.
    /// </summary>
    GiftCardArrowRight24 = 0xE69F,
    /// <summary>
    /// The gift card money20.
    /// </summary>
    GiftCardMoney20 = 0xE6A0,
    /// <summary>
    /// The gift card money24.
    /// </summary>
    GiftCardMoney24 = 0xE6A1,
    /// <summary>
    /// The gift card multiple20.
    /// </summary>
    GiftCardMultiple20 = 0xE6A2,
    /// <summary>
    /// The gift card multiple24.
    /// </summary>
    GiftCardMultiple24 = 0xE6A3,
    /// <summary>
    /// The glance20.
    /// </summary>
    Glance20 = 0xE6A4,
    /// <summary>
    /// The glance default12.
    /// </summary>
    GlanceDefault12 = 0xE6A5,
    /// <summary>
    /// The glance horizontal12.
    /// </summary>
    GlanceHorizontal12 = 0xE6A6,
    /// <summary>
    /// The glance horizontal20.
    /// </summary>
    GlanceHorizontal20 = 0xE6A7,
    /// <summary>
    /// The glance horizontal24.
    /// </summary>
    GlanceHorizontal24 = 0xE6A8,
    /// <summary>
    /// The glasses16.
    /// </summary>
    Glasses16 = 0xE6A9,
    /// <summary>
    /// The glasses20.
    /// </summary>
    Glasses20 = 0xE6AA,
    /// <summary>
    /// The glasses28.
    /// </summary>
    Glasses28 = 0xE6AB,
    /// <summary>
    /// The glasses48.
    /// </summary>
    Glasses48 = 0xE6AC,
    /// <summary>
    /// The glasses off16.
    /// </summary>
    GlassesOff16 = 0xE6AD,
    /// <summary>
    /// The glasses off20.
    /// </summary>
    GlassesOff20 = 0xE6AE,
    /// <summary>
    /// The glasses off28.
    /// </summary>
    GlassesOff28 = 0xE6AF,
    /// <summary>
    /// The glasses off48.
    /// </summary>
    GlassesOff48 = 0xE6B0,
    /// <summary>
    /// The globe16.
    /// </summary>
    Globe16 = 0xE6B1,
    /// <summary>
    /// The globe32.
    /// </summary>
    Globe32 = 0xE6B2,
    /// <summary>
    /// The globe add20.
    /// </summary>
    GlobeAdd20 = 0xE6B3,
    /// <summary>
    /// The globe clock16.
    /// </summary>
    GlobeClock16 = 0xE6B4,
    /// <summary>
    /// The globe clock20.
    /// </summary>
    GlobeClock20 = 0xE6B5,
    /// <summary>
    /// The globe desktop20.
    /// </summary>
    GlobeDesktop20 = 0xE6B6,
    /// <summary>
    /// The globe person20.
    /// </summary>
    GlobePerson20 = 0xE6B7,
    /// <summary>
    /// The globe person24.
    /// </summary>
    GlobePerson24 = 0xE6B8,
    /// <summary>
    /// The globe prohibited20.
    /// </summary>
    GlobeProhibited20 = 0xE6B9,
    /// <summary>
    /// The globe search20.
    /// </summary>
    GlobeSearch20 = 0xE6BA,
    /// <summary>
    /// The globe shield20.
    /// </summary>
    GlobeShield20 = 0xE6BB,
    /// <summary>
    /// The globe shield24.
    /// </summary>
    GlobeShield24 = 0xE6BC,
    /// <summary>
    /// The globe star20.
    /// </summary>
    GlobeStar20 = 0xE6BD,
    /// <summary>
    /// The globe surface20.
    /// </summary>
    GlobeSurface20 = 0xE6BE,
    /// <summary>
    /// The globe surface24.
    /// </summary>
    GlobeSurface24 = 0xE6BF,
    /// <summary>
    /// The globe video28.
    /// </summary>
    GlobeVideo28 = 0xE6C0,
    /// <summary>
    /// The globe video32.
    /// </summary>
    GlobeVideo32 = 0xE6C1,
    /// <summary>
    /// The globe video48.
    /// </summary>
    GlobeVideo48 = 0xE6C2,
    /// <summary>
    /// The grid16.
    /// </summary>
    Grid16 = 0xE6C3,
    /// <summary>
    /// The grid dots20.
    /// </summary>
    GridDots20 = 0xE6C4,
    /// <summary>
    /// The grid dots24.
    /// </summary>
    GridDots24 = 0xE6C5,
    /// <summary>
    /// The grid dots28.
    /// </summary>
    GridDots28 = 0xE6C6,
    /// <summary>
    /// The grid kanban20.
    /// </summary>
    GridKanban20 = 0xE6C7,
    /// <summary>
    /// The group dismiss20.
    /// </summary>
    GroupDismiss20 = 0xE6C8,
    /// <summary>
    /// The group dismiss24.
    /// </summary>
    GroupDismiss24 = 0xE6C9,
    /// <summary>
    /// The group list20.
    /// </summary>
    GroupList20 = 0xE6CA,
    /// <summary>
    /// The group return20.
    /// </summary>
    GroupReturn20 = 0xE6CB,
    /// <summary>
    /// The group return24.
    /// </summary>
    GroupReturn24 = 0xE6CC,
    /// <summary>
    /// The guardian20.
    /// </summary>
    Guardian20 = 0xE6CD,
    /// <summary>
    /// The guardian24.
    /// </summary>
    Guardian24 = 0xE6CE,
    /// <summary>
    /// The guardian28.
    /// </summary>
    Guardian28 = 0xE6CF,
    /// <summary>
    /// The guardian48.
    /// </summary>
    Guardian48 = 0xE6D0,
    /// <summary>
    /// The guest add20.
    /// </summary>
    GuestAdd20 = 0xE6D1,
    /// <summary>
    /// The guitar16.
    /// </summary>
    Guitar16 = 0xE6D2,
    /// <summary>
    /// The guitar20.
    /// </summary>
    Guitar20 = 0xE6D3,
    /// <summary>
    /// The guitar24.
    /// </summary>
    Guitar24 = 0xE6D4,
    /// <summary>
    /// The guitar28.
    /// </summary>
    Guitar28 = 0xE6D5,
    /// <summary>
    /// The hand draw16.
    /// </summary>
    HandDraw16 = 0xE6D6,
    /// <summary>
    /// The hand draw20.
    /// </summary>
    HandDraw20 = 0xE6D7,
    /// <summary>
    /// The hand draw24.
    /// </summary>
    HandDraw24 = 0xE6D8,
    /// <summary>
    /// The hand draw28.
    /// </summary>
    HandDraw28 = 0xE6D9,
    /// <summary>
    /// The hand left16.
    /// </summary>
    HandLeft16 = 0xE6DA,
    /// <summary>
    /// The hand left20.
    /// </summary>
    HandLeft20 = 0xE6DB,
    /// <summary>
    /// The hand left24.
    /// </summary>
    HandLeft24 = 0xE6DC,
    /// <summary>
    /// The hand left28.
    /// </summary>
    HandLeft28 = 0xE6DD,
    /// <summary>
    /// The hand right16.
    /// </summary>
    HandRight16 = 0xE6DE,
    /// <summary>
    /// The hand right20.
    /// </summary>
    HandRight20 = 0xE6DF,
    /// <summary>
    /// The hand right24.
    /// </summary>
    HandRight24 = 0xE6E0,
    /// <summary>
    /// The hand right28.
    /// </summary>
    HandRight28 = 0xE6E1,
    /// <summary>
    /// The hand right off20.
    /// </summary>
    HandRightOff20 = 0xE6E2,
    /// <summary>
    /// The hard drive20.
    /// </summary>
    HardDrive20 = 0xE6E3,
    /// <summary>
    /// The hat graduation12.
    /// </summary>
    HatGraduation12 = 0xE6E4,
    /// <summary>
    /// The hat graduation16.
    /// </summary>
    HatGraduation16 = 0xE6E5,
    /// <summary>
    /// The hat graduation20.
    /// </summary>
    HatGraduation20 = 0xE6E6,
    /// <summary>
    /// The hat graduation24.
    /// </summary>
    HatGraduation24 = 0xE6E7,
    /// <summary>
    /// The HD16.
    /// </summary>
    Hd16 = 0xE6E8,
    /// <summary>
    /// The HD20.
    /// </summary>
    Hd20 = 0xE6E9,
    /// <summary>
    /// The HD24.
    /// </summary>
    Hd24 = 0xE6EA,
    /// <summary>
    /// The HDR20.
    /// </summary>
    Hdr20 = 0xE6EB,
    /// <summary>
    /// The HDR off20.
    /// </summary>
    HdrOff20 = 0xE6EC,
    /// <summary>
    /// The HDR off24.
    /// </summary>
    HdrOff24 = 0xE6ED,
    /// <summary>
    /// The headphones20.
    /// </summary>
    Headphones20 = 0xE6EE,
    /// <summary>
    /// The headphones32.
    /// </summary>
    Headphones32 = 0xE6EF,
    /// <summary>
    /// The headphones48.
    /// </summary>
    Headphones48 = 0xE6F0,
    /// <summary>
    /// The headphones sound wave20.
    /// </summary>
    HeadphonesSoundWave20 = 0xE6F1,
    /// <summary>
    /// The headphones sound wave24.
    /// </summary>
    HeadphonesSoundWave24 = 0xE6F2,
    /// <summary>
    /// The headphones sound wave28.
    /// </summary>
    HeadphonesSoundWave28 = 0xE6F3,
    /// <summary>
    /// The headphones sound wave32.
    /// </summary>
    HeadphonesSoundWave32 = 0xE6F4,
    /// <summary>
    /// The headphones sound wave48.
    /// </summary>
    HeadphonesSoundWave48 = 0xE6F5,
    /// <summary>
    /// The headset16.
    /// </summary>
    Headset16 = 0xE6F6,
    /// <summary>
    /// The headset20.
    /// </summary>
    Headset20 = 0xE6F7,
    /// <summary>
    /// The headset32.
    /// </summary>
    Headset32 = 0xE6F8,
    /// <summary>
    /// The headset48.
    /// </summary>
    Headset48 = 0xE6F9,
    /// <summary>
    /// The heart12.
    /// </summary>
    Heart12 = 0xE6FA,
    /// <summary>
    /// The heart32.
    /// </summary>
    Heart32 = 0xE6FB,
    /// <summary>
    /// The heart48.
    /// </summary>
    Heart48 = 0xE6FC,
    /// <summary>
    /// The heart broken20.
    /// </summary>
    HeartBroken20 = 0xE6FD,
    /// <summary>
    /// The heart circle16.
    /// </summary>
    HeartCircle16 = 0xE6FE,
    /// <summary>
    /// The heart circle20.
    /// </summary>
    HeartCircle20 = 0xE6FF,
    /// <summary>
    /// The heart circle24.
    /// </summary>
    HeartCircle24 = 0xE700,
    /// <summary>
    /// The heart pulse20.
    /// </summary>
    HeartPulse20 = 0xE701,
    /// <summary>
    /// The heart pulse24.
    /// </summary>
    HeartPulse24 = 0xE702,
    /// <summary>
    /// The heart pulse32.
    /// </summary>
    HeartPulse32 = 0xE703,
    /// <summary>
    /// The highlight link20.
    /// </summary>
    HighlightLink20 = 0xE704,
    /// <summary>
    /// The history16.
    /// </summary>
    History16 = 0xE705,
    /// <summary>
    /// The history28.
    /// </summary>
    History28 = 0xE706,
    /// <summary>
    /// The history32.
    /// </summary>
    History32 = 0xE707,
    /// <summary>
    /// The history48.
    /// </summary>
    History48 = 0xE708,
    /// <summary>
    /// The history dismiss20.
    /// </summary>
    HistoryDismiss20 = 0xE709,
    /// <summary>
    /// The history dismiss24.
    /// </summary>
    HistoryDismiss24 = 0xE70A,
    /// <summary>
    /// The history dismiss28.
    /// </summary>
    HistoryDismiss28 = 0xE70B,
    /// <summary>
    /// The history dismiss32.
    /// </summary>
    HistoryDismiss32 = 0xE70C,
    /// <summary>
    /// The history dismiss48.
    /// </summary>
    HistoryDismiss48 = 0xE70D,
    /// <summary>
    /// The home12.
    /// </summary>
    Home12 = 0xE70E,
    /// <summary>
    /// The home16.
    /// </summary>
    Home16 = 0xE70F,
    /// <summary>
    /// The home32.
    /// </summary>
    Home32 = 0xE710,
    /// <summary>
    /// The home48.
    /// </summary>
    Home48 = 0xE711,
    /// <summary>
    /// The home add20.
    /// </summary>
    HomeAdd20 = 0xE712,
    /// <summary>
    /// The home checkmark16.
    /// </summary>
    HomeCheckmark16 = 0xE713,
    /// <summary>
    /// The home checkmark20.
    /// </summary>
    HomeCheckmark20 = 0xE714,
    /// <summary>
    /// The home database20.
    /// </summary>
    HomeDatabase20 = 0xE715,
    /// <summary>
    /// The home more20.
    /// </summary>
    HomeMore20 = 0xE716,
    /// <summary>
    /// The home person20.
    /// </summary>
    HomePerson20 = 0xE717,
    /// <summary>
    /// The home person24.
    /// </summary>
    HomePerson24 = 0xE718,
    /// <summary>
    /// The image32.
    /// </summary>
    Image32 = 0xE719,
    /// <summary>
    /// The image add20.
    /// </summary>
    ImageAdd20 = 0xE71A,
    /// <summary>
    /// The image alt text16.
    /// </summary>
    ImageAltText16 = 0xE71B,
    /// <summary>
    /// The image arrow back20.
    /// </summary>
    ImageArrowBack20 = 0xE71C,
    /// <summary>
    /// The image arrow back24.
    /// </summary>
    ImageArrowBack24 = 0xE71D,
    /// <summary>
    /// The image arrow counterclockwise20.
    /// </summary>
    ImageArrowCounterclockwise20 = 0xE71E,
    /// <summary>
    /// The image arrow counterclockwise24.
    /// </summary>
    ImageArrowCounterclockwise24 = 0xE71F,
    /// <summary>
    /// The image arrow forward20.
    /// </summary>
    ImageArrowForward20 = 0xE720,
    /// <summary>
    /// The image arrow forward24.
    /// </summary>
    ImageArrowForward24 = 0xE721,
    /// <summary>
    /// The image globe20.
    /// </summary>
    ImageGlobe20 = 0xE722,
    /// <summary>
    /// The image globe24.
    /// </summary>
    ImageGlobe24 = 0xE723,
    /// <summary>
    /// The image multiple16.
    /// </summary>
    ImageMultiple16 = 0xE724,
    /// <summary>
    /// The image multiple20.
    /// </summary>
    ImageMultiple20 = 0xE725,
    /// <summary>
    /// The image multiple24.
    /// </summary>
    ImageMultiple24 = 0xE726,
    /// <summary>
    /// The image multiple28.
    /// </summary>
    ImageMultiple28 = 0xE727,
    /// <summary>
    /// The image multiple32.
    /// </summary>
    ImageMultiple32 = 0xE728,
    /// <summary>
    /// The image multiple48.
    /// </summary>
    ImageMultiple48 = 0xE729,
    /// <summary>
    /// The image multiple off16.
    /// </summary>
    ImageMultipleOff16 = 0xE72A,
    /// <summary>
    /// The image multiple off20.
    /// </summary>
    ImageMultipleOff20 = 0xE72B,
    /// <summary>
    /// The image off20.
    /// </summary>
    ImageOff20 = 0xE72C,
    /// <summary>
    /// The image prohibited20.
    /// </summary>
    ImageProhibited20 = 0xE72D,
    /// <summary>
    /// The image prohibited24.
    /// </summary>
    ImageProhibited24 = 0xE72E,
    /// <summary>
    /// The image reflection20.
    /// </summary>
    ImageReflection20 = 0xE72F,
    /// <summary>
    /// The image reflection24.
    /// </summary>
    ImageReflection24 = 0xE730,
    /// <summary>
    /// The image shadow20.
    /// </summary>
    ImageShadow20 = 0xE731,
    /// <summary>
    /// The image shadow24.
    /// </summary>
    ImageShadow24 = 0xE732,
    /// <summary>
    /// The immersive reader16.
    /// </summary>
    ImmersiveReader16 = 0xE733,
    /// <summary>
    /// The immersive reader28.
    /// </summary>
    ImmersiveReader28 = 0xE734,
    /// <summary>
    /// The incognito20.
    /// </summary>
    Incognito20 = 0xE735,
    /// <summary>
    /// The info12.
    /// </summary>
    Info12 = 0xE736,
    /// <summary>
    /// The information shield20.
    /// </summary>
    InfoShield20 = 0xE737,
    /// <summary>
    /// The ink stroke20.
    /// </summary>
    InkStroke20 = 0xE738,
    /// <summary>
    /// The ink stroke24.
    /// </summary>
    InkStroke24 = 0xE739,
    /// <summary>
    /// The inking tool32.
    /// </summary>
    InkingTool32 = 0xE73A,
    /// <summary>
    /// The ios arrow LTR24.
    /// </summary>
    IosArrowLtr24 = 0xE73B,
    /// <summary>
    /// The ios arrow RTL24.
    /// </summary>
    IosArrowRtl24 = 0xE73C,
    /// <summary>
    /// The iot20.
    /// </summary>
    Iot20 = 0xE73D,
    /// <summary>
    /// The iot24.
    /// </summary>
    Iot24 = 0xE73E,
    /// <summary>
    /// The joystick20.
    /// </summary>
    Joystick20 = 0xE73F,
    /// <summary>
    /// The key16.
    /// </summary>
    Key16 = 0xE740,
    /// <summary>
    /// The key32.
    /// </summary>
    Key32 = 0xE741,
    /// <summary>
    /// The key command16.
    /// </summary>
    KeyCommand16 = 0xE742,
    /// <summary>
    /// The key command20.
    /// </summary>
    KeyCommand20 = 0xE743,
    /// <summary>
    /// The key command24.
    /// </summary>
    KeyCommand24 = 0xE744,
    /// <summary>
    /// The key multiple20.
    /// </summary>
    KeyMultiple20 = 0xE745,
    /// <summary>
    /// The key reset20.
    /// </summary>
    KeyReset20 = 0xE746,
    /// <summary>
    /// The key reset24.
    /// </summary>
    KeyReset24 = 0xE747,
    /// <summary>
    /// The keyboard12320.
    /// </summary>
    Keyboard12320 = 0xE748,
    /// <summary>
    /// The keyboard12324.
    /// </summary>
    Keyboard12324 = 0xE749,
    /// <summary>
    /// The keyboard16.
    /// </summary>
    Keyboard16 = 0xE74A,
    /// <summary>
    /// The keyboard dock20.
    /// </summary>
    KeyboardDock20 = 0xE74B,
    /// <summary>
    /// The keyboard layout float20.
    /// </summary>
    KeyboardLayoutFloat20 = 0xE74C,
    /// <summary>
    /// The keyboard layout one handed left20.
    /// </summary>
    KeyboardLayoutOneHandedLeft20 = 0xE74D,
    /// <summary>
    /// The keyboard layout resize20.
    /// </summary>
    KeyboardLayoutResize20 = 0xE74E,
    /// <summary>
    /// The keyboard layout split20.
    /// </summary>
    KeyboardLayoutSplit20 = 0xE74F,
    /// <summary>
    /// The keyboard shift16.
    /// </summary>
    KeyboardShift16 = 0xE750,
    /// <summary>
    /// The keyboard shift20.
    /// </summary>
    KeyboardShift20 = 0xE751,
    /// <summary>
    /// The keyboard shift uppercase16.
    /// </summary>
    KeyboardShiftUppercase16 = 0xE752,
    /// <summary>
    /// The keyboard shift uppercase20.
    /// </summary>
    KeyboardShiftUppercase20 = 0xE753,
    /// <summary>
    /// The keyboard tab20.
    /// </summary>
    KeyboardTab20 = 0xE754,
    /// <summary>
    /// The laptop dismiss20.
    /// </summary>
    LaptopDismiss20 = 0xE755,
    /// <summary>
    /// The lasso20.
    /// </summary>
    Lasso20 = 0xE756,
    /// <summary>
    /// The lasso28.
    /// </summary>
    Lasso28 = 0xE757,
    /// <summary>
    /// The launcher settings20.
    /// </summary>
    LauncherSettings20 = 0xE758,
    /// <summary>
    /// The leaf one16.
    /// </summary>
    LeafOne16 = 0xE759,
    /// <summary>
    /// The leaf one20.
    /// </summary>
    LeafOne20 = 0xE75A,
    /// <summary>
    /// The leaf one24.
    /// </summary>
    LeafOne24 = 0xE75B,
    /// <summary>
    /// The leaf three16.
    /// </summary>
    LeafThree16 = 0xE75C,
    /// <summary>
    /// The leaf three20.
    /// </summary>
    LeafThree20 = 0xE75D,
    /// <summary>
    /// The leaf three24.
    /// </summary>
    LeafThree24 = 0xE75E,
    /// <summary>
    /// The learning app20.
    /// </summary>
    LearningApp20 = 0xE75F,
    /// <summary>
    /// The learning app24.
    /// </summary>
    LearningApp24 = 0xE760,
    /// <summary>
    /// The library16.
    /// </summary>
    Library16 = 0xE761,
    /// <summary>
    /// The library20.
    /// </summary>
    Library20 = 0xE762,
    /// <summary>
    /// The lightbulb circle20.
    /// </summary>
    LightbulbCircle20 = 0xE763,
    /// <summary>
    /// The lightbulb filament48.
    /// </summary>
    LightbulbFilament48 = 0xE764,
    /// <summary>
    /// The line20.
    /// </summary>
    Line20 = 0xE765,
    /// <summary>
    /// The line24.
    /// </summary>
    Line24 = 0xE766,
    /// <summary>
    /// The line32.
    /// </summary>
    Line32 = 0xE767,
    /// <summary>
    /// The line48.
    /// </summary>
    Line48 = 0xE768,
    /// <summary>
    /// The line dashes20.
    /// </summary>
    LineDashes20 = 0xE769,
    /// <summary>
    /// The line dashes24.
    /// </summary>
    LineDashes24 = 0xE76A,
    /// <summary>
    /// The line dashes32.
    /// </summary>
    LineDashes32 = 0xE76B,
    /// <summary>
    /// The line dashes48.
    /// </summary>
    LineDashes48 = 0xE76C,
    /// <summary>
    /// The line horizontal5 error20.
    /// </summary>
    LineHorizontal5Error20 = 0xE76D,
    /// <summary>
    /// The line style20.
    /// </summary>
    LineStyle20 = 0xE76E,
    /// <summary>
    /// The line style24.
    /// </summary>
    LineStyle24 = 0xE76F,
    /// <summary>
    /// The link12.
    /// </summary>
    Link12 = 0xE770,
    /// <summary>
    /// The link32.
    /// </summary>
    Link32 = 0xE771,
    /// <summary>
    /// The link dismiss16.
    /// </summary>
    LinkDismiss16 = 0xE772,
    /// <summary>
    /// The link dismiss20.
    /// </summary>
    LinkDismiss20 = 0xE773,
    /// <summary>
    /// The link dismiss24.
    /// </summary>
    LinkDismiss24 = 0xE774,
    /// <summary>
    /// The link square12.
    /// </summary>
    LinkSquare12 = 0xE775,
    /// <summary>
    /// The link square16.
    /// </summary>
    LinkSquare16 = 0xE776,
    /// <summary>
    /// The link square20.
    /// </summary>
    LinkSquare20 = 0xE777,
    /// <summary>
    /// The link toolbox20.
    /// </summary>
    LinkToolbox20 = 0xE778,
    /// <summary>
    /// The list16.
    /// </summary>
    List16 = 0xE779,
    /// <summary>
    /// The live off20.
    /// </summary>
    LiveOff20 = 0xE77A,
    /// <summary>
    /// The live off24.
    /// </summary>
    LiveOff24 = 0xE77B,
    /// <summary>
    /// The location48.
    /// </summary>
    Location48 = 0xE77C,
    /// <summary>
    /// The location add16.
    /// </summary>
    LocationAdd16 = 0xE77D,
    /// <summary>
    /// The location add20.
    /// </summary>
    LocationAdd20 = 0xE77E,
    /// <summary>
    /// The location add24.
    /// </summary>
    LocationAdd24 = 0xE77F,
    /// <summary>
    /// The location add left20.
    /// </summary>
    LocationAddLeft20 = 0xE780,
    /// <summary>
    /// The location add right20.
    /// </summary>
    LocationAddRight20 = 0xE781,
    /// <summary>
    /// The location add up20.
    /// </summary>
    LocationAddUp20 = 0xE782,
    /// <summary>
    /// The location arrow left48.
    /// </summary>
    LocationArrowLeft48 = 0xE783,
    /// <summary>
    /// The location arrow right48.
    /// </summary>
    LocationArrowRight48 = 0xE784,
    /// <summary>
    /// The location arrow up48.
    /// </summary>
    LocationArrowUp48 = 0xE785,
    /// <summary>
    /// The location dismiss20.
    /// </summary>
    LocationDismiss20 = 0xE786,
    /// <summary>
    /// The location dismiss24.
    /// </summary>
    LocationDismiss24 = 0xE787,
    /// <summary>
    /// The location off16.
    /// </summary>
    LocationOff16 = 0xE788,
    /// <summary>
    /// The location off20.
    /// </summary>
    LocationOff20 = 0xE789,
    /// <summary>
    /// The location off24.
    /// </summary>
    LocationOff24 = 0xE78A,
    /// <summary>
    /// The location off28.
    /// </summary>
    LocationOff28 = 0xE78B,
    /// <summary>
    /// The location off48.
    /// </summary>
    LocationOff48 = 0xE78C,
    /// <summary>
    /// The lock closed12.
    /// </summary>
    LockClosed12 = 0xE78D,
    /// <summary>
    /// The lock closed16.
    /// </summary>
    LockClosed16 = 0xE78E,
    /// <summary>
    /// The lock closed20.
    /// </summary>
    LockClosed20 = 0xE78F,
    /// <summary>
    /// The lock closed24.
    /// </summary>
    LockClosed24 = 0xE790,
    /// <summary>
    /// The lock closed32.
    /// </summary>
    LockClosed32 = 0xE791,
    /// <summary>
    /// The lock multiple20.
    /// </summary>
    LockMultiple20 = 0xE792,
    /// <summary>
    /// The lock multiple24.
    /// </summary>
    LockMultiple24 = 0xE793,
    /// <summary>
    /// The lock open16.
    /// </summary>
    LockOpen16 = 0xE794,
    /// <summary>
    /// The lock open20.
    /// </summary>
    LockOpen20 = 0xE795,
    /// <summary>
    /// The lock open24.
    /// </summary>
    LockOpen24 = 0xE796,
    /// <summary>
    /// The lock open28.
    /// </summary>
    LockOpen28 = 0xE797,
    /// <summary>
    /// The lottery20.
    /// </summary>
    Lottery20 = 0xE798,
    /// <summary>
    /// The lottery24.
    /// </summary>
    Lottery24 = 0xE799,
    /// <summary>
    /// The luggage16.
    /// </summary>
    Luggage16 = 0xE79A,
    /// <summary>
    /// The luggage20.
    /// </summary>
    Luggage20 = 0xE79B,
    /// <summary>
    /// The luggage24.
    /// </summary>
    Luggage24 = 0xE79C,
    /// <summary>
    /// The luggage28.
    /// </summary>
    Luggage28 = 0xE79D,
    /// <summary>
    /// The luggage32.
    /// </summary>
    Luggage32 = 0xE79E,
    /// <summary>
    /// The luggage48.
    /// </summary>
    Luggage48 = 0xE79F,
    /// <summary>
    /// The mail12.
    /// </summary>
    Mail12 = 0xE7A0,
    /// <summary>
    /// The mail16.
    /// </summary>
    Mail16 = 0xE7A1,
    /// <summary>
    /// The mail alert28.
    /// </summary>
    MailAlert28 = 0xE7A2,
    /// <summary>
    /// The mail all read16.
    /// </summary>
    MailAllRead16 = 0xE7A3,
    /// <summary>
    /// The mail all read24.
    /// </summary>
    MailAllRead24 = 0xE7A4,
    /// <summary>
    /// The mail all read28.
    /// </summary>
    MailAllRead28 = 0xE7A5,
    /// <summary>
    /// The mail arrow double back16.
    /// </summary>
    MailArrowDoubleBack16 = 0xE7A6,
    /// <summary>
    /// The mail arrow double back20.
    /// </summary>
    MailArrowDoubleBack20 = 0xE7A7,
    /// <summary>
    /// The mail arrow down20.
    /// </summary>
    MailArrowDown20 = 0xE7A8,
    /// <summary>
    /// The mail arrow forward16.
    /// </summary>
    MailArrowForward16 = 0xE7A9,
    /// <summary>
    /// The mail arrow forward20.
    /// </summary>
    MailArrowForward20 = 0xE7AA,
    /// <summary>
    /// The mail arrow up16.
    /// </summary>
    MailArrowUp16 = 0xE7AB,
    /// <summary>
    /// The mail attach16.
    /// </summary>
    MailAttach16 = 0xE7AC,
    /// <summary>
    /// The mail attach20.
    /// </summary>
    MailAttach20 = 0xE7AD,
    /// <summary>
    /// The mail attach24.
    /// </summary>
    MailAttach24 = 0xE7AE,
    /// <summary>
    /// The mail attach28.
    /// </summary>
    MailAttach28 = 0xE7AF,
    /// <summary>
    /// The mail checkmark20.
    /// </summary>
    MailCheckmark20 = 0xE7B0,
    /// <summary>
    /// The mail dismiss16.
    /// </summary>
    MailDismiss16 = 0xE7B1,
    /// <summary>
    /// The mail dismiss28.
    /// </summary>
    MailDismiss28 = 0xE7B2,
    /// <summary>
    /// The mail edit20.
    /// </summary>
    MailEdit20 = 0xE7B3,
    /// <summary>
    /// The mail edit24.
    /// </summary>
    MailEdit24 = 0xE7B4,
    /// <summary>
    /// The mail error16.
    /// </summary>
    MailError16 = 0xE7B5,
    /// <summary>
    /// The mail inbox all20.
    /// </summary>
    MailInboxAll20 = 0xE7B6,
    /// <summary>
    /// The mail inbox all24.
    /// </summary>
    MailInboxAll24 = 0xE7B7,
    /// <summary>
    /// The mail inbox arrow down20.
    /// </summary>
    MailInboxArrowDown20 = 0xE7B8,
    /// <summary>
    /// The mail inbox arrow right20.
    /// </summary>
    MailInboxArrowRight20 = 0xE7B9,
    /// <summary>
    /// The mail inbox arrow right24.
    /// </summary>
    MailInboxArrowRight24 = 0xE7BA,
    /// <summary>
    /// The mail inbox arrow up20.
    /// </summary>
    MailInboxArrowUp20 = 0xE7BB,
    /// <summary>
    /// The mail inbox arrow up24.
    /// </summary>
    MailInboxArrowUp24 = 0xE7BC,
    /// <summary>
    /// The mail inbox checkmark16.
    /// </summary>
    MailInboxCheckmark16 = 0xE7BD,
    /// <summary>
    /// The mail inbox checkmark20.
    /// </summary>
    MailInboxCheckmark20 = 0xE7BE,
    /// <summary>
    /// The mail inbox checkmark24.
    /// </summary>
    MailInboxCheckmark24 = 0xE7BF,
    /// <summary>
    /// The mail inbox checkmark28.
    /// </summary>
    MailInboxCheckmark28 = 0xE7C0,
    /// <summary>
    /// The mail list16.
    /// </summary>
    MailList16 = 0xE7C1,
    /// <summary>
    /// The mail list20.
    /// </summary>
    MailList20 = 0xE7C2,
    /// <summary>
    /// The mail list24.
    /// </summary>
    MailList24 = 0xE7C3,
    /// <summary>
    /// The mail list28.
    /// </summary>
    MailList28 = 0xE7C4,
    /// <summary>
    /// The mail multiple16.
    /// </summary>
    MailMultiple16 = 0xE7C5,
    /// <summary>
    /// The mail multiple20.
    /// </summary>
    MailMultiple20 = 0xE7C6,
    /// <summary>
    /// The mail multiple24.
    /// </summary>
    MailMultiple24 = 0xE7C7,
    /// <summary>
    /// The mail multiple28.
    /// </summary>
    MailMultiple28 = 0xE7C8,
    /// <summary>
    /// The mail off20.
    /// </summary>
    MailOff20 = 0xE7C9,
    /// <summary>
    /// The mail off24.
    /// </summary>
    MailOff24 = 0xE7CA,
    /// <summary>
    /// The mail open person16.
    /// </summary>
    MailOpenPerson16 = 0xE7CB,
    /// <summary>
    /// The mail open person20.
    /// </summary>
    MailOpenPerson20 = 0xE7CC,
    /// <summary>
    /// The mail open person24.
    /// </summary>
    MailOpenPerson24 = 0xE7CD,
    /// <summary>
    /// The mail pause20.
    /// </summary>
    MailPause20 = 0xE7CE,
    /// <summary>
    /// The mail prohibited16.
    /// </summary>
    MailProhibited16 = 0xE7CF,
    /// <summary>
    /// The mail prohibited28.
    /// </summary>
    MailProhibited28 = 0xE7D0,
    /// <summary>
    /// The mail read16.
    /// </summary>
    MailRead16 = 0xE7D1,
    /// <summary>
    /// The mail read multiple16.
    /// </summary>
    MailReadMultiple16 = 0xE7D2,
    /// <summary>
    /// The mail read multiple24.
    /// </summary>
    MailReadMultiple24 = 0xE7D3,
    /// <summary>
    /// The mail read multiple28.
    /// </summary>
    MailReadMultiple28 = 0xE7D4,
    /// <summary>
    /// The mail settings20.
    /// </summary>
    MailSettings20 = 0xE7D5,
    /// <summary>
    /// The mail shield20.
    /// </summary>
    MailShield20 = 0xE7D6,
    /// <summary>
    /// The mail template16.
    /// </summary>
    MailTemplate16 = 0xE7D7,
    /// <summary>
    /// The mail warning20.
    /// </summary>
    MailWarning20 = 0xE7D8,
    /// <summary>
    /// The mail warning24.
    /// </summary>
    MailWarning24 = 0xE7D9,
    /// <summary>
    /// The map20.
    /// </summary>
    Map20 = 0xE7DA,
    /// <summary>
    /// The markdown20.
    /// </summary>
    Markdown20 = 0xE7DB,
    /// <summary>
    /// The match application layout20.
    /// </summary>
    MatchAppLayout20 = 0xE7DC,
    /// <summary>
    /// The math format linear20.
    /// </summary>
    MathFormatLinear20 = 0xE7DD,
    /// <summary>
    /// The math format linear24.
    /// </summary>
    MathFormatLinear24 = 0xE7DE,
    /// <summary>
    /// The math format professional20.
    /// </summary>
    MathFormatProfessional20 = 0xE7DF,
    /// <summary>
    /// The math format professional24.
    /// </summary>
    MathFormatProfessional24 = 0xE7E0,
    /// <summary>
    /// The math formula16.
    /// </summary>
    MathFormula16 = 0xE7E1,
    /// <summary>
    /// The math formula20.
    /// </summary>
    MathFormula20 = 0xE7E2,
    /// <summary>
    /// The math formula24.
    /// </summary>
    MathFormula24 = 0xE7E3,
    /// <summary>
    /// The math formula32.
    /// </summary>
    MathFormula32 = 0xE7E4,
    /// <summary>
    /// The math symbols16.
    /// </summary>
    MathSymbols16 = 0xE7E5,
    /// <summary>
    /// The math symbols20.
    /// </summary>
    MathSymbols20 = 0xE7E6,
    /// <summary>
    /// The math symbols24.
    /// </summary>
    MathSymbols24 = 0xE7E7,
    /// <summary>
    /// The math symbols28.
    /// </summary>
    MathSymbols28 = 0xE7E8,
    /// <summary>
    /// The math symbols32.
    /// </summary>
    MathSymbols32 = 0xE7E9,
    /// <summary>
    /// The math symbols48.
    /// </summary>
    MathSymbols48 = 0xE7EA,
    /// <summary>
    /// The maximize20.
    /// </summary>
    Maximize20 = 0xE7EB,
    /// <summary>
    /// The maximize24.
    /// </summary>
    Maximize24 = 0xE7EC,
    /// <summary>
    /// The maximize28.
    /// </summary>
    Maximize28 = 0xE7ED,
    /// <summary>
    /// The maximize48.
    /// </summary>
    Maximize48 = 0xE7EE,
    /// <summary>
    /// The meet now16.
    /// </summary>
    MeetNow16 = 0xE7EF,
    /// <summary>
    /// The megaphone loud24.
    /// </summary>
    MegaphoneLoud24 = 0xE7F0,
    /// <summary>
    /// The megaphone off16.
    /// </summary>
    MegaphoneOff16 = 0xE7F1,
    /// <summary>
    /// The megaphone off20.
    /// </summary>
    MegaphoneOff20 = 0xE7F2,
    /// <summary>
    /// The megaphone off28.
    /// </summary>
    MegaphoneOff28 = 0xE7F3,
    /// <summary>
    /// The mention arrow down20.
    /// </summary>
    MentionArrowDown20 = 0xE7F4,
    /// <summary>
    /// The mention brackets20.
    /// </summary>
    MentionBrackets20 = 0xE7F5,
    /// <summary>
    /// The merge16.
    /// </summary>
    Merge16 = 0xE7F6,
    /// <summary>
    /// The merge20.
    /// </summary>
    Merge20 = 0xE7F7,
    /// <summary>
    /// The mic16.
    /// </summary>
    Mic16 = 0xE7F8,
    /// <summary>
    /// The mic20.
    /// </summary>
    Mic20 = 0xE7F9,
    /// <summary>
    /// The mic24.
    /// </summary>
    Mic24 = 0xE7FA,
    /// <summary>
    /// The mic28.
    /// </summary>
    Mic28 = 0xE7FB,
    /// <summary>
    /// The mic32.
    /// </summary>
    Mic32 = 0xE7FC,
    /// <summary>
    /// The mic48.
    /// </summary>
    Mic48 = 0xE7FD,
    /// <summary>
    /// The mic off20.
    /// </summary>
    MicOff20 = 0xE7FE,
    /// <summary>
    /// The mic off32.
    /// </summary>
    MicOff32 = 0xE7FF,
    /// <summary>
    /// The mic off48.
    /// </summary>
    MicOff48 = 0xE800,
    /// <summary>
    /// The mic prohibited16.
    /// </summary>
    MicProhibited16 = 0xE801,
    /// <summary>
    /// The mic prohibited20.
    /// </summary>
    MicProhibited20 = 0xE802,
    /// <summary>
    /// The mic prohibited24.
    /// </summary>
    MicProhibited24 = 0xE803,
    /// <summary>
    /// The mic prohibited28.
    /// </summary>
    MicProhibited28 = 0xE804,
    /// <summary>
    /// The mic prohibited48.
    /// </summary>
    MicProhibited48 = 0xE805,
    /// <summary>
    /// The mic pulse16.
    /// </summary>
    MicPulse16 = 0xE806,
    /// <summary>
    /// The mic pulse20.
    /// </summary>
    MicPulse20 = 0xE807,
    /// <summary>
    /// The mic pulse24.
    /// </summary>
    MicPulse24 = 0xE808,
    /// <summary>
    /// The mic pulse28.
    /// </summary>
    MicPulse28 = 0xE809,
    /// <summary>
    /// The mic pulse32.
    /// </summary>
    MicPulse32 = 0xE80A,
    /// <summary>
    /// The mic pulse48.
    /// </summary>
    MicPulse48 = 0xE80B,
    /// <summary>
    /// The mic pulse off16.
    /// </summary>
    MicPulseOff16 = 0xE80C,
    /// <summary>
    /// The mic pulse off20.
    /// </summary>
    MicPulseOff20 = 0xE80D,
    /// <summary>
    /// The mic pulse off24.
    /// </summary>
    MicPulseOff24 = 0xE80E,
    /// <summary>
    /// The mic pulse off28.
    /// </summary>
    MicPulseOff28 = 0xE80F,
    /// <summary>
    /// The mic pulse off32.
    /// </summary>
    MicPulseOff32 = 0xE810,
    /// <summary>
    /// The mic pulse off48.
    /// </summary>
    MicPulseOff48 = 0xE811,
    /// <summary>
    /// The mic settings20.
    /// </summary>
    MicSettings20 = 0xE812,
    /// <summary>
    /// The mic sparkle16.
    /// </summary>
    MicSparkle16 = 0xE813,
    /// <summary>
    /// The mic sparkle20.
    /// </summary>
    MicSparkle20 = 0xE814,
    /// <summary>
    /// The mic sparkle24.
    /// </summary>
    MicSparkle24 = 0xE815,
    /// <summary>
    /// The mic sync20.
    /// </summary>
    MicSync20 = 0xE816,
    /// <summary>
    /// The mobile optimized20.
    /// </summary>
    MobileOptimized20 = 0xE817,
    /// <summary>
    /// The money calculator20.
    /// </summary>
    MoneyCalculator20 = 0xE818,
    /// <summary>
    /// The money calculator24.
    /// </summary>
    MoneyCalculator24 = 0xE819,
    /// <summary>
    /// The money dismiss20.
    /// </summary>
    MoneyDismiss20 = 0xE81A,
    /// <summary>
    /// The money dismiss24.
    /// </summary>
    MoneyDismiss24 = 0xE81B,
    /// <summary>
    /// The money hand20.
    /// </summary>
    MoneyHand20 = 0xE81C,
    /// <summary>
    /// The money hand24.
    /// </summary>
    MoneyHand24 = 0xE81D,
    /// <summary>
    /// The money off20.
    /// </summary>
    MoneyOff20 = 0xE81E,
    /// <summary>
    /// The money off24.
    /// </summary>
    MoneyOff24 = 0xE81F,
    /// <summary>
    /// The money settings20.
    /// </summary>
    MoneySettings20 = 0xE820,
    /// <summary>
    /// The more circle20.
    /// </summary>
    MoreCircle20 = 0xE821,
    /// <summary>
    /// The more circle32.
    /// </summary>
    MoreCircle32 = 0xE822,
    /// <summary>
    /// The more horizontal16.
    /// </summary>
    MoreHorizontal16 = 0xE823,
    /// <summary>
    /// The more horizontal20.
    /// </summary>
    MoreHorizontal20 = 0xE824,
    /// <summary>
    /// The more horizontal24.
    /// </summary>
    MoreHorizontal24 = 0xE825,
    /// <summary>
    /// The more horizontal28.
    /// </summary>
    MoreHorizontal28 = 0xE826,
    /// <summary>
    /// The more horizontal32.
    /// </summary>
    MoreHorizontal32 = 0xE827,
    /// <summary>
    /// The more horizontal48.
    /// </summary>
    MoreHorizontal48 = 0xE828,
    /// <summary>
    /// The more vertical16.
    /// </summary>
    MoreVertical16 = 0xE829,
    /// <summary>
    /// The more vertical32.
    /// </summary>
    MoreVertical32 = 0xE82A,
    /// <summary>
    /// The movies and TV16.
    /// </summary>
    MoviesAndTv16 = 0xE82B,
    /// <summary>
    /// The movies and TV20.
    /// </summary>
    MoviesAndTv20 = 0xE82C,
    /// <summary>
    /// The multiplier12x20.
    /// </summary>
    Multiplier12x20 = 0xE82D,
    /// <summary>
    /// The multiplier12x24.
    /// </summary>
    Multiplier12x24 = 0xE82E,
    /// <summary>
    /// The multiplier12x28.
    /// </summary>
    Multiplier12x28 = 0xE82F,
    /// <summary>
    /// The multiplier12x32.
    /// </summary>
    Multiplier12x32 = 0xE830,
    /// <summary>
    /// The multiplier12x48.
    /// </summary>
    Multiplier12x48 = 0xE831,
    /// <summary>
    /// The multiplier15x20.
    /// </summary>
    Multiplier15x20 = 0xE832,
    /// <summary>
    /// The multiplier15x24.
    /// </summary>
    Multiplier15x24 = 0xE833,
    /// <summary>
    /// The multiplier15x28.
    /// </summary>
    Multiplier15x28 = 0xE834,
    /// <summary>
    /// The multiplier15x32.
    /// </summary>
    Multiplier15x32 = 0xE835,
    /// <summary>
    /// The multiplier15x48.
    /// </summary>
    Multiplier15x48 = 0xE836,
    /// <summary>
    /// The multiplier18x20.
    /// </summary>
    Multiplier18x20 = 0xE837,
    /// <summary>
    /// The multiplier18x24.
    /// </summary>
    Multiplier18x24 = 0xE838,
    /// <summary>
    /// The multiplier18x28.
    /// </summary>
    Multiplier18x28 = 0xE839,
    /// <summary>
    /// The multiplier18x32.
    /// </summary>
    Multiplier18x32 = 0xE83A,
    /// <summary>
    /// The multiplier18x48.
    /// </summary>
    Multiplier18x48 = 0xE83B,
    /// <summary>
    /// The multiplier1x20.
    /// </summary>
    Multiplier1x20 = 0xE83C,
    /// <summary>
    /// The multiplier1x24.
    /// </summary>
    Multiplier1x24 = 0xE83D,
    /// <summary>
    /// The multiplier1x28.
    /// </summary>
    Multiplier1x28 = 0xE83E,
    /// <summary>
    /// The multiplier1x32.
    /// </summary>
    Multiplier1x32 = 0xE83F,
    /// <summary>
    /// The multiplier1x48.
    /// </summary>
    Multiplier1x48 = 0xE840,
    /// <summary>
    /// The multiplier2x20.
    /// </summary>
    Multiplier2x20 = 0xE841,
    /// <summary>
    /// The multiplier2x24.
    /// </summary>
    Multiplier2x24 = 0xE842,
    /// <summary>
    /// The multiplier2x28.
    /// </summary>
    Multiplier2x28 = 0xE843,
    /// <summary>
    /// The multiplier2x32.
    /// </summary>
    Multiplier2x32 = 0xE844,
    /// <summary>
    /// The multiplier2x48.
    /// </summary>
    Multiplier2x48 = 0xE845,
    /// <summary>
    /// The multiplier5x20.
    /// </summary>
    Multiplier5x20 = 0xE846,
    /// <summary>
    /// The multiplier5x24.
    /// </summary>
    Multiplier5x24 = 0xE847,
    /// <summary>
    /// The multiplier5x28.
    /// </summary>
    Multiplier5x28 = 0xE848,
    /// <summary>
    /// The multiplier5x32.
    /// </summary>
    Multiplier5x32 = 0xE849,
    /// <summary>
    /// The multiplier5x48.
    /// </summary>
    Multiplier5x48 = 0xE84A,
    /// <summary>
    /// The multiselect LTR16.
    /// </summary>
    MultiselectLtr16 = 0xE84B,
    /// <summary>
    /// The multiselect LTR20.
    /// </summary>
    MultiselectLtr20 = 0xE84C,
    /// <summary>
    /// The multiselect LTR24.
    /// </summary>
    MultiselectLtr24 = 0xE84D,
    /// <summary>
    /// The multiselect RTL16.
    /// </summary>
    MultiselectRtl16 = 0xE84E,
    /// <summary>
    /// The multiselect RTL20.
    /// </summary>
    MultiselectRtl20 = 0xE84F,
    /// <summary>
    /// The multiselect RTL24.
    /// </summary>
    MultiselectRtl24 = 0xE850,
    /// <summary>
    /// The music note120.
    /// </summary>
    MusicNote120 = 0xE851,
    /// <summary>
    /// The music note124.
    /// </summary>
    MusicNote124 = 0xE852,
    /// <summary>
    /// The music note216.
    /// </summary>
    MusicNote216 = 0xE853,
    /// <summary>
    /// The music note220.
    /// </summary>
    MusicNote220 = 0xE854,
    /// <summary>
    /// The music note224.
    /// </summary>
    MusicNote224 = 0xE855,
    /// <summary>
    /// The music note2 play20.
    /// </summary>
    MusicNote2Play20 = 0xE856,
    /// <summary>
    /// The music note off120.
    /// </summary>
    MusicNoteOff120 = 0xE857,
    /// <summary>
    /// The music note off124.
    /// </summary>
    MusicNoteOff124 = 0xE858,
    /// <summary>
    /// The music note off216.
    /// </summary>
    MusicNoteOff216 = 0xE859,
    /// <summary>
    /// The music note off220.
    /// </summary>
    MusicNoteOff220 = 0xE85A,
    /// <summary>
    /// The music note off224.
    /// </summary>
    MusicNoteOff224 = 0xE85B,
    /// <summary>
    /// My location12.
    /// </summary>
    MyLocation12 = 0xE85C,
    /// <summary>
    /// My location16.
    /// </summary>
    MyLocation16 = 0xE85D,
    /// <summary>
    /// My location20.
    /// </summary>
    MyLocation20 = 0xE85E,
    /// <summary>
    /// The navigation16.
    /// </summary>
    Navigation16 = 0xE85F,
    /// <summary>
    /// The navigation location target20.
    /// </summary>
    NavigationLocationTarget20 = 0xE860,
    /// <summary>
    /// The navigation play20.
    /// </summary>
    NavigationPlay20 = 0xE861,
    /// <summary>
    /// The navigation unread20.
    /// </summary>
    NavigationUnread20 = 0xE862,
    /// <summary>
    /// The navigation unread24.
    /// </summary>
    NavigationUnread24 = 0xE863,
    /// <summary>
    /// The network check20.
    /// </summary>
    NetworkCheck20 = 0xE864,
    /// <summary>
    /// Creates new 20.
    /// </summary>
    New20 = 0xE865,
    /// <summary>
    /// The news16.
    /// </summary>
    News16 = 0xE866,
    /// <summary>
    /// The next28.
    /// </summary>
    Next28 = 0xE867,
    /// <summary>
    /// The next32.
    /// </summary>
    Next32 = 0xE868,
    /// <summary>
    /// The next48.
    /// </summary>
    Next48 = 0xE869,
    /// <summary>
    /// The note28.
    /// </summary>
    Note28 = 0xE86A,
    /// <summary>
    /// The note48.
    /// </summary>
    Note48 = 0xE86B,
    /// <summary>
    /// The note add28.
    /// </summary>
    NoteAdd28 = 0xE86C,
    /// <summary>
    /// The note add48.
    /// </summary>
    NoteAdd48 = 0xE86D,
    /// <summary>
    /// The note edit20.
    /// </summary>
    NoteEdit20 = 0xE86E,
    /// <summary>
    /// The note edit24.
    /// </summary>
    NoteEdit24 = 0xE86F,
    /// <summary>
    /// The note pin20.
    /// </summary>
    NotePin20 = 0xE870,
    /// <summary>
    /// The notebook20.
    /// </summary>
    Notebook20 = 0xE871,
    /// <summary>
    /// The notebook add20.
    /// </summary>
    NotebookAdd20 = 0xE872,
    /// <summary>
    /// The notebook add24.
    /// </summary>
    NotebookAdd24 = 0xE873,
    /// <summary>
    /// The notebook arrow curve down20.
    /// </summary>
    NotebookArrowCurveDown20 = 0xE874,
    /// <summary>
    /// The notebook error20.
    /// </summary>
    NotebookError20 = 0xE875,
    /// <summary>
    /// The notebook eye20.
    /// </summary>
    NotebookEye20 = 0xE876,
    /// <summary>
    /// The notebook lightning20.
    /// </summary>
    NotebookLightning20 = 0xE877,
    /// <summary>
    /// The notebook question mark20.
    /// </summary>
    NotebookQuestionMark20 = 0xE878,
    /// <summary>
    /// The notebook section20.
    /// </summary>
    NotebookSection20 = 0xE879,
    /// <summary>
    /// The notebook section arrow right24.
    /// </summary>
    NotebookSectionArrowRight24 = 0xE87A,
    /// <summary>
    /// The notebook subsection20.
    /// </summary>
    NotebookSubsection20 = 0xE87B,
    /// <summary>
    /// The notebook subsection24.
    /// </summary>
    NotebookSubsection24 = 0xE87C,
    /// <summary>
    /// The notebook sync20.
    /// </summary>
    NotebookSync20 = 0xE87D,
    /// <summary>
    /// The notepad12.
    /// </summary>
    Notepad12 = 0xE87E,
    /// <summary>
    /// The notepad32.
    /// </summary>
    Notepad32 = 0xE87F,
    /// <summary>
    /// The notepad edit20.
    /// </summary>
    NotepadEdit20 = 0xE880,
    /// <summary>
    /// The notepad person16.
    /// </summary>
    NotepadPerson16 = 0xE881,
    /// <summary>
    /// The notepad person20.
    /// </summary>
    NotepadPerson20 = 0xE882,
    /// <summary>
    /// The notepad person24.
    /// </summary>
    NotepadPerson24 = 0xE883,
    /// <summary>
    /// The number circle116.
    /// </summary>
    NumberCircle116 = 0xE884,
    /// <summary>
    /// The number circle120.
    /// </summary>
    NumberCircle120 = 0xE885,
    /// <summary>
    /// The number circle124.
    /// </summary>
    NumberCircle124 = 0xE886,
    /// <summary>
    /// The number symbol28.
    /// </summary>
    NumberSymbol28 = 0xE887,
    /// <summary>
    /// The number symbol32.
    /// </summary>
    NumberSymbol32 = 0xE888,
    /// <summary>
    /// The number symbol48.
    /// </summary>
    NumberSymbol48 = 0xE889,
    /// <summary>
    /// The number symbol dismiss20.
    /// </summary>
    NumberSymbolDismiss20 = 0xE88A,
    /// <summary>
    /// The number symbol dismiss24.
    /// </summary>
    NumberSymbolDismiss24 = 0xE88B,
    /// <summary>
    /// The number symbol square20.
    /// </summary>
    NumberSymbolSquare20 = 0xE88C,
    /// <summary>
    /// The number symbol square24.
    /// </summary>
    NumberSymbolSquare24 = 0xE88D,
    /// <summary>
    /// The open28.
    /// </summary>
    Open28 = 0xE88E,
    /// <summary>
    /// The open48.
    /// </summary>
    Open48 = 0xE88F,
    /// <summary>
    /// The open folder16.
    /// </summary>
    OpenFolder16 = 0xE890,
    /// <summary>
    /// The open folder20.
    /// </summary>
    OpenFolder20 = 0xE891,
    /// <summary>
    /// The open folder28.
    /// </summary>
    OpenFolder28 = 0xE892,
    /// <summary>
    /// The open folder48.
    /// </summary>
    OpenFolder48 = 0xE893,
    /// <summary>
    /// The open off16.
    /// </summary>
    OpenOff16 = 0xE894,
    /// <summary>
    /// The open off20.
    /// </summary>
    OpenOff20 = 0xE895,
    /// <summary>
    /// The open off24.
    /// </summary>
    OpenOff24 = 0xE896,
    /// <summary>
    /// The open off28.
    /// </summary>
    OpenOff28 = 0xE897,
    /// <summary>
    /// The open off48.
    /// </summary>
    OpenOff48 = 0xE898,
    /// <summary>
    /// The options48.
    /// </summary>
    Options48 = 0xE899,
    /// <summary>
    /// The organization12.
    /// </summary>
    Organization12 = 0xE89A,
    /// <summary>
    /// The organization16.
    /// </summary>
    Organization16 = 0xE89B,
    /// <summary>
    /// The organization32.
    /// </summary>
    Organization32 = 0xE89C,
    /// <summary>
    /// The organization48.
    /// </summary>
    Organization48 = 0xE89D,
    /// <summary>
    /// The organization horizontal20.
    /// </summary>
    OrganizationHorizontal20 = 0xE89E,
    /// <summary>
    /// The orientation20.
    /// </summary>
    Orientation20 = 0xE89F,
    /// <summary>
    /// The orientation24.
    /// </summary>
    Orientation24 = 0xE8A0,
    /// <summary>
    /// The oval16.
    /// </summary>
    Oval16 = 0xE8A1,
    /// <summary>
    /// The oval20.
    /// </summary>
    Oval20 = 0xE8A2,
    /// <summary>
    /// The oval24.
    /// </summary>
    Oval24 = 0xE8A3,
    /// <summary>
    /// The oval28.
    /// </summary>
    Oval28 = 0xE8A4,
    /// <summary>
    /// The oval32.
    /// </summary>
    Oval32 = 0xE8A5,
    /// <summary>
    /// The oval48.
    /// </summary>
    Oval48 = 0xE8A6,
    /// <summary>
    /// The paint brush arrow down20.
    /// </summary>
    PaintBrushArrowDown20 = 0xE8A7,
    /// <summary>
    /// The paint brush arrow down24.
    /// </summary>
    PaintBrushArrowDown24 = 0xE8A8,
    /// <summary>
    /// The paint brush arrow up20.
    /// </summary>
    PaintBrushArrowUp20 = 0xE8A9,
    /// <summary>
    /// The paint brush arrow up24.
    /// </summary>
    PaintBrushArrowUp24 = 0xE8AA,
    /// <summary>
    /// The pair20.
    /// </summary>
    Pair20 = 0xE8AB,
    /// <summary>
    /// The panel bottom20.
    /// </summary>
    PanelBottom20 = 0xE8AC,
    /// <summary>
    /// The panel bottom contract20.
    /// </summary>
    PanelBottomContract20 = 0xE8AD,
    /// <summary>
    /// The panel bottom expand20.
    /// </summary>
    PanelBottomExpand20 = 0xE8AE,
    /// <summary>
    /// The panel left16.
    /// </summary>
    PanelLeft16 = 0xE8AF,
    /// <summary>
    /// The panel left20.
    /// </summary>
    PanelLeft20 = 0xE8B0,
    /// <summary>
    /// The panel left24.
    /// </summary>
    PanelLeft24 = 0xE8B1,
    /// <summary>
    /// The panel left28.
    /// </summary>
    PanelLeft28 = 0xE8B2,
    /// <summary>
    /// The panel left48.
    /// </summary>
    PanelLeft48 = 0xE8B3,
    /// <summary>
    /// The panel left contract16.
    /// </summary>
    PanelLeftContract16 = 0xE8B4,
    /// <summary>
    /// The panel left contract20.
    /// </summary>
    PanelLeftContract20 = 0xE8B5,
    /// <summary>
    /// The panel left contract24.
    /// </summary>
    PanelLeftContract24 = 0xE8B6,
    /// <summary>
    /// The panel left contract28.
    /// </summary>
    PanelLeftContract28 = 0xE8B7,
    /// <summary>
    /// The panel left expand16.
    /// </summary>
    PanelLeftExpand16 = 0xE8B8,
    /// <summary>
    /// The panel left expand20.
    /// </summary>
    PanelLeftExpand20 = 0xE8B9,
    /// <summary>
    /// The panel left expand24.
    /// </summary>
    PanelLeftExpand24 = 0xE8BA,
    /// <summary>
    /// The panel left expand28.
    /// </summary>
    PanelLeftExpand28 = 0xE8BB,
    /// <summary>
    /// The panel right16.
    /// </summary>
    PanelRight16 = 0xE8BC,
    /// <summary>
    /// The panel right20.
    /// </summary>
    PanelRight20 = 0xE8BD,
    /// <summary>
    /// The panel right24.
    /// </summary>
    PanelRight24 = 0xE8BE,
    /// <summary>
    /// The panel right28.
    /// </summary>
    PanelRight28 = 0xE8BF,
    /// <summary>
    /// The panel right48.
    /// </summary>
    PanelRight48 = 0xE8C0,
    /// <summary>
    /// The panel right contract16.
    /// </summary>
    PanelRightContract16 = 0xE8C1,
    /// <summary>
    /// The panel right contract20.
    /// </summary>
    PanelRightContract20 = 0xE8C2,
    /// <summary>
    /// The panel right contract24.
    /// </summary>
    PanelRightContract24 = 0xE8C3,
    /// <summary>
    /// The panel right expand20.
    /// </summary>
    PanelRightExpand20 = 0xE8C4,
    /// <summary>
    /// The panel separate window20.
    /// </summary>
    PanelSeparateWindow20 = 0xE8C5,
    /// <summary>
    /// The panel top contract20.
    /// </summary>
    PanelTopContract20 = 0xE8C6,
    /// <summary>
    /// The panel top expand20.
    /// </summary>
    PanelTopExpand20 = 0xE8C7,
    /// <summary>
    /// The password16.
    /// </summary>
    Password16 = 0xE8C8,
    /// <summary>
    /// The password20.
    /// </summary>
    Password20 = 0xE8C9,
    /// <summary>
    /// The patient20.
    /// </summary>
    Patient20 = 0xE8CA,
    /// <summary>
    /// The patient32.
    /// </summary>
    Patient32 = 0xE8CB,
    /// <summary>
    /// The pause12.
    /// </summary>
    Pause12 = 0xE8CC,
    /// <summary>
    /// The pause28.
    /// </summary>
    Pause28 = 0xE8CD,
    /// <summary>
    /// The pause32.
    /// </summary>
    Pause32 = 0xE8CE,
    /// <summary>
    /// The pause circle24.
    /// </summary>
    PauseCircle24 = 0xE8CF,
    /// <summary>
    /// The pause off16.
    /// </summary>
    PauseOff16 = 0xE8D0,
    /// <summary>
    /// The pause off20.
    /// </summary>
    PauseOff20 = 0xE8D1,
    /// <summary>
    /// The pause settings16.
    /// </summary>
    PauseSettings16 = 0xE8D2,
    /// <summary>
    /// The pause settings20.
    /// </summary>
    PauseSettings20 = 0xE8D3,
    /// <summary>
    /// The payment16.
    /// </summary>
    Payment16 = 0xE8D4,
    /// <summary>
    /// The payment28.
    /// </summary>
    Payment28 = 0xE8D5,
    /// <summary>
    /// The pen16.
    /// </summary>
    Pen16 = 0xE8D6,
    /// <summary>
    /// The pen20.
    /// </summary>
    Pen20 = 0xE8D7,
    /// <summary>
    /// The pen24.
    /// </summary>
    Pen24 = 0xE8D8,
    /// <summary>
    /// The pen28.
    /// </summary>
    Pen28 = 0xE8D9,
    /// <summary>
    /// The pen32.
    /// </summary>
    Pen32 = 0xE8DA,
    /// <summary>
    /// The pen48.
    /// </summary>
    Pen48 = 0xE8DB,
    /// <summary>
    /// The pen off16.
    /// </summary>
    PenOff16 = 0xE8DC,
    /// <summary>
    /// The pen off20.
    /// </summary>
    PenOff20 = 0xE8DD,
    /// <summary>
    /// The pen off24.
    /// </summary>
    PenOff24 = 0xE8DE,
    /// <summary>
    /// The pen off28.
    /// </summary>
    PenOff28 = 0xE8DF,
    /// <summary>
    /// The pen off32.
    /// </summary>
    PenOff32 = 0xE8E0,
    /// <summary>
    /// The pen off48.
    /// </summary>
    PenOff48 = 0xE8E1,
    /// <summary>
    /// The pen prohibited16.
    /// </summary>
    PenProhibited16 = 0xE8E2,
    /// <summary>
    /// The pen prohibited20.
    /// </summary>
    PenProhibited20 = 0xE8E3,
    /// <summary>
    /// The pen prohibited24.
    /// </summary>
    PenProhibited24 = 0xE8E4,
    /// <summary>
    /// The pen prohibited28.
    /// </summary>
    PenProhibited28 = 0xE8E5,
    /// <summary>
    /// The pen prohibited32.
    /// </summary>
    PenProhibited32 = 0xE8E6,
    /// <summary>
    /// The pen prohibited48.
    /// </summary>
    PenProhibited48 = 0xE8E7,
    /// <summary>
    /// The pentagon20.
    /// </summary>
    Pentagon20 = 0xE8E8,
    /// <summary>
    /// The pentagon32.
    /// </summary>
    Pentagon32 = 0xE8E9,
    /// <summary>
    /// The pentagon48.
    /// </summary>
    Pentagon48 = 0xE8EA,
    /// <summary>
    /// The people12.
    /// </summary>
    People12 = 0xE8EB,
    /// <summary>
    /// The people32.
    /// </summary>
    People32 = 0xE8EC,
    /// <summary>
    /// The people48.
    /// </summary>
    People48 = 0xE8ED,
    /// <summary>
    /// The people add28.
    /// </summary>
    PeopleAdd28 = 0xE8EE,
    /// <summary>
    /// The people audience20.
    /// </summary>
    PeopleAudience20 = 0xE8EF,
    /// <summary>
    /// The people call16.
    /// </summary>
    PeopleCall16 = 0xE8F0,
    /// <summary>
    /// The people call20.
    /// </summary>
    PeopleCall20 = 0xE8F1,
    /// <summary>
    /// The people checkmark16.
    /// </summary>
    PeopleCheckmark16 = 0xE8F2,
    /// <summary>
    /// The people checkmark20.
    /// </summary>
    PeopleCheckmark20 = 0xE8F3,
    /// <summary>
    /// The people checkmark24.
    /// </summary>
    PeopleCheckmark24 = 0xE8F4,
    /// <summary>
    /// The people community add20.
    /// </summary>
    PeopleCommunityAdd20 = 0xE8F5,
    /// <summary>
    /// The people community add28.
    /// </summary>
    PeopleCommunityAdd28 = 0xE8F6,
    /// <summary>
    /// The people edit20.
    /// </summary>
    PeopleEdit20 = 0xE8F7,
    /// <summary>
    /// The people error16.
    /// </summary>
    PeopleError16 = 0xE8F8,
    /// <summary>
    /// The people error20.
    /// </summary>
    PeopleError20 = 0xE8F9,
    /// <summary>
    /// The people error24.
    /// </summary>
    PeopleError24 = 0xE8FA,
    /// <summary>
    /// The people list16.
    /// </summary>
    PeopleList16 = 0xE8FB,
    /// <summary>
    /// The people list20.
    /// </summary>
    PeopleList20 = 0xE8FC,
    /// <summary>
    /// The people list24.
    /// </summary>
    PeopleList24 = 0xE8FD,
    /// <summary>
    /// The people list28.
    /// </summary>
    PeopleList28 = 0xE8FE,
    /// <summary>
    /// The people lock20.
    /// </summary>
    PeopleLock20 = 0xE8FF,
    /// <summary>
    /// The people lock24.
    /// </summary>
    PeopleLock24 = 0xE900,
    /// <summary>
    /// The people money20.
    /// </summary>
    PeopleMoney20 = 0xE901,
    /// <summary>
    /// The people money24.
    /// </summary>
    PeopleMoney24 = 0xE902,
    /// <summary>
    /// The people prohibited16.
    /// </summary>
    PeopleProhibited16 = 0xE903,
    /// <summary>
    /// The people prohibited24.
    /// </summary>
    PeopleProhibited24 = 0xE904,
    /// <summary>
    /// The people queue20.
    /// </summary>
    PeopleQueue20 = 0xE905,
    /// <summary>
    /// The people queue24.
    /// </summary>
    PeopleQueue24 = 0xE906,
    /// <summary>
    /// The people search20.
    /// </summary>
    PeopleSearch20 = 0xE907,
    /// <summary>
    /// The people settings24.
    /// </summary>
    PeopleSettings24 = 0xE908,
    /// <summary>
    /// The people settings28.
    /// </summary>
    PeopleSettings28 = 0xE909,
    /// <summary>
    /// The people swap16.
    /// </summary>
    PeopleSwap16 = 0xE90A,
    /// <summary>
    /// The people swap20.
    /// </summary>
    PeopleSwap20 = 0xE90B,
    /// <summary>
    /// The people swap24.
    /// </summary>
    PeopleSwap24 = 0xE90C,
    /// <summary>
    /// The people swap28.
    /// </summary>
    PeopleSwap28 = 0xE90D,
    /// <summary>
    /// The people sync20.
    /// </summary>
    PeopleSync20 = 0xE90E,
    /// <summary>
    /// The people sync28.
    /// </summary>
    PeopleSync28 = 0xE90F,
    /// <summary>
    /// The people team32.
    /// </summary>
    PeopleTeam32 = 0xE910,
    /// <summary>
    /// The people team add20.
    /// </summary>
    PeopleTeamAdd20 = 0xE911,
    /// <summary>
    /// The people team add24.
    /// </summary>
    PeopleTeamAdd24 = 0xE912,
    /// <summary>
    /// The people team delete16.
    /// </summary>
    PeopleTeamDelete16 = 0xE913,
    /// <summary>
    /// The people team delete20.
    /// </summary>
    PeopleTeamDelete20 = 0xE914,
    /// <summary>
    /// The people team delete24.
    /// </summary>
    PeopleTeamDelete24 = 0xE915,
    /// <summary>
    /// The people team delete28.
    /// </summary>
    PeopleTeamDelete28 = 0xE916,
    /// <summary>
    /// The people team delete32.
    /// </summary>
    PeopleTeamDelete32 = 0xE917,
    /// <summary>
    /// The people team toolbox20.
    /// </summary>
    PeopleTeamToolbox20 = 0xE918,
    /// <summary>
    /// The people team toolbox24.
    /// </summary>
    PeopleTeamToolbox24 = 0xE919,
    /// <summary>
    /// The people toolbox20.
    /// </summary>
    PeopleToolbox20 = 0xE91A,
    /// <summary>
    /// The person32.
    /// </summary>
    Person32 = 0xE91B,
    /// <summary>
    /// The person520.
    /// </summary>
    Person520 = 0xE91C,
    /// <summary>
    /// The person532.
    /// </summary>
    Person532 = 0xE91D,
    /// <summary>
    /// The person620.
    /// </summary>
    Person620 = 0xE91E,
    /// <summary>
    /// The person632.
    /// </summary>
    Person632 = 0xE91F,
    /// <summary>
    /// The person accounts20.
    /// </summary>
    PersonAccounts20 = 0xE920,
    /// <summary>
    /// The person add16.
    /// </summary>
    PersonAdd16 = 0xE921,
    /// <summary>
    /// The person add28.
    /// </summary>
    PersonAdd28 = 0xE922,
    /// <summary>
    /// The person arrow left16.
    /// </summary>
    PersonArrowLeft16 = 0xE923,
    /// <summary>
    /// The person available20.
    /// </summary>
    PersonAvailable20 = 0xE924,
    /// <summary>
    /// The person call16.
    /// </summary>
    PersonCall16 = 0xE925,
    /// <summary>
    /// The person call20.
    /// </summary>
    PersonCall20 = 0xE926,
    /// <summary>
    /// The person circle12.
    /// </summary>
    PersonCircle12 = 0xE927,
    /// <summary>
    /// The person circle20.
    /// </summary>
    PersonCircle20 = 0xE928,
    /// <summary>
    /// The person circle24.
    /// </summary>
    PersonCircle24 = 0xE929,
    /// <summary>
    /// The person clock16.
    /// </summary>
    PersonClock16 = 0xE92A,
    /// <summary>
    /// The person clock20.
    /// </summary>
    PersonClock20 = 0xE92B,
    /// <summary>
    /// The person clock24.
    /// </summary>
    PersonClock24 = 0xE92C,
    /// <summary>
    /// The person delete20.
    /// </summary>
    PersonDelete20 = 0xE92D,
    /// <summary>
    /// The person edit20.
    /// </summary>
    PersonEdit20 = 0xE92E,
    /// <summary>
    /// The person edit24.
    /// </summary>
    PersonEdit24 = 0xE92F,
    /// <summary>
    /// The person feedback16.
    /// </summary>
    PersonFeedback16 = 0xE930,
    /// <summary>
    /// The person heart24.
    /// </summary>
    PersonHeart24 = 0xE931,
    /// <summary>
    /// The person info20.
    /// </summary>
    PersonInfo20 = 0xE932,
    /// <summary>
    /// The person key20.
    /// </summary>
    PersonKey20 = 0xE933,
    /// <summary>
    /// The person lightbulb20.
    /// </summary>
    PersonLightbulb20 = 0xE934,
    /// <summary>
    /// The person lightbulb24.
    /// </summary>
    PersonLightbulb24 = 0xE935,
    /// <summary>
    /// The person lock24.
    /// </summary>
    PersonLock24 = 0xE936,
    /// <summary>
    /// The person mail16.
    /// </summary>
    PersonMail16 = 0xE937,
    /// <summary>
    /// The person mail20.
    /// </summary>
    PersonMail20 = 0xE938,
    /// <summary>
    /// The person mail24.
    /// </summary>
    PersonMail24 = 0xE939,
    /// <summary>
    /// The person mail28.
    /// </summary>
    PersonMail28 = 0xE93A,
    /// <summary>
    /// The person mail48.
    /// </summary>
    PersonMail48 = 0xE93B,
    /// <summary>
    /// The person money20.
    /// </summary>
    PersonMoney20 = 0xE93C,
    /// <summary>
    /// The person money24.
    /// </summary>
    PersonMoney24 = 0xE93D,
    /// <summary>
    /// The person note20.
    /// </summary>
    PersonNote20 = 0xE93E,
    /// <summary>
    /// The person note24.
    /// </summary>
    PersonNote24 = 0xE93F,
    /// <summary>
    /// The person pill20.
    /// </summary>
    PersonPill20 = 0xE940,
    /// <summary>
    /// The person pill24.
    /// </summary>
    PersonPill24 = 0xE941,
    /// <summary>
    /// The person prohibited16.
    /// </summary>
    PersonProhibited16 = 0xE942,
    /// <summary>
    /// The person prohibited24.
    /// </summary>
    PersonProhibited24 = 0xE943,
    /// <summary>
    /// The person prohibited28.
    /// </summary>
    PersonProhibited28 = 0xE944,
    /// <summary>
    /// The person settings16.
    /// </summary>
    PersonSettings16 = 0xE945,
    /// <summary>
    /// The person settings20.
    /// </summary>
    PersonSettings20 = 0xE946,
    /// <summary>
    /// The person subtract20.
    /// </summary>
    PersonSubtract20 = 0xE947,
    /// <summary>
    /// The person sync16.
    /// </summary>
    PersonSync16 = 0xE948,
    /// <summary>
    /// The person sync20.
    /// </summary>
    PersonSync20 = 0xE949,
    /// <summary>
    /// The person sync24.
    /// </summary>
    PersonSync24 = 0xE94A,
    /// <summary>
    /// The person sync28.
    /// </summary>
    PersonSync28 = 0xE94B,
    /// <summary>
    /// The person sync32.
    /// </summary>
    PersonSync32 = 0xE94C,
    /// <summary>
    /// The person sync48.
    /// </summary>
    PersonSync48 = 0xE94D,
    /// <summary>
    /// The person tag20.
    /// </summary>
    PersonTag20 = 0xE94E,
    /// <summary>
    /// The person tag24.
    /// </summary>
    PersonTag24 = 0xE94F,
    /// <summary>
    /// The person tag28.
    /// </summary>
    PersonTag28 = 0xE950,
    /// <summary>
    /// The person tag32.
    /// </summary>
    PersonTag32 = 0xE951,
    /// <summary>
    /// The person tag48.
    /// </summary>
    PersonTag48 = 0xE952,
    /// <summary>
    /// The phone12.
    /// </summary>
    Phone12 = 0xE953,
    /// <summary>
    /// The phone add20.
    /// </summary>
    PhoneAdd20 = 0xE954,
    /// <summary>
    /// The phone add24.
    /// </summary>
    PhoneAdd24 = 0xE955,
    /// <summary>
    /// The phone arrow right20.
    /// </summary>
    PhoneArrowRight20 = 0xE956,
    /// <summary>
    /// The phone arrow right24.
    /// </summary>
    PhoneArrowRight24 = 0xE957,
    /// <summary>
    /// The phone checkmark20.
    /// </summary>
    PhoneCheckmark20 = 0xE958,
    /// <summary>
    /// The phone desktop add20.
    /// </summary>
    PhoneDesktopAdd20 = 0xE959,
    /// <summary>
    /// The phone dismiss20.
    /// </summary>
    PhoneDismiss20 = 0xE95A,
    /// <summary>
    /// The phone dismiss24.
    /// </summary>
    PhoneDismiss24 = 0xE95B,
    /// <summary>
    /// The phone eraser16.
    /// </summary>
    PhoneEraser16 = 0xE95C,
    /// <summary>
    /// The phone eraser20.
    /// </summary>
    PhoneEraser20 = 0xE95D,
    /// <summary>
    /// The phone key20.
    /// </summary>
    PhoneKey20 = 0xE95E,
    /// <summary>
    /// The phone key24.
    /// </summary>
    PhoneKey24 = 0xE95F,
    /// <summary>
    /// The phone laptop16.
    /// </summary>
    PhoneLaptop16 = 0xE960,
    /// <summary>
    /// The phone laptop32.
    /// </summary>
    PhoneLaptop32 = 0xE961,
    /// <summary>
    /// The phone link setup20.
    /// </summary>
    PhoneLinkSetup20 = 0xE962,
    /// <summary>
    /// The phone lock20.
    /// </summary>
    PhoneLock20 = 0xE963,
    /// <summary>
    /// The phone lock24.
    /// </summary>
    PhoneLock24 = 0xE964,
    /// <summary>
    /// The phone page header20.
    /// </summary>
    PhonePageHeader20 = 0xE965,
    /// <summary>
    /// The phone pagination20.
    /// </summary>
    PhonePagination20 = 0xE966,
    /// <summary>
    /// The phone screen time20.
    /// </summary>
    PhoneScreenTime20 = 0xE967,
    /// <summary>
    /// The phone shake20.
    /// </summary>
    PhoneShake20 = 0xE968,
    /// <summary>
    /// The phone span in16.
    /// </summary>
    PhoneSpanIn16 = 0xE969,
    /// <summary>
    /// The phone span in20.
    /// </summary>
    PhoneSpanIn20 = 0xE96A,
    /// <summary>
    /// The phone span in24.
    /// </summary>
    PhoneSpanIn24 = 0xE96B,
    /// <summary>
    /// The phone span in28.
    /// </summary>
    PhoneSpanIn28 = 0xE96C,
    /// <summary>
    /// The phone span out16.
    /// </summary>
    PhoneSpanOut16 = 0xE96D,
    /// <summary>
    /// The phone span out20.
    /// </summary>
    PhoneSpanOut20 = 0xE96E,
    /// <summary>
    /// The phone span out24.
    /// </summary>
    PhoneSpanOut24 = 0xE96F,
    /// <summary>
    /// The phone span out28.
    /// </summary>
    PhoneSpanOut28 = 0xE970,
    /// <summary>
    /// The phone speaker20.
    /// </summary>
    PhoneSpeaker20 = 0xE971,
    /// <summary>
    /// The phone speaker24.
    /// </summary>
    PhoneSpeaker24 = 0xE972,
    /// <summary>
    /// The phone status bar20.
    /// </summary>
    PhoneStatusBar20 = 0xE973,
    /// <summary>
    /// The phone update20.
    /// </summary>
    PhoneUpdate20 = 0xE974,
    /// <summary>
    /// The phone update checkmark20.
    /// </summary>
    PhoneUpdateCheckmark20 = 0xE975,
    /// <summary>
    /// The phone update checkmark24.
    /// </summary>
    PhoneUpdateCheckmark24 = 0xE976,
    /// <summary>
    /// The phone vertical scroll20.
    /// </summary>
    PhoneVerticalScroll20 = 0xE977,
    /// <summary>
    /// The phone vibrate20.
    /// </summary>
    PhoneVibrate20 = 0xE978,
    /// <summary>
    /// The photo filter20.
    /// </summary>
    PhotoFilter20 = 0xE979,
    /// <summary>
    /// The pi20.
    /// </summary>
    Pi20 = 0xE97A,
    /// <summary>
    /// The pi24.
    /// </summary>
    Pi24 = 0xE97B,
    /// <summary>
    /// The picture in picture enter16.
    /// </summary>
    PictureInPictureEnter16 = 0xE97C,
    /// <summary>
    /// The picture in picture enter20.
    /// </summary>
    PictureInPictureEnter20 = 0xE97D,
    /// <summary>
    /// The picture in picture enter24.
    /// </summary>
    PictureInPictureEnter24 = 0xE97E,
    /// <summary>
    /// The picture in picture exit16.
    /// </summary>
    PictureInPictureExit16 = 0xE97F,
    /// <summary>
    /// The picture in picture exit20.
    /// </summary>
    PictureInPictureExit20 = 0xE980,
    /// <summary>
    /// The picture in picture exit24.
    /// </summary>
    PictureInPictureExit24 = 0xE981,
    /// <summary>
    /// The pin28.
    /// </summary>
    Pin28 = 0xE982,
    /// <summary>
    /// The pin32.
    /// </summary>
    Pin32 = 0xE983,
    /// <summary>
    /// The pin48.
    /// </summary>
    Pin48 = 0xE984,
    /// <summary>
    /// The pin off16.
    /// </summary>
    PinOff16 = 0xE985,
    /// <summary>
    /// The pin off28.
    /// </summary>
    PinOff28 = 0xE986,
    /// <summary>
    /// The pin off32.
    /// </summary>
    PinOff32 = 0xE987,
    /// <summary>
    /// The pin off48.
    /// </summary>
    PinOff48 = 0xE988,
    /// <summary>
    /// The pipeline20.
    /// </summary>
    Pipeline20 = 0xE989,
    /// <summary>
    /// The pipeline add20.
    /// </summary>
    PipelineAdd20 = 0xE98A,
    /// <summary>
    /// The pipeline arrow curve down20.
    /// </summary>
    PipelineArrowCurveDown20 = 0xE98B,
    /// <summary>
    /// The pipeline play20.
    /// </summary>
    PipelinePlay20 = 0xE98C,
    /// <summary>
    /// The pivot20.
    /// </summary>
    Pivot20 = 0xE98D,
    /// <summary>
    /// The pivot24.
    /// </summary>
    Pivot24 = 0xE98E,
    /// <summary>
    /// The play12.
    /// </summary>
    Play12 = 0xE98F,
    /// <summary>
    /// The play16.
    /// </summary>
    Play16 = 0xE990,
    /// <summary>
    /// The play28.
    /// </summary>
    Play28 = 0xE991,
    /// <summary>
    /// The play32.
    /// </summary>
    Play32 = 0xE992,
    /// <summary>
    /// The play circle16.
    /// </summary>
    PlayCircle16 = 0xE993,
    /// <summary>
    /// The play circle20.
    /// </summary>
    PlayCircle20 = 0xE994,
    /// <summary>
    /// The play circle28.
    /// </summary>
    PlayCircle28 = 0xE995,
    /// <summary>
    /// The play circle48.
    /// </summary>
    PlayCircle48 = 0xE996,
    /// <summary>
    /// The play settings20.
    /// </summary>
    PlaySettings20 = 0xE997,
    /// <summary>
    /// The playing cards20.
    /// </summary>
    PlayingCards20 = 0xE998,
    /// <summary>
    /// The plug connected20.
    /// </summary>
    PlugConnected20 = 0xE999,
    /// <summary>
    /// The plug connected24.
    /// </summary>
    PlugConnected24 = 0xE99A,
    /// <summary>
    /// The plug connected add20.
    /// </summary>
    PlugConnectedAdd20 = 0xE99B,
    /// <summary>
    /// The plug connected checkmark20.
    /// </summary>
    PlugConnectedCheckmark20 = 0xE99C,
    /// <summary>
    /// The point scan20.
    /// </summary>
    PointScan20 = 0xE99D,
    /// <summary>
    /// The poll16.
    /// </summary>
    Poll16 = 0xE99E,
    /// <summary>
    /// The poll20.
    /// </summary>
    Poll20 = 0xE99F,
    /// <summary>
    /// The port hdmi20.
    /// </summary>
    PortHdmi20 = 0xE9A0,
    /// <summary>
    /// The port hdmi24.
    /// </summary>
    PortHdmi24 = 0xE9A1,
    /// <summary>
    /// The port micro usb20.
    /// </summary>
    PortMicroUsb20 = 0xE9A2,
    /// <summary>
    /// The port micro usb24.
    /// </summary>
    PortMicroUsb24 = 0xE9A3,
    /// <summary>
    /// The port usb a20.
    /// </summary>
    PortUsbA20 = 0xE9A4,
    /// <summary>
    /// The port usb a24.
    /// </summary>
    PortUsbA24 = 0xE9A5,
    /// <summary>
    /// The port usb C20.
    /// </summary>
    PortUsbC20 = 0xE9A6,
    /// <summary>
    /// The port usb C24.
    /// </summary>
    PortUsbC24 = 0xE9A7,
    /// <summary>
    /// The position backward20.
    /// </summary>
    PositionBackward20 = 0xE9A8,
    /// <summary>
    /// The position backward24.
    /// </summary>
    PositionBackward24 = 0xE9A9,
    /// <summary>
    /// The position forward20.
    /// </summary>
    PositionForward20 = 0xE9AA,
    /// <summary>
    /// The position forward24.
    /// </summary>
    PositionForward24 = 0xE9AB,
    /// <summary>
    /// The position to back20.
    /// </summary>
    PositionToBack20 = 0xE9AC,
    /// <summary>
    /// The position to back24.
    /// </summary>
    PositionToBack24 = 0xE9AD,
    /// <summary>
    /// The position to front20.
    /// </summary>
    PositionToFront20 = 0xE9AE,
    /// <summary>
    /// The position to front24.
    /// </summary>
    PositionToFront24 = 0xE9AF,
    /// <summary>
    /// The predictions20.
    /// </summary>
    Predictions20 = 0xE9B0,
    /// <summary>
    /// The premium32.
    /// </summary>
    Premium32 = 0xE9B1,
    /// <summary>
    /// The premium person16.
    /// </summary>
    PremiumPerson16 = 0xE9B2,
    /// <summary>
    /// The premium person20.
    /// </summary>
    PremiumPerson20 = 0xE9B3,
    /// <summary>
    /// The premium person24.
    /// </summary>
    PremiumPerson24 = 0xE9B4,
    /// <summary>
    /// The presence available10.
    /// </summary>
    PresenceAvailable10 = 0xE9B5,
    /// <summary>
    /// The presence available12.
    /// </summary>
    PresenceAvailable12 = 0xE9B6,
    /// <summary>
    /// The presence available16.
    /// </summary>
    PresenceAvailable16 = 0xE9B7,
    /// <summary>
    /// The presence available20.
    /// </summary>
    PresenceAvailable20 = 0xE9B8,
    /// <summary>
    /// The presence available24.
    /// </summary>
    PresenceAvailable24 = 0xE9B9,
    /// <summary>
    /// The presence DND10.
    /// </summary>
    PresenceDnd10 = 0xE9BC,
    /// <summary>
    /// The presence DND12.
    /// </summary>
    PresenceDnd12 = 0xE9BD,
    /// <summary>
    /// The presence DND16.
    /// </summary>
    PresenceDnd16 = 0xE9BE,
    /// <summary>
    /// The presence DND20.
    /// </summary>
    PresenceDnd20 = 0xE9BF,
    /// <summary>
    /// The presence DND24.
    /// </summary>
    PresenceDnd24 = 0xE9C0,
    /// <summary>
    /// The presenter20.
    /// </summary>
    Presenter20 = 0xE9C7,
    /// <summary>
    /// The presenter off20.
    /// </summary>
    PresenterOff20 = 0xE9C8,
    /// <summary>
    /// The previous28.
    /// </summary>
    Previous28 = 0xE9C9,
    /// <summary>
    /// The previous32.
    /// </summary>
    Previous32 = 0xE9CA,
    /// <summary>
    /// The previous48.
    /// </summary>
    Previous48 = 0xE9CB,
    /// <summary>
    /// The print28.
    /// </summary>
    Print28 = 0xE9CC,
    /// <summary>
    /// The print32.
    /// </summary>
    Print32 = 0xE9CD,
    /// <summary>
    /// The print add24.
    /// </summary>
    PrintAdd24 = 0xE9CE,
    /// <summary>
    /// The prohibited12.
    /// </summary>
    Prohibited12 = 0xE9CF,
    /// <summary>
    /// The prohibited multiple16.
    /// </summary>
    ProhibitedMultiple16 = 0xE9D0,
    /// <summary>
    /// The prohibited multiple20.
    /// </summary>
    ProhibitedMultiple20 = 0xE9D1,
    /// <summary>
    /// The prohibited multiple24.
    /// </summary>
    ProhibitedMultiple24 = 0xE9D2,
    /// <summary>
    /// The prohibited note20.
    /// </summary>
    ProhibitedNote20 = 0xE9D3,
    /// <summary>
    /// The projection screen16.
    /// </summary>
    ProjectionScreen16 = 0xE9D4,
    /// <summary>
    /// The projection screen20.
    /// </summary>
    ProjectionScreen20 = 0xE9D5,
    /// <summary>
    /// The projection screen24.
    /// </summary>
    ProjectionScreen24 = 0xE9D6,
    /// <summary>
    /// The projection screen28.
    /// </summary>
    ProjectionScreen28 = 0xE9D7,
    /// <summary>
    /// The projection screen dismiss16.
    /// </summary>
    ProjectionScreenDismiss16 = 0xE9D8,
    /// <summary>
    /// The projection screen dismiss20.
    /// </summary>
    ProjectionScreenDismiss20 = 0xE9D9,
    /// <summary>
    /// The projection screen dismiss24.
    /// </summary>
    ProjectionScreenDismiss24 = 0xE9DA,
    /// <summary>
    /// The projection screen dismiss28.
    /// </summary>
    ProjectionScreenDismiss28 = 0xE9DB,
    /// <summary>
    /// The pulse20.
    /// </summary>
    Pulse20 = 0xE9DC,
    /// <summary>
    /// The pulse24.
    /// </summary>
    Pulse24 = 0xE9DD,
    /// <summary>
    /// The pulse28.
    /// </summary>
    Pulse28 = 0xE9DE,
    /// <summary>
    /// The pulse32.
    /// </summary>
    Pulse32 = 0xE9DF,
    /// <summary>
    /// The pulse square20.
    /// </summary>
    PulseSquare20 = 0xE9E0,
    /// <summary>
    /// The pulse square24.
    /// </summary>
    PulseSquare24 = 0xE9E1,
    /// <summary>
    /// The puzzle cube16.
    /// </summary>
    PuzzleCube16 = 0xE9E2,
    /// <summary>
    /// The puzzle cube20.
    /// </summary>
    PuzzleCube20 = 0xE9E3,
    /// <summary>
    /// The puzzle cube24.
    /// </summary>
    PuzzleCube24 = 0xE9E4,
    /// <summary>
    /// The puzzle cube28.
    /// </summary>
    PuzzleCube28 = 0xE9E5,
    /// <summary>
    /// The puzzle cube48.
    /// </summary>
    PuzzleCube48 = 0xE9E6,
    /// <summary>
    /// The puzzle cube piece20.
    /// </summary>
    PuzzleCubePiece20 = 0xE9E7,
    /// <summary>
    /// The puzzle piece16.
    /// </summary>
    PuzzlePiece16 = 0xE9E8,
    /// <summary>
    /// The puzzle piece20.
    /// </summary>
    PuzzlePiece20 = 0xE9E9,
    /// <summary>
    /// The puzzle piece24.
    /// </summary>
    PuzzlePiece24 = 0xE9EA,
    /// <summary>
    /// The puzzle piece shield20.
    /// </summary>
    PuzzlePieceShield20 = 0xE9EB,
    /// <summary>
    /// The qr code20.
    /// </summary>
    QrCode20 = 0xE9EC,
    /// <summary>
    /// The question circle12.
    /// </summary>
    QuestionCircle12 = 0xE9ED,
    /// <summary>
    /// The question circle32.
    /// </summary>
    QuestionCircle32 = 0xE9EE,
    /// <summary>
    /// The quiz new20.
    /// </summary>
    QuizNew20 = 0xE9EF,
    /// <summary>
    /// The radar20.
    /// </summary>
    Radar20 = 0xE9F0,
    /// <summary>
    /// The radar checkmark20.
    /// </summary>
    RadarCheckmark20 = 0xE9F1,
    /// <summary>
    /// The radar rectangle multiple20.
    /// </summary>
    RadarRectangleMultiple20 = 0xE9F2,
    /// <summary>
    /// The ram20.
    /// </summary>
    Ram20 = 0xE9F3,
    /// <summary>
    /// The re order dots horizontal16.
    /// </summary>
    ReOrderDotsHorizontal16 = 0xE9F4,
    /// <summary>
    /// The re order dots horizontal20.
    /// </summary>
    ReOrderDotsHorizontal20 = 0xE9F5,
    /// <summary>
    /// The re order dots horizontal24.
    /// </summary>
    ReOrderDotsHorizontal24 = 0xE9F6,
    /// <summary>
    /// The re order dots vertical16.
    /// </summary>
    ReOrderDotsVertical16 = 0xE9F7,
    /// <summary>
    /// The re order dots vertical20.
    /// </summary>
    ReOrderDotsVertical20 = 0xE9F8,
    /// <summary>
    /// The re order dots vertical24.
    /// </summary>
    ReOrderDotsVertical24 = 0xE9F9,
    /// <summary>
    /// The read aloud16.
    /// </summary>
    ReadAloud16 = 0xE9FA,
    /// <summary>
    /// The read aloud28.
    /// </summary>
    ReadAloud28 = 0xE9FB,
    /// <summary>
    /// The real estate20.
    /// </summary>
    RealEstate20 = 0xE9FC,
    /// <summary>
    /// The real estate24.
    /// </summary>
    RealEstate24 = 0xE9FD,
    /// <summary>
    /// The receipt20.
    /// </summary>
    Receipt20 = 0xE9FE,
    /// <summary>
    /// The receipt24.
    /// </summary>
    Receipt24 = 0xE9FF,
    /// <summary>
    /// The receipt add24.
    /// </summary>
    ReceiptAdd24 = 0xEA00,
    /// <summary>
    /// The receipt bag24.
    /// </summary>
    ReceiptBag24 = 0xEA01,
    /// <summary>
    /// The receipt cube24.
    /// </summary>
    ReceiptCube24 = 0xEA02,
    /// <summary>
    /// The receipt money24.
    /// </summary>
    ReceiptMoney24 = 0xEA03,
    /// <summary>
    /// The receipt play20.
    /// </summary>
    ReceiptPlay20 = 0xEA04,
    /// <summary>
    /// The receipt play24.
    /// </summary>
    ReceiptPlay24 = 0xEA05,
    /// <summary>
    /// The receipt search20.
    /// </summary>
    ReceiptSearch20 = 0xEA06,
    /// <summary>
    /// The rectangle landscape12.
    /// </summary>
    RectangleLandscape12 = 0xEA07,
    /// <summary>
    /// The rectangle landscape16.
    /// </summary>
    RectangleLandscape16 = 0xEA08,
    /// <summary>
    /// The rectangle landscape20.
    /// </summary>
    RectangleLandscape20 = 0xEA09,
    /// <summary>
    /// The rectangle landscape24.
    /// </summary>
    RectangleLandscape24 = 0xEA0A,
    /// <summary>
    /// The rectangle landscape28.
    /// </summary>
    RectangleLandscape28 = 0xEA0B,
    /// <summary>
    /// The rectangle landscape32.
    /// </summary>
    RectangleLandscape32 = 0xEA0C,
    /// <summary>
    /// The rectangle landscape48.
    /// </summary>
    RectangleLandscape48 = 0xEA0D,
    /// <summary>
    /// The rectangle portrait location target20.
    /// </summary>
    RectanglePortraitLocationTarget20 = 0xEA0E,
    /// <summary>
    /// The remote16.
    /// </summary>
    Remote16 = 0xEA0F,
    /// <summary>
    /// The remote20.
    /// </summary>
    Remote20 = 0xEA10,
    /// <summary>
    /// The reorder20.
    /// </summary>
    Reorder20 = 0xEA11,
    /// <summary>
    /// The replay20.
    /// </summary>
    Replay20 = 0xEA12,
    /// <summary>
    /// The resize24.
    /// </summary>
    Resize24 = 0xEA13,
    /// <summary>
    /// The resize image20.
    /// </summary>
    ResizeImage20 = 0xEA14,
    /// <summary>
    /// The resize large16.
    /// </summary>
    ResizeLarge16 = 0xEA15,
    /// <summary>
    /// The resize large20.
    /// </summary>
    ResizeLarge20 = 0xEA16,
    /// <summary>
    /// The resize large24.
    /// </summary>
    ResizeLarge24 = 0xEA17,
    /// <summary>
    /// The resize small16.
    /// </summary>
    ResizeSmall16 = 0xEA18,
    /// <summary>
    /// The resize small20.
    /// </summary>
    ResizeSmall20 = 0xEA19,
    /// <summary>
    /// The resize small24.
    /// </summary>
    ResizeSmall24 = 0xEA1A,
    /// <summary>
    /// The resize table20.
    /// </summary>
    ResizeTable20 = 0xEA1B,
    /// <summary>
    /// The resize video20.
    /// </summary>
    ResizeVideo20 = 0xEA1C,
    /// <summary>
    /// The rewind16.
    /// </summary>
    Rewind16 = 0xEA1D,
    /// <summary>
    /// The rewind28.
    /// </summary>
    Rewind28 = 0xEA1E,
    /// <summary>
    /// The rhombus16.
    /// </summary>
    Rhombus16 = 0xEA1F,
    /// <summary>
    /// The rhombus20.
    /// </summary>
    Rhombus20 = 0xEA20,
    /// <summary>
    /// The rhombus24.
    /// </summary>
    Rhombus24 = 0xEA21,
    /// <summary>
    /// The rhombus28.
    /// </summary>
    Rhombus28 = 0xEA22,
    /// <summary>
    /// The rhombus32.
    /// </summary>
    Rhombus32 = 0xEA23,
    /// <summary>
    /// The rhombus48.
    /// </summary>
    Rhombus48 = 0xEA24,
    /// <summary>
    /// The ribbon12.
    /// </summary>
    Ribbon12 = 0xEA25,
    /// <summary>
    /// The ribbon16.
    /// </summary>
    Ribbon16 = 0xEA26,
    /// <summary>
    /// The ribbon20.
    /// </summary>
    Ribbon20 = 0xEA27,
    /// <summary>
    /// The ribbon24.
    /// </summary>
    Ribbon24 = 0xEA28,
    /// <summary>
    /// The ribbon32.
    /// </summary>
    Ribbon32 = 0xEA29,
    /// <summary>
    /// The ribbon off12.
    /// </summary>
    RibbonOff12 = 0xEA2A,
    /// <summary>
    /// The ribbon off16.
    /// </summary>
    RibbonOff16 = 0xEA2B,
    /// <summary>
    /// The ribbon off20.
    /// </summary>
    RibbonOff20 = 0xEA2C,
    /// <summary>
    /// The ribbon off24.
    /// </summary>
    RibbonOff24 = 0xEA2D,
    /// <summary>
    /// The ribbon off32.
    /// </summary>
    RibbonOff32 = 0xEA2E,
    /// <summary>
    /// The ribbon star20.
    /// </summary>
    RibbonStar20 = 0xEA2F,
    /// <summary>
    /// The ribbon star24.
    /// </summary>
    RibbonStar24 = 0xEA30,
    /// <summary>
    /// The road cone16.
    /// </summary>
    RoadCone16 = 0xEA31,
    /// <summary>
    /// The road cone20.
    /// </summary>
    RoadCone20 = 0xEA32,
    /// <summary>
    /// The road cone24.
    /// </summary>
    RoadCone24 = 0xEA33,
    /// <summary>
    /// The road cone28.
    /// </summary>
    RoadCone28 = 0xEA34,
    /// <summary>
    /// The road cone32.
    /// </summary>
    RoadCone32 = 0xEA35,
    /// <summary>
    /// The road cone48.
    /// </summary>
    RoadCone48 = 0xEA36,
    /// <summary>
    /// The rotate left20.
    /// </summary>
    RotateLeft20 = 0xEA37,
    /// <summary>
    /// The rotate left24.
    /// </summary>
    RotateLeft24 = 0xEA38,
    /// <summary>
    /// The rotate right20.
    /// </summary>
    RotateRight20 = 0xEA39,
    /// <summary>
    /// The rotate right24.
    /// </summary>
    RotateRight24 = 0xEA3A,
    /// <summary>
    /// The router20.
    /// </summary>
    Router20 = 0xEA3B,
    /// <summary>
    /// The row triple20.
    /// </summary>
    RowTriple20 = 0xEA3C,
    /// <summary>
    /// The RSS20.
    /// </summary>
    Rss20 = 0xEA3D,
    /// <summary>
    /// The RSS24.
    /// </summary>
    Rss24 = 0xEA3E,
    /// <summary>
    /// The run16.
    /// </summary>
    Run16 = 0xEA3F,
    /// <summary>
    /// The run20.
    /// </summary>
    Run20 = 0xEA40,
    /// <summary>
    /// The sanitize20.
    /// </summary>
    Sanitize20 = 0xEA41,
    /// <summary>
    /// The sanitize24.
    /// </summary>
    Sanitize24 = 0xEA42,
    /// <summary>
    /// The save16.
    /// </summary>
    Save16 = 0xEA43,
    /// <summary>
    /// The save28.
    /// </summary>
    Save28 = 0xEA44,
    /// <summary>
    /// The save arrow right20.
    /// </summary>
    SaveArrowRight20 = 0xEA45,
    /// <summary>
    /// The save arrow right24.
    /// </summary>
    SaveArrowRight24 = 0xEA46,
    /// <summary>
    /// The save copy20.
    /// </summary>
    SaveCopy20 = 0xEA47,
    /// <summary>
    /// The save edit20.
    /// </summary>
    SaveEdit20 = 0xEA48,
    /// <summary>
    /// The save edit24.
    /// </summary>
    SaveEdit24 = 0xEA49,
    /// <summary>
    /// The save image20.
    /// </summary>
    SaveImage20 = 0xEA4A,
    /// <summary>
    /// The save multiple20.
    /// </summary>
    SaveMultiple20 = 0xEA4B,
    /// <summary>
    /// The save multiple24.
    /// </summary>
    SaveMultiple24 = 0xEA4C,
    /// <summary>
    /// The save search20.
    /// </summary>
    SaveSearch20 = 0xEA4D,
    /// <summary>
    /// The save sync20.
    /// </summary>
    SaveSync20 = 0xEA4E,
    /// <summary>
    /// The scale fill20.
    /// </summary>
    ScaleFill20 = 0xEA4F,
    /// <summary>
    /// The scales20.
    /// </summary>
    Scales20 = 0xEA50,
    /// <summary>
    /// The scales24.
    /// </summary>
    Scales24 = 0xEA51,
    /// <summary>
    /// The scales32.
    /// </summary>
    Scales32 = 0xEA52,
    /// <summary>
    /// The scan16.
    /// </summary>
    Scan16 = 0xEA53,
    /// <summary>
    /// The scan20.
    /// </summary>
    Scan20 = 0xEA54,
    /// <summary>
    /// The scan camera16.
    /// </summary>
    ScanCamera16 = 0xEA55,
    /// <summary>
    /// The scan camera20.
    /// </summary>
    ScanCamera20 = 0xEA56,
    /// <summary>
    /// The scan camera24.
    /// </summary>
    ScanCamera24 = 0xEA57,
    /// <summary>
    /// The scan camera28.
    /// </summary>
    ScanCamera28 = 0xEA58,
    /// <summary>
    /// The scan camera48.
    /// </summary>
    ScanCamera48 = 0xEA59,
    /// <summary>
    /// The scan dash12.
    /// </summary>
    ScanDash12 = 0xEA5A,
    /// <summary>
    /// The scan dash16.
    /// </summary>
    ScanDash16 = 0xEA5B,
    /// <summary>
    /// The scan dash20.
    /// </summary>
    ScanDash20 = 0xEA5C,
    /// <summary>
    /// The scan dash24.
    /// </summary>
    ScanDash24 = 0xEA5D,
    /// <summary>
    /// The scan dash28.
    /// </summary>
    ScanDash28 = 0xEA5E,
    /// <summary>
    /// The scan dash32.
    /// </summary>
    ScanDash32 = 0xEA5F,
    /// <summary>
    /// The scan dash48.
    /// </summary>
    ScanDash48 = 0xEA60,
    /// <summary>
    /// The scan object20.
    /// </summary>
    ScanObject20 = 0xEA61,
    /// <summary>
    /// The scan object24.
    /// </summary>
    ScanObject24 = 0xEA62,
    /// <summary>
    /// The scan table20.
    /// </summary>
    ScanTable20 = 0xEA63,
    /// <summary>
    /// The scan table24.
    /// </summary>
    ScanTable24 = 0xEA64,
    /// <summary>
    /// The scan text20.
    /// </summary>
    ScanText20 = 0xEA65,
    /// <summary>
    /// The scan text24.
    /// </summary>
    ScanText24 = 0xEA66,
    /// <summary>
    /// The scan thumb up16.
    /// </summary>
    ScanThumbUp16 = 0xEA67,
    /// <summary>
    /// The scan thumb up20.
    /// </summary>
    ScanThumbUp20 = 0xEA68,
    /// <summary>
    /// The scan thumb up24.
    /// </summary>
    ScanThumbUp24 = 0xEA69,
    /// <summary>
    /// The scan thumb up28.
    /// </summary>
    ScanThumbUp28 = 0xEA6A,
    /// <summary>
    /// The scan thumb up48.
    /// </summary>
    ScanThumbUp48 = 0xEA6B,
    /// <summary>
    /// The scan thumb up off16.
    /// </summary>
    ScanThumbUpOff16 = 0xEA6C,
    /// <summary>
    /// The scan thumb up off20.
    /// </summary>
    ScanThumbUpOff20 = 0xEA6D,
    /// <summary>
    /// The scan thumb up off24.
    /// </summary>
    ScanThumbUpOff24 = 0xEA6E,
    /// <summary>
    /// The scan thumb up off28.
    /// </summary>
    ScanThumbUpOff28 = 0xEA6F,
    /// <summary>
    /// The scan thumb up off48.
    /// </summary>
    ScanThumbUpOff48 = 0xEA70,
    /// <summary>
    /// The scan type20.
    /// </summary>
    ScanType20 = 0xEA71,
    /// <summary>
    /// The scan type24.
    /// </summary>
    ScanType24 = 0xEA72,
    /// <summary>
    /// The scan type checkmark20.
    /// </summary>
    ScanTypeCheckmark20 = 0xEA73,
    /// <summary>
    /// The scan type checkmark24.
    /// </summary>
    ScanTypeCheckmark24 = 0xEA74,
    /// <summary>
    /// The scan type off20.
    /// </summary>
    ScanTypeOff20 = 0xEA75,
    /// <summary>
    /// The scratchpad20.
    /// </summary>
    Scratchpad20 = 0xEA76,
    /// <summary>
    /// The screen cut20.
    /// </summary>
    ScreenCut20 = 0xEA77,
    /// <summary>
    /// The screen person20.
    /// </summary>
    ScreenPerson20 = 0xEA78,
    /// <summary>
    /// The screen search20.
    /// </summary>
    ScreenSearch20 = 0xEA79,
    /// <summary>
    /// The screen search24.
    /// </summary>
    ScreenSearch24 = 0xEA7A,
    /// <summary>
    /// The search12.
    /// </summary>
    Search12 = 0xEA7B,
    /// <summary>
    /// The search16.
    /// </summary>
    Search16 = 0xEA7C,
    /// <summary>
    /// The search32.
    /// </summary>
    Search32 = 0xEA7D,
    /// <summary>
    /// The search48.
    /// </summary>
    Search48 = 0xEA7E,
    /// <summary>
    /// The search settings20.
    /// </summary>
    SearchSettings20 = 0xEA7F,
    /// <summary>
    /// The search shield20.
    /// </summary>
    SearchShield20 = 0xEA80,
    /// <summary>
    /// The search square20.
    /// </summary>
    SearchSquare20 = 0xEA81,
    /// <summary>
    /// The search visual16.
    /// </summary>
    SearchVisual16 = 0xEA82,
    /// <summary>
    /// The search visual20.
    /// </summary>
    SearchVisual20 = 0xEA83,
    /// <summary>
    /// The search visual24.
    /// </summary>
    SearchVisual24 = 0xEA84,
    /// <summary>
    /// The select all off20.
    /// </summary>
    SelectAllOff20 = 0xEA85,
    /// <summary>
    /// The select all on20.
    /// </summary>
    SelectAllOn20 = 0xEA86,
    /// <summary>
    /// The select all on24.
    /// </summary>
    SelectAllOn24 = 0xEA87,
    /// <summary>
    /// The select object skew20.
    /// </summary>
    SelectObjectSkew20 = 0xEA88,
    /// <summary>
    /// The select object skew24.
    /// </summary>
    SelectObjectSkew24 = 0xEA89,
    /// <summary>
    /// The select object skew dismiss20.
    /// </summary>
    SelectObjectSkewDismiss20 = 0xEA8A,
    /// <summary>
    /// The select object skew dismiss24.
    /// </summary>
    SelectObjectSkewDismiss24 = 0xEA8B,
    /// <summary>
    /// The select object skew edit20.
    /// </summary>
    SelectObjectSkewEdit20 = 0xEA8C,
    /// <summary>
    /// The select object skew edit24.
    /// </summary>
    SelectObjectSkewEdit24 = 0xEA8D,
    /// <summary>
    /// The send16.
    /// </summary>
    Send16 = 0xEA8E,
    /// <summary>
    /// The send clock24.
    /// </summary>
    SendClock24 = 0xEA8F,
    /// <summary>
    /// The send copy20.
    /// </summary>
    SendCopy20 = 0xEA90,
    /// <summary>
    /// The server multiple20.
    /// </summary>
    ServerMultiple20 = 0xEA91,
    /// <summary>
    /// The server play20.
    /// </summary>
    ServerPlay20 = 0xEA92,
    /// <summary>
    /// The service bell20.
    /// </summary>
    ServiceBell20 = 0xEA93,
    /// <summary>
    /// The settings32.
    /// </summary>
    Settings32 = 0xEA94,
    /// <summary>
    /// The settings48.
    /// </summary>
    Settings48 = 0xEA95,
    /// <summary>
    /// The settings chat20.
    /// </summary>
    SettingsChat20 = 0xEA96,
    /// <summary>
    /// The settings chat24.
    /// </summary>
    SettingsChat24 = 0xEA97,
    /// <summary>
    /// The shape exclude16.
    /// </summary>
    ShapeExclude16 = 0xEA98,
    /// <summary>
    /// The shape exclude20.
    /// </summary>
    ShapeExclude20 = 0xEA99,
    /// <summary>
    /// The shape exclude24.
    /// </summary>
    ShapeExclude24 = 0xEA9A,
    /// <summary>
    /// The shape intersect16.
    /// </summary>
    ShapeIntersect16 = 0xEA9B,
    /// <summary>
    /// The shape intersect20.
    /// </summary>
    ShapeIntersect20 = 0xEA9C,
    /// <summary>
    /// The shape intersect24.
    /// </summary>
    ShapeIntersect24 = 0xEA9D,
    /// <summary>
    /// The shape subtract16.
    /// </summary>
    ShapeSubtract16 = 0xEA9E,
    /// <summary>
    /// The shape subtract20.
    /// </summary>
    ShapeSubtract20 = 0xEA9F,
    /// <summary>
    /// The shape subtract24.
    /// </summary>
    ShapeSubtract24 = 0xEAA0,
    /// <summary>
    /// The shape union16.
    /// </summary>
    ShapeUnion16 = 0xEAA1,
    /// <summary>
    /// The shape union20.
    /// </summary>
    ShapeUnion20 = 0xEAA2,
    /// <summary>
    /// The shape union24.
    /// </summary>
    ShapeUnion24 = 0xEAA3,
    /// <summary>
    /// The shapes28.
    /// </summary>
    Shapes28 = 0xEAA4,
    /// <summary>
    /// The shapes48.
    /// </summary>
    Shapes48 = 0xEAA5,
    /// <summary>
    /// The share16.
    /// </summary>
    Share16 = 0xEAA6,
    /// <summary>
    /// The share28.
    /// </summary>
    Share28 = 0xEAA7,
    /// <summary>
    /// The share48.
    /// </summary>
    Share48 = 0xEAA8,
    /// <summary>
    /// The share close tray20.
    /// </summary>
    ShareCloseTray20 = 0xEAA9,
    /// <summary>
    /// The share screen person16.
    /// </summary>
    ShareScreenPerson16 = 0xEAAA,
    /// <summary>
    /// The share screen person20.
    /// </summary>
    ShareScreenPerson20 = 0xEAAB,
    /// <summary>
    /// The share screen person24.
    /// </summary>
    ShareScreenPerson24 = 0xEAAC,
    /// <summary>
    /// The share screen person28.
    /// </summary>
    ShareScreenPerson28 = 0xEAAD,
    /// <summary>
    /// The share screen person overlay16.
    /// </summary>
    ShareScreenPersonOverlay16 = 0xEAAE,
    /// <summary>
    /// The share screen person overlay20.
    /// </summary>
    ShareScreenPersonOverlay20 = 0xEAAF,
    /// <summary>
    /// The share screen person overlay24.
    /// </summary>
    ShareScreenPersonOverlay24 = 0xEAB0,
    /// <summary>
    /// The share screen person overlay28.
    /// </summary>
    ShareScreenPersonOverlay28 = 0xEAB1,
    /// <summary>
    /// The share screen person overlay inside16.
    /// </summary>
    ShareScreenPersonOverlayInside16 = 0xEAB2,
    /// <summary>
    /// The share screen person overlay inside20.
    /// </summary>
    ShareScreenPersonOverlayInside20 = 0xEAB3,
    /// <summary>
    /// The share screen person overlay inside24.
    /// </summary>
    ShareScreenPersonOverlayInside24 = 0xEAB4,
    /// <summary>
    /// The share screen person overlay inside28.
    /// </summary>
    ShareScreenPersonOverlayInside28 = 0xEAB5,
    /// <summary>
    /// The share screen person P16.
    /// </summary>
    ShareScreenPersonP16 = 0xEAB6,
    /// <summary>
    /// The share screen person P20.
    /// </summary>
    ShareScreenPersonP20 = 0xEAB7,
    /// <summary>
    /// The share screen person P24.
    /// </summary>
    ShareScreenPersonP24 = 0xEAB8,
    /// <summary>
    /// The share screen person P28.
    /// </summary>
    ShareScreenPersonP28 = 0xEAB9,
    /// <summary>
    /// The share screen start20.
    /// </summary>
    ShareScreenStart20 = 0xEABA,
    /// <summary>
    /// The share screen start24.
    /// </summary>
    ShareScreenStart24 = 0xEABB,
    /// <summary>
    /// The share screen start28.
    /// </summary>
    ShareScreenStart28 = 0xEABC,
    /// <summary>
    /// The share screen start48.
    /// </summary>
    ShareScreenStart48 = 0xEABD,
    /// <summary>
    /// The share screen stop16.
    /// </summary>
    ShareScreenStop16 = 0xEABE,
    /// <summary>
    /// The share screen stop20.
    /// </summary>
    ShareScreenStop20 = 0xEABF,
    /// <summary>
    /// The share screen stop24.
    /// </summary>
    ShareScreenStop24 = 0xEAC0,
    /// <summary>
    /// The share screen stop28.
    /// </summary>
    ShareScreenStop28 = 0xEAC1,
    /// <summary>
    /// The share screen stop48.
    /// </summary>
    ShareScreenStop48 = 0xEAC2,
    /// <summary>
    /// The shield16.
    /// </summary>
    Shield16 = 0xEAC3,
    /// <summary>
    /// The shield28.
    /// </summary>
    Shield28 = 0xEAC4,
    /// <summary>
    /// The shield48.
    /// </summary>
    Shield48 = 0xEAC5,
    /// <summary>
    /// The shield badge24.
    /// </summary>
    ShieldBadge24 = 0xEAC6,
    /// <summary>
    /// The shield checkmark16.
    /// </summary>
    ShieldCheckmark16 = 0xEAC7,
    /// <summary>
    /// The shield checkmark20.
    /// </summary>
    ShieldCheckmark20 = 0xEAC8,
    /// <summary>
    /// The shield checkmark24.
    /// </summary>
    ShieldCheckmark24 = 0xEAC9,
    /// <summary>
    /// The shield checkmark28.
    /// </summary>
    ShieldCheckmark28 = 0xEACA,
    /// <summary>
    /// The shield checkmark48.
    /// </summary>
    ShieldCheckmark48 = 0xEACB,
    /// <summary>
    /// The shield dismiss16.
    /// </summary>
    ShieldDismiss16 = 0xEACC,
    /// <summary>
    /// The shield dismiss shield20.
    /// </summary>
    ShieldDismissShield20 = 0xEACD,
    /// <summary>
    /// The shield error16.
    /// </summary>
    ShieldError16 = 0xEACE,
    /// <summary>
    /// The shield lock16.
    /// </summary>
    ShieldLock16 = 0xEACF,
    /// <summary>
    /// The shield lock20.
    /// </summary>
    ShieldLock20 = 0xEAD0,
    /// <summary>
    /// The shield lock24.
    /// </summary>
    ShieldLock24 = 0xEAD1,
    /// <summary>
    /// The shield lock28.
    /// </summary>
    ShieldLock28 = 0xEAD2,
    /// <summary>
    /// The shield lock48.
    /// </summary>
    ShieldLock48 = 0xEAD3,
    /// <summary>
    /// The shield person20.
    /// </summary>
    ShieldPerson20 = 0xEAD4,
    /// <summary>
    /// The shield person add20.
    /// </summary>
    ShieldPersonAdd20 = 0xEAD5,
    /// <summary>
    /// The shield task16.
    /// </summary>
    ShieldTask16 = 0xEAD6,
    /// <summary>
    /// The shield task20.
    /// </summary>
    ShieldTask20 = 0xEAD7,
    /// <summary>
    /// The shield task24.
    /// </summary>
    ShieldTask24 = 0xEAD8,
    /// <summary>
    /// The shield task28.
    /// </summary>
    ShieldTask28 = 0xEAD9,
    /// <summary>
    /// The shield task48.
    /// </summary>
    ShieldTask48 = 0xEADA,
    /// <summary>
    /// The shifts16.
    /// </summary>
    Shifts16 = 0xEADB,
    /// <summary>
    /// The shifts20.
    /// </summary>
    Shifts20 = 0xEADC,
    /// <summary>
    /// The shifts30 minutes20.
    /// </summary>
    Shifts30Minutes20 = 0xEADD,
    /// <summary>
    /// The shifts32.
    /// </summary>
    Shifts32 = 0xEADE,
    /// <summary>
    /// The shifts add20.
    /// </summary>
    ShiftsAdd20 = 0xEADF,
    /// <summary>
    /// The shifts availability20.
    /// </summary>
    ShiftsAvailability20 = 0xEAE0,
    /// <summary>
    /// The shifts checkmark20.
    /// </summary>
    ShiftsCheckmark20 = 0xEAE1,
    /// <summary>
    /// The shifts checkmark24.
    /// </summary>
    ShiftsCheckmark24 = 0xEAE2,
    /// <summary>
    /// The shifts day20.
    /// </summary>
    ShiftsDay20 = 0xEAE3,
    /// <summary>
    /// The shifts day24.
    /// </summary>
    ShiftsDay24 = 0xEAE4,
    /// <summary>
    /// The shifts prohibited20.
    /// </summary>
    ShiftsProhibited20 = 0xEAE5,
    /// <summary>
    /// The shifts prohibited24.
    /// </summary>
    ShiftsProhibited24 = 0xEAE6,
    /// <summary>
    /// The shifts question mark20.
    /// </summary>
    ShiftsQuestionMark20 = 0xEAE7,
    /// <summary>
    /// The shifts question mark24.
    /// </summary>
    ShiftsQuestionMark24 = 0xEAE8,
    /// <summary>
    /// The shifts team20.
    /// </summary>
    ShiftsTeam20 = 0xEAE9,
    /// <summary>
    /// The shopping bag arrow left20.
    /// </summary>
    ShoppingBagArrowLeft20 = 0xEAEA,
    /// <summary>
    /// The shopping bag arrow left24.
    /// </summary>
    ShoppingBagArrowLeft24 = 0xEAEB,
    /// <summary>
    /// The shopping bag dismiss20.
    /// </summary>
    ShoppingBagDismiss20 = 0xEAEC,
    /// <summary>
    /// The shopping bag dismiss24.
    /// </summary>
    ShoppingBagDismiss24 = 0xEAED,
    /// <summary>
    /// The shopping bag pause20.
    /// </summary>
    ShoppingBagPause20 = 0xEAEE,
    /// <summary>
    /// The shopping bag pause24.
    /// </summary>
    ShoppingBagPause24 = 0xEAEF,
    /// <summary>
    /// The shopping bag percent20.
    /// </summary>
    ShoppingBagPercent20 = 0xEAF0,
    /// <summary>
    /// The shopping bag percent24.
    /// </summary>
    ShoppingBagPercent24 = 0xEAF1,
    /// <summary>
    /// The shopping bag play20.
    /// </summary>
    ShoppingBagPlay20 = 0xEAF2,
    /// <summary>
    /// The shopping bag play24.
    /// </summary>
    ShoppingBagPlay24 = 0xEAF3,
    /// <summary>
    /// The shopping bag tag20.
    /// </summary>
    ShoppingBagTag20 = 0xEAF4,
    /// <summary>
    /// The shopping bag tag24.
    /// </summary>
    ShoppingBagTag24 = 0xEAF5,
    /// <summary>
    /// The shortpick20.
    /// </summary>
    Shortpick20 = 0xEAF6,
    /// <summary>
    /// The shortpick24.
    /// </summary>
    Shortpick24 = 0xEAF7,
    /// <summary>
    /// The sidebar search LTR20.
    /// </summary>
    SidebarSearchLtr20 = 0xEAF8,
    /// <summary>
    /// The sidebar search RTL20.
    /// </summary>
    SidebarSearchRtl20 = 0xEAF9,
    /// <summary>
    /// The sign out20.
    /// </summary>
    SignOut20 = 0xEAFA,
    /// <summary>
    /// The skip back1020.
    /// </summary>
    SkipBack1020 = 0xEAFB,
    /// <summary>
    /// The skip back1024.
    /// </summary>
    SkipBack1024 = 0xEAFC,
    /// <summary>
    /// The skip back1028.
    /// </summary>
    SkipBack1028 = 0xEAFD,
    /// <summary>
    /// The skip back1032.
    /// </summary>
    SkipBack1032 = 0xEAFE,
    /// <summary>
    /// The skip back1048.
    /// </summary>
    SkipBack1048 = 0xEAFF,
    /// <summary>
    /// The skip forward1020.
    /// </summary>
    SkipForward1020 = 0xEB00,
    /// <summary>
    /// The skip forward1024.
    /// </summary>
    SkipForward1024 = 0xEB01,
    /// <summary>
    /// The skip forward1028.
    /// </summary>
    SkipForward1028 = 0xEB02,
    /// <summary>
    /// The skip forward1032.
    /// </summary>
    SkipForward1032 = 0xEB03,
    /// <summary>
    /// The skip forward1048.
    /// </summary>
    SkipForward1048 = 0xEB04,
    /// <summary>
    /// The skip forward3020.
    /// </summary>
    SkipForward3020 = 0xEB05,
    /// <summary>
    /// The skip forward3024.
    /// </summary>
    SkipForward3024 = 0xEB06,
    /// <summary>
    /// The skip forward3028.
    /// </summary>
    SkipForward3028 = 0xEB07,
    /// <summary>
    /// The skip forward3032.
    /// </summary>
    SkipForward3032 = 0xEB08,
    /// <summary>
    /// The skip forward3048.
    /// </summary>
    SkipForward3048 = 0xEB09,
    /// <summary>
    /// The skip forward tab20.
    /// </summary>
    SkipForwardTab20 = 0xEB0A,
    /// <summary>
    /// The skip forward tab24.
    /// </summary>
    SkipForwardTab24 = 0xEB0B,
    /// <summary>
    /// The sleep20.
    /// </summary>
    Sleep20 = 0xEB0C,
    /// <summary>
    /// The slide add16.
    /// </summary>
    SlideAdd16 = 0xEB0D,
    /// <summary>
    /// The slide add20.
    /// </summary>
    SlideAdd20 = 0xEB0E,
    /// <summary>
    /// The slide add28.
    /// </summary>
    SlideAdd28 = 0xEB0F,
    /// <summary>
    /// The slide add32.
    /// </summary>
    SlideAdd32 = 0xEB10,
    /// <summary>
    /// The slide add48.
    /// </summary>
    SlideAdd48 = 0xEB11,
    /// <summary>
    /// The slide arrow right20.
    /// </summary>
    SlideArrowRight20 = 0xEB12,
    /// <summary>
    /// The slide arrow right24.
    /// </summary>
    SlideArrowRight24 = 0xEB13,
    /// <summary>
    /// The slide eraser16.
    /// </summary>
    SlideEraser16 = 0xEB14,
    /// <summary>
    /// The slide eraser20.
    /// </summary>
    SlideEraser20 = 0xEB15,
    /// <summary>
    /// The slide eraser24.
    /// </summary>
    SlideEraser24 = 0xEB16,
    /// <summary>
    /// The slide grid20.
    /// </summary>
    SlideGrid20 = 0xEB17,
    /// <summary>
    /// The slide grid24.
    /// </summary>
    SlideGrid24 = 0xEB18,
    /// <summary>
    /// The slide hide20.
    /// </summary>
    SlideHide20 = 0xEB19,
    /// <summary>
    /// The slide microphone20.
    /// </summary>
    SlideMicrophone20 = 0xEB1A,
    /// <summary>
    /// The slide microphone32.
    /// </summary>
    SlideMicrophone32 = 0xEB1B,
    /// <summary>
    /// The slide multiple20.
    /// </summary>
    SlideMultiple20 = 0xEB1C,
    /// <summary>
    /// The slide multiple24.
    /// </summary>
    SlideMultiple24 = 0xEB1D,
    /// <summary>
    /// The slide multiple arrow right20.
    /// </summary>
    SlideMultipleArrowRight20 = 0xEB1E,
    /// <summary>
    /// The slide multiple arrow right24.
    /// </summary>
    SlideMultipleArrowRight24 = 0xEB1F,
    /// <summary>
    /// The slide search20.
    /// </summary>
    SlideSearch20 = 0xEB20,
    /// <summary>
    /// The slide search24.
    /// </summary>
    SlideSearch24 = 0xEB21,
    /// <summary>
    /// The slide search28.
    /// </summary>
    SlideSearch28 = 0xEB22,
    /// <summary>
    /// The slide settings20.
    /// </summary>
    SlideSettings20 = 0xEB23,
    /// <summary>
    /// The slide settings24.
    /// </summary>
    SlideSettings24 = 0xEB24,
    /// <summary>
    /// The slide size20.
    /// </summary>
    SlideSize20 = 0xEB25,
    /// <summary>
    /// The slide size24.
    /// </summary>
    SlideSize24 = 0xEB26,
    /// <summary>
    /// The slide text16.
    /// </summary>
    SlideText16 = 0xEB27,
    /// <summary>
    /// The slide text20.
    /// </summary>
    SlideText20 = 0xEB28,
    /// <summary>
    /// The slide text28.
    /// </summary>
    SlideText28 = 0xEB29,
    /// <summary>
    /// The slide text48.
    /// </summary>
    SlideText48 = 0xEB2A,
    /// <summary>
    /// The slide transition20.
    /// </summary>
    SlideTransition20 = 0xEB2B,
    /// <summary>
    /// The slide transition24.
    /// </summary>
    SlideTransition24 = 0xEB2C,
    /// <summary>
    /// The snooze20.
    /// </summary>
    Snooze20 = 0xEB2D,
    /// <summary>
    /// The sound source20.
    /// </summary>
    SoundSource20 = 0xEB2E,
    /// <summary>
    /// The sound wave circle20.
    /// </summary>
    SoundWaveCircle20 = 0xEB2F,
    /// <summary>
    /// The sound wave circle24.
    /// </summary>
    SoundWaveCircle24 = 0xEB30,
    /// <summary>
    /// The spacebar20.
    /// </summary>
    Spacebar20 = 0xEB31,
    /// <summary>
    /// The sparkle16.
    /// </summary>
    Sparkle16 = 0xEB32,
    /// <summary>
    /// The sparkle20.
    /// </summary>
    Sparkle20 = 0xEB33,
    /// <summary>
    /// The sparkle24.
    /// </summary>
    Sparkle24 = 0xEB34,
    /// <summary>
    /// The sparkle28.
    /// </summary>
    Sparkle28 = 0xEB35,
    /// <summary>
    /// The sparkle48.
    /// </summary>
    Sparkle48 = 0xEB36,
    /// <summary>
    /// The speaker016.
    /// </summary>
    Speaker016 = 0xEB37,
    /// <summary>
    /// The speaker020.
    /// </summary>
    Speaker020 = 0xEB38,
    /// <summary>
    /// The speaker028.
    /// </summary>
    Speaker028 = 0xEB39,
    /// <summary>
    /// The speaker032.
    /// </summary>
    Speaker032 = 0xEB3A,
    /// <summary>
    /// The speaker048.
    /// </summary>
    Speaker048 = 0xEB3B,
    /// <summary>
    /// The speaker116.
    /// </summary>
    Speaker116 = 0xEB3C,
    /// <summary>
    /// The speaker120.
    /// </summary>
    Speaker120 = 0xEB3D,
    /// <summary>
    /// The speaker128.
    /// </summary>
    Speaker128 = 0xEB3E,
    /// <summary>
    /// The speaker132.
    /// </summary>
    Speaker132 = 0xEB3F,
    /// <summary>
    /// The speaker148.
    /// </summary>
    Speaker148 = 0xEB40,
    /// <summary>
    /// The speaker216.
    /// </summary>
    Speaker216 = 0xEB41,
    /// <summary>
    /// The speaker220.
    /// </summary>
    Speaker220 = 0xEB42,
    /// <summary>
    /// The speaker224.
    /// </summary>
    Speaker224 = 0xEB43,
    /// <summary>
    /// The speaker228.
    /// </summary>
    Speaker228 = 0xEB44,
    /// <summary>
    /// The speaker232.
    /// </summary>
    Speaker232 = 0xEB45,
    /// <summary>
    /// The speaker248.
    /// </summary>
    Speaker248 = 0xEB46,
    /// <summary>
    /// The speaker bluetooth20.
    /// </summary>
    SpeakerBluetooth20 = 0xEB47,
    /// <summary>
    /// The speaker bluetooth28.
    /// </summary>
    SpeakerBluetooth28 = 0xEB48,
    /// <summary>
    /// The speaker mute16.
    /// </summary>
    SpeakerMute16 = 0xEB49,
    /// <summary>
    /// The speaker mute20.
    /// </summary>
    SpeakerMute20 = 0xEB4A,
    /// <summary>
    /// The speaker mute24.
    /// </summary>
    SpeakerMute24 = 0xEB4B,
    /// <summary>
    /// The speaker mute28.
    /// </summary>
    SpeakerMute28 = 0xEB4C,
    /// <summary>
    /// The speaker mute48.
    /// </summary>
    SpeakerMute48 = 0xEB4D,
    /// <summary>
    /// The speaker off16.
    /// </summary>
    SpeakerOff16 = 0xEB4E,
    /// <summary>
    /// The speaker off20.
    /// </summary>
    SpeakerOff20 = 0xEB4F,
    /// <summary>
    /// The speaker off48.
    /// </summary>
    SpeakerOff48 = 0xEB50,
    /// <summary>
    /// The speaker settings20.
    /// </summary>
    SpeakerSettings20 = 0xEB51,
    /// <summary>
    /// The speaker settings28.
    /// </summary>
    SpeakerSettings28 = 0xEB52,
    /// <summary>
    /// The speaker usb20.
    /// </summary>
    SpeakerUsb20 = 0xEB53,
    /// <summary>
    /// The speaker usb24.
    /// </summary>
    SpeakerUsb24 = 0xEB54,
    /// <summary>
    /// The speaker usb28.
    /// </summary>
    SpeakerUsb28 = 0xEB55,
    /// <summary>
    /// The split hint20.
    /// </summary>
    SplitHint20 = 0xEB56,
    /// <summary>
    /// The split horizontal12.
    /// </summary>
    SplitHorizontal12 = 0xEB57,
    /// <summary>
    /// The split horizontal16.
    /// </summary>
    SplitHorizontal16 = 0xEB58,
    /// <summary>
    /// The split horizontal20.
    /// </summary>
    SplitHorizontal20 = 0xEB59,
    /// <summary>
    /// The split horizontal24.
    /// </summary>
    SplitHorizontal24 = 0xEB5A,
    /// <summary>
    /// The split horizontal28.
    /// </summary>
    SplitHorizontal28 = 0xEB5B,
    /// <summary>
    /// The split horizontal32.
    /// </summary>
    SplitHorizontal32 = 0xEB5C,
    /// <summary>
    /// The split horizontal48.
    /// </summary>
    SplitHorizontal48 = 0xEB5D,
    /// <summary>
    /// The split vertical12.
    /// </summary>
    SplitVertical12 = 0xEB5E,
    /// <summary>
    /// The split vertical16.
    /// </summary>
    SplitVertical16 = 0xEB5F,
    /// <summary>
    /// The split vertical20.
    /// </summary>
    SplitVertical20 = 0xEB60,
    /// <summary>
    /// The split vertical24.
    /// </summary>
    SplitVertical24 = 0xEB61,
    /// <summary>
    /// The split vertical28.
    /// </summary>
    SplitVertical28 = 0xEB62,
    /// <summary>
    /// The split vertical32.
    /// </summary>
    SplitVertical32 = 0xEB63,
    /// <summary>
    /// The split vertical48.
    /// </summary>
    SplitVertical48 = 0xEB64,
    /// <summary>
    /// The sport16.
    /// </summary>
    Sport16 = 0xEB65,
    /// <summary>
    /// The sport20.
    /// </summary>
    Sport20 = 0xEB66,
    /// <summary>
    /// The sport24.
    /// </summary>
    Sport24 = 0xEB67,
    /// <summary>
    /// The sport american football20.
    /// </summary>
    SportAmericanFootball20 = 0xEB68,
    /// <summary>
    /// The sport american football24.
    /// </summary>
    SportAmericanFootball24 = 0xEB69,
    /// <summary>
    /// The sport baseball20.
    /// </summary>
    SportBaseball20 = 0xEB6A,
    /// <summary>
    /// The sport baseball24.
    /// </summary>
    SportBaseball24 = 0xEB6B,
    /// <summary>
    /// The sport basketball20.
    /// </summary>
    SportBasketball20 = 0xEB6C,
    /// <summary>
    /// The sport basketball24.
    /// </summary>
    SportBasketball24 = 0xEB6D,
    /// <summary>
    /// The sport hockey20.
    /// </summary>
    SportHockey20 = 0xEB6E,
    /// <summary>
    /// The sport hockey24.
    /// </summary>
    SportHockey24 = 0xEB6F,
    /// <summary>
    /// The sport soccer16.
    /// </summary>
    SportSoccer16 = 0xEB70,
    /// <summary>
    /// The sport soccer20.
    /// </summary>
    SportSoccer20 = 0xEB71,
    /// <summary>
    /// The sport soccer24.
    /// </summary>
    SportSoccer24 = 0xEB72,
    /// <summary>
    /// The square12.
    /// </summary>
    Square12 = 0xEB73,
    /// <summary>
    /// The square16.
    /// </summary>
    Square16 = 0xEB74,
    /// <summary>
    /// The square20.
    /// </summary>
    Square20 = 0xEB75,
    /// <summary>
    /// The square24.
    /// </summary>
    Square24 = 0xEB76,
    /// <summary>
    /// The square28.
    /// </summary>
    Square28 = 0xEB77,
    /// <summary>
    /// The square32.
    /// </summary>
    Square32 = 0xEB78,
    /// <summary>
    /// The square48.
    /// </summary>
    Square48 = 0xEB79,
    /// <summary>
    /// The square add16.
    /// </summary>
    SquareAdd16 = 0xEB7A,
    /// <summary>
    /// The square add20.
    /// </summary>
    SquareAdd20 = 0xEB7B,
    /// <summary>
    /// The square arrow forward16.
    /// </summary>
    SquareArrowForward16 = 0xEB7C,
    /// <summary>
    /// The square arrow forward20.
    /// </summary>
    SquareArrowForward20 = 0xEB7D,
    /// <summary>
    /// The square arrow forward24.
    /// </summary>
    SquareArrowForward24 = 0xEB7E,
    /// <summary>
    /// The square arrow forward28.
    /// </summary>
    SquareArrowForward28 = 0xEB7F,
    /// <summary>
    /// The square arrow forward32.
    /// </summary>
    SquareArrowForward32 = 0xEB80,
    /// <summary>
    /// The square arrow forward48.
    /// </summary>
    SquareArrowForward48 = 0xEB81,
    /// <summary>
    /// The square dismiss16.
    /// </summary>
    SquareDismiss16 = 0xEB82,
    /// <summary>
    /// The square dismiss20.
    /// </summary>
    SquareDismiss20 = 0xEB83,
    /// <summary>
    /// The square eraser20.
    /// </summary>
    SquareEraser20 = 0xEB84,
    /// <summary>
    /// The square hint16.
    /// </summary>
    SquareHint16 = 0xEB85,
    /// <summary>
    /// The square hint20.
    /// </summary>
    SquareHint20 = 0xEB86,
    /// <summary>
    /// The square hint24.
    /// </summary>
    SquareHint24 = 0xEB87,
    /// <summary>
    /// The square hint28.
    /// </summary>
    SquareHint28 = 0xEB88,
    /// <summary>
    /// The square hint32.
    /// </summary>
    SquareHint32 = 0xEB89,
    /// <summary>
    /// The square hint48.
    /// </summary>
    SquareHint48 = 0xEB8A,
    /// <summary>
    /// The square hint apps20.
    /// </summary>
    SquareHintApps20 = 0xEB8B,
    /// <summary>
    /// The square hint apps24.
    /// </summary>
    SquareHintApps24 = 0xEB8C,
    /// <summary>
    /// The square hint arrow back16.
    /// </summary>
    SquareHintArrowBack16 = 0xEB8D,
    /// <summary>
    /// The square hint arrow back20.
    /// </summary>
    SquareHintArrowBack20 = 0xEB8E,
    /// <summary>
    /// The square hint sparkles16.
    /// </summary>
    SquareHintSparkles16 = 0xEB8F,
    /// <summary>
    /// The square hint sparkles20.
    /// </summary>
    SquareHintSparkles20 = 0xEB90,
    /// <summary>
    /// The square hint sparkles24.
    /// </summary>
    SquareHintSparkles24 = 0xEB91,
    /// <summary>
    /// The square hint sparkles28.
    /// </summary>
    SquareHintSparkles28 = 0xEB92,
    /// <summary>
    /// The square hint sparkles32.
    /// </summary>
    SquareHintSparkles32 = 0xEB93,
    /// <summary>
    /// The square hint sparkles48.
    /// </summary>
    SquareHintSparkles48 = 0xEB94,
    /// <summary>
    /// The square multiple16.
    /// </summary>
    SquareMultiple16 = 0xEB95,
    /// <summary>
    /// The square multiple20.
    /// </summary>
    SquareMultiple20 = 0xEB96,
    /// <summary>
    /// The square shadow12.
    /// </summary>
    SquareShadow12 = 0xEB97,
    /// <summary>
    /// The square shadow20.
    /// </summary>
    SquareShadow20 = 0xEB98,
    /// <summary>
    /// The squares nested20.
    /// </summary>
    SquaresNested20 = 0xEB99,
    /// <summary>
    /// The stack arrow forward20.
    /// </summary>
    StackArrowForward20 = 0xEB9A,
    /// <summary>
    /// The stack arrow forward24.
    /// </summary>
    StackArrowForward24 = 0xEB9B,
    /// <summary>
    /// The stack star16.
    /// </summary>
    StackStar16 = 0xEB9C,
    /// <summary>
    /// The stack star20.
    /// </summary>
    StackStar20 = 0xEB9D,
    /// <summary>
    /// The stack star24.
    /// </summary>
    StackStar24 = 0xEB9E,
    /// <summary>
    /// The star48.
    /// </summary>
    Star48 = 0xEB9F,
    /// <summary>
    /// The star add28.
    /// </summary>
    StarAdd28 = 0xEBA0,
    /// <summary>
    /// The star arrow right end20.
    /// </summary>
    StarArrowRightEnd20 = 0xEBA1,
    /// <summary>
    /// The star arrow right end24.
    /// </summary>
    StarArrowRightEnd24 = 0xEBA2,
    /// <summary>
    /// The star arrow right start20.
    /// </summary>
    StarArrowRightStart20 = 0xEBA3,
    /// <summary>
    /// The star dismiss16.
    /// </summary>
    StarDismiss16 = 0xEBA4,
    /// <summary>
    /// The star dismiss20.
    /// </summary>
    StarDismiss20 = 0xEBA5,
    /// <summary>
    /// The star dismiss24.
    /// </summary>
    StarDismiss24 = 0xEBA6,
    /// <summary>
    /// The star dismiss28.
    /// </summary>
    StarDismiss28 = 0xEBA7,
    /// <summary>
    /// The star edit20.
    /// </summary>
    StarEdit20 = 0xEBA8,
    /// <summary>
    /// The star edit24.
    /// </summary>
    StarEdit24 = 0xEBA9,
    /// <summary>
    /// The star emphasis20.
    /// </summary>
    StarEmphasis20 = 0xEBAA,
    /// <summary>
    /// The star emphasis32.
    /// </summary>
    StarEmphasis32 = 0xEBAB,
    /// <summary>
    /// The star half12.
    /// </summary>
    StarHalf12 = 0xEBAC,
    /// <summary>
    /// The star half16.
    /// </summary>
    StarHalf16 = 0xEBAD,
    /// <summary>
    /// The star half20.
    /// </summary>
    StarHalf20 = 0xEBAE,
    /// <summary>
    /// The star half24.
    /// </summary>
    StarHalf24 = 0xEBAF,
    /// <summary>
    /// The star half28.
    /// </summary>
    StarHalf28 = 0xEBB0,
    /// <summary>
    /// The star line horizontal316.
    /// </summary>
    StarLineHorizontal316 = 0xEBB1,
    /// <summary>
    /// The star line horizontal320.
    /// </summary>
    StarLineHorizontal320 = 0xEBB2,
    /// <summary>
    /// The star line horizontal324.
    /// </summary>
    StarLineHorizontal324 = 0xEBB3,
    /// <summary>
    /// The star one quarter12.
    /// </summary>
    StarOneQuarter12 = 0xEBB4,
    /// <summary>
    /// The star one quarter16.
    /// </summary>
    StarOneQuarter16 = 0xEBB5,
    /// <summary>
    /// The star one quarter20.
    /// </summary>
    StarOneQuarter20 = 0xEBB6,
    /// <summary>
    /// The star one quarter24.
    /// </summary>
    StarOneQuarter24 = 0xEBB7,
    /// <summary>
    /// The star one quarter28.
    /// </summary>
    StarOneQuarter28 = 0xEBB8,
    /// <summary>
    /// The star settings20.
    /// </summary>
    StarSettings20 = 0xEBB9,
    /// <summary>
    /// The star three quarter12.
    /// </summary>
    StarThreeQuarter12 = 0xEBBA,
    /// <summary>
    /// The star three quarter16.
    /// </summary>
    StarThreeQuarter16 = 0xEBBB,
    /// <summary>
    /// The star three quarter20.
    /// </summary>
    StarThreeQuarter20 = 0xEBBC,
    /// <summary>
    /// The star three quarter24.
    /// </summary>
    StarThreeQuarter24 = 0xEBBD,
    /// <summary>
    /// The star three quarter28.
    /// </summary>
    StarThreeQuarter28 = 0xEBBE,
    /// <summary>
    /// The steps20.
    /// </summary>
    Steps20 = 0xEBBF,
    /// <summary>
    /// The steps24.
    /// </summary>
    Steps24 = 0xEBC0,
    /// <summary>
    /// The sticker12.
    /// </summary>
    Sticker12 = 0xEBC1,
    /// <summary>
    /// The sticker add20.
    /// </summary>
    StickerAdd20 = 0xEBC2,
    /// <summary>
    /// The storage20.
    /// </summary>
    Storage20 = 0xEBC3,
    /// <summary>
    /// The stream20.
    /// </summary>
    Stream20 = 0xEBC4,
    /// <summary>
    /// The stream24.
    /// </summary>
    Stream24 = 0xEBC5,
    /// <summary>
    /// The stream input20.
    /// </summary>
    StreamInput20 = 0xEBC6,
    /// <summary>
    /// The stream input output20.
    /// </summary>
    StreamInputOutput20 = 0xEBC7,
    /// <summary>
    /// The stream output20.
    /// </summary>
    StreamOutput20 = 0xEBC8,
    /// <summary>
    /// The style guide20.
    /// </summary>
    StyleGuide20 = 0xEBC9,
    /// <summary>
    /// The sub grid20.
    /// </summary>
    SubGrid20 = 0xEBCA,
    /// <summary>
    /// The subtitles16.
    /// </summary>
    Subtitles16 = 0xEBCB,
    /// <summary>
    /// The subtitles20.
    /// </summary>
    Subtitles20 = 0xEBCC,
    /// <summary>
    /// The subtitles24.
    /// </summary>
    Subtitles24 = 0xEBCD,
    /// <summary>
    /// The subtract12.
    /// </summary>
    Subtract12 = 0xEBCE,
    /// <summary>
    /// The subtract16.
    /// </summary>
    Subtract16 = 0xEBCF,
    /// <summary>
    /// The subtract20.
    /// </summary>
    Subtract20 = 0xEBD0,
    /// <summary>
    /// The subtract24.
    /// </summary>
    Subtract24 = 0xEBD1,
    /// <summary>
    /// The subtract28.
    /// </summary>
    Subtract28 = 0xEBD2,
    /// <summary>
    /// The subtract48.
    /// </summary>
    Subtract48 = 0xEBD3,
    /// <summary>
    /// The subtract circle12.
    /// </summary>
    SubtractCircle12 = 0xEBD4,
    /// <summary>
    /// The subtract circle arrow back16.
    /// </summary>
    SubtractCircleArrowBack16 = 0xEBD5,
    /// <summary>
    /// The subtract circle arrow back20.
    /// </summary>
    SubtractCircleArrowBack20 = 0xEBD6,
    /// <summary>
    /// The subtract circle arrow forward16.
    /// </summary>
    SubtractCircleArrowForward16 = 0xEBD7,
    /// <summary>
    /// The subtract circle arrow forward20.
    /// </summary>
    SubtractCircleArrowForward20 = 0xEBD8,
    /// <summary>
    /// The subtract square20.
    /// </summary>
    SubtractSquare20 = 0xEBD9,
    /// <summary>
    /// The subtract square24.
    /// </summary>
    SubtractSquare24 = 0xEBDA,
    /// <summary>
    /// The subtract square multiple16.
    /// </summary>
    SubtractSquareMultiple16 = 0xEBDB,
    /// <summary>
    /// The subtract square multiple20.
    /// </summary>
    SubtractSquareMultiple20 = 0xEBDC,
    /// <summary>
    /// The swipe down20.
    /// </summary>
    SwipeDown20 = 0xEBDD,
    /// <summary>
    /// The swipe right20.
    /// </summary>
    SwipeRight20 = 0xEBDE,
    /// <summary>
    /// The swipe up20.
    /// </summary>
    SwipeUp20 = 0xEBDF,
    /// <summary>
    /// The symbols16.
    /// </summary>
    Symbols16 = 0xEBE0,
    /// <summary>
    /// The symbols20.
    /// </summary>
    Symbols20 = 0xEBE1,
    /// <summary>
    /// The syringe20.
    /// </summary>
    Syringe20 = 0xEBE2,
    /// <summary>
    /// The syringe24.
    /// </summary>
    Syringe24 = 0xEBE3,
    /// <summary>
    /// The system20.
    /// </summary>
    System20 = 0xEBE4,
    /// <summary>
    /// The tab add20.
    /// </summary>
    TabAdd20 = 0xEBE5,
    /// <summary>
    /// The tab add24.
    /// </summary>
    TabAdd24 = 0xEBE6,
    /// <summary>
    /// The tab arrow left20.
    /// </summary>
    TabArrowLeft20 = 0xEBE7,
    /// <summary>
    /// The tab arrow left24.
    /// </summary>
    TabArrowLeft24 = 0xEBE8,
    /// <summary>
    /// The tab desktop16.
    /// </summary>
    TabDesktop16 = 0xEBE9,
    /// <summary>
    /// The tab desktop24.
    /// </summary>
    TabDesktop24 = 0xEBEA,
    /// <summary>
    /// The tab desktop arrow left20.
    /// </summary>
    TabDesktopArrowLeft20 = 0xEBEB,
    /// <summary>
    /// The tab desktop bottom20.
    /// </summary>
    TabDesktopBottom20 = 0xEBEC,
    /// <summary>
    /// The tab desktop bottom24.
    /// </summary>
    TabDesktopBottom24 = 0xEBED,
    /// <summary>
    /// The tab desktop multiple bottom20.
    /// </summary>
    TabDesktopMultipleBottom20 = 0xEBEE,
    /// <summary>
    /// The tab desktop multiple bottom24.
    /// </summary>
    TabDesktopMultipleBottom24 = 0xEBEF,
    /// <summary>
    /// The tab prohibited20.
    /// </summary>
    TabProhibited20 = 0xEBF0,
    /// <summary>
    /// The tab prohibited24.
    /// </summary>
    TabProhibited24 = 0xEBF1,
    /// <summary>
    /// The tab shield dismiss20.
    /// </summary>
    TabShieldDismiss20 = 0xEBF2,
    /// <summary>
    /// The tab shield dismiss24.
    /// </summary>
    TabShieldDismiss24 = 0xEBF3,
    /// <summary>
    /// The table16.
    /// </summary>
    Table16 = 0xEBF4,
    /// <summary>
    /// The table28.
    /// </summary>
    Table28 = 0xEBF5,
    /// <summary>
    /// The table32.
    /// </summary>
    Table32 = 0xEBF6,
    /// <summary>
    /// The table48.
    /// </summary>
    Table48 = 0xEBF7,
    /// <summary>
    /// The table add16.
    /// </summary>
    TableAdd16 = 0xEBF8,
    /// <summary>
    /// The table add20.
    /// </summary>
    TableAdd20 = 0xEBF9,
    /// <summary>
    /// The table add28.
    /// </summary>
    TableAdd28 = 0xEBFA,
    /// <summary>
    /// The table bottom row16.
    /// </summary>
    TableBottomRow16 = 0xEBFB,
    /// <summary>
    /// The table bottom row20.
    /// </summary>
    TableBottomRow20 = 0xEBFC,
    /// <summary>
    /// The table bottom row24.
    /// </summary>
    TableBottomRow24 = 0xEBFD,
    /// <summary>
    /// The table bottom row28.
    /// </summary>
    TableBottomRow28 = 0xEBFE,
    /// <summary>
    /// The table bottom row32.
    /// </summary>
    TableBottomRow32 = 0xEBFF,
    /// <summary>
    /// The table bottom row48.
    /// </summary>
    TableBottomRow48 = 0xEC00,
    /// <summary>
    /// The table cell edit16.
    /// </summary>
    TableCellEdit16 = 0xEC01,
    /// <summary>
    /// The table cell edit20.
    /// </summary>
    TableCellEdit20 = 0xEC02,
    /// <summary>
    /// The table cell edit24.
    /// </summary>
    TableCellEdit24 = 0xEC03,
    /// <summary>
    /// The table cell edit28.
    /// </summary>
    TableCellEdit28 = 0xEC04,
    /// <summary>
    /// The table cells merge16.
    /// </summary>
    TableCellsMerge16 = 0xEC05,
    /// <summary>
    /// The table cells merge28.
    /// </summary>
    TableCellsMerge28 = 0xEC06,
    /// <summary>
    /// The table cells split16.
    /// </summary>
    TableCellsSplit16 = 0xEC07,
    /// <summary>
    /// The table cells split28.
    /// </summary>
    TableCellsSplit28 = 0xEC08,
    /// <summary>
    /// The table checker20.
    /// </summary>
    TableChecker20 = 0xEC09,
    /// <summary>
    /// The table copy20.
    /// </summary>
    TableCopy20 = 0xEC0A,
    /// <summary>
    /// The table delete column16.
    /// </summary>
    TableDeleteColumn16 = 0xEC0B,
    /// <summary>
    /// The table delete column20.
    /// </summary>
    TableDeleteColumn20 = 0xEC0C,
    /// <summary>
    /// The table delete column24.
    /// </summary>
    TableDeleteColumn24 = 0xEC0D,
    /// <summary>
    /// The table delete column28.
    /// </summary>
    TableDeleteColumn28 = 0xEC0E,
    /// <summary>
    /// The table delete row16.
    /// </summary>
    TableDeleteRow16 = 0xEC0F,
    /// <summary>
    /// The table delete row20.
    /// </summary>
    TableDeleteRow20 = 0xEC10,
    /// <summary>
    /// The table delete row24.
    /// </summary>
    TableDeleteRow24 = 0xEC11,
    /// <summary>
    /// The table delete row28.
    /// </summary>
    TableDeleteRow28 = 0xEC12,
    /// <summary>
    /// The table dismiss16.
    /// </summary>
    TableDismiss16 = 0xEC13,
    /// <summary>
    /// The table dismiss20.
    /// </summary>
    TableDismiss20 = 0xEC14,
    /// <summary>
    /// The table dismiss24.
    /// </summary>
    TableDismiss24 = 0xEC15,
    /// <summary>
    /// The table dismiss28.
    /// </summary>
    TableDismiss28 = 0xEC16,
    /// <summary>
    /// The table edit16.
    /// </summary>
    TableEdit16 = 0xEC17,
    /// <summary>
    /// The table edit20.
    /// </summary>
    TableEdit20 = 0xEC18,
    /// <summary>
    /// The table edit28.
    /// </summary>
    TableEdit28 = 0xEC19,
    /// <summary>
    /// The table freeze column16.
    /// </summary>
    TableFreezeColumn16 = 0xEC1A,
    /// <summary>
    /// The table freeze column20.
    /// </summary>
    TableFreezeColumn20 = 0xEC1B,
    /// <summary>
    /// The table freeze column28.
    /// </summary>
    TableFreezeColumn28 = 0xEC1C,
    /// <summary>
    /// The table freeze column and row16.
    /// </summary>
    TableFreezeColumnAndRow16 = 0xEC1D,
    /// <summary>
    /// The table freeze column and row20.
    /// </summary>
    TableFreezeColumnAndRow20 = 0xEC1E,
    /// <summary>
    /// The table freeze column and row24.
    /// </summary>
    TableFreezeColumnAndRow24 = 0xEC1F,
    /// <summary>
    /// The table freeze column and row28.
    /// </summary>
    TableFreezeColumnAndRow28 = 0xEC20,
    /// <summary>
    /// The table freeze row16.
    /// </summary>
    TableFreezeRow16 = 0xEC21,
    /// <summary>
    /// The table freeze row20.
    /// </summary>
    TableFreezeRow20 = 0xEC22,
    /// <summary>
    /// The table freeze row28.
    /// </summary>
    TableFreezeRow28 = 0xEC23,
    /// <summary>
    /// The table image20.
    /// </summary>
    TableImage20 = 0xEC24,
    /// <summary>
    /// The table insert column16.
    /// </summary>
    TableInsertColumn16 = 0xEC25,
    /// <summary>
    /// The table insert column20.
    /// </summary>
    TableInsertColumn20 = 0xEC26,
    /// <summary>
    /// The table insert column24.
    /// </summary>
    TableInsertColumn24 = 0xEC27,
    /// <summary>
    /// The table insert column28.
    /// </summary>
    TableInsertColumn28 = 0xEC28,
    /// <summary>
    /// The table insert row16.
    /// </summary>
    TableInsertRow16 = 0xEC29,
    /// <summary>
    /// The table insert row20.
    /// </summary>
    TableInsertRow20 = 0xEC2A,
    /// <summary>
    /// The table insert row24.
    /// </summary>
    TableInsertRow24 = 0xEC2B,
    /// <summary>
    /// The table insert row28.
    /// </summary>
    TableInsertRow28 = 0xEC2C,
    /// <summary>
    /// The table lightning16.
    /// </summary>
    TableLightning16 = 0xEC2D,
    /// <summary>
    /// The table lightning20.
    /// </summary>
    TableLightning20 = 0xEC2E,
    /// <summary>
    /// The table lightning24.
    /// </summary>
    TableLightning24 = 0xEC2F,
    /// <summary>
    /// The table lightning28.
    /// </summary>
    TableLightning28 = 0xEC30,
    /// <summary>
    /// The table link16.
    /// </summary>
    TableLink16 = 0xEC31,
    /// <summary>
    /// The table link20.
    /// </summary>
    TableLink20 = 0xEC32,
    /// <summary>
    /// The table link24.
    /// </summary>
    TableLink24 = 0xEC33,
    /// <summary>
    /// The table link28.
    /// </summary>
    TableLink28 = 0xEC34,
    /// <summary>
    /// The table move above16.
    /// </summary>
    TableMoveAbove16 = 0xEC35,
    /// <summary>
    /// The table move above20.
    /// </summary>
    TableMoveAbove20 = 0xEC36,
    /// <summary>
    /// The table move above24.
    /// </summary>
    TableMoveAbove24 = 0xEC37,
    /// <summary>
    /// The table move above28.
    /// </summary>
    TableMoveAbove28 = 0xEC38,
    /// <summary>
    /// The table move below16.
    /// </summary>
    TableMoveBelow16 = 0xEC39,
    /// <summary>
    /// The table move below20.
    /// </summary>
    TableMoveBelow20 = 0xEC3A,
    /// <summary>
    /// The table move below24.
    /// </summary>
    TableMoveBelow24 = 0xEC3B,
    /// <summary>
    /// The table move below28.
    /// </summary>
    TableMoveBelow28 = 0xEC3C,
    /// <summary>
    /// The table move left16.
    /// </summary>
    TableMoveLeft16 = 0xEC3D,
    /// <summary>
    /// The table move left20.
    /// </summary>
    TableMoveLeft20 = 0xEC3E,
    /// <summary>
    /// The table move left28.
    /// </summary>
    TableMoveLeft28 = 0xEC3F,
    /// <summary>
    /// The table move right16.
    /// </summary>
    TableMoveRight16 = 0xEC40,
    /// <summary>
    /// The table move right20.
    /// </summary>
    TableMoveRight20 = 0xEC41,
    /// <summary>
    /// The table move right28.
    /// </summary>
    TableMoveRight28 = 0xEC42,
    /// <summary>
    /// The table multiple20.
    /// </summary>
    TableMultiple20 = 0xEC43,
    /// <summary>
    /// The table resize column16.
    /// </summary>
    TableResizeColumn16 = 0xEC44,
    /// <summary>
    /// The table resize column20.
    /// </summary>
    TableResizeColumn20 = 0xEC45,
    /// <summary>
    /// The table resize column24.
    /// </summary>
    TableResizeColumn24 = 0xEC46,
    /// <summary>
    /// The table resize column28.
    /// </summary>
    TableResizeColumn28 = 0xEC47,
    /// <summary>
    /// The table resize row16.
    /// </summary>
    TableResizeRow16 = 0xEC48,
    /// <summary>
    /// The table resize row20.
    /// </summary>
    TableResizeRow20 = 0xEC49,
    /// <summary>
    /// The table resize row24.
    /// </summary>
    TableResizeRow24 = 0xEC4A,
    /// <summary>
    /// The table resize row28.
    /// </summary>
    TableResizeRow28 = 0xEC4B,
    /// <summary>
    /// The table search20.
    /// </summary>
    TableSearch20 = 0xEC4C,
    /// <summary>
    /// The table settings16.
    /// </summary>
    TableSettings16 = 0xEC4D,
    /// <summary>
    /// The table settings20.
    /// </summary>
    TableSettings20 = 0xEC4E,
    /// <summary>
    /// The table settings28.
    /// </summary>
    TableSettings28 = 0xEC4F,
    /// <summary>
    /// The table simple16.
    /// </summary>
    TableSimple16 = 0xEC50,
    /// <summary>
    /// The table simple20.
    /// </summary>
    TableSimple20 = 0xEC51,
    /// <summary>
    /// The table simple24.
    /// </summary>
    TableSimple24 = 0xEC52,
    /// <summary>
    /// The table simple28.
    /// </summary>
    TableSimple28 = 0xEC53,
    /// <summary>
    /// The table simple48.
    /// </summary>
    TableSimple48 = 0xEC54,
    /// <summary>
    /// The table split20.
    /// </summary>
    TableSplit20 = 0xEC55,
    /// <summary>
    /// The table stack above16.
    /// </summary>
    TableStackAbove16 = 0xEC56,
    /// <summary>
    /// The table stack above20.
    /// </summary>
    TableStackAbove20 = 0xEC57,
    /// <summary>
    /// The table stack above24.
    /// </summary>
    TableStackAbove24 = 0xEC58,
    /// <summary>
    /// The table stack above28.
    /// </summary>
    TableStackAbove28 = 0xEC59,
    /// <summary>
    /// The table stack below16.
    /// </summary>
    TableStackBelow16 = 0xEC5A,
    /// <summary>
    /// The table stack below20.
    /// </summary>
    TableStackBelow20 = 0xEC5B,
    /// <summary>
    /// The table stack below24.
    /// </summary>
    TableStackBelow24 = 0xEC5C,
    /// <summary>
    /// The table stack below28.
    /// </summary>
    TableStackBelow28 = 0xEC5D,
    /// <summary>
    /// The table stack left16.
    /// </summary>
    TableStackLeft16 = 0xEC5E,
    /// <summary>
    /// The table stack left20.
    /// </summary>
    TableStackLeft20 = 0xEC5F,
    /// <summary>
    /// The table stack left24.
    /// </summary>
    TableStackLeft24 = 0xEC60,
    /// <summary>
    /// The table stack left28.
    /// </summary>
    TableStackLeft28 = 0xEC61,
    /// <summary>
    /// The table stack right16.
    /// </summary>
    TableStackRight16 = 0xEC62,
    /// <summary>
    /// The table stack right20.
    /// </summary>
    TableStackRight20 = 0xEC63,
    /// <summary>
    /// The table stack right24.
    /// </summary>
    TableStackRight24 = 0xEC64,
    /// <summary>
    /// The table stack right28.
    /// </summary>
    TableStackRight28 = 0xEC65,
    /// <summary>
    /// The table switch16.
    /// </summary>
    TableSwitch16 = 0xEC66,
    /// <summary>
    /// The table switch20.
    /// </summary>
    TableSwitch20 = 0xEC67,
    /// <summary>
    /// The table switch28.
    /// </summary>
    TableSwitch28 = 0xEC68,
    /// <summary>
    /// The tablet12.
    /// </summary>
    Tablet12 = 0xEC69,
    /// <summary>
    /// The tablet16.
    /// </summary>
    Tablet16 = 0xEC6A,
    /// <summary>
    /// The tablet32.
    /// </summary>
    Tablet32 = 0xEC6B,
    /// <summary>
    /// The tablet48.
    /// </summary>
    Tablet48 = 0xEC6C,
    /// <summary>
    /// The tablet speaker20.
    /// </summary>
    TabletSpeaker20 = 0xEC6D,
    /// <summary>
    /// The tablet speaker24.
    /// </summary>
    TabletSpeaker24 = 0xEC6E,
    /// <summary>
    /// The tabs20.
    /// </summary>
    Tabs20 = 0xEC6F,
    /// <summary>
    /// The tag16.
    /// </summary>
    Tag16 = 0xEC70,
    /// <summary>
    /// The tag28.
    /// </summary>
    Tag28 = 0xEC71,
    /// <summary>
    /// The tag32.
    /// </summary>
    Tag32 = 0xEC72,
    /// <summary>
    /// The tag circle20.
    /// </summary>
    TagCircle20 = 0xEC73,
    /// <summary>
    /// The tag dismiss16.
    /// </summary>
    TagDismiss16 = 0xEC74,
    /// <summary>
    /// The tag dismiss20.
    /// </summary>
    TagDismiss20 = 0xEC75,
    /// <summary>
    /// The tag dismiss24.
    /// </summary>
    TagDismiss24 = 0xEC76,
    /// <summary>
    /// The tag error16.
    /// </summary>
    TagError16 = 0xEC77,
    /// <summary>
    /// The tag error20.
    /// </summary>
    TagError20 = 0xEC78,
    /// <summary>
    /// The tag error24.
    /// </summary>
    TagError24 = 0xEC79,
    /// <summary>
    /// The tag lock16.
    /// </summary>
    TagLock16 = 0xEC7A,
    /// <summary>
    /// The tag lock20.
    /// </summary>
    TagLock20 = 0xEC7B,
    /// <summary>
    /// The tag lock24.
    /// </summary>
    TagLock24 = 0xEC7C,
    /// <summary>
    /// The tag lock32.
    /// </summary>
    TagLock32 = 0xEC7D,
    /// <summary>
    /// The tag multiple20.
    /// </summary>
    TagMultiple20 = 0xEC7E,
    /// <summary>
    /// The tag multiple24.
    /// </summary>
    TagMultiple24 = 0xEC7F,
    /// <summary>
    /// The tag off20.
    /// </summary>
    TagOff20 = 0xEC80,
    /// <summary>
    /// The tag off24.
    /// </summary>
    TagOff24 = 0xEC81,
    /// <summary>
    /// The tag question mark16.
    /// </summary>
    TagQuestionMark16 = 0xEC82,
    /// <summary>
    /// The tag question mark20.
    /// </summary>
    TagQuestionMark20 = 0xEC83,
    /// <summary>
    /// The tag question mark24.
    /// </summary>
    TagQuestionMark24 = 0xEC84,
    /// <summary>
    /// The tag question mark32.
    /// </summary>
    TagQuestionMark32 = 0xEC85,
    /// <summary>
    /// The tag reset20.
    /// </summary>
    TagReset20 = 0xEC86,
    /// <summary>
    /// The tag reset24.
    /// </summary>
    TagReset24 = 0xEC87,
    /// <summary>
    /// The tag search20.
    /// </summary>
    TagSearch20 = 0xEC88,
    /// <summary>
    /// The tag search24.
    /// </summary>
    TagSearch24 = 0xEC89,
    /// <summary>
    /// The tap double20.
    /// </summary>
    TapDouble20 = 0xEC8A,
    /// <summary>
    /// The tap double32.
    /// </summary>
    TapDouble32 = 0xEC8B,
    /// <summary>
    /// The tap double48.
    /// </summary>
    TapDouble48 = 0xEC8C,
    /// <summary>
    /// The tap single20.
    /// </summary>
    TapSingle20 = 0xEC8D,
    /// <summary>
    /// The tap single32.
    /// </summary>
    TapSingle32 = 0xEC8E,
    /// <summary>
    /// The tap single48.
    /// </summary>
    TapSingle48 = 0xEC8F,
    /// <summary>
    /// The target32.
    /// </summary>
    Target32 = 0xEC90,
    /// <summary>
    /// The target arrow24.
    /// </summary>
    TargetArrow24 = 0xEC91,
    /// <summary>
    /// The task list LTR20.
    /// </summary>
    TaskListLtr20 = 0xEC92,
    /// <summary>
    /// The task list LTR24.
    /// </summary>
    TaskListLtr24 = 0xEC93,
    /// <summary>
    /// The task list RTL20.
    /// </summary>
    TaskListRtl20 = 0xEC94,
    /// <summary>
    /// The task list RTL24.
    /// </summary>
    TaskListRtl24 = 0xEC95,
    /// <summary>
    /// The task list square add20.
    /// </summary>
    TaskListSquareAdd20 = 0xEC96,
    /// <summary>
    /// The task list square add24.
    /// </summary>
    TaskListSquareAdd24 = 0xEC97,
    /// <summary>
    /// The task list square database20.
    /// </summary>
    TaskListSquareDatabase20 = 0xEC98,
    /// <summary>
    /// The task list square LTR20.
    /// </summary>
    TaskListSquareLtr20 = 0xEC99,
    /// <summary>
    /// The task list square LTR24.
    /// </summary>
    TaskListSquareLtr24 = 0xEC9A,
    /// <summary>
    /// The task list square person20.
    /// </summary>
    TaskListSquarePerson20 = 0xEC9B,
    /// <summary>
    /// The task list square RTL20.
    /// </summary>
    TaskListSquareRtl20 = 0xEC9C,
    /// <summary>
    /// The task list square RTL24.
    /// </summary>
    TaskListSquareRtl24 = 0xEC9D,
    /// <summary>
    /// The task list square settings20.
    /// </summary>
    TaskListSquareSettings20 = 0xEC9E,
    /// <summary>
    /// The tasks app20.
    /// </summary>
    TasksApp20 = 0xEC9F,
    /// <summary>
    /// The teddy20.
    /// </summary>
    Teddy20 = 0xECA0,
    /// <summary>
    /// The temperature16.
    /// </summary>
    Temperature16 = 0xECA1,
    /// <summary>
    /// The tent12.
    /// </summary>
    Tent12 = 0xECA2,
    /// <summary>
    /// The tent16.
    /// </summary>
    Tent16 = 0xECA3,
    /// <summary>
    /// The tent20.
    /// </summary>
    Tent20 = 0xECA4,
    /// <summary>
    /// The tent28.
    /// </summary>
    Tent28 = 0xECA5,
    /// <summary>
    /// The tent48.
    /// </summary>
    Tent48 = 0xECA6,
    /// <summary>
    /// The tetris app16.
    /// </summary>
    TetrisApp16 = 0xECA7,
    /// <summary>
    /// The tetris app20.
    /// </summary>
    TetrisApp20 = 0xECA8,
    /// <summary>
    /// The tetris app24.
    /// </summary>
    TetrisApp24 = 0xECA9,
    /// <summary>
    /// The tetris app28.
    /// </summary>
    TetrisApp28 = 0xECAA,
    /// <summary>
    /// The tetris app32.
    /// </summary>
    TetrisApp32 = 0xECAB,
    /// <summary>
    /// The tetris app48.
    /// </summary>
    TetrisApp48 = 0xECAC,
    /// <summary>
    /// The text12.
    /// </summary>
    Text12 = 0xECAD,
    /// <summary>
    /// The text16.
    /// </summary>
    Text16 = 0xECAE,
    /// <summary>
    /// The text32.
    /// </summary>
    Text32 = 0xECAF,
    /// <summary>
    /// The text add20.
    /// </summary>
    TextAdd20 = 0xECB0,
    /// <summary>
    /// The text add T24.
    /// </summary>
    TextAddT24 = 0xECB1,
    /// <summary>
    /// The text align center16.
    /// </summary>
    TextAlignCenter16 = 0xECB2,
    /// <summary>
    /// The text align center rotate27016.
    /// </summary>
    TextAlignCenterRotate27016 = 0xECB3,
    /// <summary>
    /// The text align center rotate27020.
    /// </summary>
    TextAlignCenterRotate27020 = 0xECB4,
    /// <summary>
    /// The text align center rotate27024.
    /// </summary>
    TextAlignCenterRotate27024 = 0xECB5,
    /// <summary>
    /// The text align center rotate9016.
    /// </summary>
    TextAlignCenterRotate9016 = 0xECB6,
    /// <summary>
    /// The text align center rotate9020.
    /// </summary>
    TextAlignCenterRotate9020 = 0xECB7,
    /// <summary>
    /// The text align center rotate9024.
    /// </summary>
    TextAlignCenterRotate9024 = 0xECB8,
    /// <summary>
    /// The text align distributed evenly20.
    /// </summary>
    TextAlignDistributedEvenly20 = 0xECB9,
    /// <summary>
    /// The text align distributed evenly24.
    /// </summary>
    TextAlignDistributedEvenly24 = 0xECBA,
    /// <summary>
    /// The text align distributed vertical20.
    /// </summary>
    TextAlignDistributedVertical20 = 0xECBB,
    /// <summary>
    /// The text align distributed vertical24.
    /// </summary>
    TextAlignDistributedVertical24 = 0xECBC,
    /// <summary>
    /// The text align justify low20.
    /// </summary>
    TextAlignJustifyLow20 = 0xECBD,
    /// <summary>
    /// The text align justify low24.
    /// </summary>
    TextAlignJustifyLow24 = 0xECBE,
    /// <summary>
    /// The text align justify rotate27020.
    /// </summary>
    TextAlignJustifyRotate27020 = 0xECBF,
    /// <summary>
    /// The text align justify rotate27024.
    /// </summary>
    TextAlignJustifyRotate27024 = 0xECC0,
    /// <summary>
    /// The text align justify rotate9020.
    /// </summary>
    TextAlignJustifyRotate9020 = 0xECC1,
    /// <summary>
    /// The text align justify rotate9024.
    /// </summary>
    TextAlignJustifyRotate9024 = 0xECC2,
    /// <summary>
    /// The text align left16.
    /// </summary>
    TextAlignLeft16 = 0xECC3,
    /// <summary>
    /// The text align left rotate27016.
    /// </summary>
    TextAlignLeftRotate27016 = 0xECC4,
    /// <summary>
    /// The text align left rotate27020.
    /// </summary>
    TextAlignLeftRotate27020 = 0xECC5,
    /// <summary>
    /// The text align left rotate27024.
    /// </summary>
    TextAlignLeftRotate27024 = 0xECC6,
    /// <summary>
    /// The text align left rotate9016.
    /// </summary>
    TextAlignLeftRotate9016 = 0xECC7,
    /// <summary>
    /// The text align left rotate9020.
    /// </summary>
    TextAlignLeftRotate9020 = 0xECC8,
    /// <summary>
    /// The text align left rotate9024.
    /// </summary>
    TextAlignLeftRotate9024 = 0xECC9,
    /// <summary>
    /// The text align right16.
    /// </summary>
    TextAlignRight16 = 0xECCA,
    /// <summary>
    /// The text align right rotate27016.
    /// </summary>
    TextAlignRightRotate27016 = 0xECCB,
    /// <summary>
    /// The text align right rotate27020.
    /// </summary>
    TextAlignRightRotate27020 = 0xECCC,
    /// <summary>
    /// The text align right rotate27024.
    /// </summary>
    TextAlignRightRotate27024 = 0xECCD,
    /// <summary>
    /// The text align right rotate9016.
    /// </summary>
    TextAlignRightRotate9016 = 0xECCE,
    /// <summary>
    /// The text align right rotate9020.
    /// </summary>
    TextAlignRightRotate9020 = 0xECCF,
    /// <summary>
    /// The text align right rotate9024.
    /// </summary>
    TextAlignRightRotate9024 = 0xECD0,
    /// <summary>
    /// The text baseline20.
    /// </summary>
    TextBaseline20 = 0xECD1,
    /// <summary>
    /// The text bold16.
    /// </summary>
    TextBold16 = 0xECD2,
    /// <summary>
    /// The text box settings20.
    /// </summary>
    TextBoxSettings20 = 0xECD3,
    /// <summary>
    /// The text box settings24.
    /// </summary>
    TextBoxSettings24 = 0xECD4,
    /// <summary>
    /// The text bullet list add20.
    /// </summary>
    TextBulletListAdd20 = 0xECD5,
    /// <summary>
    /// The text bullet list checkmark20.
    /// </summary>
    TextBulletListCheckmark20 = 0xECD6,
    /// <summary>
    /// The text bullet list dismiss20.
    /// </summary>
    TextBulletListDismiss20 = 0xECD7,
    /// <summary>
    /// The text bullet list LTR16.
    /// </summary>
    TextBulletListLtr16 = 0xECD8,
    /// <summary>
    /// The text bullet list LTR20.
    /// </summary>
    TextBulletListLtr20 = 0xECD9,
    /// <summary>
    /// The text bullet list LTR24.
    /// </summary>
    TextBulletListLtr24 = 0xECDA,
    /// <summary>
    /// The chat multiple28.
    /// </summary>
    ChatMultiple28 = 0xECDB,
    /// <summary>
    /// The chat multiple32.
    /// </summary>
    ChatMultiple32 = 0xECDC,
    /// <summary>
    /// The document landscape split hint24.
    /// </summary>
    DocumentLandscapeSplitHint24 = 0xECDD,
    /// <summary>
    /// The glance12.
    /// </summary>
    Glance12 = 0xECDE,
    /// <summary>
    /// The text bullet list RTL16.
    /// </summary>
    TextBulletListRtl16 = 0xECDF,
    /// <summary>
    /// The text bullet list RTL20.
    /// </summary>
    TextBulletListRtl20 = 0xECE0,
    /// <summary>
    /// The text bullet list RTL24.
    /// </summary>
    TextBulletListRtl24 = 0xECE1,
    /// <summary>
    /// The text bullet list square20.
    /// </summary>
    TextBulletListSquare20 = 0xECE2,
    /// <summary>
    /// The text bullet list square clock20.
    /// </summary>
    TextBulletListSquareClock20 = 0xECE3,
    /// <summary>
    /// The text bullet list square person20.
    /// </summary>
    TextBulletListSquarePerson20 = 0xECE4,
    /// <summary>
    /// The text bullet list square search20.
    /// </summary>
    TextBulletListSquareSearch20 = 0xECE5,
    /// <summary>
    /// The text bullet list square settings20.
    /// </summary>
    TextBulletListSquareSettings20 = 0xECE6,
    /// <summary>
    /// The text bullet list square shield20.
    /// </summary>
    TextBulletListSquareShield20 = 0xECE7,
    /// <summary>
    /// The text bullet list square toolbox20.
    /// </summary>
    TextBulletListSquareToolbox20 = 0xECE8,
    /// <summary>
    /// The text case lowercase16.
    /// </summary>
    TextCaseLowercase16 = 0xECE9,
    /// <summary>
    /// The text case lowercase20.
    /// </summary>
    TextCaseLowercase20 = 0xECEA,
    /// <summary>
    /// The text case lowercase24.
    /// </summary>
    TextCaseLowercase24 = 0xECEB,
    /// <summary>
    /// The text case title16.
    /// </summary>
    TextCaseTitle16 = 0xECEC,
    /// <summary>
    /// The text case title20.
    /// </summary>
    TextCaseTitle20 = 0xECED,
    /// <summary>
    /// The text case title24.
    /// </summary>
    TextCaseTitle24 = 0xECEE,
    /// <summary>
    /// The text case uppercase16.
    /// </summary>
    TextCaseUppercase16 = 0xECEF,
    /// <summary>
    /// The text case uppercase20.
    /// </summary>
    TextCaseUppercase20 = 0xECF0,
    /// <summary>
    /// The text case uppercase24.
    /// </summary>
    TextCaseUppercase24 = 0xECF1,
    /// <summary>
    /// The text change case16.
    /// </summary>
    TextChangeCase16 = 0xECF2,
    /// <summary>
    /// The text clear formatting16.
    /// </summary>
    TextClearFormatting16 = 0xECF3,
    /// <summary>
    /// The text collapse20.
    /// </summary>
    TextCollapse20 = 0xECF4,
    /// <summary>
    /// The text color16.
    /// </summary>
    TextColor16 = 0xECF5,
    /// <summary>
    /// The text column one narrow20.
    /// </summary>
    TextColumnOneNarrow20 = 0xECF6,
    /// <summary>
    /// The text column one narrow24.
    /// </summary>
    TextColumnOneNarrow24 = 0xECF7,
    /// <summary>
    /// The text column one wide20.
    /// </summary>
    TextColumnOneWide20 = 0xECF8,
    /// <summary>
    /// The text column one wide24.
    /// </summary>
    TextColumnOneWide24 = 0xECF9,
    /// <summary>
    /// The text column one wide lightning20.
    /// </summary>
    TextColumnOneWideLightning20 = 0xECFA,
    /// <summary>
    /// The text column one wide lightning24.
    /// </summary>
    TextColumnOneWideLightning24 = 0xECFB,
    /// <summary>
    /// The text continuous20.
    /// </summary>
    TextContinuous20 = 0xECFC,
    /// <summary>
    /// The text continuous24.
    /// </summary>
    TextContinuous24 = 0xECFD,
    /// <summary>
    /// The text density16.
    /// </summary>
    TextDensity16 = 0xECFE,
    /// <summary>
    /// The text density20.
    /// </summary>
    TextDensity20 = 0xECFF,
    /// <summary>
    /// The text density24.
    /// </summary>
    TextDensity24 = 0xED00,
    /// <summary>
    /// The text density28.
    /// </summary>
    TextDensity28 = 0xED01,
    /// <summary>
    /// The text direction horizontal left20.
    /// </summary>
    TextDirectionHorizontalLeft20 = 0xED02,
    /// <summary>
    /// The text direction horizontal left24.
    /// </summary>
    TextDirectionHorizontalLeft24 = 0xED03,
    /// <summary>
    /// The text direction horizontal right20.
    /// </summary>
    TextDirectionHorizontalRight20 = 0xED04,
    /// <summary>
    /// The text direction horizontal right24.
    /// </summary>
    TextDirectionHorizontalRight24 = 0xED05,
    /// <summary>
    /// The text direction rotate270 right20.
    /// </summary>
    TextDirectionRotate270Right20 = 0xED06,
    /// <summary>
    /// The text direction rotate270 right24.
    /// </summary>
    TextDirectionRotate270Right24 = 0xED07,
    /// <summary>
    /// The text direction rotate90 left20.
    /// </summary>
    TextDirectionRotate90Left20 = 0xED08,
    /// <summary>
    /// The text direction rotate90 left24.
    /// </summary>
    TextDirectionRotate90Left24 = 0xED09,
    /// <summary>
    /// The text direction rotate90 right20.
    /// </summary>
    TextDirectionRotate90Right20 = 0xED0A,
    /// <summary>
    /// The text direction rotate90 right24.
    /// </summary>
    TextDirectionRotate90Right24 = 0xED0B,
    /// <summary>
    /// The text expand20.
    /// </summary>
    TextExpand20 = 0xED0C,
    /// <summary>
    /// The text font info16.
    /// </summary>
    TextFontInfo16 = 0xED0D,
    /// <summary>
    /// The text font info20.
    /// </summary>
    TextFontInfo20 = 0xED0E,
    /// <summary>
    /// The text font info24.
    /// </summary>
    TextFontInfo24 = 0xED0F,
    /// <summary>
    /// The text font size16.
    /// </summary>
    TextFontSize16 = 0xED10,
    /// <summary>
    /// The text grammar arrow left20.
    /// </summary>
    TextGrammarArrowLeft20 = 0xED11,
    /// <summary>
    /// The text grammar arrow left24.
    /// </summary>
    TextGrammarArrowLeft24 = 0xED12,
    /// <summary>
    /// The text grammar arrow right20.
    /// </summary>
    TextGrammarArrowRight20 = 0xED13,
    /// <summary>
    /// The text grammar arrow right24.
    /// </summary>
    TextGrammarArrowRight24 = 0xED14,
    /// <summary>
    /// The text grammar checkmark20.
    /// </summary>
    TextGrammarCheckmark20 = 0xED15,
    /// <summary>
    /// The text grammar checkmark24.
    /// </summary>
    TextGrammarCheckmark24 = 0xED16,
    /// <summary>
    /// The text grammar dismiss20.
    /// </summary>
    TextGrammarDismiss20 = 0xED17,
    /// <summary>
    /// The text grammar dismiss24.
    /// </summary>
    TextGrammarDismiss24 = 0xED18,
    /// <summary>
    /// The text grammar error20.
    /// </summary>
    TextGrammarError20 = 0xED19,
    /// <summary>
    /// The text grammar settings20.
    /// </summary>
    TextGrammarSettings20 = 0xED1A,
    /// <summary>
    /// The text grammar settings24.
    /// </summary>
    TextGrammarSettings24 = 0xED1B,
    /// <summary>
    /// The text grammar wand16.
    /// </summary>
    TextGrammarWand16 = 0xED1C,
    /// <summary>
    /// The text grammar wand20.
    /// </summary>
    TextGrammarWand20 = 0xED1D,
    /// <summary>
    /// The text grammar wand24.
    /// </summary>
    TextGrammarWand24 = 0xED1E,
    /// <summary>
    /// The text header124.
    /// </summary>
    TextHeader124 = 0xED1F,
    /// <summary>
    /// The text header224.
    /// </summary>
    TextHeader224 = 0xED20,
    /// <summary>
    /// The text header324.
    /// </summary>
    TextHeader324 = 0xED21,
    /// <summary>
    /// The text indent decrease LTR16.
    /// </summary>
    TextIndentDecreaseLtr16 = 0xED22,
    /// <summary>
    /// The text indent decrease LTR20.
    /// </summary>
    TextIndentDecreaseLtr20 = 0xED23,
    /// <summary>
    /// The text indent decrease LTR24.
    /// </summary>
    TextIndentDecreaseLtr24 = 0xED24,
    /// <summary>
    /// The text indent decrease rotate27020.
    /// </summary>
    TextIndentDecreaseRotate27020 = 0xED25,
    /// <summary>
    /// The text indent decrease rotate27024.
    /// </summary>
    TextIndentDecreaseRotate27024 = 0xED26,
    /// <summary>
    /// The text indent decrease rotate9020.
    /// </summary>
    TextIndentDecreaseRotate9020 = 0xED27,
    /// <summary>
    /// The text indent decrease rotate9024.
    /// </summary>
    TextIndentDecreaseRotate9024 = 0xED28,
    /// <summary>
    /// The text indent decrease RTL16.
    /// </summary>
    TextIndentDecreaseRtl16 = 0xED29,
    /// <summary>
    /// The text indent decrease RTL20.
    /// </summary>
    TextIndentDecreaseRtl20 = 0xED2A,
    /// <summary>
    /// The text indent decrease RTL24.
    /// </summary>
    TextIndentDecreaseRtl24 = 0xED2B,
    /// <summary>
    /// The text indent increase LTR16.
    /// </summary>
    TextIndentIncreaseLtr16 = 0xED2C,
    /// <summary>
    /// The text indent increase LTR20.
    /// </summary>
    TextIndentIncreaseLtr20 = 0xED2D,
    /// <summary>
    /// The text indent increase LTR24.
    /// </summary>
    TextIndentIncreaseLtr24 = 0xED2E,
    /// <summary>
    /// The text indent increase rotate27020.
    /// </summary>
    TextIndentIncreaseRotate27020 = 0xED2F,
    /// <summary>
    /// The text indent increase rotate27024.
    /// </summary>
    TextIndentIncreaseRotate27024 = 0xED30,
    /// <summary>
    /// The text indent increase rotate9020.
    /// </summary>
    TextIndentIncreaseRotate9020 = 0xED31,
    /// <summary>
    /// The text indent increase rotate9024.
    /// </summary>
    TextIndentIncreaseRotate9024 = 0xED32,
    /// <summary>
    /// The text indent increase RTL16.
    /// </summary>
    TextIndentIncreaseRtl16 = 0xED33,
    /// <summary>
    /// The text indent increase RTL20.
    /// </summary>
    TextIndentIncreaseRtl20 = 0xED34,
    /// <summary>
    /// The text indent increase RTL24.
    /// </summary>
    TextIndentIncreaseRtl24 = 0xED35,
    /// <summary>
    /// The text italic16.
    /// </summary>
    TextItalic16 = 0xED36,
    /// <summary>
    /// The text more20.
    /// </summary>
    TextMore20 = 0xED37,
    /// <summary>
    /// The text more24.
    /// </summary>
    TextMore24 = 0xED38,
    /// <summary>
    /// The text number format20.
    /// </summary>
    TextNumberFormat20 = 0xED39,
    /// <summary>
    /// The text number list LTR16.
    /// </summary>
    TextNumberListLtr16 = 0xED3A,
    /// <summary>
    /// The text number list rotate27020.
    /// </summary>
    TextNumberListRotate27020 = 0xED3B,
    /// <summary>
    /// The text number list rotate27024.
    /// </summary>
    TextNumberListRotate27024 = 0xED3C,
    /// <summary>
    /// The text number list rotate9020.
    /// </summary>
    TextNumberListRotate9020 = 0xED3D,
    /// <summary>
    /// The text number list rotate9024.
    /// </summary>
    TextNumberListRotate9024 = 0xED3E,
    /// <summary>
    /// The text number list RTL16.
    /// </summary>
    TextNumberListRtl16 = 0xED3F,
    /// <summary>
    /// The text number list RTL20.
    /// </summary>
    TextNumberListRtl20 = 0xED40,
    /// <summary>
    /// The text paragraph16.
    /// </summary>
    TextParagraph16 = 0xED41,
    /// <summary>
    /// The text paragraph20.
    /// </summary>
    TextParagraph20 = 0xED42,
    /// <summary>
    /// The text paragraph24.
    /// </summary>
    TextParagraph24 = 0xED43,
    /// <summary>
    /// The text paragraph direction20.
    /// </summary>
    TextParagraphDirection20 = 0xED44,
    /// <summary>
    /// The text paragraph direction24.
    /// </summary>
    TextParagraphDirection24 = 0xED45,
    /// <summary>
    /// The text paragraph direction left16.
    /// </summary>
    TextParagraphDirectionLeft16 = 0xED46,
    /// <summary>
    /// The text paragraph direction left20.
    /// </summary>
    TextParagraphDirectionLeft20 = 0xED47,
    /// <summary>
    /// The text paragraph direction right16.
    /// </summary>
    TextParagraphDirectionRight16 = 0xED48,
    /// <summary>
    /// The text paragraph direction right20.
    /// </summary>
    TextParagraphDirectionRight20 = 0xED49,
    /// <summary>
    /// The text period asterisk20.
    /// </summary>
    TextPeriodAsterisk20 = 0xED4A,
    /// <summary>
    /// The text position behind20.
    /// </summary>
    TextPositionBehind20 = 0xED4B,
    /// <summary>
    /// The text position behind24.
    /// </summary>
    TextPositionBehind24 = 0xED4C,
    /// <summary>
    /// The text position front20.
    /// </summary>
    TextPositionFront20 = 0xED4D,
    /// <summary>
    /// The text position front24.
    /// </summary>
    TextPositionFront24 = 0xED4E,
    /// <summary>
    /// The text position line20.
    /// </summary>
    TextPositionLine20 = 0xED4F,
    /// <summary>
    /// The text position line24.
    /// </summary>
    TextPositionLine24 = 0xED50,
    /// <summary>
    /// The text position square20.
    /// </summary>
    TextPositionSquare20 = 0xED51,
    /// <summary>
    /// The text position square24.
    /// </summary>
    TextPositionSquare24 = 0xED52,
    /// <summary>
    /// The text position through20.
    /// </summary>
    TextPositionThrough20 = 0xED53,
    /// <summary>
    /// The text position through24.
    /// </summary>
    TextPositionThrough24 = 0xED54,
    /// <summary>
    /// The text position tight20.
    /// </summary>
    TextPositionTight20 = 0xED55,
    /// <summary>
    /// The text position tight24.
    /// </summary>
    TextPositionTight24 = 0xED56,
    /// <summary>
    /// The text position top bottom20.
    /// </summary>
    TextPositionTopBottom20 = 0xED57,
    /// <summary>
    /// The text position top bottom24.
    /// </summary>
    TextPositionTopBottom24 = 0xED58,
    /// <summary>
    /// The text quote16.
    /// </summary>
    TextQuote16 = 0xED59,
    /// <summary>
    /// The text sort ascending16.
    /// </summary>
    TextSortAscending16 = 0xED5A,
    /// <summary>
    /// The text sort ascending24.
    /// </summary>
    TextSortAscending24 = 0xED5B,
    /// <summary>
    /// The text sort descending16.
    /// </summary>
    TextSortDescending16 = 0xED5C,
    /// <summary>
    /// The text sort descending24.
    /// </summary>
    TextSortDescending24 = 0xED5D,
    /// <summary>
    /// The text strikethrough16.
    /// </summary>
    TextStrikethrough16 = 0xED5E,
    /// <summary>
    /// The text strikethrough20.
    /// </summary>
    TextStrikethrough20 = 0xED5F,
    /// <summary>
    /// The text strikethrough24.
    /// </summary>
    TextStrikethrough24 = 0xED60,
    /// <summary>
    /// The text subscript16.
    /// </summary>
    TextSubscript16 = 0xED61,
    /// <summary>
    /// The text superscript16.
    /// </summary>
    TextSuperscript16 = 0xED62,
    /// <summary>
    /// The text T20.
    /// </summary>
    TextT20 = 0xED63,
    /// <summary>
    /// The text T24.
    /// </summary>
    TextT24 = 0xED64,
    /// <summary>
    /// The text T28.
    /// </summary>
    TextT28 = 0xED65,
    /// <summary>
    /// The text T48.
    /// </summary>
    TextT48 = 0xED66,
    /// <summary>
    /// The text underline16.
    /// </summary>
    TextUnderline16 = 0xED67,
    /// <summary>
    /// The text whole word20.
    /// </summary>
    TextWholeWord20 = 0xED68,
    /// <summary>
    /// The text wrap20.
    /// </summary>
    TextWrap20 = 0xED69,
    /// <summary>
    /// The textbox16.
    /// </summary>
    Textbox16 = 0xED6A,
    /// <summary>
    /// The textbox align bottom rotate9020.
    /// </summary>
    TextboxAlignBottomRotate9020 = 0xED6B,
    /// <summary>
    /// The textbox align bottom rotate9024.
    /// </summary>
    TextboxAlignBottomRotate9024 = 0xED6C,
    /// <summary>
    /// The textbox align center20.
    /// </summary>
    TextboxAlignCenter20 = 0xED6D,
    /// <summary>
    /// The textbox align center24.
    /// </summary>
    TextboxAlignCenter24 = 0xED6E,
    /// <summary>
    /// The textbox align middle rotate9020.
    /// </summary>
    TextboxAlignMiddleRotate9020 = 0xED6F,
    /// <summary>
    /// The textbox align middle rotate9024.
    /// </summary>
    TextboxAlignMiddleRotate9024 = 0xED70,
    /// <summary>
    /// The textbox align top rotate9020.
    /// </summary>
    TextboxAlignTopRotate9020 = 0xED71,
    /// <summary>
    /// The textbox align top rotate9024.
    /// </summary>
    TextboxAlignTopRotate9024 = 0xED72,
    /// <summary>
    /// The textbox more20.
    /// </summary>
    TextboxMore20 = 0xED73,
    /// <summary>
    /// The textbox more24.
    /// </summary>
    TextboxMore24 = 0xED74,
    /// <summary>
    /// The textbox rotate9020.
    /// </summary>
    TextboxRotate9020 = 0xED75,
    /// <summary>
    /// The textbox rotate9024.
    /// </summary>
    TextboxRotate9024 = 0xED76,
    /// <summary>
    /// The thumb dislike16.
    /// </summary>
    ThumbDislike16 = 0xED77,
    /// <summary>
    /// The thumb like16.
    /// </summary>
    ThumbLike16 = 0xED78,
    /// <summary>
    /// The thumb like28.
    /// </summary>
    ThumbLike28 = 0xED79,
    /// <summary>
    /// The thumb like48.
    /// </summary>
    ThumbLike48 = 0xED7A,
    /// <summary>
    /// The ticket diagonal16.
    /// </summary>
    TicketDiagonal16 = 0xED7B,
    /// <summary>
    /// The ticket diagonal20.
    /// </summary>
    TicketDiagonal20 = 0xED7C,
    /// <summary>
    /// The ticket diagonal24.
    /// </summary>
    TicketDiagonal24 = 0xED7D,
    /// <summary>
    /// The ticket diagonal28.
    /// </summary>
    TicketDiagonal28 = 0xED7E,
    /// <summary>
    /// The ticket horizontal20.
    /// </summary>
    TicketHorizontal20 = 0xED7F,
    /// <summary>
    /// The ticket horizontal24.
    /// </summary>
    TicketHorizontal24 = 0xED80,
    /// <summary>
    /// The time and weather20.
    /// </summary>
    TimeAndWeather20 = 0xED81,
    /// <summary>
    /// The time picker20.
    /// </summary>
    TimePicker20 = 0xED82,
    /// <summary>
    /// The timeline20.
    /// </summary>
    Timeline20 = 0xED83,
    /// <summary>
    /// The timer1020.
    /// </summary>
    Timer1020 = 0xED84,
    /// <summary>
    /// The timer12.
    /// </summary>
    Timer12 = 0xED85,
    /// <summary>
    /// The timer16.
    /// </summary>
    Timer16 = 0xED86,
    /// <summary>
    /// The timer220.
    /// </summary>
    Timer220 = 0xED87,
    /// <summary>
    /// The timer20.
    /// </summary>
    Timer20 = 0xED88,
    /// <summary>
    /// The timer28.
    /// </summary>
    Timer28 = 0xED89,
    /// <summary>
    /// The timer320.
    /// </summary>
    Timer320 = 0xED8A,
    /// <summary>
    /// The timer324.
    /// </summary>
    Timer324 = 0xED8B,
    /// <summary>
    /// The timer32.
    /// </summary>
    Timer32 = 0xED8C,
    /// <summary>
    /// The timer48.
    /// </summary>
    Timer48 = 0xED8D,
    /// <summary>
    /// The timer off20.
    /// </summary>
    TimerOff20 = 0xED8E,
    /// <summary>
    /// The toggle left16.
    /// </summary>
    ToggleLeft16 = 0xED8F,
    /// <summary>
    /// The toggle left20.
    /// </summary>
    ToggleLeft20 = 0xED90,
    /// <summary>
    /// The toggle left24.
    /// </summary>
    ToggleLeft24 = 0xED91,
    /// <summary>
    /// The toggle left28.
    /// </summary>
    ToggleLeft28 = 0xED92,
    /// <summary>
    /// The toggle left48.
    /// </summary>
    ToggleLeft48 = 0xED93,
    /// <summary>
    /// The toggle multiple16.
    /// </summary>
    ToggleMultiple16 = 0xED94,
    /// <summary>
    /// The toggle multiple20.
    /// </summary>
    ToggleMultiple20 = 0xED95,
    /// <summary>
    /// The toggle multiple24.
    /// </summary>
    ToggleMultiple24 = 0xED96,
    /// <summary>
    /// The toggle right28.
    /// </summary>
    ToggleRight28 = 0xED97,
    /// <summary>
    /// The toggle right48.
    /// </summary>
    ToggleRight48 = 0xED98,
    /// <summary>
    /// The toolbox12.
    /// </summary>
    Toolbox12 = 0xED99,
    /// <summary>
    /// The tooltip quote24.
    /// </summary>
    TooltipQuote24 = 0xED9A,
    /// <summary>
    /// The top speed20.
    /// </summary>
    TopSpeed20 = 0xED9B,
    /// <summary>
    /// The transmission20.
    /// </summary>
    Transmission20 = 0xED9C,
    /// <summary>
    /// The transmission24.
    /// </summary>
    Transmission24 = 0xED9D,
    /// <summary>
    /// The tray item add20.
    /// </summary>
    TrayItemAdd20 = 0xED9E,
    /// <summary>
    /// The tray item add24.
    /// </summary>
    TrayItemAdd24 = 0xED9F,
    /// <summary>
    /// The tray item remove20.
    /// </summary>
    TrayItemRemove20 = 0xEDA0,
    /// <summary>
    /// The tray item remove24.
    /// </summary>
    TrayItemRemove24 = 0xEDA1,
    /// <summary>
    /// The tree deciduous20.
    /// </summary>
    TreeDeciduous20 = 0xEDA2,
    /// <summary>
    /// The tree evergreen20.
    /// </summary>
    TreeEvergreen20 = 0xEDA3,
    /// <summary>
    /// The triangle12.
    /// </summary>
    Triangle12 = 0xEDA4,
    /// <summary>
    /// The triangle16.
    /// </summary>
    Triangle16 = 0xEDA5,
    /// <summary>
    /// The triangle20.
    /// </summary>
    Triangle20 = 0xEDA6,
    /// <summary>
    /// The triangle32.
    /// </summary>
    Triangle32 = 0xEDA7,
    /// <summary>
    /// The triangle48.
    /// </summary>
    Triangle48 = 0xEDA8,
    /// <summary>
    /// The triangle down12.
    /// </summary>
    TriangleDown12 = 0xEDA9,
    /// <summary>
    /// The triangle down16.
    /// </summary>
    TriangleDown16 = 0xEDAA,
    /// <summary>
    /// The triangle down20.
    /// </summary>
    TriangleDown20 = 0xEDAB,
    /// <summary>
    /// The triangle down32.
    /// </summary>
    TriangleDown32 = 0xEDAC,
    /// <summary>
    /// The triangle down48.
    /// </summary>
    TriangleDown48 = 0xEDAD,
    /// <summary>
    /// The triangle left12.
    /// </summary>
    TriangleLeft12 = 0xEDAE,
    /// <summary>
    /// The triangle left16.
    /// </summary>
    TriangleLeft16 = 0xEDAF,
    /// <summary>
    /// The triangle left20.
    /// </summary>
    TriangleLeft20 = 0xEDB0,
    /// <summary>
    /// The triangle left32.
    /// </summary>
    TriangleLeft32 = 0xEDB1,
    /// <summary>
    /// The triangle left48.
    /// </summary>
    TriangleLeft48 = 0xEDB2,
    /// <summary>
    /// The triangle right12.
    /// </summary>
    TriangleRight12 = 0xEDB3,
    /// <summary>
    /// The triangle right16.
    /// </summary>
    TriangleRight16 = 0xEDB4,
    /// <summary>
    /// The triangle right20.
    /// </summary>
    TriangleRight20 = 0xEDB5,
    /// <summary>
    /// The triangle right32.
    /// </summary>
    TriangleRight32 = 0xEDB6,
    /// <summary>
    /// The triangle right48.
    /// </summary>
    TriangleRight48 = 0xEDB7,
    /// <summary>
    /// The trophy28.
    /// </summary>
    Trophy28 = 0xEDB8,
    /// <summary>
    /// The trophy32.
    /// </summary>
    Trophy32 = 0xEDB9,
    /// <summary>
    /// The trophy48.
    /// </summary>
    Trophy48 = 0xEDBA,
    /// <summary>
    /// The trophy off16.
    /// </summary>
    TrophyOff16 = 0xEDBB,
    /// <summary>
    /// The trophy off20.
    /// </summary>
    TrophyOff20 = 0xEDBC,
    /// <summary>
    /// The trophy off24.
    /// </summary>
    TrophyOff24 = 0xEDBD,
    /// <summary>
    /// The trophy off28.
    /// </summary>
    TrophyOff28 = 0xEDBE,
    /// <summary>
    /// The trophy off32.
    /// </summary>
    TrophyOff32 = 0xEDBF,
    /// <summary>
    /// The trophy off48.
    /// </summary>
    TrophyOff48 = 0xEDC0,
    /// <summary>
    /// The TV16.
    /// </summary>
    Tv16 = 0xEDC1,
    /// <summary>
    /// The TV20.
    /// </summary>
    Tv20 = 0xEDC2,
    /// <summary>
    /// The TV24.
    /// </summary>
    Tv24 = 0xEDC3,
    /// <summary>
    /// The TV28.
    /// </summary>
    Tv28 = 0xEDC4,
    /// <summary>
    /// The TV48.
    /// </summary>
    Tv48 = 0xEDC5,
    /// <summary>
    /// The tv arrow right20.
    /// </summary>
    TvArrowRight20 = 0xEDC6,
    /// <summary>
    /// The tv usb16.
    /// </summary>
    TvUsb16 = 0xEDC7,
    /// <summary>
    /// The tv usb20.
    /// </summary>
    TvUsb20 = 0xEDC8,
    /// <summary>
    /// The tv usb24.
    /// </summary>
    TvUsb24 = 0xEDC9,
    /// <summary>
    /// The tv usb28.
    /// </summary>
    TvUsb28 = 0xEDCA,
    /// <summary>
    /// The tv usb48.
    /// </summary>
    TvUsb48 = 0xEDCB,
    /// <summary>
    /// The umbrella20.
    /// </summary>
    Umbrella20 = 0xEDCC,
    /// <summary>
    /// The umbrella24.
    /// </summary>
    Umbrella24 = 0xEDCD,
    /// <summary>
    /// The uninstall app20.
    /// </summary>
    UninstallApp20 = 0xEDCE,
    /// <summary>
    /// The usb plug20.
    /// </summary>
    UsbPlug20 = 0xEDCF,
    /// <summary>
    /// The usb plug24.
    /// </summary>
    UsbPlug24 = 0xEDD0,
    /// <summary>
    /// The vehicle bicycle16.
    /// </summary>
    VehicleBicycle16 = 0xEDD1,
    /// <summary>
    /// The vehicle bicycle20.
    /// </summary>
    VehicleBicycle20 = 0xEDD2,
    /// <summary>
    /// The vehicle bus16.
    /// </summary>
    VehicleBus16 = 0xEDD3,
    /// <summary>
    /// The vehicle bus20.
    /// </summary>
    VehicleBus20 = 0xEDD4,
    /// <summary>
    /// The vehicle cab16.
    /// </summary>
    VehicleCab16 = 0xEDD5,
    /// <summary>
    /// The vehicle cab20.
    /// </summary>
    VehicleCab20 = 0xEDD6,
    /// <summary>
    /// The vehicle cab28.
    /// </summary>
    VehicleCab28 = 0xEDD7,
    /// <summary>
    /// The vehicle car28.
    /// </summary>
    VehicleCar28 = 0xEDD8,
    /// <summary>
    /// The vehicle car48.
    /// </summary>
    VehicleCar48 = 0xEDD9,
    /// <summary>
    /// The vehicle car collision16.
    /// </summary>
    VehicleCarCollision16 = 0xEDDA,
    /// <summary>
    /// The vehicle car collision20.
    /// </summary>
    VehicleCarCollision20 = 0xEDDB,
    /// <summary>
    /// The vehicle car collision24.
    /// </summary>
    VehicleCarCollision24 = 0xEDDC,
    /// <summary>
    /// The vehicle car collision28.
    /// </summary>
    VehicleCarCollision28 = 0xEDDD,
    /// <summary>
    /// The vehicle car collision32.
    /// </summary>
    VehicleCarCollision32 = 0xEDDE,
    /// <summary>
    /// The vehicle car collision48.
    /// </summary>
    VehicleCarCollision48 = 0xEDDF,
    /// <summary>
    /// The vehicle car profile LTR20.
    /// </summary>
    VehicleCarProfileLtr20 = 0xEDE0,
    /// <summary>
    /// The vehicle car profile RTL20.
    /// </summary>
    VehicleCarProfileRtl20 = 0xEDE1,
    /// <summary>
    /// The vehicle ship16.
    /// </summary>
    VehicleShip16 = 0xEDE2,
    /// <summary>
    /// The vehicle ship20.
    /// </summary>
    VehicleShip20 = 0xEDE3,
    /// <summary>
    /// The vehicle ship24.
    /// </summary>
    VehicleShip24 = 0xEDE4,
    /// <summary>
    /// The vehicle subway16.
    /// </summary>
    VehicleSubway16 = 0xEDE5,
    /// <summary>
    /// The vehicle subway20.
    /// </summary>
    VehicleSubway20 = 0xEDE6,
    /// <summary>
    /// The vehicle subway24.
    /// </summary>
    VehicleSubway24 = 0xEDE7,
    /// <summary>
    /// The vehicle truck16.
    /// </summary>
    VehicleTruck16 = 0xEDE8,
    /// <summary>
    /// The vehicle truck20.
    /// </summary>
    VehicleTruck20 = 0xEDE9,
    /// <summary>
    /// The vehicle truck bag20.
    /// </summary>
    VehicleTruckBag20 = 0xEDEA,
    /// <summary>
    /// The vehicle truck bag24.
    /// </summary>
    VehicleTruckBag24 = 0xEDEB,
    /// <summary>
    /// The vehicle truck cube20.
    /// </summary>
    VehicleTruckCube20 = 0xEDEC,
    /// <summary>
    /// The vehicle truck cube24.
    /// </summary>
    VehicleTruckCube24 = 0xEDED,
    /// <summary>
    /// The vehicle truck profile20.
    /// </summary>
    VehicleTruckProfile20 = 0xEDEE,
    /// <summary>
    /// The vehicle truck profile24.
    /// </summary>
    VehicleTruckProfile24 = 0xEDEF,
    /// <summary>
    /// The video32.
    /// </summary>
    Video32 = 0xEDF0,
    /// <summary>
    /// The video36020.
    /// </summary>
    Video36020 = 0xEDF1,
    /// <summary>
    /// The video36024.
    /// </summary>
    Video36024 = 0xEDF2,
    /// <summary>
    /// The video360 off20.
    /// </summary>
    Video360Off20 = 0xEDF3,
    /// <summary>
    /// The video48.
    /// </summary>
    Video48 = 0xEDF4,
    /// <summary>
    /// The video add20.
    /// </summary>
    VideoAdd20 = 0xEDF5,
    /// <summary>
    /// The video add24.
    /// </summary>
    VideoAdd24 = 0xEDF6,
    /// <summary>
    /// The video background effect20.
    /// </summary>
    VideoBackgroundEffect20 = 0xEDF7,
    /// <summary>
    /// The video chat16.
    /// </summary>
    VideoChat16 = 0xEDF8,
    /// <summary>
    /// The video chat20.
    /// </summary>
    VideoChat20 = 0xEDF9,
    /// <summary>
    /// The video chat24.
    /// </summary>
    VideoChat24 = 0xEDFA,
    /// <summary>
    /// The video chat28.
    /// </summary>
    VideoChat28 = 0xEDFB,
    /// <summary>
    /// The video chat32.
    /// </summary>
    VideoChat32 = 0xEDFC,
    /// <summary>
    /// The video chat48.
    /// </summary>
    VideoChat48 = 0xEDFD,
    /// <summary>
    /// The video clip16.
    /// </summary>
    VideoClip16 = 0xEDFE,
    /// <summary>
    /// The video clip20.
    /// </summary>
    VideoClip20 = 0xEDFF,
    /// <summary>
    /// The video clip multiple16.
    /// </summary>
    VideoClipMultiple16 = 0xEE00,
    /// <summary>
    /// The video clip multiple20.
    /// </summary>
    VideoClipMultiple20 = 0xEE01,
    /// <summary>
    /// The video clip multiple24.
    /// </summary>
    VideoClipMultiple24 = 0xEE02,
    /// <summary>
    /// The video clip off16.
    /// </summary>
    VideoClipOff16 = 0xEE03,
    /// <summary>
    /// The video clip off20.
    /// </summary>
    VideoClipOff20 = 0xEE04,
    /// <summary>
    /// The video clip off24.
    /// </summary>
    VideoClipOff24 = 0xEE05,
    /// <summary>
    /// The video off32.
    /// </summary>
    VideoOff32 = 0xEE06,
    /// <summary>
    /// The video off48.
    /// </summary>
    VideoOff48 = 0xEE07,
    /// <summary>
    /// The video person12.
    /// </summary>
    VideoPerson12 = 0xEE08,
    /// <summary>
    /// The video person16.
    /// </summary>
    VideoPerson16 = 0xEE09,
    /// <summary>
    /// The video person20.
    /// </summary>
    VideoPerson20 = 0xEE0A,
    /// <summary>
    /// The video person28.
    /// </summary>
    VideoPerson28 = 0xEE0B,
    /// <summary>
    /// The video person48.
    /// </summary>
    VideoPerson48 = 0xEE0C,
    /// <summary>
    /// The video person call16.
    /// </summary>
    VideoPersonCall16 = 0xEE0D,
    /// <summary>
    /// The video person call20.
    /// </summary>
    VideoPersonCall20 = 0xEE0E,
    /// <summary>
    /// The video person call24.
    /// </summary>
    VideoPersonCall24 = 0xEE0F,
    /// <summary>
    /// The video person call32.
    /// </summary>
    VideoPersonCall32 = 0xEE10,
    /// <summary>
    /// The video person off20.
    /// </summary>
    VideoPersonOff20 = 0xEE11,
    /// <summary>
    /// The video person sparkle16.
    /// </summary>
    VideoPersonSparkle16 = 0xEE12,
    /// <summary>
    /// The video person sparkle20.
    /// </summary>
    VideoPersonSparkle20 = 0xEE13,
    /// <summary>
    /// The video person sparkle24.
    /// </summary>
    VideoPersonSparkle24 = 0xEE14,
    /// <summary>
    /// The video person sparkle28.
    /// </summary>
    VideoPersonSparkle28 = 0xEE15,
    /// <summary>
    /// The video person sparkle48.
    /// </summary>
    VideoPersonSparkle48 = 0xEE16,
    /// <summary>
    /// The video person star20.
    /// </summary>
    VideoPersonStar20 = 0xEE17,
    /// <summary>
    /// The video person star off20.
    /// </summary>
    VideoPersonStarOff20 = 0xEE18,
    /// <summary>
    /// The video person star off24.
    /// </summary>
    VideoPersonStarOff24 = 0xEE19,
    /// <summary>
    /// The video play pause20.
    /// </summary>
    VideoPlayPause20 = 0xEE1A,
    /// <summary>
    /// The video prohibited16.
    /// </summary>
    VideoProhibited16 = 0xEE1B,
    /// <summary>
    /// The video prohibited20.
    /// </summary>
    VideoProhibited20 = 0xEE1C,
    /// <summary>
    /// The video prohibited24.
    /// </summary>
    VideoProhibited24 = 0xEE1D,
    /// <summary>
    /// The video prohibited28.
    /// </summary>
    VideoProhibited28 = 0xEE1E,
    /// <summary>
    /// The video recording20.
    /// </summary>
    VideoRecording20 = 0xEE1F,
    /// <summary>
    /// The video switch20.
    /// </summary>
    VideoSwitch20 = 0xEE20,
    /// <summary>
    /// The video sync20.
    /// </summary>
    VideoSync20 = 0xEE21,
    /// <summary>
    /// The virtual network20.
    /// </summary>
    VirtualNetwork20 = 0xEE22,
    /// <summary>
    /// The virtual network toolbox20.
    /// </summary>
    VirtualNetworkToolbox20 = 0xEE23,
    /// <summary>
    /// The voicemail28.
    /// </summary>
    Voicemail28 = 0xEE24,
    /// <summary>
    /// The voicemail arrow back20.
    /// </summary>
    VoicemailArrowBack20 = 0xEE25,
    /// <summary>
    /// The voicemail arrow forward20.
    /// </summary>
    VoicemailArrowForward20 = 0xEE26,
    /// <summary>
    /// The voicemail arrow subtract20.
    /// </summary>
    VoicemailArrowSubtract20 = 0xEE27,
    /// <summary>
    /// The vote20.
    /// </summary>
    Vote20 = 0xEE28,
    /// <summary>
    /// The vote24.
    /// </summary>
    Vote24 = 0xEE29,
    /// <summary>
    /// The walkie talkie20.
    /// </summary>
    WalkieTalkie20 = 0xEE2A,
    /// <summary>
    /// The wallet16.
    /// </summary>
    Wallet16 = 0xEE2B,
    /// <summary>
    /// The wallet20.
    /// </summary>
    Wallet20 = 0xEE2C,
    /// <summary>
    /// The wallet24.
    /// </summary>
    Wallet24 = 0xEE2D,
    /// <summary>
    /// The wallet28.
    /// </summary>
    Wallet28 = 0xEE2E,
    /// <summary>
    /// The wallet32.
    /// </summary>
    Wallet32 = 0xEE2F,
    /// <summary>
    /// The wallet48.
    /// </summary>
    Wallet48 = 0xEE30,
    /// <summary>
    /// The wallet credit card16.
    /// </summary>
    WalletCreditCard16 = 0xEE31,
    /// <summary>
    /// The wallet credit card20.
    /// </summary>
    WalletCreditCard20 = 0xEE32,
    /// <summary>
    /// The wallet credit card24.
    /// </summary>
    WalletCreditCard24 = 0xEE33,
    /// <summary>
    /// The wallet credit card32.
    /// </summary>
    WalletCreditCard32 = 0xEE34,
    /// <summary>
    /// The wallpaper20.
    /// </summary>
    Wallpaper20 = 0xEE35,
    /// <summary>
    /// The wand16.
    /// </summary>
    Wand16 = 0xEE36,
    /// <summary>
    /// The wand20.
    /// </summary>
    Wand20 = 0xEE37,
    /// <summary>
    /// The wand24.
    /// </summary>
    Wand24 = 0xEE38,
    /// <summary>
    /// The wand28.
    /// </summary>
    Wand28 = 0xEE39,
    /// <summary>
    /// The wand48.
    /// </summary>
    Wand48 = 0xEE3A,
    /// <summary>
    /// The warning12.
    /// </summary>
    Warning12 = 0xEE3B,
    /// <summary>
    /// The warning28.
    /// </summary>
    Warning28 = 0xEE3C,
    /// <summary>
    /// The warning shield20.
    /// </summary>
    WarningShield20 = 0xEE3D,
    /// <summary>
    /// The weather drizzle20.
    /// </summary>
    WeatherDrizzle20 = 0xEE3E,
    /// <summary>
    /// The weather drizzle24.
    /// </summary>
    WeatherDrizzle24 = 0xEE3F,
    /// <summary>
    /// The weather drizzle48.
    /// </summary>
    WeatherDrizzle48 = 0xEE40,
    /// <summary>
    /// The weather haze20.
    /// </summary>
    WeatherHaze20 = 0xEE41,
    /// <summary>
    /// The weather haze24.
    /// </summary>
    WeatherHaze24 = 0xEE42,
    /// <summary>
    /// The weather haze48.
    /// </summary>
    WeatherHaze48 = 0xEE43,
    /// <summary>
    /// The weather moon16.
    /// </summary>
    WeatherMoon16 = 0xEE44,
    /// <summary>
    /// The weather moon28.
    /// </summary>
    WeatherMoon28 = 0xEE45,
    /// <summary>
    /// The weather moon off16.
    /// </summary>
    WeatherMoonOff16 = 0xEE46,
    /// <summary>
    /// The weather moon off20.
    /// </summary>
    WeatherMoonOff20 = 0xEE47,
    /// <summary>
    /// The weather moon off24.
    /// </summary>
    WeatherMoonOff24 = 0xEE48,
    /// <summary>
    /// The weather moon off28.
    /// </summary>
    WeatherMoonOff28 = 0xEE49,
    /// <summary>
    /// The weather moon off48.
    /// </summary>
    WeatherMoonOff48 = 0xEE4A,
    /// <summary>
    /// The weather partly cloudy day16.
    /// </summary>
    WeatherPartlyCloudyDay16 = 0xEE4B,
    /// <summary>
    /// The weather sunny16.
    /// </summary>
    WeatherSunny16 = 0xEE4C,
    /// <summary>
    /// The weather sunny28.
    /// </summary>
    WeatherSunny28 = 0xEE4D,
    /// <summary>
    /// The weather sunny32.
    /// </summary>
    WeatherSunny32 = 0xEE4E,
    /// <summary>
    /// The weather sunny high20.
    /// </summary>
    WeatherSunnyHigh20 = 0xEE4F,
    /// <summary>
    /// The weather sunny high24.
    /// </summary>
    WeatherSunnyHigh24 = 0xEE50,
    /// <summary>
    /// The weather sunny high48.
    /// </summary>
    WeatherSunnyHigh48 = 0xEE51,
    /// <summary>
    /// The weather sunny low20.
    /// </summary>
    WeatherSunnyLow20 = 0xEE52,
    /// <summary>
    /// The weather sunny low24.
    /// </summary>
    WeatherSunnyLow24 = 0xEE53,
    /// <summary>
    /// The weather sunny low48.
    /// </summary>
    WeatherSunnyLow48 = 0xEE54,
    /// <summary>
    /// The web asset20.
    /// </summary>
    WebAsset20 = 0xEE55,
    /// <summary>
    /// The whiteboard48.
    /// </summary>
    Whiteboard48 = 0xEE56,
    /// <summary>
    /// The wifi lock20.
    /// </summary>
    WifiLock20 = 0xEE57,
    /// <summary>
    /// The wifi lock24.
    /// </summary>
    WifiLock24 = 0xEE58,
    /// <summary>
    /// The wifi off20.
    /// </summary>
    WifiOff20 = 0xEE59,
    /// <summary>
    /// The wifi off24.
    /// </summary>
    WifiOff24 = 0xEE5A,
    /// <summary>
    /// The wifi settings20.
    /// </summary>
    WifiSettings20 = 0xEE5B,
    /// <summary>
    /// The wifi warning20.
    /// </summary>
    WifiWarning20 = 0xEE5C,
    /// <summary>
    /// The window16.
    /// </summary>
    Window16 = 0xEE5D,
    /// <summary>
    /// The window24.
    /// </summary>
    Window24 = 0xEE5E,
    /// <summary>
    /// The window28.
    /// </summary>
    Window28 = 0xEE5F,
    /// <summary>
    /// The window32.
    /// </summary>
    Window32 = 0xEE60,
    /// <summary>
    /// The window48.
    /// </summary>
    Window48 = 0xEE61,
    /// <summary>
    /// The window ad off20.
    /// </summary>
    WindowAdOff20 = 0xEE62,
    /// <summary>
    /// The window ad person20.
    /// </summary>
    WindowAdPerson20 = 0xEE63,
    /// <summary>
    /// The window apps16.
    /// </summary>
    WindowApps16 = 0xEE64,
    /// <summary>
    /// The window apps20.
    /// </summary>
    WindowApps20 = 0xEE65,
    /// <summary>
    /// The window apps24.
    /// </summary>
    WindowApps24 = 0xEE66,
    /// <summary>
    /// The window apps28.
    /// </summary>
    WindowApps28 = 0xEE67,
    /// <summary>
    /// The window apps32.
    /// </summary>
    WindowApps32 = 0xEE68,
    /// <summary>
    /// The window apps48.
    /// </summary>
    WindowApps48 = 0xEE69,
    /// <summary>
    /// The window arrow up16.
    /// </summary>
    WindowArrowUp16 = 0xEE6A,
    /// <summary>
    /// The window arrow up20.
    /// </summary>
    WindowArrowUp20 = 0xEE6B,
    /// <summary>
    /// The window arrow up24.
    /// </summary>
    WindowArrowUp24 = 0xEE6C,
    /// <summary>
    /// The window bullet list20.
    /// </summary>
    WindowBulletList20 = 0xEE6D,
    /// <summary>
    /// The window bullet list add20.
    /// </summary>
    WindowBulletListAdd20 = 0xEE6E,
    /// <summary>
    /// The window console20.
    /// </summary>
    WindowConsole20 = 0xEE6F,
    /// <summary>
    /// The window database20.
    /// </summary>
    WindowDatabase20 = 0xEE70,
    /// <summary>
    /// The window dev edit16.
    /// </summary>
    WindowDevEdit16 = 0xEE71,
    /// <summary>
    /// The window dev edit20.
    /// </summary>
    WindowDevEdit20 = 0xEE72,
    /// <summary>
    /// The window edit20.
    /// </summary>
    WindowEdit20 = 0xEE73,
    /// <summary>
    /// The window header horizontal20.
    /// </summary>
    WindowHeaderHorizontal20 = 0xEE74,
    /// <summary>
    /// The window header horizontal off20.
    /// </summary>
    WindowHeaderHorizontalOff20 = 0xEE75,
    /// <summary>
    /// The window header vertical20.
    /// </summary>
    WindowHeaderVertical20 = 0xEE76,
    /// <summary>
    /// The window location target20.
    /// </summary>
    WindowLocationTarget20 = 0xEE77,
    /// <summary>
    /// The window multiple16.
    /// </summary>
    WindowMultiple16 = 0xEE78,
    /// <summary>
    /// The window multiple swap20.
    /// </summary>
    WindowMultipleSwap20 = 0xEE79,
    /// <summary>
    /// The window new16.
    /// </summary>
    WindowNew16 = 0xEE7A,
    /// <summary>
    /// The window new24.
    /// </summary>
    WindowNew24 = 0xEE7B,
    /// <summary>
    /// The window play20.
    /// </summary>
    WindowPlay20 = 0xEE7C,
    /// <summary>
    /// The window settings20.
    /// </summary>
    WindowSettings20 = 0xEE7D,
    /// <summary>
    /// The window text20.
    /// </summary>
    WindowText20 = 0xEE7E,
    /// <summary>
    /// The window wrench16.
    /// </summary>
    WindowWrench16 = 0xEE7F,
    /// <summary>
    /// The window wrench20.
    /// </summary>
    WindowWrench20 = 0xEE80,
    /// <summary>
    /// The window wrench24.
    /// </summary>
    WindowWrench24 = 0xEE81,
    /// <summary>
    /// The window wrench28.
    /// </summary>
    WindowWrench28 = 0xEE82,
    /// <summary>
    /// The window wrench32.
    /// </summary>
    WindowWrench32 = 0xEE83,
    /// <summary>
    /// The window wrench48.
    /// </summary>
    WindowWrench48 = 0xEE84,
    /// <summary>
    /// The wrench16.
    /// </summary>
    Wrench16 = 0xEE85,
    /// <summary>
    /// The wrench20.
    /// </summary>
    Wrench20 = 0xEE86,
    /// <summary>
    /// The wrench screwdriver20.
    /// </summary>
    WrenchScrewdriver20 = 0xEE87,
    /// <summary>
    /// The wrench screwdriver24.
    /// </summary>
    WrenchScrewdriver24 = 0xEE88,
    /// <summary>
    /// The xray20.
    /// </summary>
    Xray20 = 0xEE89,
    /// <summary>
    /// The xray24.
    /// </summary>
    Xray24 = 0xEE8A,
    /// <summary>
    /// The zoom fit16.
    /// </summary>
    ZoomFit16 = 0xEE8B,
    /// <summary>
    /// The zoom fit20.
    /// </summary>
    ZoomFit20 = 0xEE8C,
    /// <summary>
    /// The zoom fit24.
    /// </summary>
    ZoomFit24 = 0xEE8D,
    /// <summary>
    /// The zoom in16.
    /// </summary>
    ZoomIn16 = 0xEE8E,
    /// <summary>
    /// The zoom out16.
    /// </summary>
    ZoomOut16 = 0xEE8F,
    /// <summary>
    /// The braces16.
    /// </summary>
    Braces16 = 0xEE90,
    /// <summary>
    /// The braces28.
    /// </summary>
    Braces28 = 0xEE91,
    /// <summary>
    /// The braces32.
    /// </summary>
    Braces32 = 0xEE92,
    /// <summary>
    /// The braces48.
    /// </summary>
    Braces48 = 0xEE93,
    /// <summary>
    /// The branch fork32.
    /// </summary>
    BranchFork32 = 0xEE94,
    /// <summary>
    /// The calendar data bar16.
    /// </summary>
    CalendarDataBar16 = 0xEE95,
    /// <summary>
    /// The calendar data bar20.
    /// </summary>
    CalendarDataBar20 = 0xEE96,
    /// <summary>
    /// The calendar data bar24.
    /// </summary>
    CalendarDataBar24 = 0xEE97,
    /// <summary>
    /// The calendar data bar28.
    /// </summary>
    CalendarDataBar28 = 0xEE98,
    /// <summary>
    /// The clipboard3 day16.
    /// </summary>
    Clipboard3Day16 = 0xEE99,
    /// <summary>
    /// The clipboard3 day20.
    /// </summary>
    Clipboard3Day20 = 0xEE9A,
    /// <summary>
    /// The clipboard3 day24.
    /// </summary>
    Clipboard3Day24 = 0xEE9B,
    /// <summary>
    /// The clipboard day16.
    /// </summary>
    ClipboardDay16 = 0xEE9C,
    /// <summary>
    /// The clipboard day20.
    /// </summary>
    ClipboardDay20 = 0xEE9D,
    /// <summary>
    /// The clipboard day24.
    /// </summary>
    ClipboardDay24 = 0xEE9E,
    /// <summary>
    /// The clipboard month16.
    /// </summary>
    ClipboardMonth16 = 0xEE9F,
    /// <summary>
    /// The clipboard month20.
    /// </summary>
    ClipboardMonth20 = 0xEEA0,
    /// <summary>
    /// The clipboard month24.
    /// </summary>
    ClipboardMonth24 = 0xEEA1,
    /// <summary>
    /// The content view gallery24.
    /// </summary>
    ContentViewGallery24 = 0xEEA2,
    /// <summary>
    /// The content view gallery28.
    /// </summary>
    ContentViewGallery28 = 0xEEA3,
    /// <summary>
    /// The data bar vertical16.
    /// </summary>
    DataBarVertical16 = 0xEEA4,
    /// <summary>
    /// The delete12.
    /// </summary>
    Delete12 = 0xEEA5,
    /// <summary>
    /// The delete32.
    /// </summary>
    Delete32 = 0xEEA6,
    /// <summary>
    /// The form20.
    /// </summary>
    Form20 = 0xEEA7,
    /// <summary>
    /// The form24.
    /// </summary>
    Form24 = 0xEEA8,
    /// <summary>
    /// The form28.
    /// </summary>
    Form28 = 0xEEA9,
    /// <summary>
    /// The form48.
    /// </summary>
    Form48 = 0xEEAA,
    /// <summary>
    /// The mail read multiple20.
    /// </summary>
    MailReadMultiple20 = 0xEEAB,
    /// <summary>
    /// The mail read multiple32.
    /// </summary>
    MailReadMultiple32 = 0xEEAC,
    /// <summary>
    /// The megaphone loud16.
    /// </summary>
    MegaphoneLoud16 = 0xEEAD,
    /// <summary>
    /// The panel right add20.
    /// </summary>
    PanelRightAdd20 = 0xEEAE,
    /// <summary>
    /// The person note16.
    /// </summary>
    PersonNote16 = 0xEEAF,
    /// <summary>
    /// The shield globe16.
    /// </summary>
    ShieldGlobe16 = 0xEEB0,
    /// <summary>
    /// The shield globe20.
    /// </summary>
    ShieldGlobe20 = 0xEEB1,
    /// <summary>
    /// The shield globe24.
    /// </summary>
    ShieldGlobe24 = 0xEEB2,
    /// <summary>
    /// The square multiple28.
    /// </summary>
    SquareMultiple28 = 0xEEB3,
    /// <summary>
    /// The square multiple32.
    /// </summary>
    SquareMultiple32 = 0xEEB4,
    /// <summary>
    /// The square multiple48.
    /// </summary>
    SquareMultiple48 = 0xEEB5,
    /// <summary>
    /// The table calculator20.
    /// </summary>
    TableCalculator20 = 0xEEB6,
    /// <summary>
    /// The xbox controller16.
    /// </summary>
    XboxController16 = 0xEEB7,
    /// <summary>
    /// The xbox controller20.
    /// </summary>
    XboxController20 = 0xEEB8,
    /// <summary>
    /// The xbox controller24.
    /// </summary>
    XboxController24 = 0xEEB9,
    /// <summary>
    /// The xbox controller28.
    /// </summary>
    XboxController28 = 0xEEBA,
    /// <summary>
    /// The xbox controller32.
    /// </summary>
    XboxController32 = 0xEEBB,
    /// <summary>
    /// The xbox controller48.
    /// </summary>
    XboxController48 = 0xEEBC,
    /// <summary>
    /// The apps32.
    /// </summary>
    Apps32 = 0xEEBD,
    /// <summary>
    /// The arrow paragraph16.
    /// </summary>
    ArrowParagraph16 = 0xEEBE,
    /// <summary>
    /// The arrow paragraph24.
    /// </summary>
    ArrowParagraph24 = 0xEEBF,
    /// <summary>
    /// The beaker32.
    /// </summary>
    Beaker32 = 0xEEC0,
    /// <summary>
    /// The animal rabbit32.
    /// </summary>
    AnimalRabbit32 = 0xEEC1,
    /// <summary>
    /// The building retail more32.
    /// </summary>
    BuildingRetailMore32 = 0xEEC2,
    /// <summary>
    /// The calendar month32.
    /// </summary>
    CalendarMonth32 = 0xEEC3,
    /// <summary>
    /// The content view24.
    /// </summary>
    ContentView24 = 0xEEC4,
    /// <summary>
    /// The content view28.
    /// </summary>
    ContentView28 = 0xEEC5,
    /// <summary>
    /// The credit card clock20.
    /// </summary>
    CreditCardClock20 = 0xEEC6,
    /// <summary>
    /// The credit card clock24.
    /// </summary>
    CreditCardClock24 = 0xEEC7,
    /// <summary>
    /// The credit card clock28.
    /// </summary>
    CreditCardClock28 = 0xEEC8,
    /// <summary>
    /// The credit card clock32.
    /// </summary>
    CreditCardClock32 = 0xEEC9,
    /// <summary>
    /// The cube32.
    /// </summary>
    Cube32 = 0xEECA,
    /// <summary>
    /// The data bar vertical32.
    /// </summary>
    DataBarVertical32 = 0xEECB,
    /// <summary>
    /// The database32.
    /// </summary>
    Database32 = 0xEECC,
    /// <summary>
    /// The document data32.
    /// </summary>
    DocumentData32 = 0xEECD,
    /// <summary>
    /// The folder people20.
    /// </summary>
    FolderPeople20 = 0xEECE,
    /// <summary>
    /// The folder people24.
    /// </summary>
    FolderPeople24 = 0xEECF,
    /// <summary>
    /// The gauge32.
    /// </summary>
    Gauge32 = 0xEED0,
    /// <summary>
    /// The hand left chat16.
    /// </summary>
    HandLeftChat16 = 0xEED1,
    /// <summary>
    /// The hand left chat20.
    /// </summary>
    HandLeftChat20 = 0xEED2,
    /// <summary>
    /// The hand left chat24.
    /// </summary>
    HandLeftChat24 = 0xEED3,
    /// <summary>
    /// The hand left chat28.
    /// </summary>
    HandLeftChat28 = 0xEED4,
    /// <summary>
    /// The home database24.
    /// </summary>
    HomeDatabase24 = 0xEED5,
    /// <summary>
    /// The home database32.
    /// </summary>
    HomeDatabase32 = 0xEED6,
    /// <summary>
    /// The home more24.
    /// </summary>
    HomeMore24 = 0xEED7,
    /// <summary>
    /// The home more32.
    /// </summary>
    HomeMore32 = 0xEED8,
    /// <summary>
    /// The notebook32.
    /// </summary>
    Notebook32 = 0xEED9,
    /// <summary>
    /// The payment32.
    /// </summary>
    Payment32 = 0xEEDA,
    /// <summary>
    /// The payment48.
    /// </summary>
    Payment48 = 0xEEDB,
    /// <summary>
    /// The person running20.
    /// </summary>
    PersonRunning20 = 0xEEDC,
    /// <summary>
    /// The pipeline24.
    /// </summary>
    Pipeline24 = 0xEEDD,
    /// <summary>
    /// The pipeline32.
    /// </summary>
    Pipeline32 = 0xEEDE,
    /// <summary>
    /// The stack32.
    /// </summary>
    Stack32 = 0xEEDF,
    /// <summary>
    /// The text align justify low rotate27020.
    /// </summary>
    TextAlignJustifyLowRotate27020 = 0xEEE0,
    /// <summary>
    /// The text align justify low rotate27024.
    /// </summary>
    TextAlignJustifyLowRotate27024 = 0xEEE1,
    /// <summary>
    /// The text align justify low rotate9020.
    /// </summary>
    TextAlignJustifyLowRotate9020 = 0xEEE2,
    /// <summary>
    /// The text align justify low rotate9024.
    /// </summary>
    TextAlignJustifyLowRotate9024 = 0xEEE3,
    /// <summary>
    /// The animal rabbit off20.
    /// </summary>
    AnimalRabbitOff20 = 0xEEE4,
    /// <summary>
    /// The animal rabbit off32.
    /// </summary>
    AnimalRabbitOff32 = 0xEEE5,
    /// <summary>
    /// The beaker off20.
    /// </summary>
    BeakerOff20 = 0xEEE6,
    /// <summary>
    /// The beaker off32.
    /// </summary>
    BeakerOff32 = 0xEEE7,
    /// <summary>
    /// The bowl salad20.
    /// </summary>
    BowlSalad20 = 0xEEE8,
    /// <summary>
    /// The bowl salad24.
    /// </summary>
    BowlSalad24 = 0xEEE9,
    /// <summary>
    /// The building retail more24.
    /// </summary>
    BuildingRetailMore24 = 0xEEEA,
    /// <summary>
    /// The connected16.
    /// </summary>
    Connected16 = 0xEEEB,
    /// <summary>
    /// The connected20.
    /// </summary>
    Connected20 = 0xEEEC,
    /// <summary>
    /// The document text16.
    /// </summary>
    DocumentText16 = 0xEEED,
    /// <summary>
    /// The drink bottle20.
    /// </summary>
    DrinkBottle20 = 0xEEEE,
    /// <summary>
    /// The drink bottle32.
    /// </summary>
    DrinkBottle32 = 0xEEEF,
    /// <summary>
    /// The drink bottle off20.
    /// </summary>
    DrinkBottleOff20 = 0xEEF0,
    /// <summary>
    /// The drink bottle off32.
    /// </summary>
    DrinkBottleOff32 = 0xEEF1,
    /// <summary>
    /// The earth32.
    /// </summary>
    Earth32 = 0xEEF2,
    /// <summary>
    /// The earth leaf16.
    /// </summary>
    EarthLeaf16 = 0xEEF3,
    /// <summary>
    /// The earth leaf20.
    /// </summary>
    EarthLeaf20 = 0xEEF4,
    /// <summary>
    /// The earth leaf24.
    /// </summary>
    EarthLeaf24 = 0xEEF5,
    /// <summary>
    /// The earth leaf32.
    /// </summary>
    EarthLeaf32 = 0xEEF6,
    /// <summary>
    /// The feed16.
    /// </summary>
    Feed16 = 0xEEF7,
    /// <summary>
    /// The feed20.
    /// </summary>
    Feed20 = 0xEEF8,
    /// <summary>
    /// The feed24.
    /// </summary>
    Feed24 = 0xEEF9,
    /// <summary>
    /// The feed28.
    /// </summary>
    Feed28 = 0xEEFA,
    /// <summary>
    /// The filmstrip20.
    /// </summary>
    Filmstrip20 = 0xEEFB,
    /// <summary>
    /// The filmstrip24.
    /// </summary>
    Filmstrip24 = 0xEEFC,
    /// <summary>
    /// The food carrot20.
    /// </summary>
    FoodCarrot20 = 0xEEFD,
    /// <summary>
    /// The food carrot24.
    /// </summary>
    FoodCarrot24 = 0xEEFE,
    /// <summary>
    /// The food fish20.
    /// </summary>
    FoodFish20 = 0xEEFF,
    /// <summary>
    /// The food fish24.
    /// </summary>
    FoodFish24 = 0xEF00,
    /// <summary>
    /// The hand open heart20.
    /// </summary>
    HandOpenHeart20 = 0xEF01,
    /// <summary>
    /// The hand open heart32.
    /// </summary>
    HandOpenHeart32 = 0xEF02,
    /// <summary>
    /// The hand wave16.
    /// </summary>
    HandWave16 = 0xEF03,
    /// <summary>
    /// The hand wave20.
    /// </summary>
    HandWave20 = 0xEF04,
    /// <summary>
    /// The hand wave24.
    /// </summary>
    HandWave24 = 0xEF05,
    /// <summary>
    /// The handshake32.
    /// </summary>
    Handshake32 = 0xEF06,
    /// <summary>
    /// The leaf one32.
    /// </summary>
    LeafOne32 = 0xEF07,
    /// <summary>
    /// The leaf two32.
    /// </summary>
    LeafTwo32 = 0xEF08,
    /// <summary>
    /// The notebook16.
    /// </summary>
    Notebook16 = 0xEF09,
    /// <summary>
    /// The person heart20.
    /// </summary>
    PersonHeart20 = 0xEF0A,
    /// <summary>
    /// The person star16.
    /// </summary>
    PersonStar16 = 0xEF0B,
    /// <summary>
    /// The person star20.
    /// </summary>
    PersonStar20 = 0xEF0C,
    /// <summary>
    /// The person star24.
    /// </summary>
    PersonStar24 = 0xEF0D,
    /// <summary>
    /// The person star28.
    /// </summary>
    PersonStar28 = 0xEF0E,
    /// <summary>
    /// The person star32.
    /// </summary>
    PersonStar32 = 0xEF0F,
    /// <summary>
    /// The person star48.
    /// </summary>
    PersonStar48 = 0xEF10,
    /// <summary>
    /// The pipeline add32.
    /// </summary>
    PipelineAdd32 = 0xEF11,
    /// <summary>
    /// The recycle20.
    /// </summary>
    Recycle20 = 0xEF12,
    /// <summary>
    /// The recycle32.
    /// </summary>
    Recycle32 = 0xEF13,
    /// <summary>
    /// The reward12.
    /// </summary>
    Reward12 = 0xEF14,
    /// <summary>
    /// The slide link20.
    /// </summary>
    SlideLink20 = 0xEF15,
    /// <summary>
    /// The slide link24.
    /// </summary>
    SlideLink24 = 0xEF16,
    /// <summary>
    /// The food chicken leg16.
    /// </summary>
    FoodChickenLeg16 = 0xEF17,
    /// <summary>
    /// The food chicken leg20.
    /// </summary>
    FoodChickenLeg20 = 0xEF18,
    /// <summary>
    /// The food chicken leg24.
    /// </summary>
    FoodChickenLeg24 = 0xEF19,
    /// <summary>
    /// The food chicken leg32.
    /// </summary>
    FoodChickenLeg32 = 0xEF1A,
    /// <summary>
    /// The form multiple20.
    /// </summary>
    FormMultiple20 = 0xEF1B,
    /// <summary>
    /// The form multiple24.
    /// </summary>
    FormMultiple24 = 0xEF1C,
    /// <summary>
    /// The form multiple28.
    /// </summary>
    FormMultiple28 = 0xEF1D,
    /// <summary>
    /// The form multiple48.
    /// </summary>
    FormMultiple48 = 0xEF1E,
    /// <summary>
    /// The laser tool20.
    /// </summary>
    LaserTool20 = 0xEF1F,
    /// <summary>
    /// The shield32.
    /// </summary>
    Shield32 = 0xEF20,
    /// <summary>
    /// The shield question16.
    /// </summary>
    ShieldQuestion16 = 0xEF21,
    /// <summary>
    /// The shield question20.
    /// </summary>
    ShieldQuestion20 = 0xEF22,
    /// <summary>
    /// The shield question24.
    /// </summary>
    ShieldQuestion24 = 0xEF23,
    /// <summary>
    /// The shield question32.
    /// </summary>
    ShieldQuestion32 = 0xEF24,
    /// <summary>
    /// The heart broken24.
    /// </summary>
    HeartBroken24 = 0xEF25,
    /// <summary>
    /// The layer diagonal20.
    /// </summary>
    LayerDiagonal20 = 0xEF26,
    /// <summary>
    /// The layer diagonal person20.
    /// </summary>
    LayerDiagonalPerson20 = 0xEF27,
    /// <summary>
    /// The text wrap16.
    /// </summary>
    TextWrap16 = 0xEF28,
    /// <summary>
    /// The text wrap off16.
    /// </summary>
    TextWrapOff16 = 0xEF29,
    /// <summary>
    /// The text wrap off20.
    /// </summary>
    TextWrapOff20 = 0xEF2A,
    /// <summary>
    /// The text wrap off24.
    /// </summary>
    TextWrapOff24 = 0xEF2B,
    /// <summary>
    /// The trophy lock16.
    /// </summary>
    TrophyLock16 = 0xEF2C,
    /// <summary>
    /// The trophy lock20.
    /// </summary>
    TrophyLock20 = 0xEF2D,
    /// <summary>
    /// The trophy lock24.
    /// </summary>
    TrophyLock24 = 0xEF2E,
    /// <summary>
    /// The trophy lock28.
    /// </summary>
    TrophyLock28 = 0xEF2F,
    /// <summary>
    /// The trophy lock32.
    /// </summary>
    TrophyLock32 = 0xEF30,
    /// <summary>
    /// The trophy lock48.
    /// </summary>
    TrophyLock48 = 0xEF31,
    /// <summary>
    /// The arrow repeat116.
    /// </summary>
    ArrowRepeat116 = 0xEF32,
    /// <summary>
    /// The arrow repeat120.
    /// </summary>
    ArrowRepeat120 = 0xEF33,
    /// <summary>
    /// The arrow repeat124.
    /// </summary>
    ArrowRepeat124 = 0xEF34,
    /// <summary>
    /// The arrow shuffle16.
    /// </summary>
    ArrowShuffle16 = 0xEF35,
    /// <summary>
    /// The arrow shuffle20.
    /// </summary>
    ArrowShuffle20 = 0xEF36,
    /// <summary>
    /// The arrow shuffle24.
    /// </summary>
    ArrowShuffle24 = 0xEF37,
    /// <summary>
    /// The arrow shuffle28.
    /// </summary>
    ArrowShuffle28 = 0xEF38,
    /// <summary>
    /// The arrow shuffle32.
    /// </summary>
    ArrowShuffle32 = 0xEF39,
    /// <summary>
    /// The arrow shuffle48.
    /// </summary>
    ArrowShuffle48 = 0xEF3A,
    /// <summary>
    /// The arrow shuffle off16.
    /// </summary>
    ArrowShuffleOff16 = 0xEF3B,
    /// <summary>
    /// The arrow shuffle off20.
    /// </summary>
    ArrowShuffleOff20 = 0xEF3C,
    /// <summary>
    /// The arrow shuffle off24.
    /// </summary>
    ArrowShuffleOff24 = 0xEF3D,
    /// <summary>
    /// The arrow shuffle off28.
    /// </summary>
    ArrowShuffleOff28 = 0xEF3E,
    /// <summary>
    /// The arrow shuffle off32.
    /// </summary>
    ArrowShuffleOff32 = 0xEF3F,
    /// <summary>
    /// The arrow shuffle off48.
    /// </summary>
    ArrowShuffleOff48 = 0xEF40,
    /// <summary>
    /// The building desktop16.
    /// </summary>
    BuildingDesktop16 = 0xEF41,
    /// <summary>
    /// The building desktop20.
    /// </summary>
    BuildingDesktop20 = 0xEF42,
    /// <summary>
    /// The building desktop24.
    /// </summary>
    BuildingDesktop24 = 0xEF43,
    /// <summary>
    /// The calendar empty48.
    /// </summary>
    CalendarEmpty48 = 0xEF44,
    /// <summary>
    /// The calendar lock16.
    /// </summary>
    CalendarLock16 = 0xEF45,
    /// <summary>
    /// The calendar lock20.
    /// </summary>
    CalendarLock20 = 0xEF46,
    /// <summary>
    /// The calendar lock24.
    /// </summary>
    CalendarLock24 = 0xEF47,
    /// <summary>
    /// The calendar lock28.
    /// </summary>
    CalendarLock28 = 0xEF48,
    /// <summary>
    /// The calendar lock32.
    /// </summary>
    CalendarLock32 = 0xEF49,
    /// <summary>
    /// The calendar lock48.
    /// </summary>
    CalendarLock48 = 0xEF4A,
    /// <summary>
    /// The calendar settings24.
    /// </summary>
    CalendarSettings24 = 0xEF4B,
    /// <summary>
    /// The calendar settings28.
    /// </summary>
    CalendarSettings28 = 0xEF4C,
    /// <summary>
    /// The calendar settings32.
    /// </summary>
    CalendarSettings32 = 0xEF4D,
    /// <summary>
    /// The calendar settings48.
    /// </summary>
    CalendarSettings48 = 0xEF4E,
    /// <summary>
    /// The call12.
    /// </summary>
    Call12 = 0xEF4F,
    /// <summary>
    /// The call missed12.
    /// </summary>
    CallMissed12 = 0xEF50,
    /// <summary>
    /// The chat add16.
    /// </summary>
    ChatAdd16 = 0xEF51,
    /// <summary>
    /// The chat add20.
    /// </summary>
    ChatAdd20 = 0xEF52,
    /// <summary>
    /// The chat add24.
    /// </summary>
    ChatAdd24 = 0xEF53,
    /// <summary>
    /// The chat add28.
    /// </summary>
    ChatAdd28 = 0xEF54,
    /// <summary>
    /// The chat add32.
    /// </summary>
    ChatAdd32 = 0xEF55,
    /// <summary>
    /// The chat add48.
    /// </summary>
    ChatAdd48 = 0xEF56,
    /// <summary>
    /// The chat cursor16.
    /// </summary>
    ChatCursor16 = 0xEF57,
    /// <summary>
    /// The chat cursor20.
    /// </summary>
    ChatCursor20 = 0xEF58,
    /// <summary>
    /// The chat cursor24.
    /// </summary>
    ChatCursor24 = 0xEF59,
    /// <summary>
    /// The chat empty12.
    /// </summary>
    ChatEmpty12 = 0xEF5A,
    /// <summary>
    /// The chat empty16.
    /// </summary>
    ChatEmpty16 = 0xEF5B,
    /// <summary>
    /// The chat empty20.
    /// </summary>
    ChatEmpty20 = 0xEF5C,
    /// <summary>
    /// The chat empty24.
    /// </summary>
    ChatEmpty24 = 0xEF5D,
    /// <summary>
    /// The chat empty28.
    /// </summary>
    ChatEmpty28 = 0xEF5E,
    /// <summary>
    /// The chat empty32.
    /// </summary>
    ChatEmpty32 = 0xEF5F,
    /// <summary>
    /// The chat empty48.
    /// </summary>
    ChatEmpty48 = 0xEF60,
    /// <summary>
    /// The circle image16.
    /// </summary>
    CircleImage16 = 0xEF61,
    /// <summary>
    /// The circle image24.
    /// </summary>
    CircleImage24 = 0xEF62,
    /// <summary>
    /// The circle image28.
    /// </summary>
    CircleImage28 = 0xEF63,
    /// <summary>
    /// The code text16.
    /// </summary>
    CodeText16 = 0xEF64,
    /// <summary>
    /// The desktop checkmark16.
    /// </summary>
    DesktopCheckmark16 = 0xEF65,
    /// <summary>
    /// The desktop checkmark20.
    /// </summary>
    DesktopCheckmark20 = 0xEF66,
    /// <summary>
    /// The desktop checkmark24.
    /// </summary>
    DesktopCheckmark24 = 0xEF67,
    /// <summary>
    /// The fire16.
    /// </summary>
    Fire16 = 0xEF68,
    /// <summary>
    /// The fire20.
    /// </summary>
    Fire20 = 0xEF69,
    /// <summary>
    /// The fire24.
    /// </summary>
    Fire24 = 0xEF6A,
    /// <summary>
    /// The hourglass20.
    /// </summary>
    Hourglass20 = 0xEF6B,
    /// <summary>
    /// The hourglass24.
    /// </summary>
    Hourglass24 = 0xEF6C,
    /// <summary>
    /// The hourglass half20.
    /// </summary>
    HourglassHalf20 = 0xEF6D,
    /// <summary>
    /// The hourglass half24.
    /// </summary>
    HourglassHalf24 = 0xEF6E,
    /// <summary>
    /// The hourglass one quarter20.
    /// </summary>
    HourglassOneQuarter20 = 0xEF6F,
    /// <summary>
    /// The hourglass one quarter24.
    /// </summary>
    HourglassOneQuarter24 = 0xEF70,
    /// <summary>
    /// The hourglass three quarter20.
    /// </summary>
    HourglassThreeQuarter20 = 0xEF71,
    /// <summary>
    /// The hourglass three quarter24.
    /// </summary>
    HourglassThreeQuarter24 = 0xEF72,
    /// <summary>
    /// The ink stroke arrow down20.
    /// </summary>
    InkStrokeArrowDown20 = 0xEF73,
    /// <summary>
    /// The ink stroke arrow down24.
    /// </summary>
    InkStrokeArrowDown24 = 0xEF74,
    /// <summary>
    /// The ink stroke arrow up down20.
    /// </summary>
    InkStrokeArrowUpDown20 = 0xEF75,
    /// <summary>
    /// The ink stroke arrow up down24.
    /// </summary>
    InkStrokeArrowUpDown24 = 0xEF76,
    /// <summary>
    /// The megaphone circle20.
    /// </summary>
    MegaphoneCircle20 = 0xEF77,
    /// <summary>
    /// The megaphone circle24.
    /// </summary>
    MegaphoneCircle24 = 0xEF78,
    /// <summary>
    /// The location arrow left20.
    /// </summary>
    LocationArrowLeft20 = 0xEF79,
    /// <summary>
    /// The location arrow right20.
    /// </summary>
    LocationArrowRight20 = 0xEF7A,
    /// <summary>
    /// The location arrow up20.
    /// </summary>
    LocationArrowUp20 = 0xEF7B,
    /// <summary>
    /// The notebook section arrow right20.
    /// </summary>
    NotebookSectionArrowRight20 = 0xEF7C,
    /// <summary>
    /// The person search20.
    /// </summary>
    PersonSearch20 = 0xEF7D,
    /// <summary>
    /// The person search24.
    /// </summary>
    PersonSearch24 = 0xEF7E,
    /// <summary>
    /// The re order20.
    /// </summary>
    ReOrder20 = 0xEF7F,
    /// <summary>
    /// The text add T20.
    /// </summary>
    TextAddT20 = 0xEF80,
    /// <summary>
    /// The text align justify low9020.
    /// </summary>
    TextAlignJustifyLow9020 = 0xEF81,
    /// <summary>
    /// The text align justify low9024.
    /// </summary>
    TextAlignJustifyLow9024 = 0xEF82,
    /// <summary>
    /// The text bullet list LTR9020.
    /// </summary>
    TextBulletListLtr9020 = 0xEF83,
    /// <summary>
    /// The text bullet list LTR9024.
    /// </summary>
    TextBulletListLtr9024 = 0xEF84,
    /// <summary>
    /// The text bullet list LTR rotate27024.
    /// </summary>
    TextBulletListLtrRotate27024 = 0xEF85,
    /// <summary>
    /// The text bullet list RTL9020.
    /// </summary>
    TextBulletListRtl9020 = 0xEF86,
    /// <summary>
    /// The text description LTR20.
    /// </summary>
    TextDescriptionLtr20 = 0xEF87,
    /// <summary>
    /// The text description LTR24.
    /// </summary>
    TextDescriptionLtr24 = 0xEF88,
    /// <summary>
    /// The text description RTL20.
    /// </summary>
    TextDescriptionRtl20 = 0xEF89,
    /// <summary>
    /// The text description RTL24.
    /// </summary>
    TextDescriptionRtl24 = 0xEF8A,
    /// <summary>
    /// The text indent decrease LTR9020.
    /// </summary>
    TextIndentDecreaseLtr9020 = 0xEF8B,
    /// <summary>
    /// The text indent decrease LTR9024.
    /// </summary>
    TextIndentDecreaseLtr9024 = 0xEF8C,
    /// <summary>
    /// The text indent decrease LTR rotate27020.
    /// </summary>
    TextIndentDecreaseLtrRotate27020 = 0xEF8D,
    /// <summary>
    /// The text indent decrease LTR rotate27024.
    /// </summary>
    TextIndentDecreaseLtrRotate27024 = 0xEF8E,
    /// <summary>
    /// The text indent decrease RTL9020.
    /// </summary>
    TextIndentDecreaseRtl9020 = 0xEF8F,
    /// <summary>
    /// The text indent decrease RTL9024.
    /// </summary>
    TextIndentDecreaseRtl9024 = 0xEF90,
    /// <summary>
    /// The person alert16.
    /// </summary>
    PersonAlert16 = 0xEF91,
    /// <summary>
    /// The person alert20.
    /// </summary>
    PersonAlert20 = 0xEF92,
    /// <summary>
    /// The person alert24.
    /// </summary>
    PersonAlert24 = 0xEF93,
    /// <summary>
    /// The person arrow back16.
    /// </summary>
    PersonArrowBack16 = 0xEF94,
    /// <summary>
    /// The person arrow back20.
    /// </summary>
    PersonArrowBack20 = 0xEF95,
    /// <summary>
    /// The person arrow back24.
    /// </summary>
    PersonArrowBack24 = 0xEF96,
    /// <summary>
    /// The person arrow back28.
    /// </summary>
    PersonArrowBack28 = 0xEF97,
    /// <summary>
    /// The person arrow back32.
    /// </summary>
    PersonArrowBack32 = 0xEF98,
    /// <summary>
    /// The person arrow back48.
    /// </summary>
    PersonArrowBack48 = 0xEF99,
    /// <summary>
    /// The person link16.
    /// </summary>
    PersonLink16 = 0xEF9A,
    /// <summary>
    /// The person link20.
    /// </summary>
    PersonLink20 = 0xEF9B,
    /// <summary>
    /// The person link24.
    /// </summary>
    PersonLink24 = 0xEF9C,
    /// <summary>
    /// The person link28.
    /// </summary>
    PersonLink28 = 0xEF9D,
    /// <summary>
    /// The person link32.
    /// </summary>
    PersonLink32 = 0xEF9E,
    /// <summary>
    /// The person link48.
    /// </summary>
    PersonLink48 = 0xEF9F,
    /// <summary>
    /// The phone28.
    /// </summary>
    Phone28 = 0xEFA0,
    /// <summary>
    /// The phone32.
    /// </summary>
    Phone32 = 0xEFA1,
    /// <summary>
    /// The phone48.
    /// </summary>
    Phone48 = 0xEFA2,
    /// <summary>
    /// The phone chat16.
    /// </summary>
    PhoneChat16 = 0xEFA3,
    /// <summary>
    /// The phone chat20.
    /// </summary>
    PhoneChat20 = 0xEFA4,
    /// <summary>
    /// The phone chat24.
    /// </summary>
    PhoneChat24 = 0xEFA5,
    /// <summary>
    /// The phone chat28.
    /// </summary>
    PhoneChat28 = 0xEFA6,
    /// <summary>
    /// The premium12.
    /// </summary>
    Premium12 = 0xEFA7,
    /// <summary>
    /// The shield add16.
    /// </summary>
    ShieldAdd16 = 0xEFA8,
    /// <summary>
    /// The shield add20.
    /// </summary>
    ShieldAdd20 = 0xEFA9,
    /// <summary>
    /// The shield add24.
    /// </summary>
    ShieldAdd24 = 0xEFAA,
    /// <summary>
    /// The sparkle circle20.
    /// </summary>
    SparkleCircle20 = 0xEFAB,
    /// <summary>
    /// The sparkle circle24.
    /// </summary>
    SparkleCircle24 = 0xEFAC,
    /// <summary>
    /// The task list square LTR16.
    /// </summary>
    TaskListSquareLtr16 = 0xEFAD,
    /// <summary>
    /// The task list square RTL16.
    /// </summary>
    TaskListSquareRtl16 = 0xEFAE,
    /// <summary>
    /// The text indent decrease RTL rotate27020.
    /// </summary>
    TextIndentDecreaseRtlRotate27020 = 0xEFAF,
    /// <summary>
    /// The text indent decrease RTL rotate27024.
    /// </summary>
    TextIndentDecreaseRtlRotate27024 = 0xEFB0,
    /// <summary>
    /// The text direction horizontal LTR20.
    /// </summary>
    TextDirectionHorizontalLtr20 = 0xEFB1,
    /// <summary>
    /// The text direction horizontal LTR24.
    /// </summary>
    TextDirectionHorizontalLtr24 = 0xEFB2,
    /// <summary>
    /// The text direction horizontal RTL20.
    /// </summary>
    TextDirectionHorizontalRtl20 = 0xEFB3,
    /// <summary>
    /// The text direction horizontal RTL24.
    /// </summary>
    TextDirectionHorizontalRtl24 = 0xEFB4,
    /// <summary>
    /// The text direction rotate90 LTR20.
    /// </summary>
    TextDirectionRotate90Ltr20 = 0xEFB5,
    /// <summary>
    /// The text direction rotate90 LTR24.
    /// </summary>
    TextDirectionRotate90Ltr24 = 0xEFB6,
    /// <summary>
    /// The text direction rotate90 RTL20.
    /// </summary>
    TextDirectionRotate90Rtl20 = 0xEFB7,
    /// <summary>
    /// The text direction rotate90 RTL24.
    /// </summary>
    TextDirectionRotate90Rtl24 = 0xEFB8,
    /// <summary>
    /// The application generic32.
    /// </summary>
    AppGeneric32 = 0xEFB9,
    /// <summary>
    /// The code block16.
    /// </summary>
    CodeBlock16 = 0xEFBA,
    /// <summary>
    /// The code block20.
    /// </summary>
    CodeBlock20 = 0xEFBB,
    /// <summary>
    /// The code block24.
    /// </summary>
    CodeBlock24 = 0xEFBC,
    /// <summary>
    /// The code block28.
    /// </summary>
    CodeBlock28 = 0xEFBD,
    /// <summary>
    /// The code block32.
    /// </summary>
    CodeBlock32 = 0xEFBE,
    /// <summary>
    /// The code block48.
    /// </summary>
    CodeBlock48 = 0xEFBF,
    /// <summary>
    /// The data bar vertical star16.
    /// </summary>
    DataBarVerticalStar16 = 0xEFC0,
    /// <summary>
    /// The data bar vertical star20.
    /// </summary>
    DataBarVerticalStar20 = 0xEFC1,
    /// <summary>
    /// The data bar vertical star24.
    /// </summary>
    DataBarVerticalStar24 = 0xEFC2,
    /// <summary>
    /// The data bar vertical star32.
    /// </summary>
    DataBarVerticalStar32 = 0xEFC3,
    /// <summary>
    /// The database arrow right32.
    /// </summary>
    DatabaseArrowRight32 = 0xEFC4,
    /// <summary>
    /// The document sync32.
    /// </summary>
    DocumentSync32 = 0xEFC5,
    /// <summary>
    /// The equal off12.
    /// </summary>
    EqualOff12 = 0xEFC6,
    /// <summary>
    /// The equal off16.
    /// </summary>
    EqualOff16 = 0xEFC7,
    /// <summary>
    /// The eye28.
    /// </summary>
    Eye28 = 0xEFC8,
    /// <summary>
    /// The eye32.
    /// </summary>
    Eye32 = 0xEFC9,
    /// <summary>
    /// The eye48.
    /// </summary>
    Eye48 = 0xEFCA,
    /// <summary>
    /// The eye lines20.
    /// </summary>
    EyeLines20 = 0xEFCB,
    /// <summary>
    /// The eye lines24.
    /// </summary>
    EyeLines24 = 0xEFCC,
    /// <summary>
    /// The eye lines28.
    /// </summary>
    EyeLines28 = 0xEFCD,
    /// <summary>
    /// The eye lines32.
    /// </summary>
    EyeLines32 = 0xEFCE,
    /// <summary>
    /// The eye lines48.
    /// </summary>
    EyeLines48 = 0xEFCF,
    /// <summary>
    /// The text indent increase LTR9020.
    /// </summary>
    TextIndentIncreaseLtr9020 = 0xEFD0,
    /// <summary>
    /// The text indent increase LTR9024.
    /// </summary>
    TextIndentIncreaseLtr9024 = 0xEFD1,
    /// <summary>
    /// The text indent increase LTR rotate27020.
    /// </summary>
    TextIndentIncreaseLtrRotate27020 = 0xEFD2,
    /// <summary>
    /// The text bullet list square person32.
    /// </summary>
    TextBulletListSquarePerson32 = 0xEFD3,
    /// <summary>
    /// The weather snowflake32.
    /// </summary>
    WeatherSnowflake32 = 0xEFD4,
    /// <summary>
    /// The window database24.
    /// </summary>
    WindowDatabase24 = 0xEFD5,
    /// <summary>
    /// The arrow trending12.
    /// </summary>
    ArrowTrending12 = 0xEFD6,
    /// <summary>
    /// The building people16.
    /// </summary>
    BuildingPeople16 = 0xEFD7,
    /// <summary>
    /// The building people20.
    /// </summary>
    BuildingPeople20 = 0xEFD8,
    /// <summary>
    /// The building people24.
    /// </summary>
    BuildingPeople24 = 0xEFD9,
    /// <summary>
    /// The cloud error16.
    /// </summary>
    CloudError16 = 0xEFDA,
    /// <summary>
    /// The cloud error20.
    /// </summary>
    CloudError20 = 0xEFDB,
    /// <summary>
    /// The cloud error24.
    /// </summary>
    CloudError24 = 0xEFDC,
    /// <summary>
    /// The cloud error28.
    /// </summary>
    CloudError28 = 0xEFDD,
    /// <summary>
    /// The cloud error32.
    /// </summary>
    CloudError32 = 0xEFDE,
    /// <summary>
    /// The cloud error48.
    /// </summary>
    CloudError48 = 0xEFDF,
    /// <summary>
    /// The couch32.
    /// </summary>
    Couch32 = 0xEFE0,
    /// <summary>
    /// The couch48.
    /// </summary>
    Couch48 = 0xEFE1,
    /// <summary>
    /// The database arrow right24.
    /// </summary>
    DatabaseArrowRight24 = 0xEFE2,
    /// <summary>
    /// The dishwasher20.
    /// </summary>
    Dishwasher20 = 0xEFE3,
    /// <summary>
    /// The dishwasher24.
    /// </summary>
    Dishwasher24 = 0xEFE4,
    /// <summary>
    /// The dishwasher32.
    /// </summary>
    Dishwasher32 = 0xEFE5,
    /// <summary>
    /// The dishwasher48.
    /// </summary>
    Dishwasher48 = 0xEFE6,
    /// <summary>
    /// The elevator20.
    /// </summary>
    Elevator20 = 0xEFE7,
    /// <summary>
    /// The elevator24.
    /// </summary>
    Elevator24 = 0xEFE8,
    /// <summary>
    /// The elevator32.
    /// </summary>
    Elevator32 = 0xEFE9,
    /// <summary>
    /// The feed32.
    /// </summary>
    Feed32 = 0xEFEA,
    /// <summary>
    /// The feed48.
    /// </summary>
    Feed48 = 0xEFEB,
    /// <summary>
    /// The fireplace20.
    /// </summary>
    Fireplace20 = 0xEFEC,
    /// <summary>
    /// The fireplace24.
    /// </summary>
    Fireplace24 = 0xEFED,
    /// <summary>
    /// The fireplace32.
    /// </summary>
    Fireplace32 = 0xEFEE,
    /// <summary>
    /// The fireplace48.
    /// </summary>
    Fireplace48 = 0xEFEF,
    /// <summary>
    /// The mention12.
    /// </summary>
    Mention12 = 0xEFF0,
    /// <summary>
    /// The oven20.
    /// </summary>
    Oven20 = 0xEFF1,
    /// <summary>
    /// The oven24.
    /// </summary>
    Oven24 = 0xEFF2,
    /// <summary>
    /// The oven32.
    /// </summary>
    Oven32 = 0xEFF3,
    /// <summary>
    /// The oven48.
    /// </summary>
    Oven48 = 0xEFF4,
    /// <summary>
    /// The text indent increase LTR rotate27024.
    /// </summary>
    TextIndentIncreaseLtrRotate27024 = 0xEFF5,
    /// <summary>
    /// The panel left32.
    /// </summary>
    PanelLeft32 = 0xEFF6,
    /// <summary>
    /// The panel left add16.
    /// </summary>
    PanelLeftAdd16 = 0xEFF7,
    /// <summary>
    /// The panel left add20.
    /// </summary>
    PanelLeftAdd20 = 0xEFF8,
    /// <summary>
    /// The panel left add24.
    /// </summary>
    PanelLeftAdd24 = 0xEFF9,
    /// <summary>
    /// The panel left add28.
    /// </summary>
    PanelLeftAdd28 = 0xEFFA,
    /// <summary>
    /// The panel left add32.
    /// </summary>
    PanelLeftAdd32 = 0xEFFB,
    /// <summary>
    /// The panel left add48.
    /// </summary>
    PanelLeftAdd48 = 0xEFFC,
    /// <summary>
    /// The panel left key16.
    /// </summary>
    PanelLeftKey16 = 0xEFFD,
    /// <summary>
    /// The panel left key20.
    /// </summary>
    PanelLeftKey20 = 0xEFFE,
    /// <summary>
    /// The panel left key24.
    /// </summary>
    PanelLeftKey24 = 0xEFFF,
    /// <summary>
    /// The panel right32.
    /// </summary>
    PanelRight32 = 0xF000,
    /// <summary>
    /// The status12.
    /// </summary>
    Status12 = 0xF001,
    /// <summary>
    /// The vehicle car parking20.
    /// </summary>
    VehicleCarParking20 = 0xF002,
    /// <summary>
    /// The vehicle car parking24.
    /// </summary>
    VehicleCarParking24 = 0xF003,
    /// <summary>
    /// The vehicle car profile LTR24.
    /// </summary>
    VehicleCarProfileLtr24 = 0xF004,
    /// <summary>
    /// The vehicle car profile RTL24.
    /// </summary>
    VehicleCarProfileRtl24 = 0xF005,
    /// <summary>
    /// The washer20.
    /// </summary>
    Washer20 = 0xF006,
    /// <summary>
    /// The washer24.
    /// </summary>
    Washer24 = 0xF007,
    /// <summary>
    /// The washer32.
    /// </summary>
    Washer32 = 0xF008,
    /// <summary>
    /// The washer48.
    /// </summary>
    Washer48 = 0xF009,
    /// <summary>
    /// The accessibility checkmark28.
    /// </summary>
    AccessibilityCheckmark28 = 0xF00A,
    /// <summary>
    /// The accessibility checkmark32.
    /// </summary>
    AccessibilityCheckmark32 = 0xF00B,
    /// <summary>
    /// The accessibility checkmark48.
    /// </summary>
    AccessibilityCheckmark48 = 0xF00C,
    /// <summary>
    /// The add circle12.
    /// </summary>
    AddCircle12 = 0xF00D,
    /// <summary>
    /// The arrow turn down right20.
    /// </summary>
    ArrowTurnDownRight20 = 0xF00E,
    /// <summary>
    /// The arrow turn down right48.
    /// </summary>
    ArrowTurnDownRight48 = 0xF00F,
    /// <summary>
    /// The arrow turn down up20.
    /// </summary>
    ArrowTurnDownUp20 = 0xF010,
    /// <summary>
    /// The arrow turn down up48.
    /// </summary>
    ArrowTurnDownUp48 = 0xF011,
    /// <summary>
    /// The arrow turn left down20.
    /// </summary>
    ArrowTurnLeftDown20 = 0xF012,
    /// <summary>
    /// The arrow turn left down48.
    /// </summary>
    ArrowTurnLeftDown48 = 0xF013,
    /// <summary>
    /// The arrow turn left right20.
    /// </summary>
    ArrowTurnLeftRight20 = 0xF014,
    /// <summary>
    /// The arrow turn left right48.
    /// </summary>
    ArrowTurnLeftRight48 = 0xF015,
    /// <summary>
    /// The arrow turn left up20.
    /// </summary>
    ArrowTurnLeftUp20 = 0xF016,
    /// <summary>
    /// The arrow turn left up48.
    /// </summary>
    ArrowTurnLeftUp48 = 0xF017,
    /// <summary>
    /// The arrow turn right48.
    /// </summary>
    ArrowTurnRight48 = 0xF018,
    /// <summary>
    /// The arrow turn right down20.
    /// </summary>
    ArrowTurnRightDown20 = 0xF019,
    /// <summary>
    /// The arrow turn right down48.
    /// </summary>
    ArrowTurnRightDown48 = 0xF01A,
    /// <summary>
    /// The arrow turn right left20.
    /// </summary>
    ArrowTurnRightLeft20 = 0xF01B,
    /// <summary>
    /// The arrow turn right left48.
    /// </summary>
    ArrowTurnRightLeft48 = 0xF01C,
    /// <summary>
    /// The arrow turn right up20.
    /// </summary>
    ArrowTurnRightUp20 = 0xF01D,
    /// <summary>
    /// The arrow turn right up48.
    /// </summary>
    ArrowTurnRightUp48 = 0xF01E,
    /// <summary>
    /// The arrow turn up down20.
    /// </summary>
    ArrowTurnUpDown20 = 0xF01F,
    /// <summary>
    /// The arrow turn up down48.
    /// </summary>
    ArrowTurnUpDown48 = 0xF020,
    /// <summary>
    /// The arrow turn up left20.
    /// </summary>
    ArrowTurnUpLeft20 = 0xF021,
    /// <summary>
    /// The arrow turn up left48.
    /// </summary>
    ArrowTurnUpLeft48 = 0xF022,
    /// <summary>
    /// The building townhouse20.
    /// </summary>
    BuildingTownhouse20 = 0xF023,
    /// <summary>
    /// The building townhouse24.
    /// </summary>
    BuildingTownhouse24 = 0xF024,
    /// <summary>
    /// The building townhouse32.
    /// </summary>
    BuildingTownhouse32 = 0xF025,
    /// <summary>
    /// The camera sparkles20.
    /// </summary>
    CameraSparkles20 = 0xF026,
    /// <summary>
    /// The camera sparkles24.
    /// </summary>
    CameraSparkles24 = 0xF027,
    /// <summary>
    /// The text indent increase RTL9020.
    /// </summary>
    TextIndentIncreaseRtl9020 = 0xF028,
    /// <summary>
    /// The text indent increase RTL9024.
    /// </summary>
    TextIndentIncreaseRtl9024 = 0xF029,
    /// <summary>
    /// The chat bubbles question28.
    /// </summary>
    ChatBubblesQuestion28 = 0xF02A,
    /// <summary>
    /// The chat bubbles question32.
    /// </summary>
    ChatBubblesQuestion32 = 0xF02B,
    /// <summary>
    /// The crop16.
    /// </summary>
    Crop16 = 0xF02C,
    /// <summary>
    /// The crop28.
    /// </summary>
    Crop28 = 0xF02D,
    /// <summary>
    /// The crop32.
    /// </summary>
    Crop32 = 0xF02E,
    /// <summary>
    /// The crop48.
    /// </summary>
    Crop48 = 0xF02F,
    /// <summary>
    /// The data trending28.
    /// </summary>
    DataTrending28 = 0xF030,
    /// <summary>
    /// The data trending32.
    /// </summary>
    DataTrending32 = 0xF031,
    /// <summary>
    /// The data trending48.
    /// </summary>
    DataTrending48 = 0xF032,
    /// <summary>
    /// The document database20.
    /// </summary>
    DocumentDatabase20 = 0xF033,
    /// <summary>
    /// The document database24.
    /// </summary>
    DocumentDatabase24 = 0xF034,
    /// <summary>
    /// The earth48.
    /// </summary>
    Earth48 = 0xF035,
    /// <summary>
    /// The earth leaf48.
    /// </summary>
    EarthLeaf48 = 0xF036,
    /// <summary>
    /// The elevator48.
    /// </summary>
    Elevator48 = 0xF037,
    /// <summary>
    /// The home split20.
    /// </summary>
    HomeSplit20 = 0xF038,
    /// <summary>
    /// The home split24.
    /// </summary>
    HomeSplit24 = 0xF039,
    /// <summary>
    /// The home split32.
    /// </summary>
    HomeSplit32 = 0xF03A,
    /// <summary>
    /// The home split48.
    /// </summary>
    HomeSplit48 = 0xF03B,
    /// <summary>
    /// The leaf two48.
    /// </summary>
    LeafTwo48 = 0xF03C,
    /// <summary>
    /// The panel right cursor20.
    /// </summary>
    PanelRightCursor20 = 0xF03D,
    /// <summary>
    /// The panel right cursor24.
    /// </summary>
    PanelRightCursor24 = 0xF03E,
    /// <summary>
    /// The person board28.
    /// </summary>
    PersonBoard28 = 0xF03F,
    /// <summary>
    /// The person board32.
    /// </summary>
    PersonBoard32 = 0xF040,
    /// <summary>
    /// The person circle28.
    /// </summary>
    PersonCircle28 = 0xF041,
    /// <summary>
    /// The person circle32.
    /// </summary>
    PersonCircle32 = 0xF042,
    /// <summary>
    /// The person square20.
    /// </summary>
    PersonSquare20 = 0xF043,
    /// <summary>
    /// The person square24.
    /// </summary>
    PersonSquare24 = 0xF044,
    /// <summary>
    /// The person starburst20.
    /// </summary>
    PersonStarburst20 = 0xF045,
    /// <summary>
    /// The person starburst24.
    /// </summary>
    PersonStarburst24 = 0xF046,
    /// <summary>
    /// The receipt sparkles20.
    /// </summary>
    ReceiptSparkles20 = 0xF047,
    /// <summary>
    /// The receipt sparkles24.
    /// </summary>
    ReceiptSparkles24 = 0xF048,
    /// <summary>
    /// The ruler28.
    /// </summary>
    Ruler28 = 0xF049,
    /// <summary>
    /// The ruler32.
    /// </summary>
    Ruler32 = 0xF04A,
    /// <summary>
    /// The ruler48.
    /// </summary>
    Ruler48 = 0xF04B,
    /// <summary>
    /// The scan qr code24.
    /// </summary>
    ScanQrCode24 = 0xF04C,
    /// <summary>
    /// The showerhead20.
    /// </summary>
    Showerhead20 = 0xF04D,
    /// <summary>
    /// The showerhead24.
    /// </summary>
    Showerhead24 = 0xF04E,
    /// <summary>
    /// The showerhead32.
    /// </summary>
    Showerhead32 = 0xF04F,
    /// <summary>
    /// The slide text multiple16.
    /// </summary>
    SlideTextMultiple16 = 0xF050,
    /// <summary>
    /// The slide text multiple20.
    /// </summary>
    SlideTextMultiple20 = 0xF051,
    /// <summary>
    /// The slide text multiple24.
    /// </summary>
    SlideTextMultiple24 = 0xF052,
    /// <summary>
    /// The slide text multiple32.
    /// </summary>
    SlideTextMultiple32 = 0xF053,
    /// <summary>
    /// The swimming pool20.
    /// </summary>
    SwimmingPool20 = 0xF054,
    /// <summary>
    /// The swimming pool24.
    /// </summary>
    SwimmingPool24 = 0xF055,
    /// <summary>
    /// The swimming pool32.
    /// </summary>
    SwimmingPool32 = 0xF056,
    /// <summary>
    /// The swimming pool48.
    /// </summary>
    SwimmingPool48 = 0xF057,
    /// <summary>
    /// The temperature32.
    /// </summary>
    Temperature32 = 0xF058,
    /// <summary>
    /// The temperature48.
    /// </summary>
    Temperature48 = 0xF059,
    /// <summary>
    /// The vehicle car32.
    /// </summary>
    VehicleCar32 = 0xF05A,
    /// <summary>
    /// The vehicle car parking16.
    /// </summary>
    VehicleCarParking16 = 0xF05B,
    /// <summary>
    /// The vehicle car parking32.
    /// </summary>
    VehicleCarParking32 = 0xF05C,
    /// <summary>
    /// The vehicle car parking48.
    /// </summary>
    VehicleCarParking48 = 0xF05D,
    /// <summary>
    /// The vehicle car profile LTR clock16.
    /// </summary>
    VehicleCarProfileLtrClock16 = 0xF05E,
    /// <summary>
    /// The vehicle car profile LTR clock20.
    /// </summary>
    VehicleCarProfileLtrClock20 = 0xF05F,
    /// <summary>
    /// The vehicle car profile LTR clock24.
    /// </summary>
    VehicleCarProfileLtrClock24 = 0xF060,
    /// <summary>
    /// The video people32.
    /// </summary>
    VideoPeople32 = 0xF061,
    /// <summary>
    /// The water16.
    /// </summary>
    Water16 = 0xF062,
    /// <summary>
    /// The water20.
    /// </summary>
    Water20 = 0xF063,
    /// <summary>
    /// The water24.
    /// </summary>
    Water24 = 0xF064,
    /// <summary>
    /// The water32.
    /// </summary>
    Water32 = 0xF065,
    /// <summary>
    /// The water48.
    /// </summary>
    Water48 = 0xF066,
    /// <summary>
    /// The arrow turn down left20.
    /// </summary>
    ArrowTurnDownLeft20 = 0xF067,
    /// <summary>
    /// The arrow turn down left48.
    /// </summary>
    ArrowTurnDownLeft48 = 0xF068,
    /// <summary>
    /// The autosum16.
    /// </summary>
    Autosum16 = 0xF069,
    /// <summary>
    /// The bubble multiple20.
    /// </summary>
    BubbleMultiple20 = 0xF06A,
    /// <summary>
    /// The calculator16.
    /// </summary>
    Calculator16 = 0xF06B,
    /// <summary>
    /// The calculator multiple16.
    /// </summary>
    CalculatorMultiple16 = 0xF06C,
    /// <summary>
    /// The camera sparkles16.
    /// </summary>
    CameraSparkles16 = 0xF06D,
    /// <summary>
    /// The crown16.
    /// </summary>
    Crown16 = 0xF06E,
    /// <summary>
    /// The crown20.
    /// </summary>
    Crown20 = 0xF06F,
    /// <summary>
    /// The flag checkered20.
    /// </summary>
    FlagCheckered20 = 0xF070,
    /// <summary>
    /// The glance horizontal16.
    /// </summary>
    GlanceHorizontal16 = 0xF071,
    /// <summary>
    /// The glance horizontal sparkles16.
    /// </summary>
    GlanceHorizontalSparkles16 = 0xF072,
    /// <summary>
    /// The glance horizontal sparkles24.
    /// </summary>
    GlanceHorizontalSparkles24 = 0xF073,
    /// <summary>
    /// The grid circles24.
    /// </summary>
    GridCircles24 = 0xF074,
    /// <summary>
    /// The grid circles28.
    /// </summary>
    GridCircles28 = 0xF075,
    /// <summary>
    /// The heart circle hint16.
    /// </summary>
    HeartCircleHint16 = 0xF076,
    /// <summary>
    /// The heart circle hint20.
    /// </summary>
    HeartCircleHint20 = 0xF077,
    /// <summary>
    /// The heart circle hint24.
    /// </summary>
    HeartCircleHint24 = 0xF078,
    /// <summary>
    /// The heart circle hint28.
    /// </summary>
    HeartCircleHint28 = 0xF079,
    /// <summary>
    /// The heart circle hint32.
    /// </summary>
    HeartCircleHint32 = 0xF07A,
    /// <summary>
    /// The heart circle hint48.
    /// </summary>
    HeartCircleHint48 = 0xF07B,
    /// <summary>
    /// The lightbulb28.
    /// </summary>
    Lightbulb28 = 0xF07C,
    /// <summary>
    /// The lightbulb32.
    /// </summary>
    Lightbulb32 = 0xF07D,
    /// <summary>
    /// The lightbulb48.
    /// </summary>
    Lightbulb48 = 0xF07E,
    /// <summary>
    /// The lightbulb person16.
    /// </summary>
    LightbulbPerson16 = 0xF07F,
    /// <summary>
    /// The lightbulb person20.
    /// </summary>
    LightbulbPerson20 = 0xF080,
    /// <summary>
    /// The lightbulb person24.
    /// </summary>
    LightbulbPerson24 = 0xF081,
    /// <summary>
    /// The lightbulb person28.
    /// </summary>
    LightbulbPerson28 = 0xF082,
    /// <summary>
    /// The lightbulb person32.
    /// </summary>
    LightbulbPerson32 = 0xF083,
    /// <summary>
    /// The lightbulb person48.
    /// </summary>
    LightbulbPerson48 = 0xF084,
    /// <summary>
    /// The megaphone loud28.
    /// </summary>
    MegaphoneLoud28 = 0xF085,
    /// <summary>
    /// The megaphone loud32.
    /// </summary>
    MegaphoneLoud32 = 0xF086,
    /// <summary>
    /// The person walking20.
    /// </summary>
    PersonWalking20 = 0xF087,
    /// <summary>
    /// The person walking24.
    /// </summary>
    PersonWalking24 = 0xF088,
    /// <summary>
    /// The receipt16.
    /// </summary>
    Receipt16 = 0xF089,
    /// <summary>
    /// The receipt28.
    /// </summary>
    Receipt28 = 0xF08A,
    /// <summary>
    /// The receipt sparkles16.
    /// </summary>
    ReceiptSparkles16 = 0xF08B,
    /// <summary>
    /// The scan text16.
    /// </summary>
    ScanText16 = 0xF08C,
    /// <summary>
    /// The scan text28.
    /// </summary>
    ScanText28 = 0xF08D,
    /// <summary>
    /// The table calculator16.
    /// </summary>
    TableCalculator16 = 0xF08E,
    /// <summary>
    /// The table simple checkmark16.
    /// </summary>
    TableSimpleCheckmark16 = 0xF08F,
    /// <summary>
    /// The table simple checkmark20.
    /// </summary>
    TableSimpleCheckmark20 = 0xF090,
    /// <summary>
    /// The table simple checkmark24.
    /// </summary>
    TableSimpleCheckmark24 = 0xF091,
    /// <summary>
    /// The table simple checkmark28.
    /// </summary>
    TableSimpleCheckmark28 = 0xF092,
    /// <summary>
    /// The table simple checkmark32.
    /// </summary>
    TableSimpleCheckmark32 = 0xF093,
    /// <summary>
    /// The table simple checkmark48.
    /// </summary>
    TableSimpleCheckmark48 = 0xF094,
    /// <summary>
    /// The tabs16.
    /// </summary>
    Tabs16 = 0xF095,
    /// <summary>
    /// The text underline double20.
    /// </summary>
    TextUnderlineDouble20 = 0xF096,
    /// <summary>
    /// The text underline double24.
    /// </summary>
    TextUnderlineDouble24 = 0xF097,
    /// <summary>
    /// The xbox controller error20.
    /// </summary>
    XboxControllerError20 = 0xF098,
    /// <summary>
    /// The xbox controller error24.
    /// </summary>
    XboxControllerError24 = 0xF099,
    /// <summary>
    /// The xbox controller error32.
    /// </summary>
    XboxControllerError32 = 0xF09A,
    /// <summary>
    /// The xbox controller error48.
    /// </summary>
    XboxControllerError48 = 0xF09B,
    /// <summary>
    /// The align distribute bottom16.
    /// </summary>
    AlignDistributeBottom16 = 0xF09C,
    /// <summary>
    /// The align distribute left16.
    /// </summary>
    AlignDistributeLeft16 = 0xF09D,
    /// <summary>
    /// The align distribute right16.
    /// </summary>
    AlignDistributeRight16 = 0xF09E,
    /// <summary>
    /// The align distribute top16.
    /// </summary>
    AlignDistributeTop16 = 0xF09F,
    /// <summary>
    /// The align stretch horizontal16.
    /// </summary>
    AlignStretchHorizontal16 = 0xF0A0,
    /// <summary>
    /// The align stretch vertical16.
    /// </summary>
    AlignStretchVertical16 = 0xF0A1,
    /// <summary>
    /// The arrow next16.
    /// </summary>
    ArrowNext16 = 0xF0A2,
    /// <summary>
    /// The arrow previous16.
    /// </summary>
    ArrowPrevious16 = 0xF0A3,
    /// <summary>
    /// The braces checkmark16.
    /// </summary>
    BracesCheckmark16 = 0xF0A4,
    /// <summary>
    /// The braces dismiss16.
    /// </summary>
    BracesDismiss16 = 0xF0A5,
    /// <summary>
    /// The branch16.
    /// </summary>
    Branch16 = 0xF0A6,
    /// <summary>
    /// The calendar arrow counterclockwise16.
    /// </summary>
    CalendarArrowCounterclockwise16 = 0xF0A7,
    /// <summary>
    /// The calendar arrow counterclockwise20.
    /// </summary>
    CalendarArrowCounterclockwise20 = 0xF0A8,
    /// <summary>
    /// The calendar arrow counterclockwise24.
    /// </summary>
    CalendarArrowCounterclockwise24 = 0xF0A9,
    /// <summary>
    /// The calendar arrow counterclockwise28.
    /// </summary>
    CalendarArrowCounterclockwise28 = 0xF0AA,
    /// <summary>
    /// The calendar arrow counterclockwise32.
    /// </summary>
    CalendarArrowCounterclockwise32 = 0xF0AB,
    /// <summary>
    /// The calendar arrow counterclockwise48.
    /// </summary>
    CalendarArrowCounterclockwise48 = 0xF0AC,
    /// <summary>
    /// The calendar play16.
    /// </summary>
    CalendarPlay16 = 0xF0AD,
    /// <summary>
    /// The calendar play20.
    /// </summary>
    CalendarPlay20 = 0xF0AE,
    /// <summary>
    /// The calendar play24.
    /// </summary>
    CalendarPlay24 = 0xF0AF,
    /// <summary>
    /// The calendar play28.
    /// </summary>
    CalendarPlay28 = 0xF0B0,
    /// <summary>
    /// The calendar shield16.
    /// </summary>
    CalendarShield16 = 0xF0B1,
    /// <summary>
    /// The calendar shield20.
    /// </summary>
    CalendarShield20 = 0xF0B2,
    /// <summary>
    /// The calendar shield24.
    /// </summary>
    CalendarShield24 = 0xF0B3,
    /// <summary>
    /// The calendar shield28.
    /// </summary>
    CalendarShield28 = 0xF0B4,
    /// <summary>
    /// The calendar shield32.
    /// </summary>
    CalendarShield32 = 0xF0B5,
    /// <summary>
    /// The calendar shield48.
    /// </summary>
    CalendarShield48 = 0xF0B6,
    /// <summary>
    /// The call transfer24.
    /// </summary>
    CallTransfer24 = 0xF0B7,
    /// <summary>
    /// The call transfer32.
    /// </summary>
    CallTransfer32 = 0xF0B8,
    /// <summary>
    /// The camera off16.
    /// </summary>
    CameraOff16 = 0xF0B9,
    /// <summary>
    /// The CD16.
    /// </summary>
    Cd16 = 0xF0BA,
    /// <summary>
    /// The certificate16.
    /// </summary>
    Certificate16 = 0xF0BB,
    /// <summary>
    /// The clipboard error16.
    /// </summary>
    ClipboardError16 = 0xF0BC,
    /// <summary>
    /// The clipboard multiple16.
    /// </summary>
    ClipboardMultiple16 = 0xF0BD,
    /// <summary>
    /// The clipboard note16.
    /// </summary>
    ClipboardNote16 = 0xF0BE,
    /// <summary>
    /// The clipboard task16.
    /// </summary>
    ClipboardTask16 = 0xF0BF,
    /// <summary>
    /// The clipboard text LTR16.
    /// </summary>
    ClipboardTextLtr16 = 0xF0C0,
    /// <summary>
    /// The clipboard text RTL16.
    /// </summary>
    ClipboardTextRtl16 = 0xF0C1,
    /// <summary>
    /// The cloud add24.
    /// </summary>
    CloudAdd24 = 0xF0C2,
    /// <summary>
    /// The cloud edit24.
    /// </summary>
    CloudEdit24 = 0xF0C3,
    /// <summary>
    /// The cloud link24.
    /// </summary>
    CloudLink24 = 0xF0C4,
    /// <summary>
    /// The code CS16.
    /// </summary>
    CodeCs16 = 0xF0C5,
    /// <summary>
    /// The code cs rectangle16.
    /// </summary>
    CodeCsRectangle16 = 0xF0C6,
    /// <summary>
    /// The code FS16.
    /// </summary>
    CodeFs16 = 0xF0C7,
    /// <summary>
    /// The code fs rectangle16.
    /// </summary>
    CodeFsRectangle16 = 0xF0C8,
    /// <summary>
    /// The code JS16.
    /// </summary>
    CodeJs16 = 0xF0C9,
    /// <summary>
    /// The code js rectangle16.
    /// </summary>
    CodeJsRectangle16 = 0xF0CA,
    /// <summary>
    /// The code py16.
    /// </summary>
    CodePy16 = 0xF0CB,
    /// <summary>
    /// The code py rectangle16.
    /// </summary>
    CodePyRectangle16 = 0xF0CC,
    /// <summary>
    /// The code RB16.
    /// </summary>
    CodeRb16 = 0xF0CD,
    /// <summary>
    /// The code rb rectangle16.
    /// </summary>
    CodeRbRectangle16 = 0xF0CE,
    /// <summary>
    /// The code text off16.
    /// </summary>
    CodeTextOff16 = 0xF0CF,
    /// <summary>
    /// The code TS16.
    /// </summary>
    CodeTs16 = 0xF0D0,
    /// <summary>
    /// The code ts rectangle16.
    /// </summary>
    CodeTsRectangle16 = 0xF0D1,
    /// <summary>
    /// The code VB16.
    /// </summary>
    CodeVb16 = 0xF0D2,
    /// <summary>
    /// The code vb rectangle16.
    /// </summary>
    CodeVbRectangle16 = 0xF0D3,
    /// <summary>
    /// The cone16.
    /// </summary>
    Cone16 = 0xF0D4,
    /// <summary>
    /// The data bar horizontal descending16.
    /// </summary>
    DataBarHorizontalDescending16 = 0xF0D5,
    /// <summary>
    /// The data bar vertical ascending16.
    /// </summary>
    DataBarVerticalAscending16 = 0xF0D6,
    /// <summary>
    /// The database16.
    /// </summary>
    Database16 = 0xF0D7,
    /// <summary>
    /// The database stack16.
    /// </summary>
    DatabaseStack16 = 0xF0D8,
    /// <summary>
    /// The developer board16.
    /// </summary>
    DeveloperBoard16 = 0xF0D9,
    /// <summary>
    /// The document contract16.
    /// </summary>
    DocumentContract16 = 0xF0DA,
    /// <summary>
    /// The document CS16.
    /// </summary>
    DocumentCs16 = 0xF0DB,
    /// <summary>
    /// The document CSS16.
    /// </summary>
    DocumentCss16 = 0xF0DC,
    /// <summary>
    /// The document data16.
    /// </summary>
    DocumentData16 = 0xF0DD,
    /// <summary>
    /// The document FS16.
    /// </summary>
    DocumentFs16 = 0xF0DE,
    /// <summary>
    /// The document JS16.
    /// </summary>
    DocumentJs16 = 0xF0DF,
    /// <summary>
    /// The document number116.
    /// </summary>
    DocumentNumber116 = 0xF0E0,
    /// <summary>
    /// The document py16.
    /// </summary>
    DocumentPy16 = 0xF0E1,
    /// <summary>
    /// The document RB16.
    /// </summary>
    DocumentRb16 = 0xF0E2,
    /// <summary>
    /// The document target16.
    /// </summary>
    DocumentTarget16 = 0xF0E3,
    /// <summary>
    /// The document TS16.
    /// </summary>
    DocumentTs16 = 0xF0E4,
    /// <summary>
    /// The document VB16.
    /// </summary>
    DocumentVb16 = 0xF0E5,
    /// <summary>
    /// The eyedropper16.
    /// </summary>
    Eyedropper16 = 0xF0E6,
    /// <summary>
    /// The folder multiple16.
    /// </summary>
    FolderMultiple16 = 0xF0E7,
    /// <summary>
    /// The folder open vertical16.
    /// </summary>
    FolderOpenVertical16 = 0xF0E8,
    /// <summary>
    /// The gantt chart16.
    /// </summary>
    GanttChart16 = 0xF0E9,
    /// <summary>
    /// The hard drive16.
    /// </summary>
    HardDrive16 = 0xF0EA,
    /// <summary>
    /// The hourglass16.
    /// </summary>
    Hourglass16 = 0xF0EB,
    /// <summary>
    /// The hourglass half16.
    /// </summary>
    HourglassHalf16 = 0xF0EC,
    /// <summary>
    /// The hourglass one quarter16.
    /// </summary>
    HourglassOneQuarter16 = 0xF0ED,
    /// <summary>
    /// The hourglass three quarter16.
    /// </summary>
    HourglassThreeQuarter16 = 0xF0EE,
    /// <summary>
    /// The keyboard mouse16.
    /// </summary>
    KeyboardMouse16 = 0xF0EF,
    /// <summary>
    /// The memory16.
    /// </summary>
    Memory16 = 0xF0F0,
    /// <summary>
    /// The more circle16.
    /// </summary>
    MoreCircle16 = 0xF0F1,
    /// <summary>
    /// The more circle24.
    /// </summary>
    MoreCircle24 = 0xF0F2,
    /// <summary>
    /// The more circle28.
    /// </summary>
    MoreCircle28 = 0xF0F3,
    /// <summary>
    /// The more circle48.
    /// </summary>
    MoreCircle48 = 0xF0F4,
    /// <summary>
    /// The network adapter16.
    /// </summary>
    NetworkAdapter16 = 0xF0F5,
    /// <summary>
    /// The people star16.
    /// </summary>
    PeopleStar16 = 0xF0F6,
    /// <summary>
    /// The people star20.
    /// </summary>
    PeopleStar20 = 0xF0F7,
    /// <summary>
    /// The people star24.
    /// </summary>
    PeopleStar24 = 0xF0F8,
    /// <summary>
    /// The people star28.
    /// </summary>
    PeopleStar28 = 0xF0F9,
    /// <summary>
    /// The people star32.
    /// </summary>
    PeopleStar32 = 0xF0FA,
    /// <summary>
    /// The people star48.
    /// </summary>
    PeopleStar48 = 0xF0FB,
    /// <summary>
    /// The person search16.
    /// </summary>
    PersonSearch16 = 0xF0FC,
    /// <summary>
    /// The person search32.
    /// </summary>
    PersonSearch32 = 0xF0FD,
    /// <summary>
    /// The person standing16.
    /// </summary>
    PersonStanding16 = 0xF0FE,
    /// <summary>
    /// The person walking16.
    /// </summary>
    PersonWalking16 = 0xF0FF,
    /// <summary>
    /// The play multiple16.
    /// </summary>
    PlayMultiple16 = 0xF100,
    /// <summary>
    /// The access time24.
    /// </summary>
    AccessTime24 = 0xF101,
    /// <summary>
    /// The accessibility16.
    /// </summary>
    Accessibility16 = 0xF102,
    /// <summary>
    /// The accessibility20.
    /// </summary>
    Accessibility20 = 0xF103,
    /// <summary>
    /// The accessibility24.
    /// </summary>
    Accessibility24 = 0xF104,
    /// <summary>
    /// The accessibility28.
    /// </summary>
    Accessibility28 = 0xF105,
    /// <summary>
    /// The animal cat16.
    /// </summary>
    AnimalCat16 = 0xF106,
    /// <summary>
    /// The add12.
    /// </summary>
    Add12 = 0xF107,
    /// <summary>
    /// The add16.
    /// </summary>
    Add16 = 0xF108,
    /// <summary>
    /// The add20.
    /// </summary>
    Add20 = 0xF109,
    /// <summary>
    /// The add24.
    /// </summary>
    Add24 = 0xF10A,
    /// <summary>
    /// The add28.
    /// </summary>
    Add28 = 0xF10B,
    /// <summary>
    /// The add circle20.
    /// </summary>
    AddCircle20 = 0xF10C,
    /// <summary>
    /// The add circle24.
    /// </summary>
    AddCircle24 = 0xF10D,
    /// <summary>
    /// The add circle28.
    /// </summary>
    AddCircle28 = 0xF10E,
    /// <summary>
    /// The airplane20.
    /// </summary>
    Airplane20 = 0xF10F,
    /// <summary>
    /// The airplane24.
    /// </summary>
    Airplane24 = 0xF110,
    /// <summary>
    /// The airplane take off16.
    /// </summary>
    AirplaneTakeOff16 = 0xF111,
    /// <summary>
    /// The airplane take off20.
    /// </summary>
    AirplaneTakeOff20 = 0xF112,
    /// <summary>
    /// The airplane take off24.
    /// </summary>
    AirplaneTakeOff24 = 0xF113,
    /// <summary>
    /// The alert20.
    /// </summary>
    Alert20 = 0xF114,
    /// <summary>
    /// The alert24.
    /// </summary>
    Alert24 = 0xF115,
    /// <summary>
    /// The alert28.
    /// </summary>
    Alert28 = 0xF116,
    /// <summary>
    /// The alert off16.
    /// </summary>
    AlertOff16 = 0xF117,
    /// <summary>
    /// The alert off20.
    /// </summary>
    AlertOff20 = 0xF118,
    /// <summary>
    /// The alert off24.
    /// </summary>
    AlertOff24 = 0xF119,
    /// <summary>
    /// The alert off28.
    /// </summary>
    AlertOff28 = 0xF11A,
    /// <summary>
    /// The alert on24.
    /// </summary>
    AlertOn24 = 0xF11B,
    /// <summary>
    /// The alert snooze20.
    /// </summary>
    AlertSnooze20 = 0xF11C,
    /// <summary>
    /// The alert snooze24.
    /// </summary>
    AlertSnooze24 = 0xF11D,
    /// <summary>
    /// The alert urgent20.
    /// </summary>
    AlertUrgent20 = 0xF11E,
    /// <summary>
    /// The alert urgent24.
    /// </summary>
    AlertUrgent24 = 0xF11F,
    /// <summary>
    /// The animal dog20.
    /// </summary>
    AnimalDog20 = 0xF120,
    /// <summary>
    /// The animal dog24.
    /// </summary>
    AnimalDog24 = 0xF121,
    /// <summary>
    /// The application folder20.
    /// </summary>
    AppFolder20 = 0xF122,
    /// <summary>
    /// The application folder24.
    /// </summary>
    AppFolder24 = 0xF123,
    /// <summary>
    /// The application generic24.
    /// </summary>
    AppGeneric24 = 0xF124,
    /// <summary>
    /// The application recent24.
    /// </summary>
    AppRecent24 = 0xF125,
    /// <summary>
    /// The animal cat20.
    /// </summary>
    AnimalCat20 = 0xF126,
    /// <summary>
    /// The animal cat24.
    /// </summary>
    AnimalCat24 = 0xF127,
    /// <summary>
    /// The animal cat28.
    /// </summary>
    AnimalCat28 = 0xF128,
    /// <summary>
    /// The archive settings16.
    /// </summary>
    ArchiveSettings16 = 0xF129,
    /// <summary>
    /// The application store24.
    /// </summary>
    AppStore24 = 0xF12A,
    /// <summary>
    /// The application title24.
    /// </summary>
    AppTitle24 = 0xF12B,
    /// <summary>
    /// The arrow circle down20.
    /// </summary>
    ArrowCircleDown20 = 0xF12C,
    /// <summary>
    /// The arrow circle down24.
    /// </summary>
    ArrowCircleDown24 = 0xF12D,
    /// <summary>
    /// The arrow circle down double20.
    /// </summary>
    ArrowCircleDownDouble20 = 0xF12E,
    /// <summary>
    /// The arrow circle down double24.
    /// </summary>
    ArrowCircleDownDouble24 = 0xF12F,
    /// <summary>
    /// The approvals app24.
    /// </summary>
    ApprovalsApp24 = 0xF130,
    /// <summary>
    /// The approvals app28.
    /// </summary>
    ApprovalsApp28 = 0xF131,
    /// <summary>
    /// The apps16.
    /// </summary>
    Apps16 = 0xF132,
    /// <summary>
    /// The apps20.
    /// </summary>
    Apps20 = 0xF133,
    /// <summary>
    /// The apps24.
    /// </summary>
    Apps24 = 0xF134,
    /// <summary>
    /// The apps28.
    /// </summary>
    Apps28 = 0xF135,
    /// <summary>
    /// The apps add in20.
    /// </summary>
    AppsAddIn20 = 0xF136,
    /// <summary>
    /// The apps add in24.
    /// </summary>
    AppsAddIn24 = 0xF137,
    /// <summary>
    /// The apps list24.
    /// </summary>
    AppsList24 = 0xF138,
    /// <summary>
    /// The archive20.
    /// </summary>
    Archive20 = 0xF139,
    /// <summary>
    /// The archive24.
    /// </summary>
    Archive24 = 0xF13A,
    /// <summary>
    /// The archive28.
    /// </summary>
    Archive28 = 0xF13B,
    /// <summary>
    /// The archive48.
    /// </summary>
    Archive48 = 0xF13C,
    /// <summary>
    /// The arrow clockwise20.
    /// </summary>
    ArrowClockwise20 = 0xF13D,
    /// <summary>
    /// The arrow clockwise24.
    /// </summary>
    ArrowClockwise24 = 0xF13E,
    /// <summary>
    /// The arrow counterclockwise20.
    /// </summary>
    ArrowCounterclockwise20 = 0xF13F,
    /// <summary>
    /// The arrow counterclockwise24.
    /// </summary>
    ArrowCounterclockwise24 = 0xF140,
    /// <summary>
    /// The arrow curve down left20.
    /// </summary>
    ArrowCurveDownLeft20 = 0xF141,
    /// <summary>
    /// The arrow curve down right20.
    /// </summary>
    ArrowCurveDownRight20 = 0xF142,
    /// <summary>
    /// The arrow circle down split20.
    /// </summary>
    ArrowCircleDownSplit20 = 0xF143,
    /// <summary>
    /// The arrow circle down split24.
    /// </summary>
    ArrowCircleDownSplit24 = 0xF144,
    /// <summary>
    /// The arrow curve up left20.
    /// </summary>
    ArrowCurveUpLeft20 = 0xF145,
    /// <summary>
    /// The arrow curve up right20.
    /// </summary>
    ArrowCurveUpRight20 = 0xF146,
    /// <summary>
    /// The arrow down16.
    /// </summary>
    ArrowDown16 = 0xF147,
    /// <summary>
    /// The arrow down20.
    /// </summary>
    ArrowDown20 = 0xF148,
    /// <summary>
    /// The arrow down24.
    /// </summary>
    ArrowDown24 = 0xF149,
    /// <summary>
    /// The arrow down28.
    /// </summary>
    ArrowDown28 = 0xF14A,
    /// <summary>
    /// The arrow down left24.
    /// </summary>
    ArrowDownLeft24 = 0xF14B,
    /// <summary>
    /// The arrow down32.
    /// </summary>
    ArrowDown32 = 0xF14C,
    /// <summary>
    /// The arrow down48.
    /// </summary>
    ArrowDown48 = 0xF14D,
    /// <summary>
    /// The arrow fit16.
    /// </summary>
    ArrowFit16 = 0xF14E,
    /// <summary>
    /// The arrow download16.
    /// </summary>
    ArrowDownload16 = 0xF14F,
    /// <summary>
    /// The arrow download20.
    /// </summary>
    ArrowDownload20 = 0xF150,
    /// <summary>
    /// The arrow download24.
    /// </summary>
    ArrowDownload24 = 0xF151,
    /// <summary>
    /// The arrow download48.
    /// </summary>
    ArrowDownload48 = 0xF152,
    /// <summary>
    /// The radio button16.
    /// </summary>
    RadioButton16 = 0xF153,
    /// <summary>
    /// The arrow expand24.
    /// </summary>
    ArrowExpand24 = 0xF154,
    /// <summary>
    /// The RadioButton off16.
    /// </summary>
    RadioButtonOff16 = 0xF155,
    /// <summary>
    /// The arrow forward16.
    /// </summary>
    ArrowForward16 = 0xF156,
    /// <summary>
    /// The arrow forward20.
    /// </summary>
    ArrowForward20 = 0xF157,
    /// <summary>
    /// The arrow forward24.
    /// </summary>
    ArrowForward24 = 0xF158,
    /// <summary>
    /// The arrow import20.
    /// </summary>
    ArrowImport20 = 0xF159,
    /// <summary>
    /// The arrow import24.
    /// </summary>
    ArrowImport24 = 0xF15A,
    /// <summary>
    /// The arrow left20.
    /// </summary>
    ArrowLeft20 = 0xF15B,
    /// <summary>
    /// The arrow left24.
    /// </summary>
    ArrowLeft24 = 0xF15C,
    /// <summary>
    /// The arrow left28.
    /// </summary>
    ArrowLeft28 = 0xF15D,
    /// <summary>
    /// The arrow maximize16.
    /// </summary>
    ArrowMaximize16 = 0xF15E,
    /// <summary>
    /// The arrow maximize20.
    /// </summary>
    ArrowMaximize20 = 0xF15F,
    /// <summary>
    /// The arrow maximize24.
    /// </summary>
    ArrowMaximize24 = 0xF160,
    /// <summary>
    /// The arrow maximize28.
    /// </summary>
    ArrowMaximize28 = 0xF161,
    /// <summary>
    /// The arrow maximize vertical20.
    /// </summary>
    ArrowMaximizeVertical20 = 0xF162,
    /// <summary>
    /// The arrow maximize vertical24.
    /// </summary>
    ArrowMaximizeVertical24 = 0xF163,
    /// <summary>
    /// The arrow minimize16.
    /// </summary>
    ArrowMinimize16 = 0xF164,
    /// <summary>
    /// The arrow minimize20.
    /// </summary>
    ArrowMinimize20 = 0xF165,
    /// <summary>
    /// The arrow minimize24.
    /// </summary>
    ArrowMinimize24 = 0xF166,
    /// <summary>
    /// The arrow minimize28.
    /// </summary>
    ArrowMinimize28 = 0xF167,
    /// <summary>
    /// The arrow minimize vertical24.
    /// </summary>
    ArrowMinimizeVertical24 = 0xF168,
    /// <summary>
    /// The arrow move24.
    /// </summary>
    ArrowMove24 = 0xF169,
    /// <summary>
    /// The arrow next20.
    /// </summary>
    ArrowNext20 = 0xF16A,
    /// <summary>
    /// The arrow next24.
    /// </summary>
    ArrowNext24 = 0xF16B,
    /// <summary>
    /// The arrow previous20.
    /// </summary>
    ArrowPrevious20 = 0xF16C,
    /// <summary>
    /// The arrow previous24.
    /// </summary>
    ArrowPrevious24 = 0xF16D,
    /// <summary>
    /// The arrow redo20.
    /// </summary>
    ArrowRedo20 = 0xF16E,
    /// <summary>
    /// The arrow redo24.
    /// </summary>
    ArrowRedo24 = 0xF16F,
    /// <summary>
    /// The arrow repeat all16.
    /// </summary>
    ArrowRepeatAll16 = 0xF170,
    /// <summary>
    /// The arrow repeat all20.
    /// </summary>
    ArrowRepeatAll20 = 0xF171,
    /// <summary>
    /// The arrow repeat all24.
    /// </summary>
    ArrowRepeatAll24 = 0xF172,
    /// <summary>
    /// The arrow repeat all off16.
    /// </summary>
    ArrowRepeatAllOff16 = 0xF173,
    /// <summary>
    /// The arrow repeat all off20.
    /// </summary>
    ArrowRepeatAllOff20 = 0xF174,
    /// <summary>
    /// The arrow repeat all off24.
    /// </summary>
    ArrowRepeatAllOff24 = 0xF175,
    /// <summary>
    /// The arrow reply16.
    /// </summary>
    ArrowReply16 = 0xF176,
    /// <summary>
    /// The arrow reply20.
    /// </summary>
    ArrowReply20 = 0xF177,
    /// <summary>
    /// The arrow reply24.
    /// </summary>
    ArrowReply24 = 0xF178,
    /// <summary>
    /// The arrow reply48.
    /// </summary>
    ArrowReply48 = 0xF179,
    /// <summary>
    /// The arrow reply all16.
    /// </summary>
    ArrowReplyAll16 = 0xF17A,
    /// <summary>
    /// The arrow reply all20.
    /// </summary>
    ArrowReplyAll20 = 0xF17B,
    /// <summary>
    /// The arrow reply all24.
    /// </summary>
    ArrowReplyAll24 = 0xF17C,
    /// <summary>
    /// The arrow reply all48.
    /// </summary>
    ArrowReplyAll48 = 0xF17D,
    /// <summary>
    /// The arrow reply down16.
    /// </summary>
    ArrowReplyDown16 = 0xF17E,
    /// <summary>
    /// The arrow reply down20.
    /// </summary>
    ArrowReplyDown20 = 0xF17F,
    /// <summary>
    /// The arrow reply down24.
    /// </summary>
    ArrowReplyDown24 = 0xF180,
    /// <summary>
    /// The arrow right20.
    /// </summary>
    ArrowRight20 = 0xF181,
    /// <summary>
    /// The arrow right24.
    /// </summary>
    ArrowRight24 = 0xF182,
    /// <summary>
    /// The arrow right28.
    /// </summary>
    ArrowRight28 = 0xF183,
    /// <summary>
    /// The arrow left16.
    /// </summary>
    ArrowLeft16 = 0xF184,
    /// <summary>
    /// The arrow rotate clockwise20.
    /// </summary>
    ArrowRotateClockwise20 = 0xF185,
    /// <summary>
    /// The arrow rotate clockwise24.
    /// </summary>
    ArrowRotateClockwise24 = 0xF186,
    /// <summary>
    /// The arrow rotate counterclockwise20.
    /// </summary>
    ArrowRotateCounterclockwise20 = 0xF187,
    /// <summary>
    /// The arrow rotate counterclockwise24.
    /// </summary>
    ArrowRotateCounterclockwise24 = 0xF188,
    /// <summary>
    /// The arrow left32.
    /// </summary>
    ArrowLeft32 = 0xF189,
    /// <summary>
    /// The arrow sort20.
    /// </summary>
    ArrowSort20 = 0xF18A,
    /// <summary>
    /// The arrow sort24.
    /// </summary>
    ArrowSort24 = 0xF18B,
    /// <summary>
    /// The arrow sort28.
    /// </summary>
    ArrowSort28 = 0xF18C,
    /// <summary>
    /// The arrow swap20.
    /// </summary>
    ArrowSwap20 = 0xF18D,
    /// <summary>
    /// The arrow swap24.
    /// </summary>
    ArrowSwap24 = 0xF18E,
    /// <summary>
    /// The arrow sync12.
    /// </summary>
    ArrowSync12 = 0xF18F,
    /// <summary>
    /// The arrow sync20.
    /// </summary>
    ArrowSync20 = 0xF190,
    /// <summary>
    /// The arrow sync24.
    /// </summary>
    ArrowSync24 = 0xF191,
    /// <summary>
    /// The arrow synchronize circle16.
    /// </summary>
    ArrowSyncCircle16 = 0xF192,
    /// <summary>
    /// The arrow synchronize circle20.
    /// </summary>
    ArrowSyncCircle20 = 0xF193,
    /// <summary>
    /// The arrow synchronize circle24.
    /// </summary>
    ArrowSyncCircle24 = 0xF194,
    /// <summary>
    /// The arrow synchronize off12.
    /// </summary>
    ArrowSyncOff12 = 0xF195,
    /// <summary>
    /// The arrow trending16.
    /// </summary>
    ArrowTrending16 = 0xF196,
    /// <summary>
    /// The arrow trending20.
    /// </summary>
    ArrowTrending20 = 0xF197,
    /// <summary>
    /// The arrow trending24.
    /// </summary>
    ArrowTrending24 = 0xF198,
    ArrowUndo20 = 0xF199,
    ArrowUndo24 = 0xF19A,
    ArrowUp20 = 0xF19B,
    ArrowUp24 = 0xF19C,
    ArrowUp28 = 0xF19D,
    ArrowLeft48 = 0xF19E,
    ArrowReset20 = 0xF19F,
    ArrowReset24 = 0xF1A0,
    ArrowUpLeft24 = 0xF1A1,
    ArrowRight32 = 0xF1A2,
    ArrowUpRight24 = 0xF1A3,
    ArrowUpload20 = 0xF1A4,
    ArrowUpload24 = 0xF1A5,
    ArrowsBidirectional24 = 0xF1A6,
    ArrowRight48 = 0xF1A7,
    Attach16 = 0xF1A8,
    Attach20 = 0xF1A9,
    Attach24 = 0xF1AA,
    ArrowSort16 = 0xF1AB,
    ArrowSortDown16 = 0xF1AC,
    ArrowSortDownLines16 = 0xF1AD,
    Autocorrect24 = 0xF1AE,
    Autosum20 = 0xF1AF,
    Autosum24 = 0xF1B0,
    Backspace20 = 0xF1B1,
    Backspace24 = 0xF1B2,
    ArrowSortUp16 = 0xF1B3,
    ArrowUp16 = 0xF1B4,
    Badge24 = 0xF1B5,
    Balloon20 = 0xF1B6,
    Balloon24 = 0xF1B7,
    ArrowUp32 = 0xF1B8,
    ArrowUp48 = 0xF1B9,
    BarcodeScanner20 = 0xF1BA,
    Battery020 = 0xF1BB,
    Battery024 = 0xF1BC,
    Battery120 = 0xF1BD,
    Battery124 = 0xF1BE,
    Battery220 = 0xF1BF,
    Battery224 = 0xF1C0,
    Battery320 = 0xF1C1,
    Battery324 = 0xF1C2,
    Battery420 = 0xF1C3,
    Battery424 = 0xF1C4,
    Battery520 = 0xF1C5,
    Battery524 = 0xF1C6,
    Battery620 = 0xF1C7,
    Battery624 = 0xF1C8,
    Battery720 = 0xF1C9,
    Battery724 = 0xF1CA,
    Battery820 = 0xF1CB,
    Battery824 = 0xF1CC,
    Battery920 = 0xF1CD,
    Battery924 = 0xF1CE,
    BatteryCharge20 = 0xF1CF,
    BatteryCharge24 = 0xF1D0,
    Ram16 = 0xF1D1,
    SaveMultiple16 = 0xF1D2,
    BatterySaver20 = 0xF1D3,
    BatterySaver24 = 0xF1D4,
    BatteryWarning24 = 0xF1D5,
    Beaker16 = 0xF1D6,
    Beaker20 = 0xF1D7,
    Beaker24 = 0xF1D8,
    Bed20 = 0xF1D9,
    Bed24 = 0xF1DA,
    Script16 = 0xF1DB,
    Server16 = 0xF1DC,
    ServerSurface16 = 0xF1DD,
    Bluetooth20 = 0xF1DE,
    Bluetooth24 = 0xF1DF,
    BluetoothConnected24 = 0xF1E0,
    BluetoothDisabled24 = 0xF1E1,
    BluetoothSearching24 = 0xF1E2,
    Board24 = 0xF1E3,
    BarcodeScanner24 = 0xF1E4,
    BeakerEdit20 = 0xF1E5,
    BeakerEdit24 = 0xF1E6,
    BookToolbox20 = 0xF1E7,
    BookmarkAdd20 = 0xF1E8,
    BookmarkAdd24 = 0xF1E9,
    BowlChopsticks16 = 0xF1EA,
    BowlChopsticks20 = 0xF1EB,
    BowlChopsticks24 = 0xF1EC,
    BowlChopsticks28 = 0xF1ED,
    BrainCircuit20 = 0xF1EE,
    BriefcaseMedical20 = 0xF1EF,
    BookGlobe24 = 0xF1F0,
    BookNumber16 = 0xF1F1,
    BookNumber20 = 0xF1F2,
    BookNumber24 = 0xF1F3,
    Bookmark16 = 0xF1F4,
    Bookmark20 = 0xF1F5,
    Bookmark24 = 0xF1F6,
    Bookmark28 = 0xF1F7,
    BookmarkOff24 = 0xF1F8,
    Bot24 = 0xF1F9,
    BotAdd24 = 0xF1FA,
    Branch24 = 0xF1FB,
    Briefcase20 = 0xF1FC,
    Briefcase24 = 0xF1FD,
    Broom16 = 0xF1FE,
    BuildingBankToolbox20 = 0xF1FF,
    BroadActivityFeed24 = 0xF200,
    Broom20 = 0xF201,
    Broom24 = 0xF202,
    CalendarInfo16 = 0xF203,
    CalendarMultiple16 = 0xF204,
    Building24 = 0xF205,
    ServerSurfaceMultiple16 = 0xF206,
    CallCheckmark20 = 0xF207,
    CallDismiss20 = 0xF208,
    BuildingRetail24 = 0xF209,
    Calculator20 = 0xF20A,
    CallDismiss24 = 0xF20B,
    CallPause20 = 0xF20C,
    CallPause24 = 0xF20D,
    Calendar3Day20 = 0xF20E,
    Calendar3Day24 = 0xF20F,
    Calendar3Day28 = 0xF210,
    CalendarAdd20 = 0xF211,
    CalendarAdd24 = 0xF212,
    CalendarAgenda20 = 0xF213,
    CalendarAgenda24 = 0xF214,
    CalendarAgenda28 = 0xF215,
    CalendarArrowRight20 = 0xF216,
    CalendarAssistant20 = 0xF217,
    CalendarAssistant24 = 0xF218,
    CalendarCancel20 = 0xF219,
    CalendarCancel24 = 0xF21A,
    CalendarCheckmark16 = 0xF21B,
    CalendarCheckmark20 = 0xF21C,
    CalendarClock20 = 0xF21D,
    CalendarClock24 = 0xF21E,
    Shield12 = 0xF21F,
    ChatHelp20 = 0xF220,
    ChatSettings20 = 0xF221,
    CalendarDay20 = 0xF222,
    CalendarDay24 = 0xF223,
    CalendarDay28 = 0xF224,
    CalendarEmpty16 = 0xF225,
    CalendarEmpty20 = 0xF226,
    CalendarEmpty24 = 0xF227,
    CalendarEmpty28 = 0xF228,
    ChatSettings24 = 0xF229,
    CalendarMonth20 = 0xF22A,
    CalendarMonth24 = 0xF22B,
    CalendarMonth28 = 0xF22C,
    CalendarMultiple20 = 0xF22D,
    CalendarMultiple24 = 0xF22E,
    SlideTextPerson16 = 0xF22F,
    CalendarPerson20 = 0xF230,
    CalendarReply16 = 0xF231,
    CalendarReply20 = 0xF232,
    CalendarReply24 = 0xF233,
    CalendarReply28 = 0xF234,
    CalendarSettings20 = 0xF235,
    CalendarStar20 = 0xF236,
    CalendarStar24 = 0xF237,
    CalendarSync16 = 0xF238,
    CalendarSync20 = 0xF239,
    CalendarSync24 = 0xF23A,
    CalendarToday16 = 0xF23B,
    CalendarToday20 = 0xF23C,
    CalendarToday24 = 0xF23D,
    CalendarToday28 = 0xF23E,
    CalendarWeekNumbers24 = 0xF23F,
    CalendarWeekStart20 = 0xF240,
    CalendarWeekStart24 = 0xF241,
    CalendarWeekStart28 = 0xF242,
    CalendarWorkWeek16 = 0xF243,
    CalendarWorkWeek20 = 0xF244,
    CalendarWorkWeek24 = 0xF245,
    CallAdd24 = 0xF246,
    CallEnd20 = 0xF247,
    CallEnd24 = 0xF248,
    CallEnd28 = 0xF249,
    CallForward24 = 0xF24A,
    CallInbound16 = 0xF24B,
    CallInbound24 = 0xF24C,
    CallMissed16 = 0xF24D,
    CallMissed24 = 0xF24E,
    CallOutbound16 = 0xF24F,
    CallOutbound24 = 0xF250,
    CallPark24 = 0xF251,
    CalligraphyPen20 = 0xF252,
    CalligraphyPen24 = 0xF253,
    Camera20 = 0xF254,
    Camera24 = 0xF255,
    Camera28 = 0xF256,
    CameraAdd20 = 0xF257,
    CameraAdd24 = 0xF258,
    CameraAdd48 = 0xF259,
    CameraSwitch24 = 0xF25A,
    SlideTextPerson20 = 0xF25B,
    SlideTextPerson24 = 0xF25C,
    SlideTextPerson28 = 0xF25D,
    SlideTextPerson32 = 0xF25E,
    CaretDown12 = 0xF25F,
    CaretDown16 = 0xF260,
    CaretDown20 = 0xF261,
    CaretDown24 = 0xF262,
    CaretLeft12 = 0xF263,
    CaretLeft16 = 0xF264,
    CaretLeft20 = 0xF265,
    CaretLeft24 = 0xF266,
    CaretRight12 = 0xF267,
    CaretRight16 = 0xF268,
    CaretRight20 = 0xF269,
    CaretRight24 = 0xF26A,
    Cart24 = 0xF26B,
    Cast20 = 0xF26C,
    Cast24 = 0xF26D,
    Cast28 = 0xF26E,
    Cellular3g24 = 0xF26F,
    Cellular4g24 = 0xF270,
    CellularData120 = 0xF271,
    CellularData124 = 0xF272,
    CellularData220 = 0xF273,
    CellularData224 = 0xF274,
    CellularData320 = 0xF275,
    CellularData324 = 0xF276,
    CellularData420 = 0xF277,
    CellularData424 = 0xF278,
    CellularData520 = 0xF279,
    CellularData524 = 0xF27A,
    Check20 = 0xF27B,
    CheckboxChecked16 = 0xF27C,
    CheckboxCheckedSync16 = 0xF27D,
    Certificate20 = 0xF27E,
    Certificate24 = 0xF27F,
    Channel16 = 0xF280,
    Channel20 = 0xF281,
    Channel24 = 0xF282,
    CheckmarkStarburst16 = 0xF283,
    ChevronDoubleDown16 = 0xF284,
    ChevronDoubleLeft16 = 0xF285,
    Chat20 = 0xF286,
    Chat24 = 0xF287,
    Chat28 = 0xF288,
    ChatBubblesQuestion24 = 0xF289,
    ChatHelp24 = 0xF28A,
    ChatOff24 = 0xF28B,
    ChatWarning24 = 0xF28C,
    CheckboxChecked20 = 0xF28D,
    CheckboxChecked24 = 0xF28E,
    CheckboxUnchecked12 = 0xF28F,
    CheckboxUnchecked16 = 0xF290,
    CheckboxUnchecked20 = 0xF291,
    CheckboxUnchecked24 = 0xF292,
    Checkmark12 = 0xF293,
    Checkmark20 = 0xF294,
    Checkmark24 = 0xF295,
    Checkmark28 = 0xF296,
    CheckmarkCircle16 = 0xF297,
    CheckmarkCircle20 = 0xF298,
    CheckmarkCircle24 = 0xF299,
    CheckmarkCircle48 = 0xF29A,
    CheckmarkLock16 = 0xF29B,
    CheckmarkLock20 = 0xF29C,
    CheckmarkLock24 = 0xF29D,
    CheckmarkSquare24 = 0xF29E,
    CheckmarkUnderlineCircle16 = 0xF29F,
    CheckmarkUnderlineCircle20 = 0xF2A0,
    ChevronDown12 = 0xF2A1,
    ChevronDown16 = 0xF2A2,
    ChevronDown20 = 0xF2A3,
    ChevronDown24 = 0xF2A4,
    ChevronDown28 = 0xF2A5,
    ChevronDown48 = 0xF2A6,
    ChevronDoubleRight16 = 0xF2A7,
    ChevronLeft12 = 0xF2A8,
    ChevronLeft16 = 0xF2A9,
    ChevronLeft20 = 0xF2AA,
    ChevronLeft24 = 0xF2AB,
    ChevronLeft28 = 0xF2AC,
    ChevronLeft48 = 0xF2AD,
    ChevronRight12 = 0xF2AE,
    ChevronRight16 = 0xF2AF,
    ChevronRight20 = 0xF2B0,
    ChevronRight24 = 0xF2B1,
    ChevronRight28 = 0xF2B2,
    ChevronRight48 = 0xF2B3,
    ChevronUp12 = 0xF2B4,
    ChevronUp16 = 0xF2B5,
    ChevronUp20 = 0xF2B6,
    ChevronUp24 = 0xF2B7,
    ChevronUp28 = 0xF2B8,
    ChevronUp48 = 0xF2B9,
    Circle16 = 0xF2BA,
    Circle20 = 0xF2BB,
    Circle24 = 0xF2BC,
    CircleHalfFill20 = 0xF2BD,
    CircleHalfFill24 = 0xF2BE,
    CircleLine24 = 0xF2BF,
    CircleSmall24 = 0xF2C0,
    City16 = 0xF2C1,
    City20 = 0xF2C2,
    City24 = 0xF2C3,
    Class24 = 0xF2C4,
    Classification16 = 0xF2C5,
    Classification20 = 0xF2C6,
    Classification24 = 0xF2C7,
    ClearFormatting24 = 0xF2C8,
    Clipboard20 = 0xF2C9,
    Clipboard24 = 0xF2CA,
    ClipboardCode16 = 0xF2CB,
    ClipboardCode20 = 0xF2CC,
    ClipboardCode24 = 0xF2CD,
    ClipboardLetter16 = 0xF2CE,
    ClipboardLetter20 = 0xF2CF,
    ClipboardLetter24 = 0xF2D0,
    ClipboardLink16 = 0xF2D1,
    ClipboardLink20 = 0xF2D2,
    ClipboardLink24 = 0xF2D3,
    ClipboardMore24 = 0xF2D4,
    ClipboardPaste20 = 0xF2D5,
    ClipboardPaste24 = 0xF2D6,
    ClipboardSearch20 = 0xF2D7,
    ClipboardSearch24 = 0xF2D8,
    SlideTextPerson48 = 0xF2D9,
    SprayCan16 = 0xF2DA,
    Clock12 = 0xF2DB,
    Clock16 = 0xF2DC,
    Clock20 = 0xF2DD,
    Clock24 = 0xF2DE,
    Clock28 = 0xF2DF,
    Clock48 = 0xF2E0,
    ClockAlarm20 = 0xF2E1,
    ClockAlarm24 = 0xF2E2,
    ClosedCaption24 = 0xF2E3,
    Cloud20 = 0xF2E4,
    Cloud24 = 0xF2E5,
    Cloud48 = 0xF2E6,
    Step16 = 0xF2E7,
    Steps16 = 0xF2E8,
    TableLock16 = 0xF2E9,
    CloudOff24 = 0xF2EA,
    CloudOff48 = 0xF2EB,
    TableLock20 = 0xF2EC,
    TableLock24 = 0xF2ED,
    TableLock28 = 0xF2EE,
    Code20 = 0xF2EF,
    Code24 = 0xF2F0,
    Collections20 = 0xF2F1,
    Collections24 = 0xF2F2,
    CollectionsAdd20 = 0xF2F3,
    CollectionsAdd24 = 0xF2F4,
    Color20 = 0xF2F5,
    Color24 = 0xF2F6,
    ColorBackground20 = 0xF2F7,
    ColorBackground24 = 0xF2F8,
    ColorFill20 = 0xF2F9,
    ColorFill24 = 0xF2FA,
    ColorLine20 = 0xF2FB,
    ColorLine24 = 0xF2FC,
    ColumnTriple24 = 0xF2FD,
    Comment16 = 0xF2FE,
    Comment20 = 0xF2FF,
    Comment24 = 0xF300,
    CommentAdd24 = 0xF301,
    TableLock32 = 0xF302,
    CommentMention16 = 0xF303,
    CommentMention20 = 0xF304,
    CommentMention24 = 0xF305,
    CommentMultiple16 = 0xF306,
    CommentMultiple20 = 0xF307,
    CommentMultiple24 = 0xF308,
    TableLock48 = 0xF309,
    CircleHalfFill16 = 0xF30A,
    ClipboardHeart20 = 0xF30B,
    Communication16 = 0xF30C,
    Communication20 = 0xF30D,
    Communication24 = 0xF30E,
    CompassNorthwest16 = 0xF30F,
    CompassNorthwest20 = 0xF310,
    CompassNorthwest24 = 0xF311,
    CompassNorthwest28 = 0xF312,
    Compose16 = 0xF313,
    Compose20 = 0xF314,
    Compose24 = 0xF315,
    Compose28 = 0xF316,
    ConferenceRoom16 = 0xF317,
    ConferenceRoom20 = 0xF318,
    ConferenceRoom24 = 0xF319,
    ConferenceRoom28 = 0xF31A,
    ConferenceRoom48 = 0xF31B,
    Connector16 = 0xF31C,
    Connector20 = 0xF31D,
    Connector24 = 0xF31E,
    ContactCard20 = 0xF31F,
    ContactCard24 = 0xF320,
    ContactCardGroup24 = 0xF321,
    ClipboardPulse20 = 0xF322,
    ContentSettings16 = 0xF323,
    ContentSettings20 = 0xF324,
    ContentSettings24 = 0xF325,
    TextTTag16 = 0xF326,
    VideoPerson32 = 0xF327,
    Cookies20 = 0xF328,
    Cookies24 = 0xF329,
    Copy16 = 0xF32A,
    Copy20 = 0xF32B,
    Copy24 = 0xF32C,
    ClipboardSettings20 = 0xF32D,
    ClockArrowDownload20 = 0xF32E,
    CloudAdd16 = 0xF32F,
    CloudEdit16 = 0xF330,
    Crop24 = 0xF331,
    CropInterim24 = 0xF332,
    CropInterimOff24 = 0xF333,
    Cube16 = 0xF334,
    Cube20 = 0xF335,
    Cube24 = 0xF336,
    CloudFlow20 = 0xF337,
    CloudLink16 = 0xF338,
    Code16 = 0xF339,
    Cut20 = 0xF33A,
    Cut24 = 0xF33B,
    DarkTheme24 = 0xF33C,
    DataArea24 = 0xF33D,
    DataBarHorizontal24 = 0xF33E,
    DataBarVertical20 = 0xF33F,
    DataBarVertical24 = 0xF340,
    DataFunnel24 = 0xF341,
    DataHistogram24 = 0xF342,
    DataLine24 = 0xF343,
    DataPie20 = 0xF344,
    DataPie24 = 0xF345,
    DataScatter24 = 0xF346,
    DataSunburst24 = 0xF347,
    DataTreemap24 = 0xF348,
    DataUsage24 = 0xF349,
    DataWaterfall24 = 0xF34A,
    DataWhisker24 = 0xF34B,
    Delete20 = 0xF34C,
    Delete24 = 0xF34D,
    Delete28 = 0xF34E,
    Delete48 = 0xF34F,
    CommentError16 = 0xF350,
    CommentLightning20 = 0xF351,
    DeleteOff20 = 0xF352,
    DeleteOff24 = 0xF353,
    Dentist24 = 0xF354,
    DesignIdeas16 = 0xF355,
    DesignIdeas20 = 0xF356,
    DesignIdeas24 = 0xF357,
    Desktop16 = 0xF358,
    Desktop20 = 0xF359,
    Desktop24 = 0xF35A,
    Desktop28 = 0xF35B,
    DeveloperBoard24 = 0xF35C,
    DeviceEq24 = 0xF35D,
    Dialpad20 = 0xF35E,
    Dialpad24 = 0xF35F,
    DialpadOff24 = 0xF360,
    CommentLightning24 = 0xF361,
    ContactCard16 = 0xF362,
    ContactCardLink16 = 0xF363,
    ContractDownLeft16 = 0xF364,
    Directions20 = 0xF365,
    Directions24 = 0xF366,
    Dismiss12 = 0xF367,
    Dismiss16 = 0xF368,
    Dismiss20 = 0xF369,
    Dismiss24 = 0xF36A,
    Dismiss28 = 0xF36B,
    DismissCircle16 = 0xF36C,
    DismissCircle20 = 0xF36D,
    DismissCircle24 = 0xF36E,
    DismissCircle48 = 0xF36F,
    DividerShort24 = 0xF370,
    DividerTall24 = 0xF371,
    Dock24 = 0xF372,
    ContractDownLeft20 = 0xF373,
    ContractDownLeft24 = 0xF374,
    ContractDownLeft28 = 0xF375,
    DockRow24 = 0xF376,
    Doctor24 = 0xF377,
    Document20 = 0xF378,
    Document24 = 0xF379,
    Document28 = 0xF37A,
    ContractDownLeft32 = 0xF37B,
    DocumentBriefcase20 = 0xF37C,
    DocumentBriefcase24 = 0xF37D,
    DocumentCatchUp24 = 0xF37E,
    DocumentCopy16 = 0xF37F,
    DocumentCopy20 = 0xF380,
    DocumentCopy24 = 0xF381,
    DocumentCopy48 = 0xF382,
    DocumentDismiss20 = 0xF383,
    DocumentDismiss24 = 0xF384,
    DocumentEdit16 = 0xF385,
    DocumentEdit20 = 0xF386,
    DocumentEdit24 = 0xF387,
    DocumentEndnote20 = 0xF388,
    DocumentEndnote24 = 0xF389,
    DocumentError16 = 0xF38A,
    DocumentError20 = 0xF38B,
    DocumentError24 = 0xF38C,
    DocumentFooter24 = 0xF38D,
    VideoPersonClock16 = 0xF38E,
    DocumentHeader24 = 0xF38F,
    DocumentHeaderFooter20 = 0xF390,
    DocumentHeaderFooter24 = 0xF391,
    VideoPersonClock20 = 0xF392,
    DocumentLandscape20 = 0xF393,
    DocumentLandscape24 = 0xF394,
    DocumentMargins20 = 0xF395,
    DocumentMargins24 = 0xF396,
    ContractDownLeft48 = 0xF397,
    CreditCardToolbox20 = 0xF398,
    DocumentOnePage20 = 0xF399,
    DocumentOnePage24 = 0xF39A,
    DataBarHorizontal20 = 0xF39B,
    DocumentPageBottomCenter20 = 0xF39C,
    DocumentPageBottomCenter24 = 0xF39D,
    DocumentPageBottomLeft20 = 0xF39E,
    DocumentPageBottomLeft24 = 0xF39F,
    DocumentPageBottomRight20 = 0xF3A0,
    DocumentPageBottomRight24 = 0xF3A1,
    DocumentPageBreak24 = 0xF3A2,
    DocumentPageNumber20 = 0xF3A3,
    DocumentPageNumber24 = 0xF3A4,
    DocumentPageTopCenter20 = 0xF3A5,
    DocumentPageTopCenter24 = 0xF3A6,
    DocumentPageTopLeft20 = 0xF3A7,
    DocumentPageTopLeft24 = 0xF3A8,
    DocumentPageTopRight20 = 0xF3A9,
    DocumentPageTopRight24 = 0xF3AA,
    DocumentPdf16 = 0xF3AB,
    DocumentPdf20 = 0xF3AC,
    DocumentPdf24 = 0xF3AD,
    DocumentSearch20 = 0xF3AE,
    DocumentSearch24 = 0xF3AF,
    DocumentToolbox20 = 0xF3B0,
    DocumentToolbox24 = 0xF3B1,
    DataUsageEdit20 = 0xF3B2,
    DesktopSync16 = 0xF3B3,
    DeviceMeetingRoom16 = 0xF3B4,
    DeviceMeetingRoom24 = 0xF3B5,
    DeviceMeetingRoom28 = 0xF3B6,
    DeviceMeetingRoom32 = 0xF3B7,
    DocumentWidth20 = 0xF3B8,
    DocumentWidth24 = 0xF3B9,
    DoubleSwipeDown24 = 0xF3BA,
    DoubleSwipeUp24 = 0xF3BB,
    DeviceMeetingRoom48 = 0xF3BC,
    DeviceMeetingRoomRemote16 = 0xF3BD,
    Drafts16 = 0xF3BE,
    Drafts20 = 0xF3BF,
    Drafts24 = 0xF3C0,
    Drag24 = 0xF3C1,
    DeviceMeetingRoomRemote24 = 0xF3C2,
    DrinkBeer24 = 0xF3C3,
    DrinkCoffee20 = 0xF3C4,
    DrinkCoffee24 = 0xF3C5,
    DrinkMargarita24 = 0xF3C6,
    DrinkWine24 = 0xF3C7,
    DualScreen24 = 0xF3C8,
    DualScreenAdd24 = 0xF3C9,
    DualScreenArrowRight24 = 0xF3CA,
    DualScreenClock24 = 0xF3CB,
    DualScreenDesktop24 = 0xF3CC,
    DeviceMeetingRoomRemote28 = 0xF3CD,
    DualScreenGroup24 = 0xF3CE,
    DualScreenHeader24 = 0xF3CF,
    DualScreenLock24 = 0xF3D0,
    DualScreenMirror24 = 0xF3D1,
    DualScreenPagination24 = 0xF3D2,
    DualScreenSettings24 = 0xF3D3,
    DualScreenStatusBar24 = 0xF3D4,
    DualScreenTablet24 = 0xF3D5,
    DualScreenUpdate24 = 0xF3D6,
    DualScreenVerticalScroll24 = 0xF3D7,
    DualScreenVibrate24 = 0xF3D8,
    Earth16 = 0xF3D9,
    Earth20 = 0xF3DA,
    Earth24 = 0xF3DB,
    Edit16 = 0xF3DC,
    Edit20 = 0xF3DD,
    Edit24 = 0xF3DE,
    Emoji16 = 0xF3DF,
    Emoji20 = 0xF3E0,
    Emoji24 = 0xF3E1,
    EmojiAdd24 = 0xF3E2,
    EmojiAngry20 = 0xF3E3,
    EmojiAngry24 = 0xF3E4,
    EmojiLaugh20 = 0xF3E5,
    EmojiLaugh24 = 0xF3E6,
    EmojiMeh20 = 0xF3E7,
    EmojiMeh24 = 0xF3E8,
    EmojiSad20 = 0xF3E9,
    EmojiSad24 = 0xF3EA,
    EmojiSurprise20 = 0xF3EB,
    EmojiSurprise24 = 0xF3EC,
    DeviceMeetingRoomRemote32 = 0xF3ED,
    DeviceMeetingRoomRemote48 = 0xF3EE,
    EraserTool24 = 0xF3EF,
    ErrorCircle16 = 0xF3F0,
    ErrorCircle20 = 0xF3F1,
    ErrorCircle24 = 0xF3F2,
    Dismiss32 = 0xF3F3,
    ExtendedDock24 = 0xF3F4,
    VideoPersonClock24 = 0xF3F5,
    VideoPersonClock28 = 0xF3F6,
    VideoPersonClock32 = 0xF3F7,
    VideoPersonClock48 = 0xF3F8,
    Voicemail32 = 0xF3F9,
    WebAsset16 = 0xF3FA,
    TextIndentIncreaseRtlRotate27020 = 0xF3FB,
    TextIndentIncreaseRtlRotate27024 = 0xF3FC,
    FastAcceleration24 = 0xF3FD,
    FastForward20 = 0xF3FE,
    FastForward24 = 0xF3FF,
    Dismiss48 = 0xF400,
    DocumentArrowUp16 = 0xF401,
    DocumentBulletList20 = 0xF402,
    DocumentBulletList24 = 0xF403,
    DocumentLink20 = 0xF404,
    DocumentLink24 = 0xF405,
    Filter20 = 0xF406,
    Filter24 = 0xF407,
    Filter28 = 0xF408,
    Fingerprint24 = 0xF409,
    Flag16 = 0xF40A,
    Flag20 = 0xF40B,
    Flag24 = 0xF40C,
    Flag28 = 0xF40D,
    Flag48 = 0xF40E,
    FlagOff24 = 0xF40F,
    FlagOff28 = 0xF410,
    FlagOff48 = 0xF411,
    FlashAuto24 = 0xF412,
    FlashOff24 = 0xF413,
    TextNumberListLtr9020 = 0xF414,
    TextNumberListLtr9024 = 0xF415,
    Flashlight24 = 0xF416,
    FlashlightOff24 = 0xF417,
    Folder20 = 0xF418,
    Folder24 = 0xF419,
    Folder28 = 0xF41A,
    Folder48 = 0xF41B,
    FolderAdd20 = 0xF41C,
    FolderAdd24 = 0xF41D,
    FolderAdd28 = 0xF41E,
    FolderAdd48 = 0xF41F,
    FolderBriefcase20 = 0xF420,
    DocumentPerson16 = 0xF421,
    DocumentSettings16 = 0xF422,
    DocumentSplitHint24 = 0xF423,
    DocumentSplitHintOff24 = 0xF424,
    FolderLink20 = 0xF425,
    FolderLink24 = 0xF426,
    FolderLink28 = 0xF427,
    FolderLink48 = 0xF428,
    EditArrowBack16 = 0xF429,
    EqualOff20 = 0xF42A,
    ErrorCircleSettings16 = 0xF42B,
    ExpandUpLeft16 = 0xF42C,
    FolderOpen16 = 0xF42D,
    FolderOpen20 = 0xF42E,
    FolderOpen24 = 0xF42F,
    FolderOpenVertical20 = 0xF430,
    ExpandUpLeft20 = 0xF431,
    ExpandUpLeft24 = 0xF432,
    ExpandUpLeft28 = 0xF433,
    FolderZip16 = 0xF434,
    FolderZip20 = 0xF435,
    FolderZip24 = 0xF436,
    FontDecrease20 = 0xF437,
    FontDecrease24 = 0xF438,
    FontIncrease20 = 0xF439,
    FontIncrease24 = 0xF43A,
    FontSpaceTrackingIn16 = 0xF43B,
    FontSpaceTrackingIn20 = 0xF43C,
    FontSpaceTrackingIn24 = 0xF43D,
    FontSpaceTrackingIn28 = 0xF43E,
    FontSpaceTrackingOut16 = 0xF43F,
    FontSpaceTrackingOut20 = 0xF440,
    FontSpaceTrackingOut24 = 0xF441,
    FontSpaceTrackingOut28 = 0xF442,
    Food20 = 0xF443,
    Food24 = 0xF444,
    FoodCake24 = 0xF445,
    FoodEgg24 = 0xF446,
    FoodToast24 = 0xF447,
    FormNew24 = 0xF448,
    FormNew28 = 0xF449,
    FormNew48 = 0xF44A,
    ExpandUpLeft32 = 0xF44B,
    ExpandUpLeft48 = 0xF44C,
    Fps24024 = 0xF44D,
    Fps96024 = 0xF44E,
    ExpandUpRight16 = 0xF44F,
    ExpandUpRight20 = 0xF450,
    Games24 = 0xF451,
    Gesture24 = 0xF452,
    Gif20 = 0xF453,
    Gif24 = 0xF454,
    Gift20 = 0xF455,
    Gift24 = 0xF456,
    Glance24 = 0xF457,
    Glasses24 = 0xF458,
    GlassesOff24 = 0xF459,
    Globe20 = 0xF45A,
    Globe24 = 0xF45B,
    GlobeAdd24 = 0xF45C,
    GlobeClock24 = 0xF45D,
    GlobeDesktop24 = 0xF45E,
    GlobeLocation24 = 0xF45F,
    GlobeSearch24 = 0xF460,
    GlobeVideo24 = 0xF461,
    Grid20 = 0xF462,
    Grid24 = 0xF463,
    Grid28 = 0xF464,
    Group20 = 0xF465,
    Group24 = 0xF466,
    GroupList24 = 0xF467,
    Guest16 = 0xF468,
    Guest20 = 0xF469,
    Guest24 = 0xF46A,
    Guest28 = 0xF46B,
    GuestAdd24 = 0xF46C,
    ExpandUpRight24 = 0xF46D,
    Handshake16 = 0xF46E,
    Handshake20 = 0xF46F,
    Handshake24 = 0xF470,
    Hdr24 = 0xF471,
    Headphones24 = 0xF472,
    Headphones28 = 0xF473,
    Headset24 = 0xF474,
    Headset28 = 0xF475,
    HeadsetVr20 = 0xF476,
    HeadsetVr24 = 0xF477,
    Heart16 = 0xF478,
    Heart20 = 0xF479,
    Heart24 = 0xF47A,
    Highlight16 = 0xF47B,
    Highlight20 = 0xF47C,
    Highlight24 = 0xF47D,
    History20 = 0xF47E,
    History24 = 0xF47F,
    Home20 = 0xF480,
    Home24 = 0xF481,
    Home28 = 0xF482,
    HomeAdd24 = 0xF483,
    HomeCheckmark24 = 0xF484,
    Icons20 = 0xF485,
    Icons24 = 0xF486,
    Image16 = 0xF487,
    Image20 = 0xF488,
    Image24 = 0xF489,
    Image28 = 0xF48A,
    Image48 = 0xF48B,
    ImageAdd24 = 0xF48C,
    ImageAltText20 = 0xF48D,
    ImageAltText24 = 0xF48E,
    ImageCopy20 = 0xF48F,
    ImageCopy24 = 0xF490,
    ImageCopy28 = 0xF491,
    ImageEdit16 = 0xF492,
    ImageEdit20 = 0xF493,
    ImageEdit24 = 0xF494,
    ExpandUpRight28 = 0xF495,
    ExpandUpRight32 = 0xF496,
    ExpandUpRight48 = 0xF497,
    ImageOff24 = 0xF498,
    ImageSearch20 = 0xF499,
    ImageSearch24 = 0xF49A,
    ImmersiveReader20 = 0xF49B,
    ImmersiveReader24 = 0xF49C,
    Important12 = 0xF49D,
    Important16 = 0xF49E,
    Important20 = 0xF49F,
    Important24 = 0xF4A0,
    Incognito24 = 0xF4A1,
    Info16 = 0xF4A2,
    Info20 = 0xF4A3,
    Info24 = 0xF4A4,
    Info28 = 0xF4A5,
    InkingTool16 = 0xF4A6,
    InkingTool20 = 0xF4A7,
    InkingTool24 = 0xF4A8,
    InprivateAccount16 = 0xF4A9,
    InprivateAccount20 = 0xF4AA,
    InprivateAccount24 = 0xF4AB,
    InprivateAccount28 = 0xF4AC,
    Insert20 = 0xF4AD,
    Fax16 = 0xF4AE,
    Flow16 = 0xF4AF,
    TextNumberListLtrRotate27020 = 0xF4B0,
    FolderGlobe16 = 0xF4B1,
    IosChevronRight20 = 0xF4B2,
    Javascript16 = 0xF4B3,
    Javascript20 = 0xF4B4,
    Javascript24 = 0xF4B5,
    Key20 = 0xF4B6,
    Key24 = 0xF4B7,
    Keyboard20 = 0xF4B8,
    Keyboard24 = 0xF4B9,
    KeyboardDock24 = 0xF4BA,
    KeyboardLayoutFloat24 = 0xF4BB,
    KeyboardLayoutOneHandedLeft24 = 0xF4BC,
    KeyboardLayoutResize24 = 0xF4BD,
    KeyboardLayoutSplit24 = 0xF4BE,
    KeyboardShift24 = 0xF4BF,
    KeyboardShiftUppercase24 = 0xF4C0,
    KeyboardTab24 = 0xF4C1,
    Laptop16 = 0xF4C2,
    Laptop20 = 0xF4C3,
    Laptop24 = 0xF4C4,
    Laptop28 = 0xF4C5,
    FolderPerson16 = 0xF4C6,
    Gauge20 = 0xF4C7,
    Gauge24 = 0xF4C8,
    Lasso24 = 0xF4C9,
    LauncherSettings24 = 0xF4CA,
    Layer20 = 0xF4CB,
    Layer24 = 0xF4CC,
    GiftCard16 = 0xF4CD,
    GiftCard20 = 0xF4CE,
    GiftCardAdd20 = 0xF4CF,
    LeafTwo16 = 0xF4D0,
    LeafTwo20 = 0xF4D1,
    LeafTwo24 = 0xF4D2,
    Library24 = 0xF4D3,
    Library28 = 0xF4D4,
    Lightbulb16 = 0xF4D5,
    Lightbulb20 = 0xF4D6,
    Lightbulb24 = 0xF4D7,
    LightbulbCircle24 = 0xF4D8,
    LightbulbFilament16 = 0xF4D9,
    LightbulbFilament20 = 0xF4DA,
    LightbulbFilament24 = 0xF4DB,
    GlobeLocation20 = 0xF4DC,
    Likert16 = 0xF4DD,
    Likert20 = 0xF4DE,
    Likert24 = 0xF4DF,
    LineHorizontal120 = 0xF4E0,
    LineHorizontal320 = 0xF4E1,
    LineHorizontal520 = 0xF4E2,
    Link16 = 0xF4E3,
    Link20 = 0xF4E4,
    Link24 = 0xF4E5,
    Link28 = 0xF4E6,
    Link48 = 0xF4E7,
    LinkEdit16 = 0xF4E8,
    LinkEdit20 = 0xF4E9,
    LinkEdit24 = 0xF4EA,
    GlobeStar16 = 0xF4EB,
    LinkSquare24 = 0xF4EC,
    List20 = 0xF4ED,
    List24 = 0xF4EE,
    List28 = 0xF4EF,
    Live20 = 0xF4F0,
    Live24 = 0xF4F1,
    LocalLanguage16 = 0xF4F2,
    LocalLanguage20 = 0xF4F3,
    LocalLanguage24 = 0xF4F4,
    LocalLanguage28 = 0xF4F5,
    Location12 = 0xF4F6,
    Location16 = 0xF4F7,
    Location20 = 0xF4F8,
    Location24 = 0xF4F9,
    Location28 = 0xF4FA,
    LocationLive20 = 0xF4FB,
    LocationLive24 = 0xF4FC,
    GlobeVideo20 = 0xF4FD,
    HeadsetAdd20 = 0xF4FE,
    HeadsetAdd24 = 0xF4FF,
    Heart28 = 0xF500,
    HeartBroken16 = 0xF501,
    LockShield20 = 0xF502,
    LockShield24 = 0xF503,
    LockShield48 = 0xF504,
    LaptopDismiss16 = 0xF505,
    Mail20 = 0xF506,
    Mail24 = 0xF507,
    Mail28 = 0xF508,
    Mail48 = 0xF509,
    MailAdd24 = 0xF50A,
    TextNumberListLtrRotate27024 = 0xF50B,
    TextNumberListRtl9020 = 0xF50C,
    MailAdd16 = 0xF50D,
    MailAllRead20 = 0xF50E,
    MailAllUnread20 = 0xF50F,
    MailClock20 = 0xF510,
    MailCopy20 = 0xF511,
    MailCopy24 = 0xF512,
    MailInbox16 = 0xF513,
    MailInbox20 = 0xF514,
    MailInbox24 = 0xF515,
    MailInbox28 = 0xF516,
    MailInboxAdd16 = 0xF517,
    MailInboxAdd20 = 0xF518,
    MailInboxAdd24 = 0xF519,
    MailInboxAdd28 = 0xF51A,
    MailInboxDismiss16 = 0xF51B,
    MailInboxDismiss20 = 0xF51C,
    MailInboxDismiss24 = 0xF51D,
    MailInboxDismiss28 = 0xF51E,
    MailAdd20 = 0xF51F,
    MailAlert16 = 0xF520,
    MailRead20 = 0xF521,
    MailRead24 = 0xF522,
    MailRead28 = 0xF523,
    MailRead48 = 0xF524,
    MailUnread16 = 0xF525,
    MailUnread20 = 0xF526,
    MailUnread24 = 0xF527,
    MailUnread28 = 0xF528,
    MailUnread48 = 0xF529,
    MailAlert20 = 0xF52A,
    MailAlert24 = 0xF52B,
    MailArrowDown16 = 0xF52C,
    MailArrowUp20 = 0xF52D,
    Map24 = 0xF52E,
    MapDrive16 = 0xF52F,
    MapDrive20 = 0xF530,
    MapDrive24 = 0xF531,
    MatchAppLayout24 = 0xF532,
    Maximize16 = 0xF533,
    MeetNow20 = 0xF534,
    MeetNow24 = 0xF535,
    Megaphone16 = 0xF536,
    Megaphone20 = 0xF537,
    Megaphone24 = 0xF538,
    Megaphone28 = 0xF539,
    MegaphoneOff24 = 0xF53A,
    Mention16 = 0xF53B,
    Mention20 = 0xF53C,
    Mention24 = 0xF53D,
    Merge24 = 0xF53E,
    MicOff12 = 0xF53F,
    MicOff16 = 0xF540,
    MicOff24 = 0xF541,
    MicOff28 = 0xF542,
    TextNumberListRtl9024 = 0xF543,
    TextNumberListRtlRotate27020 = 0xF544,
    TextNumberListRtlRotate27024 = 0xF545,
    TextT12 = 0xF546,
    TextT16 = 0xF547,
    MicSettings24 = 0xF548,
    Midi20 = 0xF549,
    Midi24 = 0xF54A,
    MailArrowUp24 = 0xF54B,
    MailCheckmark16 = 0xF54C,
    MobileOptimized24 = 0xF54D,
    Money16 = 0xF54E,
    Money20 = 0xF54F,
    Money24 = 0xF550,
    MailClock16 = 0xF551,
    MailClock24 = 0xF552,
    MailDismiss20 = 0xF553,
    MailDismiss24 = 0xF554,
    MailError20 = 0xF555,
    MoreVertical20 = 0xF556,
    MoreVertical24 = 0xF557,
    MoreVertical28 = 0xF558,
    MoreVertical48 = 0xF559,
    MoviesAndTv24 = 0xF55A,
    TextT32 = 0xF55B,
    TextboxSettings20 = 0xF55C,
    MailError24 = 0xF55D,
    MailInboxArrowDown16 = 0xF55E,
    MyLocation24 = 0xF55F,
    Navigation20 = 0xF560,
    Navigation24 = 0xF561,
    NetworkCheck24 = 0xF562,
    New16 = 0xF563,
    New24 = 0xF564,
    News20 = 0xF565,
    News24 = 0xF566,
    News28 = 0xF567,
    Next16 = 0xF568,
    Next20 = 0xF569,
    Next24 = 0xF56A,
    Note20 = 0xF56B,
    Note24 = 0xF56C,
    NoteAdd16 = 0xF56D,
    NoteAdd20 = 0xF56E,
    NoteAdd24 = 0xF56F,
    Notebook24 = 0xF570,
    NotebookError24 = 0xF571,
    NotebookLightning24 = 0xF572,
    NotebookQuestionMark24 = 0xF573,
    NotebookSection24 = 0xF574,
    NotebookSync24 = 0xF575,
    Notepad20 = 0xF576,
    Notepad24 = 0xF577,
    Notepad28 = 0xF578,
    NumberRow16 = 0xF579,
    NumberRow20 = 0xF57A,
    NumberRow24 = 0xF57B,
    NumberSymbol16 = 0xF57C,
    NumberSymbol20 = 0xF57D,
    NumberSymbol24 = 0xF57E,
    TextboxSettings24 = 0xF57F,
    VoicemailSubtract20 = 0xF580,
    Open16 = 0xF581,
    Open20 = 0xF582,
    Open24 = 0xF583,
    OpenFolder24 = 0xF584,
    MailLink20 = 0xF585,
    Options16 = 0xF586,
    Options20 = 0xF587,
    Options24 = 0xF588,
    Organization20 = 0xF589,
    Organization24 = 0xF58A,
    Organization28 = 0xF58B,
    MailLink24 = 0xF58C,
    Add32 = 0xF58D,
    PageFit16 = 0xF58E,
    PageFit20 = 0xF58F,
    PageFit24 = 0xF590,
    PaintBrush16 = 0xF591,
    PaintBrush20 = 0xF592,
    PaintBrush24 = 0xF593,
    PaintBucket16 = 0xF594,
    PaintBucket20 = 0xF595,
    PaintBucket24 = 0xF596,
    Pair24 = 0xF597,
    Add48 = 0xF598,
    Apps48 = 0xF599,
    ArrowTrendingSparkle20 = 0xF59A,
    ArrowTrendingSparkle24 = 0xF59B,
    Bluetooth16 = 0xF59C,
    Bluetooth32 = 0xF59D,
    Password24 = 0xF59E,
    Patient24 = 0xF59F,
    Pause16 = 0xF5A0,
    Pause20 = 0xF5A1,
    Pause24 = 0xF5A2,
    Pause48 = 0xF5A3,
    Payment20 = 0xF5A4,
    Payment24 = 0xF5A5,
    MailPause16 = 0xF5A6,
    People16 = 0xF5A7,
    People20 = 0xF5A8,
    People24 = 0xF5A9,
    People28 = 0xF5AA,
    PeopleAdd16 = 0xF5AB,
    PeopleAdd20 = 0xF5AC,
    PeopleAdd24 = 0xF5AD,
    PeopleAudience24 = 0xF5AE,
    PeopleCommunity16 = 0xF5AF,
    PeopleCommunity20 = 0xF5B0,
    PeopleCommunity24 = 0xF5B1,
    PeopleCommunity28 = 0xF5B2,
    PeopleCommunityAdd24 = 0xF5B3,
    PeopleProhibited20 = 0xF5B4,
    PeopleSearch24 = 0xF5B5,
    PeopleSettings20 = 0xF5B6,
    PeopleTeam16 = 0xF5B7,
    PeopleTeam20 = 0xF5B8,
    PeopleTeam24 = 0xF5B9,
    PeopleTeam28 = 0xF5BA,
    Person12 = 0xF5BB,
    Person16 = 0xF5BC,
    Person20 = 0xF5BD,
    Person24 = 0xF5BE,
    Person28 = 0xF5BF,
    Person48 = 0xF5C0,
    PersonAccounts24 = 0xF5C1,
    PersonAdd20 = 0xF5C2,
    PersonAdd24 = 0xF5C3,
    PersonArrowLeft20 = 0xF5C4,
    PersonArrowLeft24 = 0xF5C5,
    PersonArrowRight16 = 0xF5C6,
    PersonArrowRight20 = 0xF5C7,
    PersonArrowRight24 = 0xF5C8,
    PersonAvailable16 = 0xF5C9,
    PersonAvailable24 = 0xF5CA,
    MailProhibited20 = 0xF5CB,
    PersonBoard16 = 0xF5CC,
    PersonBoard20 = 0xF5CD,
    PersonBoard24 = 0xF5CE,
    PersonCall24 = 0xF5CF,
    PersonDelete16 = 0xF5D0,
    PersonDelete24 = 0xF5D1,
    PersonFeedback20 = 0xF5D2,
    PersonFeedback24 = 0xF5D3,
    PersonProhibited20 = 0xF5D4,
    PersonQuestionMark16 = 0xF5D5,
    PersonQuestionMark20 = 0xF5D6,
    PersonQuestionMark24 = 0xF5D7,
    PersonSupport16 = 0xF5D8,
    PersonSupport20 = 0xF5D9,
    PersonSupport24 = 0xF5DA,
    PersonSwap16 = 0xF5DB,
    PersonSwap20 = 0xF5DC,
    PersonSwap24 = 0xF5DD,
    PersonVoice20 = 0xF5DE,
    PersonVoice24 = 0xF5DF,
    Phone20 = 0xF5E0,
    Phone24 = 0xF5E1,
    MailProhibited24 = 0xF5E2,
    MailSettings16 = 0xF5E3,
    PhoneDesktop16 = 0xF5E4,
    PhoneDesktop20 = 0xF5E5,
    PhoneDesktop24 = 0xF5E6,
    PhoneDesktop28 = 0xF5E7,
    MailShield16 = 0xF5E8,
    MailTemplate20 = 0xF5E9,
    PhoneLaptop20 = 0xF5EA,
    PhoneLaptop24 = 0xF5EB,
    PhoneLinkSetup24 = 0xF5EC,
    MailTemplate24 = 0xF5ED,
    MailWarning16 = 0xF5EE,
    PhonePageHeader24 = 0xF5EF,
    PhonePagination24 = 0xF5F0,
    PhoneScreenTime24 = 0xF5F1,
    PhoneShake24 = 0xF5F2,
    PhoneStatusBar24 = 0xF5F3,
    PhoneTablet20 = 0xF5F4,
    PhoneTablet24 = 0xF5F5,
    MeetNow28 = 0xF5F6,
    MeetNow32 = 0xF5F7,
    PhoneUpdate24 = 0xF5F8,
    PhoneVerticalScroll24 = 0xF5F9,
    PhoneVibrate24 = 0xF5FA,
    PhotoFilter24 = 0xF5FB,
    PictureInPicture16 = 0xF5FC,
    PictureInPicture20 = 0xF5FD,
    PictureInPicture24 = 0xF5FE,
    Pin12 = 0xF5FF,
    Pin16 = 0xF600,
    Pin20 = 0xF601,
    Pin24 = 0xF602,
    PinOff20 = 0xF603,
    PinOff24 = 0xF604,
    Play20 = 0xF605,
    Play24 = 0xF606,
    Play48 = 0xF607,
    PlayCircle24 = 0xF608,
    PlugDisconnected20 = 0xF609,
    PlugDisconnected24 = 0xF60A,
    PlugDisconnected28 = 0xF60B,
    PointScan24 = 0xF60C,
    Poll24 = 0xF60D,
    Power20 = 0xF60E,
    Power24 = 0xF60F,
    Power28 = 0xF610,
    Predictions24 = 0xF611,
    Premium16 = 0xF612,
    Premium20 = 0xF613,
    Premium24 = 0xF614,
    Premium28 = 0xF615,
    Presenter24 = 0xF622,
    PresenterOff24 = 0xF623,
    PreviewLink16 = 0xF624,
    PreviewLink20 = 0xF625,
    PreviewLink24 = 0xF626,
    Previous16 = 0xF627,
    Previous20 = 0xF628,
    Previous24 = 0xF629,
    Print20 = 0xF62A,
    Print24 = 0xF62B,
    Print48 = 0xF62C,
    Prohibited20 = 0xF62D,
    Prohibited24 = 0xF62E,
    Prohibited28 = 0xF62F,
    Prohibited48 = 0xF630,
    MeetNow48 = 0xF631,
    ProtocolHandler16 = 0xF632,
    ProtocolHandler20 = 0xF633,
    ProtocolHandler24 = 0xF634,
    QrCode24 = 0xF635,
    QrCode28 = 0xF636,
    Question16 = 0xF637,
    Question20 = 0xF638,
    Question24 = 0xF639,
    Question28 = 0xF63A,
    Question48 = 0xF63B,
    QuestionCircle16 = 0xF63C,
    QuestionCircle20 = 0xF63D,
    QuestionCircle24 = 0xF63E,
    QuestionCircle28 = 0xF63F,
    QuestionCircle48 = 0xF640,
    QuizNew24 = 0xF641,
    QuizNew28 = 0xF642,
    QuizNew48 = 0xF643,
    RadioButton20 = 0xF644,
    RadioButton24 = 0xF645,
    RatingMature16 = 0xF646,
    RatingMature20 = 0xF647,
    RatingMature24 = 0xF648,
    ReOrder16 = 0xF649,
    ReOrder24 = 0xF64A,
    MegaphoneLoud20 = 0xF64B,
    Microscope20 = 0xF64C,
    ReadAloud20 = 0xF64D,
    ReadAloud24 = 0xF64E,
    Microscope24 = 0xF64F,
    Molecule16 = 0xF650,
    ReadingList16 = 0xF651,
    ReadingList20 = 0xF652,
    ReadingList24 = 0xF653,
    ReadingList28 = 0xF654,
    ReadingListAdd16 = 0xF655,
    ReadingListAdd20 = 0xF656,
    ReadingListAdd24 = 0xF657,
    ReadingListAdd28 = 0xF658,
    Molecule20 = 0xF659,
    Molecule24 = 0xF65A,
    ReadingModeMobile20 = 0xF65B,
    ReadingModeMobile24 = 0xF65C,
    Molecule28 = 0xF65D,
    Molecule32 = 0xF65E,
    Molecule48 = 0xF65F,
    Record16 = 0xF660,
    Record20 = 0xF661,
    Record24 = 0xF662,
    Note16 = 0xF663,
    NotePin16 = 0xF664,
    Notepad16 = 0xF665,
    NotepadEdit16 = 0xF666,
    Open32 = 0xF667,
    Rename16 = 0xF668,
    Rename20 = 0xF669,
    Rename24 = 0xF66A,
    Rename28 = 0xF66B,
    Resize20 = 0xF66C,
    ResizeImage24 = 0xF66D,
    ResizeTable24 = 0xF66E,
    ResizeVideo24 = 0xF66F,
    Bluetooth48 = 0xF670,
    Reward16 = 0xF671,
    Reward20 = 0xF672,
    Reward24 = 0xF673,
    Rewind20 = 0xF674,
    Rewind24 = 0xF675,
    Rocket16 = 0xF676,
    Rocket20 = 0xF677,
    Rocket24 = 0xF678,
    Router24 = 0xF679,
    RowTriple24 = 0xF67A,
    Ruler16 = 0xF67B,
    Ruler20 = 0xF67C,
    Ruler24 = 0xF67D,
    Run24 = 0xF67E,
    Save20 = 0xF67F,
    Save24 = 0xF680,
    PaddingDown20 = 0xF681,
    PaddingDown24 = 0xF682,
    SaveCopy24 = 0xF683,
    Savings16 = 0xF684,
    Savings20 = 0xF685,
    Savings24 = 0xF686,
    ScaleFill24 = 0xF687,
    ScaleFit16 = 0xF688,
    ScaleFit20 = 0xF689,
    ScaleFit24 = 0xF68A,
    Scan24 = 0xF68B,
    Scratchpad24 = 0xF68C,
    Screenshot20 = 0xF68D,
    Screenshot24 = 0xF68E,
    Search20 = 0xF68F,
    Search24 = 0xF690,
    Search28 = 0xF691,
    SearchInfo20 = 0xF692,
    SearchInfo24 = 0xF693,
    SearchSquare24 = 0xF694,
    PaddingLeft20 = 0xF695,
    SelectAllOff24 = 0xF696,
    SelectObject20 = 0xF697,
    SelectObject24 = 0xF698,
    Send20 = 0xF699,
    Send24 = 0xF69A,
    Send28 = 0xF69B,
    SendClock20 = 0xF69C,
    SendCopy24 = 0xF69D,
    PaddingLeft24 = 0xF69E,
    PaddingRight20 = 0xF69F,
    PaddingRight24 = 0xF6A0,
    SerialPort16 = 0xF6A1,
    SerialPort20 = 0xF6A2,
    SerialPort24 = 0xF6A3,
    ServiceBell24 = 0xF6A4,
    BotSparkle20 = 0xF6A5,
    BotSparkle24 = 0xF6A6,
    BoxSearch16 = 0xF6A7,
    Settings16 = 0xF6A8,
    Settings20 = 0xF6A9,
    Settings24 = 0xF6AA,
    Settings28 = 0xF6AB,
    Shapes16 = 0xF6AC,
    Shapes20 = 0xF6AD,
    Shapes24 = 0xF6AE,
    Share20 = 0xF6AF,
    Share24 = 0xF6B0,
    ShareAndroid20 = 0xF6B1,
    ShareAndroid24 = 0xF6B2,
    ShareCloseTray24 = 0xF6B3,
    PaddingTop20 = 0xF6B4,
    ShareIos20 = 0xF6B5,
    ShareIos24 = 0xF6B6,
    ShareIos28 = 0xF6B7,
    ShareIos48 = 0xF6B8,
    PaddingTop24 = 0xF6B9,
    Patch20 = 0xF6BA,
    Patch24 = 0xF6BB,
    PauseCircle20 = 0xF6BC,
    PeopleSync16 = 0xF6BD,
    Shield20 = 0xF6BE,
    Shield24 = 0xF6BF,
    ShieldDismiss20 = 0xF6C0,
    ShieldDismiss24 = 0xF6C1,
    ShieldError20 = 0xF6C2,
    ShieldError24 = 0xF6C3,
    ShieldKeyhole16 = 0xF6C4,
    ShieldKeyhole20 = 0xF6C5,
    ShieldKeyhole24 = 0xF6C6,
    ShieldProhibited20 = 0xF6C7,
    ShieldProhibited24 = 0xF6C8,
    Shifts24 = 0xF6C9,
    PeopleToolbox16 = 0xF6CA,
    PersonChat16 = 0xF6CB,
    Shifts28 = 0xF6CC,
    Shifts30Minutes24 = 0xF6CD,
    ShiftsActivity20 = 0xF6CE,
    ShiftsActivity24 = 0xF6CF,
    ShiftsAdd24 = 0xF6D0,
    PersonChat20 = 0xF6D1,
    ShiftsAvailability24 = 0xF6D2,
    PersonChat24 = 0xF6D3,
    ShiftsOpen20 = 0xF6D4,
    ShiftsOpen24 = 0xF6D5,
    PersonInfo16 = 0xF6D6,
    ShiftsTeam24 = 0xF6D7,
    PersonLock16 = 0xF6D8,
    PersonLock20 = 0xF6D9,
    SignOut24 = 0xF6DA,
    Signature16 = 0xF6DB,
    Signature20 = 0xF6DC,
    Signature24 = 0xF6DD,
    Signature28 = 0xF6DE,
    Building32 = 0xF6DF,
    Building48 = 0xF6E0,
    CalendarError16 = 0xF6E1,
    Sim16 = 0xF6E2,
    Sim20 = 0xF6E3,
    Sim24 = 0xF6E4,
    Sleep24 = 0xF6E5,
    SlideAdd24 = 0xF6E6,
    CallForward32 = 0xF6E7,
    SlideHide24 = 0xF6E8,
    SlideLayout20 = 0xF6E9,
    SlideLayout24 = 0xF6EA,
    SlideMicrophone24 = 0xF6EB,
    SlideText24 = 0xF6EC,
    PersonSubtract16 = 0xF6ED,
    Phone16 = 0xF6EE,
    PhoneCheckmark16 = 0xF6EF,
    Pill16 = 0xF6F0,
    Pill20 = 0xF6F1,
    Pill24 = 0xF6F2,
    Pill28 = 0xF6F3,
    Snooze16 = 0xF6F4,
    Snooze24 = 0xF6F5,
    SoundSource24 = 0xF6F6,
    SoundSource28 = 0xF6F7,
    Spacebar24 = 0xF6F8,
    Speaker024 = 0xF6F9,
    Print16 = 0xF6FA,
    Speaker124 = 0xF6FB,
    PrintAdd20 = 0xF6FC,
    Production20 = 0xF6FD,
    Production24 = 0xF6FE,
    SpeakerBluetooth24 = 0xF6FF,
    SpeakerEdit16 = 0xF700,
    SpeakerEdit20 = 0xF701,
    SpeakerEdit24 = 0xF702,
    ProductionCheckmark20 = 0xF703,
    ProductionCheckmark24 = 0xF704,
    Prohibited16 = 0xF705,
    SpeakerOff24 = 0xF706,
    SpeakerOff28 = 0xF707,
    SpeakerSettings24 = 0xF708,
    SpinnerIos20 = 0xF709,
    RatioOneToOne20 = 0xF70A,
    RatioOneToOne24 = 0xF70B,
    ReceiptAdd20 = 0xF70C,
    Star12 = 0xF70D,
    Star16 = 0xF70E,
    Star20 = 0xF70F,
    Star24 = 0xF710,
    Star28 = 0xF711,
    StarAdd16 = 0xF712,
    StarAdd20 = 0xF713,
    StarAdd24 = 0xF714,
    ReceiptBag20 = 0xF715,
    StarArrowRightStart24 = 0xF716,
    StarEmphasis24 = 0xF717,
    StarOff12 = 0xF718,
    StarOff16 = 0xF719,
    StarOff20 = 0xF71A,
    StarOff24 = 0xF71B,
    StarOff28 = 0xF71C,
    StarProhibited16 = 0xF71D,
    StarProhibited20 = 0xF71E,
    StarProhibited24 = 0xF71F,
    StarSettings24 = 0xF720,
    Status16 = 0xF721,
    Status20 = 0xF722,
    Status24 = 0xF723,
    Stethoscope20 = 0xF724,
    Stethoscope24 = 0xF725,
    Sticker20 = 0xF726,
    Sticker24 = 0xF727,
    StickerAdd24 = 0xF728,
    Stop16 = 0xF729,
    Stop20 = 0xF72A,
    Stop24 = 0xF72B,
    Storage24 = 0xF72C,
    ReceiptCube20 = 0xF72D,
    ReceiptMoney20 = 0xF72E,
    Record12 = 0xF72F,
    StoreMicrosoft16 = 0xF730,
    StoreMicrosoft20 = 0xF731,
    StoreMicrosoft24 = 0xF732,
    StyleGuide24 = 0xF733,
    SubGrid24 = 0xF734,
    Record28 = 0xF735,
    Record32 = 0xF736,
    Record48 = 0xF737,
    SurfaceEarbuds20 = 0xF738,
    SurfaceEarbuds24 = 0xF739,
    SurfaceHub20 = 0xF73A,
    SurfaceHub24 = 0xF73B,
    SwipeDown24 = 0xF73C,
    SwipeRight24 = 0xF73D,
    SwipeUp24 = 0xF73E,
    Symbols24 = 0xF73F,
    SyncOff16 = 0xF740,
    SyncOff20 = 0xF741,
    System24 = 0xF742,
    Tab16 = 0xF743,
    Tab20 = 0xF744,
    Tab24 = 0xF745,
    Tab28 = 0xF746,
    TabDesktop20 = 0xF747,
    TabDesktopArrowClockwise16 = 0xF748,
    TabDesktopArrowClockwise20 = 0xF749,
    TabDesktopArrowClockwise24 = 0xF74A,
    TabDesktopClock20 = 0xF74B,
    TabDesktopCopy20 = 0xF74C,
    TabDesktopImage16 = 0xF74D,
    TabDesktopImage20 = 0xF74E,
    TabDesktopImage24 = 0xF74F,
    TabDesktopMultiple20 = 0xF750,
    TabDesktopNewPage20 = 0xF751,
    TabInPrivate16 = 0xF752,
    TabInPrivate20 = 0xF753,
    TabInPrivate24 = 0xF754,
    TabInPrivate28 = 0xF755,
    TabInprivateAccount20 = 0xF756,
    TabInprivateAccount24 = 0xF757,
    RecordStop12 = 0xF758,
    RecordStop16 = 0xF759,
    RecordStop20 = 0xF75A,
    RecordStop24 = 0xF75B,
    RecordStop28 = 0xF75C,
    Table20 = 0xF75D,
    Table24 = 0xF75E,
    TableAdd24 = 0xF75F,
    TableCellsMerge20 = 0xF760,
    TableCellsMerge24 = 0xF761,
    TableCellsSplit20 = 0xF762,
    TableCellsSplit24 = 0xF763,
    RecordStop32 = 0xF764,
    RecordStop48 = 0xF765,
    RibbonAdd20 = 0xF766,
    RibbonAdd24 = 0xF767,
    TableEdit24 = 0xF768,
    Server20 = 0xF769,
    TableFreezeColumn24 = 0xF76A,
    TableFreezeRow24 = 0xF76B,
    Server24 = 0xF76C,
    ShieldBadge20 = 0xF76D,
    ShoppingBag16 = 0xF76E,
    ShoppingBag20 = 0xF76F,
    ShoppingBag24 = 0xF770,
    TableMoveLeft24 = 0xF771,
    TableMoveRight24 = 0xF772,
    SlideMultipleSearch20 = 0xF773,
    SlideMultipleSearch24 = 0xF774,
    Smartwatch20 = 0xF775,
    Smartwatch24 = 0xF776,
    TableSettings24 = 0xF777,
    TableSwitch24 = 0xF778,
    Tablet20 = 0xF779,
    Tablet24 = 0xF77A,
    Tabs24 = 0xF77B,
    Tag20 = 0xF77C,
    Tag24 = 0xF77D,
    TapDouble24 = 0xF77E,
    TapSingle24 = 0xF77F,
    Target16 = 0xF780,
    Target20 = 0xF781,
    Target24 = 0xF782,
    TargetEdit16 = 0xF783,
    TargetEdit20 = 0xF784,
    TargetEdit24 = 0xF785,
    SmartwatchDot20 = 0xF786,
    SmartwatchDot24 = 0xF787,
    TaskListAdd20 = 0xF788,
    TaskListAdd24 = 0xF789,
    TasksApp24 = 0xF78A,
    TasksApp28 = 0xF78B,
    SquareMultiple24 = 0xF78C,
    Stack16 = 0xF78D,
    Teddy24 = 0xF78E,
    Temperature20 = 0xF78F,
    Temperature24 = 0xF790,
    Tent24 = 0xF791,
    Stack20 = 0xF792,
    ChatMultipleHeart16 = 0xF793,
    ChatMultipleHeart20 = 0xF794,
    TextAddSpaceAfter20 = 0xF795,
    TextAddSpaceAfter24 = 0xF796,
    TextAddSpaceBefore20 = 0xF797,
    TextAddSpaceBefore24 = 0xF798,
    TextAlignCenter20 = 0xF799,
    TextAlignCenter24 = 0xF79A,
    TextAlignDistributed20 = 0xF79B,
    TextAlignDistributed24 = 0xF79C,
    TextAlignJustify20 = 0xF79D,
    TextAlignJustify24 = 0xF79E,
    TextAlignLeft20 = 0xF79F,
    TextAlignLeft24 = 0xF7A0,
    TextAlignRight20 = 0xF7A1,
    TextAlignRight24 = 0xF7A2,
    TextAsterisk20 = 0xF7A3,
    TextBold20 = 0xF7A4,
    TextBold24 = 0xF7A5,
    Stack24 = 0xF7A6,
    SubtractCircle16 = 0xF7A7,
    TextBulletListAdd24 = 0xF7A8,
    TextBulletListSquare24 = 0xF7A9,
    TextBulletListSquareWarning16 = 0xF7AA,
    TextBulletListSquareWarning20 = 0xF7AB,
    TextBulletListSquareWarning24 = 0xF7AC,
    TextBulletListTree16 = 0xF7AD,
    TextBulletListTree20 = 0xF7AE,
    TextBulletListTree24 = 0xF7AF,
    SubtractCircle20 = 0xF7B0,
    SubtractCircle24 = 0xF7B1,
    TextChangeCase20 = 0xF7B2,
    TextChangeCase24 = 0xF7B3,
    SubtractCircle28 = 0xF7B4,
    SubtractCircle32 = 0xF7B5,
    TagMultiple16 = 0xF7B6,
    TargetArrow16 = 0xF7B7,
    TargetArrow20 = 0xF7B8,
    TextBulletListSquareEdit20 = 0xF7B9,
    TextBulletListSquareEdit24 = 0xF7BA,
    TooltipQuote20 = 0xF7BB,
    TextClearFormatting20 = 0xF7BC,
    TextClearFormatting24 = 0xF7BD,
    TextCollapse24 = 0xF7BE,
    TextColor20 = 0xF7BF,
    TextColor24 = 0xF7C0,
    TextColumnOne20 = 0xF7C1,
    TextColumnOne24 = 0xF7C2,
    TextColumnThree20 = 0xF7C3,
    TextColumnThree24 = 0xF7C4,
    TextColumnTwo20 = 0xF7C5,
    TextColumnTwo24 = 0xF7C6,
    TextColumnTwoLeft20 = 0xF7C7,
    TextColumnTwoLeft24 = 0xF7C8,
    TextColumnTwoRight20 = 0xF7C9,
    TextColumnTwoRight24 = 0xF7CA,
    TextDescription20 = 0xF7CB,
    TextDescription24 = 0xF7CC,
    VehicleCarProfileLtr16 = 0xF7CD,
    VehicleCarProfileRtl16 = 0xF7CE,
    ChatMultipleHeart24 = 0xF7CF,
    ChatMultipleHeart28 = 0xF7D0,
    ChatMultipleHeart32 = 0xF7D1,
    ChatSparkle16 = 0xF7D2,
    ChatSparkle20 = 0xF7D3,
    ChatSparkle24 = 0xF7D4,
    ChatSparkle28 = 0xF7D5,
    ChatSparkle32 = 0xF7D6,
    TextDirectionVertical20 = 0xF7D7,
    TextDirectionVertical24 = 0xF7D8,
    TextEditStyle20 = 0xF7D9,
    TextEditStyle24 = 0xF7DA,
    TextEffects20 = 0xF7DB,
    TextEffects24 = 0xF7DC,
    TextExpand24 = 0xF7DD,
    TextField16 = 0xF7DE,
    TextField20 = 0xF7DF,
    TextField24 = 0xF7E0,
    TextFirstLine20 = 0xF7E1,
    TextFirstLine24 = 0xF7E2,
    TextFont16 = 0xF7E3,
    TextFont20 = 0xF7E4,
    TextFont24 = 0xF7E5,
    TextFontSize20 = 0xF7E6,
    TextFontSize24 = 0xF7E7,
    TextFootnote20 = 0xF7E8,
    TextFootnote24 = 0xF7E9,
    VehicleTruckProfile16 = 0xF7EA,
    VoicemailArrowBack16 = 0xF7EB,
    VoicemailArrowForward16 = 0xF7EC,
    TextHanging20 = 0xF7ED,
    TextHanging24 = 0xF7EE,
    TextHeader120 = 0xF7EF,
    TextHeader220 = 0xF7F0,
    TextHeader320 = 0xF7F1,
    ChatSparkle48 = 0xF7F2,
    ClipboardCheckmark16 = 0xF7F3,
    TextItalic20 = 0xF7F4,
    TextItalic24 = 0xF7F5,
    TextLineSpacing20 = 0xF7F6,
    TextLineSpacing24 = 0xF7F7,
    TextNumberFormat24 = 0xF7F8,
    TextNumberListLtr20 = 0xF7F9,
    TextNumberListLtr24 = 0xF7FA,
    TextNumberListRtl24 = 0xF7FB,
    VoicemailSubtract16 = 0xF7FC,
    WifiWarning24 = 0xF7FD,
    TextProofingTools20 = 0xF7FE,
    TextProofingTools24 = 0xF7FF,
    TextQuote20 = 0xF800,
    TextQuote24 = 0xF801,
    TextSortAscending20 = 0xF802,
    TextSortDescending20 = 0xF803,
    WindowEdit16 = 0xF804,
    ArrowSortDown20 = 0xF805,
    TextSubscript20 = 0xF806,
    TextSubscript24 = 0xF807,
    TextSuperscript20 = 0xF808,
    TextSuperscript24 = 0xF809,
    TextUnderline20 = 0xF80A,
    TextUnderline24 = 0xF80B,
    TextWordCount20 = 0xF80C,
    TextWordCount24 = 0xF80D,
    TextWrap24 = 0xF80E,
    Textbox20 = 0xF80F,
    Textbox24 = 0xF810,
    ArrowSortDown24 = 0xF811,
    ArrowSortUp20 = 0xF812,
    TextboxAlignBottom20 = 0xF813,
    TextboxAlignBottom24 = 0xF814,
    TextboxAlignMiddle20 = 0xF815,
    TextboxAlignMiddle24 = 0xF816,
    TextboxAlignTop20 = 0xF817,
    TextboxAlignTop24 = 0xF818,
    ClockLock16 = 0xF819,
    ClockLock20 = 0xF81A,
    Thinking20 = 0xF81B,
    Thinking24 = 0xF81C,
    ThumbDislike20 = 0xF81D,
    ThumbDislike24 = 0xF81E,
    ThumbLike20 = 0xF81F,
    ThumbLike24 = 0xF820,
    ArrowSortUp24 = 0xF821,
    ArrowTurnBidirectionalDownRight24 = 0xF822,
    TimeAndWeather24 = 0xF823,
    TimePicker24 = 0xF824,
    Timeline24 = 0xF825,
    Timer1024 = 0xF826,
    Timer24 = 0xF827,
    Timer224 = 0xF828,
    TimerOff24 = 0xF829,
    ToggleRight16 = 0xF82A,
    ToggleRight20 = 0xF82B,
    ToggleRight24 = 0xF82C,
    Toolbox16 = 0xF82D,
    Toolbox20 = 0xF82E,
    Toolbox24 = 0xF82F,
    Toolbox28 = 0xF830,
    TopSpeed24 = 0xF831,
    Translate16 = 0xF832,
    Translate20 = 0xF833,
    Translate24 = 0xF834,
    Trophy16 = 0xF835,
    Trophy20 = 0xF836,
    Trophy24 = 0xF837,
    UninstallApp24 = 0xF838,
    ArrowTurnRight24 = 0xF839,
    BookQuestionMarkRtl24 = 0xF83A,
    BrainCircuit24 = 0xF83B,
    BuildingBankToolbox24 = 0xF83C,
    ClockLock24 = 0xF83D,
    Clover16 = 0xF83E,
    UsbStick20 = 0xF83F,
    UsbStick24 = 0xF840,
    Vault16 = 0xF841,
    Vault20 = 0xF842,
    Vault24 = 0xF843,
    VehicleBicycle24 = 0xF844,
    VehicleBus24 = 0xF845,
    VehicleCab24 = 0xF846,
    VehicleCar16 = 0xF847,
    VehicleCar20 = 0xF848,
    VehicleCar24 = 0xF849,
    VehicleTruck24 = 0xF84A,
    Video16 = 0xF84B,
    Video20 = 0xF84C,
    Video24 = 0xF84D,
    Video28 = 0xF84E,
    VideoBackgroundEffect24 = 0xF84F,
    VideoClip24 = 0xF850,
    VideoOff20 = 0xF851,
    VideoOff24 = 0xF852,
    VideoOff28 = 0xF853,
    VideoPerson24 = 0xF854,
    VideoPersonOff24 = 0xF855,
    VideoPersonStar24 = 0xF856,
    VideoPlayPause24 = 0xF857,
    VideoSecurity20 = 0xF858,
    VideoSecurity24 = 0xF859,
    VideoSwitch24 = 0xF85A,
    ViewDesktop20 = 0xF85B,
    ViewDesktop24 = 0xF85C,
    ViewDesktopMobile20 = 0xF85D,
    ViewDesktopMobile24 = 0xF85E,
    CalendarCheckmark28 = 0xF85F,
    CalendarSearch16 = 0xF860,
    CallPark32 = 0xF861,
    Voicemail16 = 0xF862,
    Voicemail20 = 0xF863,
    Voicemail24 = 0xF864,
    WalkieTalkie24 = 0xF865,
    WalkieTalkie28 = 0xF866,
    Wallpaper24 = 0xF867,
    Warning16 = 0xF868,
    Warning20 = 0xF869,
    Warning24 = 0xF86A,
    WeatherBlowingSnow20 = 0xF86B,
    WeatherBlowingSnow24 = 0xF86C,
    WeatherBlowingSnow48 = 0xF86D,
    WeatherCloudy20 = 0xF86E,
    WeatherCloudy24 = 0xF86F,
    WeatherCloudy48 = 0xF870,
    WeatherDuststorm20 = 0xF871,
    WeatherDuststorm24 = 0xF872,
    WeatherDuststorm48 = 0xF873,
    WeatherFog20 = 0xF874,
    WeatherFog24 = 0xF875,
    WeatherFog48 = 0xF876,
    WeatherHailDay20 = 0xF877,
    WeatherHailDay24 = 0xF878,
    WeatherHailDay48 = 0xF879,
    WeatherHailNight20 = 0xF87A,
    WeatherHailNight24 = 0xF87B,
    WeatherHailNight48 = 0xF87C,
    WeatherMoon20 = 0xF87D,
    WeatherMoon24 = 0xF87E,
    WeatherMoon48 = 0xF87F,
    WeatherPartlyCloudyDay20 = 0xF880,
    WeatherPartlyCloudyDay24 = 0xF881,
    WeatherPartlyCloudyDay48 = 0xF882,
    WeatherPartlyCloudyNight20 = 0xF883,
    WeatherPartlyCloudyNight24 = 0xF884,
    WeatherPartlyCloudyNight48 = 0xF885,
    WeatherRain20 = 0xF886,
    WeatherRain24 = 0xF887,
    WeatherRain48 = 0xF888,
    WeatherRainShowersDay20 = 0xF889,
    WeatherRainShowersDay24 = 0xF88A,
    WeatherRainShowersDay48 = 0xF88B,
    WeatherRainShowersNight20 = 0xF88C,
    WeatherRainShowersNight24 = 0xF88D,
    WeatherRainShowersNight48 = 0xF88E,
    WeatherRainSnow20 = 0xF88F,
    WeatherRainSnow24 = 0xF890,
    WeatherRainSnow48 = 0xF891,
    WeatherSnow20 = 0xF892,
    WeatherSnow24 = 0xF893,
    WeatherSnow48 = 0xF894,
    WeatherSnowShowerDay20 = 0xF895,
    WeatherSnowShowerDay24 = 0xF896,
    WeatherSnowShowerDay48 = 0xF897,
    WeatherSnowShowerNight20 = 0xF898,
    WeatherSnowShowerNight24 = 0xF899,
    WeatherSnowShowerNight48 = 0xF89A,
    WeatherSnowflake20 = 0xF89B,
    WeatherSnowflake24 = 0xF89C,
    WeatherSnowflake48 = 0xF89D,
    WeatherSqualls20 = 0xF89E,
    WeatherSqualls24 = 0xF89F,
    WeatherSqualls48 = 0xF8A0,
    WeatherSunny20 = 0xF8A1,
    WeatherSunny24 = 0xF8A2,
    WeatherSunny48 = 0xF8A3,
    WeatherThunderstorm20 = 0xF8A4,
    WeatherThunderstorm24 = 0xF8A5,
    WeatherThunderstorm48 = 0xF8A6,
    WebAsset24 = 0xF8A7,
    ChatBubblesQuestion16 = 0xF8A8,
    ChatMultiple16 = 0xF8A9,
    Whiteboard20 = 0xF8AA,
    Whiteboard24 = 0xF8AB,
    Wifi120 = 0xF8AC,
    Wifi124 = 0xF8AD,
    Wifi220 = 0xF8AE,
    Wifi224 = 0xF8AF,
    Wifi320 = 0xF8B0,
    Wifi324 = 0xF8B1,
    Wifi420 = 0xF8B2,
    Wifi424 = 0xF8B3,
    Clover20 = 0xF8B4,
    Window20 = 0xF8B5,
    WindowAd20 = 0xF8B6,
    WindowDevTools16 = 0xF8B7,
    WindowDevTools20 = 0xF8B8,
    WindowDevTools24 = 0xF8B9,
    WindowInprivate20 = 0xF8BA,
    WindowInprivateAccount20 = 0xF8BB,
    WindowMultiple20 = 0xF8BC,
    WindowNew20 = 0xF8BD,
    WindowShield16 = 0xF8BE,
    WindowShield20 = 0xF8BF,
    WindowShield24 = 0xF8C0,
    Wrench24 = 0xF8C1,
    XboxConsole20 = 0xF8C2,
    XboxConsole24 = 0xF8C3,
    ZoomIn20 = 0xF8C4,
    ZoomIn24 = 0xF8C5,
    ZoomOut20 = 0xF8C6,
    ZoomOut24 = 0xF8C7,
    ChatMultiple20 = 0xF8C8,
    CalendarCheckmark24 = 0xF8C9,
    AddSquare24 = 0xF8CA,
    AppsList20 = 0xF8CB,
    Archive16 = 0xF8CC,
    ArrowAutofitHeight24 = 0xF8CD,
    ArrowAutofitWidth24 = 0xF8CE,
    ArrowCounterclockwise28 = 0xF8CF,
    ArrowDown12 = 0xF8D0,
    ArrowDownLeft16 = 0xF8D1,
    ArrowExportRtl20 = 0xF8D2,
    ChatMultiple24 = 0xF8D3,
    Checkmark32 = 0xF8D4,
    ArrowHookDownLeft16 = 0xF8D5,
    ArrowHookDownLeft20 = 0xF8D6,
    ArrowHookDownLeft24 = 0xF8D7,
    ArrowHookDownLeft28 = 0xF8D8,
    ArrowHookDownRight16 = 0xF8D9,
    ArrowHookDownRight20 = 0xF8DA,
    ArrowHookDownRight24 = 0xF8DB,
    ArrowHookDownRight28 = 0xF8DC,
    ArrowHookUpLeft16 = 0xF8DD,
    ArrowHookUpLeft20 = 0xF8DE,
    ArrowHookUpLeft24 = 0xF8DF,
    ArrowHookUpLeft28 = 0xF8E0,
    ArrowHookUpRight16 = 0xF8E1,
    ArrowHookUpRight20 = 0xF8E2,
    ArrowHookUpRight24 = 0xF8E3,
    ArrowHookUpRight28 = 0xF8E4,
    ArrowMove20 = 0xF8E5,
    ArrowRedo32 = 0xF8E6,
    ArrowRedo48 = 0xF8E7,
    CheckmarkCircle32 = 0xF8E8,
    Clover24 = 0xF8E9,
    Clover28 = 0xF8EA,
    ArrowUpRight16 = 0xF8EB,
    AttachArrowRight20 = 0xF8EC,
    AttachArrowRight24 = 0xF8ED,
    AttachText24 = 0xF8EE,
    Clover32 = 0xF8EF,
    Backpack12 = 0xF8F0,
    Backpack16 = 0xF8F1,
    Backpack20 = 0xF8F2,
    Backpack24 = 0xF8F3,
    Backpack28 = 0xF8F4,
    Backpack48 = 0xF8F5,
    Balloon16 = 0xF8F6,
    Bed16 = 0xF8F7,
    Bluetooth28 = 0xF8F8,
    Blur16 = 0xF8F9,
    Blur20 = 0xF8FA,
    Blur24 = 0xF8FB,
    Blur28 = 0xF8FC,
    Book20 = 0xF8FD,
    Book24 = 0xF8FE,
    BookAdd20 = 0xF8FF,
    Clover48 = 0xF0000,
    CommentLink16 = 0xF0001,
    CommentLink20 = 0xF0002,
    CommentLink24 = 0xF0003,
    CommentLink28 = 0xF0004,
    CommentLink48 = 0xF0005,
    Copy32 = 0xF0006,
    CopySelect24 = 0xF0007,
    Database48 = 0xF0008,
    DatabaseMultiple32 = 0xF0009,
    DeviceEq16 = 0xF000A,
    Document10016 = 0xF000B,
    Document10020 = 0xF000C,
    Document10024 = 0xF000D,
    DocumentBorder20 = 0xF000E,
    DocumentBorder24 = 0xF000F,
    DocumentBorder32 = 0xF0010,
    DocumentBorderPrint20 = 0xF0011,
    DocumentBorderPrint24 = 0xF0012,
    DocumentBorderPrint32 = 0xF0013,
    DocumentBulletList16 = 0xF0014,
    DocumentBulletListArrowLeft16 = 0xF0015,
    DocumentBulletListArrowLeft20 = 0xF0016,
    DocumentBulletListArrowLeft24 = 0xF0017,
    DocumentBulletListCube16 = 0xF0018,
    DocumentBulletListCube20 = 0xF0019,
    DocumentBulletListCube24 = 0xF001A,
    DocumentDataLink16 = 0xF001B,
    DocumentDataLink20 = 0xF001C,
    DocumentDataLink24 = 0xF001D,
    DocumentDataLink32 = 0xF001E,
    DocumentFit16 = 0xF001F,
    DocumentFit20 = 0xF0020,
    DocumentFit24 = 0xF0021,
    DocumentFolder16 = 0xF0022,
    DocumentFolder20 = 0xF0023,
    DocumentFolder24 = 0xF0024,
    DocumentOnePage16 = 0xF0025,
    DocumentOnePageAdd16 = 0xF0026,
    DocumentOnePageAdd20 = 0xF0027,
    DocumentOnePageAdd24 = 0xF0028,
    DocumentOnePageColumns20 = 0xF0029,
    DocumentOnePageColumns24 = 0xF002A,
    DocumentOnePageLink16 = 0xF002B,
    DocumentOnePageLink20 = 0xF002C,
    DocumentOnePageLink24 = 0xF002D,
    DocumentPrint20 = 0xF002E,
    DocumentPrint24 = 0xF002F,
    DocumentPrint28 = 0xF0030,
    DocumentPrint32 = 0xF0031,
    DocumentPrint48 = 0xF0032,
    EmojiAngry16 = 0xF0033,
    EmojiHand16 = 0xF0034,
    EmojiMeh16 = 0xF0035,
    Filmstrip16 = 0xF0036,
    Filmstrip32 = 0xF0037,
    FilmstripPlay16 = 0xF0038,
    FilmstripPlay20 = 0xF0039,
    FilmstripPlay24 = 0xF003A,
    FilmstripPlay32 = 0xF003B,
    Flag32 = 0xF003C,
    FlagClock16 = 0xF003D,
    FlagClock20 = 0xF003E,
    FlagClock24 = 0xF003F,
    FlagClock28 = 0xF0040,
    FlagClock32 = 0xF0041,
    FlagClock48 = 0xF0042,
    Glasses32 = 0xF0043,
    GlassesOff32 = 0xF0044,
    GlobeSurface32 = 0xF0045,
    HomeMore48 = 0xF0046,
    ImageBorder16 = 0xF0047,
    ImageBorder20 = 0xF0048,
    ImageBorder24 = 0xF0049,
    ImageBorder28 = 0xF004A,
    ImageBorder32 = 0xF004B,
    ImageBorder48 = 0xF004C,
    ImageCircle16 = 0xF004D,
    ImageCircle20 = 0xF004E,
    ImageCircle24 = 0xF004F,
    ImageCircle28 = 0xF0050,
    ImageCircle32 = 0xF0051,
    ImageCircle48 = 0xF0052,
    ImageTable16 = 0xF0053,
    ImageTable20 = 0xF0054,
    ImageTable24 = 0xF0055,
    ImageTable28 = 0xF0056,
    ImageTable32 = 0xF0057,
    ImageTable48 = 0xF0058,
    Info32 = 0xF0059,
    Info48 = 0xF005A,
    Iot16 = 0xF005B,
    IotAlert16 = 0xF005C,
    IotAlert20 = 0xF005D,
    IotAlert24 = 0xF005E,
    LineHorizontal420 = 0xF005F,
    LineHorizontal4Search20 = 0xF0060,
    LineThickness20 = 0xF0061,
    LineThickness24 = 0xF0062,
    LocationArrow12 = 0xF0063,
    LocationArrow16 = 0xF0064,
    LocationArrow20 = 0xF0065,
    LocationArrow24 = 0xF0066,
    LocationArrow28 = 0xF0067,
    LocationArrow32 = 0xF0068,
    LocationArrow48 = 0xF0069,
    LocationArrowLeft16 = 0xF006A,
    LocationArrowRight16 = 0xF006B,
    LocationArrowUp16 = 0xF006C,
    MailArrowDoubleBack24 = 0xF006D,
    MailCheckmark24 = 0xF006E,
    MailUnread12 = 0xF006F,
    Map16 = 0xF0070,
    Mention32 = 0xF0071,
    Mention48 = 0xF0072,
    PanelLeftHeader16 = 0xF0073,
    PanelLeftHeader20 = 0xF0074,
    PanelLeftHeader24 = 0xF0075,
    PanelLeftHeader28 = 0xF0076,
    PanelLeftHeader32 = 0xF0077,
    PanelLeftHeader48 = 0xF0078,
    PanelLeftHeaderAdd16 = 0xF0079,
    PanelLeftHeaderAdd20 = 0xF007A,
    PanelLeftHeaderAdd24 = 0xF007B,
    PanelLeftHeaderAdd28 = 0xF007C,
    PanelLeftHeaderAdd32 = 0xF007D,
    PanelLeftHeaderAdd48 = 0xF007E,
    PanelLeftHeaderKey16 = 0xF007F,
    PanelLeftHeaderKey20 = 0xF0080,
    PanelLeftHeaderKey24 = 0xF0081,
    PeopleCall24 = 0xF0082,
    PeopleCommunity32 = 0xF0083,
    PeopleCommunity48 = 0xF0084,
    PersonFeedback28 = 0xF0085,
    PersonFeedback32 = 0xF0086,
    PersonFeedback48 = 0xF0087,
    PhoneDesktop32 = 0xF0088,
    PhoneDesktop48 = 0xF0089,
    PlayCircleHint16 = 0xF008A,
    PlayCircleHint20 = 0xF008B,
    PlayCircleHint24 = 0xF008C,
    PollHorizontal16 = 0xF008D,
    PollHorizontal20 = 0xF008E,
    PollHorizontal24 = 0xF008F,
    PresenceAway10 = 0xF0090,
    PresenceAway12 = 0xF0091,
    PresenceAway16 = 0xF0092,
    PresenceAway20 = 0xF0093,
    PresenceAway24 = 0xF0094,
    ProjectionScreenText24 = 0xF0095,
    Receipt32 = 0xF0096,
    ReceiptMoney16 = 0xF0097,
    Send32 = 0xF0098,
    Send48 = 0xF0099,
    ServiceBell16 = 0xF009A,
    ShiftsActivity16 = 0xF009B,
    SlashForward12 = 0xF009C,
    SlashForward16 = 0xF009D,
    SlashForward20 = 0xF009E,
    SlashForward24 = 0xF009F,
    Space3d16 = 0xF00A0,
    Space3d20 = 0xF00A1,
    Space3d24 = 0xF00A2,
    Space3d28 = 0xF00A3,
    Space3d32 = 0xF00A4,
    Space3d48 = 0xF00A5,
    Sparkle32 = 0xF00A6,
    SparkleCircle16 = 0xF00A7,
    SparkleCircle28 = 0xF00A8,
    SparkleCircle32 = 0xF00A9,
    SparkleCircle48 = 0xF00AA,
    StarArrowBack16 = 0xF00AB,
    StarArrowBack20 = 0xF00AC,
    StarArrowBack24 = 0xF00AD,
    TableSimpleMultiple20 = 0xF00AE,
    TableSimpleMultiple24 = 0xF00AF,
    TextAbcUnderlineDouble32 = 0xF00B0,
    TextColumnOneSemiNarrow20 = 0xF00B1,
    TextColumnOneSemiNarrow24 = 0xF00B2,
    TextExpand16 = 0xF00B3,
    TextPositionSquareLeft16 = 0xF00B4,
    TextPositionSquareLeft20 = 0xF00B5,
    TextPositionSquareLeft24 = 0xF00B6,
    TextPositionSquareRight16 = 0xF00B7,
    TextPositionSquareRight20 = 0xF00B8,
    TextPositionSquareRight24 = 0xF00B9,
    TextUnderlineCharacterU16 = 0xF00BA,
    TextUnderlineCharacterU20 = 0xF00BB,
    TextUnderlineCharacterU24 = 0xF00BC,
    TranslateOff16 = 0xF00BD,
    TranslateOff20 = 0xF00BE,
    TranslateOff24 = 0xF00BF,
    VideoBackgroundEffect16 = 0xF00C0,
    VideoBackgroundEffect28 = 0xF00C1,
    VideoBackgroundEffect32 = 0xF00C2,
    VideoBackgroundEffect48 = 0xF00C3,
    VideoBackgroundEffectHorizontal16 = 0xF00C4,
    VideoBackgroundEffectHorizontal20 = 0xF00C5,
    VideoBackgroundEffectHorizontal24 = 0xF00C6,
    VideoBackgroundEffectHorizontal28 = 0xF00C7,
    VideoBackgroundEffectHorizontal32 = 0xF00C8,
    VideoBackgroundEffectHorizontal48 = 0xF00C9,
    VideoClip28 = 0xF00CA,
    VideoClip32 = 0xF00CB,
    VideoClip48 = 0xF00CC,
    Voicemail48 = 0xF00CD,
    ArrowCircleUpRight20 = 0xF00CE,
    ArrowCircleUpRight24 = 0xF00CF,
    Backspace16 = 0xF00D0,
    BinderTriangle20 = 0xF00D1,
    BinderTriangle24 = 0xF00D2,
    BinderTriangle32 = 0xF00D3,
    BowTie20 = 0xF00D4,
    BowTie24 = 0xF00D5,
    Circle28 = 0xF00D6,
    DocumentOnePageSparkle16 = 0xF00D7,
    DocumentOnePageSparkle20 = 0xF00D8,
    DocumentOnePageSparkle24 = 0xF00D9,
    EmojiHand32 = 0xF00DA,
    EmojiHand48 = 0xF00DB,
    Frame16 = 0xF00DC,
    Frame20 = 0xF00DD,
    Frame24 = 0xF00DE,
    LockClosedKey16 = 0xF00DF,
    LockClosedKey20 = 0xF00E0,
    LockClosedKey24 = 0xF00E1,
    MountainLocationBottom20 = 0xF00E2,
    MountainLocationBottom24 = 0xF00E3,
    MountainLocationBottom28 = 0xF00E4,
    MountainLocationTop20 = 0xF00E5,
    MountainLocationTop24 = 0xF00E6,
    MountainLocationTop28 = 0xF00E7,
    MountainTrail20 = 0xF00E8,
    MountainTrail24 = 0xF00E9,
    MountainTrail28 = 0xF00EA,
    PenDismiss16 = 0xF00EB,
    PenDismiss20 = 0xF00EC,
    PenDismiss24 = 0xF00ED,
    PenDismiss28 = 0xF00EE,
    PenDismiss32 = 0xF00EF,
    PenDismiss48 = 0xF00F0,
    PhoneEdit20 = 0xF00F1,
    PhoneEdit24 = 0xF00F2,
    SendBeaker16 = 0xF00F3,
    SendBeaker20 = 0xF00F4,
    SendBeaker24 = 0xF00F5,
    SendBeaker28 = 0xF00F6,
    SendBeaker32 = 0xF00F7,
    SendBeaker48 = 0xF00F8,
    SlideTextSparkle16 = 0xF00F9,
    SlideTextSparkle20 = 0xF00FA,
    SlideTextSparkle24 = 0xF00FB,
    SlideTextSparkle28 = 0xF00FC,
    SlideTextSparkle32 = 0xF00FD,
    SlideTextSparkle48 = 0xF00FE,
    StackVertical20 = 0xF00FF,
    StackVertical24 = 0xF0100,
    TableColumnTopBottom20 = 0xF0101,
    TableColumnTopBottom24 = 0xF0102,
    TableOffset20 = 0xF0103,
    TableOffset24 = 0xF0104,
    TableOffsetAdd20 = 0xF0105,
    TableOffsetAdd24 = 0xF0106,
    TableOffsetLessThanOrEqualTo20 = 0xF0107,
    TableOffsetLessThanOrEqualTo24 = 0xF0108,
    TableOffsetSettings20 = 0xF0109,
    TableOffsetSettings24 = 0xF010A,
    VehicleCableCar20 = 0xF010B,
    VehicleCableCar24 = 0xF010C,
    VehicleCableCar28 = 0xF010D,
    ArrowAutofitHeightIn20 = 0xF010E,
    ArrowAutofitHeightIn24 = 0xF010F,
    CircleHint16 = 0xF0110,
    CircleHint20 = 0xF0111,
    CloudDatabase20 = 0xF0112,
    CloudDesktop20 = 0xF0113,
    CodeCircle24 = 0xF0114,
    CodeCircle32 = 0xF0115,
    ColumnSingle16 = 0xF0116,
    DesktopArrowDown16 = 0xF0117,
    DesktopArrowDown20 = 0xF0118,
    DesktopArrowDown24 = 0xF0119,
    DesktopTower20 = 0xF011A,
    DesktopTower24 = 0xF011B,
    DocumentCheckmark16 = 0xF011C,
    DocumentKey20 = 0xF011D,
    Dust20 = 0xF011E,
    Dust24 = 0xF011F,
    Dust28 = 0xF0120,
    EditArrowBack24 = 0xF0121,
    EmojiHint16 = 0xF0122,
    EmojiHint20 = 0xF0123,
    EmojiHint24 = 0xF0124,
    EmojiHint28 = 0xF0125,
    EmojiHint32 = 0xF0126,
    EmojiHint48 = 0xF0127,
    FolderList16 = 0xF0128,
    FolderList20 = 0xF0129,
    LightbulbCheckmark20 = 0xF012A,
    LineHorizontal416 = 0xF012B,
    LineHorizontal4Search16 = 0xF012C,
    MathFormatProfessional16 = 0xF012D,
    Mold20 = 0xF012E,
    Mold24 = 0xF012F,
    Mold28 = 0xF0130,
    PeopleTeam48 = 0xF0131,
    PersonDesktop20 = 0xF0132,
    PersonRibbon16 = 0xF0133,
    PersonRibbon20 = 0xF0134,
    PersonWrench20 = 0xF0135,
    PlantGrass20 = 0xF0136,
    PlantGrass24 = 0xF0137,
    PlantGrass28 = 0xF0138,
    PlantRagweed20 = 0xF0139,
    PlantRagweed24 = 0xF013A,
    PlantRagweed28 = 0xF013B,
    SettingsCogMultiple20 = 0xF013C,
    SettingsCogMultiple24 = 0xF013D,
    SlideContent24 = 0xF013E,
    SlideRecord16 = 0xF013F,
    SlideRecord20 = 0xF0140,
    SlideRecord24 = 0xF0141,
    SlideRecord28 = 0xF0142,
    SlideRecord48 = 0xF0143,
    StackAdd20 = 0xF0144,
    StackAdd24 = 0xF0145,
    StarCheckmark16 = 0xF0146,
    StarCheckmark20 = 0xF0147,
    StarCheckmark24 = 0xF0148,
    StarCheckmark28 = 0xF0149,
    Stream32 = 0xF014A,
    SubtractSquare16 = 0xF014B,
    TableDefault32 = 0xF014C,
    TableSimple32 = 0xF014D,
    TableSimpleExclude16 = 0xF014E,
    TableSimpleExclude20 = 0xF014F,
    TableSimpleExclude24 = 0xF0150,
    TableSimpleExclude28 = 0xF0151,
    TableSimpleExclude32 = 0xF0152,
    TableSimpleExclude48 = 0xF0153,
    TableSimpleInclude16 = 0xF0154,
    TableSimpleInclude20 = 0xF0155,
    TableSimpleInclude24 = 0xF0156,
    TableSimpleInclude28 = 0xF0157,
    TableSimpleInclude32 = 0xF0158,
    TableSimpleInclude48 = 0xF0159,
    TabletLaptop20 = 0xF015A,
    TextboxAlignMiddle16 = 0xF015B,
    TreeDeciduous24 = 0xF015C,
    TreeDeciduous28 = 0xF015D,
    AppGeneric48 = 0xF015E,
    ArrowEnter16 = 0xF015F,
    ArrowSprint16 = 0xF0160,
    ArrowSprint20 = 0xF0161,
    BeakerSettings16 = 0xF0162,
    BeakerSettings20 = 0xF0163,
    BinderTriangle16 = 0xF0164,
    BookDismiss16 = 0xF0165,
    BookDismiss20 = 0xF0166,
    Button16 = 0xF0167,
    Button20 = 0xF0168,
    CardUi20 = 0xF0169,
    CardUi24 = 0xF016A,
    ChevronDownUp16 = 0xF016B,
    ChevronDownUp20 = 0xF016C,
    ChevronDownUp24 = 0xF016D,
    ColumnSingleCompare16 = 0xF016E,
    ColumnSingleCompare20 = 0xF016F,
    CropSparkle24 = 0xF0170,
    Cursor16 = 0xF0171,
    CursorProhibited16 = 0xF0172,
    CursorProhibited20 = 0xF0173,
    DataHistogram16 = 0xF0174,
    DocumentImage16 = 0xF0175,
    DocumentImage20 = 0xF0176,
    DocumentJava16 = 0xF0177,
    DocumentJava20 = 0xF0178,
    DocumentOnePageBeaker16 = 0xF0179,
    DocumentOnePageMultiple16 = 0xF017A,
    DocumentOnePageMultiple20 = 0xF017B,
    DocumentOnePageMultiple24 = 0xF017C,
    DocumentSass16 = 0xF017D,
    DocumentSass20 = 0xF017E,
    DocumentYml16 = 0xF017F,
    DocumentYml20 = 0xF0180,
    FilmstripSplit16 = 0xF0181,
    FilmstripSplit20 = 0xF0182,
    FilmstripSplit24 = 0xF0183,
    FilmstripSplit32 = 0xF0184,
    Gavel16 = 0xF0185,
    GavelProhibited16 = 0xF0186,
    GavelProhibited20 = 0xF0187,
    GiftOpen16 = 0xF0188,
    GiftOpen20 = 0xF0189,
    GiftOpen24 = 0xF018A,
    Globe12 = 0xF018B,
    GridKanban16 = 0xF018C,
    ImageStack16 = 0xF018D,
    ImageStack20 = 0xF018E,
    LaptopShield16 = 0xF018F,
    LaptopShield20 = 0xF0190,
    ListBar16 = 0xF0191,
    ListBar20 = 0xF0192,
    ListBarTree16 = 0xF0193,
    ListBarTree20 = 0xF0194,
    ListBarTreeOffset16 = 0xF0195,
    ListBarTreeOffset20 = 0xF0196,
    ListRtl16 = 0xF0197,
    ListRtl20 = 0xF0198,
    PanelLeftText16 = 0xF0199,
    PanelLeftText20 = 0xF019A,
    PanelLeftText24 = 0xF019B,
    PanelLeftText28 = 0xF019C,
    PanelLeftText32 = 0xF019D,
    PanelLeftText48 = 0xF019E,
    PanelLeftTextAdd16 = 0xF019F,
    PanelLeftTextAdd20 = 0xF01A0,
    PanelLeftTextAdd24 = 0xF01A1,
    PanelLeftTextAdd28 = 0xF01A2,
    PanelLeftTextAdd32 = 0xF01A3,
    PanelLeftTextAdd48 = 0xF01A4,
    PanelLeftTextDismiss16 = 0xF01A5,
    PanelLeftTextDismiss20 = 0xF01A6,
    PanelLeftTextDismiss24 = 0xF01A7,
    PanelLeftTextDismiss28 = 0xF01A8,
    PanelLeftTextDismiss32 = 0xF01A9,
    PanelLeftTextDismiss48 = 0xF01AA,
    PersonLightning16 = 0xF01AB,
    PersonLightning20 = 0xF01AC,
    TextBulletListSquare16 = 0xF01AD,
    TextBulletListSquare32 = 0xF01AE,
    TextBulletListSquareSparkle16 = 0xF01AF,
    TextBulletListSquareSparkle20 = 0xF01B0,
    TextBulletListSquareSparkle24 = 0xF01B1,
    TranslateAuto16 = 0xF01B2,
    TranslateAuto20 = 0xF01B3,
    TranslateAuto24 = 0xF01B4,
    AlignSpaceEvenlyVertical24 = 0xF01B5,
    AlignStraighten20 = 0xF01B6,
    AlignStraighten24 = 0xF01B7,
    ArrowFlowDiagonalUpRight16 = 0xF01B8,
    ArrowFlowDiagonalUpRight20 = 0xF01B9,
    ArrowFlowDiagonalUpRight24 = 0xF01BA,
    ArrowFlowDiagonalUpRight32 = 0xF01BB,
    ArrowFlowUpRight16 = 0xF01BC,
    ArrowFlowUpRight20 = 0xF01BD,
    ArrowFlowUpRight24 = 0xF01BE,
    ArrowFlowUpRight32 = 0xF01BF,
    ArrowFlowUpRightRectangleMultiple20 = 0xF01C0,
    ArrowFlowUpRightRectangleMultiple24 = 0xF01C1,
    ArrowSquareUpRight20 = 0xF01C2,
    ArrowSquareUpRight24 = 0xF01C3,
    BinRecycle20 = 0xF01C4,
    BinRecycle24 = 0xF01C5,
    BinRecycleFull20 = 0xF01C6,
    BinRecycleFull24 = 0xF01C7,
    BriefcaseSearch20 = 0xF01C8,
    BriefcaseSearch24 = 0xF01C9,
    CircleLine16 = 0xF01CA,
    Desk20 = 0xF01CB,
    Desk24 = 0xF01CC,
    Filmstrip48 = 0xF01CD,
    FilmstripOff48 = 0xF01CE,
    Flash32 = 0xF01CF,
    Flow24 = 0xF01D0,
    Flow32 = 0xF01D1,
    HeartPulseCheckmark20 = 0xF01D2,
    HeartPulseError20 = 0xF01D3,
    HeartPulseWarning20 = 0xF01D4,
    HomeHeart16 = 0xF01D5,
    HomeHeart20 = 0xF01D6,
    HomeHeart24 = 0xF01D7,
    HomeHeart32 = 0xF01D8,
    ImageOff28 = 0xF01D9,
    ImageOff32 = 0xF01DA,
    ImageOff48 = 0xF01DB,
    MoneyHand16 = 0xF01DC,
    MoneySettings16 = 0xF01DD,
    MoneySettings24 = 0xF01DE,
    PeopleEdit16 = 0xF01DF,
    PeopleEdit24 = 0xF01E0,
    TriangleUp20 = 0xF01E1,
    AddSquare16 = 0xF01E2,
    AddSquare28 = 0xF01E3,
    AddSquare32 = 0xF01E4,
    AddSquare48 = 0xF01E5,
    ArrowRouting20 = 0xF01E6,
    ArrowRouting24 = 0xF01E7,
    ArrowRoutingRectangleMultiple20 = 0xF01E8,
    ArrowRoutingRectangleMultiple24 = 0xF01E9,
    BookAdd28 = 0xF01EA,
    BookDefault28 = 0xF01EB,
    FolderLightning16 = 0xF01EC,
    FolderLightning20 = 0xF01ED,
    FolderLightning24 = 0xF01EE,
    HatGraduation28 = 0xF01EF,
    ImageSparkle16 = 0xF01F0,
    ImageSparkle20 = 0xF01F1,
    ImageSparkle24 = 0xF01F2,
    Mail32 = 0xF01F3,
    PersonInfo24 = 0xF01F4,
    Prohibited32 = 0xF01F5,
    ProhibitedMultiple28 = 0xF01F6,
    SpinnerIos16 = 0xF01F7,
    StarEmphasis16 = 0xF01F8,
    TextDirectionRotate315Right20 = 0xF01F9,
    TextDirectionRotate315Right24 = 0xF01FA,
    TextDirectionRotate45Right20 = 0xF01FB,
    TextDirectionRotate45Right24 = 0xF01FC,
    ArrowOutlineDownLeft16 = 0xF01FD,
    ArrowOutlineDownLeft20 = 0xF01FE,
    ArrowOutlineDownLeft24 = 0xF01FF,
    ArrowOutlineDownLeft28 = 0xF0200,
    ArrowOutlineDownLeft32 = 0xF0201,
    ArrowOutlineDownLeft48 = 0xF0202,
    ArrowStepInDiagonalDownLeft16 = 0xF0203,
    ArrowStepInDiagonalDownLeft20 = 0xF0204,
    ArrowStepInDiagonalDownLeft24 = 0xF0205,
    ArrowStepInDiagonalDownLeft28 = 0xF0206,
    ArrowUpSquareSettings24 = 0xF0207,
    BriefcasePerson24 = 0xF0208,
    BuildingCloud24 = 0xF0209,
    CalendarEye20 = 0xF020A,
    ClipboardPaste32 = 0xF020B,
    CloudBidirectional20 = 0xF020C,
    CloudBidirectional24 = 0xF020D,
    CommentEdit16 = 0xF020E,
    Crown24 = 0xF020F,
    CrownSubtract24 = 0xF0210,
    FolderAdd32 = 0xF0211,
    FolderArrowLeft48 = 0xF0212,
    FolderArrowRight32 = 0xF0213,
    FolderArrowUp32 = 0xF0214,
    FolderLink16 = 0xF0215,
    FolderLink32 = 0xF0216,
    FolderProhibited32 = 0xF0217,
    HatGraduationSparkle20 = 0xF0218,
    HatGraduationSparkle24 = 0xF0219,
    HatGraduationSparkle28 = 0xF021A,
    Kiosk24 = 0xF021B,
    LaptopMultiple24 = 0xF021C,
    LinkAdd24 = 0xF021D,
    LinkSettings24 = 0xF021E,
    LockClosed28 = 0xF021F,
    LockClosed48 = 0xF0220,
    LockOpen12 = 0xF0221,
    LockOpen32 = 0xF0222,
    LockOpen48 = 0xF0223,
    PaintBrush32 = 0xF0224,
    PauseCircle32 = 0xF0225,
    PauseCircle48 = 0xF0226,
    PenSparkle16 = 0xF0227,
    PenSparkle20 = 0xF0228,
    PenSparkle24 = 0xF0229,
    PenSparkle28 = 0xF022A,
    PenSparkle32 = 0xF022B,
    PenSparkle48 = 0xF022C,
    PersonPhone24 = 0xF022D,
    PersonSubtract24 = 0xF022E,
    PhoneBriefcase24 = 0xF022F,
    PhoneMultiple24 = 0xF0230,
    PhoneMultipleSettings24 = 0xF0231,
    PhonePerson24 = 0xF0232,
    PhoneSubtract24 = 0xF0233,
    PlugConnectedSettings20 = 0xF0234,
    PlugConnectedSettings24 = 0xF0235,
    RectangleLandscapeHintCopy16 = 0xF0236,
    RectangleLandscapeHintCopy20 = 0xF0237,
    RectangleLandscapeHintCopy24 = 0xF0238,
    Script20 = 0xF0239,
    Script24 = 0xF023A,
    Script32 = 0xF023B,
    ServerLink24 = 0xF023C,
    Signature32 = 0xF023D,
    SpeakerMute32 = 0xF023E,
    TabDesktop28 = 0xF023F,
    TabDesktopLink16 = 0xF0240,
    TabDesktopLink20 = 0xF0241,
    TabDesktopLink24 = 0xF0242,
    TabDesktopLink28 = 0xF0243,
    TableArrowUp20 = 0xF0244,
    TableArrowUp24 = 0xF0245,
    TabletLaptop24 = 0xF0246,
    ThumbLikeDislike16 = 0xF0247,
    ThumbLikeDislike20 = 0xF0248,
    ThumbLikeDislike24 = 0xF0249,
    Warning32 = 0xF024A,
    NumberCircle128 = 0xF024B,
    NumberCircle132 = 0xF024C,
    NumberCircle148 = 0xF024D,
    NumberCircle216 = 0xF024E,
    NumberCircle220 = 0xF024F,
    NumberCircle224 = 0xF0250,
    NumberCircle228 = 0xF0251,
    NumberCircle232 = 0xF0252,
    NumberCircle248 = 0xF0253,
    NumberCircle316 = 0xF0254,
    NumberCircle320 = 0xF0255,
    NumberCircle324 = 0xF0256,
    NumberCircle328 = 0xF0257,
    NumberCircle332 = 0xF0258,
    NumberCircle348 = 0xF0259,
    NumberCircle416 = 0xF025A,
    NumberCircle420 = 0xF025B,
    NumberCircle424 = 0xF025C,
    NumberCircle428 = 0xF025D,
    NumberCircle432 = 0xF025E,
    NumberCircle448 = 0xF025F,
    NumberCircle516 = 0xF0260,
    NumberCircle520 = 0xF0261,
    NumberCircle524 = 0xF0262,
    NumberCircle528 = 0xF0263,
    NumberCircle532 = 0xF0264,
    NumberCircle548 = 0xF0265,
    AddSquareMultiple24 = 0xF0266,
    BracesVariable48 = 0xF0267,
    Cube48 = 0xF0268,
    Desk16 = 0xF0269,
    Desk28 = 0xF026A,
    Desk32 = 0xF026B,
    Desk48 = 0xF026C,
    FolderOpenVertical24 = 0xF026D,
    Globe48 = 0xF026E,
    GlobeShield48 = 0xF026F,
    HandRightOff16 = 0xF0270,
    HandRightOff24 = 0xF0271,
    HandRightOff28 = 0xF0272,
    HatGraduationSparkle16 = 0xF0273,
    KeyMultiple16 = 0xF0274,
    KeyMultiple24 = 0xF0275,
    LinkMultiple16 = 0xF0276,
    LinkMultiple20 = 0xF0277,
    LinkMultiple24 = 0xF0278,
    MailOff16 = 0xF0279,
    PersonEdit48 = 0xF027A,
    PlugDisconnected48 = 0xF027B,
    Stream48 = 0xF027C,
    TextBulletListSquare48 = 0xF027D,
    TextBulletListSquareShield48 = 0xF027E,
    ArrowExport16 = 0xF027F,
    ArrowExport20 = 0xF0280,
    ArrowExport24 = 0xF0281,
    Calendar12 = 0xF0282,
    Calendar16 = 0xF0283,
    Calendar20 = 0xF0284,
    Calendar24 = 0xF0285,
    Calendar28 = 0xF0286,
    Calendar32 = 0xF0287,
    Calendar48 = 0xF0288,
    CalendarDate20 = 0xF0289,
    CalendarDate24 = 0xF028A,
    CalendarDate28 = 0xF028B,
    ClipboardBulletList16 = 0xF028C,
    ClipboardBulletList20 = 0xF028D,
    IosArrow24 = 0xF028E,
    TextBulletList16 = 0xF028F,
    TextBulletList20 = 0xF0290,
    TextBulletList24 = 0xF0291,
    TextBulletList27024 = 0xF0292,
    TextBulletList9020 = 0xF0293,
    TextBulletList9024 = 0xF0294,
    TextColumnWide20 = 0xF0295,
    TextColumnWide24 = 0xF0296,
    TextIndentDecrease16 = 0xF0297,
    TextIndentDecrease20 = 0xF0298,
    TextIndentDecrease24 = 0xF0299,
    TextIndentIncrease16 = 0xF029A,
    TextIndentIncrease20 = 0xF029B,
    TextIndentIncrease24 = 0xF029C,
    VehicleCarProfile16 = 0xF029D,
    VehicleCarProfile20 = 0xF029E,
    VehicleCarProfile24 = 0xF029F,
    ArrowBidirectionalLeftRight16 = 0xF02A0,
    ArrowBidirectionalLeftRight20 = 0xF02A1,
    ArrowBidirectionalLeftRight24 = 0xF02A2,
    ArrowBidirectionalLeftRight28 = 0xF02A3,
    ArrowSwap16 = 0xF02A4,
    ArrowSwap28 = 0xF02A5,
    BuildingMosque12 = 0xF02A6,
    BuildingMosque16 = 0xF02A7,
    BuildingMosque20 = 0xF02A8,
    BuildingMosque24 = 0xF02A9,
    BuildingMosque28 = 0xF02AA,
    BuildingMosque32 = 0xF02AB,
    BuildingMosque48 = 0xF02AC,
    CheckmarkCircleSquare16 = 0xF02AD,
    CheckmarkCircleSquare20 = 0xF02AE,
    CheckmarkCircleSquare24 = 0xF02AF,
    HeartOff16 = 0xF02B0,
    HeartOff20 = 0xF02B1,
    HeartOff24 = 0xF02B2,
    Hexagon16 = 0xF02B3,
    Hexagon20 = 0xF02B4,
    HexagonThree16 = 0xF02B5,
    HexagonThree20 = 0xF02B6,
    LineHorizontal116 = 0xF02B7,
    LineHorizontal124 = 0xF02B8,
    LineHorizontal128 = 0xF02B9,
    LineHorizontal1Dashes16 = 0xF02BA,
    LineHorizontal1Dashes20 = 0xF02BB,
    LineHorizontal1Dashes24 = 0xF02BC,
    LineHorizontal1Dashes28 = 0xF02BD,
    LineHorizontal2DashesSolid16 = 0xF02BE,
    LineHorizontal2DashesSolid20 = 0xF02BF,
    LineHorizontal2DashesSolid24 = 0xF02C0,
    LineHorizontal2DashesSolid28 = 0xF02C1,
    MicRecord20 = 0xF02C2,
    MicRecord24 = 0xF02C3,
    MicRecord28 = 0xF02C4,
    Open12 = 0xF02C5,
    RemixAdd16 = 0xF02C6,
    RemixAdd20 = 0xF02C7,
    RemixAdd24 = 0xF02C8,
    RemixAdd32 = 0xF02C9,
    VideoPersonSparkleOff20 = 0xF02CA,
    VideoPersonSparkleOff24 = 0xF02CB,
    VoicemailShield20 = 0xF02CC,
    VoicemailShield24 = 0xF02CD,
    VoicemailShield32 = 0xF02CE,
    WindowDatabase32 = 0xF02CF,
    CastMultiple20 = 0xF02D0,
    CastMultiple24 = 0xF02D1,
    CastMultiple28 = 0xF02D2,
    CircleHintHalfVertical16 = 0xF02D3,
    CircleHintHalfVertical20 = 0xF02D4,
    CircleHintHalfVertical24 = 0xF02D5,
    FlashSparkle20 = 0xF02D6,
    FlashSparkle24 = 0xF02D7,
    Hexagon12 = 0xF02D8,
    Hexagon24 = 0xF02D9,
    HexagonThree12 = 0xF02DA,
    HexagonThree24 = 0xF02DB,
    NextFrame20 = 0xF02DC,
    NextFrame24 = 0xF02DD,
    PreviousFrame20 = 0xF02DE,
    PreviousFrame24 = 0xF02DF,
    TextboxAlignBottomCenter16 = 0xF02E0,
    TextboxAlignBottomCenter20 = 0xF02E1,
    TextboxAlignBottomCenter24 = 0xF02E2,
    TextboxAlignBottomLeft16 = 0xF02E3,
    TextboxAlignBottomLeft20 = 0xF02E4,
    TextboxAlignBottomLeft24 = 0xF02E5,
    TextboxAlignBottomRight16 = 0xF02E6,
    TextboxAlignBottomRight20 = 0xF02E7,
    TextboxAlignBottomRight24 = 0xF02E8,
    TextboxAlignCenter16 = 0xF02E9,
    TextboxAlignMiddleLeft16 = 0xF02EA,
    TextboxAlignMiddleLeft20 = 0xF02EB,
    TextboxAlignMiddleLeft24 = 0xF02EC,
    TextboxAlignMiddleRight16 = 0xF02ED,
    TextboxAlignMiddleRight20 = 0xF02EE,
    TextboxAlignMiddleRight24 = 0xF02EF,
    TextboxAlignTopCenter16 = 0xF02F0,
    TextboxAlignTopCenter20 = 0xF02F1,
    TextboxAlignTopCenter24 = 0xF02F2,
    TextboxAlignTopLeft16 = 0xF02F3,
    TextboxAlignTopLeft20 = 0xF02F4,
    TextboxAlignTopLeft24 = 0xF02F5,
    TextboxAlignTopRight16 = 0xF02F6,
    TextboxAlignTopRight20 = 0xF02F7,
    TextboxAlignTopRight24 = 0xF02F8,
    TriangleDown24 = 0xF02F9,
    CallEnd12 = 0xF02FA,
    CallEnd32 = 0xF02FB,
    CallEnd48 = 0xF02FC,
    ContentViewGallery16 = 0xF02FD,
    ContentViewGalleryLightning16 = 0xF02FE,
    ContentViewGalleryLightning20 = 0xF02FF,
    ContentViewGalleryLightning24 = 0xF0300,
    ContentViewGalleryLightning28 = 0xF0301,
    GlobeArrowForward16 = 0xF0302,
    GlobeArrowForward20 = 0xF0303,
    GlobeArrowForward24 = 0xF0304,
    GlobeArrowForward32 = 0xF0305,
    HardDrive24 = 0xF0306,
    HardDrive32 = 0xF0307,
    HardDriveCall24 = 0xF0308,
    HardDriveCall32 = 0xF0309,
    MailRewind16 = 0xF030A,
    MailRewind20 = 0xF030B,
    MailRewind24 = 0xF030C,
    PanelRightGallery16 = 0xF030D,
    PanelRightGallery20 = 0xF030E,
    PanelRightGallery24 = 0xF030F,
    PanelRightGallery28 = 0xF0310,
    PanelTopGallery16 = 0xF0311,
    PanelTopGallery20 = 0xF0312,
    PanelTopGallery24 = 0xF0313,
    PanelTopGallery28 = 0xF0314,
    RectangleLandscapeSparkle16 = 0xF0315,
    RectangleLandscapeSparkle20 = 0xF0316,
    RectangleLandscapeSparkle24 = 0xF0317,
    RectangleLandscapeSparkle28 = 0xF0318,
    RectangleLandscapeSparkle32 = 0xF0319,
    ScanPerson16 = 0xF031A,
    ScanPerson20 = 0xF031B,
    ScanPerson24 = 0xF031C,
    ScanPerson28 = 0xF031D,
    ScanPerson48 = 0xF031E,
    VoicemailShield16 = 0xF031F,
    ChevronDown32 = 0xF0320,
    ChevronLeft32 = 0xF0321,
    ChevronRight32 = 0xF0322,
    ChevronUp32 = 0xF0323,
    DocumentLightning16 = 0xF0324,
    DocumentLightning20 = 0xF0325,
    DocumentLightning24 = 0xF0326,
    DocumentLightning28 = 0xF0327,
    DocumentLightning32 = 0xF0328,
    DocumentLightning48 = 0xF0329,
    Edit12 = 0xF032A,
    ServerLink16 = 0xF032B,
    ServerLink20 = 0xF032C,
    Step20 = 0xF032D,
    Step24 = 0xF032E,
    TabDesktopMultipleAdd20 = 0xF032F,
    TextDescription16 = 0xF0330,
    TextDescription28 = 0xF0331,
    TextDescription32 = 0xF0332,
    TextGrammarLightning16 = 0xF0333,
    TextGrammarLightning20 = 0xF0334,
    TextGrammarLightning24 = 0xF0335,
    TextGrammarLightning28 = 0xF0336,
    TextGrammarLightning32 = 0xF0337,
    BeakerAdd20 = 0xF0338,
    BeakerAdd24 = 0xF0339,
    BeakerDismiss20 = 0xF033A,
    BeakerDismiss24 = 0xF033B,
    DocumentCube20 = 0xF033C,
    DocumentCube24 = 0xF033D,
    Drawer20 = 0xF033E,
    Drawer24 = 0xF033F,
    FilmstripImage20 = 0xF0340,
    FilmstripImage24 = 0xF0341,
    NumberCircle016 = 0xF0342,
    NumberCircle020 = 0xF0343,
    NumberCircle024 = 0xF0344,
    NumberCircle028 = 0xF0345,
    NumberCircle032 = 0xF0346,
    NumberCircle048 = 0xF0347,
    NumberCircle616 = 0xF0348,
    NumberCircle620 = 0xF0349,
    NumberCircle624 = 0xF034A,
    NumberCircle628 = 0xF034B,
    NumberCircle632 = 0xF034C,
    NumberCircle648 = 0xF034D,
    NumberCircle716 = 0xF034E,
    NumberCircle720 = 0xF034F,
    NumberCircle724 = 0xF0350,
    NumberCircle728 = 0xF0351,
    NumberCircle732 = 0xF0352,
    NumberCircle748 = 0xF0353,
    NumberCircle816 = 0xF0354,
    NumberCircle820 = 0xF0355,
    NumberCircle824 = 0xF0356,
    NumberCircle828 = 0xF0357,
    NumberCircle832 = 0xF0358,
    NumberCircle848 = 0xF0359,
    NumberCircle916 = 0xF035A,
    NumberCircle920 = 0xF035B,
    NumberCircle924 = 0xF035C,
    NumberCircle928 = 0xF035D,
    NumberCircle932 = 0xF035E,
    NumberCircle948 = 0xF035F,
    Server12 = 0xF0360,
    SquareHintHexagon12 = 0xF0361,
    SquareHintHexagon16 = 0xF0362,
    SquareHintHexagon20 = 0xF0363,
    SquareHintHexagon24 = 0xF0364,
    SquareHintHexagon28 = 0xF0365,
    SquareHintHexagon32 = 0xF0366,
    SquareHintHexagon48 = 0xF0367,
    TabDesktopMultiple16 = 0xF0368,
    TabDesktopMultipleAdd16 = 0xF0369,
    TargetAdd20 = 0xF036A,
    TargetAdd24 = 0xF036B,
    TargetDismiss20 = 0xF036C,
    TargetDismiss24 = 0xF036D,
    TextHeader1Lines16 = 0xF036E,
    TextHeader1Lines20 = 0xF036F,
    TextHeader1Lines24 = 0xF0370,
    TextHeader1LinesCaret16 = 0xF0371,
    TextHeader1LinesCaret20 = 0xF0372,
    TextHeader1LinesCaret24 = 0xF0373,
    TextHeader2Lines16 = 0xF0374,
    TextHeader2Lines20 = 0xF0375,
    TextHeader2Lines24 = 0xF0376,
    TextHeader2LinesCaret16 = 0xF0377,
    TextHeader2LinesCaret20 = 0xF0378,
    TextHeader2LinesCaret24 = 0xF0379,
    TextHeader3Lines16 = 0xF037A,
    TextHeader3Lines20 = 0xF037B,
    TextHeader3Lines24 = 0xF037C,
    TextHeader3LinesCaret16 = 0xF037D,
    TextHeader3LinesCaret20 = 0xF037E,
    TextHeader3LinesCaret24 = 0xF037F,
    ArrowDownload28 = 0xF0380,
    ArrowDownload32 = 0xF0381,
    ArrowExpand16 = 0xF0382,
    ArrowExportUp16 = 0xF0383,
    ArrowImport16 = 0xF0384,
    ArrowUpRightDashes16 = 0xF0385,
    Battery1016 = 0xF0386,
    BeakerEmpty16 = 0xF0387,
    Book16 = 0xF0388,
    BorderNone16 = 0xF0389,
    BranchRequest16 = 0xF038A,
    ClipboardTaskList16 = 0xF038B,
    Cut16 = 0xF038C,
    FolderSearch16 = 0xF038D,
    FolderSearch20 = 0xF038E,
    FolderSearch24 = 0xF038F,
    Hexagon28 = 0xF0390,
    Hexagon32 = 0xF0391,
    Hexagon48 = 0xF0392,
    PlugConnected16 = 0xF0393,
    PlugDisconnected16 = 0xF0394,
    ProjectionScreenText20 = 0xF0395,
    Rss16 = 0xF0396,
    ShapeOrganic16 = 0xF0397,
    ShapeOrganic20 = 0xF0398,
    ShapeOrganic24 = 0xF0399,
    ShapeOrganic28 = 0xF039A,
    ShapeOrganic32 = 0xF039B,
    ShapeOrganic48 = 0xF039C,
    TeardropBottomRight16 = 0xF039D,
    TeardropBottomRight20 = 0xF039E,
    TeardropBottomRight24 = 0xF039F,
    TeardropBottomRight28 = 0xF03A0,
    TeardropBottomRight32 = 0xF03A1,
    TeardropBottomRight48 = 0xF03A2,
    TextEditStyle16 = 0xF03A3,
    TextWholeWord16 = 0xF03A4,
    Triangle24 = 0xF03A5,
    Triangle28 = 0xF03A6,
    TextAsterisk16 = 0xF03A7,
    ArrowDownloadOff16 = 0xF03A8,
    ArrowDownloadOff20 = 0xF03A9,
    ArrowDownloadOff24 = 0xF03AA,
    ArrowDownloadOff28 = 0xF03AB,
    ArrowDownloadOff32 = 0xF03AC,
    ArrowDownloadOff48 = 0xF03AD,
    BorderInside16 = 0xF03AE,
    BorderInside20 = 0xF03AF,
    BorderInside24 = 0xF03B0,
    ChatLock16 = 0xF03B1,
    ChatLock20 = 0xF03B2,
    ChatLock24 = 0xF03B3,
    ChatLock28 = 0xF03B4,
    ErrorCircle48 = 0xF03B5,
    FullScreenMaximize28 = 0xF03B6,
    FullScreenMaximize32 = 0xF03B7,
    FullScreenMinimize28 = 0xF03B8,
    FullScreenMinimize32 = 0xF03B9,
    LinkPerson16 = 0xF03BA,
    LinkPerson20 = 0xF03BB,
    LinkPerson24 = 0xF03BC,
    LinkPerson32 = 0xF03BD,
    LinkPerson48 = 0xF03BE,
    PeopleChat16 = 0xF03BF,
    PeopleChat20 = 0xF03C0,
    PeopleChat24 = 0xF03C1,
    PersonSupport28 = 0xF03C2,
    Shapes32 = 0xF03C3,
    SlideTextEdit16 = 0xF03C4,
    SlideTextEdit20 = 0xF03C5,
    SlideTextEdit24 = 0xF03C6,
    SlideTextEdit28 = 0xF03C7,
    SubtractCircle48 = 0xF03C8,
    SubtractParentheses16 = 0xF03C9,
    SubtractParentheses20 = 0xF03CA,
    SubtractParentheses24 = 0xF03CB,
    SubtractParentheses28 = 0xF03CC,
    SubtractParentheses32 = 0xF03CD,
    SubtractParentheses48 = 0xF03CE,
    Warning48 = 0xF03CF,
    AlertOn16 = 0xF03D0,
    ArrowDownExclamation16 = 0xF03D1,
    ArrowDownExclamation20 = 0xF03D2,
    ArrowFit24 = 0xF03D3,
    ArrowFitIn24 = 0xF03D4,
    Book32 = 0xF03D5,
    BookDatabase16 = 0xF03D6,
    BookDatabase32 = 0xF03D7,
    BookToolbox16 = 0xF03D8,
    BuildingDesktop32 = 0xF03D9,
    BuildingGovernment16 = 0xF03DA,
    BuildingGovernmentSearch16 = 0xF03DB,
    BuildingGovernmentSearch20 = 0xF03DC,
    BuildingGovernmentSearch24 = 0xF03DD,
    BuildingGovernmentSearch32 = 0xF03DE,
    CalendarRecord16 = 0xF03DF,
    CalendarRecord20 = 0xF03E0,
    CalendarRecord24 = 0xF03E1,
    CalendarRecord28 = 0xF03E2,
    CalendarRecord32 = 0xF03E3,
    CalendarRecord48 = 0xF03E4,
    Clipboard28 = 0xF03E5,
    ClipboardMathFormula16 = 0xF03E6,
    ClipboardMathFormula20 = 0xF03E7,
    ClipboardMathFormula24 = 0xF03E8,
    ClipboardMathFormula28 = 0xF03E9,
    ClipboardMathFormula32 = 0xF03EA,
    ClipboardNumber12316 = 0xF03EB,
    ClipboardNumber12320 = 0xF03EC,
    ClipboardNumber12324 = 0xF03ED,
    ClipboardNumber12328 = 0xF03EE,
    ClipboardNumber12332 = 0xF03EF,
    Collections16 = 0xF03F0,
    CommunicationShield16 = 0xF03F1,
    CommunicationShield20 = 0xF03F2,
    CommunicationShield24 = 0xF03F3,
    DialpadQuestionMark20 = 0xF03F4,
    DialpadQuestionMark24 = 0xF03F5,
    DocumentBriefcase16 = 0xF03F6,
    DocumentBriefcase32 = 0xF03F7,
    DocumentSearch32 = 0xF03F8,
    Fingerprint16 = 0xF03F9,
    Fingerprint32 = 0xF03FA,
    FolderPerson24 = 0xF03FB,
    FolderPerson28 = 0xF03FC,
    FolderPerson32 = 0xF03FD,
    FolderPerson48 = 0xF03FE,
    HatGraduationAdd16 = 0xF03FF,
    HatGraduationAdd20 = 0xF0400,
    HatGraduationAdd24 = 0xF0401,
    LayerDiagonalAdd20 = 0xF0402,
    Library32 = 0xF0403,
    LightbulbFilament32 = 0xF0404,
    LinkAdd16 = 0xF0405,
    LinkAdd20 = 0xF0406,
    LockShield16 = 0xF0407,
    LockShield28 = 0xF0408,
    LockShield32 = 0xF0409,
    PersonVoice16 = 0xF040A,
    PersonWarning16 = 0xF040B,
    PersonWarning20 = 0xF040C,
    PersonWarning24 = 0xF040D,
    PersonWarning28 = 0xF040E,
    PersonWarning32 = 0xF040F,
    PersonWarning48 = 0xF0410,
    ScanTypeOff24 = 0xF0411,
    Screenshot16 = 0xF0412,
    ScreenshotRecord16 = 0xF0413,
    ScreenshotRecord20 = 0xF0414,
    ScreenshotRecord24 = 0xF0415,
    SlideSearch16 = 0xF0416,
    SlideSearch32 = 0xF0417,
    VehicleSubwayClock16 = 0xF0418,
    VehicleSubwayClock20 = 0xF0419,
    VehicleSubwayClock24 = 0xF041A,
    VideoClipOptimize16 = 0xF041B,
    VideoClipOptimize20 = 0xF041C,
    VideoClipOptimize24 = 0xF041D,
    VideoClipOptimize28 = 0xF041E,
    VideoPersonPulse16 = 0xF041F,
    VideoPersonPulse20 = 0xF0420,
    VideoPersonPulse24 = 0xF0421,
    VideoPersonPulse28 = 0xF0422,
    ArchiveSettings32 = 0xF0423,
    ArrowForward32 = 0xF0424,
    ArrowReply32 = 0xF0425,
    ArrowReplyAll32 = 0xF0426,
    Attach32 = 0xF0427,
    Autocorrect32 = 0xF0428,
    Broom32 = 0xF0429,
    CalendarNote16 = 0xF042A,
    CalendarNote20 = 0xF042B,
    CalendarNote24 = 0xF042C,
    CalendarNote32 = 0xF042D,
    CheckmarkUnderlineCircle24 = 0xF042E,
    DataBarVerticalAscending20 = 0xF042F,
    DataBarVerticalAscending24 = 0xF0430,
    Diversity16 = 0xF0431,
    Filter32 = 0xF0432,
    FolderMail32 = 0xF0433,
    GlanceHorizontal32 = 0xF0434,
    GlanceHorizontalSparkle32 = 0xF0435,
    GlobeArrowUp16 = 0xF0436,
    GlobeArrowUp20 = 0xF0437,
    GlobeArrowUp24 = 0xF0438,
    GlobeError16 = 0xF0439,
    GlobeError20 = 0xF043A,
    GlobeError24 = 0xF043B,
    GlobeProhibited16 = 0xF043C,
    GlobeProhibited24 = 0xF043D,
    GlobeSync16 = 0xF043E,
    GlobeSync20 = 0xF043F,
    GlobeSync24 = 0xF0440,
    GlobeWarning16 = 0xF0441,
    GlobeWarning20 = 0xF0442,
    GlobeWarning24 = 0xF0443,
    Important32 = 0xF0444,
    LayerDiagonal16 = 0xF0445,
    LayerDiagonalPerson16 = 0xF0446,
    MailMultiple32 = 0xF0447,
    MailRead32 = 0xF0448,
    MailUnread32 = 0xF0449,
    Mailbox16 = 0xF044A,
    Mailbox20 = 0xF044B,
    OrganizationHorizontal16 = 0xF044C,
    OrganizationHorizontal24 = 0xF044D,
    PeopleList32 = 0xF044E,
    PersonAdd32 = 0xF044F,
    PersonSquare16 = 0xF0450,
    PersonSquare32 = 0xF0451,
    PersonSquareCheckmark16 = 0xF0452,
    PersonSquareCheckmark20 = 0xF0453,
    PersonSquareCheckmark24 = 0xF0454,
    PersonSquareCheckmark32 = 0xF0455,
    PhoneFooterArrowDown20 = 0xF0456,
    PhoneFooterArrowDown24 = 0xF0457,
    PhoneHeaderArrowUp20 = 0xF0458,
    PhoneHeaderArrowUp24 = 0xF0459,
    Poll32 = 0xF045A,
    Question32 = 0xF045B,
    Screenshot28 = 0xF045C,
    ScreenshotRecord28 = 0xF045D,
    Star32 = 0xF045E,
    TextDensity32 = 0xF045F,
    TextEditStyleCharacterA32 = 0xF0460,
    WrenchScrewdriver32 = 0xF0461,
    ArrowClockwiseDashes16 = 0xF0462,
    ArrowClockwiseDashes32 = 0xF0463,
    BuildingSwap16 = 0xF0464,
    BuildingSwap20 = 0xF0465,
    BuildingSwap24 = 0xF0466,
    BuildingSwap32 = 0xF0467,
    BuildingSwap48 = 0xF0468,
    Certificate32 = 0xF0469,
    ClipboardBrush16 = 0xF046A,
    ClipboardBrush20 = 0xF046B,
    ClipboardBrush24 = 0xF046C,
    ClipboardBrush28 = 0xF046D,
    ClipboardBrush32 = 0xF046E,
    CloudBeaker16 = 0xF046F,
    CloudBeaker20 = 0xF0470,
    CloudBeaker24 = 0xF0471,
    CloudBeaker28 = 0xF0472,
    CloudBeaker32 = 0xF0473,
    CloudBeaker48 = 0xF0474,
    CloudCube16 = 0xF0475,
    CloudCube20 = 0xF0476,
    CloudCube24 = 0xF0477,
    CloudCube28 = 0xF0478,
    CloudCube32 = 0xF0479,
    CloudCube48 = 0xF047A,
    ContractUpRight16 = 0xF047B,
    ContractUpRight20 = 0xF047C,
    ContractUpRight24 = 0xF047D,
    ContractUpRight28 = 0xF047E,
    ContractUpRight32 = 0xF047F,
    ContractUpRight48 = 0xF0480,
    DocumentDataLock16 = 0xF0481,
    DocumentDataLock20 = 0xF0482,
    DocumentDataLock24 = 0xF0483,
    DocumentDataLock32 = 0xF0484,
    GlanceHorizontalSparkles20 = 0xF0485,
    LayoutCellFour16 = 0xF0486,
    LayoutCellFour20 = 0xF0487,
    LayoutCellFour24 = 0xF0488,
    LayoutColumnFour16 = 0xF0489,
    LayoutColumnFour20 = 0xF048A,
    LayoutColumnFour24 = 0xF048B,
    LayoutColumnOneThirdLeft16 = 0xF048C,
    LayoutColumnOneThirdLeft20 = 0xF048D,
    LayoutColumnOneThirdLeft24 = 0xF048E,
    LayoutColumnOneThirdRight16 = 0xF048F,
    LayoutColumnOneThirdRight20 = 0xF0490,
    LayoutColumnOneThirdRight24 = 0xF0491,
    LayoutColumnOneThirdRightHint16 = 0xF0492,
    LayoutColumnOneThirdRightHint20 = 0xF0493,
    LayoutColumnOneThirdRightHint24 = 0xF0494,
    LayoutColumnThree16 = 0xF0495,
    LayoutColumnThree20 = 0xF0496,
    LayoutColumnThree24 = 0xF0497,
    LayoutColumnTwo16 = 0xF0498,
    LayoutColumnTwo20 = 0xF0499,
    LayoutColumnTwo24 = 0xF049A,
    LayoutColumnTwoSplitLeft16 = 0xF049B,
    LayoutColumnTwoSplitLeft20 = 0xF049C,
    LayoutColumnTwoSplitLeft24 = 0xF049D,
    LayoutColumnTwoSplitRight16 = 0xF049E,
    LayoutColumnTwoSplitRight20 = 0xF049F,
    LayoutColumnTwoSplitRight24 = 0xF04A0,
    LayoutRowFour16 = 0xF04A1,
    LayoutRowFour20 = 0xF04A2,
    LayoutRowFour24 = 0xF04A3,
    LayoutRowThree16 = 0xF04A4,
    LayoutRowThree20 = 0xF04A5,
    LayoutRowThree24 = 0xF04A6,
    LayoutRowTwo16 = 0xF04A7,
    LayoutRowTwo20 = 0xF04A8,
    LayoutRowTwo24 = 0xF04A9,
    LayoutRowTwoSplitBottom16 = 0xF04AA,
    LayoutRowTwoSplitBottom20 = 0xF04AB,
    LayoutRowTwoSplitBottom24 = 0xF04AC,
    LayoutRowTwoSplitTop16 = 0xF04AD,
    LayoutRowTwoSplitTop20 = 0xF04AE,
    LayoutRowTwoSplitTop24 = 0xF04AF,
    LocationTargetSquare16 = 0xF04B0,
    LocationTargetSquare20 = 0xF04B1,
    LocationTargetSquare24 = 0xF04B2,
    LocationTargetSquare32 = 0xF04B3,
    Resize16 = 0xF04B4,
    Resize28 = 0xF04B5,
    Resize32 = 0xF04B6,
    Resize48 = 0xF04B7,
    SelectAllOff16 = 0xF04B8,
    SelectAllOn16 = 0xF04B9,
    ShareAndroid16 = 0xF04BA,
    ShareAndroid32 = 0xF04BB,
    TextArrowDownRightColumn16 = 0xF04BC,
    TextArrowDownRightColumn20 = 0xF04BD,
    TextArrowDownRightColumn24 = 0xF04BE,
    TextArrowDownRightColumn28 = 0xF04BF,
    TextArrowDownRightColumn32 = 0xF04C0,
    TextArrowDownRightColumn48 = 0xF04C1,
    TextEffectsSparkle20 = 0xF04C2,
    TextEffectsSparkle24 = 0xF04C3,
    Whiteboard16 = 0xF04C4,
    WhiteboardOff16 = 0xF04C5,
    WhiteboardOff20 = 0xF04C6,
    WhiteboardOff24 = 0xF04C7,
    Flowchart16 = 0xF04C8,
    Flowchart32 = 0xF04C9,
    LayerDiagonal24 = 0xF04CA,
    LayerDiagonalPerson24 = 0xF04CB,
    PollOff16 = 0xF04CC,
    PollOff20 = 0xF04CD,
    PollOff24 = 0xF04CE,
    PollOff32 = 0xF04CF,
    RectangleLandscapeSparkle48 = 0xF04D0,
    RectangleLandscapeSync16 = 0xF04D1,
    RectangleLandscapeSync20 = 0xF04D2,
    RectangleLandscapeSync24 = 0xF04D3,
    RectangleLandscapeSync28 = 0xF04D4,
    RectangleLandscapeSyncOff16 = 0xF04D5,
    RectangleLandscapeSyncOff20 = 0xF04D6,
    RectangleLandscapeSyncOff24 = 0xF04D7,
    RectangleLandscapeSyncOff28 = 0xF04D8,
    Seat16 = 0xF04D9,
    Seat20 = 0xF04DA,
    Seat24 = 0xF04DB,
    SeatAdd16 = 0xF04DC,
    SeatAdd20 = 0xF04DD,
    SeatAdd24 = 0xF04DE,
    SpeakerBox16 = 0xF04DF,
    SpeakerBox20 = 0xF04E0,
    SpeakerBox24 = 0xF04E1,
    TextEditStyleCharacterGa32 = 0xF04E2,
    WindowAd24 = 0xF04E3,
    WrenchSettings20 = 0xF04E4,
    WrenchSettings24 = 0xF04E5,
    BuildingLighthouse24 = 0xF04E6,
    BuildingLighthouse32 = 0xF04E7,
    BuildingLighthouse48 = 0xF04E8,
    CalendarLink24 = 0xF04E9,
    CalendarLink28 = 0xF04EA,
    CalendarVideo24 = 0xF04EB,
    CalendarVideo28 = 0xF04EC,
    Cookies16 = 0xF04ED,
    Cookies28 = 0xF04EE,
    Cookies32 = 0xF04EF,
    Cookies48 = 0xF04F0,
    HardDrive28 = 0xF04F1,
    HardDrive48 = 0xF04F2,
    Laptop32 = 0xF04F3,
    LaptopSettings20 = 0xF04F4,
    LaptopSettings24 = 0xF04F5,
    LaptopSettings32 = 0xF04F6,
    PeopleAudience32 = 0xF04F7,
    ShoppingBagAdd20 = 0xF04F8,
    ShoppingBagAdd24 = 0xF04F9,
    StreetSign20 = 0xF04FA,
    StreetSign24 = 0xF04FB,
    VideoLink24 = 0xF04FC,
    VideoLink28 = 0xF04FD,
}

#pragma warning restore CS1591
