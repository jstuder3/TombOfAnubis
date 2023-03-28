using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TombOfAnubis
{
    public enum DispenserType
    {
        BodyPowerup,
        WisdomPowerup,
        ResurrectionPowerup,
        None
    }
    public class Dispenser : Entity
    {
        DispenserType dispenserType = DispenserType.None;
        double lastUsedTime;
        float cooldown;
        public Dispenser(Vector2 position, Vector2 scale, Texture2D texture, DispenserType dispenserType)
        {
            Transform transform = new Transform(position, scale);
            AddComponent(transform);

            Sprite sprite = new Sprite(texture, 1);
            AddComponent(sprite);

            RectangleCollider collider = new RectangleCollider(position, Size());
            AddComponent(collider);

            this.dispenserType = dispenserType;

            // set up cooldown
            this.lastUsedTime = 0.0;
            this.cooldown = 2f;
        }

        public bool TryGiveItem(Inventory inventory, double currentTime)
        {
            //only give an item if the dispenser isn't on cooldown
            if (IsOnCooldown(currentTime)) return false;

            InventorySlot emptyItemSlot;

            Random random = new Random();

            //if there is space, put an item in the empty slot according to which type of dispenser this is

            if(dispenserType == DispenserType.BodyPowerup)
            {
                emptyItemSlot = inventory.GetEmptyBodyPowerupSlot();
                if (emptyItemSlot == null) return false;

                switch(random.Next(0, 3))
                {
                    case (0):
                        emptyItemSlot.Item = new InventoryItem(ItemType.Speedup);
                        break;
                    case (1):
                        emptyItemSlot.Item = new InventoryItem(ItemType.Speedup);
                        break;
                    case (2):
                        emptyItemSlot.Item = new InventoryItem(ItemType.Speedup);
                        break;
                    default:
                        return false;
                }
                Console.WriteLine("Put BodyPowerup in somebody's inventory!");
            }
            else if(dispenserType == DispenserType.WisdomPowerup)
            {
                emptyItemSlot = inventory.GetEmptyWisdomPowerupSlot();
                if (emptyItemSlot == null) return false;

                switch (random.Next(0, 1))
                {
                    case (0):
                        emptyItemSlot.Item = new InventoryItem(ItemType.IncreaseViewDistance);
                        break;
                    default:
                        return false; 
                }
                Console.WriteLine("Put WisdomPowerup in somebody's inventory!");

            }
            else if(dispenserType == DispenserType.ResurrectionPowerup)
            {
                emptyItemSlot = inventory.GetEmptyResurrectionSlot();
                if (emptyItemSlot == null) return false;

                emptyItemSlot.Item = new InventoryItem(ItemType.Resurrection);
                Console.WriteLine("Put ResurrectionPowerup in somebody's inventory!");
            }
            else
            {
                return false;
            }

            // put dispenser on cooldown
            lastUsedTime = currentTime;

            return true;

        }

        public bool IsOnCooldown(double currentTime)
        {
            return lastUsedTime + cooldown > currentTime;
        }

    }
}
