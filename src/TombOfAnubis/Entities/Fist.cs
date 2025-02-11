﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Sdcb.FFmpeg.Utils;
using System; using System.Diagnostics;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO.Compression;

namespace TombOfAnubis
{
    public class Fist : Entity
    {
        private Vector2 startPosition;
        private float velocity;
        private float range;

        public static Vector2 Scale = new Vector2(0.6f, 0.6f);
        public static Texture2D Texture;
        public static List<AnimationClip> AnimationClipList;
        //private static 

        public Fist(Vector2 position, Vector2 forwardVector)
        {
            Transform transform = new Transform(position, Scale, Visibility.Game);
            AddComponent(transform);

            if (Texture == null)
            {
                Debug.WriteLine("Error: Trying to instantiate fist without having first loaded its texture.");
                return;
            }

            Animation animation = new Animation(AnimationClipList, Visibility.Game);
            AddComponent(animation);

            Debug.WriteLine(forwardVector.ToString());

            // select sprite based on forward vector
            if(forwardVector.X == 1 && forwardVector.Y == 0)
            {
                //Debug.WriteLine();
                animation.SetActiveClip(AnimationClipType.WalkingRight);
            }
            else if(forwardVector.X == -1 && forwardVector.Y == 0)
            {
                //Debug.WriteLine();
                animation.SetActiveClip(AnimationClipType.WalkingLeft);
            }
            else if (forwardVector.X == 0 && forwardVector.Y == 1)
            {
                //Debug.WriteLine();

                animation.SetActiveClip(AnimationClipType.WalkingDown);
            }
            else if (forwardVector.X == 0 && forwardVector.Y == -1)
            {
                //Debug.WriteLine();
                animation.SetActiveClip(AnimationClipType.WalkingUp);
            }

            Sprite sprite = new Sprite(Texture, animation.DefaultSourceRectangle, 2, Visibility.Game);
            AddComponent(sprite);

            RectangleCollider collider = new RectangleCollider(TopLeftCornerPosition(), Size(), false);
            AddComponent(collider);





            Initialize();
        }

        public static void LoadContent(GameScreenManager gameScreenManager)
        {
            ContentManager content = gameScreenManager.Game.Content;
            string textureFullPath = "Textures/Objects/Items/selfmade_fist_spritesheet";
            Texture = content.Load<Texture2D>(textureFullPath);

            AnimationClipList = new List<AnimationClip> {
                            new AnimationClip(AnimationClipType.WalkingLeft, 1, 200, new Point(128, 128)),
                            new AnimationClip(AnimationClipType.WalkingRight, 1, 200, new Point(128, 128)),
                            new AnimationClip(AnimationClipType.WalkingUp, 1, 200, new Point(128, 128)),
                            new AnimationClip(AnimationClipType.WalkingDown, 1, 200, new Point(128, 128))
                        };

        }

    }
}
