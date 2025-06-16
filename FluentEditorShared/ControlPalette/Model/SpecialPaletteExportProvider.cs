using Avalonia.Media;
using FluentEditor;
using FluentEditor.ControlPalette;
using FluentEditor.ControlPalette.Export;
using FluentEditor.ControlPalette.Model;
using FluentEditorShared.ColorPalette;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FluentEditorShared.ControlPalette.Model
{
    public class SpecialPaletteExportProvider : IControlPaletteExportProvider
    {
        private ExportViewModel _exportViewModel;
        private const string LightPalette = "Light";
        private const string DarkPalette = "Dark";

        public string GenerateExportData(IControlPaletteModel model, ControlPaletteViewModel viewModel,
            bool showAllColors = false)
        {
            StringBuilder sb = new StringBuilder();
            AppendMapping(model.LightColorMapping, LightPalette, model.LightRegion, sb);
            AppendMapping(model.DarkColorMapping, DarkPalette, model.DarkRegion, sb);

            var retVal = sb.ToString();
            return retVal;
        }

        private static void AppendMapping(IReadOnlyList<ColorMapping> mappings, string paletteName, IColorPaletteEntry regionColor, StringBuilder sb)
        {
            if (mappings == null || mappings.Count == 0)
            {
                return;
            }
            foreach (var mapping in mappings)
            {
                AppendColor(paletteName, mapping.Target.ToString(), mapping.Source, sb);
            }
            AppendColor(paletteName, "RegionColor", regionColor, sb);
        }

        private static void AppendColor(string paletteName, string name, IColorPaletteEntry entry, StringBuilder sb)
        {
            sb.Append('"');
            sb.Append(paletteName);
            sb.Append('_');
            sb.Append(name);
            sb.Append('"');
            sb.Append(" : ");
            sb.Append('"');
            sb.Append(ToCssColor(entry.ActiveColor));
            sb.Append('"');
            sb.AppendLine(",");
        }

        private static string ToCssColor(Color color)
        {
            var hexColor = string.Concat(color.R.ToString("X2", CultureInfo.InvariantCulture),
                           color.G.ToString("X2", CultureInfo.InvariantCulture),
                           color.B.ToString("X2", CultureInfo.InvariantCulture));
            if (color.A < 255)
            {
                hexColor = string.Concat(hexColor, color.A.ToString("X2", CultureInfo.InvariantCulture));
            }
            return $"#{hexColor}";
        }

        public void ShowExportView(string exportData)
        {
            _exportViewModel = new ExportViewModel(exportData);
            var exportView = new ExportView(_exportViewModel);
            exportView.DataContext = _exportViewModel;
            App.NavPage.OverlayContainer.Content = exportView;
        }
    }
}
