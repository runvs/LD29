using System;
using System.Collections.Generic;
using JamUtilities;
using SFML.Graphics;
using SFML.Window;

namespace JamTemplate
{
    class Player
    {
        #region Fields

        public int playerNumber;
        public string PlayerName { get; private set; }
        private SmartSprite _sprite;
        public Vector2f AbsolutePositionInPixel { get; private set; }
        public Vector2f ShouldBePosition { get; private set; }

        public Texture _glowspriteTexture;
        public Sprite _glowSpriteSprite;


        private List<Vector2f> _waypointList;

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
            _waypointList = new List<Vector2f>();
            try
            {
                LoadGraphics();
                JamUtilities.GlowSpriteCreator.CreateRadialGlow(out _glowspriteTexture, 64, GameProperties.ColorGrey1, 0.2f, PennerDoubleAnimation.EquationType.SineEaseIn);
                _glowSpriteSprite = new Sprite(_glowspriteTexture);
                _glowSpriteSprite.Scale = new Vector2f(1.0f, 0.25f);
                _glowSpriteSprite.Origin = new Vector2f(GameProperties.TileSizeInPixelScaled / 2, GameProperties.TileSizeInPixelScaled - 35);
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

            GetMouseInput();
        }

        private void GetMouseInput()
        {
            if (_waypointList.Count == 0)
            {
                Vector2f AbsoluteMousePosition = new Vector2f(JamUtilities.Mouse.MousePositionInWindow.X, JamUtilities.Mouse.MousePositionInWindow.Y + 64) + Camera.CameraPosition;
                if (SFML.Window.Mouse.IsButtonPressed(SFML.Window.Mouse.Button.Left))
                {
                    _waypointList = _world.GetWaypointListToPosition(AbsolutePositionInPixel, AbsoluteMousePosition);
                   
                }
            }
        }

        public void Update(TimeObject deltaT)
        {
            _sprite.Update(deltaT);
            DoPlayerMovement(deltaT);
        }

        private void DoPlayerMovement(TimeObject deltaT)
        {
            // calculate the difference vector between current and shouldbe position
            if (_waypointList.Count > 0)
            {
                ShouldBePosition = _waypointList[0];


                Vector2f dif = ShouldBePosition - AbsolutePositionInPixel;

                // TODO juice for movement feel

                float difLenght = MathStuff.GetLength(dif);
                if (difLenght >= GameProperties.PlayerDistanceToWaypointAccepted)
                {
                    Vector2f moveVelocity = dif / difLenght * deltaT.ElapsedGameTime * GameProperties.PlayerMaxVelocity * ScalingFactor(difLenght);
                    AbsolutePositionInPixel += moveVelocity;
                }
                else
                {
                    AbsolutePositionInPixel = _waypointList[0];
                    _waypointList.RemoveAt(0);
                }
            }
        }

        public void ResetPathfinding()
        {
            _waypointList.Clear();
            ShouldBePosition = AbsolutePositionInPixel;
        }

        public void SetPlayerPosition(Vector2i pos)
        {
            AbsolutePositionInPixel = new Vector2f(pos.X * GameProperties.TileSizeInPixelScaled, pos.Y * GameProperties.TileSizeInPixelScaled);
            ShouldBePosition = new Vector2f(pos.X * GameProperties.TileSizeInPixelScaled, pos.Y * GameProperties.TileSizeInPixelScaled);
        }

        // probably some juice
        private float ScalingFactor(float difLenght)
        {
            float factor = 1.0f;

            if (difLenght <= GameProperties.PlayerDistanceToWaypointAccepted * 2)
            {
                factor = 0.85f;
            }
            return factor;
        }


        public Vector2f GetOnScreenPosition()
        {
            return AbsolutePositionInPixel - Camera.CameraPosition;
        }

        public void Draw(SFML.Graphics.RenderWindow rw)
        {
            _sprite.Position = GetOnScreenPosition();
            _glowSpriteSprite.Position = GetOnScreenPosition();

            Tile tilePlayerIsOn = _world.GetTileOnPosition((int)(AbsolutePositionInPixel.X / GameProperties.TileSizeInPixelScaled), (int)(AbsolutePositionInPixel.Y / GameProperties.TileSizeInPixelScaled));
            if (tilePlayerIsOn != null && tilePlayerIsOn.GetTileType() != Tile.TileType.LADDER)
            {
                rw.Draw(_glowSpriteSprite);
            }

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
            _sprite.Origin = new Vector2f(GameProperties.TileSizeInPixelOriginal / 2.0f, GameProperties.TileSizeInPixelOriginal);
        }

        #endregion Methods

    }
}
