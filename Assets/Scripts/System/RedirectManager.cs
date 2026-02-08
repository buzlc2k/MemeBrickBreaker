using System;
using UnityEngine;

namespace BullBrukBruker
{
    public class RedirectManager : SingletonMono<RedirectManager>
    {
        private Action<object> redirectToStore;

        private void OnEnable()
        {
            InitializeDelegate();
            RegisterEvent();
        }

        private void OnDisable()
        {
            UnRegisterEvent();
        }

        private void InitializeDelegate()
        {
            redirectToStore ??= (param) =>
            {
                RedirectToStore();
            };
        }

        private void RegisterEvent()
        {
            Observer.AddListener(EventID.ContactButton_Clicked, redirectToStore);
        }

        private void UnRegisterEvent()
        {
            Observer.RemoveListener(EventID.ContactButton_Clicked, redirectToStore);
        }
        
        public void RedirectToStore()
        {
#if UNITY_ANDROID
            Application.OpenURL("https://play.google.com/store/apps/developer?id=GeDa+DevTeam");
#endif
        }
    }
}