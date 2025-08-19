using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        private StaticEmissionSource _testStaticSource = new(new(50, 50));
        private StaticEmissionSources _testStaticSources = new(new List<GroupPartData>());
        private RasterFont _rasterFont;
        
        /*
        TODO:
        - Revise piece spin detection
        - Text rendering system (for action text)
        - Garbage system
            - basic line garbage line addition system (o)
        - Revise effects
            - drop particle effect (o)
            - piece hit stack effect (x)

        TEST:
        - Seperating sub grid movement from physical piece location
        
        FUTURE:
        - Menu system
        - New (hopefully better) sounds
        - Music?
        - Guideline compliant modes (Marathon (150l/Endless), 40l sprint, Ultra)
        - Classic arcade modes (Sega Marathon, TGM1 Master)
        - Freeplay mode
        - Better customizeable ARR
        - JSON support (for saving scores, times)
        */

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            uGameTime = new GameTime();

#if DEBUG
            _graphics.PreferredBackBufferWidth = 960;
            _graphics.PreferredBackBufferHeight = 540;
            _graphics.ApplyChanges();
            _scaledDisp = new RenderTarget2D(GraphicsDevice, (int)(GraphicsDevice.DisplayMode.Width / 2), (int)(GraphicsDevice.DisplayMode.Height / 2));
            _sceneManager = new SceneManager();

# else
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.ApplyChanges();
            _scaledDisp = new RenderTarget2D(GraphicsDevice, GraphicsDevice.DisplayMode.Width / 4, GraphicsDevice.DisplayMode.Height / 4);
            _sceneManager = new SceneManager();

# endif

            

            IsMouseVisible = true;
        }   

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            GetContent.Initialize(this);
            _testScene = new TestScene();

            _sceneManager.EnterScene(new TestScene());
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



            // TODO: Add your drawing code here
            GraphicsDevice.SetRenderTarget(null);
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(_scaledDisp, new Rectangle(0, 0, GraphicsDevice.DisplayMode.Width, GraphicsDevice.DisplayMode.Height), Color.White);
            _spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}
