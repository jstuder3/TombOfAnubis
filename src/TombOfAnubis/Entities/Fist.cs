using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Sdcb.FFmpeg.Utils;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using TombOfAnubisContentData;

namespace TombOfAnubis
{
    public class Fist : Entity
    {
        private Vector2 startPosition;
        private float velocity;
        private float range;

        public static Vector2 Scale = new Vector2(0.2f, 0.2f);
        public static Texture2D Texture;
        public static List<AnimationClip> AnimationClipList;
        //private static 

        public Fist(Vector2 position, Vector2 forwardVector)
        {
            Transform transform = new Transform(position, Scale);
            AddComponent(transform);

            if (Texture == null)
            {
                Console.WriteLine("Error: Trying to instantiate fist without having first loaded its texture.");
                return;
            }

            Animation animation = new Animation(AnimationClipList);
            AddComponent(animation);

            Console.WriteLine(forwardVector.ToString());

            // select sprite based on forward vector
            if(forwardVector.X == 1 && forwardVector.Y == 0)
            {
                Console.WriteLine(animation.SetActiveClip(AnimationClipType.WalkingRight));
            }
            else if(forwardVector.X == -1 && forwardVector.Y == 0)
            {
                Console.WriteLine(animation.SetActiveClip(AnimationClipType.WalkingLeft));
            }
            else if (forwardVector.X == 0 && forwardVector.Y == 1)
            {
                Console.WriteLine(animation.SetActiveClip(AnimationClipType.WalkingDown));
            }
            else if (forwardVector.X == 0 && forwardVector.Y == -1)
            {
                Console.WriteLine(animation.SetActiveClip(AnimationClipType.WalkingUp));
            }

            Sprite sprite = new Sprite(Texture, animation.DefaultSourceRectangle, 2);
            AddComponent(sprite);

            RectangleCollider collider = new RectangleCollider(position, Size());
            AddComponent(collider);
        }

        public static void LoadContent(GameScreenManager gameScreenManager)
        {
            ContentManager content = gameScreenManager.Game.Content;
            string textureFullPath = "Textures/Objects/Items/plagiarized_fist_and_attributed_smoke_spritesheet";
            Texture = content.Load<Texture2D>(textureFullPath);

            AnimationClipList = new List<AnimationClip> {
                            new AnimationClip(AnimationClipType.WalkingLeft, 1, 200, new Point(400, 400)),
                            new AnimationClip(AnimationClipType.WalkingRight, 1, 200, new Point(400, 400)),
                            new AnimationClip(AnimationClipType.WalkingUp, 1, 200, new Point(400, 400)),
                            new AnimationClip(AnimationClipType.WalkingDown, 1, 200, new Point(400, 400)),
                            new AnimationClip(AnimationClipType.VFX_01, 5, 100, new Point(400, 400))
                        };

        }

    }
}
