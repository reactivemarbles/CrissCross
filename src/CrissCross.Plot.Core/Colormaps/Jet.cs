// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

// Adapted from https://github.com/matplotlib/cmocean/tree/master/cmocean/rgb

/*
--- NOTICE BEGIN ---
The MIT License (MIT)

Copyright (c) <2012> ICRL

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
--- NOTICE END ---
*/

namespace CrissCross.Plot.Colormaps;

/// <summary>Represents the Jet lookup-table colormap.</summary>
public class Jet : IColormap
{
    /// <summary>The palette used to map normalized positions to colors.</summary>
    private readonly CustomPalette _colormap;

    /// <summary>Initializes a new instance of the <see cref="Jet"/> class.</summary>
    public Jet()
    {
        int[] rgbColors =
        {
            127, 132, 136, 141, 145, 150, 154, 159,
            163, 168, 172, 177, 182, 186, 191, 195,
            200, 204, 209, 213, 218, 222, 227, 232,
            236, 241, 245, 250, 254, 255, 255, 255,
            255, 1_279, 2_303, 3_327, 4_351, 5_375, 6_399, 7_423,
            8_447, 9_471, 10_495, 11_519, 12_543, 13_567, 14_591, 15_615,
            16_639, 17_663, 18_687, 19_711, 20_735, 21_759, 22_783, 23_807,
            24_831, 25_855, 26_879, 27_903, 28_927, 29_951, 30_975, 31_999,
            33_023, 34_047, 35_071, 36_095, 37_119, 38_143, 39_167, 40_191,
            41_215, 42_239, 43_263, 44_287, 45_311, 46_335, 47_359, 48_383,
            49_407, 50_431, 51_455, 52_479, 53_503, 54_527, 55_551, 56_574,
            57_594, 58_615, 190_708, 388_337, 585_965, 849_130, 1_046_759, 1_244_388,
            1_441_761, 1_638_365, 1_900_506, 2_097_111, 2_293_716, 2_490_320, 2_752_461, 2_949_066,
            3_145_671, 3_342_275, 3_604_416, 3_801_021, 3_997_626, 4_194_231, 4_390_835, 4_652_976,
            4_849_581, 5_046_186, 5_242_790, 5_504_931, 5_701_536, 5_898_141, 6_094_746, 6_291_350,
            6_553_491, 6_750_096, 6_946_701, 7_143_305, 7_405_446, 7_602_051, 7_798_656, 7_995_261,
            8_191_865, 8_454_006, 8_650_611, 8_847_216, 9_043_820, 9_305_961, 9_502_566, 9_699_171,
            9_895_775, 10_157_916, 10_354_521, 10_551_126, 10_747_731, 10_944_335, 11_206_476, 11_403_081,
            11_599_686, 11_796_290, 12_058_431, 12_255_036, 12_451_641, 12_648_246, 12_844_850, 13_106_991,
            13_303_596, 13_500_201, 13_696_805, 13_958_946, 14_155_551, 14_352_156, 14_548_760, 14_745_365,
            15_007_506, 15_204_111, 15_400_716, 15_597_320, 15_858_693, 16_054_274, 16_249_856, 16_445_440,
            16_706_816, 16_771_328, 16_770_304, 16_769_536, 16_768_512, 16_767_488, 16_766_720, 16_765_696,
            16_764_672, 16_763_648, 16_762_880, 16_761_856, 16_760_832, 16_760_064, 16_759_040, 16_758_016,
            16_756_992, 16_756_224, 16_755_200, 16_754_176, 16_753_408, 16_752_384, 16_751_360, 16_750_592,
            16_749_568, 16_748_544, 16_747_520, 16_746_752, 16_745_728, 16_744_704, 16_743_936, 16_742_912,
            16_741_888, 16_741_120, 16_740_096, 16_739_072, 16_738_048, 16_737_280, 16_736_256, 16_735_232,
            16_734_464, 16_733_440, 16_732_416, 16_731_392, 16_730_624, 16_729_600, 16_728_576, 16_727_808,
            16_726_784, 16_725_760, 16_724_992, 16_723_968, 16_722_944, 16_721_920, 16_721_152, 16_720_128,
            16_719_104, 16_718_336, 16_717_312, 16_650_752, 16_387_840, 16_059_136, 15_795_968, 15_467_264,
            15_204_352, 14_876_672, 14_548_992, 14_286_848, 13_959_168, 13_697_024, 13_369_344, 13_107_200,
            12_779_520, 12_517_376, 12_189_696, 11_927_552, 11_599_872, 11_272_192, 11_010_048, 10_682_368,
            10_420_224, 10_092_544, 9_830_400, 9_502_720, 9_240_576, 8_912_896, 8_650_752, 8_323_072,
        };

        Color[] colors = new Color[rgbColors.Length];
        for (int index = 0; index < rgbColors.Length; index++)
        {
            colors[index] = Color.FromARGB(unchecked((uint)(0xFF << 24) | (uint)rgbColors[index]));
        }

        _colormap = new(colors);
    }

    /// <summary>Gets the colormap name.</summary>
    public string Name => "Jet";

    /// <summary>Gets the color at the specified position.</summary>
    /// <param name="position">The position in the palette.</param>
    /// <returns>The color at the specified position.</returns>
    public Color GetColor(double position) => _colormap.GetColor(position);
}
