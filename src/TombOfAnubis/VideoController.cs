using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Framework.Media;
using MonoGame.Extended.VideoPlayback;
using System;
using System.Collections.Generic;

namespace TombOfAnubis
{
    public static class VideoController
    {
        private static VideoPlayer videoPlayer;
        private static Dictionary<string, Video> videos;
        private static Texture2D previousTexture;
        private static Rectangle previousRectangle;

        private static bool active;
        private static GraphicsDevice graphics;
        private static float startDelay = 0.1f;
        private static float startCounter = 0f;
        private static bool videoStarted = false;
        private static Queue<Video> loopedVideoTrashbin;

        public static void LoadContent(GraphicsDevice _graphics)
        {
            graphics = _graphics;
            loopedVideoTrashbin = new Queue<Video>();
            videos = new Dictionary<string, Video>
                {
                    { @"Content/Videos/IntroVideo.mp4", VideoHelper.LoadFromFile(@"Content/Videos/IntroVideo.mp4")}
                };
            videoPlayer = new VideoPlayer(graphics);

        }

        public static void PlayVideo(string title, bool looped, bool muted)
        {
            if(videos.ContainsKey(title))
            {
                videoPlayer.IsLooped = looped;
                if(looped)
                {
                    loopedVideoTrashbin.Enqueue(videos[title]);
                    videos[title] = VideoHelper.LoadFromFile(title);
                }
                videoPlayer.Play(videos[title]);
                videoPlayer.IsMuted = muted;
                active = true;
                startCounter = 0;
            }
            else
            {
                videoPlayer.Stop();
                active = false;
            }
        }
        public static void Update(GameTime gameTime)
        {
            if( videoPlayer?.Video != null && active)
            {
                if (videoStarted)
                {
                    if (!videoPlayer.IsLooped)
                    {
                        TimeSpan remaining = videoPlayer.Video.Duration - videoPlayer.PlayPosition;
                        TimeSpan eps = new TimeSpan(0, 0, 0, 0, 100);
                        if (remaining < eps)
                        {
                            previousTexture = CloneTexture(videoPlayer.GetTexture(), graphics, previousRectangle);

                            videoPlayer.Stop();
                        }
                    }
                }
                else
                {
                    startCounter += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (startCounter > startDelay)
                    {
                        videoStarted = true;
                    }
                }
            }
            if(loopedVideoTrashbin.Count > 5)
            {
                Video video = loopedVideoTrashbin.Dequeue();
                video.Dispose();
            }

        }

        public static void Draw(SpriteBatch spriteBatch, Rectangle destRectangle)
        {
            if (active & videoStarted)
            {
                var videoTexture = videoPlayer.GetTexture();
                previousRectangle = destRectangle;

                if (videoTexture != null && videoPlayer.Video != null)
                {
                    spriteBatch.Begin();
                    spriteBatch.Draw(videoTexture, destRectangle, Color.White);
                    spriteBatch.End();
                }
                else
                {
                    spriteBatch.Begin();
                    spriteBatch.Draw(previousTexture, destRectangle, Color.White);
                    spriteBatch.End();
                }
            }
        }
        public static void StopVideo()
        {
            videoPlayer.Stop();
            active = false;
            videoStarted = false;
        }
        public static void PauseVideo()
        {
            videoPlayer.Pause();
        }
        public static void ResumeVideo()
        {
            videoPlayer.Resume();
        }
        private static Texture2D CloneTexture(this Texture2D src, GraphicsDevice graphics, Rectangle rect)
        {
            Texture2D tex = new Texture2D(graphics, rect.Width, rect.Height);
            int count = rect.Width * rect.Height;
            Color[] data = new Color[count];
            src.GetData(0, rect, data, 0, count);
            tex.SetData(data);
            return tex;
        }
    }
}
