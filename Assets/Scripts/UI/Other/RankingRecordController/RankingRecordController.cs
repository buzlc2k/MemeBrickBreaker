using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BullBrukBruker{
    public class RankingRecordController : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI indexText;
        [SerializeField] protected TextMeshProUGUI userIdText;
        [SerializeField] protected TextMeshProUGUI scoreText;
        [SerializeField] protected Image borderImage;

        [SerializeField] protected Color playerColor;
        [SerializeField] protected Color otherUserColor;

        public virtual void SetRankingRecord(string index, string userID, string score, bool isPlayer)
        {
            indexText.text = index;
            userIdText.text = userID;
            scoreText.text = score;
            
            borderImage.color = isPlayer ? playerColor : otherUserColor;
        }
    }
}