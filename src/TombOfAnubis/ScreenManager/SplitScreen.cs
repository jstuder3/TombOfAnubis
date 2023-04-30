using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System; using System.Diagnostics;
using System.Collections.Generic;

namespace TombOfAnubis
{
    public static class SplitScreen
    {

        private static int numberOfPlayers;

        private static GraphicsDevice graphics;

        public static int NumberOfPlayers
        {
            get { return numberOfPlayers; }
        }

        private static List<Viewport> playerViewports;

        public static List<Viewport> PlayerViewports
        {
            get { return playerViewports; }
            private set { playerViewports = value; }
        }

        public static List<Vector2> WorldScaleFactorsBasedOnNPlayers = new List<Vector2>()
        {
            new Vector2(1, 1),
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
        };

        private static Viewport gameScreenViewport;

        public static Viewport GameScreenViewport
        {

            get { return gameScreenViewport; }

        }
        public static void Initialize(GraphicsDevice graphicsDevice, int numPlayers)
        {
            graphics = graphicsDevice;
            numberOfPlayers = numPlayers;
            gameScreenViewport = graphics.Viewport;
            PlayerViewports = new List<Viewport>();
            switch (numberOfPlayers)
            {
                case 1:
                    playerViewports.Add(graphics.Viewport); break;
                case 2:
                    CreateTwoPlayerViewports(graphicsDevice); break;
                case 3:
                    CreateThreePlayerViewports(graphicsDevice); break;
                case 4:
                    CreateFourPlayerViewports(graphicsDevice); break;
                default: throw new ArgumentException("Unsupported number of players");

            }

        }

        public static Viewport SetViewport(int playerIndex)
        {
            graphics.Viewport = playerViewports[playerIndex];
            //TileEngine.Viewport = graphics.Viewport;
            return playerViewports[playerIndex];
        }

        public static Viewport ResetViewport()
        {
            graphics.Viewport = gameScreenViewport;
            return gameScreenViewport;
        }

        private static void CreateFourPlayerViewports(GraphicsDevice graphicsDevice)
        {
            int w = gameScreenViewport.Width;
            int h = gameScreenViewport.Height;
            int x = gameScreenViewport.X;
            int y = gameScreenViewport.Y;


            Viewport topLeft = new Viewport();
            topLeft.X = x;
            topLeft.Y = y;
            topLeft.Width = w / 2 - 1;
            topLeft.Height = h / 2 - 1;
            topLeft.MinDepth = 0;
            topLeft.MaxDepth = 1;

            Viewport topRight = new Viewport();
            topRight.X = x + w / 2 + 1;
            topRight.Y = y;
            topRight.Width = w / 2 - 1;
            topRight.Height = h / 2 - 1;
            topRight.MinDepth = 0;
            topRight.MaxDepth = 1;

            Viewport bottomLeft = new Viewport();
            bottomLeft.X = x;
            bottomLeft.Y = y + h / 2 + 1;
            bottomLeft.Width = w / 2 - 1;
            bottomLeft.Height = h / 2 - 1;
            bottomLeft.MinDepth = 0;
            bottomLeft.MaxDepth = 1;

            Viewport bottomRight = new Viewport();
            bottomRight.X = x + w / 2 + 1;
            bottomRight.Y = y + h / 2 + 1;
            bottomRight.Width = w / 2 - 1;
            bottomRight.Height = h / 2 - 1;
            bottomRight.MinDepth = 0;
            bottomRight.MaxDepth = 1;

            playerViewports.Add(topLeft);
            playerViewports.Add(topRight);
            playerViewports.Add(bottomLeft);
            playerViewports.Add(bottomRight);
        }
        private static void CreateTwoPlayerViewports(GraphicsDevice graphicsDevice)
        {
            int w = gameScreenViewport.Width;
            int h = gameScreenViewport.Height;
            int x = gameScreenViewport.X;
            int y = gameScreenViewport.Y;


            Viewport left = new Viewport();
            left.X = x;
            left.Y = y;
            left.Width = w / 2 - 1;
            left.Height = h;
            left.MinDepth = 0;
            left.MaxDepth = 1;

            Viewport right = new Viewport();
            right.X = x + w / 2 + 1;
            right.Y = y;
            right.Width = w / 2 - 1;
            right.Height = h;
            right.MinDepth = 0;
            right.MaxDepth = 1;

            playerViewports.Add(left);
            playerViewports.Add(right);
        }

        private static void CreateThreePlayerViewports(GraphicsDevice graphicsDevice)
        {
            int w = gameScreenViewport.Width;
            int h = gameScreenViewport.Height;
            int x = gameScreenViewport.X;
            int y = gameScreenViewport.Y;


            Viewport topLeft = new Viewport();
            topLeft.X = x;
            topLeft.Y = y;
            topLeft.Width = w / 2 - 1;
            topLeft.Height = h / 2 - 1;
            topLeft.MinDepth = 0;
            topLeft.MaxDepth = 1;

            Viewport topRight = new Viewport();
            topRight.X = x + w / 2 + 1;
            topRight.Y = y;
            topRight.Width = w / 2 - 1;
            topRight.Height = h / 2 - 1;
            topRight.MinDepth = 0;
            topRight.MaxDepth = 1;

            Viewport bottom = new Viewport();
            bottom.X = x + w / 4;
            bottom.Y = y + h / 2 + 1;
            bottom.Width = w / 2 - 1;
            bottom.Height = h / 2 - 1;
            bottom.MinDepth = 0;
            bottom.MaxDepth = 1;

            playerViewports.Add(topLeft);
            playerViewports.Add(topRight);
            playerViewports.Add(bottom);
        }
    }
}
