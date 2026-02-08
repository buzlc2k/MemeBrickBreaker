using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;


namespace BullBrukBruker
{
    public static class JsonUtils
    {
        public static T ForceDeserialize<T>(string json) where T : class
        {
            if (string.IsNullOrEmpty(json))
                return null;

            var jObject = JObject.Parse(json);

            var requiredFields = typeof(T).GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                                        .Select(f => f.Name)
                                        .ToList();

            foreach (var field in requiredFields)
                if (!jObject.ContainsKey(field) || jObject[field].Type == JTokenType.Null)
                    return null;

            var obj = JsonConvert.DeserializeObject<T>(json);

            return obj;
        }
    }
}

