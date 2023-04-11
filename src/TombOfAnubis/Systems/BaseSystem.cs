using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace TombOfAnubis
{
    public class BaseSystem<T> where T : Component
    {
        protected static List<T> components = new List<T>();

        public static void Register(T component)
        {
            components.Add(component);
        }
        public static void Deregister(T component)
        {
            components.Remove(component);
        }
        public static void Clear()
        {
            components.Clear();
        }
        public static void SortComponents(Comparison<T> comparison)
        {
            components.Sort(comparison);
        }
        public static List<T> GetRegisteredComponents()
        {
            return components;
        }
        public virtual void Update(GameTime gameTime)
        {
        }
        public virtual void Draw(GameTime gameTime)
        {
        }
    }
}
