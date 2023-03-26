using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class Entity
    {
        public int Id { get; set; }
        private List<Component> components = new List<Component>();
        public Entity Parent { get; set; }

        private List<Entity> children = new List<Entity>();

        public void AddComponent(Component component)
        {
            components.Add(component);
            component.Entity = this;
        }

        //delete entity and all child entities
        public virtual void DeleteEntity()
        { 
            //note that this is just a prototype implementation. If any additional systems/components exist, this method has to be extended or overwritten.
          
            //For correct removal, call .Deregister() on the respective systems.
            
            foreach(Component component in components)
            {
                component.DeleteComponent();
            }

            components.Clear(); //let's hope C#'s garbage collector handles this correctly, otherwise it's MemoryLeak time :)
            components = null;

            foreach (Entity child in children)
            {
                child.DeleteEntity();
            }

            if (Parent != null)
            {
                Parent.children.Remove(this);
            }
        }

        public void DeleteChild(Entity child)
        {
            child.DeleteEntity();
        }

        public T GetComponent<T>() where T : Component
        {
            if (components != null)
            {
                foreach (Component component in components)
                {
                    if (component.GetType().Equals(typeof(T)))
                    {
                        return (T)component;
                    }
                }
            }
            return null;
        }

        public bool HasComponent<T>() where T : Component
        {
            if (GetComponent<T>() != null) return true;
            return false;
        }

        public List<T> GetChildrenOfType<T>() where T : Entity
        {
            List<T> foundChildren = new List<T>();
            foreach (Entity entity in children)
            {
                if (entity.GetType().Equals(typeof(T)))
                {
                    foundChildren.Add((T)entity);
                }
            }
            return foundChildren;
        }

        public void AddChild(Entity entity) { 
            children.Add(entity);
            entity.Parent = this;
        }
        public void AddChildren(List<Entity> entities)
        {
            foreach(Entity entity in entities)
            {
                AddChild(entity);
            }
        }

        /// <summary>
        /// Computes the size of the Entity in world coordinates.
        /// Returns zero if the entity has no transform or no sprite component attached
        /// </summary>
        public Vector2 Size()
        {
            Vector2 size = Vector2.Zero;
            if (HasComponent<Transform>() && HasComponent<Sprite>()) //check for existence because the object might have been destroyed
            {
                Transform transform = GetComponent<Transform>().ToWorld();
                Sprite sprite = GetComponent<Sprite>();
                if (sprite != null && transform != null)
                {
                    size = new Vector2(sprite.SourceRectangle.Width * transform.Scale.X, sprite.SourceRectangle.Height * transform.Scale.Y);
                }
            }
            return size;
        }
    }
}
