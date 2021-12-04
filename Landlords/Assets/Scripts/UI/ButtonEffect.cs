using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace PIXEL.Landlords.UI
{

    public class ButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Material initialMaterial;
        public Material buttonEffect;

        private void Start()
        {
            initialMaterial = gameObject.GetComponent<Image>().material;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //切换按钮特殊效果材质
            gameObject.GetComponent<Image>().material = buttonEffect;
            //DOTweening制作按钮放大效果动画
            gameObject.GetComponent<RectTransform>().DOScale(new Vector3(2f, 2f, 2f), 0.5f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            gameObject.GetComponent<Image>().material = initialMaterial;
            gameObject.GetComponent<RectTransform>().DOScale(new Vector3(1f, 1f, 1f), 0.5f);
        }
    }
}