using Microsoft.Xna.Framework;

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
        public Vector2 Size { get; set; }
        private Vector2 centerPosition { get; set; }


        public RectangleCollider(Vector2 position, Vector2 size) : base()
        {
            Size = size;
            Position = position;
            centerPosition = position + new Vector2(size.X / 2, size.Y / 2);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //update location of collider based on owner
            Position = Entity.TopLeftCornerPosition();
            Size = Entity.Size();
            centerPosition = Position + new Vector2(Size.X / 2, Size.Y / 2);
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

        public Vector2 GetCenter()
        {
            return centerPosition;
        }
    }
}
