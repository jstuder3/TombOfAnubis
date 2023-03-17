using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public static class CollisionManager
    {

        public static void HandleAllCollisions(List<ICollidable> collidables)
        {
            for (int i = 0; i < collidables.Count; i++)
            {
                for (int j = i + 1; j < collidables.Count; j++)
                {
                    if (collidables[i].Collider.Intersects(collidables[j].Collider))
                    {
                        collidables[i].HandleCollision(collidables[j]);
                        collidables[j].HandleCollision(collidables[i]);
                    }
                }
            }
        }

    }
}
