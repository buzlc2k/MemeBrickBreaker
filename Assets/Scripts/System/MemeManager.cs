using System.Collections;
using UnityEngine;
using UnityEngine.Video;

namespace BullBrukBruker
{
    public class MemeManager : SingletonMono<MemeManager>
    {
        [SerializeField] private MemeVideoPlayer videoPlayer = default;
        [SerializeField] private MemeAudioCue audioCue = default;

        [Header("Configuration")]
        [SerializeField] private float transDuration = 0.5f;

        private Coroutine transCourotine = default;
        private bool isHappy => transCourotine != null;

        public void StartPlay()
        {
            transCourotine = null;

            audioCue.InitializeMemeAudioCue();
            videoPlayer.InitializeMemeVideoPlayer();
        }

        public void ChangeToHappy()
        {
            if (!isHappy)
            {
                audioCue.Play(true);
                videoPlayer.Play(true);
            }
            else
                StopCoroutine(transCourotine);

            transCourotine = StartCoroutine(FinishedPlayingHappy());
        }

        public void Pause()
        {
            audioCue.Pause(isHappy);
            videoPlayer.Pause(); 
        }

        public void Resume()
        {
            audioCue.Resume(isHappy);
            videoPlayer.Resume();
        }

        private IEnumerator FinishedPlayingHappy()
        {
            yield return new WaitForSeconds(transDuration);

            while (LevelManager.Instance.CurrentBalls.Count >= 20) //TO DO: Remove magic number
                yield return null;

            audioCue.Play(false);
            videoPlayer.Play(false);

            transCourotine = null;
        }
    }
}   