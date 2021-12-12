using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using PIXEL.Landlords.FrameWork;
using PIXEL.Landlords.Card;
using UnityEngine.UI;
using PIXEL.Landlords.Audio;
using PIXEL.Landlords.Animation.SpecialCardAnimations;

namespace PIXEL.Landlords.Game
{
    public class PlayCardManager : SingletonPattern<PlayCardManager>
    {
        [Header("玩家出牌List"),Tooltip("显示玩家当前回合出的牌")]
        public List<GameObject> playerPlayCardList = new List<GameObject>();
        public PlayCardType currentPlayerCardType;
        private int currentTurnPoints;

        [Header("AI出牌List"), Tooltip("显示AI当前回合出的牌")]
        public List<GameObject> aiNo1PlayCardList = new List<GameObject>();
        public List<GameObject> aiNo2PlayCardList = new List<GameObject>();

        [Header("各个角色出牌的桌子")]
        private Transform playerTable;
        private Transform aiNo1Table;
        private Transform aiNo2Table;

        [Header("提示界面")]
        private GameObject playerTipPanel;
        private GameObject aiNo1TipPanel;
        private GameObject aiNo2TipPanel;

        [Header("按钮")]
        private Button button_Play;
        private Button button_GiveUp;

        [Header("计时器")]
        private Image clock_Player;
        [HideInInspector] public Image clock_AINo1;
        [HideInInspector] public Image clock_AINo2;

        [Header("计时器计时数字")]
        private GameObject number_9;
        private GameObject number_8;
        private GameObject number_7;
        private GameObject number_6;
        private GameObject number_5;
        private GameObject number_4;
        private GameObject number_3;
        private GameObject number_2;
        private GameObject number_1;
        private GameObject number_0;

        [Header("计时器计时时间")]
        public float clockTime = 20f;
        private string currentTime;
        private string targetTime;
        public bool isStart = false;

        IEnumerator countDownNumerator;

        [Header("音效播放器")]
        private AudioSource playerAudioSource;

        [Header("卡牌回收父物体")]
        private GameObject cardsRecycleBin;
        private void Start()
        {
            //获取玩家音效播放器
            playerAudioSource = DealCardManager.Instance.playerHand.gameObject.GetComponent<AudioSource>();

            //获取角色出牌桌
            playerTable = GameObject.Find("Table_player").transform;
            aiNo1Table = GameObject.Find("Table_aiNo1").transform;
            aiNo2Table = GameObject.Find("Table_aiNo2").transform;

            //获取角色对应提示panel
            playerTipPanel = GameObject.Find("Tip_Player");
            aiNo1TipPanel = GameObject.Find("Tip_AINo1");
            aiNo2TipPanel = GameObject.Find("Tip_AINo2");

            //获取卡牌回收父物体
            cardsRecycleBin = GameObject.Find("CardsRecycleBin");

            //获取按钮
            button_Play = playerTipPanel.transform.GetChild(0).GetComponent<Button>();
            button_GiveUp = playerTipPanel.transform.GetChild(1).GetComponent<Button>();

            //获取倒计时器
            clock_Player = playerTipPanel.transform.GetChild(2).GetComponent<Image>();
            clock_AINo1 = aiNo1TipPanel.transform.GetChild(2).GetComponent<Image>();
            clock_AINo2 = aiNo2TipPanel.transform.GetChild(2).GetComponent<Image>();

            //隐藏计时器
            clock_Player.gameObject.SetActive(!clock_Player.gameObject.activeSelf);
            clock_AINo1.gameObject.SetActive(!clock_AINo1.gameObject.activeSelf);
            clock_AINo2.gameObject.SetActive(!clock_AINo2.gameObject.activeSelf);

            //只为玩家的按钮添加功能
            button_Play.onClick.AddListener(() => PlayerPlay());
            button_GiveUp.onClick.AddListener(() => PlayerGiveUpPlayCard());

            //隐藏提示panel
            playerTipPanel.SetActive(!playerTipPanel.activeSelf);
            aiNo1TipPanel.SetActive(!aiNo1TipPanel.activeSelf);
            aiNo2TipPanel.SetActive(!aiNo2TipPanel.activeSelf);

            string path = "Landlords_Clock/clock_";

            //获取倒计时计时器数字Assets
            number_9 = Resources.Load(path + 9.ToString()) as GameObject;
            number_8 = Resources.Load(path + 8.ToString()) as GameObject;
            number_7 = Resources.Load(path + 7.ToString()) as GameObject;
            number_6 = Resources.Load(path + 6.ToString()) as GameObject;
            number_5 = Resources.Load(path + 5.ToString()) as GameObject;
            number_4 = Resources.Load(path + 4.ToString()) as GameObject;
            number_3 = Resources.Load(path + 3.ToString()) as GameObject;
            number_2 = Resources.Load(path + 2.ToString()) as GameObject;
            number_1 = Resources.Load(path + 1.ToString()) as GameObject;
            number_0 = Resources.Load(path + 0.ToString()) as GameObject;

            //销毁卡牌回收父物体的所有卡牌
            for (int i = 0; i < cardsRecycleBin.transform.childCount; i++)
            {
                Destroy(cardsRecycleBin.transform.GetChild(i).gameObject);
            }
        }

        private void Update()
        {
            currentPlayerCardType = PlayCardTypeJudgmentManager.PlayCardTypeJudge(playerPlayCardList);

            playerTipPanel.SetActive(RoundJudgmentManager.Instance.isPlayer);

            aiNo1TipPanel.SetActive(RoundJudgmentManager.Instance.isAiNo1);

            aiNo2TipPanel.SetActive(RoundJudgmentManager.Instance.isAiNo2);
        }

        #region Player
        //玩家出牌
        private void PlayerPlay() 
        {
            //如果玩家当前回合出牌列表没有牌，则不出
            if (playerPlayCardList.Count == 0 || playerPlayCardList == null)
            {
                return;
            }

            //判断当前玩家出牌桌上是否有牌，若有则将它们清理掉
            if (playerTable.childCount != 0)
            {
                for (int i = playerTable.childCount - 1; i >= 0 ; i--)
                {
                    CardRecycle(playerTable.GetChild(i).gameObject, cardsRecycleBin);
                }
            }

            if (RoundJudgmentManager.Instance.isPlayer == true)
            {
                if (RoundJudgmentManager.Instance.transform_AiNo1.childCount == 0 && RoundJudgmentManager.Instance.transform_AiNo2.childCount == 0)
                {
                    for (int i = RoundJudgmentManager.Instance.currentTurnCardList.Count - 1; i >= 0; i--)
                    {
                        RoundJudgmentManager.Instance.currentTurnCardList.Remove(RoundJudgmentManager.Instance.currentTurnCardList[i]);
                    }

                    RoundJudgmentManager.Instance.currentTurnCardList.Clear();

                    RoundJudgmentManager.Instance.currentCardCounts = 0;
                    RoundJudgmentManager.Instance.currentRoundCardPoints = 0;
                    RoundJudgmentManager.Instance.currentRoundCardType = PlayCardType.None;
                }
            }

            ////如果AI1和AI2的出牌桌上都没有牌，就说明这回合是玩家先手，所以玩家直接出牌
            if (aiNo1Table.childCount == 0 && aiNo2Table.childCount == 0)
            {
                //如果玩家出牌不符合出牌规则，则退回
                if (PlayCardTypeJudgmentManager.PlayCardTypeJudge(playerPlayCardList) == PlayCardType.None)
                {
                    for (int i = 0; i < playerPlayCardList.Count; i++)
                    {
                        playerPlayCardList[i].GetComponent<CardControl>().isSelected = false;
                        playerPlayCardList[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(playerPlayCardList[i].GetComponent<RectTransform>().anchoredPosition.x, -96f);
                    }

                    playerPlayCardList.Clear();

                    countDownNumerator = CountDown(clock_Player, clockTime);
                    StopCoroutine(countDownNumerator);
                    ReSetCountDown();

                    return;
                }

                for (int i = 0; i < playerPlayCardList.Count; i++)
                {
                    playerPlayCardList[i].transform.SetParent(playerTable);
                    playerPlayCardList[i].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                    //并将玩家出牌的相应手牌移除
                    DealCardManager.Instance.playerHandCards.Remove(playerPlayCardList[i]);
                }

                AudioManager.PlayCard(playerAudioSource);
                AudioManager.ManPlayCard(playerAudioSource, playerPlayCardList);
                DetectLastOneCard();
                SpecialCardCombinationsAnimations.Instance.ShowAnimations("Player", PlayCardTypeJudgmentManager.PlayCardTypeJudge(playerPlayCardList));

                RoundJudgmentManager.Instance.GetCurrentTrunCardInfos_For_Player(playerPlayCardList);

                //清理当前回合出牌列表
                playerPlayCardList.Clear();

                //过渡玩家回合
                Invoke("TurnChange", 0.3f);

                countDownNumerator = CountDown(clock_Player, clockTime);
                StopCoroutine(countDownNumerator);
                ReSetCountDown();

                return;
            }

            //如果满足出牌条件，则出牌
            if (RoundJudgmentManager.Instance.JudgementCurrentPoints(playerPlayCardList) == true)
            {
                for (int i = 0; i < playerPlayCardList.Count; i++)
                {
                    playerPlayCardList[i].transform.SetParent(playerTable);
                    playerPlayCardList[i].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                    //并将玩家出牌的相应手牌移除
                    DealCardManager.Instance.playerHandCards.Remove(playerPlayCardList[i]);
                }

                AudioManager.PlayCard(playerAudioSource);
                AudioManager.ManPlayCard(playerAudioSource, playerPlayCardList);
                DetectLastOneCard();
                SpecialCardCombinationsAnimations.Instance.ShowAnimations("Player", PlayCardTypeJudgmentManager.PlayCardTypeJudge(playerPlayCardList));

                RoundJudgmentManager.Instance.GetCurrentTrunCardInfos_For_Player(playerPlayCardList);

                //清理当前回合出牌列表
                playerPlayCardList.Clear();

                //过渡玩家回合
                Invoke("TurnChange", 0.3f);

                countDownNumerator = CountDown(clock_Player, clockTime);
                StopCoroutine(countDownNumerator);
                ReSetCountDown();

                return;
            }
            else
            {
                for (int i = 0; i < playerPlayCardList.Count; i++)
                {
                    playerPlayCardList[i].GetComponent<CardControl>().isSelected = false;
                    playerPlayCardList[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(playerPlayCardList[i].GetComponent<RectTransform>().anchoredPosition.x, -96f);
                }

                playerPlayCardList.Clear();

                return;
            }
        }

        //玩家放弃出牌
        private void PlayerGiveUpPlayCard()
        {
            //如果AI1和AI2的出牌桌上都没有牌，就说明这回合是玩家先手，玩家必须出牌
            if (aiNo1Table.childCount == 0 && aiNo2Table.childCount == 0)
            {
                return;
            }

            if (playerTable.childCount != 0)
            {
                for (int i = playerTable.childCount - 1; i >= 0; i--)
                {
                    CardRecycle(playerTable.GetChild(i).gameObject, cardsRecycleBin);
                }
            }

            AudioManager.ManGiveUp(playerAudioSource);

            //清理当前回合出牌列表
            playerPlayCardList.Clear();

            //过渡玩家回合
            Invoke("TurnChange", 0.3f);

            return;
        }

        //判断是否是最后一张牌
        private void DetectLastOneCard() 
        {
            if (DealCardManager.Instance.playerHandCards.Count == 1)
            {
                AudioManager.ChooseCard(playerAudioSource);
            }
        }
        #endregion

        #region AI

        //AI出牌，作用获取当前AI的出牌信息
        public void AIPlayCard(int _AINumber,List<GameObject> _AIPlayCardList) 
        {
            switch (_AINumber)
            {
                case 1:
                    AIPlayeCard(_AIPlayCardList, aiNo1Table,"AI_1");
                    break;
                case 2:
                    AIPlayeCard(_AIPlayCardList, aiNo2Table, "AI_2");
                    break;
            }
        }

        //重写AI出牌，作用将当前AI出的牌，打出
        private void AIPlayeCard(List<GameObject> _AIPlayCardList,Transform _AITable,string _currentAI) 
        {
            //判断当前AI出牌桌上是否有牌，若有则将它们清理掉
            if (_AITable.childCount != 0)
            {
                for (int i = _AITable.childCount - 1; i >= 0 ; i--)
                {
                    CardRecycle(_AITable.GetChild(i).gameObject, cardsRecycleBin);
                }
            }

            //出牌
            for (int i = 0; i < _AIPlayCardList.Count; i++)
            {
                _AIPlayCardList[i].GetComponent<Image>().sprite = _AIPlayCardList[i].GetComponent<CardInformations>().CardInitialSprite;
                _AIPlayCardList[i].transform.SetParent(_AITable);
                _AIPlayCardList[i].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }

            SpecialCardCombinationsAnimations.Instance.ShowAnimations(_currentAI, PlayCardTypeJudgmentManager.PlayCardTypeJudge(_AIPlayCardList));

            RoundJudgmentManager.Instance.GetCurrentTrunCardInfos_For_AI(_AIPlayCardList);
        }

        //AI放弃不出牌
        public void AIGiveUpPlayCard(int _AINumber) 
        {
            switch (_AINumber)
            {
                case 1:
                    AIGivePlayCard(aiNo1Table);
                    break;
                case 2:
                    AIGivePlayCard(aiNo2Table);
                    break;
            }
        }

        //重写AI放弃不出牌
        private void AIGivePlayCard(Transform _AITable) 
        {
            if (_AITable.childCount != 0)
            {
                for (int i = _AITable.childCount - 1; i >= 0 ; i--)
                {
                    CardRecycle(_AITable.GetChild(i).gameObject, cardsRecycleBin);
                }
            }
        }

        #endregion

        //回合过渡
        private void TurnChange() 
        {
            RoundJudgmentManager.Instance.isPlayer = !RoundJudgmentManager.Instance.isPlayer;
            RoundJudgmentManager.Instance.isAiNo1 = !RoundJudgmentManager.Instance.isAiNo1;
        }

        //卡牌回收
        private void CardRecycle(GameObject _currentCard,GameObject _cardRecycleBin) 
        {
            _currentCard.SetActive(false);
            _currentCard.transform.SetParent(_cardRecycleBin.transform);
            _currentCard.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

        /// <summary>
        /// 测试倒计时功能
        /// </summary>
        /// <param name="_targetClock"></param>
        /// <param name="_countDownTime"></param>
        /// <returns></returns>
        //出牌倒计时
        public IEnumerator CountDown(Image _targetClock,float _countDownTime)
        {
            while (_countDownTime > 0)
            {
                yield return new WaitForSeconds(1f);
                _countDownTime--;
                clockTime = _countDownTime;
                Debug.Log(clockTime);

                switch (_countDownTime)
                {
                    case 9:
                        _targetClock.transform.GetChild(0).GetComponent<Image>().sprite = number_9.GetComponent<SpriteRenderer>().sprite;
                        break;
                    case 8:
                        _targetClock.transform.GetChild(0).GetComponent<Image>().sprite = number_8.GetComponent<SpriteRenderer>().sprite;
                        break;
                    case 7:
                        _targetClock.transform.GetChild(0).GetComponent<Image>().sprite = number_7.GetComponent<SpriteRenderer>().sprite;
                        break;
                    case 6:
                        _targetClock.transform.GetChild(0).GetComponent<Image>().sprite = number_6.GetComponent<SpriteRenderer>().sprite;
                        break;
                    case 5:
                        _targetClock.transform.GetChild(0).GetComponent<Image>().sprite = number_5.GetComponent<SpriteRenderer>().sprite;
                        break;
                    case 4:
                        _targetClock.transform.GetChild(0).GetComponent<Image>().sprite = number_4.GetComponent<SpriteRenderer>().sprite;
                        break;
                    case 3:
                        _targetClock.transform.GetChild(0).GetComponent<Image>().sprite = number_3.GetComponent<SpriteRenderer>().sprite;
                        break;
                    case 2:
                        _targetClock.transform.GetChild(0).GetComponent<Image>().sprite = number_2.GetComponent<SpriteRenderer>().sprite;
                        break;
                    case 1:
                        _targetClock.transform.GetChild(0).GetComponent<Image>().sprite = number_1.GetComponent<SpriteRenderer>().sprite;
                        break;
                    case 0:
                        _targetClock.transform.GetChild(0).GetComponent<Image>().sprite = number_0.GetComponent<SpriteRenderer>().sprite;
                        break;
                }
            }        
        }

        //重置倒计时
        public void ReSetCountDown() 
        {
            clockTime = 20;
            isStart = false;
        }

        public void ExecuteCountDown(string _ai,float _countDown) 
        {
            if (_ai == "Table_aiNo1")
            {
                if (isStart == false)
                {
                    Debug.Log("AI---1");
                    clock_AINo1.gameObject.SetActive(true);
                    StartCoroutine(CountDown(clock_AINo1, _countDown));
                    isStart = true;
                }
            }

            if (_ai == "Table_aiNo2")
            {
                if (isStart == false)
                {
                    Debug.Log("AI---2");
                    clock_AINo2.gameObject.SetActive(true);
                    StartCoroutine(CountDown(clock_AINo2, _countDown));
                    isStart = true;
                }
            }
        }
    }
}

#region No Use test 1

////玩家出牌
//private void PlayerPlayCard(List<GameObject> _playerPlayCard)
//{
//    //判断当前玩家出牌桌上是否有牌，若有则将它们清理掉
//    if (playerTable.childCount != 0)
//    {
//        for (int i = 0; i < playerTable.childCount; i++)
//        {
//            Destroy(playerTable.GetChild(i).gameObject);
//        }
//    }

//    RoundJudgmentManager.Instance.GetCurrentTrunCardInfos(_playerPlayCard);

//    //满足出牌条件后，出牌
//    for (int i = 0; i < playerPlayCardList.Count; i++)
//    {
//        playerPlayCardList[i].transform.SetParent(playerTable);
//        playerPlayCardList[i].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

//        DealCardManager.Instance.playerHandCards.Remove(playerPlayCardList[i]);
//    }

//    //清理当前回合出牌列表
//    playerPlayCardList.Clear();

//    RoundJudgmentManager.Instance.isPlayer = !RoundJudgmentManager.Instance.isPlayer;
//    RoundJudgmentManager.Instance.isAiNo1 = !RoundJudgmentManager.Instance.isAiNo1;
//}

#endregion