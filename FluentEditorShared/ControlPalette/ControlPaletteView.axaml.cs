// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace FluentEditor.ControlPalette
{
    public sealed partial class ControlPaletteView : UserControl
    {
        public ControlPaletteView()
        {
            this.InitializeComponent();
        }

        #region ViewModelProperty

        public static readonly StyledProperty<ControlPaletteViewModel> ViewModelProperty = AvaloniaProperty.Register<ControlPaletteView, ControlPaletteViewModel>("ViewModel");

        public ControlPaletteViewModel ViewModel
        {
            get { return GetValue(ViewModelProperty) as ControlPaletteViewModel; }
            set { SetValue(ViewModelProperty, value); }
        }

        #endregion
    }
}
