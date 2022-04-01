using System;
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

        // Parameterized Constructor
        public Predator(Texture2D texture, Rectangle position, int windowHeight, int windowWidth, int predHeight, int predWidth) :
            base(texture, position, windowHeight, windowWidth)
        {
            this.texture = texture;
            this.position = position;
            this.predHeight = predHeight;
            this.predWidth = predWidth;
        }

        //
        /* Attempting to make a rotating predator
        public override void Draw(SpriteBatch sb)
        {           
            sb.Draw(texture,                                                                   // Texture
                new Vector2(position.X, position.Y),                                           // Position
                position,                                                                      // Rectangle
                Color.Black,                                                                   // Color
                rotation,                                                                      // Rotation Angle
                new Vector2(position.X, position.Y),                                           // Origin
                new Vector2(0, 0),                                                             // Scale
                SpriteEffects.None,                                                            // Sprite Effect
                1);                                                                            // Depth           
        }
        //*/

        // Movement
        public override void Update(GameTime gameTime)                                         // Each If statement keeps the predator inside a set of boundaries
        {
            KeyboardState keyBState = Keyboard.GetState();

            if (keyBState.IsKeyDown(Keys.Left) || keyBState.IsKeyDown(Keys.A))
            {
                //rotation = 0;
                if (position.X >= 0)
                {
                    position.X -= 5;
                }
            }

            if (keyBState.IsKeyDown(Keys.Up) || keyBState.IsKeyDown(Keys.W))
            {
                //rotation = MathHelper.ToRadians(-90);
                if (position.Y >= 0)
                {
                    position.Y -= 5;
                }
            }

            if (keyBState.IsKeyDown(Keys.Right) || keyBState.IsKeyDown(Keys.D))
            {
                //rotation = MathHelper.ToRadians(90);
                if ((position.X + predWidth) <= windowWidth)
                {
                    position.X += 5;
                }
            }

            if (keyBState.IsKeyDown(Keys.Down) || keyBState.IsKeyDown(Keys.S))
            {
                //rotation = MathHelper.ToRadians(180);
                if ((position.Y + predHeight) <= windowHeight)
                {
                    position.Y += 5;
                }
            }
        }
    }
}