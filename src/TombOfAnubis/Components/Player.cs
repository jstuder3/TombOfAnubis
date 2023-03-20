using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public enum PlayerState
    {
        Idle,
        Walking,
        Running,
        Jumping,
        Falling,
        Hiding,
        Climbing,
        Dead,
        Trapped
    }
    public class Player : Component
    {
        public int PlayerID { get; set; }

        public PlayerState PlayerState { get; set; }
        public Player(int playerID) { 
            PlayerID = playerID;
            PlayerState = PlayerState.Idle;
        } 
    }
}
