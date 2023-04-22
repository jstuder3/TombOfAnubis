﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace TombOfAnubis
{
    public class TombOfAnubis : Game
    {
        private GraphicsDeviceManager graphics;
        GameScreenManager screenManager;


        public TombOfAnubis()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;

            screenManager = new GameScreenManager(this);
            Components.Add(screenManager);
            IsMouseVisible = true;

        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            LevelGenerator gen = new LevelGenerator(new Point(30, 30), new List<LevelBlock>(), 4);
            gen.GenerateLevel();
            base.Initialize();

            screenManager.AddScreen(new IntroScreen());

        }

        protected override void LoadContent()
        {
            Fonts.LoadContent(Content);
            AudioController.LoadContent(Content);
            VideoController.LoadContent(graphics.GraphicsDevice);
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            InputController.Update(gameTime);
            VideoController.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Transparent);

            base.Draw(gameTime);
        }
    }
}