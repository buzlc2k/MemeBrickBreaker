using System;
using System.Collections.Generic;
using System.Reflection;

namespace BullBrukBruker
{
    public static class ReflectionExtension
    {
        public static FieldInfo[] GetSortedFieldInfo(Type type)
        {
            List<FieldInfo> fieldInfos = new();

            var baseTypes = GetBaseTypes(type);
            for (int i = baseTypes.Count - 1; i >= 0; i--)
            {
                FieldInfo[] fields = baseTypes[i].GetFields(
                                                BindingFlags.Instance
                                                | BindingFlags.Public
                                                | BindingFlags.NonPublic
                                                | BindingFlags.DeclaredOnly);
                fieldInfos.AddRange(fields);
            }

            return fieldInfos.ToArray();
        }

        public static List<Type> GetBaseTypes(Type type)
        {
            List<Type> baseTypes = new() { type };

            while (type.BaseType != null)
            {
                type = type.BaseType;
                baseTypes.Add(type);
            }

            return baseTypes;
        }
    }
}