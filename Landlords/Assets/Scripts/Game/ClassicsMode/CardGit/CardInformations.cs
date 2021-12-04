using UnityEngine;
using UnityEngine.UI;

namespace PIXEL.Landlords.Card
{
    public class CardInformations : MonoBehaviour
    {
        private int cardValue;
        private int indexNumber;
        private Sprite initialSprite;
        private Sprite currentSprite;

        //封装两个属性，牌面点数和牌索引值
        public int CardValue 
        {
            get => cardValue;

            set 
            {
                cardValue = value;
            }
        }

        public int IndexNumber 
        {
            get => indexNumber;

            set 
            {
                indexNumber = value;
            }
        }

        //牌面和牌背
        public Sprite CardInitialSprite 
        {
            get => initialSprite;

            set 
            {
                initialSprite = value;
            }
        }

        public Sprite CardCurrentSprite 
        {
            get => currentSprite;

            set 
            {
                currentSprite = value;
            }
        }
    }
}