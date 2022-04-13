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
        private int upScoreGoal = -1;
        private SoundEffect smallBash;
        private SoundEffect mediumBash;
        private SoundEffect largeBash;
        private SoundEffect timeIncrease;
        private SoundEffect addBoids;
        
        // Properties
        /// <summary>
        /// Returns the list of Pens
        /// </summary>
        public List<Rectangle> Pens
        {
            get { return pens;  }
        }

        /// <summary>
        /// Sets and returns the list of score prints
        /// </summary>
        public List<Vector3> ScorePrints
        {
            get { return scorePrints; }
            set { scorePrints = value; }
        }

        /// <summary>
        /// Sets and returns the list of score timers
        /// </summary>
        public List<float> ScoreTimers
        {
            get { return scoreTimers; }
            set { scoreTimers = value; }
        }

        /// <summary>
        /// Sets and returns the list of special score prints
        /// </summary>
        public List<Vector3> SpecialScorePrints
        {
            get { return specialScorePrints; }
            set { specialScorePrints = value; }
        }

        /// <summary>
        /// Sets and returns the list of special score timers
        /// </summary>
        public List<float> SpecialScoreTimers
        {
            get { return specialScoreTimers; }
            set { specialScoreTimers = value; }
        }

        /// <summary>
        /// Sets and returns the total boids bashed
        /// </summary>
        public int TotalBoidsBashed
        {
            get { return totalBoidsBashed; }
            set { totalBoidsBashed = value; }
        }

        /// <summary>
        /// Sets and returns the total special boids bashed
        /// </summary>
        public int TotalSpecialBoidsBashed
        {
            get { return totalSpecialBoidsBashed; }
            set { totalSpecialBoidsBashed = value; }
        }

        public int UpScoreGoal
        {
            get { return upScoreGoal; }
            set { upScoreGoal = value; }
        }


        // Sound Effect Properties
        /// <summary>
        /// Sets the smallbash sfx
        /// </summary>
        public SoundEffect SmallBash
        {
            set { smallBash = value; }
        }

        /// <summary>
        /// Sets the smallbash sfx
        /// </summary>
        public SoundEffect MediumBash
        {
            set { mediumBash = value; }
        }

        /// <summary>
        /// Sets the smallbash sfx
        /// </summary>
        public SoundEffect LargeBash
        {
            set { largeBash = value; }
        }

        /// <summary>
        /// Sets the smallbash sfx
        /// </summary>
        public SoundEffect TimeIncrease
        {
            set { timeIncrease = value; }
        }

        /// <summary>
        /// Sets the smallbash sfx
        /// </summary>
        public SoundEffect AddBoids
        {
            set { addBoids = value; }
        }

        /// <summary>
        /// Constructs a new Basher
        /// </summary>
        /// <param name="smallBash"></param>
        /// <param name="mediumBash"></param>
        /// <param name="largeBash"></param>
        /// <param name="timeIncrease"></param>
        /// <param name="addBoids"></param>
        public Bashers()
        {
            
        }

        // Methods

        /// <summary>
        /// Destroys all boids within the specified pen.
        /// Takes the flock, the index of the rectangle the boid is in, and the current reached score goal as parameters
        /// </summary>
        /// <param name="flock"></param>
        /// <param name="pen"></param>
        public long BashContainedBoids(Flock flock, int pen, int scoregoal)
        {
            // Create necessary values
            // The value the score will be increased by
            long scoreIncrease = 0;
            int toAdd = 0;
            // number of boids that have been bashed
            int boidsBashed = 0;
            // The bonus based on boids bashed, capping at 7
            int bashBonus = 0;
            // Value passed through Vector2 to determine if the score goal is increased
            int ifScoreGoal = -1;
            // Bools to perform actions outside the loop
            bool upScoreGoal = false;
            bool bashedSpecial = false;
            // Vector to be returned
            Vector2 returnNums;

            // Loop through all boids that are in the pen
            for (int x = flock.Boids.Count - 1; x >= 0; x--)
            {
                // If in the pen that is bashing
                if (flock.Boids[x].Pen == pen)
                {
                    // Reset toAdd
                    toAdd = 0;
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
                        toAdd += (int)Math.Pow(scoregoal, 7);
                    }
                    else
                    {
                        toAdd += (int)Math.Pow(boidsBashed, bashBonus);
                    }
                    
                    // Cap toadd
                    if (toAdd > 158120256)
                    {
                        toAdd = 158120256;
                    }

                    scoreIncrease += toAdd;

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

                    scorePrints.Add(new Vector3(flock.Boids[x].Position.X, flock.Boids[x].Position.Y,
                    toAdd));
                    ScoreTimers.Add(1);


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

            // Set score goal property
            this.upScoreGoal = ifScoreGoal;

            // Return vector
            return scoreIncrease;
        }
    }
}
