// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Avalonia.Controls;

namespace FluentEditor.OuterNav
{
    public sealed partial class OuterNavPage : UserControl
    {
        public OuterNavPage(OuterNavViewModel viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException("viewModel");
            }
            _viewModel = viewModel;
            _viewModel.NavigateToItem += _viewModel_NavigateToItem;

            this.InitializeComponent();

            NavigateToViewModel(_viewModel.SelectedNavItem);
        }

        private readonly OuterNavViewModel _viewModel;
        public OuterNavViewModel ViewModel { get { return _viewModel; } }

        private void _viewModel_NavigateToItem(OuterNavViewModel source, INavItem navItem)
        {
            NavigateToViewModel(navItem);
        }

        private void NavigateToViewModel(object viewModel)
        {
            NavigationFrame.Content = viewModel;
        }
    }
}
