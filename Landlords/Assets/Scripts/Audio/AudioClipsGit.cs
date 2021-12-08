using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PIXEL.Landlords.Audio
{
    public class AudioClipsGit
    {
        [Header("直接获取创建的AudioClips库")]
        public static AudioClipsProperty clipsProperty = Resources.Load<AudioClipsProperty>("AudioClips/AudioClipsGit");

        [Tooltip("所有牌型的音效都要分别储存储存")]
        [Header("男人-单牌-音频片段")]
        public static AudioClip man_Single_3 = clipsProperty.man_Single[0];
        public static AudioClip man_Single_4 = clipsProperty.man_Single[1];
        public static AudioClip man_Single_5 = clipsProperty.man_Single[2];
        public static AudioClip man_Single_6 = clipsProperty.man_Single[3];
        public static AudioClip man_Single_7 = clipsProperty.man_Single[4];
        public static AudioClip man_Single_8 = clipsProperty.man_Single[5];
        public static AudioClip man_Single_9 = clipsProperty.man_Single[6];
        public static AudioClip man_Single_10 = clipsProperty.man_Single[7];
        public static AudioClip man_Single_J = clipsProperty.man_Single[8];
        public static AudioClip man_Single_Q = clipsProperty.man_Single[9];
        public static AudioClip man_Single_K = clipsProperty.man_Single[10];
        public static AudioClip man_Single_A = clipsProperty.man_Single[11];
        public static AudioClip man_Single_2 = clipsProperty.man_Single[12];
        public static AudioClip man_Single_SmallJoker = clipsProperty.man_Single[13];
        public static AudioClip man_Single_BigJoker = clipsProperty.man_Single[14];

        [Header("男人-对子-音频片段")]
        public static AudioClip man_Pair_3 = clipsProperty.man_Pair[0];
        public static AudioClip man_Pair_4 = clipsProperty.man_Pair[1];
        public static AudioClip man_Pair_5 = clipsProperty.man_Pair[2];
        public static AudioClip man_Pair_6 = clipsProperty.man_Pair[3];
        public static AudioClip man_Pair_7 = clipsProperty.man_Pair[4];
        public static AudioClip man_Pair_8 = clipsProperty.man_Pair[5];
        public static AudioClip man_Pair_9 = clipsProperty.man_Pair[6];
        public static AudioClip man_Pair_10 = clipsProperty.man_Pair[7];
        public static AudioClip man_Pair_J = clipsProperty.man_Pair[8];
        public static AudioClip man_Pair_Q = clipsProperty.man_Pair[9];
        public static AudioClip man_Pair_K = clipsProperty.man_Pair[10];
        public static AudioClip man_Pair_A = clipsProperty.man_Pair[11];
        public static AudioClip man_Pair_2 = clipsProperty.man_Pair[12];

        [Header("男人-三个-音频片段")]
        public static AudioClip man_Triple_3 = clipsProperty.man_Triple[0];
        public static AudioClip man_Triple_4 = clipsProperty.man_Triple[1];
        public static AudioClip man_Triple_5 = clipsProperty.man_Triple[2];
        public static AudioClip man_Triple_6 = clipsProperty.man_Triple[3];
        public static AudioClip man_Triple_7 = clipsProperty.man_Triple[4];
        public static AudioClip man_Triple_8 = clipsProperty.man_Triple[5];
        public static AudioClip man_Triple_9 = clipsProperty.man_Triple[6];
        public static AudioClip man_Triple_10 = clipsProperty.man_Triple[7];
        public static AudioClip man_Triple_J = clipsProperty.man_Triple[8];
        public static AudioClip man_Triple_Q = clipsProperty.man_Triple[9];
        public static AudioClip man_Triple_K = clipsProperty.man_Triple[10];
        public static AudioClip man_Triple_A = clipsProperty.man_Triple[11];
        public static AudioClip man_Triple_2 = clipsProperty.man_Triple[12];

        [Header("男人-特殊牌型-音频片段")]
        public static AudioClip man_Special_TripleSingle = clipsProperty.man_SpecialCardCombination[0];
        public static AudioClip man_Special_TriplePair = clipsProperty.man_SpecialCardCombination[1];
        public static AudioClip man_Special_Straight = clipsProperty.man_SpecialCardCombination[2];
        public static AudioClip man_Special_StraightPair = clipsProperty.man_SpecialCardCombination[3];
        public static AudioClip man_Special_Plane = clipsProperty.man_SpecialCardCombination[4];
        public static AudioClip man_Special_BoomSingle = clipsProperty.man_SpecialCardCombination[5];
        public static AudioClip man_Special_BoomPair = clipsProperty.man_SpecialCardCombination[6];
        public static AudioClip man_Special_Boom = clipsProperty.man_SpecialCardCombination[7];
        public static AudioClip man_Special_JokerBoom = clipsProperty.man_SpecialCardCombination[8];

        [Header("男人-其他音效片段")]
        public static AudioClip man_Bigger = clipsProperty.man_Others[0];
        public static AudioClip man_CantAfford = clipsProperty.man_Others[1];
        public static AudioClip man_GiveUp = clipsProperty.man_Others[2];
        public static AudioClip man_LastOneCard = clipsProperty.man_Others[3];

        //-----------------------------------------------------------------------

        [Header("女人-单牌-音频片段")]
        public static AudioClip woman_Single_3 = clipsProperty.women_Single[0];
        public static AudioClip woman_Single_4 = clipsProperty.women_Single[1];
        public static AudioClip woman_Single_5 = clipsProperty.women_Single[2];
        public static AudioClip woman_Single_6 = clipsProperty.women_Single[3];
        public static AudioClip woman_Single_7 = clipsProperty.women_Single[4];
        public static AudioClip woman_Single_8 = clipsProperty.women_Single[5];
        public static AudioClip woman_Single_9 = clipsProperty.women_Single[6];
        public static AudioClip woman_Single_10 = clipsProperty.women_Single[7];
        public static AudioClip woman_Single_J = clipsProperty.women_Single[8];
        public static AudioClip woman_Single_Q = clipsProperty.women_Single[9];
        public static AudioClip woman_Single_K = clipsProperty.women_Single[10];
        public static AudioClip woman_Single_A = clipsProperty.women_Single[11];
        public static AudioClip woman_Single_2 = clipsProperty.women_Single[12];
        public static AudioClip woman_Single_SmallJoker = clipsProperty.women_Single[13];
        public static AudioClip woman_Single_BigJoker = clipsProperty.women_Single[14];

        [Header("女人-对子-音频片段")]
        public static AudioClip woman_Pair_3 = clipsProperty.women_Pair[0];
        public static AudioClip woman_Pair_4 = clipsProperty.women_Pair[1];
        public static AudioClip woman_Pair_5 = clipsProperty.women_Pair[2];
        public static AudioClip woman_Pair_6 = clipsProperty.women_Pair[3];
        public static AudioClip woman_Pair_7 = clipsProperty.women_Pair[4];
        public static AudioClip woman_Pair_8 = clipsProperty.women_Pair[5];
        public static AudioClip woman_Pair_9 = clipsProperty.women_Pair[6];
        public static AudioClip woman_Pair_10 = clipsProperty.women_Pair[7];
        public static AudioClip woman_Pair_J = clipsProperty.women_Pair[8];
        public static AudioClip woman_Pair_Q = clipsProperty.women_Pair[9];
        public static AudioClip woman_Pair_K = clipsProperty.women_Pair[10];
        public static AudioClip woman_Pair_A = clipsProperty.women_Pair[11];
        public static AudioClip woman_Pair_2 = clipsProperty.women_Pair[12];

        [Header("女人-三个-音频片段")]
        public static AudioClip woman_Triple_3 = clipsProperty.women_Triple[0];
        public static AudioClip woman_Triple_4 = clipsProperty.women_Triple[1];
        public static AudioClip woman_Triple_5 = clipsProperty.women_Triple[2];
        public static AudioClip woman_Triple_6 = clipsProperty.women_Triple[3];
        public static AudioClip woman_Triple_7 = clipsProperty.women_Triple[4];
        public static AudioClip woman_Triple_8 = clipsProperty.women_Triple[5];
        public static AudioClip woman_Triple_9 = clipsProperty.women_Triple[6];
        public static AudioClip woman_Triple_10 = clipsProperty.women_Triple[7];
        public static AudioClip woman_Triple_J = clipsProperty.women_Triple[8];
        public static AudioClip woman_Triple_Q = clipsProperty.women_Triple[9];
        public static AudioClip woman_Triple_K = clipsProperty.women_Triple[10];
        public static AudioClip woman_Triple_A = clipsProperty.women_Triple[11];
        public static AudioClip woman_Triple_2 = clipsProperty.women_Triple[12];

        [Header("女人-特殊牌型-音频片段")]
        public static AudioClip woman_Special_TripleSingle = clipsProperty.women_SpecialCardCombination[0];
        public static AudioClip woman_Special_TriplePair = clipsProperty.women_SpecialCardCombination[1];
        public static AudioClip woman_Special_Straight = clipsProperty.women_SpecialCardCombination[2];
        public static AudioClip woman_Special_StraightPair = clipsProperty.women_SpecialCardCombination[3];
        public static AudioClip woman_Special_Plane = clipsProperty.women_SpecialCardCombination[4];
        public static AudioClip woman_Special_BoomSingle = clipsProperty.women_SpecialCardCombination[5];
        public static AudioClip woman_Special_BoomPair = clipsProperty.women_SpecialCardCombination[6];
        public static AudioClip woman_Special_Boom = clipsProperty.women_SpecialCardCombination[7];
        public static AudioClip woman_Special_JokerBoom = clipsProperty.women_SpecialCardCombination[8];

        [Header("女人-其他音效片段")]
        public static AudioClip woman_Bigger = clipsProperty.women_Others[0];
        public static AudioClip woman_CantAfford = clipsProperty.women_Others[1];
        public static AudioClip woman_GiveUp = clipsProperty.women_Others[2];
        public static AudioClip woman_LastOneCard = clipsProperty.women_Others[3];

        //-----------------------------------------------------------------------

        [Header("特殊牌型音效片段")]
        public static AudioClip general_BoomEffect = clipsProperty.generalAudioClips[0];
        public static AudioClip general_PlaneEffect = clipsProperty.generalAudioClips[1];
        public static AudioClip general_DealCard = clipsProperty.generalAudioClips[2];
        public static AudioClip general_ChooseCard = clipsProperty.generalAudioClips[3];
        public static AudioClip general_PlayCard = clipsProperty.generalAudioClips[4];
        public static AudioClip general_SpecialWarning = clipsProperty.generalAudioClips[5];
        public static AudioClip general_Win = clipsProperty.generalAudioClips[6];
        public static AudioClip general_Lose = clipsProperty.generalAudioClips[7];

        //-----------------------------------------------------------------------
        [Header("背景音乐")]
        public static AudioClip bgm_1 = clipsProperty.backGroundMusic[0];
        public static AudioClip bgm_2 = clipsProperty.backGroundMusic[1];
        public static AudioClip bgm_3 = clipsProperty.backGroundMusic[2];
    }
}