using Microsoft.Xna.Framework;
using System; using System.Diagnostics;
using System.Collections.Generic;

namespace TombOfAnubis
{
    public class CollisionSystem : BaseSystem<Collider>
    {
        public static HashSet<Tuple<Collider, Collider>> SkippedCollisions = new HashSet<Tuple<Collider, Collider>>();
        public static List<Collider> StaticColliders = new List<Collider>();
        public static List<Collider> DynamicColliders = new List<Collider>();

        new public static void Register(Collider collider)
        {
            components.Add(collider);
            if (collider.IsStatic())
            {
                StaticColliders.Add(collider);
            }
            else
            {
                DynamicColliders.Add(collider);
            }
        }
        new public static void Deregister(Collider collider)
        {
            components.Remove(collider);
            if (collider.IsStatic())
            {
                StaticColliders.Remove(collider);
            }
            else
            {
                DynamicColliders.Remove(collider);
            }
        }

        public override void Update(GameTime gameTime)
        {
            GameLogic.DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            GameLogic.GameTime = gameTime;

            foreach (Collider collider in GetComponents())
            {
                collider.Update(gameTime); // clears overlap list and updates position based on owner's transform.position
            }

            HandleAllCollisions();
        }
        private static void HandleAllCollisions()
        {
            SkippedCollisions = new HashSet<Tuple<Collider, Collider>>();
            var visibleStaticColliders = GetComponents(StaticColliders);
            var visibleDynamicColliders = GetComponents(DynamicColliders);

            //run static-to-dynamic collision detection
            for (int i = 0; i < visibleStaticColliders.Count; i++)
            {
                for (int j = 0; j < visibleDynamicColliders.Count; j++)
                {
                    if (Intersect(visibleStaticColliders[i], visibleDynamicColliders[j]))
                    {
                        visibleStaticColliders[i].AddOverlap(visibleDynamicColliders[j]);
                        visibleDynamicColliders[j].AddOverlap(visibleStaticColliders[i]);

                        GameLogic.OnCollision(visibleStaticColliders[i].Entity, visibleDynamicColliders[j].Entity);
                    }
                }
            }

            //run dynamic-to-dynamic collision detection
            for(int i = 0; i< visibleDynamicColliders.Count; i++)
            {
                for (int j = i + 1; j < visibleDynamicColliders.Count; j++)
                {
                    if (Intersect(visibleDynamicColliders[i], visibleDynamicColliders[j]))
                    {
                        visibleDynamicColliders[i].AddOverlap(visibleDynamicColliders[j]);
                        visibleDynamicColliders[j].AddOverlap(visibleDynamicColliders[i]);

                        GameLogic.OnCollision(visibleDynamicColliders[i].Entity, visibleDynamicColliders[j].Entity);
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
            if (c1.IsStatic() && c2.IsStatic()) return false;

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

        public static new void Clear()
        {
            components.Clear();
            StaticColliders.Clear();
            DynamicColliders.Clear();
            SkippedCollisions.Clear();
        }
    }
}
