using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class AISystem : BaseSystem<AI>
    {
        public Scene Scene {  get; set; }
        public AISystem(Scene scene) { 
            Scene = scene;
        }

        public override void Update(GameTime deltaTime)
        {

            //Console.WriteLine("start update AnubisAISystem");
            Random rnd = new Random();
            
            foreach (AI ai in components)
            {
                //Console.WriteLine("reached anubis section");
                Entity entity = ai.Entity;
                Transform transform = entity.GetComponent<Transform>();
                Movement movement = entity.GetComponent<Movement>();

                MovementGraph movementGraph = ai.MovementGraph;
                List<Character> characters = Scene.GetChildrenOfType<Character>();
                List<Artefact> artefacts = Scene.GetChildrenOfType<Artefact>();

                // Node anubis = movementGraph.GetNode(entity)
                // Node c1 = movementGraph.GetNode(characters[0])
                // List<Vector2> currentPath = movementGraph.CreatePath(anubis, c1)
                // Vector2 currentTarget = currentPath[0]

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
