using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TombOfAnubis
{
    public static class ResolutionController
    {
        public static RenderTarget2D RenderTarget { get; set; }

        public static Point TargetResolution { get; set; } = new Point(1920, 1080);

        public static Viewport TargetViewport { get; set; }
        private static GraphicsDeviceManager graphics;

        public static void Initialize(GraphicsDeviceManager _graphics)
        {
            graphics = _graphics;
            graphics.HardwareModeSwitch = false;
            graphics.PreferredBackBufferWidth = TargetResolution.X;
            graphics.PreferredBackBufferHeight = TargetResolution.Y;
            graphics.ApplyChanges();

            RenderTarget = new RenderTarget2D(
                graphics.GraphicsDevice,
                TargetResolution.X,
                TargetResolution.Y,
                false,
                graphics.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
            TargetViewport = new Viewport(0, 0, RenderTarget.Width, RenderTarget.Height);

            if (Settings.Read().IsFullscreen)
            {
                ToggleFullscreen();
            }
        }
        public static void ToggleFullscreen()
        {
            if (!graphics.IsFullScreen)
            {
                graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }
            else
            {
                graphics.PreferredBackBufferWidth = TargetResolution.X;
                graphics.PreferredBackBufferHeight = TargetResolution.Y;
            }
            graphics.IsFullScreen = !graphics.IsFullScreen;
            graphics.ApplyChanges();
        }

        public static void SetRenderTarget()
        {
            graphics.GraphicsDevice.SetRenderTarget(RenderTarget);
            graphics.GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
            graphics.GraphicsDevice.Clear(Color.Black);
        }
        public static void Draw(SpriteBatch spriteBatch)
        {
            graphics.GraphicsDevice.SetRenderTarget(null);

            graphics.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                        SamplerState.LinearClamp, DepthStencilState.Default,
                        RasterizerState.CullNone);
            if(graphics.IsFullScreen)
            {
                Vector2 fraction = new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height) / new Vector2(TargetResolution.X, TargetResolution.Y);
                Vector2 scale = Vector2.One * Math.Min(fraction.X, fraction.Y);
                Vector2 pos = new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2) - scale * new Vector2(TargetResolution.X / 2, TargetResolution.Y / 2);
                
                spriteBatch.Draw(RenderTarget, pos, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
            else
            {
                spriteBatch.Draw(RenderTarget, new Rectangle(0, 0, TargetResolution.X, TargetResolution.Y), Color.White);
            }

            spriteBatch.End();
        }
    }
}
