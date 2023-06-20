// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using FluentEditor.ControlPalette.Export;
using System;
using System.Text;
using System.Threading.Tasks;

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
            sb.Append("    <ColorPaletteResources x:Key=\"Default\"");
            if (model.LightColorMapping != null)
            {
                foreach (var m in model.LightColorMapping)
                {
                    sb.Append(" ");
                    sb.Append(m.Target.ToString());
                    sb.Append("=\"");
                    sb.Append(m.Source.ActiveColor.ToString());
                    sb.Append("\"");
                }
            }

            sb.AppendLine(" />");
            sb.Append("    <ColorPaletteResources x:Key=\"Dark\"");
            if (model.DarkColorMapping != null)
            {
                foreach (var m in model.DarkColorMapping)
                {
                    sb.Append(" ");
                    sb.Append(m.Target.ToString());
                    sb.Append("=\"");
                    sb.Append(m.Source.ActiveColor.ToString());
                    sb.Append("\"");
                }
            }

            sb.AppendLine(" />");

            sb.AppendLine("  </FluentTheme.Palettes>");
            sb.AppendLine("</FluentTheme>");

            var retVal = sb.ToString();
            return retVal;
        }

        public async Task ShowExportView(string exportData)
        {
            _exportViewModel = new ExportViewModel(exportData);
            var exportView = new ExportView(_exportViewModel);
            App.NavPage.NavigationFrame.Content = exportView;
        }
    }
}