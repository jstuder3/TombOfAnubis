using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class PlayerInputSystem : BaseSystem<PlayerInput>
    {
        public PlayerInputSystem() { }

        public override void Update(GameTime deltaTime)
        {
            
            float maxSpeed = 100; // TODO: Move to components

            for (int playerIdx = 0; playerIdx < components.Count; playerIdx++) {
                Player player = (Player)components[playerIdx].Entity;
                Transform transform = player.GetComponent<Transform>();
                //isWalking = false;
                float deltaTimeSeconds = (float)deltaTime.ElapsedGameTime.TotalSeconds;

                //if (!isTrapped)
                //{
                PlayerActions[] currentActions = InputController.GetActionsOfCurrentPlayer(playerIdx);
                Vector2 newPosition = transform.Position;

                if (currentActions.Contains(PlayerActions.WalkLeft))
                {
                    newPosition.X -= maxSpeed * deltaTimeSeconds;
                    //isWalking = true;
                    //orientation = Orientation.West;
                }

                if (currentActions.Contains(PlayerActions.WalkRight))
                {
                    newPosition.X += maxSpeed * deltaTimeSeconds;
                    //isWalking = true;
                    //orientation = Orientation.East;
                }

                if (currentActions.Contains(PlayerActions.WalkUp))
                {
                    newPosition.Y -= maxSpeed * deltaTimeSeconds;
                    //isWalking = true;
                    //orientation = Orientation.North;
                }

                if (currentActions.Contains(PlayerActions.WalkDown))
                {
                    newPosition.Y += maxSpeed * deltaTimeSeconds;
                    //isWalking = true;
                    //orientation = Orientation.South;
                }

                //if (currentActions.Contains(PlayerActions.UseObject))
                //{
                //    //check which objects are currently colliding with the player. then check that they are in the orientation the player is looking in
                //    // if the targeted object is iteractable (i.e. an artefact or an item dispenser, or anything else that can be interacted with), trigger the corresponding interaction

                //    //if item dispenser: give player an item of the type corresponding to the dispenser, assuming the player can carry such an item

                //    //if a button: trigger the interaction corresponding to that button

                //    //if a player: check if trapped/unconscious, then check if the current player can free/resurrect that player

                //}

                transform.Position = newPosition;

                //}
            }
        }
    }
}
