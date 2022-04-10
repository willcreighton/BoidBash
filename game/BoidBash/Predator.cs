﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BoidBash
{
    class Predator : GameObject
    {
        // Fields
        private int predHeight;
        private int predWidth;

        //private float rotation;
        private Vector2 actualPosition;
        private Rectangle predatorBounds = new Rectangle(200, 200, 800, 500);

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
        public Predator(Texture2D texture, Rectangle position, int windowHeight, int windowWidth, int predHeight, int predWidth) :
            base(texture, position, windowHeight, windowWidth)
        {
            this.texture = texture;
            this.position = position;
            this.predHeight = predHeight;
            this.predWidth = predWidth;
        }

        /*
         public override void Draw(SpriteBatch sb)
         {
             MouseState mouseState = Mouse.GetState();
             float rotation = (float)Math.Atan2((double)mouseState.Y -
                 actualPosition.Y, (double)mouseState.X - actualPosition.X);

             sb.Draw(texture,                                                                   // Texture
                 new Vector2(position.X, position.Y),                                           // Position
                 position,                                                                      // Rectangle
                 Color.Red,                                                                     // Color
                 rotation,                                                                      // Rotation Angle
                 new Vector2(position.X, position.Y),                                           // Origin
                 new Vector2(0, 0),                                                             // Scale
                 SpriteEffects.None,                                                            // Sprite Effect
                 1);                                                                            // Depth           
         }
         */

        // Movement
        public override void Update(GameTime gameTime)
        {
            KeyboardState keyBState = Keyboard.GetState();

            // Each if statement keeps the predator inside a set of boundaries
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

            actualPosition = new Vector2(position.Center.X, position.Center.Y);
        }
    }
}