using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PIXEL.Landlords.Audio
{
    [CreateAssetMenu(fileName = "AudioClip", menuName = "Audio/AudioClip")]
    public class AudioClipsProperty : ScriptableObject
    {
        [Header("男人音频"),Tooltip("男人的音效，分类型储存")]
        public AudioClip[] man_Single;
        public AudioClip[] man_Pair;
        public AudioClip[] man_Triple;
        public AudioClip[] man_SpecialCardCombination;
        public AudioClip[] man_Others;

        [Header("女人音频"), Tooltip("女人的音效，分类型储存"), Space(39)]
        public AudioClip[] women_Single;
        public AudioClip[] women_Pair;
        public AudioClip[] women_Triple;
        public AudioClip[] women_SpecialCardCombination;
        public AudioClip[] women_Others;

        [Header("其他通用音效"), Tooltip("通用的音效"), Space(39)]
        public AudioClip[] generalAudioClips;

        [Header("背景音乐"), Space(39)]
        public AudioClip[] backGroundMusic;
    }
}
