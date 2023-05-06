using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace TombOfAnubis
{
    public class Entity
    {
        private List<Component> components = new List<Component>();
        public Entity Parent { get; set; }

        private List<Entity> children = new List<Entity>();

        public void AddComponent(Component component)
        {
            components.Add(component);
            component.Entity = this;
        }

        public void RemoveComponentWithoutDeleting(Component component)
        {
            if (components != null)
            {
                components.Remove(component);
            }
        }

        public void RemoveComponent(Component component)
        {
            if (components != null)
            {
                components.Remove(component);
                component.Delete();
            }
        }

        /// <summary>
        /// Delete entity, its components and children. Removes this entity from its parent.children list.
        /// </summary>
        public void Delete()
        {
            if (components != null)
            {
                foreach (Component component in components)
                {
                    component.Delete();
                }

                components.Clear();
                components = null;
            }
            List<Entity> childrenToRemove = new List<Entity>();
            foreach (Entity child in children)
            {
                childrenToRemove.Add(child);
            }
            foreach (Entity child in childrenToRemove)
            {
                child.Delete();
            }

            Parent?.children.Remove(this);
        }

        public T GetComponent<T>() where T : Component
        {
            if (components != null)
            {
                foreach (Component component in components)
                {
                    if ( component.Visible() && component.GetType().Equals(typeof(T)))
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

        public void DeleteChildrenOfType(Type type)
        {
            List<Entity> foundChildren = new List<Entity>();
            foreach (Entity entity in children)
            {
                if (entity.GetType().Equals(type))
                {
                    foundChildren.Add(entity);
                }
            }
            foreach(Entity child in foundChildren)
            {
                children.Remove(child);
                child.Delete();
            }
        }

        public void DeleteChildrenExcept( List<Type> exceptions)
        {
            HashSet<Type> childrenTypes = new HashSet<Type>();
            foreach (Entity child in children)
            {
                if (!exceptions.Contains(child.GetType()))
                {
                    childrenTypes.Add(child.GetType());
                }
            }
            foreach (Type type in childrenTypes)
            {
                List<Entity> foundChildren = new List<Entity>();
                foreach (Entity entity in children)
                {
                    if (entity.GetType().Equals(type))
                    {
                        if (entity.GetType().Equals(typeof(Particle)))
                        {
                            if (!exceptions.Contains(((Particle)entity).ParentEmitter.Entity.GetType()))
                            {
                                foundChildren.Add(entity);
                            }
                        }
                        else
                        {
                            foundChildren.Add(entity);
                        }
                    }
                }
                foreach (Entity child in foundChildren)
                {
                    children.Remove(child);
                    child.Delete();
                }
            }
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

        public List<T> GetComponentsOfType<T>() where T: Component
        {
            List<T> foundComponents = new List<T>();
            foreach(Component component in components)
            {
                if (component.GetType().Equals(typeof(T))) {
                    foundComponents.Add((T)component);
                }
            }
            return foundComponents;
        }

        public void AddChild(Entity entity)
        {
            children.Add(entity);
            entity.Parent = this;
        }
        public void AddChildren(List<Entity> entities)
        {
            foreach (Entity entity in entities)
            {
                AddChild(entity);
            }
        }

        private Vector2 VisibleSize(Visibility visibility, bool world)
        {
            Vector2 size = Vector2.Zero;
            
            Transform transform = null;
            Sprite sprite = null;


            foreach(Transform candidate in GetComponentsOfType<Transform>())
            {
                if(visibility == candidate.Visibility || candidate.Visibility == Visibility.Both)
                {
                    if (world)
                    {
                        transform = candidate.ToWorld();
                    }
                    else
                    {
                        transform = candidate;
                    }

                }
            }
            foreach (Sprite candidate in GetComponentsOfType<Sprite>())
            {
                if (visibility == candidate.Visibility || candidate.Visibility == Visibility.Both)
                {
                    sprite = candidate;
                }
            }

            if (sprite != null && transform != null)
            {
                size = new Vector2(sprite.SourceRectangle.Width * transform.Scale.X, sprite.SourceRectangle.Height * transform.Scale.Y);
            }
            
            return size;
        }

        public void Initialize()
        {
            Transform transform = GetComponent<Transform>();

            if(transform != null)
            {
                foreach(Transform candidate in GetComponentsOfType<Transform>())
                {
                    if (!transform.Equals(candidate) && candidate.Visibility == Visibility.Minimap)
                    {
                        Vector2 entityCenter = CenterPosition();

                        Vector2 minimapEntityCenter = MinimapCenterPosition();

                        candidate.Position = transform.Position + entityCenter - minimapEntityCenter;
                    }
                }
            }
        }


        /// <summary>
        /// Computes the size of the Entity in world coordinates.
        /// </summary>
        /// <returns>Zero if the entity has no transform or no sprite component of Game visibility attached</returns>
        public Vector2 Size()
        {
            return VisibleSize(Visibility.Game, true);
        }

        /// <summary>
        /// Computes the size of the Entity on the minimap in world coordinates.
        /// </summary>
        /// <returns>Zero if the entity has no transform or no sprite component of Minimap visibility attached</returns>
        public Vector2 MinimapSize()
        {
            return VisibleSize(Visibility.Minimap, true);
        }

        /// <summary>
        /// Computes the size of the Sprite texture without scaling.
        /// </summary>
        /// <returns>Zero if the entity has no transform or no sprite component of Game visibility attached</returns>
        public Vector2 SpriteSize()
        {
            Visibility visibility = Visibility.Game;
            Sprite sprite = null;
            Vector2 size = Vector2.Zero;

            foreach (Sprite candidate in GetComponentsOfType<Sprite>())
            {
                if (visibility == candidate.Visibility || candidate.Visibility == Visibility.Both)
                {
                    sprite = candidate;
                }
            }
            if (sprite != null)
            {
                size = new Vector2(sprite.SourceRectangle.Width, sprite.SourceRectangle.Height);
            }
            return size;
        }

        /// <summary>
        /// The position of the center of this entity in world coordinates.
        /// </summary>
        /// <returns></returns>
        public Vector2 CenterPosition()
        {
            return VisibleCenterPosition(Visibility.Game);
        }
        /// <summary>
        /// The minimap position of the center of this entity in world coordinates.
        /// </summary>
        /// <returns></returns>
        public Vector2 MinimapCenterPosition()
        {
            return VisibleCenterPosition(Visibility.Minimap);
        }

        /// <summary>
        /// The position of the top left corner of this entity in world coordinates.
        /// </summary>
        /// <returns></returns>
        public Vector2 TopLeftCornerPosition()
        {
            return VisibleTopLeftCornerPosition(Visibility.Game);
        }

        /// <summary>
        /// The minimap position of the top left corner of this entity in world coordinates.
        /// </summary>
        /// <returns></returns>
        public Vector2 TopLeftCornerMinimapPosition()
        {
            return VisibleTopLeftCornerPosition(Visibility.Minimap);
        }
        /// <summary>
        /// The position where the entity is drawn by the sprite system
        /// </summary>
        /// <returns></returns>
        public Vector2 DrawingPosition()
        {
            Vector2 worldOrigin = Session.GetInstance().World.Origin;
            Vector2 worldScale = Session.GetInstance().World.Scale;

            if (Session.GetInstance().Visibility == Visibility.Game)
            {
                return worldOrigin + worldScale * TopLeftCornerPosition();
            }
            else
            {
                return worldOrigin + worldScale * TopLeftCornerMinimapPosition();
            }
        }
        /// <summary>
        /// The Size of the entity drawn by the sprite system
        /// </summary>
        /// <returns></returns>
        public Vector2 DrawingSize()
        {
            Vector2 worldScale = Session.GetInstance().World.Scale;

            if (Session.GetInstance().Visibility == Visibility.Game)
            {
                return worldScale * Size();
            }
            else
            {
                return worldScale * MinimapSize();
            }
        }

        private Vector2 VisibleCenterPosition(Visibility visibility)
        {
            foreach (Transform candidate in GetComponentsOfType<Transform>())
            {
                if (visibility == candidate.Visibility || candidate.Visibility == Visibility.Both)
                {
                    return candidate.ToWorld().Position + VisibleSize(visibility, true) / 2f;
                }
            }
            return Vector2.Zero;
        }

        private Vector2 VisibleTopLeftCornerPosition(Visibility visibility)
        {
            foreach (Transform candidate in GetComponentsOfType<Transform>())
            {
                if (visibility == candidate.Visibility || candidate.Visibility == Visibility.Both)
                {
                    return candidate.ToWorld().Position;
                }
            }
            return Vector2.Zero;
        }
    }
}
