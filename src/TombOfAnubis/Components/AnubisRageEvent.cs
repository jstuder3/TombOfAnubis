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
            Session.GetInstance().AnubisAISystem.deactivateRageMode();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }

    public class AnubisCastEvent : WorldEvent
    {
        public AnubisCastEvent() : base(3)
        {

        }

        public override void Start()
        {
            base.Start();
            //this chooses a position of a currently visible player
            Session.GetInstance().AnubisAISystem.initiateCastMode();
        }
        public override void Stop()
        {
            base.Stop();
            //teleport to the chosen position 
            Session.GetInstance().AnubisAISystem.executeCastMode();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
