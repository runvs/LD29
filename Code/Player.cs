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
        public Vector2f ShouldBePosition { get; private set; }

        Dictionary<Keyboard.Key, Action> _actionMap;
        private float movementTimer = 0.0f; // time between two successive movement commands
        private World _world;

        #endregion Fields

        #region Methods

        public Player(World world, int number)
        {
            _world = world;
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

            AbsolutePositionInPixel = new Vector2f(800, 14*64);
            ShouldBePosition = new Vector2f(800, 14 * 64);
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

            GetMouseInput();
        }

        private void GetMouseInput()
        {
            Vector2f AbsoluteMousePosition = new Vector2f(JamUtilities.Mouse.MousePositionInWindow.X, JamUtilities.Mouse.MousePositionInWindow.Y) + Camera.CameraPosition;
            if (SFML.Window.Mouse.IsButtonPressed(SFML.Window.Mouse.Button.Left))
            {
                System.Console.WriteLine(AbsoluteMousePosition);
                  ShouldBePosition = AbsoluteMousePosition;
            }

            // check if clicked Position is valid;

          

        }

        public void Update(TimeObject deltaT)
        {
			_sprite.Update(deltaT);

            DoPlayerMovement(deltaT);
        }

        private void DoPlayerMovement(TimeObject deltaT)
        {
            // calculate the difference vector between current and shouldbe position

            Vector2f dif = ShouldBePosition - AbsolutePositionInPixel;

            // TODO juice for movement feel

            float difLenght = MathStuff.GetLength(dif);
            if (difLenght >= 1.0f)
            {
                Vector2f moveVelocity = dif * deltaT.ElapsedGameTime * GameProperties.PlayerMaxVelocity;
                AbsolutePositionInPixel += moveVelocity;
            }

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
            _sprite = new SmartSprite("../GFX/player.png");
        }

        #endregion Methods

    }
}
