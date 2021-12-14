using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PIXEL.Landlords.Audio
{
    public static class AudioMixerValue
    {
        [Header("背景音乐声音大小")]
        private static float backMusicValue;

        [Header("音效声音大小")]
        private static float audioEffectValue;

        public static float BackMusicValue
        {
            get => backMusicValue;

            set
            {
                backMusicValue = value;
            }
        }

        public static float AudioEffectValue
        {
            get => audioEffectValue;

            set
            {
                audioEffectValue = value;
            }
        }
    }
}