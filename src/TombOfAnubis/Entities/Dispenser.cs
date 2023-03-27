using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TombOfAnubis
{
    public enum DispenserType
    {
        BodyPowerup,
        WisdomPowerup,
        ResurrectionPowerup,
        None
    }
    public class Dispenser : Entity
    {
        public Dispenser(Vector2 position, Vector2 scale, Texture2D texture)
        {
            Transform transform = new Transform(position, scale);
            AddComponent(transform);

            Sprite sprite = new Sprite(texture, 1);
            AddComponent(sprite);

            // TODO: Add collider
            //Collider collider = new Collider();
            //AddComponent(collider);
        }
    }
}
