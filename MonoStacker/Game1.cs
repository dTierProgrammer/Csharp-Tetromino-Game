using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoStacker.Source.GameObj;
using MonoStacker.Source.GameObj.Tetromino;
using MonoStacker.Source.Generic;
using MonoStacker.Source.Global;

namespace MonoStacker
{
    public class Game1 : Game
    { // this is the real deal i promise
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private RenderTarget2D scaledDisp;

        private Texture2D test;

        private Grid testGrid;
        private PlayField playField;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            _graphics.PreferredBackBufferWidth = 1536;
            _graphics.PreferredBackBufferHeight = 864;
            _graphics.ApplyChanges();

            scaledDisp = new RenderTarget2D(GraphicsDevice, GraphicsDevice.DisplayMode.Width / 4, GraphicsDevice.DisplayMode.Height / 4);

            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            GetContent.Initialize(this);

            // init any custom classes above base method call
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: use this.Content to load your game content here

            playField = new PlayField(new Vector2(150, 35));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            playField.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(scaledDisp);
            GraphicsDevice.Clear(Color.Black);
            playField.Draw(_spriteBatch);

            // TODO: Add your drawing code here
            GraphicsDevice.SetRenderTarget(null);
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(scaledDisp, new Rectangle(0, 0, GraphicsDevice.DisplayMode.Width, GraphicsDevice.DisplayMode.Height), Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
