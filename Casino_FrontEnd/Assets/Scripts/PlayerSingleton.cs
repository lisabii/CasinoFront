using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.TestApp.TestApp.Scripts
{
    public class PlayerSingleton
    {
        private static CasinoWarPlayer player;
        public static CasinoWarPlayer GetPlayer()
        {
            return player;
        }
        public static void SetPlayer(CasinoWarPlayer player)
        {
            PlayerSingleton.player = player;
        }
    }
}
