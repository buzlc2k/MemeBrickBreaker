using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BullBrukBruker{
    public class ReturnMenuButtonController : ButtonController
    {    
        protected override void OnClick()
        {
            base.OnClick();
            
            Observer.PostEvent(EventID.ReturnMenuButton_Clicked, null);
        }
    }
}