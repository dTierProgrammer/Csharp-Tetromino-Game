using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoStacker.Source.GameObj.Tetromino.Factory;
using MonoStacker.Source.GameObj.Tetromino.Randomizer;
using MonoStacker.Source.Global;
using MonoStacker.Source.Scene;
using MonoStacker.Source.Scene.GameMode;
using MonoStacker.Source.Scene.GameScenes;
using MonoStacker.Source.VisualEffects;
using MonoStacker.Source.VisualEffects.ParticleSys;
using MonoStacker.Source.VisualEffects.ParticleSys.Emitter;
using MonoStacker.Source.VisualEffects.ParticleSys.Library.Source;
using MonoStacker.Source.VisualEffects.ParticleSys.Particle;
using RasterFontLibrary.Source;
using System.Collections.Generic;
using System.Diagnostics;

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
        private SevenBagRandomizer sBag1;
        private SevenBagRandomizer sBag2;

        /*
        
        
        FUTURE:
        - Menu system
        - New (hopefully better) sounds
        - Music?
        - Battle Mode
        - Classic arcade modes (Sega Marathon, TGM1 Master)
        - Freeplay mode
        - JSON support (for saving scores, times)
        */

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            uGameTime = new GameTime();
            Window.Title = "Demo";

#if DEBUG
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.ApplyChanges();
            _scaledDisp = new RenderTarget2D(GraphicsDevice, (int)(GraphicsDevice.DisplayMode.Width / 4), (int)(GraphicsDevice.DisplayMode.Height / 4));
            _sceneManager = new SceneManager();

# else
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            //_graphics.IsFullScreen = true;
            _graphics.ApplyChanges();
            _scaledDisp = new RenderTarget2D(GraphicsDevice, GraphicsDevice.DisplayMode.Width / 4, GraphicsDevice.DisplayMode.Height / 4);
            _sceneManager = new SceneManager();

# endif

            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //sBag1 = new(100);
            //sBag2 = new(100);
            GetContent.Initialize(this);
            _testScene = new TestScene();

            _sceneManager.EnterScene(new MarathonMode());
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
            GetFps.Update(gameTime);
            uGameTime.ElapsedGameTime = gameTime.ElapsedGameTime;
            uGameTime.TotalGameTime = gameTime.TotalGameTime;
            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            

            // TODO: Add your update logic here
            _sceneManager.CurrentScene().Update(gameTime);
            AnimatedEffectManager.Update(gameTime);
            ParticleManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(_scaledDisp);
            GraphicsDevice.Clear(Color.Black);
            _sceneManager.CurrentScene().Draw(_spriteBatch);
            AnimatedEffectManager.Draw(_spriteBatch);
            ParticleManager.Draw(_spriteBatch);
            _spriteBatch.Begin();
            Font.DefaultSmallOutlineGradient.RenderString(_spriteBatch, new Vector2(479, 0), $"{GetFps.fps:F0}", Color.Yellow, OriginSetting.TopRight);
            //_spriteBatch.Draw(GetContent.Load<Texture2D>("Image/Effect/lockFlashEffect"), new Rectangle(10,10, 256, 256), Color.Red);
            _spriteBatch.End();
            _sceneManager.CurrentScene().DrawText(_spriteBatch);


            // TODO: Add your drawing code here
            GraphicsDevice.SetRenderTarget(null);
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(_scaledDisp, new Rectangle(0, 0, GraphicsDevice.DisplayMode.Width, GraphicsDevice.DisplayMode.Height), Color.White);
            _spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}
