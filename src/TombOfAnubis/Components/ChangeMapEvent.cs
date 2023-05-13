using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis.Components
{
    public class ChangeMapEvent : WorldEvent
    {
        bool turnedBlack = false;

        public ChangeMapEvent() : base(3)
        {
        }
        public override void Start()
        {
            base.Start();
            Session.GetInstance().IsEarthquake = true;

        }
        public override void Stop()
        {
            base.Stop();
            Session.RegenerateMap();
            Session.GetInstance().PauseDrawing = false;
            turnedBlack = false;


        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Progress > 0.8f && !turnedBlack)
            {
                Session.GetInstance().IsEarthquake = false;
                Session.GetInstance().PauseDrawing = true;
                Session.GetInstance().World.GetChildrenOfType<Anubis>()[0].AddComponent(new GameplayEffect(EffectType.MultiplicativeSpeedModification, 3f, 0.001f, Visibility.Game));
                turnedBlack = true;
            }
        }
    }
}
