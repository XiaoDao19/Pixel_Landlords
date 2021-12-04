using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PIXEL.Landlords.Game;
using PIXEL.Landlords.Card;

namespace PIXEL.Landlords.AI
{
    public class CurrentStatu : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

    //待机状态
    public class Statu_Standby : Statues
    {
        private FSM fSM;

        public Statu_Standby(FSM fSM)
        {
            this.fSM = fSM;
        }

        public void OnEnter()
        {
            //Debug.Log("Stand By");    

            //for (int i = 0; i < fSM.currentTurnPlayCard.Count; i++)
            //{
            //    fSM.currentTurnPlayCard.Remove(fSM.currentTurnPlayCard[i]);
            //}
        }

        public void OnUpdate()
        {
            //AI手牌分类
            if (fSM.handCardGit.Count != 0)
            {
                fSM.ClassifyHandCards();
            }

            if (fSM.currentAINumber == 1)
            {
                if (RoundJudgmentManager.Instance.isAiNo1 == true)
                {
                    fSM.SwitchState(AIStatues.WakeUp);
                }
            }

            if (fSM.currentAINumber == 2)
            {
                if (RoundJudgmentManager.Instance.isAiNo2 == true)
                {
                    fSM.SwitchState(AIStatues.WakeUp);
                }
            }
        }

        public void OnExit()
        {

        }
    }

    //唤醒状态
    public class Statu_WakeUp : Statues
    {
        private FSM fSM;

        private float time = 1f;
        //float delayTime = 10f;

        public Statu_WakeUp(FSM fSM)
        {
            this.fSM = fSM;
        }

        public void OnEnter()
        {
            //Debug.Log("Wake Up");
            
        }

        public void OnUpdate()
        {        
            RoundJudgmentManager.Instance.WhenAIFirstHand();

            if (RoundJudgmentManager.Instance.isAiNo1 == true)
            {
                if (RoundJudgmentManager.Instance.transform_Plyer.childCount == 0 && RoundJudgmentManager.Instance.transform_AiNo2.childCount == 0)
                {
                    RoundJudgmentManager.Instance.first_AINo1 = true;

                    for (int i = RoundJudgmentManager.Instance.currentTurnCardList.Count - 1; i >= 0; i--)
                    {
                        RoundJudgmentManager.Instance.currentTurnCardList.Remove(RoundJudgmentManager.Instance.currentTurnCardList[i]);
                    }

                    RoundJudgmentManager.Instance.currentTurnCardList.Clear();

                    RoundJudgmentManager.Instance.currentCardCounts = 0;
                    RoundJudgmentManager.Instance.currentRoundCardPoints = 0;
                    RoundJudgmentManager.Instance.currentRoundCardType = PlayCardType.None;
                }
                else
                {
                    RoundJudgmentManager.Instance.first_AINo1 = false;
                }
            }

            if (RoundJudgmentManager.Instance.isAiNo2 == true)
            {
                if (RoundJudgmentManager.Instance.transform_AiNo1.childCount == 0 && RoundJudgmentManager.Instance.transform_Plyer.childCount == 0)
                {
                    RoundJudgmentManager.Instance.first_AINo2 = true;

                    for (int i = RoundJudgmentManager.Instance.currentTurnCardList.Count - 1; i >= 0; i--)
                    {
                        RoundJudgmentManager.Instance.currentTurnCardList.Remove(RoundJudgmentManager.Instance.currentTurnCardList[i]);
                    }

                    RoundJudgmentManager.Instance.currentTurnCardList.Clear();

                    RoundJudgmentManager.Instance.currentCardCounts = 0;
                    RoundJudgmentManager.Instance.currentRoundCardPoints = 0;
                    RoundJudgmentManager.Instance.currentRoundCardType = PlayCardType.None;
                }
                else
                {
                    RoundJudgmentManager.Instance.first_AINo2 = false;
                }
            }

            //模拟等待时间
            if (time > 0f)
            {
                time -= Time.deltaTime;
            }
            else
            {
                fSM.SwitchState(AIStatues.Judgement);
            }
        }

        public void OnExit()
        {

        }
    }

    //获取当前回合出牌信息
    public class Statu_Judgement : Statues
    {
        private FSM fSM;

        public Statu_Judgement(FSM fSM)
        {
            this.fSM = fSM;
        }

        public void OnEnter()
        {
            //Debug.Log("Judgement Current Turn");
        }

        public void OnUpdate()
        {
            fSM.currentTurnCardList = RoundJudgmentManager.Instance.currentTurnCardList;
            fSM.currentTurnCardType = RoundJudgmentManager.Instance.currentRoundCardType;
            fSM.currentTurnCardCounts = RoundJudgmentManager.Instance.currentCardCounts;
            fSM.currentTurnCardPoints = RoundJudgmentManager.Instance.currentRoundCardPoints;

            fSM.SwitchState(AIStatues.CombineCards);
        }

        public void OnExit()
        {

        }
    }

    //组合手牌状态
    public class Statu_CombineCards : Statues
    {
        private FSM fSM;

        public Statu_CombineCards(FSM fSM)
        {
            this.fSM = fSM;
        }

        public void OnEnter()
        {

        }

        public void OnUpdate()
        {
            float delayTime = Random.Range(0, 17);


            if (fSM.currentAINumber == 1)
            {
                if (RoundJudgmentManager.Instance.isAiNo1 == true)
                {
                    if (RoundJudgmentManager.Instance.first_AINo1 == true)
                    {
                        fSM.Judge_OutCard();

                        for (int i = fSM.temp_AI_First_Hand.Count - 1; i >= 0; i--)
                        {
                            fSM.currentTurnAIPlayCard.Add(fSM.temp_AI_First_Hand[i]);
                        }

                        for (int i = fSM.temp_AI_First_Hand.Count - 1; i >= 0; i--)
                        {
                            fSM.temp_AI_First_Hand.Remove(fSM.temp_AI_First_Hand[i]);
                        }

                        fSM.ReClassify();
                    }
                }
            }

            if (fSM.currentAINumber == 2)
            {
                if (RoundJudgmentManager.Instance.isAiNo2 == true)
                {
                    if (RoundJudgmentManager.Instance.first_AINo2 == true)
                    {
                        fSM.Judge_OutCard();

                        for (int i = fSM.temp_AI_First_Hand.Count - 1; i >= 0; i--)
                        {
                            fSM.currentTurnAIPlayCard.Add(fSM.temp_AI_First_Hand[i]);
                        }

                        for (int i = fSM.temp_AI_First_Hand.Count - 1; i >= 0; i--)
                        {
                            fSM.temp_AI_First_Hand.Remove(fSM.temp_AI_First_Hand[i]);
                        }

                        fSM.ReClassify();
                    }
                }
            }

            //if (RoundJudgmentManager.Instance.currentRoundCardType == PlayCardType.None)
            //{
            //    fSM.Judge_OutCard();

            //    for (int i = fSM.temp_AI_First_Hand.Count - 1; i >= 0; i--)
            //    {
            //        fSM.currentTurnAIPlayCard.Add(fSM.temp_AI_First_Hand[i]);
            //    }

            //    fSM.temp_AI_First_Hand.Clear();

            //    fSM.ReClassify();
            //}

            if (RoundJudgmentManager.Instance.currentRoundCardType == PlayCardType.Single)
            {
                fSM.currentTurnAIPlayCard = fSM.CurrentTurn_Single(fSM.currentTurnCardPoints);
            }

            if (RoundJudgmentManager.Instance.currentRoundCardType == PlayCardType.Pair)
            {
                fSM.currentTurnAIPlayCard = fSM.CurrentTurn_Pair(fSM.currentTurnCardPoints);
            }

            if (RoundJudgmentManager.Instance.currentRoundCardType == PlayCardType.Straight)
            {
                fSM.currentTurnAIPlayCard = fSM.CurrentTurn_Straight(fSM.currentTurnCardPoints, fSM.currentTurnCardCounts);
            }

            if (RoundJudgmentManager.Instance.currentRoundCardType == PlayCardType.Straight_pair)
            {
                fSM.currentTurnAIPlayCard = fSM.CurrentTurn_Straight_Pair(fSM.currentTurnCardPoints, fSM.currentTurnCardCounts);
            }

            if (RoundJudgmentManager.Instance.currentRoundCardType == PlayCardType.Triple)
            {
                fSM.currentTurnAIPlayCard = fSM.CurrentTurn_Triple_None(fSM.currentTurnCardPoints);
            }

            if (RoundJudgmentManager.Instance.currentRoundCardType == PlayCardType.Triple_single)
            {
                fSM.currentTurnAIPlayCard = fSM.CurrentTurn_Triple_Single(fSM.currentTurnCardList);
            }

            if (RoundJudgmentManager.Instance.currentRoundCardType == PlayCardType.Triple_pair)
            {
                fSM.currentTurnAIPlayCard = fSM.CurrentTurn_Triple_Pair(fSM.currentTurnCardList);
            }

            if (RoundJudgmentManager.Instance.currentRoundCardType == PlayCardType.Plane_none)
            {
                fSM.currentTurnAIPlayCard = fSM.CurrentTurn_Plane_None(fSM.currentTurnCardPoints,fSM.currentTurnCardCounts);
            }

            if (RoundJudgmentManager.Instance.currentRoundCardType == PlayCardType.Plane_single)
            {
                fSM.currentTurnAIPlayCard = fSM.CurrentTurn_Plane_Single(fSM.currentTurnCardList);
            }

            if (RoundJudgmentManager.Instance.currentRoundCardType == PlayCardType.Plane_pair)
            {
                fSM.currentTurnAIPlayCard = fSM.CurrentTurn_Plane_Pair(fSM.currentTurnCardList);
            }

            if (RoundJudgmentManager.Instance.currentRoundCardType == PlayCardType.Boom)
            {
                fSM.currentTurnAIPlayCard = fSM.CurrentTurn_Boom(fSM.currentTurnCardPoints);
            }

            if (RoundJudgmentManager.Instance.currentRoundCardType == PlayCardType.Boom_single)
            {
                fSM.currentTurnAIPlayCard = fSM.CurrentTurn_Boom_Single(fSM.currentTurnCardList);
            }

            if (RoundJudgmentManager.Instance.currentRoundCardType == PlayCardType.Boom_pair)
            {
                fSM.currentTurnAIPlayCard = fSM.CurrentTurn_Boom_Pair(fSM.currentTurnCardList);
            }

            if (RoundJudgmentManager.Instance.currentRoundCardType == PlayCardType.JokerBoom)
            {
                fSM.SwitchState(AIStatues.GiveUpPlay);
            }

            if (fSM.currentTurnAIPlayCard != null)
            {
                fSM.ReClassify_Gote();
                fSM.SwitchState(AIStatues.PlayCard);
            }
            else
            {
                fSM.ReClassify_Gote();
                fSM.SwitchState(AIStatues.GiveUpPlay);
            }
        }

        public void OnExit()
        {

        }
    }

    //出牌状态
    public class Statu_PlayCard : Statues
    {
        private FSM fSM;

        public Statu_PlayCard(FSM fSM)
        {
            this.fSM = fSM;
        }

        public void OnEnter()
        {

        }

        public void OnUpdate()
        {
            fSM.OrderCardsList(fSM.currentTurnAIPlayCard);

            PlayCardManager.Instance.AIPlayCard(fSM.currentAINumber, fSM.currentTurnAIPlayCard);

            for (int i = fSM.currentTurnAIPlayCard.Count - 1; i >= 0; i--)
            {
                fSM.currentTurnAIPlayCard.Remove(fSM.currentTurnAIPlayCard[i]);
            }

            fSM.SwitchState(AIStatues.Done);
        }

        public void OnExit()
        {

        }
    }

    //放弃出牌状态
    public class Statu_GiveUpPlay : Statues
    {
        private FSM fSM;

        public Statu_GiveUpPlay(FSM fSM)
        {
            this.fSM = fSM;
        }

        public void OnEnter()
        {
            Debug.Log("不出");
        }

        public void OnUpdate()
        {
            PlayCardManager.Instance.AIGiveUpPlayCard(fSM.currentAINumber);

            fSM.SwitchState(AIStatues.Done);
        }

        public void OnExit()
        {

        }
    }

    //出牌结束状态
    public class Statu_Done : Statues
    {
        private FSM fSM;

        public Statu_Done(FSM fSM)
        {
            this.fSM = fSM;
        }

        public void OnEnter()
        {
            
        }

        public void OnUpdate()
        {
            switch (fSM.currentAINumber)
            {
                case 1:
                    RoundJudgmentManager.Instance.isAiNo1 = !RoundJudgmentManager.Instance.isAiNo1;
                    RoundJudgmentManager.Instance.isAiNo2 = !RoundJudgmentManager.Instance.isAiNo2;
                    break;
                case 2:
                    RoundJudgmentManager.Instance.isAiNo2 = !RoundJudgmentManager.Instance.isAiNo2;
                    RoundJudgmentManager.Instance.isPlayer = !RoundJudgmentManager.Instance.isPlayer;
                    break;
            }

            fSM.SwitchState(AIStatues.Standby);
        }

        public void OnExit()
        {

        }
    }
}