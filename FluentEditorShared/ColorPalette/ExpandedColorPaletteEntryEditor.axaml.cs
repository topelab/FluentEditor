// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace FluentEditorShared.ColorPalette
{
    public partial class ExpandedColorPaletteEntryEditor : UserControl
    {
        public ExpandedColorPaletteEntryEditor()
        {
            InitializeComponent();
        }

        #region ColorPaletteEntryProperty

        public static readonly StyledProperty<IColorPaletteEntry> ColorPaletteEntryProperty = AvaloniaProperty.Register<ExpandedColorPaletteEntryEditor, IColorPaletteEntry>("ColorPaletteEntry");

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == ColorPaletteEntryProperty)
            {
                var (oldValue, newValue) = change.GetOldAndNewValue<IColorPaletteEntry>();
                OnColorPaletteEntryChanged(oldValue, newValue);
            }
        }

        private void OnColorPaletteEntryChanged(IColorPaletteEntry oldValue, IColorPaletteEntry newValue)
        {
            if (oldValue != null)
            {
                oldValue.ActiveColorChanged -= ColorPaletteEntry_ActiveColorChanged;
            }

            if (newValue != null)
            {
                ColorPicker.Color = newValue.ActiveColor;
                newValue.ActiveColorChanged += ColorPaletteEntry_ActiveColorChanged;

                if (string.IsNullOrEmpty(newValue.Title))
                {
                    TitleField.IsVisible = false;
                    TitleField.Text = string.Empty;
                }
                else
                {
                    TitleField.IsVisible = true;
                    TitleField.Text = newValue.Title;
                }
                if (string.IsNullOrEmpty(newValue.Description))
                {
                    DescriptionField.IsVisible = false;
                    DescriptionField.Text = string.Empty;
                }
                else
                {
                    DescriptionField.IsVisible = true;
                    DescriptionField.Text = newValue.Description;
                }
                if (newValue is EditableColorPaletteEntry editableNewValue)
                {
                    RevertButton.IsVisible = true;
                    RevertButton.IsEnabled = editableNewValue.UseCustomColor;
                }
                else
                {
                    RevertButton.IsVisible = false;
                }

                if (newValue.ContrastColors != null)
                {
                    List<ContrastListItem> contrastList = new List<ContrastListItem>();

                    foreach (var c in newValue.ContrastColors)
                    {
                        if (c.ShowInContrastList)
                        {
                            if (c.ShowContrastErrors)
                            {
                                contrastList.Add(new ContrastListItem(newValue, c.Color));
                            }
                            else
                            {
                                contrastList.Add(new ContrastListItem(newValue, c.Color, 0));
                            }

                        }
                    }

                    SetValue(ContrastListProperty, contrastList);
                }
                else
                {
                    SetValue(ContrastListProperty, null);
                }
            }
            else
            {
                ColorPicker.Color = default(Color);
                SetValue(ContrastListProperty, null);
            }
        }

        public IColorPaletteEntry ColorPaletteEntry
        {
            get { return GetValue(ColorPaletteEntryProperty) as IColorPaletteEntry; }
            set { SetValue(ColorPaletteEntryProperty, value); }
        }

        #endregion

        #region ContrastListProperty

        public static readonly StyledProperty<List<ContrastListItem>> ContrastListProperty = AvaloniaProperty.Register<ExpandedColorPaletteEntryEditor, List<ContrastListItem>>("ContrastList");

        public List<ContrastListItem> ContrastList
        {
            get { return GetValue(ContrastListProperty) as List<ContrastListItem>; }
        }

        #endregion

        private void ColorPaletteEntry_ActiveColorChanged(IColorPaletteEntry obj)
        {
            if (obj is EditableColorPaletteEntry editablePaletteEntry)
            {
                RevertButton.IsEnabled = editablePaletteEntry.UseCustomColor;
            }
            if (obj != null)
            {
                ColorPicker.Color = obj.ActiveColor;
            }
        }

        private void ColorPicker_ColorChanged(object sender, ColorChangedEventArgs args)
        {
            // In this case it is easier to deal with an event than the loopbacks that the ColorPicker creates with a two way binding
            var paletteEntry = ColorPaletteEntry;
            if (paletteEntry != null)
            {
                paletteEntry.ActiveColor = args.NewColor;
            }
        }

        private void RevertButton_Click(object sender, RoutedEventArgs e)
        {
            var paletteEntry = ColorPaletteEntry;
            if (paletteEntry is EditableColorPaletteEntry editablePaletteEntry)
            {
                editablePaletteEntry.UseCustomColor = false;
            }
        }
    }
}
