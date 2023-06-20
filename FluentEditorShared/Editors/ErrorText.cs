// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace FluentEditorShared.Editors
{
    public class ErrorText : TemplatedControl
    {
        public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<ErrorText, string>(
            "Text");

        public string Text
        {
            get => GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly StyledProperty<bool> ShowErrorStateProperty = AvaloniaProperty.Register<ErrorText, bool>(
            "ShowErrorState");

        public bool ShowErrorState
        {
            get => GetValue(ShowErrorStateProperty);
            set => SetValue(ShowErrorStateProperty, value);
        }
        
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            UpdateState();
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == ShowErrorStateProperty)
            {
                UpdateState();
            }
        }

        private void UpdateState()
        {
            PseudoClasses.Set(":active", ShowErrorState);
        }
    }
}
