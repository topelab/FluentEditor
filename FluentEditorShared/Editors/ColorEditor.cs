// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;
using FluentEditorShared.Utils;

namespace FluentEditorShared.Editors
{
    public class ColorEditor : TemplatedControl
    {
        private ContentPresenter _headerPresenter;
        private Control _alphaInputBox;
        
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _alphaInputBox = e.NameScope.Find<Control>("AlphaInputBox")!;
            _headerPresenter = e.NameScope.Find<ContentPresenter>("HeaderContentPresenter")!;

            if (Header is {} header)
            {
                _headerPresenter.IsVisible = header is not null;
            }
        }

        // Dependency properties can safely be assumed to be single threaded so this is sufficient for dealing with loopbacks on property changed
        private bool _internalValueChanging = false;

        private void UpdateProperties(Color color)
        {
            ValueString = Utils.ColorUtils.FormatColorString(color, ColorStringFormat, Precision);
            var b = ValueBrush;
            if (b == null)
            {
                ValueBrush = new SolidColorBrush(color);
            }
            else
            {
                b.Color = color;
            }
            ValueRed = color.R;
            ValueRedString = color.R.ToString();
            ValueGreen = color.G;
            ValueGreenString = color.G.ToString();
            ValueBlue = color.B;
            ValueBlueString = color.B.ToString();
            if (UseAlpha)
            {
                ValueAlpha = color.A;
                ValueAlphaString = color.A.ToString();
            }
            else
            {
                ValueAlpha = 255;
                ValueAlphaString = "255";
            }
        }

        #region ValueColor

        public static readonly StyledProperty<Color> ValueColorProperty = AvaloniaProperty.Register<ColorEditor, Color>("ValueColor", Colors.Black);

        private void OnValueColorChanged(Color oldValue, Color newValue)
        {
            if (!_internalValueChanging)
            {
                _internalValueChanging = true;

                UpdateProperties(newValue);

                _internalValueChanging = false;
            }
        }

        public Color ValueColor
        {
            get => GetValue(ValueColorProperty);
            set => SetValue(ValueColorProperty, value);
        }

        #endregion

        #region ValueStringProperty

        public static readonly StyledProperty<string> ValueStringProperty = AvaloniaProperty.Register<ColorEditor, string>("ValueString", "000000");

        private void OnValueStringChanged(string oldValue, string newValue)
        {
            if (!_internalValueChanging)
            {
                Color.TryParse(newValue, out var c);

                _internalValueChanging = true;

                UpdateProperties(c);

                _internalValueChanging = false;
            }
        }

        public string ValueString
        {
            get => GetValue(ValueStringProperty) as string;
            set => SetValue(ValueStringProperty, value);
        }

        #endregion

        #region ValueBrushProperty

        public static readonly StyledProperty<SolidColorBrush> ValueBrushProperty = AvaloniaProperty.Register<ColorEditor, SolidColorBrush>("ValueBrush", new SolidColorBrush(Colors.Black));

        private void OnValueBrushChanged(SolidColorBrush oldValue, SolidColorBrush newValue)
        {
            if (!_internalValueChanging)
            {
                _internalValueChanging = true;
                if (newValue == null)
                {
                    ValueBrush = new SolidColorBrush(ValueColor);
                }
                else
                {
                    var b = ValueBrush;
                    if (b != null)
                    {
                        UpdateProperties(b.Color);
                    }
                }

                _internalValueChanging = false;
            }
        }

        public SolidColorBrush ValueBrush
        {
            get => GetValue(ValueBrushProperty) as SolidColorBrush;
            set => SetValue(ValueBrushProperty, value);
        }

        #endregion

        #region ValueRed

        public static readonly StyledProperty<byte> ValueRedProperty = AvaloniaProperty.Register<ColorEditor, byte>("ValueRed");

        private void OnValueRedChanged(byte oldValue, byte newValue)
        {
            if (!_internalValueChanging)
            {
                _internalValueChanging = true;

                var oldColor = ValueColor;
                UpdateProperties(Color.FromArgb(oldColor.A, newValue, oldColor.G, oldColor.B));

                _internalValueChanging = false;
            }
        }

        public byte ValueRed
        {
            get => GetValue(ValueRedProperty);
            set => SetValue(ValueRedProperty, value);
        }

        #endregion

        #region ValueRedString

        public static readonly StyledProperty<string> ValueRedStringProperty = AvaloniaProperty.Register<ColorEditor, string>("ValueRedString", "0");

        private void OnValueRedStringChanged(string oldValue, string newValue)
        {
            if (!_internalValueChanging)
            {
                if (string.IsNullOrEmpty(newValue))
                {
                    return;
                }

                _internalValueChanging = true;
                byte newByte;
                if (byte.TryParse(newValue, out newByte))
                {
                    var oldColor = ValueColor;
                    UpdateProperties(Color.FromArgb(oldColor.A, newByte, oldColor.G, oldColor.B));
                }

                _internalValueChanging = false;
            }
        }

        public string ValueRedString
        {
            get => GetValue(ValueRedStringProperty) as string;
            set => SetValue(ValueRedStringProperty, value);
        }

        #endregion

        #region ValueGreen

        public static readonly StyledProperty<byte> ValueGreenProperty = AvaloniaProperty.Register<ColorEditor, byte>("ValueGreen");

        private void OnValueGreenChanged(byte oldValue, byte newValue)
        {
            if (!_internalValueChanging)
            {
                _internalValueChanging = true;

                var oldColor = ValueColor;
                UpdateProperties(Color.FromArgb(oldColor.A, oldColor.R, newValue, oldColor.B));

                _internalValueChanging = false;
            }
        }

        public byte ValueGreen
        {
            get => GetValue(ValueGreenProperty);
            set => SetValue(ValueGreenProperty, value);
        }

        #endregion

        #region ValueGreenString

        public static readonly StyledProperty<string> ValueGreenStringProperty = AvaloniaProperty.Register<ColorEditor, string>("ValueGreenString", "0");

        private void OnValueGreenStringChanged(string oldValue, string newValue)
        {
            if (!_internalValueChanging)
            {
                if (string.IsNullOrEmpty(newValue))
                {
                    return;
                }

                _internalValueChanging = true;
                byte newByte;
                if (byte.TryParse(newValue, out newByte))
                {
                    var oldColor = ValueColor;
                    UpdateProperties(Color.FromArgb(oldColor.A, oldColor.R, newByte, oldColor.B));
                }

                _internalValueChanging = false;
            }
        }

        public string ValueGreenString
        {
            get => GetValue(ValueGreenStringProperty) as string;
            set => SetValue(ValueGreenStringProperty, value);
        }

        #endregion

        #region ValueBlue

        public static readonly StyledProperty<byte> ValueBlueProperty = AvaloniaProperty.Register<ColorEditor, byte>("ValueBlue");

        private void OnValueBlueChanged(byte oldValue, byte newValue)
        {
            if (!_internalValueChanging)
            {
                _internalValueChanging = true;

                var oldColor = ValueColor;
                UpdateProperties(Color.FromArgb(oldColor.A, oldColor.R, oldColor.G, newValue));

                _internalValueChanging = false;
            }
        }

        public byte ValueBlue
        {
            get => GetValue(ValueBlueProperty);
            set => SetValue(ValueBlueProperty, value);
        }

        #endregion

        #region ValueBlueString

        public static readonly StyledProperty<string> ValueBlueStringProperty = AvaloniaProperty.Register<ColorEditor, string>("ValueBlueString", "0");

        private void OnValueBlueStringChanged(string oldValue, string newValue)
        {
            if (!_internalValueChanging)
            {
                if (string.IsNullOrEmpty(newValue))
                {
                    return;
                }

                _internalValueChanging = true;
                byte newByte;
                if (byte.TryParse(newValue, out newByte))
                {
                    var oldColor = ValueColor;
                    UpdateProperties(Color.FromArgb(oldColor.A, oldColor.R, oldColor.G, newByte));
                }

                _internalValueChanging = false;
            }
        }

        public string ValueBlueString
        {
            get => GetValue(ValueBlueStringProperty) as string;
            set => SetValue(ValueBlueStringProperty, value);
        }

        #endregion

        #region ValueAlpha

        public static readonly StyledProperty<byte> ValueAlphaProperty = AvaloniaProperty.Register<ColorEditor, byte>("ValueAlpha", 255);

        private void OnValueAlphaChanged(byte oldValue, byte newValue)
        {
            if (!_internalValueChanging)
            {
                _internalValueChanging = true;

                var oldColor = ValueColor;
                UpdateProperties(Color.FromArgb(newValue, oldColor.R, oldColor.G, oldColor.B));

                _internalValueChanging = false;
            }
        }

        public byte ValueAlpha
        {
            get => GetValue(ValueAlphaProperty);
            set => SetValue(ValueAlphaProperty, value);
        }

        #endregion

        #region ValueAlphaString

        public static readonly StyledProperty<string> ValueAlphaStringProperty = AvaloniaProperty.Register<ColorEditor, string>("ValueAlphaString", "255");

        private void OnValueAlphaStringChanged(string oldValue, string newValue)
        {
            if (!_internalValueChanging)
            {
                if (string.IsNullOrEmpty(newValue))
                {
                    return;
                }

                _internalValueChanging = true;
                byte newByte;
                if (byte.TryParse(newValue, out newByte))
                {
                    var oldColor = ValueColor;
                    UpdateProperties(Color.FromArgb(newByte, oldColor.R, oldColor.G, oldColor.B));
                }

                _internalValueChanging = false;
            }
        }

        public string ValueAlphaString
        {
            get => GetValue(ValueAlphaStringProperty) as string;
            set => SetValue(ValueAlphaStringProperty, value);
        }

        #endregion

        // could easily add more properties like this for HSL or whatever color space is needed

        #region UseAlphaProperty

        public static readonly StyledProperty<bool> UseAlphaProperty = AvaloniaProperty.Register<ColorEditor, bool>("UseAlpha", true);

        private void OnUseAlphaChanged(bool oldValue, bool newValue)
        {
            if (!_internalValueChanging)
            {
                _internalValueChanging = true;

                if(!newValue)
                {
                    var oldColor = ValueColor;
                    UpdateProperties(Color.FromArgb(255, oldColor.R, oldColor.G, oldColor.B));
                }

                _alphaInputBox.IsVisible = newValue;

                _internalValueChanging = false;
            }
        }

        public bool UseAlpha
        {
            get => GetValue(UseAlphaProperty);
            set => SetValue(UseAlphaProperty, value);
        }

        #endregion

        #region ColorStringFormatProperty

        public static readonly StyledProperty<FluentEditorShared.Utils.ColorStringFormat> ColorStringFormatProperty = AvaloniaProperty.Register<ColorEditor, FluentEditorShared.Utils.ColorStringFormat>("ColorStringFormat", FluentEditorShared.Utils.ColorStringFormat.PoundRGB);

        private void OnColorStringFormatChanged(FluentEditorShared.Utils.ColorStringFormat oldValue, FluentEditorShared.Utils.ColorStringFormat newValue)
        {
            if (!_internalValueChanging)
            {
                _internalValueChanging = true;

                ValueString = Utils.ColorUtils.FormatColorString(ValueColor, newValue, Precision);

                _internalValueChanging = false;
            }
        }

        public FluentEditorShared.Utils.ColorStringFormat ColorStringFormat
        {
            get => GetValue(ColorStringFormatProperty);
            set => SetValue(ColorStringFormatProperty, value);
        }

        #endregion

        #region PrecisionProperty

        public static readonly StyledProperty<int> PrecisionProperty = AvaloniaProperty.Register<ColorEditor, int>("Precision", 4);

        private void OnPrecisionChanged(int oldValue, int newValue)
        {
            if (!_internalValueChanging)
            {
                _internalValueChanging = true;

                ValueString = Utils.ColorUtils.FormatColorString(ValueColor, ColorStringFormat, newValue);

                _internalValueChanging = false;
            }
        }

        public int Precision
        {
            get => GetValue(PrecisionProperty);
            set => SetValue(PrecisionProperty, value);
        }

        #endregion

        #region HeaderProperty

        public static readonly StyledProperty<object> HeaderProperty = AvaloniaProperty.Register<ColorEditor, object>("Header");

        private void OnHeaderChanged(object oldVal, object newVal)
        {
            _headerPresenter!.IsVisible = newVal is not null;
        }

        public object Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        #endregion

        #region HeaderTemplateProperty

        public static readonly StyledProperty<IDataTemplate> HeaderTemplateProperty = AvaloniaProperty.Register<ColorEditor, IDataTemplate>("HeaderTemplate");

        public IDataTemplate HeaderTemplate
        {
            get => GetValue(HeaderTemplateProperty) as DataTemplate;
            set => SetValue(HeaderTemplateProperty, value);
        }

        #endregion

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == HeaderProperty)
            {
                var (oldValue, newValue) = change.GetOldAndNewValue<object>();
                OnHeaderChanged(oldValue, newValue);
            }
            else if (change.Property == PrecisionProperty)
            {
                var (oldValue, newValue) = change.GetOldAndNewValue<int>();
                OnPrecisionChanged(oldValue, newValue);
            }
            else if (change.Property == UseAlphaProperty)
            {
                var (oldValue, newValue) = change.GetOldAndNewValue<bool>();
                OnUseAlphaChanged(oldValue, newValue);
            }
            else if (change.Property == ValueAlphaProperty)
            {
                var (oldValue, newValue) = change.GetOldAndNewValue<byte>();
                OnValueAlphaChanged(oldValue, newValue);
            }
            else if (change.Property == ValueRedProperty)
            {
                var (oldValue, newValue) = change.GetOldAndNewValue<byte>();
                OnValueRedChanged(oldValue, newValue);
            }
            else if (change.Property == ValueGreenProperty)
            {
                var (oldValue, newValue) = change.GetOldAndNewValue<byte>();
                OnValueGreenChanged(oldValue, newValue);
            }
            else if (change.Property == ValueBlueProperty)
            {
                var (oldValue, newValue) = change.GetOldAndNewValue<byte>();
                OnValueBlueChanged(oldValue, newValue);
            }
            else if (change.Property == ColorStringFormatProperty)
            {
                var (oldValue, newValue) = change.GetOldAndNewValue<ColorStringFormat>();
                OnColorStringFormatChanged(oldValue, newValue);
            }
            else if (change.Property == ValueAlphaStringProperty)
            {
                var (oldValue, newValue) = change.GetOldAndNewValue<string>();
                OnValueAlphaStringChanged(oldValue, newValue);
            }
            else if (change.Property == ValueRedStringProperty)
            {
                var (oldValue, newValue) = change.GetOldAndNewValue<string>();
                OnValueRedStringChanged(oldValue, newValue);
            }
            else if (change.Property == ValueGreenStringProperty)
            {
                var (oldValue, newValue) = change.GetOldAndNewValue<string>();
                OnValueGreenStringChanged(oldValue, newValue);
            }
            else if (change.Property == ValueBlueStringProperty)
            {
                var (oldValue, newValue) = change.GetOldAndNewValue<string>();
                OnValueBlueStringChanged(oldValue, newValue);
            }
            else if (change.Property == ValueColorProperty)
            {
                var (oldValue, newValue) = change.GetOldAndNewValue<Color>();
                OnValueColorChanged(oldValue, newValue);
            }
            else if (change.Property == ValueBrushProperty)
            {
                var (oldValue, newValue) = change.GetOldAndNewValue<SolidColorBrush>();
                OnValueBrushChanged(oldValue, newValue);
            }
            else if (change.Property == ValueStringProperty)
            {
                var (oldValue, newValue) = change.GetOldAndNewValue<string>();
                OnValueStringChanged(oldValue, newValue);
            }
        }
    }
}
