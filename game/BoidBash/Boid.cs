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
        // Set to true if it is a special boid
        private bool isSpecial = false;
        // If the boid uses Flock's default color
        private bool useDefaultColor = true;
        // Boids color, used if it doesn't use default
        private Color color;
        // If the boid has a trail
        private bool hasTrail = false;
        // The boids trail list of points, if it has a trail
        private List<Vector2> trail = null;
        // If not in a pen, the boid is set to -1.
        // If it IS in a pen, the boid is set to the index of that pen in the list of pens until it leaves
        private int pen = -1;
        private bool useSpecialAsset = false;
        private Texture2D specialAsset;

        /// <summary>
        /// Set's the boid's special asset
        /// </summary>
        public Texture2D SpecialAsset
        {
            get { return specialAsset; }
            set { specialAsset = value; }
        }

        /// <summary>
        /// Returns if the boid uses a special asset
        /// </summary>
        public bool UseSpecialAsset
        {
            set { useSpecialAsset = value; }
            get { return useSpecialAsset; }
        }

        /// <summary>
        /// Sets and returns the boid's trail list
        /// </summary>
        public List<Vector2> Trail
        {
            get { return trail; }
            set { trail = value; }
        }
        /// <summary>
        /// Returns and handles if a boid has a trail
        /// </summary>
        public bool HasTrail
        {
            get { return hasTrail; }
            set
            {
                // Removing reference to list if removing the trail
                if (!value)
                {
                    trail = null;
                }
                // Initializes new trail if adding a trail
                else if (trail == null)
                {
                    trail = new List<Vector2>();
                }
                // Sets value
                hasTrail = value;
            }
        }

        /// <summary>
        /// Sets and returns the Boid's color
        /// (only used if UseDefaultColor is false) 
        /// </summary>
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        /// <summary>
        /// Sets and returns if the boids uses the flock's default color
        /// </summary>
        public bool UseDefaultColor
        {
            get { return useDefaultColor; }
            set { useDefaultColor = value; }
        }

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