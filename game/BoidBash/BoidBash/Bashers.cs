using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
        private int totalBoidsBashed;
        private int totalSpecialBoidsBashed;
        private SoundEffect smallBash;
        private SoundEffect mediumBash;
        private SoundEffect largeBash;
        private SoundEffect timeIncrease;
        private SoundEffect addBoids;

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

        // Total boids bashed
        public int TotalBoidsBashed
        {
            get { return totalBoidsBashed; }
            set { totalBoidsBashed = value; }
        }

        // Total special boids bashed
        public int TotalSpecialBoidsBashed
        {
            get { return totalSpecialBoidsBashed; }
            set { totalSpecialBoidsBashed = value; }
        }

        // Constructor
        public Bashers(SoundEffect smallBash, SoundEffect mediumBash, SoundEffect largeBash, SoundEffect timeIncrease, SoundEffect addBoids)
        {
            this.smallBash = smallBash;
            this.mediumBash = mediumBash;
            this.largeBash = largeBash;
            this.timeIncrease = timeIncrease;
            this.addBoids = addBoids;
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

            for (int x = flock.Boids.Count - 1; x >= 0; x--)
            {
                if (flock.Boids[x].Pen == pen)
                {
                    // Score Increment
                    // Up the number of boids bashed for every bashed boid
                    boidsBashed++;

                    // Update the bash bonus with a maximum of 7
                    if (bashBonus < 7 && bashBonus < scoregoal)
                    {
                        bashBonus++;
                    }

                    // Calculate the Score increase
                    // Maximum of power of 7 to disallow incalculably high scores
                    if (bashBonus >= 7)
                    {
                        scoreIncrease += (int)Math.Pow(scoregoal, bashBonus);
                    }
                    else
                    {
                        scoreIncrease += (int)Math.Pow(boidsBashed, bashBonus);
                    }
                    
                    // Detect if a special boid was destroyed
                    if (flock.Boids[x].IsSpecial)
                    {
                        bashedSpecial = true;
                    }

                    // Checking if new score goal has been reached
                    if (scoregoal <= boidsBashed)
                    {
                        upScoreGoal = true;                      
                    }
                     
                    // Adds a score print for every boid at the position it was destroyed at
                    if (bashBonus < 7)
                    {
                        scorePrints.Add(new Vector3(flock.Boids[x].Position.X, flock.Boids[x].Position.Y,
                        (int)Math.Pow(boidsBashed, bashBonus)));
                        ScoreTimers.Add(1);
                    }
                    else
                    {
                        scorePrints.Add(new Vector3(flock.Boids[x].Position.X, flock.Boids[x].Position.Y,
                        (int)Math.Pow(scoregoal, bashBonus)));
                        ScoreTimers.Add(1);
                    }

                    // Add Score print for golden boid's timer increase
                    if (bashedSpecial)
                    {
                        SpecialScorePrints.Add(new Vector3(flock.Boids[x].Position.X + 3, flock.Boids[x].Position.Y - 3, 2));
                        SpecialScoreTimers.Add(2);
                    }
                    
                    // Repositions the boid within the creation bounds
                    flock.RepositionBoid(flock.Boids[x]);
                }
            }

            // Deal with special and scoregoal increases
            if (bashedSpecial && upScoreGoal)
            {
                // Play special boid sound
                timeIncrease.Play();
                ifScoreGoal = 3;
                totalSpecialBoidsBashed++;
            }
            else if (bashedSpecial)
            {
                timeIncrease.Play();
                ifScoreGoal = 2;
                totalSpecialBoidsBashed++;
            }
            else if (upScoreGoal)
            {
                ifScoreGoal = 1;
            }

            // Adds boids if the scoregoal has been surpassed beyond 7
            if (boidsBashed > scoregoal && scoregoal >= 7)
            {
                addBoids.Play();
                flock.AddBoids((boidsBashed - scoregoal) * 2);
            }

            // Assemble Vector
            returnNums = new Vector2(scoreIncrease, ifScoreGoal);

            // Play sound for number of boids bashed
            if (boidsBashed > 7)
            {
                largeBash.Play();
            }
            else if (boidsBashed > 3)
            {
                mediumBash.Play();
            }
            else if (boidsBashed > 0)
            {
                smallBash.Play();
            }

            // increment total boids bashed
            totalBoidsBashed += boidsBashed;

            // Return vector
            return returnNums;
        }
    }
}
