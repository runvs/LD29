/// This Program is provided as is with absolutely no warranty.
/// This File is published under the LGPL 3. See lgpl.txt
/// Published by Julian Dinges and Simon Weis, 2013
/// Contact laguna_1989@gmx.net

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
            LADDER,
            CABLE,
            GENERATOR_1,
            GENERATOR_2,
            LAMP,
            MINING_BUILDING,

            OVERWORLD_TOP,
            OVERWORLD_TOP_LEFT,
            OVERWORLD_TOP_RIGHT,
            OVERWORLD_TOP_BOTTOM,
            OVERWORLD_TOP_BOTTOM_LEFT,
            OVERWORLD_TOP_BOTTOM_RIGHT,
            OVERWORLD_TOP_LEFT_RIGHT,
            OVERWORLD_FLOAT,

            UNDERWORLD_MID,
            UNDERWORLD_TOP,
            UNDERWORLD_BOTTOM,
            UNDERWORLD_TOP_LEFT,
            UNDERWORLD_TOP_RIGHT,
            UNDERWORLD_TOP_BOTTOM,
            UNDERWORLD_TOP_BOTTOM_LEFT,
            UNDERWORLD_TOP_BOTTOM_RIGHT,
            UNDERWORLD_TOP_LEFT_RIGHT,
            UNDERWORLD_LEFT,
            UNDERWORLD_RIGHT,
            UNDERWORLD_BOTTOM_LEFT,
            UNDERWORLD_BOTTOM_RIGHT,
            UNDERWORLD_LEFT_RIGHT,
            UNDERWORLD_BOTTOM_LEFT_RIGHT,
            UNDERWORLD_FLOAT

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

            if (_type == TileType.OVERWORLD_TOP)
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
                case TileType.LADDER:
                    _sprite = new SmartSprite("../GFX/ladder.png");
                    break;
                case TileType.CABLE:
                    _sprite = new SmartSprite("../GFX/cable.png");
                    break;
                case TileType.GENERATOR_1:
                    _sprite = new SmartSprite("../GFX/generator_1.png");
                    break;
                case TileType.GENERATOR_2:
                    _sprite = new SmartSprite("../GFX/generator_2.png");
                    break;
                case TileType.LAMP:
                    _sprite = new SmartSprite("../GFX/ceiling_lamp.png");
                    break;
                case TileType.MINING_BUILDING:
                    _sprite = new SmartSprite("../GFX/mining_building.png");
                    break;
                default:
                    _sprite = new SmartSprite(string.Format("../GFX/tiles/{0}.png", _type.ToString()));
                    break;
            }
        }


        #endregion Methods

    }
}
