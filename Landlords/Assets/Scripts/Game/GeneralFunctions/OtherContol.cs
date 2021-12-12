using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PIXEL.Landlords.UI;

public class OtherContol : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField]private Button button_reStart;
    [SerializeField] private Button button_Sets;

    [Header("UIAnimations")]
    private static GameObject transitionPanel_First;
    private static GameObject transitionPanel_Second;
    private static GameObject transitionPanel_Third;

    [Header("设置面板")]
    private GameObject settingsPanel;
    private GameObject settingPanel_Back;
    private Button settingsPanel_closeButton;

    void Start()
    {
        transitionPanel_First = GameObject.Find("UIAnimation_First");
        transitionPanel_Second = GameObject.Find("UIAnimation_Second");
        transitionPanel_Third = GameObject.Find("UIAnimation_Third");

        settingsPanel = transform.GetChild(2).GetChild(1).gameObject;
        settingPanel_Back = transform.GetChild(2).GetChild(0).gameObject;
        settingsPanel_closeButton = settingsPanel.transform.GetChild(0).GetComponent<Button>();

        button_reStart.onClick.AddListener(() => {
            int currentUIAnima = Random.Range(0, 4);

            PlayerPrefs.SetInt("UISceneAnimation", currentUIAnima);
            UIAnimations.SceneTransition_Out(transitionPanel_First, transitionPanel_Second, transitionPanel_Third);
            Invoke("Back", 0.6f); });

        button_Sets.onClick.AddListener(() => { ShowSettingsPanel(); });
        settingsPanel_closeButton.onClick.AddListener(() => { CloseSettingsPanel(); });
    }

    void Update()
    {
        
    }

    //加载主界面
    private void Back() 
    {
        SceneManager.LoadScene(0);
    }

    //设置面板
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
}
