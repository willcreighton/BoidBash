﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PredatorBuild
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Predator predator;
        private Texture2D predTexture;
        private int width;
        private int height;

        public int Width
        {
            get { return width; }
        }
        public int Height
        {
            get { return height; }
        }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            width = _graphics.GraphicsDevice.Viewport.Width;
            height = _graphics.GraphicsDevice.Viewport.Height;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            predTexture = Content.Load<Texture2D>("Pentagon");

            predator.PredHeight = 30;
            predator.PredWidth = 30;

            predator = new Predator(predTexture, new Rectangle(width / 2, height / 2, 
                predator.PredHeight, predator.PredWidth), 
                height, width, predator.PredHeight, predator.PredWidth);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            predator.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();

            predator.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}