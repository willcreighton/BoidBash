﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PredatorBuild
{
    class Predator : GameObject
    {
        // Fields
        private int predHeight;
        private int predWidth;
        private float rotation;

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

        // Parameterized Constructor
        public Predator(Texture2D texture, Rectangle position, int windowHeight, int windowWidth, int predHeight, int predWidth) : 
            base(texture, position, windowHeight, windowWidth)
        {
            this.texture = texture;
            this.position = position;
        }
     
        //
        /* Attempting to make a rotating predator
        public override void Draw(SpriteBatch sb)
        {           
            sb.Draw(texture,                                                                        // Texture
                new Vector2(position.X, position.Y),                                                // Position
                position,                                                                           // Rectangle
                Color.Black,                                                                        // Color
                rotation,                                                                           // Rotation Angle
                new Vector2(position.X, position.Y),                                                // Origin
                new Vector2(0, 0),                                                                  // Scale
                SpriteEffects.None,                                                                 // Sprite Effect
                1);                                                                                 // Depth           
        }
        //*/
        
        // Movement
        public override void Update(GameTime gameTime)
        {
            KeyboardState keyBState = Keyboard.GetState();

            if (keyBState.IsKeyDown(Keys.Left))
            {
                //rotation = 0;
                if (position.X >= 0)
                {
                    position.X -= 5;
                }
                
            }
            if (keyBState.IsKeyDown(Keys.Up))
            {
                //rotation = MathHelper.ToRadians(-90);
                if (position.Y >= 0)
                {
                    position.Y -= 5;
                }               
            }
            
            if (keyBState.IsKeyDown(Keys.Right))
            {
                //rotation = MathHelper.ToRadians(90);
                if ((position.X + predWidth) <= windowWidth)
                {
                    position.X += 5;
                }                
            }           
            if (keyBState.IsKeyDown(Keys.Down))
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
