using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BoidBash
{
    /// <summary>
    /// This class handles drawing the Main Menu state's UI
    /// </summary>
    class MainMenuUI
    {
        // Fields
        private int windowWidth;
        private int windowHeight;
        private SpriteFont headerFont;
        private SpriteFont primaryFont;
        private Texture2D playPrompt;
        private Texture2D boidBashLogo;

        // Constructor
        public MainMenuUI(int windowWidth, int windowHeight, SpriteFont headerFont, SpriteFont primaryFont, Texture2D playPrompt, Texture2D boidBashLogo)
        {
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
            this.headerFont = headerFont;
            this.primaryFont = primaryFont;
            this.playPrompt = playPrompt;
            this.boidBashLogo = boidBashLogo;
        }

        /// <summary>
        /// Draw the Main Menu
        /// </summary>
        /// <param name="_spriteBatch"></param>
        public void Draw(SpriteBatch _spriteBatch)
        {
            // State display
            _spriteBatch.Draw(
                boidBashLogo,
                new Rectangle(405, 100, 342, 300),
                Color.White
                );

            // Game prompt
            _spriteBatch.Draw(
                playPrompt,
                new Vector2(460, windowHeight - 400),
                Color.White
                );
        }
    }
}
