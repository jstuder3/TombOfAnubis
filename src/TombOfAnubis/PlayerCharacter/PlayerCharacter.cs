using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TombOfAnubis.CollisionSystem;
using TombOfAnubis.InventorySystem;

namespace TombOfAnubis.PlayerCharacter
{

    enum PlayerState
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

    enum PlayerActions
    {
        WalkLeft,
        WalkRight,
        WalkUp,
        WalkDown,
        UseObject
    }

    enum PlayerType
    {
        Player,
        Anubis,
        Enemy
    }

    enum Orientation
    {
        North,
        East,
        South,
        West
    }

    internal class PlayerCharacter : ICollidable
    {
        PlayerType type;
        PlayerState state;
        float health;
        float maxHealth;
        float speed;
        float maxSpeed;
        Vector2 position;
        InventoryManager inventory;
        Texture2D texture;
        InputController inputController;
        Orientation orientation = Orientation.South;
        bool isWalking = false;
        bool isTrapped = false;

        int playerNumber = -1;

        private Collider _collider; //this is fucking disgusting about C# interfaces: you need an extra variable to implement an interface property... wtf?
        public Collider collider { get => _collider; set => _collider = value; } 

        public PlayerCharacter(PlayerType type, float maxHealth, float maxSpeed, float posX, float posY, Texture2D texture, int playerNumber, InputController inputController)
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
            this.inputController = inputController;
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

        public void Update(float deltaTime)
        {
            isWalking = false;

            if (!isTrapped)
            {
                PlayerActions[] currentActions = inputController.GetActionsOfCurrentPlayer(playerNumber);

                if (currentActions.Contains(PlayerActions.WalkLeft))
                {
                    position.X -= speed * deltaTime;
                    isWalking = true;
                    orientation = Orientation.West;
                }

                if (currentActions.Contains(PlayerActions.WalkRight))
                {
                    position.X += speed * deltaTime;
                    isWalking = true;
                    orientation = Orientation.East;
                }

                if (currentActions.Contains(PlayerActions.WalkUp))
                {
                    position.Y -= speed * deltaTime;
                    isWalking = true;
                    orientation = Orientation.North;
                }

                if (currentActions.Contains(PlayerActions.WalkDown))
                {
                    position.Y += speed * deltaTime;
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
            }

        }

        public void Draw()
        {
            //draw the current player at its current position
            //the player is drawn based on its orientation, its walking state, its trapped state 
            throw new NotImplementedException();
        }

    }
}
