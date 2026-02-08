using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace BullBrukBruker
{
    public class MemeVideoPlayer : MonoBehaviour
    {
        [Header("Animation")]
        [SerializeField] private List<AnimationClip> sadClips = default;
        [SerializeField] private List<AnimationClip> happyClips = default;
        [SerializeField] private Animator memeAnimator;
        private AnimationClip currentClip;

        public void InitializeMemeVideoPlayer()
        {
            StartCoroutine(C_InitializeMemeVideoPlayer());
        }

        private IEnumerator C_InitializeMemeVideoPlayer()
        {
            while (!memeAnimator.gameObject.activeInHierarchy)
                yield return null;

            currentClip = null;

            memeAnimator.speed = 1f;
            Play(false);
        }

        public void Play(bool isHappy)
        {
            currentClip = isHappy ? GetClip(happyClips) : GetClip(sadClips);
            memeAnimator.Play(currentClip.name);
        }

        public void Pause()
            => memeAnimator.speed = 0f;

        public void Resume()
        {
            StartCoroutine(C_Resume());
        }

        private IEnumerator C_Resume()
        {
            while (!memeAnimator.gameObject.activeInHierarchy)
                yield return null;
            
            memeAnimator.speed = 1f;
            memeAnimator.Play(currentClip.name);
        }

        private AnimationClip GetClip(List<AnimationClip> animationClips)
        {
            if (animationClips.Count == 1)
                return animationClips[0];

            return animationClips[Random.Range(0, animationClips.Count - 1)];
        }
    }
}   