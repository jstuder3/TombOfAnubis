using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public static class GameLogic
    {
        public static void OnCollision(Entity source, Entity target)
        {
            switch (source.GetType().Name, target.GetType().Name)
            {
                case (nameof(Player), nameof(Player)):
                    OnCollision((Player)source, (Player)target);
                    break;
                case (nameof(Player), nameof(Wall)):
                    OnCollision((Player)source, (Wall)target);
                    break;
                case (nameof(Wall), nameof(Player)):
                    OnCollision((Player)target, (Wall)source);
                    break;
            }
        }
        public static void OnCollision(Player p1, Player p2)
        {

            // TODO: Implement
        }
        public static void OnCollision(Player player, Wall wall)
        {

            // TODO: Implement
        }
    }
}
