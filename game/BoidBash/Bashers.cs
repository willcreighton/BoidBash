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
        private List<Rectangle> pens = new List<Rectangle>();
        private List<Vector3> scorePrints = new List<Vector3>();

        // Properties
        public List<Rectangle> Pens
        {
            get { return pens;  }
        }
        // Clear after drawing all scores in draw method
        public List<Vector3> ScorePrints
        {
            get { return scorePrints; }
            set { scorePrints = value; }
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
        //  Use the first int that this returns to add to the score,
        //  and if the second is 1, add to the reached scoregoals, else, don't
        /// <summary>
        /// Destroys all boids within the specified pen.
        /// Takes the flock, the index of the rectangle the boid is in, and the current reached score goal as parameters
        /// </summary>
        /// <param name="flock"></param>
        /// <param name="pen"></param>
        public Vector2 DestroyContainedBoids(Flock flock, int pen, int scoregoal)
        {
            int scoreIncrease = 0;
            int boidsBashed = 0;
            int ifScoreGoal = -1;
            bool upScoreGoal = false;
            Vector2 returnNums;

            foreach (Boid boid in flock.Boids)
            {
                if (boid.Pen == pen)
                {
                    // Score Increment
                    boidsBashed++;
                    scoreIncrease += (int)Math.Pow(scoregoal, boidsBashed);

                    // Checking if new score goal has been reached
                    if (scoregoal < boidsBashed)
                    {
                        upScoreGoal = true;
                    }

                    // TODO - apply visuals
                    scorePrints.Add(new Vector3(boid.Position.X, boid.Position.Y, (float)Math.Pow(scoregoal, boidsBashed)));
                    flock.RemoveBoid(boid);
                }
            }

            // If reached score goal, set ifScoreGoal to 1 so it can be determined in Game
            if (upScoreGoal)
            {
                ifScoreGoal = 1;
            }

            // Assemble Vector
            returnNums = new Vector2(scoreIncrease, ifScoreGoal);

            return returnNums;
        }
    }
}
