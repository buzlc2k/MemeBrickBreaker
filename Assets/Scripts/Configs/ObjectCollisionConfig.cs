using System;

namespace BullBrukBruker
{
    [Serializable]
    public class ObjectCollisionConfigRecord
    {
        public ObjectID ID;
        public float Width;
        public float Height;
        public float RayLength;
    }

    [Serializable]
    public class BallCollisionConfigRecord : ObjectCollisionConfigRecord
    {
        public float AddedBouncePerAngle;
    }

    public class ObjectCollisionConfig : ConfigTable<ObjectCollisionConfigRecord>
    {
        public override ConfigRecordComparer<ObjectCollisionConfigRecord> DefineConfigComparer()
        {
            recordComparer = new ConfigRecordComparer<ObjectCollisionConfigRecord>("ID");
            return recordComparer;
        }
    }   
}