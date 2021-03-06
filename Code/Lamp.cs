﻿using System;
using JamUtilities;
using JamUtilities.ScreenEffects;
using SFML.Graphics;
using SFML.Window;

namespace JamTemplate
{
    public class Lamp : IGameObject
    {
        private Texture _glowSpriteTexture;
        private Sprite _glowSpriteSprite;
        private bool IsInitialized = false;
        public Vector2f AbsolutePositionInPixel { get; private set; }
        private Vector2f _positionOffset;
        private float _lifeTime;
        private float _frequencyFactor;

        public Lamp(Vector2f position)
        {
            AbsolutePositionInPixel = position;
            _positionOffset = new Vector2f(30, 5);
            _lifeTime = 0.0f;
            _frequencyFactor = ((float)(0.1 + 2.5 * RandomGenerator.Random.NextDouble()));

            //Console.WriteLine(_frequencyFactor);
            if (!IsInitialized)
            {
                GlowSpriteCreator.CreateRadialGlow(
                    out _glowSpriteTexture,
                    (uint)(GameProperties.TileSizeInPixelScaled * 1.75),
                    GameProperties.ColorBlue1,
                    0.85f, PennerDoubleAnimation.EquationType.CubicEaseOut);
                _glowSpriteSprite = new Sprite(_glowSpriteTexture);
                _glowSpriteSprite.Origin = new Vector2f(
                    GameProperties.TileSizeInPixelScaled / 2.0f + 21, 
                    GameProperties.TileSizeInPixelScaled / 3.0f + 27);
            }

        }

        public bool IsDead()
        {
            return false;   // lamps cannot die
        }

        public void GetInput()
        {
            // lamps can not get input
        }

        public void Update(TimeObject timeObject)
        {
            _lifeTime += timeObject.ElapsedGameTime * _frequencyFactor;

        }

        public void Draw(SFML.Graphics.RenderWindow rw)
        {
            DrawGlow(rw);
        }

        public void DrawGlow(SFML.Graphics.RenderWindow rw)
        {
            Vector2f pos = AbsolutePositionInPixel - Camera.CameraPosition + _positionOffset + ScreenEffects.GlobalSpriteOffset; ;
            if (StoryProgress.HasRepairedGenerator || !StoryProgress.ExplosionHasHappened)
            {
                // change the glowsprites alpha 
                float t = _lifeTime + _frequencyFactor;
                Color col = Color.White;
                double alphaValue = Math.Abs(Math.Cos(Math.Sin(t * 3.0) + t * 3.0));
                col.A = (byte)(240.0 - 140.0 * alphaValue);

                _glowSpriteSprite.Color = col;

                // draw Glowsprite
                _glowSpriteSprite.Position = pos;
                rw.Draw(_glowSpriteSprite);




            }

        }


    }
}
