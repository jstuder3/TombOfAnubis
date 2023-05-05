using Microsoft.Xna.Framework;
using System; using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class DiscoverySystem : BaseSystem<Discovery>
    {
        public override void Update(GameTime deltaTime)
        {
            List<Character> characters = Session.GetInstance().World.GetChildrenOfType<Character>();

            foreach (Discovery discovery in GetComponents())
            {
                foreach (Character character in characters)
                {
                    Vector2 discoveryPosition = discovery.Entity.CenterPosition();
                    Vector2 characterPosition = character.CenterPosition();

                    float distance = (characterPosition - discoveryPosition).Length();

                    if(distance < 1.5f*Session.GetInstance().Map.TileSize.X)
                    {
                        discovery.Discovered = true;
                    }
                }
            }
        }

    }
}
