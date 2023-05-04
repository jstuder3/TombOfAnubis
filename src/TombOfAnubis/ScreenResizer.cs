using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TombOfAnubis
{
    public class ScreenResizer
    {
        GraphicsDeviceManager graphics;
        GameWindow window;

        bool _isFullscreen = false;
        bool _isBorderless = false;
        int _width = 0;
        int _height = 0;

        public static readonly int numSupportedResolutions = 2;
        public static readonly ushort[] supportedWidths = new ushort[] { 2560, 1920 };
        public static readonly ushort[] supportedHeights = new ushort[] { 1440, 1080 };

        public ScreenResizer(GraphicsDeviceManager graphics, GameWindow window)
        {
            this.graphics = graphics;
            this.window = window;
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
        }
        private void UnsetFullscreen()
        {
            graphics.PreferredBackBufferWidth = _width;
            graphics.PreferredBackBufferHeight = _height;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
        }

        public void ChangeResolution(int width, int height)
        {
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            // Apply the changes
            graphics.ApplyChanges();
        }
    }
}
