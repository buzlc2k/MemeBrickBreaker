using UnityEngine;
using UnityEditor;
using Firebase.Database;
using System.Threading.Tasks;

namespace BullBrukBruker
{
#if UNITY_EDITOR
    public static class DefaultFirebaseTableMaker
    {
        [MenuItem("Assets/Tools/CovertToFirebaseTable", false, 1)]
        private static async Task CreateFirbaseTable()
        {
            foreach (UnityEngine.Object csvObj in Selection.objects)
            {
                if (csvObj is not TextAsset)
                {
                    Debug.Log("====== Invalid File Format ======");
                    return;
                }

                TextAsset csvFile = (TextAsset)csvObj;
                string rootNodeName = csvFile.name.Replace("Table", "");

                int currentIndex = 0;

                foreach (var dto in CSVConverter.CovertToObjs(csvFile))
                {
                    var dbRef = FirebaseDatabase.DefaultInstance.RootReference.Child(rootNodeName).Child($"{rootNodeName}Child_{currentIndex}");
                    await FirebaseDatabaseUtils.SetValueAsync(dbRef, dto);
                    currentIndex++;
                }

                Debug.Log("====== CREATE FIREBASE TABLE SUCCESSS ======");
            }
        }
    }
#endif
}