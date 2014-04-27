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
        public static bool HasBeenToGenerator = false;
        public static bool HasPickedUpCable = false;
        public static bool HasRepairedGenerator = false;
        public static bool HasPickedUpHelm = false;
        public static bool HasReachedExit = false;

        private static void DoExplosion()
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
            _world.PlayExplosionSound();
        }

        public static void CaveCollapse(object o = null)
        {
            var shake = ScreenEffects.GetDynamicEffect("shake");
            shake.StartEffect(1.0f, .025f, Color.White, 10.0f, ShakeDirection.AllDirections);

            ScreenEffects.GetDynamicEffect("fadeOut").StartEffect(0.75f, 0.05f, GameProperties.ColorBrown2, 2);

            DoExplosion();

            ExplosionHasHappened = true;


            for (int i = 9; i != 13; i++)
            {
                _world._waypointGrid[17, i] = PathFinderHelper.BLOCKED_TILE;
                _world.RemoveTileAt(new Vector2i(17, i));
            }

            // todo Spawn Debris and rocks, block the path
        }

        public static void TellMinerToGoIntoMine(object o = null)
        {
            _world.AddSpeechBubble("Meh, I am already late. I should hurry and get down in the mine.",
                new Vector2f(_world._player.AbsolutePositionInPixel.X, _world._player.AbsolutePositionInPixel.Y - 200));
        }

        internal static void SoWeWere(object obj)
        {
            _world.AddSpeechBubble("So we were trapped beneath the surface with this way out blocked.",
                new Vector2f(_world._player.AbsolutePositionInPixel.X, _world._player.AbsolutePositionInPixel.Y - 200));
        }
        internal static void WhatToDo(object obj)
        {
            _world.AddSpeechBubble("At first Light would be a great Idea. Let's power the generator to the left.",
                         new Vector2f(_world._player.AbsolutePositionInPixel.X - 150, _world._player.AbsolutePositionInPixel.Y - 256));
        }

        internal static void VisitGeneratorArea(object obj)
        {
            if (HasPickedUpCable)
            {
                if (HasBeenToGenerator)
                {
                    _world.AddSpeechBubble("I knew the generator could be repaired with the cable!",
                        new Vector2f(_world._player.AbsolutePositionInPixel.X - 150, _world._player.AbsolutePositionInPixel.Y - 256));
                    RepairGenerator();
                }
                else
                {
                    _world.AddSpeechBubble("Huch?",
                        new Vector2f(_world._player.AbsolutePositionInPixel.X - 150, _world._player.AbsolutePositionInPixel.Y - 256));
                    RepairGenerator();
                }
            }
            else
            {
                _world.AddSpeechBubble("We need to repair the Generator. Some Cables were damaged by the collapse.",
                     new Vector2f(_world._player.AbsolutePositionInPixel.X - 150, _world._player.AbsolutePositionInPixel.Y - 256));
                HasBeenToGenerator = true;
            }
        }

        private static void RepairGenerator()
        {
            HasRepairedGenerator = true;
            ScreenEffects.GetDynamicEffect("fadeOut").StartEffect(0.75f, 0.05f, GameProperties.ColorBlue1, 1);
        }

        internal static void PickupCable(object obj)
        {
            string Text = "";
            if (HasBeenToGenerator)
            {
                Text = "This cable could be useful to repair the generator.";
            }
            else
            {
                Text = "This cable might be useful. I'll take it with me";
            }
            HasPickedUpCable = true;
            _world.AddSpeechBubble(Text,
                    new Vector2f(_world._player.AbsolutePositionInPixel.X - 200, _world._player.AbsolutePositionInPixel.Y - 256));
            _world.RemoveDecorationAt(new Vector2i(5, 22));

            // Re-enable the generator's trigger area
            foreach (var area in _world._triggerAreaList)
            {
                if (area.Type == TriggerAreaType.FUNCTION && area.Id == "GeneratorArea")
                {
                    area.ResetTriggered();
                }
            }
        }

        internal static void Helm(object obj)
        {
            _world.AddSpeechBubble("*Pick up Helmet*.",
                         new Vector2f(_world._player.AbsolutePositionInPixel.X - 150, _world._player.AbsolutePositionInPixel.Y - 256));
            _world._player.ChangeSprite("../GFX/player_helmet.png");
            HasPickedUpHelm = true;
        }

        internal static void MoveIn(object obj)
        {
            if (!HasPickedUpHelm)
            {
                _world.AddSpeechBubble("I should get my Helm first. Its in the Corp's house.",
                         new Vector2f(_world._player.AbsolutePositionInPixel.X - 150, _world._player.AbsolutePositionInPixel.Y - 256));
                // move Player up
                _world._player.ResetPathfinding();
                _world._player.SetWaypoint(new Vector2f(_world._player.AbsolutePositionInPixel.X, _world._player.AbsolutePositionInPixel.Y - 64));
                foreach (var area in _world._triggerAreaList)
                {
                    if (area.Type == TriggerAreaType.FUNCTION && area.Id == "MoveIn")
                    {
                        area.ResetTriggered();
                    }
                }
            }
        }

        internal static void LadderDamaged(object obj)
        {
            _world.AddSpeechBubble("Oh no. The Ladder is damaged. I need to take the long way.",
                         new Vector2f(_world._player.AbsolutePositionInPixel.X - 150, _world._player.AbsolutePositionInPixel.Y - 256));
        }

        internal static void Finished(object obj)
        {
            _world.AddSpeechBubble("So I finally managed to reach the surface. What a relief",
                         new Vector2f(_world._player.AbsolutePositionInPixel.X - 150, _world._player.AbsolutePositionInPixel.Y - 256));
            Color col = GameProperties.ColorBlue1;
            col.A = 175;
            ScreenEffects.GetDynamicEffect("fadeIn").StartEffect(0.9f, 0.05f, col, 1);
            ScreenEffects.GetDynamicEffect("fadeOut").StartEffect(0.2f, 0.05f, col, 1);
            HasReachedExit = true;
        }



        
    }
}

