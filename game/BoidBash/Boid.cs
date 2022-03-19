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
        // Random Bias to give mind of own
        private Vector2 bias;

        /// <summary>
        /// Returns and sets the Bias of the boid
        /// </summary>
        public Vector2 Bias
        {
            get { return bias; }

            set { bias = value; }

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
        /// Constructs a boid given a starting position and starting velocity
        /// </summary>
        /// <param name="position"></param>
        public Boid(Vector2 position, Vector2 velocity)
        {
            this.position = position;
            this.velocity = velocity;
            bias = velocity;
        }
    }
}