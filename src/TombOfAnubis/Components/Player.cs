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
        public Player(int playerID)
        {
            PlayerID = playerID;
            PlayerState = PlayerState.Idle;
        }
    }
}
