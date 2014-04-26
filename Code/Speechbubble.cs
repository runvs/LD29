using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JamUtilities;
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


        public Speechbubble(string text, Vector2f position)
        {
            if (!_isInitialized)
            {
                _isInitialized = true;
                _sprite = new SmartSprite("../GFX/speech.png");
                _spriteOffset = new Vector2f(-25, -15);
                _sprite.Alpha = 185;
            }
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
                Vector2f screenPos = AbsolutePosition - Camera.CameraPosition ;
                _sprite.Position = screenPos + _spriteOffset;

                if (IsFadingOut)
                {
                    _sprite.Alpha = (byte)(185.0 * (1.0 - 
                        PennerDoubleAnimation.GetValue(
                        PennerDoubleAnimation.EquationType.CubicEaseIn,
                        GameProperties.SpeechBubbleFadeTime- _remainingDisplayTime,
                        0,1,GameProperties.SpeechBubbleFadeTime)));
                }
                else
                {
                    _sprite.Alpha = 185;
                }
                _sprite.Draw(rw);
                Color col = GameProperties.ColorBlue1;
                col.A = (byte)((float)_sprite.Alpha / 185.0f * 255.0f);
                SmartText.DrawTextWithLineBreaks(Text, TextAlignment.LEFT, screenPos, new Vector2f(1.0f, 1.0f), col, rw);
            }
        }

        bool IGameObject.IsDead()
        {
            return IsDead;
        }
    }
}
