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
        public List<Tile> DecorationLayer { get; private set; }
        public List<Lamp> LampList { get; private set; }
        public List<TriggerArea> TriggerAreaList { get; private set; }
        public Vector2i PlayerPosition { get; private set; }
        public Vector2i WorldSize { get; private set; }
        public Vector2i TileSize { get; private set; }

        public MapParser(string fileName, bool isChangingLevel = false)
        {
            DecorationLayer = new List<Tile>();
            TerrainLayer = new List<Tile>();
            LampList = new List<Lamp>();
            TriggerAreaList = new List<TriggerArea>();

            var doc = new XmlDocument();
            doc.Load(fileName);

            int terrainOffset = int.Parse(doc.SelectSingleNode("/map/tileset[@name='Terrain']").Attributes["firstgid"].Value);
            int decorationOffset = int.Parse(doc.SelectSingleNode("/map/tileset[@name='Decoration']").Attributes["firstgid"].Value);

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
                Tile.TileType type = Tile.TileType.OVERWORLD_TOP;

                switch (gid)
                {
                    case 0:
                        type = Tile.TileType.UNDERWORLD_FLOAT;
                        break;
                    case 1:
                        type = Tile.TileType.UNDERWORLD_TOP_LEFT_RIGHT;
                        break;
                    case 2:
                        type = Tile.TileType.UNDERWORLD_TOP_BOTTOM_LEFT;
                        break;
                    case 3:
                        type = Tile.TileType.UNDERWORLD_TOP_BOTTOM_RIGHT;
                        break;
                    case 4:
                        type = Tile.TileType.UNDERWORLD_TOP_BOTTOM;
                        break;
                    case 5:
                        type = Tile.TileType.UNDERWORLD_TOP_LEFT;
                        break;
                    case 6:
                        type = Tile.TileType.UNDERWORLD_TOP_RIGHT;
                        break;
                    case 7:
                        type = Tile.TileType.UNDERWORLD_TOP;
                        break;
                    case 8:
                        type = Tile.TileType.OVERWORLD_FLOAT;
                        break;
                    case 9:
                        type = Tile.TileType.OVERWORLD_TOP_LEFT_RIGHT;
                        break;
                    case 10:
                        type = Tile.TileType.OVERWORLD_TOP_BOTTOM_LEFT;
                        break;
                    case 11:
                        type = Tile.TileType.UNDERWORLD_BOTTOM_LEFT_RIGHT;
                        break;
                    case 12:
                        type = Tile.TileType.OVERWORLD_TOP_BOTTOM_RIGHT;
                        break;
                    case 13:
                        type = Tile.TileType.UNDERWORLD_LEFT_RIGHT;
                        break;
                    case 14:
                        type = Tile.TileType.OVERWORLD_TOP_BOTTOM;
                        break;
                    case 15:
                        type = Tile.TileType.OVERWORLD_TOP_LEFT;
                        break;
                    case 16:
                        type = Tile.TileType.UNDERWORLD_BOTTOM_LEFT;
                        break;
                    case 17:
                        type = Tile.TileType.UNDERWORLD_BOTTOM_RIGHT;
                        break;
                    case 18:
                        type = Tile.TileType.OVERWORLD_TOP_RIGHT;
                        break;
                    case 19:
                        type = Tile.TileType.UNDERWORLD_LEFT;
                        break;
                    case 20:
                        type = Tile.TileType.UNDERWORLD_BOTTOM;
                        break;
                    case 21:
                        type = Tile.TileType.UNDERWORLD_RIGHT;
                        break;
                    case 22:
                        type = Tile.TileType.OVERWORLD_TOP;
                        break;
                    case 23:
                        type = Tile.TileType.UNDERWORLD_MID;
                        break;
                    case 24:
                        type = Tile.TileType.LADDER;
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

            #region Load the decorations layer

            xPos = yPos = 0;

            foreach (XmlNode layerNode in doc.SelectNodes("/map/layer[@name='Decorations']/data/tile"))
            {
                int gid = int.Parse(layerNode.Attributes["gid"].Value) - decorationOffset;
                Tile.TileType type = Tile.TileType.LAMP;

                switch (gid)
                {
                    // The mining building
                    // We get a lot of gids from the tileset for the building. Just use this (upper left corner)
                    case 0:
                        type = Tile.TileType.MINING_BUILDING;
                        break;

                    // Miner NPC
                    case 4:
                        type = Tile.TileType.MINER;
                        break;

                    // Right part of the generator
                    case 5:
                        type = Tile.TileType.GENERATOR_1;
                        break;

                    // Lamp
                    case 6:
                        type = Tile.TileType.LAMP;
                        LampList.Add(new Lamp(new Vector2f(
                            xPos * TileSize.X * SmartSprite._scaleVector.X,
                            yPos * TileSize.Y * SmartSprite._scaleVector.Y
                        )));
                        break;

                    // Left part of the generator
                    case 11:
                        type = Tile.TileType.GENERATOR_2;
                        break;

                    // Drill machine
                    case 12:
                        type = Tile.TileType.DRILL;
                        break;

                    // The cable that has to be picked up
                    case 13:
                        type = Tile.TileType.CABLE;
                        break;

                    default:
                        if (xPos != 0 && (xPos + 1) % WorldSize.X == 0)
                        {
                            yPos++;
                        }
                        xPos = (xPos + 1) % WorldSize.X;
                        continue;
                }

                DecorationLayer.Add(new Tile(xPos, yPos, type));

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
                            case "Function":
                                TriggerAreaList.Add(new TriggerArea(floatRect, TriggerAreaType.FUNCTION, id, true));
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
