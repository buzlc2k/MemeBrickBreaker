using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using UnityEngine;

namespace BullBrukBruker
{
    public class DataAccessorsManager
    {   
        private static bool hasSetFirebasePersistence = false;

        private string userID;
        private Dictionary<DataID, IUserSpecificDataAccessor> userSpecificDataAccessors;
        private SystemDataAccessor systemDataAccessor;

        public IEnumerator LoadDataAccessors()
        {
            systemDataAccessor = new();

            yield return DataManager.Instance.StartCoroutine(ConnectFirebase());

            yield return DataManager.Instance.StartCoroutine(LoadUser());


            userSpecificDataAccessors = new()
            {
                { DataID.LevelProgress, new LevelProgressDataAccessor(userID) },
                { DataID.User, new UserDataAccessor(userID) },
            };

            foreach (var model in userSpecificDataAccessors.Values)
                yield return DataManager.Instance.StartCoroutine(model.LoadData());
        }

        private IEnumerator ConnectFirebase()
        {
            var checkAndFixDependenciesTask = FirebaseApp.CheckAndFixDependenciesAsync();
            yield return new YieldTask(checkAndFixDependenciesTask);

            if (checkAndFixDependenciesTask.Result != DependencyStatus.Available)
                Debug.Log("CANT CONNECT TO FIREBASE");
            else
                Debug.Log($"CONNECTED TO FIREBASE");

            if (!hasSetFirebasePersistence)
            {
                FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(false);
                hasSetFirebasePersistence = true;
            }

            yield return null;
        }

        private IEnumerator LoadUser()
        {
            if (SaveSystem.HasKey(RootDataNodes.SystemDataNode))
                userID = SaveSystem.GetString(RootDataNodes.SystemDataNode);
            else
                yield return DataManager.Instance.StartCoroutine(CreateNewUser());
        }

        private IEnumerator CreateNewUser()
        {
            var getUserCount = systemDataAccessor.FetchAndUpdateTotalUser();

            yield return new YieldTask(getUserCount);

            int userCount = getUserCount.Result; 
            userID = $"user_{userCount + 1}";

            SaveNewUser();
        }

        private void SaveNewUser()
        {
            SaveSystem.SetString(RootDataNodes.SystemDataNode, userID);
            SaveSystem.SaveToDisk();
        }

        public string GetUserID()
            => userID;

        public DataAccessor GetUserSpecificDataAccessor<DataAccessor>(DataID dataID) where DataAccessor : class
        {
            if (userSpecificDataAccessors.TryGetValue(dataID, out var dataModel))
                if (dataModel is DataAccessor)
                    return dataModel as DataAccessor;

            Debug.Log("Not Found Data Model");
            return null;
        }

        public SystemDataAccessor GetSystemDataAccessor()
            => systemDataAccessor;
    }
}