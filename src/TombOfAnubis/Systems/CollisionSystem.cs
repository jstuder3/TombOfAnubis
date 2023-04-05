using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace TombOfAnubis
{
    public class CollisionSystem : BaseSystem<Collider>
    {
        public static HashSet<Tuple<Collider, Collider>> SkippedCollisions;
        public override void Update(GameTime gameTime)
        {
            GameLogic.DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            GameLogic.GameTime = gameTime;

            foreach (Collider collider in components)
            {
                collider.Update(gameTime);
            }

            HandleAllCollisions();
        }
        private static void HandleAllCollisions()
        {
            SkippedCollisions = new HashSet<Tuple<Collider, Collider>>();
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
            // Try skipped collisions again if the still collide
            foreach (Tuple<Collider, Collider> tuple in SkippedCollisions)
            {
                if(Intersect(tuple.Item1, tuple.Item2))
                {
                    GameLogic.OnCollision(tuple.Item1.Entity, tuple.Item2.Entity);
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
