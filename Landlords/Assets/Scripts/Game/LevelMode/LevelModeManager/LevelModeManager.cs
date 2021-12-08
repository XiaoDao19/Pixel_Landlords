using PIXEL.Landlords.Card;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PIXEL.Landlords.FrameWork;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PIXEL.Landlords.UI;

namespace PIXEL.Landlords.Game.LevelMode
{

    public class LevelModeManager : SingletonPattern<LevelModeManager>
    {
        [Header("关卡Excel表路径")]
        private string levelExcelTablePath;

        [Header("当前读取卡牌布局信息")]
        private LevelInformations temporaryInformations;

        [Header("各个角色当前关卡手牌")]
        public List<GameObject> playerCardGit = new List<GameObject>();
        public List<GameObject> ai1CardGit = new List<GameObject>();
        public List<GameObject> ai2CardGit = new List<GameObject>();

        [Header("LevelModePanel")]
        private GameObject levelModePanel;
        private GameObject WinPanel;
        private Text titleText;
        private Button button_Next;
        public int levelNumber = 1;

        [Header("UIAnimations")]
        private static GameObject transitionPanel_First;
        private static GameObject transitionPanel_Second;
        private static GameObject transitionPanel_Third;

        private bool levelIsUp;
        private void Start()
        {
            levelExcelTablePath = Application.streamingAssetsPath + "/Lanlords_Level" + ".xlsx";//一定要加后缀.xlsx，因为他只能读取这个格式的Excel文件

            PlayerPrefs.SetString("SceneName", SceneManager.GetActiveScene().name);
            transitionPanel_First = GameObject.Find("UIAnimation_First");
            transitionPanel_Second = GameObject.Find("UIAnimation_Second");
            transitionPanel_Third = GameObject.Find("UIAnimation_Third");

            levelModePanel = gameObject;
            WinPanel = levelModePanel.transform.GetChild(0).gameObject;
            titleText = WinPanel.transform.GetChild(0).GetComponent<Text>();
            button_Next = WinPanel.transform.GetChild(1).GetComponent<Button>();

            button_Next.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                WinPanel.SetActive(!WinPanel.activeSelf);
                //RoundJudgmentManager.Instance.isPlayer = true;
                //RoundJudgmentManager.Instance.isAiNo1 = false;
                //RoundJudgmentManager.Instance.isAiNo2 = false;
                //RoundJudgmentManager.Instance.currentCardCounts = 0;
                //RoundJudgmentManager.Instance.currentRoundCardPoints = 0;
                //RoundJudgmentManager.Instance.currentRoundCardType = PlayCardType.None;

                //for (int i = RoundJudgmentManager.Instance.currentTurnCardList.Count - 1; i >= 0; i--)
                //{
                //    RoundJudgmentManager.Instance.currentTurnCardList.Remove(RoundJudgmentManager.Instance.currentTurnCardList[i]);
                //}
            });

            UIAnimations.SceneTransition_In(transitionPanel_First, transitionPanel_Second, transitionPanel_Third);

            if (PlayerPrefs.HasKey("LevelNumber") == true)
            {
                levelNumber = PlayerPrefs.GetInt("LevelNumber");
                ReadLayOutInformations(PlayerPrefs.GetInt("LevelNumber"));
            }
            //else
            //{
            //    PlayerPrefs.SetInt("LevelNumber", levelNumber);
            //    ReadLayOutInformations(PlayerPrefs.GetInt("LevelNumber"));
            //    levelNumber++;//just test
            //    PlayerPrefs.SetInt("LevelNumber", levelNumber);
            //}

            levelIsUp = false;
        }

        private void Update()
        {
            if (DealCardManager.Instance.playerHand.childCount == 0)
            {
                Time.timeScale = 0;

                if (levelIsUp == false)
                {
                    levelNumber++;//just test
                    PlayerPrefs.SetInt("LevelNumber", levelNumber);

                    levelIsUp = true;
                }

                WinPanel.SetActive(true);
            }
            else
            {
                Time.timeScale = 1;
                WinPanel.SetActive(false);
            }
        }

        //读取关卡信息，并生成指定卡牌，再将其发给指定角色
        public void ReadLayOutInformations(int _levelNumber)
        {
            //获取指定关卡的信息
            temporaryInformations = ReadExcelFile.ReadLevelExcel(levelExcelTablePath, _levelNumber);

            int levelNumber = (int)temporaryInformations.LevelId;

            string[] playerCards = temporaryInformations.PlayerCards.Split('|');
            string[] AINo1Cards = temporaryInformations.AINo1Cards.Split('|');
            string[] AINo2Cards = temporaryInformations.AINo2Cards.Split('|');

            CreateCard(playerCards, playerCardGit);
            CreateCard(AINo1Cards, ai1CardGit);
            CreateCard(AINo2Cards, ai2CardGit);

            for (int i = 0; i < playerCardGit.Count; i++)
            {
                DealCardManager.Instance.playerHandCards.Add(playerCardGit[i]);
            }

            for (int i = 0; i < ai1CardGit.Count; i++)
            {
                DealCardManager.Instance.aiNo1HandCards.Add(ai1CardGit[i]);
            }

            for (int i = 0; i < ai2CardGit.Count; i++)
            {
                DealCardManager.Instance.aiNo2HandCards.Add(ai2CardGit[i]);
            }

            DealCardManager.Instance.OrderTheCharacterHandCards(DealCardManager.Instance.playerHandCards, DealCardManager.Instance.playerHand);
            DealCardManager.Instance.OrderTheCharacterHandCards(DealCardManager.Instance.aiNo1HandCards, DealCardManager.Instance.aiNo1Hand);
            DealCardManager.Instance.OrderTheCharacterHandCards(DealCardManager.Instance.aiNo2HandCards, DealCardManager.Instance.aiNo2Hand);
        }

        //获取对应牌
        private void CreateCard(string[] _target, List<GameObject> _targetGit)
        {
            for (int i = 0; i < _target.Length; i++)
            {
                if (_target[i].IndexOf('1') == 0)
                {
                    for (int j = 0; j < DealCardManager.Instance.hongTaoCardGit.Count; j++)
                    {
                        if (_target[i] == DealCardManager.Instance.hongTaoCardGit[j].GetComponent<CardInformations>().IndexNumber.ToString())
                        {
                            _targetGit.Add(DealCardManager.Instance.hongTaoCardGit[j]);
                        }
                    }
                }

                if (_target[i].IndexOf('2') == 0)
                {
                    for (int j = 0; j < DealCardManager.Instance.fangKuaiCardGit.Count; j++)
                    {
                        if (_target[i] == DealCardManager.Instance.fangKuaiCardGit[j].GetComponent<CardInformations>().IndexNumber.ToString())
                        {
                            _targetGit.Add(DealCardManager.Instance.fangKuaiCardGit[j]);
                        }
                    }
                }

                if (_target[i].IndexOf('3') == 0)
                {
                    for (int j = 0; j < DealCardManager.Instance.heiTaoCardGit.Count; j++)
                    {
                        if (_target[i] == DealCardManager.Instance.heiTaoCardGit[j].GetComponent<CardInformations>().IndexNumber.ToString())
                        {
                            _targetGit.Add(DealCardManager.Instance.heiTaoCardGit[j]);
                        }
                    }
                }

                if (_target[i].IndexOf('4') == 0)
                {
                    for (int j = 0; j < DealCardManager.Instance.meiHuaCardGit.Count; j++)
                    {
                        if (_target[i] == DealCardManager.Instance.meiHuaCardGit[j].GetComponent<CardInformations>().IndexNumber.ToString())
                        {
                            _targetGit.Add(DealCardManager.Instance.meiHuaCardGit[j]);
                        }
                    }
                }

                if (_target[i].IndexOf('5') == 0)
                {
                    for (int j = 0; j < DealCardManager.Instance.jokerCardGit.Count; j++)
                    {
                        if (_target[i] == DealCardManager.Instance.jokerCardGit[j].GetComponent<CardInformations>().IndexNumber.ToString())
                        {
                            _targetGit.Add(DealCardManager.Instance.jokerCardGit[j]);
                        }
                    }
                }
            }
        }
    }
}