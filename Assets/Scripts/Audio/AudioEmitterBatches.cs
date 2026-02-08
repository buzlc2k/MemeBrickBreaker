using System;
using System.Collections;
using System.Collections.Generic;

namespace BullBrukBruker
{
    public class AudioEmitterBatches
    {
        private int _nextUniqueKey;
        private Dictionary<int, List<AudioEmitter>> emittersBatches;
        private Dictionary<AudioEmitter, int> emitterToKeyMap;

        public AudioEmitterBatches()
        {
            _nextUniqueKey = 0;
            emittersBatches = new();
            emitterToKeyMap = new();
        }

        public int CreateNewBatch()
        {
            _nextUniqueKey++;
            emittersBatches[_nextUniqueKey] = new List<AudioEmitter>();

            return _nextUniqueKey;
        }

        public void AddEmitterToBatch(int key, AudioEmitter emitter)
        {
            if (emittersBatches.ContainsKey(key))
            {
                emittersBatches[key].Add(emitter);
                emitterToKeyMap[emitter] = key;
            }
        }

        public bool TryGetBatch(int key, out List<AudioEmitter> emitters)
        {
            return emittersBatches.TryGetValue(key, out emitters);
        }

        public void RemoveEmitterInBatch(AudioEmitter emitter)
        {
            if (!emitterToKeyMap.TryGetValue(emitter, out var key))
                return;

            var emitters = emittersBatches[key];

            emitters.Remove(emitter);
            emitterToKeyMap.Remove(emitter);

            if (emitters.Count == 0)
                emittersBatches.Remove(key);
        }
    }
}