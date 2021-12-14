using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SmartDLL;
using PIXEL.Landlords.FrameWork;

namespace PIXEL.Landlords.Sets.PlayerInformatioSets
{
    public static class PlayerInformations
    {
        [Header("玩家信息")]
        private static RawImage player_Icon;
        private static string player_Name;

        public static RawImage Player_Icon
        {
            get => player_Icon;

            set
            {
                player_Icon = value;
            }
        }

        public static string Player_Name
        {
            get => player_Name;

            set
            {
                player_Name = value;
            }
        }
    }
}