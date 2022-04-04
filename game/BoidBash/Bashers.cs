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
    /// (The reason the basher is being handled this way is so
    /// that in the event we add multiplayer, we can different
    /// bashers increment different points, but still store multiple
    /// pens per basher)
    /// </summary>
    class Bashers
    {
        // Fields
        private List<Rectangle> pens;

        // Properties
        public List<Rectangle> Pens
        {
            get { return pens;  }
        }

        // Constructor
        public Bashers()
        {
            
        }

        /// <summary>
        /// Adds a new pen to detect for the basher
        /// </summary>
        /// <param name="pen"></param>
        public void AddPen(Rectangle pen)
        {
            pens.Add(pen);
        }

        // Methods
        // --- TODO ---
        // Implement buttons as seen in Events and Delegates PE

        // Hey Team K, to use this, each button is associated with a rectangle. 
        //  Call this method when the button is clicked, and pass in the flock of boids and the index
        //  that the rectangle is at in the list in order to use it. So, button 1; Pen 1; send in 1 as the parameter.
        /// <summary>
        /// Destroys all boids within the specified pen.
        /// Takes the flock and the index of the rectangle the boid is in as parameters
        /// </summary>
        /// <param name="flock"></param>
        /// <param name="pen"></param>
        public void DestroyContainedBoids(Flock flock, int pen)
        {
            foreach (Boid boid in flock.Boids)
            {
                if (boid.Pen == pen)
                {
                    // TODO - increment score
                    // TODO - apply visuals
                    flock.RemoveBoid(boid);
                }
            }
        }
    }
}
