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

        private float _remainingDisplayTime;

        private SmartSprite _sprite;


        public void GetInput()
        {
        }

        public void Update(TimeObject timeObject)
        {
            _remainingDisplayTime -= timeObject.ElapsedGameTime;
        }

        public void Draw(SFML.Graphics.RenderWindow rw)
        {
            _sprite.Draw(rw);
            SmartText.DrawText(Text, TextAlignment.LEFT, new Vector2f(), new Vector2f(), GameProperties.ColorBlue1, rw);
        }
    }
}
