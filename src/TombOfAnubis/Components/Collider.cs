using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class Collider : Component
    {
        public Collider()
        {
            CollisionSystem.Register(this);
        }
        public virtual void Update(GameTime gameTime) { }

        public void Deregister()
        {
            base.DeleteComponent();
            CollisionSystem.Deregister(this);
        }

    }
}
