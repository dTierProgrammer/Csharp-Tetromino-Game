using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoStacker.Source.VisualEffects.ParticleSys.Particle;

public enum ParticleOriginSetting
{
    BottomLeft = 1,
    Bottom = 2,
    BottomRight = 3,
    CenterLeft = 4,
    Center = 5,
    CenterRight = 6,
    TopLeft = 7,
    Top = 8,
    TopRight = 9,
}

public class ParticleObj : AnimatedEffect
{ // the particle itself
    private readonly ParticleData _data; // given data
    private Vector2 _position;
    public float activeTimeLeft { get; private set; } // how long particle is displayed
    private float _activeTimeAmount; // percentage of active time left
    private Color _color;
    private float _opacity;
    private float _scale; // size/scale of particle
    private Vector2 _origin; // origin point of particle
    private Vector2 _direction; // movement direction of particle
    private float _startingOrientation = 0; // initial orientation of particle (before rotation is applied)

    public ParticleObj(Vector2 position, ParticleData data, ParticleOriginSetting originSetting)
    {
        _data = data;
        activeTimeLeft = _data.activeTime;
        _activeTimeAmount = 1f;
        _position = position;
        _color = _data.colorTimeLine.color1;
        _opacity = _data.opacityTimeLine.X;


        if (data.speed != 0)
        {
            _data.angle = MathHelper.ToRadians(_data.angle);
            _direction = new((float)Math.Sin(_data.angle), (float)-Math.Cos(_data.angle));
        }
        else
            _direction = Vector2.Zero;

        switch (originSetting) // set origin of particle
        {
            case (ParticleOriginSetting)1: _origin = new(0, _data.texture.Height); break;
            case (ParticleOriginSetting)2: _origin = new(_data.texture.Width / 2, _data.texture.Height); break;
            case (ParticleOriginSetting)3: _origin = new(_data.texture.Width, _data.texture.Height); break;
            case (ParticleOriginSetting)4: _origin = new(0, _data.texture.Height / 2); break;
            case (ParticleOriginSetting)5: _origin = new(_data.texture.Width / 2, _data.texture.Height / 2); break;
            case (ParticleOriginSetting)6: _origin = new(_data.texture.Width, _data.texture.Height / 2); break;
            case (ParticleOriginSetting)7: _origin = new(0, 0); break;
            case (ParticleOriginSetting)8: _origin = new(_data.texture.Width / 2, 0); break;
            case (ParticleOriginSetting)9: _origin = new(_data.texture.Width, 0);break;
        }
    }
    
    public ParticleObj(Vector2 position, ParticleData data)
    {
        _data = data;
        activeTimeLeft = _data.activeTime;
        _activeTimeAmount = 1f;
        _position = position;
        _color = _data.colorTimeLine.color1;
        _opacity = _data.opacityTimeLine.X;

        if (data.speed != 0)
        {
            _data.angle = MathHelper.ToRadians(_data.angle);
            _direction = new((float)Math.Sin(_data.angle), (float)-Math.Cos(_data.angle));
        }
        else
            _direction = Vector2.Zero;

        var originSetting = ParticleOriginSetting.Center;
        switch (originSetting) // set origin of particle
        {
            case (ParticleOriginSetting)1: _origin = new(0, _data.texture.Height); break;
            case (ParticleOriginSetting)2: _origin = new(_data.texture.Width / 2, _data.texture.Height); break;
            case (ParticleOriginSetting)3: _origin = new(_data.texture.Width, _data.texture.Height); break;
            case (ParticleOriginSetting)4: _origin = new(0, _data.texture.Height / 2); break;
            case (ParticleOriginSetting)5: _origin = new(_data.texture.Width / 2, _data.texture.Height / 2); break;
            case (ParticleOriginSetting)6: _origin = new(_data.texture.Width, _data.texture.Height / 2); break;
            case (ParticleOriginSetting)7: _origin = new(0, 0); break;
            case (ParticleOriginSetting)8: _origin = new(_data.texture.Width / 2, 0); break;
            case (ParticleOriginSetting)9: _origin = new(_data.texture.Width, 0);break;
        }
    }

    public void Update(GameTime gameTime)
    {
        activeTimeLeft -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        _activeTimeAmount = MathHelper.Clamp(activeTimeLeft / _data.activeTime, 0, 1);
        _color = Color.Lerp(_data.colorTimeLine.color2, _data.colorTimeLine.color1, _activeTimeAmount);
        _opacity = MathHelper.Clamp(MathHelper.Lerp(_data.opacityTimeLine.Y, _data.opacityTimeLine.X, _activeTimeAmount), 0, 1);
        _scale = MathHelper.Lerp(_data.scaleTimeLine.Y, _data.scaleTimeLine.X, _activeTimeAmount) / _data.texture.Width;
        _position += _direction * _data.speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        _startingOrientation += _data.rotationSpeed;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw
            (
                _data.texture,
                _position,
                null,
                _color * _opacity,
                _startingOrientation,
                _origin,
                _scale,
                SpriteEffects.None,
                1f
            );
    }
}