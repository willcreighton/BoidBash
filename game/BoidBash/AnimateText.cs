using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoidBash
{
    /// <summary>
    /// This class allows you to easily animate text that is passed in
    /// </summary>
    class AnimateText
    {
        private float timerIn = 0; // -> 1
        private float timerOut = 1; // -> 0

        // Constructor
        public AnimateText()
        {

        }

        public void Fade(string text, SpriteFont senRegular, Vector2 position, Color color, SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Fade in
            while (timerIn < 1)
            {
                spriteBatch.DrawString(
                    senRegular,
                    text,
                    position,
                    color * timerIn
                    );

                timerIn += 0.01f;
                // Super mini delay so it does not occur instantly
            }

            // Wait x amount of seconds before fade out begins

            // Fade out
            while (timerOut > 0)
            {
                spriteBatch.DrawString(
                    senRegular,
                    text,
                    position,
                    color * timerOut
                    );

                timerOut -= 0.01f;
                // Super mini delay so it does not occur instantly
            }
        }
    }
}
