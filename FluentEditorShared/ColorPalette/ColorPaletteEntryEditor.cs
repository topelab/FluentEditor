// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;
using Avalonia.Styling;

namespace FluentEditorShared.ColorPalette
{
    public enum ColorPaletteEntryCaptionMode { None, ActiveColorString, Title };

    public class ColorPaletteEntryEditor : Button
    {
        public ColorPaletteEntryEditor()
        {
            // Make sure each instance of the control gets its own brush instances
            ActiveColorBrush = new SolidColorBrush();
            ContrastColorBrush = new SolidColorBrush();

            this.Click += ColorPaletteEntryEditor_Click;
        }

        #region ColorPaletteEntryProperty

        public static readonly StyledProperty<IColorPaletteEntry> ColorPaletteEntryProperty = AvaloniaProperty.Register<ColorPaletteEntryEditor, IColorPaletteEntry>("ColorPaletteEntry");
        
        private void OnColorPaletteEntryChanged(IColorPaletteEntry oldValue, IColorPaletteEntry newValue)
        {
            if (oldValue != null)
            {
                oldValue.ActiveColorChanged -= ColorPaletteEntry_ActiveColorChanged;
                oldValue.ContrastColorChanged -= ColorPaletteEntry_ContrastColorChanged;
            }

            if (newValue != null)
            {
                ActiveColorBrush.Color = newValue.ActiveColor;
                ContrastColorBrush.Color = newValue.BestContrastColor != null ? newValue.BestContrastColor.Color.ActiveColor : default(Color);
                newValue.ActiveColorChanged += ColorPaletteEntry_ActiveColorChanged;
                newValue.ContrastColorChanged += ColorPaletteEntry_ContrastColorChanged;

                if (_flyoutContent != null)
                {
                    _flyoutContent.Content = newValue;
                }
            }
            else
            {
                HideFlyout();
                ActiveColorBrush.Color = default(Color);
                ContrastColorBrush.Color = default(Color);
            }

            UpdateCaption();
        }

        public IColorPaletteEntry ColorPaletteEntry
        {
            get { return GetValue(ColorPaletteEntryProperty) as IColorPaletteEntry; }
            set { SetValue(ColorPaletteEntryProperty, value); }
        }

        #endregion

        #region CaptionModeProperty

        public static readonly StyledProperty<ColorPaletteEntryCaptionMode> CaptionModeProperty = AvaloniaProperty.Register<ColorPaletteEntryEditor, ColorPaletteEntryCaptionMode>("CaptionMode");

        public ColorPaletteEntryCaptionMode CaptionMode
        {
            get { return (ColorPaletteEntryCaptionMode)GetValue(CaptionModeProperty); }
            set { SetValue(CaptionModeProperty, value); }
        }

        #endregion

        #region CaptionProperty

        public static readonly StyledProperty<string> CaptionProperty = AvaloniaProperty.Register<ColorPaletteEntryEditor, string>("Caption");

        public string Caption
        {
            get { return GetValue(CaptionProperty) as string; }
            private set { SetValue(CaptionProperty, value); }
        }

        #endregion

        #region FlyoutTemplateProperty

        public static readonly StyledProperty<DataTemplate> FlyoutTemplateProperty = AvaloniaProperty.Register<ColorPaletteEntryEditor, DataTemplate>("FlyoutTemplate");

        public DataTemplate FlyoutTemplate
        {
            get { return GetValue(FlyoutTemplateProperty) as DataTemplate; }
            set { SetValue(FlyoutTemplateProperty, value); }
        }

        #endregion

        #region FlyoutPresenterStyleProperty

        public static readonly StyledProperty<ControlTheme> FlyoutPresenterStyleProperty = AvaloniaProperty.Register<ColorPaletteEntryEditor, ControlTheme>("FlyoutPresenterStyle");

        public ControlTheme FlyoutPresenterStyle
        {
            get { return GetValue(FlyoutPresenterStyleProperty) as ControlTheme; }
            set { SetValue(FlyoutPresenterStyleProperty, value); }
        }

        #endregion

        #region ActiveColorBrushProperty

        public static readonly StyledProperty<SolidColorBrush> ActiveColorBrushProperty = AvaloniaProperty.Register<ColorPaletteEntryEditor, SolidColorBrush>("ActiveColorBrush", new SolidColorBrush());

        public SolidColorBrush ActiveColorBrush
        {
            get { return GetValue(ActiveColorBrushProperty) as SolidColorBrush; }
            private set { SetValue(ActiveColorBrushProperty, value); }
        }

        #endregion

        #region ContrastColorBrushProperty

        public static readonly StyledProperty<SolidColorBrush> ContrastColorBrushProperty = AvaloniaProperty.Register<ColorPaletteEntryEditor, SolidColorBrush>("ContrastColorBrush", new SolidColorBrush());

        public SolidColorBrush ContrastColorBrush
        {
            get { return GetValue(ContrastColorBrushProperty) as SolidColorBrush; }
            private set { SetValue(ContrastColorBrushProperty, value); }
        }

        #endregion

        private void ColorPaletteEntry_ActiveColorChanged(IColorPaletteEntry obj)
        {
            var paletteEntry = ColorPaletteEntry;
            if (paletteEntry == null)
            {
                return;
            }
            ActiveColorBrush.Color = paletteEntry.ActiveColor;

            UpdateCaption();
        }

        private void ColorPaletteEntry_ContrastColorChanged(IColorPaletteEntry obj)
        {
            var paletteEntry = ColorPaletteEntry;
            if (paletteEntry == null)
            {
                return;
            }
            ContrastColorBrush.Color = paletteEntry.BestContrastColor != null ? paletteEntry.BestContrastColor.Color.ActiveColor : default(Color);
        }

        private void UpdateCaption()
        {
            var captionMode = CaptionMode;
            var paletteEntry = ColorPaletteEntry;
            switch (captionMode)
            {
                case ColorPaletteEntryCaptionMode.None:
                    Caption = string.Empty;
                    break;
                case ColorPaletteEntryCaptionMode.ActiveColorString:
                    Caption = paletteEntry != null ? paletteEntry.ActiveColorString : string.Empty;
                    break;
                case ColorPaletteEntryCaptionMode.Title:
                    Caption = paletteEntry != null ? paletteEntry.Title : string.Empty;
                    break;
            }
        }

        private void ColorPaletteEntryEditor_Click(object sender, RoutedEventArgs e)
        {
            ShowFlyout();
        }

        private Flyout _flyout;
        private ContentControl _flyoutContent;

        private void ShowFlyout()
        {
            HideFlyout();

            var flyoutTemplate = FlyoutTemplate;
            if (flyoutTemplate == null)
            {
                return;
            }

            var paletteEntry = ColorPaletteEntry;

            Flyout flyout = new Flyout();

            var flyoutPresenterStyle = FlyoutPresenterStyle;
            if(flyoutPresenterStyle != null)
            {
                flyout.FlyoutPresenterTheme = flyoutPresenterStyle;
            }

            ContentControl flyoutContent = new ContentControl();
            flyoutContent.ContentTemplate = flyoutTemplate;
            flyoutContent.Content = paletteEntry;
            flyoutContent.HorizontalAlignment = HorizontalAlignment.Stretch;
            flyoutContent.VerticalAlignment = VerticalAlignment.Stretch;
            flyoutContent.Margin = new Thickness(0);
            flyoutContent.Padding = new Thickness(0);

            flyout.Content = flyoutContent;

            flyout.ShowAt(this);
        }

        private void HideFlyout()
        {
            if (_flyout != null)
            {
                _flyout.Hide();
                _flyout = null;
                _flyoutContent = null;
            }
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == CaptionModeProperty)
            {
                UpdateCaption();
            }
            else if (change.Property == ColorPaletteEntryProperty)
            {
                var (oldValue, newValue) = change.GetOldAndNewValue<IColorPaletteEntry>();
                OnColorPaletteEntryChanged(oldValue, newValue);
            }
        }
    }
}
