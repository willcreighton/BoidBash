using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PredatorBuild
{
    class Predator : GameObject
    {
        // Parameterized Constructor
        public Predator(Texture2D texture, Rectangle position) : 
            base(texture, position)
        {

        }

        // Movement
        public override void Update(GameTime gameTime)
        {
            KeyboardState keyBState = Keyboard.GetState();

            if (keyBState.IsKeyDown(Keys.Left))
            {
                position.X -= 5;
            }
            if (keyBState.IsKeyDown(Keys.Right))
            {
                position.X += 5;
            }
            if (keyBState.IsKeyDown(Keys.Up))
            {
                position.Y -= 5;
            }
            if (keyBState.IsKeyDown(Keys.Down))
            {
                position.Y += 5;
            }
        }
    }
}
