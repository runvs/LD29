using System;
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

        private List<TriggerArea> _triggerAreaList;
        private List<Tile> _tileList;
        // holds all the Waypoints in absolut coordinates
        private List<Vector2f> _waypointList;
        private List<IGameObject> _speechBubbleList;
        private Player _player;
        private byte[,] _waypointGrid;
        private Dictionary<string, Action<object>> _functionDict;

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

            List<IGameObject> newSpeechBubbleList = new List<IGameObject>();
            foreach (var sb in _speechBubbleList)
            {
                if (!sb.IsDead())
                {
                    sb.Update(timeObject);
                    newSpeechBubbleList.Add(sb);
                }
            }
            _speechBubbleList = newSpeechBubbleList;


            CheckIfAreaTriggered();

            Camera.ShouldBePosition = _player.AbsolutePositionInPixel - new Vector2f(400, 300);
            Camera.DoCameraMovement(timeObject);
        }

        private void CheckIfAreaTriggered()
        {
            foreach (var area in _triggerAreaList)
            {
                if (area.CheckIsInside(_player.AbsolutePositionInPixel))
                {
                    switch (area.Type)
                    {
                        case TriggerAreaType.PORTAL:
                            LoadWorld(area.Id);
                            Console.WriteLine("Loading level {0}", area.Id);
                            return;

                        case TriggerAreaType.EXPLOSION:
                            if (_functionDict.ContainsKey(area.Id))
                            {
                                _functionDict[area.Id](null);
                            }
                            break;

                        default: break;
                    }
                }
            }
        }

        public void Draw(RenderWindow rw)
        {
            rw.Clear(GameProperties.ColorGrey3);
            ParticleManager.Draw(rw);

            foreach (var t in _tileList)
            {
                t.Draw(rw);
            }

            _player.Draw(rw);

            foreach (var sb in _speechBubbleList)
            {
                sb.Draw(rw);
            }

            ScreenEffects.GetStaticEffect("vignette").Draw(rw);
            ScreenEffects.Draw(rw);

        }

        private void InitGame()
        {
            _tileList = new List<Tile>();
            _waypointList = new List<Vector2f>();
            _speechBubbleList = new List<IGameObject>();
            _functionDict = new Dictionary<string, Action<object>>();
            _player = new Player(this, 0);
            LoadWorld();

            AddSpeechBubble("Wow this already looks great!", new Vector2f(150, 25));

            _functionDict.Add("basicExplosion", BasicExplosion);
        }

        private void BasicExplosion(object o = null)
        {
            var shake = ScreenEffects.GetDynamicEffect("shake");
            shake.StartEffect(1.0f, .05f, Color.White, 10.0f, ShakeDirection.AllDirections);

            ParticleProperties props = new ParticleProperties();
            props.Type = ParticleManager.ParticleType.PT_SmokeCloud;
            props.col = Color.Black;
            props.lifeTime = 4.0f;
            props.sizeMultiple = 100.0f;
            props.sizeSingle = 15;
            props.RotationType = ParticleManager.ParticleRotationType.PRT_Velocity;
            props.AffectedByGravity = true;
            var emitter = new ParticleEmitter(new FloatRect(50, 150, 100, 100), props, 10);
            emitter.Update(3);
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
            Vector2f absoluteWayPointCoordinates = new Vector2f((0.5f + (float)(tilePos.X)) * GameProperties.TileSizeInPixelScaled, (float) tilePos.Y * (float)GameProperties.TileSizeInPixelScaled);

            return absoluteWayPointCoordinates;
        }

        private void CreateWayPoints()
        {

            int width = GameProperties.WorldSizeInTiles.X;
            int height = GameProperties.WorldSizeInTiles.Y;
            width = PathFinderHelper.RoundToNearestPowerOfTwo(width);
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

            if (_waypointGrid[startPositionInTiles.X, startPositionInTiles.Y] == 0) // this should not happen
            {
                Console.WriteLine("Broken Player Position");
            }
           
          

            /*System.Console.WriteLine("");
            System.Console.WriteLine(startPositionInTiles.X + " " + startPositionInTiles.Y + " " + _waypointGrid[startPositionInTiles.X, startPositionInTiles.Y]);
            System.Console.WriteLine(endPositionInTiles.X + " " + endPositionInTiles.Y + " " + _waypointGrid[endPositionInTiles.X, endPositionInTiles.Y]);
            System.Console.WriteLine("");*/


            Point startPoint = new Point(startPositionInTiles.X, startPositionInTiles.Y);
            Point endPoint = new Point(endPositionInTiles.X, endPositionInTiles.Y);
            List<PathFinderNode> path = new PathFinderFast(_waypointGrid).FindPath(startPoint, endPoint);
            if (path != null)
            {
                path.Reverse();
                foreach (var pfn in path)
                {
                    ret.Add(GetWayPointForTile(new Vector2i(pfn.X, pfn.Y)));
                }
            }
            else
            {
                System.Console.WriteLine("Could not find Path");
            }

            return ret;
        }

        private void SetWorldDependentSettings()
        {
            Camera.MaxPosition = new Vector2f(GameProperties.WorldSizeInTiles.X, GameProperties.WorldSizeInTiles.Y) * GameProperties.TileSizeInPixelScaled;
        }

        private void LoadWorld(string levelName = null)
        {
            var parser = new MapParser(levelName == null ? "../Data/Overworld.tmx" : string.Format("../Data/{0}.tmx", levelName), levelName != null);
            GameProperties.WorldSizeInTiles = parser.WorldSize;

            _tileList = parser.TerrainLayer;
            _triggerAreaList = parser.TriggerAreaList;
            _player.SetPlayerPosition(parser.PlayerPosition);
            SetWorldDependentSettings();
            CreateWayPoints();
            ClearSpeechBubbles();

            _player.ResetPathfinding();

        }

        private void ClearSpeechBubbles()
        {
            _speechBubbleList.Clear();
        }

        public void AddSpeechBubble(String text, Vector2f position)
        {
            Speechbubble sb = new Speechbubble(text, position);
            _speechBubbleList.Add(sb);
        }

        #endregion Methods

    }
}
