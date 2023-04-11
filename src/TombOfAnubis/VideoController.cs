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

        public static void LoadContent(GraphicsDevice graphics)
        {
            videos = new Dictionary<string, Video>
                {
                    { "test_video", VideoHelper.LoadFromFile(@"Content/Videos/SampleVideo_1280x720_1mb.mp4")}
                };
            videoPlayer = new VideoPlayer(graphics);

            videoPlayer.IsLooped = true;
        }

        public static void PlayVideo(string title, bool muted = false)
        {
            if(videos.ContainsKey(title))
            {
                videoPlayer.Play(videos[title]);
                videoPlayer.IsMuted = muted;
            }
            else
            {
                videoPlayer.Stop();
            }
        }

        public static void Draw(SpriteBatch spriteBatch, Rectangle destRectangle)
        {
            if(videoPlayer.Video != null)
            {
                var videoTexture = videoPlayer.GetTexture();

                spriteBatch.Begin();
                spriteBatch.Draw(videoTexture, destRectangle, Color.White);
                spriteBatch.End();
            }
        }
        public static void StopVideo()
        {
            videoPlayer.Stop();
        }
        public static void PauseVideo()
        {
            videoPlayer.Pause();
        }
        public static void ResumeVideo()
        {
            videoPlayer.Resume();
        }
    }
}
