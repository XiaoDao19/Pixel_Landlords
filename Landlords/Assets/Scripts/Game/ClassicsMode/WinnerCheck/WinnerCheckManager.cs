using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PIXEL.Landlords.Card;
using PIXEL.Landlords.UI;

namespace PIXEL.Landlords.Game
{

    public class WinnerCheckManager : MonoBehaviour
    {
        private GameObject accountPanel;
        private Text tipText;
        private Button button_Back;
        private Button button_Quit;

        [Header("UIAnimations")]
        private static GameObject transitionPanel_First;
        private static GameObject transitionPanel_Second;
        private static GameObject transitionPanel_Third;

        void Start()
        {
            Time.timeScale = 1f;

            accountPanel = transform.GetChild(0).gameObject;
            tipText = accountPanel.transform.GetChild(0).GetComponent<Text>();
            button_Back = accountPanel.transform.GetChild(1).GetComponent<Button>();
            button_Quit = accountPanel.transform.GetChild(2).GetComponent<Button>();

            button_Back.onClick.AddListener(() => { BackMenu(); });
            button_Quit.onClick.AddListener(() => { Quit(); });

            PlayerPrefs.SetString("SceneName", SceneManager.GetActiveScene().name);
            transitionPanel_First = GameObject.Find("UIAnimation_First");
            transitionPanel_Second = GameObject.Find("UIAnimation_Second");
            transitionPanel_Third = GameObject.Find("UIAnimation_Third");

            UIAnimations.SceneTransition_In(transitionPanel_First, transitionPanel_Second, transitionPanel_Third);
        }

        void Update()
        {
            CheckWinner();
        }

        private void CheckWinner()
        {
            if (DealCardManager.Instance.playerHand.childCount == 0)
            {
                accountPanel.SetActive(true);
                tipText.text = "胜利";
                tipText.color = Color.red;

                for (int i = 0; i < DealCardManager.Instance.aiNo1Hand.childCount; i++)
                {
                    DealCardManager.Instance.aiNo1Hand.GetChild(i).GetComponent<Image>().sprite = DealCardManager.Instance.aiNo1Hand.GetChild(i).GetComponent<CardInformations>().CardInitialSprite;
                }

                for (int i = 0; i < DealCardManager.Instance.aiNo2Hand.childCount; i++)
                {
                    DealCardManager.Instance.aiNo2Hand.GetChild(i).GetComponent<Image>().sprite = DealCardManager.Instance.aiNo2Hand.GetChild(i).GetComponent<CardInformations>().CardInitialSprite;
                }

                return;
            }

            if (DealCardManager.Instance.aiNo1Hand.childCount == 0)
            {
                accountPanel.SetActive(true);
                tipText.text = "失败";

                for (int i = 0; i < DealCardManager.Instance.aiNo2Hand.childCount; i++)
                {
                    DealCardManager.Instance.aiNo2Hand.GetChild(i).GetComponent<Image>().sprite = DealCardManager.Instance.aiNo2Hand.GetChild(i).GetComponent<CardInformations>().CardInitialSprite;
                }

                return;
            }

            if (DealCardManager.Instance.aiNo2Hand.childCount == 0)
            {
                accountPanel.SetActive(true);
                tipText.text = "失败";

                for (int i = 0; i < DealCardManager.Instance.aiNo1Hand.childCount; i++)
                {
                    DealCardManager.Instance.aiNo1Hand.GetChild(i).GetComponent<Image>().sprite = DealCardManager.Instance.aiNo1Hand.GetChild(i).GetComponent<CardInformations>().CardInitialSprite;
                }

                return;
            }
        }
        private void BackMenu() 
        {
            int currentUIAnima = Random.Range(0, 4);

            PlayerPrefs.SetInt("UISceneAnimation", currentUIAnima);

            UIAnimations.SceneTransition_Out(transitionPanel_First, transitionPanel_Second, transitionPanel_Third);

            Invoke("BackMenuChange", 0.6f);
        }

        private void BackMenuChange() 
        {
            SceneManager.LoadScene(0);
        }

        private void Quit() 
        {
            Application.Quit(0);
        }
    }
}