using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class CollisionSystem : BaseSystem<Collider>
    {
        public override void Update(GameTime gameTime)
        {
            HandleAllCollisions();
        }
        private static void HandleAllCollisions()
        {
            for (int i = 0; i < components.Count; i++)
            {
                for (int j = i + 1; j < components.Count; j++)
                {
                    if (components[i].Intersects(components[j]))
                    {
                        GameLogic.OnCollision(components[i].Entity, components[j].Entity);
                    }
                }
            }
        }
    }
}
