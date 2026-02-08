using UnityEngine;
using System;

namespace BullBrukBruker
{
    [Serializable]
    public class BlockConfigRecord
    {
        public BlockID ID;
        public bool CanCollide;
        public int NumCollide;
    }

    public class BlockConfig : ConfigTable<BlockConfigRecord>
    {
        public override ConfigRecordComparer<BlockConfigRecord> DefineConfigComparer()
        {
            recordComparer = new ConfigRecordComparer<BlockConfigRecord>("ID");
            return recordComparer; 
        }
    }   
}