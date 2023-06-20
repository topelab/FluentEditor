// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Globalization;
using Avalonia.Media;
using System.Numerics;
using System.Text.Json.Nodes;

namespace FluentEditorShared.Utils
{
    public static class JsonExtensionMethods
    {
        public static string GetOptionalString(this JsonObject data, string key)
        {
            if (data == null || string.IsNullOrEmpty(key))
            {
                return null;
            }
            if (data.ContainsKey(key))
            {
                return data[key].GetOptionalString();
            }
            else
            {
                return null;
            }
        }

        public static string GetOptionalString(this JsonNode data)
        {
            if (data == null)
            {
                return null;
            }
            if (data is JsonValue value
                && value.TryGetValue<string>(out var val))
            {
                return val;
            }
            return data.ToString();
        }

        public static int GetInt(this JsonNode data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (data is JsonValue value)
            {
                if (value.TryGetValue<string>(out var val))
                {
                    int retVal;
                    if (!int.TryParse(val, out retVal))
                    {
                        throw new Exception("JsonObject is type string but cannot be parsed to int");
                    }

                    return retVal;
                }
                else if (value.TryGetValue<int>(out var number))
                {
                    return number;
                }
            }

            throw new InvalidOperationException();
        }

        public static bool TryGetInt(this JsonNode data, out int retVal)
        {
            if (data == null)
            {
                retVal = default(int);
                return false;
            }
            if (data is JsonValue value)
            {
                if (value.TryGetValue<string>(out var val))
                {
                    if (!int.TryParse(val, out retVal))
                    {
                        throw new Exception("JsonObject is type string but cannot be parsed to int");
                    }

                    return true;
                }
                else if (value.TryGetValue<int>(out retVal))
                {
                    return true;
                }
            }

            retVal = default;
            return false;
        }

        public static bool TryGetFloat(this JsonNode data, out float retVal)
        {
            if (data == null)
            {
                retVal = default(float);
                return false;
            }
            if (data is JsonValue value)
            {
                if (value.TryGetValue<string>(out var val))
                {
                    if (!float.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out retVal))
                    {
                        throw new Exception("JsonObject is type string but cannot be parsed to int");
                    }

                    return true;
                }
                else if (value.TryGetValue<float>(out retVal))
                {
                    return true;
                }
            }
            retVal = default(float);
            return false;
        }

        public static Vector2 GetVector2(this JsonObject data)
        {
            if (data == null)
            {
                return default(Vector2);
            }
            float x = 0f, y = 0f;

            JsonValue xnode = null;
            if (data.ContainsKey("X"))
            {
                xnode = data["X"].AsValue();
            }
            else if (data.ContainsKey("x"))
            {
                xnode = data["x"].AsValue();
            }
            if (xnode != null)
            {
                if (!xnode.TryGetFloat(out x))
                {
                    x = 0f;
                }
            }

            JsonValue ynode = null;
            if (data.ContainsKey("Y"))
            {
                ynode = data["Y"].AsValue();
            }
            else if (data.ContainsKey("y"))
            {
                ynode = data["y"].AsValue();
            }
            if (ynode != null)
            {
                if (!ynode.TryGetFloat(out y))
                {
                    y = 0f;
                }
            }

            return new Vector2(x, y);
        }

        public static Vector3 GetVector3(this JsonObject data)
        {
            if (data == null)
            {
                return default(Vector3);
            }
            float x = 0f, y = 0f, z = 0f;

            JsonValue xnode = null;
            if (data.ContainsKey("X"))
            {
                xnode = data["X"].AsValue();
            }
            else if (data.ContainsKey("x"))
            {
                xnode = data["x"].AsValue();
            }
            if (xnode != null)
            {
                if (!xnode.TryGetFloat(out x))
                {
                    x = 0f;
                }
            }

            JsonValue ynode = null;
            if (data.ContainsKey("Y"))
            {
                ynode = data["Y"].AsValue();
            }
            else if (data.ContainsKey("y"))
            {
                ynode = data["y"].AsValue();
            }
            if (ynode != null)
            {
                if (!ynode.TryGetFloat(out y))
                {
                    y = 0f;
                }
            }

            JsonValue znode = null;
            if (data.ContainsKey("Z"))
            {
                znode = data["Z"].AsValue();
            }
            else if (data.ContainsKey("z"))
            {
                znode = data["z"].AsValue();
            }
            if (znode != null)
            {
                if (!znode.TryGetFloat(out z))
                {
                    z = 0f;
                }
            }

            return new Vector3(x, y, z);
        }

        public static Color GetColor(this JsonNode data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (data is JsonValue value
                && value.TryGetValue<string>(out var str))
            {
                Color retVal;
                if (!FluentEditorShared.Utils.ColorUtils.TryParseColorString(str, out retVal))
                {
                    throw new Exception("JsonObject is type string but cannot be parsed to Color");
                }
                return retVal;
            }

            throw new InvalidOperationException();
        }

        public static T GetEnum<T>(this JsonNode data) where T : struct
        {
            string dataString = data.GetOptionalString();
            if (Enum.TryParse<T>(dataString, out T retVal))
            {
                return retVal;
            }
            else
            {
                throw new Exception(string.Format("Unable to parse {0} into enum of type {1}", dataString, typeof(T).ToString()));
            }
        }
    }
}
