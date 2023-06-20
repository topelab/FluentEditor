// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Threading;
using FluentEditorShared;
using FluentEditorShared.ColorPalette;
using FluentEditorShared.Utils;

namespace FluentEditor.ControlPalette.Model
{
    public interface IControlPaletteModel
    {
        Task InitializeData(StringProvider stringProvider, string dataPath);
        Task HandleAppSuspend();

        void AddOrReplacePreset(Preset preset);
        void ApplyPreset(Preset preset);
        AvaloniaList<Preset> Presets { get; }
        Preset ActivePreset { get; }
        event Action<IControlPaletteModel> ActivePresetChanged;
        event EventHandler PaletteUpdated;

        IReadOnlyList<ColorMapping> LightColorMapping { get; }
        IReadOnlyList<ColorMapping> DarkColorMapping { get; }
        ColorPaletteEntry LightRegion { get; }
        ColorPaletteEntry DarkRegion { get; }
        ColorPalette LightBase { get; }
        ColorPalette DarkBase { get; }
        ColorPalette LightPrimary { get; }
        ColorPalette DarkPrimary { get; }
    }

    public class ControlPaletteModel : IControlPaletteModel
    {
        public async Task InitializeData(StringProvider stringProvider, string dataPath)
        {
            _stringProvider = stringProvider;

            var stream = AssetLoader.Open(new Uri(dataPath));
            var rootObject = JsonObject.Parse(stream).AsObject();

            _whiteColor = new ColorPaletteEntry(Colors.White, _stringProvider.GetString("DarkThemeTextContrastTitle"), null, ColorStringFormat.PoundRGB, null);
            _blackColor = new ColorPaletteEntry(Colors.Black, _stringProvider.GetString("LightThemeTextContrastTitle"), null, ColorStringFormat.PoundRGB, null);

            var lightRegionNode = rootObject["LightRegion"].AsObject();
            _lightRegion = ColorPaletteEntry.Parse(lightRegionNode, null);

            var darkRegionNode = rootObject["DarkRegion"].AsObject();
            _darkRegion = ColorPaletteEntry.Parse(darkRegionNode, null);

            var lightBaseNode = rootObject["LightBase"].AsObject();
            _lightBase = ColorPalette.Parse(lightBaseNode, null);

            var darkBaseNode = rootObject["DarkBase"].AsObject();
            _darkBase = ColorPalette.Parse(darkBaseNode, null);

            var lightPrimaryNode = rootObject["LightPrimary"].AsObject();
            _lightPrimary = ColorPalette.Parse(lightPrimaryNode, null);

            var darkPrimaryNode = rootObject["DarkPrimary"].AsObject();
            _darkPrimary = ColorPalette.Parse(darkPrimaryNode, null);

            _presets = new AvaloniaList<Preset>();
            if (rootObject.ContainsKey("Presets"))
            {
                var presetsNode = rootObject["Presets"].AsArray();
                foreach (var presetNode in presetsNode)
                {
                    _presets.Add(Preset.Parse(presetNode.AsObject()));
                }
            }
            if (_presets.Count >= 1)
            {
                ApplyPreset(_presets[0]);
            }

            UpdateActivePreset();

            _lightRegion.ContrastColors = new List<ContrastColorWrapper> { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, true, true), new ContrastColorWrapper(_lightBase.BaseColor, true, false), new ContrastColorWrapper(_lightPrimary.BaseColor, true, false) };
            _darkRegion.ContrastColors = new List<ContrastColorWrapper> { new ContrastColorWrapper(_whiteColor, true, true), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_darkBase.BaseColor, true, false), new ContrastColorWrapper(_darkPrimary.BaseColor, true, false) };
            _lightBase.ContrastColors = new List<ContrastColorWrapper> { new ContrastColorWrapper(_whiteColor, false, false), new ContrastColorWrapper(_blackColor, true, true), new ContrastColorWrapper(_lightRegion, true, false), new ContrastColorWrapper(_lightPrimary.BaseColor, true, false) };
            _darkBase.ContrastColors = new List<ContrastColorWrapper> { new ContrastColorWrapper(_whiteColor, true, true), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_darkRegion, true, false), new ContrastColorWrapper(_darkPrimary.BaseColor, true, false) };
            _lightPrimary.ContrastColors = new List<ContrastColorWrapper> { new ContrastColorWrapper(_whiteColor, true, true), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_lightRegion, true, false), new ContrastColorWrapper(_lightBase.BaseColor, true, false) };
            _darkPrimary.ContrastColors = new List<ContrastColorWrapper> { new ContrastColorWrapper(_whiteColor, true, true), new ContrastColorWrapper(_blackColor, false, false), new ContrastColorWrapper(_darkRegion, true, false), new ContrastColorWrapper(_darkBase.BaseColor, true, false) };

            _lightColorMappings = ColorMapping.ParseList(rootObject["LightPaletteMapping"].AsArray(), _lightRegion, _darkRegion, _lightBase, _darkBase, _lightPrimary, _darkPrimary, _whiteColor, _blackColor);
            _lightColorMappings.Sort((a, b) =>
            {
                return a.Target.ToString().CompareTo(b.Target.ToString());
            });

            _darkColorMappings = ColorMapping.ParseList(rootObject["DarkPaletteMapping"].AsArray(), _lightRegion, _darkRegion, _lightBase, _darkBase, _lightPrimary, _darkPrimary, _whiteColor, _blackColor);
            _darkColorMappings.Sort((a, b) =>
            {
                return a.Target.ToString().CompareTo(b.Target.ToString());
            });

            _lightRegion.ActiveColorChanged += PaletteEntry_ActiveColorChanged;
            _darkRegion.ActiveColorChanged += PaletteEntry_ActiveColorChanged;
            _lightBase.BaseColor.ActiveColorChanged += PaletteEntry_ActiveColorChanged;
            _darkBase.BaseColor.ActiveColorChanged += PaletteEntry_ActiveColorChanged;
            _lightPrimary.BaseColor.ActiveColorChanged += PaletteEntry_ActiveColorChanged;
            _darkPrimary.BaseColor.ActiveColorChanged += PaletteEntry_ActiveColorChanged;
            foreach (var entry in _lightBase.Palette)
            {
                entry.ActiveColorChanged += PaletteEntry_ActiveColorChanged;
            }
            foreach (var entry in _darkBase.Palette)
            {
                entry.ActiveColorChanged += PaletteEntry_ActiveColorChanged;
            }
            foreach (var entry in _lightPrimary.Palette)
            {
                entry.ActiveColorChanged += PaletteEntry_ActiveColorChanged;
            }
            foreach (var entry in _darkPrimary.Palette)
            {
                entry.ActiveColorChanged += PaletteEntry_ActiveColorChanged;
            }

            if (_lightRegion.Description == null)
            {
                _lightRegion.Description = GenerateMappingDescription(_lightRegion, _lightColorMappings);
            }
            if (_darkRegion.Description == null)
            {
                _darkRegion.Description = GenerateMappingDescription(_darkRegion, _darkColorMappings);
            }
            foreach (var entry in _lightBase.Palette)
            {
                if (entry.Description == null)
                {
                    entry.Description = GenerateMappingDescription(entry, _lightColorMappings);
                }
            }
            foreach (var entry in _darkBase.Palette)
            {
                if (entry.Description == null)
                {
                    entry.Description = GenerateMappingDescription(entry, _darkColorMappings);
                }
            }
            foreach (var entry in _lightPrimary.Palette)
            {
                if (entry.Description == null)
                {
                    entry.Description = GenerateMappingDescription(entry, _lightColorMappings);
                }
            }
            foreach (var entry in _darkPrimary.Palette)
            {
                if (entry.Description == null)
                {
                    entry.Description = GenerateMappingDescription(entry, _darkColorMappings);
                }
            }
        }
        
        private string GenerateMappingDescription(IColorPaletteEntry paletteEntry, List<ColorMapping> mappings)
        {
            string retVal = string.Empty;

            foreach (var mapping in mappings)
            {
                if (mapping.Source == paletteEntry)
                {
                    if (retVal != string.Empty)
                    {
                        retVal += ", ";
                    }
                    retVal += mapping.Target.ToString();
                }
            }

            if (retVal != string.Empty)
            {
                return string.Format(_stringProvider.GetString("ColorFlyoutMappingDescription"), retVal);
            }

            return null;
        }

        public Task HandleAppSuspend()
        {
            // Currently nothing to do here
            return Task.CompletedTask;
        }

        private DispatcherTimer _timer;
        private void PaletteEntry_ActiveColorChanged(IColorPaletteEntry obj)
        {
            UpdateActivePreset();

            _timer?.Stop();
            _timer = new DispatcherTimer(TimeSpan.FromMilliseconds(500), DispatcherPriority.Background, (_, _) =>
            {
                _timer.Stop();
                PaletteUpdated?.Invoke(this, EventArgs.Empty);
            });
        }

        private void UpdateActivePreset()
        {
            if (_presets != null)
            {
                for (int i = 0; i < _presets.Count; i++)
                {
                    if (_presets[i].IsPresetActive(this))
                    {
                        ActivePreset = _presets[i];
                        return;
                    }
                }
            }
            ActivePreset = null;
        }

        private StringProvider _stringProvider;

        public void AddOrReplacePreset(Preset preset)
        {
            if (!string.IsNullOrEmpty(preset.Name))
            {
                var oldPreset = _presets.FirstOrDefault(a => a.Id == preset.Id);
                if (oldPreset != null)
                {
                    _presets.Remove(oldPreset);
                }
            }

            _presets.Add(preset);

            UpdateActivePreset();
        }

        public void ApplyPreset(Preset preset)
        {
            if (preset == null)
            {
                ActivePreset = null;
                return;
            }

            _lightRegion.ActiveColor = preset.LightRegionColor;
            _darkRegion.ActiveColor = preset.DarkRegionColor;
            
            _lightBase.BaseColor.ActiveColor = preset.LightBaseColor;
            _darkBase.BaseColor.ActiveColor = preset.DarkBaseColor;
            _lightPrimary.BaseColor.ActiveColor = preset.LightPrimaryColor;
            _darkPrimary.BaseColor.ActiveColor = preset.DarkPrimaryColor;

            ApplyPresetOverrides(_lightBase.Palette, preset.LightBaseOverrides);
            ApplyPresetOverrides(_darkBase.Palette, preset.DarkBaseOverrides);
            ApplyPresetOverrides(_lightPrimary.Palette, preset.LightPrimaryOverrides);
            ApplyPresetOverrides(_darkPrimary.Palette, preset.DarkPrimaryOverrides);
        }

        private void ApplyPresetOverrides(IReadOnlyList<EditableColorPaletteEntry> palette, Dictionary<int, Color> overrides)
        {
            for (int i = 0; i < palette.Count; i++)
            {
                if (overrides != null && overrides.ContainsKey(i))
                {
                    palette[i].CustomColor = overrides[i];
                    palette[i].UseCustomColor = true;
                }
                else
                {
                    palette[i].UseCustomColor = false;
                }
            }
        }

        private AvaloniaList<Preset> _presets;
        public AvaloniaList<Preset> Presets => _presets;

        private Preset _activePreset;
        public Preset ActivePreset
        {
            get => _activePreset;
            private set
            {
                if (_activePreset != value)
                {
                    _activePreset = value;
                    ActivePresetChanged?.Invoke(this);
                }
            }
        }

        public event Action<IControlPaletteModel> ActivePresetChanged;
        public event EventHandler PaletteUpdated;

        private List<ColorMapping> _lightColorMappings;
        public IReadOnlyList<ColorMapping> LightColorMapping => _lightColorMappings;

        private List<ColorMapping> _darkColorMappings;
        public IReadOnlyList<ColorMapping> DarkColorMapping => _darkColorMappings;

        private ColorPaletteEntry _whiteColor;
        private ColorPaletteEntry _blackColor;

        private ColorPaletteEntry _lightRegion;
        public ColorPaletteEntry LightRegion => _lightRegion;

        private ColorPaletteEntry _darkRegion;
        public ColorPaletteEntry DarkRegion => _darkRegion;

        private ColorPalette _lightBase;
        public ColorPalette LightBase => _lightBase;

        private ColorPalette _darkBase;
        public ColorPalette DarkBase => _darkBase;

        private ColorPalette _lightPrimary;
        public ColorPalette LightPrimary => _lightPrimary;

        private ColorPalette _darkPrimary;
        public ColorPalette DarkPrimary => _darkPrimary;
    }
}
