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
        private Color backgroundColor;

        private long score = 0;
        private int timer = 60;
        private int scoreGoal = 1;

        public Timer aTmr = new Timer(1000);

        // Constructor
        public GameUI(int windowWidth, int windowHeight, SpriteFont headerFont, SpriteFont primaryFont)
        {
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
            this.headerFont = headerFont;
            this.primaryFont = primaryFont;

            backgroundColor = new Color(20, 20, 20);
        }

        /// <summary>
        /// This method tracks the increase in score
        /// </summary>
        /// <param name="scoreIncrease"></param>
        /// <returns></returns>
        public long ScoreUpdater(long scoreIncrease)
        {
            score += scoreIncrease;

            return score;
        }

        /// <summary>
        /// This method decrements the timer
        /// </summary>
        public void TimerUpdater()
        {
            aTmr.Elapsed -= ATmr_Elapsed;
            aTmr.Enabled = true;
            aTmr.AutoReset = true;
            aTmr.Start();
            Console.Read();
        }

        public void ATmr_Elapsed(object sender, ElapsedEventArgs e)
        {
            //timer = timer - 1;
            timer--;
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method updates the score goal
        /// </summary>
        /// <returns></returns>
        public int ScoreGoalUpdater()
        {
            // If score goal has increased then
            // update scoreGoal

            return scoreGoal;
        }

        /// <summary>
        /// Draw the Game UI
        /// </summary>
        /// <param name="_spriteBatch"></param>
        public void Draw(SpriteBatch _spriteBatch)
        {
            // State display
            _spriteBatch.DrawString(
                headerFont,
                "BOID BASH",
                new Vector2(20, 15),
                Color.White
                );

            // Pause Menu prompt
            _spriteBatch.DrawString(
                primaryFont,
                "Press 'P' to access the Pause Menu.",
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
                "Score: " + score,
                new Vector2(1000, 15),
                Color.White
                );
        }

        /// <summary>
        /// Draw the Timer
        /// </summary>
        /// <param name="_spriteBatch"></param>
        public void DrawTimer(SpriteBatch _spriteBatch)
        {
            // Score display
            _spriteBatch.DrawString(
                headerFont,
                "Timer: " + timer,
                new Vector2(500, 15),
                Color.White
                );
        }

        /// <summary>
        /// Draw the Score Goal
        /// </summary>
        /// <param name="_spriteBatch"></param>
        public void DrawScoreGoal(SpriteBatch _spriteBatch)
        {
            // Score display
            _spriteBatch.DrawString(
                headerFont,
                "Score Goal: " + scoreGoal,
                new Vector2(20, 300),
                Color.White
                );
        }
    }
}
