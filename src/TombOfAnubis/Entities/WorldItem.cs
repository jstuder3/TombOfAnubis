using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class WorldItem : Entity
    {
        public ItemType ItemType { get; set; }
        public WorldItem(Vector2 position, Vector2 scale, ItemType itemType)
        {
            Transform transform = new Transform(position, scale, Visibility.Game);
            AddComponent(transform);

            Sprite sprite = new Sprite(ItemTextureLibrary.GetTexture(itemType), 0, Visibility.Game);
            AddComponent(sprite);

            RectangleCollider collider = new RectangleCollider(TopLeftCornerPosition(), Size(), false);
            AddComponent(collider);

            ItemType = itemType;

            Initialize();
        }
    }
}
