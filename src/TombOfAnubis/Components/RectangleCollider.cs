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
        public List<BlockDirections> blockedDirections { get; set; }
        public Vector2 Position { get; set; } //position is always top left of collider
        public Vector2 Size { get; set; } 

        public RectangleCollider(Vector2 position, Vector2 size) : base()
        {
            Size = size;
            Position = position;
            blockedDirections = new List<BlockDirections>();
        }

        public override void Update(GameTime gameTime)
        {
            blockedDirections.Clear();

            //update location of collider based on owner
            Position = new Vector2(Entity.GetComponent<Transform>().Position.X, Entity.GetComponent<Transform>().Position.Y);
        }

        public float GetLeft()
        {
            return Position.X;
        }
        public float GetRight()
        {
            return Position.X + Size.X * Entity.GetComponent<Transform>().Scale.X;
        }

        public float GetTop()
        {
            return Position.Y;
        }
        public float GetBottom()
        {
            return Position.Y + Size.Y * Entity.GetComponent<Transform>().Scale.Y;
        }
    }
}
