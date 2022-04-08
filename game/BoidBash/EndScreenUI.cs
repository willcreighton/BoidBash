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
        private Texture2D continuePrompt;
        private Texture2D gameOver;

        // Constructor
        public EndScreenUI(int windowWidth, int windowHeight, Texture2D continuePrompt, Texture2D gameOver)
        {
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
            this.continuePrompt = continuePrompt;
            this.gameOver = gameOver;
        }

        /// <summary>
        /// Draw the Main Menu
        /// </summary>
        /// <param name="_spriteBatch"></param>
        public void Draw(SpriteBatch _spriteBatch)
        {
            // Game over display
            _spriteBatch.Draw(
                gameOver,
                new Rectangle(370, 250, 460, 60),
                Color.White
                );

            // Continue to Main Menu prompt
            _spriteBatch.Draw(
                continuePrompt,
                new Vector2(460, windowHeight - 400),
                Color.White
                );
        }
    }
}
