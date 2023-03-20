using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class Scene : Entity
    {
        public Scene(Vector2 worldOrigin) {
            Transform transform = new Transform(worldOrigin);
            AddComponent(transform);
        }
    }
}
