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
        private List<float> scoreTimers = new List<float>();
        private List<Vector3> specialScorePrints = new List<Vector3>();
        private List<float> specialScoreTimers = new List<float>();

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

        // Timers for all the scoreprints
        public List<float> ScoreTimers
        {
            get { return scoreTimers; }
            set { scoreTimers = value; }
        }

        // All special score prints
        public List<Vector3> SpecialScorePrints
        {
            get { return specialScorePrints; }
            set { specialScorePrints = value; }
        }

        // All special score timers
        public List<float> SpecialScoreTimers
        {
            get { return specialScoreTimers; }
            set { specialScoreTimers = value; }
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

        /*
        ***** To use this, each button is associated with a rectangle. *****
        Call this method when the button is clicked, and pass in the flock of boids and the index
        that the rectangle is at in the list in order to use it. So, button 1; Pen 1; send in 1 as the parameter.
        Use the first int that this returns to add to the score,
        and if the second is 1, add to the reached scoregoals, else, don't
        */
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
            int bashBonus = 0;
            int ifScoreGoal = -1;
            bool upScoreGoal = false;
            bool bashedSpecial = false;
            Vector2 returnNums;

            for (int x = flock.Boids.Count -1; x >= 0; x--)
            {
                if (flock.Boids[x].Pen == pen)
                {
                    // Score Increment
                    boidsBashed++;
                    if (bashBonus < 7)
                    {
                        bashBonus++;
                    }
                    if (bashBonus == 7)
                    {
                        scoreIncrease += (int)Math.Pow(scoregoal, bashBonus) * scoregoal;
                    }
                    else
                    {
                        scoreIncrease += (int)Math.Pow(scoregoal, bashBonus);
                    }
                    
                    if (flock.Boids[x].IsSpecial)
                    {
                        bashedSpecial = true;
                    }

                    // Checking if new score goal has been reached
                    if (scoregoal <= bashBonus)
                    {
                        upScoreGoal = true;                      
                    }
                     
                    // TODO - apply visuals
                    if (bashBonus < 7)
                    {
                        scorePrints.Add(new Vector3(flock.Boids[x].Position.X, flock.Boids[x].Position.Y,
                        (float)Math.Pow(scoregoal, bashBonus)));
                        ScoreTimers.Add(1);
                    }
                    else
                    {
                        scorePrints.Add(new Vector3(flock.Boids[x].Position.X, flock.Boids[x].Position.Y,
                        (float)Math.Pow(scoregoal, bashBonus) * scoregoal));
                        ScoreTimers.Add(1);
                    }

                    if (bashedSpecial)
                    {
                        SpecialScorePrints.Add(new Vector3(flock.Boids[x].Position.X + 3, flock.Boids[x].Position.Y - 3, 2));
                        SpecialScoreTimers.Add(2);
                    }
                    
                    flock.RepositionBoid(flock.Boids[x]);
                }
            }

            if (bashedSpecial && upScoreGoal)
            {
                ifScoreGoal = 3;
            }
            else if (bashedSpecial)
            {
                ifScoreGoal = 2;
            }
            else if (upScoreGoal)
            {
                ifScoreGoal = 1;
            }

            if (boidsBashed > scoregoal && scoregoal >= 7)
            {
                flock.AddBoids((boidsBashed - scoregoal) * 2);
            }

            // Assemble Vector
            returnNums = new Vector2(scoreIncrease, ifScoreGoal);

            return returnNums;
        }
    }
}
