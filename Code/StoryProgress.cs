using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JamUtilities;
using JamUtilities.Particles;
using JamUtilities.ScreenEffects;
using SFML.Graphics;

namespace JamTemplate
{
    public static class StoryProgress
    {

        public static World _world;

        public static bool ExplosionHasHappened = false;

        public static void BasicExplosion(object o = null)
        {
            var shake = ScreenEffects.GetDynamicEffect("shake");
            shake.StartEffect(1.0f, .025f, Color.White, 10.0f, ShakeDirection.AllDirections);

            //ScreenEffects.GetDynamicEffect("fadeOut").StartEffect(0.75f, 0.05f, GameProperties.ColorBrown2, 2);

            ParticleProperties props = new ParticleProperties();
            props.Type = ParticleManager.ParticleType.PT_SmokeCloud;
            props.col = Color.Black;
            props.lifeTime = 4.0f;
            props.sizeMultiple = 100.0f;
            props.sizeSingle = 10;
            props.RotationType = ParticleManager.ParticleRotationType.PRT_Velocity;
            props.AffectedByGravity = true;
            var emitter = new ParticleEmitter(new FloatRect(960, 704, 128, 256), props, 4);
            emitter.Update(3);
            ExplosionHasHappened = true;

            // todo Spawn Debris and rocks, block the path
        }

        public static void TellMinerToGoIntoMine(object o = null)
        {
            _world.AddSpeechBubble("Meeh, I am already late. Should hurry and go down in the mine.", new SFML.Window.Vector2f(128, 45));
        }
    }
}
