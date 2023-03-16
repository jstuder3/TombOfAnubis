using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    internal class BoxCollider : Collider
    {
        public BoxCollider(float posX, float posY, float posZ, float width, float length, float height)
        {
            this.position = new Vector3(posX, posY, posZ);
            this.shape = new Vector3(width, length, height);
        }

        Vector3 position; //center of the collision box
        Vector3 shape { get; set; }

        public override bool Intersects(Collider other)
        {
            if(other == this) return true;
            switch (other.colliderType)
            {
                case ColliderType.Rectangle:
                    throw new NotImplementedException();
                    //break;
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
        }
    }
}
