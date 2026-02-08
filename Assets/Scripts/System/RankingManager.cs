using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace BullBrukBruker
{
    [Serializable]
    public class RankingUser
    {
        public string UserId { get; private set; }
        public int Score { get; private set; }
        public bool IsPlayer { get; private set; }

        public RankingUser(string userId, int score, bool isPlayer)
        {
            UserId = userId;
            Score = score;
            IsPlayer = isPlayer;
        }
    }

    [Serializable]
    public class RankingPlayer
    {
        public string UserId { get; private set; }
        public int Score { get; private set; }
        public string Rank { get; private set; }

        public RankingPlayer(string userId, int score, string rank)
        {
            UserId = userId;
            Score = score;
            Rank = rank;
        }

        public void SetRank(string rank)
        {
            Rank = rank;
        }
    }

    public class RankingResult
    {
        public List<RankingUser> TopRankUsers { get; }
        public RankingPlayer RankPlayer { get; }

        public RankingResult(List<RankingUser> topRankUsers, RankingPlayer rankPlayer)
        {
            TopRankUsers = topRankUsers;
            RankPlayer = rankPlayer;
        }
    }

    public class RankingManager : SingletonMono<RankingManager>
    {
        public async Task<RankingResult> CalculateRanking(int topRankingSize)
        {
            var createRawDataTopRankUsersTask = CreateRawDataTopRankUsers(topRankingSize);
            var createRawDataRankPlayerTask = CreateRawDataRankPlayer();

            await Task.WhenAll(createRawDataTopRankUsersTask, createRawDataRankPlayerTask);

            var topRankUsers = createRawDataTopRankUsersTask.Result;
            var player = createRawDataRankPlayerTask.Result;

            return new RankingResult(topRankUsers, player);
        }

        private async Task<List<RankingUser>> CreateRawDataTopRankUsers(int topRankingSize)
        {
            var playerId = DataManager.Instance.GetUserID();
            List<(string userId, string score)> topRankUsersData = await DataManager.Instance.GetTopRankUsers(topRankingSize);

            return topRankUsersData
                .Select(user => new RankingUser(user.userId, int.Parse(user.score), user.userId.Equals(playerId)))
                .ToList();
        }

        private async Task<RankingPlayer> CreateRawDataRankPlayer()
        {
            var playerId = DataManager.Instance.GetUserID();
            var score = DataManager.Instance.GetScore();
            var currentRank = await DataManager.Instance.GetCurrentRank();
            
            return new RankingPlayer(playerId, score, currentRank);
        }
    }
}