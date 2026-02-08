using System.Collections;
using UnityEngine;
using UnityEngine.Video;

namespace BullBrukBruker
{
    public class MemeAudioCue : MonoBehaviour
    {
        [Header("Sounds")]
        [SerializeField] private AudioSO sadAudioSO = default;
        [SerializeField] private AudioSO happyAudioSO = default;

        private int sadEmittersKey = 0;
        private int happyEmittersKey = 0;

        public void InitializeMemeAudioCue()
        {
            //Reset data
            if (sadEmittersKey != 0)
            {
                AudioManager.Instance.Stop(sadEmittersKey);
                sadEmittersKey = 0;
            }
            if (happyEmittersKey != 0)
            {
                AudioManager.Instance.Stop(happyEmittersKey);
                happyEmittersKey = 0;
            }

            sadEmittersKey = AudioManager.Instance.Play(sadAudioSO, 0, 0.05f, 0.05f);
        }

        public void Play(bool isHappy)
        {
            if (isHappy)
            {
                AudioManager.Instance.Pause(sadEmittersKey);
                happyEmittersKey = AudioManager.Instance.Play(happyAudioSO, 0, 0.05f, 0.05f);
            }
            else
            {
                AudioManager.Instance.Stop(happyEmittersKey);
                AudioManager.Instance.Resume(sadEmittersKey);
            }
        }

        public void Pause(bool isHappy)
        {
            if (isHappy)
                AudioManager.Instance.Pause(happyEmittersKey);
            else
                AudioManager.Instance.Pause(sadEmittersKey);
        }

        public void Resume(bool isHappy)
        {
            if (isHappy)
                AudioManager.Instance.Resume(happyEmittersKey);
            else
                AudioManager.Instance.Resume(sadEmittersKey);
        }
    }
}   