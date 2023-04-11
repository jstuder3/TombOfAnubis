using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TombOfAnubisContentData;

namespace TombOfAnubis
{
    public class ButtonControllerSystem : BaseSystem<ButtonController>
    {
        public override void Update(GameTime gameTime)
        {
            foreach (ButtonController buttonController in components)
            {
                buttonController.Update(gameTime);
            }
        }
    }
}
