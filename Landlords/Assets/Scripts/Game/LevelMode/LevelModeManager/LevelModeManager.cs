using PIXEL.Landlords.Card;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PIXEL.Landlords.FrameWork;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PIXEL.Landlords.UI;
using System.IO;

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
        public static int levelNumber = 1;

        [Header("UIAnimations")]
        private static GameObject transitionPanel_First;
        private static GameObject transitionPanel_Second;
        private static GameObject transitionPanel_Third;

        private bool levelIsUp;
        private void Start()
        {
            levelExcelTablePath = Application.streamingAssetsPath + "/ExcelFiles" + "/Landlords_Level" + ".xlsx";//一定要加后缀.xlsx，因为他只能读取这个格式的Excel文件

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
            });

            UIAnimations.SceneTransition_In(transitionPanel_First, transitionPanel_Second, transitionPanel_Third);
          
            if (levelNumber != 0)
            {
                //第一种加载方式，读取Excel表
                //ReadLayOutInformations(PlayerPrefs.GetInt("LevelNumber"));

                //第二种加载方式，读取TXT文件
                ReadLayOutInfoamationsFromTXT(levelNumber);
            }

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
            string[] aINo1Cards = temporaryInformations.AINo1Cards.Split('|');
            string[] aINo2Cards = temporaryInformations.AINo2Cards.Split('|');

            CreateCard(playerCards, playerCardGit);
            CreateCard(aINo1Cards, ai1CardGit);
            CreateCard(aINo2Cards, ai2CardGit);

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

        //2021-12-10，最新加载关卡信息方法。（TXT版）
        private void ReadLayOutInfoamationsFromTXT(int _levelNumber) 
        {
            //关卡表路径
            string loadPath = Application.streamingAssetsPath + "/TXTFiles" + "/LandlordsLevelSets" + ".txt";

            //获取关卡表信息
            string levelInformaitons = File.ReadAllText(loadPath);

            //获取所有关卡（根据关卡表设置的符号来切割）
            string[] totalLevel = levelInformaitons.Split('/');

            //当前关卡
            string currentLevel = "";

            //当前关卡等于关卡编号-1
            currentLevel = totalLevel[_levelNumber - 1];

            //for (int i = 0; i < totalLevel.Length; i++)
            //{
            //    string[] temp = totalLevel[i].Split(':');

            //    if (temp[0] == _levelNumber.ToString())
            //    {
            //        currentLevel = totalLevel[i];
            //    }
            //}

            //获取当前关卡牌信息
            string[] currentLevelCards = currentLevel.Split(':');

            //获取当前关卡所有牌
            string[] totalCards = currentLevelCards[1].Split('|');

            //分别获取每个角色的牌信息
            string[] playerCards = totalCards[0].Split(',');
            string[] aiNo1Cards = totalCards[1].Split(',');
            string[] aiNo2Cards = totalCards[2].Split(',');

            CreateCard(playerCards, playerCardGit);
            CreateCard(aiNo1Cards, ai1CardGit);
            CreateCard(aiNo2Cards, ai2CardGit);

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
    }
}


//先从excel文件中读取当前关卡的卡牌信息，然后将其写入一个txt文件，然后再读取这个txt文件，根据信息发牌
//但是还是不行，因为打包导出之后就读取不了excel文件
#region New Way To Laod Excel

//write in TxT file
//public void WriteLayOutInfomationsFromTXT(int _levelNumber)
//{
//    temporaryInformations = ReadExcelFile.ReadLevelExcel(levelExcelTablePath, _levelNumber);

//    string path = Application.streamingAssetsPath + "/LandlordsLevels/Level_" + _levelNumber + ".txt";

//    StreamWriter sw;
//    FileInfo fileInfo = new FileInfo(path);

//    if (!File.Exists(path))
//    {
//        sw = fileInfo.CreateText();

//        sw.WriteLine(temporaryInformations.PlayerCards);
//        sw.WriteLine(":");
//        sw.WriteLine(temporaryInformations.AINo1Cards);
//        sw.WriteLine(":");
//        sw.WriteLine(temporaryInformations.AINo2Cards);

//        sw.Close();
//        sw.Dispose();
//    }

//    ReadLayOutInfomationsFromTXT(_levelNumber);
//}

////read in TxT file
//public void ReadLayOutInfomationsFromTXT(int _levelNumber)
//{
//    string path = Application.streamingAssetsPath + "/LandlordsLevels/Level_" + _levelNumber + ".txt";

//    string temp = File.ReadAllText(path);

//    string[] currentCharacterCards = temp.Split(':');

//    string[] player = currentCharacterCards[0].Split('|');
//    string[] aiNo1 = currentCharacterCards[1].Split('|');
//    string[] aiNo2 = currentCharacterCards[2].Split('|');

//    CreateCard(player, playerCardGit);
//    CreateCard(aiNo1, ai1CardGit);
//    CreateCard(aiNo2, ai2CardGit);

//    for (int i = 0; i < playerCardGit.Count; i++)
//    {
//        DealCardManager.Instance.playerHandCards.Add(playerCardGit[i]);
//    }

//    for (int i = 0; i < ai1CardGit.Count; i++)
//    {
//        DealCardManager.Instance.aiNo1HandCards.Add(ai1CardGit[i]);
//    }

//    for (int i = 0; i < ai2CardGit.Count; i++)
//    {
//        DealCardManager.Instance.aiNo2HandCards.Add(ai2CardGit[i]);
//    }

//    DealCardManager.Instance.OrderTheCharacterHandCards(DealCardManager.Instance.playerHandCards, DealCardManager.Instance.playerHand);
//    DealCardManager.Instance.OrderTheCharacterHandCards(DealCardManager.Instance.aiNo1HandCards, DealCardManager.Instance.aiNo1Hand);
//    DealCardManager.Instance.OrderTheCharacterHandCards(DealCardManager.Instance.aiNo2HandCards, DealCardManager.Instance.aiNo2Hand);
//}

#endregion