using System;
using JamUtilities;
using JamUtilities.Particles;
using JamUtilities.ScreenEffects;
using SFML.Audio;
using SFML.Graphics;
using SFML.Window;

namespace JamTemplate
{
    class Game
    {

        #region Fields

        private State _gameState;

        World _myWorld;

        Music _bgm; 
        float _timeTilNextInput = 0.0f;

        bool loading = false; 
        float _endingTimer;

        #endregion Fields

        #region Methods

        public Game()
        {
            // Predefine game state to menu
            _gameState = State.Menu;

            //TODO  Default values, replace with correct ones !
            SmartSprite._scaleVector = new Vector2f(4.0f, 4.0f);
            ScreenEffects.Init(new Vector2u(800, 600));
            //ParticleManager.SetPositionRect(new FloatRect(-500, 0, 1400, 600));
            ParticleManager.Gravity = new Vector2f(0,3);
            Camera.MinPosition = new Vector2f(0, -200);
            _endingTimer = 0.0f; 
            
            try
            {
                SmartText._font = new Font("../GFX/font.ttf");

                SmartText._lineLengthInChars =22;
                SmartText._lineSpread = 1.2f;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            _bgm = new Music("../SFX/music.ogg");
            _bgm.Volume = 75;
            _bgm.Loop = true;
            _bgm.Play();

        }

        public void GetInput()
        {
            if (_timeTilNextInput < 0.0f)
            {
                if (_gameState == State.Menu)
                {
                    GetInputMenu();
                }
                else if (_gameState == State.Game)
                {
                    _myWorld.GetInput();
                }
                else if (_gameState == State.Credits)
                {
                    GetInputCreditsScore();
                }
            }
        }

        private void GetInputMenu()
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.Return) || SFML.Window.Mouse.IsButtonPressed(SFML.Window.Mouse.Button.Left) )
            {
                loading = true;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.C))
            {
                ChangeGameState(State.Credits);
            }

        }

        private void GetInputCreditsScore()
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.Escape) || Keyboard.IsKeyPressed(Keyboard.Key.Return) || Keyboard.IsKeyPressed(Keyboard.Key.Space))
            {
                ChangeGameState(State.Menu, 1.0f);
            }
        }

        public void Update(float deltaT)
        {
            if (_timeTilNextInput >= 0.0f)
            {
                _timeTilNextInput -= deltaT;
            }

            CanBeQuit = false;
            if (_gameState == State.Game)
            {
                _myWorld.Update(Timing.Update(deltaT));

               // Game End Condition
                if (StoryProgress.HasReachedExit)
                {
                    _myWorld._player.StopPlayerWalkSound();

                    _endingTimer += deltaT;
                    if (_endingTimer >= 5)
                    {
                        ChangeGameState(State.Credits);
                    }
                    if(_bgm.Volume > 1.0f)
                    {
                        _bgm.Volume -= 75.0f/5.0f*deltaT;
                    }
                }
                else
                {
                    _bgm.Volume = 75;
                }

            }
            else if (_gameState == State.Menu && this._timeTilNextInput <= 0.0f)
            {
                _bgm.Volume = 75;
                CanBeQuit = true;
            }

        }

        public void Draw(RenderWindow rw)
        {
            rw.Clear();
            if (_gameState == State.Menu)
            {
                DrawMenu(rw);
            }
            else if (_gameState == State.Game)
            {
                _myWorld.Draw(rw);
            }
            else if (_gameState == State.Credits)
            {
                DrawCredits(rw);
            }
        }

        private void DrawMenu(RenderWindow rw)
        {
            if (!loading)
            {
                SmartText.DrawText("Escaping Scofield Mine", TextAlignment.MID, new Vector2f(400.0f, 150.0f), 1.5f, rw);

                SmartText.DrawText("Start [Click]", TextAlignment.MID, new Vector2f(400.0f, 250.0f), rw);
                //SmartText.DrawText("W A S D & LShift", TextAlignment.MID, new Vector2f(530.0f, 340.0f), rw);
                //SmartText.DrawText("Arrows & RCtrl", TextAlignment.MID, new Vector2f(180.0f, 340.0f), rw);

                SmartText.DrawText("[C]redits", TextAlignment.LEFT, new Vector2f(30.0f, 550.0f), rw);
                ScreenEffects.GetStaticEffect("vignette").Draw(rw);
            }
            else
            {
                SmartText.DrawText("Loading", TextAlignment.MID, new Vector2f(400.0f, 150.0f), 1.5f, rw);
                ScreenEffects.GetStaticEffect("vignette").Draw(rw);
                rw.Display();
                
                StartGame();
                
            }
        }

        private void DrawCredits(RenderWindow rw)
        {

            SmartText.DrawText("Thank you for playing", TextAlignment.MID, new Vector2f(400.0f, 2.0f), 0.75f, rw);
            SmartText.DrawText("Escaping Scofield Mine", TextAlignment.MID, new Vector2f(400.0f, 20.0f), 1.5f, rw);

            SmartText.DrawText("A Game by", TextAlignment.MID, new Vector2f(400.0f, 100.0f), 0.75f, rw);
            SmartText.DrawText("Julian Dinges & Simon Weis", TextAlignment.MID, new Vector2f(400.0f, 135.0f), rw);

            SmartText.DrawText("Visual Studio 2012 \t C#", TextAlignment.MID, new Vector2f(400, 170), 0.75f, rw);
            SmartText.DrawText("aseprite \t SFML.NET 2.1", TextAlignment.MID, new Vector2f(400, 200), 0.75f, rw);
            SmartText.DrawText("Cubase 5 \t SFXR", TextAlignment.MID, new Vector2f(400, 230), 0.75f, rw);
            SmartText.DrawText("AStarPathfinder \t AutoTileGen", TextAlignment.MID, new Vector2f(400, 260), 0.75f, rw);
            SmartText.DrawText("SVN \t SmartTimelapse", TextAlignment.MID, new Vector2f(400, 290), 0.75f, rw);

            SmartText.DrawText("Thanks to", TextAlignment.MID, new Vector2f(400, 350), 0.75f, rw);
            SmartText.DrawText("Families & Friends for their great support", TextAlignment.MID, new Vector2f(400, 375), 0.75f, rw);

            SmartText.DrawText("Created April 2014 for LD29", TextAlignment.MID, new Vector2f(400.0f, 500.0f), 0.75f, rw);
            ScreenEffects.GetStaticEffect("vignette").Draw(rw);
        }

        private void ChangeGameState(State newState, float inputdeadTime = 0.5f)
        {
            this._gameState = newState;
            _timeTilNextInput = inputdeadTime;
        }


        private void StartGame()
        {
            if (loading)
            {
                _myWorld = new World();
                ChangeGameState(State.Game, 0.1f);
                loading = false;
                StoryProgress.ResetStory();
            }
        }


        #endregion Methods

        #region Subclasses/Enums

        private enum State
        {
            Menu,
            Game,
            Credits
        }

        #endregion Subclasses/Enums


        public bool CanBeQuit { get; set; }
    }
}
