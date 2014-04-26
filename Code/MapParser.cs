/// This Program is provided as is with absolutely no warranty.
/// This File is published under the LGPL 3. See lgpl.txt
/// Published by Julian Dinges and Simon Weis, 2013
/// Contact laguna_1989@gmx.net

using System.Collections.Generic;
using System.Xml;
using JamUtilities;
using SFML.Graphics;
using SFML.Window;

namespace JamTemplate
{
    public class MapParser
    {
        public List<Tile> TerrainLayer { get; private set; }
        public List<TriggerArea> TriggerAreaList { get; private set; }
        public Vector2i PlayerPosition { get; private set; }
        public Vector2i WorldSize { get; private set; }
        public Vector2i TileSize { get; private set; }

        public MapParser(string fileName, bool isChangingLevel = false)
        {
            TerrainLayer = new List<Tile>();
            TriggerAreaList = new List<TriggerArea>();

            var doc = new XmlDocument();
            doc.Load(fileName);

            int terrainOffset = int.Parse(doc.SelectSingleNode("/map/tileset[@name='Terrain']").Attributes["firstgid"].Value);
            TileSize = new Vector2i(
                int.Parse(doc.SelectSingleNode("/map").Attributes["tilewidth"].Value),
                int.Parse(doc.SelectSingleNode("/map").Attributes["tilewidth"].Value)
            );

            WorldSize = new Vector2i(
                int.Parse(doc.SelectSingleNode("/map").Attributes["width"].Value),
                int.Parse(doc.SelectSingleNode("/map").Attributes["height"].Value)
            );

            #region Load the terrain layer

            int xPos = 0, yPos = 0;

            foreach (XmlNode layerNode in doc.SelectNodes("/map/layer[@name='Terrain']/data/tile"))
            {
                int gid = int.Parse(layerNode.Attributes["gid"].Value) - terrainOffset;
                Tile.TileType type = Tile.TileType.GRASS;

                switch (gid)
                {
                    case 0:
                        type = Tile.TileType.EARTH;
                        break;
                    case 1:
                        type = Tile.TileType.GRASS;
                        break;
                    default:
                        if (xPos != 0 && (xPos + 1) % WorldSize.X == 0)
                        {
                            yPos++;
                        }
                        xPos = (xPos + 1) % WorldSize.X;
                        continue;
                }

                TerrainLayer.Add(new Tile(xPos, yPos, type));

                if (xPos != 0 && (xPos + 1) % WorldSize.X == 0)
                {
                    yPos++;
                }
                xPos = (xPos + 1) % WorldSize.X;
            }

            #endregion

            #region Load the object layer

            foreach (XmlNode node in doc.SelectNodes("/map/objectgroup[@name='Objects']/object"))
            {
                switch (node.Attributes["type"].Value)
                {
                    case "TriggerArea":
                        var nodeName = node.Attributes["name"].Value;
                        var floatRect = new FloatRect(
                            float.Parse(node.Attributes["x"].Value) * SmartSprite._scaleVector.X,
                            float.Parse(node.Attributes["y"].Value) * SmartSprite._scaleVector.Y,
                            float.Parse(node.Attributes["width"].Value) * SmartSprite._scaleVector.X,
                            float.Parse(node.Attributes["height"].Value) * SmartSprite._scaleVector.Y
                        );
                        var id = node.SelectSingleNode("properties/property[@name='id']").Attributes["value"].Value;

                        switch (nodeName)
                        {
                            case "Portal":
                                TriggerAreaList.Add(new TriggerArea(floatRect, TriggerAreaType.PORTAL, id));
                                break;
                            case "Explosion":
                                TriggerAreaList.Add(new TriggerArea(floatRect, TriggerAreaType.EXPLOSION, id, true));
                                break;
                            default: break;
                        }
                        break;
                    case "Spawn":
                        if (!isChangingLevel && node.Attributes["name"].Value == "PlayerSpawn")
                        {
                            PlayerPosition = new Vector2i(
                                int.Parse(node.Attributes["x"].Value) / TileSize.X,
                                int.Parse(node.Attributes["y"].Value) / TileSize.Y + 1
                            );
                        }
                        else if (isChangingLevel && node.Attributes["name"].Value == "PlayerPortalSpawn")
                        {
                            PlayerPosition = new Vector2i(
                                int.Parse(node.Attributes["x"].Value) / TileSize.X,
                                int.Parse(node.Attributes["y"].Value) / TileSize.Y + 1
                            );
                        }
                        break;

                    default: break;
                }
            }

            #endregion
        }
    }
}
