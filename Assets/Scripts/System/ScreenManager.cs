using System.Collections;
using UnityEngine;

namespace BullBrukBruker
{
    public class ScreenManager : SingletonMono<ScreenManager>
    {
        [field: SerializeField] public Camera MainCamera { get; private set; }
        public float ScreenWidth { get; private set; }
        public float TopScreenHeight { get; private set; }
        public float DownScreenHeight { get; private set; }

        public IEnumerator InitScreenManager()
        {
            while (MainCamera == null)
            {
                MainCamera = FindFirstObjectByType<Camera>();
                yield return null;
            }

            CalculateFrameRate();
            CalculateCurrentScreenSize();
            yield return null;
        }

        private void CalculateFrameRate()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = ConfigsManager.Instance.ScreenConfig.TargetFrameRate;
        }

        private void CalculateCurrentScreenSize()
        {
            float aspectRatio = (float)Screen.width / Screen.height;

            float minWidth = ConfigsManager.Instance.ScreenConfig.DefaultWidth;
            float minHeight = ConfigsManager.Instance.ScreenConfig.DefaultHeight;

            float normalizeWidth = (float)Screen.width / minWidth;
            float normalizeHeight = (float)Screen.height / minHeight;

            if (normalizeWidth >= normalizeHeight)
            {
                TopScreenHeight = minHeight;
                ScreenWidth = minHeight * aspectRatio;
            }
            else
            {
                ScreenWidth = minWidth;
                TopScreenHeight = minWidth / aspectRatio;
            }

            DownScreenHeight = TopScreenHeight - ConfigsManager.Instance.ScreenConfig.TopDownHeightOffset;
            MainCamera.orthographicSize = TopScreenHeight;
        }
    }
}