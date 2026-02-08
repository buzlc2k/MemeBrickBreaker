using System;
using System.Collections.Generic;
using UnityEngine;

namespace BullBrukBruker
{
    [Serializable]
    public struct Audio
    {
        public AudioClip Clip;
        public PrioritySoundLevel Priority;
        public bool Mute;
        [Range(0f, 1f)] public float Volume;
        [Range(-3f, 3f)] public float Pitch;
        [Range(-1f, 1f)] public float PanStereo;

        public void ApplyTo(AudioSource audioSource)
        {
            audioSource.clip = Clip;
            audioSource.priority = (int)Priority;
            audioSource.mute = Mute;
            audioSource.pitch = Pitch;
            audioSource.panStereo = PanStereo;
        }
    }

    [Serializable]
    public struct AudioGroup
    {
        public List<Audio> Group;
    }

    [CreateAssetMenu(menuName = "AudioSO")]
    public class AudioSO : ScriptableObject
    {
        [SerializeField] private AudioSequenceMode sequenceMode;
        [field: SerializeField] public bool Loop { get; private set; } 
        [SerializeField] private List<AudioGroup> audioGroups;
        private int nextClipToPlay = -1;
        private int lastClipPlayed = -1;

        public List<Audio> GetAudio()
        {
            if (audioGroups.Count == 1)
                return audioGroups[0].Group;

            if (nextClipToPlay == -1)
                nextClipToPlay = (sequenceMode == AudioSequenceMode.Sequential) ? 0 : UnityEngine.Random.Range(0, audioGroups.Count);

            else
            {
                switch (sequenceMode)
                {
                    case AudioSequenceMode.Random:
                        nextClipToPlay = UnityEngine.Random.Range(0, audioGroups.Count);
                        break;

                    case AudioSequenceMode.RandomNoImmediateRepeat:
                        do
                        {
                            nextClipToPlay = UnityEngine.Random.Range(0, audioGroups.Count);
                        } while (nextClipToPlay == lastClipPlayed);
                        break;

                    case AudioSequenceMode.Sequential:
                        nextClipToPlay = (int)Mathf.Repeat(++nextClipToPlay, audioGroups.Count);
                        break;
                }
            }

            lastClipPlayed = nextClipToPlay;

            return audioGroups[nextClipToPlay].Group;
        }
    }
}