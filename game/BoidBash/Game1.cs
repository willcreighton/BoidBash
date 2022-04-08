using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;

namespace BoidBash
{
    // This enumerator references the different states of the game
    enum GameState
    {
        // The key's are for testing, specific events or buttons will trigger
        // the states in the final version.
        MainMenu,       // 'M'
        Game,           // 'G'
        PauseMenu,      // 'P'
        EndScreen       // 'E'
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GameState currentState;

        // Screen size
        private int windowWidth;
        private int windowHeight;

        // Keyboard states
        private KeyboardState keyboardState;
        private KeyboardState lastKeyboardState;
        
        // Fonts
        private SpriteFont primaryFont;
        private SpriteFont headerFont;

        // Colors
        private Color backgroundColor = new Color(20, 20, 20);
        private Color boidColor = new Color(104, 226, 255);

        // State UI
        private MainMenuUI mainMenuUI;
        private GameUI gameUI;
        private PauseMenuUI pauseMenuUI;
        private EndScreenUI endScreenUI;

        // Boids
        private Texture2D boidSprite;
        private Flock flock;

        // Predator
        private Predator predator;
        private Texture2D predTexture;
        private int width;
        private int height;

        // Player Score
        private long player1Score = 0;

        //Timer
        private float timer = 60f;

        // Debug
        private Texture2D blank;
        private List<Rectangle> bounds = new List<Rectangle>();
        private bool inDebug = false;

        public int Width
        {
            get { return width; }
        }
        public int Height
        {
            get { return height; }
        }

        public long Score
        {
            get { return player1Score; }
        }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Adjust the window size
            windowWidth = 1200;
            windowHeight = 900;
            _graphics.PreferredBackBufferWidth = windowWidth;
            _graphics.PreferredBackBufferHeight = windowHeight;
            _graphics.ApplyChanges();

            width = _graphics.GraphicsDevice.Viewport.Width;
            height = _graphics.GraphicsDevice.Viewport.Height;

            currentState = GameState.MainMenu;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            primaryFont = Content.Load<SpriteFont>("PrimaryFont");
            headerFont = Content.Load<SpriteFont>("HeaderFont");

            boidSprite = this.Content.Load<Texture2D>("BoidSprite");
            blank = this.Content.Load<Texture2D>("White Square");
            flock = new Flock(30, new Rectangle(200, 200, 800, 500), new Rectangle(300, 300, 400, 300),
                boidSprite, new Vector2(5, 7), boidColor,_spriteBatch);

            // Add boundaries for Game flock
            bounds.Add(new Rectangle(350, 100, 750, 100));
            bounds.Add(new Rectangle(100, 100, 100, 450));
            bounds.Add(new Rectangle(100, 700, 750, 100));
            bounds.Add(new Rectangle(1000, 350, 100, 450));
            bounds.Add(new Rectangle(100, 0, 350, 100));
            bounds.Add(new Rectangle(0, 450, 100, 350));
            bounds.Add(new Rectangle(1100, 100, 100, 350));
            bounds.Add(new Rectangle(750, 800, 350, 100));
            flock.Boundaries = bounds;

            flock.Pens.AddPen(new Rectangle(200, 100, 150, 100));
            flock.Pens.AddPen(new Rectangle(1000, 200, 100, 150));
            flock.Pens.AddPen(new Rectangle(850, 700, 150, 100));
            flock.Pens.AddPen(new Rectangle(100, 550, 100, 150));

            predTexture = Content.Load<Texture2D>("PredatorSprite");

            predator = new Predator(predTexture, new Rectangle(width / 2, height / 2,
                25, 25),
                windowHeight, windowWidth, 25, 25);

            mainMenuUI = new MainMenuUI(windowWidth, windowHeight, headerFont, primaryFont);
            gameUI = new GameUI(windowWidth, windowHeight, headerFont, primaryFont);
            pauseMenuUI = new PauseMenuUI(windowWidth, windowHeight, headerFont, primaryFont);
            endScreenUI = new EndScreenUI(windowWidth, windowHeight, headerFont, primaryFont);

            headerFont = Content.Load<SpriteFont>("headerFont");

            // TEMPORARY TESTING
            // Enable to test File IO
            
            /*
            player1Score = 2;
            UpdateScores(10);
            UpdateScores(0444214);
            UpdateScores(-5);
            UpdateScores(855);
            UpdateScores(7);
            UpdateScores(2112123123211111);
            UpdateScores(9);
            UpdateScores(131232131);
            UpdateScores(8);
            UpdateScores(7);


            System.Diagnostics.Debug.WriteLine(GetScoreList());
            */

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            keyboardState = Keyboard.GetState();

            // GameState switches
            switch (currentState)
            {
                case GameState.MainMenu:
                    ProcessMainMenu();
                    break;
                case GameState.Game:
                    ProcessGame();
                    flock.ProcessBoids(new Vector2(predator.PredatorPosition.X, predator.PredatorPosition.Y));
                    predator.Update(gameTime);

                    if (timer > 0)
                    {
                        timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    }
                    
                    // Testing score incrementing
                    /*
                    player1Score++;
                    gameUI.ScoreUpdater(player1Score);
                    */
                    break;
                case GameState.PauseMenu:
                    ProcessPauseMenu();
                    break;
                case GameState.EndScreen:
                    ProcessEndScreen();
                    break;
                default:
                    break;
            }

            lastKeyboardState = keyboardState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(backgroundColor);

            // Begin the Sprite Batch and the ShapeBatch
            _spriteBatch.Begin();
            ShapeBatch.Begin(GraphicsDevice);

            // GameState switches
            switch (currentState)
            {
                case GameState.MainMenu:
                    mainMenuUI.Draw(_spriteBatch);
                    break;
                case GameState.Game:

                    //Draws the main box area for the game
                    _spriteBatch.Draw(blank, new Rectangle(200, 200, 800, 500), Color.Blue);

                    /*Draws the Crushers from top to bottom -
                     * Top Left
                     * Top Right
                     * Bottom Right
                     * Bottom Left
                     */
                    _spriteBatch.Draw(blank, new Rectangle(200, 100, 150, 100), Color.Red);
                    _spriteBatch.Draw(blank, new Rectangle(1000, 200, 100, 150), Color.Red);
                    _spriteBatch.Draw(blank, new Rectangle(850, 700, 150, 100), Color.Red);
                    _spriteBatch.Draw(blank, new Rectangle(100, 550, 100, 150), Color.Red);

                    // Draws items only meant to be seen in debug
                    if (inDebug)
                    {
                        foreach (Rectangle bound in flock.Boundaries)
                        { 
                            _spriteBatch.Draw(blank, bound, Color.Green);
                        }
                        _spriteBatch.Draw(blank, new Rectangle(200, 200, 800, 500), Color.White);
                        foreach (Rectangle pen in flock.Pens.Pens)
                        {
                            _spriteBatch.Draw(blank, pen, Color.Red);
                        }
                    }
                    // Ususal items to be drawn
                    gameUI.Draw(_spriteBatch);
                    gameUI.DrawScore(_spriteBatch);
                    _spriteBatch.DrawString(headerFont, "Timer: " + timer.ToString("0"), new Vector2(500, 15),
                    Color.White);
                    gameUI.DrawScoreGoal(_spriteBatch);
                    flock.Draw();
                    predator.Draw(_spriteBatch);
                    // Draws and removes any new point numbers that show up after destroying boids
                    foreach (Vector3 info in flock.Pens.ScorePrints)
                    {
                        _spriteBatch.DrawString(primaryFont, "+" + info.Z.ToString(), new Vector2(info.X, info.Y), Color.Yellow);
                    }
                    flock.Pens.ScorePrints.Clear();

                    // TODO - Make these messages appear for more than one frame
                    break;
                case GameState.PauseMenu:
                    gameUI.DrawScore(_spriteBatch);
                    pauseMenuUI.Draw(_spriteBatch);
                    flock.Draw();
                    predator.Draw(_spriteBatch);
                    break;
                case GameState.EndScreen:
                    endScreenUI.Draw(_spriteBatch);
                    break;
                default:
                    break;
            }

            // End the Sprite Batch and the ShapeBatch
            _spriteBatch.End();

            ShapeBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// This method checks for a single key press
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool IsSingleKeyPress(Keys key)
        {
            if (keyboardState.IsKeyDown(key) && !lastKeyboardState.IsKeyDown(key))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// This method processes the Main Menu state
        /// </summary>
        private void ProcessMainMenu()
        {
            if (IsSingleKeyPress(Keys.G))
            {
                currentState = GameState.Game;
                flock.AddBoids(50);
            }
        }

        /// <summary>
        /// This method processes the Game state
        /// </summary>
        private void ProcessGame()
        {
            if (IsSingleKeyPress(Keys.P))
            {
                currentState = GameState.PauseMenu;
            }
            // For testing the End Screen
            else if (IsSingleKeyPress(Keys.E))
            {
                currentState = GameState.EndScreen;
            }
            // For toggling debug mode
            if (IsSingleKeyPress(Keys.Enter))
            {
                if (inDebug)
                {
                    inDebug = false;
                }
                else
                {
                    inDebug = true;
                }
            }
        }

        /// <summary>
        /// This method processes the Pause Menu state
        /// </summary>
        private void ProcessPauseMenu()
        {
            if (IsSingleKeyPress(Keys.G))
            {
                currentState = GameState.Game;
            }
            else if (IsSingleKeyPress(Keys.M))
            {
                currentState = GameState.MainMenu;
                flock.ClearFlock();
                predator.Position = new Rectangle(width / 2, height / 2, 25, 25);
            }
        }

        /// <summary>
        /// This method processes the End Screen state
        /// </summary>
        private void ProcessEndScreen()
        {
            if (IsSingleKeyPress(Keys.M))
            {
                currentState = GameState.MainMenu;
            }
        }

        // TODO - Add playernames to text file
        /// <summary>
        /// This method updates the high scores text file
        /// </summary>
        private void UpdateScores(long score)
        {
            List<long> scores = new List<long>();
            string line = null;
            StreamReader input = null;
            StreamWriter output = null;
            bool willAdd = false;
            bool added = false;

            // Read through text file, add them to the list of scores
            try
            {
                input = new StreamReader("..//..//..//Highscores.txt");

                while ((line = input.ReadLine()) != null)
                {
                    scores.Add(long.Parse(line));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            if (input != null)
            {
                input.Close();
            }

            // If there are less than 10 numbers in the list, willAdd is automatically true
            if (scores.Count < 10)
            {
                willAdd = true;
            }
            // Compare with new score
            for (int x = scores.Count - 1; x >= 0; x--)
            {
                // Starting at the lowest number, compare them
                // If it is greater than the current score, set willAdd to true
                if (score > scores[x])
                {
                    willAdd = true;
                }
                // If it is less than or equal to the current item but willAdd is true,
                //  Insert the score in the previous position
                else if ((score < scores[x] || score == scores[x]) && willAdd)
                {
                    scores.Insert(x + 1, score);
                    added = true;
                    break;
                }
                // If it is less than the current item and it will not be added, stop the loop
                else
                {
                    break;
                }
            }

            // If it hasn't been added yet, add it now
            if (willAdd && !added)
            {
                scores.Insert(0, score);
            }

            // If the score was added, and there are more than 10 items in the list,
            //  remove the last item in the list
            if (willAdd && scores.Count > 10)
            {
                scores.RemoveAt(scores.Count - 1);
            }

            // If the score was added, write the new text file
            if (willAdd)
            {
                try
                {
                    output = new StreamWriter("..//..//..//Highscores.txt");

                    // Write a line for each score
                    foreach (long num in scores)
                    {
                        output.WriteLine(num);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                output.Close();
            }
        }
        
        /// <summary>
        /// Returns a string containing a formatted list of the top scores
        /// </summary>
        /// <returns></returns>
        private string GetScoreList()
        {
            string scores = "";
            string line = null;
            StreamReader input = null;

            try
            {
                // Create streamreader
                input = new StreamReader("..//..//..//Highscores.txt");

                // Loop through the 10 or less scores in the list
                for (int x = 0; x < 10 && ((line = input.ReadLine()) != null); x++)
                {
                    // Add which place they are in, then the score, then a new line
                    scores += ((x + 1) + ". " + line + "\n");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            // Close streamreader
            if (input != null)
            {
                input.Close();
            }

            // return string
            return scores;
        }

    }
}
