using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Reflection;

namespace BullBrukBruker
{
#if UNITY_EDITOR
    public static class ConfigTableMaker
    {
        [MenuItem("Assets/Tools/CovertToScriptableObject", false, 1)]
        private static void CreateConfigTable()
        {
            foreach (UnityEngine.Object csvObj in Selection.objects)
            {
                if (csvObj is not TextAsset)
                {
                    Debug.Log("====== Invalid File Format ======");
                    return;
                }

                TextAsset csvFile = (TextAsset)csvObj;
                //Name CSV File is the same as ConfigTable Implementation Type
                string tableName = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(csvFile));

                ScriptableObject configTable = ScriptableObject.CreateInstance(tableName);
                if (configTable == null) return;

                string configTablePath = "Assets/Resources/Configs/" + tableName + ".asset";
                if (AssetDatabase.LoadAssetAtPath<ScriptableObject>(configTablePath) != null)
                    AssetDatabase.DeleteAsset(configTablePath);
                AssetDatabase.CreateAsset(configTable, configTablePath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                //Get RecordType in Implementation Config Table
                foreach (var record in CSVConverter.CovertToObjs(csvFile))
                    ((IAddRecordToTable)configTable).Add(record);

                EditorUtility.SetDirty(configTable);
                Debug.Log("====== CREATE CONFIG TABLE SUCCESSS ======");
            }
        }
    }
#endif
}