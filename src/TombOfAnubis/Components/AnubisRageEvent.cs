using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class AnubisRageEvent : WorldEvent
    {
        public AnubisRageEvent() : base(5)
        {

        }

        public override void Start()
        {
            base.Start();
            Session.GetInstance().AnubisAISystem.activateRageMode();
        }
        public override void Stop()
        {
            base.Stop();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
