using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BullBrukBruker
{
    public class AudioManager : SingletonMono<AudioManager>
    {
        [SerializeField] private GameObject audioEmitterPrefab;
        [SerializeField] private Transform audioEmitterHolder;
        private ObjectPooler<AudioEmitter> audioEmitterPooler;

        private AudioEmitterBatches audioEmitterBatches;

        public IEnumerator InitAudioManager()
        {
            audioEmitterPooler = new ObjectPooler<AudioEmitter>(audioEmitterPrefab.GetComponent<AudioEmitter>(), audioEmitterHolder, 500);
            audioEmitterBatches = new();

            yield return null;
        }

        public int Play(AudioSO audio, float startTime = 0, float fadeInTime = 0, float fadeOutTime = 0, Vector3 position = default)
        {
            var clipsToPlay = audio.GetAudio();
            var soundEmitters = new AudioEmitter[clipsToPlay.Count];
            var key = audioEmitterBatches.CreateNewBatch();

            for (int i = 0; i < soundEmitters.Length; i++)
            {
                soundEmitters[i] = audioEmitterPooler.Get();

                if (soundEmitters[i] != null)
                {
                    soundEmitters[i].Play(clipsToPlay[i], audio.Loop, startTime, fadeInTime, fadeOutTime, position);
                    audioEmitterBatches.AddEmitterToBatch(key, soundEmitters[i]);
                    soundEmitters[i].ReleaseCallback += RemoveEmitterInBatch;
                }
            }

            return key;
        }

        public void Pause(int key)
        {
            if (!audioEmitterBatches.TryGetBatch(key, out var emitters))
                return;

            for (int i = 0; i < emitters.Count; i++)
                emitters[i].Pause();
        }

        public void Resume(int key)
        {
            if (!audioEmitterBatches.TryGetBatch(key, out var emitters))
                return;

            for (int i = 0; i < emitters.Count; i++)
                emitters[i].Resume();
        }

        public void Finish(int key, float fadeOutTime)
        {
            if (!audioEmitterBatches.TryGetBatch(key, out var emitters))
                return;

            for (int i = 0; i < emitters.Count; i++)
                emitters[i].Finish(fadeOutTime);
        }

        public void Stop(int key)
        {
            if (!audioEmitterBatches.TryGetBatch(key, out var emitters))
                return;

            for (int i = 0; i < emitters.Count; i++)
                emitters[i].Stop();
        }

        private void RemoveEmitterInBatch(GameObject emitterObj)
        {
            var emitter = emitterObj.GetComponent<AudioEmitter>();

            audioEmitterBatches.RemoveEmitterInBatch(emitter);
            emitter.ReleaseCallback -= RemoveEmitterInBatch;
        }
    }
}