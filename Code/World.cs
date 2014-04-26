using SFML.Graphics;
using System;
using JamUtilities;
using JamUtilities.Particles;
using JamUtilities.ScreenEffects;
using System.Collections.Generic;
using SFML.Window;

namespace JamTemplate
{
    class World
    {

        #region Fields

        private List<Tile> _tileList;
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

            Camera.ShouldBePosition = _player.GetOnScreenPosition() ;
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
            _player = new Player(this, 0);
            //LoadWorld();
            CreateDefaultWorld();

            SetWorldDependentSettings();
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

    }
}
