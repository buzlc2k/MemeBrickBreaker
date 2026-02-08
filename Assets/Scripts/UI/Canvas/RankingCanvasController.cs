using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace BullBrukBruker
{
    public class RankingCanvasController : BaseCanvasController
    {
        [SerializeField] int vipRankingThreshold = 3;
        [SerializeField] int topRankingTableSize = 10;

        [SerializeField] GameObject vipRankRecordPrefab;
        [SerializeField] GameObject normalRankRecordPrefab;

        [SerializeField] RectTransform rankingRecordLayout;
        [SerializeField] RectTransform userRankingRecordContainer;

        private float defaultLayoutHeight;

        public override IEnumerator InitCanvas()
        {
            defaultLayoutHeight = rankingRecordLayout.rect.height;
            yield return null;
        }

        public override IEnumerator TurnOnCanvas()
        {
            yield return CanvasManager.Instance.StartCoroutine(GenerateRankingRecords());

            ResizeRankingRecordLayout();

            yield return CanvasManager.Instance.StartCoroutine(base.TurnOnCanvas());
        }

        private IEnumerator GenerateRankingRecords()
        {
            foreach(Transform child in rankingRecordLayout.transform)
                Destroy(child.gameObject);
            foreach(Transform child in userRankingRecordContainer.transform)
                Destroy(child.gameObject);

            var getRankingResultTask = RankingManager.Instance.CalculateRanking(topRankingTableSize);
            yield return new YieldTask(getRankingResultTask);

            var rankingResult = getRankingResultTask.Result;

            GenerateTopRankingUsers(rankingResult.TopRankUsers);
            GeneratePlayerRankingRecord(rankingResult.RankPlayer);
        }

        private void GenerateTopRankingUsers(List<RankingUser> topRankUsers)
        {
            for (int rank = 0; rank < topRankUsers.Count; rank++)
            {
                bool isVipRecord = rank <= 2;

                var recordPrefab = isVipRecord ? vipRankRecordPrefab : normalRankRecordPrefab;
                var rankRecord = Instantiate(recordPrefab.GetComponent<RankingRecordController>(), rankingRecordLayout);
                rankRecord.SetRankingRecord((rank + 1).ToString(),
                                            topRankUsers[rank].UserId,
                                            topRankUsers[rank].Score.ToString(),
                                            topRankUsers[rank].IsPlayer);
            }
        }

        private void GeneratePlayerRankingRecord(RankingPlayer rankPlayer)
        {
            bool inVipRanking = false;

            if (int.TryParse(rankPlayer.Rank, out int playerRank) && playerRank < vipRankingThreshold + 1)
                inVipRanking = true;

            var recordPrefab = inVipRanking ? vipRankRecordPrefab : normalRankRecordPrefab;
            var rankRecord = Instantiate(recordPrefab.GetComponent<RankingRecordController>(), userRankingRecordContainer);

            rankRecord.SetRankingRecord(rankPlayer.Rank, rankPlayer.UserId, rankPlayer.Score.ToString(), true);

            var rectTransform = rankRecord.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
        }

        private void ResizeRankingRecordLayout()
        {
            var layout = rankingRecordLayout.GetComponent<VerticalLayoutGroup>();

            float requiredHeight = layout.padding.top + layout.padding.bottom;

            int activeChildCount = 0;
            for (int i = 0; i < rankingRecordLayout.childCount; i++)
            {
                var child = rankingRecordLayout.GetChild(i);
                activeChildCount++;
                requiredHeight += child.GetComponent<RectTransform>().rect.height;
            }

            if (activeChildCount > 1)
                requiredHeight += layout.spacing * activeChildCount;

            if (requiredHeight > defaultLayoutHeight)
                rankingRecordLayout.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, requiredHeight);

            rankingRecordLayout.anchoredPosition = new Vector2(0, -requiredHeight / 2);
        }
    }
}