using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace TombOfAnubis
{
    public static class GameLogic
    {
        public static float deltaTime { get; set; }
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

            Transform t1 = p1.GetComponent<Transform>();
            Transform t2 = p2.GetComponent <Transform>();

            Vector2 overlap_direction = new Vector2(t2.Position.X - t1.Position.X, t2.Position.Y - t1.Position.Y);
            overlap_direction.Normalize();
            //invert the values to push the players away faster/stronger if the overlap is bigger
            //overlap_direction.X = 1 / overlap_direction.X;
            //overlap_direction.Y = 1 / overlap_direction.Y;
            //overlap_direction.Normalize();

            t1.Position -= overlap_direction * deltaTime * 50;
            t2.Position += overlap_direction * deltaTime * 50;
            
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
