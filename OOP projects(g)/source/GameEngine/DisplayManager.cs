using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Numerics;
using GLFW;
using static OOP_projects.OpenGL.GL;

namespace OOP_projects.GameEngine
{
    static class DisplayManager
    {
        public static Window Window { get; set; }
        public static Vector2 WindowSize { get; set; }
        public static void CreateWindow(int width, int height, string title)
        {
            WindowSize = new Vector2(width, height);

            Glfw.Init();


            Glfw.WindowHint(Hint.ContextVersionMajor, 3);
            Glfw.WindowHint(Hint.ContextVersionMinor, 3);
            Glfw.WindowHint(Hint.OpenglProfile, Profile.Core);

            Glfw.WindowHint(Hint.Focused, true);

            Glfw.WindowHint(Hint.Resizable, false);

            Window = Glfw.CreateWindow(width, height, title, Monitor.None, Window.None);

            if(Window == Window.None)
            {
                Console.WriteLine("Error: Display manager failed to create a window");
                return;
            }

            

            Glfw.MakeContextCurrent(Window);
            Import(Glfw.GetProcAddress);

            glViewport(0, 0, width, height);
            Glfw.SwapInterval(0);
        }

        public static void CloseWindow()
        {
            Glfw.Terminate();
        }
    }
}
