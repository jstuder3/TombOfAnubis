using Microsoft.VisualBasic;
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
                case (nameof(Character), nameof(Character)):
                    OnCollision((Character)source, (Character)target);
                    break;
                case (nameof(Character), nameof(Wall)):
                    OnCollision((Character)source, (Wall)target);
                    break;
                case (nameof(Wall), nameof(Character)):
                    OnCollision((Character)target, (Wall)source);
                    break;
                case (nameof(Character), nameof(Artefact)):
                    OnCollision((Character)target, (Artefact)source);
                    break;
                case (nameof(Artefact), nameof(Character)):
                    OnCollision((Character)target, (Artefact)source);
                    break;
                case (nameof(Character), nameof(Dispenser)):
                    OnCollision((Character)target, (Dispenser)source);
                    break;
                case (nameof(Dispenser), nameof(Character)):
                    OnCollision((Character)target, (Dispenser)source);
                    break;
            }
        }
        public static void OnCollision(Character p1, Character p2)
        {

            // TODO: Implement
        }
        public static void OnCollision(Character character, Wall wall)
        {
            // TODO: Implement
        }
        public static void OnCollision(Character character, Artefact artefact)
        {

            // TODO: Implement
        }
        public static void OnCollision(Character character, Dispenser dispenser)
        {

            // TODO: Implement
        }
    }
}
