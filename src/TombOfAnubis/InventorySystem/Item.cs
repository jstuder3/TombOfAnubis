using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TombOfAnubis.CollisionSystem;

namespace TombOfAnubis.InventorySystem
{
    enum ItemTypes
    {
        None,
        Speedup
    }

    internal class Item : ICollidable
    {
        ItemTypes itemType;
        public string Name { get; set; }
        public string Description { get; set; }
        private Collider _collider;
        public Collider collider { get => _collider; set => _collider = value; }

        public bool isInWorld = false;
        public bool isInInventory = false;
        public Item() { }

        public void handleCollision(Collider other)
        {
            throw new NotImplementedException();
        }

        public void handleCollision(ICollidable other)
        {
            throw new NotImplementedException();
        }
    }
}
