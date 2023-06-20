// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace FluentEditorShared.ColorPalette
{
    public class ColorPaletteEditor : TemplatedControl
    {
        private double collapsedHeight = 56; // what
        private double expandedHeight = 465;
        public ColorPaletteEditor()
        {
            Height = collapsedHeight;
        }

        #region ColorPaletteProperty

        public static readonly StyledProperty<ColorPalette> ColorPaletteProperty = AvaloniaProperty.Register<ColorPaletteEditor, ColorPalette>("ColorPalette");

        private void OnColorPaletteChanged(ColorPalette oldValue, ColorPalette newValue)
        {
            if(newValue == null)
            {
                SetValue(PaletteEntriesProperty, null);
            }
            else
            {
                SetValue(PaletteEntriesProperty, newValue.Palette);
            }
        }

        public ColorPalette ColorPalette
        {
            get { return GetValue(ColorPaletteProperty) as ColorPalette; }
            set { SetValue(ColorPaletteProperty, value); }
        }

        #endregion

        #region PaletteEntriesProperty

        public static readonly StyledProperty<IReadOnlyList<IColorPaletteEntry>> PaletteEntriesProperty = AvaloniaProperty.Register<ColorPaletteEditor, IReadOnlyList<IColorPaletteEntry>>("PaletteEntries");

        public IReadOnlyList<IColorPaletteEntry> PaletteEntries
        {
            get { return GetValue(PaletteEntriesProperty) as IReadOnlyList<IColorPaletteEntry>; }
        }

        #endregion

        #region IsExpandedProperty

        public static readonly StyledProperty<bool> IsExpandedProperty = AvaloniaProperty.Register<ColorPaletteEditor, bool>("IsExpanded", true);

        public bool IsExpanded
        {
            get
            {
                return (bool)GetValue(IsExpandedProperty);
            }
            set
            {
                AdjustHeight(value);
                SetValue(IsExpandedProperty, value);
            }
        }

        private void AdjustHeight(bool isExpanded)
        {
            if (isExpanded)
                this.Height = expandedHeight;
            else
                this.Height = collapsedHeight;
        }

        #endregion
        
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == ColorPaletteProperty)
            {
                var (oldValue, newValue) = change.GetOldAndNewValue<ColorPalette>();
                OnColorPaletteChanged(oldValue, newValue);
            }
        }
    }
}
