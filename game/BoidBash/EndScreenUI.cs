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
        private ulong score;
        private SpriteFont senBold;

        // This is a property that is named Score
        public ulong Score
        {
            set { score = value; }
        }

        // Constructor
        public EndScreenUI(int windowWidth, int windowHeight, Texture2D continuePrompt, Texture2D gameOver, SpriteFont senBold)
        {
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
            this.continuePrompt = continuePrompt;
            this.gameOver = gameOver;
            this.senBold = senBold;
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
                new Rectangle(360, 300, 470, 60),
                Color.White
                );

            // Continue to Main Menu prompt
            _spriteBatch.Draw(
                continuePrompt,
                new Vector2(460, windowHeight - 400),
                Color.White
                );

            // Draw the score achieved
            _spriteBatch.DrawString(
                senBold,
                String.Format("Score: {0:n0}", score),
                new Vector2(15, 15),
                Color.White
                );
        }
    }
}
