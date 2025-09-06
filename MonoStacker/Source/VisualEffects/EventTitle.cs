using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoStacker.Source.Global;
using MonoStacker.Source.VisualEffects.ParticleSys.Emitter;
using MonoStacker.Source.VisualEffects.ParticleSys.Library.Source;
using MonoStacker.Source.VisualEffects.ParticleSys.Particle;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoStacker.Source.VisualEffects
{
    enum EventTitleState
    {
        Entering,
        Active,
        Exiting,
        Inactive
    }
    public class EventTitle: AnimatedEffect
    {
        private EventTitleState _currentState;
        private Vector2 _entranceDistortion;
        private (float timer, float timerMax) _entranceTime;
        private float _entranceTimeAmount;
        private (float timer, float timerMax) _activeTime;
        private float _activeTimeAmount;
        private Vector2 _exitDistortion;
        private float _exitTimeAmount;
        private Color _tint = Color.White;
        private Rectangle _drawRect;
        private Vector2 _currentScale;
        private float _opacity = 0;
        private StaticEmissionSource emissionSource ;
        private ParticleData particleData;
        private EmitterData emitterData;
        private EmitterObj emitter;
        bool emit;
        private float rotation;

        public event Action IsActive;

        public EventTitle(Texture2D texture, Vector2 position, Vector2 entranceDistortion, float entranceTime, float activeTime, Vector2 exitDistortion, float exitTime) : base (position)
        {
            _currentState = EventTitleState.Entering;
            image = texture;
            _drawRect = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            _currentScale = entranceDistortion;
            _entranceDistortion = entranceDistortion;
            _entranceTime = (entranceTime, entranceTime);
            _activeTime = (activeTime - entranceTime, activeTime - entranceTime);
            TimeDisplayed = exitTime;
            MaxTimeDisplayed = exitTime;
            _exitDistortion = exitDistortion;
            emissionSource = new (position);
            particleData = new()
            {
                texture = GetContent.Load<Texture2D>("Image/Effect/starLarge"),
                colorTimeLine = (Color.Yellow, Color.Orange),
                //rotationSpeed = .03f,
                scaleTimeLine = new Vector2(8, 0)
            };
            emitterData = new()
            {
                particleData = particleData,
                activeTimeLeft = activeTime,
                angleVarianceMax = 180,
                particleActiveTime = (1, 2),
                speed = (10, 50),
                density = 40,
                offsetX = (-image.Width / 2 , image.Width / 2),
                offsetY = (-image.Height / 2, image.Height / 2),
                rotationSpeed = (-.05f, .05f)
            };
            emitter = new(emissionSource, emitterData, EmissionType.Burst);
            emit = true;
            //rotation = 5;
        }

        public EventTitle(Texture2D texture, Vector2 position, Vector2 entranceDistortion, float entranceTime, float activeTime, Vector2 exitDistortion, float exitTime, Color color, Color particleColor1, Color particleColor2, bool emit) : base(position)
        {
            _currentState = EventTitleState.Entering;
            image = texture;
            _drawRect = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            _currentScale = entranceDistortion;
            _entranceDistortion = entranceDistortion;
            _entranceTime = (entranceTime, entranceTime);
            _activeTime = (activeTime - entranceTime, activeTime - entranceTime);
            TimeDisplayed = exitTime;
            MaxTimeDisplayed = exitTime;
            _exitDistortion = exitDistortion;
            emissionSource = new(position);
            particleData = new()
            {
                texture = GetContent.Load<Texture2D>("Image/Effect/starLarge"),
                colorTimeLine = (particleColor1, particleColor2),
                //rotationSpeed = .03f,
                scaleTimeLine = new Vector2(8, 0)
            };
            emitterData = new()
            {
                particleData = particleData,
                activeTimeLeft = activeTime,
                angleVarianceMax = 180,
                particleActiveTime = (1, 2),
                speed = (10, 50),
                density = 40,
                offsetX = (-image.Width / 2, image.Width / 2),
                offsetY = (-image.Height / 2, image.Height / 2),
                rotationSpeed = (-.05f, .05f)
            };
            emitter = new(emissionSource, emitterData, EmissionType.Burst);
            _tint = color;
            this.emit = emit;
            //rotation = 5;
            rotation = 0;
        }

        public override void Update(float deltaTime) 
        {
            switch (_currentState) 
            {
                case EventTitleState.Entering:
                    if (_entranceTime.timer <= 0) { _currentState = EventTitleState.Active; }
                    _entranceTime.timer -= deltaTime;
                    _entranceTimeAmount = MathHelper.Clamp(_entranceTime.timer / _entranceTime.timerMax, 0, 1);
                    _opacity = MathHelper.Lerp(1, 0, _entranceTimeAmount);
                    _currentScale.X = MathHelper.Lerp(image.Width, _entranceDistortion.X, _entranceTimeAmount * _entranceTimeAmount);
                    _currentScale.Y = MathHelper.Lerp(image.Height, _entranceDistortion.Y, _entranceTimeAmount * _entranceTimeAmount);
                    break;
                case EventTitleState.Active:
                    if (_activeTime.timer <= 0) { _currentState = EventTitleState.Exiting; }
                    else if(_activeTime.timer == _activeTime.timerMax){ if(emit) ParticleManager.AddEmitter(emitter); }
                    _activeTime.timer -= deltaTime;
                    _opacity = 1;
                    break;
                case EventTitleState.Exiting:
                    if (TimeDisplayed <= 0) { _currentState = EventTitleState.Inactive; }
                    TimeDisplayed -= deltaTime;
                    _exitTimeAmount = MathHelper.Clamp(TimeDisplayed / MaxTimeDisplayed, 0, 1);
                    _opacity = MathHelper.Lerp(0, 1, _exitTimeAmount);
                    _currentScale.X = MathHelper.Lerp(_exitDistortion.X, image.Width, 1 - (1 - _exitTimeAmount) * (1 - _exitTimeAmount));
                    _currentScale.Y = MathHelper.Lerp(_exitDistortion.Y, image.Height, 1 - (1 - _exitTimeAmount) * (1 - _exitTimeAmount));


                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch) 
        {
            if (_currentState is EventTitleState.Inactive) return;
            spriteBatch.Draw
                (
                    image,
                    new Rectangle((int)position.X, (int)position.Y, (int)_currentScale.X, (int)_currentScale.Y),
                    null,
                    _tint * _opacity,
                    rotation,
                    new Vector2(image.Width / 2, image.Height / 2),
                    SpriteEffects.None,
                    0
                );
        }
    }
}
