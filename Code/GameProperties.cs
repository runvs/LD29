using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JamUtilities;
using SFML.Window;

namespace JamTemplate
{
    public static class GameProperties
    {

        #region Colors

        public static SFML.Graphics.Color ColorBlue1 { get { return new SFML.Graphics.Color(255, 255, 204); } }
        public static SFML.Graphics.Color ColorBlue2 { get { return new SFML.Graphics.Color(221, 238, 204); } }
        public static SFML.Graphics.Color ColorBlue3 { get { return new SFML.Graphics.Color(187, 221, 204); } }
        public static SFML.Graphics.Color ColorBlue4 { get { return new SFML.Graphics.Color(153, 204, 204); } }

        public static SFML.Graphics.Color ColorBrown1 { get { return new SFML.Graphics.Color(228, 132, 000); } }
        public static SFML.Graphics.Color ColorBrown2 { get { return new SFML.Graphics.Color(182, 101, 004); } }
        public static SFML.Graphics.Color ColorBrown3 { get { return new SFML.Graphics.Color(135, 069, 007); } }
        public static SFML.Graphics.Color ColorBrown4 { get { return new SFML.Graphics.Color(089, 038, 011); } }

        public static SFML.Graphics.Color ColorPink1 { get { return new SFML.Graphics.Color(200, 200, 220); } }
        public static SFML.Graphics.Color ColorPink2 { get { return new SFML.Graphics.Color(155, 155, 175); } }
        public static SFML.Graphics.Color ColorPink3 { get { return new SFML.Graphics.Color(109, 109, 130); } }
        public static SFML.Graphics.Color ColorPink4 { get { return new SFML.Graphics.Color(064, 064, 085); } }

        public static SFML.Graphics.Color ColorGreen1 { get { return new SFML.Graphics.Color(050, 180,  050); } }
        public static SFML.Graphics.Color ColorGreen2 { get { return new SFML.Graphics.Color(055, 148,  055); } }
        public static SFML.Graphics.Color ColorGreen3 { get { return new SFML.Graphics.Color(059, 117,  059); } }
        public static SFML.Graphics.Color ColorGreen4 { get { return new SFML.Graphics.Color(064, 085,  064); } }

        public static SFML.Graphics.Color ColorGrey1 { get { return new SFML.Graphics.Color(032,  032,  032); } }
        public static SFML.Graphics.Color ColorGrey2 { get { return new SFML.Graphics.Color(016,  016,  016); } }
        public static SFML.Graphics.Color ColorGrey3 { get { return new SFML.Graphics.Color(240,  240,  220); } }
        public static SFML.Graphics.Color ColorGrey4 { get { return new SFML.Graphics.Color(255,  255,  240); } }

        #endregion Colors


        public static float TileSizeInPixelOriginal { get {return 16.0f; } }
        public static float TileSizeInPixelScaled { get { return TileSizeInPixelOriginal  * SmartSprite._scaleVector.X; } }
        public static Vector2i WorldSizeInTiles { get; set; }

        public static float PlayerMaxVelocity { get { return 240.0f; } }

        public static float PlayerDistanceToWaypointAccepted { get { return 3.0f; } }


        public static float SpeechBubbleLifeTime { get { return 4.5f; } }
        public static float SpeechBubbleFadeTime { get { return 1.5f; } }

        public static int WorldWaterDropSpaces { get { return 20; } }
    }
}
