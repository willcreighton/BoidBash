using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Timers;

namespace BoidBash
{
    /// <summary>
    /// This class handles drawing the Game state's UI
    /// </summary>
    class GameUI
    {
        // Fields
        private int windowWidth;
        private int windowHeight;
        private SpriteFont headerFont;
        private SpriteFont primaryFont;
        private Texture2D boidBashLogo;

        private ulong score = 0;
        private int timer = 60;

        // Constructor
        public GameUI(int windowWidth, int windowHeight, SpriteFont headerFont, SpriteFont primaryFont, Texture2D boidBashLogo)
        {
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
            this.headerFont = headerFont;
            this.primaryFont = primaryFont;
            this.boidBashLogo = boidBashLogo;
        }

        /// <summary>
        /// This method tracks the increase in score
        /// </summary>
        /// <param name="scoreIncrease"></param>
        /// <returns></returns>
        public ulong ScoreUpdater(ulong scoreIncrease)
        {
            score = scoreIncrease;

            return score;
        }

        /// <summary>
        /// Draw the Game UI
        /// </summary>
        /// <param name="_spriteBatch"></param>
        public void Draw(SpriteBatch _spriteBatch)
        {
            // State display
            _spriteBatch.Draw(
                boidBashLogo,
                new Rectangle(1085, 800, 100, 90),
                Color.White
                );

            // Pause Menu prompt
            _spriteBatch.DrawString(
                primaryFont,
                "Press P to Pause",
                new Vector2(20, windowHeight - 40),
                Color.White
                );
        }

        /// <summary>
        /// Draw the Score
        /// </summary>
        /// <param name="_spriteBatch"></param>
        public void DrawScore(SpriteBatch _spriteBatch)
        {
            // Score display
            _spriteBatch.DrawString(
                headerFont,
                String.Format("Score: {0:n0}", score),
                new Vector2(15, 15),
                Color.White
                );
        }

        /// <summary>
        /// Draw the Timer
        /// </summary>
        /// <param name="_spriteBatch"></param>
        public void DrawTimer(SpriteBatch _spriteBatch)
        {
            // Timer display
            _spriteBatch.DrawString(
                headerFont,
                "Time: " + timer,
                new Vector2(500, 15),
                Color.White
                );
        }

        /// <summary>
        /// Draw the Score Goal
        /// </summary>
        /// <param name="_spriteBatch"></param>
        public void DrawScoreGoal(SpriteBatch _spriteBatch, int scoreGoal)
        {
            // Score Goal display
            _spriteBatch.DrawString(
                headerFont,
                "Bash Goal: " + scoreGoal,
                new Vector2(500, 115),
                Color.White
                );
        }
    }
}
