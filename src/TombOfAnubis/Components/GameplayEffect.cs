using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis { 
    public enum EffectType
    {
        Speedup,
        IncreaseViewDistance,
        Resurrection
    }
    public class GameplayEffect : Component
    {

        float duration;
        float startTime;
        float endTime;

        bool applied;

        public EffectType type { get; set; }

        public GameplayEffect(EffectType type, float duration, Entity entity)
        {
            this.type = type;
            this.duration = duration;
            startTime = Session.GameTime.TotalGameTime.Seconds;
            endTime = startTime + duration;
            Entity = entity;

        }

        public bool IsActive()
        {
            return Session.GameTime.TotalGameTime.Seconds < endTime;
        }

        public bool HasBeenApplied()
        {
            return applied;
        }

        public void Apply()
        {

            applied = true;
        }

        public bool IsAppliedEveryUpdate() //effects like "damage over time" would, for example, be applied every update
        {
            switch (type)
            {
                case EffectType.Speedup: //speedup directly changes maxSpeed, so it is only applied in the very beginning (and removed again in the very end)
                    return false;
                case EffectType.IncreaseViewDistance: //same story
                    return false;
            }
            return false;
        }

    }
}
