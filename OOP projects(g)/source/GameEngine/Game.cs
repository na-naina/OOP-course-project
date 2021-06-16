using System;
using System.Collections.Generic;
using System.Text;
using GLFW;

namespace OOP_projects.GameEngine
{
    abstract class Game
    {
        public void Run()
        {
            Init();
            Load();

            while(!ShouldTerminate())
            {
                
                OnUpdate();
                OnRender();
            }

            Terminate();
        }

        protected abstract void Init();
        protected abstract void Load();

        protected abstract void OnUpdate();
        protected abstract void OnRender();

        protected abstract bool ShouldTerminate();

        protected abstract void Terminate();

    }
}
