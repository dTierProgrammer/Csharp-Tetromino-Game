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
        for (int y = 0; y < Grid.ROWS; y++)
        {
            if (grid.rowsToClear.Contains(y))
            {
                AnimatedEffectManager.AddEffect(new ClearFlash(new Vector2(39 + pos.X, (int)(y * 8) + pos.Y - 155.5f), color, timeDisplayed, new Vector2(3, 0)));
            }
        }
    } // move to "effect manager" class

    public static void BoardExplosion(Grid grid, Vector2 pos) 
    {
        StaticEmissionSources sources = new([]);
        for (var y = 0; y < Grid.ROWS; y++)
        {
            for (var x = 0; x < Grid.COLUMNS; x++)
            {
                if (grid._matrix[y][x] == 0) continue;
                sources.Members.Add(new GroupPartData()
                {
                    Position = new Vector2((x * 8) + pos.X + 4, (y * 8) + (pos.Y - 160)),
                    Data = new EmitterData
                    {
                        density = 1,
                        angleVarianceMax = 180,
                        speed = (30, 60),
                        rotationSpeed = (-.05f, .05f),
                        particleActiveTime = (3, 3),

                        particleData = new ParticleData()
                        {
                            texture = ImgBank.BlockTexture,
                            textureSourceRect = grid.imageTiles[grid._matrix[y][x] - 1],
                            colorTimeLine = (Color.White, Color.White),
                            opacityTimeLine = new(1, 1),
                            frictionFactor = new Vector2(0, .003f),
                            scaleTimeLine = new(64, 64),
                            originOverride = new(4, 4)
                        }
                    }
                });
            }
        }
        GroupEmitterObj clear = new(sources, EmissionType.Burst);
        ParticleManager.AddEmitter(clear);
    }

    public static void LineClearAltEffect(Grid grid, Vector2 pos)
    {
        StaticEmissionSources sources = new([]);
        for (var y = 0; y < Grid.ROWS; y++) 
        {
            if (grid.rowsToClear.Contains(y)) 
            {
                for (var x = 0; x < Grid.COLUMNS; x++)
                {
                    sources.Members.Add(new GroupPartData()
                    {
                        Position = new Vector2((x * 8) + pos.X + 4, (y * 8) + (pos.Y - 160)),
                        Data = new EmitterData
                        {
                            density = 1,
                            angleVarianceMax = 50,
                            speed = (30, 60),
                            rotationSpeed = (-.05f, .05f),
                            particleActiveTime = (.5f, 1),
                            
                            particleData = new ParticleData()
                            {
                                texture = ImgBank.BlockTexture,
                                textureSourceRect = grid.imageTiles[8],
                                colorTimeLine = (Color.White, Color.White),
                                opacityTimeLine = new(.4f, 0),
                                frictionFactor = new Vector2(0, .003f),
                                scaleTimeLine = new(64, 64),
                                originOverride = new(4, 4)
                            }
                        }
                    });
                }
            
            }
        }
        GroupEmitterObj clear = new(sources, EmissionType.Burst);
        ParticleManager.AddEmitter(clear);
    }

    public static void LineClearEffect(Grid grid, Vector2 pos)
    {
        StaticEmissionSources sources = new([]);
        for (int y = 0; y < Grid.ROWS; y++)
        {
            for (int x = 0; x < Grid.COLUMNS; x++)
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
                        Position = new Vector2((x * 8) + pos.X + 4, (y * 8) + (pos.Y - 160)),
                        Data = new EmitterData
                        {
                            emissionInterval = 1f,
                            density = 20,
                            angleVarianceMax = 90,
                            particleActiveTime = (.01f, .3f),
                            speed = (50, 100),
                            particleData = new ParticleData()
                            {
                                texture = GetContent.Load<Texture2D>("Image/Effect/lockFlashEffect"),
                                colorTimeLine = (color, color),
                                scaleTimeLine = new(3, 1),
                                opacityTimeLine = new(1, 1),
                                frictionFactor = new Vector2(0, .003f)
                            }
                        }
                    });
                }
            }
        }
        GroupEmitterObj clear = new(sources, EmissionType.Burst);
        ParticleManager.AddEmitter(clear);
    }

    public static void DropSparkle(Piece piece, Vector2 pos) 
    {
        StaticEmissionSources sources = new(new());
        for (var y = 0; y < piece.currentRotation.GetLength(0); y++) 
        {
            for (var x = 0; x < piece.currentRotation.GetLength(1); x++) 
            {
                if (piece.currentRotation[y, x] != 0) 
                {
                    sources.Members.Add(new GroupPartData()
                    {
                        Position = new Vector2((x * 8) + pos.X + ((int)piece.offsetX * 8), (y * 8) + (pos.Y) + ((int)(piece.offsetY * 8) - 160)),
                        Data = new EmitterData 
                        {
                            emissionInterval = 1f,
                            density = ExtendedMath.Rng.Next(1, 4),
                            angleVarianceMax = 0,
                            particleActiveTime = (.01f, .5f),
                            speed = (50, 200),
                            rotationSpeed = (-.05f, .05f),
                            offsetX = (0, 8),
                            offsetY = (0, 8),
                            particleData = new ParticleData() 
                            {
                                texture = GetContent.Load<Texture2D>("Image/Effect/Particle/starLarge"),
                                rotationSpeed = .05f,
                                colorTimeLine = (Color.White, Color.White),
                                scaleTimeLine = new (5, 7),
                                opacityTimeLine = new (1, 0),
                                frictionFactor = new Vector2(0, .0005f),
                            }
                        }
                    });
                }
                
            }
        }
        GroupEmitterObj drop = new(sources, EmissionType.Burst);
        ParticleManager.AddEmitter(drop);
    }
}