using Microsoft.Xna.Framework;

namespace TombOfAnubis
{
    public class BoxCollider : Collider
    {
        public Vector3 Position { get; set; }
        public Vector3 Size { get; set; }
        public BoxCollider(Vector3 position, Vector3 size) : base(false)
        {
            Position = position;
            Size = size;
        }
    }
}
