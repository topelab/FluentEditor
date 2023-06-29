// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Text;
using System.Threading.Tasks;
using FluentEditor.ControlPalette.Export;
using FluentEditorShared.ColorPalette;

namespace FluentEditor.ControlPalette.Model
{
    public interface IControlPaletteExportProvider
    {
        Task ShowExportView(string exportData);

        string GenerateExportData(IControlPaletteModel model, ControlPaletteViewModel viewModel,
            bool showAllColors = false);
    }

    public class ControlPaletteExportProvider : IControlPaletteExportProvider
    {
        private bool _isWindowInitializing = false;

        // This is owned by the UI thread for the _exportWindow
        private ExportViewModel _exportViewModel;

        public string GenerateExportData(IControlPaletteModel model, ControlPaletteViewModel viewModel,
            bool showAllColors = false)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<FluentTheme>");
            sb.AppendLine("  <FluentTheme.Palettes>");
            sb.Append("    <ColorPaletteResources x:Key=\"Light\"");
            if (model.LightColorMapping != null)
            {
                foreach (var m in model.LightColorMapping)
                {
                    AppendColor(m.Target.ToString(), m.Source);
                }
            }
            AppendColor("RegionColor", model.LightRegion);

            sb.AppendLine(" />");
            sb.Append("    <ColorPaletteResources x:Key=\"Dark\"");
            if (model.DarkColorMapping != null)
            {
                foreach (var m in model.DarkColorMapping)
                {
                    AppendColor(m.Target.ToString(), m.Source);
                }
            }
            AppendColor("RegionColor", model.DarkRegion);

            sb.AppendLine(" />");

            sb.AppendLine("  </FluentTheme.Palettes>");
            sb.AppendLine("</FluentTheme>");

            var retVal = sb.ToString();
            return retVal;

            void AppendColor(string name, IColorPaletteEntry entry)
            {
                sb.Append(" ");
                sb.Append(name);
                sb.Append("=\"");
                sb.Append(entry.ActiveColor.ToString());
                sb.Append("\"");
            }
        }

        public async Task ShowExportView(string exportData)
        {
            _exportViewModel = new ExportViewModel(exportData);
            var exportView = new ExportView(_exportViewModel);
            exportView.DataContext = _exportViewModel;
            App.NavPage.OverlayContainer.Content = exportView;
        }
    }
}