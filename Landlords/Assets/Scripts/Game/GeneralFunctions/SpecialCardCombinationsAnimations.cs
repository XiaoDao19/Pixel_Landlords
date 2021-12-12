using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PIXEL.Landlords.FrameWork;
using PIXEL.Landlords.Game;

namespace PIXEL.Landlords.Animation.SpecialCardAnimations
{
    public class SpecialCardCombinationsAnimations : SingletonPattern<SpecialCardCombinationsAnimations>
    {
        [Header("特殊牌型动画展示")]
        private GameObject scca_JokerBoom;
        private GameObject scca_Boom;
        private GameObject scca_Plane;
        private GameObject scca_StraightPair;
        private GameObject scca_Straight;
        private string loadPath = "SpecialCardCombinationAnimations/";

        [Header("动画展示位置")]
        private Transform effectPlayPoint_Player;
        private Transform effectPlayPoint_AI_1;
        private Transform effectPlayPoint_AI_2;

        private void Start()
        {
            scca_JokerBoom = Resources.Load<GameObject>(loadPath + "JokerBoom");
            scca_Boom = Resources.Load<GameObject>(loadPath + "Boom");
            scca_Plane = Resources.Load<GameObject>(loadPath + "Plane");
            scca_StraightPair = Resources.Load<GameObject>(loadPath + "Straight_Pair");
            scca_Straight = Resources.Load<GameObject>(loadPath + "Straight");

            effectPlayPoint_Player = gameObject.transform.GetChild(0);
            effectPlayPoint_AI_1 = gameObject.transform.GetChild(1);
            effectPlayPoint_AI_2 = gameObject.transform.GetChild(2);
        }

        public void ShowAnimations(string _character,PlayCardType _currentType) 
        {
            Transform playPoint = null;

            switch (_character)
            {
                case "Player":
                    playPoint = effectPlayPoint_Player;
                    break;
                case "AI_1":
                    playPoint = effectPlayPoint_AI_1;
                    break;
                case "AI_2":
                    playPoint = effectPlayPoint_AI_2;
                    break;
            }

            switch (_currentType)
            {              
                case PlayCardType.Straight:
                    Instantiate(scca_Straight, playPoint);
                    break;
                case PlayCardType.Straight_pair:
                    Instantiate(scca_StraightPair, playPoint);
                    break;
                case PlayCardType.Plane_none:                 
                case PlayCardType.Plane_single:
                case PlayCardType.Plane_pair:
                    Instantiate(scca_Plane, playPoint);
                    break;
                case PlayCardType.Boom:
                    Instantiate(scca_Boom, playPoint);
                    Debug.Log(22);
                    break;
                case PlayCardType.JokerBoom:
                    Instantiate(scca_JokerBoom, playPoint);
                    break;
            }
        }

        public void Recycle(GameObject _target) 
        {
            _target.SetActive(gameObject);

            _target.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            _target.SetActive(false);
        }
    }
}