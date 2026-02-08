using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace BullBrukBruker
{
#if UNITY_EDITOR
    public static class CSVConverter
    {
        public static IEnumerable CovertToObjs(TextAsset csvFile)
        {
            List<List<string>> grids = SplitCSVFile(csvFile);

            Assembly assembly = typeof(CSVConverter).Assembly;
            string nameSpace = nameof(BullBrukBruker); // Default Namespace

            for (int i = 0; i < grids.Count; i++)
            {
                List<string> dataLine = grids[i];

                string typeName = dataLine[0];
                Type recordType = assembly.GetType($"{nameSpace}.{typeName}");
                FieldInfo[] fieldInfos = ReflectionExtension.GetSortedFieldInfo(recordType);

                string jsonText = "{";
                for (int x = 0; x < fieldInfos.Length; x++)
                {
                    if (x > 0)
                        jsonText += ",";

                    if (fieldInfos[x].FieldType.IsGenericType && fieldInfos[x].FieldType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        Type elementType = fieldInfos[x].FieldType.GetGenericArguments()[0];
                        string fieldName = fieldInfos[x].Name;
                        string data = dataLine[x + 1];

                        if (elementType == typeof(string))
                        {
                            string[] elements = data.Split(new[] { ", " }, StringSplitOptions.None);
                            string formattedElements = string.Join(", ", elements.Select(e => $"\"{e.Trim()}\""));
                            jsonText += "\"" + fieldName + "\":" + "[" + formattedElements + "]";
                        }
                        else
                            jsonText += "\"" + fieldName + "\":" + "[" + data + "]";
                    }

                    else if (fieldInfos[x].FieldType != typeof(string))
                        jsonText += "\"" + fieldInfos[x].Name + "\":" + dataLine[x + 1];

                    else
                        jsonText += "\"" + fieldInfos[x].Name + "\":\"" + dataLine[x + 1] + "\"";
                }
                jsonText += "}";

                Debug.Log(jsonText);

                yield return JsonUtility.FromJson(jsonText, recordType);
            }
        }

        private static List<List<string>> SplitCSVFile(TextAsset csvFile)
        {
            List<List<string>> grids = new();

            string[] lines = csvFile.text.Split('\n');

            if (lines.Length <= 1) Debug.Log("===== EMPTY CSV FILE =====");

            for (int i = 1; i < lines.Length; i++)
            {
                string s = lines[i];
                if (s.CompareTo(string.Empty) != 0)
                {
                    string pattern = @",(?=(?:[^""]*""[^""]*"")*[^""]*$)";
                    string[] lineData = Regex.Split(s, pattern);
                    List<string> lsLine = new();
                    foreach (string e in lineData)
                    {
                        string newchar = Regex.Replace(e, @"\t|\n|\r", "");
                        newchar = Regex.Replace(newchar, @"""", @"");
                        newchar = Regex.Replace(newchar, @"\\", @"\\");

                        if (!String.IsNullOrEmpty(newchar))
                            lsLine.Add(newchar);
                    }

                    grids.Add(lsLine);
                }
            }

            return grids;
        }
    }
#endif
}