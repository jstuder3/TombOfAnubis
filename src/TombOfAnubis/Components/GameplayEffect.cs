using Microsoft.Xna.Framework;

namespace TombOfAnubis { 
    public enum EffectType
    {
        Speedup,
        IncreaseViewDistance,
        Resurrection
    }
    public class GameplayEffect : Component
    {
        private bool started;
        private readonly float duration;
        private float startTime;
        private float endTime;

        private bool applied;

        public EffectType Type { get; set; }

        public GameplayEffect(EffectType type, float duration)
        {
            Type = type;
            this.duration = duration;
            started = false;
            GameplayEffectSystem.Register(this);
        }
        public override void Delete()
        {
            GameplayEffectSystem.Deregister(this);
        }
        public void Start(GameTime gameTime)
        {
            startTime = gameTime.TotalGameTime.Seconds;
            endTime = startTime + duration;
            started = true;
        }
        public bool IsStarted()
        {
            return started;
        }
        public bool IsActive(GameTime gameTime)
        {
            return gameTime.TotalGameTime.Seconds < endTime;
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
            switch (Type)
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
