using System;
using System.Collections.Generic;
using DeenGames.Utils;
using DeenGames.Utils.AStarPathFinder;
using JamUtilities;
using JamUtilities.Particles;
using JamUtilities.ScreenEffects;
using SFML.Audio;
using SFML.Graphics;
using SFML.Window;

namespace JamTemplate
{
    public class World
    {

        #region Fields

        public List<TriggerArea> _triggerAreaList;
        private List<Tile> _tileList;
        private List<Tile> _decorationTileList;
        private List<Tile> _backgroundDecorationTileList;
        // holds all the Waypoints in absolut coordinates
        private List<Vector2f> _waypointList;
        private List<IGameObject> _speechBubbleList;
        private List<Lamp> _lampList;
        private List<AreatricCloud> _cloudList;
        public Player _player;
        internal byte[,] _waypointGrid;
        private Dictionary<string, Action<object>> _functionDict;
        private SmartSprite _backgroundImage;

        private Music _generatorHum;
        private SoundBuffer _explosionSndBuf;
        private Sound _explosionSound;


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
            ParticleManager.GlobalPositionOffset = Camera.CameraPosition;
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
            foreach (var l in _lampList)
            {
                l.Update(timeObject);
            }

            UpdateGeneratorSound();

            _backgroundImage.Position = new Vector2f(-0.1f * Camera.CameraPosition.X, -Camera.CameraPosition.Y);
            _backgroundImage.Update(timeObject);


            CheckIfAreaTriggered();

            Camera.ShouldBePosition = _player.AbsolutePositionInPixel - new Vector2f(400, 300);
            Camera.DoCameraMovement(timeObject);
            AreatricCloud.GlobalPositionOffset = Camera.CameraPosition * 0.75f;
        }

        private void UpdateGeneratorSound()
        {
            // 
            if (StoryProgress.HasRepairedGenerator && _generatorHum.Status == SoundStatus.Stopped || _generatorHum.Status == SoundStatus.Paused)
            {
                _generatorHum.Play();
            }

            Vector2f GeneratorPosition = new Vector2f(3, 14) * GameProperties.TileSizeInPixelScaled;
            float distanceSquaredInverse = 1.0f / MathStuff.GetLengthSquared(GeneratorPosition - _player.AbsolutePositionInPixel);

            _generatorHum.Volume = distanceSquaredInverse * 800000;

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

                        case TriggerAreaType.FUNCTION:
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
            DrawBackgroundColor(rw);

            _backgroundImage.Draw(rw);

            foreach (var ac in _cloudList)
            {
                ac.Draw(rw);
            }

            foreach (var b in _backgroundDecorationTileList)
            {
                b.Draw(rw);
            }
            ParticleManager.Draw(rw);
            foreach (var t in _tileList)
            {
                t.Draw(rw);
            }
            foreach (var d in _decorationTileList)
            {
                d.Draw(rw);
            }

            _player.Draw(rw);

            foreach (var sb in _speechBubbleList)
            {
                sb.Draw(rw);
            }
            foreach (var l in _lampList)
            {
                l.DrawGlow(rw);
            }

            DrawOverlayEffect(rw);


            ScreenEffects.Draw(rw);
        }

        private void DrawOverlayEffect(RenderWindow rw)
        {
            ScreenEffects.GetStaticEffect("vignette").Draw(rw);
            ScreenEffects.GetStaticEffect("scanlines").Draw(rw);
            if (StoryProgress.ExplosionHasHappened && !StoryProgress.HasReachedExit)
            {
                ScreenEffects.GetStaticEffect("vignette").Draw(rw);
                if (!StoryProgress.HasRepairedGenerator)
                {
                    ScreenEffects.GetStaticEffect("vignette").Draw(rw);
                }
            }
        }

        private void DrawBackgroundColor(RenderWindow rw)
        {
            Color backgroundColor = GameProperties.ColorPink1;

            if (StoryProgress.ExplosionHasHappened)
            {
                backgroundColor = GameProperties.ColorPink4;
            }
            if (StoryProgress.HasRepairedGenerator)
            {
                backgroundColor = GameProperties.ColorPink3;
            }
            if (StoryProgress.HasReachedExit)
            {
                backgroundColor = GameProperties.ColorPink1;
            }

            rw.Clear(backgroundColor);
        }

        private void InitGame()
        {

            _generatorHum = new Music("../SFX/generator.ogg");
            _generatorHum.Loop = true;

            _explosionSndBuf = new SoundBuffer("../SFX/explosion.ogg");
            _explosionSound = new Sound(_explosionSndBuf);
            _explosionSound.Volume = 50;
            StoryProgress._world = this;
            _decorationTileList = new List<Tile>();
            _tileList = new List<Tile>();
            _waypointList = new List<Vector2f>();
            _speechBubbleList = new List<IGameObject>();
            _lampList = new List<Lamp>();
            _functionDict = new Dictionary<string, Action<object>>();

            _player = new Player(this, 0);
            LoadWorld();

            CreateClouds();

            _functionDict.Add("Helm", StoryProgress.Helm);
            _functionDict.Add("MoveIn", StoryProgress.MoveIn);
            _functionDict.Add("basicExplosion", StoryProgress.CaveCollapse);
            _functionDict.Add("GoIntoMine", StoryProgress.TellMinerToGoIntoMine);
            _functionDict.Add("SoWeWere", StoryProgress.SoWeWere);
            _functionDict.Add("WhatToDo", StoryProgress.WhatToDo);
            _functionDict.Add("GeneratorArea", StoryProgress.VisitGeneratorArea);
            _functionDict.Add("Cable", StoryProgress.PickupCable);

            _functionDict.Add("Miner1", StoryProgress.Miner1);
            _functionDict.Add("Miner2", StoryProgress.Miner2);
            _functionDict.Add("Miner3", StoryProgress.Miner3);

            _functionDict.Add("DrillFound", StoryProgress.DrillFound);
            _functionDict.Add("DrillMachine", StoryProgress.DrillMachine);



            _functionDict.Add("LadderDamaged", StoryProgress.LadderDamaged);
            _functionDict.Add("ThisWay", StoryProgress.ThisWay);
            _functionDict.Add("UpNorth", StoryProgress.UpNorth);




            _functionDict.Add("Finish", StoryProgress.Finished);

        }

        internal void PlayExplosionSound()
        {
            _explosionSound.Play();
        }

        private void CreateClouds()
        {
            Color cloudColor = GameProperties.ColorGrey1;
            Color backgroundColor = GameProperties.ColorPink4;

            _cloudList = new System.Collections.Generic.List<AreatricCloud>();
            for (int i = 0; i != 120; i++)
            {
                AreatricCloud ac = new AreatricCloud(
                    RandomGenerator.GetRandomVector2f(new Vector2f(-64, 1600), new Vector2f(-100, 64)),
                    cloudColor,
                    backgroundColor);
                _cloudList.Add(ac);
            }
        }

        public Tile GetTileOnPosition(int x, int y)
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

        internal void RemoveTileAt(Vector2i tilePos)
        {
            List<Tile> newList = new List<Tile>();

            foreach (var t in _tileList)
            {
                if (!t.TilePosition.Equals(tilePos))
                {
                    newList.Add(t);
                }
            }
            _tileList = newList;
        }

        internal void RemoveDecorationAt(Vector2i tilePos)
        {
            List<Tile> newList = new List<Tile>();

            foreach (var t in _decorationTileList)
            {
                if (!t.TilePosition.Equals(tilePos))
                {
                    newList.Add(t);
                }
            }
            _decorationTileList = newList;
        }

        private Vector2f GetWayPointForTile(Vector2i tilePos)
        {
            Vector2f absoluteWayPointCoordinates = new Vector2f((0.5f + (float)(tilePos.X)) * GameProperties.TileSizeInPixelScaled, (float)tilePos.Y * (float)GameProperties.TileSizeInPixelScaled);

            return absoluteWayPointCoordinates;
        }

        private void CreateWaypoints()
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
                    else if (GetTileOnPosition(i, j) != null && GetTileOnPosition(i, j).GetTileType() == Tile.TileType.LADDER)
                    {
                        _waypointGrid[i, j] = PathFinderHelper.EMPTY_TILE;
                    }
                    else if (GetTileOnPosition(i, j - 1) != null && GetTileOnPosition(i, j - 1).GetTileType() == Tile.TileType.LADDER)
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
            
            System.Console.WriteLine("");*/


            Point startPoint = new Point(startPositionInTiles.X, startPositionInTiles.Y);
            Point endPoint = new Point(endPositionInTiles.X, endPositionInTiles.Y);
            //System.Console.WriteLine(startPoint.X + " " + startPoint.Y + " " + _waypointGrid[startPoint.X, startPoint.Y]);
            //System.Console.WriteLine(endPoint.X + " " + endPoint.Y + " " + _waypointGrid[endPoint.X, endPoint.Y]);
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
            Camera.MaxPosition = new Vector2f(GameProperties.WorldSizeInTiles.X - 13, GameProperties.WorldSizeInTiles.Y - 10) * GameProperties.TileSizeInPixelScaled;
        }

        private void LoadWorld(string levelName = null)
        {
            var parser = new MapParser(levelName == null ? "../Data/Overworld.tmx" : string.Format("../Data/{0}.tmx", levelName), levelName != null);
            GameProperties.WorldSizeInTiles = parser.WorldSize;

            _decorationTileList = parser.DecorationLayer;
            _backgroundDecorationTileList = parser.BackgroundDecorationLayer;
            _lampList = parser.LampList;
            _tileList = parser.TerrainLayer;
            _triggerAreaList = parser.TriggerAreaList;
            _player.SetPlayerPosition(parser.PlayerPosition);
            SetWorldDependentSettings();
            CreateWaypoints();
            ResetSpeechBubbles();

            CreateWaterDropSpaces();

            _player.ResetPathfinding();

            AddSpeechBubble("Tap above the ground to walk.", new Vector2f(_player.AbsolutePositionInPixel.X, _player.AbsolutePositionInPixel.Y - 200));

            ParticleManager.CreateChimney(
                new FloatRect(20 * GameProperties.TileSizeInPixelScaled + 48, 2 * GameProperties.TileSizeInPixelScaled + 3, 8, 2),
                GameProperties.ColorPink4,
                90);

            // Create background image
            _backgroundImage = new SmartSprite("../GFX/background_top.png");
            _backgroundImage.Origin = new Vector2f(0, 67);
        }

        private void CreateWaterDropSpaces()
        {
            for (int i = 0; i <= GameProperties.WorldWaterDropSpaces; i++)
            {
                bool searching = true;
                Tile finallyFound = null;
                while (searching)
                {
                    int randomposition = RandomGenerator.Random.Next(_tileList.Count);
                    Tile t = _tileList[randomposition];
                    Vector2i tilePosition = t.TilePosition;
                    Tile tBelow = GetTileOnPosition(tilePosition.X, tilePosition.Y + 1);
                    if (tBelow == null)
                    {
                        finallyFound = t;
                        searching = true;
                        break;
                    }
                }
                Vector2f dropSpawnPosition =
                    new Vector2f(finallyFound.TilePosition.X, finallyFound.TilePosition.Y) * GameProperties.TileSizeInPixelScaled;
                ParticleManager.CreateRainArea(new FloatRect(
                    dropSpawnPosition.X,
                    dropSpawnPosition.Y + GameProperties.TileSizeInPixelScaled,
                    GameProperties.TileSizeInPixelScaled,
                    GameProperties.TileSizeInPixelScaled / 8),
                    GameProperties.ColorBlue4,
                    0.35f + 0.1f * (float)RandomGenerator.Random.NextDouble());
            }
        }

        private void ResetSpeechBubbles()
        {
            _speechBubbleList.Clear();
        }

        public void AddSpeechBubble(String text, Vector2f position)
        {
            Speechbubble sb = new Speechbubble(text, position);
            _speechBubbleList.Add(sb);
        }

        #endregion Methods


        internal void AddTile(Tile tile)
        {
            _tileList.Add(tile);
        }
    }
}
