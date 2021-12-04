using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PIXEL.Landlords.FrameWork;
using PIXEL.Landlords.Card;

namespace PIXEL.Landlords.Game
{
    public class RoundJudgmentManager : SingletonPattern<RoundJudgmentManager>
    {
        [Header("回合控制"),Tooltip("用来表示当前轮到谁出牌")]//默认出牌顺序是玩家 --- aiNo1 --- aiNo2
        public bool isPlayer;
        public bool isAiNo1;
        public bool isAiNo2;

        [Header("先手回合")]
        public bool first_AINo1;
        public bool first_AINo2;

        [Header("各个角色的出牌桌子")]
        [HideInInspector] public Transform transform_Plyer;
        [HideInInspector] public Transform transform_AiNo1;
        [HideInInspector] public Transform transform_AiNo2;

        [Header("当前回合最大出牌的牌型信息以及角色信息"), Tooltip("当前回合出牌最大的牌型信息以及角色信息")]
        public List<GameObject> currentTurnCardList;//用来存储当前回合的出牌列表
        public int currentCardCounts;//用一个int值来表示是当前回合牌的数量
        public PlayCardType currentRoundCardType;//表示当前回合最大的牌的牌型
        public int currentRoundCardPoints;//表示当前回合最大的牌的点数

        private void Start()
        {
            transform_Plyer = GameObject.Find("Table_player").transform;
            transform_AiNo1 = GameObject.Find("Table_aiNo1").transform;
            transform_AiNo2 = GameObject.Find("Table_aiNo2").transform;

            isPlayer = true;
        }

        //获取当前传入的AI卡牌列表组合的类型及点数
        public void GetCurrentTrunCardInfos_For_AI(List<GameObject> _targetCardList)
        {
            for (int i = 0; i < _targetCardList.Count; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (_targetCardList[i].GetComponent<CardInformations>().CardValue < _targetCardList[j].GetComponent<CardInformations>().CardValue)
                    {
                        GameObject temp = _targetCardList[i];
                        _targetCardList[i] = _targetCardList[j];
                        _targetCardList[j] = temp;
                    }
                }
            }

            PlayCardType tempPlayeCardType = PlayCardTypeJudgmentManager.PlayCardTypeJudge(_targetCardList);

            int tempPoints = 0;

            for (int i = 0; i < _targetCardList.Count; i++)
            {
                tempPoints += _targetCardList[i].gameObject.GetComponent<CardInformations>().CardValue;
            }

            UpdateCurrentTurnInfos(_targetCardList, tempPlayeCardType, tempPoints, _targetCardList.Count);

            return;
        }

        //获取当前传入的玩家卡牌列表组合的类型及点数
        public void GetCurrentTrunCardInfos_For_Player(List<GameObject> _targetCardList) 
        {
            for (int i = 0; i < _targetCardList.Count; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (_targetCardList[i].GetComponent<CardInformations>().CardValue < _targetCardList[j].GetComponent<CardInformations>().CardValue)
                    {
                        GameObject temp = _targetCardList[i];
                        _targetCardList[i] = _targetCardList[j];
                        _targetCardList[j] = temp;
                    }
                }
            }

            PlayCardType tempPlayeCardType = PlayCardTypeJudgmentManager.PlayCardTypeJudge(_targetCardList);

            int tempPoints = 0;

            for (int i = 0; i < _targetCardList.Count; i++)
            {
                tempPoints += _targetCardList[i].gameObject.GetComponent<CardInformations>().CardValue;
            }

            //int fakeTempPoints = tempPoints;

            //if (tempPlayeCardType == PlayCardType.JokerBoom)
            //{
            //    fakeTempPoints = fakeTempPoints * 10000;
            //}

            //if (tempPlayeCardType == PlayCardType.Boom)
            //{
            //    fakeTempPoints = fakeTempPoints * 1000;
            //}

            //if (tempPlayeCardType == PlayCardType.Boom_pair)
            //{
            //    fakeTempPoints = fakeTempPoints * 100;
            //}

            //if (tempPlayeCardType == PlayCardType.Boom_single)
            //{
            //    fakeTempPoints = fakeTempPoints * 100;
            //}

            if (currentRoundCardType == PlayCardType.None)
            {
                Debug.Log("Current Turn Is None");
                UpdateCurrentTurnInfos(_targetCardList, tempPlayeCardType, tempPoints, _targetCardList.Count);
                return;
            }
            else
            {
                if (JudgementCurrentPoints(_targetCardList) == true)
                {
                    UpdateCurrentTurnInfos(_targetCardList, tempPlayeCardType, tempPoints, _targetCardList.Count);
                }
            }
        }

        //给玩家提供的判断点数方法
        public bool JudgementCurrentPoints(List<GameObject> _targetCardList) 
        {
            //if (currentRoundCardType == PlayCardType.JokerBoom)
            //{
            //    return false;
            //}

            if (PlayCardTypeJudgmentManager.PlayCardTypeJudge(_targetCardList) == currentRoundCardType)
            {
                int tempPoints = 0;

                //当当前回合出牌类型是单牌、对子、三连、飞机不带、顺子、连对时
                if (currentRoundCardType == PlayCardType.Single || currentRoundCardType == PlayCardType.Pair ||
                    currentRoundCardType == PlayCardType.Triple || currentRoundCardType == PlayCardType.Plane_none ||
                    currentRoundCardType == PlayCardType.Straight || currentRoundCardType == PlayCardType.Straight_pair)
                {
                    for (int i = 0; i < _targetCardList.Count; i++)
                    {
                        tempPoints += _targetCardList[i].GetComponent<CardInformations>().CardValue;
                    }

                    if (tempPoints > currentRoundCardPoints)
                    {
                        return true;
                    }
                }

                //当当前回合出牌类型是三带一或三带一对时
                if (currentRoundCardType == PlayCardType.Triple_single || currentRoundCardType == PlayCardType.Triple_pair)
                {
                    for (int i = 0; i < _targetCardList.Count - 2; i++)
                    {
                        if (_targetCardList[i].GetComponent<CardInformations>().CardValue == _targetCardList[i + 1].GetComponent<CardInformations>().CardValue &&
                            _targetCardList[i].GetComponent<CardInformations>().CardValue == _targetCardList[i + 2].GetComponent<CardInformations>().CardValue)
                        {
                            tempPoints += _targetCardList[i].GetComponent<CardInformations>().CardValue * 3;
                            break;
                        }
                    }

                    //建立临时变量，来获取当前回合出牌列表的三带部分的点数
                    int tempCurrentRoundCardPoints = 0;

                    for (int i = 0; i < currentTurnCardList.Count - 2; i++)
                    {
                        if (currentTurnCardList[i].GetComponent<CardInformations>().CardValue == currentTurnCardList[i + 1].GetComponent<CardInformations>().CardValue &&
                           currentTurnCardList[i].GetComponent<CardInformations>().CardValue == currentTurnCardList[i + 2].GetComponent<CardInformations>().CardValue)
                        {
                            tempCurrentRoundCardPoints += currentTurnCardList[i].GetComponent<CardInformations>().CardValue * 3;
                            break;
                        }
                    }

                    if (tempPoints > tempCurrentRoundCardPoints)
                    {
                        return true;
                    }
                }

                //当当前回合出牌类型是飞机带单或飞机带对时
                if (currentRoundCardType == PlayCardType.Plane_single || currentRoundCardType == PlayCardType.Plane_pair)
                {
                    //获取当前回合飞机不带部分
                    for (int i = 0; i < _targetCardList.Count - 2; i++)
                    {
                        if (_targetCardList[i].GetComponent<CardInformations>().CardValue == _targetCardList[i + 1].GetComponent<CardInformations>().CardValue &&
                            _targetCardList[i].GetComponent<CardInformations>().CardValue == _targetCardList[i + 2].GetComponent<CardInformations>().CardValue)
                        {
                            tempPoints += _targetCardList[i].GetComponent<CardInformations>().CardValue * 3;
                            break;
                        }
                    }

                    //建立临时变量，来获取当前回合出牌列表的飞机部分的点数
                    int tempCurrentRoundCardPoints = 0;

                    for (int i = 0; i < currentTurnCardList.Count - 2; i++)
                    {
                        if (currentTurnCardList[i].GetComponent<CardInformations>().CardValue == currentTurnCardList[i + 1].GetComponent<CardInformations>().CardValue &&
                           currentTurnCardList[i].GetComponent<CardInformations>().CardValue == currentTurnCardList[i + 2].GetComponent<CardInformations>().CardValue)
                        {
                            tempCurrentRoundCardPoints += currentTurnCardList[i].GetComponent<CardInformations>().CardValue * 3;
                            break;
                        }
                    }

                    if (tempPoints > tempCurrentRoundCardPoints)
                    {
                        return true;
                    }
                }

                //当当前回合出牌类型是炸弹带单或炸弹带对时
                if (currentRoundCardType == PlayCardType.Boom_single || currentRoundCardType == PlayCardType.Boom_pair)
                {
                    for (int i = 0; i < _targetCardList.Count - 2; i++)
                    {
                        if (_targetCardList[i].GetComponent<CardInformations>().CardValue == _targetCardList[i + 1].GetComponent<CardInformations>().CardValue &&
                            _targetCardList[i].GetComponent<CardInformations>().CardValue == _targetCardList[i + 2].GetComponent<CardInformations>().CardValue &&
                            _targetCardList[i].GetComponent<CardInformations>().CardValue == _targetCardList[i + 3].GetComponent<CardInformations>().CardValue)
                        {
                            tempPoints += _targetCardList[i].GetComponent<CardInformations>().CardValue * 4;
                            break;
                        }
                    }

                    //建立临时变量，来获取当前回合出牌列表的四连部分的点数
                    int tempCurrentRoundCardPoints = 0;

                    for (int i = 0; i < currentTurnCardList.Count - 2; i++)
                    {
                        if (currentTurnCardList[i].GetComponent<CardInformations>().CardValue == currentTurnCardList[i + 1].GetComponent<CardInformations>().CardValue &&
                           currentTurnCardList[i].GetComponent<CardInformations>().CardValue == currentTurnCardList[i + 2].GetComponent<CardInformations>().CardValue &&
                           currentTurnCardList[i].GetComponent<CardInformations>().CardValue == currentTurnCardList[i + 3].GetComponent<CardInformations>().CardValue)
                        {
                            tempCurrentRoundCardPoints += currentTurnCardList[i].GetComponent<CardInformations>().CardValue * 4;
                            break;
                        }
                    }

                    if (tempPoints > tempCurrentRoundCardPoints)
                    {
                        return true;
                    }
                }

                //当当前回合出牌类型是炸弹时
                if (currentRoundCardType == PlayCardType.Boom)
                {
                    tempPoints += _targetCardList[0].GetComponent<CardInformations>().CardValue * 4;

                    if (tempPoints > currentRoundCardPoints)
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (PlayCardTypeJudgmentManager.PlayCardTypeJudge(_targetCardList) == PlayCardType.Boom ||
                    PlayCardTypeJudgmentManager.PlayCardTypeJudge(_targetCardList) == PlayCardType.JokerBoom)
                {
                    Debug.Log(1);
                    return true;
                }
            }

            return false;
        }

        //玩家胜利回合出牌，即玩家赢得上一回合时的下一回合出牌
        public void PlayerSenteTurn(List<GameObject> _targetCardList) 
        {
            PlayCardType tempPlayeCardType = PlayCardType.None;

            int tempPoints = 0;

            tempPlayeCardType = PlayCardTypeJudgmentManager.PlayCardTypeJudge(_targetCardList);

            for (int i = 0; i < _targetCardList.Count; i++)
            {
                tempPoints += _targetCardList[i].gameObject.GetComponent<CardInformations>().CardValue;
            }

            UpdateCurrentTurnInfos(_targetCardList,tempPlayeCardType, tempPoints, _targetCardList.Count);

            isPlayer = !isPlayer;
            isAiNo1 = !isAiNo1;
        }

        //根据上面这个函数判断出来的信息进行更新
        private void UpdateCurrentTurnInfos(List<GameObject> _currentCardList,PlayCardType _currentCardType,int _currentPoints,int _currentCount) 
        {
            for (int i = currentTurnCardList.Count - 1; i >= 0; i--)
            {
                currentTurnCardList.Remove(currentTurnCardList[i]);
            }

            currentCardCounts = 0;
            currentRoundCardPoints = 0;
            currentRoundCardType = PlayCardType.None;

            for (int i = 0; i < _currentCardList.Count; i++)
            {
                currentTurnCardList.Add(_currentCardList[i]);
            }

            currentCardCounts = _currentCount;
            currentRoundCardType = _currentCardType;
            currentRoundCardPoints = _currentPoints;
        }

        //用于AI判断当前先手
        public void WhenAIFirstHand() 
        {
            if (isAiNo1 == true)
            {
                if (transform_Plyer.childCount == 0 && transform_AiNo2.childCount == 0)
                {
                    currentTurnCardList.Clear();
                    currentCardCounts = 0;
                    currentRoundCardType = PlayCardType.None;
                    currentRoundCardPoints = 0;
                }
            }

            if (isAiNo2 == true)
            {
                if (transform_Plyer.childCount == 0 && transform_AiNo1.childCount == 0)
                {
                    currentTurnCardList.Clear();
                    currentCardCounts = 0;
                    currentRoundCardType = PlayCardType.None;
                    currentRoundCardPoints = 0;
                }
            }
        }
    }
}