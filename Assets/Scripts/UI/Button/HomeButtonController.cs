using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BullBrukBruker{
    public class HomeButtonController : ButtonController
    {    
        protected override void OnClick()
        {
            base.OnClick();
            
            Observer.PostEvent(EventID.HomeButton_Clicked, null);
        }
    }
}