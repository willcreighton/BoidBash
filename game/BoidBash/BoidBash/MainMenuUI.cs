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
        private Texture2D insertCoin;
        private SpriteFont senBold;
        private float time = 2f;

        // Constructor
        public MainMenuUI(int windowWidth, int windowHeight, Texture2D playPrompt, Texture2D boidBashLogo, Texture2D insertCoin, SpriteFont senBold)
        {
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
            this.playPrompt = playPrompt;
            this.boidBashLogo = boidBashLogo;
            this.insertCoin = insertCoin;
            this.senBold = senBold;
        }

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
                new Rectangle(405, 100, 342, 300),
                Color.White
                );

            // Play prompt
            _spriteBatch.Draw(
                playPrompt,
                new Vector2(460, windowHeight - 400),
                Color.White
                );

            

            //TRYING TO MAKE THE INSERT COIN PROMPT BLINK - RYAN
            
              //Note from brian: Since it's a float, it won't work with modulus, since it is almost never exactly % 2 == 0
             // Instead, maybe detect if the decimal part of the value is > or < .5? that might work
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
            //_spriteBatch.DrawString(senBold, "Time: " + String.Format("{0:0.00}", time.ToString("0")), new Vector2(900, 15),Color.White);
            
        }
    }
}
