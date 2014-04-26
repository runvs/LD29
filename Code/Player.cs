using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using JamUtilities;

namespace JamTemplate
{
    class Player
    {
        #region Fields

        public int playerNumber;
        public string PlayerName { get; private set; }
        private SmartSprite _sprite;
        public Vector2f AbsolutePositionInPixel{ get; private set;}

        Dictionary<Keyboard.Key, Action> _actionMap;
        private float movementTimer = 0.0f; // time between two successive movement commands
        private World myWorld;

        #endregion Fields

        #region Methods

        public Player(World world, int number)
        {
            myWorld = world;
            playerNumber = number;

            _actionMap = new Dictionary<Keyboard.Key, Action>();

            try
            {
                LoadGraphics();
            }
            catch (SFML.LoadingFailedException e)
            {
                System.Console.Out.WriteLine("Error loading player Graphics.");
                System.Console.Out.WriteLine(e.ToString());
            }
        }

        private void SetPlayerNumberDependendProperties()
        {
            PlayerName = "Player" + playerNumber.ToString();
        }

        public void GetInput()
        {
            if (movementTimer <= 0.0f)
            {
                MapInputToActions();
            }
        }

        public void Update(float deltaT)
        {
			_sprite.Update(deltaT);
        }


        public Vector2f GetOnScreenPosition()
        {
            return AbsolutePositionInPixel - Camera.CameraPosition;
        }

        public void Draw(SFML.Graphics.RenderWindow rw)
        {
            _sprite.Position = GetOnScreenPosition();

            _sprite.Draw(rw);
        }

        private void SetupActionMap()
        {
            // e.g. _actionMap.Add(Keyboard.Key.Escape, ResetActionMap);
        }

        private void MapInputToActions()
        {
            foreach (var kvp in _actionMap)
            {
                if (Keyboard.IsKeyPressed(kvp.Key))
                {
                    // Execute the saved callback
                    kvp.Value();
                }
            }
        }

        private void LoadGraphics()
        {

            _sprite = new SmartSprite("../GFX/grass.png");
        }

        #endregion Methods

    }
}
