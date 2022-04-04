using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BoidBash
{
    /// <summary>
    /// This class handles the Basher object
    /// </summary>
    class Basher
    {
        // Fields
        private Rectangle penBounds;

        // Properties
        public Rectangle PenBounds
        {
            get { return PenBounds;  }
        }
        // Constructor
        public Basher(Rectangle bounds)
        {
            penBounds = bounds;
        }

        // Methods
        // --- TODO ---
        // Implement buttons as seen in Events and Delegates PE
    }
}
