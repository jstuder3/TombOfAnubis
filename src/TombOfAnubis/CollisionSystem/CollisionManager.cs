using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis.CollisionSystem
{
    internal class CollisionManager
    {

        public void handleAllCollisions(List<ICollidable> collidables)
        {
            for (int i = 0; i < collidables.Count; i++)
            {
                for (int j = i + 1; j < collidables.Count; j++)
                {
                    if (collidables[i].collider.intersects(collidables[j].collider))
                    {
                        collidables[i].handleCollision(collidables[j]);
                        collidables[j].handleCollision(collidables[i]);
                    }
                }
            }
        }

    }
}
