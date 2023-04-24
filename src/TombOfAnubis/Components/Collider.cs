using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace TombOfAnubis
{
    public class Collider : Component
    {
        public List<Collider> OverlappingColliders = new List<Collider>();

        public bool IsStaticCollider = false;

        public Collider(bool isStatic)
        {
            IsStaticCollider = isStatic;
            CollisionSystem.Register(this);
        }

        public virtual void Update(GameTime gameTime) {
            ClearOverlap();
        }

        public virtual void ClearOverlap()
        {
            OverlappingColliders.Clear();
        }
        public virtual void AddOverlap(Collider other)
        {
            OverlappingColliders.Add(other);
        }

        public override void Delete()
        {
            base.Delete();
            CollisionSystem.Deregister(this);
        }

        public virtual bool IsStatic()
        {
            return IsStaticCollider;
        }



    }
}
