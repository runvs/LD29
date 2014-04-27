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

        public static bool HasPickedUpHelm = false;
        public static bool ExplosionHasHappened = false;

        public static bool HasBeenToGenerator = false;
        public static bool HasPickedUpCable = false;
        public static bool HasRepairedGenerator = false;

        public static int MinersRescued = 0;

        public static bool HasBeenToDrill = false;
        public static bool HasPickedUpDrillHead = false;
        public static bool HasRepairedDrill = false;

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
            _world.AddSpeechBubble("Now I'm trapped beneath the surface. I hope there's another way out!",
                new Vector2f(_world._player.AbsolutePositionInPixel.X, _world._player.AbsolutePositionInPixel.Y - 200));
        }
        internal static void WhatToDo(object obj)
        {
            _world.AddSpeechBubble("First we need some light. Let's power the generator to the left.",
                         new Vector2f(_world._player.AbsolutePositionInPixel.X - 150, _world._player.AbsolutePositionInPixel.Y - 256));
        }

        internal static void VisitGeneratorArea(object obj)
        {
            if (HasPickedUpCable)
            {
                if (HasBeenToGenerator)
                {
                    _world.AddSpeechBubble("Now that we have light, let's find a way out of here...",
                        new Vector2f(_world._player.AbsolutePositionInPixel.X - 150, _world._player.AbsolutePositionInPixel.Y - 256));
                    RepairGenerator();
                }
                else
                {
                    _world.AddSpeechBubble("Oh that's what it's for. Now let's find a way out of here...",
                        new Vector2f(_world._player.AbsolutePositionInPixel.X - 150, _world._player.AbsolutePositionInPixel.Y - 256));
                    RepairGenerator();
                }
            }
            else
            {
                _world.AddSpeechBubble("I need to repair the generator. Some cables have been damaged by the collapse.",
                     new Vector2f(_world._player.AbsolutePositionInPixel.X - 150, _world._player.AbsolutePositionInPixel.Y - 256));
                HasBeenToGenerator = true;
            }
        }

        private static void RepairGenerator()
        {
            HasRepairedGenerator = true;
            Color col = GameProperties.ColorBlue1;
            col.A = 175;
            ScreenEffects.GetDynamicEffect("fadeOut").StartEffect(0.75f, 0.05f, col, 1);
            FloatRect rect = new FloatRect(128, 14 * GameProperties.TileSizeInPixelScaled, 128, 64);
            ParticleManager.CreateSparksSpawner(rect, GameProperties.ColorBlue1, 0.5f);
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
                Text = "This cable might be useful. I'll take it with me.";
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
            _world.AddSpeechBubble("There's my helmet.",
                         new Vector2f(_world._player.AbsolutePositionInPixel.X - 150, _world._player.AbsolutePositionInPixel.Y - 256));
            _world._player.ChangeSprite("../GFX/player_helmet.png");
            HasPickedUpHelm = true;
        }

        internal static void MoveIn(object obj)
        {
            if (!HasPickedUpHelm)
            {
                _world.AddSpeechBubble("I should get my helmet first. Its outside the mining cabin.",
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
            _world.AddSpeechBubble("Oh no. The ladder has also been damaged. I need to take the long way.",
                         new Vector2f(_world._player.AbsolutePositionInPixel.X - 150, _world._player.AbsolutePositionInPixel.Y - 256));
        }

        internal static void Finished(object obj)
        {
            _world.AddSpeechBubble("I finally managed to reach the surface. What a relief!",
                         new Vector2f(_world._player.AbsolutePositionInPixel.X - 150, _world._player.AbsolutePositionInPixel.Y - 256));
            Color col = GameProperties.ColorBlue1;
            ScreenEffects.GetDynamicEffect("fadeIn").StartEffect(5.0f, 0.05f, col, 1);
            col.A = 175;
            ScreenEffects.GetDynamicEffect("fadeOut").StartEffect(0.2f, 0.05f, col, 1);
            HasReachedExit = true;
        }

        private static void TellMinersStory()
        {
            if (MinersRescued == 1)
            {
                _world.AddSpeechBubble("I'm so happy you found me. Bad idea going down today.",
                        new Vector2f(_world._player.AbsolutePositionInPixel.X - 150, _world._player.AbsolutePositionInPixel.Y - 256));
            }
            else if (MinersRescued == 2)
            {
                _world.AddSpeechBubble("Edward was getting the new drill head from the southwest.",
                        new Vector2f(_world._player.AbsolutePositionInPixel.X - 150, _world._player.AbsolutePositionInPixel.Y - 256));
            }
            else if (MinersRescued == 3)
            {
                _world.AddSpeechBubble("There is a second exit in the east that could still be open.",
                        new Vector2f(_world._player.AbsolutePositionInPixel.X - 150, _world._player.AbsolutePositionInPixel.Y - 256));
            }
        }

        internal static void Miner1(object obj)
        {
            if (!HasRepairedGenerator)
            {
                _world.AddSpeechBubble("I can't see anything. Can someone switch on the lights?",
                               new Vector2f(_world._player.AbsolutePositionInPixel.X - 150, _world._player.AbsolutePositionInPixel.Y - 256));
                _world._player.ResetPathfinding();
                _world._player.SetWaypoint(new Vector2f(_world._player.AbsolutePositionInPixel.X - 128, _world._player.AbsolutePositionInPixel.Y));
                foreach (var area in _world._triggerAreaList)
                {
                    if (area.Type == TriggerAreaType.FUNCTION && area.Id == "Miner1")
                    {
                        area.ResetTriggered();
                    }
                }
            }
            else
            {
                MinersRescued++;
                TellMinersStory();
            }
        }

        internal static void Miner2(object obj)
        {
            if (!HasRepairedGenerator)
            {
                _world.AddSpeechBubble("I can't see anything. Can someone switch on the lights?",
                               new Vector2f(_world._player.AbsolutePositionInPixel.X - 150, _world._player.AbsolutePositionInPixel.Y - 256));
                _world._player.ResetPathfinding();
                _world._player.SetWaypoint(new Vector2f(_world._player.AbsolutePositionInPixel.X + 128, _world._player.AbsolutePositionInPixel.Y));
                foreach (var area in _world._triggerAreaList)
                {
                    if (area.Type == TriggerAreaType.FUNCTION && area.Id == "Miner2")
                    {
                        area.ResetTriggered();
                    }
                }
            }
            else
            {
                MinersRescued++;
                TellMinersStory();
            }
        }

        internal static void Miner3(object obj)
        {
            if (!HasRepairedGenerator)
            {
                _world.AddSpeechBubble("I can't see anything. Can someone switch on the lights?",
                               new Vector2f(_world._player.AbsolutePositionInPixel.X - 150, _world._player.AbsolutePositionInPixel.Y - 256));
                _world._player.ResetPathfinding();
                _world._player.SetWaypoint(new Vector2f(_world._player.AbsolutePositionInPixel.X + 128, _world._player.AbsolutePositionInPixel.Y));
                foreach (var area in _world._triggerAreaList)
                {
                    if (area.Type == TriggerAreaType.FUNCTION && area.Id == "Miner3")
                    {
                        area.ResetTriggered();
                    }
                }
            }
            else
            {
                MinersRescued++;
                TellMinersStory();
            }
        }

        internal static void DrillFound(object obj)
        {
            if (!HasRepairedGenerator)
            {
                _world.AddSpeechBubble("I can't see anything. Can someone switch on the lights?",
                               new Vector2f(_world._player.AbsolutePositionInPixel.X - 150, _world._player.AbsolutePositionInPixel.Y - 256));
                _world._player.ResetPathfinding();
                _world._player.SetWaypoint(new Vector2f(_world._player.AbsolutePositionInPixel.X + 64, _world._player.AbsolutePositionInPixel.Y + 64));
                foreach (var area in _world._triggerAreaList)
                {
                    if (area.Type == TriggerAreaType.FUNCTION && area.Id == "DrillFound")
                    {
                        area.ResetTriggered();
                    }
                }
            }
            else
            {
                if (MinersRescued < 3)
                {
                    _world.AddSpeechBubble("Please see for my three friends. They were in the shaft tunnels.",
                                new Vector2f(_world._player.AbsolutePositionInPixel.X - 150, _world._player.AbsolutePositionInPixel.Y - 256));

                    _world._player.ResetPathfinding();
                    _world._player.SetWaypoint(new Vector2f(_world._player.AbsolutePositionInPixel.X + 64, _world._player.AbsolutePositionInPixel.Y + 64));
                    foreach (var area in _world._triggerAreaList)
                    {
                        if (area.Type == TriggerAreaType.FUNCTION && area.Id == "DrillFound")
                        {
                            area.ResetTriggered();
                        }
                    }
                }
                else
                {
                    _world.AddSpeechBubble("Yes of course. Here is the drill head.",
                                new Vector2f(_world._player.AbsolutePositionInPixel.X - 150, _world._player.AbsolutePositionInPixel.Y - 256));
                    HasPickedUpDrillHead = true;
                }
            }

        }

        internal static void DrillMachine(object obj)
        {
            if (!HasPickedUpDrillHead)
            {
                _world.AddSpeechBubble("A state-of-the-art drill. Unfortunately the head has broken down.",
                               new Vector2f(_world._player.AbsolutePositionInPixel.X - 150, _world._player.AbsolutePositionInPixel.Y - 256));
                _world._player.ResetPathfinding();
                _world._player.SetWaypoint(new Vector2f(_world._player.AbsolutePositionInPixel.X - 128, _world._player.AbsolutePositionInPixel.Y));
                foreach (var area in _world._triggerAreaList)
                {
                    if (area.Type == TriggerAreaType.FUNCTION && area.Id == "DrillMachine")
                    {
                        area.ResetTriggered();
                    }
                }
            }
            else
            {
                _world.AddSpeechBubble("I place the drillhead.",
                               new Vector2f(_world._player.AbsolutePositionInPixel.X - 150, _world._player.AbsolutePositionInPixel.Y - 256));

                DoNiceDrillStuff();
                HasRepairedDrill = true;
            }
        }

        private static void DoNiceDrillStuff()
        {
            for (int i = 40; i != 46; i++)
            {
                // remove the middle layer
                _world._waypointGrid[i, 38] = PathFinderHelper.EMPTY_TILE;
                _world.RemoveTileAt(new Vector2i(i, 37));

                // remove the layer below and replace it with the correct tiles
                _world.RemoveTileAt(new Vector2i(i, 36));
                _world.AddTile(new Tile(i, 36, Tile.TileType.UNDERWORLD_BOTTOM));

                // remove the layer above and replace it with the correct tiles
                _world.RemoveTileAt(new Vector2i(i, 38));
                _world.AddTile(new Tile(i, 38, Tile.TileType.UNDERWORLD_TOP));
            }

            ParticleProperties props = new ParticleProperties();
            props.Type = ParticleManager.ParticleType.PT_SmokeCloud;
            props.col = GameProperties.ColorBrown4;
            props.lifeTime = 2.5f;
            props.sizeMultiple = 100.0f;
            props.sizeSingle = 8;
            props.RotationType = ParticleManager.ParticleRotationType.PRT_None;
            props.AffectedByGravity = false;
            var emitter = new ParticleEmitter(new FloatRect(40*GameProperties.TileSizeInPixelScaled, 37 * GameProperties.TileSizeInPixelScaled, 5 * GameProperties.TileSizeInPixelScaled, GameProperties.TileSizeInPixelScaled), props, 20);
            emitter.Update(3);
        }

        public static void ResetStory()
        {
            HasPickedUpHelm         = false;
            ExplosionHasHappened    = false;
        
            HasBeenToGenerator      = false;
            HasPickedUpCable        = false;
            HasRepairedGenerator    = false;
            
            MinersRescued           = 0;

            HasBeenToDrill          = false;
            HasPickedUpDrillHead    = false;
            HasRepairedDrill        = false;
        
            HasReachedExit          = false;
        }

        internal static void ThisWay(object obj)
        {
            _world.AddSpeechBubble("Let's go! This way.",
                               new Vector2f(_world._player.AbsolutePositionInPixel.X - 150, _world._player.AbsolutePositionInPixel.Y - 256));
        }

        internal static void UpNorth(object obj)
        {
            _world.AddSpeechBubble("Just go up and east.",
                               new Vector2f(_world._player.AbsolutePositionInPixel.X - 150, _world._player.AbsolutePositionInPixel.Y - 256));
        }
    }
}

