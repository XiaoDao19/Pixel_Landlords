using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PIXEL.Landlords.Game;
using PIXEL.Landlords.Card;
using PIXEL.Landlords.Game.LevelMode;
using UnityEngine.SceneManagement;
using PIXEL.Landlords.UI;

namespace PIXEL.Landlords.MainMenu
{
    public class MainMenu : MonoBehaviour
    {
        [Header("模式按钮")]
        private Button button_ClassicMode;
        private Button button_LevelMode;
        private Button button_BossMode;
        private Button button_DevelopMode;

        [Header("开发者模式panel")]
        private GameObject developMode;
        private GameObject developPanel;

        [Header("Panels")]
        private GameObject tipPanel;
        private GameObject bossModeTipPanel;
        private GameObject levelModePanel;

        [Header("Buttons")]
        private Button bossModeTipPanel_closeButton;

        [Header("UIAnimations")]
        private static GameObject transitionPanel_First;
        private static GameObject transitionPanel_Second;
        private static GameObject transitionPanel_Third;
        private void Start()
        {
            button_ClassicMode = gameObject.transform.GetChild(0).GetComponent<Button>();
            button_LevelMode = gameObject.transform.GetChild(1).GetComponent<Button>();
            button_BossMode = gameObject.transform.GetChild(2).GetComponent<Button>();
            button_DevelopMode = gameObject.transform.GetChild(3).GetComponent<Button>();

            //developMode = GameObject.Find("DeveloperMode");
            //developMode.SetActive(!developMode.activeSelf);
            //developPanel = GameObject.Find("DevelopPanel");
            //developPanel.SetActive(!developPanel.activeSelf);

            levelModePanel = GameObject.Find("LevelModePanel");

            tipPanel = GameObject.Find("TipPanel");
            bossModeTipPanel = tipPanel.transform.GetChild(0).gameObject;

            transitionPanel_First = GameObject.Find("UIAnimation_First");
            transitionPanel_Second = GameObject.Find("UIAnimation_Second");
            transitionPanel_Third = GameObject.Find("UIAnimation_Third");

            bossModeTipPanel_closeButton = bossModeTipPanel.transform.GetChild(1).GetComponent<Button>();
            bossModeTipPanel_closeButton.onClick.AddListener(() => { bossModeTipPanel.SetActive(!bossModeTipPanel.activeSelf); });

            button_ClassicMode.onClick.AddListener(() => { ClassicMode(); });
            button_LevelMode.onClick.AddListener(() => { LevelMode(); });
            button_BossMode.onClick.AddListener(() => { BossMode(); });
            button_DevelopMode.onClick.AddListener(() => { DevelopMode(); });

            if (PlayerPrefs.GetString("SceneName") != SceneManager.GetActiveScene().name)
            {
                UIAnimations.SceneTransition_In(transitionPanel_First,transitionPanel_Second,transitionPanel_Third);
            }
        }

        #region ClassicsMode
        //经典模式启动按钮
        private void ClassicMode()
        {
            //存储普通模式发牌，若为1，则是普通模式，若为0，则是开发者模式发牌，若为2，则是关卡模式发牌
            PlayerPrefs.SetInt("ClassicsMode", 1);

            int currentUIAnima = Random.Range(0, 4);

            PlayerPrefs.SetInt("UISceneAnimation", currentUIAnima);

            UIAnimations.SceneTransition_Out(transitionPanel_First, transitionPanel_Second, transitionPanel_Third);         
            //等待1秒过后执行
            Invoke("ClassicModeChange", 0.6f);
        }

        //经典模式
        private void ClassicModeChange() 
        {
            //DealCardManager.Instance.ClassicModeOn();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        #endregion

        #region LevelMode
        //原理同上
        private void LevelMode()
        {
            PlayerPrefs.SetInt("ClassicsMode", 2);

            int currentUIAnima = Random.Range(0, 4);

            PlayerPrefs.SetInt("UISceneAnimation", currentUIAnima);

            UIAnimations.SceneTransition_Out(transitionPanel_First, transitionPanel_Second, transitionPanel_Third);
            PlayerPrefs.SetInt("LevelNumber", 1);

            //gameObject.SetActive(!gameObject.activeSelf);
            //LevelModeManager.Instance.ReadLayOutInformations(1);
            Invoke("LevelModeChange", 0.6f);
        }

        private void LevelModeChange() 
        {
            //UIAnimations.SceneTransition_In(transitionPanel);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
        }

        #endregion

        #region BossMode
        private void BossMode()
        {
            bossModeTipPanel.SetActive(!bossModeTipPanel.activeSelf);
        }

        #endregion

        #region DevelopMode
        //同上
        private void DevelopMode()
        {
            PlayerPrefs.SetInt("ClassicsMode", 0);

            int currentUIAnima = Random.Range(0, 4);

            PlayerPrefs.SetInt("UISceneAnimation", currentUIAnima);
            UIAnimations.SceneTransition_Out(transitionPanel_First, transitionPanel_Second, transitionPanel_Third);
            //等待1秒过后执行
            Invoke("DevelopModeChange", 0.6f);
        }

        private void DevelopModeChange() 
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 3);
        }

        #endregion
    }
}