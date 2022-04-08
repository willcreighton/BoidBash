using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BoidBash
{
    /// <summary>
    /// This class handles drawing the End Screen state's UI
    /// </summary>
    class EndScreenUI
    {
        // Fields
        private int windowWidth;
        private int windowHeight;
        private SpriteFont headerFont;
        private SpriteFont primaryFont;
        private Color backgroundColor;

        // Constructor
        public EndScreenUI(int windowWidth, int windowHeight, SpriteFont headerFont, SpriteFont primaryFont)
        {
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
            this.headerFont = headerFont;
            this.primaryFont = primaryFont;

            backgroundColor = new Color(20, 20, 20);
        }

        /// <summary>
        /// Draw the Main Menu
        /// </summary>
        /// <param name="_spriteBatch"></param>
        public void Draw(SpriteBatch _spriteBatch)
        {
            // State display
            _spriteBatch.DrawString(
                headerFont,
                "End Screen",
                new Vector2(510, 350),
                Color.White
                );

            // Main Menu prompt
            _spriteBatch.DrawString(
                primaryFont,
                "Press 'M' to return to the Main Menu.",
                new Vector2(20, windowHeight - 40),
                Color.White
                );
        }
    }
}
