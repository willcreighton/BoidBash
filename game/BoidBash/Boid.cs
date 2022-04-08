using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BoidBash
{
    /// <summary>
    /// Data of individual boids
    /// </summary>
    class Boid
    {
        // Only Necessary values for boids
        // Tracks where the boid is
        private Vector2 position;
        // Tracks where the boids is headed
        private Vector2 velocity;
        // set to true if it is a special boid
        private bool isSpecial = false;

        // If not in a pen, the boid is set to -1.
        // If it IS in a pen, the boid is set to the index of that pen in the list of pens until it leaves
        private int pen = -1;

        /// <summary>
        /// Returns and sets the pen the boid is in
        /// </summary>
        public int Pen
        {
            get { return pen; }

            set { pen = value; }
        }

        /// <summary>
        /// Returns and sets the position of the boid
        /// </summary>
        public Vector2 Position
        {
            get { return position; }

            set { position = value; }
        }

        /// <summary>
        /// Returns and sets the velocity of the boid
        /// </summary>
        public Vector2 Velocity
        {
            get { return velocity; }

            set { velocity = value; }
        }

        /// <summary>
        /// Returns and sets if the boid has a special property
        /// </summary>
        public bool IsSpecial
        {
            get { return isSpecial; }
            set { isSpecial = value; }
        }
 
        /// <summary>
        /// Constructs a boid given a starting position and starting velocity
        /// </summary>
        /// <param name="position"></param>
        public Boid(Vector2 position, Vector2 velocity)
        {
            this.position = position;
            this.velocity = velocity;
        }
    }
}