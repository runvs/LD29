using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JamUtilities;
using JamUtilities.ScreenEffects;
using SFML.Graphics;
using SFML.Window;

namespace JamTemplate
{
    public class Speechbubble : IGameObject
    {
        public string Text {get; private set;}
        public Vector2f AbsolutePosition { get; private set; }

        public bool IsDead { get; private set; }
        public bool IsFadingOut { get; private set; }

        private float _remainingDisplayTime;

        private static SmartSprite _sprite;
        private static bool _isInitialized = false;
        private static Vector2f _spriteOffset;
        private static Texture _glowShadeTexture;
        private static Sprite _glowShadeSprite;
        private float _timeSinceStart;
        private static byte _spriteBasicAlphaValue = 155;



        public Speechbubble(string text, Vector2f position)
        {
            if (!_isInitialized)
            {
                _isInitialized = true;
                _sprite = new SmartSprite("../GFX/speech.png");
                _spriteOffset = new Vector2f(-25, -5);
                _sprite.Alpha = _spriteBasicAlphaValue;

                GlowSpriteCreator.CreateRadialGlow(out _glowShadeTexture, 450, GameProperties.ColorPink4, 0.7f, PennerDoubleAnimation.EquationType.Linear);
                _glowShadeSprite = new Sprite(_glowShadeTexture);
                _glowShadeSprite.Scale = new Vector2f(1.0f, 0.35f);
            }
            _timeSinceStart = 0.0f;
            Text = text;
            AbsolutePosition = position;

            _remainingDisplayTime = GameProperties.SpeechBubbleLifeTime;
            IsDead = false;
            IsFadingOut = false;
        }

        public void GetInput()
        {
        }

        public void Update(TimeObject timeObject)
        {
            _sprite.Update(timeObject);
            _remainingDisplayTime -= timeObject.ElapsedGameTime;
            _timeSinceStart += timeObject.ElapsedGameTime;
            if (!IsFadingOut)
            {
                if (_remainingDisplayTime <= 0)
                {
                    IsFadingOut = true;
                    _remainingDisplayTime = GameProperties.SpeechBubbleFadeTime;
                }
            }
            else
            {
                if (_remainingDisplayTime <= 0)
                {
                    IsDead = true;
                }
            }
        }

        public void Draw(SFML.Graphics.RenderWindow rw)
        {
            if (!IsDead)
            {
                // position Stuff
                Vector2f screenPos = 
                    AbsolutePosition - 
                    Camera.CameraPosition + 
                    new Vector2f(3.0f * (float)Math.Sin(_timeSinceStart*2 + 4.2f), 
                        12 * (float)Math.Sin(_timeSinceStart * 1.5));
                _sprite.Position = screenPos + _spriteOffset;
                _glowShadeSprite.Position = screenPos + _spriteOffset;

                // fading Stuff
                float alphaVal = (1.0f - (float)PennerDoubleAnimation.GetValue(
                        PennerDoubleAnimation.EquationType.CubicEaseIn,
                        GameProperties.SpeechBubbleFadeTime- _remainingDisplayTime,
                        0,1,GameProperties.SpeechBubbleFadeTime));
                if (IsFadingOut)
                {
                    _sprite.Alpha = (byte)((float)_spriteBasicAlphaValue * alphaVal);
                    Color colshade = Color.White;
                    colshade.A = (byte)(255.0f * alphaVal);
                    _glowShadeSprite.Color = colshade;
                }
                else
                {
                    Color colshade = Color.White;
                    colshade.A = 255;
                    _glowShadeSprite.Color = colshade;
                    _sprite.Alpha = _spriteBasicAlphaValue;
                }
                rw.Draw(_glowShadeSprite);
                _sprite.Draw(rw);

                // Text Stuff
                Color col = GameProperties.ColorBlue1;
                col.A = (byte)((float)_sprite.Alpha / 185.0f * 255.0f);
                screenPos += ScreenEffects.GlobalSpriteOffset + 
                        new Vector2f(- 1.0f * (float)Math.Sin(_timeSinceStart * 0.5 + 3.2f),
                        - 4 * (float)Math.Sin(_timeSinceStart* 1.25 + 2)); ;
                SmartText.DrawTextWithLineBreaks(Text, TextAlignment.LEFT, screenPos , new Vector2f(1.0f, 1.0f), col, rw);
            }
        }

        bool IGameObject.IsDead()
        {
            return IsDead;
        }
    }
}
