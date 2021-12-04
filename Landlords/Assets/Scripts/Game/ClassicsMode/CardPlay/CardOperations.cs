using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using PIXEL.Landlords.Game;
using PIXEL.Landlords.FrameWork;

namespace PIXEL.Landlords.Card
{
    public class CardOperations : SingletonPattern<CardOperations>
    {
        public List<GameObject> downList = new List<GameObject>();
        public List<GameObject> upList = new List<GameObject>();

        private Color32 intialColor = new Color32(255, 255, 255, 255);
        private Color32 selectedColor = new Color32(139, 139, 139, 255);

        public bool isHold;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) == true)
            {
                isHold = true;
            }

            if (Input.GetMouseButtonUp(0) == true)
            {                         
                isHold = false;
                BackEffect(downList);
                BackEffect(upList);
            }
        }

        //添加第一次点击时的卡牌，即第一张卡牌
        public void FirstAddCard(GameObject _card) 
        {
            if (_card.GetComponent<CardControl>().isSelected == true)
            {
                if (!downList.Contains(_card))
                {
                    _card.GetComponent<Image>().color = selectedColor;
                    downList.Add(_card);
                    return;
                }
                else
                {
                    _card.GetComponent<Image>().color = intialColor;
                    downList.Remove(_card);
                    return;
                }
            }

            if (_card.GetComponent<CardControl>().isSelected == false)
            {
                if (!upList.Contains(_card))
                {
                    _card.GetComponent<Image>().color = selectedColor;
                    upList.Add(_card);
                    return;
                }
                else
                {
                    _card.GetComponent<Image>().color = intialColor;
                    upList.Remove(_card);
                    return;
                }
            }
        }

        //鼠标滑动添加卡牌
        public void AddCard(GameObject _card) 
        {
            if (isHold == false)
            {
                return;
            }

            if (_card.GetComponent<CardControl>().isSelected == true)
            {
                if (!downList.Contains(_card))
                {
                    _card.GetComponent<Image>().color = selectedColor;
                    downList.Add(_card);
                    return;
                }
                else
                {
                    _card.GetComponent<Image>().color = intialColor;
                    downList.Remove(_card);
                    return;
                }
            }

            if (_card.GetComponent<CardControl>().isSelected == false)
            {
                if (!upList.Contains(_card))
                {
                    _card.GetComponent<Image>().color = selectedColor;
                    upList.Add(_card);
                    return;
                }
                else
                {
                    _card.GetComponent<Image>().color = intialColor;
                    upList.Remove(_card);
                    return;
                }
            }
        }

        //移除卡牌
        public void RemoveCard(GameObject _card) 
        {
            if (isHold == false)
            {
                return;
            }

            if (_card.GetComponent<CardControl>().isSelected == true)
            {
                if (downList.Contains(_card))
                {
                    _card.GetComponent<Image>().color = intialColor;
                    downList.Remove(_card);
                    return;
                }
                //else
                //{
                //    _card.GetComponent<Image>().color = intialColor;
                //    downList.Remove(_card);
                //    return;
                //}
            }

            if (_card.GetComponent<CardControl>().isSelected == false)
            {
                if (upList.Contains(_card))
                {
                    _card.GetComponent<Image>().color = intialColor;
                    upList.Remove(_card);
                    return;
                }
                //else
                //{
                //    _card.GetComponent<Image>().color = intialColor;
                //    upList.Remove(_card);
                //    return;
                //}
            }
        }

        //鼠标处于按下状态且从添加过的卡牌上移开时，即取消当前卡牌的滑动出牌
        private void BackEffect(List<GameObject> _list)
        {
            if (_list.Count == 0)
            {
                return;
            }

            for (int i = 0; i < _list.Count; i++)
            {
                                                                   //判断当前牌的isSelected参数是否为true，若为真则证明是已经被选择了的卡牌，则返回false，反之返回true。原理下同
                _list[i].GetComponent<CardControl>().isSelected = _list[i].GetComponent<CardControl>().isSelected == true ? false : true;
                _list[i].GetComponent<Image>().color = intialColor;
                _list[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(_list[i].GetComponent<RectTransform>().anchoredPosition.x,
                                                                                        _list[i].GetComponent<CardControl>().isSelected == true ? -39f : -96f);

                if (_list == downList)
                {
                    PlayCardManager.Instance.playerPlayCardList.Remove(_list[i]);
                }

                if (_list == upList)
                {
                    PlayCardManager.Instance.playerPlayCardList.Add(_list[i]);
                }
            }

            _list.Clear();
        }

        //有一个问题就是，滑动选牌之后，取消选择的第一张牌不响应
    }
}