using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class AISystem : BaseSystem<Input>
    {
        public AISystem() { }

        public override void Update(GameTime deltaTime)
        {

            //Console.WriteLine("start update AnubisAISystem");
            Random rnd = new Random();
            
            foreach (Input input in components)
            {
                //Console.WriteLine("type: " + input.Entity.GetType());
                if (!(input.Entity is Anubis))
                {
                    continue;
                }
                //Console.WriteLine("reached anubis section");
                Anubis anubis = (Anubis)input.Entity;
                Transform transform = anubis.GetComponent<Transform>();
                Movement movement = anubis.GetComponent<Movement>();
                movement.IsWalking = false;
                float deltaTimeSeconds = (float)deltaTime.ElapsedGameTime.TotalSeconds;

                int direc = rnd.Next(1, 5);
                Console.WriteLine("direction: " + direc);
                bool use_random_directions = true;

                if(use_random_directions && !movement.IsTrapped)
                {
                    Vector2 newPosition = transform.Position;

                    if (direc == 3)
                    {
                        //walk up
                        newPosition.Y -= movement.MaxSpeed * deltaTimeSeconds;
                        movement.IsWalking = true;
                        movement.Orientation = Orientation.Up;

                    } else if (direc == 2) {
                        // walk right
                        newPosition.X += movement.MaxSpeed * deltaTimeSeconds;
                        movement.IsWalking = true;
                        movement.Orientation = Orientation.Right;
                    } else if (direc == 1)
                    {
                        //walk down
                        newPosition.Y += movement.MaxSpeed * deltaTimeSeconds;
                        movement.IsWalking = true;
                        movement.Orientation = Orientation.Down;
                    } else
                    {
                        //walk left
                        newPosition.Y -= movement.MaxSpeed * deltaTimeSeconds;
                        movement.IsWalking = true;
                        movement.Orientation = Orientation.Left;
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

                    return;

                }
            }
        }
    }
}
