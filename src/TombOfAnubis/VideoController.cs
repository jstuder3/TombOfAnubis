using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Framework.Media;
using MonoGame.Extended.VideoPlayback;
using System; using System.Diagnostics;
using System.Collections.Generic;

namespace TombOfAnubis
{
    public static class VideoController
    {
        private static VideoPlayer videoPlayer;
        private static Dictionary<string, Video> videos;

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
                    { @"Content/Videos/IntroVideo.mp4", VideoHelper.LoadFromFile(@"Content/Videos/IntroVideo.mp4")},
                    { @"Content/Videos/IntroVideo_v2.mp4", VideoHelper.LoadFromFile(@"Content/Videos/IntroVideo_v2.mp4")},
                    { @"Content/Videos/CreditsScreen.mp4", VideoHelper.LoadFromFile(@"Content/Videos/CreditsScreen.mp4")},

                };
            videoPlayer = new VideoPlayer(graphics);

        }

        public static void LoadGameWonVideo(int numPlayers)
        {
            switch (numPlayers)
            {
                default: break;

                case 1: if (!videos.ContainsKey(@"Content/Videos/GameWon1.mp4")) videos.Add(@"Content/Videos/GameWon1.mp4", VideoHelper.LoadFromFile(@"Content/Videos/GameWon1.mp4")); break;
                case 2: if (!videos.ContainsKey(@"Content/Videos/GameWon2.mp4")) videos.Add(@"Content/Videos/GameWon2.mp4", VideoHelper.LoadFromFile(@"Content/Videos/GameWon2.mp4")); break;
                case 3: if (!videos.ContainsKey(@"Content/Videos/GameWon3.mp4")) videos.Add(@"Content/Videos/GameWon3.mp4", VideoHelper.LoadFromFile(@"Content/Videos/GameWon3.mp4")); break;
                case 4: if (!videos.ContainsKey(@"Content/Videos/GameWon4.mp4")) videos.Add(@"Content/Videos/GameWon4.mp4", VideoHelper.LoadFromFile(@"Content/Videos/GameWon4.mp4")); break;
            }
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
                if (!videoStarted)
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
                spriteBatch.Begin();
                spriteBatch.Draw(videoTexture, destRectangle, Color.White);
                spriteBatch.End();
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

        public static Microsoft.Xna.Framework.Media.MediaState GetState()
        {
            return videoPlayer.State;
        }
    }
}
