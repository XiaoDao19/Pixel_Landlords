using UnityEngine;
using System.Collections.Generic;
using PIXEL.Landlords.Card;

namespace PIXEL.Landlords.Game
{
    /// <summary>
    /// 出牌类型：
    /// 0, 无类型 --- None
    /// 1，单牌 --- Single
    /// 2，对子 --- Pair
    /// 3，顺子 --- Straight
    /// 4，连对 --- Straight_pair
    /// 5，三连 --- Triple
    /// 6，三带一 --- Triple_single
    /// 7，三带对 --- Triple_pair
    /// 8，飞机不带 --- Plane_none
    /// 9，飞机带单 --- Plane_single
    /// 10，飞机带对 --- Plane_pair
    /// 11，炸弹 --- Boom
    /// 12，炸弹带单 --- Boom_single
    /// 13，炸弹带对 --- Boom_pair
    /// 14，王炸 --- JokerBoom
    /// </summary>
    public enum PlayCardType
    {
        None,//无类型，即不满足其他所有类型时返回的类型，即不符合出牌规则的类型
        Single, Pair, Straight,
        Straight_pair, Triple, Triple_single,
        Triple_pair, Plane_none, Plane_single,
        Plane_pair, Boom, Boom_single,
        Boom_pair, JokerBoom
    }

    public static class PlayCardTypeJudgmentManager
    {
        public static PlayCardType PlayCardTypeJudge(List<GameObject> _playCardList)
        {
            _playCardList = RankPlayCard(_playCardList);

            if (Single(_playCardList) == PlayCardType.Single)
            {
                return PlayCardType.Single;
            }

            if (Pair(_playCardList) == PlayCardType.Pair)
            {
                return PlayCardType.Pair;
            }

            if (Straight(_playCardList) == PlayCardType.Straight)
            {
                return PlayCardType.Straight;
            }

            if (Straight_pair(_playCardList) == PlayCardType.Straight_pair)
            {
                return PlayCardType.Straight_pair;
            }

            if (Triple(_playCardList) == PlayCardType.Triple)
            {
                return PlayCardType.Triple;
            }

            if (Triple_single(_playCardList) == PlayCardType.Triple_single)
            {
                return PlayCardType.Triple_single;
            }

            if (Triple_pair(_playCardList) == PlayCardType.Triple_pair)
            {
                return PlayCardType.Triple_pair;
            }

            if (Plane_none(_playCardList) == PlayCardType.Plane_none)
            {
                return PlayCardType.Plane_none;
            }

            if (Plane_single(_playCardList) == PlayCardType.Plane_single)
            {
                return PlayCardType.Plane_single;
            }

            if (Plane_pair(_playCardList) == PlayCardType.Plane_pair)
            {
                return PlayCardType.Plane_pair;
            }

            if (Boom(_playCardList) == PlayCardType.Boom)
            {
                return PlayCardType.Boom;
            }

            if (Boom_single(_playCardList) == PlayCardType.Boom_single)
            {
                return PlayCardType.Boom_single;
            }

            if (Boom_pair(_playCardList) == PlayCardType.Boom_pair)
            {
                return PlayCardType.Boom_pair;
            }

            if (JokerBoom(_playCardList) == PlayCardType.JokerBoom)
            {
                return PlayCardType.JokerBoom;
            }

            return PlayCardType.None;
        }

        //单牌（1张）
        private static PlayCardType Single(List<GameObject> _playCardList)
        {
            //当当前出牌数量等于1
            if (_playCardList.Count == 1)
            {
                return PlayCardType.Single;
            }

            return PlayCardType.None;

        }

        //对子（2张）
        private static PlayCardType Pair(List<GameObject> _playCardList)
        {
            //当当前出牌数量不等于2时
            if (_playCardList.Count != 2)
            {
                return PlayCardType.None;
            }

            //判断这两张牌是否相等
            if (_playCardList[0].GetComponent<CardInformations>().CardValue == _playCardList[1].GetComponent<CardInformations>().CardValue)
            {
                return PlayCardType.Pair;
            }

            return PlayCardType.None;
        }

        //顺子（最少5张，最多12张）
        private static PlayCardType Straight(List<GameObject> _playCardList)
        {
            //当当前出牌数量小5，或大于12时
            if (_playCardList.Count < 5 || _playCardList.Count > 12)
            {
                return PlayCardType.None;
            }

            //for循环嵌套判断每张相邻的牌差值是否为1
            for (int i = 0; i < _playCardList.Count; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (_playCardList[j].GetComponent<CardInformations>().CardValue - _playCardList[j + 1].GetComponent<CardInformations>().CardValue != -1)
                    {
                        return PlayCardType.None;
                    }
                }
            }

            return PlayCardType.Straight;
        }

        //连对（最少3对即6张，最多10对即20张（抢地主之后手牌共20张））
        private static PlayCardType Straight_pair(List<GameObject> _playCardList)
        {
            //当当前出牌数量小于6，或大于20时
            if (_playCardList.Count < 6 || _playCardList.Count > 20)
            {
                return PlayCardType.None;
            }

            //无法整除说明_playCardList的数量不是偶数
            if (_playCardList.Count % 2 != 0)
            {
                return PlayCardType.None;
            }

            //因为一次判断两张，所有根据连对牌型数量的特点，只需循环，当前出牌数量除以2再减1的次数
            for (int i = 0; i < (_playCardList.Count / 2) - 1; i += 2)
            {
                //判断13，24位置的牌差值是否等于1
                if (_playCardList[i].GetComponent<CardInformations>().CardValue - _playCardList[i + 2].GetComponent<CardInformations>().CardValue != -1 ||
                    _playCardList[i + 1].GetComponent<CardInformations>().CardValue - _playCardList[i + 3].GetComponent<CardInformations>().CardValue != -1)
                {
                    return PlayCardType.None;
                }
            }

            return PlayCardType.Straight_pair;
        }

        //三连（3张）
        private static PlayCardType Triple(List<GameObject> _playCardList) 
        {
            //当当前出牌数量不等于3
            if (_playCardList.Count != 3)
            {
                return PlayCardType.None;
            }

            //判断这三张牌是否相同
            if (_playCardList[0].GetComponent<CardInformations>().CardValue == _playCardList[1].GetComponent<CardInformations>().CardValue &&
                _playCardList[0].GetComponent<CardInformations>().CardValue == _playCardList[2].GetComponent<CardInformations>().CardValue)
            {
                return PlayCardType.Triple;
            }

            return PlayCardType.None;
        }

        //三带一（4张）
        private static PlayCardType Triple_single(List<GameObject> _playCardList) 
        {
            //当当前出牌数量不等于4
            if (_playCardList.Count != 4)
            {
                return PlayCardType.None;
            }

            //三带一的特点，在排序之后，那张单牌要么在第一张，要么在最后一张
            //当单牌在最后一张时，判断第一张牌是否等于第二张牌等于第三张牌
            if (_playCardList[0].GetComponent<CardInformations>().CardValue == _playCardList[1].GetComponent<CardInformations>().CardValue &&
                _playCardList[0].GetComponent<CardInformations>().CardValue == _playCardList[2].GetComponent<CardInformations>().CardValue &&
                _playCardList[0].GetComponent<CardInformations>().CardValue != _playCardList[3].GetComponent<CardInformations>().CardValue)
            {
                return PlayCardType.Triple_single;
            }

            //当单牌在最前一张时，同上
            if (_playCardList[0].GetComponent<CardInformations>().CardValue != _playCardList[1].GetComponent<CardInformations>().CardValue &&
                _playCardList[1].GetComponent<CardInformations>().CardValue == _playCardList[2].GetComponent<CardInformations>().CardValue &&
                _playCardList[1].GetComponent<CardInformations>().CardValue == _playCardList[3].GetComponent<CardInformations>().CardValue)
            {
                return PlayCardType.Triple_single;
            }

            return PlayCardType.None;
        }

        //三带对（5张）
        private static PlayCardType Triple_pair(List<GameObject> _playCardList)
        {
            //当当前出牌数量不等于5
            if (_playCardList.Count != 5)
            {
                return PlayCardType.None;
            }

            //三带对特点与三带一相同
            //当对子在最后时，判断第一张牌是否等于第二张牌等于第三张牌，且最后两张牌相同
            if (_playCardList[0].GetComponent<CardInformations>().CardValue == _playCardList[1].GetComponent<CardInformations>().CardValue &&
                _playCardList[0].GetComponent<CardInformations>().CardValue == _playCardList[2].GetComponent<CardInformations>().CardValue &&
                _playCardList[0].GetComponent<CardInformations>().CardValue != _playCardList[3].GetComponent<CardInformations>().CardValue &&
                _playCardList[0].GetComponent<CardInformations>().CardValue != _playCardList[4].GetComponent<CardInformations>().CardValue &&
                _playCardList[3].GetComponent<CardInformations>().CardValue == _playCardList[4].GetComponent<CardInformations>().CardValue)
            {
                return PlayCardType.Triple_pair;
            }

            //当对子在最前时，同上
            if (_playCardList[0].GetComponent<CardInformations>().CardValue == _playCardList[1].GetComponent<CardInformations>().CardValue &&
                _playCardList[2].GetComponent<CardInformations>().CardValue != _playCardList[0].GetComponent<CardInformations>().CardValue &&
                _playCardList[2].GetComponent<CardInformations>().CardValue != _playCardList[1].GetComponent<CardInformations>().CardValue &&
                _playCardList[2].GetComponent<CardInformations>().CardValue == _playCardList[3].GetComponent<CardInformations>().CardValue &&
                _playCardList[2].GetComponent<CardInformations>().CardValue == _playCardList[4].GetComponent<CardInformations>().CardValue)
            {
                return PlayCardType.Triple_pair;
            }

            return PlayCardType.None;
        }

        //飞机不带（最少6张即2组，最多18张即6组）
        private static PlayCardType Plane_none(List<GameObject> _playCardList) 
        {
            //当当前出牌数量小于6或大于18，则返回不执行
            if (_playCardList.Count < 6 || _playCardList.Count > 18)
            {
                return PlayCardType.None;
            }

            //如果_playCardList的数量除3除不尽，那就说明_playCardList的数量不满足飞机不带的规则
            if (_playCardList.Count % 3 != 0)
            {
                return PlayCardType.None;
            }

            //因为一次判断三张牌，所以执行次数为，当前出牌数量除以3
            for (int i = 0; i < Plane_None_Counts(_playCardList.Count); i += 3)
            {
                if (_playCardList[i].GetComponent<CardInformations>().CardValue - _playCardList[i + 3].GetComponent<CardInformations>().CardValue != -1 ||
                    _playCardList[i + 1].GetComponent<CardInformations>().CardValue - _playCardList[i + 4].GetComponent<CardInformations>().CardValue != -1 ||
                    _playCardList[i + 2].GetComponent<CardInformations>().CardValue - _playCardList[i + 5].GetComponent<CardInformations>().CardValue != -1)
                {
                    return PlayCardType.None;
                }
            }

            return PlayCardType.Plane_none;
        }

        //因为飞机个数有一个确定范围，所以可以用这种方法，但我在想一种可以通用的方法
        private static int Plane_None_Counts(int _playCardsCounts) 
        {
            switch (_playCardsCounts)
            {
                case 6:
                    return 1;
                case 9:
                    return 4;
                case 12:
                    return 7;
                case 15:
                    return 10;
                case 18:
                    return 13;
            }

            return 0;
        }

        //飞机带单（最少8张即2组，最多20张即5组（抢地主之后手牌共20张））
        private static PlayCardType Plane_single(List<GameObject> _playCardList)
        {
            //当当前出牌数量小于8或大于20，则返回不执行
            if (_playCardList.Count < 8 || _playCardList.Count > 20)
            {
                return PlayCardType.None;
            }

            //如果_playCardList的数量除以2不等于0，则说明_playCardList不是偶数
            if (_playCardList.Count % 4 != 0)
            {
                return PlayCardType.None;
            }

            //常见两个临时变量分别装三连和单牌
            List<GameObject> tempTriple = new List<GameObject>();
            List<GameObject> tempSingle = new List<GameObject>();

            //循环_playCardList将其中相同的牌放入tempTriple列表中
            for (int i = 0; i < _playCardList.Count - 2; i++)
            {
                if (_playCardList[i].GetComponent<CardInformations>().CardValue == _playCardList[i + 1].GetComponent<CardInformations>().CardValue &&
                    _playCardList[i].GetComponent<CardInformations>().CardValue == _playCardList[i + 2].GetComponent<CardInformations>().CardValue)
                {
                    if (tempTriple.Contains(_playCardList[i]) == false)
                    {
                        tempTriple.Add(_playCardList[i]);
                    }

                    if (tempTriple.Contains(_playCardList[i + 1]) == false)
                    {
                        tempTriple.Add(_playCardList[i + 1]);
                    }

                    if (tempTriple.Contains(_playCardList[i + 2]) == false)
                    {
                        tempTriple.Add(_playCardList[i + 2]);
                    }
                }
            }

            //将剩余牌加入tempSingle列表
            for (int i = 0; i < _playCardList.Count; i++)
            {
                if (tempTriple.Contains(_playCardList[i]) == false)
                {
                    tempSingle.Add(_playCardList[i]);
                }
            }

            //判断当tempSingle剩余牌数量等于tempTripl列表数量除3时，则符合规则
            if (tempSingle.Count == tempTriple.Count / 3)
            {
                return PlayCardType.Plane_single;
            }

            return PlayCardType.None;
        }

        //飞机带对（最少10张即2组，最多20张即5组（抢地主之后手牌共20张））
        private static PlayCardType Plane_pair(List<GameObject> _playCardList)
        {
            //当当前出牌数量小于10或大于20，则返回不执行
            if (_playCardList.Count < 10 || _playCardList.Count > 20)
            {
                return PlayCardType.None;
            }

            //当当前出牌数量不能被5整除，则说明不满足飞机带对牌型数量的要求，则返回不执行
            if (_playCardList.Count % 5 != 0)
            {
                return PlayCardType.None;
            }

            //常见两个临时变量分别装三连和对子
            List<GameObject> tempTriple = new List<GameObject>();
            List<GameObject> tempPair = new List<GameObject>();

            //循环_playCardList将其中相同的牌放入tempTriple列表中
            for (int i = 0; i < _playCardList.Count - 2; i++)
            {
                //判断得到三连
                if (_playCardList[i].GetComponent<CardInformations>().CardValue == _playCardList[i + 1].GetComponent<CardInformations>().CardValue &&
                    _playCardList[i].GetComponent<CardInformations>().CardValue == _playCardList[i + 2].GetComponent<CardInformations>().CardValue)
                {
                    //若tempTriple没有当前牌，则放入列表中
                    if (tempTriple.Contains(_playCardList[i]) == false)
                    {
                        tempTriple.Add(_playCardList[i]);
                    }

                    if (tempTriple.Contains(_playCardList[i + 1]) == false)
                    {
                        tempTriple.Add(_playCardList[i + 1]);
                    }

                    if (tempTriple.Contains(_playCardList[i + 2]) == false)
                    {
                        tempTriple.Add(_playCardList[i + 2]);
                    }
                }
            }

            //将剩余牌中不存在于tempTriple临时列表中的牌放入对子列表中
            for (int i = 0; i < _playCardList.Count; i++)
            {
                if (tempTriple.Contains(_playCardList[i]) == false)
                {
                    tempPair.Add(_playCardList[i]);
                }
            }

            //如果tempPair的数量不等于tempTriple.Count / 3 * 2，也就是没有相应对数的对子
            if (tempPair.Count != tempTriple.Count / 3 * 2)
            {
                return PlayCardType.None;
            }

            //循环tempTriple临时列表判断其中是否存在不满足对子的牌
            for (int i = 0; i < tempPair.Count / 2; i += 2)
            {
                if (tempPair[i].GetComponent<CardInformations>().CardValue != tempPair[i + 1].GetComponent<CardInformations>().CardValue)
                {
                    return PlayCardType.None;
                }
            }

            return PlayCardType.Plane_pair;
        }

        //炸弹（4张）
        private static PlayCardType Boom(List<GameObject> _playCardList)
        {
            //当当前出牌数量不等于4则返回不执行
            if (_playCardList.Count != 4)
            {
                return PlayCardType.None;
            }

            //当四张牌的牌点数都相等，则满足条件
            if (_playCardList[0].GetComponent<CardInformations>().CardValue == _playCardList[1].GetComponent<CardInformations>().CardValue &&
                _playCardList[0].GetComponent<CardInformations>().CardValue == _playCardList[2].GetComponent<CardInformations>().CardValue &&
                _playCardList[0].GetComponent<CardInformations>().CardValue == _playCardList[3].GetComponent<CardInformations>().CardValue)
            {
                return PlayCardType.Boom;
            }

            return PlayCardType.None;
        }

        //四带两单（6张）
        private static PlayCardType Boom_single(List<GameObject> _playCardList)
        {
            //当当前出牌数量不等于6则返回不执行
            if (_playCardList.Count != 6)
            {
                return PlayCardType.None;
            }

            //因为一次判断四个，所以只需要执行3次
            for (int i = 0; i < _playCardList.Count / 2; i++)
            {
                if (_playCardList[i].GetComponent<CardInformations>().CardValue == _playCardList[i + 1].GetComponent<CardInformations>().CardValue &&
                    _playCardList[i].GetComponent<CardInformations>().CardValue == _playCardList[i + 2].GetComponent<CardInformations>().CardValue &&
                    _playCardList[i].GetComponent<CardInformations>().CardValue == _playCardList[i + 3].GetComponent<CardInformations>().CardValue)
                {
                    return PlayCardType.Boom_single;
                }
            }

            return PlayCardType.None;
        }

        //四带两对（8张）
        private static PlayCardType Boom_pair(List<GameObject> _playCardList)
        {
            //当当前出牌数量不等于8则返回不执行
            if (_playCardList.Count != 8)
            {
                return PlayCardType.None;
            }

            //临时列表，分别用来装炸弹，和对子
            List<GameObject> tempFour = new List<GameObject>();
            List<GameObject> tempPair = new List<GameObject>();

            //因为一次判断四个，所以只需要执行3次
            for (int i = 0; i < _playCardList.Count / 2 + 1; i++)
            {
                if (_playCardList[i].GetComponent<CardInformations>().CardValue == _playCardList[i + 1].GetComponent<CardInformations>().CardValue &&
                    _playCardList[i].GetComponent<CardInformations>().CardValue == _playCardList[i + 2].GetComponent<CardInformations>().CardValue &&
                    _playCardList[i].GetComponent<CardInformations>().CardValue == _playCardList[i + 3].GetComponent<CardInformations>().CardValue)
                {
                    tempFour.Add(_playCardList[i]);
                    tempFour.Add(_playCardList[i + 1]);
                    tempFour.Add(_playCardList[i + 2]);
                    tempFour.Add(_playCardList[i + 3]);
                }
            }

            //将剩余卡牌放入tempPair临时列表中
            for (int i = 0; i < _playCardList.Count; i++)
            {
                if (tempFour.Contains(_playCardList[i]) == false)
                {
                    tempPair.Add(_playCardList[i]);
                }
            }

            //如果tempPair中的卡牌两两相等，则满足条件
            if (tempPair[0].GetComponent<CardInformations>().CardValue == tempPair[1].GetComponent<CardInformations>().CardValue &&
                tempPair[2].GetComponent<CardInformations>().CardValue == tempPair[3].GetComponent<CardInformations>().CardValue)
            {
                return PlayCardType.Boom_pair;
            }

            return PlayCardType.None;
        }

        //王炸（两张）
        private static PlayCardType JokerBoom(List<GameObject> _playCardList)
        {
            //当当前出牌数量不等于二则返回不执行
            if (_playCardList.Count != 2)
            {
                return PlayCardType.None;
            }

            //判断这两张牌的牌点数是否为519，520即可
            if (_playCardList[0].GetComponent<CardInformations>().CardValue == 519 &&
                _playCardList[1].GetComponent<CardInformations>().CardValue == 520 ||
                _playCardList[0].GetComponent<CardInformations>().CardValue == 520 &&
                _playCardList[1].GetComponent<CardInformations>().CardValue == 519)
            {
                return PlayCardType.JokerBoom;
            }

            return PlayCardType.None;
        }

        //出牌列表排序（升序）
        private static List<GameObject> RankPlayCard(List<GameObject> _playCardList)
        {
            for (int i = 0; i < _playCardList.Count; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (_playCardList[j].GetComponent<CardInformations>().CardValue > _playCardList[j + 1].GetComponent<CardInformations>().CardValue)
                    {
                        GameObject temp = _playCardList[j];
                        _playCardList[j] = _playCardList[j + 1];
                        _playCardList[j + 1] = temp;
                    }
                }
            }

            return _playCardList;
        }
    }
}
