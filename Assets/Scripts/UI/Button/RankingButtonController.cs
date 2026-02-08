using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BullBrukBruker{
    public class RankingButtonController : ButtonController
    {    
        protected override void OnClick()
        {
            base.OnClick();
            
            Observer.PostEvent(EventID.RankingButton_Clicked, null);
        }
    }
}