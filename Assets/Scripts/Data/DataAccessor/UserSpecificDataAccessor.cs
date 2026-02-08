using System;
using UnityEngine;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections;
using Firebase.Database;

namespace BullBrukBruker
{
    public abstract class UserSpecificDataAccessor<DTO> : IUserSpecificDataAccessor where DTO : class
    {
        protected string dataNode;
        protected string userID;
        protected DatabaseReference nodeRef;
        protected DatabaseReference userNodeRef;
        protected DTO data;

        protected abstract DTO CreateDefaultData();

        public virtual IEnumerator LoadData()
        {
            var getValueTask = userNodeRef.GetValueAsync();
            yield return new YieldTask(getValueTask);

            DataSnapshot snapshot = getValueTask.Result;
            string jsonString = snapshot.GetRawJsonValue();

            if (!string.IsNullOrEmpty(jsonString))
            {
                data = JsonUtility.FromJson<DTO>(jsonString);
                yield return null;
            }
            else
            {
                data = CreateDefaultData();
                yield return DataManager.Instance.StartCoroutine(SaveData());
            }
        }

        public virtual T Read<T>(Expression<Func<DTO, T>> fieldSelector)
        {
            var func = fieldSelector.Compile();

            return func(data);
        }

        public virtual void Write<T>(Expression<Func<DTO, T>> fieldSelector, T overridedValue)
        {
            if (fieldSelector.Body is MemberExpression memberExpression
                && memberExpression.Member is FieldInfo fieldInfo)
            {
                fieldInfo.SetValue(data, overridedValue);
                DataManager.Instance.StartCoroutine(SaveFieldData(fieldInfo, overridedValue));
            }
        }

        protected virtual IEnumerator SaveFieldData<T>(FieldInfo fieldInfo, T overridedValue)
        {
            var fieldRef = userNodeRef.Child(fieldInfo.Name);
            yield return new YieldTask(FirebaseDatabaseUtils.SetValueAsync(fieldRef, overridedValue));
        }

        public virtual IEnumerator SaveData()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
                yield break;
            yield return new YieldTask(FirebaseDatabaseUtils.SetValueAsync(userNodeRef, data));
        }
    }
}