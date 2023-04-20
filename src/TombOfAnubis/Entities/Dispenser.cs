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
            Transform transform = new Transform(position, scale, Visibility.Game);
            AddComponent(transform);

            Transform minimapTransform = new Transform(position, 4f * scale, Visibility.Minimap);
            AddComponent(minimapTransform);

            Sprite sprite = new Sprite(texture, 1, Visibility.Both);
            AddComponent(sprite);

            RectangleCollider collider = new RectangleCollider(position, Size(Visibility.Game));
            AddComponent(collider);

            Discovery discovery = new Discovery();
            AddComponent(discovery);

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

            emptyItemSlot = inventory.GetEmptyItemSlot();
            if (emptyItemSlot == null)
            {
                Console.WriteLine("No more space in inventory!");
                return false;
            }

            if(dispenserType == DispenserType.BodyPowerup)
            {

                switch(random.Next(0, 2))
                {
                    case 0:
                        emptyItemSlot.Item = new InventoryItem(ItemType.Speedup, emptyItemSlot.Entity);
                        Console.WriteLine("Put Speedup in somebody's inventory!");
                        break;
                    case 1:
                        emptyItemSlot.Item = new InventoryItem(ItemType.Fist, emptyItemSlot.Entity);
                        Console.WriteLine("Put Fist in somebody's inventory!");
                        break;
                    case 2:
                        emptyItemSlot.Item = new InventoryItem(ItemType.HidingCloak, emptyItemSlot.Entity);
                        Console.WriteLine("Put HidingCloak in somebody's inventory!");
                        break;
                    default:
                        return false;
                }
                //Console.WriteLine("Put BodyPowerup in somebody's inventory!");
            }
            else if(dispenserType == DispenserType.WisdomPowerup)
            {

                switch (random.Next(0, 1))
                {
                    case (0):
                        emptyItemSlot.Item = new InventoryItem(ItemType.IncreaseViewDistance, emptyItemSlot.Entity);
                        Console.WriteLine("Put IncreaseViewDistance in somebody's inventory!");
                        break;
                    default:
                        return false; 
                }

            }
            else if(dispenserType == DispenserType.ResurrectionPowerup)
            {

                emptyItemSlot.Item = new InventoryItem(ItemType.Resurrection, emptyItemSlot.Entity);
                Console.WriteLine("Put ResurrectionPowerup in somebody's inventory!");
            }
            else
            {
                Console.WriteLine("Unknown dispenser type!");
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
