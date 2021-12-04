using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PIXEL.Landlords.Card;
using PIXEL.Landlords.FrameWork;
using System.Collections;

namespace PIXEL.Landlords.Game
{
    public class DealCardManager : SingletonPattern<DealCardManager>
    {
        /// <summary>
        ///把不该公共的公共了，只是为了做开发者模式，用来自定手牌
        ///做完之后会改回去的
        /// </summary>
        
        [Header("角色手牌位置")]
        public Transform playerHand;
        public Transform aiNo1Hand;
        public Transform aiNo2Hand;

        [Header("各个花色牌库")]
        [HideInInspector] public List<GameObject> hongTaoCardGit = new List<GameObject>(13);
        [HideInInspector] public List<GameObject> fangKuaiCardGit = new List<GameObject>(13);
        [HideInInspector] public List<GameObject> heiTaoCardGit = new List<GameObject>(13);
        [HideInInspector] public List<GameObject> meiHuaCardGit = new List<GameObject>(13);
        [HideInInspector] public List<GameObject> jokerCardGit = new List<GameObject>(13);
        public Transform cardFather;

        [Header("角色手牌List")]
        public List<GameObject> playerHandCards = new List<GameObject>();
        public List<GameObject> aiNo1HandCards = new List<GameObject>();
        public List<GameObject> aiNo2HandCards = new List<GameObject>();

        [Header("卡背")]
        private GameObject cardBack;

        void Start()
        {
            playerHand = GameObject.Find("Hand_player").transform;
            aiNo1Hand = GameObject.Find("Hand_aiNo1").transform;
            aiNo2Hand = GameObject.Find("Hand_aiNo2").transform;

            cardBack = Resources.Load("Assets/CardAssets/card_back") as GameObject;

            //红桃 --- 1
            //方块 --- 2
            //黑桃 --- 3
            //梅花 --- 4
            //王 --- 5

            CreateCardGit(CardScriptableObjectGain.Instance.hongTaoGit.Count, CardScriptableObjectGain.Instance.hongTaoGit, hongTaoCardGit);
            CreateCardGit(CardScriptableObjectGain.Instance.fangKuaiGit.Count, CardScriptableObjectGain.Instance.fangKuaiGit, fangKuaiCardGit);
            CreateCardGit(CardScriptableObjectGain.Instance.heiTaoGit.Count, CardScriptableObjectGain.Instance.heiTaoGit, heiTaoCardGit);
            CreateCardGit(CardScriptableObjectGain.Instance.meiHuaGit.Count, CardScriptableObjectGain.Instance.meiHuaGit, meiHuaCardGit);
            CreateCardGit(CardScriptableObjectGain.Instance.jokerGit.Count, CardScriptableObjectGain.Instance.jokerGit, jokerCardGit);

            if (PlayerPrefs.GetInt("ClassicsMode") == 1)
            {
                DealCardManager.Instance.ClassicModeOn();
            }           
        }

        //经典模式发牌
        public void ClassicModeOn() 
        {
            for (int i = 0; i < 51; i++)
            {
                switch (Random.Range(0, 3))
                {
                    case 0:
                        if (playerHand.childCount == 17)
                        {
                            continue;
                        }
                        RandomCardOut_1(playerHand, playerHandCards);
                        break;
                    case 1:
                        if (aiNo1Hand.childCount == 17)
                        {
                            continue;
                        }
                        RandomCardOut_1(aiNo1Hand, aiNo1HandCards);
                        break;
                    case 2:
                        if (aiNo2Hand.childCount == 17)
                        {
                            continue;
                        }
                        RandomCardOut_1(aiNo2Hand, aiNo2HandCards);
                        break;
                }
            }
        }

        //随机出牌滴第一步，随机花色
        private void RandomCardOut_1(Transform _table, List<GameObject> _characterHandCard) 
        {
            //无限循环，循出一个花色
            for (int i = 1; i > 0; i++)
            {
                //如果当前出入的手牌list数量已满17张，则断开循环
                if (_characterHandCard.Count == 17)
                {
                    OrderTheCharacterHandCards(_characterHandCard, _table);
                   
                    break;
                }

                //随机1，2，3，4，5，分别代表四种花色和王牌库
                int temp = Random.Range(1, 6);

                //随机到当前花色后
                //判断当前花色的牌库是否为空，若为空则执行下一次循环，反之进入随机发牌第二步
                //下同
                if (temp == 1)
                {
                    if (hongTaoCardGit.Count == 0)
                    {
                        //Debug.Log("HongTao Card Git Is Empty!");
                        continue;
                    }

                   RandomCardOut_2(hongTaoCardGit, _characterHandCard);
                }

                if (temp == 2)
                {
                    if (fangKuaiCardGit.Count == 0)
                    {
                        //Debug.Log("FangKuai Card Git Is Empty!");
                        continue;
                    }

                   RandomCardOut_2(fangKuaiCardGit,_characterHandCard);
                }

                if (temp == 3)
                {
                    if (heiTaoCardGit.Count == 0)
                    {
                        //Debug.Log("HeiTao Card Git Is Empty!");
                        continue;
                    }

                    RandomCardOut_2(heiTaoCardGit, _characterHandCard);
                }

                if (temp == 4)
                {
                    if (meiHuaCardGit.Count == 0)
                    {
                        //Debug.Log("MeiHua Card Git Is Empty!");
                        continue;
                    }

                    RandomCardOut_2(meiHuaCardGit, _characterHandCard);
                }

                if (temp == 5)
                {
                    if (jokerCardGit.Count == 0)
                    {
                        //Debug.Log("Joker Card Git Is Empty!");
                        continue;
                    }

                    RandomCardOut_2(jokerCardGit,_characterHandCard);
                }
            }
        }

        //随机出牌滴第二步，随机牌
        private void RandomCardOut_2(List<GameObject> _cardGit, List<GameObject> _characterHandCard) 
        {
            //特殊王牌库
            if (_cardGit == jokerCardGit)
            {
                int temp = Random.Range(0,_cardGit.Count);

                _characterHandCard.Add(_cardGit[temp]);
                _cardGit.Remove(_cardGit[temp]);

                return;
            }

            //由于牌点数最小是3，最大是15，所以随机数从3到16，代表13张牌
            int x = Random.Range(3, 16);

            //循环遍历取出当前这张牌
            for (int i = 0; i < _cardGit.Count; i++)
            {
                if (x == 15)
                {
                    x = 19;
                }

                if (x == _cardGit[i].GetComponent<CardInformations>().CardValue)
                {
                    _characterHandCard.Add(_cardGit[i]);
                    _cardGit.Remove(_cardGit[i]);
                }
            }       
        }

        //创建牌库
        public void CreateCardGit(int _times, List<CardProperty> _cardType,List<GameObject> _cardGit) 
        {
            for (int i = 0; i < _times; i++)
            {
                GameObject currentCard = Instantiate(_cardType[i].cardPrefab);
                currentCard.transform.GetComponent<Image>().sprite = _cardType[i].cardType;

                currentCard.transform.SetParent(cardFather);
                currentCard.transform.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                currentCard.gameObject.name = _cardType[i].cardTypeName + _cardType[i].cardNumber;

                currentCard.GetComponent<CardInformations>().CardValue = _cardType[i].cardValue;
                currentCard.GetComponent<CardInformations>().IndexNumber = _cardType[i].indexNumber;
                currentCard.GetComponent<CardInformations>().CardInitialSprite = _cardType[i].cardType;
                currentCard.GetComponent<CardInformations>().CardCurrentSprite = _cardType[i].cardBack;

                currentCard.GetComponent<Image>().sprite = currentCard.GetComponent<CardInformations>().CardCurrentSprite;

                _cardGit.Add(currentCard);
            }
        }

        //将手牌排序（降序）
        public void OrderTheCharacterHandCards(List<GameObject> _characterHandCards,Transform _table) 
        {
            for (int i = 0; i < _characterHandCards.Count; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (_characterHandCards[i].GetComponent<CardInformations>().CardValue > _characterHandCards[j].GetComponent<CardInformations>().CardValue)
                    {
                        GameObject temp = _characterHandCards[i];
                        _characterHandCards[i] = _characterHandCards[j];
                        _characterHandCards[j] = temp;
                    }
                }
            }

            //将AI手中的卡牌翻过来，背面朝上
            if (_table.name != "Hand_player")
            {
                for (int i = 0; i < _characterHandCards.Count; i++)
                {
                    _characterHandCards[i].GetComponent<Image>().sprite = _characterHandCards[i].GetComponent<CardInformations>().CardCurrentSprite;
                }
            }
            else
            {
                for (int i = 0; i < _characterHandCards.Count; i++)
                {
                    _characterHandCards[i].GetComponent<Image>().sprite = _characterHandCards[i].GetComponent<CardInformations>().CardInitialSprite;
                }
            }

            for (int i = 0; i < _characterHandCards.Count; i++)
            {
                _characterHandCards[i].transform.SetParent(_table);
                _characterHandCards[i].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                _characterHandCards[i].SetActive(false);
            }

            //开启发牌动画携程
            StartCoroutine(PlayCard(_table));

            if (_table.gameObject.name == "Hand_player")
            {
                //为玩家手牌添加卡片控制器（只有玩家手牌可控）
                //并将每张手牌的Tag修改为Card
                for (int i = 0; i < playerHand.childCount; i++)
                {
                    playerHand.GetChild(i).gameObject.AddComponent<CardControl>();
                    playerHand.GetChild(i).gameObject.tag = "Card";
                }
            }
        }

        //发牌动画携程
        private IEnumerator PlayCard(Transform _targetTable) 
        {
            for (int i = 0; i < _targetTable.childCount; i++)
            {
                yield return new WaitForSeconds(0.3f);

                _targetTable.GetChild(i).gameObject.SetActive(true);
            }
        }
    }
}