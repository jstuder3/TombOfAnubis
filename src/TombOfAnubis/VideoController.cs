using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Framework.Media;
using MonoGame.Extended.VideoPlayback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static void LoadContent(GraphicsDevice _graphics)
        {
            graphics = _graphics;
            videos = new Dictionary<string, Video>
                {
                    { "intro_video", VideoHelper.LoadFromFile(@"Content/Videos/IntroVideo.mp4")}
                };
            videoPlayer = new VideoPlayer(graphics);

        }

        public static void PlayVideo(string title, bool looped, bool muted = false)
        {
            if(videos.ContainsKey(title))
            {
                videoPlayer.IsLooped = looped;
                videoPlayer.Play(videos[title]);
                videoPlayer.IsMuted = muted;
                active = true;
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

        }

        public static void Draw(SpriteBatch spriteBatch, Rectangle destRectangle)
        {
            if (active)
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
