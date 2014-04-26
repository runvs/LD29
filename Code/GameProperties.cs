﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JamUtilities;
using SFML.Window;

namespace JamTemplate
{
    public static class GameProperties
    {
        public static float TileSizeInPixelOriginal { get {return 16.0f; } }
        public static float TileSizeInPixelScaled { get { return TileSizeInPixelOriginal  * SmartSprite._scaleVector.X; } }
        public static Vector2i WorldSizeInTiles { get; set; }
    }
}