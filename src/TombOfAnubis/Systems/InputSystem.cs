using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace TombOfAnubis
{
    public class InputSystem : BaseSystem<Input>
    {
        public InputSystem() { }

        public override void Update(GameTime deltaTime)
        {
            foreach (Input input in components)
            {
                Character character = (Character)input.Entity;
                Transform transform = character.GetComponent<Transform>();
                Movement movement = character.GetComponent<Movement>();
                RectangleCollider collider = character.GetComponent<RectangleCollider>();
                movement.IsWalking = false;
                float deltaTimeSeconds = (float)deltaTime.ElapsedGameTime.TotalSeconds;

                if (!movement.IsTrapped)
                {
                    PlayerActions[] currentActions = InputController.GetActionsOfCurrentPlayer(character.GetComponent<Player>().PlayerID);

                    Vector2 newPosition = transform.Position;
                    Vector2 movementVector = Vector2.Zero;

                    if (currentActions.Contains(PlayerActions.WalkLeft))
                    {
                        //newPosition.X -= movement.MaxSpeed * deltaTimeSeconds;
                        movementVector.X = -1f;
                        movement.IsWalking = true;
                        movement.Orientation = Orientation.Left;
                    }

                    if (currentActions.Contains(PlayerActions.WalkRight))
                    {
                        //newPosition.X += movement.MaxSpeed * deltaTimeSeconds;
                        movementVector.X = 1f;
                        movement.IsWalking = true;
                        movement.Orientation = Orientation.Right;
                    }

                    if (currentActions.Contains(PlayerActions.WalkUp))
                    {
                        //newPosition.Y -= movement.MaxSpeed * deltaTimeSeconds;
                        movementVector.Y = -1f;
                        movement.IsWalking = true;
                        movement.Orientation = Orientation.Up;
                    }

                    if (currentActions.Contains(PlayerActions.WalkDown))
                    {
                        //newPosition.Y += movement.MaxSpeed * deltaTimeSeconds;
                        movementVector.Y = 1f;
                        movement.IsWalking = true;
                        movement.Orientation = Orientation.Down;
                    }

                    if (movementVector.Length() > 0)
                    {
                        movementVector.Normalize();
                        newPosition += movementVector * movement.MaxSpeed * deltaTimeSeconds;
                    }

                    //if (currentActions.Contains(PlayerActions.UseObject))
                    //{
                    //    //check which objects are currently colliding with the player. then check that they are in the orientation the player is looking in
                    //    // if the targeted object is iteractable (i.e. an artefact or an item dispenser, or anything else that can be interacted with), trigger the corresponding interaction

                    //    //if item dispenser: give player an item of the type corresponding to the dispenser, assuming the player can carry such an item

                    //    //if a button: trigger the interaction corresponding to that button

                    //    //if a player: check if trapped/unconscious, then check if the current player can free/resurrect that player

                    //}

                    if (currentActions.Contains(PlayerActions.UseObject))
                    {
                        character.GetComponent<Inventory>().GetFullItemSlot()?.TryUseItem();
                    }

                    transform.Position = newPosition;

                }
            }
        }
    }
}
