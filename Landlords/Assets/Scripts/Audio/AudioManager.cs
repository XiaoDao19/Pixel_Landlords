using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PIXEL.Landlords.FrameWork;
using PIXEL.Landlords.Game;
using PIXEL.Landlords.Card;

namespace PIXEL.Landlords.Audio
{
    public class AudioManager : SingletonPattern<AudioManager>
    {
        #region 男人音效

        //男人出牌,牌型的音效
        public static void ManPlayCard(AudioSource _currentAudioPlayer, List<GameObject> _currentPlayCardCombination)
        {
            //获取当前出牌的类型
            PlayCardType currentCardType = PlayCardTypeJudgmentManager.PlayCardTypeJudge(_currentPlayCardCombination);

            //int tempAudio = Random.Range(0, 2);

            ////如果随机数0，则播放男人的“大你”音效片段，反之播放相应的牌型音频
            //if (tempAudio == 0)
            //{
            //    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Bigger);
            //    return;
            //}

            //如果当前出牌类型为单牌，则判断音频库中，男人的单牌音频的对应音频，并播放
            if (currentCardType == PlayCardType.Single)
            {
                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 103)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Single_3);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 104)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Single_4);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 105)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Single_5);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 106)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Single_6);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 107)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Single_7);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 108)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Single_8);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 109)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Single_9);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 110)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Single_10);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 111)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Single_J);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 112)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Single_Q);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 113)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Single_K);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 114)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Single_A);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 119)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Single_2);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 519)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Single_SmallJoker);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 520)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Single_BigJoker);
                }
            }

            //理同上
            if (currentCardType == PlayCardType.Pair)
            {
                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 103)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Pair_3);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 104)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Pair_4);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 105)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Pair_5);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 106)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Pair_6);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 107)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Pair_7);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 108)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Pair_8);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 109)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Pair_9);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 110)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Pair_10);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 111)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Pair_J);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 112)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Pair_Q);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 113)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Pair_K);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 114)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Pair_A);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 119)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Pair_2);
                }
            }

            //理同上
            if (currentCardType == PlayCardType.Triple)
            {
                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 103)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Triple_3);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 104)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Triple_4);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 105)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Triple_5);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 106)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Triple_6);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 107)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Triple_7);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 108)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Triple_8);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 109)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Triple_9);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 110)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Triple_10);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 111)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Triple_J);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 112)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Triple_Q);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 113)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Triple_K);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 114)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Triple_A);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 119)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Triple_2);
                }
            }

            //特殊牌型就直接播放
            if (currentCardType == PlayCardType.Triple_single)
            {
                _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Special_TripleSingle);
            }

            //同理
            if (currentCardType == PlayCardType.Triple_pair)
            {
                _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Special_TriplePair);
            }

            //同理
            if (currentCardType == PlayCardType.Straight)
            {
                _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Special_Straight);
            }

            //同理
            if (currentCardType == PlayCardType.Straight_pair)
            {
                _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Special_StraightPair);
            }

            //同理
            if (currentCardType == PlayCardType.Plane_none ||
                currentCardType == PlayCardType.Plane_single ||
                currentCardType == PlayCardType.Plane_pair)
            {
                _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Special_Plane);
            }

            //同理
            if (currentCardType == PlayCardType.Boom_single)
            {
                _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Special_BoomSingle);
            }

            //同理
            if (currentCardType == PlayCardType.Boom_pair)
            {
                _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Special_BoomPair);
            }

            //同理
            if (currentCardType == PlayCardType.Boom)
            {
                _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Special_Boom);
            }

            //同理
            if (currentCardType == PlayCardType.JokerBoom)
            {
                _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_Special_JokerBoom);
            }
        }

        //男人放弃出牌音效
        public static void ManGiveUp(AudioSource _currentAudioPlayer)
        {
            int temp = Random.Range(0, 2);

            if (temp == 0)
            {
                _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_CantAfford);
            }

            if (temp == 1)
            {
                _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_GiveUp);
            }
        }

        //男人最后一张牌警告
        public static void ManLastOneCard(AudioSource _currentAudioPlayer)
        {
            _currentAudioPlayer.PlayOneShot(AudioClipsGit.man_LastOneCard);
        }

        #endregion

        #region 女人音效

        //男人出牌,牌型的音效
        public static void WomanPlayCard(AudioSource _currentAudioPlayer, List<GameObject> _currentPlayCardCombination)
        {
            //获取当前出牌的类型
            PlayCardType currentCardType = PlayCardTypeJudgmentManager.PlayCardTypeJudge(_currentPlayCardCombination);

            //int tempAudio = Random.Range(0, 2);

            ////如果随机数0，则播放男人的“大你”音效片段，反之播放相应的牌型音频
            //if (tempAudio == 0)
            //{
            //    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Bigger);
            //    return;
            //}

            //如果当前出牌类型为单牌，则判断音频库中，男人的单牌音频的对应音频，并播放
            if (currentCardType == PlayCardType.Single)
            {
                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 103)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Single_3);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 104)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Single_4);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 105)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Single_5);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 106)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Single_6);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 107)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Single_7);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 108)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Single_8);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 109)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Single_9);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 110)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Single_10);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 111)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Single_J);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 112)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Single_Q);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 113)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Single_K);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 114)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Single_A);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 119)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Single_2);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 519)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Single_SmallJoker);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 520)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Single_BigJoker);
                }
            }

            //理同上
            if (currentCardType == PlayCardType.Pair)
            {
                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 103)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Pair_3);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 104)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Pair_4);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 105)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Pair_5);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 106)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Pair_6);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 107)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Pair_7);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 108)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Pair_8);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 109)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Pair_9);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 110)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Pair_10);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 111)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Pair_J);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 112)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Pair_Q);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 113)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Pair_K);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 114)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Pair_A);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 119)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Pair_2);
                }
            }

            //理同上
            if (currentCardType == PlayCardType.Triple)
            {
                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 103)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Triple_3);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 104)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Triple_4);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 105)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Triple_5);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 106)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Triple_6);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 107)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Triple_7);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 108)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Triple_8);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 109)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Triple_9);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 110)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Triple_10);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 111)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Triple_J);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 112)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Triple_Q);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 113)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Triple_K);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 114)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Triple_A);
                }

                if (_currentPlayCardCombination[0].GetComponent<CardInformations>().IndexNumber == 119)
                {
                    _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Triple_2);
                }
            }

            //特殊牌型就直接播放
            if (currentCardType == PlayCardType.Triple_single)
            {
                _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Special_TripleSingle);
            }

            //同理
            if (currentCardType == PlayCardType.Triple_pair)
            {
                _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Special_TriplePair);
            }

            //同理
            if (currentCardType == PlayCardType.Straight)
            {
                _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Special_Straight);
            }

            //同理
            if (currentCardType == PlayCardType.Straight_pair)
            {
                _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Special_StraightPair);
            }

            //同理
            if (currentCardType == PlayCardType.Plane_none ||
                currentCardType == PlayCardType.Plane_single ||
                currentCardType == PlayCardType.Plane_pair)
            {
                _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Special_Plane);
            }

            //同理
            if (currentCardType == PlayCardType.Boom_single)
            {
                _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Special_BoomSingle);
            }

            //同理
            if (currentCardType == PlayCardType.Boom_pair)
            {
                _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Special_BoomPair);
            }

            //同理
            if (currentCardType == PlayCardType.Boom)
            {
                _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Special_Boom);
            }

            //同理
            if (currentCardType == PlayCardType.JokerBoom)
            {
                _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_Special_JokerBoom);
            }
        }

        //男人放弃出牌音效
        public static void WomanGiveUp(AudioSource _currentAudioPlayer)
        {
            int temp = Random.Range(0, 2);

            if (temp == 0)
            {
                _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_CantAfford);
            }

            if (temp == 1)
            {
                _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_GiveUp);
            }
        }

        //男人最后一张牌警告
        public static void WomanLastOneCard(AudioSource _currentAudioPlayer)
        {
            _currentAudioPlayer.PlayOneShot(AudioClipsGit.woman_LastOneCard);
        }

        #endregion

        #region 通用音效

        //炸弹，王炸音效
        public static void BoomEffect(AudioSource _currentAudioPlayer)
        {
            _currentAudioPlayer.PlayOneShot(AudioClipsGit.general_BoomEffect);
        }

        //飞机音效
        public static void PlaneEffect(AudioSource _currentAudioPlayer)
        {
            _currentAudioPlayer.PlayOneShot(AudioClipsGit.general_PlaneEffect);
        }

        //发牌音效
        public static void DealCard(AudioSource _currentAudioPlayer) 
        {
            _currentAudioPlayer.PlayOneShot(AudioClipsGit.general_DealCard);
        }

        //选牌音效
        public static void ChooseCard(AudioSource _currentAudioPlayer)
        {
            _currentAudioPlayer.PlayOneShot(AudioClipsGit.general_ChooseCard);
        }

        //出牌音效
        public static void PlayCard(AudioSource _currentAudioPlayer)
        {
            _currentAudioPlayer.PlayOneShot(AudioClipsGit.general_PlayCard);
        }

        //特殊警告音效
        public static void SpecialWarning(AudioSource _currentAudioPlayer)
        {
            _currentAudioPlayer.PlayOneShot(AudioClipsGit.general_DealCard);
        }

        //胜利音效
        public static void Win(AudioSource _currentAudioPlayer)
        {
            _currentAudioPlayer.PlayOneShot(AudioClipsGit.general_Win);
        }

        //失败音效
        public static void Lose(AudioSource _currentAudioPlayer)
        {
            _currentAudioPlayer.PlayOneShot(AudioClipsGit.general_Lose);
        }

        #endregion
    }
}