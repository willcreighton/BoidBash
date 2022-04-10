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
        private Texture2D resumePrompt;
        private Texture2D returnPrompt;
        private Texture2D pausedDisplay;

        // Constructor
        public PauseMenuUI(int windowWidth, int windowHeight, Texture2D resumePrompt, Texture2D returnPrompt, Texture2D pausedDisplay)
        {
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
            this.resumePrompt = resumePrompt;
            this.returnPrompt = returnPrompt;
            this.pausedDisplay = pausedDisplay;
        }

        /// <summary>
        /// Draw the Main Menu
        /// </summary>
        /// <param name="_spriteBatch"></param>
        public void Draw(SpriteBatch _spriteBatch)
        {
            // Paused display
            _spriteBatch.Draw(
                pausedDisplay,
                new Rectangle(500, 400, 200, 45),
                Color.White
                );

            // Resume prompt
            _spriteBatch.Draw(
                resumePrompt,
                new Vector2(20, windowHeight - 40),
                Color.White
                );

            // Main Menu prompt
            _spriteBatch.Draw(
                returnPrompt,
                new Vector2(20, windowHeight - 80),
                Color.White
                );
        }
    }
}
