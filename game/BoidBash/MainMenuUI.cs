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
        private Texture2D playPrompt;
        private Texture2D boidBashLogo;
        private SpriteFont senBold;
        private SpriteFont senRegular;
        private float time = 2f;

        // Constructor
        public MainMenuUI(int windowWidth, int windowHeight, Texture2D playPrompt, Texture2D boidBashLogo, SpriteFont senBold, SpriteFont senRegular)
        {
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
            this.playPrompt = playPrompt;
            this.boidBashLogo = boidBashLogo;
            this.senBold = senBold;
            this.senRegular = senRegular;
        }

        //Updates the time and resets it
        public void Update(GameTime gameTime)
        {
            if (time > 0)
            {
                time -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (time <= 0)
            {
                time = 2f;
            }
            
        }

        /// <summary>
        /// Draw the Main Menu
        /// </summary>
        /// <param name="_spriteBatch"></param>
        public void Draw(SpriteBatch _spriteBatch)
        {
            // Boid Bash logo
            _spriteBatch.Draw(
                boidBashLogo,
                new Rectangle(325, 75, 500, 300),
                Color.White
                );
            
            // Play prompt
            _spriteBatch.Draw(
                playPrompt,
                new Vector2(460, windowHeight - 450),
                Color.White
                );

            //Draws Insert Coin every other second
            if (time > 1)
            {
                //Insert coin text
                _spriteBatch.DrawString(
                    senBold, 
                    "Insert Coin",
                    new Vector2(475, windowHeight - 50),
                    Color.Gold
                    );
            }

            //Draws title for Highscores
            _spriteBatch.DrawString(
                senRegular, 
                String.Format("HIGH SCORES"), 
                new Vector2(500, 550), 
                Color.White
                );
        }
    }
}
