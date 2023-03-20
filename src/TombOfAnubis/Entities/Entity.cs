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

        public T GetComponent<T>() where T : Component
        {
            foreach (Component component in components)
            {
                if (component.GetType().Equals(typeof(T)))
                {
                    return (T)component;
                }
            }
            return null;
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
    }
}
