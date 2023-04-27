using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TombOfAnubis
{
    public class InputSystem : BaseSystem<Input>
    {
        private GameScreenManager ScreenManager;
        public InputSystem(GameScreenManager screenManager) 
        {
            ScreenManager = screenManager;
        }

        public override void Update(GameTime deltaTime)
        {
            if (InputController.IsPauseTriggered())
            {
                ScreenManager.AddScreen(new PauseMenuScreen());
            }
            foreach (Input input in GetComponents())
            {
                Entity entity = input.Entity;
                Transform transform = entity.GetComponent<Transform>();
                Movement movement = entity.GetComponent<Movement>();
                RectangleCollider collider = entity.GetComponent<RectangleCollider>();
                float deltaTimeSeconds = (float)deltaTime.ElapsedGameTime.TotalSeconds;

                if (movement.CanMove())
                {
                    movement.State = MovementState.Idle;
                    int playerID = entity.GetComponent<Player>().PlayerID;
                    HashSet<PlayerAction> currentActions = InputController.PlayerActions[playerID];

                    Vector2 newPosition = transform.Position;

                    Vector2 movementVector = InputController.PlayerMovementDirections[playerID];
                    if(movementVector.Length() > 0)
                    {
                        movement.State = MovementState.Walking;

                        Vector2 movementVectorNorm = new Vector2(movementVector.X, movementVector.Y);
                        movementVectorNorm.Normalize();

                        List<float> costhetas = new List<float>() { 
                            Vector2.Dot(movementVectorNorm, InputController.UP),
                            Vector2.Dot(movementVectorNorm, InputController.RIGHT),
                            Vector2.Dot(movementVectorNorm, InputController.DOWN),
                            Vector2.Dot(movementVectorNorm, InputController.LEFT)
                        };
                        List<Orientation> orientations = new List<Orientation>()
                        {
                            Orientation.Up, Orientation.Right, Orientation.Down, Orientation.Left,
                        };

                        movement.Orientation = orientations[costhetas.IndexOf(costhetas.Max())];
                        newPosition += movementVector * movement.MaxSpeed * deltaTimeSeconds;

                    }

                    //use item, if there is one
                    if (currentActions.Contains(PlayerAction.UseObject))
                    {
                        entity.GetComponent<Inventory>().GetFullItemSlot()?.TryUseItem();
                    }
                    //drop item, if there is one
                    if (currentActions.Contains(PlayerAction.DropObject)) {
                        InventorySlot inventorySlot = entity.GetComponent<Inventory>().GetFullItemSlot();
                        if (inventorySlot != null)
                        {
                            inventorySlot.DropItem();
                        }
                    }
                    transform.Position = newPosition;

                }
                //if the player has self-revive item, he can also use it when he's dead
                //use item, if there is one
                else if (InputController.PlayerActions[entity.GetComponent<Player>().PlayerID].Contains(PlayerAction.UseObject) && entity.GetComponent<Inventory>().GetFullItemSlot()?.Item.ItemType == ItemType.Resurrection)
                {
                    entity.GetComponent<Inventory>().GetFullItemSlot()?.TryUseItem();
                }

            }
        }
    }
}
