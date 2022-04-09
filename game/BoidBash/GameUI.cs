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
        private Texture2D boidBashLogo;
        private Texture2D pausePrompt;

        private ulong score = 0;

        // Constructor
        public GameUI(int windowWidth, int windowHeight, SpriteFont headerFont, Texture2D boidBashLogo, Texture2D pausePrompt)
        {
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
            this.headerFont = headerFont;
            this.boidBashLogo = boidBashLogo;
            this.pausePrompt = pausePrompt;
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
        /// Draw the Pause prompt
        /// </summary>
        /// <param name="_spriteBatch"></param>
        public void DrawPausePrompt(SpriteBatch _spriteBatch)
        {
            // Pause Menu prompt
            _spriteBatch.Draw(
                pausePrompt,
                new Vector2(20, windowHeight - 40),
                Color.White
                );
        }

        /// <summary>
        /// Draw the Boid Bash logo
        /// </summary>
        /// <param name="_spriteBatch"></param>
        public void DrawLogo(SpriteBatch _spriteBatch)
        {
            // Boid Bash logo display
            _spriteBatch.Draw(
                boidBashLogo,
                new Rectangle(1085, 800, 100, 90),
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
        /// Draw the Score Goal
        /// </summary>
        /// <param name="_spriteBatch"></param>
        public void DrawScoreGoal(SpriteBatch _spriteBatch, int scoreGoal)
        {
            // Score Goal display
            _spriteBatch.DrawString(
                headerFont,
                "Bash Goal: " + scoreGoal,
                new Vector2(505, 115),
                Color.White
                );
        }
    }
}
