﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;
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
                movement.State = MovementState.Idle;
                float deltaTimeSeconds = (float)deltaTime.ElapsedGameTime.TotalSeconds;

                if (movement.CanMove())
                {
                    int playerID = character.GetComponent<Player>().PlayerID;
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

                    if (currentActions.Contains(PlayerAction.UseObject))
                    {
                        character.GetComponent<Inventory>().GetFullItemSlot()?.TryUseItem();
                    }

                    transform.Position = newPosition;

                }
            }
        }
    }
}
