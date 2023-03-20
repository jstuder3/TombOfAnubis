using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class PlayerInput : Component
    {
        public int PlayerID { get; set; }

        public PlayerInput(int playerID)
        {
            PlayerID = playerID;
            PlayerInputSystem.Register(this);
            PlayerInputSystem.SortComponents((x, y) => x.PlayerID.CompareTo(y.PlayerID));
        }
    }
}
