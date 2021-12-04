using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PIXEL.Landlords.Game.LevelMode
{

    [SerializeField]
    public class LevelInformations
    {
        public uint LevelId;//关卡编号
        public string PlayerCards;//玩家卡牌
        public string AINo1Cards;//AI---1卡牌
        public string AINo2Cards;//AI---2卡牌
    }
}