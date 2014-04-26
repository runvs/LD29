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
            Air,
            Grass,

        }
        #endregion Enums

        #region Fields


        SFML.Graphics.Texture TileTexture;

        Sprite TileSprite;

        public SFML.Window.Vector2i TilePosition { get; private set; }
        public bool IsTileBlockd { get; private set; }
        private TileType _type;

        #endregion Fields

        #region Methods

        public Tile(int posX, int posY, TileType tt)
        {
            _type = tt;
            TilePosition = new SFML.Window.Vector2i(posX, posY);
            LoadGraphics();
            IsTileBlockd = false;

            if (_type == TileType.Grass )
            {
                IsTileBlockd = true;
            }

        }

        public void Draw(RenderWindow rw)
        {
            TileSprite.Position = new Vector2f(
               GameProperties.TileSizeInPixelScaled * TilePosition.X - Camera.CameraPosition.X,
               GameProperties.TileSizeInPixelScaled * TilePosition.Y - Camera.CameraPosition.Y
           );

            rw.Draw(TileSprite);
        }

        public void LoadGraphics()
        {
            if (_type == TileType.Grass)
            {
                TileTexture = new Texture("../GFX/grass.png");
            }
            TileSprite = new Sprite(TileTexture);
            TileSprite.Scale = new SFML.Window.Vector2f(2.0f, 2.0f);

        }


        #endregion Methods

    }
}
