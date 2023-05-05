using Microsoft.Xna.Framework;
using TombOfAnubis.Components;

namespace TombOfAnubis
{
    public class World : Entity
    {
        public Vector2 Origin { get; set; }
        public Vector2 Scale { get; set; }

        /// <summary>
        /// The world entity is the root entity of the session and is placed at the WorldOrigin with WorldScale
        /// </summary>
        public World( Vector2 origin, Vector2 scale)
        {
            Origin = origin;
            Scale = scale;
            
            AnubisRageEvent anubisRageEvent = new AnubisRageEvent();
            AddComponent( anubisRageEvent );

            AnubisCastEvent anubisCastEvent = new AnubisCastEvent();
            AddComponent( anubisCastEvent );

            ChangeMapEvent changeMapEvent = new ChangeMapEvent();
            AddComponent(changeMapEvent );
            
            AnubisBlockPowerups anubisBlockPowerupsEvent = new AnubisBlockPowerups();
            AddComponent(anubisBlockPowerupsEvent );

        }
    }
}
