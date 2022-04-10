using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;

namespace BoidBash
{
    // This enumerator references the different states of the game
    // TODO: Add Options
    // TODO: Add Instructions
    // TODO: Add Credits
    enum GameState
    {
        MainMenu,
        Game,
        PauseMenu,
        EndScreen,
        Options,
        Instructions,
        Credits
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GameState currentState;
        private MouseState mouseState;

        // Border timer sync
        int rInterval;
        int gInterval;
        int bInterval;
        Color colorDrawn;

        // Buttons
        private List<Button> buttons = new List<Button>();
        private Color bgColor = Color.White;
        private Random rng = new Random();
        private Texture2D bashButton;

        // Textures
        private Texture2D playPrompt;
        private Texture2D continuePrompt;
        private Texture2D resumePrompt;
        private Texture2D pausePrompt;
        private Texture2D returnPrompt;
        private Texture2D boidBashLogo;
        private Texture2D gameOver;
        private Texture2D pausedDisplay;
        private Texture2D customCursor;

        // Sounds
        SoundEffect bash;
        SoundEffect clicked;
        SoundEffect stateChange;
        SoundEffect gameOverSound;
        //SoundEffect timeIncrease;
        //SoundEffect scored;
        Song menuMusic;
        Song gameMusic;

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
        private Color boidColor = new Color(0, 200, 255);
        private Color penColor = new Color(70, 70, 70);
        private Color playAreaColor = new Color(5, 5, 5);

        // State UI
        private MainMenuUI mainMenuUI;
        private GameUI gameUI;
        private PauseMenuUI pauseMenuUI;
        private EndScreenUI endScreenUI;

        // Boids
        private Texture2D boidSprite;
        private Flock flock;
        private Flock menuFlock;

        // Predator
        private Predator predator;
        private Texture2D predTexture;
        private int width;
        private int height;

        // Player Score
        private ulong player1Score = 0;
        private int scoreGoal = 1;

        // Timer
        private float timer = 30f;

        // Debug
        private Texture2D blank;
        private Texture2D gradient;
        private Texture2D glowBorder;
        private List<Rectangle> bounds = new List<Rectangle>();
        private List<Rectangle> menuBounds = new List<Rectangle>();
        private bool inDebug = false;

        public int Width
        {
            get { return width; }
        }
        public int Height
        {
            get { return height; }
        }

        public ulong Score
        {
            get { return player1Score; }
        }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
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
            mouseState = new MouseState();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            primaryFont = Content.Load<SpriteFont>("PrimaryFont");
            headerFont = Content.Load<SpriteFont>("HeaderFont");

            clicked = Content.Load<SoundEffect>("clicked");
            stateChange = Content.Load<SoundEffect>("stateChange");
            bash = Content.Load<SoundEffect>("bash");
            gameOverSound = Content.Load<SoundEffect>("gameOverSound");
            gameMusic = Content.Load<Song>("gameMusic");
            menuMusic = Content.Load<Song>("mainMenuMusic");

            MediaPlayer.Play(menuMusic);
            MediaPlayer.IsRepeating = true;

            bashButton = Content.Load<Texture2D>("BashButton2");
            playPrompt = Content.Load<Texture2D>("StartPrompt");
            boidBashLogo = Content.Load<Texture2D>("BoidBashLogo");
            continuePrompt = Content.Load<Texture2D>("ContinuePrompt");
            gameOver = Content.Load<Texture2D>("GameOver");
            resumePrompt = Content.Load<Texture2D>("ResumePrompt");
            pausePrompt = Content.Load<Texture2D>("PausePrompt");
            returnPrompt = Content.Load<Texture2D>("ReturnMainMenu");
            pausedDisplay = Content.Load<Texture2D>("Paused");
            customCursor = Content.Load<Texture2D>("CustomCursor");

            boidSprite = Content.Load<Texture2D>("BoidSp4");
            blank = Content.Load<Texture2D>("WhiteSquare");
            gradient = Content.Load<Texture2D>("SquareArt");
            glowBorder = Content.Load<Texture2D>("SquareGlow");
            flock = new Flock(70, new Rectangle(300, 300, 600, 300),
            boidSprite, new Vector2(10, 12), boidColor, _spriteBatch, bash);
            menuFlock = new Flock(100, new Rectangle(300, 300, 600, 300),
            boidSprite, new Vector2(10, 12), boidColor, _spriteBatch, bash);

            foreach (Boid boid in menuFlock.Boids)
            {
                if (boid.IsSpecial)
                {
                    boid.Color = Color.Gold;
                    boid.UseDefaultColor = false;
                }
                else
                {
                    menuFlock.GiveColor(boid);
                    boid.UseDefaultColor = true;
                }
            }

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

            // Menu flock's boundaries
            menuBounds.Add(new Rectangle(0, -100, 1200, 100));
            menuBounds.Add(new Rectangle(-100, 0, 100, 900));
            menuBounds.Add(new Rectangle(0, 900, 1200, 100));
            menuBounds.Add(new Rectangle(1200, 0, 100, 900));
            menuFlock.Boundaries = menuBounds;

            flock.Pens.AddPen(new Rectangle(200, 100, 150, 100));
            flock.Pens.AddPen(new Rectangle(1000, 200, 100, 150));
            flock.Pens.AddPen(new Rectangle(850, 700, 150, 100));
            flock.Pens.AddPen(new Rectangle(100, 550, 100, 150));

            predTexture = Content.Load<Texture2D>("PredSp");

            predator = new Predator(predTexture, new Rectangle(width / 2, height / 2,
                35, 35),
                windowHeight, windowWidth, 35, 35);

            mainMenuUI = new MainMenuUI(windowWidth, windowHeight, headerFont, primaryFont, playPrompt, boidBashLogo);
            gameUI = new GameUI(windowWidth, windowHeight, headerFont, boidBashLogo, pausePrompt);
            pauseMenuUI = new PauseMenuUI(windowWidth, windowHeight, resumePrompt, returnPrompt, pausedDisplay);
            endScreenUI = new EndScreenUI(windowWidth, windowHeight, continuePrompt, gameOver, headerFont);

            headerFont = Content.Load<SpriteFont>("headerFont");

            // TEMPORARY TESTING
            // Enable to test File IO

            //player1Score = 2;
            /*
            UpdateScores(325306027);
            UpdateScores(260714129);
            UpdateScores(229811503);
            UpdateScores(179622342);
            UpdateScores(173690822);
            UpdateScores(165747780);
            UpdateScores(161942856);
            UpdateScores(145060864);
            UpdateScores(142758264);
            UpdateScores(139509943);
            */
            System.Diagnostics.Debug.WriteLine(GetScoreList());


            // Add buttons
            buttons.Add(new Button(
                    _graphics.GraphicsDevice,           // device to create a custom texture
                    new Rectangle(110, 110, 80, 80),    // where to put the button
                    Color.DarkRed,                      // button color
                    0,                                  // pen number
                    bashButton,                         // texture
                    clicked));                        
            buttons[0].OnButtonClick += this.Bashed;

            buttons.Add(new Button(
                    _graphics.GraphicsDevice,           // device to create a custom texture
                    new Rectangle(1010, 110, 80, 80),   // where to put the button
                    Color.DarkRed,                      // button color
                    1,                                  // pen number
                    bashButton,                         // texture
                    clicked));
            buttons[1].OnButtonClick += this.Bashed;

            buttons.Add(new Button(
                    _graphics.GraphicsDevice,           // device to create a custom texture
                    new Rectangle(1010, 710, 80, 80),   // where to put the button
                    Color.DarkRed,                      // button color
                    2,                                  // pen number
                    bashButton,                         // texture
                    clicked));
            buttons[2].OnButtonClick += this.Bashed;

            buttons.Add(new Button(
                    _graphics.GraphicsDevice,           // device to create a custom texture
                    new Rectangle(110, 710, 80, 80),    // where to put the button
                    Color.DarkRed,                      // button color
                    3,                                  // pen number
                    bashButton,                         // texture
                    clicked));
            buttons[3].OnButtonClick += this.Bashed;
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
                    menuFlock.ProcessBoids(new Vector2(-300, -300));
                    break;
                case GameState.Game:
                    ProcessGame();
                    flock.ProcessBoids(predator.ActualPosition);
                    predator.Update(gameTime);

                    if (timer > 0)
                    {
                        timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }

                    gameUI.ScoreUpdater(player1Score);
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

            mouseState = Mouse.GetState();

            // GameState switches
            switch (currentState)
            {
                case GameState.MainMenu:
                    menuFlock.Draw();
                    mainMenuUI.Draw(_spriteBatch);
                    _spriteBatch.DrawString(primaryFont, GetScoreList(), new Vector2(500, windowHeight - 280), Color.White);
                    break;
                case GameState.Game:

                    //Draws the main box area for the game
                    _spriteBatch.Draw(blank, new Rectangle(200, 200, 800, 500), playAreaColor);

                    /* Draws the Crushers from top to bottom
                     * Top Left
                     * Top Right
                     * Bottom Right
                     * Bottom Left
                     */
                    _spriteBatch.Draw(gradient, new Rectangle(200, 100, 150, 100), penColor);
                    _spriteBatch.Draw(gradient, new Rectangle(1000, 200, 100, 150), penColor);
                    _spriteBatch.Draw(gradient, new Rectangle(850, 700, 150, 100), penColor);
                    _spriteBatch.Draw(gradient, new Rectangle(100, 550, 100, 150), penColor);

                    // Epic color-changing border system
                    rInterval = (Color.Red.R - Color.Lime.R) / 30;
                    gInterval = (Color.Red.G - Color.Lime.G) / 30;
                    bInterval = (Color.Red.B - Color.Lime.B) / 30;
                    colorDrawn = new Color(
                        (Color.Red.R + rInterval * (int)timer * -1),
                        (Color.Red.G + gInterval * (int)timer * -1),
                        (Color.Red.B + bInterval * (int)timer * -1)
                        );

                    _spriteBatch.Draw(blank, new Rectangle(200, 95, 150, 5), colorDrawn);
                    _spriteBatch.Draw(blank, new Rectangle(195, 95, 5, 455), colorDrawn);
                    _spriteBatch.Draw(blank, new Rectangle(350, 95, 5, 105), colorDrawn);
                    _spriteBatch.Draw(blank, new Rectangle(350, 195, 750, 5), colorDrawn);
                    _spriteBatch.Draw(blank, new Rectangle(1100, 195, 5, 160), colorDrawn);
                    _spriteBatch.Draw(blank, new Rectangle(1000, 350, 100, 5), colorDrawn);
                    _spriteBatch.Draw(blank, new Rectangle(1000, 350, 5, 450), colorDrawn);
                    _spriteBatch.Draw(blank, new Rectangle(850, 800, 155, 5), colorDrawn);
                    _spriteBatch.Draw(blank, new Rectangle(845, 700, 5, 105), colorDrawn);
                    _spriteBatch.Draw(blank, new Rectangle(95, 700, 750, 5), colorDrawn);
                    _spriteBatch.Draw(blank, new Rectangle(95, 550, 5, 150 ), colorDrawn);
                    _spriteBatch.Draw(blank, new Rectangle(95, 550, 105, 5), colorDrawn);

                    // Draws items only meant to be seen in debug
                    if (inDebug)
                    {
                        foreach (Rectangle bound in flock.Boundaries)
                        {
                            _spriteBatch.Draw(blank, bound, Color.Green);
                        }
                        _spriteBatch.Draw(blank, new Rectangle(200, 200, 800, 500), Color.Black);
                        foreach (Rectangle pen in flock.Pens.Pens)
                        {
                            _spriteBatch.Draw(blank, pen, Color.Red);
                        }
                        _spriteBatch.Draw(blank, new Rectangle(300, 300, 600, 300), Color.Blue);
                    }
                    // Ususal items to be drawn
                    gameUI.DrawPausePrompt(_spriteBatch);
                    gameUI.DrawLogo(_spriteBatch);
                    gameUI.DrawScore(_spriteBatch);
                    _spriteBatch.DrawString(headerFont, "Time: " + timer.ToString("0"), new Vector2(1060, 15),
                    Color.White);
                    gameUI.DrawScoreGoal(_spriteBatch, scoreGoal);
                    flock.Draw();
                    predator.Draw(_spriteBatch);
                    // Draws and removes any new point numbers that show up after destroying boids
                    foreach (Vector3 info in flock.Pens.ScorePrints)
                    {
                        _spriteBatch.DrawString(primaryFont, "+" + String.Format("{0:n0}", info.Z), new Vector2(info.X, info.Y), Color.Yellow);
                    }
                    // Change the amount of time left on the timers
                    for (int x = flock.Pens.ScoreTimers.Count - 1; x >= 0; x--)
                    {
                        flock.Pens.ScoreTimers[x] -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (flock.Pens.ScoreTimers[x] <= 0)
                        {
                            flock.Pens.ScorePrints.RemoveAt(x);
                            flock.Pens.ScoreTimers.RemoveAt(x);
                        }
                    }

                    // Draws and removes any new point numbers that show up after destroying special boids
                    foreach (Vector3 info in flock.Pens.SpecialScorePrints)
                    {
                        _spriteBatch.DrawString(primaryFont, "+" + info.Z.ToString(), new Vector2(info.X, info.Y), Color.Magenta);
                    }
                    // Change the amount of time left on the special timers
                    for (int x = flock.Pens.SpecialScoreTimers.Count - 1; x >= 0; x--)
                    {
                        flock.Pens.SpecialScoreTimers[x] -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (flock.Pens.SpecialScoreTimers[x] <= 0)
                        {
                            flock.Pens.SpecialScorePrints.RemoveAt(x);
                            flock.Pens.SpecialScoreTimers.RemoveAt(x);
                        }
                    }

                    foreach (Button b in buttons)
                    {
                        b.Draw(_spriteBatch);
                    }

                    // TODO - Make these messages appear for more than one frame
                    break;
                case GameState.PauseMenu:
                    //Draws the main box area for the game
                    _spriteBatch.Draw(blank, new Rectangle(200, 200, 800, 500), playAreaColor);

                    /* Draws the Crushers from top to bottom -
                     * Top Left
                     * Top Right
                     * Bottom Right
                     * Bottom Left
                     */
                    _spriteBatch.Draw(gradient, new Rectangle(200, 100, 150, 100), penColor);
                    _spriteBatch.Draw(gradient, new Rectangle(1000, 200, 100, 150), penColor);
                    _spriteBatch.Draw(gradient, new Rectangle(850, 700, 150, 100), penColor);
                    _spriteBatch.Draw(gradient, new Rectangle(100, 550, 100, 150), penColor);

                    _spriteBatch.Draw(blank, new Rectangle(200, 95, 150, 5), colorDrawn);
                    _spriteBatch.Draw(blank, new Rectangle(195, 95, 5, 455), colorDrawn);
                    _spriteBatch.Draw(blank, new Rectangle(350, 95, 5, 105), colorDrawn);
                    _spriteBatch.Draw(blank, new Rectangle(350, 195, 750, 5), colorDrawn);
                    _spriteBatch.Draw(blank, new Rectangle(1100, 195, 5, 160), colorDrawn);
                    _spriteBatch.Draw(blank, new Rectangle(1000, 350, 100, 5), colorDrawn);
                    _spriteBatch.Draw(blank, new Rectangle(1000, 350, 5, 450), colorDrawn);
                    _spriteBatch.Draw(blank, new Rectangle(850, 800, 155, 5), colorDrawn);
                    _spriteBatch.Draw(blank, new Rectangle(845, 700, 5, 105), colorDrawn);
                    _spriteBatch.Draw(blank, new Rectangle(95, 700, 750, 5), colorDrawn);
                    _spriteBatch.Draw(blank, new Rectangle(95, 550, 5, 150), colorDrawn);
                    _spriteBatch.Draw(blank, new Rectangle(95, 550, 105, 5), colorDrawn);

                    _spriteBatch.DrawString(headerFont, "Time: " + timer.ToString("0"), new Vector2(1060, 15),
                    Color.White);
                    gameUI.DrawScore(_spriteBatch);
                    pauseMenuUI.Draw(_spriteBatch);
                    gameUI.DrawLogo(_spriteBatch);
                    flock.Draw();
                    predator.Draw(_spriteBatch);
                    break;
                case GameState.EndScreen:
                    endScreenUI.Draw(_spriteBatch);
                    break;
                default:
                    break;
            }

            // Draw the cursor on top
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                _spriteBatch.Draw(
                    customCursor,
                    new Rectangle(mouseState.X, mouseState.Y, 16, 16),
                    Color.Red
                    );
            }
            else if (mouseState.RightButton == ButtonState.Pressed)
            {
                _spriteBatch.Draw(
                    customCursor,
                    new Rectangle(mouseState.X, mouseState.Y, 16, 16),
                    Disco()
                    );

                foreach (Boid boid in menuFlock.Boids)
                {
                    boid.UseDefaultColor = false;
                }
            }
            else
            {
                _spriteBatch.Draw(
                    customCursor,
                    new Rectangle(mouseState.X, mouseState.Y, 16, 16),
                    Color.White
                    );

                foreach (Boid boid in menuFlock.Boids)
                {
                    if (boid.IsSpecial)
                    {
                        boid.UseDefaultColor = false;
                    }
                    else
                    {
                        boid.UseDefaultColor = true;
                    }
                }
            }


            // End the Sprite Batch and the ShapeBatch
            _spriteBatch.End();
            ShapeBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Fun little method for those who find it :)
        /// ~ Will <3
        /// </summary>
        /// <returns></returns>
        private Color Disco()
        {
            Color color = new Color(
                rng.Next(0, 256),
                rng.Next(0, 256),
                rng.Next(0, 256)
                );

            return color;
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
            if (IsSingleKeyPress(Keys.Space))
            {
                MediaPlayer.Play(gameMusic);
                MediaPlayer.IsRepeating = true;
                stateChange.Play();
                timer = 30;
                flock.Pens.ScoreTimers.Clear();
                flock.Pens.ScorePrints.Clear();
                currentState = GameState.Game;
                flock.ClearFlock();
                flock.AddBoids(50);
            }
        }

        /// <summary>
        /// This method processes the Game state
        /// </summary>
        private void ProcessGame()
        {
            foreach (Button button in buttons)
            {
                button.Update();
            }
            if (IsSingleKeyPress(Keys.Tab))
            {
                MediaPlayer.Pause();
                stateChange.Play();
                currentState = GameState.PauseMenu;
            }
            // For toggling debug mode
            else if (IsSingleKeyPress(Keys.Back))
            {
                stateChange.Play();
                if (inDebug)
                {
                    inDebug = false;
                }
                else
                {
                    inDebug = true;
                }
            }
            if (timer < 0.01f)
            {
                UpdateScores(player1Score);
                endScreenUI.Score = player1Score;
                currentState = GameState.EndScreen;
                gameOverSound.Play();
                MediaPlayer.Stop();
            }
        }

        /// <summary>
        /// This method processes the Pause Menu state
        /// </summary>
        private void ProcessPauseMenu()
        {
            if (IsSingleKeyPress(Keys.Space))
            {
                MediaPlayer.Resume();
                stateChange.Play();
                currentState = GameState.Game;
            }
            else if (IsSingleKeyPress(Keys.M))
            {
                MediaPlayer.Play(menuMusic);
                MediaPlayer.IsRepeating = true;
                stateChange.Play();
                currentState = GameState.MainMenu;
                predator.Position = new Rectangle(width / 2, height / 2, 25, 25);
                player1Score = 0;
                scoreGoal = 1;
            }
        }

        /// <summary>
        /// This method processes the End Screen state
        /// </summary>
        private void ProcessEndScreen()
        {
            if (IsSingleKeyPress(Keys.Space))
            {
                MediaPlayer.Play(menuMusic);
                MediaPlayer.IsRepeating = true;
                stateChange.Play();
                currentState = GameState.MainMenu;
                player1Score = 0;
                scoreGoal = 1;
            }
        }

        // TODO - Add playernames to text file
        /// <summary>
        /// This method updates the high scores text file
        /// Returns true if the score was added to the list
        /// </summary>
        private bool UpdateScores(ulong score)
        {
            List<ulong> scores = new List<ulong>();
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
                    scores.Add(ulong.Parse(line));
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
                    foreach (ulong num in scores)
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

            return willAdd;
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
                    if (x < 9)
                    {
                        scores += "  ";
                    }
                    // Add which place they are in, then the score, then a new line
                    scores += ((x + 1) + ". " + String.Format("{0:n0}", long.Parse(line)) + "\n");
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

        /// <summary>
        /// Called When a button is clicked in order to bash the boids in the pen
        /// </summary>
        /// <param name="pen"></param>
        public void Bashed(int pen)
        {
            Vector2 dataReturn;
            dataReturn = flock.Pens.DestroyContainedBoids(flock, pen, scoreGoal);

            player1Score += (ulong)dataReturn.X;

            if (dataReturn.Y == 1)
            {
                scoreGoal++;
            }
            else if (dataReturn.Y == 2)
            {
                timer += 2;
            }
            else if (dataReturn.Y == 3)
            {
                timer += 2;
                scoreGoal++;
            }
        }
    }
}
