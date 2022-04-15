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
    enum GameState
    {
        MainMenu,
        SingleGame,
        VersusGame,
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
        private List<long> totalScoreIncrementPrint = new List<long>();
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
        private List<Button> instructionButtons = new List<Button>();
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
        private Texture2D mouseControls;
        private Texture2D arrowControls;
        private Texture2D wasdControls;

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
        private Song raveMusic;

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
        private Flock instructionsFlock;
        private Flock versusFlock;
        private Texture2D displayBoid;

        // Predator
        private Predator predatorWASD;
        private Predator predatorArrows;
        private Predator predatorWASDArrows;
        private Texture2D predTexture;
        private int width;
        private int height;

        // Player Score
        private ulong player1Score = 0;
        private int scoreGoal = 1;

        // Text Input
        private string name = "";
        private string code = "";
        private Keys key;
        private List<Keys> excludedKeys = new List<Keys>();       
        private bool rave = false;
        private bool addCursorBoid = false;
        private bool displayToolTip = true;

        // Timer
        private float timer = 30f;
        private float versusTimer1 = 30f;
        private float versusTimer2 = 10f;
        private float raveTimer = 10.5f;
        private float menuTimer = 5;
        private float toolTipTimer = 10;

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

        private Rectangle[] versusLeftBorders = new Rectangle[]
            { new Rectangle (100, 95, 150, 5), new Rectangle (250, 95, 5, 100), new Rectangle (95, 95, 5, 705),
              new Rectangle (95, 800, 155, 5), new Rectangle (250, 700, 5, 105), new Rectangle (250, 700, 350, 5),
              new Rectangle (250, 195, 350, 5)};

        private Rectangle[] versusRightBorders = new Rectangle[]
            { 
              new Rectangle (945, 95, 5, 100), new Rectangle (945, 95, 155, 5), new Rectangle (1100, 95, 5, 705),
              new Rectangle (945, 700, 5, 100), new Rectangle (945, 800, 160, 5), new Rectangle (600, 195, 345, 5),
              new Rectangle (600, 700, 345, 5)};

        private Rectangle[] instructionsBashers = new Rectangle[2]
            { new Rectangle(100, 550, 100, 200), new Rectangle(1000, 550, 100, 200) };

        private Rectangle[] versusBashers = new Rectangle[4]
            { new Rectangle(100, 100, 150, 100), new Rectangle(950, 100, 150, 100),
             new Rectangle(100, 700, 150, 100), new Rectangle(950, 700, 150, 100)};

        private Rectangle[] instructionsBarriers = new Rectangle[8] 
        { new Rectangle(100, 400, 100, 150), new Rectangle(100, 400, 1000, 100), new Rectangle(1000, 400, 100, 150),
          new Rectangle(1000, 750, 100, 150), new Rectangle(100, 750, 100, 150), new Rectangle(100, 800, 1000, 100),
          new Rectangle(0, 400, 100, 700), new Rectangle(1100, 400, 100, 700)};

        private Rectangle singlePlayerPlayArea = new Rectangle(200, 200, 800, 500);

        private Rectangle instructionsPlayArea = new Rectangle(200, 500, 800, 300);

        private Rectangle versusPlayArea = new Rectangle(100, 200, 1000, 500);

        private Rectangle[] displayBoids = new Rectangle[4]
            { new Rectangle(15, 290, 46, 60), new Rectangle(15, 390, 46, 60),
            new Rectangle(240, 400, 46, 60), new Rectangle(905, 400, 46, 60)};

        private Rectangle musicSlider = new Rectangle(440, 140, 320, 10);
        private Rectangle soundSlider = new Rectangle(440, 240, 320, 10);

        private Rectangle[] boidColorSelectors = new Rectangle[7]
        { new Rectangle(280, 335, 40, 40), new Rectangle(380, 335, 40, 40), new Rectangle(480, 335, 40, 40),
          new Rectangle(580, 335, 40, 40), new Rectangle(680, 335, 40, 40), new Rectangle(780, 335, 40, 40),
          new Rectangle(880, 335, 40, 40) };

        private Rectangle[] predatorColorSelectors = new Rectangle[7]
            {new Rectangle(280, 435, 40, 40), new Rectangle(380, 435, 40, 40), new Rectangle(480, 435, 40, 40),
             new Rectangle(580, 435, 40, 40), new Rectangle(680, 435, 40, 40), new Rectangle(780, 435, 40, 40),
             new Rectangle(880, 435, 40, 40)};

        private Rectangle[] buttonColorSelectors = new Rectangle[7]
            {new Rectangle(280, 535, 40, 40), new Rectangle(380, 535, 40, 40), new Rectangle(480, 535, 40, 40),
             new Rectangle(580, 535, 40, 40), new Rectangle(680, 535, 40, 40), new Rectangle(780, 535, 40, 40),
             new Rectangle(880, 535, 40, 40)};

        private Rectangle[] borderColorSelectors = new Rectangle[8]
            { new Rectangle(335, 635, 40, 40), new Rectangle(375, 635, 40, 40), new Rectangle(485, 635, 40, 40),
              new Rectangle(525, 635, 40, 40), new Rectangle(635, 635, 40, 40), new Rectangle(675, 635, 40, 40),
              new Rectangle(785, 635, 40, 40), new Rectangle(825, 635, 40, 40)};

        private Rectangle centerMarker;

        // Options
        private float musicVolume = 1;
        private float soundVolume = 1;
        private int optionsSelection = 1;
        private int boidColorSelection = 1;
        private int predatorColorSelection1 = 4;
        private int predatorColorSelection2 = 6;
        private int borderFadeSelection = 1;
        private int buttonColorSelection = 4;
        private int menuSelection = 1;
        private Color fadeStart = Color.Lime;
        private Color fadeEnd = Color.Red;

        // Debug
        private Texture2D blank;
        private Texture2D gradient;
        private Texture2D glowBorder;
        private List<Rectangle> bounds = new List<Rectangle>();
        private List<Rectangle> menuBounds = new List<Rectangle>();
        private List<Rectangle> instructionBounds = new List<Rectangle>();
        private List<Rectangle> versusBounds = new List<Rectangle>();
        private bool inDebug = false;
        private Rectangle creationBounds = new Rectangle(300, 300, 600, 300);

        //Case Field
        private int choice = 1;

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
            raveMusic = Content.Load<Song>("raveMusic");

            // Play music
            MediaPlayer.Play(menuMusic);
            MediaPlayer.IsRepeating = true;

            // Load all textures
            bashButton = Content.Load<Texture2D>("BashButtonNew");
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
            displayBoid = Content.Load<Texture2D>("DisplayBoid");
            blank = Content.Load<Texture2D>("WhiteSquare");
            gradient = Content.Load<Texture2D>("SquareArt");
            glowBorder = Content.Load<Texture2D>("SquareGlow");
            predTexture = Content.Load<Texture2D>("PredSp");
            mouseControls = Content.Load<Texture2D>("Mouse");
            arrowControls = Content.Load<Texture2D>("Player2Controls");
            wasdControls = Content.Load<Texture2D>("Player1Controls");

            // Create flocks
            flock = new Flock(70, new Rectangle(300, 300, 600, 300),
            boidSprite, new Vector2(10, 12), boidColor, _spriteBatch);

            versusFlock = new Flock(100, new Rectangle(500, 500, 100, 100),
                boidSprite, new Vector2(10, 12), boidColor, _spriteBatch);

            menuFlock = new Flock(100, new Rectangle(300, 300, 600, 300),
            boidSprite, new Vector2(10, 12), boidColor, _spriteBatch);

            instructionsFlock = new Flock(10, new Rectangle(500, 600, 200, 100),
            boidSprite, new Vector2(10, 12), boidColor, _spriteBatch);

            // Set menu flock backgroundColor
            menuFlock.BackgroundColor = backgroundColor;

            // Assign SFX to bashers
            flock.Bashers.SmallBash = smallBash;
            flock.Bashers.MediumBash = mediumBash;
            flock.Bashers.LargeBash = largeBash;
            flock.Bashers.TimeIncrease = timeIncrease;
            flock.Bashers.AddBoids = addBoids;

            menuFlock.Bashers.SmallBash = smallBash;
            menuFlock.Bashers.MediumBash = mediumBash;
            menuFlock.Bashers.LargeBash = largeBash;
            menuFlock.Bashers.TimeIncrease = timeIncrease;
            menuFlock.Bashers.AddBoids = addBoids;

            instructionsFlock.Bashers.SmallBash = smallBash;
            instructionsFlock.Bashers.MediumBash = mediumBash;
            instructionsFlock.Bashers.LargeBash = largeBash;
            instructionsFlock.Bashers.TimeIncrease = timeIncrease;
            instructionsFlock.Bashers.AddBoids = addBoids;

            versusFlock.Bashers.SmallBash = smallBash;
            versusFlock.Bashers.MediumBash = mediumBash;
            versusFlock.Bashers.LargeBash = largeBash;
            versusFlock.Bashers.TimeIncrease = timeIncrease;
            versusFlock.Bashers.AddBoids = addBoids;

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

            // Instructions boundaries
            instructionBounds.Add(new Rectangle(100, 400, 100, 150));
            instructionBounds.Add(new Rectangle(100, 400, 1000, 100));
            instructionBounds.Add(new Rectangle(1000, 400, 100, 150));
            instructionBounds.Add(new Rectangle(1000, 750, 100, 150));
            instructionBounds.Add(new Rectangle(100, 750, 100, 150));
            instructionBounds.Add(new Rectangle(100, 800, 1000, 100));
            instructionBounds.Add(new Rectangle(0, 400, 100, 700));
            instructionBounds.Add(new Rectangle(1100, 400, 100, 700));
            instructionsFlock.Boundaries = instructionBounds;

            // Versus boundaries
            versusBounds.Add(new Rectangle(0, 0, 350, 100));
            versusBounds.Add(new Rectangle(0, 0, 100, 900));
            versusBounds.Add(new Rectangle(250, 0, 100, 200));
            versusBounds.Add(new Rectangle(250, 100, 700, 100));
            versusBounds.Add(new Rectangle(850, 0, 350, 100));
            versusBounds.Add(new Rectangle(1100, 0, 100, 900));
            versusBounds.Add(new Rectangle(0, 800, 350, 100));
            versusBounds.Add(new Rectangle(250, 700, 700, 100));
            versusBounds.Add(new Rectangle(850, 700, 100, 200));
            versusBounds.Add(new Rectangle(850, 800, 250, 100));
            versusFlock.Boundaries = versusBounds;

            // Add bashing pens to flock's bashers
            flock.Bashers.Pens.Add(new Rectangle(200, 100, 150, 100));
            flock.Bashers.Pens.Add(new Rectangle(1000, 200, 100, 150));
            flock.Bashers.Pens.Add(new Rectangle(850, 700, 150, 100));
            flock.Bashers.Pens.Add(new Rectangle(100, 550, 100, 150));

            // Add bashing pens to Instruction flock
            instructionsFlock.Bashers.Pens.Add(new Rectangle(100, 550, 100, 200));
            instructionsFlock.Bashers.Pens.Add(new Rectangle(1000, 550, 100, 200));

            // Create the predator
            predatorWASDArrows = new Predator(predTexture, new Rectangle(width / 2, height / 2,
                35, 35),
                windowHeight, windowWidth, Color.Red, 35, 35, ControlScheme.WASDArrows);
            predatorWASD = new Predator(predTexture, new Rectangle(width / 2, height / 2,
                35, 35),
                windowHeight, windowWidth, Color.Red, 35, 35, ControlScheme.WASD);
            predatorArrows = new Predator(predTexture, new Rectangle(width / 2, height / 2,
                35, 35),
                windowHeight, windowWidth, Color.Blue, 35, 35, ControlScheme.Arrows);

            // *Debug* set center marker
            centerMarker = new Rectangle(width / 2 - 5, 0, 10, 1000); 

            // Initialize all UI Objects
            mainMenuUI = new MainMenuUI(windowWidth, windowHeight, playPrompt, boidBashLogo, senBold, senRegular);
            gameUI = new GameUI(windowWidth, windowHeight, senBold, senExtraBold, boidBashLogo, pausePrompt);
            pauseMenuUI = new PauseMenuUI(windowWidth, windowHeight, resumePrompt, returnPrompt, pausedDisplay);
            endScreenUI = new EndScreenUI(windowWidth, windowHeight, continuePrompt, gameOver, senBold, returnPrompt);

            // Add buttons
            buttons.Add(new Button(
                    _graphics.GraphicsDevice,           // Device to create a custom texture
                    new Rectangle(110, 110, 80, 80),    // Where to put the button
                    Color.Red,                      // Button color
                    0,                                  // Pen number
                    bashButton,                         // Texture
                    clicked));
            buttons[0].OnButtonClick += this.Bashed;

            buttons.Add(new Button(
                    _graphics.GraphicsDevice,           // Device to create a custom texture
                    new Rectangle(1010, 110, 80, 80),   // Where to put the button
                    Color.Red,                      // Button color
                    1,                                  // Pen number
                    bashButton,                         // Texture
                    clicked));
            buttons[1].OnButtonClick += this.Bashed;

            buttons.Add(new Button(
                    _graphics.GraphicsDevice,           // Device to create a custom texture
                    new Rectangle(1010, 710, 80, 80),   // Where to put the button
                    Color.Red,                      // Button color
                    2,                                  // Pen number
                    bashButton,                         // Texture
                    clicked));
            buttons[2].OnButtonClick += this.Bashed;

            buttons.Add(new Button(
                    _graphics.GraphicsDevice,           // Device to create a custom texture
                    new Rectangle(110, 710, 80, 80),    // Where to put the button
                    Color.Red,                      // Button color
                    3,                                  // Pen number
                    bashButton,                         // Texture
                    clicked));
            buttons[3].OnButtonClick += this.Bashed;

            // Add instruction Buttons
            instructionButtons.Add(new Button(
                   _graphics.GraphicsDevice,           // Device to create a custom texture
                   new Rectangle(110, 110, 80, 80),    // Where to put the button
                   Color.Red,                      // Button color
                   0,                                  // Pen number
                   bashButton,                         // Texture
                   clicked));
            instructionButtons[0].OnButtonClick += this.Bashed;
            instructionButtons.Add(new Button(
                   _graphics.GraphicsDevice,           // Device to create a custom texture
                   new Rectangle(110, 110, 80, 80),    // Where to put the button
                   Color.Red,                      // Button color
                   0,                                  // Pen number
                   bashButton,                         // Texture
                   clicked));
            instructionButtons[0].OnButtonClick += this.Bashed;

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

                // Single Mode
                case GameState.SingleGame:
                    // Apply Game processing
                    ProcessGame(gameTime);
                    break;

                // Options
                case GameState.Options:
                    // Apply Options processing
                    ProcessOptions();
                    break;

                // Instructions 
                case GameState.Instructions:
                    ProcessInstructions(gameTime);
                    break;

                // Versus Mode
                case GameState.VersusGame:
                    ProcessVersus(gameTime);
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

                // Credits Screen
                case GameState.Credits:
                    // Apply Credits processing
                    ProcessCredits();
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
                    switch (menuSelection)
                    {
                        // Single player
                        case 1:
                            _spriteBatch.DrawString(senRegular, String.Format("Single Player"), new Vector2(515, 410), Color.Yellow);
                            break;

                        // Versus
                        case 2:
                            _spriteBatch.DrawString(senRegular, String.Format("Versus"), new Vector2(545, 410), Color.Yellow);
                            break;

                        // Options
                        case 3:
                            _spriteBatch.DrawString(senRegular, String.Format("Options"), new Vector2(540, 410), Color.Yellow);
                            break;

                        // Instructions
                        case 4:
                            _spriteBatch.DrawString(senRegular, String.Format("Instructions"), new Vector2(520, 410), Color.Yellow);
                            break;

                        // Credits
                        case 5:
                            _spriteBatch.DrawString(senRegular, String.Format("Credits"), new Vector2(540, 410), Color.Yellow);
                            break;
                    }
                    _spriteBatch.DrawString(senRegular, String.Format("<                            >"), new Vector2(485, 410), Color.White);

                    //Draws the highscores
                    _spriteBatch.DrawString(senRegular, GetScoreList(), new Vector2(450, 580), Color.White);

                    if (displayToolTip)
                    {
                        _spriteBatch.DrawString(senRegular, String.Format("Use A, D, or Left and Right arrow keys\n" +
                            "        to Cycle through menu options"), new Vector2(775, 30), Color.White);
                    }
                    else if (toolTipTimer > 0)
                    {
                        // WILL - when you implement animated text class, could use some help with this
                        _spriteBatch.DrawString(senRegular, String.Format("Use A, D, or Left and Right arrow keys\n" +
                            "        to Cycle through menu options"), new Vector2(775, 30),
                            new Color(backgroundColor.R, backgroundColor.G, backgroundColor.B));
                    }


                    break;

                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                //                                         Game State
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                case GameState.SingleGame:

                    //Draws the main play area for the game
                    _spriteBatch.Draw(blank, singlePlayerPlayArea, playAreaColor);

                    // Draws the Bashers from top to bottom
                    foreach (Rectangle basher in singlePlayerBashers)
                    {
                        _spriteBatch.Draw(gradient, basher, penColor);
                    }

                    // Calculate border colors
                    rInterval = (fadeEnd.R - fadeStart.R) / 30;
                    gInterval = (fadeEnd.G - fadeStart.G) / 30;
                    bInterval = (fadeEnd.B - fadeStart.B) / 30;
                    colorDrawn = new Color(
                        (fadeEnd.R + rInterval * (int)timer * -1),
                        (fadeEnd.G + gInterval * (int)timer * -1),
                        (fadeEnd.B + bInterval * (int)timer * -1)
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
                    predatorWASDArrows.Draw(_spriteBatch);

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
                //                                         Versus Mode 
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                case GameState.VersusGame:

                    // Draw play area
                    _spriteBatch.Draw(blank, versusPlayArea, playAreaColor);

                    // Draw bashers
                    foreach (Rectangle pen in versusBashers)
                    {
                        _spriteBatch.Draw(blank, pen, penColor);
                    }

                    // Set fade color for right
                    rInterval = (fadeEnd.R - fadeStart.R) / 30;
                    gInterval = (fadeEnd.G - fadeStart.G) / 30;
                    bInterval = (fadeEnd.B - fadeStart.B) / 30;
                    colorDrawn = new Color(
                        (fadeEnd.R + rInterval * (int)versusTimer1 * -1),
                        (fadeEnd.G + gInterval * (int)versusTimer1 * -1),
                        (fadeEnd.B + bInterval * (int)versusTimer1 * -1)
                        );

                    foreach (Rectangle border in versusRightBorders)
                    {
                        _spriteBatch.Draw(blank, border, colorDrawn);
                    }

                    rInterval = (fadeEnd.R - fadeStart.R) / 30;
                    gInterval = (fadeEnd.G - fadeStart.G) / 30;
                    bInterval = (fadeEnd.B - fadeStart.B) / 30;
                    colorDrawn = new Color(
                        (fadeEnd.R + rInterval * (int)versusTimer2 * -1),
                        (fadeEnd.G + gInterval * (int)versusTimer2 * -1),
                        (fadeEnd.B + bInterval * (int)versusTimer2 * -1)
                        );

                    foreach (Rectangle border in versusLeftBorders)
                    {
                        _spriteBatch.Draw(blank, border, colorDrawn);
                    }

                    

                    versusFlock.Draw();
                    predatorArrows.Draw(_spriteBatch);
                    predatorWASD.Draw(_spriteBatch);
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
                    predatorWASDArrows.Draw(_spriteBatch);

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
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                //                                         Instructions
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                // I AM WORKING ON THE CONTROLS FOR THE GAME, PLEASE DON'T TOUCH IT, I HAVE A VISION - RYAN
                case GameState.Instructions:

                    // Draw Play Area
                    _spriteBatch.Draw(blank, instructionsPlayArea, playAreaColor);
                    if (inDebug)
                    {       
                        
                    }
                    // Draw Pens
                    foreach (Rectangle pen in instructionsBashers)
                    {
                        _spriteBatch.Draw(blank, pen, penColor);
                    }
                    // Draw Barriers
                    foreach (Rectangle barrier in instructionsBarriers)
                    {
                        _spriteBatch.Draw(blank, barrier, Color.Gray);
                    }

                    // Draw Flock
                    instructionsFlock.Draw();
                    predatorWASDArrows.Draw(_spriteBatch);

                    _spriteBatch.DrawString(
                     senExtraBold,
                     String.Format("PRACTICE"),
                     new Vector2(490, 420),
                    Color.Red
                     );

                    // Main Menu prompt
                    _spriteBatch.Draw(
                        returnPrompt,
                        new Vector2(10, windowHeight - 30),
                        Color.White
                        );

                    switch (choice)
                    {
                        case 1:

                            //Draws Instructions
                            _spriteBatch.DrawString(
                             senBold,
                             String.Format("< Instructions >"),
                             new Vector2(470, 15),
                             Color.White
                             );

                            _spriteBatch.DrawString(
                            senBold,
                            String.Format("1/3"),
                            new Vector2(15, 350),
                           Color.White
                            );

                            _spriteBatch.DrawString(
                             senBold,
                             String.Format("You have 30 seconds to move the PREDATOR\ntowards the BOIDS to trap them in\n" +
                             "the BASHERS and BASH them\nto score as many points as possible!"),
                             new Vector2(250, 125),
                             Color.White
                             );

                            break;
                        case 2:
                            _spriteBatch.DrawString(
                             senBold,
                             String.Format("< Single Player Controls >"),
                             new Vector2(390, 15),
                             Color.White
                             );

                            _spriteBatch.DrawString(
                             senBold,
                             String.Format("Bash"),
                             new Vector2(750, 100),
                            Color.White
                             );

                            _spriteBatch.Draw(bashButton, new Vector2(825, 75), Color.Red);

                            _spriteBatch.DrawString(
                             senBold,
                             String.Format("Move"),
                             new Vector2(355, 100),
                            Color.White
                             );

                            _spriteBatch.DrawString(
                             senBold,
                             String.Format("Or"),
                             new Vector2(383, 220),
                            Color.White
                             );

                            //Draws the mouse for the instructions
                            _spriteBatch.Draw(mouseControls, new Vector2(775, 175), Color.White);

                            //Draws the Keyboard Keys
                            _spriteBatch.Draw(arrowControls, new Vector2(430, 180), Color.White);
                            _spriteBatch.Draw(wasdControls, new Vector2(170, 175), Color.White);

                            //Page 2 out of 3
                            _spriteBatch.DrawString(
                            senBold,
                            String.Format("2/3"),
                            new Vector2(15, 350),
                           Color.White
                            );
                            break;

                        case 3:

                            //Draws the title for versus controls
                            _spriteBatch.DrawString(
                             senBold,
                             String.Format("< Versus Controls >"),
                             new Vector2(450, 15),
                             Color.White
                             );

                            //Draw page 3 out of 3
                            _spriteBatch.DrawString(
                            senBold,
                            String.Format("3/3"),
                            new Vector2(15, 350),
                           Color.White
                            );

                            //PLayer 1 controls
                            _spriteBatch.DrawString(
                             senBold,
                             String.Format("Player 1 Controls"),
                             new Vector2(150, 75),
                            Color.White
                             );

                            _spriteBatch.DrawString(
                             senBold,
                             String.Format("Move"),
                             new Vector2(100, 160),
                            Color.White
                             );

                            _spriteBatch.Draw(wasdControls, new Vector2(40, 190), Color.White);

                            _spriteBatch.DrawString(
                             senBold,
                             String.Format("Bash"),
                             new Vector2(360, 160),
                            Color.White
                             );

                            _spriteBatch.Draw(bashButton, new Vector2(430, 130), Color.Red);

                            //Player 2 Controls
                            _spriteBatch.DrawString(
                             senBold,
                             String.Format("Player 2 Controls"),
                             new Vector2(780, 75),
                            Color.White
                             );

                            _spriteBatch.DrawString(
                             senBold,
                             String.Format("Move"),
                             new Vector2(730, 160),
                            Color.White
                             );

                            _spriteBatch.Draw(arrowControls, new Vector2(670, 190), Color.White);

                            _spriteBatch.DrawString(
                             senBold,
                             String.Format("Bash"),
                             new Vector2(980, 160),
                             Color.White
                             );
                            _spriteBatch.Draw(bashButton, new Vector2(1050, 130), Color.Red);

                            break;

                        default:
                            break;
                    }

                    break;

                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                //                                         Options
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                case GameState.Options:
                    // Title
                    _spriteBatch.DrawString(senExtraBold, "Options", new Vector2(510, 20), Color.White);

                    // Music Volume Selection
                    _spriteBatch.DrawString(senRegular, "0", new Vector2(445, 105), Color.White);
                    _spriteBatch.DrawString(senRegular, "100", new Vector2(735, 105), Color.White);
                    _spriteBatch.DrawString(senRegular, "Music Volume", new Vector2(530, 85), Color.White);
                    _spriteBatch.Draw(blank, musicSlider, Color.Gray);
                    if (optionsSelection == 1)
                    {
                        _spriteBatch.Draw(blank, new Rectangle((int)(musicVolume * 300) + 440, 130, 30, 30), Color.Yellow);
                    }
                    else
                    {
                        _spriteBatch.Draw(blank, new Rectangle((int)(musicVolume * 300) + 440, 130, 30, 30), Color.White);
                    }

                    // Sound Volume Selection
                    _spriteBatch.DrawString(senRegular, "0", new Vector2(445, 205), Color.White);
                    _spriteBatch.DrawString(senRegular, "100", new Vector2(735, 205), Color.White);
                    _spriteBatch.DrawString(senRegular, "Sound Volume", new Vector2(530, 185), Color.White);
                    _spriteBatch.Draw(blank, soundSlider, Color.Gray);
                    if (optionsSelection == 2)
                    {
                        _spriteBatch.Draw(blank, new Rectangle((int)(soundVolume * 300) + 440, 230, 30, 30), Color.Yellow);
                    }
                    else
                    {
                        _spriteBatch.Draw(blank, new Rectangle((int)(soundVolume * 300) + 440, 230, 30, 30), Color.White);
                    }

                    // Boid Color Selection
                    _spriteBatch.DrawString(senRegular, "Boid Color", new Vector2(550, 285), Color.White);
                    if (optionsSelection == 3)
                    {
                        _spriteBatch.Draw(blank, new Rectangle(boidColorSelection * 100 + 175, 330, 50, 50), Color.Yellow);
                    }
                    else
                    {
                        _spriteBatch.Draw(blank, new Rectangle(boidColorSelection * 100 + 175, 330, 50, 50), Color.White);
                    }
                    _spriteBatch.Draw(blank, boidColorSelectors[0], boidColor);
                    _spriteBatch.Draw(blank, boidColorSelectors[1], Color.Green);
                    _spriteBatch.Draw(blank, boidColorSelectors[2], Color.Orange);
                    _spriteBatch.Draw(blank, boidColorSelectors[3], Color.Red);
                    _spriteBatch.Draw(blank, boidColorSelectors[4], Color.Magenta);
                    _spriteBatch.Draw(blank, boidColorSelectors[5], Color.Blue);
                    _spriteBatch.Draw(blank, boidColorSelectors[6], Color.White);

                    // Predator Color Selection
                    _spriteBatch.DrawString(senRegular, "Predator Color", new Vector2(525, 390), Color.White);
                    if (optionsSelection == 4)
                    {
                        _spriteBatch.DrawString(senRegular, "Main", new Vector2(predatorColorSelection1 * 100 + 155, 407), Color.Yellow);
                        _spriteBatch.DrawString(senRegular, "P2", new Vector2(predatorColorSelection2 * 100 + 205, 407), Color.Yellow);
                        _spriteBatch.Draw(blank, new Rectangle(predatorColorSelection1 * 100 + 175, 430, 50, 50), Color.Yellow);
                        _spriteBatch.Draw(blank, new Rectangle(predatorColorSelection2 * 100 + 175, 430, 50, 50), Color.Yellow);
                    }
                    else
                    {
                        _spriteBatch.Draw(blank, new Rectangle(predatorColorSelection1 * 100 + 175, 430, 50, 50), Color.White);
                        _spriteBatch.Draw(blank, new Rectangle(predatorColorSelection2 * 100 + 175, 430, 50, 50), Color.White);
                    }
                    _spriteBatch.Draw(blank, predatorColorSelectors[0], boidColor);
                    _spriteBatch.Draw(blank, predatorColorSelectors[1], Color.Green);
                    _spriteBatch.Draw(blank, predatorColorSelectors[2], Color.Orange);
                    _spriteBatch.Draw(blank, predatorColorSelectors[3], Color.Red);
                    _spriteBatch.Draw(blank, predatorColorSelectors[4], Color.Magenta);
                    _spriteBatch.Draw(blank, predatorColorSelectors[5], Color.Blue);
                    _spriteBatch.Draw(blank, predatorColorSelectors[6], Color.White);

                    // Button Color Selection
                    _spriteBatch.DrawString(senRegular, "Button Color", new Vector2(535, 490), Color.White);
                    if (optionsSelection == 5)
                    {
                        _spriteBatch.Draw(blank, new Rectangle(buttonColorSelection * 100 + 175, 530, 50, 50), Color.Yellow);
                    }
                    else
                    {
                        _spriteBatch.Draw(blank, new Rectangle(buttonColorSelection * 100 + 175, 530, 50, 50), Color.White);
                    }
                    _spriteBatch.Draw(blank, buttonColorSelectors[0], boidColor);
                    _spriteBatch.Draw(blank, buttonColorSelectors[1], Color.Green);
                    _spriteBatch.Draw(blank, buttonColorSelectors[2], Color.Orange);
                    _spriteBatch.Draw(blank, buttonColorSelectors[3], Color.Red);
                    _spriteBatch.Draw(blank, buttonColorSelectors[4], Color.Magenta);
                    _spriteBatch.Draw(blank, buttonColorSelectors[5], Color.Blue);
                    _spriteBatch.Draw(blank, buttonColorSelectors[6], Color.White);

                    // Border Color Selection
                    _spriteBatch.DrawString(senRegular, "Border Fade Colors", new Vector2(500, 590), Color.White);
                    if (optionsSelection == 6)
                    {
                        _spriteBatch.Draw(blank, new Rectangle(borderFadeSelection * 150 + 180, 630, 90, 50), Color.Yellow);
                    }
                    else
                    {
                        _spriteBatch.Draw(blank, new Rectangle(borderFadeSelection * 150 + 180, 630, 90, 50), Color.White);
                    }
                    _spriteBatch.Draw(blank, borderColorSelectors[0], Color.Lime);
                    _spriteBatch.Draw(blank, borderColorSelectors[1], Color.Red);
                    _spriteBatch.Draw(blank, borderColorSelectors[2], Color.White);
                    _spriteBatch.Draw(blank, borderColorSelectors[3], Color.Black);
                    _spriteBatch.Draw(blank, borderColorSelectors[4], Color.Blue);
                    _spriteBatch.Draw(blank, borderColorSelectors[5], Color.OrangeRed);
                    _spriteBatch.Draw(blank, borderColorSelectors[6], Color.Orange);
                    _spriteBatch.Draw(blank, borderColorSelectors[7], Color.Purple);

                    _spriteBatch.DrawString(senRegular, "\"M\" to go back to main Menu and confirm", new Vector2(410, 700), Color.White);
                    break;

                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                //                                         Credits
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                case GameState.Credits:

                    // Main Menu prompt
                    _spriteBatch.Draw(
                        returnPrompt,
                        new Vector2(10, windowHeight - 30),
                        Color.White
                        );

                    _spriteBatch.Draw(
                         boidBashLogo,
                         new Rectangle(405, 280, 342, 300),
                         Color.White
                         );

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
            else if (mouseState.RightButton == ButtonState.Pressed && !addCursorBoid)
            {
                addCursorBoid = true;
                menuFlock.AddBoids(1);
                menuFlock.Boids[menuFlock.Boids.Count - 1].SpecialAsset = customCursor;
                menuFlock.Boids[menuFlock.Boids.Count - 1].UseSpecialAsset = true;
                menuFlock.Boids[menuFlock.Boids.Count - 1].UseDefaultColor = false;
                menuFlock.Boids[menuFlock.Boids.Count - 1].Color = Disco();
                menuFlock.Boids[menuFlock.Boids.Count - 1].HasTrail = true;
                menuFlock.RepositionBoidTo(menuFlock.Boids[menuFlock.Boids.Count - 1],
                    new Vector2(mouseState.Position.X, mouseState.Position.Y));
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
                    if (boid.IsSpecial || boid.UseSpecialAsset)
                    {
                        boid.UseDefaultColor = false;
                    }
                    else
                    {
                        boid.UseDefaultColor = true;
                    }
                }
            }

            // Rave mechanics
            if (rave)
            {
                if (raveTimer <= 0)
                {
                    _spriteBatch.Draw(
                                        customCursor,
                                        new Rectangle(mouseState.X, mouseState.Y, 16, 16),
                                        Disco()
                                        );

                    foreach (Boid boid in menuFlock.Boids)
                    {
                        boid.UseDefaultColor = false;
                        boid.Color = Disco();
                    }
                }
                else
                {
                    _spriteBatch.Draw(
                    customCursor,
                    new Rectangle(mouseState.X, mouseState.Y, 16, 16),
                    Color.White
                    );
                    raveTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }                              
            }

            // Custom cursor boid processing
            if (addCursorBoid)
            {
                menuFlock.Boids[menuFlock.Boids.Count - 1].Color = Disco();
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
            // Choose Option in menu
            if ((IsSingleKeyPress(Keys.A) || IsSingleKeyPress(Keys.Left)) && menuSelection > 1)
            {
                clicked.Play();
                menuSelection--;
            }
            else if ((IsSingleKeyPress(Keys.A) || IsSingleKeyPress(Keys.Left)))
            {
                clicked.Play();
                menuSelection = 5;
            }
            if ((IsSingleKeyPress(Keys.D) || IsSingleKeyPress(Keys.Right)) && menuSelection < 5)
            {
                clicked.Play();
                menuSelection++;
            }
            else if ((IsSingleKeyPress(Keys.D) || IsSingleKeyPress(Keys.Right)))
            {
                clicked.Play();
                menuSelection = 1;
            }

            // If they start the game with space bar
            if (IsSingleKeyPress(Keys.Space))
            {
                // Game State
                if (menuSelection == 1)
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
                    // Set Predator Boundaries
                    predatorWASDArrows.PredatorBounds = singlePlayerPlayArea;
                    predatorWASDArrows.Position = new Rectangle(width / 2, height / 2, 35, 35);
                    // Reset bash totals
                    flock.Bashers.TotalBoidsBashed = 0;
                    flock.Bashers.TotalSpecialBoidsBashed = 0;

                    // Turn off rave
                    rave = false;
                    code = "";

                    // Reset cursor Boid
                    menuFlock.RemoveBoid(menuFlock.Boids[menuFlock.Boids.Count - 1]);
                    addCursorBoid = false;

                    // Change Game state
                    currentState = GameState.SingleGame;
                }
                // Versus Mode
                else if (menuSelection == 2)
                {
                    code = "";
                    predatorWASD.PredatorBounds = versusPlayArea;
                    predatorArrows.PredatorBounds = versusPlayArea;
                    stateChange.Play();
                    currentState = GameState.VersusGame;
                }
                // Options
                else if (menuSelection == 3)
                {
                    code = "";
                    stateChange.Play();
                    currentState = GameState.Options;
                }
                // Instructions
                else if (menuSelection == 4)
                {
                    code = "";
                    stateChange.Play();
                    currentState = GameState.Instructions;
                    predatorWASDArrows.Position = new Rectangle(550, 500, 35, 35);
                    predatorWASDArrows.PredatorBounds = instructionsPlayArea;
                }
                // Credits
                else if (menuSelection == 5)
                {
                    currentState = GameState.Credits;
                }


            }

            // Detect Konami Code
            if (keyboardState.GetPressedKeys().Length == 1 && !rave)
            {
                // Set key to the key that was pressed
                key = keyboardState.GetPressedKeys()[0];
                // If it is a single key press of that key
                if (IsSingleKeyPress(key) && key != (Keys.Space) && key != (Keys.D))
                {
                    code += key.ToString();
                }
            }

            if (code == "UpUpDownDownLeftRightLeftRightBAEnter" && !rave)
            {
                rave = true;
                MediaPlayer.Play(raveMusic);
                MediaPlayer.IsRepeating = true;
            }

            // Tip on cycling options           
            if (menuTimer > 0)
            {
                menuTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                displayToolTip = true;
            }
            else
            {
                displayToolTip = false;
                if (toolTipTimer > 0)
                {
                    toolTipTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }

            // Process Menuflock boids
            menuFlock.ProcessBoids(new Vector2[1] { new Vector2(-300, -300) });
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
            predatorWASDArrows.Update(gameTime);
            flock.ProcessBoids(new Vector2[1] { predatorWASDArrows.ActualPosition });

            // Update Game timer.
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
                currentState = GameState.SingleGame;
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
                predatorWASDArrows.Position = new Rectangle(width / 2, height / 2, 25, 25);
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
                    if (!(name.Length <= 2))
                    {
                        // Play menu music
                        MediaPlayer.Play(menuMusic);
                        MediaPlayer.IsRepeating = true;
                        stateChange.Play();
                        // Change game state
                        currentState = GameState.MainMenu;
                        UpdateScores(player1Score);

                        name = "";
                        // Reset player score and score goal
                        scoreGoal = 1;
                        player1Score = 0;
                    }
                }
                else
                {
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
                    UpdateScores(player1Score);
                }


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
                        name = "---";
                    }
                    UpdateScores(player1Score);
                }

                // Clear Game data
                name = "";
                player1Score = 0;
                scoreGoal = 1;

                // Change Game state
                currentState = GameState.SingleGame;
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

        /// <summary>
        /// This method processes the Options state
        /// </summary>
        private void ProcessOptions()
        {
            // Select volume setting

            // Change what option is selected
            if ((IsSingleKeyPress(Keys.W) || IsSingleKeyPress(Keys.Up)) && optionsSelection > 1)
            {
                clicked.Play();
                optionsSelection--;
            }
            else if ((IsSingleKeyPress(Keys.W) || IsSingleKeyPress(Keys.Up)))
            {
                clicked.Play();
                optionsSelection = 6;
            }
            if ((IsSingleKeyPress(Keys.S) || IsSingleKeyPress(Keys.Down)) && optionsSelection < 6)
            {
                clicked.Play();
                optionsSelection++;
            }
            else if ((IsSingleKeyPress(Keys.S) || IsSingleKeyPress(Keys.Down)))
            {
                clicked.Play();
                optionsSelection = 1;
            }

            switch (optionsSelection)
            {
                case 1:
                    // Change Music Volume
                    if ((IsSingleKeyPress(Keys.A) || IsSingleKeyPress(Keys.Left)) && musicVolume > 0.2f)
                    {
                        clicked.Play();
                        musicVolume -= 0.2f;
                    }
                    if ((IsSingleKeyPress(Keys.D) || IsSingleKeyPress(Keys.Right)) && musicVolume < 1)
                    {
                        clicked.Play();
                        musicVolume += 0.2f;
                    }
                    break;
                case 2:
                    if ((IsSingleKeyPress(Keys.A) || IsSingleKeyPress(Keys.Left)) && soundVolume > 0.2f)
                    {
                        clicked.Play();
                        soundVolume -= 0.2f;
                    }
                    if ((IsSingleKeyPress(Keys.D) || IsSingleKeyPress(Keys.Right)) && soundVolume < 1)
                    {
                        clicked.Play();
                        soundVolume += 0.2f;
                    }
                    break;
                case 3:
                    // Change Boid Color
                    if ((IsSingleKeyPress(Keys.A) || IsSingleKeyPress(Keys.Left)) && boidColorSelection > 1)
                    {
                        clicked.Play();
                        boidColorSelection--;
                    }
                    else if ((IsSingleKeyPress(Keys.A) || IsSingleKeyPress(Keys.Left)))
                    {
                        clicked.Play();
                        boidColorSelection = 7;
                    }
                    if ((IsSingleKeyPress(Keys.D) || IsSingleKeyPress(Keys.Right)) && boidColorSelection < 7)
                    {
                        clicked.Play();
                        boidColorSelection++;
                    }
                    else if ((IsSingleKeyPress(Keys.D) || IsSingleKeyPress(Keys.Right)))
                    {
                        clicked.Play();
                        boidColorSelection = 1;
                    }
                    // Apply Color
                    switch (boidColorSelection)
                    {
                        case 1:
                            flock.DefaultColor = boidColor;
                            menuFlock.DefaultColor = boidColor;
                            instructionsFlock.DefaultColor = boidColor;
                            break;

                        case 2:
                            flock.DefaultColor = Color.Green;
                            menuFlock.DefaultColor = Color.Green;
                            instructionsFlock.DefaultColor = Color.Green;
                            break;

                        case 3:
                            flock.DefaultColor = Color.Orange;
                            menuFlock.DefaultColor = Color.Orange;
                            instructionsFlock.DefaultColor = Color.Orange;
                            break;

                        case 4:
                            flock.DefaultColor = Color.Red;
                            menuFlock.DefaultColor = Color.Red;
                            instructionsFlock.DefaultColor = Color.Red;
                            break;

                        case 5:
                            flock.DefaultColor = Color.Magenta;
                            menuFlock.DefaultColor = Color.Magenta;
                            instructionsFlock.DefaultColor = Color.Magenta;
                            break;

                        case 6:
                            flock.DefaultColor = Color.Blue;
                            menuFlock.DefaultColor = Color.Blue;
                            instructionsFlock.DefaultColor = Color.Blue;
                            break;

                        case 7:
                            flock.DefaultColor = Color.White;
                            menuFlock.DefaultColor = Color.White;
                            instructionsFlock.DefaultColor = Color.White;
                            break;
                    }

                    break;
                case 4:
                    // Change Main Predator Color
                    if (IsSingleKeyPress(Keys.A) && predatorColorSelection1 > 1)
                    {
                        clicked.Play();
                        predatorColorSelection1--;
                    }
                    else if (IsSingleKeyPress(Keys.A))
                    {
                        clicked.Play();
                        predatorColorSelection1 = 7;
                    }
                    if (IsSingleKeyPress(Keys.D) && predatorColorSelection1 < 7)
                    {
                        clicked.Play();
                        predatorColorSelection1++;
                    }
                    else if (IsSingleKeyPress(Keys.D))
                    {
                        clicked.Play();
                        predatorColorSelection1 = 1;
                    }

                    // Change Player 2 Predator Color
                    if (IsSingleKeyPress(Keys.Left) && predatorColorSelection2 > 1)
                    {
                        clicked.Play();
                        predatorColorSelection2--;
                    }
                    else if (IsSingleKeyPress(Keys.Left))
                    {
                        clicked.Play();
                        predatorColorSelection2 = 7;
                    }
                    if (IsSingleKeyPress(Keys.Right) && predatorColorSelection2 < 7)
                    {
                        clicked.Play();
                        predatorColorSelection2++;
                    }
                    else if (IsSingleKeyPress(Keys.Right))
                    {
                        clicked.Play();
                        predatorColorSelection2 = 1;
                    }

                    // Apply Color to main predator
                    switch (predatorColorSelection1)
                    {
                        case 1:
                            predatorWASD.Color = boidColor;
                            predatorWASDArrows.Color = boidColor;                           
                            break;

                        case 2:
                            predatorWASD.Color = Color.Green;
                            predatorWASDArrows.Color = Color.Green;
                            break;

                        case 3:
                            predatorWASD.Color = Color.Orange;
                            predatorWASDArrows.Color = Color.Orange;
                            break;

                        case 4:
                            predatorWASD.Color = Color.Red;
                            predatorWASDArrows.Color = Color.Red;
                            break;

                        case 5:
                            predatorWASD.Color = Color.Magenta;
                            predatorWASDArrows.Color = Color.Magenta;
                            break;

                        case 6:
                            predatorWASD.Color = Color.Blue;
                            predatorWASDArrows.Color = Color.Blue;
                            break;

                        case 7:
                            predatorWASD.Color = Color.White;
                            predatorWASDArrows.Color = Color.White;
                            break;
                    }
                    
                    switch (predatorColorSelection2)
                    {
                        case 1:
                            predatorArrows.Color = boidColor;
                            break;

                        case 2:
                            predatorArrows.Color = Color.Green;
                            break;

                        case 3:
                            predatorArrows.Color = Color.Orange;
                            break;

                        case 4:
                            predatorArrows.Color = Color.Red;
                            break;

                        case 5:
                            predatorArrows.Color = Color.Magenta;
                            break;

                        case 6:
                            predatorArrows.Color = Color.Blue;
                            break;

                        case 7:
                            predatorArrows.Color = Color.White;
                            break;
                    }

                break;
                // Change Button Color
                case 5:
                    if ((IsSingleKeyPress(Keys.A) || IsSingleKeyPress(Keys.Left)) && buttonColorSelection > 1)
                    {
                        clicked.Play();
                        buttonColorSelection--;
                    }
                    else if ((IsSingleKeyPress(Keys.A) || IsSingleKeyPress(Keys.Left)))
                    {
                        clicked.Play();
                        buttonColorSelection = 7;
                    }
                    if ((IsSingleKeyPress(Keys.D) || IsSingleKeyPress(Keys.Right)) && buttonColorSelection < 7)
                    {
                        clicked.Play();
                        buttonColorSelection++;
                    }
                    else if ((IsSingleKeyPress(Keys.D) || IsSingleKeyPress(Keys.Right)))
                    {
                        clicked.Play();
                        buttonColorSelection = 1;
                    }
                    switch (buttonColorSelection)
                    {
                        case 1:
                            foreach (Button button in buttons)
                            {
                                button.Color = boidColor;
                            }
                            break;
                        case 2:
                            foreach (Button button in buttons)
                            {
                                button.Color = Color.Green;
                            }
                            break;
                        case 3:
                            foreach (Button button in buttons)
                            {
                                button.Color = Color.Orange;
                            }
                            break;
                        case 4:
                            foreach (Button button in buttons)
                            {
                                button.Color = Color.Red;
                            }
                            break;
                        case 5:
                            foreach (Button button in buttons)
                            {
                                button.Color = Color.Magenta;
                            }
                            break;
                        case 6:
                            foreach (Button button in buttons)
                            {
                                button.Color = Color.Blue;
                            }
                            break;
                        case 7:
                            foreach (Button button in buttons)
                            {
                                button.Color = Color.White;
                            }
                            break;
                    }
                    break;
                case 6:
                    // Change Border Fade
                    if ((IsSingleKeyPress(Keys.A) || IsSingleKeyPress(Keys.Left)) && borderFadeSelection > 1)
                    {
                        clicked.Play();
                        borderFadeSelection--;
                    }
                    else if ((IsSingleKeyPress(Keys.A) || IsSingleKeyPress(Keys.Left)))
                    {
                        clicked.Play();
                        borderFadeSelection = 4;
                    }
                    if ((IsSingleKeyPress(Keys.D) || IsSingleKeyPress(Keys.Right)) && borderFadeSelection < 4)
                    {
                        clicked.Play();
                        borderFadeSelection++;
                    }
                    else if ((IsSingleKeyPress(Keys.D) || IsSingleKeyPress(Keys.Right)))
                    {
                        clicked.Play();
                        borderFadeSelection = 1;
                    }
                    // Apply Border Fade
                    switch (borderFadeSelection)
                    {
                        case 1:
                            fadeStart = Color.Lime;
                            fadeEnd = Color.Red;
                            break;
                        case 2:
                            fadeStart = Color.White;
                            fadeEnd = Color.Black;
                            break;
                        case 3:
                            fadeStart = Color.Blue;
                            fadeEnd = Color.OrangeRed;
                            break;
                        case 4:
                            fadeStart = Color.Orange;
                            fadeEnd = Color.Purple;
                            break;
                    }
                    break;
            }

            if (IsSingleKeyPress(Keys.M))
            {
                stateChange.Play();
                currentState = GameState.MainMenu;
            }
            SoundEffect.MasterVolume = soundVolume;
            MediaPlayer.Volume = musicVolume;
        }

        /// <summary>
        /// Processes the instructions menu
        /// </summary>
        private void ProcessInstructions(GameTime gameTime)
        {
            instructionsFlock.ProcessBoids(new Vector2[] 
            { new Vector2(predatorWASDArrows.ActualPosition.X,
            predatorWASDArrows.ActualPosition.Y) });

            predatorWASDArrows.Update(gameTime);

            //Return to Main menu
            if (IsSingleKeyPress(Keys.M))
            {
                stateChange.Play();
                currentState = GameState.MainMenu;
            }

            if (IsSingleKeyPress(Keys.NumPad1) || IsSingleKeyPress(Keys.D1))
            {
                choice = 1;
            }
            if (IsSingleKeyPress(Keys.NumPad2) || IsSingleKeyPress(Keys.D2))
            {
                choice = 2;
            }
            if (IsSingleKeyPress(Keys.NumPad3) || IsSingleKeyPress(Keys.D3))
            {
                choice = 3;
            }
        }

        /// <summary>
        /// Processes the Credits menu
        /// </summary>
        private void ProcessCredits()
        {
            //Returns to Main menu
            if (IsSingleKeyPress(Keys.M))
            {
                stateChange.Play();
                currentState = GameState.MainMenu;
            }
        }

        /// <summary>
        /// Processes the Versus Game
        /// </summary>
        /// <param name="gameTime"></param>
        private void ProcessVersus(GameTime gameTime)
        {
            // Process Versus Flock
            versusFlock.ProcessBoids(new Vector2[]
            { new Vector2(predatorWASD.ActualPosition.X,
            predatorWASD.ActualPosition.Y), new Vector2(predatorArrows.ActualPosition.X,
            predatorArrows.ActualPosition.Y) });

            // Update predators
            predatorArrows.Update(gameTime);
            predatorWASD.Update(gameTime);

            // Update timer
            versusTimer1 -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            versusTimer2 -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Return to Main menu
            if (IsSingleKeyPress(Keys.M))
            {
                stateChange.Play();
                currentState = GameState.MainMenu;
            }
        }


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
        /// </summary>
        /// <param name="pen"></param>
        public void Bashed(int pen)
        {
            // Bash boids in pen, and get the data from it
            long dataReturn;
            dataReturn = flock.Bashers.BashContainedBoids(flock, pen, scoreGoal);

            // Add score increment to player score
            player1Score += (ulong)dataReturn;

            // Add total to totalscoreincrementprints, as long as points were added
            if (dataReturn > 0)
            {
                totalScoreIncrementPrint.Add(dataReturn);
                totalScoreIncrementTimer.Add(2);
            }

            // Determine if score goal, timer, or both are being incremented
            // 1 - > up score goal
            // 2 - > timer goes up
            // 3 - > both go up
            // Anything else -> no effect
            if (flock.Bashers.UpScoreGoal == 1)
            {
                scoreGoal++;
            }
            else if (flock.Bashers.UpScoreGoal == 2)
            {
                // Add to timerincrementprints and timers
                totalTimeIncrementPrint.Add(2);
                totalTimeIncrementTimer.Add(2);
                timer += 2;
            }
            else if (flock.Bashers.UpScoreGoal == 3)
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
