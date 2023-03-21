﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class Character : Entity
    {

        public Character(int playerID, Vector2 position, Vector2 scale, Texture2D texture, int maxMovementSpeed)
        {
            Transform transform = new Transform(position, scale);
            AddComponent(transform);

            Sprite sprite = new Sprite(texture, 2);
            AddComponent(sprite);

            Player player = new Player(playerID);
            AddComponent(player);

            Input input = new Input();
            AddComponent(input);

            Movement movement = new Movement(maxMovementSpeed);
            AddComponent(movement);

            Inventory inventory = new Inventory();
            AddComponent(inventory);

            //RectangleCollider collider = new RectangleCollider(position, new Vector2(texture.Width * scale.X, texture.Height * scale.Y), debugTexture, this);// texture.Width * scale.X, texture.Height * scale.Y));
            RectangleCollider collider = new RectangleCollider(position, new Vector2(texture.Width * scale.X, texture.Height * scale.Y));// texture.Width * scale.X, texture.Height * scale.Y));
            AddComponent(collider);

        }
        public Character(int playerID, Vector2 position, Vector2 scale, Texture2D texture, int maxMovementSpeed, Texture2D debugTexture) 
            : this(playerID, position, scale, texture, maxMovementSpeed)
        {
            Sprite debugSprite = new Sprite(debugTexture, new Rectangle(0, 0, texture.Width, texture.Height), 3);
            AddComponent(debugSprite);

        }
    }
}
