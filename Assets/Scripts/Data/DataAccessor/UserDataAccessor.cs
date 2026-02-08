using System;
using Firebase.Database;

namespace BullBrukBruker
{
    public class UserDataAccessor : UserSpecificDataAccessor<UserDTO>
    {
        public UserDataAccessor(string userID)
        {
            dataNode = RootDataNodes.UserNode;
            this.userID = userID;
            nodeRef = FirebaseDatabase.DefaultInstance.RootReference.Child(dataNode);
            userNodeRef = nodeRef.Child(userID);
        }

        protected override UserDTO CreateDefaultData()
        {
            return new UserDTO
            {
                TimeCreated = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Score = 0,
            };
        }
    }
}