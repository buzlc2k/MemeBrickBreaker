using UnityEngine;
using System;
using System.Collections.Generic;

namespace BullBrukBruker
{
    [CreateAssetMenu(menuName = "ScreenConfig")]
    public class ScreenConfig : ScriptableObject
    {
        [field: SerializeField] public float DefaultWidth { get; private set; }
        [field: SerializeField] public float DefaultHeight { get; private set; }
        [field: SerializeField] public float TopDownHeightOffset { get; private set; }
        [field: SerializeField] public int TargetFrameRate { get; private set; }
    }   
}