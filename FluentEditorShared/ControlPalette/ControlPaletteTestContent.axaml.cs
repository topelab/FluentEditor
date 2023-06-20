// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Avalonia;
using Avalonia.Controls;

namespace FluentEditor.ControlPalette
{
    public sealed partial class ControlPaletteTestContent : UserControl
    {
        public ControlPaletteTestContent()
        {
            this.InitializeComponent();
        }

        public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<ControlPaletteTestContent, string>("Title");

        public string Title
        {
            get { return GetValue(TitleProperty) as string; }
            set { SetValue(TitleProperty, value); }
        }
    }
}
