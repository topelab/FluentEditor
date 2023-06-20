// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace FluentEditor.ControlPalette.Export
{
    public partial class ExportView : UserControl
    {
        public ExportView(ExportViewModel viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException("viewModel");
            }
            _viewModel = viewModel;
            InitializeComponent();
        }

        private readonly ExportViewModel _viewModel;
        public ExportViewModel ViewModel => _viewModel;

        private void ExportClick(object sender, RoutedEventArgs e)
        {
            TopLevel.GetTopLevel(this)?.Clipboard?.SetTextAsync(ViewModel.ExportText);
        }

        private void LearnMoreClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            ((ContentControl)Parent!).Content = null;
        }
    }
}
