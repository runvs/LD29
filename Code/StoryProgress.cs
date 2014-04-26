using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeenGames.Utils.AStarPathFinder;
using JamUtilities;
using JamUtilities.Particles;
using JamUtilities.ScreenEffects;
using SFML.Graphics;
using SFML.Window;

namespace JamTemplate
{
    public static class StoryProgress
    {

        public static World _world;

        public static bool ExplosionHasHappened = false;

        private static void DoExplosion(  )
        {
            ParticleProperties props = new ParticleProperties();
            props.Type = ParticleManager.ParticleType.PT_SmokeCloud;
            props.col = GameProperties.ColorBrown2;
            props.lifeTime = 2.5f;
            props.sizeMultiple = 100.0f;
            props.sizeSingle = 8;
            props.RotationType = ParticleManager.ParticleRotationType.PRT_None;
            props.AffectedByGravity = false;
            var emitter = new ParticleEmitter(new FloatRect(960, 704, 128, 256), props, 5);
            emitter.Update(3);

            props.col = Color.Black;
            props.sizeSingle = 12;
            props.lifeTime = 5.5f;
            emitter = new ParticleEmitter(new FloatRect(960, 704, 128, 256), props, 14);
            emitter.Update(3);
        }

        public static void CaveCollapse(object o = null)
        {
            var shake = ScreenEffects.GetDynamicEffect("shake");
            shake.StartEffect(1.0f, .025f, Color.White, 10.0f, ShakeDirection.AllDirections);

            ScreenEffects.GetDynamicEffect("fadeOut").StartEffect(0.75f, 0.05f, GameProperties.ColorBrown2, 2);

            DoExplosion();

            ExplosionHasHappened = true;



            for(int i = 9; i != 13; i++)
            {
                _world._waypointGrid[17, i] = PathFinderHelper.BLOCKED_TILE;
                _world.RemoveTileAt(new Vector2i(17, i));
            }

            // todo Spawn Debris and rocks, block the path
        }

        public static void TellMinerToGoIntoMine(object o = null)
        {
            _world.AddSpeechBubble("Meeh, I am already late. I should hurry and get down in the mine.", 
                new Vector2f(_world._player.AbsolutePositionInPixel.X, _world._player.AbsolutePositionInPixel.Y - 200));
        }

        internal static void SoWeWere(object obj)
        {
            _world.AddSpeechBubble("So we were trapped beneath the surface with this way out blocked.", 
                new Vector2f(_world._player.AbsolutePositionInPixel.X, _world._player.AbsolutePositionInPixel.Y - 256));
        }

        internal static void NoEscape(object obj)
        {
            //_world.AddSpeechBubble("So we were trapped beneath the surface with this way out blocked.");
        }
    }
}
