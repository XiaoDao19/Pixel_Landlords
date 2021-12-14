using System.Collections.Generic;
using UnityEngine;
using PIXEL.Landlords.Game;
using PIXEL.Landlords.Card;
using System.Collections;

namespace PIXEL.Landlords.AI
{
    public enum AIStatues 
    {
        //待机  苏醒   判断比较  组合牌       出牌     不出       出牌结束
        Standby,WakeUp,Judgement,CombineCards,PlayCard,GiveUpPlay,Done
    }

    public class FSM : MonoBehaviour
    {
        //状态接口
        public Statues statues;

        //字典存储状态和AI状态
        private Dictionary<AIStatues, Statues> AIStatues = new Dictionary<AIStatues, Statues>();

        public List<GameObject> handCardGit;//当前AI总牌库

        //分类牌库
        public List<GameObject> singleList = new List<GameObject>();
        public List<GameObject> pairList = new List<GameObject>();
        public List<GameObject> tripleList = new List<GameObject>();
        public List<GameObject> boomList = new List<GameObject>();
        public List<GameObject> jokerBoomList = new List<GameObject>();

        //当前回合信息，即当前回合出牌类型、张数、点数
        [HideInInspector] public List<GameObject> currentTurnCardList;
        [HideInInspector] public PlayCardType currentTurnCardType;
        [HideInInspector] public int currentTurnCardCounts;
        [HideInInspector] public int currentTurnCardPoints;

        //当前出牌列表
        public List<GameObject> currentTurnAIPlayCard = new List<GameObject>();

        public int currentAINumber;//ai编号，用于分辨是哪个ai

        //临时列表用来承接飞机不带
        private List<GameObject> temp_plane_none = new List<GameObject>();

        //临时列表用来承接当前回合符合条件的飞机不带
        private List<GameObject> tempConformPlaneNone = new List<GameObject>();

        //临时列表用来承接飞机个数
        private List<GameObject> temp_plane_counts = new List<GameObject>();

        //临时列表用来承接---AI先手出牌的手牌
        public List<GameObject> temp_AI_First_Hand = new List<GameObject>();

        [Header("AI对应音效播放器")]
        public AudioSource aiNo1AudioPlayer;
        public AudioSource aiNo2AudioPlayer;
        public AudioSource generalAudioSource;
        void Start()
        {
            //添加状态
            AIStatues.Add(AI.AIStatues.Standby, new Statu_Standby(this));
            AIStatues.Add(AI.AIStatues.WakeUp, new Statu_WakeUp(this));
            AIStatues.Add(AI.AIStatues.Judgement, new Statu_Judgement(this));
            AIStatues.Add(AI.AIStatues.CombineCards, new Statu_CombineCards(this));
            AIStatues.Add(AI.AIStatues.PlayCard, new Statu_PlayCard(this));
            AIStatues.Add(AI.AIStatues.GiveUpPlay, new Statu_GiveUpPlay(this));
            AIStatues.Add(AI.AIStatues.Done, new Statu_Done(this));

            //获取对应ai的音效播放器
            aiNo1AudioPlayer = DealCardManager.Instance.aiNo1Hand.GetComponent<AudioSource>();
            aiNo2AudioPlayer = DealCardManager.Instance.aiNo2Hand.GetComponent<AudioSource>();
            generalAudioSource = DealCardManager.Instance.aiNo1Hand.parent.GetComponent<AudioSource>();

            //将对应AI手牌添加到handCardGit
            if (gameObject.name == "Hand_aiNo1")
            {
                handCardGit = DealCardManager.Instance.aiNo1HandCards;
                currentAINumber = 1;
            }
            else if (gameObject.name == "Hand_aiNo2")
            {
                handCardGit = DealCardManager.Instance.aiNo2HandCards;
                currentAINumber = 2;
            }

            //设置初始状态
            SwitchState(AI.AIStatues.Standby);
        }

        void Update()
        {

        }

        private void FixedUpdate()
        {
            statues.OnUpdate();
        }

        //状态切换
        public void SwitchState(AIStatues aIStatues)
        {
            //如果当前ai有状态，则直接退出当前状态
            if (statues != null)
            {
                statues.OnExit();
            }

            //再将当前ai状态设置为指定状态
            statues = AIStatues[aIStatues];
            statues.OnEnter();
        }


        //将当前AI手牌分类
        #region 手牌分类
        public void ClassifyHandCards()
        {
            if (handCardGit.Count != 0 || handCardGit != null)
            {
                OrderCardsList(handCardGit);
            }

            //添加炸弹牌
            Classify_Boom();


            //添加三连
            Classify_Triple();


            //添加对子
            Classify_Pair();


            //添加王炸
            Classify_Joker();


            //添加单牌
            Classify_Single();

            OrderCardsList(jokerBoomList);
            OrderCardsList(boomList);
            OrderCardsList(tripleList);
            OrderCardsList(pairList);
            OrderCardsList(singleList);
        }

        //在做分类的时候，我忽略掉了一个点，就是，List泛型集合有一个特点就是：每当你从中移除某个元素的时候，后面的元素就会自动往前填补
        //所以，我在同一个List里一边进行添加，一边进行移除的时候，其实是会有一个元素没有被检查到的，因为我移除一个元素之后，后面的元素往前补位，但是循环的次数却一直往前增加，所以就跳过了补位的那个元素
        private void Classify_Boom()
        {
            OrderCardsList(handCardGit);

            //添加炸弹牌
            ///先添加
            for (int i = 0; i < handCardGit.Count - 3; i++)
            {
                if (handCardGit[i].gameObject.GetComponent<CardInformations>().CardValue == handCardGit[i + 1].gameObject.GetComponent<CardInformations>().CardValue &&
                    handCardGit[i].gameObject.GetComponent<CardInformations>().CardValue == handCardGit[i + 2].gameObject.GetComponent<CardInformations>().CardValue &&
                    handCardGit[i].gameObject.GetComponent<CardInformations>().CardValue == handCardGit[i + 3].gameObject.GetComponent<CardInformations>().CardValue)
                {
                    if (boomList.Contains(handCardGit[i]) == false)
                    {
                        boomList.Add(handCardGit[i]);
                    }

                    if (boomList.Contains(handCardGit[i + 1]) == false)
                    {
                        boomList.Add(handCardGit[i + 1]);
                    }

                    if (boomList.Contains(handCardGit[i + 2]) == false)
                    {
                        boomList.Add(handCardGit[i + 2]);
                    }

                    if (boomList.Contains(handCardGit[i + 3]) == false)
                    {
                        boomList.Add(handCardGit[i + 3]);
                    }
                }
            }

            OrderCardsList(boomList);

            ///再从手牌库中移除
            for (int i = boomList.Count - 1; i >= 0 ; i--)
            {
                if (handCardGit.Contains(boomList[i]) == true)
                {
                    handCardGit.Remove(boomList[i]);
                }
            }

            OrderCardsList(boomList);
            OrderCardsList(handCardGit);
        }

        private void Classify_Triple()
        {
            OrderCardsList(handCardGit);

            //添加三连
            for (int i = 0; i < handCardGit.Count - 2; i++)
            {
                if (handCardGit[i].gameObject.GetComponent<CardInformations>().CardValue == handCardGit[i + 1].gameObject.GetComponent<CardInformations>().CardValue &&
                    handCardGit[i].gameObject.GetComponent<CardInformations>().CardValue == handCardGit[i + 2].gameObject.GetComponent<CardInformations>().CardValue)
                {
                    if (tripleList.Contains(handCardGit[i]) == false)
                    {
                        tripleList.Add(handCardGit[i]);
                    }

                    if (tripleList.Contains(handCardGit[i + 1]) == false)
                    {
                        tripleList.Add(handCardGit[i + 1]);
                    }

                    if (tripleList.Contains(handCardGit[i + 2]) == false)
                    {
                        tripleList.Add(handCardGit[i + 2]);
                    }
                }
            }

            OrderCardsList(tripleList);

            for (int i = tripleList.Count - 1; i >= 0 ; i--)
            {
                if (handCardGit.Contains(tripleList[i]) == true)
                {
                    handCardGit.Remove(tripleList[i]);
                }
            }

            OrderCardsList(tripleList);
            OrderCardsList(handCardGit);
        }

        private void Classify_Pair()
        {
            OrderCardsList(handCardGit);

            for (int i = 0; i < handCardGit.Count - 1; i++)
            {
                if (handCardGit[i].gameObject.GetComponent<CardInformations>().CardValue == handCardGit[i + 1].gameObject.GetComponent<CardInformations>().CardValue)
                {
                    if (pairList.Contains(handCardGit[i]) == false)
                    {
                        pairList.Add(handCardGit[i]);
                    }

                    if (pairList.Contains(handCardGit[i + 1]) == false)
                    {
                        pairList.Add(handCardGit[i + 1]);
                    }
                }
            }

            OrderCardsList(pairList);

            for (int i = pairList.Count - 1; i >= 0; i--)
            {
                if (handCardGit.Contains(pairList[i]) == true)
                {
                    handCardGit.Remove(pairList[i]);
                }
            }

            OrderCardsList(pairList);
            OrderCardsList(handCardGit);
        }

        private void Classify_Joker()
        {
            OrderCardsList(handCardGit);

            //添加王炸
            for (int i = 0; i < handCardGit.Count - 1; i++)
            {
                if (handCardGit[i].gameObject.GetComponent<CardInformations>().CardValue == 519 &&
                    handCardGit[i + 1].gameObject.GetComponent<CardInformations>().CardValue == 520 ||
                    handCardGit[i].gameObject.GetComponent<CardInformations>().CardValue == 520 &&
                    handCardGit[i + 1].gameObject.GetComponent<CardInformations>().CardValue == 519)
                {
                    jokerBoomList.Add(handCardGit[i]);
                    jokerBoomList.Add(handCardGit[i + 1]);

                    break;
                }
            }

            OrderCardsList(jokerBoomList);

            for (int i = jokerBoomList.Count - 1; i >= 0 ; i--)
            {
                if (handCardGit.Contains(jokerBoomList[i]) == true)
                {
                    handCardGit.Remove(jokerBoomList[i]);
                }
            }

            OrderCardsList(jokerBoomList);
            OrderCardsList(handCardGit);
        }

        private void Classify_Single()
        {
            OrderCardsList(handCardGit);

            //添加单牌
            for (int i = 0; i < handCardGit.Count; i++)
            {
                if (singleList.Contains(handCardGit[i]) == false)
                {
                    singleList.Add(handCardGit[i]);
                }
            }

            OrderCardsList(singleList);

            for (int i = handCardGit.Count - 1; i >= 0 ; i--)
            {
                handCardGit.Remove(handCardGit[i]);
            }

            OrderCardsList(singleList);
        }
        #endregion


        //升序排列
        public void OrderCardsList(List<GameObject> _targetList)
        {
            for (int i = 0; i < _targetList.Count; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (_targetList[i].GetComponent<CardInformations>().CardValue < _targetList[j].GetComponent<CardInformations>().CardValue)
                    {
                        GameObject temp = _targetList[i];
                        _targetList[i] = _targetList[j];
                        _targetList[j] = temp;
                    }
                }
            }
        }


        #region AI先手出牌算法判断

        public List<GameObject> Judge_OutCard() 
        {
            ReClassify();

            Judge_Plane_Pair();
            if (temp_AI_First_Hand != null && temp_AI_First_Hand.Count != 0)
            {
                return temp_AI_First_Hand;
            }

            ReClassify();

            Judge_Plane_Single();
            if (temp_AI_First_Hand != null && temp_AI_First_Hand.Count != 0)
            {
                return temp_AI_First_Hand;
            }

            ReClassify();

            Judge_Plane_None();
            if (temp_AI_First_Hand != null && temp_AI_First_Hand.Count != 0)
            {
                return temp_AI_First_Hand;
            }

            ReClassify();

            Judge_Straight_Pair();
            if (temp_AI_First_Hand != null && temp_AI_First_Hand.Count != 0)
            {
                return temp_AI_First_Hand;
            }

            ReClassify();

            Judge_Straight_Single();
            if (temp_AI_First_Hand != null && temp_AI_First_Hand.Count != 0)
            {
                return temp_AI_First_Hand;
            }

            ReClassify();

            Judge_Boom_Pair();
            if (temp_AI_First_Hand != null && temp_AI_First_Hand.Count != 0)
            {
                return temp_AI_First_Hand;
            }

            ReClassify();

            Judge_Boom_Single();
            if (temp_AI_First_Hand != null && temp_AI_First_Hand.Count != 0)
            {
                return temp_AI_First_Hand;
            }

            ReClassify();

            Judge_Boom();
            if (temp_AI_First_Hand != null && temp_AI_First_Hand.Count != 0)
            {
                return temp_AI_First_Hand;
            }

            ReClassify();

            Judge_Triple_Pair();
            if (temp_AI_First_Hand != null && temp_AI_First_Hand.Count != 0)
            {
                return temp_AI_First_Hand;
            }

            ReClassify();

            Judge_Triple_Single();
            if (temp_AI_First_Hand != null && temp_AI_First_Hand.Count != 0)
            {
                return temp_AI_First_Hand;
            }

            ReClassify();

            Judge_Triple_None();
            if (temp_AI_First_Hand != null && temp_AI_First_Hand.Count != 0)
            {
                return temp_AI_First_Hand;
            }

            ReClassify();

            Judge_JokerBoom();
            if (temp_AI_First_Hand != null && temp_AI_First_Hand.Count != 0)
            {
                return temp_AI_First_Hand;
            }

            ReClassify();

            Judge_Pair();
            if (temp_AI_First_Hand != null && temp_AI_First_Hand.Count != 0)
            {
                return temp_AI_First_Hand;
            }

            ReClassify();

            Judge_Single();
            if (temp_AI_First_Hand != null && temp_AI_First_Hand.Count != 0)
            {
                return temp_AI_First_Hand;
            }

            ReClassify();

            return null;
        }

        //1，判断---飞机带对
        private void Judge_Plane_Pair()
        {
            //如果三连库为空，不执行
            if (tripleList == null)
            {
                return;
            }

            //如果三连库的个数小于6，不执行
            if (tripleList.Count < 6)
            {
                return;
            }

            //获取当前回合飞机个数
            GetCurrentPlaneCounts();

            //如果没有获取到飞机，则不执行
            if (temp_plane_counts == null || temp_plane_counts.Count < 6)
            {
                ReClassify();
                return;
            }

            //获取相应飞机个数的对子个数
            //如果对子库的牌数量加上三连库的牌数量 / 3 * 2，小于当前飞机需要的对子数量，则不执行，并重新分类各个牌库
            if (pairList.Count + tripleList.Count / 3 * 2 < temp_plane_counts.Count / 3 * 2)
            {
                ReClassify();
                return;
            }

            //若满足条件，则将得到的飞机放入当前出牌库
            for (int i = 0; i < temp_plane_counts.Count; i++)
            {
                temp_AI_First_Hand.Add(temp_plane_counts[i]);
            }

            //将添加了的飞机从飞机库移除
            for (int i = temp_AI_First_Hand.Count - 1; i >= 0 ; i--)
            {
                if (temp_plane_counts.Contains(temp_AI_First_Hand[i]) == true)
                {
                    temp_plane_counts.Remove(temp_AI_First_Hand[i]);
                }
            }

            //当对子库的牌数量小于当前回合飞机需要的对子数量时，拆三连库
            if (pairList.Count < temp_plane_counts.Count / 3 * 2)
            {
                for (int i = 0; i < tripleList.Count - 2; i+=3)
                {
                    if (pairList.Contains(tripleList[i]) == false)
                    {
                        pairList.Add(tripleList[i]);
                        pairList.Add(tripleList[i + 1]);

                        //singleList.Add(tripleList[i + 2]);
                    }
                }

                //重新排序对子库
                OrderCardsList(pairList);

                //从三连库中移除添加过的牌
                for (int i = pairList.Count - 1; i >= 0 ; i--)
                {
                    if (tripleList.Contains(pairList[i]) == true)
                    {
                        tripleList.Remove(pairList[i]);
                    }
                }

                //for (int i = singleList.Count - 1; i >= 0 ; i--)
                //{
                //    if (tripleList.Contains(singleList[i]) == true)
                //    {
                //        tripleList.Remove(singleList[i]);
                //    }
                //}
            }

            //再从最小的开始添加
            //循环次数就是当前回合飞机需要的对子数量
            for (int i = 0; i < temp_plane_counts.Count / 3 * 2; i++)
            {
                temp_AI_First_Hand.Add(pairList[i]);
            }

            //将添加了的对子从对子库中移除
            for (int i = temp_AI_First_Hand.Count - 1; i >= 0 ; i--)
            {
                if (pairList.Contains(temp_AI_First_Hand[i]) == true)
                {
                    pairList.Remove(temp_AI_First_Hand[i]);
                }
            }
        }

        //2，判断---飞机带单
        private void Judge_Plane_Single()   
        {
            ///与飞机带对同理

            if (tripleList == null)
            {
                return;
            }

            if (tripleList.Count < 6)
            {
                return;
            }

            GetCurrentPlaneCounts();

            if (temp_plane_counts == null || temp_plane_counts.Count < 6)
            {
                ReClassify();
                return;
            }

            if (singleList.Count + pairList.Count / 2 < temp_plane_counts.Count / 3)
            {
                ReClassify();
                return;
            }

            for (int i = 0; i < temp_plane_counts.Count; i++)
            {
                temp_AI_First_Hand.Add(temp_plane_counts[i]);
            }

            for (int i = temp_AI_First_Hand.Count - 1; i >= 0 ; i--)
            {
                if (temp_plane_counts.Contains(temp_AI_First_Hand[i]) == true)
                {
                    temp_plane_counts.Remove(temp_AI_First_Hand[i]);
                }
            }

            //如果单牌库个数小于当前回合飞机需要的个数，则拆对子库
            if (singleList.Count < temp_plane_counts.Count / 3)
            {
                for (int i = 0; i < pairList.Count - 1; i += 2)
                {
                    if (singleList.Contains(pairList[i]) == false)
                    {
                        singleList.Add(pairList[i]);
                    }
                }

                OrderCardsList(singleList);

                for (int i = singleList.Count - 1; i >= 0; i--)
                {
                    if (pairList.Contains(singleList[i]) == true)
                    {
                        pairList.Remove(singleList[i]);
                    }
                }
            }

            //然后同理，从最小的开始获取
            //循环次数就是当前飞机需要的单牌个数
            for (int i = 0; i < temp_plane_counts.Count / 3; i++)
            {
                temp_AI_First_Hand.Add(singleList[i]);
            }

            for (int i = temp_AI_First_Hand.Count - 1; i >= 0 ; i--)
            {
                if (singleList.Contains(temp_AI_First_Hand[i]) == true)
                {
                    singleList.Remove(temp_AI_First_Hand[i]);
                }
            }
        }

        //3，判断---飞机不带
        private void Judge_Plane_None()
        {
            if (tripleList == null)
            {
                return;
            }

            if (tripleList.Count < 6)
            {
                return;
            }

            GetCurrentPlaneCounts();

            if (temp_plane_counts == null || temp_plane_counts.Count < 6)
            {
                ReClassify();
                return;
            }

            for (int i = 0; i < temp_plane_counts.Count; i++)
            {
                temp_AI_First_Hand.Add(temp_plane_counts[i]);
            }

            for (int i = temp_AI_First_Hand.Count - 1; i >= 0 ; i--)
            {
                if (temp_plane_counts.Contains(temp_AI_First_Hand[i]) == true)
                {
                    temp_plane_counts.Remove(temp_AI_First_Hand[i]);
                }
            }
        }

        //4，判断---连对
        private void Judge_Straight_Pair()
        {
            //判断，如果对子库的牌数量加上三连库对子的数量，小于连对的最低数量6时，则不执行
            if (pairList.Count + tripleList.Count / 3 * 2 < 6)
            {
                return;
            }

            //如果对子库数量小于连对的最低数量6时，拆三连库
            for (int i = 0; i < tripleList.Count - 2; i += 3)
            {
                if (pairList.Contains(tripleList[i]) == false)
                {
                    pairList.Add(tripleList[i]);
                    pairList.Add(tripleList[i + 1]);

                    //singleList.Add(tripleList[i + 2]);
                }
            }

            //重新排序对子库
            OrderCardsList(pairList);

            //从三连库中移除添加过的牌
            for (int i = pairList.Count - 1; i >= 0; i--)
            {
                if (tripleList.Contains(pairList[i]) == true)
                {
                    tripleList.Remove(pairList[i]);
                }
            }

            //判断连对
            if (pairList.Count >= 6)
            {
                for (int i = 0; i < pairList.Count - 4; i += 2)
                {
                    //若ai临时出牌库有至少6张连对，则只判断最后四张牌是否构成连对，若满足则添加到ai临时出牌库，反之，跳过
                    if (temp_AI_First_Hand.Count >= 6)
                    {
                        if (temp_AI_First_Hand[temp_AI_First_Hand.Count - 2].GetComponent<CardInformations>().CardValue - pairList[i].GetComponent<CardInformations>().CardValue == -1 &&
                            temp_AI_First_Hand[temp_AI_First_Hand.Count - 1].GetComponent<CardInformations>().CardValue - pairList[i + 1].GetComponent<CardInformations>().CardValue == -1)
                        {
                            temp_AI_First_Hand.Add(pairList[i]);
                            temp_AI_First_Hand.Add(pairList[i + 1]);

                            continue;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    //判断得到最少6张连对，并加入ai临时出牌库
                    if (pairList[i].GetComponent<CardInformations>().CardValue - pairList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 1].GetComponent<CardInformations>().CardValue - pairList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 2].GetComponent<CardInformations>().CardValue - pairList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 3].GetComponent<CardInformations>().CardValue - pairList[i + 5].GetComponent<CardInformations>().CardValue == -1)
                    {
                        temp_AI_First_Hand.Add(pairList[i]);
                        temp_AI_First_Hand.Add(pairList[i + 1]);
                        temp_AI_First_Hand.Add(pairList[i + 2]);
                        temp_AI_First_Hand.Add(pairList[i + 3]);
                        temp_AI_First_Hand.Add(pairList[i + 4]);
                        temp_AI_First_Hand.Add(pairList[i + 5]);
                    }
                }
            }

            //移除添加的对子
            for (int i = temp_AI_First_Hand.Count - 1; i >= 0; i--)
            {
                if (pairList.Contains(temp_AI_First_Hand[i]) == true)
                {
                    pairList.Remove(temp_AI_First_Hand[i]);
                }
            }
        }

        //5，判断---顺子
        private void Judge_Straight_Single()
        {
            ///与连对同理

            if (singleList.Count + pairList.Count / 2 < 5)
            {
                return;
            }

            for (int i = 0; i < pairList.Count - 1; i += 2)
            {
                if (singleList.Contains(pairList[i]) == false)
                {
                    singleList.Add(pairList[i]);
                }
            }

            OrderCardsList(singleList);

            for (int i = singleList.Count - 1; i >= 0; i--)
            {
                if (pairList.Contains(singleList[i]) == true)
                {
                    pairList.Remove(singleList[i]);
                }
            }

            //for循环判断顺子
            for (int i = 0; i < singleList.Count - 1; i++)
            {
                //若ai临时出牌库的数量大于等于2，则只比较最后两张牌是否满足顺子条件
                if (temp_AI_First_Hand.Count >= 2)
                {
                    if (temp_AI_First_Hand[temp_AI_First_Hand.Count - 1].GetComponent<CardInformations>().CardValue - singleList[i + 1].GetComponent<CardInformations>().CardValue == -1)
                    {
                        if (temp_AI_First_Hand.Contains(singleList[i]) == false)
                        {
                            temp_AI_First_Hand.Add(singleList[i]);

                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }

                //依次判断单牌库的两张牌之间的牌面值是否满足顺子条件，若满足，则添加到ai临时出牌库
                if (singleList[i].GetComponent<CardInformations>().CardValue - singleList[i+1].GetComponent<CardInformations>().CardValue == -1)
                {
                    if (temp_AI_First_Hand.Contains(singleList[i]) == false)
                    {
                        temp_AI_First_Hand.Add(singleList[i]);
                    }

                    if (temp_AI_First_Hand.Contains(singleList[i + 1]) == false)
                    {
                        temp_AI_First_Hand.Add(singleList[i + 1]);
                    }
                }
            }

            OrderCardsList(temp_AI_First_Hand);

            for (int i = temp_AI_First_Hand.Count - 1; i >= 0; i--)
            {
                if (singleList.Contains(temp_AI_First_Hand[i]) == true)
                {
                    singleList.Remove(temp_AI_First_Hand[i]);
                }
            }

            //若最后ai临时出牌库的数量小于5张，则不满足顺子，则将其全部退回到单牌库
            if (temp_AI_First_Hand.Count < 5)
            {
                for (int i = temp_AI_First_Hand.Count - 1; i >= 0 ; i--)
                {
                    singleList.Add(temp_AI_First_Hand[i]);
                }

                for (int i = temp_AI_First_Hand.Count - 1; i >= 0; i--)
                {
                    temp_AI_First_Hand.Remove(temp_AI_First_Hand[i]);
                }
            }
        }

        //6，判断---四带对
        private void Judge_Boom_Pair()
        {
            //同理
            if (boomList == null || boomList.Count < 4)
            {
                return;
            }

            //因为四带对需要带两对，也就是四张牌，所以只需要判断对子库的牌数量加三连库的对子牌数量是否大于4
            //若小于4，则不执行
            if (pairList.Count + tripleList.Count / 3 * 2 < 4)
            {
                return;
            }

            //添加炸弹
            for (int i = 0; i < 4; i++)
            {
                temp_AI_First_Hand.Add(boomList[i]);
            }

            //移除已经添加的单牌
            for (int i = temp_AI_First_Hand.Count - 1; i >=0 ; i--)
            {
                if (boomList.Contains(temp_AI_First_Hand[i]) == true)
                {
                    boomList.Remove(temp_AI_First_Hand[i]);
                }
            }

            //若对字库的牌数量小于4，则拆三连库
            if (pairList.Count < 4)
            {
                for (int i = 0; i < tripleList.Count - 2; i+=3)
                {
                    if (pairList.Contains(tripleList[i]) == false)
                    {
                        pairList.Add(tripleList[i]);
                        pairList.Add(tripleList[i + 1]);

                        //singleList.Add(tripleList[i + 2]);
                    }
                }

                //重新排序对子库
                OrderCardsList(pairList);

                for (int i = pairList.Count - 1; i >= 0 ; i--)
                {
                    if (tripleList.Contains(pairList[i]) == true)
                    {
                        tripleList.Remove(pairList[i]);
                    }
                }
            }

            for (int i = 0; i < 4; i++)
            {
                temp_AI_First_Hand.Add(pairList[i]);
            }

            for (int i = temp_AI_First_Hand.Count - 1; i >= 0 ; i--)
            {
                if (pairList.Contains(temp_AI_First_Hand[i]) == true)
                {
                    pairList.Remove(temp_AI_First_Hand[i]);
                }
            }
        }

        //7，判断---四带单
        private void Judge_Boom_Single()
        {
            ///同理

            if (boomList == null || boomList.Count < 4)
            {
                return;
            }

            if (singleList.Count + pairList.Count / 2 < 2)
            {
                return;
            }

            //添加炸弹
            for (int i = 0; i < 4; i++)
            {
                temp_AI_First_Hand.Add(boomList[i]);
            }

            //移除已经添加的单牌
            for (int i = temp_AI_First_Hand.Count - 1; i >= 0; i--)
            {
                if (boomList.Contains(temp_AI_First_Hand[i]) == true)
                {
                    boomList.Remove(temp_AI_First_Hand[i]);
                }
            }

            if (singleList.Count < 2)
            {
                for (int i = 0; i < pairList.Count - 1; i += 2)
                {
                    if (singleList.Contains(pairList[i]) == false)
                    {
                        singleList.Add(pairList[i]);
                    }
                }

                OrderCardsList(singleList);

                for (int i = singleList.Count - 1; i >= 0; i--)
                {
                    if (pairList.Contains(singleList[i]) == true)
                    {
                        pairList.Remove(singleList[i]);
                    }
                }
            }

            temp_AI_First_Hand.Add(singleList[0]);
            temp_AI_First_Hand.Add(singleList[1]);

            singleList.Remove(singleList[1]);
            singleList.Remove(singleList[0]);
        }

        //8，判断---炸弹
        private void Judge_Boom() 
        {
            if (boomList == null || boomList.Count < 4)
            {
                return;
            }

            for (int i = 0; i < 4; i++)
            {
                temp_AI_First_Hand.Add(boomList[i]);
            }

            for (int i = temp_AI_First_Hand.Count - 1; i >= 0 ; i--)
            {
                if (boomList.Contains(temp_AI_First_Hand[i]) == true)
                {
                    boomList.Remove(temp_AI_First_Hand[i]);
                }
            }
        }

        //9，判断---三带对
        private void Judge_Triple_Pair()
        {
            if (tripleList == null)
            {
                return;
            }

            if (tripleList.Count < 6)
            {
                return;
            }

            //这里要先添加三连，因为这有可能是唯一一个三连，所以要先添加
            //否则后面对子库数量不够时，拆三连库也不够，然后就会出错
            for (int i = 0; i < 3; i++)
            {
                temp_AI_First_Hand.Add(tripleList[tripleList.Count - (i + 1)]);
            }

            //如果添加三连之后，出现对子库牌数量加上三连库牌数量，小于2的情况，则不执行
            if (pairList.Count + tripleList.Count / 3 * 2 < 2)
            {
                ReClassify();
                return;
            }

            //若满足，则将添加了的三连移除
            for (int i = temp_AI_First_Hand.Count - 1; i >= 0 ; i--)
            {
                if (tripleList.Contains(temp_AI_First_Hand[i]) == true)
                {
                    tripleList.Remove(temp_AI_First_Hand[i]);
                }
            }

            //以下同理
            if (pairList.Count <= 2)
            {
                for (int i = 0; i < tripleList.Count - 2; i+=3)
                {
                    if (pairList.Contains(tripleList[i]) == false)
                    {
                        pairList.Add(tripleList[i]);
                        pairList.Add(tripleList[i + 1]);

                        //singleList.Add(tripleList[i + 2]);
                    }
                }

                //重新排序对子库
                OrderCardsList(pairList);

                for (int i = pairList.Count - 1; i >= 0 ; i--)
                {
                    if (tripleList.Contains(pairList[i]) == true)
                    {
                        tripleList.Remove(pairList[i]);
                    }
                }
            }

            //从最小的开始添加
            for (int i = 0; i < pairList.Count / 2; i += 2)
            {
                if (pairList[i].GetComponent<CardInformations>().CardValue == pairList[i + 1].GetComponent<CardInformations>().CardValue)
                {
                    temp_AI_First_Hand.Add(pairList[i]);
                    temp_AI_First_Hand.Add(pairList[i + 1]);

                    break;
                }
            }

            for (int i = temp_AI_First_Hand.Count - 1; i >= 0; i--)
            {
                if (pairList.Contains(temp_AI_First_Hand[i]) == true)
                {
                    pairList.Remove(temp_AI_First_Hand[i]);
                }
            }
        }

        //10，判断---三带单
        private void Judge_Triple_Single()
        {
            //与三带对同理

            if (tripleList == null)
            {
                return;
            }

            if (tripleList.Count < 3)
            {
                return;
            }

            for (int i = 0; i < 3; i++)
            {
                temp_AI_First_Hand.Add(tripleList[tripleList.Count - (i + 1)]);
            }

            if (singleList.Count + pairList.Count / 2 < 1)
            {
                ReClassify();
                return;
            }

            //若满足，则将添加了的三连移除
            for (int i = temp_AI_First_Hand.Count - 1; i >= 0; i--)
            {
                if (tripleList.Contains(temp_AI_First_Hand[i]) == true)
                {
                    tripleList.Remove(temp_AI_First_Hand[i]);
                }
            }

            if (singleList.Count < 1)
            {
                for (int i = 0; i < pairList.Count - 1; i += 2)
                {
                    if (singleList.Contains(pairList[i]) == false)
                    {
                        singleList.Add(pairList[i]);
                    }
                }

                OrderCardsList(singleList);

                for (int i = singleList.Count - 1; i >= 0; i--)
                {
                    if (pairList.Contains(singleList[i]) == true)
                    {
                        pairList.Remove(singleList[i]);
                    }
                }
            }

            temp_AI_First_Hand.Add(singleList[0]);

            singleList.Remove(singleList[0]);
        }

        //11，判断---三不带
        private void Judge_Triple_None() 
        {
            if (tripleList == null)
            {
                return;
            }

            if (tripleList.Count < 3)
            {
                return;
            }

            for (int i = 0; i < 3; i++)
            {
                temp_AI_First_Hand.Add(tripleList[tripleList.Count - (i + 1)]);
            }

            for (int i = temp_AI_First_Hand.Count - 1; i >= 0 ; i--)
            {
                if (tripleList.Contains(temp_AI_First_Hand[i]) == true)
                {
                    tripleList.Remove(temp_AI_First_Hand[i]);
                }
            }
        }

        //12，判断---王炸
        private void Judge_JokerBoom() 
        {
            if (jokerBoomList == null || jokerBoomList.Count < 2)
            {
                return;
            }

            for (int i = 0; i < jokerBoomList.Count; i++)
            {
                temp_AI_First_Hand.Add(jokerBoomList[i]);
            }

            for (int i = temp_AI_First_Hand.Count - 1; i >= 0 ; i--)
            {
                if (jokerBoomList.Contains(temp_AI_First_Hand[i]) == true)
                {
                    jokerBoomList.Remove(temp_AI_First_Hand[i]);
                }
            }
        }

        //13，判断---对子
        private void Judge_Pair() 
        {
            if (pairList.Count + tripleList.Count / 3 * 2 < 2)
            {
                return;
            }

            if (pairList.Count <= 2)
            {
                for (int i = 0; i < tripleList.Count - 2; i+=3)
                {
                    if (pairList.Contains(tripleList[i]) == false)
                    {
                        pairList.Add(tripleList[i]);
                        pairList.Add(tripleList[i + 1]);

                        //singleList.Add(tripleList[i + 2]);
                    }
                }

                //重新排序对子库
                OrderCardsList(pairList);

                for (int i = pairList.Count - 1; i >= 0 ; i--)
                {
                    if (tripleList.Contains(pairList[i]) == true)
                    {
                        tripleList.Remove(pairList[i]);
                    }
                }
            }

            for (int i = 0; i < pairList.Count/2; i+=2)
            {
                if (pairList[i].GetComponent<CardInformations>().CardValue == pairList[i+1].GetComponent<CardInformations>().CardValue)
                {
                    temp_AI_First_Hand.Add(pairList[i]);
                    temp_AI_First_Hand.Add(pairList[i + 1]);

                    break;
                }
            }

            for (int i = temp_AI_First_Hand.Count - 1; i >= 0; i--)
            {
                if (pairList.Contains(temp_AI_First_Hand[i]) == true)
                {
                    pairList.Remove(temp_AI_First_Hand[i]);
                }
            }
        }

        //14，判断---单牌
        private void Judge_Single() 
        {
            if (singleList.Count + pairList.Count / 2 < 1)
            {
                return;
            }

            if (singleList.Count < 1)
            {
                for (int i = 0; i < pairList.Count - 1; i += 2)
                {
                    if (singleList.Contains(pairList[i]) == false)
                    {
                        singleList.Add(pairList[i]);
                    }
                }

                OrderCardsList(singleList);

                for (int i = singleList.Count - 1; i >= 0; i--)
                {
                    if (pairList.Contains(singleList[i]) == true)
                    {
                        pairList.Remove(singleList[i]);
                    }
                }
            }

            temp_AI_First_Hand.Add(singleList[0]);

            singleList.Remove(singleList[0]);
        }

        //获取当前回合飞机不带个数
        private void GetCurrentPlaneCounts() 
        {
            for (int i = 0; i < tripleList.Count - 3; i+=3)
            {
                if (tripleList[i].GetComponent<CardInformations>().CardValue - tripleList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
                    tripleList[i + 1].GetComponent<CardInformations>().CardValue - tripleList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
                    tripleList[i + 2].GetComponent<CardInformations>().CardValue - tripleList[i + 5].GetComponent<CardInformations>().CardValue == -1)
                {
                    if (temp_plane_counts.Contains(tripleList[i]) == false)
                    {
                        temp_plane_counts.Add(tripleList[i]);
                    }

                    if (temp_plane_counts.Contains(tripleList[i + 1]) == false)
                    {
                        temp_plane_counts.Add(tripleList[i + 1]);
                    }

                    if (temp_plane_counts.Contains(tripleList[i + 2]) == false)
                    {
                        temp_plane_counts.Add(tripleList[i + 2]);
                    }

                    if (temp_plane_counts.Contains(tripleList[i + 3]) == false)
                    {
                        temp_plane_counts.Add(tripleList[i + 3]);
                    }

                    if (temp_plane_counts.Contains(tripleList[i + 4]) == false)
                    {
                        temp_plane_counts.Add(tripleList[i + 4]);
                    }

                    if (temp_plane_counts.Contains(tripleList[i + 5]) == false)
                    {
                        temp_plane_counts.Add(tripleList[i + 5]);
                    }
                }
            }

            for (int i = temp_plane_counts.Count - 1; i >= 0 ; i--)
            {
                if (tripleList.Contains(temp_plane_counts[i]) == true)
                {
                    tripleList.Remove(temp_plane_counts[i]);
                }
            }
        }

        //重新分类手牌
        public void ReClassify() 
        {
            //将所有牌库的牌重新添加到单牌库

            if (singleList.Count != 0 || singleList != null)
            {
                for (int i = 0; i < singleList.Count; i++)
                {
                    handCardGit.Add(singleList[i]);
                }
            }

            if (pairList.Count != 0 || pairList != null)
            {
                for (int i = 0; i < pairList.Count; i++)
                {
                    handCardGit.Add(pairList[i]);
                }
            }

            if (tripleList.Count != 0 || tripleList != null)
            {
                for (int i = 0; i < tripleList.Count; i++)
                {
                    handCardGit.Add(tripleList[i]);
                }
            }

            if (boomList.Count != 0 || boomList != null)
            {
                for (int i = 0; i < boomList.Count; i++)
                {
                    handCardGit.Add(boomList[i]);
                }
            }

            if (jokerBoomList.Count != 0 || jokerBoomList != null)
            {
                for (int i = 0; i < jokerBoomList.Count; i++)
                {
                    handCardGit.Add(jokerBoomList[i]);
                }
            }

            if (temp_plane_counts.Count != 0 || temp_plane_counts != null)
            {
                for (int i = 0; i < temp_plane_counts.Count; i++)
                {
                    handCardGit.Add(temp_plane_counts[i]);
                }
            }

            //将所有牌库清理
            RemoveCardList(singleList);
            RemoveCardList(pairList);
            RemoveCardList(tripleList);
            RemoveCardList(boomList);
            RemoveCardList(jokerBoomList);
            RemoveCardList(temp_plane_counts);

            //清理当前AI手牌
            RemoveCardList(temp_AI_First_Hand);
           
            //重新分类手牌
            ClassifyHandCards();
        }

        #endregion


        //移除目标牌库中的所有元素
        private void RemoveCardList(List<GameObject> _targetCardList) 
        {
            for (int i = _targetCardList.Count - 1; i >= 0; i--)
            {
                _targetCardList.Remove(_targetCardList[i]);
            }
        }


        /// <summary>
        /// 所有牌型的出牌算法判断
        /// </summary>
        #region AI后手出牌算法判断

        //1，若当前回合是---单牌
        public List<GameObject> CurrentTurn_Single(int _currentTurnCardPoint) 
        {
            List<GameObject> tempPlayCardList = new List<GameObject>();
            tempPlayCardList.Clear();

            if (singleList.Count + pairList.Count / 2 < 1)
            {
                return CurrentTurn_Boom();
            }

            if (singleList.Count < 1)
            {
                for (int i = 0; i < pairList.Count - 1; i+=2)
                {
                    if (singleList.Contains(pairList[i]) == false)
                    {
                        singleList.Add(pairList[i]);
                    }
                }

                OrderCardsList(singleList);

                for (int i = singleList.Count - 1; i >= 0 ; i--)
                {
                    if (pairList.Contains(singleList[i]) == true)
                    {
                        pairList.Remove(singleList[i]);
                    }
                }
            }

            //获取满足条件的单牌
            for (int i = singleList.Count - 1; i >= 0 ; i--)
            {
                if (singleList[i].GetComponent<CardInformations>().CardValue > _currentTurnCardPoint)
                {
                    tempPlayCardList.Add(singleList[i]);

                    singleList.Remove(singleList[i]);

                    break;
                }
            }

            //如果没有取得满足条件的牌组合，则判断炸弹库
            if (tempPlayCardList == null || tempPlayCardList.Count == 0)
            {
                return CurrentTurn_Boom();
            }

            OrderCardsList(tempPlayCardList);

            return tempPlayCardList;
        }

        //2，若当前回合是---对子
        public List<GameObject> CurrentTurn_Pair(int _currentTurnCardPoint) 
        {
            Debug.Log("AI --- " + currentAINumber);

            List<GameObject> tempPlayCardList = new List<GameObject>();
            tempPlayCardList.Clear();

            if (pairList.Count + tripleList.Count / 3 * 2 < 2)
            {
                return CurrentTurn_Boom();
            }

            if (pairList.Count < 2)
            {
                for (int i = 0; i < tripleList.Count - 2; i += 3)
                {
                    pairList.Add(tripleList[i]);
                    pairList.Add(tripleList[i + 1]);
                }

                //重新排序对子库
                OrderCardsList(pairList);

                for (int i = pairList.Count - 1; i >= 0 ; i--)
                {
                    if (tripleList.Contains(pairList[i]) == true)
                    {
                        tripleList.Remove(pairList[i]);
                    }
                }
            }


            if (pairList.Count > 2)
            {
                //获取满足条件的对子
                for (int i = 0; i < pairList.Count - 2; i += 2)
                {
                    if (pairList[pairList.Count - i - 1].GetComponent<CardInformations>().CardValue + pairList[pairList.Count - i - 2].GetComponent<CardInformations>().CardValue > _currentTurnCardPoint)
                    {
                        tempPlayCardList.Add(pairList[pairList.Count - i - 1]);
                        tempPlayCardList.Add(pairList[pairList.Count - i - 2]);

                        break;
                    }
                }
            }
            else
            {
                if (pairList[0].GetComponent<CardInformations>().CardValue + pairList[1].GetComponent<CardInformations>().CardValue > _currentTurnCardPoint)
                {
                    tempPlayCardList.Add(pairList[0]);
                    tempPlayCardList.Add(pairList[1]);
                }
            }


            for (int i = tempPlayCardList.Count - 1; i >= 0; i--)
            {
                if (pairList.Contains(tempPlayCardList[i]) == true)
                {
                    pairList.Remove(tempPlayCardList[i]);
                }
            }

            //如果没有取得满足条件的牌组合，则判断炸弹库
            if (tempPlayCardList == null || tempPlayCardList.Count == 0)
            {
                return CurrentTurn_Boom();
            }

            OrderCardsList(tempPlayCardList);

            return tempPlayCardList;
        }

        //3，若当前回合是---三连
        public List<GameObject> CurrentTurn_Triple_None(int _currentTurnCardPoint) 
        {
            List<GameObject> tempPlayCardList = new List<GameObject>();
            tempPlayCardList.Clear();

            if (tripleList.Count < 3)
            {
                return CurrentTurn_Boom();
            }

            for (int i = 0; i < Circulation_TripleTimes(tripleList.Count); i += 3)
            {
                if (tripleList[i].GetComponent<CardInformations>().CardValue +
                    tripleList[i + 1].GetComponent<CardInformations>().CardValue +
                    tripleList[i + 2].GetComponent<CardInformations>().CardValue > _currentTurnCardPoint)
                {
                    tempPlayCardList.Add(tripleList[i]);
                    tempPlayCardList.Add(tripleList[i + 1]);
                    tempPlayCardList.Add(tripleList[i + 2]);

                    tripleList.Remove(tripleList[i + 2]);
                    tripleList.Remove(tripleList[i + 1]);
                    tripleList.Remove(tripleList[i]);

                    break;
                }
            }

            //如果没有取得满足条件的牌组合，则判断炸弹库
            if (tempPlayCardList == null || tempPlayCardList.Count == 0)
            {
                return CurrentTurn_Boom();
            }

            OrderCardsList(tempPlayCardList);

            return tempPlayCardList;
        }

        //4，若当前回合是---三带一
        public List<GameObject> CurrentTurn_Triple_Single(List<GameObject> _currentTurnCardList) 
        {
            List<GameObject> tempPlayCardList = new List<GameObject>();
            tempPlayCardList.Clear();

            //如果三连库牌数小于3，则不执行
            if (tripleList.Count < 3)
            {
                return CurrentTurn_Boom();
            }

            //因为是三带一
            //所以，如果单牌库牌数量加上对子库单牌数量小于1，则不执行
            if (singleList.Count + pairList.Count / 2 < 1)
            {
                return CurrentTurn_Boom();
            }

            //用于存储当前回合的三带一的三连部分
            List<GameObject> currentTurnTriplePart = new List<GameObject>();
            currentTurnTriplePart.Clear();

            for (int i = 0; i < _currentTurnCardList.Count - 2; i++)
            {
                if (_currentTurnCardList[i].GetComponent<CardInformations>().CardValue == _currentTurnCardList[i+1].GetComponent<CardInformations>().CardValue &&
                    _currentTurnCardList[i].GetComponent<CardInformations>().CardValue == _currentTurnCardList[i+2].GetComponent<CardInformations>().CardValue)
                {
                    currentTurnTriplePart.Add(_currentTurnCardList[i]);
                    currentTurnTriplePart.Add(_currentTurnCardList[i + 1]);
                    currentTurnTriplePart.Add(_currentTurnCardList[i + 2]);
                }
            }

            //用于存储当前回合的三带一的三连部分的点数
            int currentTurnTriplePartPoints = 0;

            for (int i = 0; i < currentTurnTriplePart.Count; i++)
            {
                currentTurnTriplePartPoints += currentTurnTriplePart[i].GetComponent<CardInformations>().CardValue;
            }

            //获取满足条件的三连
            for (int i = 0; i < Circulation_TripleTimes(tripleList.Count); i += 3)
            {
                if (tripleList[i].GetComponent<CardInformations>().CardValue +
                    tripleList[i + 1].GetComponent<CardInformations>().CardValue +
                    tripleList[i + 2].GetComponent<CardInformations>().CardValue > currentTurnTriplePartPoints)
                {
                    tempPlayCardList.Add(tripleList[i]);
                    tempPlayCardList.Add(tripleList[i + 1]);
                    tempPlayCardList.Add(tripleList[i + 2]);

                    break;
                }
            }

            //如果这里没有满足要求的三连就退回
            if (tempPlayCardList.Count == 0 || tempPlayCardList == null)
            {
                ReClassify_Gote();
                return null;
            }

            for (int i = tempPlayCardList.Count - 1; i >= 0; i--)
            {
                if (tripleList.Contains(tempPlayCardList[i]) == true)
                {
                    tripleList.Remove(tempPlayCardList[i]);
                }
            }

            //如果单牌库没有牌，则拆对子库
            if (singleList.Count < 1)
            {
                for (int i = 0; i < pairList.Count - 1; i += 2)
                {
                    if (singleList.Contains(pairList[i]) == false)
                    {
                        singleList.Add(pairList[i]);
                    }
                }

                OrderCardsList(singleList);

                for (int i = singleList.Count - 1; i >= 0; i--)
                {
                    if (pairList.Contains(singleList[i]) == true)
                    {
                        pairList.Remove(singleList[i]);
                    }
                }
            }

            tempPlayCardList.Add(singleList[0]);

            singleList.Remove(singleList[0]);

            //如果没有取得满足条件的牌组合，则判断炸弹库
            if (tempPlayCardList == null || tempPlayCardList.Count == 0)
            {
                return CurrentTurn_Boom();
            }

            OrderCardsList(tempPlayCardList);

            return tempPlayCardList;
        }

        //5，若当前回合是---三带一对
        public List<GameObject> CurrentTurn_Triple_Pair(List<GameObject> _currentTurnCardList) 
        {
            //与三带一同理

            List<GameObject> tempPlayCardList = new List<GameObject>();
            tempPlayCardList.Clear();

            //如果三连库牌数小于3，则不执行
            if (tripleList.Count < 3)
            {
                return CurrentTurn_Boom();
            }

            //因为三带一对，之后判断对子会拆三连库，所以这里要将三连库牌数先减去三个，再进行计算
            if (pairList.Count + (tripleList.Count - 3) / 3 * 2 < 3)
            {
                return CurrentTurn_Boom();
            }

            List<GameObject> currentTurnTriplePart = new List<GameObject>();
            currentTurnTriplePart.Clear();

            for (int i = 0; i < _currentTurnCardList.Count - 2; i++)
            {
                if (_currentTurnCardList[i].GetComponent<CardInformations>().CardValue == _currentTurnCardList[i + 1].GetComponent<CardInformations>().CardValue &&
                    _currentTurnCardList[i].GetComponent<CardInformations>().CardValue == _currentTurnCardList[i + 2].GetComponent<CardInformations>().CardValue)
                {
                    currentTurnTriplePart.Add(_currentTurnCardList[i]);
                    currentTurnTriplePart.Add(_currentTurnCardList[i + 1]);
                    currentTurnTriplePart.Add(_currentTurnCardList[i + 2]);
                }
            }

            int currentTurnTriplePartPoints = 0;

            for (int i = 0; i < currentTurnTriplePart.Count; i++)
            {
                currentTurnTriplePartPoints += currentTurnTriplePart[i].GetComponent<CardInformations>().CardValue;
            }

            //获取满足条件的三连
            for (int i = 0; i < Circulation_TripleTimes(tripleList.Count); i += 3)
            {
                if (tripleList[i].GetComponent<CardInformations>().CardValue +
                    tripleList[i + 1].GetComponent<CardInformations>().CardValue +
                    tripleList[i + 2].GetComponent<CardInformations>().CardValue > currentTurnTriplePartPoints)
                {
                    tempPlayCardList.Add(tripleList[i]);
                    tempPlayCardList.Add(tripleList[i + 1]);
                    tempPlayCardList.Add(tripleList[i + 2]);

                    break;
                }
            }

            //如果这里没有满足要求的三连就退回
            if (tempPlayCardList.Count == 0 || tempPlayCardList == null)
            {
                ReClassify_Gote();
                return null;
            }

            for (int i = tempPlayCardList.Count - 1; i >= 0; i--)
            {
                if (tripleList.Contains(tempPlayCardList[i]) == true)
                {
                    tripleList.Remove(tempPlayCardList[i]);
                }
            }

            if (pairList.Count < 2)
            {
                for (int i = 0; i < tripleList.Count - 2; i += 3)
                {
                    if (pairList.Contains(tripleList[i]) == false)
                    {
                        pairList.Add(tripleList[i]);
                        pairList.Add(tripleList[i + 1]);
                    }
                }

                //重新排序对子库
                OrderCardsList(pairList);

                for (int i = pairList.Count - 1; i >= 0 ; i--)
                {
                    if (tripleList.Contains(pairList[i]) == true)
                    {
                        tripleList.Remove(pairList[i]);
                    }
                }
            }

            //同理从最小的开始添加
            tempPlayCardList.Add(pairList[0]);
            tempPlayCardList.Add(pairList[1]);

            pairList.Remove(pairList[1]);
            pairList.Remove(pairList[0]);

            //如果没有取得满足条件的牌组合，则判断炸弹库
            if (tempPlayCardList == null || tempPlayCardList.Count == 0)
            {
                return CurrentTurn_Boom();
            }

            OrderCardsList(tempPlayCardList);

            return tempPlayCardList;
        }

        //6，若当前回合是---炸弹 
        public List<GameObject> CurrentTurn_Boom(int _currentTurnCardPoint) 
        {
            List<GameObject> tempPlayCardList = new List<GameObject>();
            tempPlayCardList.Clear();

            if (boomList.Count < 4)
            {
                return CurrentTurn_JokerBoom();
            }

            //设定的炸弹的点数要，这是为了防止被一些特殊牌型影响
            for (int i = 0; i < Circulation_BoomTimes(boomList.Count); i += 4)
            {
                if ((boomList[i].GetComponent<CardInformations>().CardValue + boomList[i + 1].GetComponent<CardInformations>().CardValue +
                     boomList[i + 2].GetComponent<CardInformations>().CardValue + boomList[i + 3].GetComponent<CardInformations>().CardValue) > _currentTurnCardPoint)
                {
                    tempPlayCardList.Add(boomList[i]);
                    tempPlayCardList.Add(boomList[i + 1]);
                    tempPlayCardList.Add(boomList[i + 2]);
                    tempPlayCardList.Add(boomList[i + 3]);

                    boomList.Remove(boomList[i + 3]);
                    boomList.Remove(boomList[i + 2]);
                    boomList.Remove(boomList[i + 1]);
                    boomList.Remove(boomList[i]);

                    break;
                }
            }

            if (tempPlayCardList == null || tempPlayCardList.Count == 0)
            {
                return CurrentTurn_JokerBoom();
            }

            OrderCardsList(tempPlayCardList);

            return tempPlayCardList;
        }

        //7，若当前回合是---四带一
        public List<GameObject> CurrentTurn_Boom_Single(List<GameObject> _currentTurnCardList)
        {
            List<GameObject> tempPlayCardList = new List<GameObject>();
            tempPlayCardList.Clear();

            if (boomList.Count < 4)
            {
                return CurrentTurn_Boom();
            }

            if (singleList.Count + pairList.Count / 2 < 2)
            {
                return CurrentTurn_Boom();
            }

            //用于存储当前回合的四带一的炸弹部分
            List<GameObject> currentTurnBoomPart = new List<GameObject>();
            currentTurnBoomPart.Clear();

            for (int i = 0; i < _currentTurnCardList.Count - 2; i++)
            {
                if (_currentTurnCardList[i].GetComponent<CardInformations>().CardValue == _currentTurnCardList[i + 1].GetComponent<CardInformations>().CardValue &&
                    _currentTurnCardList[i].GetComponent<CardInformations>().CardValue == _currentTurnCardList[i + 2].GetComponent<CardInformations>().CardValue &&
                    _currentTurnCardList[i].GetComponent<CardInformations>().CardValue == _currentTurnCardList[i + 3].GetComponent<CardInformations>().CardValue)
                {
                    currentTurnBoomPart.Add(_currentTurnCardList[i]);
                    currentTurnBoomPart.Add(_currentTurnCardList[i + 1]);
                    currentTurnBoomPart.Add(_currentTurnCardList[i + 2]);
                    currentTurnBoomPart.Add(_currentTurnCardList[i + 3]);

                    break;
                }
            }

            int currentTurnBoomPartPoints = 0;

            for (int i = 0; i < currentTurnBoomPart.Count; i++)
            {
                currentTurnBoomPartPoints += currentTurnBoomPart[i].GetComponent<CardInformations>().CardValue;
            }

            //设定的四带一的点数要*100，这是为了防止被一些特殊牌型影响
            for (int i = 0; i < Circulation_BoomTimes(boomList.Count); i+=4)
            {
                if ((boomList[i].GetComponent<CardInformations>().CardValue + boomList[i + 1].GetComponent<CardInformations>().CardValue +
                     boomList[i + 2].GetComponent<CardInformations>().CardValue + boomList[i + 3].GetComponent<CardInformations>().CardValue) > currentTurnBoomPartPoints)
                {
                    tempPlayCardList.Add(boomList[i]);
                    tempPlayCardList.Add(boomList[i + 1]);
                    tempPlayCardList.Add(boomList[i + 2]);
                    tempPlayCardList.Add(boomList[i + 3]);

                    break;
                }
            }

            //如果没有找到满足条件的炸弹，则判断炸弹库
            if (tempPlayCardList.Count == 0 || tempPlayCardList == null)
            {
                return CurrentTurn_Boom();
            }

            for (int i = tempPlayCardList.Count - 1; i >= 0; i--)
            {
                if (boomList.Contains(tempPlayCardList[i]) == true)
                {
                    boomList.Remove(tempPlayCardList[i]);
                }
            }

            if (singleList.Count < 2)
            {
                for (int i = 0; i < pairList.Count - 1; i += 2)
                {
                    if (singleList.Contains(pairList[i]) == false)
                    {
                        singleList.Add(pairList[i]);
                    }
                }

                OrderCardsList(singleList);

                for (int i = singleList.Count - 1; i >= 0; i--)
                {
                    if (pairList.Contains(singleList[i]) == true)
                    {
                        pairList.Remove(singleList[i]);
                    }
                }
            }

            tempPlayCardList.Add(singleList[0]);
            tempPlayCardList.Add(singleList[1]);

            singleList.Remove(singleList[1]);
            singleList.Remove(singleList[0]);

            //如果没有取得满足条件的牌组合，则判断炸弹库
            if (tempPlayCardList == null || tempPlayCardList.Count == 0)
            {
                return CurrentTurn_Boom();
            }

            OrderCardsList(tempPlayCardList);

            return tempPlayCardList;
        }

        //8，若当前回合是---四带对
        public List<GameObject> CurrentTurn_Boom_Pair(List<GameObject> _currentTurnCardList) 
        {
            List<GameObject> tempPlayCardList = new List<GameObject>();
            tempPlayCardList.Clear();

            if (boomList.Count < 4)
            {
                return CurrentTurn_Boom();
            }

            if (pairList.Count + tripleList.Count / 3 * 2 < 4)
            {
                return CurrentTurn_Boom();
            }

            //用于存储当前回合的四带一的炸弹部分
            List<GameObject> currentTurnBoomPart = new List<GameObject>();
            currentTurnBoomPart.Clear();

            for (int i = 0; i < _currentTurnCardList.Count - 2; i++)
            {
                if (_currentTurnCardList[i].GetComponent<CardInformations>().CardValue == _currentTurnCardList[i + 1].GetComponent<CardInformations>().CardValue &&
                    _currentTurnCardList[i].GetComponent<CardInformations>().CardValue == _currentTurnCardList[i + 2].GetComponent<CardInformations>().CardValue &&
                    _currentTurnCardList[i].GetComponent<CardInformations>().CardValue == _currentTurnCardList[i + 3].GetComponent<CardInformations>().CardValue)
                {
                    currentTurnBoomPart.Add(_currentTurnCardList[i]);
                    currentTurnBoomPart.Add(_currentTurnCardList[i + 1]);
                    currentTurnBoomPart.Add(_currentTurnCardList[i + 2]);
                    currentTurnBoomPart.Add(_currentTurnCardList[i + 3]);

                    break;
                }
            }

            int currentTurnBoomPartPoints = 0;

            for (int i = 0; i < currentTurnBoomPart.Count; i++)
            {
                currentTurnBoomPartPoints += currentTurnBoomPart[i].GetComponent<CardInformations>().CardValue;
            }

            //设定的四带一对的点数要*100，这是为了防止被一些特殊牌型影响
            for (int i = 0; i < Circulation_BoomTimes(boomList.Count); i+=4)
            {
                if ((boomList[i].GetComponent<CardInformations>().CardValue + boomList[i + 1].GetComponent<CardInformations>().CardValue +
                     boomList[i + 2].GetComponent<CardInformations>().CardValue + boomList[i + 3].GetComponent<CardInformations>().CardValue) > currentTurnBoomPartPoints)
                {
                    tempPlayCardList.Add(boomList[i]);
                    tempPlayCardList.Add(boomList[i + 1]);
                    tempPlayCardList.Add(boomList[i + 2]);
                    tempPlayCardList.Add(boomList[i + 3]);

                    boomList.Remove(boomList[i + 3]);
                    boomList.Remove(boomList[i + 2]);
                    boomList.Remove(boomList[i + 1]);
                    boomList.Remove(boomList[i]);

                    break;
                }
            }

            //如果没有找到满足条件的炸弹，则判断炸弹库
            if (tempPlayCardList.Count == 0 || tempPlayCardList == null)
            {
                return CurrentTurn_Boom();
            }

            if (pairList.Count < 4)
            {
                for (int i = 0; i < tripleList.Count - 2; i+=3)
                {
                    if (pairList.Contains(tripleList[i]) == false)
                    {
                        pairList.Add(tripleList[i]);
                        pairList.Add(tripleList[i + 1]);
                    }
                }

                //重新排序对子库
                OrderCardsList(pairList);

                for (int i = pairList.Count - 1; i >= 0; i--)
                {
                    if (tripleList.Contains(pairList[i]) == true)
                    {
                        tripleList.Remove(pairList[i]);
                    }
                }
            }

            for (int i = 0; i < 4; i++)
            {
                tempPlayCardList.Add(pairList[i]);
            }

            for (int i = 0; i < tempPlayCardList.Count; i++)
            {
                if (pairList.Contains(tempPlayCardList[i]) == true)
                {
                    pairList.Remove(tempPlayCardList[i]);
                }
            }

            //如果没有取得满足条件的牌组合，则判断炸弹库
            if (tempPlayCardList == null || tempPlayCardList.Count == 0)
            {
                return CurrentTurn_Boom();
            }

            OrderCardsList(tempPlayCardList);

            return tempPlayCardList;
        }

        //9，若当前回合是---顺子
        public List<GameObject> CurrentTurn_Straight(int _currentTurnCardPoint,int _currentTurnCardCounts) 
        {
            List<GameObject> tempPlayCardList = new List<GameObject>();
            tempPlayCardList.Clear();

            if (singleList.Count + pairList.Count / 2 < _currentTurnCardCounts)
            {
                //return null;
                return CurrentTurn_Boom();
            }

            //拆对子库
            for (int i = 0; i < pairList.Count - 1; i += 2)
            {
                if (singleList.Contains(pairList[i]) == false)
                {
                    singleList.Add(pairList[i]);
                }
            }

            OrderCardsList(singleList);

            for (int i = singleList.Count - 1; i >= 0; i--)
            {
                if (pairList.Contains(singleList[i]) == true)
                {
                    pairList.Remove(singleList[i]);
                }
            }

            //还是得分类，因为我不知道他的顺子到底会有几张牌

            //判断连对的循环次数为：z = x - y + 1
            //--------------------------------------本来新写了一个方法的，但是写完发现，我忽略了ai后手出牌是需要计算点数的，所以还是用老方法

            //1，5张顺子
            if (_currentTurnCardCounts == 5)
            {
                for (int i = 0; i < (singleList.Count - _currentTurnCardCounts) + 1; i++)
                {
                    if (singleList[i].GetComponent<CardInformations>().CardValue - singleList[i + 1].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 1].GetComponent<CardInformations>().CardValue - singleList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 2].GetComponent<CardInformations>().CardValue - singleList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 3].GetComponent<CardInformations>().CardValue - singleList[i + 4].GetComponent<CardInformations>().CardValue == -1)
                    {
                        if (singleList[i].GetComponent<CardInformations>().CardValue + singleList[i + 1].GetComponent<CardInformations>().CardValue +
                            singleList[i + 2].GetComponent<CardInformations>().CardValue + singleList[i + 3].GetComponent<CardInformations>().CardValue +
                            singleList[i + 4].GetComponent<CardInformations>().CardValue > _currentTurnCardPoint)
                        {
                            tempPlayCardList.Add(singleList[i]);
                            tempPlayCardList.Add(singleList[i + 1]);
                            tempPlayCardList.Add(singleList[i + 2]);
                            tempPlayCardList.Add(singleList[i + 3]);
                            tempPlayCardList.Add(singleList[i + 4]);

                            //将已经添加的单牌移除
                            for (int j = tempPlayCardList.Count - 1; j >= 0; j--)
                            {
                                if (singleList.Contains(tempPlayCardList[j]) == true)
                                {
                                    singleList.Remove(tempPlayCardList[j]);
                                }
                            }

                            OrderCardsList(tempPlayCardList);

                            return tempPlayCardList;
                        }
                    }
                }
            }

            //2，6张顺子
            if (_currentTurnCardCounts == 6)
            {
                for (int i = 0; i < (singleList.Count - _currentTurnCardCounts) + 1; i++)
                {
                    if (singleList[i].GetComponent<CardInformations>().CardValue - singleList[i + 1].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 1].GetComponent<CardInformations>().CardValue - singleList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 2].GetComponent<CardInformations>().CardValue - singleList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 3].GetComponent<CardInformations>().CardValue - singleList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 4].GetComponent<CardInformations>().CardValue - singleList[i + 5].GetComponent<CardInformations>().CardValue == -1)
                    {
                        if (singleList[i].GetComponent<CardInformations>().CardValue + singleList[i + 1].GetComponent<CardInformations>().CardValue +
                            singleList[i + 2].GetComponent<CardInformations>().CardValue + singleList[i + 3].GetComponent<CardInformations>().CardValue +
                            singleList[i + 4].GetComponent<CardInformations>().CardValue + singleList[i + 5].GetComponent<CardInformations>().CardValue > _currentTurnCardPoint)
                        {
                            tempPlayCardList.Add(singleList[i]);
                            tempPlayCardList.Add(singleList[i + 1]);
                            tempPlayCardList.Add(singleList[i + 2]);
                            tempPlayCardList.Add(singleList[i + 3]);
                            tempPlayCardList.Add(singleList[i + 4]);
                            tempPlayCardList.Add(singleList[i + 5]);

                            //将已经添加的单牌移除
                            for (int j = tempPlayCardList.Count - 1; j >= 0; j--)
                            {
                                if (singleList.Contains(tempPlayCardList[j]) == true)
                                {
                                    singleList.Remove(tempPlayCardList[j]);
                                }
                            }

                            OrderCardsList(tempPlayCardList);

                            return tempPlayCardList;
                        }
                    }
                }
            }

            //3，7张顺子
            if (_currentTurnCardCounts == 7)
            {
                for (int i = 0; i < (singleList.Count - _currentTurnCardCounts) + 1; i++)
                {
                    if (singleList[i].GetComponent<CardInformations>().CardValue - singleList[i + 1].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 1].GetComponent<CardInformations>().CardValue - singleList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 2].GetComponent<CardInformations>().CardValue - singleList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 3].GetComponent<CardInformations>().CardValue - singleList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 4].GetComponent<CardInformations>().CardValue - singleList[i + 5].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 5].GetComponent<CardInformations>().CardValue - singleList[i + 6].GetComponent<CardInformations>().CardValue == -1)
                    {
                        if (singleList[i].GetComponent<CardInformations>().CardValue + singleList[i + 1].GetComponent<CardInformations>().CardValue +
                            singleList[i + 2].GetComponent<CardInformations>().CardValue + singleList[i + 3].GetComponent<CardInformations>().CardValue +
                            singleList[i + 4].GetComponent<CardInformations>().CardValue + singleList[i + 5].GetComponent<CardInformations>().CardValue +
                            singleList[i + 6].GetComponent<CardInformations>().CardValue > _currentTurnCardPoint)
                        {
                            tempPlayCardList.Add(singleList[i]);
                            tempPlayCardList.Add(singleList[i + 1]);
                            tempPlayCardList.Add(singleList[i + 2]);
                            tempPlayCardList.Add(singleList[i + 3]);
                            tempPlayCardList.Add(singleList[i + 4]);
                            tempPlayCardList.Add(singleList[i + 5]);
                            tempPlayCardList.Add(singleList[i + 6]);

                            //将已经添加的单牌移除
                            for (int j = tempPlayCardList.Count - 1; j >= 0; j--)
                            {
                                if (singleList.Contains(tempPlayCardList[j]) == true)
                                {
                                    singleList.Remove(tempPlayCardList[j]);
                                }
                            }

                            OrderCardsList(tempPlayCardList);

                            return tempPlayCardList;
                        }
                    }
                }
            }

            //4，8张顺子
            if (_currentTurnCardCounts == 8)
            {
                for (int i = 0; i < (singleList.Count - _currentTurnCardCounts) + 1; i++)
                {
                    if (singleList[i].GetComponent<CardInformations>().CardValue - singleList[i + 1].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 1].GetComponent<CardInformations>().CardValue - singleList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 2].GetComponent<CardInformations>().CardValue - singleList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 3].GetComponent<CardInformations>().CardValue - singleList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 4].GetComponent<CardInformations>().CardValue - singleList[i + 5].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 5].GetComponent<CardInformations>().CardValue - singleList[i + 6].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 6].GetComponent<CardInformations>().CardValue - singleList[i + 7].GetComponent<CardInformations>().CardValue == -1)
                    {
                        if (singleList[i].GetComponent<CardInformations>().CardValue + singleList[i + 1].GetComponent<CardInformations>().CardValue +
                            singleList[i + 2].GetComponent<CardInformations>().CardValue + singleList[i + 3].GetComponent<CardInformations>().CardValue +
                            singleList[i + 4].GetComponent<CardInformations>().CardValue + singleList[i + 5].GetComponent<CardInformations>().CardValue +
                            singleList[i + 6].GetComponent<CardInformations>().CardValue + singleList[i + 7].GetComponent<CardInformations>().CardValue > _currentTurnCardPoint)
                        {
                            tempPlayCardList.Add(singleList[i]);
                            tempPlayCardList.Add(singleList[i + 1]);
                            tempPlayCardList.Add(singleList[i + 2]);
                            tempPlayCardList.Add(singleList[i + 3]);
                            tempPlayCardList.Add(singleList[i + 4]);
                            tempPlayCardList.Add(singleList[i + 5]);
                            tempPlayCardList.Add(singleList[i + 6]);
                            tempPlayCardList.Add(singleList[i + 7]);

                            //将已经添加的单牌移除
                            for (int j = tempPlayCardList.Count - 1; j >= 0; j--)
                            {
                                if (singleList.Contains(tempPlayCardList[j]) == true)
                                {
                                    singleList.Remove(tempPlayCardList[j]);
                                }
                            }

                            OrderCardsList(tempPlayCardList);

                            return tempPlayCardList;
                        }
                    }
                }
            }

            //5，9张顺子
            if (_currentTurnCardCounts == 9)
            {
                for (int i = 0; i < (singleList.Count - _currentTurnCardCounts) + 1; i++)
                {
                    if (singleList[i].GetComponent<CardInformations>().CardValue - singleList[i + 1].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 1].GetComponent<CardInformations>().CardValue - singleList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 2].GetComponent<CardInformations>().CardValue - singleList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 3].GetComponent<CardInformations>().CardValue - singleList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 4].GetComponent<CardInformations>().CardValue - singleList[i + 5].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 5].GetComponent<CardInformations>().CardValue - singleList[i + 6].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 6].GetComponent<CardInformations>().CardValue - singleList[i + 7].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 7].GetComponent<CardInformations>().CardValue - singleList[i + 8].GetComponent<CardInformations>().CardValue == -1)
                    {
                        if (singleList[i].GetComponent<CardInformations>().CardValue + singleList[i + 1].GetComponent<CardInformations>().CardValue +
                            singleList[i + 2].GetComponent<CardInformations>().CardValue + singleList[i + 3].GetComponent<CardInformations>().CardValue +
                            singleList[i + 4].GetComponent<CardInformations>().CardValue + singleList[i + 5].GetComponent<CardInformations>().CardValue +
                            singleList[i + 6].GetComponent<CardInformations>().CardValue + singleList[i + 7].GetComponent<CardInformations>().CardValue +
                            singleList[i + 8].GetComponent<CardInformations>().CardValue > _currentTurnCardPoint)
                        {
                            tempPlayCardList.Add(singleList[i]);
                            tempPlayCardList.Add(singleList[i + 1]);
                            tempPlayCardList.Add(singleList[i + 2]);
                            tempPlayCardList.Add(singleList[i + 3]);
                            tempPlayCardList.Add(singleList[i + 4]);
                            tempPlayCardList.Add(singleList[i + 5]);
                            tempPlayCardList.Add(singleList[i + 6]);
                            tempPlayCardList.Add(singleList[i + 7]);
                            tempPlayCardList.Add(singleList[i + 8]);

                            //将已经添加的单牌移除
                            for (int j = tempPlayCardList.Count - 1; j >= 0; j--)
                            {
                                if (singleList.Contains(tempPlayCardList[j]) == true)
                                {
                                    singleList.Remove(tempPlayCardList[j]);
                                }
                            }

                            OrderCardsList(tempPlayCardList);

                            return tempPlayCardList;
                        }
                    }
                }
            }

            //6，10张顺子
            if (_currentTurnCardCounts == 10)
            {
                for (int i = 0; i < (singleList.Count - _currentTurnCardCounts) + 1; i++)
                {
                    if (singleList[i].GetComponent<CardInformations>().CardValue - singleList[i + 1].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 1].GetComponent<CardInformations>().CardValue - singleList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 2].GetComponent<CardInformations>().CardValue - singleList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 3].GetComponent<CardInformations>().CardValue - singleList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 4].GetComponent<CardInformations>().CardValue - singleList[i + 5].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 5].GetComponent<CardInformations>().CardValue - singleList[i + 6].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 6].GetComponent<CardInformations>().CardValue - singleList[i + 7].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 7].GetComponent<CardInformations>().CardValue - singleList[i + 8].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 8].GetComponent<CardInformations>().CardValue - singleList[i + 9].GetComponent<CardInformations>().CardValue == -1)
                    {
                        if (singleList[i].GetComponent<CardInformations>().CardValue + singleList[i + 1].GetComponent<CardInformations>().CardValue +
                            singleList[i + 2].GetComponent<CardInformations>().CardValue + singleList[i + 3].GetComponent<CardInformations>().CardValue +
                            singleList[i + 4].GetComponent<CardInformations>().CardValue + singleList[i + 5].GetComponent<CardInformations>().CardValue +
                            singleList[i + 6].GetComponent<CardInformations>().CardValue + singleList[i + 7].GetComponent<CardInformations>().CardValue +
                            singleList[i + 8].GetComponent<CardInformations>().CardValue + singleList[i + 9].GetComponent<CardInformations>().CardValue > _currentTurnCardPoint)
                        {
                            tempPlayCardList.Add(singleList[i]);
                            tempPlayCardList.Add(singleList[i + 1]);
                            tempPlayCardList.Add(singleList[i + 2]);
                            tempPlayCardList.Add(singleList[i + 3]);
                            tempPlayCardList.Add(singleList[i + 4]);
                            tempPlayCardList.Add(singleList[i + 5]);
                            tempPlayCardList.Add(singleList[i + 6]);
                            tempPlayCardList.Add(singleList[i + 7]);
                            tempPlayCardList.Add(singleList[i + 8]);
                            tempPlayCardList.Add(singleList[i + 9]);

                            //将已经添加的单牌移除
                            for (int j = tempPlayCardList.Count - 1; j >= 0; j--)
                            {
                                if (singleList.Contains(tempPlayCardList[j]) == true)
                                {
                                    singleList.Remove(tempPlayCardList[j]);
                                }
                            }

                            OrderCardsList(tempPlayCardList);

                            return tempPlayCardList;
                        }
                    }
                }
            }

            //7，11张顺子
            if (_currentTurnCardCounts == 11)
            {
                for (int i = 0; i < (singleList.Count - _currentTurnCardCounts) + 1; i++)
                {
                    if (singleList[i].GetComponent<CardInformations>().CardValue - singleList[i + 1].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 1].GetComponent<CardInformations>().CardValue - singleList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 2].GetComponent<CardInformations>().CardValue - singleList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 3].GetComponent<CardInformations>().CardValue - singleList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 4].GetComponent<CardInformations>().CardValue - singleList[i + 5].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 5].GetComponent<CardInformations>().CardValue - singleList[i + 6].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 6].GetComponent<CardInformations>().CardValue - singleList[i + 7].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 7].GetComponent<CardInformations>().CardValue - singleList[i + 8].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 8].GetComponent<CardInformations>().CardValue - singleList[i + 9].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 9].GetComponent<CardInformations>().CardValue - singleList[i + 10].GetComponent<CardInformations>().CardValue == -1)
                    {
                        if (singleList[i].GetComponent<CardInformations>().CardValue + singleList[i + 1].GetComponent<CardInformations>().CardValue +
                            singleList[i + 2].GetComponent<CardInformations>().CardValue + singleList[i + 3].GetComponent<CardInformations>().CardValue +
                            singleList[i + 4].GetComponent<CardInformations>().CardValue + singleList[i + 5].GetComponent<CardInformations>().CardValue +
                            singleList[i + 6].GetComponent<CardInformations>().CardValue + singleList[i + 7].GetComponent<CardInformations>().CardValue +
                            singleList[i + 8].GetComponent<CardInformations>().CardValue + singleList[i + 9].GetComponent<CardInformations>().CardValue +
                            singleList[i + 10].GetComponent<CardInformations>().CardValue > _currentTurnCardPoint)
                        {
                            tempPlayCardList.Add(singleList[i]);
                            tempPlayCardList.Add(singleList[i + 1]);
                            tempPlayCardList.Add(singleList[i + 2]);
                            tempPlayCardList.Add(singleList[i + 3]);
                            tempPlayCardList.Add(singleList[i + 4]);
                            tempPlayCardList.Add(singleList[i + 5]);
                            tempPlayCardList.Add(singleList[i + 6]);
                            tempPlayCardList.Add(singleList[i + 7]);
                            tempPlayCardList.Add(singleList[i + 8]);
                            tempPlayCardList.Add(singleList[i + 9]);
                            tempPlayCardList.Add(singleList[i + 10]);

                            //将已经添加的单牌移除
                            for (int j = tempPlayCardList.Count - 1; j >= 0; j--)
                            {
                                if (singleList.Contains(tempPlayCardList[j]) == true)
                                {
                                    singleList.Remove(tempPlayCardList[j]);
                                }
                            }

                            OrderCardsList(tempPlayCardList);

                            return tempPlayCardList;
                        }
                    }
                }
            }

            //8，12张顺子
            if (_currentTurnCardCounts == 12)
            {
                for (int i = 0; i < (singleList.Count - _currentTurnCardCounts) + 1; i++)
                {
                    if (singleList[i].GetComponent<CardInformations>().CardValue - singleList[i + 1].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 1].GetComponent<CardInformations>().CardValue - singleList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 2].GetComponent<CardInformations>().CardValue - singleList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 3].GetComponent<CardInformations>().CardValue - singleList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 4].GetComponent<CardInformations>().CardValue - singleList[i + 5].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 5].GetComponent<CardInformations>().CardValue - singleList[i + 6].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 6].GetComponent<CardInformations>().CardValue - singleList[i + 7].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 7].GetComponent<CardInformations>().CardValue - singleList[i + 8].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 8].GetComponent<CardInformations>().CardValue - singleList[i + 9].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 9].GetComponent<CardInformations>().CardValue - singleList[i + 10].GetComponent<CardInformations>().CardValue == -1 &&
                        singleList[i + 10].GetComponent<CardInformations>().CardValue - singleList[i + 11].GetComponent<CardInformations>().CardValue == -1)
                    {
                        if (singleList[i].GetComponent<CardInformations>().CardValue + singleList[i + 1].GetComponent<CardInformations>().CardValue +
                            singleList[i + 2].GetComponent<CardInformations>().CardValue + singleList[i + 3].GetComponent<CardInformations>().CardValue +
                            singleList[i + 4].GetComponent<CardInformations>().CardValue + singleList[i + 5].GetComponent<CardInformations>().CardValue +
                            singleList[i + 6].GetComponent<CardInformations>().CardValue + singleList[i + 7].GetComponent<CardInformations>().CardValue +
                            singleList[i + 8].GetComponent<CardInformations>().CardValue + singleList[i + 9].GetComponent<CardInformations>().CardValue +
                            singleList[i + 10].GetComponent<CardInformations>().CardValue + singleList[i + 11].GetComponent<CardInformations>().CardValue > _currentTurnCardPoint)
                        {
                            tempPlayCardList.Add(singleList[i]);
                            tempPlayCardList.Add(singleList[i + 1]);
                            tempPlayCardList.Add(singleList[i + 2]);
                            tempPlayCardList.Add(singleList[i + 3]);
                            tempPlayCardList.Add(singleList[i + 4]);
                            tempPlayCardList.Add(singleList[i + 5]);
                            tempPlayCardList.Add(singleList[i + 6]);
                            tempPlayCardList.Add(singleList[i + 7]);
                            tempPlayCardList.Add(singleList[i + 8]);
                            tempPlayCardList.Add(singleList[i + 9]);
                            tempPlayCardList.Add(singleList[i + 10]);
                            tempPlayCardList.Add(singleList[i + 11]);

                            //将已经添加的单牌移除
                            for (int j = tempPlayCardList.Count - 1; j >= 0; j--)
                            {
                                if (singleList.Contains(tempPlayCardList[j]) == true)
                                {
                                    singleList.Remove(tempPlayCardList[j]);
                                }
                            }

                            OrderCardsList(tempPlayCardList);

                            return tempPlayCardList;
                        }
                    }
                }
            }

            //如果没有取得满足条件的牌组合，则判断炸弹库
            if (tempPlayCardList == null || tempPlayCardList.Count == 0)
            {
                return CurrentTurn_Boom();
            }

            OrderCardsList(tempPlayCardList);

            return tempPlayCardList;
        }

        //9，若当前回合是---连对
        public List<GameObject> CurrentTurn_Straight_Pair(int _currentTurnCardPoint, int _currentTurnCardCounts)
        {
            List<GameObject> tempPlayCardList = new List<GameObject>();
            tempPlayCardList.Clear();

            if (pairList.Count + tripleList.Count / 3 * 2 < _currentTurnCardCounts)
            {
                return CurrentTurn_Boom();
            }

            //直接拆三连库
            for (int i = 0; i < tripleList.Count - 2; i += 3)
            {
                if (pairList.Contains(tripleList[i]) == false)
                {
                    pairList.Add(tripleList[i]);
                    pairList.Add(tripleList[i + 1]);
                }
            }

            //重新排序对子库
            OrderCardsList(pairList);

            for (int i = pairList.Count - 1; i >= 0; i--)
            {
                if (tripleList.Contains(pairList[i]) == true)
                {
                    tripleList.Remove(pairList[i]);
                }
            }

            //跟顺子同理，也要分情况

            //判断连对的循环次数为：z = x - y + 1

            //1，3个连对，6张牌
            if (_currentTurnCardCounts == 6)
            {
                for (int i = 0; i < (pairList.Count - _currentTurnCardCounts) + 1; i++)
                {
                    if (pairList[i].GetComponent<CardInformations>().CardValue - pairList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 1].GetComponent<CardInformations>().CardValue - pairList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 2].GetComponent<CardInformations>().CardValue - pairList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 3].GetComponent<CardInformations>().CardValue - pairList[i + 5].GetComponent<CardInformations>().CardValue == -1)
                    {
                        if (pairList[i].GetComponent<CardInformations>().CardValue + pairList[i + 1].GetComponent<CardInformations>().CardValue +
                            pairList[i + 2].GetComponent<CardInformations>().CardValue + pairList[i + 3].GetComponent<CardInformations>().CardValue +
                            pairList[i + 4].GetComponent<CardInformations>().CardValue + pairList[i + 5].GetComponent<CardInformations>().CardValue > _currentTurnCardPoint)
                        {
                            tempPlayCardList.Add(pairList[i]);
                            tempPlayCardList.Add(pairList[i + 1]);
                            tempPlayCardList.Add(pairList[i + 2]);
                            tempPlayCardList.Add(pairList[i + 3]);
                            tempPlayCardList.Add(pairList[i + 4]);
                            tempPlayCardList.Add(pairList[i + 5]);

                            break;
                        }
                    }
                }
            }

            //2，4个连对，8张牌
            if (_currentTurnCardCounts == 8)
            {
                for (int i = 0; i < (pairList.Count - _currentTurnCardCounts) + 1; i++)
                {
                    if (pairList[i].GetComponent<CardInformations>().CardValue - pairList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 1].GetComponent<CardInformations>().CardValue - pairList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 2].GetComponent<CardInformations>().CardValue - pairList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 3].GetComponent<CardInformations>().CardValue - pairList[i + 5].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 4].GetComponent<CardInformations>().CardValue - pairList[i + 6].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 5].GetComponent<CardInformations>().CardValue - pairList[i + 7].GetComponent<CardInformations>().CardValue == -1)
                    {
                        if (pairList[i].GetComponent<CardInformations>().CardValue + pairList[i + 1].GetComponent<CardInformations>().CardValue +
                            pairList[i + 2].GetComponent<CardInformations>().CardValue + pairList[i + 3].GetComponent<CardInformations>().CardValue +
                            pairList[i + 4].GetComponent<CardInformations>().CardValue + pairList[i + 5].GetComponent<CardInformations>().CardValue +
                            pairList[i + 6].GetComponent<CardInformations>().CardValue + pairList[i + 7].GetComponent<CardInformations>().CardValue > _currentTurnCardPoint)
                        {
                            tempPlayCardList.Add(pairList[i]);
                            tempPlayCardList.Add(pairList[i + 1]);
                            tempPlayCardList.Add(pairList[i + 2]);
                            tempPlayCardList.Add(pairList[i + 3]);
                            tempPlayCardList.Add(pairList[i + 4]);
                            tempPlayCardList.Add(pairList[i + 5]);
                            tempPlayCardList.Add(pairList[i + 6]);
                            tempPlayCardList.Add(pairList[i + 7]);

                            break;
                        }
                    }
                }
            }

            //3，5个连对，10张牌
            if (_currentTurnCardCounts == 10)
            {
                for (int i = 0; i < (pairList.Count - _currentTurnCardCounts) + 1; i++)
                {
                    if (pairList[i].GetComponent<CardInformations>().CardValue - pairList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 1].GetComponent<CardInformations>().CardValue - pairList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 2].GetComponent<CardInformations>().CardValue - pairList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 3].GetComponent<CardInformations>().CardValue - pairList[i + 5].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 4].GetComponent<CardInformations>().CardValue - pairList[i + 6].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 5].GetComponent<CardInformations>().CardValue - pairList[i + 7].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 6].GetComponent<CardInformations>().CardValue - pairList[i + 8].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 7].GetComponent<CardInformations>().CardValue - pairList[i + 9].GetComponent<CardInformations>().CardValue == -1)
                    {
                        if (pairList[i].GetComponent<CardInformations>().CardValue + pairList[i + 1].GetComponent<CardInformations>().CardValue +
                            pairList[i + 2].GetComponent<CardInformations>().CardValue + pairList[i + 3].GetComponent<CardInformations>().CardValue +
                            pairList[i + 4].GetComponent<CardInformations>().CardValue + pairList[i + 5].GetComponent<CardInformations>().CardValue +
                            pairList[i + 6].GetComponent<CardInformations>().CardValue + pairList[i + 7].GetComponent<CardInformations>().CardValue +
                            pairList[i + 8].GetComponent<CardInformations>().CardValue + pairList[i + 9].GetComponent<CardInformations>().CardValue > _currentTurnCardPoint)
                        {
                            tempPlayCardList.Add(pairList[i]);
                            tempPlayCardList.Add(pairList[i + 1]);
                            tempPlayCardList.Add(pairList[i + 2]);
                            tempPlayCardList.Add(pairList[i + 3]);
                            tempPlayCardList.Add(pairList[i + 4]);
                            tempPlayCardList.Add(pairList[i + 5]);
                            tempPlayCardList.Add(pairList[i + 6]);
                            tempPlayCardList.Add(pairList[i + 7]);
                            tempPlayCardList.Add(pairList[i + 8]);
                            tempPlayCardList.Add(pairList[i + 9]);

                            break;
                        }
                    }
                }
            }

            //4，6个连对，12张牌
            if (_currentTurnCardCounts == 12)
            {
                for (int i = 0; i < (pairList.Count - _currentTurnCardCounts) + 1; i++)
                {
                    if (pairList[i].GetComponent<CardInformations>().CardValue - pairList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 1].GetComponent<CardInformations>().CardValue - pairList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 2].GetComponent<CardInformations>().CardValue - pairList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 3].GetComponent<CardInformations>().CardValue - pairList[i + 5].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 4].GetComponent<CardInformations>().CardValue - pairList[i + 6].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 5].GetComponent<CardInformations>().CardValue - pairList[i + 7].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 6].GetComponent<CardInformations>().CardValue - pairList[i + 8].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 7].GetComponent<CardInformations>().CardValue - pairList[i + 9].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 8].GetComponent<CardInformations>().CardValue - pairList[i + 10].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 9].GetComponent<CardInformations>().CardValue - pairList[i + 11].GetComponent<CardInformations>().CardValue == -1)
                    {
                        if (pairList[i].GetComponent<CardInformations>().CardValue + pairList[i + 1].GetComponent<CardInformations>().CardValue +
                            pairList[i + 2].GetComponent<CardInformations>().CardValue + pairList[i + 3].GetComponent<CardInformations>().CardValue +
                            pairList[i + 4].GetComponent<CardInformations>().CardValue + pairList[i + 5].GetComponent<CardInformations>().CardValue +
                            pairList[i + 6].GetComponent<CardInformations>().CardValue + pairList[i + 7].GetComponent<CardInformations>().CardValue +
                            pairList[i + 8].GetComponent<CardInformations>().CardValue + pairList[i + 9].GetComponent<CardInformations>().CardValue +
                            pairList[i + 10].GetComponent<CardInformations>().CardValue + pairList[i + 11].GetComponent<CardInformations>().CardValue > _currentTurnCardPoint)
                        {
                            tempPlayCardList.Add(pairList[i]);
                            tempPlayCardList.Add(pairList[i + 1]);
                            tempPlayCardList.Add(pairList[i + 2]);
                            tempPlayCardList.Add(pairList[i + 3]);
                            tempPlayCardList.Add(pairList[i + 4]);
                            tempPlayCardList.Add(pairList[i + 5]);
                            tempPlayCardList.Add(pairList[i + 6]);
                            tempPlayCardList.Add(pairList[i + 7]);
                            tempPlayCardList.Add(pairList[i + 8]);
                            tempPlayCardList.Add(pairList[i + 9]);
                            tempPlayCardList.Add(pairList[i + 10]);
                            tempPlayCardList.Add(pairList[i + 11]);

                            break;
                        }
                    }
                }
            }

            //5，7个连对，14张牌
            if (_currentTurnCardCounts == 14)
            {
                for (int i = 0; i < (pairList.Count - _currentTurnCardCounts) + 1; i++)
                {
                    if (pairList[i].GetComponent<CardInformations>().CardValue - pairList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 1].GetComponent<CardInformations>().CardValue - pairList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 2].GetComponent<CardInformations>().CardValue - pairList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 3].GetComponent<CardInformations>().CardValue - pairList[i + 5].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 4].GetComponent<CardInformations>().CardValue - pairList[i + 6].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 5].GetComponent<CardInformations>().CardValue - pairList[i + 7].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 6].GetComponent<CardInformations>().CardValue - pairList[i + 8].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 7].GetComponent<CardInformations>().CardValue - pairList[i + 9].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 8].GetComponent<CardInformations>().CardValue - pairList[i + 10].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 9].GetComponent<CardInformations>().CardValue - pairList[i + 11].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 10].GetComponent<CardInformations>().CardValue - pairList[i + 12].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 11].GetComponent<CardInformations>().CardValue - pairList[i + 13].GetComponent<CardInformations>().CardValue == -1)
                    {
                        if (pairList[i].GetComponent<CardInformations>().CardValue + pairList[i + 1].GetComponent<CardInformations>().CardValue +
                            pairList[i + 2].GetComponent<CardInformations>().CardValue + pairList[i + 3].GetComponent<CardInformations>().CardValue +
                            pairList[i + 4].GetComponent<CardInformations>().CardValue + pairList[i + 5].GetComponent<CardInformations>().CardValue +
                            pairList[i + 6].GetComponent<CardInformations>().CardValue + pairList[i + 7].GetComponent<CardInformations>().CardValue +
                            pairList[i + 8].GetComponent<CardInformations>().CardValue + pairList[i + 9].GetComponent<CardInformations>().CardValue +
                            pairList[i + 10].GetComponent<CardInformations>().CardValue + pairList[i + 11].GetComponent<CardInformations>().CardValue +
                            pairList[i + 12].GetComponent<CardInformations>().CardValue + pairList[i + 13].GetComponent<CardInformations>().CardValue > _currentTurnCardPoint)
                        {
                            tempPlayCardList.Add(pairList[i]);
                            tempPlayCardList.Add(pairList[i + 1]);
                            tempPlayCardList.Add(pairList[i + 2]);
                            tempPlayCardList.Add(pairList[i + 3]);
                            tempPlayCardList.Add(pairList[i + 4]);
                            tempPlayCardList.Add(pairList[i + 5]);
                            tempPlayCardList.Add(pairList[i + 6]);
                            tempPlayCardList.Add(pairList[i + 7]);
                            tempPlayCardList.Add(pairList[i + 8]);
                            tempPlayCardList.Add(pairList[i + 9]);
                            tempPlayCardList.Add(pairList[i + 10]);
                            tempPlayCardList.Add(pairList[i + 11]);
                            tempPlayCardList.Add(pairList[i + 12]);
                            tempPlayCardList.Add(pairList[i + 13]);

                            break;
                        }
                    }
                }
            }

            //6，8个连对，16张牌
            if (_currentTurnCardCounts == 16)
            {
                for (int i = 0; i < (pairList.Count - _currentTurnCardCounts) + 1; i++)
                {
                    if (pairList[i].GetComponent<CardInformations>().CardValue - pairList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 1].GetComponent<CardInformations>().CardValue - pairList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 2].GetComponent<CardInformations>().CardValue - pairList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 3].GetComponent<CardInformations>().CardValue - pairList[i + 5].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 4].GetComponent<CardInformations>().CardValue - pairList[i + 6].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 5].GetComponent<CardInformations>().CardValue - pairList[i + 7].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 6].GetComponent<CardInformations>().CardValue - pairList[i + 8].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 7].GetComponent<CardInformations>().CardValue - pairList[i + 9].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 8].GetComponent<CardInformations>().CardValue - pairList[i + 10].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 9].GetComponent<CardInformations>().CardValue - pairList[i + 11].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 10].GetComponent<CardInformations>().CardValue - pairList[i + 12].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 11].GetComponent<CardInformations>().CardValue - pairList[i + 13].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 12].GetComponent<CardInformations>().CardValue - pairList[i + 14].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 13].GetComponent<CardInformations>().CardValue - pairList[i + 15].GetComponent<CardInformations>().CardValue == -1)
                    {
                        if (pairList[i].GetComponent<CardInformations>().CardValue + pairList[i + 1].GetComponent<CardInformations>().CardValue +
                            pairList[i + 2].GetComponent<CardInformations>().CardValue + pairList[i + 3].GetComponent<CardInformations>().CardValue +
                            pairList[i + 4].GetComponent<CardInformations>().CardValue + pairList[i + 5].GetComponent<CardInformations>().CardValue +
                            pairList[i + 6].GetComponent<CardInformations>().CardValue + pairList[i + 7].GetComponent<CardInformations>().CardValue +
                            pairList[i + 8].GetComponent<CardInformations>().CardValue + pairList[i + 9].GetComponent<CardInformations>().CardValue +
                            pairList[i + 10].GetComponent<CardInformations>().CardValue + pairList[i + 11].GetComponent<CardInformations>().CardValue +
                            pairList[i + 12].GetComponent<CardInformations>().CardValue + pairList[i + 13].GetComponent<CardInformations>().CardValue +
                            pairList[i + 14].GetComponent<CardInformations>().CardValue + pairList[i + 15].GetComponent<CardInformations>().CardValue > _currentTurnCardPoint)
                        {
                            tempPlayCardList.Add(pairList[i]);
                            tempPlayCardList.Add(pairList[i + 1]);
                            tempPlayCardList.Add(pairList[i + 2]);
                            tempPlayCardList.Add(pairList[i + 3]);
                            tempPlayCardList.Add(pairList[i + 4]);
                            tempPlayCardList.Add(pairList[i + 5]);
                            tempPlayCardList.Add(pairList[i + 6]);
                            tempPlayCardList.Add(pairList[i + 7]);
                            tempPlayCardList.Add(pairList[i + 8]);
                            tempPlayCardList.Add(pairList[i + 9]);
                            tempPlayCardList.Add(pairList[i + 10]);
                            tempPlayCardList.Add(pairList[i + 11]);
                            tempPlayCardList.Add(pairList[i + 12]);
                            tempPlayCardList.Add(pairList[i + 13]);
                            tempPlayCardList.Add(pairList[i + 14]);
                            tempPlayCardList.Add(pairList[i + 15]);

                            break;
                        }
                    }
                }
            }

            //7，9个连对，18张牌
            if (_currentTurnCardCounts == 18)
            {
                for (int i = 0; i < (pairList.Count - _currentTurnCardCounts) + 1; i++)
                {
                    if (pairList[i].GetComponent<CardInformations>().CardValue - pairList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 1].GetComponent<CardInformations>().CardValue - pairList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 2].GetComponent<CardInformations>().CardValue - pairList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 3].GetComponent<CardInformations>().CardValue - pairList[i + 5].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 4].GetComponent<CardInformations>().CardValue - pairList[i + 6].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 5].GetComponent<CardInformations>().CardValue - pairList[i + 7].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 6].GetComponent<CardInformations>().CardValue - pairList[i + 8].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 7].GetComponent<CardInformations>().CardValue - pairList[i + 9].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 8].GetComponent<CardInformations>().CardValue - pairList[i + 10].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 9].GetComponent<CardInformations>().CardValue - pairList[i + 11].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 10].GetComponent<CardInformations>().CardValue - pairList[i + 12].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 11].GetComponent<CardInformations>().CardValue - pairList[i + 13].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 12].GetComponent<CardInformations>().CardValue - pairList[i + 14].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 13].GetComponent<CardInformations>().CardValue - pairList[i + 15].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 14].GetComponent<CardInformations>().CardValue - pairList[i + 16].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 15].GetComponent<CardInformations>().CardValue - pairList[i + 17].GetComponent<CardInformations>().CardValue == -1)
                    {
                        if (pairList[i].GetComponent<CardInformations>().CardValue + pairList[i + 1].GetComponent<CardInformations>().CardValue +
                            pairList[i + 2].GetComponent<CardInformations>().CardValue + pairList[i + 3].GetComponent<CardInformations>().CardValue +
                            pairList[i + 4].GetComponent<CardInformations>().CardValue + pairList[i + 5].GetComponent<CardInformations>().CardValue +
                            pairList[i + 6].GetComponent<CardInformations>().CardValue + pairList[i + 7].GetComponent<CardInformations>().CardValue +
                            pairList[i + 8].GetComponent<CardInformations>().CardValue + pairList[i + 9].GetComponent<CardInformations>().CardValue +
                            pairList[i + 10].GetComponent<CardInformations>().CardValue + pairList[i + 11].GetComponent<CardInformations>().CardValue +
                            pairList[i + 12].GetComponent<CardInformations>().CardValue + pairList[i + 13].GetComponent<CardInformations>().CardValue +
                            pairList[i + 14].GetComponent<CardInformations>().CardValue + pairList[i + 15].GetComponent<CardInformations>().CardValue +
                            pairList[i + 16].GetComponent<CardInformations>().CardValue + pairList[i + 17].GetComponent<CardInformations>().CardValue > _currentTurnCardPoint)
                        {
                            tempPlayCardList.Add(pairList[i]);
                            tempPlayCardList.Add(pairList[i + 1]);
                            tempPlayCardList.Add(pairList[i + 2]);
                            tempPlayCardList.Add(pairList[i + 3]);
                            tempPlayCardList.Add(pairList[i + 4]);
                            tempPlayCardList.Add(pairList[i + 5]);
                            tempPlayCardList.Add(pairList[i + 6]);
                            tempPlayCardList.Add(pairList[i + 7]);
                            tempPlayCardList.Add(pairList[i + 8]);
                            tempPlayCardList.Add(pairList[i + 9]);
                            tempPlayCardList.Add(pairList[i + 10]);
                            tempPlayCardList.Add(pairList[i + 11]);
                            tempPlayCardList.Add(pairList[i + 12]);
                            tempPlayCardList.Add(pairList[i + 13]);
                            tempPlayCardList.Add(pairList[i + 14]);
                            tempPlayCardList.Add(pairList[i + 15]);
                            tempPlayCardList.Add(pairList[i + 16]);
                            tempPlayCardList.Add(pairList[i + 17]);

                            break;
                        }
                    }
                }
            }

            //8，10个连对，20张牌
            if (_currentTurnCardCounts == 20)
            {
                for (int i = 0; i < (pairList.Count - _currentTurnCardCounts) + 1; i++)
                {
                    if (pairList[i].GetComponent<CardInformations>().CardValue - pairList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 1].GetComponent<CardInformations>().CardValue - pairList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 2].GetComponent<CardInformations>().CardValue - pairList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 3].GetComponent<CardInformations>().CardValue - pairList[i + 5].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 4].GetComponent<CardInformations>().CardValue - pairList[i + 6].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 5].GetComponent<CardInformations>().CardValue - pairList[i + 7].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 6].GetComponent<CardInformations>().CardValue - pairList[i + 8].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 7].GetComponent<CardInformations>().CardValue - pairList[i + 9].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 8].GetComponent<CardInformations>().CardValue - pairList[i + 10].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 9].GetComponent<CardInformations>().CardValue - pairList[i + 11].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 10].GetComponent<CardInformations>().CardValue - pairList[i + 12].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 11].GetComponent<CardInformations>().CardValue - pairList[i + 13].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 12].GetComponent<CardInformations>().CardValue - pairList[i + 14].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 13].GetComponent<CardInformations>().CardValue - pairList[i + 15].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 14].GetComponent<CardInformations>().CardValue - pairList[i + 16].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 15].GetComponent<CardInformations>().CardValue - pairList[i + 17].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 16].GetComponent<CardInformations>().CardValue - pairList[i + 18].GetComponent<CardInformations>().CardValue == -1 &&
                        pairList[i + 17].GetComponent<CardInformations>().CardValue - pairList[i + 19].GetComponent<CardInformations>().CardValue == -1)
                    {
                        if (pairList[i].GetComponent<CardInformations>().CardValue + pairList[i + 1].GetComponent<CardInformations>().CardValue +
                            pairList[i + 2].GetComponent<CardInformations>().CardValue + pairList[i + 3].GetComponent<CardInformations>().CardValue +
                            pairList[i + 4].GetComponent<CardInformations>().CardValue + pairList[i + 5].GetComponent<CardInformations>().CardValue +
                            pairList[i + 6].GetComponent<CardInformations>().CardValue + pairList[i + 7].GetComponent<CardInformations>().CardValue +
                            pairList[i + 8].GetComponent<CardInformations>().CardValue + pairList[i + 9].GetComponent<CardInformations>().CardValue +
                            pairList[i + 10].GetComponent<CardInformations>().CardValue + pairList[i + 11].GetComponent<CardInformations>().CardValue +
                            pairList[i + 12].GetComponent<CardInformations>().CardValue + pairList[i + 13].GetComponent<CardInformations>().CardValue +
                            pairList[i + 14].GetComponent<CardInformations>().CardValue + pairList[i + 15].GetComponent<CardInformations>().CardValue +
                            pairList[i + 16].GetComponent<CardInformations>().CardValue + pairList[i + 17].GetComponent<CardInformations>().CardValue +
                            pairList[i + 18].GetComponent<CardInformations>().CardValue + pairList[i + 19].GetComponent<CardInformations>().CardValue > _currentTurnCardPoint)
                        {
                            tempPlayCardList.Add(pairList[i]);
                            tempPlayCardList.Add(pairList[i + 1]);
                            tempPlayCardList.Add(pairList[i + 2]);
                            tempPlayCardList.Add(pairList[i + 3]);
                            tempPlayCardList.Add(pairList[i + 4]);
                            tempPlayCardList.Add(pairList[i + 5]);
                            tempPlayCardList.Add(pairList[i + 6]);
                            tempPlayCardList.Add(pairList[i + 7]);
                            tempPlayCardList.Add(pairList[i + 8]);
                            tempPlayCardList.Add(pairList[i + 9]);
                            tempPlayCardList.Add(pairList[i + 10]);
                            tempPlayCardList.Add(pairList[i + 11]);
                            tempPlayCardList.Add(pairList[i + 12]);
                            tempPlayCardList.Add(pairList[i + 13]);
                            tempPlayCardList.Add(pairList[i + 14]);
                            tempPlayCardList.Add(pairList[i + 15]);
                            tempPlayCardList.Add(pairList[i + 16]);
                            tempPlayCardList.Add(pairList[i + 17]);
                            tempPlayCardList.Add(pairList[i + 18]);
                            tempPlayCardList.Add(pairList[i + 19]);

                            break;
                        }
                    }
                }
            }

            for (int i = tempPlayCardList.Count - 1; i >= 0 ; i--)
            {
                if (pairList.Contains(tempPlayCardList[i]) == true)
                {
                    pairList.Remove(tempPlayCardList[i]);
                }
            }

            //如果没有取得满足条件的牌组合，则判断炸弹库
            if (tempPlayCardList == null || tempPlayCardList.Count == 0)
            {
                return CurrentTurn_Boom();
            }

            OrderCardsList(tempPlayCardList);

            return tempPlayCardList;
        }

        //10，若当前回合是---飞机不带
        public List<GameObject> CurrentTurn_Plane_None(int _currentTurnCardPoint, int _currentTurnCardCounts) 
        {
            List<GameObject> tempPlayCardList = new List<GameObject>();
            tempPlayCardList.Clear();

            if (tripleList.Count < _currentTurnCardCounts)
            {
                return CurrentTurn_Boom();
            }

            //获取飞机不带的总个数
            GetCurrentTurnPlaneNone();

            //如果获取过后的temp_plane_none数量小于当前回合飞机不带数量，则重新分类，然后不执行
            if (temp_plane_none.Count < _currentTurnCardCounts)
            {
                ReClassify_Gote();
                return CurrentTurn_Boom();
            }

            //从飞机不带库中获取符合条件的飞机不带
            GetCurrentTurnConformPlaneNone(_currentTurnCardPoint, _currentTurnCardCounts);

            //将符合条件的飞机不带加入临时出牌列表
            for (int i = 0; i < tempConformPlaneNone.Count; i++)
            {
                tempPlayCardList.Add(tempConformPlaneNone[i]);
            }

            //移除已经添加了的飞机不带
            for (int i = tempConformPlaneNone.Count - 1; i >= 0; i--)
            {
                tempConformPlaneNone.Remove(tempConformPlaneNone[i]);
            }

            //如果没有取得满足条件的牌组合，则判断炸弹库
            if (tempPlayCardList == null || tempPlayCardList.Count == 0)
            {
                return CurrentTurn_Boom();
            }

            OrderCardsList(tempPlayCardList);

            return tempPlayCardList;
        }

        //11，若当前回合是---飞机带单
        public List<GameObject> CurrentTurn_Plane_Single(List<GameObject> _currentTurnCardList) 
        {
            List<GameObject> tempPlayCardList = new List<GameObject>();
            tempPlayCardList.Clear();

            //用来装当前回合的飞机不带部分
            List<GameObject> currentTurnPlaneNonePart = new List<GameObject>();
            currentTurnPlaneNonePart.Clear();

            //获取当前回合飞机不带部分
            for (int i = 0; i < _currentTurnCardList.Count - 2; i++)
            {
                if (_currentTurnCardList[i].GetComponent<CardInformations>().CardValue == _currentTurnCardList[i + 1].GetComponent<CardInformations>().CardValue &&
                    _currentTurnCardList[i].GetComponent<CardInformations>().CardValue == _currentTurnCardList[i + 2].GetComponent<CardInformations>().CardValue)
                {
                    if (currentTurnPlaneNonePart.Contains(_currentTurnCardList[i]) == false)
                    {
                        currentTurnPlaneNonePart.Add(_currentTurnCardList[i]);
                    }

                    if (currentTurnPlaneNonePart.Contains(_currentTurnCardList[i + 1]) == false)
                    {
                        currentTurnPlaneNonePart.Add(_currentTurnCardList[i + 1]);
                    }

                    if (currentTurnPlaneNonePart.Contains(_currentTurnCardList[i + 2]) == false)
                    {
                        currentTurnPlaneNonePart.Add(_currentTurnCardList[i + 2]);
                    }
                }
            }

            //如果AI手中的单牌不足够去组合飞机带单，则返回
            if (singleList.Count + pairList.Count / 2 < currentTurnPlaneNonePart.Count / 3)
            {
                return CurrentTurn_Boom();
            }

            //获取AI飞机不带
            GetCurrentTurnPlaneNone();

            //如果AI手中的飞机不带总个数小于当前回合的飞机不带个数，则重新分类AI手牌，然后不执行
            if (temp_plane_none.Count < currentTurnPlaneNonePart.Count)
            {
                ReClassify_Gote();
                return CurrentTurn_Boom();
            }

            //获取当前回合飞机不带部分的点数
            int currentTurnPlaneNonePartPoints = 0;

            for (int i = 0; i < currentTurnPlaneNonePart.Count; i++)
            {
                currentTurnPlaneNonePartPoints += currentTurnPlaneNonePart[i].GetComponent<CardInformations>().CardValue;
            }

            //获取当前回合符合条件的飞机不带
            GetCurrentTurnConformPlaneNone(currentTurnPlaneNonePartPoints, currentTurnPlaneNonePart.Count);

            //如果没有得到符合条件的飞机不带，则重新分类AI手牌，然后不执行
            if (tempConformPlaneNone.Count != currentTurnPlaneNonePart.Count || tempConformPlaneNone == null)
            {
                ReClassify_Gote();
                return CurrentTurn_Boom();
            }

            //将符合条件的飞机不带加入临时出牌列表
            for (int i = 0; i < tempConformPlaneNone.Count; i++)
            {
                tempPlayCardList.Add(tempConformPlaneNone[i]);
            }

            //移除已经添加了的飞机不带
            for (int i = tempConformPlaneNone.Count - 1; i >= 0; i--)
            {
                tempConformPlaneNone.Remove(tempConformPlaneNone[i]);
            }

            //如果单牌库的牌数量不满足条件，则拆对子库
            if (singleList.Count < currentTurnPlaneNonePart.Count / 3)
            {
                for (int i = 0; i < pairList.Count - 1; i += 2)
                {
                    if (singleList.Contains(pairList[i]) == false)
                    {
                        singleList.Add(pairList[i]);
                    }
                }

                OrderCardsList(singleList);

                for (int i = singleList.Count - 1; i >= 0; i--)
                {
                    if (pairList.Contains(singleList[i]) == true)
                    {
                        pairList.Remove(singleList[i]);
                    }
                }
            }

            //获取相应数量的单牌，同理，从小开始获取
            for (int i = 0; i < currentTurnPlaneNonePart.Count / 3; i++)
            {
                tempPlayCardList.Add(singleList[i]);
            }

            for (int i = tempPlayCardList.Count - 1; i >= 0 ; i--)
            {
                if (singleList.Contains(tempPlayCardList[i]) == true)
                {
                    singleList.Remove(tempPlayCardList[i]);
                }
            }

            //如果没有取得满足条件的牌组合，则判断炸弹库
            if (tempPlayCardList == null || tempPlayCardList.Count == 0)
            {
                return CurrentTurn_Boom();
            }

            OrderCardsList(tempPlayCardList);

            return tempPlayCardList;
        }

        //12，若当前回合是---飞机带对
        public List<GameObject> CurrentTurn_Plane_Pair(List<GameObject> _currentTurnCardList) 
        {
            List<GameObject> tempPlayCardList = new List<GameObject>();
            tempPlayCardList.Clear();

            //用来装当前回合的飞机不带部分
            List<GameObject> currentTurnPlaneNonePart = new List<GameObject>();
            currentTurnPlaneNonePart.Clear();

            //获取当前回合飞机不带部分
            for (int i = 0; i < _currentTurnCardList.Count - 2; i++)
            {
                if (_currentTurnCardList[i].GetComponent<CardInformations>().CardValue == _currentTurnCardList[i + 1].GetComponent<CardInformations>().CardValue &&
                    _currentTurnCardList[i].GetComponent<CardInformations>().CardValue == _currentTurnCardList[i + 2].GetComponent<CardInformations>().CardValue)
                {
                    if (currentTurnPlaneNonePart.Contains(_currentTurnCardList[i]) == false)
                    {
                        currentTurnPlaneNonePart.Add(_currentTurnCardList[i]);
                    }

                    if (currentTurnPlaneNonePart.Contains(_currentTurnCardList[i + 1]) == false)
                    {
                        currentTurnPlaneNonePart.Add(_currentTurnCardList[i + 1]);
                    }

                    if (currentTurnPlaneNonePart.Contains(_currentTurnCardList[i + 2]) == false)
                    {
                        currentTurnPlaneNonePart.Add(_currentTurnCardList[i + 2]);
                    }
                }
            }

            //如果AI手中的单牌不足够去组合飞机带单，则判断炸弹库
            if (singleList.Count + pairList.Count / 2 < currentTurnPlaneNonePart.Count / 3)
            {
                return CurrentTurn_Boom();
            }

            //获取AI飞机不带
            GetCurrentTurnPlaneNone();

            //如果AI手中的飞机不带总个数小于当前回合的飞机不带个数，则重新分类AI手牌，然后不执行
            if (temp_plane_none.Count < currentTurnPlaneNonePart.Count)
            {
                ReClassify_Gote();
                return CurrentTurn_Boom();
            }

            //获取当前回合飞机不带部分的点数
            int currentTurnPlaneNonePartPoints = 0;

            for (int i = 0; i < currentTurnPlaneNonePart.Count; i++)
            {
                currentTurnPlaneNonePartPoints += currentTurnPlaneNonePart[i].GetComponent<CardInformations>().CardValue;
            }

            //获取当前回合符合条件的飞机不带
            GetCurrentTurnConformPlaneNone(currentTurnPlaneNonePartPoints, currentTurnPlaneNonePart.Count);

            //如果没有得到符合条件的飞机不带，则重新分类AI手牌，然后不执行
            if (tempConformPlaneNone.Count != currentTurnPlaneNonePart.Count || tempConformPlaneNone == null)
            {
                ReClassify_Gote();
                return CurrentTurn_Boom();
            }

            //如果当前回合的对子库牌数量加三连库的对子牌数量不满足当前回合需要的对子牌数量，则重新分类，然后不执行
            if (pairList.Count + tripleList.Count / 3 * 2 < currentTurnPlaneNonePart.Count / 3 * 2)
            {
                ReClassify_Gote();
                return CurrentTurn_Boom();
            }

            //将符合条件的飞机不带加入临时出牌列表
            for (int i = 0; i < tempConformPlaneNone.Count; i++)
            {
                tempPlayCardList.Add(tempConformPlaneNone[i]);
            }

            //移除已经添加了的飞机不带
            for (int i = tempConformPlaneNone.Count - 1; i >= 0; i--)
            {
                tempConformPlaneNone.Remove(tempConformPlaneNone[i]);
            }

            //同理，拆三连库
            if (pairList.Count < currentTurnPlaneNonePart.Count / 3 * 2)
            {
                for (int i = 0; i < tripleList.Count - 2; i+=3)
                {
                    if (pairList.Contains(tripleList[i]) == false)
                    {
                        pairList.Add(tripleList[i]);
                        pairList.Add(tripleList[i + 1]);
                    }
                }

                //重新排序对子库
                OrderCardsList(pairList);

                for (int i = pairList.Count - 1; i >= 0 ; i--)
                {
                    if (tripleList.Contains(pairList[i]) == true)
                    {
                        tripleList.Remove(pairList[i]);
                    }
                }
            }

            //同理，从最小的开始获取
            for (int i = 0; i < currentTurnPlaneNonePart.Count / 3 * 2; i++)
            {
                tempPlayCardList.Add(pairList[i]);
            }

            for (int i = tempPlayCardList.Count - 1; i >= 0 ; i--)
            {
                if (pairList.Contains(tempPlayCardList[i]) == true)
                {
                    pairList.Remove(tempPlayCardList[i]);
                }
            }

            //如果没有取得满足条件的牌组合，则判断炸弹库
            if (tempPlayCardList == null || tempPlayCardList.Count == 0)
            {
                return CurrentTurn_Boom();
            }

            OrderCardsList(tempPlayCardList);

            return tempPlayCardList;
        }

        //当没有满足的卡牌组合的时候，判断炸弹库
        private List<GameObject> CurrentTurn_Boom() 
        {
            List<GameObject> tempPlayCardList = new List<GameObject>();
            tempPlayCardList.Clear();

            if (boomList.Count == 0 || boomList == null)
            {
                return CurrentTurn_JokerBoom();
            }

            //当炸弹库有牌时
            if (boomList.Count != 0 || boomList != null)
            {
                //添加炸弹
                for (int i = 0; i < 4; i++)
                {
                    tempPlayCardList.Add(boomList[i]);
                }

                //移除已经被添加的炸弹
                for (int i = tempPlayCardList.Count - 1; i >= 0; i--)
                {
                    if (boomList.Contains(tempPlayCardList[i]) == true)
                    {
                        boomList.Remove(tempPlayCardList[i]);
                    }
                }

                return tempPlayCardList;
            }

            if (tempPlayCardList.Count == 0||tempPlayCardList == null)
            {
                return CurrentTurn_JokerBoom();
            }

            return null;
        }

        private List<GameObject> CurrentTurn_JokerBoom() 
        {
            List<GameObject> tempPlayCardList = new List<GameObject>();
            tempPlayCardList.Clear();

            if (jokerBoomList.Count == 0 || jokerBoomList == null)
            {
                return null;
            }

            if (jokerBoomList.Count != 0 || jokerBoomList != null)
            {
                tempPlayCardList.Add(jokerBoomList[0]);
                tempPlayCardList.Add(jokerBoomList[1]);

                jokerBoomList.Remove(jokerBoomList[1]);
                jokerBoomList.Remove(jokerBoomList[0]);

                return tempPlayCardList;
            }

            return null;
        }

        //获取当前回合符合条件的飞机不带
        private void GetCurrentTurnConformPlaneNone(int _currentTurnCardPoint, int _currentTurnCardCounts) 
        {
            //同理，也要分类

            //1，2个飞机，6张牌
            if (_currentTurnCardCounts == 6)
            {
                for (int i = 0; i < Circulation_PlaneNoneTimes(_currentTurnCardCounts); i++)
                {
                    if (temp_plane_none[i].GetComponent<CardInformations>().CardValue + temp_plane_none[i + 1].GetComponent<CardInformations>().CardValue +
                        temp_plane_none[i + 2].GetComponent<CardInformations>().CardValue + temp_plane_none[i + 3].GetComponent<CardInformations>().CardValue +
                        temp_plane_none[i + 4].GetComponent<CardInformations>().CardValue + temp_plane_none[i + 5].GetComponent<CardInformations>().CardValue > _currentTurnCardPoint)
                    {
                        tempConformPlaneNone.Add(temp_plane_none[i]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 1]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 2]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 3]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 4]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 5]);

                        break;
                    }
                }
            }

            //2，3个飞机，9张牌
            if (_currentTurnCardCounts == 9)
            {
                for (int i = 0; i < Circulation_PlaneNoneTimes(_currentTurnCardCounts); i++)
                {
                    if (temp_plane_none[i].GetComponent<CardInformations>().CardValue + temp_plane_none[i + 1].GetComponent<CardInformations>().CardValue +
                        temp_plane_none[i + 2].GetComponent<CardInformations>().CardValue + temp_plane_none[i + 3].GetComponent<CardInformations>().CardValue +
                        temp_plane_none[i + 4].GetComponent<CardInformations>().CardValue + temp_plane_none[i + 5].GetComponent<CardInformations>().CardValue + 
                        temp_plane_none[i + 6].GetComponent<CardInformations>().CardValue + temp_plane_none[i + 7].GetComponent<CardInformations>().CardValue + 
                        temp_plane_none[i + 8].GetComponent<CardInformations>().CardValue > _currentTurnCardPoint)
                    {
                        tempConformPlaneNone.Add(temp_plane_none[i]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 1]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 2]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 3]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 4]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 5]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 6]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 7]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 8]);

                        break;
                    }
                }
            }

            //3，4个飞机，12张牌
            if (_currentTurnCardCounts == 12)
            {
                for (int i = 0; i < Circulation_PlaneNoneTimes(_currentTurnCardCounts); i++)
                {
                    if (temp_plane_none[i].GetComponent<CardInformations>().CardValue + temp_plane_none[i + 1].GetComponent<CardInformations>().CardValue +
                        temp_plane_none[i + 2].GetComponent<CardInformations>().CardValue + temp_plane_none[i + 3].GetComponent<CardInformations>().CardValue +
                        temp_plane_none[i + 4].GetComponent<CardInformations>().CardValue + temp_plane_none[i + 5].GetComponent<CardInformations>().CardValue +
                        temp_plane_none[i + 6].GetComponent<CardInformations>().CardValue + temp_plane_none[i + 7].GetComponent<CardInformations>().CardValue +
                        temp_plane_none[i + 8].GetComponent<CardInformations>().CardValue + temp_plane_none[i + 9].GetComponent<CardInformations>().CardValue + 
                        temp_plane_none[i + 10].GetComponent<CardInformations>().CardValue + temp_plane_none[i + 11].GetComponent<CardInformations>().CardValue > _currentTurnCardPoint)
                    {
                        tempConformPlaneNone.Add(temp_plane_none[i]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 1]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 2]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 3]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 4]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 5]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 6]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 7]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 8]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 9]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 10]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 11]);

                        break;
                    }
                }
            }

            //4，5个飞机，15张牌
            if (_currentTurnCardCounts == 15)
            {
                for (int i = 0; i < Circulation_PlaneNoneTimes(_currentTurnCardCounts); i++)
                {
                    if (temp_plane_none[i].GetComponent<CardInformations>().CardValue + temp_plane_none[i + 1].GetComponent<CardInformations>().CardValue +
                        temp_plane_none[i + 2].GetComponent<CardInformations>().CardValue + temp_plane_none[i + 3].GetComponent<CardInformations>().CardValue +
                        temp_plane_none[i + 4].GetComponent<CardInformations>().CardValue + temp_plane_none[i + 5].GetComponent<CardInformations>().CardValue +
                        temp_plane_none[i + 6].GetComponent<CardInformations>().CardValue + temp_plane_none[i + 7].GetComponent<CardInformations>().CardValue +
                        temp_plane_none[i + 8].GetComponent<CardInformations>().CardValue + temp_plane_none[i + 9].GetComponent<CardInformations>().CardValue +
                        temp_plane_none[i + 10].GetComponent<CardInformations>().CardValue + temp_plane_none[i + 11].GetComponent<CardInformations>().CardValue +
                        temp_plane_none[i + 12].GetComponent<CardInformations>().CardValue + temp_plane_none[i + 13].GetComponent<CardInformations>().CardValue +
                        temp_plane_none[i + 14].GetComponent<CardInformations>().CardValue  > _currentTurnCardPoint)
                    {
                        tempConformPlaneNone.Add(temp_plane_none[i]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 1]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 2]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 3]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 4]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 5]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 6]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 7]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 8]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 9]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 10]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 11]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 12]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 13]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 14]);

                        break;
                    }
                }
            }

            //5，6个飞机，18张牌
            if (_currentTurnCardCounts == 18)
            {
                for (int i = 0; i < Circulation_PlaneNoneTimes(_currentTurnCardCounts); i++)
                {
                    if (temp_plane_none[i].GetComponent<CardInformations>().CardValue + temp_plane_none[i + 1].GetComponent<CardInformations>().CardValue +
                        temp_plane_none[i + 2].GetComponent<CardInformations>().CardValue + temp_plane_none[i + 3].GetComponent<CardInformations>().CardValue +
                        temp_plane_none[i + 4].GetComponent<CardInformations>().CardValue + temp_plane_none[i + 5].GetComponent<CardInformations>().CardValue +
                        temp_plane_none[i + 6].GetComponent<CardInformations>().CardValue + temp_plane_none[i + 7].GetComponent<CardInformations>().CardValue +
                        temp_plane_none[i + 8].GetComponent<CardInformations>().CardValue + temp_plane_none[i + 9].GetComponent<CardInformations>().CardValue +
                        temp_plane_none[i + 10].GetComponent<CardInformations>().CardValue + temp_plane_none[i + 11].GetComponent<CardInformations>().CardValue +
                        temp_plane_none[i + 12].GetComponent<CardInformations>().CardValue + temp_plane_none[i + 13].GetComponent<CardInformations>().CardValue +
                        temp_plane_none[i + 14].GetComponent<CardInformations>().CardValue + temp_plane_none[i + 15].GetComponent<CardInformations>().CardValue +
                        temp_plane_none[i + 16].GetComponent<CardInformations>().CardValue + temp_plane_none[i + 17].GetComponent<CardInformations>().CardValue > _currentTurnCardPoint)
                    {
                        tempConformPlaneNone.Add(temp_plane_none[i]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 1]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 2]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 3]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 4]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 5]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 6]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 7]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 8]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 9]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 10]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 11]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 12]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 13]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 14]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 15]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 16]);
                        tempConformPlaneNone.Add(temp_plane_none[i + 17]);

                        break;
                    }
                }
            }

            //移除已添加的飞机不带
            for (int i = tempConformPlaneNone.Count - 1; i >= 0 ; i--)
            {
                if (temp_plane_none.Contains(tempConformPlaneNone[i]) == true)
                {
                    temp_plane_none.Remove(tempConformPlaneNone[i]);
                }
            }

            if (temp_plane_none.Count > 0 || temp_plane_none != null)
            {
                for (int i = 0; i < temp_plane_none.Count; i++)
                {
                    tripleList.Add(temp_plane_none[i]);
                }

                RemoveCardList(temp_plane_none);
            }

            OrderCardsList(tripleList);

            OrderCardsList(tempConformPlaneNone);
        }

        //获取当前回合的飞机不带
        private void GetCurrentTurnPlaneNone() 
        {
            for (int i = 0; i < Circulation_PlaneNoneTimes(tripleList.Count); i += 3)
            {
                if (tripleList[i].GetComponent<CardInformations>().CardValue - tripleList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
                    tripleList[i + 1].GetComponent<CardInformations>().CardValue - tripleList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
                    tripleList[i + 2].GetComponent<CardInformations>().CardValue - tripleList[i + 5].GetComponent<CardInformations>().CardValue == -1)
                {
                    if (temp_plane_none.Contains(tripleList[i]) == false)
                    {
                        temp_plane_none.Add(tripleList[i]);
                    }

                    if (temp_plane_none.Contains(tripleList[i + 1]) == false)
                    {
                        temp_plane_none.Add(tripleList[i + 1]);
                    }

                    if (temp_plane_none.Contains(tripleList[i + 2]) == false)
                    {
                        temp_plane_none.Add(tripleList[i + 2]);
                    }

                    if (temp_plane_none.Contains(tripleList[i + 3]) == false)
                    {
                        temp_plane_none.Add(tripleList[i + 3]);
                    }

                    if (temp_plane_none.Contains(tripleList[i + 4]) == false)
                    {
                        temp_plane_none.Add(tripleList[i + 4]);
                    }

                    if (temp_plane_none.Contains(tripleList[i + 5]) == false)
                    {
                        temp_plane_none.Add(tripleList[i + 5]);
                    }
                }
            }

            for (int i = temp_plane_none.Count - 1; i >= 0 ; i--)
            {
                if (tripleList.Contains(temp_plane_none[i]) == true)
                {
                    tripleList.Remove(temp_plane_none[i]);
                }
            }

            OrderCardsList(temp_plane_none);
        }

        //判断当前回合循环获取飞机不带的次数
        private int Circulation_PlaneNoneTimes(int _planeNone) 
        {
            switch (_planeNone)
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

        //判断当前回合循环boom库的次数
        private int Circulation_BoomTimes(int _boom) 
        {
            switch (_boom)
            {
                case 4:
                    return 1;
                case 8:
                    return 5;
                case 12:
                    return 9;
                case 16:
                    return 13;
                case 20:
                    return 17;
            }

            return 0;
        }

        //判断当前回合循环triple库的次数
        private int Circulation_TripleTimes(int _triple) 
        {
            switch (_triple)
            {
                case 3:
                    return 1;
                case 6:
                    return 4;
                case 9:
                    return 7;
                case 12:
                    return 10;
                case 15:
                    return 13;
                case 18:
                    return 16;
            }

            return 0;
        }

        //重新分类-后手出牌用
        public void ReClassify_Gote() 
        {
            if (singleList.Count != 0 || singleList != null)
            {
                for (int i = 0; i < singleList.Count; i++)
                {
                    handCardGit.Add(singleList[i]);
                }
            }

            if (pairList.Count != 0 || pairList != null)
            {
                for (int i = 0; i < pairList.Count; i++)
                {
                    handCardGit.Add(pairList[i]);
                }
            }

            if (tripleList.Count != 0 || tripleList != null)
            {
                for (int i = 0; i < tripleList.Count; i++)
                {
                    handCardGit.Add(tripleList[i]);
                }
            }

            if (boomList.Count != 0 || boomList != null)
            {
                for (int i = 0; i < boomList.Count; i++)
                {
                    handCardGit.Add(boomList[i]);
                }
            }

            if (jokerBoomList.Count != 0 || jokerBoomList != null)
            {
                for (int i = 0; i < jokerBoomList.Count; i++)
                {
                    handCardGit.Add(jokerBoomList[i]);
                }
            }

            if (temp_plane_none.Count != 0 || temp_plane_none != null)
            {
                for (int i = 0; i < temp_plane_none.Count; i++)
                {
                    handCardGit.Add(temp_plane_none[i]);
                }
            }

            RemoveCardList(singleList);
            RemoveCardList(pairList);
            RemoveCardList(tripleList);
            RemoveCardList(boomList);
            RemoveCardList(jokerBoomList);
            RemoveCardList(temp_plane_none);

            ClassifyHandCards();
        }

        #endregion
    }
}

#region No Use test 1

//test
//public void CurrentTurn_Single(int _currentTurnCardPoints)
//{
//    //当单牌库有牌时
//    if (singleList.Count > 0)
//    {
//        //循环判断是否有满足条件的牌
//        for (int i = 0; i < singleList.Count; i++)
//        {
//            if (singleList[i].GetComponent<CardInformations>().CardValue > _currentTurnCardPoints)
//            {
//                //若有则添加到出牌列表
//                currentTurnPlayCard.Add(singleList[i]);

//                //再从单牌库中将其移除
//                singleList.Remove(singleList[i]);

//                OrderCardsList(singleList);

//                break;
//            }
//        }
//    }

//    //当单牌库没牌时
//    //判断对子库有牌时
//    if (pairList.Count > 0)
//    {
//        //同理
//        for (int i = 0; i < pairList.Count; i++)
//        {
//            if (pairList[i].GetComponent<CardInformations>().CardValue > _currentTurnCardPoints)
//            {
//                //同理，若有满足条件的牌，则将其添加到出牌列表   
//                currentTurnPlayCard.Add(pairList[i]);

//                //然后将另外一张多出来的单牌放到单牌库
//                singleList.Add(pairList[i + 1]);

//                //最后将它们从对字库中移除
//                pairList.Remove(pairList[i + 1]);
//                pairList.Remove(pairList[i]);

//                OrderCardsList(singleList);
//                OrderCardsList(pairList);

//                break;
//            }
//        }
//    }

//    //当单牌、对字库均没有符合条件的牌时
//    //判断三连库
//    if (tripleList.Count > 0)
//    {
//        //同理
//        for (int i = 0; i < tripleList.Count; i++)
//        {
//            if (tripleList[i].GetComponent<CardInformations>().CardValue > _currentTurnCardPoints)
//            {
//                currentTurnPlayCard.Add(tripleList[i]);

//                pairList.Add(tripleList[i + 1]);
//                pairList.Add(tripleList[i + 2]);

//                tripleList.Remove(tripleList[i + 2]);
//                tripleList.Remove(tripleList[i + 1]);
//                tripleList.Remove(tripleList[i]);

//                OrderCardsList(singleList);
//                OrderCardsList(pairList);
//                OrderCardsList(tripleList);

//                break;
//            }
//        }
//    }

//    //return null;
//}

#endregion

#region No Use test 2

//test 2
//private List<GameObject> CurrentTurn_Straight(List<GameObject> _targetCardTypeList, List<GameObject> _targetCardList, int _currentTurnCardCounts, int _currentTurnCardPoints)
//{
//    //1，当有5张顺子时
//    if (_currentTurnCardCounts == 5)
//    {
//        for (int i = 0; i < _targetCardTypeList.Count - (_currentTurnCardCounts - 1); i++)
//        {
//            if (_targetCardTypeList[i].GetComponent<CardInformations>().CardValue - _targetCardTypeList[i + 1].GetComponent<CardInformations>().CardValue == -1 &&
//                _targetCardTypeList[i + 1].GetComponent<CardInformations>().CardValue - _targetCardTypeList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
//                _targetCardTypeList[i + 2].GetComponent<CardInformations>().CardValue - _targetCardTypeList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
//                _targetCardTypeList[i + 3].GetComponent<CardInformations>().CardValue - _targetCardTypeList[i + 4].GetComponent<CardInformations>().CardValue == -1)
//            {
//                if (_targetCardTypeList[i].GetComponent<CardInformations>().CardValue +
//                    _targetCardTypeList[i + 1].GetComponent<CardInformations>().CardValue +
//                    _targetCardTypeList[i + 2].GetComponent<CardInformations>().CardValue +
//                    _targetCardTypeList[i + 3].GetComponent<CardInformations>().CardValue +
//                    _targetCardTypeList[i + 4].GetComponent<CardInformations>().CardValue > _currentTurnCardPoints)
//                {
//                    _targetCardList.Add(_targetCardTypeList[i]);
//                    _targetCardList.Add(_targetCardTypeList[i + 1]);
//                    _targetCardList.Add(_targetCardTypeList[i + 2]);
//                    _targetCardList.Add(_targetCardTypeList[i + 3]);
//                    _targetCardList.Add(_targetCardTypeList[i + 4]);

//                    _targetCardTypeList.Remove(_targetCardTypeList[i + 4]);
//                    _targetCardTypeList.Remove(_targetCardTypeList[i + 3]);
//                    _targetCardTypeList.Remove(_targetCardTypeList[i + 2]);
//                    _targetCardTypeList.Remove(_targetCardTypeList[i + 1]);
//                    _targetCardTypeList.Remove(_targetCardTypeList[i]);

//                    OrderCardsList(_targetCardTypeList);

//                    return _targetCardList;
//                }
//            }
//        }
//    }

//    //2，当有6张顺子时
//    if (_currentTurnCardCounts == 6)
//    {
//        for (int i = 0; i < singleList.Count - (_currentTurnCardCounts - 1); i++)
//        {
//            if (singleList[i].GetComponent<CardInformations>().CardValue - singleList[i + 1].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 1].GetComponent<CardInformations>().CardValue - singleList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 2].GetComponent<CardInformations>().CardValue - singleList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 3].GetComponent<CardInformations>().CardValue - singleList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 4].GetComponent<CardInformations>().CardValue - singleList[i + 5].GetComponent<CardInformations>().CardValue == -1)
//            {
//                if (singleList[i].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 1].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 2].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 3].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 4].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 5].GetComponent<CardInformations>().CardValue > _currentTurnCardPoints)
//                {
//                    tempPlayCardList.Add(singleList[i]);
//                    tempPlayCardList.Add(singleList[i + 1]);
//                    tempPlayCardList.Add(singleList[i + 2]);
//                    tempPlayCardList.Add(singleList[i + 3]);
//                    tempPlayCardList.Add(singleList[i + 4]);
//                    tempPlayCardList.Add(singleList[i + 5]);

//                    singleList.Remove(singleList[i + 5]);
//                    singleList.Remove(singleList[i + 4]);
//                    singleList.Remove(singleList[i + 3]);
//                    singleList.Remove(singleList[i + 2]);
//                    singleList.Remove(singleList[i + 1]);
//                    singleList.Remove(singleList[i]);

//                    OrderCardsList(singleList);

//                    return tempPlayCardList;
//                }
//            }
//        }
//    }

//    //3，当有7张顺子时
//    if (_currentTurnCardCounts == 7)
//    {
//        for (int i = 0; i < singleList.Count - (_currentTurnCardCounts - 1); i++)
//        {
//            if (singleList[i].GetComponent<CardInformations>().CardValue - singleList[i + 1].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 1].GetComponent<CardInformations>().CardValue - singleList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 2].GetComponent<CardInformations>().CardValue - singleList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 3].GetComponent<CardInformations>().CardValue - singleList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 4].GetComponent<CardInformations>().CardValue - singleList[i + 5].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 5].GetComponent<CardInformations>().CardValue - singleList[i + 6].GetComponent<CardInformations>().CardValue == -1)
//            {
//                if (singleList[i].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 1].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 2].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 3].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 4].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 5].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 6].GetComponent<CardInformations>().CardValue > _currentTurnCardPoints)
//                {
//                    tempPlayCardList.Add(singleList[i]);
//                    tempPlayCardList.Add(singleList[i + 1]);
//                    tempPlayCardList.Add(singleList[i + 2]);
//                    tempPlayCardList.Add(singleList[i + 3]);
//                    tempPlayCardList.Add(singleList[i + 4]);
//                    tempPlayCardList.Add(singleList[i + 5]);
//                    tempPlayCardList.Add(singleList[i + 6]);

//                    singleList.Remove(singleList[i + 6]);
//                    singleList.Remove(singleList[i + 5]);
//                    singleList.Remove(singleList[i + 4]);
//                    singleList.Remove(singleList[i + 3]);
//                    singleList.Remove(singleList[i + 2]);
//                    singleList.Remove(singleList[i + 1]);
//                    singleList.Remove(singleList[i]);

//                    OrderCardsList(singleList);

//                    return tempPlayCardList;
//                }
//            }
//        }
//    }

//    //4，当有8张顺子时
//    if (_currentTurnCardCounts == 8)
//    {
//        for (int i = 0; i < singleList.Count - (_currentTurnCardCounts - 1); i++)
//        {
//            if (singleList[i].GetComponent<CardInformations>().CardValue - singleList[i + 1].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 1].GetComponent<CardInformations>().CardValue - singleList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 2].GetComponent<CardInformations>().CardValue - singleList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 3].GetComponent<CardInformations>().CardValue - singleList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 4].GetComponent<CardInformations>().CardValue - singleList[i + 5].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 5].GetComponent<CardInformations>().CardValue - singleList[i + 6].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 6].GetComponent<CardInformations>().CardValue - singleList[i + 7].GetComponent<CardInformations>().CardValue == -1)
//            {
//                if (singleList[i].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 1].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 2].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 3].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 4].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 5].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 6].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 7].GetComponent<CardInformations>().CardValue > _currentTurnCardPoints)
//                {
//                    tempPlayCardList.Add(singleList[i]);
//                    tempPlayCardList.Add(singleList[i + 1]);
//                    tempPlayCardList.Add(singleList[i + 2]);
//                    tempPlayCardList.Add(singleList[i + 3]);
//                    tempPlayCardList.Add(singleList[i + 4]);
//                    tempPlayCardList.Add(singleList[i + 5]);
//                    tempPlayCardList.Add(singleList[i + 6]);
//                    tempPlayCardList.Add(singleList[i + 7]);

//                    singleList.Remove(singleList[i + 7]);
//                    singleList.Remove(singleList[i + 6]);
//                    singleList.Remove(singleList[i + 5]);
//                    singleList.Remove(singleList[i + 4]);
//                    singleList.Remove(singleList[i + 3]);
//                    singleList.Remove(singleList[i + 2]);
//                    singleList.Remove(singleList[i + 1]);
//                    singleList.Remove(singleList[i]);

//                    OrderCardsList(singleList);

//                    return tempPlayCardList;
//                }
//            }
//        }
//    }

//    //5，当有9张顺子时
//    if (_currentTurnCardCounts == 9)
//    {
//        for (int i = 0; i < singleList.Count - (_currentTurnCardCounts - 1); i++)
//        {
//            if (singleList[i].GetComponent<CardInformations>().CardValue - singleList[i + 1].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 1].GetComponent<CardInformations>().CardValue - singleList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 2].GetComponent<CardInformations>().CardValue - singleList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 3].GetComponent<CardInformations>().CardValue - singleList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 4].GetComponent<CardInformations>().CardValue - singleList[i + 5].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 5].GetComponent<CardInformations>().CardValue - singleList[i + 6].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 6].GetComponent<CardInformations>().CardValue - singleList[i + 7].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 7].GetComponent<CardInformations>().CardValue - singleList[i + 8].GetComponent<CardInformations>().CardValue == -1)
//            {
//                if (singleList[i].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 1].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 2].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 3].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 4].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 5].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 6].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 7].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 8].GetComponent<CardInformations>().CardValue > _currentTurnCardPoints)
//                {
//                    tempPlayCardList.Add(singleList[i]);
//                    tempPlayCardList.Add(singleList[i + 1]);
//                    tempPlayCardList.Add(singleList[i + 2]);
//                    tempPlayCardList.Add(singleList[i + 3]);
//                    tempPlayCardList.Add(singleList[i + 4]);
//                    tempPlayCardList.Add(singleList[i + 5]);
//                    tempPlayCardList.Add(singleList[i + 6]);
//                    tempPlayCardList.Add(singleList[i + 7]);
//                    tempPlayCardList.Add(singleList[i + 8]);

//                    singleList.Remove(singleList[i + 8]);
//                    singleList.Remove(singleList[i + 7]);
//                    singleList.Remove(singleList[i + 6]);
//                    singleList.Remove(singleList[i + 5]);
//                    singleList.Remove(singleList[i + 4]);
//                    singleList.Remove(singleList[i + 3]);
//                    singleList.Remove(singleList[i + 2]);
//                    singleList.Remove(singleList[i + 1]);
//                    singleList.Remove(singleList[i]);

//                    OrderCardsList(singleList);

//                    return tempPlayCardList;
//                }
//            }
//        }
//    }

//    //6，当有10张顺子时
//    if (_currentTurnCardCounts == 10)
//    {
//        for (int i = 0; i < singleList.Count - (_currentTurnCardCounts - 1); i++)
//        {
//            if (singleList[i].GetComponent<CardInformations>().CardValue - singleList[i + 1].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 1].GetComponent<CardInformations>().CardValue - singleList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 2].GetComponent<CardInformations>().CardValue - singleList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 3].GetComponent<CardInformations>().CardValue - singleList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 4].GetComponent<CardInformations>().CardValue - singleList[i + 5].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 5].GetComponent<CardInformations>().CardValue - singleList[i + 6].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 6].GetComponent<CardInformations>().CardValue - singleList[i + 7].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 7].GetComponent<CardInformations>().CardValue - singleList[i + 8].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 8].GetComponent<CardInformations>().CardValue - singleList[i + 9].GetComponent<CardInformations>().CardValue == -1)
//            {
//                if (singleList[i].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 1].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 2].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 3].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 4].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 5].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 6].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 7].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 8].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 9].GetComponent<CardInformations>().CardValue > _currentTurnCardPoints)
//                {
//                    tempPlayCardList.Add(singleList[i]);
//                    tempPlayCardList.Add(singleList[i + 1]);
//                    tempPlayCardList.Add(singleList[i + 2]);
//                    tempPlayCardList.Add(singleList[i + 3]);
//                    tempPlayCardList.Add(singleList[i + 4]);
//                    tempPlayCardList.Add(singleList[i + 5]);
//                    tempPlayCardList.Add(singleList[i + 6]);
//                    tempPlayCardList.Add(singleList[i + 7]);
//                    tempPlayCardList.Add(singleList[i + 8]);
//                    tempPlayCardList.Add(singleList[i + 9]);

//                    singleList.Remove(singleList[i + 9]);
//                    singleList.Remove(singleList[i + 8]);
//                    singleList.Remove(singleList[i + 7]);
//                    singleList.Remove(singleList[i + 6]);
//                    singleList.Remove(singleList[i + 5]);
//                    singleList.Remove(singleList[i + 4]);
//                    singleList.Remove(singleList[i + 3]);
//                    singleList.Remove(singleList[i + 2]);
//                    singleList.Remove(singleList[i + 1]);
//                    singleList.Remove(singleList[i]);

//                    OrderCardsList(singleList);

//                    return tempPlayCardList;
//                }
//            }
//        }
//    }

//    //7，当有11张顺子时
//    if (_currentTurnCardCounts == 11)
//    {
//        for (int i = 0; i < singleList.Count - (_currentTurnCardCounts - 1); i++)
//        {
//            if (singleList[i].GetComponent<CardInformations>().CardValue - singleList[i + 1].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 1].GetComponent<CardInformations>().CardValue - singleList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 2].GetComponent<CardInformations>().CardValue - singleList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 3].GetComponent<CardInformations>().CardValue - singleList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 4].GetComponent<CardInformations>().CardValue - singleList[i + 5].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 5].GetComponent<CardInformations>().CardValue - singleList[i + 6].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 6].GetComponent<CardInformations>().CardValue - singleList[i + 7].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 7].GetComponent<CardInformations>().CardValue - singleList[i + 8].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 8].GetComponent<CardInformations>().CardValue - singleList[i + 9].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 9].GetComponent<CardInformations>().CardValue - singleList[i + 10].GetComponent<CardInformations>().CardValue == -1)
//            {
//                if (singleList[i].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 1].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 2].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 3].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 4].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 5].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 6].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 7].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 8].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 9].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 10].GetComponent<CardInformations>().CardValue > _currentTurnCardPoints)
//                {
//                    tempPlayCardList.Add(singleList[i]);
//                    tempPlayCardList.Add(singleList[i + 1]);
//                    tempPlayCardList.Add(singleList[i + 2]);
//                    tempPlayCardList.Add(singleList[i + 3]);
//                    tempPlayCardList.Add(singleList[i + 4]);
//                    tempPlayCardList.Add(singleList[i + 5]);
//                    tempPlayCardList.Add(singleList[i + 6]);
//                    tempPlayCardList.Add(singleList[i + 7]);
//                    tempPlayCardList.Add(singleList[i + 8]);
//                    tempPlayCardList.Add(singleList[i + 9]);
//                    tempPlayCardList.Add(singleList[i + 10]);

//                    singleList.Remove(singleList[i + 10]);
//                    singleList.Remove(singleList[i + 9]);
//                    singleList.Remove(singleList[i + 8]);
//                    singleList.Remove(singleList[i + 7]);
//                    singleList.Remove(singleList[i + 6]);
//                    singleList.Remove(singleList[i + 5]);
//                    singleList.Remove(singleList[i + 4]);
//                    singleList.Remove(singleList[i + 3]);
//                    singleList.Remove(singleList[i + 2]);
//                    singleList.Remove(singleList[i + 1]);
//                    singleList.Remove(singleList[i]);

//                    OrderCardsList(singleList);

//                    return tempPlayCardList;
//                }
//            }
//        }
//    }

//    //8，当有12张顺子时
//    if (_currentTurnCardCounts == 12)
//    {
//        for (int i = 0; i < singleList.Count - (_currentTurnCardCounts - 1); i++)
//        {
//            if (singleList[i].GetComponent<CardInformations>().CardValue - singleList[i + 1].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 1].GetComponent<CardInformations>().CardValue - singleList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 2].GetComponent<CardInformations>().CardValue - singleList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 3].GetComponent<CardInformations>().CardValue - singleList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 4].GetComponent<CardInformations>().CardValue - singleList[i + 5].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 5].GetComponent<CardInformations>().CardValue - singleList[i + 6].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 6].GetComponent<CardInformations>().CardValue - singleList[i + 7].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 7].GetComponent<CardInformations>().CardValue - singleList[i + 8].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 8].GetComponent<CardInformations>().CardValue - singleList[i + 9].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 9].GetComponent<CardInformations>().CardValue - singleList[i + 10].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 10].GetComponent<CardInformations>().CardValue - singleList[i + 11].GetComponent<CardInformations>().CardValue == -1)
//            {
//                if (singleList[i].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 1].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 2].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 3].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 4].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 5].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 6].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 7].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 8].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 9].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 10].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 11].GetComponent<CardInformations>().CardValue > _currentTurnCardPoints)
//                {
//                    tempPlayCardList.Add(singleList[i]);
//                    tempPlayCardList.Add(singleList[i + 1]);
//                    tempPlayCardList.Add(singleList[i + 2]);
//                    tempPlayCardList.Add(singleList[i + 3]);
//                    tempPlayCardList.Add(singleList[i + 4]);
//                    tempPlayCardList.Add(singleList[i + 5]);
//                    tempPlayCardList.Add(singleList[i + 6]);
//                    tempPlayCardList.Add(singleList[i + 7]);
//                    tempPlayCardList.Add(singleList[i + 8]);
//                    tempPlayCardList.Add(singleList[i + 9]);
//                    tempPlayCardList.Add(singleList[i + 10]);
//                    tempPlayCardList.Add(singleList[i + 11]);

//                    singleList.Remove(singleList[i + 11]);
//                    singleList.Remove(singleList[i + 10]);
//                    singleList.Remove(singleList[i + 9]);
//                    singleList.Remove(singleList[i + 8]);
//                    singleList.Remove(singleList[i + 7]);
//                    singleList.Remove(singleList[i + 6]);
//                    singleList.Remove(singleList[i + 5]);
//                    singleList.Remove(singleList[i + 4]);
//                    singleList.Remove(singleList[i + 3]);
//                    singleList.Remove(singleList[i + 2]);
//                    singleList.Remove(singleList[i + 1]);
//                    singleList.Remove(singleList[i]);

//                    OrderCardsList(singleList);

//                    return tempPlayCardList;
//                }
//            }
//        }
//    }

//    return null;
//}

#endregion

#region No Use test 3

//private IEnumerator OrderForJudgeStraight2()
//{
//    OrderCardsList(singleList);

//    yield return new WaitForFixedUpdate();

//    for (int j = 0; j < pairList.Count; j++)
//    {
//        singleList.Add(pairList[j]);
//    }

//    pairList.Clear();

//    yield return new WaitForFixedUpdate();

//    OrderCardsList(singleList);

//    yield return new WaitForFixedUpdate();

//    //从单牌库中重新添加对子
//    for (int j = 0; j < singleList.Count - 1; j++)
//    {
//        if (singleList[j].gameObject.GetComponent<CardInformations>().CardValue == singleList[j + 1].gameObject.GetComponent<CardInformations>().CardValue)
//        {
//            pairList.Add(singleList[j]);
//            pairList.Add(singleList[j + 1]);

//            singleList.Remove(singleList[j + 1]);
//            singleList.Remove(singleList[j]);
//        }
//    }

//    yield return new WaitForFixedUpdate();

//    OrderCardsList(singleList);

//    yield return new WaitForFixedUpdate();
//}

#endregion

#region No Use test 4

//private void RecoverPairList()
//{
//    ///Question:为什么我用我的之前的方法会出问题
//    ///而用我现在这样的算法就没有问题
//    ///

//    List<GameObject> temp = new List<GameObject>();

//    temp.Clear();

//    for (int i = 0; i < singleList.Count; i++)
//    {
//        temp.Add(singleList[i]);
//    }

//    for (int i = 0; i < pairList.Count; i++)
//    {
//        temp.Add(pairList[i]);
//    }

//    singleList.Clear();
//    pairList.Clear();

//    OrderCardsList(temp);

//    for (int i = 0; i < temp.Count - 1; i++)
//    {
//        if (temp[i].gameObject.GetComponent<CardInformations>().CardValue == temp[i + 1].gameObject.GetComponent<CardInformations>().CardValue)
//        {
//            pairList.Add(temp[i]);
//            pairList.Add(temp[i + 1]);

//            temp.Remove(temp[i + 1]);
//            temp.Remove(temp[i]);
//        }
//    }

//    for (int i = 0; i < temp.Count; i++)
//    {
//        singleList.Add(temp[i]);
//    }

//    temp.Clear();

//    OrderCardsList(singleList);
//    OrderCardsList(pairList);
//}

#endregion

#region Wrong 1

//List<GameObject> tempPlayCardList = new List<GameObject>();

//tempPlayCardList.Clear();

//int _currentTurnTriplePartPoint = 0;

////得到当前回合的三带一的三连部分的点数
//for (int i = 0; i < _currentTurnCardList.Count - 2; i++)
//{
//    if (_currentTurnCardList[i].GetComponent<CardInformations>().CardValue == _currentTurnCardList[i + 1].GetComponent<CardInformations>().CardValue &&
//        _currentTurnCardList[i].GetComponent<CardInformations>().CardValue == _currentTurnCardList[i + 2].GetComponent<CardInformations>().CardValue)
//    {
//        _currentTurnTriplePartPoint += _currentTurnCardList[i].GetComponent<CardInformations>().CardValue;
//        _currentTurnTriplePartPoint += _currentTurnCardList[i + 1].GetComponent<CardInformations>().CardValue;
//        _currentTurnTriplePartPoint += _currentTurnCardList[i + 2].GetComponent<CardInformations>().CardValue;

//        Debug.Log(_currentTurnTriplePartPoint);
//    }
//}

//for (int i = 0; i < TripleTimes(tripleList.Count); i += 3)
//{
//    if (tripleList[i].GetComponent<CardInformations>().CardValue +
//        tripleList[i + 1].GetComponent<CardInformations>().CardValue +
//        tripleList[i + 2].GetComponent<CardInformations>().CardValue > _currentTurnTriplePartPoint)
//    {
//        tempPlayCardList.Add(tripleList[i]);
//        tempPlayCardList.Add(tripleList[i + 1]);
//        tempPlayCardList.Add(tripleList[i + 2]);

//        tripleList.Remove(tripleList[i + 2]);
//        tripleList.Remove(tripleList[i + 1]);
//        tripleList.Remove(tripleList[i]);

//        if (singleList.Count > 0)
//        {
//            tempPlayCardList.Add(singleList[0]);
//        }
//        else
//        {
//            //同理
//            //当单牌库没牌时
//            //判断对子库有牌时
//            if (pairList.Count > 0)
//            {
//                tempPlayCardList.Add(pairList[0]);

//                singleList.Add(pairList[1]);

//                pairList.Remove(pairList[1]);
//                pairList.Remove(pairList[0]);
//            }
//        }

//        for (int j = 0; j < tempPlayCardList.Count; j++)
//        {
//            if (singleList.Contains(tempPlayCardList[j]) == true)
//            {
//                singleList.Remove(tempPlayCardList[j]);
//            }
//        }

//        OrderCardsList(singleList);
//        OrderCardsList(pairList);
//        OrderCardsList(tripleList);

//        OrderCardsList(tempPlayCardList);

//        return tempPlayCardList;

//    }
//}

//return null;

#endregion

#region No Use Test 5

//#region No Use test 5

////若当前回合是AI先手
//public List<GameObject> CurrentTurn_AIFirst()
//{
//    List<GameObject> tempPlayCardList = new List<GameObject>();
//    tempPlayCardList.Clear();

//    Judge_Plane_Pair();
//    Judge_Plane_Single();
//    Judge_Plane_None();
//    Judge_Boom_Pair();
//    Judge_BoomSingle();
//    Judge_Straight_Pair();
//    Judge_Straight();
//    Judge_Triple_Pair();
//    Judge_Triple_Single();
//    Judge_Boom();
//    Judge_Triple();
//    Judge_Pair();
//    Judge_JokerBoom();
//    Judge_Single();

//    for (int i = 0; i < temp_AI_First_Hand.Count; i++)
//    {
//        tempPlayCardList.Add(temp_AI_First_Hand[i]);
//    }

//    OrderCardsList(tempPlayCardList);

//    ReClassify();

//    return tempPlayCardList;
//}

////重新分类
//private void ReClassify()
//{
//    for (int i = 0; i < singleList.Count; i++)
//    {
//        if (handCardGit.Contains(singleList[i]) == false)
//        {
//            handCardGit.Add(singleList[i]);
//        }
//    }

//    for (int i = 0; i < pairList.Count; i++)
//    {
//        if (handCardGit.Contains(pairList[i]) == false)
//        {
//            handCardGit.Add(pairList[i]);
//        }
//    }

//    for (int i = 0; i < tripleList.Count; i++)
//    {
//        if (handCardGit.Contains(tripleList[i]) == false)
//        {
//            handCardGit.Add(tripleList[i]);
//        }
//    }

//    for (int i = 0; i < boomList.Count; i++)
//    {
//        if (handCardGit.Contains(boomList[i]) == false)
//        {
//            handCardGit.Add(boomList[i]);
//        }
//    }

//    for (int i = 0; i < jokerBoomList.Count; i++)
//    {
//        if (handCardGit.Contains(jokerBoomList[i]) == false)
//        {
//            handCardGit.Add(jokerBoomList[i]);
//        }
//    }

//    for (int i = 0; i < temp_plane_counts.Count; i++)
//    {
//        if (handCardGit.Contains(temp_plane_counts[i]) == false)
//        {
//            handCardGit.Add(temp_plane_counts[i]);
//        }
//    }

//    for (int i = 0; i < temp_AI_First_Hand.Count; i++)
//    {
//        if (handCardGit.Contains(temp_AI_First_Hand[i]) == false)
//        {
//            handCardGit.Add(temp_AI_First_Hand[i]);
//        }
//    }

//    singleList.Clear();
//    pairList.Clear();
//    tripleList.Clear();
//    boomList.Clear();
//    jokerBoomList.Clear();
//    temp_plane_counts.Clear();
//    temp_AI_First_Hand.Clear();

//    ClassifyHandCards();
//}

////判断---飞机带对---至少10张
//private void Judge_Plane_Pair()
//{
//    List<GameObject> tempPlayCardList = new List<GameObject>();
//    tempPlayCardList.Clear();

//    //获取飞机
//    JudgePlaneCounts();

//    if (temp_plane_counts.Count < 6)
//    {
//        ReClassify();
//        return;
//    }
//    else
//    {
//        for (int i = 0; i < temp_plane_counts.Count; i++)
//        {
//            tempPlayCardList.Add(temp_plane_counts[i]);
//        }
//    }

//    //当对子数量加上三连里的对子数量还是小于当前飞机带对需要的数量时，将飞机退回一个
//    while (pairList.Count + tripleList.Count / 3 * 2 < temp_plane_counts.Count / 3 * 2)
//    {
//        SendBackPlane();

//        //清理当前出牌列表，重新添加飞机
//        tempPlayCardList.Clear();

//        for (int i = 0; i < temp_plane_counts.Count; i++)
//        {
//            tempPlayCardList.Add(temp_plane_counts[i]);
//        }
//    }

//    //因为一直退回可能导致最后破坏飞机牌型，甚至完全退回了
//    if (temp_plane_counts.Count < 6)
//    {
//        ReClassify();
//        return;
//    }

//    //如果对子数量加上拆三连库后得到的总的对子数量还是小于当前飞机的对应的对子数量，那就为null
//    if (pairList.Count < temp_plane_counts.Count / 3 * 2 &&
//        pairList.Count + tripleList.Count / 3 * 2 < temp_plane_counts.Count / 3 * 2)
//    {
//        ReClassify();
//        return;
//    }

//    //获取对子
//    //若对子数量足够组合出飞机带对，则完成组合出牌
//    if (pairList.Count >= temp_plane_counts.Count / 3 * 2)
//    {
//        for (int i = 0; i < temp_plane_counts.Count / 3 * 2; i++)
//        {
//            tempPlayCardList.Add(pairList[i]);
//        }

//        for (int i = 0; i < tempPlayCardList.Count; i++)
//        {
//            if (pairList.Contains(tempPlayCardList[i]) == true)
//            {
//                pairList.Remove(tempPlayCardList[i]);
//            }
//        }
//    }
//    else
//    {
//        //反之拆三连库
//        for (int i = 0; i < tripleList.Count; i++)
//        {
//            if (pairList.Contains(tripleList[i]) == false)
//            {
//                pairList.Add(tripleList[i]);
//                pairList.Add(tripleList[i + 1]);

//                tripleList.Remove(tripleList[i + 1]);
//                tripleList.Remove(tripleList[i]);
//            }
//        }

//        for (int i = 0; i < temp_plane_counts.Count / 3 * 2; i++)
//        {
//            tempPlayCardList.Add(pairList[i]);
//        }

//        //从对子库中移除使用过得牌
//        for (int i = 0; i < tempPlayCardList.Count; i++)
//        {
//            if (pairList.Contains(tempPlayCardList[i]) == true)
//            {
//                pairList.Remove(tempPlayCardList[i]);
//            }
//        }
//    }

//    OrderCardsList(tempPlayCardList);

//    JudgeCurrentCardCombination(tempPlayCardList);

//    ReClassify();

//    tempPlayCardList.Clear();
//}

////判断---飞机带单---至少8张
//private void Judge_Plane_Single()
//{
//    //与飞机带单同理
//    List<GameObject> tempPlayCardList = new List<GameObject>();
//    tempPlayCardList.Clear();

//    //获取飞机
//    JudgePlaneCounts();

//    if (temp_plane_counts.Count < 0)
//    {
//        ReClassify();
//        return;
//    }
//    else
//    {
//        for (int i = 0; i < temp_plane_counts.Count; i++)
//        {
//            tempPlayCardList.Add(temp_plane_counts[i]);
//        }
//    }

//    while (singleList.Count + pairList.Count / 2 < temp_plane_counts.Count / 3)
//    {
//        SendBackPlane();

//        //清理当前出牌列表，重新添加飞机
//        tempPlayCardList.Clear();

//        for (int i = 0; i < temp_plane_counts.Count; i++)
//        {
//            tempPlayCardList.Add(temp_plane_counts[i]);
//        }
//    }

//    if (temp_plane_counts.Count < 6)
//    {
//        ReClassify();
//        return;
//    }

//    //获取单牌
//    if (singleList.Count < temp_plane_counts.Count / 3 &&
//        singleList.Count + pairList.Count / 2 < temp_plane_counts.Count / 3)
//    {
//        ReClassify();
//        return;
//    }

//    //若对子数量足够组合出飞机带对，则完成组合出牌
//    if (singleList.Count >= temp_plane_counts.Count / 3)
//    {
//        for (int i = 0; i < temp_plane_counts.Count / 3; i++)
//        {
//            tempPlayCardList.Add(singleList[i]);
//        }

//        for (int i = 0; i < tempPlayCardList.Count; i++)
//        {
//            if (singleList.Contains(tempPlayCardList[i]) == true)
//            {
//                singleList.Remove(tempPlayCardList[i]);
//            }
//        }
//    }
//    else
//    {
//        //反之拆对子库
//        for (int i = 0; i < pairList.Count; i++)
//        {
//            if (singleList.Contains(pairList[i]) == false)
//            {
//                singleList.Add(pairList[i]);
//            }
//        }

//        for (int i = 0; i < singleList.Count; i++)
//        {
//            if (pairList.Contains(singleList[i]) == true)
//            {
//                pairList.Remove(singleList[i]);
//            }
//        }

//        for (int i = 0; i < temp_plane_counts.Count / 3; i++)
//        {
//            tempPlayCardList.Add(singleList[i]);
//        }

//        //从单牌库中移除使用过得牌
//        for (int i = 0; i < tempPlayCardList.Count; i++)
//        {
//            if (singleList.Contains(tempPlayCardList[i]) == true)
//            {
//                singleList.Remove(tempPlayCardList[i]);
//            }
//        }
//    }

//    OrderCardsList(tempPlayCardList);

//    JudgeCurrentCardCombination(tempPlayCardList);

//    ReClassify();

//    tempPlayCardList.Clear();
//}

////判断---四带对---至少8张
//private void Judge_Boom_Pair()
//{
//    List<GameObject> tempPlayCardList = new List<GameObject>();
//    tempPlayCardList.Clear();

//    //当炸弹库没有牌时返回
//    if (boomList.Count < 4)
//    {
//        ReClassify();
//        return;
//    }

//    //当对子库加上拆三连库得到的对子数量还是小于4时返回
//    if (pairList.Count + tripleList.Count / 3 * 2 < 4)
//    {
//        ReClassify();
//        return;
//    }

//    tempPlayCardList.Add(boomList[boomList.Count - 1]);
//    tempPlayCardList.Add(boomList[boomList.Count - 2]);
//    tempPlayCardList.Add(boomList[boomList.Count - 3]);
//    tempPlayCardList.Add(boomList[boomList.Count - 4]);

//    for (int i = 0; i < tempPlayCardList.Count; i++)
//    {
//        if (boomList.Contains(tempPlayCardList[i]) == true)
//        {
//            boomList.Remove(tempPlayCardList[i]);
//        }
//    }

//    if (pairList.Count >= 4)
//    {
//        for (int i = 0; i < 4; i++)
//        {
//            tempPlayCardList.Add(pairList[i]);
//        }

//        for (int i = 0; i < tempPlayCardList.Count; i++)
//        {
//            if (pairList.Contains(tempPlayCardList[i]) == true)
//            {
//                pairList.Remove(tempPlayCardList[i]);
//            }
//        }
//    }
//    else
//    {
//        for (int i = 0; i < tripleList.Count; i++)
//        {
//            if (pairList.Contains(tripleList[i]) == false)
//            {
//                pairList.Add(tripleList[i]);
//                pairList.Add(tripleList[i + 1]);

//                tripleList.Remove(tripleList[i + 1]);
//                tripleList.Remove(tripleList[i]);
//            }
//        }

//        for (int i = 0; i < 4; i++)
//        {
//            tempPlayCardList.Add(pairList[i]);
//        }

//        for (int i = 0; i < tempPlayCardList.Count; i++)
//        {
//            if (pairList.Contains(tempPlayCardList[i]) == true)
//            {
//                pairList.Remove(tempPlayCardList[i]);
//            }
//        }
//    }

//    OrderCardsList(tempPlayCardList);

//    JudgeCurrentCardCombination(tempPlayCardList);

//    ReClassify();

//    tempPlayCardList.Clear();
//}

////判断---四带单---至少6张
//private void Judge_BoomSingle()
//{
//    //与炸弹带对同理

//    List<GameObject> tempPlayCardList = new List<GameObject>();
//    tempPlayCardList.Clear();

//    //当炸弹库没有牌时返回
//    if (boomList.Count < 4)
//    {
//        ReClassify();
//        return;
//    }

//    //当对子库加上拆三连库得到的对子数量还是小于4时返回
//    if (pairList.Count + tripleList.Count / 3 * 2 < 4)
//    {
//        ReClassify();
//        return;
//    }

//    tempPlayCardList.Add(boomList[boomList.Count - 1]);
//    tempPlayCardList.Add(boomList[boomList.Count - 2]);
//    tempPlayCardList.Add(boomList[boomList.Count - 3]);
//    tempPlayCardList.Add(boomList[boomList.Count - 4]);

//    for (int i = 0; i < tempPlayCardList.Count; i++)
//    {
//        if (boomList.Contains(tempPlayCardList[i]) == true)
//        {
//            boomList.Remove(tempPlayCardList[i]);
//        }
//    }

//    if (singleList.Count + pairList.Count / 2 < 2)
//    {
//        ReClassify();
//        return;
//    }

//    if (singleList.Count >= 2)
//    {
//        for (int i = 0; i < 2; i++)
//        {
//            tempPlayCardList.Add(singleList[i]);
//        }

//        for (int i = 0; i < tempPlayCardList.Count; i++)
//        {
//            if (singleList.Contains(tempPlayCardList[i]) == true)
//            {
//                singleList.Remove(tempPlayCardList[i]);
//            }
//        }
//    }
//    else
//    {
//        for (int i = 0; i < pairList.Count; i++)
//        {
//            if (singleList.Contains(pairList[i]) == false)
//            {
//                singleList.Add(pairList[i]);

//                pairList.Remove(pairList[i]);
//            }
//        }

//        for (int i = 0; i < 2; i++)
//        {
//            tempPlayCardList.Add(singleList[i]);
//        }

//        for (int i = 0; i < tempPlayCardList.Count; i++)
//        {
//            if (singleList.Contains(tempPlayCardList[i]) == true)
//            {
//                singleList.Remove(tempPlayCardList[i]);
//            }
//        }
//    }

//    OrderCardsList(tempPlayCardList);

//    JudgeCurrentCardCombination(tempPlayCardList);

//    ReClassify();

//    tempPlayCardList.Clear();

//}

////判断---飞机不带---至少6张
//private void Judge_Plane_None()
//{
//    List<GameObject> tempPlayCardList = new List<GameObject>();
//    tempPlayCardList.Clear();

//    //获取飞机
//    JudgePlaneCounts();

//    if (temp_plane_counts.Count < 6)
//    {
//        ReClassify();
//        return;
//    }

//    for (int i = 0; i < temp_plane_counts.Count; i++)
//    {
//        tempPlayCardList.Add(temp_plane_counts[i]);
//    }

//    OrderCardsList(tempPlayCardList);

//    JudgeCurrentCardCombination(tempPlayCardList);

//    ReClassify();

//    tempPlayCardList.Clear();
//}

////判断---连对---至少6张
//private void Judge_Straight_Pair()
//{
//    List<GameObject> tempPlayCardList = new List<GameObject>();
//    tempPlayCardList.Clear();

//    //同理，如果对字库数量加上tripleList.Count / 3 * 2的数量小于连对的最小数量6，则返回
//    if (pairList.Count + tripleList.Count / 3 * 2 < 6)
//    {
//        ReClassify();
//        return;
//    }

//    //先拆三连库，再取对子
//    for (int i = 0; i < tripleList.Count; i++)
//    {
//        if (pairList.Contains(tripleList[i]) == false)
//        {
//            pairList.Add(tripleList[i]);
//            pairList.Add(tripleList[i + 1]);

//            tripleList.Remove(tripleList[i + 1]);
//            tripleList.Remove(tripleList[i]);
//        }
//    }

//    OrderCardsList(pairList);

//    for (int i = 0; i < PairTimes(pairList.Count); i += 2)
//    {
//        //当已有三对及以上的对子时，只需要判断最后两张对子，与之后要添加的两张对子是否成顺子关系即可
//        if (tempPlayCardList.Count >= 6)
//        {
//            if (tempPlayCardList[tempPlayCardList.Count - 2].GetComponent<CardInformations>().CardValue - pairList[i].GetComponent<CardInformations>().CardValue == -1 &&
//                tempPlayCardList[tempPlayCardList.Count - 1].GetComponent<CardInformations>().CardValue - pairList[i + 1].GetComponent<CardInformations>().CardValue == -1)
//            {
//                if (tempPlayCardList.Contains(pairList[i]) == false)
//                {
//                    tempPlayCardList.Add(pairList[i]);
//                }

//                if (tempPlayCardList.Contains(pairList[i + 1]) == false)
//                {
//                    tempPlayCardList.Add(pairList[i + 1]);
//                }
//            }
//        }

//        if (pairList[i].GetComponent<CardInformations>().CardValue - pairList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 1].GetComponent<CardInformations>().CardValue - pairList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 2].GetComponent<CardInformations>().CardValue - pairList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 3].GetComponent<CardInformations>().CardValue - pairList[i + 5].GetComponent<CardInformations>().CardValue == -1)
//        {
//            if (tempPlayCardList.Contains(pairList[i]) == false)
//            {
//                tempPlayCardList.Add(pairList[i]);
//            }

//            if (tempPlayCardList.Contains(pairList[i + 1]) == false)
//            {
//                tempPlayCardList.Add(pairList[i + 1]);
//            }

//            if (tempPlayCardList.Contains(pairList[i + 2]) == false)
//            {
//                tempPlayCardList.Add(pairList[i + 2]);
//            }

//            if (tempPlayCardList.Contains(pairList[i + 3]) == false)
//            {
//                tempPlayCardList.Add(pairList[i + 3]);
//            }

//            if (tempPlayCardList.Contains(pairList[i + 4]) == false)
//            {
//                tempPlayCardList.Add(pairList[i + 4]);
//            }

//            if (tempPlayCardList.Contains(pairList[i + 5]) == false)
//            {
//                tempPlayCardList.Add(pairList[i + 5]);
//            }
//        }
//    }

//    for (int i = 0; i < tempPlayCardList.Count; i++)
//    {
//        if (pairList.Contains(tempPlayCardList[i]) == true)
//        {
//            pairList.Remove(tempPlayCardList[i]);
//        }
//    }

//    OrderCardsList(tempPlayCardList);

//    JudgeCurrentCardCombination(tempPlayCardList);

//    ReClassify();

//    OrderCardsList(tempPlayCardList);
//}
//private int PairTimes(int _currentPairCounts)
//{
//    switch (_currentPairCounts)
//    {
//        case 6:
//            return 2;
//        case 8:
//            return 4;
//        case 10:
//            return 6;
//        case 12:
//            return 8;
//        case 14:
//            return 10;
//        case 16:
//            return 12;
//        case 18:
//            return 14;
//        case 20:
//            return 16;
//    }

//    return 0;
//}

////判断---顺子---至少5张
//private void Judge_Straight()
//{
//    List<GameObject> tempPlayCardList = new List<GameObject>();
//    tempPlayCardList.Clear();

//    //同理，如果单牌的数量加上对子拆掉过后的数量，仍小于顺子的最小数量5，则返回
//    if (singleList.Count + pairList.Count / 2 < 5)
//    {
//        ReClassify();
//        return;
//    }

//    for (int i = 0; i < pairList.Count; i++)
//    {
//        if (singleList.Contains(pairList[i]) == false)
//        {
//            singleList.Add(pairList[i]);

//            pairList.Remove(pairList[i]);
//        }
//    }

//    OrderCardsList(singleList);

//    for (int i = 0; i < (singleList.Count - 5) + 1; i++)
//    {
//        if (tempPlayCardList.Count >= 5)
//        {
//            if (tempPlayCardList[tempPlayCardList.Count - 1].GetComponent<CardInformations>().CardValue - singleList[i].GetComponent<CardInformations>().CardValue == -1)
//            {
//                if (tempPlayCardList.Contains(singleList[i]) == false)
//                {
//                    tempPlayCardList.Add(singleList[i]);
//                }
//            }
//        }

//        if (singleList[i].GetComponent<CardInformations>().CardValue - singleList[i + 1].GetComponent<CardInformations>().CardValue == -1 &&
//            singleList[i + 1].GetComponent<CardInformations>().CardValue - singleList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
//            singleList[i + 2].GetComponent<CardInformations>().CardValue - singleList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
//            singleList[i + 3].GetComponent<CardInformations>().CardValue - singleList[i + 4].GetComponent<CardInformations>().CardValue == -1)
//        {
//            if (tempPlayCardList.Contains(singleList[i]) == false)
//            {
//                tempPlayCardList.Add(singleList[i]);
//            }

//            if (tempPlayCardList.Contains(singleList[i + 1]) == false)
//            {
//                tempPlayCardList.Add(singleList[i + 1]);
//            }

//            if (tempPlayCardList.Contains(singleList[i + 2]) == false)
//            {
//                tempPlayCardList.Add(singleList[i + 2]);
//            }

//            if (tempPlayCardList.Contains(singleList[i + 3]) == false)
//            {
//                tempPlayCardList.Add(singleList[i + 3]);
//            }

//            if (tempPlayCardList.Contains(singleList[i + 4]) == false)
//            {
//                tempPlayCardList.Add(singleList[i + 4]);
//            }
//        }
//    }

//    for (int i = 0; i < tempPlayCardList.Count; i++)
//    {
//        if (singleList.Contains(tempPlayCardList[i]) == true)
//        {
//            singleList.Remove(tempPlayCardList[i]);
//        }
//    }

//    OrderCardsList(tempPlayCardList);

//    JudgeCurrentCardCombination(tempPlayCardList);

//    ReClassify();

//    tempPlayCardList.Clear();
//}

////判断---三带一对---至少5张
//private void Judge_Triple_Pair()
//{
//    List<GameObject> tempPlayCardList = new List<GameObject>();
//    tempPlayCardList.Clear();

//    if (tripleList.Count == 0)
//    {
//        ReClassify();
//        return;
//    }

//    //先走最大的
//    for (int i = 0; i < 3; i++)
//    {
//        tempPlayCardList.Add(tripleList[tripleList.Count - (i + 1)]);
//    }

//    for (int i = 0; i < tempPlayCardList.Count; i++)
//    {
//        if (tripleList.Contains(tempPlayCardList[i]) == true)
//        {
//            tripleList.Remove(tempPlayCardList[i]);
//        }
//    }

//    if (pairList.Count + tripleList.Count / 3 * 2 < 2)
//    {
//        ReClassify();
//        return;
//    }

//    if (pairList.Count >= 2)
//    {
//        tempPlayCardList.Add(pairList[0]);
//        tempPlayCardList.Add(pairList[1]);

//        pairList.Remove(pairList[1]);
//        pairList.Remove(pairList[0]);
//    }
//    else
//    {
//        for (int i = 0; i < tripleList.Count; i++)
//        {
//            if (pairList.Contains(tripleList[i]) == false)
//            {
//                pairList.Add(tripleList[i]);
//                pairList.Add(tripleList[i + 1]);

//                tripleList.Remove(tripleList[i + 1]);
//                tripleList.Remove(tripleList[i]);
//            }
//        }

//        for (int i = 0; i < 2; i++)
//        {
//            tempPlayCardList.Add(pairList[i]);
//        }

//        for (int i = 0; i < tempPlayCardList.Count; i++)
//        {
//            if (pairList.Contains(tempPlayCardList[i]) == true)
//            {
//                pairList.Remove(tempPlayCardList[i]);
//            }
//        }
//    }

//    OrderCardsList(tempPlayCardList);

//    JudgeCurrentCardCombination(tempPlayCardList);

//    ReClassify();

//    tempPlayCardList.Clear();
//}

////判断---三带一---至少4张
//private void Judge_Triple_Single()
//{
//    List<GameObject> tempPlayCardList = new List<GameObject>();
//    tempPlayCardList.Clear();

//    if (tripleList.Count == 0)
//    {
//        ReClassify();
//        return;
//    }

//    //先走最大的
//    for (int i = 0; i < 3; i++)
//    {
//        tempPlayCardList.Add(tripleList[tripleList.Count - (i + 1)]);
//    }

//    for (int i = 0; i < tempPlayCardList.Count; i++)
//    {
//        if (tripleList.Contains(tempPlayCardList[i]) == true)
//        {
//            tripleList.Remove(tempPlayCardList[i]);
//        }
//    }

//    if (singleList.Count + pairList.Count / 2 < 1)
//    {
//        ReClassify();
//        return;
//    }

//    if (singleList.Count >= 1)
//    {
//        tempPlayCardList.Add(singleList[0]);

//        singleList.Remove(singleList[0]);
//    }
//    else
//    {
//        for (int i = 0; i < pairList.Count; i++)
//        {
//            if (singleList.Contains(pairList[i]) == false)
//            {
//                singleList.Add(pairList[i]);

//                pairList.Remove(pairList[i]);
//            }
//        }

//        tempPlayCardList.Add(singleList[0]);

//        singleList.Remove(singleList[0]);
//    }

//    OrderCardsList(tempPlayCardList);

//    JudgeCurrentCardCombination(tempPlayCardList);

//    ReClassify();

//    tempPlayCardList.Clear();
//}

////判断---炸弹---至少4张
//private void Judge_Boom()
//{
//    List<GameObject> tempPlayCardList = new List<GameObject>();
//    tempPlayCardList.Clear();

//    if (boomList.Count == 0)
//    {
//        ReClassify();
//        return;
//    }

//    for (int i = 0; i < 4; i++)
//    {
//        tempPlayCardList.Add(boomList[i]);
//    }

//    for (int i = 0; i < tempPlayCardList.Count; i++)
//    {
//        if (boomList.Contains(tempPlayCardList[i]) == true)
//        {
//            boomList.Remove(tempPlayCardList[i]);
//        }
//    }

//    OrderCardsList(tempPlayCardList);

//    JudgeCurrentCardCombination(tempPlayCardList);

//    ReClassify();

//    tempPlayCardList.Clear();
//}

////判断---三连---至少3张
//private void Judge_Triple()
//{
//    List<GameObject> tempPlayCardList = new List<GameObject>();
//    tempPlayCardList.Clear();

//    if (tripleList.Count == 0)
//    {
//        ReClassify();
//        return;
//    }

//    for (int i = 0; i < 3; i++)
//    {
//        tempPlayCardList.Add(tripleList[tripleList.Count - (i + 1)]);
//    }

//    for (int i = 0; i < tempPlayCardList.Count; i++)
//    {
//        if (tripleList.Contains(tempPlayCardList[i]) == true)
//        {
//            tripleList.Remove(tempPlayCardList[i]);
//        }
//    }

//    OrderCardsList(tempPlayCardList);

//    JudgeCurrentCardCombination(tempPlayCardList);

//    ReClassify();

//    tempPlayCardList.Clear();
//}

////判断---对子---至少2张
//private void Judge_Pair()
//{
//    List<GameObject> tempPlayCardList = new List<GameObject>();
//    tempPlayCardList.Clear();

//    if (pairList.Count + tripleList.Count / 3 * 2 < 2)
//    {
//        ReClassify();
//        return;
//    }

//    if (pairList.Count >= 2)
//    {
//        tempPlayCardList.Add(pairList[0]);
//        tempPlayCardList.Add(pairList[1]);

//        pairList.Remove(pairList[1]);
//        pairList.Remove(pairList[0]);
//    }
//    else
//    {
//        for (int i = 0; i < tripleList.Count; i++)
//        {
//            if (pairList.Contains(tripleList[i]) == false)
//            {
//                pairList.Add(tripleList[i]);
//                pairList.Add(tripleList[i + 1]);

//                tripleList.Remove(tripleList[i + 1]);
//                tripleList.Remove(tripleList[i]);
//            }
//        }

//        tempPlayCardList.Add(pairList[0]);
//        tempPlayCardList.Add(pairList[1]);

//        pairList.Remove(pairList[1]);
//        pairList.Remove(pairList[0]);
//    }

//    OrderCardsList(tempPlayCardList);

//    JudgeCurrentCardCombination(tempPlayCardList);

//    ReClassify();

//    tempPlayCardList.Clear();
//}

////判断---王炸---至少2张
//private void Judge_JokerBoom()
//{
//    List<GameObject> tempPlayCardList = new List<GameObject>();
//    tempPlayCardList.Clear();

//    if (jokerBoomList.Count == 0)
//    {
//        ReClassify();
//        return;
//    }

//    tempPlayCardList.Add(jokerBoomList[0]);
//    tempPlayCardList.Add(jokerBoomList[1]);

//    jokerBoomList.Remove(jokerBoomList[1]);
//    jokerBoomList.Remove(jokerBoomList[0]);

//    OrderCardsList(tempPlayCardList);

//    JudgeCurrentCardCombination(tempPlayCardList);

//    ReClassify();

//    tempPlayCardList.Clear();
//}

////判断---单牌---至少1张
//private void Judge_Single()
//{
//    List<GameObject> tempPlayCardList = new List<GameObject>();
//    tempPlayCardList.Clear();

//    if (singleList.Count + pairList.Count / 2 < 1)
//    {
//        ReClassify();
//        return;
//    }

//    if (singleList.Count >= 1)
//    {
//        tempPlayCardList.Add(singleList[0]);

//        singleList.Remove(singleList[0]);
//    }
//    else
//    {
//        for (int i = 0; i < pairList.Count; i++)
//        {
//            if (singleList.Contains(pairList[i]) == false)
//            {
//                singleList.Add(pairList[i]);

//                pairList.Remove(pairList[i]);
//            }
//        }

//        tempPlayCardList.Add(singleList[0]);

//        singleList.Remove(singleList[0]);
//    }

//    OrderCardsList(tempPlayCardList);

//    JudgeCurrentCardCombination(tempPlayCardList);

//    ReClassify();

//    tempPlayCardList.Clear();
//}

////判断当前谁的牌多，若牌数相同，则比谁的点数大
//private void JudgeCurrentCardCombination(List<GameObject> _currentCardCombination)
//{
//    if (_currentCardCombination.Count < temp_AI_First_Hand.Count)
//    {
//        ReClassify();

//        return;
//    }

//    //优先出牌数量多的
//    if (_currentCardCombination.Count > temp_AI_First_Hand.Count)
//    {
//        temp_AI_First_Hand.Clear();

//        OrderCardsList(_currentCardCombination);

//        for (int i = 0; i < _currentCardCombination.Count; i++)
//        {
//            temp_AI_First_Hand.Add(_currentCardCombination[i]);
//        }

//        ReClassify();

//        return;
//    }

//    //如果牌数量一样，则优先出点数大的
//    if (_currentCardCombination.Count == temp_AI_First_Hand.Count)
//    {
//        int currentPoints = 0;
//        int tempAiPoints = 0;

//        for (int i = 0; i < _currentCardCombination.Count; i++)
//        {
//            currentPoints += _currentCardCombination[i].GetComponent<CardInformations>().CardValue;
//        }

//        for (int i = 0; i < temp_AI_First_Hand.Count; i++)
//        {
//            tempAiPoints += temp_AI_First_Hand[i].GetComponent<CardInformations>().CardValue;
//        }

//        if (currentPoints > tempAiPoints)
//        {
//            temp_AI_First_Hand.Clear();

//            OrderCardsList(_currentCardCombination);

//            for (int i = 0; i < _currentCardCombination.Count; i++)
//            {
//                temp_AI_First_Hand.Add(_currentCardCombination[i]);
//            }

//            ReClassify();

//            return;
//        }

//        //若点数也一样，则出当前的牌组合
//        if (currentPoints == tempAiPoints)
//        {
//            ReClassify();
//            return;
//        }
//    }
//}

////获取临时飞机列表
//private void JudgePlaneCounts()
//{
//    temp_plane_counts.Clear();

//    if (tripleList.Count < 6)
//    {
//        return;
//    }

//    for (int i = 0; i < tripleList.Count - 3; i += 3)
//    {
//        if (tripleList[i].GetComponent<CardInformations>().CardValue - tripleList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
//            tripleList[i + 1].GetComponent<CardInformations>().CardValue - tripleList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
//            tripleList[i + 2].GetComponent<CardInformations>().CardValue - tripleList[i + 5].GetComponent<CardInformations>().CardValue == -1)
//        {
//            if (temp_plane_counts.Contains(tripleList[i]) == false)
//            {
//                temp_plane_counts.Add(tripleList[i]);
//            }

//            if (temp_plane_counts.Contains(tripleList[i + 1]) == false)
//            {
//                temp_plane_counts.Add(tripleList[i + 1]);
//            }

//            if (temp_plane_counts.Contains(tripleList[i + 2]) == false)
//            {
//                temp_plane_counts.Add(tripleList[i + 2]);
//            }

//            if (temp_plane_counts.Contains(tripleList[i + 3]) == false)
//            {
//                temp_plane_counts.Add(tripleList[i + 3]);
//            }

//            if (temp_plane_counts.Contains(tripleList[i + 4]) == false)
//            {
//                temp_plane_counts.Add(tripleList[i + 4]);
//            }

//            if (temp_plane_counts.Contains(tripleList[i + 5]) == false)
//            {
//                temp_plane_counts.Add(tripleList[i + 5]);
//            }
//        }
//    }

//    for (int i = 0; i < temp_plane_counts.Count; i++)
//    {
//        if (tripleList.Contains(temp_plane_counts[i]) == true)
//        {
//            tripleList.Remove(temp_plane_counts[i]);
//        }
//    }
//}

////当组合牌不满足时，退回飞机
//private void SendBackPlane()
//{
//    //退回最小的
//    tripleList.Add(temp_plane_counts[0]);
//    tripleList.Add(temp_plane_counts[1]);
//    tripleList.Add(temp_plane_counts[2]);

//    temp_plane_counts.Remove(temp_plane_counts[2]);
//    temp_plane_counts.Remove(temp_plane_counts[1]);
//    temp_plane_counts.Remove(temp_plane_counts[0]);

//    OrderCardsList(tripleList);
//    OrderCardsList(temp_plane_counts);
//}

//#endregion

#endregion

#region No Use Test 6
//#region AI后手出牌算法判断

////1，若当前回合是---单牌
//public List<GameObject> CurrentTurn_Single(int _currentTurnCardPoints)
//{
//    List<GameObject> tempPlayCardList = new List<GameObject>();

//    tempPlayCardList.Clear();

//    //当单牌库有牌时
//    if (singleList.Count >= 1)
//    {
//        //循环判断是否有满足条件的牌
//        for (int i = 0; i < singleList.Count; i++)
//        {
//            if (singleList[i].GetComponent<CardInformations>().CardValue > _currentTurnCardPoints)
//            {
//                //若有则添加到临时出牌列表
//                tempPlayCardList.Add(singleList[i]);

//                //再从单牌库中将其移除
//                singleList.Remove(singleList[i]);

//                OrderCardsList(singleList);

//                OrderCardsList(tempPlayCardList);

//                return tempPlayCardList;
//            }
//        }
//    }

//    //当单牌库没牌时
//    //判断对子库有牌时
//    if (pairList.Count >= 1)
//    {
//        //同理
//        for (int i = 0; i < pairList.Count; i++)
//        {
//            if (pairList[i].GetComponent<CardInformations>().CardValue > _currentTurnCardPoints)
//            {
//                //同理，若有满足条件的牌，则将其添加到临时出牌列表   
//                tempPlayCardList.Add(pairList[i]);

//                //然后将另外一张多出来的单牌放到单牌库
//                singleList.Add(pairList[i + 1]);

//                //最后将它们从对字库中移除
//                pairList.Remove(pairList[i + 1]);
//                pairList.Remove(pairList[i]);

//                OrderCardsList(singleList);
//                OrderCardsList(pairList);

//                OrderCardsList(tempPlayCardList);

//                return tempPlayCardList;
//            }
//        }
//    }

//    //当单牌、对字库均没有符合条件的牌时
//    //判断三连库
//    if (tripleList.Count >= 1)
//    {
//        //同理
//        for (int i = 0; i < tripleList.Count; i++)
//        {
//            if (tripleList[i].GetComponent<CardInformations>().CardValue > _currentTurnCardPoints)
//            {
//                tempPlayCardList.Add(tripleList[i]);

//                pairList.Add(tripleList[i + 1]);
//                pairList.Add(tripleList[i + 2]);

//                tripleList.Remove(tripleList[i + 2]);
//                tripleList.Remove(tripleList[i + 1]);
//                tripleList.Remove(tripleList[i]);

//                OrderCardsList(singleList);
//                OrderCardsList(pairList);
//                OrderCardsList(tripleList);

//                OrderCardsList(tempPlayCardList);

//                return tempPlayCardList;
//            }
//        }
//    }

//    //判断炸弹。。。。。。

//    return null;
//}


////2，若当前回合是---对子
//public List<GameObject> CurrentTurn_Pair(int _currentTurnCardPoints)
//{
//    List<GameObject> tempPlayCardList = new List<GameObject>();

//    tempPlayCardList.Clear();

//    //判断对子库是否存在符合条件的牌
//    if (pairList.Count >= 2)
//    {
//        for (int i = 0; i < pairList.Count - 1; i++)
//        {
//            if (pairList[i].GetComponent<CardInformations>().CardValue == pairList[i + 1].GetComponent<CardInformations>().CardValue)
//            {
//                if (pairList[i].GetComponent<CardInformations>().CardValue + pairList[i + 1].GetComponent<CardInformations>().CardValue > _currentTurnCardPoints)
//                {
//                    tempPlayCardList.Add(pairList[i + 1]);
//                    tempPlayCardList.Add(pairList[i]);

//                    pairList.Remove(pairList[i + 1]);
//                    pairList.Remove(pairList[i]);

//                    OrderCardsList(pairList);

//                    OrderCardsList(tempPlayCardList);

//                    return tempPlayCardList;
//                }
//            }
//        }
//    }

//    //如果对字库没有牌，则判断三连库
//    if (tripleList.Count >= 2)
//    {
//        for (int i = 0; i < tripleList.Count - 1; i++)
//        {
//            if (tripleList[i].GetComponent<CardInformations>().CardValue == tripleList[i + 1].GetComponent<CardInformations>().CardValue)
//            {
//                if (tripleList[i].GetComponent<CardInformations>().CardValue +
//                    tripleList[i + 1].GetComponent<CardInformations>().CardValue > _currentTurnCardPoints)
//                {
//                    tempPlayCardList.Add(tripleList[i]);
//                    tempPlayCardList.Add(tripleList[i + 1]);

//                    singleList.Add(tripleList[i + 2]);

//                    tripleList.Remove(tripleList[i + 2]);
//                    tripleList.Remove(tripleList[i + 1]);
//                    tripleList.Remove(tripleList[i]);

//                    OrderCardsList(singleList);
//                    OrderCardsList(tripleList);

//                    OrderCardsList(tempPlayCardList);

//                    return tempPlayCardList;
//                }
//            }
//        }
//    }

//    return null;
//}


////3，若当前回合是---顺子
//#region CurrentTurn_Straight
///// <summary>
///// 判断原理：
/////       先判断组合的牌是否牌面点数相差1，也就是判断他们是否满足顺子条件
/////       若满足顺子条件，在判断他们的点数是否大于当前回合的点数
///// </summary>
///// <param name="_currentTurnCardPoints"></param>
///// <param name="_currentTurnCardCounts"></param>
///// <returns></returns>
//public List<GameObject> CurrentTurn_Straight(int _currentTurnCardPoints, int _currentTurnCardCounts)
//{
//    List<GameObject> tempPlayCardList = new List<GameObject>();

//    tempPlayCardList.Clear();

//    ///
//    /// 仔细思考了一下，似乎不应该有这个判断
//    /// 
//    ////当单牌库数量大于等于当前回合牌数量
//    //if (singleList.Count >= _currentTurnCardCounts)
//    //{
//    //    return JudgeStraight(_currentTurnCardPoints, _currentTurnCardCounts);
//    //}

//    //当单牌库没有满足条件的牌时，从对子库里拆
//    for (int i = 0; i < pairList.Count; i++)
//    {
//        if (singleList.Contains(pairList[i]) == false)
//        {
//            singleList.Add(pairList[i]);
//        }
//    }

//    for (int i = 0; i < singleList.Count; i++)
//    {
//        if (pairList.Contains(singleList[i]) == true)
//        {
//            pairList.Remove(singleList[i]);
//        }
//    }

//    OrderCardsList(singleList);

//    //如果单牌库数量不满足当前回合出牌数量
//    if (singleList.Count < _currentTurnCardCounts)
//    {
//        RecoverForStraight();

//        return null;
//    }

//    //在判断此时单牌库中是否有满足条件的组合

//    //1，当有5张顺子时
//    if (_currentTurnCardCounts == 5)
//    {
//        for (int i = 0; i < singleList.Count - (_currentTurnCardCounts - 1); i++)
//        {
//            if (singleList[i].GetComponent<CardInformations>().CardValue - singleList[i + 1].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 1].GetComponent<CardInformations>().CardValue - singleList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 2].GetComponent<CardInformations>().CardValue - singleList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 3].GetComponent<CardInformations>().CardValue - singleList[i + 4].GetComponent<CardInformations>().CardValue == -1)
//            {
//                if (singleList[i].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 1].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 2].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 3].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 4].GetComponent<CardInformations>().CardValue > _currentTurnCardPoints)
//                {
//                    tempPlayCardList.Add(singleList[i]);
//                    tempPlayCardList.Add(singleList[i + 1]);
//                    tempPlayCardList.Add(singleList[i + 2]);
//                    tempPlayCardList.Add(singleList[i + 3]);
//                    tempPlayCardList.Add(singleList[i + 4]);

//                    singleList.Remove(singleList[i + 4]);
//                    singleList.Remove(singleList[i + 3]);
//                    singleList.Remove(singleList[i + 2]);
//                    singleList.Remove(singleList[i + 1]);
//                    singleList.Remove(singleList[i]);

//                    RecoverForStraight();

//                    OrderCardsList(tempPlayCardList);

//                    return tempPlayCardList;
//                }
//            }
//        }
//    }

//    //2，当有6张顺子时
//    if (_currentTurnCardCounts == 6)
//    {
//        for (int i = 0; i < singleList.Count - (_currentTurnCardCounts - 1); i++)
//        {
//            if (singleList[i].GetComponent<CardInformations>().CardValue - singleList[i + 1].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 1].GetComponent<CardInformations>().CardValue - singleList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 2].GetComponent<CardInformations>().CardValue - singleList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 3].GetComponent<CardInformations>().CardValue - singleList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 4].GetComponent<CardInformations>().CardValue - singleList[i + 5].GetComponent<CardInformations>().CardValue == -1)
//            {
//                if (singleList[i].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 1].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 2].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 3].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 4].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 5].GetComponent<CardInformations>().CardValue > _currentTurnCardPoints)
//                {
//                    tempPlayCardList.Add(singleList[i]);
//                    tempPlayCardList.Add(singleList[i + 1]);
//                    tempPlayCardList.Add(singleList[i + 2]);
//                    tempPlayCardList.Add(singleList[i + 3]);
//                    tempPlayCardList.Add(singleList[i + 4]);
//                    tempPlayCardList.Add(singleList[i + 5]);

//                    singleList.Remove(singleList[i + 5]);
//                    singleList.Remove(singleList[i + 4]);
//                    singleList.Remove(singleList[i + 3]);
//                    singleList.Remove(singleList[i + 2]);
//                    singleList.Remove(singleList[i + 1]);
//                    singleList.Remove(singleList[i]);

//                    RecoverForStraight();

//                    OrderCardsList(tempPlayCardList);

//                    return tempPlayCardList;
//                }
//            }
//        }
//    }

//    //3，当有7张顺子时
//    if (_currentTurnCardCounts == 7)
//    {
//        for (int i = 0; i < singleList.Count - (_currentTurnCardCounts - 1); i++)
//        {
//            if (singleList[i].GetComponent<CardInformations>().CardValue - singleList[i + 1].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 1].GetComponent<CardInformations>().CardValue - singleList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 2].GetComponent<CardInformations>().CardValue - singleList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 3].GetComponent<CardInformations>().CardValue - singleList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 4].GetComponent<CardInformations>().CardValue - singleList[i + 5].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 5].GetComponent<CardInformations>().CardValue - singleList[i + 6].GetComponent<CardInformations>().CardValue == -1)
//            {
//                if (singleList[i].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 1].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 2].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 3].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 4].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 5].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 6].GetComponent<CardInformations>().CardValue > _currentTurnCardPoints)
//                {
//                    tempPlayCardList.Add(singleList[i]);
//                    tempPlayCardList.Add(singleList[i + 1]);
//                    tempPlayCardList.Add(singleList[i + 2]);
//                    tempPlayCardList.Add(singleList[i + 3]);
//                    tempPlayCardList.Add(singleList[i + 4]);
//                    tempPlayCardList.Add(singleList[i + 5]);
//                    tempPlayCardList.Add(singleList[i + 6]);

//                    singleList.Remove(singleList[i + 6]);
//                    singleList.Remove(singleList[i + 5]);
//                    singleList.Remove(singleList[i + 4]);
//                    singleList.Remove(singleList[i + 3]);
//                    singleList.Remove(singleList[i + 2]);
//                    singleList.Remove(singleList[i + 1]);
//                    singleList.Remove(singleList[i]);

//                    RecoverForStraight();

//                    OrderCardsList(tempPlayCardList);

//                    return tempPlayCardList;
//                }
//            }
//        }
//    }

//    //4，当有8张顺子时
//    if (_currentTurnCardCounts == 8)
//    {
//        for (int i = 0; i < singleList.Count - (_currentTurnCardCounts - 1); i++)
//        {
//            if (singleList[i].GetComponent<CardInformations>().CardValue - singleList[i + 1].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 1].GetComponent<CardInformations>().CardValue - singleList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 2].GetComponent<CardInformations>().CardValue - singleList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 3].GetComponent<CardInformations>().CardValue - singleList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 4].GetComponent<CardInformations>().CardValue - singleList[i + 5].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 5].GetComponent<CardInformations>().CardValue - singleList[i + 6].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 6].GetComponent<CardInformations>().CardValue - singleList[i + 7].GetComponent<CardInformations>().CardValue == -1)
//            {
//                if (singleList[i].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 1].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 2].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 3].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 4].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 5].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 6].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 7].GetComponent<CardInformations>().CardValue > _currentTurnCardPoints)
//                {
//                    tempPlayCardList.Add(singleList[i]);
//                    tempPlayCardList.Add(singleList[i + 1]);
//                    tempPlayCardList.Add(singleList[i + 2]);
//                    tempPlayCardList.Add(singleList[i + 3]);
//                    tempPlayCardList.Add(singleList[i + 4]);
//                    tempPlayCardList.Add(singleList[i + 5]);
//                    tempPlayCardList.Add(singleList[i + 6]);
//                    tempPlayCardList.Add(singleList[i + 7]);

//                    singleList.Remove(singleList[i + 7]);
//                    singleList.Remove(singleList[i + 6]);
//                    singleList.Remove(singleList[i + 5]);
//                    singleList.Remove(singleList[i + 4]);
//                    singleList.Remove(singleList[i + 3]);
//                    singleList.Remove(singleList[i + 2]);
//                    singleList.Remove(singleList[i + 1]);
//                    singleList.Remove(singleList[i]);

//                    RecoverForStraight();

//                    OrderCardsList(tempPlayCardList);

//                    return tempPlayCardList;
//                }
//            }
//        }
//    }

//    //5，当有9张顺子时
//    if (_currentTurnCardCounts == 9)
//    {
//        for (int i = 0; i < singleList.Count - (_currentTurnCardCounts - 1); i++)
//        {
//            if (singleList[i].GetComponent<CardInformations>().CardValue - singleList[i + 1].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 1].GetComponent<CardInformations>().CardValue - singleList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 2].GetComponent<CardInformations>().CardValue - singleList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 3].GetComponent<CardInformations>().CardValue - singleList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 4].GetComponent<CardInformations>().CardValue - singleList[i + 5].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 5].GetComponent<CardInformations>().CardValue - singleList[i + 6].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 6].GetComponent<CardInformations>().CardValue - singleList[i + 7].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 7].GetComponent<CardInformations>().CardValue - singleList[i + 8].GetComponent<CardInformations>().CardValue == -1)
//            {
//                if (singleList[i].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 1].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 2].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 3].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 4].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 5].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 6].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 7].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 8].GetComponent<CardInformations>().CardValue > _currentTurnCardPoints)
//                {
//                    tempPlayCardList.Add(singleList[i]);
//                    tempPlayCardList.Add(singleList[i + 1]);
//                    tempPlayCardList.Add(singleList[i + 2]);
//                    tempPlayCardList.Add(singleList[i + 3]);
//                    tempPlayCardList.Add(singleList[i + 4]);
//                    tempPlayCardList.Add(singleList[i + 5]);
//                    tempPlayCardList.Add(singleList[i + 6]);
//                    tempPlayCardList.Add(singleList[i + 7]);
//                    tempPlayCardList.Add(singleList[i + 8]);

//                    singleList.Remove(singleList[i + 8]);
//                    singleList.Remove(singleList[i + 7]);
//                    singleList.Remove(singleList[i + 6]);
//                    singleList.Remove(singleList[i + 5]);
//                    singleList.Remove(singleList[i + 4]);
//                    singleList.Remove(singleList[i + 3]);
//                    singleList.Remove(singleList[i + 2]);
//                    singleList.Remove(singleList[i + 1]);
//                    singleList.Remove(singleList[i]);

//                    RecoverForStraight();

//                    OrderCardsList(tempPlayCardList);

//                    return tempPlayCardList;
//                }
//            }
//        }
//    }

//    //6，当有10张顺子时
//    if (_currentTurnCardCounts == 10)
//    {
//        for (int i = 0; i < singleList.Count - (_currentTurnCardCounts - 1); i++)
//        {
//            if (singleList[i].GetComponent<CardInformations>().CardValue - singleList[i + 1].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 1].GetComponent<CardInformations>().CardValue - singleList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 2].GetComponent<CardInformations>().CardValue - singleList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 3].GetComponent<CardInformations>().CardValue - singleList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 4].GetComponent<CardInformations>().CardValue - singleList[i + 5].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 5].GetComponent<CardInformations>().CardValue - singleList[i + 6].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 6].GetComponent<CardInformations>().CardValue - singleList[i + 7].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 7].GetComponent<CardInformations>().CardValue - singleList[i + 8].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 8].GetComponent<CardInformations>().CardValue - singleList[i + 9].GetComponent<CardInformations>().CardValue == -1)
//            {
//                if (singleList[i].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 1].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 2].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 3].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 4].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 5].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 6].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 7].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 8].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 9].GetComponent<CardInformations>().CardValue > _currentTurnCardPoints)
//                {
//                    tempPlayCardList.Add(singleList[i]);
//                    tempPlayCardList.Add(singleList[i + 1]);
//                    tempPlayCardList.Add(singleList[i + 2]);
//                    tempPlayCardList.Add(singleList[i + 3]);
//                    tempPlayCardList.Add(singleList[i + 4]);
//                    tempPlayCardList.Add(singleList[i + 5]);
//                    tempPlayCardList.Add(singleList[i + 6]);
//                    tempPlayCardList.Add(singleList[i + 7]);
//                    tempPlayCardList.Add(singleList[i + 8]);
//                    tempPlayCardList.Add(singleList[i + 9]);

//                    singleList.Remove(singleList[i + 9]);
//                    singleList.Remove(singleList[i + 8]);
//                    singleList.Remove(singleList[i + 7]);
//                    singleList.Remove(singleList[i + 6]);
//                    singleList.Remove(singleList[i + 5]);
//                    singleList.Remove(singleList[i + 4]);
//                    singleList.Remove(singleList[i + 3]);
//                    singleList.Remove(singleList[i + 2]);
//                    singleList.Remove(singleList[i + 1]);
//                    singleList.Remove(singleList[i]);

//                    RecoverForStraight();

//                    OrderCardsList(tempPlayCardList);

//                    return tempPlayCardList;
//                }
//            }
//        }

//    }

//    //7，当有11张顺子时
//    if (_currentTurnCardCounts == 11)
//    {
//        for (int i = 0; i < singleList.Count - (_currentTurnCardCounts - 1); i++)
//        {
//            if (singleList[i].GetComponent<CardInformations>().CardValue - singleList[i + 1].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 1].GetComponent<CardInformations>().CardValue - singleList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 2].GetComponent<CardInformations>().CardValue - singleList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 3].GetComponent<CardInformations>().CardValue - singleList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 4].GetComponent<CardInformations>().CardValue - singleList[i + 5].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 5].GetComponent<CardInformations>().CardValue - singleList[i + 6].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 6].GetComponent<CardInformations>().CardValue - singleList[i + 7].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 7].GetComponent<CardInformations>().CardValue - singleList[i + 8].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 8].GetComponent<CardInformations>().CardValue - singleList[i + 9].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 9].GetComponent<CardInformations>().CardValue - singleList[i + 10].GetComponent<CardInformations>().CardValue == -1)
//            {
//                if (singleList[i].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 1].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 2].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 3].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 4].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 5].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 6].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 7].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 8].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 9].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 10].GetComponent<CardInformations>().CardValue > _currentTurnCardPoints)
//                {
//                    tempPlayCardList.Add(singleList[i]);
//                    tempPlayCardList.Add(singleList[i + 1]);
//                    tempPlayCardList.Add(singleList[i + 2]);
//                    tempPlayCardList.Add(singleList[i + 3]);
//                    tempPlayCardList.Add(singleList[i + 4]);
//                    tempPlayCardList.Add(singleList[i + 5]);
//                    tempPlayCardList.Add(singleList[i + 6]);
//                    tempPlayCardList.Add(singleList[i + 7]);
//                    tempPlayCardList.Add(singleList[i + 8]);
//                    tempPlayCardList.Add(singleList[i + 9]);
//                    tempPlayCardList.Add(singleList[i + 10]);

//                    singleList.Remove(singleList[i + 10]);
//                    singleList.Remove(singleList[i + 9]);
//                    singleList.Remove(singleList[i + 8]);
//                    singleList.Remove(singleList[i + 7]);
//                    singleList.Remove(singleList[i + 6]);
//                    singleList.Remove(singleList[i + 5]);
//                    singleList.Remove(singleList[i + 4]);
//                    singleList.Remove(singleList[i + 3]);
//                    singleList.Remove(singleList[i + 2]);
//                    singleList.Remove(singleList[i + 1]);
//                    singleList.Remove(singleList[i]);

//                    RecoverForStraight();

//                    OrderCardsList(tempPlayCardList);

//                    return tempPlayCardList;
//                }
//            }
//        }
//    }

//    //8，当有12张顺子时
//    if (_currentTurnCardCounts == 12)
//    {
//        for (int i = 0; i < singleList.Count - (_currentTurnCardCounts - 1); i++)
//        {
//            if (singleList[i].GetComponent<CardInformations>().CardValue - singleList[i + 1].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 1].GetComponent<CardInformations>().CardValue - singleList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 2].GetComponent<CardInformations>().CardValue - singleList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 3].GetComponent<CardInformations>().CardValue - singleList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 4].GetComponent<CardInformations>().CardValue - singleList[i + 5].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 5].GetComponent<CardInformations>().CardValue - singleList[i + 6].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 6].GetComponent<CardInformations>().CardValue - singleList[i + 7].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 7].GetComponent<CardInformations>().CardValue - singleList[i + 8].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 8].GetComponent<CardInformations>().CardValue - singleList[i + 9].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 9].GetComponent<CardInformations>().CardValue - singleList[i + 10].GetComponent<CardInformations>().CardValue == -1 &&
//                singleList[i + 10].GetComponent<CardInformations>().CardValue - singleList[i + 11].GetComponent<CardInformations>().CardValue == -1)
//            {
//                if (singleList[i].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 1].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 2].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 3].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 4].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 5].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 6].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 7].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 8].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 9].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 10].GetComponent<CardInformations>().CardValue +
//                    singleList[i + 11].GetComponent<CardInformations>().CardValue > _currentTurnCardPoints)
//                {
//                    tempPlayCardList.Add(singleList[i]);
//                    tempPlayCardList.Add(singleList[i + 1]);
//                    tempPlayCardList.Add(singleList[i + 2]);
//                    tempPlayCardList.Add(singleList[i + 3]);
//                    tempPlayCardList.Add(singleList[i + 4]);
//                    tempPlayCardList.Add(singleList[i + 5]);
//                    tempPlayCardList.Add(singleList[i + 6]);
//                    tempPlayCardList.Add(singleList[i + 7]);
//                    tempPlayCardList.Add(singleList[i + 8]);
//                    tempPlayCardList.Add(singleList[i + 9]);
//                    tempPlayCardList.Add(singleList[i + 10]);
//                    tempPlayCardList.Add(singleList[i + 11]);

//                    singleList.Remove(singleList[i + 11]);
//                    singleList.Remove(singleList[i + 10]);
//                    singleList.Remove(singleList[i + 9]);
//                    singleList.Remove(singleList[i + 8]);
//                    singleList.Remove(singleList[i + 7]);
//                    singleList.Remove(singleList[i + 6]);
//                    singleList.Remove(singleList[i + 5]);
//                    singleList.Remove(singleList[i + 4]);
//                    singleList.Remove(singleList[i + 3]);
//                    singleList.Remove(singleList[i + 2]);
//                    singleList.Remove(singleList[i + 1]);
//                    singleList.Remove(singleList[i]);

//                    RecoverForStraight();

//                    OrderCardsList(tempPlayCardList);

//                    return tempPlayCardList;
//                }
//            }
//        }
//    }

//    //复原对子库
//    RecoverForStraight();

//    //tip：不打算拆三连，所以这之后
//    //直接判断炸弹。。。。。。


//    return null;
//}

////为取得顺子后进行相关列表重新排序
////复原对子库
///// <summary>
///// 因为会出现即使拆分了对子库也找不到满足条件组合的牌，这时候的需要将他们恢复
///// </summary>
//private void RecoverForStraight()
//{
//    OrderCardsList(singleList);

//    for (int i = 0; i < pairList.Count; i++)
//    {
//        singleList.Add(pairList[i]);
//    }

//    pairList.Clear();

//    OrderCardsList(singleList);

//    //从单牌库中重新添加对子
//    for (int i = 0; i < singleList.Count - 1; i++)
//    {
//        if (singleList[i].gameObject.GetComponent<CardInformations>().CardValue == singleList[i + 1].gameObject.GetComponent<CardInformations>().CardValue)
//        {
//            pairList.Add(singleList[i]);
//            pairList.Add(singleList[i + 1]);
//        }
//    }

//    for (int i = 0; i < pairList.Count; i++)
//    {
//        if (singleList.Contains(pairList[i]) == true)
//        {
//            singleList.Remove(pairList[i]);
//        }
//    }

//    OrderCardsList(singleList);
//    OrderCardsList(pairList);
//}
//#endregion


////4，若当前回合是---连对
//#region CurrentTurn_Stright_Pair

//public List<GameObject> CurrentTurn_Stright_Pair(int _currentTurnCardPoints, int _currentTurnCardCounts)
//{
//    ///tip:原理与顺子相同

//    List<GameObject> tempPlayCardList = new List<GameObject>();

//    tempPlayCardList.Clear();

//    for (int i = 0; i < tripleList.Count; i++)
//    {
//        if (pairList.Contains(tripleList[i]) == false)
//        {
//            pairList.Add(tripleList[i]);
//            pairList.Add(tripleList[i + 1]);
//            singleList.Add(tripleList[i + 2]);
//        }
//    }

//    //从三连库中移除添加过的牌
//    for (int i = 0; i < pairList.Count; i++)
//    {
//        if (tripleList.Contains(pairList[i]) == true)
//        {
//            tripleList.Remove(pairList[i]);
//        }
//    }

//    for (int i = 0; i < singleList.Count; i++)
//    {
//        if (tripleList.Contains(singleList[i]) == true)
//        {
//            tripleList.Remove(singleList[i]);
//        }
//    }

//    OrderCardsList(pairList);

//    //如果对子数量不满足当前回合出牌数量
//    if (pairList.Count < _currentTurnCardCounts)
//    {
//        RecoverForStraight_Pair();

//        return null;
//    }

//    //1，当有3对连对，即6张牌时
//    if (_currentTurnCardCounts == 6)
//    {
//        for (int i = 0; i < pairList.Count / 2 - 2; i += 2)
//        {
//            //因为加入对子库的一定是对子，所以不需要一一判断，只需要判断相对位置是否相差1
//            if (pairList[i].GetComponent<CardInformations>().CardValue - pairList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
//                pairList[i + 2].GetComponent<CardInformations>().CardValue - pairList[i + 4].GetComponent<CardInformations>().CardValue == -1)
//            {
//                //然后判断点数就必须将对应数量的牌的点数加起来才行
//                if (pairList[i].GetComponent<CardInformations>().CardValue + pairList[i + 1].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 2].GetComponent<CardInformations>().CardValue + pairList[i + 3].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 4].GetComponent<CardInformations>().CardValue + pairList[i + 5].GetComponent<CardInformations>().CardValue > _currentTurnCardPoints)
//                {
//                    tempPlayCardList.Add(pairList[i]);
//                    tempPlayCardList.Add(pairList[i + 1]);
//                    tempPlayCardList.Add(pairList[i + 2]);
//                    tempPlayCardList.Add(pairList[i + 3]);
//                    tempPlayCardList.Add(pairList[i + 4]);
//                    tempPlayCardList.Add(pairList[i + 5]);

//                    pairList.Remove(pairList[i + 5]);
//                    pairList.Remove(pairList[i + 4]);
//                    pairList.Remove(pairList[i + 3]);
//                    pairList.Remove(pairList[i + 2]);
//                    pairList.Remove(pairList[i + 1]);
//                    pairList.Remove(pairList[i]);

//                    RecoverForStraight_Pair();

//                    OrderCardsList(tempPlayCardList);

//                    return tempPlayCardList;
//                }
//            }
//        }
//    }

//    //2，当有4对连对，即8张牌时
//    if (_currentTurnCardCounts == 8)
//    {
//        for (int i = 0; i < pairList.Count / 2 - 2; i += 2)
//        {
//            //同理
//            if (pairList[i].GetComponent<CardInformations>().CardValue - pairList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
//                pairList[i + 2].GetComponent<CardInformations>().CardValue - pairList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
//                pairList[i + 4].GetComponent<CardInformations>().CardValue - pairList[i + 6].GetComponent<CardInformations>().CardValue == -1)
//            {
//                if (pairList[i].GetComponent<CardInformations>().CardValue + pairList[i + 1].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 2].GetComponent<CardInformations>().CardValue + pairList[i + 3].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 4].GetComponent<CardInformations>().CardValue + pairList[i + 5].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 6].GetComponent<CardInformations>().CardValue + pairList[i + 7].GetComponent<CardInformations>().CardValue > _currentTurnCardPoints)
//                {
//                    tempPlayCardList.Add(pairList[i]);
//                    tempPlayCardList.Add(pairList[i + 1]);
//                    tempPlayCardList.Add(pairList[i + 2]);
//                    tempPlayCardList.Add(pairList[i + 3]);
//                    tempPlayCardList.Add(pairList[i + 4]);
//                    tempPlayCardList.Add(pairList[i + 5]);
//                    tempPlayCardList.Add(pairList[i + 6]);
//                    tempPlayCardList.Add(pairList[i + 7]);

//                    pairList.Remove(pairList[i + 7]);
//                    pairList.Remove(pairList[i + 6]);
//                    pairList.Remove(pairList[i + 5]);
//                    pairList.Remove(pairList[i + 4]);
//                    pairList.Remove(pairList[i + 3]);
//                    pairList.Remove(pairList[i + 2]);
//                    pairList.Remove(pairList[i + 1]);
//                    pairList.Remove(pairList[i]);

//                    RecoverForStraight_Pair();

//                    OrderCardsList(tempPlayCardList);

//                    return tempPlayCardList;
//                }
//            }
//        }
//    }

//    //3，当有5对连对，即10张牌时
//    if (_currentTurnCardCounts == 10)
//    {
//        for (int i = 0; i < pairList.Count / 2 - 2; i += 2)
//        {
//            //同理
//            if (pairList[i].GetComponent<CardInformations>().CardValue - pairList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
//                pairList[i + 2].GetComponent<CardInformations>().CardValue - pairList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
//                pairList[i + 4].GetComponent<CardInformations>().CardValue - pairList[i + 6].GetComponent<CardInformations>().CardValue == -1 &&
//                pairList[i + 6].GetComponent<CardInformations>().CardValue - pairList[i + 8].GetComponent<CardInformations>().CardValue == -1)
//            {
//                if (pairList[i].GetComponent<CardInformations>().CardValue + pairList[i + 1].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 2].GetComponent<CardInformations>().CardValue + pairList[i + 3].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 4].GetComponent<CardInformations>().CardValue + pairList[i + 5].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 6].GetComponent<CardInformations>().CardValue + pairList[i + 7].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 8].GetComponent<CardInformations>().CardValue + pairList[i + 9].GetComponent<CardInformations>().CardValue > _currentTurnCardPoints)
//                {
//                    tempPlayCardList.Add(pairList[i]);
//                    tempPlayCardList.Add(pairList[i + 1]);
//                    tempPlayCardList.Add(pairList[i + 2]);
//                    tempPlayCardList.Add(pairList[i + 3]);
//                    tempPlayCardList.Add(pairList[i + 4]);
//                    tempPlayCardList.Add(pairList[i + 5]);
//                    tempPlayCardList.Add(pairList[i + 6]);
//                    tempPlayCardList.Add(pairList[i + 7]);
//                    tempPlayCardList.Add(pairList[i + 8]);
//                    tempPlayCardList.Add(pairList[i + 9]);

//                    pairList.Remove(pairList[i + 9]);
//                    pairList.Remove(pairList[i + 8]);
//                    pairList.Remove(pairList[i + 7]);
//                    pairList.Remove(pairList[i + 6]);
//                    pairList.Remove(pairList[i + 5]);
//                    pairList.Remove(pairList[i + 4]);
//                    pairList.Remove(pairList[i + 3]);
//                    pairList.Remove(pairList[i + 2]);
//                    pairList.Remove(pairList[i + 1]);
//                    pairList.Remove(pairList[i]);

//                    RecoverForStraight_Pair();

//                    OrderCardsList(tempPlayCardList);

//                    return tempPlayCardList;
//                }
//            }
//        }
//    }

//    //4，当有6对连对，即12张牌时
//    if (_currentTurnCardCounts == 12)
//    {
//        for (int i = 0; i < pairList.Count / 2 - 2; i += 2)
//        {
//            //同理
//            if (pairList[i].GetComponent<CardInformations>().CardValue - pairList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
//                pairList[i + 2].GetComponent<CardInformations>().CardValue - pairList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
//                pairList[i + 4].GetComponent<CardInformations>().CardValue - pairList[i + 6].GetComponent<CardInformations>().CardValue == -1 &&
//                pairList[i + 6].GetComponent<CardInformations>().CardValue - pairList[i + 8].GetComponent<CardInformations>().CardValue == -1 &&
//                pairList[i + 8].GetComponent<CardInformations>().CardValue - pairList[i + 10].GetComponent<CardInformations>().CardValue == -1)
//            {
//                if (pairList[i].GetComponent<CardInformations>().CardValue + pairList[i + 1].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 2].GetComponent<CardInformations>().CardValue + pairList[i + 3].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 4].GetComponent<CardInformations>().CardValue + pairList[i + 5].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 6].GetComponent<CardInformations>().CardValue + pairList[i + 7].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 8].GetComponent<CardInformations>().CardValue + pairList[i + 9].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 10].GetComponent<CardInformations>().CardValue + pairList[i + 11].GetComponent<CardInformations>().CardValue > _currentTurnCardPoints)
//                {
//                    tempPlayCardList.Add(pairList[i]);
//                    tempPlayCardList.Add(pairList[i + 1]);
//                    tempPlayCardList.Add(pairList[i + 2]);
//                    tempPlayCardList.Add(pairList[i + 3]);
//                    tempPlayCardList.Add(pairList[i + 4]);
//                    tempPlayCardList.Add(pairList[i + 5]);
//                    tempPlayCardList.Add(pairList[i + 6]);
//                    tempPlayCardList.Add(pairList[i + 7]);
//                    tempPlayCardList.Add(pairList[i + 8]);
//                    tempPlayCardList.Add(pairList[i + 9]);
//                    tempPlayCardList.Add(pairList[i + 10]);
//                    tempPlayCardList.Add(pairList[i + 11]);

//                    pairList.Remove(pairList[i + 11]);
//                    pairList.Remove(pairList[i + 10]);
//                    pairList.Remove(pairList[i + 9]);
//                    pairList.Remove(pairList[i + 8]);
//                    pairList.Remove(pairList[i + 7]);
//                    pairList.Remove(pairList[i + 6]);
//                    pairList.Remove(pairList[i + 5]);
//                    pairList.Remove(pairList[i + 4]);
//                    pairList.Remove(pairList[i + 3]);
//                    pairList.Remove(pairList[i + 2]);
//                    pairList.Remove(pairList[i + 1]);
//                    pairList.Remove(pairList[i]);

//                    RecoverForStraight_Pair();

//                    OrderCardsList(tempPlayCardList);

//                    return tempPlayCardList;
//                }
//            }
//        }
//    }

//    //5，当有7对连对，即14张牌时
//    if (_currentTurnCardCounts == 14)
//    {
//        for (int i = 0; i < pairList.Count / 2 - 2; i += 2)
//        {
//            //同理
//            if (pairList[i].GetComponent<CardInformations>().CardValue - pairList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
//                pairList[i + 2].GetComponent<CardInformations>().CardValue - pairList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
//                pairList[i + 4].GetComponent<CardInformations>().CardValue - pairList[i + 6].GetComponent<CardInformations>().CardValue == -1 &&
//                pairList[i + 6].GetComponent<CardInformations>().CardValue - pairList[i + 8].GetComponent<CardInformations>().CardValue == -1 &&
//                pairList[i + 8].GetComponent<CardInformations>().CardValue - pairList[i + 10].GetComponent<CardInformations>().CardValue == -1 &&
//                pairList[i + 10].GetComponent<CardInformations>().CardValue - pairList[i + 12].GetComponent<CardInformations>().CardValue == -1)
//            {
//                if (pairList[i].GetComponent<CardInformations>().CardValue + pairList[i + 1].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 2].GetComponent<CardInformations>().CardValue + pairList[i + 3].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 4].GetComponent<CardInformations>().CardValue + pairList[i + 5].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 6].GetComponent<CardInformations>().CardValue + pairList[i + 7].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 8].GetComponent<CardInformations>().CardValue + pairList[i + 9].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 10].GetComponent<CardInformations>().CardValue + pairList[i + 11].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 12].GetComponent<CardInformations>().CardValue + pairList[i + 13].GetComponent<CardInformations>().CardValue > _currentTurnCardPoints)
//                {
//                    tempPlayCardList.Add(pairList[i]);
//                    tempPlayCardList.Add(pairList[i + 1]);
//                    tempPlayCardList.Add(pairList[i + 2]);
//                    tempPlayCardList.Add(pairList[i + 3]);
//                    tempPlayCardList.Add(pairList[i + 4]);
//                    tempPlayCardList.Add(pairList[i + 5]);
//                    tempPlayCardList.Add(pairList[i + 6]);
//                    tempPlayCardList.Add(pairList[i + 7]);
//                    tempPlayCardList.Add(pairList[i + 8]);
//                    tempPlayCardList.Add(pairList[i + 9]);
//                    tempPlayCardList.Add(pairList[i + 10]);
//                    tempPlayCardList.Add(pairList[i + 11]);
//                    tempPlayCardList.Add(pairList[i + 12]);
//                    tempPlayCardList.Add(pairList[i + 13]);

//                    pairList.Remove(pairList[i + 13]);
//                    pairList.Remove(pairList[i + 12]);
//                    pairList.Remove(pairList[i + 11]);
//                    pairList.Remove(pairList[i + 10]);
//                    pairList.Remove(pairList[i + 9]);
//                    pairList.Remove(pairList[i + 8]);
//                    pairList.Remove(pairList[i + 7]);
//                    pairList.Remove(pairList[i + 6]);
//                    pairList.Remove(pairList[i + 5]);
//                    pairList.Remove(pairList[i + 4]);
//                    pairList.Remove(pairList[i + 3]);
//                    pairList.Remove(pairList[i + 2]);
//                    pairList.Remove(pairList[i + 1]);
//                    pairList.Remove(pairList[i]);

//                    RecoverForStraight_Pair();

//                    OrderCardsList(tempPlayCardList);

//                    return tempPlayCardList;
//                }
//            }
//        }
//    }

//    //6，当有8对连对，即16张牌时
//    if (_currentTurnCardCounts == 16)
//    {
//        for (int i = 0; i < pairList.Count / 2 - 2; i += 2)
//        {
//            //同理
//            if (pairList[i].GetComponent<CardInformations>().CardValue - pairList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
//                pairList[i + 2].GetComponent<CardInformations>().CardValue - pairList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
//                pairList[i + 4].GetComponent<CardInformations>().CardValue - pairList[i + 6].GetComponent<CardInformations>().CardValue == -1 &&
//                pairList[i + 6].GetComponent<CardInformations>().CardValue - pairList[i + 8].GetComponent<CardInformations>().CardValue == -1 &&
//                pairList[i + 8].GetComponent<CardInformations>().CardValue - pairList[i + 10].GetComponent<CardInformations>().CardValue == -1 &&
//                pairList[i + 10].GetComponent<CardInformations>().CardValue - pairList[i + 12].GetComponent<CardInformations>().CardValue == -1 &&
//                pairList[i + 12].GetComponent<CardInformations>().CardValue - pairList[i + 14].GetComponent<CardInformations>().CardValue == -1)
//            {
//                if (pairList[i].GetComponent<CardInformations>().CardValue + pairList[i + 1].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 2].GetComponent<CardInformations>().CardValue + pairList[i + 3].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 4].GetComponent<CardInformations>().CardValue + pairList[i + 5].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 6].GetComponent<CardInformations>().CardValue + pairList[i + 7].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 8].GetComponent<CardInformations>().CardValue + pairList[i + 9].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 10].GetComponent<CardInformations>().CardValue + pairList[i + 11].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 12].GetComponent<CardInformations>().CardValue + pairList[i + 13].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 14].GetComponent<CardInformations>().CardValue + pairList[i + 15].GetComponent<CardInformations>().CardValue > _currentTurnCardPoints)
//                {
//                    tempPlayCardList.Add(pairList[i]);
//                    tempPlayCardList.Add(pairList[i + 1]);
//                    tempPlayCardList.Add(pairList[i + 2]);
//                    tempPlayCardList.Add(pairList[i + 3]);
//                    tempPlayCardList.Add(pairList[i + 4]);
//                    tempPlayCardList.Add(pairList[i + 5]);
//                    tempPlayCardList.Add(pairList[i + 6]);
//                    tempPlayCardList.Add(pairList[i + 7]);
//                    tempPlayCardList.Add(pairList[i + 8]);
//                    tempPlayCardList.Add(pairList[i + 9]);
//                    tempPlayCardList.Add(pairList[i + 10]);
//                    tempPlayCardList.Add(pairList[i + 11]);
//                    tempPlayCardList.Add(pairList[i + 12]);
//                    tempPlayCardList.Add(pairList[i + 13]);
//                    tempPlayCardList.Add(pairList[i + 14]);
//                    tempPlayCardList.Add(pairList[i + 15]);

//                    pairList.Remove(pairList[i + 15]);
//                    pairList.Remove(pairList[i + 14]);
//                    pairList.Remove(pairList[i + 13]);
//                    pairList.Remove(pairList[i + 12]);
//                    pairList.Remove(pairList[i + 11]);
//                    pairList.Remove(pairList[i + 10]);
//                    pairList.Remove(pairList[i + 9]);
//                    pairList.Remove(pairList[i + 8]);
//                    pairList.Remove(pairList[i + 7]);
//                    pairList.Remove(pairList[i + 6]);
//                    pairList.Remove(pairList[i + 5]);
//                    pairList.Remove(pairList[i + 4]);
//                    pairList.Remove(pairList[i + 3]);
//                    pairList.Remove(pairList[i + 2]);
//                    pairList.Remove(pairList[i + 1]);
//                    pairList.Remove(pairList[i]);

//                    RecoverForStraight_Pair();

//                    OrderCardsList(tempPlayCardList);

//                    return tempPlayCardList;
//                }
//            }
//        }
//    }

//    //7，当有9对连对，即18张牌时
//    if (_currentTurnCardCounts == 18)
//    {
//        for (int i = 0; i < pairList.Count / 2 - 2; i += 2)
//        {
//            //同理
//            if (pairList[i].GetComponent<CardInformations>().CardValue - pairList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
//                pairList[i + 2].GetComponent<CardInformations>().CardValue - pairList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
//                pairList[i + 4].GetComponent<CardInformations>().CardValue - pairList[i + 6].GetComponent<CardInformations>().CardValue == -1 &&
//                pairList[i + 6].GetComponent<CardInformations>().CardValue - pairList[i + 8].GetComponent<CardInformations>().CardValue == -1 &&
//                pairList[i + 8].GetComponent<CardInformations>().CardValue - pairList[i + 10].GetComponent<CardInformations>().CardValue == -1 &&
//                pairList[i + 10].GetComponent<CardInformations>().CardValue - pairList[i + 12].GetComponent<CardInformations>().CardValue == -1 &&
//                pairList[i + 12].GetComponent<CardInformations>().CardValue - pairList[i + 14].GetComponent<CardInformations>().CardValue == -1 &&
//                pairList[i + 14].GetComponent<CardInformations>().CardValue - pairList[i + 16].GetComponent<CardInformations>().CardValue == -1)
//            {
//                if (pairList[i].GetComponent<CardInformations>().CardValue + pairList[i + 1].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 2].GetComponent<CardInformations>().CardValue + pairList[i + 3].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 4].GetComponent<CardInformations>().CardValue + pairList[i + 5].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 6].GetComponent<CardInformations>().CardValue + pairList[i + 7].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 8].GetComponent<CardInformations>().CardValue + pairList[i + 9].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 10].GetComponent<CardInformations>().CardValue + pairList[i + 11].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 12].GetComponent<CardInformations>().CardValue + pairList[i + 13].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 14].GetComponent<CardInformations>().CardValue + pairList[i + 15].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 16].GetComponent<CardInformations>().CardValue + pairList[i + 17].GetComponent<CardInformations>().CardValue > _currentTurnCardPoints)
//                {
//                    tempPlayCardList.Add(pairList[i]);
//                    tempPlayCardList.Add(pairList[i + 1]);
//                    tempPlayCardList.Add(pairList[i + 2]);
//                    tempPlayCardList.Add(pairList[i + 3]);
//                    tempPlayCardList.Add(pairList[i + 4]);
//                    tempPlayCardList.Add(pairList[i + 5]);
//                    tempPlayCardList.Add(pairList[i + 6]);
//                    tempPlayCardList.Add(pairList[i + 7]);
//                    tempPlayCardList.Add(pairList[i + 8]);
//                    tempPlayCardList.Add(pairList[i + 9]);
//                    tempPlayCardList.Add(pairList[i + 10]);
//                    tempPlayCardList.Add(pairList[i + 11]);
//                    tempPlayCardList.Add(pairList[i + 12]);
//                    tempPlayCardList.Add(pairList[i + 13]);
//                    tempPlayCardList.Add(pairList[i + 14]);
//                    tempPlayCardList.Add(pairList[i + 15]);
//                    tempPlayCardList.Add(pairList[i + 16]);
//                    tempPlayCardList.Add(pairList[i + 17]);

//                    pairList.Remove(pairList[i + 17]);
//                    pairList.Remove(pairList[i + 16]);
//                    pairList.Remove(pairList[i + 15]);
//                    pairList.Remove(pairList[i + 14]);
//                    pairList.Remove(pairList[i + 13]);
//                    pairList.Remove(pairList[i + 12]);
//                    pairList.Remove(pairList[i + 11]);
//                    pairList.Remove(pairList[i + 10]);
//                    pairList.Remove(pairList[i + 9]);
//                    pairList.Remove(pairList[i + 8]);
//                    pairList.Remove(pairList[i + 7]);
//                    pairList.Remove(pairList[i + 6]);
//                    pairList.Remove(pairList[i + 5]);
//                    pairList.Remove(pairList[i + 4]);
//                    pairList.Remove(pairList[i + 3]);
//                    pairList.Remove(pairList[i + 2]);
//                    pairList.Remove(pairList[i + 1]);
//                    pairList.Remove(pairList[i]);

//                    RecoverForStraight_Pair();

//                    OrderCardsList(tempPlayCardList);

//                    return tempPlayCardList;
//                }
//            }
//        }
//    }

//    //8，当有10对连对，即20张牌时
//    if (_currentTurnCardCounts == 20)
//    {
//        for (int i = 0; i < pairList.Count / 2 - 2; i += 2)
//        {
//            //同理
//            if (pairList[i].GetComponent<CardInformations>().CardValue - pairList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
//                pairList[i + 2].GetComponent<CardInformations>().CardValue - pairList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
//                pairList[i + 4].GetComponent<CardInformations>().CardValue - pairList[i + 6].GetComponent<CardInformations>().CardValue == -1 &&
//                pairList[i + 6].GetComponent<CardInformations>().CardValue - pairList[i + 8].GetComponent<CardInformations>().CardValue == -1 &&
//                pairList[i + 8].GetComponent<CardInformations>().CardValue - pairList[i + 10].GetComponent<CardInformations>().CardValue == -1 &&
//                pairList[i + 10].GetComponent<CardInformations>().CardValue - pairList[i + 12].GetComponent<CardInformations>().CardValue == -1 &&
//                pairList[i + 12].GetComponent<CardInformations>().CardValue - pairList[i + 14].GetComponent<CardInformations>().CardValue == -1 &&
//                pairList[i + 14].GetComponent<CardInformations>().CardValue - pairList[i + 16].GetComponent<CardInformations>().CardValue == -1 &&
//                pairList[i + 16].GetComponent<CardInformations>().CardValue - pairList[i + 18].GetComponent<CardInformations>().CardValue == -1)
//            {
//                if (pairList[i].GetComponent<CardInformations>().CardValue + pairList[i + 1].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 2].GetComponent<CardInformations>().CardValue + pairList[i + 3].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 4].GetComponent<CardInformations>().CardValue + pairList[i + 5].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 6].GetComponent<CardInformations>().CardValue + pairList[i + 7].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 8].GetComponent<CardInformations>().CardValue + pairList[i + 9].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 10].GetComponent<CardInformations>().CardValue + pairList[i + 11].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 12].GetComponent<CardInformations>().CardValue + pairList[i + 13].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 14].GetComponent<CardInformations>().CardValue + pairList[i + 15].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 16].GetComponent<CardInformations>().CardValue + pairList[i + 17].GetComponent<CardInformations>().CardValue +
//                    pairList[i + 18].GetComponent<CardInformations>().CardValue + pairList[i + 19].GetComponent<CardInformations>().CardValue > _currentTurnCardPoints)
//                {
//                    tempPlayCardList.Add(pairList[i]);
//                    tempPlayCardList.Add(pairList[i + 1]);
//                    tempPlayCardList.Add(pairList[i + 2]);
//                    tempPlayCardList.Add(pairList[i + 3]);
//                    tempPlayCardList.Add(pairList[i + 4]);
//                    tempPlayCardList.Add(pairList[i + 5]);
//                    tempPlayCardList.Add(pairList[i + 6]);
//                    tempPlayCardList.Add(pairList[i + 7]);
//                    tempPlayCardList.Add(pairList[i + 8]);
//                    tempPlayCardList.Add(pairList[i + 9]);
//                    tempPlayCardList.Add(pairList[i + 10]);
//                    tempPlayCardList.Add(pairList[i + 11]);
//                    tempPlayCardList.Add(pairList[i + 12]);
//                    tempPlayCardList.Add(pairList[i + 13]);
//                    tempPlayCardList.Add(pairList[i + 14]);
//                    tempPlayCardList.Add(pairList[i + 15]);
//                    tempPlayCardList.Add(pairList[i + 16]);
//                    tempPlayCardList.Add(pairList[i + 17]);
//                    tempPlayCardList.Add(pairList[i + 18]);
//                    tempPlayCardList.Add(pairList[i + 19]);

//                    pairList.Remove(pairList[i + 19]);
//                    pairList.Remove(pairList[i + 18]);
//                    pairList.Remove(pairList[i + 17]);
//                    pairList.Remove(pairList[i + 16]);
//                    pairList.Remove(pairList[i + 15]);
//                    pairList.Remove(pairList[i + 14]);
//                    pairList.Remove(pairList[i + 13]);
//                    pairList.Remove(pairList[i + 12]);
//                    pairList.Remove(pairList[i + 11]);
//                    pairList.Remove(pairList[i + 10]);
//                    pairList.Remove(pairList[i + 9]);
//                    pairList.Remove(pairList[i + 8]);
//                    pairList.Remove(pairList[i + 7]);
//                    pairList.Remove(pairList[i + 6]);
//                    pairList.Remove(pairList[i + 5]);
//                    pairList.Remove(pairList[i + 4]);
//                    pairList.Remove(pairList[i + 3]);
//                    pairList.Remove(pairList[i + 2]);
//                    pairList.Remove(pairList[i + 1]);
//                    pairList.Remove(pairList[i]);

//                    RecoverForStraight_Pair();

//                    OrderCardsList(tempPlayCardList);

//                    return tempPlayCardList;
//                }
//            }
//        }
//    }

//    RecoverForStraight_Pair();

//    //判断炸弹。。。。。。

//    return null;
//}

////复原对子库和三连库
//private void RecoverForStraight_Pair()
//{
//    OrderCardsList(singleList);

//    for (int i = 0; i < pairList.Count; i++)
//    {
//        singleList.Add(pairList[i]);
//    }

//    for (int i = 0; i < tripleList.Count; i++)
//    {
//        singleList.Add(tripleList[i]);
//    }

//    pairList.Clear();
//    tripleList.Clear();

//    OrderCardsList(singleList);

//    //添加三连
//    for (int i = 0; i < singleList.Count - 2; i++)
//    {
//        if (singleList[i].gameObject.GetComponent<CardInformations>().CardValue == singleList[i + 1].gameObject.GetComponent<CardInformations>().CardValue &&
//            singleList[i].gameObject.GetComponent<CardInformations>().CardValue == singleList[i + 2].gameObject.GetComponent<CardInformations>().CardValue)
//        {
//            tripleList.Add(singleList[i]);
//            tripleList.Add(singleList[i + 1]);
//            tripleList.Add(singleList[i + 2]);
//        }
//    }

//    for (int i = 0; i < tripleList.Count; i++)
//    {
//        if (singleList.Contains(tripleList[i]) == true)
//        {
//            singleList.Remove(tripleList[i]);
//        }
//    }

//    //添加对子
//    for (int i = 0; i < singleList.Count - 1; i++)
//    {
//        if (singleList[i].gameObject.GetComponent<CardInformations>().CardValue == singleList[i + 1].gameObject.GetComponent<CardInformations>().CardValue)
//        {
//            pairList.Add(singleList[i]);
//            pairList.Add(singleList[i + 1]);
//        }
//    }

//    for (int i = 0; i < pairList.Count; i++)
//    {
//        if (singleList.Contains(pairList[i]) == true)
//        {
//            singleList.Remove(pairList[i]);
//        }
//    }

//    OrderCardsList(singleList);
//    OrderCardsList(pairList);
//    OrderCardsList(tripleList);
//}

//#endregion


////5，若当前回合是---三连
//public List<GameObject> CurrentTurn_Triple(int _currentTurnCardPoints)
//{
//    List<GameObject> tempPlayCardList = new List<GameObject>();

//    tempPlayCardList.Clear();

//    for (int i = 0; i < TripleTimes(tripleList.Count); i += 3)
//    {
//        if (tripleList[i].GetComponent<CardInformations>().CardValue +
//            tripleList[i + 1].GetComponent<CardInformations>().CardValue +
//            tripleList[i + 2].GetComponent<CardInformations>().CardValue > _currentTurnCardPoints)
//        {
//            tempPlayCardList.Add(tripleList[i]);
//            tempPlayCardList.Add(tripleList[i + 1]);
//            tempPlayCardList.Add(tripleList[i + 2]);

//            tripleList.Remove(tripleList[i + 2]);
//            tripleList.Remove(tripleList[i + 1]);
//            tripleList.Remove(tripleList[i]);

//            OrderCardsList(tripleList);

//            OrderCardsList(tempPlayCardList);

//            return tempPlayCardList;
//        }
//    }

//    //判断炸弹。。。。。。

//    return null;
//}


////6，若当前回合是---三带一
///// <summary>
///// 三带一就只需要判断三连部分的点数就可以了，单盘部分随便加一张就好
///// </summary>
///// <param name="_currentTurnCardPoints"></param>
///// <returns></returns>
//public List<GameObject> CurrentTurn_Triple_Single(List<GameObject> _currentTurnCardList)
//{
//    List<GameObject> tempPlayCardList = new List<GameObject>();
//    tempPlayCardList.Clear();

//    if (tripleList.Count == 0 || tripleList == null)
//    {
//        return null;
//    }

//    JudgeTriplePart(_currentTurnCardList);

//    if (temp_triple == null)
//    {
//        return null;
//    }

//    for (int i = 0; i < temp_triple.Count; i++)
//    {
//        tempPlayCardList.Add(temp_triple[i]);
//    }

//    if (singleList.Count > 0)
//    {
//        tempPlayCardList.Add(singleList[0]);              
//    }
//    else
//    {
//        if (pairList.Count > 0)
//        {
//            tempPlayCardList.Add(pairList[0]);

//            singleList.Add(pairList[1]);

//            pairList.Remove(pairList[1]);
//            pairList.Remove(pairList[0]);                   
//        }
//        else
//        {
//            return null;
//        }
//    }

//    OrderCardsList(singleList);
//    OrderCardsList(pairList);
//    OrderCardsList(tripleList);
//    OrderCardsList(tempPlayCardList);

//    return tempPlayCardList;

//    //再判断单牌库是否有牌，若有，直接将单牌库第零号位的牌添加到当前出牌列表，因为零号位的牌是最小的嘛

//    //判断炸弹。。。。。。
//}


////7，若当前回合是---三带一对
//public List<GameObject> CurrentTurn_Triple_Pair(List<GameObject> _currentTurnCardList)
//{
//    //与三带一同理

//    List<GameObject> tempPlayCardList = new List<GameObject>();

//    tempPlayCardList.Clear();

//    if (tripleList.Count == 0 || tripleList == null)
//    {
//        return null;
//    }

//    JudgeTriplePart(_currentTurnCardList);

//    if (temp_triple == null || temp_triple.Count == 0)
//    {
//        return null;
//    }

//    for (int i = 0; i < temp_triple.Count; i++)
//    {
//        tempPlayCardList.Add(temp_triple[i]);
//    }

//    if (pairList.Count >= 2)
//    {
//        tempPlayCardList.Add(pairList[0]);
//        tempPlayCardList.Add(pairList[1]);

//        pairList.Remove(pairList[1]);
//        pairList.Remove(pairList[0]);
//    }
//    else
//    {
//        if (tripleList.Count >= 3)
//        {
//            tempPlayCardList.Add(tripleList[0]);
//            tempPlayCardList.Add(tripleList[1]);

//            singleList.Add(tripleList[2]);

//            tripleList.Remove(tripleList[2]);
//            tripleList.Remove(tripleList[1]);
//            tripleList.Remove(tripleList[0]);
//        }
//        else
//        {
//            return null;
//        }
//    }

//    OrderCardsList(singleList);
//    OrderCardsList(pairList);
//    OrderCardsList(tripleList);

//    OrderCardsList(tempPlayCardList);

//    return tempPlayCardList;     
//}


////判断当前部分的三连部分
//private void JudgeTriplePart(List<GameObject> _currentTurnCardList) 
//{
//    temp_triple.Clear();

//    int currentTurnCardPoints = 0;

//    for (int i = 0; i < _currentTurnCardList.Count - 2; i++)
//    {
//        if (_currentTurnCardList[i].GetComponent<CardInformations>().CardValue == _currentTurnCardList[i + 1].GetComponent<CardInformations>().CardValue &&
//            _currentTurnCardList[i].GetComponent<CardInformations>().CardValue == _currentTurnCardList[i + 2].GetComponent<CardInformations>().CardValue)
//        {
//            currentTurnCardPoints += _currentTurnCardList[i].GetComponent<CardInformations>().CardValue * 3;
//        }
//    }

//    if (tripleList == null)
//    {
//        temp_triple = null;

//        return;
//    }

//    for (int i = 0; i < TripleTimes(tripleList.Count); i += 3)
//    {
//        if (tripleList[i].GetComponent<CardInformations>().CardValue +
//            tripleList[i + 1].GetComponent<CardInformations>().CardValue +
//            tripleList[i + 2].GetComponent<CardInformations>().CardValue > currentTurnCardPoints)
//        {
//            temp_triple.Add(tripleList[i]);
//            temp_triple.Add(tripleList[i + 1]);
//            temp_triple.Add(tripleList[i + 2]);

//            tripleList.Remove(tripleList[i + 2]);
//            tripleList.Remove(tripleList[i + 1]);
//            tripleList.Remove(tripleList[i]);

//            break;
//        }
//    }
//}

//private int TripleTimes(int _currentTripleCounts)
//{
//    switch (_currentTripleCounts)
//    {
//        case 3:
//            return 1;
//        case 6:
//            return 4;
//        case 9:
//            return 7;
//        case 12:
//            return 10;
//        case 15:
//            return 13;
//        case 18:
//            return 16;
//    }

//    return 0;
//}

////8，若当前回合是---飞机不带
//public List<GameObject> CurrentTurn_Plane_None(List<GameObject> _currentTurnCardList)
//{
//    temp_plane_none.Clear();

//    int currentTurnCardPoints = 0;

//    //得到当前回合的飞机不带的三连部分的点数
//    for (int i = 0; i < _currentTurnCardList.Count; i++)
//    {
//        currentTurnCardPoints += _currentTurnCardList[i].GetComponent<CardInformations>().CardValue;
//    }

//    if (tripleList.Count < _currentTurnCardList.Count)
//    {
//        return null;
//    }

//    OrderCardsList(tripleList);

//    //1，若有2个飞机，即6张牌
//    if (_currentTurnCardList.Count == 6)
//    {
//        for (int i = 0; i < Times(tripleList.Count); i += 3)
//        {
//            if (tripleList[i].GetComponent<CardInformations>().CardValue - tripleList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 1].GetComponent<CardInformations>().CardValue - tripleList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 2].GetComponent<CardInformations>().CardValue - tripleList[i + 5].GetComponent<CardInformations>().CardValue == -1)
//            {
//                if (tripleList[i].GetComponent<CardInformations>().CardValue + tripleList[i + 1].GetComponent<CardInformations>().CardValue +
//                    tripleList[i + 2].GetComponent<CardInformations>().CardValue + tripleList[i + 3].GetComponent<CardInformations>().CardValue +
//                    tripleList[i + 4].GetComponent<CardInformations>().CardValue + tripleList[i + 5].GetComponent<CardInformations>().CardValue > currentTurnCardPoints)
//                {
//                    temp_plane_none.Add(tripleList[i]);
//                    temp_plane_none.Add(tripleList[i + 1]);
//                    temp_plane_none.Add(tripleList[i + 2]);
//                    temp_plane_none.Add(tripleList[i + 3]);
//                    temp_plane_none.Add(tripleList[i + 4]);
//                    temp_plane_none.Add(tripleList[i + 5]);

//                    tripleList.Remove(tripleList[i + 5]);
//                    tripleList.Remove(tripleList[i + 4]);
//                    tripleList.Remove(tripleList[i + 3]);
//                    tripleList.Remove(tripleList[i + 2]);
//                    tripleList.Remove(tripleList[i + 1]);
//                    tripleList.Remove(tripleList[i]);

//                    OrderCardsList(tripleList);

//                    OrderCardsList(temp_plane_none);

//                    return temp_plane_none;
//                }
//            }
//        }
//    }

//    //2，若有3个飞机，即9张牌
//    if (_currentTurnCardList.Count == 9)
//    {
//        for (int i = 0; i < Times(tripleList.Count); i += 3)
//        {
//            if (tripleList[i].GetComponent<CardInformations>().CardValue - tripleList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 1].GetComponent<CardInformations>().CardValue - tripleList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 2].GetComponent<CardInformations>().CardValue - tripleList[i + 5].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 3].GetComponent<CardInformations>().CardValue - tripleList[i + 6].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 4].GetComponent<CardInformations>().CardValue - tripleList[i + 7].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 5].GetComponent<CardInformations>().CardValue - tripleList[i + 8].GetComponent<CardInformations>().CardValue == -1)
//            {
//                if (tripleList[i].GetComponent<CardInformations>().CardValue + tripleList[i + 1].GetComponent<CardInformations>().CardValue +
//                    tripleList[i + 2].GetComponent<CardInformations>().CardValue + tripleList[i + 3].GetComponent<CardInformations>().CardValue +
//                    tripleList[i + 4].GetComponent<CardInformations>().CardValue + tripleList[i + 5].GetComponent<CardInformations>().CardValue +
//                    tripleList[i + 6].GetComponent<CardInformations>().CardValue + tripleList[i + 7].GetComponent<CardInformations>().CardValue +
//                    tripleList[i + 8].GetComponent<CardInformations>().CardValue > currentTurnCardPoints)
//                {
//                    temp_plane_none.Add(tripleList[i]);
//                    temp_plane_none.Add(tripleList[i + 1]);
//                    temp_plane_none.Add(tripleList[i + 2]);
//                    temp_plane_none.Add(tripleList[i + 3]);
//                    temp_plane_none.Add(tripleList[i + 4]);
//                    temp_plane_none.Add(tripleList[i + 5]);
//                    temp_plane_none.Add(tripleList[i + 6]);
//                    temp_plane_none.Add(tripleList[i + 7]);
//                    temp_plane_none.Add(tripleList[i + 8]);

//                    tripleList.Remove(tripleList[i + 8]);
//                    tripleList.Remove(tripleList[i + 7]);
//                    tripleList.Remove(tripleList[i + 6]);
//                    tripleList.Remove(tripleList[i + 5]);
//                    tripleList.Remove(tripleList[i + 4]);
//                    tripleList.Remove(tripleList[i + 3]);
//                    tripleList.Remove(tripleList[i + 2]);
//                    tripleList.Remove(tripleList[i + 1]);
//                    tripleList.Remove(tripleList[i]);

//                    OrderCardsList(tripleList);

//                    OrderCardsList(temp_plane_none);

//                    return temp_plane_none;
//                }
//            }
//        }
//    }

//    //3，若有4个飞机，即12张牌
//    if (_currentTurnCardList.Count == 12)
//    {
//        for (int i = 0; i < Times(tripleList.Count); i += 3)
//        {
//            if (tripleList[i].GetComponent<CardInformations>().CardValue - tripleList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 1].GetComponent<CardInformations>().CardValue - tripleList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 2].GetComponent<CardInformations>().CardValue - tripleList[i + 5].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 3].GetComponent<CardInformations>().CardValue - tripleList[i + 6].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 4].GetComponent<CardInformations>().CardValue - tripleList[i + 7].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 5].GetComponent<CardInformations>().CardValue - tripleList[i + 8].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 6].GetComponent<CardInformations>().CardValue - tripleList[i + 9].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 7].GetComponent<CardInformations>().CardValue - tripleList[i + 10].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 8].GetComponent<CardInformations>().CardValue - tripleList[i + 11].GetComponent<CardInformations>().CardValue == -1)
//            {
//                if (tripleList[i].GetComponent<CardInformations>().CardValue + tripleList[i + 1].GetComponent<CardInformations>().CardValue +
//                    tripleList[i + 2].GetComponent<CardInformations>().CardValue + tripleList[i + 3].GetComponent<CardInformations>().CardValue +
//                    tripleList[i + 4].GetComponent<CardInformations>().CardValue + tripleList[i + 5].GetComponent<CardInformations>().CardValue +
//                    tripleList[i + 6].GetComponent<CardInformations>().CardValue + tripleList[i + 7].GetComponent<CardInformations>().CardValue +
//                    tripleList[i + 8].GetComponent<CardInformations>().CardValue + tripleList[i + 9].GetComponent<CardInformations>().CardValue +
//                    tripleList[i + 10].GetComponent<CardInformations>().CardValue + tripleList[i + 11].GetComponent<CardInformations>().CardValue > currentTurnCardPoints)
//                {
//                    temp_plane_none.Add(tripleList[i]);
//                    temp_plane_none.Add(tripleList[i + 1]);
//                    temp_plane_none.Add(tripleList[i + 2]);
//                    temp_plane_none.Add(tripleList[i + 3]);
//                    temp_plane_none.Add(tripleList[i + 4]);
//                    temp_plane_none.Add(tripleList[i + 5]);
//                    temp_plane_none.Add(tripleList[i + 6]);
//                    temp_plane_none.Add(tripleList[i + 7]);
//                    temp_plane_none.Add(tripleList[i + 8]);
//                    temp_plane_none.Add(tripleList[i + 9]);
//                    temp_plane_none.Add(tripleList[i + 10]);
//                    temp_plane_none.Add(tripleList[i + 11]);

//                    tripleList.Remove(tripleList[i + 11]);
//                    tripleList.Remove(tripleList[i + 10]);
//                    tripleList.Remove(tripleList[i + 9]);
//                    tripleList.Remove(tripleList[i + 8]);
//                    tripleList.Remove(tripleList[i + 7]);
//                    tripleList.Remove(tripleList[i + 6]);
//                    tripleList.Remove(tripleList[i + 5]);
//                    tripleList.Remove(tripleList[i + 4]);
//                    tripleList.Remove(tripleList[i + 3]);
//                    tripleList.Remove(tripleList[i + 2]);
//                    tripleList.Remove(tripleList[i + 1]);
//                    tripleList.Remove(tripleList[i]);

//                    OrderCardsList(tripleList);

//                    OrderCardsList(temp_plane_none);

//                    return temp_plane_none;
//                }
//            }
//        }
//    }

//    //4，若有5个飞机，即15张牌
//    if (_currentTurnCardList.Count == 15)
//    {
//        for (int i = 0; i < Times(tripleList.Count); i += 3)
//        {
//            if (tripleList[i].GetComponent<CardInformations>().CardValue - tripleList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 1].GetComponent<CardInformations>().CardValue - tripleList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 2].GetComponent<CardInformations>().CardValue - tripleList[i + 5].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 3].GetComponent<CardInformations>().CardValue - tripleList[i + 6].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 4].GetComponent<CardInformations>().CardValue - tripleList[i + 7].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 5].GetComponent<CardInformations>().CardValue - tripleList[i + 8].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 6].GetComponent<CardInformations>().CardValue - tripleList[i + 9].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 7].GetComponent<CardInformations>().CardValue - tripleList[i + 10].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 8].GetComponent<CardInformations>().CardValue - tripleList[i + 11].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 9].GetComponent<CardInformations>().CardValue - tripleList[i + 12].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 10].GetComponent<CardInformations>().CardValue - tripleList[i + 13].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 11].GetComponent<CardInformations>().CardValue - tripleList[i + 14].GetComponent<CardInformations>().CardValue == -1)
//            {
//                if (tripleList[i].GetComponent<CardInformations>().CardValue + tripleList[i + 1].GetComponent<CardInformations>().CardValue +
//                    tripleList[i + 2].GetComponent<CardInformations>().CardValue + tripleList[i + 3].GetComponent<CardInformations>().CardValue +
//                    tripleList[i + 4].GetComponent<CardInformations>().CardValue + tripleList[i + 5].GetComponent<CardInformations>().CardValue +
//                    tripleList[i + 6].GetComponent<CardInformations>().CardValue + tripleList[i + 7].GetComponent<CardInformations>().CardValue +
//                    tripleList[i + 8].GetComponent<CardInformations>().CardValue + tripleList[i + 9].GetComponent<CardInformations>().CardValue +
//                    tripleList[i + 10].GetComponent<CardInformations>().CardValue + tripleList[i + 11].GetComponent<CardInformations>().CardValue +
//                    tripleList[i + 12].GetComponent<CardInformations>().CardValue + tripleList[i + 13].GetComponent<CardInformations>().CardValue +
//                    tripleList[i + 14].GetComponent<CardInformations>().CardValue > currentTurnCardPoints)
//                {
//                    temp_plane_none.Add(tripleList[i]);
//                    temp_plane_none.Add(tripleList[i + 1]);
//                    temp_plane_none.Add(tripleList[i + 2]);
//                    temp_plane_none.Add(tripleList[i + 3]);
//                    temp_plane_none.Add(tripleList[i + 4]);
//                    temp_plane_none.Add(tripleList[i + 5]);
//                    temp_plane_none.Add(tripleList[i + 6]);
//                    temp_plane_none.Add(tripleList[i + 7]);
//                    temp_plane_none.Add(tripleList[i + 8]);
//                    temp_plane_none.Add(tripleList[i + 9]);
//                    temp_plane_none.Add(tripleList[i + 10]);
//                    temp_plane_none.Add(tripleList[i + 11]);
//                    temp_plane_none.Add(tripleList[i + 12]);
//                    temp_plane_none.Add(tripleList[i + 13]);
//                    temp_plane_none.Add(tripleList[i + 14]);

//                    tripleList.Remove(tripleList[i + 14]);
//                    tripleList.Remove(tripleList[i + 13]);
//                    tripleList.Remove(tripleList[i + 12]);
//                    tripleList.Remove(tripleList[i + 11]);
//                    tripleList.Remove(tripleList[i + 10]);
//                    tripleList.Remove(tripleList[i + 9]);
//                    tripleList.Remove(tripleList[i + 8]);
//                    tripleList.Remove(tripleList[i + 7]);
//                    tripleList.Remove(tripleList[i + 6]);
//                    tripleList.Remove(tripleList[i + 5]);
//                    tripleList.Remove(tripleList[i + 4]);
//                    tripleList.Remove(tripleList[i + 3]);
//                    tripleList.Remove(tripleList[i + 2]);
//                    tripleList.Remove(tripleList[i + 1]);
//                    tripleList.Remove(tripleList[i]);

//                    OrderCardsList(tripleList);

//                    OrderCardsList(temp_plane_none);

//                    return temp_plane_none;
//                }
//            }
//        }
//    }

//    //5，若有6个飞机，即18张牌
//    if (_currentTurnCardList.Count == 18)
//    {
//        for (int i = 0; i < Times(tripleList.Count); i += 3)
//        {
//            if (tripleList[i].GetComponent<CardInformations>().CardValue - tripleList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 1].GetComponent<CardInformations>().CardValue - tripleList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 2].GetComponent<CardInformations>().CardValue - tripleList[i + 5].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 3].GetComponent<CardInformations>().CardValue - tripleList[i + 6].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 4].GetComponent<CardInformations>().CardValue - tripleList[i + 7].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 5].GetComponent<CardInformations>().CardValue - tripleList[i + 8].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 6].GetComponent<CardInformations>().CardValue - tripleList[i + 9].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 7].GetComponent<CardInformations>().CardValue - tripleList[i + 10].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 8].GetComponent<CardInformations>().CardValue - tripleList[i + 11].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 9].GetComponent<CardInformations>().CardValue - tripleList[i + 12].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 10].GetComponent<CardInformations>().CardValue - tripleList[i + 13].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 11].GetComponent<CardInformations>().CardValue - tripleList[i + 14].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 12].GetComponent<CardInformations>().CardValue - tripleList[i + 15].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 13].GetComponent<CardInformations>().CardValue - tripleList[i + 16].GetComponent<CardInformations>().CardValue == -1 &&
//                tripleList[i + 14].GetComponent<CardInformations>().CardValue - tripleList[i + 17].GetComponent<CardInformations>().CardValue == -1)
//            {
//                if (tripleList[i].GetComponent<CardInformations>().CardValue + tripleList[i + 1].GetComponent<CardInformations>().CardValue +
//                    tripleList[i + 2].GetComponent<CardInformations>().CardValue + tripleList[i + 3].GetComponent<CardInformations>().CardValue +
//                    tripleList[i + 4].GetComponent<CardInformations>().CardValue + tripleList[i + 5].GetComponent<CardInformations>().CardValue +
//                    tripleList[i + 6].GetComponent<CardInformations>().CardValue + tripleList[i + 7].GetComponent<CardInformations>().CardValue +
//                    tripleList[i + 8].GetComponent<CardInformations>().CardValue + tripleList[i + 9].GetComponent<CardInformations>().CardValue +
//                    tripleList[i + 10].GetComponent<CardInformations>().CardValue + tripleList[i + 11].GetComponent<CardInformations>().CardValue +
//                    tripleList[i + 12].GetComponent<CardInformations>().CardValue + tripleList[i + 13].GetComponent<CardInformations>().CardValue +
//                    tripleList[i + 14].GetComponent<CardInformations>().CardValue + tripleList[i + 15].GetComponent<CardInformations>().CardValue +
//                    tripleList[i + 16].GetComponent<CardInformations>().CardValue + tripleList[i + 17].GetComponent<CardInformations>().CardValue > currentTurnCardPoints)
//                {
//                    temp_plane_none.Add(tripleList[i]);
//                    temp_plane_none.Add(tripleList[i + 1]);
//                    temp_plane_none.Add(tripleList[i + 2]);
//                    temp_plane_none.Add(tripleList[i + 3]);
//                    temp_plane_none.Add(tripleList[i + 4]);
//                    temp_plane_none.Add(tripleList[i + 5]);
//                    temp_plane_none.Add(tripleList[i + 6]);
//                    temp_plane_none.Add(tripleList[i + 7]);
//                    temp_plane_none.Add(tripleList[i + 8]);
//                    temp_plane_none.Add(tripleList[i + 9]);
//                    temp_plane_none.Add(tripleList[i + 10]);
//                    temp_plane_none.Add(tripleList[i + 11]);
//                    temp_plane_none.Add(tripleList[i + 12]);
//                    temp_plane_none.Add(tripleList[i + 13]);
//                    temp_plane_none.Add(tripleList[i + 14]);
//                    temp_plane_none.Add(tripleList[i + 15]);
//                    temp_plane_none.Add(tripleList[i + 16]);
//                    temp_plane_none.Add(tripleList[i + 17]);

//                    tripleList.Remove(tripleList[i + 17]);
//                    tripleList.Remove(tripleList[i + 16]);
//                    tripleList.Remove(tripleList[i + 15]);
//                    tripleList.Remove(tripleList[i + 14]);
//                    tripleList.Remove(tripleList[i + 13]);
//                    tripleList.Remove(tripleList[i + 12]);
//                    tripleList.Remove(tripleList[i + 11]);
//                    tripleList.Remove(tripleList[i + 10]);
//                    tripleList.Remove(tripleList[i + 9]);
//                    tripleList.Remove(tripleList[i + 8]);
//                    tripleList.Remove(tripleList[i + 7]);
//                    tripleList.Remove(tripleList[i + 6]);
//                    tripleList.Remove(tripleList[i + 5]);
//                    tripleList.Remove(tripleList[i + 4]);
//                    tripleList.Remove(tripleList[i + 3]);
//                    tripleList.Remove(tripleList[i + 2]);
//                    tripleList.Remove(tripleList[i + 1]);
//                    tripleList.Remove(tripleList[i]);

//                    OrderCardsList(tripleList);

//                    OrderCardsList(temp_plane_none);

//                    return temp_plane_none;
//                }
//            }
//        }
//    }

//    //判断炸弹库。。。。。。

//    return null;
//}


////判断飞机循环次数
//private int Times(int _tripleCounts)
//{
//    switch (_tripleCounts)
//    {
//        case 6:
//            return 1;
//        case 9:
//            return 4;
//        case 12:
//            return 7;
//        case 15:
//            return 10;
//        case 18:
//            return 13;
//    }

//    return 0;
//}


////9，若当前回合是---飞机带单
//public List<GameObject> CurrentTurn_Plane_Single(List<GameObject> _currentTurnCardList)
//{
//    List<GameObject> tempPlayCardList = new List<GameObject>();

//    tempPlayCardList.Clear();

//    //创建临时变量currentPlaneTriplePart，用于存储当前回合飞机不带的三连部分
//    List<GameObject> currentPlaneTriplePart = new List<GameObject>();

//    currentPlaneTriplePart.Clear();

//    //获取当前飞机不带的三连部分
//    for (int i = 0; i < _currentTurnCardList.Count - 2; i++)
//    {
//        if (_currentTurnCardList[i].gameObject.GetComponent<CardInformations>().CardValue == _currentTurnCardList[i + 1].gameObject.GetComponent<CardInformations>().CardValue &&
//            _currentTurnCardList[i].gameObject.GetComponent<CardInformations>().CardValue == _currentTurnCardList[i + 2].gameObject.GetComponent<CardInformations>().CardValue)
//        {
//            currentPlaneTriplePart.Add(_currentTurnCardList[i]);
//            currentPlaneTriplePart.Add(_currentTurnCardList[i + 1]);
//            currentPlaneTriplePart.Add(_currentTurnCardList[i + 2]);
//        }
//    }

//    //移除已添加的三连部分
//    for (int i = 0; i < currentPlaneTriplePart.Count; i++)
//    {
//        if (_currentTurnCardList.Contains(currentPlaneTriplePart[i]) == true)
//        {
//            _currentTurnCardList.Remove(currentPlaneTriplePart[i]);
//        }
//    }

//    //获取满足条件的飞机不带部分
//    temp_plane_none = CurrentTurn_Plane_None(currentPlaneTriplePart);

//    if (temp_plane_none == null)
//    {
//        return null;
//    }

//    //将得到的符合条件的飞机不带部分放入tempPlayCardList中
//    for (int i = 0; i < temp_plane_none.Count; i++)
//    {
//        tempPlayCardList.Add(temp_plane_none[i]);
//    }

//    //获取与三连个数响应的单牌数量，单牌数量就等于三连牌数除以3

//    //如果单牌库的牌数量大于等于三连牌数除以3
//    if (singleList.Count >= currentPlaneTriplePart.Count / 3)
//    {
//        //将他们从小开始获取
//        for (int i = 0; i < currentPlaneTriplePart.Count / 3; i++)
//        {
//            tempPlayCardList.Add(singleList[i]);
//        }

//        //再循环tempPlayCardList，将其中与单牌库重复的移除
//        for (int i = 0; i < tempPlayCardList.Count; i++)
//        {
//            if (singleList.Contains(tempPlayCardList[i]) == true)
//            {
//                singleList.Remove(tempPlayCardList[i]);
//            }
//        }
//    }
//    else
//    {
//        //反之拆对子库
//        //如果对子库牌数量/2的加上单牌库牌数量仍不满足条件，则返回
//        if (singleList.Count + pairList.Count / 2 < currentPlaneTriplePart.Count / 3)
//        {
//            return null;
//        }
//        else
//        {
//            //反之，拆对子库
//            for (int i = 0; i < pairList.Count; i++)
//            {
//                if (singleList.Contains(pairList[i]) == false)
//                {
//                    singleList.Add(pairList[i]);

//                    pairList.Remove(pairList[i]);
//                }
//            }

//            //重新升序排列
//            OrderCardsList(singleList);

//            //从小获取
//            for (int i = 0; i < currentPlaneTriplePart.Count / 3; i++)
//            {
//                tempPlayCardList.Add(singleList[i]);
//            }

//            //再循环tempPlayCardList，将其中与单牌库重复的移除
//            for (int i = 0; i < tempPlayCardList.Count; i++)
//            {
//                if (singleList.Contains(tempPlayCardList[i]) == true)
//                {
//                    singleList.Remove(tempPlayCardList[i]);
//                }
//            }

//            //复原对子库
//            RecoverForStraight();
//        }
//    }

//    OrderCardsList(tempPlayCardList);
//    OrderCardsList(tripleList);

//    //判断炸弹库。。。。。。

//    return tempPlayCardList;
//}


////10，若当前回合是---飞机带对
//public List<GameObject> CurrentTurn_Plane_Pair(List<GameObject> _currentTurnCardList)
//{
//    List<GameObject> tempPlayCardList = new List<GameObject>();

//    tempPlayCardList.Clear();

//    //创建临时变量currentPlaneTriplePart，用于存储当前回合飞机不带的三连部分
//    List<GameObject> currentPlaneTriplePart = new List<GameObject>();

//    currentPlaneTriplePart.Clear();

//    //获取当前飞机不带的三连部分
//    for (int i = 0; i < _currentTurnCardList.Count - 2; i++)
//    {
//        if (_currentTurnCardList[i].gameObject.GetComponent<CardInformations>().CardValue == _currentTurnCardList[i + 1].gameObject.GetComponent<CardInformations>().CardValue &&
//            _currentTurnCardList[i + 1].gameObject.GetComponent<CardInformations>().CardValue == _currentTurnCardList[i + 2].gameObject.GetComponent<CardInformations>().CardValue)
//        {
//            currentPlaneTriplePart.Add(_currentTurnCardList[i]);
//            currentPlaneTriplePart.Add(_currentTurnCardList[i + 1]);
//            currentPlaneTriplePart.Add(_currentTurnCardList[i + 2]);
//        }
//    }

//    for (int i = 0; i < currentPlaneTriplePart.Count; i++)
//    {
//        if (_currentTurnCardList.Contains(currentPlaneTriplePart[i]) == true)
//        {
//            _currentTurnCardList.Remove(currentPlaneTriplePart[i]);
//        }
//    }

//    //获取满足条件的飞机不带部分
//    temp_plane_none = CurrentTurn_Plane_None(currentPlaneTriplePart);

//    if (temp_plane_none  ==  null)
//    {
//        return null;
//    }

//    //将得到的符合条件的飞机不带部分放入tempPlayCardList中
//    for (int i = 0; i < temp_plane_none.Count; i++)
//    {
//        tempPlayCardList.Add(temp_plane_none[i]);
//    }

//    //获取与三连个数响应的对子数量，对子数量就等于三连牌数除以3*2

//    //如果对子牌库的牌数量大于等于三连牌数除以3
//    if (pairList.Count >= (currentPlaneTriplePart.Count / 3) * 2)
//    {
//        //将他们从小开始获取
//        for (int i = 0; i < (currentPlaneTriplePart.Count / 3) * 2; i++)
//        {
//            tempPlayCardList.Add(pairList[i]);
//        }

//        //再循环从对子库中去除已经添加的牌
//        for (int i = 0; i < tempPlayCardList.Count; i++)
//        {
//            if (pairList.Contains(tempPlayCardList[i]) == true)
//            {
//                pairList.Remove(tempPlayCardList[i]);
//            }
//        }
//    }
//    else
//    {
//        //反之拆三连库
//        //如果三连库库牌数量/3*2的加上对子库牌数量仍不满足条件，则返回
//        if (pairList.Count + (tripleList.Count / 3) * 2 < (currentPlaneTriplePart.Count / 3) * 2)
//        {
//            return null;
//        }
//        else
//        {
//            //反之，拆三连库
//            for (int i = 0; i < tripleList.Count; i++)
//            {
//                if (pairList.Contains(tripleList[i]) == false)
//                {
//                    pairList.Add(tripleList[i]);
//                    pairList.Add(tripleList[i + 1]);

//                    tripleList.Remove(tripleList[i + 1]);
//                    tripleList.Remove(tripleList[i]);
//                }
//            }

//            //重新升序排列
//            OrderCardsList(pairList);

//            //从小获取
//            for (int i = 0; i < (currentPlaneTriplePart.Count / 3) * 2; i++)
//            {
//                tempPlayCardList.Add(pairList[i]);
//            }

//            for (int i = 0; i < tempPlayCardList.Count; i++)
//            {
//                pairList.Remove(tempPlayCardList[i]);
//            }

//            //复原三连库
//            RecoverForStraight_Pair();
//        }
//    }

//    OrderCardsList(tempPlayCardList);
//    OrderCardsList(tripleList);

//    //判断炸弹库。。。。。。

//    return tempPlayCardList;
//}


////11，若当前回合是---炸弹
//public List<GameObject> CurrentTurn_Boom(int _currentTurnCardPoints)
//{
//    temp_boom.Clear();

//    if (boomList.Count != 0)
//    {
//        for (int i = 0; i < boomList.Count / 4; i += 4)
//        {
//            if ((boomList[i].GetComponent<CardInformations>().CardValue + boomList[i + 1].GetComponent<CardInformations>().CardValue +
//                 boomList[i + 2].GetComponent<CardInformations>().CardValue + boomList[i + 3].GetComponent<CardInformations>().CardValue)*100 > _currentTurnCardPoints)
//            {
//                temp_boom.Add(boomList[i]);
//                temp_boom.Add(boomList[i + 1]);
//                temp_boom.Add(boomList[i + 2]);
//                temp_boom.Add(boomList[i + 3]);

//                boomList.Remove(boomList[i + 3]);
//                boomList.Remove(boomList[i + 2]);
//                boomList.Remove(boomList[i + 1]);
//                boomList.Remove(boomList[i]);

//                OrderCardsList(boomList);

//                OrderCardsList(temp_boom);

//                return temp_boom;
//            }
//        }
//    }

//    //判断王炸库。。。。。。

//    return null;
//}


////12，若当前回合是---炸弹带单
//public List<GameObject> CurrentTurn_Boom_Single(List<GameObject> _currentTurnCardList)
//{
//    ///与判断飞机带单、带双同理
//    ///炸弹带单，只需判断炸弹部分的牌的大小即可

//    List<GameObject> tempPlayCardList = new List<GameObject>();

//    tempPlayCardList.Clear();

//    int currentBoomPartPoints = 0;

//    //获取炸弹部分
//    for (int i = 0; i < _currentTurnCardList.Count / 2; i++)
//    {
//        if (_currentTurnCardList[i].GetComponent<CardInformations>().CardValue == _currentTurnCardList[i + 1].GetComponent<CardInformations>().CardValue &&
//            _currentTurnCardList[i].GetComponent<CardInformations>().CardValue == _currentTurnCardList[i + 2].GetComponent<CardInformations>().CardValue &&
//            _currentTurnCardList[i].GetComponent<CardInformations>().CardValue == _currentTurnCardList[i + 3].GetComponent<CardInformations>().CardValue)
//        {
//            currentBoomPartPoints += _currentTurnCardList[i].GetComponent<CardInformations>().CardValue;
//            currentBoomPartPoints += _currentTurnCardList[i + 1].GetComponent<CardInformations>().CardValue;
//            currentBoomPartPoints += _currentTurnCardList[i + 2].GetComponent<CardInformations>().CardValue;
//            currentBoomPartPoints += _currentTurnCardList[i + 3].GetComponent<CardInformations>().CardValue;

//            currentBoomPartPoints = currentBoomPartPoints * 100;
//        }
//    }

//    temp_boom = CurrentTurn_Boom(currentBoomPartPoints);

//    //如果没有满足条件的炸弹就返回
//    if (temp_boom == null)
//    {
//        return null;
//    }

//    for (int i = 0; i < temp_boom.Count; i++)
//    {
//        tempPlayCardList.Add(temp_boom[i]);
//    }

//    //反之，判断单牌库，添加单牌
//    if (singleList.Count >= 2)
//    {
//        tempPlayCardList.Add(singleList[0]);
//        tempPlayCardList.Add(singleList[1]);

//        singleList.Remove(singleList[1]);
//        singleList.Remove(singleList[0]);
//    }
//    else
//    {
//        if (singleList.Count + pairList.Count / 2 < 2)
//        {
//            return null;
//        }
//        else
//        {
//            //反之，拆对子库
//            for (int i = 0; i < pairList.Count; i++)
//            {
//                if (singleList.Contains(pairList[i]) == false)
//                {
//                    singleList.Add(pairList[i]);                          
//                }
//            }

//            for (int i = 0; i < singleList.Count; i++)
//            {
//                if (pairList.Contains(singleList[i]) == true)
//                {
//                    pairList.Remove(singleList[i]);
//                }                       
//            }

//            //重新升序排列
//            OrderCardsList(singleList);

//            //从小获取
//            tempPlayCardList.Add(singleList[0]);
//            tempPlayCardList.Add(singleList[1]);

//            singleList.Remove(singleList[1]);
//            singleList.Remove(singleList[0]);

//            //复原对子库
//            RecoverForStraight();
//        }
//    }

//    OrderCardsList(boomList);

//    OrderCardsList(tempPlayCardList);

//    return tempPlayCardList;
//}


////13，若当前回合是---炸弹带对
//public List<GameObject> CurrentTurn_Boom_Pair(List<GameObject> _currentTurnCardList)
//{
//    ///与判断炸弹带单同理

//    List<GameObject> tempPlayCardList = new List<GameObject>();

//    tempPlayCardList.Clear();

//    int currentBoomPartPoints = 0;

//    //获取炸弹部分
//    for (int i = 0; i < _currentTurnCardList.Count / 2; i++)
//    {
//        if (_currentTurnCardList[i].GetComponent<CardInformations>().CardValue == _currentTurnCardList[i + 1].GetComponent<CardInformations>().CardValue &&
//            _currentTurnCardList[i].GetComponent<CardInformations>().CardValue == _currentTurnCardList[i + 2].GetComponent<CardInformations>().CardValue &&
//            _currentTurnCardList[i].GetComponent<CardInformations>().CardValue == _currentTurnCardList[i + 3].GetComponent<CardInformations>().CardValue)
//        {
//            currentBoomPartPoints += _currentTurnCardList[i].GetComponent<CardInformations>().CardValue;
//            currentBoomPartPoints += _currentTurnCardList[i + 1].GetComponent<CardInformations>().CardValue;
//            currentBoomPartPoints += _currentTurnCardList[i + 2].GetComponent<CardInformations>().CardValue;
//            currentBoomPartPoints += _currentTurnCardList[i + 3].GetComponent<CardInformations>().CardValue;

//            currentBoomPartPoints = currentBoomPartPoints * 100;
//        }
//    }

//    temp_boom = CurrentTurn_Boom(currentBoomPartPoints);

//    //如果没有满足条件的炸弹就返回
//    if (temp_boom == null)
//    {
//        return null;
//    }

//    for (int i = 0; i < temp_boom.Count; i++)
//    {
//        tempPlayCardList.Add(temp_boom[i]);
//    }

//    //同理
//    if (pairList.Count >= 4)
//    {
//        tempPlayCardList.Add(pairList[0]);
//        tempPlayCardList.Add(pairList[1]);
//        tempPlayCardList.Add(pairList[2]);
//        tempPlayCardList.Add(pairList[3]);

//        pairList.Remove(pairList[3]);
//        pairList.Remove(pairList[2]);
//        pairList.Remove(pairList[1]);
//        pairList.Remove(pairList[0]);
//    }
//    else
//    {
//        if (pairList.Count + (tripleList.Count / 3) * 2 < 2)
//        {
//            return null;
//        }
//        else
//        {
//            //反之，拆三连库
//            for (int i = 0; i < tripleList.Count; i++)
//            {
//                if (pairList.Contains(tripleList[i]) == false)
//                {
//                    pairList.Add(tripleList[i]);
//                    pairList.Add(tripleList[i + 1]);

//                    tripleList.Remove(tripleList[i + 1]);
//                    tripleList.Remove(tripleList[i]);
//                }
//            }

//            //重新升序排列
//            OrderCardsList(pairList);

//            //从小获取
//            tempPlayCardList.Add(pairList[0]);
//            tempPlayCardList.Add(pairList[1]);
//            tempPlayCardList.Add(pairList[2]);
//            tempPlayCardList.Add(pairList[3]);

//            pairList.Remove(pairList[3]);
//            pairList.Remove(pairList[2]);
//            pairList.Remove(pairList[1]);
//            pairList.Remove(pairList[0]);

//            //复原三连库
//            RecoverForStraight_Pair();
//        }
//    }

//    OrderCardsList(boomList);

//    OrderCardsList(tempPlayCardList);

//    return tempPlayCardList;
//}


////当所有类型牌库都判断完之后，若仍没有满足条件的牌时，再判断炸弹库、王炸库
//private List<GameObject> LastCheck_BoomCheck()
//{
//    /// <summary>
//    /// 当玩家手牌少于某个指定值时再使用
//    /// </summary>

//    List<GameObject> tempPlayCardList = new List<GameObject>();

//    if (boomList.Count > 0)
//    {
//        tempPlayCardList.Add(boomList[0]);
//        tempPlayCardList.Add(boomList[1]);
//        tempPlayCardList.Add(boomList[2]);
//        tempPlayCardList.Add(boomList[3]);

//        boomList.Remove(boomList[3]);
//        boomList.Remove(boomList[2]);
//        boomList.Remove(boomList[1]);
//        boomList.Remove(boomList[0]);

//        return tempPlayCardList;
//    }

//    if (jokerBoomList.Count > 0)
//    {
//        tempPlayCardList.Add(jokerBoomList[0]);
//        tempPlayCardList.Add(jokerBoomList[1]);

//        jokerBoomList.Remove(jokerBoomList[1]);
//        jokerBoomList.Remove(jokerBoomList[0]);

//        return tempPlayCardList;
//    }

//    return null;
//}

//#endregion

#endregion

//--------------------------------------------------

#region 之前老旧的，AI先手出牌时，判断连对的方式

//同样，这里也要分类写
//1，10个连对，20张牌
//if (pairList.Count >= 20)
//{
//    for (int i = 0; i < pairList.Count - 18; i += 2)
//    {
//        if (pairList[i].GetComponent<CardInformations>().CardValue - pairList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 1].GetComponent<CardInformations>().CardValue - pairList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 2].GetComponent<CardInformations>().CardValue - pairList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 3].GetComponent<CardInformations>().CardValue - pairList[i + 5].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 4].GetComponent<CardInformations>().CardValue - pairList[i + 6].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 5].GetComponent<CardInformations>().CardValue - pairList[i + 7].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 6].GetComponent<CardInformations>().CardValue - pairList[i + 8].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 7].GetComponent<CardInformations>().CardValue - pairList[i + 9].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 8].GetComponent<CardInformations>().CardValue - pairList[i + 10].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 9].GetComponent<CardInformations>().CardValue - pairList[i + 11].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 10].GetComponent<CardInformations>().CardValue - pairList[i + 12].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 11].GetComponent<CardInformations>().CardValue - pairList[i + 13].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 12].GetComponent<CardInformations>().CardValue - pairList[i + 14].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 13].GetComponent<CardInformations>().CardValue - pairList[i + 15].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 14].GetComponent<CardInformations>().CardValue - pairList[i + 16].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 15].GetComponent<CardInformations>().CardValue - pairList[i + 17].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 16].GetComponent<CardInformations>().CardValue - pairList[i + 18].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 17].GetComponent<CardInformations>().CardValue - pairList[i + 19].GetComponent<CardInformations>().CardValue == -1)
//        {
//            temp_AI_First_Hand.Add(pairList[i]);
//            temp_AI_First_Hand.Add(pairList[i + 1]);
//            temp_AI_First_Hand.Add(pairList[i + 2]);
//            temp_AI_First_Hand.Add(pairList[i + 3]);
//            temp_AI_First_Hand.Add(pairList[i + 4]);
//            temp_AI_First_Hand.Add(pairList[i + 5]);
//            temp_AI_First_Hand.Add(pairList[i + 6]);
//            temp_AI_First_Hand.Add(pairList[i + 7]);
//            temp_AI_First_Hand.Add(pairList[i + 8]);
//            temp_AI_First_Hand.Add(pairList[i + 9]);
//            temp_AI_First_Hand.Add(pairList[i + 10]);
//            temp_AI_First_Hand.Add(pairList[i + 11]);
//            temp_AI_First_Hand.Add(pairList[i + 12]);
//            temp_AI_First_Hand.Add(pairList[i + 13]);
//            temp_AI_First_Hand.Add(pairList[i + 14]);
//            temp_AI_First_Hand.Add(pairList[i + 15]);
//            temp_AI_First_Hand.Add(pairList[i + 16]);
//            temp_AI_First_Hand.Add(pairList[i + 17]);
//            temp_AI_First_Hand.Add(pairList[i + 18]);
//            temp_AI_First_Hand.Add(pairList[i + 19]);

//            //移除添加的对子
//            for (int j = temp_AI_First_Hand.Count - 1; j >= 0; j--)
//            {
//                if (pairList.Contains(temp_AI_First_Hand[j]) == true)
//                {
//                    pairList.Remove(temp_AI_First_Hand[j]);
//                }
//            }

//            return;
//        }
//    }
//}

////2，9个连对，18张牌
//if (pairList.Count >= 18)
//{
//    for (int i = 0; i < pairList.Count - 16; i += 2)
//    {
//        if (pairList[i].GetComponent<CardInformations>().CardValue - pairList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 1].GetComponent<CardInformations>().CardValue - pairList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 2].GetComponent<CardInformations>().CardValue - pairList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 3].GetComponent<CardInformations>().CardValue - pairList[i + 5].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 4].GetComponent<CardInformations>().CardValue - pairList[i + 6].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 5].GetComponent<CardInformations>().CardValue - pairList[i + 7].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 6].GetComponent<CardInformations>().CardValue - pairList[i + 8].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 7].GetComponent<CardInformations>().CardValue - pairList[i + 9].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 8].GetComponent<CardInformations>().CardValue - pairList[i + 10].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 9].GetComponent<CardInformations>().CardValue - pairList[i + 11].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 10].GetComponent<CardInformations>().CardValue - pairList[i + 12].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 11].GetComponent<CardInformations>().CardValue - pairList[i + 13].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 12].GetComponent<CardInformations>().CardValue - pairList[i + 14].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 13].GetComponent<CardInformations>().CardValue - pairList[i + 15].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 14].GetComponent<CardInformations>().CardValue - pairList[i + 16].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 15].GetComponent<CardInformations>().CardValue - pairList[i + 17].GetComponent<CardInformations>().CardValue == -1)
//        {
//            temp_AI_First_Hand.Add(pairList[i]);
//            temp_AI_First_Hand.Add(pairList[i + 1]);
//            temp_AI_First_Hand.Add(pairList[i + 2]);
//            temp_AI_First_Hand.Add(pairList[i + 3]);
//            temp_AI_First_Hand.Add(pairList[i + 4]);
//            temp_AI_First_Hand.Add(pairList[i + 5]);
//            temp_AI_First_Hand.Add(pairList[i + 6]);
//            temp_AI_First_Hand.Add(pairList[i + 7]);
//            temp_AI_First_Hand.Add(pairList[i + 8]);
//            temp_AI_First_Hand.Add(pairList[i + 9]);
//            temp_AI_First_Hand.Add(pairList[i + 10]);
//            temp_AI_First_Hand.Add(pairList[i + 11]);
//            temp_AI_First_Hand.Add(pairList[i + 12]);
//            temp_AI_First_Hand.Add(pairList[i + 13]);
//            temp_AI_First_Hand.Add(pairList[i + 14]);
//            temp_AI_First_Hand.Add(pairList[i + 15]);
//            temp_AI_First_Hand.Add(pairList[i + 16]);
//            temp_AI_First_Hand.Add(pairList[i + 17]);

//            //移除添加的对子
//            for (int j = temp_AI_First_Hand.Count - 1; j >= 0; j--)
//            {
//                if (pairList.Contains(temp_AI_First_Hand[j]) == true)
//                {
//                    pairList.Remove(temp_AI_First_Hand[j]);
//                }
//            }

//            return;
//        }
//    }
//}

////3，8个连对，16张牌
//if (pairList.Count >= 16)
//{
//    for (int i = 0; i < pairList.Count - 14; i += 2)
//    {
//        if (pairList[i].GetComponent<CardInformations>().CardValue - pairList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 1].GetComponent<CardInformations>().CardValue - pairList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 2].GetComponent<CardInformations>().CardValue - pairList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 3].GetComponent<CardInformations>().CardValue - pairList[i + 5].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 4].GetComponent<CardInformations>().CardValue - pairList[i + 6].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 5].GetComponent<CardInformations>().CardValue - pairList[i + 7].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 6].GetComponent<CardInformations>().CardValue - pairList[i + 8].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 7].GetComponent<CardInformations>().CardValue - pairList[i + 9].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 8].GetComponent<CardInformations>().CardValue - pairList[i + 10].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 9].GetComponent<CardInformations>().CardValue - pairList[i + 11].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 10].GetComponent<CardInformations>().CardValue - pairList[i + 12].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 11].GetComponent<CardInformations>().CardValue - pairList[i + 13].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 12].GetComponent<CardInformations>().CardValue - pairList[i + 14].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 13].GetComponent<CardInformations>().CardValue - pairList[i + 15].GetComponent<CardInformations>().CardValue == -1)
//        {
//            temp_AI_First_Hand.Add(pairList[i]);
//            temp_AI_First_Hand.Add(pairList[i + 1]);
//            temp_AI_First_Hand.Add(pairList[i + 2]);
//            temp_AI_First_Hand.Add(pairList[i + 3]);
//            temp_AI_First_Hand.Add(pairList[i + 4]);
//            temp_AI_First_Hand.Add(pairList[i + 5]);
//            temp_AI_First_Hand.Add(pairList[i + 6]);
//            temp_AI_First_Hand.Add(pairList[i + 7]);
//            temp_AI_First_Hand.Add(pairList[i + 8]);
//            temp_AI_First_Hand.Add(pairList[i + 9]);
//            temp_AI_First_Hand.Add(pairList[i + 10]);
//            temp_AI_First_Hand.Add(pairList[i + 11]);
//            temp_AI_First_Hand.Add(pairList[i + 12]);
//            temp_AI_First_Hand.Add(pairList[i + 13]);
//            temp_AI_First_Hand.Add(pairList[i + 14]);
//            temp_AI_First_Hand.Add(pairList[i + 15]);

//            //移除添加的对子
//            for (int j = temp_AI_First_Hand.Count - 1; j >= 0; j--)
//            {
//                if (pairList.Contains(temp_AI_First_Hand[j]) == true)
//                {
//                    pairList.Remove(temp_AI_First_Hand[j]);
//                }
//            }

//            return;
//        }
//    }
//}

////4，7个连对，14张牌
//if (pairList.Count >= 14)
//{
//    for (int i = 0; i < pairList.Count - 12; i += 2)
//    {
//        if (pairList[i].GetComponent<CardInformations>().CardValue - pairList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 1].GetComponent<CardInformations>().CardValue - pairList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 2].GetComponent<CardInformations>().CardValue - pairList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 3].GetComponent<CardInformations>().CardValue - pairList[i + 5].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 4].GetComponent<CardInformations>().CardValue - pairList[i + 6].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 5].GetComponent<CardInformations>().CardValue - pairList[i + 7].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 6].GetComponent<CardInformations>().CardValue - pairList[i + 8].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 7].GetComponent<CardInformations>().CardValue - pairList[i + 9].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 8].GetComponent<CardInformations>().CardValue - pairList[i + 10].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 9].GetComponent<CardInformations>().CardValue - pairList[i + 11].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 10].GetComponent<CardInformations>().CardValue - pairList[i + 12].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 11].GetComponent<CardInformations>().CardValue - pairList[i + 13].GetComponent<CardInformations>().CardValue == -1)
//        {
//            temp_AI_First_Hand.Add(pairList[i]);
//            temp_AI_First_Hand.Add(pairList[i + 1]);
//            temp_AI_First_Hand.Add(pairList[i + 2]);
//            temp_AI_First_Hand.Add(pairList[i + 3]);
//            temp_AI_First_Hand.Add(pairList[i + 4]);
//            temp_AI_First_Hand.Add(pairList[i + 5]);
//            temp_AI_First_Hand.Add(pairList[i + 6]);
//            temp_AI_First_Hand.Add(pairList[i + 7]);
//            temp_AI_First_Hand.Add(pairList[i + 8]);
//            temp_AI_First_Hand.Add(pairList[i + 9]);
//            temp_AI_First_Hand.Add(pairList[i + 10]);
//            temp_AI_First_Hand.Add(pairList[i + 11]);
//            temp_AI_First_Hand.Add(pairList[i + 12]);
//            temp_AI_First_Hand.Add(pairList[i + 13]);

//            //移除添加的对子
//            for (int j = temp_AI_First_Hand.Count - 1; j >= 0; j--)
//            {
//                if (pairList.Contains(temp_AI_First_Hand[j]) == true)
//                {
//                    pairList.Remove(temp_AI_First_Hand[j]);
//                }
//            }

//            return;
//        }
//    }
//}

////5，6个连对，12张牌
//if (pairList.Count >= 12)
//{
//    for (int i = 0; i < pairList.Count - 10; i += 2)
//    {
//        if (pairList[i].GetComponent<CardInformations>().CardValue - pairList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 1].GetComponent<CardInformations>().CardValue - pairList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 2].GetComponent<CardInformations>().CardValue - pairList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 3].GetComponent<CardInformations>().CardValue - pairList[i + 5].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 4].GetComponent<CardInformations>().CardValue - pairList[i + 6].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 5].GetComponent<CardInformations>().CardValue - pairList[i + 7].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 6].GetComponent<CardInformations>().CardValue - pairList[i + 8].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 7].GetComponent<CardInformations>().CardValue - pairList[i + 9].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 8].GetComponent<CardInformations>().CardValue - pairList[i + 10].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 9].GetComponent<CardInformations>().CardValue - pairList[i + 11].GetComponent<CardInformations>().CardValue == -1)
//        {
//            temp_AI_First_Hand.Add(pairList[i]);
//            temp_AI_First_Hand.Add(pairList[i + 1]);
//            temp_AI_First_Hand.Add(pairList[i + 2]);
//            temp_AI_First_Hand.Add(pairList[i + 3]);
//            temp_AI_First_Hand.Add(pairList[i + 4]);
//            temp_AI_First_Hand.Add(pairList[i + 5]);
//            temp_AI_First_Hand.Add(pairList[i + 6]);
//            temp_AI_First_Hand.Add(pairList[i + 7]);
//            temp_AI_First_Hand.Add(pairList[i + 8]);
//            temp_AI_First_Hand.Add(pairList[i + 9]);
//            temp_AI_First_Hand.Add(pairList[i + 10]);
//            temp_AI_First_Hand.Add(pairList[i + 11]);

//            //移除添加的对子
//            for (int j = temp_AI_First_Hand.Count - 1; j >= 0; j--)
//            {
//                if (pairList.Contains(temp_AI_First_Hand[j]) == true)
//                {
//                    pairList.Remove(temp_AI_First_Hand[j]);
//                }
//            }

//            return;
//        }
//    }
//}

////6，5个连对，10张牌
//if (pairList.Count >= 10)
//{
//    for (int i = 0; i < pairList.Count - 8; i += 2)
//    {
//        if (pairList[i].GetComponent<CardInformations>().CardValue - pairList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 1].GetComponent<CardInformations>().CardValue - pairList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 2].GetComponent<CardInformations>().CardValue - pairList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 3].GetComponent<CardInformations>().CardValue - pairList[i + 5].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 4].GetComponent<CardInformations>().CardValue - pairList[i + 6].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 5].GetComponent<CardInformations>().CardValue - pairList[i + 7].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 6].GetComponent<CardInformations>().CardValue - pairList[i + 8].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 7].GetComponent<CardInformations>().CardValue - pairList[i + 9].GetComponent<CardInformations>().CardValue == -1)
//        {
//            temp_AI_First_Hand.Add(pairList[i]);
//            temp_AI_First_Hand.Add(pairList[i + 1]);
//            temp_AI_First_Hand.Add(pairList[i + 2]);
//            temp_AI_First_Hand.Add(pairList[i + 3]);
//            temp_AI_First_Hand.Add(pairList[i + 4]);
//            temp_AI_First_Hand.Add(pairList[i + 5]);
//            temp_AI_First_Hand.Add(pairList[i + 6]);
//            temp_AI_First_Hand.Add(pairList[i + 7]);
//            temp_AI_First_Hand.Add(pairList[i + 8]);
//            temp_AI_First_Hand.Add(pairList[i + 9]);

//            //移除添加的对子
//            for (int j = temp_AI_First_Hand.Count - 1; j >= 0; j--)
//            {
//                if (pairList.Contains(temp_AI_First_Hand[j]) == true)
//                {
//                    pairList.Remove(temp_AI_First_Hand[j]);
//                }
//            }

//            return;
//        }
//    }
//}

////7，4个连对，8张牌
//if (pairList.Count >= 8)
//{
//    for (int i = 0; i < pairList.Count - 6; i += 2)
//    {
//        if (pairList[i].GetComponent<CardInformations>().CardValue - pairList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 1].GetComponent<CardInformations>().CardValue - pairList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 2].GetComponent<CardInformations>().CardValue - pairList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 3].GetComponent<CardInformations>().CardValue - pairList[i + 5].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 4].GetComponent<CardInformations>().CardValue - pairList[i + 6].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 5].GetComponent<CardInformations>().CardValue - pairList[i + 7].GetComponent<CardInformations>().CardValue == -1)
//        {
//            temp_AI_First_Hand.Add(pairList[i]);
//            temp_AI_First_Hand.Add(pairList[i + 1]);
//            temp_AI_First_Hand.Add(pairList[i + 2]);
//            temp_AI_First_Hand.Add(pairList[i + 3]);
//            temp_AI_First_Hand.Add(pairList[i + 4]);
//            temp_AI_First_Hand.Add(pairList[i + 5]);
//            temp_AI_First_Hand.Add(pairList[i + 6]);
//            temp_AI_First_Hand.Add(pairList[i + 7]);

//            //移除添加的对子
//            for (int j = temp_AI_First_Hand.Count - 1; j >= 0; j--)
//            {
//                if (pairList.Contains(temp_AI_First_Hand[j]) == true)
//                {
//                    pairList.Remove(temp_AI_First_Hand[j]);
//                }
//            }

//            return;
//        }
//    }
//}

////8，3个连对，6张牌
//if (pairList.Count >= 6)
//{
//    for (int i = 0; i < pairList.Count - 4; i += 2)
//    {
//        if (pairList[i].GetComponent<CardInformations>().CardValue - pairList[i + 2].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 1].GetComponent<CardInformations>().CardValue - pairList[i + 3].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 2].GetComponent<CardInformations>().CardValue - pairList[i + 4].GetComponent<CardInformations>().CardValue == -1 &&
//            pairList[i + 3].GetComponent<CardInformations>().CardValue - pairList[i + 5].GetComponent<CardInformations>().CardValue == -1)
//        {
//            temp_AI_First_Hand.Add(pairList[i]);
//            temp_AI_First_Hand.Add(pairList[i + 1]);
//            temp_AI_First_Hand.Add(pairList[i + 2]);
//            temp_AI_First_Hand.Add(pairList[i + 3]);
//            temp_AI_First_Hand.Add(pairList[i + 4]);
//            temp_AI_First_Hand.Add(pairList[i + 5]);

//            //移除添加的对子
//            for (int j = temp_AI_First_Hand.Count - 1; j >= 0; j--)
//            {
//                if (pairList.Contains(temp_AI_First_Hand[j]) == true)
//                {
//                    pairList.Remove(temp_AI_First_Hand[j]);
//                }
//            }

//            return;
//        }
//    }
//}

#endregion


#region 之前老旧的，AI先手出牌时，判断顺子的方式

////1，12个顺子
//if (singleList.Count >= 12)
//{
//    for (int i = singleList.Count - 11; i >= 0; i--)
//    {
//        if (singleList[i].GetComponent<CardInformations>().CardValue - singleList[i - 1].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 1].GetComponent<CardInformations>().CardValue - singleList[i - 2].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 2].GetComponent<CardInformations>().CardValue - singleList[i - 3].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 3].GetComponent<CardInformations>().CardValue - singleList[i - 4].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 4].GetComponent<CardInformations>().CardValue - singleList[i - 5].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 5].GetComponent<CardInformations>().CardValue - singleList[i - 6].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 6].GetComponent<CardInformations>().CardValue - singleList[i - 7].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 7].GetComponent<CardInformations>().CardValue - singleList[i - 8].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 8].GetComponent<CardInformations>().CardValue - singleList[i - 9].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 9].GetComponent<CardInformations>().CardValue - singleList[i - 10].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 10].GetComponent<CardInformations>().CardValue - singleList[i - 11].GetComponent<CardInformations>().CardValue == 1)
//        {
//            temp_AI_First_Hand.Add(singleList[i]);
//            temp_AI_First_Hand.Add(singleList[i - 1]);
//            temp_AI_First_Hand.Add(singleList[i - 2]);
//            temp_AI_First_Hand.Add(singleList[i - 3]);
//            temp_AI_First_Hand.Add(singleList[i - 4]);
//            temp_AI_First_Hand.Add(singleList[i - 5]);
//            temp_AI_First_Hand.Add(singleList[i - 6]);
//            temp_AI_First_Hand.Add(singleList[i - 7]);
//            temp_AI_First_Hand.Add(singleList[i - 8]);
//            temp_AI_First_Hand.Add(singleList[i - 9]);
//            temp_AI_First_Hand.Add(singleList[i - 10]);
//            temp_AI_First_Hand.Add(singleList[i - 11]);

//            for (int j = temp_AI_First_Hand.Count - 1; j >= 0; j--)
//            {
//                if (singleList.Contains(temp_AI_First_Hand[j]) == true)
//                {
//                    singleList.Remove(temp_AI_First_Hand[j]);
//                }
//            }

//            return;
//        }
//    }
//}

////2，11个顺子
//if (singleList.Count >= 11)
//{
//    for (int i = singleList.Count - 10; i >= 0; i--)
//    {
//        if (singleList[i].GetComponent<CardInformations>().CardValue - singleList[i - 1].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 1].GetComponent<CardInformations>().CardValue - singleList[i - 2].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 2].GetComponent<CardInformations>().CardValue - singleList[i - 3].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 3].GetComponent<CardInformations>().CardValue - singleList[i - 4].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 4].GetComponent<CardInformations>().CardValue - singleList[i - 5].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 5].GetComponent<CardInformations>().CardValue - singleList[i - 6].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 6].GetComponent<CardInformations>().CardValue - singleList[i - 7].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 7].GetComponent<CardInformations>().CardValue - singleList[i - 8].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 8].GetComponent<CardInformations>().CardValue - singleList[i - 9].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 9].GetComponent<CardInformations>().CardValue - singleList[i - 10].GetComponent<CardInformations>().CardValue == 1)
//        {
//            temp_AI_First_Hand.Add(singleList[i]);
//            temp_AI_First_Hand.Add(singleList[i - 1]);
//            temp_AI_First_Hand.Add(singleList[i - 2]);
//            temp_AI_First_Hand.Add(singleList[i - 3]);
//            temp_AI_First_Hand.Add(singleList[i - 4]);
//            temp_AI_First_Hand.Add(singleList[i - 5]);
//            temp_AI_First_Hand.Add(singleList[i - 6]);
//            temp_AI_First_Hand.Add(singleList[i - 7]);
//            temp_AI_First_Hand.Add(singleList[i - 8]);
//            temp_AI_First_Hand.Add(singleList[i - 9]);
//            temp_AI_First_Hand.Add(singleList[i - 10]);

//            for (int j = temp_AI_First_Hand.Count - 1; j >= 0; j--)
//            {
//                if (singleList.Contains(temp_AI_First_Hand[j]) == true)
//                {
//                    singleList.Remove(temp_AI_First_Hand[j]);
//                }
//            }

//            return;
//        }
//    }
//}

////3，10个顺子
//if (singleList.Count >= 10)
//{
//    for (int i = singleList.Count - 9; i >= 0; i--)
//    {
//        if (singleList[i].GetComponent<CardInformations>().CardValue - singleList[i - 1].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 1].GetComponent<CardInformations>().CardValue - singleList[i - 2].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 2].GetComponent<CardInformations>().CardValue - singleList[i - 3].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 3].GetComponent<CardInformations>().CardValue - singleList[i - 4].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 4].GetComponent<CardInformations>().CardValue - singleList[i - 5].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 5].GetComponent<CardInformations>().CardValue - singleList[i - 6].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 6].GetComponent<CardInformations>().CardValue - singleList[i - 7].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 7].GetComponent<CardInformations>().CardValue - singleList[i - 8].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 8].GetComponent<CardInformations>().CardValue - singleList[i - 9].GetComponent<CardInformations>().CardValue == 1)
//        {
//            temp_AI_First_Hand.Add(singleList[i]);
//            temp_AI_First_Hand.Add(singleList[i - 1]);
//            temp_AI_First_Hand.Add(singleList[i - 2]);
//            temp_AI_First_Hand.Add(singleList[i - 3]);
//            temp_AI_First_Hand.Add(singleList[i - 4]);
//            temp_AI_First_Hand.Add(singleList[i - 5]);
//            temp_AI_First_Hand.Add(singleList[i - 6]);
//            temp_AI_First_Hand.Add(singleList[i - 7]);
//            temp_AI_First_Hand.Add(singleList[i - 8]);
//            temp_AI_First_Hand.Add(singleList[i - 9]);

//            for (int j = temp_AI_First_Hand.Count - 1; j >= 0; j--)
//            {
//                if (singleList.Contains(temp_AI_First_Hand[j]) == true)
//                {
//                    singleList.Remove(temp_AI_First_Hand[j]);
//                }
//            }

//            return;
//        }
//    }
//}

////4，9个顺子
//if (singleList.Count >= 9)
//{
//    for (int i = singleList.Count - 8; i >= 0; i--)
//    {
//        if (singleList[i].GetComponent<CardInformations>().CardValue - singleList[i - 1].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 1].GetComponent<CardInformations>().CardValue - singleList[i - 2].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 2].GetComponent<CardInformations>().CardValue - singleList[i - 3].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 3].GetComponent<CardInformations>().CardValue - singleList[i - 4].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 4].GetComponent<CardInformations>().CardValue - singleList[i - 5].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 5].GetComponent<CardInformations>().CardValue - singleList[i - 6].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 6].GetComponent<CardInformations>().CardValue - singleList[i - 7].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 7].GetComponent<CardInformations>().CardValue - singleList[i - 8].GetComponent<CardInformations>().CardValue == 1)
//        {
//            temp_AI_First_Hand.Add(singleList[i]);
//            temp_AI_First_Hand.Add(singleList[i - 1]);
//            temp_AI_First_Hand.Add(singleList[i - 2]);
//            temp_AI_First_Hand.Add(singleList[i - 3]);
//            temp_AI_First_Hand.Add(singleList[i - 4]);
//            temp_AI_First_Hand.Add(singleList[i - 5]);
//            temp_AI_First_Hand.Add(singleList[i - 6]);
//            temp_AI_First_Hand.Add(singleList[i - 7]);
//            temp_AI_First_Hand.Add(singleList[i - 8]);

//            for (int j = temp_AI_First_Hand.Count - 1; j >= 0; j--)
//            {
//                if (singleList.Contains(temp_AI_First_Hand[j]) == true)
//                {
//                    singleList.Remove(temp_AI_First_Hand[j]);
//                }
//            }

//            return;
//        }
//    }
//}

////5，8个顺子
//if (singleList.Count >= 8)
//{
//    for (int i = singleList.Count - 7; i >= 0; i--)
//    {
//        if (singleList[i].GetComponent<CardInformations>().CardValue - singleList[i - 1].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 1].GetComponent<CardInformations>().CardValue - singleList[i - 2].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 2].GetComponent<CardInformations>().CardValue - singleList[i - 3].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 3].GetComponent<CardInformations>().CardValue - singleList[i - 4].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 4].GetComponent<CardInformations>().CardValue - singleList[i - 5].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 5].GetComponent<CardInformations>().CardValue - singleList[i - 6].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 6].GetComponent<CardInformations>().CardValue - singleList[i - 7].GetComponent<CardInformations>().CardValue == 1)
//        {
//            temp_AI_First_Hand.Add(singleList[i]);
//            temp_AI_First_Hand.Add(singleList[i - 1]);
//            temp_AI_First_Hand.Add(singleList[i - 2]);
//            temp_AI_First_Hand.Add(singleList[i - 3]);
//            temp_AI_First_Hand.Add(singleList[i - 4]);
//            temp_AI_First_Hand.Add(singleList[i - 5]);
//            temp_AI_First_Hand.Add(singleList[i - 6]);
//            temp_AI_First_Hand.Add(singleList[i - 7]);

//            for (int j = temp_AI_First_Hand.Count - 1; j >= 0; j--)
//            {
//                if (singleList.Contains(temp_AI_First_Hand[j]) == true)
//                {
//                    singleList.Remove(temp_AI_First_Hand[j]);
//                }
//            }

//            return;
//        }
//    }
//}

////6，7个顺子
//if (singleList.Count >= 7)
//{
//    for (int i = singleList.Count - 5 - 1; i >= 0; i--)
//    {
//        if (singleList[i].GetComponent<CardInformations>().CardValue - singleList[i - 1].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 1].GetComponent<CardInformations>().CardValue - singleList[i - 2].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 2].GetComponent<CardInformations>().CardValue - singleList[i - 3].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 3].GetComponent<CardInformations>().CardValue - singleList[i - 4].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 4].GetComponent<CardInformations>().CardValue - singleList[i - 5].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 5].GetComponent<CardInformations>().CardValue - singleList[i - 6].GetComponent<CardInformations>().CardValue == 1)
//        {
//            temp_AI_First_Hand.Add(singleList[i]);
//            temp_AI_First_Hand.Add(singleList[i - 1]);
//            temp_AI_First_Hand.Add(singleList[i - 2]);
//            temp_AI_First_Hand.Add(singleList[i - 3]);
//            temp_AI_First_Hand.Add(singleList[i - 4]);
//            temp_AI_First_Hand.Add(singleList[i - 5]);
//            temp_AI_First_Hand.Add(singleList[i - 6]);

//            for (int j = temp_AI_First_Hand.Count - 1; j >= 0; j--)
//            {
//                if (singleList.Contains(temp_AI_First_Hand[j]) == true)
//                {
//                    singleList.Remove(temp_AI_First_Hand[j]);
//                }
//            }

//            return;
//        }
//    }
//}

////7，6个顺子
//if (singleList.Count >= 6)
//{
//    for (int i = singleList.Count - 4 - 1; i >= 0; i--)
//    {
//        if (singleList[i].GetComponent<CardInformations>().CardValue - singleList[i - 1].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 1].GetComponent<CardInformations>().CardValue - singleList[i - 2].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 2].GetComponent<CardInformations>().CardValue - singleList[i - 3].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 3].GetComponent<CardInformations>().CardValue - singleList[i - 4].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 4].GetComponent<CardInformations>().CardValue - singleList[i - 5].GetComponent<CardInformations>().CardValue == 1)
//        {
//            temp_AI_First_Hand.Add(singleList[i]);
//            temp_AI_First_Hand.Add(singleList[i - 1]);
//            temp_AI_First_Hand.Add(singleList[i - 2]);
//            temp_AI_First_Hand.Add(singleList[i - 3]);
//            temp_AI_First_Hand.Add(singleList[i - 4]);
//            temp_AI_First_Hand.Add(singleList[i - 5]);

//            for (int j = temp_AI_First_Hand.Count - 1; j >= 0; j--)
//            {
//                if (singleList.Contains(temp_AI_First_Hand[j]) == true)
//                {
//                    singleList.Remove(temp_AI_First_Hand[j]);
//                }
//            }

//            return;
//        }
//    }
//}

////8，5个顺子
//if (singleList.Count >= 5)
//{
//    for (int i = singleList.Count - 3 - 1; i >= 0; i--)
//    {
//        if (singleList[i].GetComponent<CardInformations>().CardValue - singleList[i - 1].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 1].GetComponent<CardInformations>().CardValue - singleList[i - 2].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 2].GetComponent<CardInformations>().CardValue - singleList[i - 3].GetComponent<CardInformations>().CardValue == 1 &&
//            singleList[i - 3].GetComponent<CardInformations>().CardValue - singleList[i - 4].GetComponent<CardInformations>().CardValue == 1)
//        {
//            temp_AI_First_Hand.Add(singleList[i]);
//            temp_AI_First_Hand.Add(singleList[i - 1]);
//            temp_AI_First_Hand.Add(singleList[i - 2]);
//            temp_AI_First_Hand.Add(singleList[i - 3]);
//            temp_AI_First_Hand.Add(singleList[i - 4]);

//            for (int j = temp_AI_First_Hand.Count - 1; j >= 0; j--)
//            {
//                if (singleList.Contains(temp_AI_First_Hand[j]) == true)
//                {
//                    singleList.Remove(temp_AI_First_Hand[j]);
//                }
//            }

//            return;
//        }
//    }
//}

#endregion