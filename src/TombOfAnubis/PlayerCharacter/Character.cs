using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{

    public enum PlayerState
    {
        Idle,
        Walking,
        Running,
        Jumping,
        Falling,
        Hiding,
        Climbing,
        Dead,
        Trapped
    }

    public enum PlayerActions
    {
        WalkLeft,
        WalkRight,
        WalkUp,
        WalkDown,
        UseObject
    }

    public enum PlayerType
    {
        Player,
        Anubis,
        Enemy
    }

    public enum Orientation
    {
        North,
        East,
        South,
        West
    }

    internal class Character : ICollidable
    {
        PlayerType type;
        PlayerState state;
        float health;
        float maxHealth;
        float speed;
        float maxSpeed;
        private Vector2 position;
        public Vector2 Position { get { return position; } set {  position = value; } }
        InventoryManager inventory;
        Texture2D texture;
        Orientation orientation = Orientation.South;
        bool isWalking = false;
        bool isTrapped = false;

        int playerNumber = -1;

        private Collider _collider; //this is fucking disgusting about C# interfaces: you need an extra variable to implement an interface property... wtf?
        public Collider collider { get => _collider; set => _collider = value; } 

        public Character(PlayerType type, float maxHealth, float maxSpeed, float posX, float posY, Texture2D texture, int playerNumber)
        {
            this.type = type;
            this.state = PlayerState.Idle;
            this.health = maxHealth;
            this.maxHealth = maxHealth;
            this.speed = 0;
            this.maxSpeed = maxSpeed;
            position = new Vector2(posX, posY);
            this.inventory = new InventoryManager();
            this.texture = texture;
            this.collider = new RectangleCollider(position.X, position.Y, texture.Width, texture.Height);
            this.playerNumber = playerNumber;
        }

        public void ReduceHealth(float damage)
        {
            Debug.Assert(damage > 0);
            health = MathF.Max(0, health - damage);
        }

        public void IncreaseHealth(float healing)
        {
            Debug.Assert(healing > 0);
            health = MathF.Min(maxHealth, health + healing);
        }

        public void HandleCollision(ICollidable other)
        {
            throw new NotImplementedException();
        }

        public bool GetIsTrapped()
        {
            return isTrapped;
        }

        public void PutInTrap()
        {
            isTrapped = true;
        }

        public void FreeFromTrap()
        {
            isTrapped = false;
        }

        public void Update(GameTime deltaTime)
        {
            isWalking = false;
            float deltaTimeSeconds = (float)deltaTime.ElapsedGameTime.TotalSeconds;

            if (!isTrapped)
            {
                PlayerActions[] currentActions = InputController.GetActionsOfCurrentPlayer(playerNumber);

                if (currentActions.Contains(PlayerActions.WalkLeft))
                {
                    position.X -= maxSpeed * deltaTimeSeconds;
                    isWalking = true;
                    orientation = Orientation.West;
                }

                if (currentActions.Contains(PlayerActions.WalkRight))
                {
                    position.X += maxSpeed * deltaTimeSeconds;
                    isWalking = true;
                    orientation = Orientation.East;
                }

                if (currentActions.Contains(PlayerActions.WalkUp))
                {
                    position.Y -= maxSpeed * deltaTimeSeconds;
                    isWalking = true;
                    orientation = Orientation.North;
                }

                if (currentActions.Contains(PlayerActions.WalkDown))
                {
                    position.Y += maxSpeed * deltaTimeSeconds;
                    isWalking = true;
                    orientation = Orientation.South;
                }

                if (currentActions.Contains(PlayerActions.UseObject))
                {
                    //check which objects are currently colliding with the player. then check that they are in the orientation the player is looking in
                    // if the targeted object is iteractable (i.e. an artefact or an item dispenser, or anything else that can be interacted with), trigger the corresponding interaction

                    //if item dispenser: give player an item of the type corresponding to the dispenser, assuming the player can carry such an item

                    //if a button: trigger the interaction corresponding to that button

                    //if a player: check if trapped/unconscious, then check if the current player can free/resurrect that player

                }
                Console.WriteLine("Position: " + position.X + ", " + position.Y);
                Console.Write("Actions: ");
                foreach(PlayerActions action in currentActions)
                {
                    Console.Write(action.ToString());
                }
                Console.Write("\n");
            }


        }

        public void Draw(SpriteBatch spriteBatch, Vector2 mapOriginPosition)
        {
            //draw the current player at its current position
            //the player is drawn based on its orientation, its walking state, its trapped state 
            spriteBatch.Draw(texture, mapOriginPosition + position, null, Color.White, 0f, new Vector2(texture.Width / 2, texture.Height / 2), Vector2.One * 0.25f, SpriteEffects.None, 0f);
        }

    }
}
