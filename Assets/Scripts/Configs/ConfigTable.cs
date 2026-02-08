using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

namespace BullBrukBruker
{
    public class ConfigRecordComparer<T> : IComparer<T> where T : class, new()
    {
        private readonly List<FieldInfo> keyInfos = new();

        public ConfigRecordComparer(params string[] keyInfoNames) // ConfigCompareKey("a","b","c")
        {
            for (int i = 0; i < keyInfoNames.Length; i++)
            {
                FieldInfo keyInfo = typeof(T).GetField(keyInfoNames[i], BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                keyInfos.Add(keyInfo);
            }
        }

        public int Compare(T x, T y)
        {
            int result = 0;
            for (int i = 0; i < keyInfos.Count; i++)
            {
                object val_x = keyInfos[i].GetValue(x);
                object val_y = keyInfos[i].GetValue(y);

                result = ((IComparable)val_x).CompareTo(val_y);

                if (result != 0)
                {
                    break;
                }
            }

            return result;
        }

        public T SetValueSearch(params object[] value)
        {
            T key = new();

            for (int i = 0; i < value.Length; i++)
                keyInfos[i].SetValue(key, value[i]);

            return key;
        }
    }

    public interface IAddRecordToTable
    {
        public void Add(object record);
    }

    public abstract class ConfigTable<T> : ScriptableObject, IAddRecordToTable where T : class, new()
    {
        [SerializeField, SerializeReference] protected internal List<T> Records = new();
        protected ConfigRecordComparer<T> recordComparer;

        public abstract ConfigRecordComparer<T> DefineConfigComparer();

        public void OnEnable()
        {
            DefineConfigComparer();
        }

        public void Add(object record)
        {
            DefineConfigComparer();

            Records ??= new();
            Records.Add(record as T);
            Records.Sort(recordComparer);
        }

        public List<T> GetAllRecord() => Records;

        public T GetRecordByKeySearch(params object[] key)
        {
            T objectkey = recordComparer.SetValueSearch(key);

            int index = Records.BinarySearch(objectkey, recordComparer);

            if (index >= 0 && index < Records.Count)
                return Records[index];
            else
                return null;
        }
    }
}