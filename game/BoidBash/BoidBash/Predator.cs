using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BoidBash
{
    
    // Determines if the player is playing Singleplayer mode or the Versus mode
    enum GameMode
    {
        Single,
        Versus
    }

    // Holds the different control schemes
    enum ControlScheme
    {
        WASD,
        Arrows,
        WASDArrows
    }
    class Predator : GameObject   
    {
        // Fields
        private int predHeight;
        private int predWidth;
        private Vector2 actualPosition;
        private Rectangle predatorBounds = new Rectangle(200, 200, 800, 500);

        private GameMode currentMode = GameMode.Single;
        private ControlScheme currentScheme = ControlScheme.WASDArrows;

        // Properties
        public int PredHeight
        {
            get { return predHeight; }
            set { predHeight = value; }
        }
        public int PredWidth
        {
            get { return predWidth; }
            set { predWidth = value; }
        }
        public Rectangle PredatorPosition
        {
            get { return position; }
        }
        /// <summary>
        /// ActualPosition returns the center of the rectangle that the predator is drawn upon
        /// </summary>
        public Vector2 ActualPosition
        {
            get { return actualPosition; }
        }

        /// <summary>
        /// Parameterized Constructor
        /// 
        /// Predator Constructor that takes in a Texture2D for the predator texture, 
        /// a Rectangle for the position/size, 
        /// an integer for both the height and width of the game window to create boundaries,
        /// and an integer for the height and width of the predator to help keep the sprite inside the boundaries.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        /// <param name="windowHeight"></param>
        /// <param name="windowWidth"></param>
        /// <param name="predHeight"></param>
        /// <param name="predWidth"></param>
        public Predator(Texture2D texture, Rectangle position, int windowHeight, int windowWidth, int predHeight, int predWidth, ControlScheme scheme) :
            base(texture, position, windowHeight, windowWidth)
        {
            this.texture = texture;
            this.position = position;
            this.predHeight = predHeight;
            this.predWidth = predWidth;
            currentScheme = scheme;
        }

        // Movement
        public override void Update(GameTime gameTime)
        {
            KeyboardState keyBState = Keyboard.GetState();

            switch (currentMode)
            {
                case GameMode.Single:                    
                    ChooseScheme(ControlScheme.WASDArrows);
                    break;
                case GameMode.Versus:
                    ChooseScheme(currentScheme);
                    break;
            }            

            actualPosition = new Vector2(position.Center.X, position.Center.Y);
        }

        private void ChooseScheme(ControlScheme scheme)
        {
            KeyboardState keyBState = Keyboard.GetState();
            switch (currentScheme)
            {
                case ControlScheme.WASDArrows:
                    if (keyBState.IsKeyDown(Keys.Left) || keyBState.IsKeyDown(Keys.A))
                    {
                        //rotation = 0;
                        if (position.X > predatorBounds.X)
                        {
                            position.X -= 5;
                        }
                    }

                    if (keyBState.IsKeyDown(Keys.Up) || keyBState.IsKeyDown(Keys.W))
                    {
                        //rotation = MathHelper.ToRadians(-90);
                        if (position.Y > predatorBounds.Y)
                        {
                            position.Y -= 5;
                        }
                    }

                    if (keyBState.IsKeyDown(Keys.Right) || keyBState.IsKeyDown(Keys.D))
                    {
                        //rotation = MathHelper.ToRadians(90);
                        if (position.X + predWidth < predatorBounds.X + predatorBounds.Width)
                        {
                            position.X += 5;
                        }
                    }

                    if (keyBState.IsKeyDown(Keys.Down) || keyBState.IsKeyDown(Keys.S))
                    {
                        //rotation = MathHelper.ToRadians(180);
                        if (position.Y + predHeight < predatorBounds.Y + predatorBounds.Height)
                        {
                            position.Y += 5;
                        }
                    }
                    break;

                case ControlScheme.WASD:
                    if (keyBState.IsKeyDown(Keys.A))
                    {
                        //rotation = 0;
                        if (position.X > predatorBounds.X)
                        {
                            position.X -= 5;
                        }
                    }

                    if (keyBState.IsKeyDown(Keys.W))
                    {
                        //rotation = MathHelper.ToRadians(-90);
                        if (position.Y > predatorBounds.Y)
                        {
                            position.Y -= 5;
                        }
                    }

                    if (keyBState.IsKeyDown(Keys.D))
                    {
                        //rotation = MathHelper.ToRadians(90);
                        if (position.X + predWidth < predatorBounds.X + predatorBounds.Width)
                        {
                            position.X += 5;
                        }
                    }

                    if (keyBState.IsKeyDown(Keys.S))
                    {
                        //rotation = MathHelper.ToRadians(180);
                        if (position.Y + predHeight < predatorBounds.Y + predatorBounds.Height)
                        {
                            position.Y += 5;
                        }
                    }
                    break;

                case ControlScheme.Arrows:
                    if (keyBState.IsKeyDown(Keys.Left))
                    {
                        //rotation = 0;
                        if (position.X > predatorBounds.X)
                        {
                            position.X -= 5;
                        }
                    }

                    if (keyBState.IsKeyDown(Keys.Up))
                    {
                        //rotation = MathHelper.ToRadians(-90);
                        if (position.Y > predatorBounds.Y)
                        {
                            position.Y -= 5;
                        }
                    }

                    if (keyBState.IsKeyDown(Keys.Right))
                    {
                        //rotation = MathHelper.ToRadians(90);
                        if (position.X + predWidth < predatorBounds.X + predatorBounds.Width)
                        {
                            position.X += 5;
                        }
                    }

                    if (keyBState.IsKeyDown(Keys.Down))
                    {
                        //rotation = MathHelper.ToRadians(180);
                        if (position.Y + predHeight < predatorBounds.Y + predatorBounds.Height)
                        {
                            position.Y += 5;
                        }
                    }
                    break;
            }
        }
    }
}
