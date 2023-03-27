using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public enum BlockDirections
    {
        Up,
        Right,
        Down,
        Left
    }
    public class RectangleCollider : Collider
    {
        public Vector2 Position { get; set; } //position is always top left of collider, but we separately keep the center position
        public Vector2 CenterPosition { get; set; }
        public Vector2 Size { get; set; } 

        public RectangleCollider(Vector2 position, Vector2 size) : base()
        {
            Size = size;
            Position = position;
            CenterPosition = position + new Vector2(size.X / 2, size.Y / 2);
        }

        public override void Update(GameTime gameTime)
        {
            //update location of collider based on owner
            Position = Entity.GetComponent<Transform>().Position;
            Size = Entity.Size();
            CenterPosition = Position + new Vector2(Size.X / 2, Size.Y / 2);
        }

        public float GetLeft()
        {
            return Position.X;
        }
        public float GetRight()
        {
            return Position.X + Size.X;
        }

        public float GetTop()
        {
            return Position.Y;
        }
        public float GetBottom()
        {
            return Position.Y + Size.Y;
        }
    }
}
