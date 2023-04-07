namespace TombOfAnubis
{
    public class Player : Component
    {
        public int PlayerID { get; set; }
        public Player(int playerID)
        {
            PlayerID = playerID;
        }
    }
}
