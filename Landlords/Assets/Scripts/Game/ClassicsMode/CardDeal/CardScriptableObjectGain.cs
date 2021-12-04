using System.Collections.Generic;
using UnityEngine;
using PIXEL.Landlords.FrameWork;

namespace PIXEL.Landlords.Card
{
    public class CardScriptableObjectGain : SingletonPattern<CardScriptableObjectGain>
    {
        /// <summary>
        /// 分别为各个花色创建牌库，并从Resource文件夹中加载其相应卡牌信息
        /// </summary>
        public List<CardProperty> hongTaoGit = new List<CardProperty>();
        public List<CardProperty> fangKuaiGit = new List<CardProperty>();
        public List<CardProperty> heiTaoGit = new List<CardProperty>();
        public List<CardProperty> meiHuaGit = new List<CardProperty>();
        public List<CardProperty> jokerGit = new List<CardProperty>();

        protected override void Awake()
        {
            base.Awake();

            //根据牌信息命名，从3开始循环至15，各13张牌
            //分别将各个花色牌库的牌信息ScriptObject存入相应花色牌库List
            for (int i = 3; i < 16; i++)
            {
                hongTaoGit.Add(Resources.Load<CardProperty>("CardGit/HongTao/" + i.ToString()));
                fangKuaiGit.Add(Resources.Load<CardProperty>("CardGit/FangKuai/" + i.ToString()));
                heiTaoGit.Add(Resources.Load<CardProperty>("CardGit/HeiTao/" + i.ToString()));
                meiHuaGit.Add(Resources.Load<CardProperty>("CardGit/MeiHua/" + i.ToString()));
            }

            jokerGit.Add(Resources.Load<CardProperty>("CardGit/Jokers/LittleKing"));
            jokerGit.Add(Resources.Load<CardProperty>("CardGit/Jokers/BigKing"));
        }
    }
}