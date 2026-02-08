using System.Collections;
using UnityEngine;

namespace BullBrukBruker
{
    public static class CoroutineUtils
    {
        public static void StartSafeCourotine(MonoBehaviour starter, IEnumerator routine)
        {
            if (starter != null && starter.gameObject.activeInHierarchy)
                starter.StartCoroutine(routine);
        }
    }
}