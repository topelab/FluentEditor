// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace FluentEditorShared
{
    public interface IStringProvider
    {
        string GetString(string id);
    }

    public class StringProvider : IStringProvider
    {
        public string GetString(string id)
        {
            return Resources.Resources.ResourceManager.GetString(id) ?? id;
        }
    }
}
