using UnityEngine;

namespace PIXEL.Landlords.Card
{
    [CreateAssetMenu(fileName = "Card", menuName = "Inventory/Card")]
    public class CardProperty : ScriptableObject
    {
        public GameObject cardPrefab;//卡牌预制体
        public string cardTypeName;//卡牌花色名称
        public Sprite cardType;//卡片花色
        public string cardNumber;//卡牌牌面
        public int cardValue;//卡牌数值
        public Color32 color;//当前花色颜色
        public int indexNumber;//10开头为红桃，20开头为方块，30开头为黑桃，40开头为梅花，519，520为大小王
        public Sprite cardBack;
    }
}