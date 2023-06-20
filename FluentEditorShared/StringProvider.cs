// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FluentEditorShared
{
    public interface IStringProvider
    {
        string GetString(string id);
    }

    public class StringProvider : IStringProvider
    {
        private readonly Dictionary<string, Func<string>> _properties;

        [DynamicDependency(DynamicallyAccessedMemberTypes.PublicProperties, typeof(Resources.Resources))]
        public StringProvider()
        {
            _properties = typeof(Resources.Resources).GetProperties()
                .ToDictionary(
                    p => p.Name,
                    p => (Func<string>)p.GetMethod!.CreateDelegate(typeof(Func<string>)));
        }
        
        public string GetString(string id)
        {
            return _properties.TryGetValue(id, out var func) ? func() : id;
        }
    }
}
