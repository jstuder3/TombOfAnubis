using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class WorldEvent : Component
    {
        public bool IsStarted { get; set; } = false;
        public float Duration { get; set; }

        public float CooldownAfterEvent {get; set;}

        public WorldEvent(float duration, float coolDownAfterEvent = 10) 
        {
            Duration = duration;
            CooldownAfterEvent = coolDownAfterEvent;
            WorldEventSystem.Register(this);
        }

        public override void Delete()
        {
            base.Delete();
            WorldEventSystem.Deregister(this);
        }

        public virtual void Start() 
        {
            IsStarted = true;
        }
        public virtual void Stop()
        {
            IsStarted = false;
        }
        public virtual void Update(GameTime gameTime)
        {

        }
    }
}
