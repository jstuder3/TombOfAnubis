﻿using Microsoft.Xna.Framework;
using System; using System.Diagnostics;

namespace TombOfAnubis
{
    public enum Orientation
    {
        Up,
        Right,
        Down,
        Left
    }
    public enum MovementState
    {
        Idle,
        Walking,
        Running,
        Jumping,
        Falling,
        Hiding,
        Climbing,
        Dead,
        Trapped,
        Stunned
    }
    public class Movement : Component
    {
        public int BaseMovementSpeed { get; set; }
        public int MaxSpeed { get; set; }
        public Orientation Orientation { get; set; }

        public MovementState State { get; set; }

        public bool HiddenFromAnubis = false;

        public float MultiplicativeSpeedModifier = 1f;
        public float AdditiveSpeedModifier = 0f;

        public Movement(int maxSpeed, MovementState state = MovementState.Idle)
        {
            BaseMovementSpeed = maxSpeed;
            MaxSpeed = maxSpeed;
            Orientation = Orientation.Up;
            State = state;
            MovementSystem.Register(this);
        }

        public override void Delete()
        {
            MovementSystem.Deregister(this);
        }

        public bool IsWalking()
        {
            return State == MovementState.Walking;
        }
        public bool IsTrapped()
        {
            return State == MovementState.Trapped;
        }

        public bool CanMove()
        {
            return State != MovementState.Trapped && State != MovementState.Stunned && State != MovementState.Dead;
        }

        public bool IsVisibleToAnubis()
        {
            return State != MovementState.Trapped && State != MovementState.Dead && State != MovementState.Hiding && !HiddenFromAnubis;
        }

        public Vector2 GetForwardVector()
        {
            switch(Orientation)
            {
                case Orientation.Left:
                    return new Vector2(-1, 0);
                case Orientation.Right:
                    return new Vector2(1, 0);
                case Orientation.Down:
                    return new Vector2(0, 1);
                case Orientation.Up:
                    return new Vector2(0, -1);
            }
            Debug.WriteLine("Error: Orientation of character not found. GetForwardVector() returns faulty vector!");
            return new Vector2(0, 0);
        }

        public void UpdateMovementSpeed()
        {
            MaxSpeed = (int)((BaseMovementSpeed + AdditiveSpeedModifier) * MultiplicativeSpeedModifier);
        }

    }
}
