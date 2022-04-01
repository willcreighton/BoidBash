using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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

        // Temp
        private Texture2D blank;

        public int Width
        {
            get { return width; }
        }
        public int Height
        {
            get { return height; }
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
            flock = new Flock(30, new Rectangle(200, 200, 800, 500), new Rectangle(300, 300, 700, 400), boidSprite, new Vector2(5, 7), boidColor,
                _spriteBatch);

            predTexture = Content.Load<Texture2D>("PredatorSprite");



            predator = new Predator(predTexture, new Rectangle(width / 2, height / 2,
                25, 25),
                windowHeight, windowWidth, 25, 25);

            mainMenuUI = new MainMenuUI(windowWidth, windowHeight, headerFont, primaryFont);
            gameUI = new GameUI(windowWidth, windowHeight, headerFont, primaryFont);
            pauseMenuUI = new PauseMenuUI(windowWidth, windowHeight, headerFont, primaryFont);
            endScreenUI = new EndScreenUI(windowWidth, windowHeight, headerFont, primaryFont);
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
                    flock.ProcessBoids(new Vector2(0, 0));
                    predator.Update(gameTime);
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

            // Begin the Sprite Batch
            _spriteBatch.Begin();

            // GameState switches
            switch (currentState)
            {
                case GameState.MainMenu:
                    mainMenuUI.Draw(_spriteBatch);
                    break;
                case GameState.Game:
                    _spriteBatch.Draw(blank, new Rectangle(200, 200, 800, 500), Color.White);
                    gameUI.Draw(_spriteBatch);
                    flock.Draw();
                    predator.Draw(_spriteBatch);
                    break;
                case GameState.PauseMenu:
                    pauseMenuUI.Draw(_spriteBatch);
                    break;
                case GameState.EndScreen:
                    endScreenUI.Draw(_spriteBatch);
                    break;
                default:
                    break;
            }

            // End the Sprite Batch
            _spriteBatch.End();

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
    }
}
