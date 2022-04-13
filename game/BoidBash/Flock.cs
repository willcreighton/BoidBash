using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// Uses many references
// https://www.youtube.com/watch?v=bqtqltqcQhw
// https://www.youtube.com/watch?v=QbUPfMXXQIY
// https://www.youtube.com/watch?v=mhjuuHl6qHM
// https://www.youtube.com/watch?v=x-GwBH4dhko
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
        // The boundaries the boids are created within
        private Rectangle creationBounds;
        // Random object determining starting positions
        private Random rng;
        // List of all areas to avoid
        private List<Rectangle> boundaries = new List<Rectangle>();
        // List of Pens
        private Bashers bashers;
        private Color backgroundColor = new Color(5, 5, 5);

        // Boid values
        private Texture2D asset;
        private Vector2 size;
        private Color defaultColor;

        // Values to mess with:
        // Arbitrary scalars that control how the boids move
        // Cohesion scalar determines how much the cohesion vector is divided by, and how far the boid will move
        private const float cohesionScalar = 10000;
        // Separation scalar determines the distance boids try to keep from other boids
        private const float separationScalar = 15;
        // Alignment scalar determines what the vector is divided by, and so the amount alignment impacts
        private const float alignmentScalar = 0.5f;
        // Velocity Limit determines the velocity that boids are not allowed to exceed
        private const float velocityLimit = 3;
        // Bounds avoidance is the distance where boids start to turn away from bounds
        private const float boundsAvoidance = 10;
        // Determines the velocity range from -BV to +BV the boids can start at
        private const float beginningVelocity = 3;
        // Visual Range determines how far the boid can see
        private const float visualRange = 50;
        // Predator avoidance describes what distance boids attempt to keep from the predator
        private const float predatorAvoidance = 113;

        // Experimental: Boid randomness
        private const float randomnessRange = 0.5f;

        // Properties
        // !Debug Property!
        public List<Rectangle> Boundaries
        {
            get { return boundaries; }
            set { boundaries = value; }
        }

        /// <summary>
        /// Accesses the list of boids in the flock
        /// </summary>
        public List<Boid> Boids
        {
            get { return boids; }
        }

        /// <summary>
        /// Accesses the pens that the flock has
        /// </summary>
        public Bashers Bashers
        {
            get { return bashers; }
        }

        /// <summary>
        /// Aids trail transition
        /// </summary>
        public Color BackgroundColor
        {
            set { backgroundColor = value; }
        }

        /// <summary>
        /// Sets and returns default boid color
        /// </summary>
        public Color DefaultColor
        {
            get { return defaultColor; }
            set { defaultColor = value; }
        }

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
        public Flock(int numBoids, Rectangle creationBounds,
            Texture2D asset, Vector2 size, Color defaultColor,
            SpriteBatch sb)
        {
            // Initialzie Random0
            rng = new Random();

            // Pass down values to appropriate fields
            this.creationBounds = creationBounds;
            this.asset = asset;
            this.size = size;
            this.defaultColor = defaultColor;
            this.sb = sb;

            // Add the inital amount of boids to the list
            AddBoids(numBoids);

            // Initialize bashers
            bashers = new Bashers();
        }

        /// <summary>
        /// Processes the next frame of boids. Should be put in update method
        /// </summary>
        /// <param name="predatorPositions"></param>
        public void ProcessBoids(Vector2[] predatorPositions)
        {
            // Vectors determining the decisions of the boids
            // These are the rules the boids follow to determine their behavior
            Vector2 cohesion;
            Vector2 separation;
            Vector2 alignment;
            Vector2[] predatorAvoidance = new Vector2[predatorPositions.Length];

            // Loop through the list for each boid
            foreach (Boid b in boids)
            {
                // Calculate Velocity
                // Fetch the appropriate values for the boids
                cohesion = Cohesion(b);
                separation = Separation(b);
                alignment = Alignment(b);
                for (int x = 0; x < predatorPositions.Length; x++)
                {
                    predatorAvoidance[x] = PredatorAvoidance(b, predatorPositions[x]);
                }

                // Add all velocity modifiers to the boid
                b.Velocity = b.Velocity + cohesion + separation + alignment;
                foreach (Vector2 avoidance in predatorAvoidance)
                {
                    b.Velocity += avoidance;
                }
                // Limit the speed of boids so they don't go too fast
                LimitVelocity(b);

                // Calculate and apply bounds avoidance
                Bounds(b);

                // Update position based on rules being applied
                b.Position = b.Position + b.Velocity;

                // Update necessary boid values
                // Track if the boid is in a pen & update trail list
                InPen(b);
                // Update all Boid Trails
                UpdateTrailList(b);
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
            float distance;

            // Loop through all boids in the list
            foreach (Boid b in boids)
            {
                // Determine distance from other boid
                distance = Vector2.Distance(b.Position, boid.Position);

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
            float distance;

            // Loop through list of boids
            foreach (Boid b in boids)
            {
                // If it is not the boid these calculations are for
                if (b != boid)
                {
                    // Determine distance from other boid
                    distance = Vector2.Distance(b.Position, boid.Position);

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
        // TODO - Check that normalization does not hurt the intended direction

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
            float distance;

            // Loop through all boids in list
            foreach (Boid b in boids)
            {
                // Determine distance from other boid
                distance = Vector2.Distance(b.Position, boid.Position);

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
            // Makes the maximum velocity based on if it goes over the velocity limit
            if (Math.Sqrt(boid.Velocity.X * boid.Velocity.X + boid.Velocity.Y * boid.Velocity.Y) > velocityLimit)
            {
                // If higher than limit, set to max speed but same angle
                boid.Velocity = (boid.Velocity / (float)Math.Sqrt(boid.Velocity.X * boid.Velocity.X + boid.Velocity.Y * boid.Velocity.Y)) * velocityLimit;
            }
        }

        /// <summary>
        /// Returns a vector determining the boid's avoidance of the edges of the boundaries
        /// This is a post process on the boids velocity, ensuring they do not pass the bounds
        /// </summary>
        /// <param name="boid"></param>
        /// <returns></returns>
        private void Bounds(Boid boid)
        {
            // This will Take the velocity, and change it to make sure it does not pass through the boundaries

            // Check for each boundary
            foreach (Rectangle bound in boundaries)
            {

                // If within a boundary, break and start repositioning
                if (bound.Contains(boid.Position))
                {
                    RepositionBoid(boid);
                    break;
                }

                // Find the left, right, height and width
                float left = bound.X;
                float top = bound.Y;
                float width = bound.Width;
                float height = bound.Height;

                Vector2 newVelocity = boid.Velocity;

                // New variables referring to initial point
                float x = boid.Position.X;
                float y = boid.Position.Y;

                // Find right and bottom
                float right = left + width;
                float bottom = top + height;

                // Clamp the x and y values, so the nearest point will be
                // within the range of the rectangle
                x = Math.Clamp(x, left, right);
                y = Math.Clamp(y, top, bottom);

                // Calculate distances to sides
                float dl = Math.Abs(x - left);
                float dr = Math.Abs(x - right);
                float dt = Math.Abs(y - top);
                float db = Math.Abs(y - bottom);
                // Find minimum distance
                float m = Math.Min(Math.Min(dl, dr), Math.Min(dt, db));

                // If the minimum distance is one of the sides,
                // Apply Bouncing
                if (m == dt && bound.Contains(boid.Position + boid.Velocity))
                {
                    newVelocity = Vector2.Reflect((boid.Velocity), new Vector2(0, 1));
                }
                else if (m == db && bound.Contains(boid.Position + boid.Velocity))
                {
                    newVelocity = Vector2.Reflect((boid.Velocity), new Vector2(0, -1));
                }
                else if (m == dl && bound.Contains(boid.Position + boid.Velocity))
                {
                    newVelocity = Vector2.Reflect((boid.Velocity), new Vector2(-1, 0));
                }
                else if (bound.Contains(boid.Position + boid.Velocity))
                {
                    newVelocity = Vector2.Reflect((boid.Velocity), new Vector2(1, 0));
                }

                boid.Velocity = newVelocity;
            }
        }

        /// <summary>
        /// Returns a vector determing how much boids steer away from the predator
        /// </summary>
        /// <param name="b"></param>
        /// <param name="predatorPos"></param>
        /// <returns></returns>
        private Vector2 PredatorAvoidance(Boid boid, Vector2 predatorPos)
        {
            // Currently unimplemented
            Vector2 predatorAvoidanceVector = new Vector2(0, 0);
            float distance;

            // Determine distance from predator
            distance = Vector2.Distance(boid.Position, predatorPos);

            // If distance is less than predator avoidance scalar
            // predatorAvoidance scalar determines the distance boids try to keep from the predator
            if (distance < predatorAvoidance)
            {
                // If too close, move away appropriate amount
                predatorAvoidanceVector -= (predatorPos - boid.Position);
            }

            // Return Predator avoidance
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
                    velocity = new Vector2(rng.Next(-30, 30), rng.Next(-30, 30));

                    // Add new boid to list
                    boids.Add(new Boid(position, velocity));
                }
            }
            // Set one special boid
            Boids[0].IsSpecial = true;
            Boids[0].UseDefaultColor = false;
            boids[0].Color = Color.Gold;
            boids[0].HasTrail = true;

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

        /// <summary>
        /// Removes all boids from the flock
        /// </summary>
        public void ClearFlock()
        {
            boids.Clear();
        }

        /// <summary>
        /// Draws the trails of all boids with trails
        /// </summary>
        /// <param name="boid"></param>
        public void DrawTrail(Boid boid)
        {
            // If the boid has a trail, and the trail has points
            if (boid.HasTrail && boid.Trail != null && boid.Trail.Count > 0)
            {
                // Initialize interval integers
                int rInterval;
                int gInterval;
                int bInterval;

                // Determine what color interval to use
                if (boid.UseDefaultColor)
                {
                    // Calculate the difference in color and divide by the number of points in the trail
                    rInterval = (defaultColor.R - backgroundColor.R) / boid.Trail.Count;
                    gInterval = (defaultColor.G - backgroundColor.G) / boid.Trail.Count;
                    bInterval = (defaultColor.B - backgroundColor.B) / boid.Trail.Count;
                }
                else
                {
                    rInterval = (boid.Color.R - backgroundColor.R) / boid.Trail.Count;
                    gInterval = (boid.Color.G - backgroundColor.G) / boid.Trail.Count;
                    bInterval = (boid.Color.B - backgroundColor.B) / boid.Trail.Count;
                }

                // Draw the lines
                // Add a blank space leading up to the boid so that they can't tell it doesn't exactly match up
                for (int x = 0; x < boid.Trail.Count - 7; x++)
                {
                    ShapeBatch.Line(boid.Trail[x], boid.Trail[x + 1],
                        new Color(backgroundColor.R + (rInterval * x), backgroundColor.G
                        + (gInterval * x), backgroundColor.B + (bInterval * x)));
                }
            }
        }

        /// <summary>
        /// Gives a random color value to the boid and sets tells it to use it
        /// </summary>
        /// <param name="boid"></param>
        public void GiveColor(Boid boid)
        {
            // Tell to draw with this color
            boid.UseDefaultColor = false;

            // Random pastel values
            int r = rng.Next(125, 256);
            int g = rng.Next(125, 256);
            int b = rng.Next(125, 256);

            // Assign color
            boid.Color = new Color(r, g, b);
        }

        /// <summary>
        /// Updates the list of points for the trail
        /// </summary>
        /// <param name="boid"></param>
        public void UpdateTrailList(Boid boid)
        {
            // If the trail has a trail
            if (boid.HasTrail)
            {
                // Add current position to the list
                boid.Trail.Add(boid.Position);

                // Keep trail to specified length
                if (boid.Trail.Count > 50)
                {
                    boid.Trail.RemoveAt(0);
                }
            }
        }

        /// <summary>
        /// Places boid at random place within the creation bounds
        /// </summary>
        /// <param name="boid"></param>
        public void RepositionBoid(Boid boid)
        {
            Vector2 position;

            // Randomize position to within creation bounds
            position = new Vector2(rng.Next(creationBounds.X, creationBounds.X + creationBounds.Width),
                rng.Next(creationBounds.Y, creationBounds.Y + creationBounds.Height));

            // Clears trail for no awkward movement
            if (boid.HasTrail)
            {
                boid.Trail.Clear();
            }

            // Add new boid to list
            boid.Position = position;
        }

        /// <summary>
        /// Draws each boid in the list
        /// </summary>
        public void Draw()
        {
            // Loop through all boids in list
            foreach (Boid b in boids)
            {
                float angle = (float)Math.Atan2((double)b.Velocity.X, (double)b.Velocity.Y);

                if (b.UseSpecialAsset)
                {
                    sb.Draw(b.SpecialAsset, new Rectangle((int)b.Position.X, (int)b.Position.Y, (int)size.X + 4, (int)size.Y + 5),
                        null, b.Color, 90 + angle, new Vector2(0, 0), SpriteEffects.None, 0);
                }
                else if (b.UseDefaultColor)
                {
                    // Draw the boid to the spritebatch
                    sb.Draw(asset, new Rectangle((int)b.Position.X, (int)b.Position.Y, (int)size.X, (int)size.Y),
                        null, defaultColor, angle, new Vector2(0, 0), SpriteEffects.None, 0);
                }               
                else if (b.IsSpecial)
                {
                    // Draw the boid to the spritebatch
                    sb.Draw(asset, new Rectangle((int)b.Position.X, (int)b.Position.Y, (int)size.X + 2, (int)size.Y + 2),
                        null, b.Color, angle, new Vector2(0, 0), SpriteEffects.None, 0);
                }
                else
                {
                    // Draw the boid to the spritebatch
                    sb.Draw(asset, new Rectangle((int)b.Position.X, (int)b.Position.Y, (int)size.X, (int)size.Y),
                        null, b.Color, angle, new Vector2(0, 0), SpriteEffects.None, 0);
                }
                // Draw the boid trail
                DrawTrail(b);
            }
        }

        /// <summary>
        /// Detect if a boid is within a pen, and if so, flag it within said pen
        /// </summary>
        /// <returns></returns>
        public void InPen(Boid boid)
        {
            bool within = false;
            // If there are pens
            if (bashers.Pens.Count > 0)
            {
                // For each pen in the list
                for (int x = 0; x < bashers.Pens.Count; x++)
                {
                    // If the boid is within the pen, flag it as such
                    if (bashers.Pens[x].Contains(boid.Position.X, boid.Position.Y))
                    {
                        within = true;
                        boid.Pen = x;
                    }
                    if (!within)
                    {
                        // If the boid is not within any pen, give it -1
                        boid.Pen = -1;
                    }
                }
            }
        }

        /// <summary>
        /// Repositions all boids within the flock to random points within the creation bounds
        /// </summary>
        public void RepositionBoids()
        {
            // Loop from 0 to desired number of boids
            foreach (Boid boid in boids)
            {
                RepositionBoid(boid);
            }
        }

        /// <summary>
        /// Repositions a boid to certain place
        /// </summary>
        /// <param name="boid"></param>
        /// <param name="pos"></param>
        public void RepositionBoidTo(Boid boid, Vector2 pos)
        {
            boid.Position = pos;
        }
    }
}