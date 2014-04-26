/// This Program is provided as is with absolutely no warranty.
/// This File is published under the LGPL 3. See lgpl.txt
/// Published by Julian Dinges and Simon Weis, 2013
/// Contact laguna_1989@gmx.net

using System.Collections.Generic;
using JamUtilities;
using SFML.Graphics;
using SFML.Window;

namespace JamTemplate
{
    public class Tile
    {

        #region Enums
        public enum TileType
        {
            GRASS,
            EARTH,
            LADDER
        }
        #endregion Enums

        #region Fields

        SmartSprite _sprite;

        public SFML.Window.Vector2i TilePosition { get; private set; }
        public bool IsTileBlocked { get; private set; }
        private TileType _type;

        public TileType GetTileType()
        {
            return _type;
        }



        #endregion Fields

        #region Methods

        public Tile(int posX, int posY, TileType tt)
        {
            _type = tt;
            TilePosition = new SFML.Window.Vector2i(posX, posY);
            LoadGraphics();
            IsTileBlocked = false;

            if (_type == TileType.GRASS)
            {
                IsTileBlocked = true;
            }

        }

        public void Draw(RenderWindow rw)
        {
            _sprite.Position = new Vector2f(
               GameProperties.TileSizeInPixelScaled * TilePosition.X - Camera.CameraPosition.X,
               GameProperties.TileSizeInPixelScaled * TilePosition.Y - Camera.CameraPosition.Y
           );

            _sprite.Draw(rw);
        }

        private void LoadGraphics()
        {
            switch (_type)
            {
                case TileType.GRASS:
                    _sprite = new SmartSprite("../GFX/grass.png");
                    break;
                case TileType.EARTH:
                    _sprite = new SmartSprite("../GFX/earth.png");
                    break;
                case TileType.LADDER:
                    _sprite = new SmartSprite("../GFX/ladder.png");
                    break;
            }
        }


        #endregion Methods

    }
}
