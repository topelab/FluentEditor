// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using FluentEditor.OuterNav;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentEditorShared.ColorPalette;
using FluentEditor.ControlPalette.Model;
using FluentEditorShared;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using FluentEditorShared.Utils;

namespace FluentEditor.ControlPalette
{
    public class ControlPaletteViewModel : INavItem
    {
        public static ControlPaletteViewModel Parse(IStringProvider stringProvider, JsonObject data, IControlPaletteModel paletteModel, IControlPaletteExportProvider exportProvider)
        {
            return new ControlPaletteViewModel(stringProvider, paletteModel, data["Id"].GetOptionalString(), data["Title"].GetOptionalString(), data["Glyph"].GetOptionalString(), exportProvider);
        }

        public ControlPaletteViewModel(IStringProvider stringProvider, IControlPaletteModel paletteModel, string id, string title, string glyph, IControlPaletteExportProvider exportProvider)
        {
            _stringProvider = stringProvider;
            _id = id;
            _title = title;
            _glyph = glyph;

            _paletteModel = paletteModel;
            _exportProvider = exportProvider;

            _controlBorderThickness = new Thickness(1);
            _controlCornerRadius = new CornerRadius(2);
            _overlayCornerRadius = new CornerRadius(4);

            _lightRegionBrush = new SolidColorBrush(_paletteModel.LightRegion.ActiveColor);
            _darkRegionBrush = new SolidColorBrush(_paletteModel.DarkRegion.ActiveColor);

            _paletteModel.LightRegion.ActiveColorChanged += LightRegion_ActiveColorChanged;
            _paletteModel.DarkRegion.ActiveColorChanged += DarkRegion_ActiveColorChanged;

            _paletteModel.ActivePresetChanged += OnActivePresetChanged;
        }

        private IStringProvider _stringProvider;

        private readonly string _id;
        public string Id
        {
            get { return _id; }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    RaisePropertyChangedFromSource();
                }
            }
        }

        private string _glyph;
        public string Glyph
        {
            get { return _glyph; }
            set
            {
                if (_glyph != value)
                {
                    _glyph = value;
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

        private void OnActivePresetChanged(IControlPaletteModel obj)
        {
            RaisePropertyChanged("ActivePreset");
        }

        private readonly IControlPaletteModel _paletteModel;
        private readonly IControlPaletteExportProvider _exportProvider;

        public Preset ActivePreset
        {
            get { return _paletteModel.ActivePreset; }
            set
            {
                _paletteModel.ApplyPreset(value);
            }
        }

        public IReadOnlyList<Preset> Presets
        {
            get { return _paletteModel.Presets; }
        }

        public ColorPaletteEntry LightRegion
        {
            get { return _paletteModel.LightRegion; }
        }

        private void LightRegion_ActiveColorChanged(IColorPaletteEntry obj)
        {
            _lightRegionBrush.Color = obj.ActiveColor;
        }

        private SolidColorBrush _lightRegionBrush;
        public SolidColorBrush LightRegionBrush
        {
            get { return _lightRegionBrush; }
        }

        private CornerRadius _controlCornerRadius;
        public CornerRadius ControlCornerRadiusValue
        {
            set { _controlCornerRadius = value; }
            get { return _controlCornerRadius; }
        }

        private CornerRadius _overlayCornerRadius;
        public CornerRadius OverlayCornerRadiusValue
        {
            set { _overlayCornerRadius = value; }
            get { return _overlayCornerRadius; }
        }

        private Thickness _controlBorderThickness;
        public Thickness ControlBorderThicknessValue
        {
            set { _controlBorderThickness = value; }
            get { return _controlBorderThickness; }
        }

        public ColorPaletteEntry DarkRegion
        {
            get { return _paletteModel.DarkRegion; }
        }

        private void DarkRegion_ActiveColorChanged(IColorPaletteEntry obj)
        {
            _darkRegionBrush.Color = obj.ActiveColor;
        }

        private SolidColorBrush _darkRegionBrush;
        public SolidColorBrush DarkRegionBrush
        {
            get { return _darkRegionBrush; }
        }

        public ColorPalette LightBase
        {
            get { return _paletteModel.LightBase; }
        }

        public ColorPalette DarkBase
        {
            get { return _paletteModel.DarkBase; }
        }

        public ColorPalette LightPrimary
        {
            get { return _paletteModel.LightPrimary; }
        }

        public ColorPalette DarkPrimary
        {
            get { return _paletteModel.DarkPrimary; }
        }

        public IReadOnlyList<ColorMapping> LightColorMapping
        {
            get { return _paletteModel.LightColorMapping; }
        }

        public IReadOnlyList<ColorMapping> DarkColorMapping
        {
            get { return _paletteModel.DarkColorMapping; }
        }

        public void OnExportRequested()
        {
            _exportProvider.ShowExportView(_exportProvider.GenerateExportData(_paletteModel, this));
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
