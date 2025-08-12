using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoStacker.Source.Generic;
using MonoStacker.Source.Global;
using MonoStacker.Source.VisualEffects.ParticleSys;
using MonoStacker.Source.VisualEffects.ParticleSys.Emitter;
using MonoStacker.Source.VisualEffects.ParticleSys.Library.Source;
using MonoStacker.Source.VisualEffects.ParticleSys.Particle;

namespace MonoStacker.Source.VisualEffects;

public static class PlayfieldEffects
{
    public static void FlashPiece(Piece piece, Color color, float timeDislplayed, Vector2 distortFactor, Vector2 pos)
    {
        for (int y = 0; y < piece.currentRotation.GetLength(0); y++)
        {
            for (int x = 0; x < piece.currentRotation.GetLength(1); x++)
            {
                if (piece.currentRotation[y, x] != 0)
                {
                    AnimatedEffectManager.AddEffect(new LockFlash(new Vector2((x * 8) + ((int)piece.offsetX * 8) + (int)pos.X, (y * 8) + ((int)piece.offsetY * 8) + (int)pos.Y - 160), color, timeDislplayed, distortFactor));
                }
            }
        }
    } // move to "effect manager" class
    
    public static void FlashPiece(Piece piece, Color color, float timeDislplayed, Vector2 pos) 
    {
        for (int y = 0; y < piece.currentRotation.GetLength(0); y++) 
        {
            for (int x = 0; x < piece.currentRotation.GetLength(1); x++) 
            {
                if (piece.currentRotation[y, x] != 0) 
                {
                    AnimatedEffectManager.AddEffect(new LockFlash(new Vector2((x * 8) + ((int)piece.offsetX * 8) + (int)pos.X, (y * 8) + ((int)piece.offsetY * 8) + (int)pos.Y - 160), color, timeDislplayed));
                }
            }
        }
    } // move to "effect manager" class

    public static void FlashPiece(Piece piece, Color color, float timeDislplayed, Vector2 pos, AnimatedEffectLayer layer)
    {
        for (int y = 0; y < piece.currentRotation.GetLength(0); y++)
        {
            for (int x = 0; x < piece.currentRotation.GetLength(1); x++)
            {
                if (piece.currentRotation[y, x] != 0)
                {
                    layer.AddEffect(new LockFlash(new Vector2((x * 8) + ((int)piece.offsetX * 8) + (int)pos.X, (y * 8) + ((int)piece.offsetY * 8) + (int)pos.Y - 160), color, timeDislplayed));
                }
            }
        }
    } // move to "effect manager" class

    public static void LineClearFlash(Color color, float timeDisplayed, Grid grid, Vector2 pos) 
    {
        for (int y = 0; y < 40; y++)
        {
            if (grid.rowsToClear.Contains(y))
            {
                AnimatedEffectManager.AddEffect(new ClearFlash(new Vector2(39 + pos.X, (int)(y * 8) + pos.Y - 155.5f), color, timeDisplayed, new Vector2(3, 0)));
            }
        }
    } // move to "effect manager" class

    public static void LineClearEffect(Grid grid, Vector2 pos)
    {
        StaticEmissionSources sources = new(new());
        for (int y = 0; y < 40; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                if (grid.rowsToClear.Contains(y))
                {
                    Color color = Color.White;
                    switch (grid._matrix[y][x])
                    {
                        case 1: color = Color.Cyan; break;
                        case 2: color = Color.Blue; break;
                        case 3: color = Color.Orange; break;
                        case 4: color = Color.Yellow; break;
                        case 5: color = new Color(0, 255, 0); break;
                        case 6: color = Color.Magenta; break;
                        case 7: color = Color.Red; break;
                    }

                    sources.Members.Add(new GroupPartData()
                    {
                        Position = new Vector2((x * 8) + pos.X, (y * 8) + (pos.Y - 160)),
                        Data = new EmitterData
                        {
                            emissionInterval = 1f,
                            density = 20,
                            angleVarianceMax = 180,
                            particleActiveTime = (.01f, .3f),
                            speed = (50, 100),
                            particleData = new ParticleData()
                            {
                                texture = GetContent.Load<Texture2D>("Image/Effect/Particle/default"),
                                colorTimeLine = (color, color),
                                scaleTimeLine = new(3, 1),
                                opacityTimeLine = new(1, 1)
                            }
                        }
                    });
                }
            }
        }
        GroupEmitterObj clear = new(sources, EmissionType.Burst);
        ParticleManager.AddEmitter(clear);
    }
}