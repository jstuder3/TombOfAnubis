using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    internal class WorldObject
    {
        public string name;
        public string description;

        public Vector3 position;

        public Texture2D texture;

        public Collider collider;

    }
}
