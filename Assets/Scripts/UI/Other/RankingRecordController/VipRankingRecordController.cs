using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BullBrukBruker{
    public class VipRankingRecordController : RankingRecordController
    {
        [SerializeField] protected Image starImage;
        
        public override void SetRankingRecord(string index, string userID, string score, bool isPlayer)
        {
            base.SetRankingRecord(index, userID, score, isPlayer);

            starImage.color = isPlayer ? playerColor : otherUserColor;
        }
    }
}