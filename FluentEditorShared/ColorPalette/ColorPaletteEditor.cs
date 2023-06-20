// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace FluentEditorShared.ColorPalette
{
    public class ColorPaletteEditor : TemplatedControl
    {
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
            get => GetValue(ColorPaletteProperty);
            set => SetValue(ColorPaletteProperty, value);
        }

        #endregion

        #region PaletteEntriesProperty

        public static readonly StyledProperty<IReadOnlyList<IColorPaletteEntry>> PaletteEntriesProperty = AvaloniaProperty.Register<ColorPaletteEditor, IReadOnlyList<IColorPaletteEntry>>("PaletteEntries");

        public IReadOnlyList<IColorPaletteEntry> PaletteEntries => GetValue(PaletteEntriesProperty);

        #endregion

        #region IsExpandedProperty

        public static readonly StyledProperty<bool> IsExpandedProperty = AvaloniaProperty.Register<ColorPaletteEditor, bool>("IsExpanded", true);

        public bool IsExpanded
        {
            get => GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
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
            else if (change.Property == IsExpandedProperty)
            {
                PseudoClasses.Set(":expanded", IsExpanded);
            }
        }
    }
}
