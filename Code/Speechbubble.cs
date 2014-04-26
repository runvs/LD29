using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JamUtilities;
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

        private SmartSprite _sprite;


        public Speechbubble(string text, Vector2f position)
        {
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
            _sprite.Draw(rw);
            SmartText.DrawText(Text, TextAlignment.LEFT, AbsolutePosition, new Vector2f(1.0f, 1.0f), GameProperties.ColorBlue1, rw);
        }

        bool IGameObject.IsDead()
        {
            return IsDead;
        }
    }
}
