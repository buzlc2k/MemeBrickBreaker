namespace BullBrukBruker{
    public class SelectLevelButtonController : ButtonController
    {
        protected override void OnClick()
        {
            base.OnClick();
            
            Observer.PostEvent(EventID.SelectLevelButton_Clicked, null);
        }
    }
}