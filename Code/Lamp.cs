using System;
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
        private static SmartSprite _sprite;

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
                    (uint)(GameProperties.TileSizeInPixelScaled),
                    GameProperties.ColorBlue1,
                    0.75f, PennerDoubleAnimation.EquationType.CubicEaseOut);
                _glowSpriteSprite = new Sprite(_glowSpriteTexture);
                _glowSpriteSprite.Origin = new Vector2f(GameProperties.TileSizeInPixelScaled / 2.0f, GameProperties.TileSizeInPixelScaled / 3.0f + 6);

                _sprite = new SmartSprite("../GFX/ceiling_lamp.png");
                _sprite.Origin = new Vector2f(GameProperties.TileSizeInPixelOriginal / 2.0f, 2.5f);
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
            DrawLamp(rw);
            DrawGlow(rw);
        }

        public void DrawLamp(SFML.Graphics.RenderWindow rw)
        {
            Vector2f pos = AbsolutePositionInPixel - Camera.CameraPosition + _positionOffset + ScreenEffects.GlobalSpriteOffset; ;

            // draw Lamp Sprite?
            _sprite.Position = pos;
            _sprite.Draw(rw);
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
