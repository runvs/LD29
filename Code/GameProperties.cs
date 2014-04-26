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

        public static SFML.Graphics.Color ColorBlue1 { get { return new SFML.Graphics.Color(255, 255, 204); } }
        public static SFML.Graphics.Color ColorBlue2 { get { return new SFML.Graphics.Color(221, 238, 204); } }
        public static SFML.Graphics.Color ColorBlue3 { get { return new SFML.Graphics.Color(187, 221, 204); } }
        public static SFML.Graphics.Color ColorBlue4 { get { return new SFML.Graphics.Color(153, 204, 204); } }


        //228 132 000  Untitled
        //182 101 004  Untitled
        //135 069 007  Untitled
        //089 038 011  Untitled

        //200 200 220  Untitled
        //155 155 175  Untitled
        //109 109 130  Untitled
        //064 064 085  Untitled

        //050 180 050  Untitled
        //055 148 055  Untitled
        //059 117 059  Untitled
        //064 085 064  Untitled

        //032 032 032  Untitled
        //016 016 016  Untitled
        //240 240 220  Untitled
        //255 255 240  Untitled

        public static float TileSizeInPixelOriginal { get {return 16.0f; } }
        public static float TileSizeInPixelScaled { get { return TileSizeInPixelOriginal  * SmartSprite._scaleVector.X; } }
        public static Vector2i WorldSizeInTiles { get; set; }

        public static float PlayerMaxVelocity { get { return 240.0f; } }

        public static float PlayerDistanceToWaypointAccepted { get { return 5.0f; } }

        
    }
}
