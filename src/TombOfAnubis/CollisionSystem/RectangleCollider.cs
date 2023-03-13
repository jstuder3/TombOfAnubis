using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis.CollisionSystem
{
    internal class RectangleCollider : Collider
    {
        public RectangleCollider(float posX, float posY,float width, float height)
        {
            this.position = new Vector2(posX, posY);
            this.shape = new Vector2(width, height);
        }

        Vector2 position; //center of the collision rectangle
        Vector2 shape { get; set; }

        public override bool Intersects(Collider other)
        {
            if (other == this) return true;
            switch (other.colliderType)
            {
                case ColliderType.Rectangle:
                    RectangleCollider otherRect = (RectangleCollider)other;
                    if ((otherRect.position.X - this.position.X) < this.shape.X / 2f + otherRect.shape.X / 2f)
                    {
                        if ((otherRect.position.Y - this.position.Y) < this.shape.Y / 2f + otherRect.shape.Y / 2f)
                        {
                            return true;
                        }
                    }
                    break;
                case ColliderType.Box:
                    throw new NotImplementedException();
                    //break;
                case ColliderType.Circle:
                    throw new NotImplementedException();
                    //break;
                case ColliderType.Sphere:
                    throw new NotImplementedException();
                    //break;
                case ColliderType.Capsule:
                    throw new NotImplementedException();
                    //break;
                default:
                    throw new NotImplementedException();
            }
            return false;
        }
    }
}
