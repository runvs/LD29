using System.Collections.Generic;
using DeenGames.Utils;
using DeenGames.Utils.AStarPathFinder;
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
        private byte[,] _waypointGrid;

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
            LoadWorld();
            //CreateDefaultWorld();

            SetWorldDependentSettings();

            CreateWayPoints();
        }

        private Tile GetTileOnPosition(int x, int y)
        {
            Tile ret = null;
            foreach (var t in _tileList)
            {
                if (t.TilePosition.X == x && t.TilePosition.Y == y)
                {
                    ret = t;
                    break;
                }
            }
            return ret;
        }

        private Vector2f GetWayPointForTile(Vector2i tilePos)
        {
            Vector2f waypointPos = new Vector2f();
            Vector2f absoluteWayPointCoordinates = new Vector2f((0.5f + (float)(tilePos.X)) * GameProperties.TileSizeInPixelScaled, tilePos.Y * GameProperties.TileSizeInPixelScaled - 1);

            return waypointPos;
        }

        private void CreateWayPoints()
        {
            //foreach (var t in _tileList)
            //{
            //    Vector2i tilePosition = t.TilePosition;
            //    if (GetTileOnPosition(tilePosition.X, tilePosition.Y - 1) != null)  // there is a tile above the current t Tile
            //    {
            //        continue;
            //    }
            //    // TODO more sophisticated Method here!
                
            //    _waypointList.Add(absoluteWayPointCoordinates);
            //}



            int width = GameProperties.WorldSizeInTiles.X;
            int height = GameProperties.WorldSizeInTiles.Y;
            width =  PathFinderHelper.RoundToNearestPowerOfTwo(width);
            height = PathFinderHelper.RoundToNearestPowerOfTwo(height);

            _waypointGrid = new byte[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    // the tile itself is existing and the tile above it is not existing
                    if (GetTileOnPosition(i, j) != null && GetTileOnPosition(i, j - 1) == null)
                    {
                        _waypointGrid[i, j] = PathFinderHelper.EMPTY_TILE;
                    }
                    else
                    {
                        _waypointGrid[i, j] = PathFinderHelper.BLOCKED_TILE;
                    }
                }
            }

        }

        public Vector2i GetTileToPosition(Vector2f vec)
        {
            return new Vector2i((int)(vec.X / GameProperties.TileSizeInPixelScaled), (int)(vec.Y / GameProperties.TileSizeInPixelScaled));
        }

        public List<Vector2f> GetWaypointListToPosition(Vector2f start, Vector2f end)
        {
            List<Vector2f> ret = new List<Vector2f>();

            Vector2i startPositionInTiles = GetTileToPosition(start);
            Vector2i endPositionInTiles = GetTileToPosition(end);

            List<PathFinderNode> path = new PathFinderFast(_waypointGrid).FindPath(new Point(startPositionInTiles.X, startPositionInTiles.Y), new Point(endPositionInTiles.X, endPositionInTiles.Y));
            if (path != null)
            {
                foreach (var pfn in path)
                {
                    ret.Add(GetWayPointForTile(new Vector2i(pfn.X, pfn.Y)));
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
            GameProperties.WorldSizeInTiles = new Vector2i(16, 16);
            for (int i = 0; i != GameProperties.WorldSizeInTiles.X; i++)
            {
                for (int j = 0; j != GameProperties.WorldSizeInTiles.Y; j++)
                {
                    Tile newTile;
                    if (j >= GameProperties.WorldSizeInTiles.Y - 1)
                    {
                        newTile = new Tile(i, j, Tile.TileType.GRASS);
                        _tileList.Add(newTile);
                    }

                }
            }
        }

        private void LoadWorld()
        {
            var parser = new MapParser("../Data/Overworld.tmx");
            GameProperties.WorldSizeInTiles = parser.WorldSize;

            _tileList = parser.TerrainLayer;
            _player.SetPlayerPosition(parser.PlayerPosition);
        }

        #endregion Methods

    }
}
