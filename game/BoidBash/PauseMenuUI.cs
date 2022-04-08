using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BoidBash
{
    /// <summary>
    /// This class handles drawing the Pause Menu state's UI
    /// </summary>
    class PauseMenuUI
    {
        // Fields
        private int windowWidth;
        private int windowHeight;
        private SpriteFont headerFont;
        private SpriteFont primaryFont;

        // Constructor
        public PauseMenuUI(int windowWidth, int windowHeight, SpriteFont headerFont, SpriteFont primaryFont)
        {
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
            this.headerFont = headerFont;
            this.primaryFont = primaryFont;
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
                "Pause Menu",
                new Vector2(20, 15),
                Color.White
                );

            // Game prompt
            _spriteBatch.DrawString(
                primaryFont,
                "Press ENTER to Resume",
                new Vector2(20, windowHeight - 40),
                Color.White
                );

            // Main Menu prompt
            _spriteBatch.DrawString(
                primaryFont,
                "Press M to Return to Main Menu",
                new Vector2(20, windowHeight - 80),
                Color.White
                );
        }
    }
}
