using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PIXEL.Landlords.FrameWork;
using UnityEngine.Audio;
using PIXEL.Landlords.Audio;

namespace PIXEL.Landlords.Sets.SystemSets
{
    public class SettingsManager : SingletonPattern<SettingsManager>
    {
        [Header("窗口模式")]
        private Toggle windowsModeToggle;

        [Header("声音控制Sliders")]
        private Slider slider_Music;
        private Slider slider_AudioEffect;

        [Header("声音混合器")]
        [SerializeField]private AudioMixer audioMixer; 

        void Start()
        {
            windowsModeToggle = transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Toggle>();

            slider_Music = transform.GetChild(1).GetChild(2).GetChild(1).GetComponent<Slider>();
            slider_AudioEffect = transform.GetChild(1).GetChild(3).GetChild(1).GetComponent<Slider>();

            //初始设置全屏
            Screen.fullScreen = true;

            windowsModeToggle.onValueChanged.AddListener((bool valueChange) => { WindownsToggle(valueChange); });

            slider_Music.onValueChanged.AddListener((float value) => { BackGroundMusicValueContorl(value); });
            slider_AudioEffect.onValueChanged.AddListener((float value) => { AudioEffectValueControl(value); });

            //设置mixer的声音以及对应的slider的value值
            if (AudioMixerValue.BackMusicValue >= -80f && AudioMixerValue.BackMusicValue <= 20f)
            {
                slider_Music.value = AudioMixerValue.BackMusicValue;
            }

            if (AudioMixerValue.AudioEffectValue >= -80f && AudioMixerValue.AudioEffectValue <= 20f)
            {
                slider_AudioEffect.value = AudioMixerValue.AudioEffectValue;
            }
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

        //背景音乐声音大小控制
        private void BackGroundMusicValueContorl(float _value) 
        {
            slider_Music.value = _value;
            AudioMixerValue.BackMusicValue = slider_Music.value;
            audioMixer.SetFloat("MusicValue", AudioMixerValue.BackMusicValue);
        }

        //音效声音大小控制
        private void AudioEffectValueControl(float _value) 
        {
            slider_AudioEffect.value = _value;
            AudioMixerValue.AudioEffectValue = slider_AudioEffect.value;
            audioMixer.SetFloat("AudioEffectValue", AudioMixerValue.AudioEffectValue);
        }
    }
}