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

        public bool HandleCollision(ICollidable other) { 
            throw new NotImplementedException();
        }

        public void Update(float deltaTime)
        {
            throw new NotImplementedException();
        }

        public void Draw()
        {
            throw new NotImplementedException();
        }

    }
}
