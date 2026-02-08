using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BullBrukBruker{
    public class PlayButtonController : ButtonController
    {    
        protected override void OnClick()
        {
            base.OnClick();
            
            Observer.PostEvent(EventID.PlayButton_Clicked, null);
        }
    }
}