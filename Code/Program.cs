using System;
using SFML.Graphics;
using SFML.Window;

namespace JamTemplate
{
    class Program
    {
        #region Event handlers

        static void OnClose(object sender, EventArgs e)
        {
            // Close the window when OnClose event is received
            SFML.Graphics.RenderWindow window = (SFML.Graphics.RenderWindow)sender;
            window.Close();
        }

        #endregion Event handlers

        static void Main(string[] args)
        {
            var applicationWindow = new RenderWindow(new VideoMode(800, 600, 32), "Escaping Scofield Mine");

            applicationWindow.SetFramerateLimit(60);
            applicationWindow.SetVerticalSyncEnabled(true);

            applicationWindow.Closed += new EventHandler(OnClose);

            Game myGame = new Game();
            JamUtilities.Mouse.Window = applicationWindow;


            int startTime = Environment.TickCount;
            int endTime = startTime;
            float time = 16.7f; // 60 fps -> 16.7 ms per frame

            while (applicationWindow.IsOpen())
            {
                if (startTime != endTime)
                {
                    time = (float)(endTime - startTime) / 1000.0f;
                }
                startTime = Environment.TickCount;

                applicationWindow.DispatchEvents();
                JamUtilities.Mouse.Update();

                myGame.GetInput();
                if (myGame.CanBeQuit)
                {
                    if (Keyboard.IsKeyPressed(Keyboard.Key.Escape))
                    {
                        applicationWindow.Close();
                    }
                }

                myGame.Update(time);

                myGame.Draw(applicationWindow);

                applicationWindow.Display();
                endTime = Environment.TickCount;
            }
        }
    }
}
