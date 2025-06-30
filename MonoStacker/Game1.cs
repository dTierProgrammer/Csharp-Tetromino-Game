using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoStacker.Source.Global;
using MonoStacker.Source.Scene;
using MonoStacker.Source.Scene.GameScenes;

namespace MonoStacker
{
    public class Game1 : Game
    { // this is the real deal I promise
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private readonly RenderTarget2D _scaledDisp;
        private readonly SceneManager _sceneManager;
        private TestScene _testScene;
        public static GameTime uGameTime;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            uGameTime = new GameTime();

            # if DEBUG
            _graphics.PreferredBackBufferWidth = 1712;
            _graphics.PreferredBackBufferHeight = 960;
            _graphics.ApplyChanges();

# else
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.ApplyChanges();

# endif

            _scaledDisp = new RenderTarget2D(GraphicsDevice, GraphicsDevice.DisplayMode.Width / 4, GraphicsDevice.DisplayMode.Height / 4);
            _sceneManager = new SceneManager();

            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            GetContent.Initialize(this);
            _testScene = new TestScene();

            _sceneManager.EnterScene(_testScene);
            // init any custom classes above base method call
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: use this.Content to load your game content here
            
        }

        protected override void Update(GameTime gameTime)
        {
            uGameTime.ElapsedGameTime = gameTime.ElapsedGameTime;
            uGameTime.TotalGameTime = gameTime.TotalGameTime;
            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            _sceneManager.CurrentScene().Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(_scaledDisp);
            GraphicsDevice.Clear(Color.Black);
            _sceneManager.CurrentScene().Draw(_spriteBatch);
            _spriteBatch.Begin();
            _spriteBatch.End();
  

            // TODO: Add your drawing code here
            GraphicsDevice.SetRenderTarget(null);
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(_scaledDisp, new Rectangle(0, 0, GraphicsDevice.DisplayMode.Width, GraphicsDevice.DisplayMode.Height), Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
