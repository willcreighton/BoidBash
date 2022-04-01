using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// Uses many references
// http://www.kfish.org/boids/pseudocode.html

namespace BoidBash
{
    /// <summary>
    /// Controls a list of boids, and enforces their rules
    /// </summary>
    class Flock
    {
        // Spritebatch to draw on
        SpriteBatch sb;

        // Flock values
        // List of all boid objects
        private List<Boid> boids = new List<Boid>();
        // The boundaries the boids must stay within
        private Rectangle bounds;
        // The boundaries the boids are created within
        private Rectangle creationBounds;
        // Random object determining starting positions
        private Random rng;

        // Boid values
        private Texture2D asset;
        private Vector2 size;
        private Color defaultColor;

        // Values to mess with:
        // Arbitrary scalars that control how the boids move
        // Cohesion scalar determines how much the cohesion vector is divided by, and how far the boid will move
        private const float cohesionScalar = .5f;
        // Separation scalar determines the distance boids try to keep from other boids
        private const float separationScalar = 2;
        // Alignment scalar determines what the vector is divided by, and so the amount alignment impacts
        private const float alignmentScalar = .3f;
        // Velocity Limit determines the velocity that boids are not allowed to exceed
        private const float velocityLimit = 3;
        // Bounds avoidance determines the speed the boids turn away from the boids
        private const float boundsAvoidance = 10;
        // Determines the velocity range from -BV to +BV the boids can start at
        private const float beginningVelocity = 3;
        // Visual Range determines how far the boid can see
        private const float visualRange = 999;

        // Experimental: Boid randomness
        private const float randomnessRange = 0.5f;

        /// <summary>
        /// Creates a Flock
        /// </summary>
        /// <param name="numBoids"></param>
        /// <param name="bounds"></param>
        /// <param name="creationBounds"></param>
        /// <param name="asset"></param>
        /// <param name="size"></param>
        /// <param name="defaultColor"></param>
        /// <param name="sb"></param>
        public Flock(int numBoids, Rectangle bounds, Rectangle creationBounds,
            Texture2D asset, Vector2 size, Color defaultColor, SpriteBatch sb)
        {
            // Initialzie Random0
            rng = new Random();

            // Pass down values to appropriate fields
            this.bounds = bounds;
            this.creationBounds = creationBounds;
            this.asset = asset;
            this.size = size;
            this.defaultColor = defaultColor;
            this.sb = sb;

            // Add the inital amount of boids to the list
            AddBoids(numBoids);
        }

        /// <summary>
        /// Processes the next frame of boids. Should be put in update method
        /// </summary>
        /// <param name="predatorPosition"></param>
        public void ProcessBoids(Vector2 predatorPosition)
        {
            // Vectors determining the decisions of the boids
            // These are the rules the boids follow to determine their behavior
            Vector2 cohesion;
            Vector2 separation;
            Vector2 alignment;
            Vector2 bounds;
            Vector2 predatorAvoidance;

            // Loop through the list for each boid
            foreach (Boid b in boids)
            {
                // Fetch the appropriate values for the boids
                cohesion = Cohesion(b);
                separation = Separation(b);
                alignment = Alignment(b);
                bounds = Bounds(b);
                predatorAvoidance = PredatorAvoidance(b, predatorPosition);

                // Add all velocity modifiers to the boid
                b.Velocity = b.Velocity + cohesion + separation + alignment + bounds + predatorAvoidance;
                // Limit the speed of boids so they don't go too fast
                LimitVelocity(b);
                // Update position based on rules being applied
                b.Position = b.Position + b.Velocity;
            }
        }

        /// <summary>
        /// Returns a vector representing the decision to move towards the center of the flock
        /// </summary>
        /// <param name="boid"></param>
        /// <returns></returns>
        private Vector2 Cohesion(Boid boid)
        {
            // Vector to be returned, initalized at zero so others can be added to it
            Vector2 cohesionVector = new Vector2(0, 0);
            // Distance from boid
            int distance;

            // Loop through all boids in the list
            foreach (Boid b in boids)
            {
                // Determine distance from other boid
                distance = (int)Math.Sqrt(Math.Pow(boid.Position.X - b.Position.X, 2)
                    + Math.Pow(boid.Position.Y - b.Position.Y, 2));

                // If boid is within distance
                if (distance < visualRange)
                {
                    // Add the position of each boid to the vector
                    cohesionVector += b.Position;
                }
            }
            // Remove the position of the current boid so it is not factored in
            cohesionVector -= boid.Position;
            // Get the average position of all boids
            cohesionVector /= boids.Count - 1;

            // Return the vector divided by the cohesion scalar
            // Cohesion scalar determines how much the vector is divided by, and how far the boid will move
            return cohesionVector / cohesionScalar;
        }

        /// <summary>
        /// Returns a vector representing the decison to stay away from other boids
        /// </summary>
        /// <param name="boid"></param>
        /// <returns></returns>
        private Vector2 Separation(Boid boid)
        {
            // Vector to be returned, intialized at zero so it can be added/subtracted to
            Vector2 separationVector = new Vector2(0, 0);
            // Distance variable for how far current boid is from boid in loop
            int distance;

            // Loop through list of boids
            foreach (Boid b in boids)
            {
                // If it is not the boid these calculations are for
                if (b != boid)
                {
                    // Determine distance from other boid
                    distance = (int)Math.Sqrt(Math.Pow(boid.Position.X - b.Position.X, 2)
                        + Math.Pow(boid.Position.Y - b.Position.Y, 2));

                    // If distance is less than separation scalar
                    // Separation scalar determines the distance boids try to keep from other boids
                    if (distance < separationScalar)
                    {
                        // If too close, move away appropriate amount
                        separationVector -= (b.Position - boid.Position);
                    }
                }
            }

            // Return finished separation
            return separationVector;
        }

        /// <summary>
        /// Returns a vector representing the decion to align with other boids
        /// </summary>
        /// <param name="boid"></param>
        /// <returns></returns>
        private Vector2 Alignment(Boid boid)
        {
            // Vector to be returned, initialized at zero so it can be added to
            Vector2 alignmentVector = new Vector2(0, 0);
            // Distance from boid
            int distance;

            // Loop through all boids in list
            foreach (Boid b in boids)
            {
                // Determine distance from other boid
                distance = (int)Math.Sqrt(Math.Pow(boid.Position.X - b.Position.X, 2)
                    + Math.Pow(boid.Position.Y - b.Position.Y, 2));

                // If the boid is within a certain range
                if (distance < visualRange)
                {
                    // Add all velocities togerher
                    alignmentVector += b.Velocity;
                }
            }

            // This time, we do account for the current boid being processed as well
            // Get Average velocity
            alignmentVector /= boids.Count - 1;

            // Return the vector divided by alignment scalar
            // Alignment scalar determines what the vector is divided by, and so the amount alignment impacts
            return alignmentVector / alignmentScalar;
        }

        /// <summary>
        /// Limits the boid's velocity to preferred speed
        /// </summary>
        /// <param name="boid"></param>
        private void LimitVelocity(Boid boid)
        {
            // Get the absolute velocity of the boid
            int absoluteVelocity = (int)Math.Sqrt(Math.Pow(boid.Position.X, 2)
                + Math.Pow(boid.Position.Y, 2));

            // If the absolute velocity is higher than the limit
            if (absoluteVelocity > velocityLimit)
            {
                // Keep boid direction, but change magnitude
                boid.Velocity = (boid.Velocity / absoluteVelocity) * velocityLimit;
            }
        }

        /// <summary>
        /// Returns a vector determining the boid's avoidance of the edges of the boundaries
        /// </summary>
        /// <param name="boid"></param>
        /// <returns></returns>
        private Vector2 Bounds(Boid boid)
        {
            // Get the minimum and max values to stay within
            int xMin = bounds.X;
            int xMax = bounds.X + bounds.Width;
            int yMin = bounds.Y + bounds.Height;
            int yMax = bounds.Y;

            // Velocity to be returned, initialized to zero if x/y value does not need to change
            Vector2 boundsVelocity = new Vector2(0, 0);

            // Determine if out of bounds
            if (boid.Position.X < xMin)
            {
                // If out of bounds, change velocity by bounds avoidance
                // Bounds avoidance determines the speed the boids turn away from the boids
                boundsVelocity.X = boundsAvoidance;
            }
            if (boid.Position.X > xMax)
            {
                boundsVelocity.X = boundsAvoidance * -1;
            }
            if (boid.Position.Y < yMin)
            {
                boundsVelocity.Y = boundsAvoidance;
            }
            if (boid.Position.Y > yMax)
            {
                boundsVelocity.Y = boundsAvoidance * -1;
            }

            // Return bounds velocity
            return boundsVelocity;

        }
        // Possibility: Second method that is a hard limit for the boids, make current bounds method cushioned by few pixels

        /// <summary>
        /// Returns a vector determing how much boids steer away from the predator
        /// </summary>
        /// <param name="b"></param>
        /// <param name="predatorPos"></param>
        /// <returns></returns>
        private Vector2 PredatorAvoidance(Boid b, Vector2 predatorPos)
        {
            // Currently unimplemented
            Vector2 predatorAvoidanceVector = new Vector2(0, 0);

            return predatorAvoidanceVector;
        }

        /// <summary>
        /// Adds boids using desired number to be added
        /// </summary>
        /// <param name="numBoids"></param>
        public void AddBoids(int numBoids)
        {
            // Vectors for new boid position and velocity
            Vector2 position;
            Vector2 velocity;

            // If boids are being added
            if (numBoids > 0)
            {
                // Loop from 0 to desired number of boids
                for (int x = 0; x < numBoids; x++)
                {
                    // Randomize position to within creation bounds
                    position = new Vector2(rng.Next(creationBounds.X, creationBounds.X + creationBounds.Width),
                        rng.Next(creationBounds.Y, creationBounds.Y + creationBounds.Height));

                    // Randomize velocity for new boids
                    velocity = new Vector2(rng.Next(-3, 3), rng.Next(-3, 3));

                    // Add new boid to list
                    boids.Add(new Boid(position, velocity));
                }
            }
        }
        // Possibility: Change creation bounds to a circle

        /// <summary>
        /// Remove a specified boid from list
        /// </summary>
        /// <param name="boid"></param>
        public void RemoveBoid(Boid boid)
        {
            boids.Remove(boid);
        }
        // To Do: Determine if boid is in any of specified zones, so crushers can function

        public void ClearFlock()
        {
            for (int x = boids.Count - 1; x >= 0; x--)
            {
                RemoveBoid(boids[x]);
            }
        }

        /// <summary>
        /// Draws each boid in the list
        /// </summary>
        public void Draw()
        {
            // Loop through all boids in list
            foreach (Boid b in boids)
            {
                // Calculate rotation in radians
                float angle = (float)Math.Atan2((double)b.Position.X, (double)b.Position.Y);
                // Draw the boid to the spritebatch
                sb.Draw(asset, new Rectangle((int)b.Position.X, (int)b.Position.Y, (int)size.X, (int)size.Y),
                    null, defaultColor, angle, new Vector2(0, 0), SpriteEffects.None, 0);
            }
        }
    }
}