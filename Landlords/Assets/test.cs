using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PIXEL.Landlords.UI;

public class test : MonoBehaviour
{
    public Button reStartButton;
    [Header("UIAnimations")]
    private static GameObject transitionPanel_First;
    private static GameObject transitionPanel_Second;
    private static GameObject transitionPanel_Third;
    void Start()
    {
        transitionPanel_First = GameObject.Find("UIAnimation_First");
        transitionPanel_Second = GameObject.Find("UIAnimation_Second");
        transitionPanel_Third = GameObject.Find("UIAnimation_Third");

        reStartButton.onClick.AddListener(() => {
            int currentUIAnima = Random.Range(0, 4);

            PlayerPrefs.SetInt("UISceneAnimation", currentUIAnima);
            UIAnimations.SceneTransition_Out(transitionPanel_First, transitionPanel_Second, transitionPanel_Third);
            Invoke("Back", 0.6f); });
    }

    void Update()
    {
        
    }

    private void Back() 
    {
        SceneManager.LoadScene(0);
    }
}
