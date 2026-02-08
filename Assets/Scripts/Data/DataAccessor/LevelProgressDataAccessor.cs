using Firebase.Database;

namespace BullBrukBruker
{
    public class LevelProgressDataAccessor : UserSpecificDataAccessor<LevelProgressDTO>
    {
        public LevelProgressDataAccessor(string userID)
        {
            dataNode = RootDataNodes.LevelProgressNode;
            this.userID = userID;
            nodeRef = FirebaseDatabase.DefaultInstance.RootReference.Child(dataNode);
            userNodeRef = nodeRef.Child(userID);
        }

        protected override LevelProgressDTO CreateDefaultData()
        {
            return new LevelProgressDTO
            {
                CurrentLevel = 1,
                HighestLevel = 1,
                StarsPerLevel = new() { 0 },
            };
        }
    }
}