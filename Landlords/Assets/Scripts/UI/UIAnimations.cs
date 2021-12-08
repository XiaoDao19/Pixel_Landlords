using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace PIXEL.Landlords.UI
{

    public static class UIAnimations
    {

        #region 转场动画
        /// <summary>
        /// 分成四种情况，上下左右这四种动画方式
        /// </summary>
        /// <param name="_currentGameobject"></param>

        //退出当前场景过场动画
        public static void SceneTransition_Out(GameObject _currentGameobjectFirst, GameObject _currentGameobjectSecond, GameObject _currentGameobjectThird)
        {
            //右
            if (PlayerPrefs.GetInt("UISceneAnimation") == 0)
            {
                Debug.Log("Out 0");
                _currentGameobjectFirst.GetComponent<RectTransform>().anchoredPosition = new Vector2(1920f, 0f);
                _currentGameobjectSecond.GetComponent<RectTransform>().anchoredPosition = new Vector2(1920f, 0f);
                _currentGameobjectThird.GetComponent<RectTransform>().anchoredPosition = new Vector2(1920f, 0f);
                //重置过场panel color 透明度
                //transitionImage.color = new Color(transitionImageColor.r, transitionImageColor.g, transitionImageColor.b, 0);

                //透明度线性过渡
                //transitionImage.DOColor(transitionImageColor, 0.5f);

                //位置线性过渡
                _currentGameobjectFirst.GetComponent<RectTransform>().DOMoveX(960f, 0.5f);
                _currentGameobjectSecond.GetComponent<RectTransform>().DOMoveX(960f, 0.8f);
                _currentGameobjectThird.GetComponent<RectTransform>().DOMoveX(960f, 1.1f);
            }

            //左
            if (PlayerPrefs.GetInt("UISceneAnimation") == 1)
            {
                Debug.Log("Out 1");
                _currentGameobjectFirst.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1920f, 0f);
                _currentGameobjectSecond.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1920f, 0f);
                _currentGameobjectThird.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1920f, 0f);
                //重置过场panel color 透明度
                //transitionImage.color = new Color(transitionImageColor.r, transitionImageColor.g, transitionImageColor.b, 0);

                //透明度线性过渡
                //transitionImage.DOColor(transitionImageColor, 0.5f);

                //位置线性过渡
                _currentGameobjectFirst.GetComponent<RectTransform>().DOMoveX(960f, 0.5f);
                _currentGameobjectSecond.GetComponent<RectTransform>().DOMoveX(960f, 0.8f);
                _currentGameobjectThird.GetComponent<RectTransform>().DOMoveX(960f, 1.1f);
            }

            //上
            if (PlayerPrefs.GetInt("UISceneAnimation") == 2)
            {
                Debug.Log("Out 2");
                _currentGameobjectFirst.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -1080f);
                _currentGameobjectSecond.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -1080f);
                _currentGameobjectThird.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -1080f);

                //重置过场panel color 透明度
                //transitionImage.color = new Color(transitionImageColor.r, transitionImageColor.g, transitionImageColor.b, 0);

                //透明度线性过渡
                //transitionImage.DOColor(transitionImageColor, 0.5f);

                //位置线性过渡
                _currentGameobjectFirst.GetComponent<RectTransform>().DOMoveY(540f, 0.5f);
                _currentGameobjectSecond.GetComponent<RectTransform>().DOMoveY(540f, 0.8f);
                _currentGameobjectThird.GetComponent<RectTransform>().DOMoveY(540f, 1.1f);
            }

            //下
            if (PlayerPrefs.GetInt("UISceneAnimation") == 3)
            {
                Debug.Log("Out 3");
                _currentGameobjectFirst.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 1080f);
                _currentGameobjectSecond.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 1080f);
                _currentGameobjectThird.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 1080f);

                //重置过场panel color 透明度
                //transitionImage.color = new Color(transitionImageColor.r, transitionImageColor.g, transitionImageColor.b, 0);

                //透明度线性过渡
                //transitionImage.DOColor(transitionImageColor, 0.5f);

                //位置线性过渡
                _currentGameobjectFirst.GetComponent<RectTransform>().DOMoveY(540f, 0.5f);
                _currentGameobjectSecond.GetComponent<RectTransform>().DOMoveY(540f, 0.8f);
                _currentGameobjectThird.GetComponent<RectTransform>().DOMoveY(540f, 1.1f);
            }
        }

        //进入新场景过场动画
        public static void SceneTransition_In(GameObject _currentGameobjectFirst, GameObject _currentGameobjectSecond, GameObject _currentGameobjectThird)
        {
            _currentGameobjectFirst.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            _currentGameobjectSecond.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            _currentGameobjectThird.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            //右
            if (PlayerPrefs.GetInt("UISceneAnimation") == 0)
            {
                Debug.Log("In 0");

                //透明度线性过渡
                //_currentGameobjectFirst.GetComponent<Image>().DOColor(new Color(_currentGameobjectFirst.GetComponent<Image>().color.r,
                //                                                                _currentGameobjectFirst.GetComponent<Image>().color.g,
                //                                                                _currentGameobjectFirst.GetComponent<Image>().color.b,
                //                                                                1f), 0.5f);

                //_currentGameobjectSecond.GetComponent<Image>().DOColor(new Color(_currentGameobjectSecond.GetComponent<Image>().color.r,
                //                                                                _currentGameobjectSecond.GetComponent<Image>().color.g,
                //                                                                _currentGameobjectSecond.GetComponent<Image>().color.b,
                //                                                                1f), 0.7f);

                //_currentGameobjectThird.GetComponent<Image>().DOColor(new Color(_currentGameobjectThird.GetComponent<Image>().color.r,
                //                                                                _currentGameobjectThird.GetComponent<Image>().color.g,
                //                                                                _currentGameobjectThird.GetComponent<Image>().color.b,
                //                                                                1f), 0.9f);

                _currentGameobjectFirst.GetComponent<Image>().DOColor(new Color(_currentGameobjectFirst.GetComponent<Image>().color.r,
                                                                                _currentGameobjectFirst.GetComponent<Image>().color.g,
                                                                                _currentGameobjectFirst.GetComponent<Image>().color.b,
                                                                                1f), 0.5f);

                _currentGameobjectSecond.GetComponent<Image>().DOColor(new Color(_currentGameobjectFirst.GetComponent<Image>().color.r,
                                                                                _currentGameobjectFirst.GetComponent<Image>().color.g,
                                                                                _currentGameobjectFirst.GetComponent<Image>().color.b,
                                                                                1f), 0.7f);

                _currentGameobjectThird.GetComponent<Image>().DOColor(new Color(_currentGameobjectFirst.GetComponent<Image>().color.r,
                                                                                _currentGameobjectFirst.GetComponent<Image>().color.g,
                                                                                _currentGameobjectFirst.GetComponent<Image>().color.b,
                                                                                1f), 0.9f);

                //位置线性过渡
                _currentGameobjectFirst.GetComponent<RectTransform>().DOMoveX(1920f + 960f, 0.5f);
                _currentGameobjectSecond.GetComponent<RectTransform>().DOMoveX(1920f + 960f, 0.7f);
                _currentGameobjectThird.GetComponent<RectTransform>().DOMoveX(1920f + 960f, 0.9f);
            }

            //左
            if (PlayerPrefs.GetInt("UISceneAnimation") == 1)
            {
                Debug.Log("In 1");

                //透明度线性过渡
                //_currentGameobjectFirst.GetComponent<Image>().DOColor(new Color(_currentGameobjectFirst.GetComponent<Image>().color.r,
                //                                                                _currentGameobjectFirst.GetComponent<Image>().color.g,
                //                                                                _currentGameobjectFirst.GetComponent<Image>().color.b,
                //                                                                1f), 0.5f);

                //_currentGameobjectSecond.GetComponent<Image>().DOColor(new Color(_currentGameobjectSecond.GetComponent<Image>().color.r,
                //                                                                _currentGameobjectSecond.GetComponent<Image>().color.g,
                //                                                                _currentGameobjectSecond.GetComponent<Image>().color.b,
                //                                                                1f), 0.7f);

                //_currentGameobjectThird.GetComponent<Image>().DOColor(new Color(_currentGameobjectThird.GetComponent<Image>().color.r,
                //                                                                _currentGameobjectThird.GetComponent<Image>().color.g,
                //                                                                _currentGameobjectThird.GetComponent<Image>().color.b,
                //                                                                1f), 0.9f);

                _currentGameobjectFirst.GetComponent<Image>().DOColor(new Color(_currentGameobjectFirst.GetComponent<Image>().color.r,
                                                                                _currentGameobjectFirst.GetComponent<Image>().color.g,
                                                                                _currentGameobjectFirst.GetComponent<Image>().color.b,
                                                                                1f), 0.5f);

                _currentGameobjectSecond.GetComponent<Image>().DOColor(new Color(_currentGameobjectFirst.GetComponent<Image>().color.r,
                                                                                _currentGameobjectFirst.GetComponent<Image>().color.g,
                                                                                _currentGameobjectFirst.GetComponent<Image>().color.b,
                                                                                1f), 0.7f);

                _currentGameobjectThird.GetComponent<Image>().DOColor(new Color(_currentGameobjectFirst.GetComponent<Image>().color.r,
                                                                                _currentGameobjectFirst.GetComponent<Image>().color.g,
                                                                                _currentGameobjectFirst.GetComponent<Image>().color.b,
                                                                                1f), 0.9f);

                //位置线性过渡
                _currentGameobjectFirst.GetComponent<RectTransform>().DOMoveX(-1920f + 960f, 0.5f);
                _currentGameobjectSecond.GetComponent<RectTransform>().DOMoveX(-1920f + 960f, 0.7f);
                _currentGameobjectThird.GetComponent<RectTransform>().DOMoveX(-1920f + 960f, 0.9f);
            }

            //上
            if (PlayerPrefs.GetInt("UISceneAnimation") == 2)
            {
                Debug.Log("In 2");

                //透明度线性过渡
                //_currentGameobjectFirst.GetComponent<Image>().DOColor(new Color(_currentGameobjectFirst.GetComponent<Image>().color.r,
                //                                                                _currentGameobjectFirst.GetComponent<Image>().color.g,
                //                                                                _currentGameobjectFirst.GetComponent<Image>().color.b,
                //                                                                1f), 0.5f);

                //_currentGameobjectSecond.GetComponent<Image>().DOColor(new Color(_currentGameobjectSecond.GetComponent<Image>().color.r,
                //                                                                _currentGameobjectSecond.GetComponent<Image>().color.g,
                //                                                                _currentGameobjectSecond.GetComponent<Image>().color.b,
                //                                                                1f), 0.7f);

                //_currentGameobjectThird.GetComponent<Image>().DOColor(new Color(_currentGameobjectThird.GetComponent<Image>().color.r,
                //                                                                _currentGameobjectThird.GetComponent<Image>().color.g,
                //                                                                _currentGameobjectThird.GetComponent<Image>().color.b,
                //                                                                1f), 0.9f);

                _currentGameobjectFirst.GetComponent<Image>().DOColor(new Color(_currentGameobjectFirst.GetComponent<Image>().color.r,
                                                                                _currentGameobjectFirst.GetComponent<Image>().color.g,
                                                                                _currentGameobjectFirst.GetComponent<Image>().color.b,
                                                                                1f), 0.5f);

                _currentGameobjectSecond.GetComponent<Image>().DOColor(new Color(_currentGameobjectFirst.GetComponent<Image>().color.r,
                                                                                _currentGameobjectFirst.GetComponent<Image>().color.g,
                                                                                _currentGameobjectFirst.GetComponent<Image>().color.b,
                                                                                1f), 0.7f);

                _currentGameobjectThird.GetComponent<Image>().DOColor(new Color(_currentGameobjectFirst.GetComponent<Image>().color.r,
                                                                                _currentGameobjectFirst.GetComponent<Image>().color.g,
                                                                                _currentGameobjectFirst.GetComponent<Image>().color.b,
                                                                                1f), 0.9f);

                //位置线性过渡
                _currentGameobjectFirst.GetComponent<RectTransform>().DOMoveY(-1080f + 540f, 0.5f);
                _currentGameobjectSecond.GetComponent<RectTransform>().DOMoveY(-1080f + 540f, 0.7f);
                _currentGameobjectThird.GetComponent<RectTransform>().DOMoveY(-1080f + 540f, 0.9f);
            }

            //下
            if (PlayerPrefs.GetInt("UISceneAnimation") == 3)
            {
                Debug.Log("In 3");

                //透明度线性过渡
                //_currentGameobjectFirst.GetComponent<Image>().DOColor(new Color(_currentGameobjectFirst.GetComponent<Image>().color.r,
                //                                                                _currentGameobjectFirst.GetComponent<Image>().color.g,
                //                                                                _currentGameobjectFirst.GetComponent<Image>().color.b,
                //                                                                1f), 0.5f);

                //_currentGameobjectSecond.GetComponent<Image>().DOColor(new Color(_currentGameobjectSecond.GetComponent<Image>().color.r,
                //                                                                _currentGameobjectSecond.GetComponent<Image>().color.g,
                //                                                                _currentGameobjectSecond.GetComponent<Image>().color.b,
                //                                                                1f), 0.7f);

                //_currentGameobjectThird.GetComponent<Image>().DOColor(new Color(_currentGameobjectThird.GetComponent<Image>().color.r,
                //                                                                _currentGameobjectThird.GetComponent<Image>().color.g,
                //                                                                _currentGameobjectThird.GetComponent<Image>().color.b,
                //                                                                1f), 0.9f);

                _currentGameobjectFirst.GetComponent<Image>().DOColor(new Color(_currentGameobjectFirst.GetComponent<Image>().color.r,
                                                                                _currentGameobjectFirst.GetComponent<Image>().color.g,
                                                                                _currentGameobjectFirst.GetComponent<Image>().color.b,
                                                                                1f), 0.5f);

                _currentGameobjectSecond.GetComponent<Image>().DOColor(new Color(_currentGameobjectFirst.GetComponent<Image>().color.r,
                                                                                _currentGameobjectFirst.GetComponent<Image>().color.g,
                                                                                _currentGameobjectFirst.GetComponent<Image>().color.b,
                                                                                1f), 0.7f);

                _currentGameobjectThird.GetComponent<Image>().DOColor(new Color(_currentGameobjectFirst.GetComponent<Image>().color.r,
                                                                                _currentGameobjectFirst.GetComponent<Image>().color.g,
                                                                                _currentGameobjectFirst.GetComponent<Image>().color.b,
                                                                                1f), 0.9f);

                //位置线性过渡
                _currentGameobjectFirst.GetComponent<RectTransform>().DOMoveY(1080f + 540f, 0.5f);
                _currentGameobjectSecond.GetComponent<RectTransform>().DOMoveY(1080f + 540f, 0.7f);
                _currentGameobjectThird.GetComponent<RectTransform>().DOMoveY(1080f + 540f, 0.9f);
            }
        }

        #endregion

        #region 设置面板动画

        //设置面板动画
        public static void SettingsPanel_Show(GameObject _currentGameobject) 
        {
            _currentGameobject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 1920f);

            _currentGameobject.GetComponent<RectTransform>().DOMoveY(540f,0.5f);
        }

        public static void SettingsPanel_Close(GameObject _currentGameobject)
        {
            _currentGameobject.GetComponent<RectTransform>().DOMoveY(1080f + 540f, 0.5f);
        }

        //设置面板背景渐变动画
        public static void SettingsPanel_Back_Show(GameObject _currentGameobject) 
        {
            _currentGameobject.GetComponent<Image>().raycastTarget = true;

            Color32 currentColor = new Color32(39, 39, 39, 199);
            _currentGameobject.GetComponent<Image>().DOColor(currentColor, 0.5f);
        }

        public static void SettingsPanel_Back_Hide(GameObject _currentGameobject)
        {
            _currentGameobject.GetComponent<Image>().raycastTarget = false;

            Color32 currentColor = new Color32(39, 39, 39, 1);
            _currentGameobject.GetComponent<Image>().DOColor(currentColor, 0.5f);
        }



        #endregion
    }
}