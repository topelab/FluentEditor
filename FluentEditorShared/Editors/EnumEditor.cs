// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml.Templates;

namespace FluentEditorShared.Editors
{
    public class EnumEditor : TemplatedControl
    {
        private ContentPresenter _headerContentPresenter;
        
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _headerContentPresenter = e.NameScope.Find<ContentPresenter>("HeaderContentPresenter");

            var h = Header;
            if (h != null)
            {
                _headerContentPresenter.IsVisible = h != null;
            }
        }

        // Dependency properties can safely be assumed to be single threaded so this is sufficient for dealing with loopbacks on property changed
        private bool _internalValueChanging = false;

        public T GetSelectedValue<T>() where T : struct
        {
            T output;
            if (Enum.TryParse<T>(SelectedString, out output))
            {
                return output;
            }
            return default(T);
        }

        public void SetSelectedValue<T>(T value) where T : struct
        {
            var e = EnumType;
            if (e == null || typeof(T) != EnumType)
            {
                return;
            }
            SelectedString = value.ToString();
        }

        #region EnumTypeProperty

        public static readonly StyledProperty<Type> EnumTypeProperty = AvaloniaProperty.Register<EnumEditor, Type>("EnumType");
        
        private void OnEnumTypeChanged(Type oldValue, Type newValue)
        {
            if (!_internalValueChanging)
            {
                _internalValueChanging = true;

                if (newValue == null)
                {
                    AvailableStrings = null;
                }
                else
                {
                    try
                    {
                        AvailableStrings = Enum.GetNames(newValue);
                    }
                    catch
                    {
                        AvailableStrings = null;
                    }
                }

                string s = SelectedString;
                if (s != null)
                {
                    var a = AvailableStrings;
                    if (!a.Contains(s))
                    {
                        SelectedString = null;
                        SelectedBoxedEnum = null;
                    }
                }

                _internalValueChanging = false;
            }
        }

        public Type EnumType
        {
            get => GetValue(EnumTypeProperty);
            set => SetValue(EnumTypeProperty, value);
        }

        #endregion

        #region AvailableStringsProperty

        public static readonly StyledProperty<string[]> AvailableStringsProperty = AvaloniaProperty.Register<EnumEditor, string[]>("AvailableStrings");

        public string[] AvailableStrings
        {
            get => GetValue(AvailableStringsProperty);
            set => SetValue(AvailableStringsProperty, value);
        }
        #endregion

        #region SelectedString

        public static readonly StyledProperty<string> SelectedStringProperty = AvaloniaProperty.Register<EnumEditor, string>("SelectedString");
        
        private void OnSelectedStringChanged(string oldValue, string newValue)
        {
            if (!_internalValueChanging)
            {
                _internalValueChanging = true;

                if (newValue != null)
                {
                    var a = AvailableStrings;
                    if (!a.Contains(newValue))
                    {
                        SelectedString = null;
                        SelectedBoxedEnum = null;
                    }
                    else
                    {
                        SelectedBoxedEnum = Enum.Parse(EnumType, newValue);
                    }
                }
                else
                {
                    SelectedBoxedEnum = null;
                }

                _internalValueChanging = false;
            }
        }

        public string SelectedString
        {
            get => GetValue(SelectedStringProperty);
            set => SetValue(SelectedStringProperty, value);
        }

        #endregion

        #region SelectedBoxedEnumProperty

        public static readonly StyledProperty<object> SelectedBoxedEnumProperty = AvaloniaProperty.Register<EnumEditor, object>("SelectedBoxedEnum");
        
        private void OnSelectedBoxedEnumChanged(object oldValue, object newValue)
        {
            if (!_internalValueChanging)
            {
                _internalValueChanging = true;

                var t = EnumType;
                if (newValue == null || newValue.GetType() != t)
                {
                    SelectedString = null;
                }
                else
                {
                    SelectedString = newValue.ToString();
                }

                _internalValueChanging = false;
            }
        }

        public object SelectedBoxedEnum
        {
            get => GetValue(SelectedBoxedEnumProperty);
            set => SetValue(SelectedBoxedEnumProperty, value);
        }

        #endregion

        #region HeaderProperty

        public static readonly StyledProperty<object> HeaderProperty = AvaloniaProperty.Register<EnumEditor, object>("Header");
        
        private void OnHeaderChanged(object oldVal, object newVal)
        {
            _headerContentPresenter.IsVisible = newVal != null;
        }

        public object Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        #endregion

        #region HeaderTemplateProperty

        public static readonly StyledProperty<DataTemplate> HeaderTemplateProperty = AvaloniaProperty.Register<EnumEditor, DataTemplate>("HeaderTemplate");

        public DataTemplate HeaderTemplate
        {
            get => GetValue(HeaderTemplateProperty);
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
            else if (change.Property == SelectedBoxedEnumProperty)
            {
                var (oldValue, newValue) = change.GetOldAndNewValue<object>();
                OnSelectedBoxedEnumChanged(oldValue, newValue);
            }
            else if (change.Property == SelectedStringProperty)
            {
                var (oldValue, newValue) = change.GetOldAndNewValue<string>();
                OnSelectedStringChanged(oldValue, newValue);
            }
            else if (change.Property == EnumTypeProperty)
            {
                var (oldValue, newValue) = change.GetOldAndNewValue<Type>();
                OnEnumTypeChanged(oldValue, newValue);
            }
        }
    }
}
