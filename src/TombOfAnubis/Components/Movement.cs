namespace TombOfAnubis
{
    public enum Orientation
    {
        Up,
        Right,
        Down,
        Left
    }
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
    public class Movement : Component
    {
        public int MaxSpeed { get; set; }
        public bool IsWalking { get; set; }
        public Orientation Orientation { get; set; }

        public MovementState State { get; set; }


        public Movement(int maxSpeed, MovementState state = MovementState.Idle)
        {

            MaxSpeed = maxSpeed;
            IsWalking = false;
            Orientation = Orientation.Up;
            State = state;
        }

        public bool IsTrapped()
        {
            return State == MovementState.Trapped;
        }
        

    }
}
