﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Avalonia.Platform;
using FluentEditor.ControlPalette;
using FluentEditor.ControlPalette.Model;
using FluentEditor.OuterNav;
using FluentEditorShared;
using FluentEditorShared.Utils;

namespace FluentEditor.Model
{
    public interface IMainNavModel
    {
        Task InitializeData(string dataPath, IControlPaletteModel paletteModel, IControlPaletteExportProvider controlPaletteExportProvider);
        Task HandleAppSuspend();

        IReadOnlyList<INavItem> NavItems { get; }
        INavItem DefaultNavItem { get; }
    }

    public class MainNavModel : IMainNavModel
    {
        public MainNavModel(IStringProvider stringProvider)
        {
            _stringProvider = stringProvider;
        }

        private IStringProvider _stringProvider;

        public async Task InitializeData(string dataPath, IControlPaletteModel paletteModel, IControlPaletteExportProvider controlPaletteExportProvider)
        {
            // Use Task.Run to ensure the method runs asynchronously
            await Task.Run(() =>
            {
                var asset = AssetLoader.Open(new Uri(dataPath));
                var rootObject = JsonObject.Parse(asset).AsObject();

                List<INavItem> navItems = new List<INavItem>();

                if (rootObject.ContainsKey("Demos"))
                {
                    JsonArray demoDataList = rootObject["Demos"].AsArray();
                    foreach (var demoData in demoDataList)
                    {
                        navItems.Add(ParseNavItem(demoData.AsObject(), paletteModel, controlPaletteExportProvider));
                    }
                }

                string defaultDemoId = rootObject.GetOptionalString("DefaultDemoId");
                if (!string.IsNullOrEmpty(defaultDemoId))
                {
                    _defaultNavItem = navItems.FirstOrDefault(a => a.Id == defaultDemoId);
                }

                _navItems = navItems;
            });
        }

        public Task HandleAppSuspend()
        {
            // Currently nothing to do here
            return Task.CompletedTask;
        }

        private INavItem ParseNavItem(JsonObject data, IControlPaletteModel paletteModel, IControlPaletteExportProvider controlPaletteExportProvider)
        {
            string type = data.GetOptionalString("Type");

            switch (type)
            {
                case "ControlPalette":
                    return ControlPaletteViewModel.Parse(_stringProvider, data, paletteModel, controlPaletteExportProvider);
                default:
                    throw new Exception(string.Format("Unknown nav item type {0}", type));
            }
        }

        private List<INavItem> _navItems;
        public IReadOnlyList<INavItem> NavItems => _navItems;

        private INavItem _defaultNavItem;
        public INavItem DefaultNavItem => _defaultNavItem;
    }
}
