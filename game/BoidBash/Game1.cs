using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BoidBash
{
    // This enumerator references the different states of the game
    enum GameState
    {
        MainMenu,       // 'M'
        Game,           // 'G'
        OptionsMenu,    // 'O'
        PauseMenu,      // 'P'
        EndScreen       // 'E' (will add later)
    }

    // This enumerator references the different colors of the game
    enum GameColor
    {
        // TODO: Implement
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GameState currentState;

        // Screen size
        private int screenWidth;
        private int screenHeight;

        // Keyboard states
        private KeyboardState keyboardState;
        private KeyboardState lastKeyboardState;
        
        // Fonts
        SpriteFont primaryFont;

        // Colors
        Color backgroundColor = new Color(20, 20, 20);

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Adjust the window size
            screenWidth = 800;
            screenHeight = 800;
            _graphics.PreferredBackBufferWidth = screenWidth;
            _graphics.PreferredBackBufferHeight = screenHeight;
            _graphics.ApplyChanges();

            currentState = GameState.MainMenu;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            primaryFont = Content.Load<SpriteFont>("PrimaryFont");
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
                    break;
                case GameState.OptionsMenu:
                    ProcessOptionsMenu();
                    break;
                case GameState.PauseMenu:
                    ProcessPauseMenu();
                    break;
                case GameState.EndScreen:
                    // TODO: Implement
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

            switch (currentState)
            {
                case GameState.MainMenu:
                    // Options Menu prompt
                    _spriteBatch.DrawString(
                        primaryFont,
                        "Press 'O' to access the Options Menu.",
                        new Vector2(20, screenHeight - 40),
                        Color.White
                        );

                    // Game prompt
                    _spriteBatch.DrawString(
                        primaryFont,
                        "Press 'G' to access the Game.",
                        new Vector2(20, screenHeight - 80),
                        Color.White
                        );
                    break;
                case GameState.Game:
                    // Main Menu prompt
                    _spriteBatch.DrawString(
                        primaryFont,
                        "Press 'M' to access the Main Menu.",
                        new Vector2(20, screenHeight - 40),
                        Color.White
                        );

                    // Pause Menu prompt
                    _spriteBatch.DrawString(
                        primaryFont,
                        "Press 'P' to access the Pause Menu.",
                        new Vector2(20, screenHeight - 80),
                        Color.White
                        );
                    break;
                case GameState.OptionsMenu:
                    // Main Menu prompt
                    _spriteBatch.DrawString(
                        primaryFont,
                        "Press 'M' to return to the Main Menu.",
                        new Vector2(20, screenHeight - 40),
                        Color.White
                        );
                    break;
                case GameState.PauseMenu:
                    // Main Menu Prompt
                    _spriteBatch.DrawString(
                        primaryFont,
                        "Press 'M' to return to the Main Menu.",
                        new Vector2(20, screenHeight - 40),
                        Color.White
                        );

                    // Game prompt
                    _spriteBatch.DrawString(
                        primaryFont,
                        "Press 'G' to return to the Game.",
                        new Vector2(20, screenHeight - 80),
                        Color.White
                        );
                    break;
                case GameState.EndScreen:
                    // TODO: Implement
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
            }
            else if (IsSingleKeyPress(Keys.O))
            {
                currentState = GameState.OptionsMenu;
            }
        }

        /// <summary>
        /// This method processes the Game state
        /// </summary>
        private void ProcessGame()
        {
            if (IsSingleKeyPress(Keys.M))
            {
                currentState = GameState.MainMenu;
            }
            else if (IsSingleKeyPress(Keys.P))
            {
                currentState = GameState.PauseMenu;
            }
        }

        /// <summary>
        /// This method processes the Options Menu state
        /// </summary>
        private void ProcessOptionsMenu()
        {
            if (IsSingleKeyPress(Keys.M))
            {
                currentState = GameState.MainMenu;
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
            else if (IsSingleKeyPress(Keys.P))
            {
                currentState = GameState.PauseMenu;
            }
        }
    }
}
