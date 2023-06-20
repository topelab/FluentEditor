// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;

namespace FluentEditor.ControlPalette
{
    public sealed partial class ControlPaletteView : UserControl
    {
        public ControlPaletteView()
        {
            InitializeComponent();
        }

        #region ViewModelProperty

        public static readonly StyledProperty<ControlPaletteViewModel> ViewModelProperty = AvaloniaProperty.Register<ControlPaletteView, ControlPaletteViewModel>("ViewModel");

        public ControlPaletteViewModel ViewModel
        {
            get => GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        #endregion

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == ViewModelProperty)
            {
                ViewModel.RebuldPreviews += ViewModelOnRebuldPreviews;
                ViewModelOnRebuldPreviews(this, EventArgs.Empty);
            }
        }

        private void ViewModelOnRebuldPreviews(object sender, EventArgs e)
        {
            TestContentContainer.Styles.Clear();
            TestContentContainer.Styles.Add(new FluentTheme
            {
                Palettes =
                {
                    [ThemeVariant.Default] = ViewModel.CreateResources(false),
                    [ThemeVariant.Light] = ViewModel.CreateResources(false),
                    [ThemeVariant.Dark] = ViewModel.CreateResources(true)
                }
            });
            
            LightTestContentContainer.Child = new ControlPaletteTestContent
            {
                Title = App.StringProvider.GetString("ControlPaletteLightTestContent")
            };
            DarkTestContentContainer.Child = new ControlPaletteTestContent
            {
                Title = App.StringProvider.GetString("ControlPaletteDarkTestContent")
            };
        }
    }
}
