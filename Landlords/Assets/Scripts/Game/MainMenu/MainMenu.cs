using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PIXEL.Landlords.Game;
using PIXEL.Landlords.Card;
using PIXEL.Landlords.Game.LevelMode;
using UnityEngine.SceneManagement;
using PIXEL.Landlords.UI;
using PIXEL.Landlords.Audio;

namespace PIXEL.Landlords.MainMenu
{
    public class MainMenu : MonoBehaviour
    {
        [Header("模式按钮")]
        private Button button_ClassicMode;
        private Button button_LevelMode;
        private Button button_BossMode;
        private Button button_DevelopMode;

        [Header("功能按钮")]
        private Button button_SettingsButton;

        [Header("Panels")]
        private GameObject tipPanel;
        private GameObject bossModeTipPanel;
        private GameObject settingsPanel;
        private GameObject settingPanel_Back;

        [Header("Buttons")]
        private Button bossModeTipPanel_closeButton;
        private Button settingsPanel_closeButton;

        [Header("转场UI动画")]
        private static GameObject transitionPanel_First;
        private static GameObject transitionPanel_Second;
        private static GameObject transitionPanel_Third;

        private void Start()
        {
            //所有需要获取的组件和面板
            button_ClassicMode = gameObject.transform.GetChild(0).GetComponent<Button>();
            button_LevelMode = gameObject.transform.GetChild(1).GetComponent<Button>();
            button_BossMode = gameObject.transform.GetChild(2).GetComponent<Button>();
            button_DevelopMode = gameObject.transform.GetChild(3).GetComponent<Button>();

            button_SettingsButton = gameObject.transform.GetChild(4).GetComponent<Button>();

            tipPanel = GameObject.Find("TipPanel");
            bossModeTipPanel = tipPanel.transform.GetChild(0).gameObject;

            settingsPanel = transform.parent.GetChild(2).GetChild(1).gameObject;
            settingPanel_Back = transform.parent.GetChild(2).GetChild(0).gameObject;    
            settingsPanel_closeButton = settingsPanel.transform.GetChild(0).GetComponent<Button>();

            transitionPanel_First = GameObject.Find("UIAnimation_First");
            transitionPanel_Second = GameObject.Find("UIAnimation_Second");
            transitionPanel_Third = GameObject.Find("UIAnimation_Third");

            bossModeTipPanel_closeButton = bossModeTipPanel.transform.GetChild(1).GetComponent<Button>();
            bossModeTipPanel_closeButton.onClick.AddListener(() => { bossModeTipPanel.SetActive(!bossModeTipPanel.activeSelf); });

            //为所有button添加相应事件
            button_ClassicMode.onClick.AddListener(() => { ClassicMode(); });
            button_LevelMode.onClick.AddListener(() => { LevelMode(); });
            button_BossMode.onClick.AddListener(() => { BossMode(); });
            button_DevelopMode.onClick.AddListener(() => { DevelopMode(); });

            button_SettingsButton.onClick.AddListener(() => { ShowSettingsPanel(); });
            settingsPanel_closeButton.onClick.AddListener(() => { CloseSettingsPanel(); });

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
            if (PlayerPrefs.HasKey("LevelNumber") == false)
            {
                PlayerPrefs.SetInt("LevelNumber", 1);
            }

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

        #region SettingsPanel

        //设置面板动画
        private void ShowSettingsPanel() 
        {
            UIAnimations.SettingsPanel_Show(settingsPanel);
            UIAnimations.SettingsPanel_Back_Show(settingPanel_Back);
        }

        private void CloseSettingsPanel() 
        {
            UIAnimations.SettingsPanel_Close(settingsPanel);
            UIAnimations.SettingsPanel_Back_Hide(settingPanel_Back);
        }

        #endregion
    }
}