﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class Player : Entity
    {
        public Player(int playerID, Vector2 position, Vector2 scale, Texture2D texture)
        {
            Transform transform = new Transform(position, scale);
            AddComponent(transform);
            Sprite sprite = new Sprite(texture, 1);
            AddComponent(sprite);
            PlayerInput playerInput = new PlayerInput(playerID);
            AddComponent(playerInput);
        }
    }
}
