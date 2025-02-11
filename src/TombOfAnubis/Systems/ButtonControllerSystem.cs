﻿using Microsoft.Xna.Framework;
using System; using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class ButtonControllerSystem : BaseSystem<ButtonController>
    {
        public override void Update(GameTime gameTime)
        {
            foreach (ButtonController buttonController in GetComponents())
            {
                buttonController.Update(gameTime);
            }
        }
    }
}
