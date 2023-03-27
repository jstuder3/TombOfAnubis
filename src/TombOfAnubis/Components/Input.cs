namespace TombOfAnubis
{
    public class Input : Component
    {
        public Input()
        {
            InputSystem.Register(this);
        }

        public override void Delete()
        {
            base.Delete();
            InputSystem.Deregister(this);
        }
    }
}
