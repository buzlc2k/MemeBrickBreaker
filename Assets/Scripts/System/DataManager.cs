using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BullBrukBruker
{
    public class DataManager : SingletonMono<DataManager>
    {
        private DataAccessorsManager dataAccessorManager;

        public IEnumerator InitDataManager()
        {
            dataAccessorManager = new();

            yield return StartCoroutine(dataAccessorManager.LoadDataAccessors());
        }

        public string GetUserID()
            => dataAccessorManager.GetUserID();
        public int GetCurrentLevel()
            => dataAccessorManager.GetUserSpecificDataAccessor<LevelProgressDataAccessor>(DataID.LevelProgress).Read(x => x.CurrentLevel);

        public int GetHighestLevel()
            => dataAccessorManager.GetUserSpecificDataAccessor<LevelProgressDataAccessor>(DataID.LevelProgress).Read(x => x.HighestLevel);

        public List<int> GetStarsPerLevel()
            => dataAccessorManager.GetUserSpecificDataAccessor<LevelProgressDataAccessor>(DataID.LevelProgress).Read(x => x.StarsPerLevel);

        public int GetScore()
            => dataAccessorManager.GetUserSpecificDataAccessor<UserDataAccessor>(DataID.User).Read(x => x.Score);

        public void WriteCurrentLevel(int currentLevel)
            => dataAccessorManager.GetUserSpecificDataAccessor<LevelProgressDataAccessor>(DataID.LevelProgress).Write(x => x.CurrentLevel, currentLevel);

        public void WriteHighestLevel(int highestLevel)
            => dataAccessorManager.GetUserSpecificDataAccessor<LevelProgressDataAccessor>(DataID.LevelProgress).Write(x => x.HighestLevel, highestLevel);

        public void AddStarsPerLevelAndScore(int star)
        {
            AddStarsPerLevel(star);
            AddScore(star);
        }

        public void AddStarsPerLevel(int star)
        {
            var stars = new List<int>(GetStarsPerLevel())
            {
                star
            };

            dataAccessorManager.GetUserSpecificDataAccessor<LevelProgressDataAccessor>(DataID.LevelProgress).Write(x => x.StarsPerLevel, stars);
        }

        public void AddScore(int score)
            => dataAccessorManager.GetUserSpecificDataAccessor<UserDataAccessor>(DataID.User).Write(x => x.Score, GetScore() + score);

        public async Task<string> GetCurrentRank()
            => await dataAccessorManager.GetSystemDataAccessor().FetchCurrentRank(GetUserID(), GetScore());

        public async Task<List<(string, string)>> GetTopRankUsers(int num)
            => await dataAccessorManager.GetSystemDataAccessor().FetchTopRankUsers(num);
    }
}