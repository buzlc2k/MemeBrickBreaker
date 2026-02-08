using UnityEngine;
using System;

namespace BullBrukBruker
{
    [Serializable]
    public class ObjectMovingConfigRecord
    {
        public ObjectID ID;
        public float Speed;
    }

    public class ObjectMovingConfig : ConfigTable<ObjectMovingConfigRecord>
    {
        public override ConfigRecordComparer<ObjectMovingConfigRecord> DefineConfigComparer()
        {
            recordComparer = new ConfigRecordComparer<ObjectMovingConfigRecord>("ID");
            return recordComparer; 
        }
    }   
}