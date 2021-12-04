using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PIXEL.Landlords.FrameWork;
using UnityEngine.SceneManagement;
using PIXEL.Landlords.UI;

namespace PIXEL.Landlords.Game.ClassicsMode
{
    public class ClassicsModeManager :  SingletonPattern<ClassicsModeManager>
    {
        [Header("UIAnimations")]
        private static GameObject transitionPanel_First;
        private static GameObject transitionPanel_Second;
        private static GameObject transitionPanel_Third;

        private void Start()
        {
            PlayerPrefs.SetString("SceneName", SceneManager.GetActiveScene().name);
            transitionPanel_First = GameObject.Find("UIAnimation_First");
            transitionPanel_Second = GameObject.Find("UIAnimation_Second");
            transitionPanel_Third = GameObject.Find("UIAnimation_Third");

            UIAnimations.SceneTransition_In(transitionPanel_First, transitionPanel_Second, transitionPanel_Third);
        }
    }
}