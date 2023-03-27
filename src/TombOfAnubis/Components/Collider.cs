using Microsoft.Xna.Framework;

namespace TombOfAnubis
{
    public class Collider : Component
    {
        public Collider()
        {
            CollisionSystem.Register(this);
        }
        public virtual void Update(GameTime gameTime) { }

        public override void Delete()
        {
            base.Delete();
            CollisionSystem.Deregister(this);
        }

    }
}
