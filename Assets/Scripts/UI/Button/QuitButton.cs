using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BullBrukBruker{
    public class QuitButton : ButtonController
    {    
        protected override void OnClick()
        {
            base.OnClick();
            
            Observer.PostEvent(EventID.QuitButton_Clicked, null);
        }
    }
}