using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class CollisionSystem : BaseSystem<Collider>
    {

        public override void Update(GameTime gameTime)
        {
            GameLogic.deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (Collider collider in components)
            {
                if (collider.Active) collider.Update(gameTime);
            }

            HandleAllCollisions();
        }
        private static void HandleAllCollisions()
        {
            for (int i = 0; i < components.Count; i++)
            {
                for (int j = i + 1; j < components.Count; j++)
                {
                    if (Intersect(components[i], components[j]))
                    {
                        GameLogic.OnCollision(components[i].Entity, components[j].Entity);
                    }
                }
            }
        }

        private static bool Intersect(Collider c1, Collider c2)
        {
            switch (c1.GetType().Name, c2.GetType().Name)
            {
                case (nameof(RectangleCollider), nameof(RectangleCollider)):
                    return Intersect((RectangleCollider)c1, (RectangleCollider)c2);
                case (nameof(BoxCollider), nameof(BoxCollider)):
                    return Intersect((BoxCollider)c1, (BoxCollider)c2);
                default: return false;
            }
        }
        private static bool Intersect(RectangleCollider c1, RectangleCollider c2)
        {

            //adapted from https://kishimotostudios.com/articles/aabb_collision/
            bool AisToTheRightOfB = c1.GetLeft() > c2.GetRight();
            bool AisToTheLeftOfB = c1.GetRight() < c2.GetLeft();
            bool AisAboveB = c1.GetBottom() < c2.GetTop();
            bool AisBelowB = c1.GetTop() > c2.GetBottom();

            return !(AisToTheRightOfB || AisToTheLeftOfB || AisAboveB || AisBelowB);

        }
        private static bool Intersect(BoxCollider c1, BoxCollider c2)
        {
            // TODO: Implement
            return false;
        }
    }
}
