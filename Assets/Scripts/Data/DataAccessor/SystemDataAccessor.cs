using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using UnityEngine;

namespace BullBrukBruker
{
    public sealed class SystemDataAccessor
    {
        public async Task<int> FetchAndUpdateTotalUser()
        {
            DatabaseReference dbRef = FirebaseDatabase.DefaultInstance.RootReference.Child(RootDataNodes.SystemDataNode).Child(ChildDataNodes.S_DefaultChildNode);
            var snapshot = await dbRef.GetValueAsync();

            SystemDataDTO systemDataDTO = JsonUtility.FromJson<SystemDataDTO>(snapshot.GetRawJsonValue());
            var currentTotal = systemDataDTO.TotalUser;

            systemDataDTO.TotalUser++;
            await FirebaseDatabaseUtils.SetValueAsync(dbRef, systemDataDTO);

            return currentTotal;
        }

        public async Task<List<(string, string)>> FetchTopRankUsers(int num)
        {
            var snapshot = await FirebaseDatabase.DefaultInstance.RootReference.Child(RootDataNodes.UserNode)
                                .OrderByChild(ChildDataNodes.U_Score)
                                .LimitToLast(num)
                                .GetValueAsync();

            return snapshot.Children
                            .Reverse()
                            .Select(result => (
                                result.Key,
                                result.Child(ChildDataNodes.U_Score).Value?.ToString()))
                            .ToList();
        }

        public async Task<string> FetchCurrentRank(string userID, int score)
        {
            var rankingNumber = await FetchCurrentRankingNumber(userID, score);
            if (!String.IsNullOrEmpty(rankingNumber))
                return rankingNumber;

            return await FetchCurrentRankingRange(score);
        }

        private async Task<string> FetchCurrentRankingNumber(string userID, int score)
        {
            var topRankUsers = await FetchTopRankUsers(100);
            
            var user = topRankUsers
                    .Select((item, index) => new { UserId = item.Item1, Score = int.Parse(item.Item2), Index = index })
                    .FirstOrDefault(x => x.Score <= score && x.UserId.Equals(userID));

            return user != null ? (user.Index + 1).ToString() : "";
        }

        private async Task<string> FetchCurrentRankingRange(int score)
        {
            var rankingRanges = await FetchRankingRanges();
            int currentRangeIndex = FindCurrentRangeIndex(score, rankingRanges);

            return rankingRanges[currentRangeIndex].Name;
        }

        private async Task<List<RankingRangeDTO>> FetchRankingRanges()
        {
            var dbRef = FirebaseDatabase.DefaultInstance.RootReference.Child(RootDataNodes.RankingRangeNode);
            var snapshot = await dbRef.GetValueAsync();

            var rankingRanges = new List<RankingRangeDTO>();
            foreach (var child in snapshot.Children)
            {
                var json = child.GetRawJsonValue();

                rankingRanges.Add(JsonUtility.FromJson<RankingRangeDTO>(json));
            }

            return rankingRanges;
        }

        private int FindCurrentRangeIndex(int score, List<RankingRangeDTO> rankingRanges)
        {
            for (int i = rankingRanges.Count - 1; i >= 0; i--)
                if (score < rankingRanges[i].HightestValue)
                    return i;
            
            return 0; 
        }
    }
}