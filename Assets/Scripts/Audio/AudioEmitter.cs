using System;
using System.Collections;
using UnityEngine;

namespace BullBrukBruker
{
    public class AudioEmitter : MonoBehaviour, IPooled
    {
        private bool isPlaying = false;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private ObjectDespawning despawning;
        
        public Action<GameObject> ReleaseCallback { get; set; }

        private void Awake()
        {
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
            if (despawning == null)
                despawning = GetComponentInChildren<ObjectDespawning>();
        }

        protected virtual void OnDisable()
        {
            ReleaseCallback?.Invoke(gameObject);
        }

        public IEnumerator Fade(float duration, float targetVolume)
        {
            float timer = duration;
            float volumeRange = audioSource.volume - targetVolume;

            while (timer > 0)
            {
                timer -= Time.deltaTime;
                audioSource.volume = targetVolume + (timer / duration) * volumeRange;
                yield return null;
            }
        }

        public void Play(Audio audio, bool hasToLoop, float startTime, float fadeInTime, float fadeOutTime, Vector3 position)
        {
            audio.ApplyTo(audioSource);

            audioSource.time = Mathf.Clamp(startTime, 0, audioSource.clip.length);
            audioSource.loop = hasToLoop;

            if (fadeInTime > 0)
            {
                audioSource.volume = 0;
                StartCoroutine(Fade(fadeInTime, audio.Volume));
            }
            else
                audioSource.volume = audio.Volume;

            transform.position = position;

            audioSource.Play();
            isPlaying = true;

            if (!hasToLoop)
                StartCoroutine(FinishedPlaying(audioSource.clip.length - audioSource.time, fadeOutTime));
        }

        public void Resume()
        {
            audioSource.Play();
            isPlaying = true;
        }

        public void Pause()
        {
            audioSource.Pause();
            isPlaying = false;
        }

        public void Finish(float fadeOutTime)
        {
            if (audioSource.loop)
            {
                audioSource.loop = false;
                float timeRemaining = audioSource.clip.length - audioSource.time;
                StartCoroutine(FinishedPlaying(timeRemaining, fadeOutTime));
            }
        }

        public IEnumerator FinishedPlaying(float offsetTime, float fadeTime)
        {
            var timer = offsetTime - fadeTime;

            while (timer >= 0)
            {
                if (isPlaying)
                    timer -= Time.deltaTime;
                yield return null;
            }

            yield return StartCoroutine(Fade(fadeTime, 0));

            Stop();
        }

        public void Stop()
        {
            audioSource.Stop();
            isPlaying = false;

            despawning.Despawn();
        }
    }
}