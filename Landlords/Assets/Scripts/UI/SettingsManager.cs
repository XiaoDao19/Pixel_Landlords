using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PIXEL.Landlords.FrameWork;

namespace PIXEL.Landlords.Sets
{
    public class SettingsManager : SingletonPattern<SettingsManager>
    {
        private Toggle windowsModeToggle;

        void Start()
        {
            windowsModeToggle = transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Toggle>();

            //初始设置全屏
            Screen.fullScreen = true;

            windowsModeToggle.onValueChanged.AddListener((bool value) => { WindownsToggle(value); });
        }

        void Update()
        {

        }

        //设置全屏或非全屏模式
        private void WindownsToggle(bool _toggleValue)
        {
            if (_toggleValue == true)
            {
                //1920，1080分辨率，不全屏
                Screen.SetResolution(1920, 1080, false);
            }
            else
            {
                //1920，1080分辨率，全屏
                Screen.SetResolution(1920, 1080, true);
            }
        }
    }
}