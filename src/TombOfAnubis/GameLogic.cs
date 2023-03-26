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
            Transform t2 = p2.GetComponent<Transform>();

            RectangleCollider c1 = p1.GetComponent<RectangleCollider>();
            RectangleCollider c2 = p2.GetComponent<RectangleCollider>();

            Vector2 center1 = p1.GetComponent<RectangleCollider>().CenterPosition;
            Vector2 center2 = p2.GetComponent <RectangleCollider>().CenterPosition;

            Vector2 overlap_direction = center2 - center1;
            overlap_direction.Normalize();

            t1.Position -= overlap_direction * deltaTime * 50;
            t2.Position += overlap_direction * deltaTime * 50;
            
        }
        public static void OnCollision(Character character, Wall wall)
        {
            Transform t1 = character.GetComponent<Transform>();
            Transform t2 = wall.GetComponent<Transform>();

            RectangleCollider c1 = character.GetComponent<RectangleCollider>();
            RectangleCollider c2 = wall.GetComponent<RectangleCollider>();

            float sum_half_widths = c1.Size.X / 2f + c2.Size.X / 2f;
            float sum_half_heights = c1.Size.Y / 2f + c2.Size.Y / 2f;

            //Console.WriteLine("sum half widths: " + sum_half_widths + "; sum half heights: " + sum_half_heights); 

            Vector2 overlap = c1.CenterPosition - c2.CenterPosition; //IMPORTANT: Center difference and top-left-corner difference is NOT necessarily the same because the boxes don't have to be quadratic (stupid error that cost me like 3 hours)

            while (overlap.Length() == 0) //prevent NaNs
            {
                Random random = new Random();
                overlap.X = (float)random.NextDouble() - 0.5f;
                overlap.Y = (float)random.NextDouble() - 0.5f;
            }

            //String debug_out = "Overlap: " + overlap.ToString() + " - " + t1.Position + " -- " + t2.Position + " -- Collider sizes: Player: " + 2*c1_half_width + ", " + 2* c1_half_height + "; Wall: " + 2*c2_half_width + ", " + 2*c2_half_height+";";
            //Console.WriteLine(debug_out);

            if (MathF.Abs(overlap.X / sum_half_widths) > MathF.Abs(overlap.Y / sum_half_heights))
            {
                overlap.X = MathF.Sign(overlap.X) * (sum_half_widths - MathF.Abs(overlap.X)); //push out so much that the overlap is zero
                overlap.Y = 0;
                t1.Position += overlap;
                //Console.WriteLine("X was bigger. Overlap adjustment: " + overlap.ToString());
            }
            else
            {
                overlap.X = 0;
                overlap.Y = MathF.Sign(overlap.Y) * (sum_half_heights - MathF.Abs(overlap.Y)); //push out so much that the overlap is zero
                t1.Position += overlap;
                //Console.WriteLine("Y was bigger. Overlap adjustment: " + overlap.ToString());
            }
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
