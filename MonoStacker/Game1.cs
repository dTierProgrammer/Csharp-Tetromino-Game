using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoStacker.Source.Global;
using MonoStacker.Source.Scene;
using MonoStacker.Source.Scene.GameScenes;
using MonoStacker.Source.VisualEffects.ParticleSys;
using MonoStacker.Source.VisualEffects.ParticleSys.Emitter;
using MonoStacker.Source.VisualEffects.ParticleSys.Library.Source;
using MonoStacker.Source.VisualEffects.ParticleSys.Particle;

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
        

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            uGameTime = new GameTime();

#if DEBUG
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


            EmitterData testData = new()
            {
                emissionInterval = 1f,
                density = 100,
                angleVarianceMax = 360,
                particleActiveTime = (2, 2f),
                speed = (250f, 250f),
                particleData = new ParticleData()
                {
                    texture = GetContent.Load<Texture2D>("Image/Effect/Particle/default"),
                    colorTimeLine = (Color.Yellow, Color.Red),
                    scaleTimeLine = new(2, 1),
                    
                }

            };

            EmitterObj test = new(_testStaticSource, testData, EmissionType.Continuous);
            //ParticleManager.AddEmitter(test);
            
            /*
            _testStaticSources.Positions.Add(new(20, 20));
            _testStaticSources.Positions.Add(new(40, 40));
            _testStaticSources.Positions.Add(new(60, 60));
            _testStaticSources.Positions.Add(new(80, 80));

            EmitterData testData2 = new()
            {
                emissionInterval = 1f,
                density = 100,
                angleVarianceMax = 180,
                particleActiveTime = (.01f, 3f),
                speed = (50, 100),
                particleData = new ParticleData()
                {
                    texture = GetContent.Load<Texture2D>("Image/Effect/Particle/particle_3"),
                    colorTimeLine = (Color.Yellow, Color.Red),
                    scaleTimeLine = new(4, 1),
                    opacityTimeLine = new(1, 0)
                }

            };

            GroupEmitterObj test2 = new(_testStaticSources, testData2, EmissionType.Continuous);
            ParticleManager.AddEmitter(test2);
            */
            _testStaticSources.Members.Add(new GroupPartData 
            {
                Position = new(40, 40),
                Data = new EmitterData()
                {
                    emissionInterval = 1f,
                    density = 50,
                    angleVarianceMax = 180,
                    particleActiveTime = (.01f, 2f),
                    speed = (50, 100),
                    particleData = new ParticleData()
                    {
                        texture = GetContent.Load<Texture2D>("Image/Effect/Particle/particle_3"),
                        colorTimeLine = (Color.Magenta, Color.White),
                        scaleTimeLine = new(2, 1),
                        opacityTimeLine = new(1, 0)
                    }
                }
            });

            _testStaticSources.Members.Add(new GroupPartData 
            {
                Position = new(60, 60),
                Data = new EmitterData()
                {
                    emissionInterval = 1f,
                    density = 50,
                    angleVarianceMax = 180,
                    particleActiveTime = (.01f, 2f),
                    speed = (50, 100),
                    particleData = new ParticleData()
                    {
                        texture = GetContent.Load<Texture2D>("Image/Effect/Particle/particle_3"),
                        colorTimeLine = (Color.Yellow, Color.White),
                        scaleTimeLine = new(2, 1),
                        opacityTimeLine = new(1, 0)
                    }
                }
            });

            //GroupEmitterObj test2 = new(_testStaticSources, EmissionType.Continuous);
            //ParticleManager.AddEmitter(test2);

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
            ParticleManager.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(_scaledDisp);
            GraphicsDevice.Clear(Color.Black);
            _sceneManager.CurrentScene().Draw(_spriteBatch);
            _spriteBatch.Begin();
            ParticleManager.Draw(_spriteBatch);
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
