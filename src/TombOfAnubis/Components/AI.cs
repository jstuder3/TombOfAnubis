namespace TombOfAnubis
{
    public class AI : Component
    {
        public MovementState MovementState { get; set; }

        public MovementGraph MovementGraph { get; set; }
        public AI(Map map)
        {
            AISystem.Register(this);
            MovementGraph = new MovementGraph(map);
        }

        public override void Delete()
        {
            base.Delete();
            AISystem.Deregister(this);
            MovementGraph = null;
        }
    }
}
