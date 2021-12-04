using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PIXEL.Landlords.Card;
using System.Linq;
using UnityEngine.SceneManagement;
using PIXEL.Landlords.AI;

namespace PIXEL.Landlords.Game.DeveloperMode
{
    public class ReadEditedCard : MonoBehaviour
    {
        [Header("InputFields")]
        public InputField playerInputField;
        public InputField ai1InputField;
        public InputField ai2InputField;

        [Header("Edit Card String[]s")]
        public string[] playerCards;
        public string[] ai1Cards;
        public string[] ai2Cards;

        [Header("Edit Cards Git")]
        public List<GameObject> playerCardGit = new List<GameObject>();
        public List<GameObject> ai1CardGit = new List<GameObject>();
        public List<GameObject> ai2CardGit = new List<GameObject>();

        [Header("Buttons")]
        public Button confirmButton;

        [Header("Wrong Tip Panel")]
        public GameObject wrongTipPanel;
        public Button closeButton;

        public GameObject DeveloperModePanel;

        private string[] totalCards = new string[] { "103", "104", "105", "106", "107", "108", "109", "110", "111", "112", "113", "114", "119" ,
                                                     "203", "204", "205", "206", "207", "208", "209", "210", "211", "212", "213", "214", "219" ,
                                                     "303", "304", "305", "306", "307", "308", "309", "310", "311", "312", "313", "314", "319" ,
                                                     "403", "404", "405", "406", "407", "408", "409", "410", "411", "412", "413", "414", "419" ,
                                                     "519", "520"
                                                    };

        [Header("AITransforms")]
        public Transform ai1;
        public Transform ai2;

        void Start()
        {
            confirmButton.onClick.AddListener(() =>
            {
                Confirm();
            });

            closeButton.onClick.AddListener(() => { wrongTipPanel.SetActive(!wrongTipPanel.activeSelf); });
        }

        //Check Your Edit Card Are There Any Of The Same And Any Empty
        private bool ConfirmSameOne()
        {
            if (playerInputField.text == "" ||
                ai1InputField.text == "" ||
                ai2InputField.text == "")
            {
                wrongTipPanel.SetActive(!wrongTipPanel.activeSelf);

                wrongTipPanel.transform.GetChild(0).GetComponent<Text>().text = "有空白！";

                return true;
            }

            for (int i = 0; i < playerCards.Length; i++)
            {
                if (totalCards.ToList().IndexOf(playerCards[i]) == -1)
                {
                    wrongTipPanel.SetActive(!wrongTipPanel.activeSelf);

                    wrongTipPanel.transform.GetChild(0).GetComponent<Text>().text = "不存在的牌！" + "Player: " + playerCards[i];

                    return true;
                }

                for (int j = 0; j < ai1Cards.Length; j++)
                {
                    if (playerCards[i] == ai1Cards[j])
                    {
                        wrongTipPanel.SetActive(!wrongTipPanel.activeSelf);

                        wrongTipPanel.transform.GetChild(0).GetComponent<Text>().text = "有相同卡牌！" + "Player: " + playerCards[i] + " | " + "AI-1: " + ai1Cards[j];

                        return true;
                    }
                }

                for (int j = 0; j < ai2Cards.Length; j++)
                {
                    if (playerCards[i] == ai2Cards[j])
                    {
                        wrongTipPanel.SetActive(!wrongTipPanel.activeSelf);

                        wrongTipPanel.transform.GetChild(0).GetComponent<Text>().text = "有相同卡牌！" + "Player: " + playerCards[i] + " | " + "AI-2: " + ai2Cards[j];

                        return true;
                    }
                }
            }

            for (int i = 0; i < ai1Cards.Length; i++)
            {
                if (totalCards.ToList().IndexOf(ai1Cards[i]) == -1)
                {
                    wrongTipPanel.SetActive(!wrongTipPanel.activeSelf);

                    wrongTipPanel.transform.GetChild(0).GetComponent<Text>().text = "不存在的牌！" + "AI-1: " + ai1Cards[i];

                    return true;
                }

                for (int j = 0; j < playerCards.Length; j++)
                {
                    if (ai1Cards[i] == playerCards[j])
                    {
                        wrongTipPanel.SetActive(!wrongTipPanel.activeSelf);

                        wrongTipPanel.transform.GetChild(0).GetComponent<Text>().text = "有相同卡牌！" + "AI-1: " + ai1Cards[i] + " | " + "Player: " + playerCards[j];

                        return true;
                    }
                }

                for (int j = 0; j < ai2Cards.Length; j++)
                {
                    if (ai1Cards[i] == ai2Cards[j])
                    {
                        wrongTipPanel.SetActive(!wrongTipPanel.activeSelf);

                        wrongTipPanel.transform.GetChild(0).GetComponent<Text>().text = "有相同卡牌！" + "AI-1: " + ai1Cards[i] + " | " + "AI-2: " + ai2Cards[j];

                        return true;
                    }
                }
            }

            for (int i = 0; i < ai2Cards.Length; i++)
            {
                if (totalCards.ToList().IndexOf(ai2Cards[i]) == -1)
                {
                    wrongTipPanel.SetActive(!wrongTipPanel.activeSelf);

                    wrongTipPanel.transform.GetChild(0).GetComponent<Text>().text = "不存在的牌！" + "AI-2: " + ai2Cards[i];

                    return true;
                }

                for (int j = 0; j < ai1Cards.Length; j++)
                {
                    if (ai2Cards[i] == ai1Cards[j])
                    {
                        wrongTipPanel.SetActive(!wrongTipPanel.activeSelf);

                        wrongTipPanel.transform.GetChild(0).GetComponent<Text>().text = "有相同卡牌！" + "AI-2: " + ai2Cards[i] + " | " + "AI-1: " + ai1Cards[j];

                        return true;
                    }
                }

                for (int j = 0; j < playerCards.Length; j++)
                {
                    if (ai2Cards[i] == playerCards[j])
                    {
                        wrongTipPanel.SetActive(!wrongTipPanel.activeSelf);

                        wrongTipPanel.transform.GetChild(0).GetComponent<Text>().text = "有相同卡牌！" + "AI-2: " + ai2Cards[i] + " | " + "Player: " + playerCards[j];

                        return true;
                    }
                }
            }

            return false;
        }

        //Create Your Edit Cards
        private void CreateCard(string[] _target,List<GameObject> _targetGit)
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

        //Confirm
        private void Confirm() 
        {
            playerCards = playerInputField.text.Split(',');
            ai1Cards = ai1InputField.text.Split(',');
            ai2Cards = ai2InputField.text.Split(',');

            if (ConfirmSameOne() == false)
            {
                CreateCard(playerCards, playerCardGit);
                CreateCard(ai1Cards, ai1CardGit);
                CreateCard(ai2Cards, ai2CardGit);

                OrderTheTargetHandCards(playerCardGit);
                OrderTheTargetHandCards(ai1CardGit);
                OrderTheTargetHandCards(ai2CardGit);

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

                //ai1.GetComponent<FSM>().ClassifyHandCards();
                //ai2.GetComponent<FSM>().ClassifyHandCards();

                DeveloperModePanel.SetActive(!DeveloperModePanel.activeSelf);
            }
        }

        //Order Card Git
        private void OrderTheTargetHandCards(List<GameObject> _characterHandCards)
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
        }
    }
}