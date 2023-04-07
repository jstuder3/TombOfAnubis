namespace TombOfAnubis
{
    public enum MovementState
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

        public MovementState PlayerState { get; set; }
        public Player(int playerID)
        {
            PlayerID = playerID;
            PlayerState = MovementState.Idle;
        }
    }
}
