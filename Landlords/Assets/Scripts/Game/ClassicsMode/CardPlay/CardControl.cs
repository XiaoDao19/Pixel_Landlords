using UnityEngine;
using UnityEngine.EventSystems;
using PIXEL.Landlords.Game;

namespace PIXEL.Landlords.Card
{
    public class CardControl : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler,IPointerEnterHandler,IPointerExitHandler
    {
        [Header("卡牌状态")]
        public bool isSelected;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (Input.GetMouseButtonDown(1))
            {
                return;
            }

            //如果点击的当前物体的tag不是Card
            if (eventData.pointerCurrentRaycast.gameObject.CompareTag("Card") == false)
            {
                return;
            }

            GameObject currentCard = eventData.pointerCurrentRaycast.gameObject;
            RectTransform currentCardrectTransform = currentCard.GetComponent<RectTransform>();

            //卡牌抬起与放下
            //if (isSelected == false)
            //{
            //    PlayCardManager.Instance.playerPlayCardList.Add(eventData.pointerCurrentRaycast.gameObject);
            //    currentCardrectTransform.anchoredPosition = new Vector2(currentCardrectTransform.anchoredPosition.x, -39f);
            //    isSelected = true;
            //    return;
            //}
            //else
            //{
            //    PlayCardManager.Instance.playerPlayCardList.Remove(eventData.pointerCurrentRaycast.gameObject);
            //    currentCardrectTransform.anchoredPosition = new Vector2(currentCardrectTransform.anchoredPosition.x, -96f);
            //    isSelected = false;
            //    return;
            //}

        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (Input.GetMouseButtonDown(1))
            {
                return;
            }

            //点击时添加到滑动出牌
            CardOperations.Instance.FirstAddCard(gameObject);
        }
       
        public void OnPointerUp(PointerEventData eventData)
        {

        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (Input.GetMouseButtonDown(1))
            {
                return;
            }

            if (eventData.pointerCurrentRaycast.gameObject.CompareTag("Card"))
            {
                CardOperations.Instance.AddCard(gameObject);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //CardOperations.Instance.RemoveCard(gameObject);
        }
    }
}