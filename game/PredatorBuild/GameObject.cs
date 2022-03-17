﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PredatorBuild
{
    abstract class GameObject
    {
        // Fields
        protected Texture2D texture;
        protected Rectangle position;

        // Properties
        /// <summary>
        /// Read-only property for the position of the game object
        /// </summary>
        public Rectangle Position
        {
            get { return position; }
        }

        // Protected Constructor
        /// <summary>
        /// Protected constructor (Takes in Texture2D and Rectangle)
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        protected GameObject(Texture2D texture, Rectangle position)
        {
            this.texture = texture;
            this.position = position;
        }

        // Methods
        /// <summary>
        /// Draws the GameObject SpriteBatch
        /// </summary>
        /// <param name="sb"></param>
        public virtual void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, position, Color.White);
        }
        /// <summary>
        /// Updates GameObject with GameTime
        /// </summary>
        /// <param name="gameTime"></param>
        public abstract void Update(GameTime gameTime);
    }
}
