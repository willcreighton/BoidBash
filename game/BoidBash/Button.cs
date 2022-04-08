using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BoidBash
{
    /// <summary>
    /// This class was initially created by Erika Mesh for the Events & Delegates PE
    /// 
    /// If the client wants to be notified when a button is clicked, it must
    /// implement a method matching this delegate and then tie that method to
    /// the button's "OnButtonClick" event.
    /// </summary>
    public delegate void OnButtonClickDelegate(int pen);

    /// <summary>
    /// Builds, monitors, and draws a customized Button
    /// </summary>
    public class Button
    {
        // Button specific fields
        private MouseState prevMState;
        private Rectangle position; // Button position and size
        private Texture2D buttonImg;
        private int pen;

        /// <summary>
        /// If the client wants to be notified when a button is clicked, it must
        /// implement a method matching OnButtonClickDelegate and then tie that method to
        /// the button's "OnButtonClick" event.
        /// 
        /// The delegate will be called with a reference to the clicked button.
        /// </summary>
        public event OnButtonClickDelegate OnButtonClick;

        /// <summary>
        /// Create a new custom button
        /// </summary>
        /// <param name="device">The graphics device for this game - needed to create custom button textures.</param>
        /// <param name="position">Where to draw the button's top left corner</param>
        /// <param name="color">The color to make the button's texture.</param>
        public Button(GraphicsDevice device, Rectangle position, Color color, int pen, Texture2D texture)
        {
            // Save copies/references to the info we'll need later
            this.position = position;
            this.pen = pen;

            // Load in the button's texture
            buttonImg = texture;
        }

        /// <summary>
        /// Each frame, update its status if it's been clicked.
        /// </summary>
        public void Update()
        {
            // Check/capture the mouse state regardless of whether this button
            // if active so that it's up to date next time!
            MouseState mState = Mouse.GetState();
            if (mState.LeftButton == ButtonState.Released &&
                prevMState.LeftButton == ButtonState.Pressed &&
                position.Contains(mState.Position))
            {
                if (OnButtonClick != null)
                {
                    // Call ALL methods attached to this button
                    OnButtonClick(pen);
                }
            }

            prevMState = mState;
        }

        /// <summary>
        /// Override the GameObject Draw() to draw the button
        /// </summary>
        /// <param name="spriteBatch">The spriteBatch on which to draw this button. The button 
        /// assumes that Begin() has already been called and End() will be called later.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the button itself
            spriteBatch.Draw(buttonImg, position, Color.White);
        }
    }
}
