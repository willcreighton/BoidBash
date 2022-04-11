﻿using Microsoft.Xna.Framework;
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

        // Total addition print lists
        private List<int> totalScoreIncrementPrint = new List<int>();
        private List<float> totalScoreIncrementTimer = new List<float>();
        private List<int> totalTimeIncrementPrint = new List<int>();
        private List<float> totalTimeIncrementTimer = new List<float>();

        // Border color & timer sync
        private int rInterval;
        private int gInterval;
        private int bInterval;
        private Color colorDrawn;

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
        private Texture2D insertCoin;

        // Sounds
        private SoundEffect smallBash;
        private SoundEffect mediumBash;
        private SoundEffect largeBash;
        private SoundEffect clicked;
        private SoundEffect stateChange;
        private SoundEffect gameOverSound;
        private SoundEffect timeIncrease;
        private SoundEffect addBoids;

        // Songs
        private Song menuMusic;
        private Song gameMusic;
        //private Song discoMusic;

        // Screen size
        private int windowWidth;
        private int windowHeight;

        // Keyboard states
        private KeyboardState keyboardState;
        private KeyboardState lastKeyboardState;

        // Fonts
        private SpriteFont primaryFont;
        private SpriteFont headerFont;
        private SpriteFont senRegular;
        private SpriteFont senBold;
        private SpriteFont senExtraBold;

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
        private Texture2D displayBoid;

        // Predator
        private Predator predator;
        private Texture2D predTexture;
        private int width;
        private int height;

        // Player Score
        private ulong player1Score = 0;
        private int scoreGoal = 1;

        // Text Input
        private string name = "";
        private Keys key;
        private List<Keys> excludedKeys = new List<Keys>();

        // Timer
        private float timer = 30f;

        // Update Score Fields
        private List<ulong> scores = new List<ulong>();
        private List<string> names = new List<string>();
        private string line = null;
        private StreamReader input = null;
        private StreamWriter output = null;
        private bool willAdd = false;
        private bool added = false;

        // Rectangles to draw
        private Rectangle[] singlePlayerBashers = new Rectangle[4] 
            { new Rectangle(200, 100, 150, 100), new Rectangle(1000, 200, 100, 150),
             new Rectangle(850, 700, 150, 100), new Rectangle(100, 550, 100, 150)};

        private Rectangle[] singlePlayerBorders = new Rectangle[12]
            { new Rectangle(200, 95, 150, 5), new Rectangle(195, 95, 5, 455), new Rectangle(350, 95, 5, 105),
              new Rectangle(350, 195, 750, 5), new Rectangle(1100, 195, 5, 160), new Rectangle(1000, 350, 100, 5),
              new Rectangle(1000, 350, 5, 450), new Rectangle(850, 800, 155, 5), new Rectangle(845, 700, 5, 105),
              new Rectangle(95, 700, 750, 5), new Rectangle(95, 550, 5, 150), new Rectangle(95, 550, 105, 5)};

        private Rectangle singlePlayerPlayArea = new Rectangle(200, 200, 800, 500);

        private Rectangle[] displayBoids = new Rectangle[4]
            { new Rectangle(15, 290, 46, 60), new Rectangle(15, 390, 46, 60),
            new Rectangle(240, 400, 46, 60), new Rectangle(905, 400, 46, 60)};

        // Debug
        private Texture2D blank;
        private Texture2D gradient;
        private Texture2D glowBorder;
        private List<Rectangle> bounds = new List<Rectangle>();
        private List<Rectangle> menuBounds = new List<Rectangle>();
        private bool inDebug = false;
        private Rectangle creationBounds = new Rectangle(300, 300, 600, 300);

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
            // Initialize spritebatch
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load all SpriteFonts
            primaryFont = Content.Load<SpriteFont>("PrimaryFont");
            headerFont = Content.Load<SpriteFont>("HeaderFont");
            senRegular = Content.Load<SpriteFont>("SenRegular");
            senBold = Content.Load<SpriteFont>("SenBoldFont");
            senExtraBold = Content.Load<SpriteFont>("SenExtraBoldFont");
            headerFont = Content.Load<SpriteFont>("headerFont");

            // Load all Sound Effects
            clicked = Content.Load<SoundEffect>("clicked");
            stateChange = Content.Load<SoundEffect>("stateChange");
            smallBash = Content.Load<SoundEffect>("smallBash");
            mediumBash = Content.Load<SoundEffect>("mediumBash");
            largeBash = Content.Load<SoundEffect>("largeBash");
            gameOverSound = Content.Load<SoundEffect>("gameOverSound");
            timeIncrease = Content.Load<SoundEffect>("timeIncrease");
            addBoids = Content.Load<SoundEffect>("boidsAdded");

            // Load all Songs
            gameMusic = Content.Load<Song>("gameMusic");
            menuMusic = Content.Load<Song>("mainMenuMusic");
            //discoMusic = Content.Load<Song>("discoTheme");

            // Play music
            MediaPlayer.Play(menuMusic);
            MediaPlayer.IsRepeating = true;

            // Load all textures
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
            insertCoin = Content.Load<Texture2D>("insertCoin");
            boidSprite = Content.Load<Texture2D>("BoidSp4");
            displayBoid = Content.Load<Texture2D>("DisplayBoid");
            blank = Content.Load<Texture2D>("WhiteSquare");
            gradient = Content.Load<Texture2D>("SquareArt");
            glowBorder = Content.Load<Texture2D>("SquareGlow");
            predTexture = Content.Load<Texture2D>("PredSp");

            // Create flocks
            flock = new Flock(70, new Rectangle(300, 300, 600, 300),
            boidSprite, new Vector2(10, 12), boidColor, _spriteBatch);

            menuFlock = new Flock(100, new Rectangle(300, 300, 600, 300),
            boidSprite, new Vector2(10, 12), boidColor, _spriteBatch);

            // Set menu flock backgroundColor
            menuFlock.BackgroundColor = backgroundColor;

            // Assign SFX to bashers
            flock.Bashers.SmallBash = smallBash;
            flock.Bashers.MediumBash =  mediumBash;
            flock.Bashers.LargeBash = largeBash;
            flock.Bashers.TimeIncrease = timeIncrease;
            flock.Bashers.AddBoids = addBoids;

            menuFlock.Bashers.SmallBash = smallBash;
            menuFlock.Bashers.MediumBash = mediumBash;
            menuFlock.Bashers.LargeBash = largeBash;
            menuFlock.Bashers.TimeIncrease = timeIncrease;
            menuFlock.Bashers.AddBoids = addBoids;

            // Apply Disco mode colors to menu boids but do not activate
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

            // Add bashing pens to flock's bashers
            flock.Bashers.Pens.Add(new Rectangle(200, 100, 150, 100));
            flock.Bashers.Pens.Add(new Rectangle(1000, 200, 100, 150));
            flock.Bashers.Pens.Add(new Rectangle(850, 700, 150, 100));
            flock.Bashers.Pens.Add(new Rectangle(100, 550, 100, 150));
  
            // Create the predator
            predator = new Predator(predTexture, new Rectangle(width / 2, height / 2,
                35, 35),
                windowHeight, windowWidth, 35, 35);

            // Initialize all UI Objects
            mainMenuUI = new MainMenuUI(windowWidth, windowHeight, playPrompt, boidBashLogo, insertCoin, senBold);
            gameUI = new GameUI(windowWidth, windowHeight, senBold, senExtraBold, boidBashLogo, pausePrompt);
            pauseMenuUI = new PauseMenuUI(windowWidth, windowHeight, resumePrompt, returnPrompt, pausedDisplay);
            endScreenUI = new EndScreenUI(windowWidth, windowHeight, continuePrompt, gameOver, senBold);
           
            // Add buttons
            buttons.Add(new Button(
                    _graphics.GraphicsDevice,           // Device to create a custom texture
                    new Rectangle(110, 110, 80, 80),    // Where to put the button
                    Color.DarkRed,                      // Button color
                    0,                                  // Pen number
                    bashButton,                         // Texture
                    clicked));
            buttons[0].OnButtonClick += this.Bashed;

            buttons.Add(new Button(
                    _graphics.GraphicsDevice,           // Device to create a custom texture
                    new Rectangle(1010, 110, 80, 80),   // Where to put the button
                    Color.DarkRed,                      // Button color
                    1,                                  // Pen number
                    bashButton,                         // Texture
                    clicked));
            buttons[1].OnButtonClick += this.Bashed;

            buttons.Add(new Button(
                    _graphics.GraphicsDevice,           // Device to create a custom texture
                    new Rectangle(1010, 710, 80, 80),   // Where to put the button
                    Color.DarkRed,                      // Button color
                    2,                                  // Pen number
                    bashButton,                         // Texture
                    clicked));
            buttons[2].OnButtonClick += this.Bashed;

            buttons.Add(new Button(
                    _graphics.GraphicsDevice,           // Device to create a custom texture
                    new Rectangle(110, 710, 80, 80),    // Where to put the button
                    Color.DarkRed,                      // Button color
                    3,                                  // Pen number
                    bashButton,                         // Texture
                    clicked));
            buttons[3].OnButtonClick += this.Bashed;

            // Add Excluded keys for names
            excludedKeys.Add(Keys.Space);
            excludedKeys.Add(Keys.LeftShift);
            excludedKeys.Add(Keys.RightShift);
            excludedKeys.Add(Keys.LeftControl);
            excludedKeys.Add(Keys.RightControl);
            excludedKeys.Add(Keys.LeftAlt);
            excludedKeys.Add(Keys.RightAlt);
            excludedKeys.Add(Keys.Up);
            excludedKeys.Add(Keys.Down);
            excludedKeys.Add(Keys.Left);
            excludedKeys.Add(Keys.Right);
            excludedKeys.Add(Keys.Back);
            excludedKeys.Add(Keys.Enter);
            excludedKeys.Add(Keys.Tab);
            excludedKeys.Add(Keys.CapsLock);
            excludedKeys.Add(Keys.Delete);
            excludedKeys.Add(Keys.Insert);
            excludedKeys.Add(Keys.Home);
            excludedKeys.Add(Keys.End);
            excludedKeys.Add(Keys.PageDown);
            excludedKeys.Add(Keys.PageUp);           
        }

        protected override void Update(GameTime gameTime)
        {
            // Exit if Escape is pressed
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Update keyboard state
            keyboardState = Keyboard.GetState();

            // GameState switches
            switch (currentState)
            {
                // Main Menu
                case GameState.MainMenu:
                    // Apply Main Menu processing
                    ProcessMainMenu(gameTime);
                    break;

                // Game
                case GameState.Game:
                    // Apply Game processing
                    ProcessGame(gameTime);               
                    break;

                // Pause Menu
                case GameState.PauseMenu:
                    // Apply Pause Menu processing
                    ProcessPauseMenu();
                    break;

                // End Screen
                case GameState.EndScreen:
                    // Apply End Screen processing
                    ProcessEndScreen();
                    break;

                default:
                    break;
            }

            // Update previous keyboard state
            lastKeyboardState = keyboardState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Apply the background in the back
            GraphicsDevice.Clear(backgroundColor);

            // Begin the Sprite Batch and the ShapeBatch
            ShapeBatch.Begin(GraphicsDevice);
            _spriteBatch.Begin();
            

            mouseState = Mouse.GetState();

            // GameState switches
            switch (currentState)
            {
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                //                                         Main Menu
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                case GameState.MainMenu:
                    menuFlock.Draw();
                    mainMenuUI.Draw(_spriteBatch);
                    _spriteBatch.DrawString(senRegular, String.Format("HIGH SCORES"), new Vector2(40, 15), Color.White);
                    _spriteBatch.DrawString(senRegular, String.Format("________________"), new Vector2(30, 20), Color.White);
                    _spriteBatch.DrawString(senRegular, GetScoreList(), new Vector2(15, 45), Color.White);
                    break;

                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                //                                         Game State
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                case GameState.Game:

                    //Draws the main play area for the game
                    _spriteBatch.Draw(blank, singlePlayerPlayArea, playAreaColor);

                    // Draws the Bashers from top to bottom
                    foreach (Rectangle basher in singlePlayerBashers)
                    {
                        _spriteBatch.Draw(gradient, basher, penColor);
                    }          

                    // Calculate border colors
                    rInterval = (Color.Red.R - Color.Lime.R) / 30;
                    gInterval = (Color.Red.G - Color.Lime.G) / 30;
                    bInterval = (Color.Red.B - Color.Lime.B) / 30;
                    colorDrawn = new Color(
                        (Color.Red.R + rInterval * (int)timer * -1),
                        (Color.Red.G + gInterval * (int)timer * -1),
                        (Color.Red.B + bInterval * (int)timer * -1)
                        );

                    // Draw Border
                    foreach (Rectangle border in singlePlayerBorders)
                    {
                        _spriteBatch.Draw(blank, border, colorDrawn);
                    }

                    // Draws debug view if in debug mode
                    if (inDebug)
                    {
                        // Grabbing actual values from the flock to make sure they line up
                        foreach (Rectangle bound in flock.Boundaries)
                        {
                            _spriteBatch.Draw(blank, bound, Color.Green);
                        }
                        foreach (Rectangle pen in flock.Bashers.Pens)
                        {
                            _spriteBatch.Draw(blank, pen, Color.Red);
                        }
                        _spriteBatch.Draw(blank, creationBounds, Color.Blue);
                    }

                    // Draw all text on screen
                    gameUI.DrawPausePrompt(_spriteBatch);
                    gameUI.DrawScore(_spriteBatch);
                    gameUI.DrawScoreGoal(_spriteBatch, scoreGoal);

                    //Draws the clock red at 5 seconds
                    if (timer > 5)
                    {
                        _spriteBatch.DrawString(senBold, "Time: " + String.Format("{0:0.00}", timer.ToString("0")), new Vector2(1050, 15),
                         Color.White);
                    }
                    else
                    {
                        _spriteBatch.DrawString(senBold, "Time: " + String.Format("{0:0.00}", timer.ToString("0")), new Vector2(1050, 15),
                         Color.Red);
                    }

                    // Draw display boid pictures
                    _spriteBatch.Draw(
                        displayBoid,
                        displayBoids[0],
                        boidColor
                        );
                    _spriteBatch.Draw(
                        displayBoid,
                        displayBoids[1],
                        Color.Gold
                        );
                    // Draw the number of types of boids bashed
                    _spriteBatch.DrawString(senBold, "x " + String.Format("{0:n0}", flock.Bashers.TotalBoidsBashed), new Vector2(75, 300),
                    Color.White);
                    _spriteBatch.DrawString(senBold, "x " + String.Format("{0:n0}", flock.Bashers.TotalSpecialBoidsBashed), new Vector2(75, 400),
                    Color.White);

                    // Call Draw functions on the player and AI
                    flock.Draw();
                    predator.Draw(_spriteBatch);

                    // Draws and removes any new point numbers that show up after destroying boids
                    foreach (Vector3 info in flock.Bashers.ScorePrints)
                    {
                        _spriteBatch.DrawString(senRegular, "+" + String.Format("{0:n0}", info.Z), new Vector2(info.X, info.Y), Color.Yellow);
                    }
                    // Change the amount of time left on the timers
                    for (int x = flock.Bashers.ScoreTimers.Count - 1; x >= 0; x--)
                    {
                        flock.Bashers.ScoreTimers[x] -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (flock.Bashers.ScoreTimers[x] <= 0)
                        {
                            flock.Bashers.ScorePrints.RemoveAt(x);
                            flock.Bashers.ScoreTimers.RemoveAt(x);
                        }
                    }

                    // Draws and removes any new point numbers that show up after destroying special boids
                    foreach (Vector3 info in flock.Bashers.SpecialScorePrints)
                    {
                        _spriteBatch.DrawString(senRegular, "+" + info.Z.ToString(), new Vector2(info.X, info.Y), Color.Magenta);
                    }
                    // Change the amount of time left on the special timers
                    for (int x = flock.Bashers.SpecialScoreTimers.Count - 1; x >= 0; x--)
                    {
                        flock.Bashers.SpecialScoreTimers[x] -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (flock.Bashers.SpecialScoreTimers[x] <= 0)
                        {
                            flock.Bashers.SpecialScorePrints.RemoveAt(x);
                            flock.Bashers.SpecialScoreTimers.RemoveAt(x);
                        }
                    }

                    // Draw Bashing buttons
                    foreach (Button b in buttons)
                    {
                        b.Draw(_spriteBatch);
                    }


                    // Draw total score increment
                    if (totalScoreIncrementPrint.Count > 0)
                    {
                        // Draw the string
                        _spriteBatch.DrawString(senRegular, "+ " + string.Format("{0:n0}", totalScoreIncrementPrint[0]), new Vector2(60, 60), Color.Yellow);
                        // Increment timer
                        totalScoreIncrementTimer[0] -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                        // Remove if timer is over
                        if (totalScoreIncrementTimer[0] <= 0)
                        {
                            totalScoreIncrementPrint.RemoveAt(0);
                            totalScoreIncrementTimer.RemoveAt(0);
                        }
                    }
                    // Do same for other prints
                    if (totalScoreIncrementPrint.Count > 1)
                    {
                        _spriteBatch.DrawString(senRegular, "+ " + string.Format("{0:n0}", totalScoreIncrementPrint[1]), new Vector2(80, 80), Color.Yellow);
                        
                        totalScoreIncrementTimer[1] -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                        
                        if (totalScoreIncrementTimer[1] <= 0)
                        {
                            totalScoreIncrementPrint.RemoveAt(1);
                            totalScoreIncrementTimer.RemoveAt(1);
                        }
                    }
                    if (totalScoreIncrementPrint.Count > 2)
                    {
                        _spriteBatch.DrawString(senRegular, "+ " + string.Format("{0:n0}", totalScoreIncrementPrint[2]), new Vector2(100, 100), Color.Yellow);
                        
                        totalScoreIncrementTimer[2] -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                        
                        if (totalScoreIncrementTimer[2] <= 0)
                        {
                            totalScoreIncrementPrint.RemoveAt(2);
                            totalScoreIncrementTimer.RemoveAt(2);

                        }
                    }

                    // Draw total time increment
                    if (totalTimeIncrementPrint.Count > 0)
                    {
                        _spriteBatch.DrawString(senRegular, "+ " + string.Format("{0:n0}", totalTimeIncrementPrint[0]), new Vector2(1100, 60), Color.Magenta);
                        totalTimeIncrementTimer[0] -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (totalTimeIncrementTimer[0] <= 0)
                        {
                            totalTimeIncrementPrint.RemoveAt(0);
                            totalTimeIncrementTimer.RemoveAt(0);
                        }
                    }
                    if (totalTimeIncrementPrint.Count > 1)
                    {
                        _spriteBatch.DrawString(senRegular, "+ " + string.Format("{0:n0}", totalTimeIncrementPrint[1]), new Vector2(1120, 80), Color.Magenta);
                        totalTimeIncrementTimer[1] -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (totalTimeIncrementTimer[1] <= 0)
                        {
                            totalTimeIncrementPrint.RemoveAt(1);
                            totalTimeIncrementTimer.RemoveAt(1);
                        }
                    }
                    if (totalTimeIncrementPrint.Count > 2)
                    {
                        _spriteBatch.DrawString(senRegular, "+ " + string.Format("{0:n0}", totalTimeIncrementPrint[2]), new Vector2(1140, 100), Color.Magenta);
                        totalTimeIncrementTimer[2] -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (totalTimeIncrementTimer[2] <= 0)
                        {
                            totalTimeIncrementPrint.RemoveAt(2);
                            totalTimeIncrementTimer.RemoveAt(2);

                        }
                    }
                    break;


                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                //                                         Pause Menu
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                case GameState.PauseMenu:

                    //Draw the main play area for the game
                    _spriteBatch.Draw(blank, singlePlayerPlayArea, playAreaColor);

                    // Draw the bashers from top to bottom
                    foreach (Rectangle basher in singlePlayerBashers)
                    {
                        _spriteBatch.Draw(gradient, basher, penColor);
                    }

                    //Draws border lines for play area when paused
                    foreach (Rectangle border in singlePlayerBorders)
                    {
                        _spriteBatch.Draw(blank, border, colorDrawn);
                    }

                    // Draw Text on the screen
                    _spriteBatch.DrawString(senBold, "Time: " + timer.ToString("0"), new Vector2(1050, 15),
                    Color.White);
                    gameUI.DrawScore(_spriteBatch);
                    gameUI.DrawScoreGoal(_spriteBatch, scoreGoal);
                    pauseMenuUI.Draw(_spriteBatch); 

                    // Draw Display boids
                    _spriteBatch.Draw(
                        displayBoid,
                        displayBoids[0],
                        boidColor
                        );
                    _spriteBatch.Draw(
                        displayBoid,
                        displayBoids[1],
                        Color.Gold
                        );
                    // Draw number of types of boids bashed
                    _spriteBatch.DrawString(senBold, "x " + String.Format("{0:n0}", flock.Bashers.TotalBoidsBashed), new Vector2(75, 300),
                    Color.White);
                    _spriteBatch.DrawString(senBold, "x " + String.Format("{0:n0}", flock.Bashers.TotalSpecialBoidsBashed), new Vector2(75, 400),
                    Color.White);

                    // Draw player and AI
                    flock.Draw();
                    predator.Draw(_spriteBatch);

                    break;

                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                //                                         End Screen
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                case GameState.EndScreen:

                    // Draw text on screen
                    endScreenUI.Draw(_spriteBatch);

                    // Draw Display boids
                    _spriteBatch.Draw(
                        displayBoid,
                       displayBoids[2],
                        boidColor
                        );
                    _spriteBatch.Draw(
                        displayBoid,
                        displayBoids[3],
                        Color.Gold
                        );
                    // Draw number of types of boids bashed
                    _spriteBatch.DrawString(senBold, "x " + String.Format("{0:n0}", flock.Bashers.TotalBoidsBashed), new Vector2(230, 470),
                    Color.White);
                    _spriteBatch.DrawString(senBold, "x " + String.Format("{0:n0}", flock.Bashers.TotalSpecialBoidsBashed), new Vector2(905, 470),
                    Color.White);

                    // Draw Name input
                    if (CompareToList(player1Score))
                    {
                        _spriteBatch.DrawString(senBold, "New notable Score!", new Vector2(460, 500),
                    Color.White);
                        _spriteBatch.DrawString(senBold, "Input Name: " + name + "|", new Vector2(460, 530),
                    Color.White);
                    }
                    

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
            // Create new color from random values
            Color color = new Color(
                rng.Next(0, 256),
                rng.Next(0, 256),
                rng.Next(0, 256)
                );

            // Return color
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
        private void ProcessMainMenu(GameTime gameTime)
        {
            // If they start the game with space bar
            if (IsSingleKeyPress(Keys.Space))
            {
                // Play game music
                MediaPlayer.Play(gameMusic);
                MediaPlayer.IsRepeating = true;

                // Play state change SFX
                stateChange.Play();

                // Reset timer to 30
                timer = 30;

                // Clear all prints and timers
                flock.Bashers.ScoreTimers.Clear();
                flock.Bashers.ScorePrints.Clear();
                totalScoreIncrementPrint.Clear();
                totalScoreIncrementTimer.Clear();
                totalTimeIncrementPrint.Clear();
                totalTimeIncrementTimer.Clear();

                // Clear the Game flock of all excess boids above 50
                // This is more efficient than clearing and adding all back in
                for (int x = flock.Boids.Count - 1; x >= 50; x--)
                {
                    flock.RemoveBoid(flock.Boids[x]);
                }
                // Reposition all boids
                flock.RepositionBoids();

                // Reset bash totals
                flock.Bashers.TotalBoidsBashed = 0;
                flock.Bashers.TotalSpecialBoidsBashed = 0;

                // Change Game state
                currentState = GameState.Game; 
            }

            // Process Menuflock boids
            menuFlock.ProcessBoids(new Vector2(-300, -300));
            // Update the UI for main menu
            mainMenuUI.Update(gameTime);
        }

        /// <summary>
        /// This method processes the Game state
        /// </summary>
        private void ProcessGame(GameTime gameTime)
        {
            // Call each button's update method
            foreach (Button button in buttons)
            {
                button.Update();
            }

            // Pause if tab is pressed
            if (IsSingleKeyPress(Keys.Tab))
            {
                // Pause music
                MediaPlayer.Pause();
                stateChange.Play();
                // Change game state
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

            // End game when timer is up 
            if (timer < 0.01f)
            {      
                // Update End screen UI
                endScreenUI.Score = player1Score;
                // Change state
                currentState = GameState.EndScreen;
                // Stop music and play sound
                gameOverSound.Play();
                MediaPlayer.Stop();
            }

            // Process Boids and predator
            flock.ProcessBoids(predator.ActualPosition);
            predator.Update(gameTime);

            // Update Game timer
            if (timer > 0)
            {
                timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            // Update score for game UI
            gameUI.ScoreUpdater(player1Score);
        }

        /// <summary>
        /// This method processes the Pause Menu state
        /// </summary>
        private void ProcessPauseMenu()
        {
            // If space is pressed, resume game
            if (IsSingleKeyPress(Keys.Space))
            {
                // Resume music
                MediaPlayer.Resume();
                stateChange.Play();
                // Change game state
                currentState = GameState.Game;
            }
            // Otherwise, if M is pressed, go to main menu
            else if (IsSingleKeyPress(Keys.M))
            {
                // Play main menu music
                MediaPlayer.Play(menuMusic);
                MediaPlayer.IsRepeating = true;
                stateChange.Play();
                // Change game state
                currentState = GameState.MainMenu;
                // Put predator back in center
                predator.Position = new Rectangle(width / 2, height / 2, 25, 25);
                // Reset player score
                player1Score = 0;
                // Reset Score goal
                scoreGoal = 1;
            }
        }

        /// <summary>
        /// This method processes the End Screen state
        /// </summary>
        private void ProcessEndScreen()
        {
            // If M is pressed, go back to Main Menu
            if (IsSingleKeyPress(Keys.M))
            {
                // Update player highscores
                if (CompareToList(player1Score))
                {
                    if (name == "")
                    {
                        name = "UNNAMED";
                    }
                    UpdateScores(player1Score);
                }

                // Play menu music
                MediaPlayer.Play(menuMusic);
                MediaPlayer.IsRepeating = true;
                stateChange.Play();
                // Change game state
                currentState = GameState.MainMenu;
                // Reset player score and score goal
                player1Score = 0;
                scoreGoal = 1;

                name = "";
            }
            // Otherwise, if Space is pressed, go back to Game
            else if (IsSingleKeyPress(Keys.Space))
            {
                // Play game music
                MediaPlayer.Play(gameMusic);
                MediaPlayer.IsRepeating = true;

                // Play state change SFX
                stateChange.Play();

                // Reset timer to 30
                timer = 30;

                // Clear all prints and timers
                flock.Bashers.ScoreTimers.Clear();
                flock.Bashers.ScorePrints.Clear();
                totalScoreIncrementPrint.Clear();
                totalScoreIncrementTimer.Clear();
                totalTimeIncrementPrint.Clear();
                totalTimeIncrementTimer.Clear();

                // Clear the Game flock of all excess boids above 50
                // This is more efficient than clearing and adding all back in
                for (int x = flock.Boids.Count - 1; x >= 50; x--)
                {
                    flock.RemoveBoid(flock.Boids[x]);
                }
                // Reposition all boids
                flock.RepositionBoids();

                // Reset bash totals
                flock.Bashers.TotalBoidsBashed = 0;
                flock.Bashers.TotalSpecialBoidsBashed = 0;

                // Update player highscores
                if (CompareToList(player1Score))
                {
                    if (name == "")
                    {
                        name = "UNNAMED";
                    }
                    UpdateScores(player1Score);
                }

                // Clear Game data
                name = "";
                player1Score = 0;
                scoreGoal = 1;

                // Change Game state
                currentState = GameState.Game;
            }

            // Name input
            // If only one key is being pressed
            if (keyboardState.GetPressedKeys().Length == 1)
            {
                // Set key to the key that was pressed
                key = keyboardState.GetPressedKeys()[0];
                // If it is a single key press of that key
                if (IsSingleKeyPress(key))
                {
                    // If the string has characters and you hit backspace
                    if (key == Keys.Back && name.Length > 0)
                    {
                        // Take off the last letter in the string
                        name = name.Substring(0, name.Length - 1);
                    }
                    // If the name isn't at max length, and it is a valid key, add to string
                    if (name.Length <= 2 && !excludedKeys.Contains(key))
                    {
                        name += key.ToString();
                    }
                }
            }

        }

        // TODO - Add playernames to text file
        /// <summary>
        /// This method updates the high scores text file
        /// Returns true if the score was added to the list
        /// </summary>
        private void UpdateScores(ulong score)
        {
            string[] splits = new string[2];

            // Read through text file, add them to the list of scores
            try
            {
                input = new StreamReader("..//..//..//Highscores.txt");

                while ((line = input.ReadLine()) != null)
                {
                    splits = line.Split(',');
                    scores.Add(ulong.Parse(splits[0]));
                    names.Add(splits[1]);
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
                    names.Insert(x + 1, name);
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
                names.Insert(0, name);
            }

            // If the score was added, and there are more than 10 items in the list,
            //  remove the last item in the list
            if (willAdd && scores.Count > 10)
            {
                scores.RemoveAt(scores.Count - 1);
                names.RemoveAt(names.Count - 1);
            }

            // If the score was added, write the new text file
            if (willAdd)
            {
                try
                {
                    output = new StreamWriter("..//..//..//Highscores.txt");

                    // Write a line for each score
                    for (int x = 0; x < 10; x++)
                    {
                        output.WriteLine(scores[x] + "," + names[x]);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                output.Close();
            }

            scores.Clear();
            names.Clear();
        }

        /// <summary>
        /// Compares the score to the list of scores to see if it will be added
        /// </summary>
        /// <returns></returns>
        private bool CompareToList(ulong score)
        {
            bool willAdd = false;
            string[] splits = new string[2];
            // Read through text file, add them to the list of scores
            try
            {
                input = new StreamReader("..//..//..//Highscores.txt");

                while ((line = input.ReadLine()) != null)
                {
                    splits = line.Split(',');
                    scores.Add(ulong.Parse(splits[0]));
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

            // Comparison
            if (scores.Count < 10)
            {
                willAdd = true;
            }
            if (score >= scores[scores.Count - 1])
            {
                willAdd = true;
            }

            scores.Clear();
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
            string[] splits = new string[2];
            StreamReader input = null;

            try
            {
                // Create streamreader
                input = new StreamReader("..//..//..//Highscores.txt");

                // Loop through the 10 or less scores in the list
                for (int x = 0; x < 10 && ((line = input.ReadLine()) != null); x++)
                {
                    splits = line.Split(",");
                    if (x < 9)
                    {
                        scores += "  ";
                    }
                    // Add which place they are in, then the score, then name, then a new line
                    scores += ((x + 1) + ". " + String.Format("{0:n0}", ulong.Parse(splits[0])) + " - " + splits[1] + "\n");
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
        /// ~~~ Note from Brian -> When we update this for multiplayer, we'll have to pass in which player score to increment
        /// </summary>
        /// <param name="pen"></param>
        public void Bashed(int pen)
        {
            // Bash boids in pen, and get the data from it
            Vector2 dataReturn;
            dataReturn = flock.Bashers.BashContainedBoids(flock, pen, scoreGoal);

            // Add score increment to player score
            player1Score += (ulong)dataReturn.X;

            // Add total to totalscoreincrementprints, as long as points were added
            if (dataReturn.X > 0)
            {
                totalScoreIncrementPrint.Add((int)dataReturn.X);
                totalScoreIncrementTimer.Add(2);
            }

            // Determine if score goal, timer, or both are being incremented
            // 1 - > up score goal
            // 2 - > timer goes up
            // 3 - > both go up
            // Anything else -> no effect
            if (dataReturn.Y == 1)
            {
                scoreGoal++;
            }
            else if (dataReturn.Y == 2)
            {
                // Add to timerincrementprints and timers
                totalTimeIncrementPrint.Add(2);
                totalTimeIncrementTimer.Add(2);
                timer += 2;
            }
            else if (dataReturn.Y == 3)
            {
                // Add to timerincrementprints and timers
                totalTimeIncrementPrint.Add(2);
                totalTimeIncrementTimer.Add(2);
                timer += 2;
                scoreGoal++;
            }
        }
    }
}