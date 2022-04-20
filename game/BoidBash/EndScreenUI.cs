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
        private Texture2D playAgainPrompt;
        private Texture2D gameOver;
        private ulong score;
        private SpriteFont senBold;
        private Texture2D returnPrompt;

        // This is a property that is named Score
        public ulong Score
        {
            set { score = value; }
        }

        // Constructor
        public EndScreenUI(int windowWidth, int windowHeight, Texture2D playAgainPrompt, Texture2D gameOver, SpriteFont senBold, Texture2D returnPrompt)
        {
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
            this.playAgainPrompt = playAgainPrompt;
            this.gameOver = gameOver;
            this.senBold = senBold;
            this.returnPrompt = returnPrompt;
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
                returnPrompt,
                new Vector2(430, windowHeight - 50),
                Color.White
                );

            // Draw the score achieved
            _spriteBatch.DrawString(
                senBold,
                String.Format("Score: {0:n0}", score),
                new Vector2(15, 15),
                Color.White
                );

            // Play again prompt
            _spriteBatch.Draw(
                playAgainPrompt,
                new Vector2(445, windowHeight - 450),
                Color.White
                );
        }
    }
}
