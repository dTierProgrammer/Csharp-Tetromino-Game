using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoStacker.Source.Data;
using MonoStacker.Source.GameObj;
using MonoStacker.Source.GameObj.Tetromino.Randomizer;
using MonoStacker.Source.Generic;
using MonoStacker.Source.Generic.GarbageSystem.Factory;
using MonoStacker.Source.Generic.Rotation;
using MonoStacker.Source.Global;
using MonoStacker.Source.Interface.Input;
using MonoStacker.Source.VisualEffects.ParticleSys.Emitter;
using MonoStacker.Source.VisualEffects.ParticleSys.Library.Source;
using MonoStacker.Source.VisualEffects.ParticleSys.Particle;
using RasterFontLibrary.Source;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Gaming.Input;
using Windows.Media.Protection.PlayReady;

namespace MonoStacker.Source.Scene.GameMode
{
    public class Battle2p: IScene
    {
        protected Texture2D bg;
        private int seed;
        private List<PlayField> players = new();
        private List<InputDevice> _inputDevices;
        private List<Vector2> posits;
        private StandardGarbageGenerator garbageGen = new();
        protected StaticEmissionSource _streakFireSource;
        protected EmitterObj _streakFireEmitter;
        protected EmitterData _streakFire;
        protected ParticleLayer _particleLayer = new();

        private void GetDevices() 
        {
            var id = 0;
            bool kbConnected = false ;
            ManagementObjectSearcher kbSearch = new("SELECT * FROM Win32_Keyboard");
            foreach (var device in kbSearch.Get())
            { 
                Debug.WriteLine($"Device Name: {device}");
                kbConnected = true;
            }
            if (kbSearch.Get().Count > 0) 
            {
                id++;
                _inputDevices.Add(InputDevice.Keyboard);
            }

            for (var i = 0; i < 4; i++) 
            {
                Debug.WriteLine($"Index{i} GamePad Is Connected: {GamePad.GetState((PlayerIndex) i).IsConnected}");
                if (GamePad.GetState(i).IsConnected)
                { _inputDevices.Add(InputDevice.Gamepad); id++; }
            }
            
        }
        private int ComboMultiplier(int combo) 
        {
            return combo switch
            {
                -1 => 0,
                0 => 0,
                1 => 0,
                2 => 1,
                3 => 1,
                4 => 2,
                5 => 2,
                6 => 3,
                7 => 3,
                8 => 4,
                9 => 4,
                10 => 4,
                11 => 5,
                12 => 5,
                13 => 5,
                14 => 6,
                15 => 6,
                16 => 6,
                17 => 7,
                18 => 8,
                _ => combo
            };
        }

        private void P1Attack() 
        {
            players[0].SendAttack(players[1].garbageMeter);
        }

        private void P2Attack()
        {
            players[1].SendAttack(players[0].garbageMeter);
        }

        private void InitPlayers() 
        {
            for (var p = 0; p < 2; p++) 
            {
                if (p < _inputDevices.Count)
                {
                    PlayerIndex? index = p switch 
                    {
                        0 => null,
                        1 => PlayerIndex.One,
                        2 => PlayerIndex.Two,
                        3 => PlayerIndex.Three,
                        4 => PlayerIndex.Four,
                        _ => null
                    };
                    players.Add(new PlayField
                    (
                        posits[p],
                        new SevenBagRandomizer(seed),
                        PlayFieldPresets.Battle,
                        _inputDevices[p], _inputDevices[p] is not (InputDevice.Keyboard) ? index : null,
                        new InputBinds())
                    );
                    Debug.WriteLine($"Player: {(PlayerIndex)p}");
                }
            }
            foreach (var player in players) 
            {
                player.ConstructAttackSys();
                player.ConstructGarbageSys();
            }

            Debug.WriteLine($"{players.Count}");
        }

        public void Initialize() 
        {
            posits = new();
            posits.Add(new(240 - 93, 135));
            posits.Add(new(240 + 93, 135));
            _inputDevices = new();
            GetDevices();
            seed = ExtendedMath.Rng.Next();
            players = new();
            InitPlayers();
            players[0].EmptyPiecePlacement += P1Attack;
            players[1].EmptyPiecePlacement += P2Attack;

            foreach (var item in players)
                item.Start();


            _streakFireSource = new(new(10, 10));
            _streakFire = new EmitterData()
            {
                particleData = new ParticleData
                {
                    texture = GetContent.Load<Texture2D>("Image/Effect/Particle/ball"),
                    angle = 330,
                    opacityTimeLine = new(1f, 0f),
                    scaleTimeLine = new(18, 0),
                    colorTimeLine = (Color.Cyan, Color.Blue),
                    rotationSpeed = .05f
                },
                angleVarianceMax = 3,
                particleActiveTime = (1, 3),
                emissionInterval = .01f,
                speed = (0, 0),
                density = 1,
                //rotationSpeed = (-.03f, .03f)
            };
            _streakFireEmitter = new EmitterObj(_streakFireSource, _streakFire, EmissionType.Continuous, true);
            ParticleManager.AddEmitter(_streakFireEmitter);


            Debug.WriteLine($"Battle2p | {TimeSpan.FromSeconds(Game1.uGameTime.TotalGameTime.TotalSeconds).ToString(@"mm\:ss\.ff")} | Initialization success, seed: {seed}.");
        }
        public void Load() 
        {
            bg = GetContent.Load<Texture2D>("Image/Background/bg_1080");
        }

        public void Update(GameTime gameTime) 
        {
            foreach (var item in players)
                item.Update(gameTime);

            _streakFireSource.Position += Vector2.One * 2;
        }

        public void Draw(SpriteBatch spriteBatch) 
        {
            spriteBatch.Begin();
            Font.DefaultSmallOutlineGradient.RenderString(spriteBatch, new Vector2(posits[0].X - 47, posits[0].Y - 85), players[0].garbageQueued.ToString(), Color.Red, OriginSetting.Bottom);
            Font.DefaultSmallOutlineGradient.RenderString(spriteBatch, new Vector2(posits[1].X - 47, posits[1].Y - 85), players[1].garbageQueued.ToString(), Color.Red, OriginSetting.Bottom);
            spriteBatch.Draw(bg, new Vector2(0, 0), Color.White);
            //Debug.WriteLine("everything else");
            spriteBatch.End();
            foreach (var item in players)
                item.Draw(spriteBatch);

            
        }
        public void DrawText(SpriteBatch spriteBatch) { }
    }
}
