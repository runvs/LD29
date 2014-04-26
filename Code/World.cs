using System;
using System.Collections.Generic;
using JamUtilities;
using JamUtilities.Particles;
using JamUtilities.ScreenEffects;
using SFML.Graphics;
using SFML.Window;

namespace JamTemplate
{
    public class World
    {

        #region Fields

        private List<Tile> _tileList;
        // holds all the Waypoints in absolut coordinates
        private List<Vector2f> _waypointList;
        private Player _player;

        #endregion Fields

        #region Methods

        public World()
        {
            InitGame();
        }

        public void GetInput()
        {
            //if (SFML.Window.Keyboard.IsKeyPressed(Keyboard.Key.C))
            //{
            //    //ScreenEffects.ScreenFlash(SFML.Graphics.Color.Black, 4.0f);
            //}

            _player.GetInput();
        }

        public void Update(TimeObject timeObject)
        {
            ScreenEffects.Update(timeObject);
            SpriteTrail.Update(timeObject);
            ParticleManager.Update(timeObject);

            _player.Update(timeObject);

            Camera.ShouldBePosition = _player.GetOnScreenPosition();
            Camera.DoCameraMovement(timeObject);
        }

        public void Draw(RenderWindow rw)
        {
            rw.Clear(SFML.Graphics.Color.Blue);
            ParticleManager.Draw(rw);

            foreach (var t in _tileList)
            {
                t.Draw(rw);
            }

            _player.Draw(rw);

            ScreenEffects.GetStaticEffect("vignette").Draw(rw);
            ScreenEffects.Draw(rw);

        }

        private void InitGame()
        {
            _tileList = new List<Tile>();
            _waypointList = new List<Vector2f>();
            _player = new Player(this, 0);
            //LoadWorld();
            CreateDefaultWorld();

            SetWorldDependentSettings();

            CreateWayPoints();
        }


        private void CreateWayPoints()
        {
            foreach (var t in _tileList)
            {
                // TODO more sophisticated Method here!
                Vector2f absoluteWayPointCoordinates = new Vector2f((0.5f + (float)(t.TilePosition.X)) * GameProperties.TileSizeInPixelScaled, t.TilePosition.Y * GameProperties.TileSizeInPixelScaled - 1);
                _waypointList.Add(absoluteWayPointCoordinates);
            }
        }

        public Vector2f GetNearestWayPointToPosition(Vector2f vec)
        {
            Vector2f ret = new Vector2f();
            float smallestDistanceSquared = 99999999999999999;
            foreach (var wp in _waypointList)
            {
                float newDistance = MathStuff.GetLengthSquared(vec - wp);
                if (smallestDistanceSquared >= newDistance)
                {
                    ret = wp;
                    smallestDistanceSquared = newDistance;
                }
            }
            return ret;
        }

        private void SetWorldDependentSettings()
        {
            Camera.MaxPosition = new Vector2f(GameProperties.WorldSizeInTiles.X, GameProperties.WorldSizeInTiles.Y) * GameProperties.TileSizeInPixelScaled;
            Camera.CameraPosition = _player.GetOnScreenPosition() - new Vector2f(400, 300);

        }

        private void CreateDefaultWorld()
        {
            GameProperties.WorldSizeInTiles = new SFML.Window.Vector2i(16, 16);
            for (int i = 0; i != GameProperties.WorldSizeInTiles.X; i++)
            {
                for (int j = 0; j != GameProperties.WorldSizeInTiles.Y; j++)
                {
                    Tile newTile;
                    if (j >= GameProperties.WorldSizeInTiles.Y - 1)
                    {
                        newTile = new Tile(i, j, Tile.TileType.Grass);
                        _tileList.Add(newTile);
                    }

                }
            }
        }

        private void LoadWorld()
        {
            throw new NotImplementedException();
        }

        #endregion Methods


        internal void GetWaypointPosition(Vector2f AbsoluteMousePosition)
        {
            
        }
    }
}
