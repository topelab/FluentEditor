// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Avalonia.Themes.Fluent;
using FluentEditorShared.ColorPalette;
using FluentEditorShared.Utils;

namespace FluentEditor.ControlPalette
{
    public enum ColorTarget { Accent, ErrorText, AltHigh, AltLow, AltMedium, AltMediumHigh, AltMediumLow, BaseHigh, BaseLow, BaseMedium, BaseMediumHigh, BaseMediumLow, ChromeAltLow, ChromeBlackHigh, ChromeBlackLow, ChromeBlackMedium, ChromeBlackMediumLow, ChromeDisabledHigh, ChromeDisabledLow, ChromeGray, ChromeHigh, ChromeLow, ChromeMedium, ChromeMediumLow, ChromeWhite, ListLow, ListMedium }
    public enum ColorSource { LightRegion, DarkRegion, LightBase, DarkBase, LightPrimary, DarkPrimary, White, Black }

    public class ColorMapping
    {
        public static ColorMapping Parse(JsonObject data, IColorPaletteEntry lightRegion, IColorPaletteEntry darkRegion, ColorPalette lightBase, ColorPalette darkBase, ColorPalette lightPrimary, ColorPalette darkPrimary, IColorPaletteEntry white, IColorPaletteEntry black)
        {
            var target = data["Target"].GetEnum<ColorTarget>();
            var source = data["Source"].GetEnum<ColorSource>();
            int index = 0;
            if (data.ContainsKey("SourceIndex"))
            {
                index = data["SourceIndex"].GetInt();
            }

            switch (source)
            {
                case ColorSource.LightRegion:
                    return new ColorMapping(lightRegion, target);
                case ColorSource.DarkRegion:
                    return new ColorMapping(darkRegion, target);
                case ColorSource.LightBase:
                    return new ColorMapping(lightBase.Palette[index], target);
                case ColorSource.DarkBase:
                    return new ColorMapping(darkBase.Palette[index], target);
                case ColorSource.LightPrimary:
                    return new ColorMapping(lightPrimary.Palette[index], target);
                case ColorSource.DarkPrimary:
                    return new ColorMapping(darkPrimary.Palette[index], target);
                case ColorSource.White:
                    return new ColorMapping(white, target);
                case ColorSource.Black:
                    return new ColorMapping(black, target);
            }

            return null;
        }

        public static List<ColorMapping> ParseList(JsonArray data, IColorPaletteEntry lightRegion, IColorPaletteEntry darkRegion, ColorPalette lightBase, ColorPalette darkBase, ColorPalette lightPrimary, ColorPalette darkPrimary, IColorPaletteEntry white, IColorPaletteEntry black)
        {
            List<ColorMapping> retVal = new List<ColorMapping>();
            foreach (var node in data)
            {
                retVal.Add(Parse(node.AsObject(), lightRegion, darkRegion, lightBase, darkBase, lightPrimary, darkPrimary, white, black));
            }
            return retVal;
        }

        public ColorMapping(IColorPaletteEntry source, ColorTarget targetColor)
        {
            _source = source;
            _targetColor = targetColor;
        }

        private readonly IColorPaletteEntry _source;
        public IColorPaletteEntry Source => _source;

        private readonly ColorTarget _targetColor;
        public ColorTarget Target => _targetColor;
    }
}
