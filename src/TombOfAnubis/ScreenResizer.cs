using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class ScreenResizer
    {
        GraphicsDeviceManager graphics;
        GameWindow window;
        Viewport viewport;

        bool _isFullscreen = false;
        bool _isBorderless = false;
        int _width = 0;
        int _height = 0;

        public static readonly int numSupportedResolutions = 2;
        public static readonly ushort[] supportedWidths = new ushort[] { 2560, 1920 };
        public static readonly ushort[] supportedHeights = new ushort[] { 1440, 1080 };

        public RenderTarget2D renderTarget;
        public Rectangle renderTargetDestination;
        public Color renderTargetColor = Color.Black;

        public GraphicsDeviceManager Graphics 
        {
            get { return graphics; }
        }

        public Viewport Viewport
        {
            get { return viewport; }
        }

        public ScreenResizer(GraphicsDeviceManager graphics, GameWindow window, int width, int height)
        {
            this.graphics = graphics;
            this.window = window;
            this._width = width;
            this._height = height;

            this.viewport = graphics.GraphicsDevice.Viewport;

            Point gameResolution = new Point(_width, _height);
            renderTarget = new RenderTarget2D(graphics.GraphicsDevice, width, height);
            renderTargetDestination = GetRenderTargetDestination(gameResolution, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            graphics.GraphicsDevice.Viewport = new Viewport(renderTargetDestination.X, renderTargetDestination.Y, renderTargetDestination.Width, renderTargetDestination.Height);
            Debug.WriteLine("Viewport: " + viewport);
        }

        public void ToggleFullscreen()
        {
            bool oldIsFullscreen = _isFullscreen;

            if (_isBorderless)
            {
                _isBorderless = false;
            }
            else
            {
                _isFullscreen = !_isFullscreen;
            }

            ApplyFullscreenChange(oldIsFullscreen);
        }
        public void ToggleBorderless()
        {
            bool oldIsFullscreen = _isFullscreen;

            _isBorderless = !_isBorderless;
            _isFullscreen = _isBorderless;

            ApplyFullscreenChange(oldIsFullscreen);
        }

        private void ApplyFullscreenChange(bool oldIsFullscreen)
        {
            if (_isFullscreen)
            {
                if (oldIsFullscreen)
                {
                    ApplyHardwareMode();
                }
                else
                {
                    SetFullscreen();
                }
            }
            else
            {
                UnsetFullscreen();
            }
        }
        private void ApplyHardwareMode()
        {
            graphics.HardwareModeSwitch = !_isBorderless;
            graphics.ApplyChanges();
        }
        private void SetFullscreen()
        {
            _width = window.ClientBounds.Width;
            _height = window.ClientBounds.Height;

            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.HardwareModeSwitch = !_isBorderless;

            graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            Point gameResolution = new Point(_width, _height);
            renderTargetDestination = GetRenderTargetDestination(gameResolution, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            viewport = new Viewport(renderTargetDestination.X, renderTargetDestination.Y, renderTargetDestination.Width, renderTargetDestination.Height);
            Debug.WriteLine("Viewport: " + viewport);
        }
        private void UnsetFullscreen()
        {
            graphics.PreferredBackBufferWidth = _width;
            graphics.PreferredBackBufferHeight = _height;
            graphics.IsFullScreen = false;
            window.BeginScreenDeviceChange(false);
            window.EndScreenDeviceChange(window.ScreenDeviceName, _width, _height);
            graphics.ApplyChanges();

            Point gameResolution = new Point(_width, _height);
            renderTargetDestination = GetRenderTargetDestination(gameResolution, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            viewport = new Viewport(renderTargetDestination.X, renderTargetDestination.Y, renderTargetDestination.Width, renderTargetDestination.Height);
            Debug.WriteLine("Viewport: " + viewport);
        }

        public void ChangeResolution(int width, int height)
        {
            Point gameResolution = new Point(width, height);
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            // Apply the changes
            graphics.ApplyChanges();

            renderTarget = new RenderTarget2D(graphics.GraphicsDevice, width, height);
            renderTargetDestination = GetRenderTargetDestination(gameResolution, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
        }

        Rectangle GetRenderTargetDestination(Point resolution, int preferredBackBufferWidth, int preferredBackBufferHeight)
        {
            float resolutionRatio = (float)resolution.X / resolution.Y;
            float screenRatio;
            Point bounds = new Point(preferredBackBufferWidth, preferredBackBufferHeight);
            screenRatio = (float)bounds.X / bounds.Y;
            float scale;
            Rectangle rectangle = new Rectangle();

            if (resolutionRatio < screenRatio)
                scale = (float)bounds.Y / resolution.Y;
            else if (resolutionRatio > screenRatio)
                scale = (float)bounds.X / resolution.X;
            else
            {
                // Resolution and window/screen share aspect ratio
                rectangle.Size = bounds;
                return rectangle;
            }
            rectangle.Width = (int)(resolution.X * scale);
            rectangle.Height = (int)(resolution.Y * scale);
            return CenterRectangle(new Rectangle(Point.Zero, bounds), rectangle);
        }

        static Rectangle CenterRectangle(Rectangle outerRectangle, Rectangle innerRectangle)
        {
            Point delta = outerRectangle.Center - innerRectangle.Center;
            innerRectangle.Offset(delta);
            return innerRectangle;
        }

    }
}
