// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Themes.Fluent;
using FluentEditor.ControlPalette.Model;
using FluentEditor.OuterNav;
using FluentEditorShared;
using FluentEditorShared.ColorPalette;
using FluentEditorShared.Utils;

namespace FluentEditor.ControlPalette
{
    public class ControlPaletteViewModel : INavItem
    {
        public static ControlPaletteViewModel Parse(IStringProvider stringProvider, JsonObject data, IControlPaletteModel paletteModel, IControlPaletteExportProvider exportProvider)
        {
            return new ControlPaletteViewModel(stringProvider, paletteModel, data["Id"].GetOptionalString(), data["Title"].GetOptionalString(), exportProvider);
        }

        public ControlPaletteViewModel(IStringProvider stringProvider, IControlPaletteModel paletteModel, string id, string title, IControlPaletteExportProvider exportProvider)
        {
            _stringProvider = stringProvider;
            _id = id;
            _title = title;

            _paletteModel = paletteModel;
            _exportProvider = exportProvider;

            _paletteModel.PaletteUpdated += PaletteModelOnPaletteUpdated;
            _paletteModel.ActivePresetChanged += OnActivePresetChanged;
        }

        public event EventHandler RebuldPreviews;

        private IStringProvider _stringProvider;

        private readonly string _id;
        public string Id => _id;

        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    RaisePropertyChangedFromSource();
                }
            }
        }
        
        public void OnSaveDataRequested()
        {
            _ = SaveData();
        }

        private async Task SaveData()
        {
            var provider = TopLevel.GetTopLevel(App.NavPage).StorageProvider;
            var file = await provider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                FileTypeChoices = new[] { App.JsonFileType }
            });

            if (file == null)
            {
                return;
            }

            Preset savePreset = new Preset(file.Name, file.Name, _paletteModel);
            var saveData = Preset.Serialize(savePreset);
            var saveString = saveData.ToString();

            await using var stream = await file.OpenWriteAsync();
            await using var writer = new StreamWriter(stream);
            await writer.WriteAsync(saveString);
        }

        public void OnLoadDataRequested()
        {
            _ = LoadData();
        }

        private async Task LoadData()
        {
            var provider = TopLevel.GetTopLevel(App.NavPage).StorageProvider;
            var file = (await provider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                FileTypeFilter = new[] { App.JsonFileType }
            })).FirstOrDefault();

            if (file == null)
            {
                return;
            }

            await using var data = await file.OpenReadAsync();
            using var reader = new StreamReader(data);
            var rootObject = JsonObject.Parse(await reader.ReadToEndAsync()).AsObject();
            Preset loadedPreset = null;
            try
            {
                loadedPreset = Preset.Parse(rootObject, file.TryGetLocalPath() ?? file.Name, file.Name);
            }
            catch
            {
                loadedPreset = null;
            }

            _paletteModel.AddOrReplacePreset(loadedPreset);
            _paletteModel.ApplyPreset(loadedPreset);
        }

        private void PaletteModelOnPaletteUpdated(object sender, EventArgs e)
        {
            RebuldPreviews?.Invoke(this, e);
        }
        
        private void OnActivePresetChanged(IControlPaletteModel obj)
        {
            RaisePropertyChanged("ActivePreset");
        }

        private readonly IControlPaletteModel _paletteModel;
        private readonly IControlPaletteExportProvider _exportProvider;

        public Preset ActivePreset
        {
            get => _paletteModel.ActivePreset;
            set => _paletteModel.ApplyPreset(value);
        }

        public IReadOnlyList<Preset> Presets => _paletteModel.Presets;

        public ColorPaletteEntry LightRegion => _paletteModel.LightRegion;

        public ColorPaletteEntry DarkRegion => _paletteModel.DarkRegion;

        public ColorPalette LightBase => _paletteModel.LightBase;

        public ColorPalette DarkBase => _paletteModel.DarkBase;

        public ColorPalette LightPrimary => _paletteModel.LightPrimary;

        public ColorPalette DarkPrimary => _paletteModel.DarkPrimary;

        public IReadOnlyList<ColorMapping> LightColorMapping => _paletteModel.LightColorMapping;

        public IReadOnlyList<ColorMapping> DarkColorMapping => _paletteModel.DarkColorMapping;

        public void OnExportRequested()
        {
            _exportProvider.ShowExportView(_exportProvider.GenerateExportData(_paletteModel, this));
        }

        public ColorPaletteResources CreateResources(bool dark)
        {
            var res = new ColorPaletteResources();
            res.RegionColor = dark ? DarkRegion.ActiveColor : LightRegion.ActiveColor;
            res.Accent = GetColor(ColorTarget.Accent);
            res.ErrorText = GetColor(ColorTarget.ErrorText);
            res.AltHigh = GetColor(ColorTarget.AltHigh);
            res.AltLow = GetColor(ColorTarget.AltLow);
            res.AltMedium = GetColor(ColorTarget.AltMedium);
            res.AltMediumHigh = GetColor(ColorTarget.AltMediumHigh);
            res.AltMediumLow = GetColor(ColorTarget.AltMediumLow);
            res.BaseHigh = GetColor(ColorTarget.BaseHigh);
            res.BaseLow = GetColor(ColorTarget.BaseLow);
            res.BaseMedium = GetColor(ColorTarget.BaseMedium);
            res.BaseMediumHigh = GetColor(ColorTarget.BaseMediumHigh);
            res.BaseMediumLow = GetColor(ColorTarget.BaseMediumLow);
            res.ChromeAltLow = GetColor(ColorTarget.ChromeAltLow);
            res.ChromeBlackHigh = GetColor(ColorTarget.ChromeBlackHigh);
            res.ChromeBlackLow = GetColor(ColorTarget.ChromeBlackLow);
            res.ChromeBlackMedium = GetColor(ColorTarget.ChromeBlackMedium);
            res.ChromeBlackMediumLow = GetColor(ColorTarget.ChromeBlackMediumLow);
            res.ChromeDisabledHigh = GetColor(ColorTarget.ChromeDisabledHigh);
            res.ChromeDisabledLow = GetColor(ColorTarget.ChromeDisabledLow);
            res.ChromeGray = GetColor(ColorTarget.ChromeGray);
            res.ChromeHigh = GetColor(ColorTarget.ChromeHigh);
            res.ChromeLow = GetColor(ColorTarget.ChromeLow);
            res.ChromeMedium = GetColor(ColorTarget.ChromeMedium);
            res.ChromeMediumLow = GetColor(ColorTarget.ChromeMediumLow);
            res.ChromeWhite = GetColor(ColorTarget.ChromeWhite);
            res.ListLow = GetColor(ColorTarget.ListLow);
            res.ListMedium = GetColor(ColorTarget.ListMedium);
            return res;
            
            Color GetColor(ColorTarget target)
            {
                var mappings = dark ? DarkColorMapping : LightColorMapping;
                return mappings.FirstOrDefault(m => m.Target == target)?.Source.ActiveColor ?? default;
            }
        }
        
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void RaisePropertyChangedFromSource([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
