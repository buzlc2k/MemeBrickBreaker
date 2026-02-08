using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BullBrukBruker{
    public abstract class ButtonController : MonoBehaviour
    {
        [SerializeField] protected Button button;
        [SerializeField] protected AudioSO clickSFX;

        protected UnityAction onButtonClickAction;

        protected virtual void Awake()
        {
            SetButton();
        }

        protected virtual void OnEnable()
        {
            AddButtonClickAction();
        }

        protected virtual void OnDisable()
        {
            button.onClick.RemoveListener(onButtonClickAction);
        }

        protected virtual void SetButton(){
            if (button == null) button = GetComponent<Button>();
        }

        private void AddButtonClickAction()
        {
            onButtonClickAction ??= () => {
                OnClick();
            };

            button.onClick.AddListener(onButtonClickAction);
        }

        protected virtual void OnClick()
        {
            AudioManager.Instance.Play(clickSFX, 0, 0, 0);
        }
    }
}
