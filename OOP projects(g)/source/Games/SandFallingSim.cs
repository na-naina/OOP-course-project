using System;
using System.Numerics;
using GLFW;
using System.Drawing;
using System.Runtime.InteropServices;
using OOP_projects.GameEngine;
using static OOP_projects.GameEngine.CustomMath;
using OOP_projects.GraphicsEngine;
using static OOP_projects.OpenGL.GL;


namespace OOP_projects.Games
{
    class SandFallingSim : Game
    {
        private GameWorld World;
        private UI ui;
        private Shader shader;



        public SandFallingSim(uint H, uint W, int sizex = 300, int sizey = 300)
        {
            World = new GameWorld(sizex, sizey);
            DisplayManager.CreateWindow((int)H, (int)W, "Cellular Sandbox");
        }

        protected override void Init()
        {

            glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
            glEnable(GL_BLEND);

            glClearColor(Constantas.empty_color[0], Constantas.empty_color[1], Constantas.empty_color[2], Constantas.empty_color[3]);
            Renderer.Init();


            
            ui = new UI(World);


            Glfw.SetDropCallback(DisplayManager.Window, ui.drop_callback);
        }

        protected override void Load()
        {

            shader = new Shader("Resources/Shaders/BatchRendering.shader");

            int[] samplers = new int[32];
            for (int i = 0; i < 32; i++)
                samplers[i] = i;
            shader.Bind();
            shader.SetUniform1iv("u_Textures", 32, samplers);
            shader.SetUniformMat4f("u_ViewProj", Matrix4x4.Identity);
        }

        

        protected override void OnUpdate()
        {
            GameTime.DeltaTime = (float)Glfw.Time - GameTime.TotalElapsedSeconds;
            GameTime.TotalElapsedSeconds = (float)Glfw.Time;

            ui.Update();

            if (ui.pause == 1)
                World.Update();

            Glfw.PollEvents();
            
            Console.WriteLine(1f / GameTime.DeltaTime);
        }
        protected override void OnRender()
        {
            glClear(GL_COLOR_BUFFER_BIT);
            World.Render();
            ui.Render();
            Glfw.SwapBuffers(DisplayManager.Window);
        }

        protected override bool ShouldTerminate()
        {
            return Glfw.WindowShouldClose(DisplayManager.Window);
        }

        protected override void Terminate()
        {
            Renderer.Terminate();
            DisplayManager.CloseWindow();
        }








        private class GameWorld
        {
            private Vector2Int size;
            private particle[,] WorldData;

            public Vector2Int getSize { get { return size; } }
            public particle[,] getData { get { return WorldData; } }

            private static Random randomGen = new Random();
            public GameWorld(int sizeX = 300, int sizeY = 300)
            {
                size = new Vector2Int(sizeX, sizeY);
                WorldData = new particle[size.X, size.Y];
                for (int i = 0; i < size.X; i++)
                {
                    for (int j = 0; j < size.X; j++)
                    {
                        WorldData[i, j] = new empty_particle();
                    }
                }
                
            }
            public ParticleId this[int x, int y]
            {
                get
                {
                    return WorldData[x, y].id;
                }
                set
                {
                    if (!inBounds(x, y) ||  y > size.Y)
                        return;

                    switch (value)
                    {
                        case ParticleId.Sand:
                            WorldData[x, y] = new sand_particle();
                            break;

                        case ParticleId.Water:
                            WorldData[x, y] = new water_particle();
                            break;

                        case ParticleId.Metal:
                            WorldData[x, y] = new metal_particle();
                            break;

                        case ParticleId.Sparkle:
                            WorldData[x, y] = new sparkle_particle();
                            break;

                        case ParticleId.Smoke:
                            WorldData[x, y] = new smoke_particle();
                            break;

                        case ParticleId.Wood:
                            WorldData[x, y] = new wood_particle();
                            break;

                        case ParticleId.Fire:
                            WorldData[x, y] = new fire_particle();
                            break;

                        case ParticleId.Oil:
                            WorldData[x, y] = new oil_particle();
                            break;

                        case ParticleId.Steam:
                            WorldData[x, y] = new steam_particle();
                            break;

                        case ParticleId.Acid:
                            WorldData[x, y] = new acid_particle();
                            break;

                        default:
                            WorldData[x, y] = new empty_particle();
                            break;

                    }
                }
            }


            private int updatecycle = 1;
            public void Update()
            {
                updatecycle *= -1;

                for (int j = size.Y - 1; j >= 0 && j < size.Y; j -= 1)
                {
                    for (int i = (updatecycle == 1) ? 0 : size.X - 1; i < size.X && i >= 0; i += updatecycle)
                    {
                        if (WorldData[i, j].updated)
                            continue;

                        WorldData[i, j].updated = true;

                        switch (this[i, j])
                        {
                            case ParticleId.Empty:
                                continue;

                            case ParticleId.Sand:
                                UpdateSand(i, j);
                                continue;

                            case ParticleId.Water:
                                UpdateWater(i, j);
                                continue;

                            case ParticleId.Sparkle:
                                UpdateSparkle(i, j);
                                continue;

                            case ParticleId.Smoke:
                                UpdateSmoke(i, j);
                                continue;

                            case ParticleId.Steam:
                                UpdateSmoke(i, j);
                                continue;

                            case ParticleId.Fire:
                                UpdateFire(i, j);
                                continue;

                            case ParticleId.Oil:
                                UpdateOil(i, j);
                                continue;

                            case ParticleId.Acid:
                                UpdateAcid(i, j);
                                continue;

                            default:
                                continue;
                        }
                    }
                }



                for (int i = 0; i < size.X; i++)
                    for (int j = 0; j < size.Y; j++)
                    {
                        WorldData[i, j].updated = false;
                    }


            }

            public void Render()
            {
                Renderer.BeginBatch();
                float gridHBegin = (float)size.Y * 1.0f / (float)size.Y - 2.0f / (float)size.Y;
                float gridWBegin = -(float)size.X * 1.0f / (float)size.X;
                for (int i = 0; i < size.X; i++)
                    for (int j = 0; j < size.Y; j++)
                    {
                        if(this[i, j] == ParticleId.Empty)
                        {
                            continue;
                        }
                        Vector4 color = WorldData[i, j].color;
                        Vector2 position = new Vector2(gridWBegin + (float)i * 2.0f / (float)this.size.X, gridHBegin - (float)j * 2.0f / (float)this.size.Y);
                        Vector2 size = new Vector2(2.0f / (float)this.size.X, 2.0f / (float)this.size.Y);
                        Renderer.DrawQuad(position, size, color);
                    }
                Renderer.EndBatch();
                Renderer.Flush();
            }




            private void Swap(int x1, int y1, int x2, int y2)
            {
                particle temp = WorldData[x1, y1];
                WorldData[x1, y1] = WorldData[x2, y2];
                WorldData[x2, y2] = temp;
                WorldData[x1, y1].updated = true;
            }


            private bool inBounds(int x, int y)
            {
                return (x > 0 && x < size.X && y > 0 && y < size.Y);
            }



            private bool flameable(ParticleId id)
            {
                return id == ParticleId.Wood || id == ParticleId.Oil;
            }

            private bool gas(ParticleId id)
            {
                return id == ParticleId.Empty || id == ParticleId.Smoke || id == ParticleId.Steam;
            }

            private bool liquid(ParticleId id)
            {
                return id == ParticleId.Water || id == ParticleId.Oil || id == ParticleId.Acid;
            }

            private bool solid(ParticleId id)
            {
                return id == ParticleId.Sand || id == ParticleId.Metal || id == ParticleId.Wood;
            }

            private float dtVal(uint value)
            {
                return (float)value * GameTime.DeltaTime * 30.0f;
            }









            public enum ParticleId
            {
                Empty,
                Sand,
                Water,
                Metal,
                Fire,
                Smoke,
                Wood,
                Sparkle,
                Oil,
                Steam,
                Acid
            }

            abstract public class particle
            {
                public ParticleId id;
                public float life_time;
                public Vector2 velocity;
                public Vector2Int pos;
                public Vector4 color;
                public bool updated;
            }


            class empty_particle : particle
            {
                public empty_particle()
                {
                    id = ParticleId.Empty;
                    color = new Vector4(Constantas.empty_color[0], Constantas.empty_color[1], Constantas.empty_color[2], Constantas.empty_color[3]);

                    life_time = 0;
                    velocity = new Vector2();
                    pos = new Vector2Int();
                    updated = false;
                }
            }

            class sand_particle : particle
            {
                public sand_particle()
                {
                    id = ParticleId.Sand;
                    color = new Vector4(Constantas.sand_color[0], Constantas.sand_color[1], Constantas.sand_color[2], Constantas.sand_color[3]);

                    life_time = 0;
                    velocity = new Vector2(0.0f, 1.0f);
                    pos = new Vector2Int();
                    updated = false;
                }

            }

            class water_particle : particle
            {
                public water_particle()
                {
                    id = ParticleId.Water;
                    color = new Vector4(Constantas.water_color[0], Constantas.water_color[1], Constantas.water_color[2], Constantas.water_color[3]);

                    life_time = 0;
                    velocity = new Vector2(0.0f, 1.0f);
                    pos = new Vector2Int();
                    updated = false;
                }
            }

            class acid_particle : particle
            {
                public acid_particle()
                {
                    id = ParticleId.Acid;
                    color = new Vector4(Constantas.acid_color[0], Constantas.acid_color[1], Constantas.acid_color[2], Constantas.acid_color[3]);

                    life_time = 0;
                    velocity = new Vector2(0.0f, 1.0f);
                    pos = new Vector2Int();
                    updated = false;
                }
            }

            class oil_particle : particle
            {
                public oil_particle()
                {
                    id = ParticleId.Oil;
                    color = new Vector4(Constantas.oil_color[0], Constantas.oil_color[1], Constantas.oil_color[2], Constantas.oil_color[3]);

                    life_time = 0;
                    velocity = new Vector2(0.0f, 1.0f);
                    pos = new Vector2Int();
                    updated = false;
                }
            }

            class metal_particle : particle
            {
                public metal_particle()
                {
                    id = ParticleId.Metal;
                    color = new Vector4(Constantas.metal_color[0], Constantas.metal_color[1], Constantas.metal_color[2], Constantas.metal_color[3]);

                    life_time = 0;
                    velocity = new Vector2();
                    pos = new Vector2Int();
                    updated = false;
                }
            }

            class wood_particle : particle
            {
                public wood_particle()
                {
                    id = ParticleId.Wood;
                    color = new Vector4(Constantas.wood_color[0], Constantas.wood_color[1], Constantas.wood_color[2], Constantas.wood_color[3]);

                    life_time = 0;
                    velocity = new Vector2(0.0f, 0.0f);
                    pos = new Vector2Int();
                    updated = false;
                }
            }

            class sparkle_particle : particle
            {
                public sparkle_particle()
                {
                    id = ParticleId.Sparkle;
                    color = new Vector4(Constantas.sparkle_color[0], Constantas.sparkle_color[1], Constantas.sparkle_color[2], Constantas.sparkle_color[3]);

                    life_time = 15;
                    velocity = new Vector2();
                    pos = new Vector2Int();
                    updated = false;
                }
            }

            class smoke_particle : particle
            {
                public smoke_particle()
                {
                    id = ParticleId.Smoke;
                    color = new Vector4(Constantas.smoke_color[0], Constantas.smoke_color[1], Constantas.smoke_color[2], Constantas.smoke_color[3]);

                    life_time = randomGen.Next(50, 450);
                    velocity = new Vector2();
                    pos = new Vector2Int();
                    updated = false;
                }
            }

            class steam_particle : particle
            {
                public steam_particle()
                {
                    id = ParticleId.Steam;
                    color = new Vector4(Constantas.steam_color[0], Constantas.steam_color[1], Constantas.steam_color[2], Constantas.steam_color[3]);

                    life_time = life_time = randomGen.Next(50, 200);
                    
                    velocity = new Vector2();
                    pos = new Vector2Int();
                    updated = false;
                }
            }

            class fire_particle : particle
            {
                
                public fire_particle(float y_velocity = 0)
                {
                    id = ParticleId.Fire;
                    color = new Vector4(Constantas.fire_color[0], Constantas.fire_color[1], Constantas.fire_color[2], Constantas.fire_color[3]);

                    life_time = randomGen.Next(50, 150);
                    velocity = new Vector2(0.0f, y_velocity);
                    pos = new Vector2Int();
                    updated = false;
                }
            }







            void UpdateSand(int x, int y)
            {
                int rand = randomGen.Next(0, 2);
                rand = (rand == 0) ? -1 : 1;

                WorldData[x, y].velocity.Y += Constantas.gravity * GameTime.DeltaTime;
                int y_vel = (int)WorldData[x, y].velocity.Y;
                for (int i = 0; i < y_vel && inBounds(x, y + 1); i++)
                {

                    if (gas(this[x, y + 1]) || liquid(this[x, y + 1]) || this[x, y + 1] == ParticleId.Fire)
                    {
                        Swap(x, y + 1, x, y);
                        y++;
                        continue;
                    }

                    if (inBounds(x + rand, y + 1) && (gas(this[x + rand, y + 1]) || this[x + rand, y + 1] == ParticleId.Fire
                    || liquid(this[x + rand, y + 1])))
                    {
                        Swap(x + rand, y + 1, x, y);
                        y++;
                        x += rand;
                        continue;
                    }
                    if (inBounds(x - rand, y + 1) && (gas(this[x - rand, y + 1]) || this[x - rand, y + 1] == ParticleId.Fire
                    || liquid(this[x - rand, y + 1])))
                    {
                        Swap(x - rand, y + 1, x, y);
                        y++;
                        x -= rand;
                        continue;
                    }




                    WorldData[x, y].velocity.Y = Math.Max(0, WorldData[x, y].velocity.Y - 0.5f);
                    return;


                }


            }
            void UpdateWater(int x, int y)
            {
                int rand = randomGen.Next(0, 2);
                rand = (rand == 0) ? -1 : 1;


                bool flag = false;

                int t = 0;

                int y_val = (int)Math.Round(dtVal(Constantas.water_fall_speed));

                if (inBounds(x, y - 1) && this[x, y - 1] == ParticleId.Fire)
                {
                    this[x, y - 1] = ParticleId.Steam;
                }

                while (y_val - t > 0 && inBounds(x, y + 1) && (gas(this[x, y + 1]) 
                    || this[x, y + 1] == ParticleId.Oil || this[x, y + 1] == ParticleId.Acid))
                {
                    Swap(x, y + 1, x, y);
                    y++;
                    flag = true;
                    t++;
                }
                if (inBounds(x, y + 1) && this[x, y + 1] == ParticleId.Fire)
                {
                    this[x, y + 1] = ParticleId.Steam;
                }
                while (y_val - t > 0 && inBounds(x + rand, y + 1) && (gas(this[x + rand, y + 1]) /* == ParticleId.Empty*/ 
                    || this[x + rand, y + 1] == ParticleId.Oil || this[x + rand, y + 1] == ParticleId.Acid))
                {
                    Swap(x + rand, y + 1, x, y);
                    y++;
                    x += rand;
                    flag = true;
                    t++;
                }
                if (inBounds(x + rand, y + 1) && this[x + rand, y + 1] == ParticleId.Fire)
                {
                    this[x + rand, y + 1] = ParticleId.Steam;
                }

                while (y_val - t > 0 && inBounds(x - rand, y + 1) && (gas(this[x - rand, y + 1]) /* == ParticleId.Empty*/ 
                    || this[x - rand, y + 1] == ParticleId.Oil || this[x - rand, y + 1] == ParticleId.Acid))
                {

                    Swap(x - rand, y + 1, x, y);
                    y++;
                    x -= rand;
                    flag = true;
                    t++;
                }

                if (inBounds(x - rand, y + 1) && this[x - rand, y + 1] == ParticleId.Fire)
                {
                    this[x - rand, y + 1] = ParticleId.Steam;
                }

                if (flag)
                    return;

                int x_val = (int)Math.Round(dtVal(Constantas.water_spread_factor));

                for (int i = 0; i < x_val; i++)
                {
                    if (inBounds(x + rand, y) && (this[x + rand, y] == ParticleId.Empty 
                        || this[x + rand, y] == ParticleId.Oil || this[x + rand, y] == ParticleId.Acid))
                    {
                        Swap(x + rand, y, x, y);
                        flag = true;
                        x += rand;
                        continue;
                    }
                    break;
                }
                if (flag)
                    return;


                for (int i = 0; i < x_val; i++)
                {
                    if (inBounds(x - rand, y) && (this[x - rand, y] == ParticleId.Empty 
                        || this[x - rand, y] == ParticleId.Oil || this[x - rand, y] == ParticleId.Acid))
                        {
                            Swap(x - rand, y, x, y);
                            x -= rand;
                            continue;
                        }
                    break;
                }



            }
            void UpdateSparkle(int x, int y)
            {
                particle current = WorldData[x, y];
                this[x, y] = ParticleId.Empty;
                current.life_time -= dtVal(1);

                int rnd = randomGen.Next(0, 4);
                if(current.life_time <= 0)
                {
                    this[x, y] = ParticleId.Smoke;
                    return;
                }

                switch(rnd)
                {
                    case 0:
                        if (inBounds(x, y + 1)) 
                        {
                            if ((this[x, y + 1] == ParticleId.Empty))
                            {
                                WorldData[x, y + 1] = current;
                            }
                            if(flameable(this[x, y + 1]))
                            {
                                rnd = randomGen.Next(0, 3);
                                if(rnd == 0)
                                this[x, y + 1] = ParticleId.Fire;
                            }
                        }

                        return;
                    case 1:
                        if (inBounds(x, y - 1))
                        {
                            if ((this[x, y - 1] == ParticleId.Empty))
                            {
                                WorldData[x, y - 1] = current;
                            }
                            if (flameable(this[x, y - 1]))
                            {
                                rnd = randomGen.Next(0, 3);
                                if (rnd == 0)
                                    this[x, y - 1] = ParticleId.Fire;
                            }
                        }
                        return;

                    case 2:
                        if (inBounds(x + 1, y))
                        {
                            if ((this[x + 1, y] == ParticleId.Empty))
                            {
                                WorldData[x + 1, y] = current;
                            }
                            if (flameable(this[x + 1, y]))
                            {
                                rnd = randomGen.Next(0, 3);
                                if (rnd == 0)
                                    this[x + 1, y] = ParticleId.Fire;
                            }
                        }
                        return;

                    case 3:
                        if (inBounds(x - 1, y))
                        {
                            if ((this[x - 1, y] == ParticleId.Empty))
                            {
                                WorldData[x - 1, y] = current;
                            }
                            if (flameable(this[x - 1, y]))
                            {
                                rnd = randomGen.Next(0, 3);
                                if (rnd == 0)
                                    this[x - 1, y] = ParticleId.Fire;
                            }
                        }
                        return;

                }

            }
            void UpdateSmoke(int x, int y)
            {
                WorldData[x, y].life_time -= dtVal(1);
                if(WorldData[x, y].life_time <= 0)
                {
                    this[x, y] = ParticleId.Empty;
                    return;
                }
                int rand = randomGen.Next(0, 2);
                rand = (rand == 0) ? -1 : 1;


                int y_val = (int)Math.Round(dtVal(Constantas.smoke_floating_speed));

                for (int i = 0; i < y_val && inBounds(x, y - 1); i++)
                {
                    
                        if (this[x, y - 1] == ParticleId.Empty)
                        {
                            Swap(x, y - 1, x, y);
                            y--;
                            continue;
                        }


                        if (inBounds(x + rand, y - 1))
                            if (this[x + rand, y - 1] == ParticleId.Empty)
                            {
                                Swap(x + rand, y - 1, x, y);
                                y--;
                                x += rand;
                                continue;
                            }
                        if (inBounds(x - rand, y - 1))
                            if (this[x - rand, y - 1] == ParticleId.Empty)
                            {
                                Swap(x - rand, y - 1, x, y);
                                y--;
                                x -= rand;
                                continue;
                            }

                   
                    return;

                }
            }
            void UpdateFire(int x, int y)
            {
                WorldData[x, y].life_time -= dtVal(1);
                if (WorldData[x, y].life_time <= 0)
                {
                    this[x, y] = ParticleId.Empty;
                    return;
                }

                int rnd = randomGen.Next(0, 100);



                if (rnd < 3 && inBounds(x, y - 1) && this[x, y - 1] == ParticleId.Empty)
                {
                    this[x, y - 1] = ParticleId.Sparkle;
                }
                if (rnd <= Constantas.fire_smoke_generation_factor && inBounds(x, y - 1) && this[x, y - 1] == ParticleId.Empty)
                {
                    this[x, y - 1] = ParticleId.Smoke;
                }

                if (rnd <= Constantas.fire_spread_factor)
                {
                    rnd = randomGen.Next(0, 4);

                    switch (rnd)
                    {
                        case 0:
                            if (inBounds(x, y + 1) && flameable(this[x, y + 1]))
                            {
                                this[x, y + 1] = ParticleId.Fire;
                            }
                            break;
                        case 1:
                            if (inBounds(x, y - 1) && flameable(this[x, y - 1]))
                            {
                                this[x, y - 1] = ParticleId.Fire;
                            }
                            break;
                        case 2:
                            if (inBounds(x + 1, y) && flameable(this[x + 1, y]))
                            {
                                this[x + 1, y] = ParticleId.Fire;
                            }
                            break;
                        case 3:
                            if (inBounds(x - 1, y) && flameable(this[x - 1, y]))
                            {
                                this[x - 1, y] = ParticleId.Fire;
                            }
                            break;


                    }
                }

                int rand = randomGen.Next(0, 2);
                rand = (rand == 0) ? -1 : 1;

                WorldData[x, y].velocity.Y += Constantas.gravity/5 * GameTime.DeltaTime;
                int y_vel = (int)WorldData[x, y].velocity.Y;
                for (int i = 0; i < y_vel && inBounds(x, y + 1); i++)
                {

                    if (gas(this[x, y + 1]))
                    {
                        Swap(x, y + 1, x, y);
                        y++;
                        continue;
                    }




                    if (inBounds(x + rand, y + 1) && gas(this[x + rand, y + 1]))
                    {
                        Swap(x + rand, y + 1, x, y);
                        y++;
                        x += rand;
                        continue;
                    }




                    if (inBounds(x - rand, y + 1) && gas(this[x - rand, y + 1]))
                    {
                        Swap(x - rand, y + 1, x, y);
                        y++;
                        x -= rand;
                        continue;
                    }




                    WorldData[x, y].velocity.Y = Math.Max(0, WorldData[x, y].velocity.Y - 0.5f);
                    break;
                }



                
                


            }
            void UpdateOil(int x, int y)
            {
                int rand = randomGen.Next(0, 2);
                rand = (rand == 0) ? -1 : 1;


                bool flag = false;

                int t = 0;

                int y_val = (int)Math.Round(dtVal(Constantas.oil_fall_speed));

              

                while (y_val - t > 0 && inBounds(x, y + 1) && gas(this[x, y + 1]))
                {
                    Swap(x, y + 1, x, y);
                    y++;
                    flag = true;
                    t++;
                }
               
                while (y_val - t > 0 && inBounds(x + rand, y + 1) && gas(this[x + rand, y + 1]))
                {
                    Swap(x + rand, y + 1, x, y);
                    y++;
                    x += rand;
                    flag = true;
                    t++;
                }
             
                while (y_val - t > 0 && inBounds(x - rand, y + 1) && gas(this[x - rand, y + 1]))
                {

                    Swap(x - rand, y + 1, x, y);
                    y++;
                    x -= rand;
                    flag = true;
                    t++;
                }

              

                if (flag)
                    return;

                int x_val = (int)Math.Round(dtVal(Constantas.oil_spread_factor));

                for (int i = 0; i < x_val; i++)
                {
                    if (inBounds(x + rand, y) && (this[x + rand, y] == ParticleId.Empty || this[x + rand, y] == ParticleId.Fire))
                    {
                        Swap(x + rand, y, x, y);
                        flag = true;
                        x += rand;
                        continue;
                    }
                    break;
                }
                if (flag)
                    return;


                for (int i = 0; i < x_val; i++)
                {
                    if (inBounds(x - rand, y) && (this[x - rand, y] == ParticleId.Empty || this[x - rand, y] == ParticleId.Fire))
                    {
                        Swap(x - rand, y, x, y);
                        x -= rand;
                        continue;
                    }
                    break;
                }


            }
            void UpdateAcid(int x, int y)
            {

                int rand = randomGen.Next(0, 100);

                if(inBounds(x, y + 1) && solid(this[x, y + 1]) && rand < Constantas.acid_acidity_factor)
                {
                    this[x, y + 1] = ParticleId.Empty;
                }

                if (inBounds(x + 1, y) && solid(this[x + 1, y]) && rand > 100 - Constantas.acid_acidity_factor/2)
                {
                    this[x + 1, y] = ParticleId.Empty;
                }

                if (inBounds(x - 1, y) && solid(this[x - 1, y]) && rand > Math.Abs(50 - Constantas.acid_acidity_factor/2) + 50)
                {
                    this[x - 1, y] = ParticleId.Empty;
                }

                rand  = randomGen.Next(0, 2);
                rand = (rand == 0) ? -1 : 1;


                bool flag = false;

                int t = 0;

                int y_val = (int)Math.Round(dtVal(Constantas.acid_fall_speed));



                while (y_val - t > 0 && inBounds(x, y + 1) && gas(this[x, y + 1]))
                {
                    Swap(x, y + 1, x, y);
                    y++;
                    flag = true;
                    t++;
                }

                while (y_val - t > 0 && inBounds(x + rand, y + 1) && gas(this[x + rand, y + 1]))
                {
                    Swap(x + rand, y + 1, x, y);
                    y++;
                    x += rand;
                    flag = true;
                    t++;
                }

                while (y_val - t > 0 && inBounds(x - rand, y + 1) && gas(this[x - rand, y + 1]))
                {

                    Swap(x - rand, y + 1, x, y);
                    y++;
                    x -= rand;
                    flag = true;
                    t++;
                }



                if (flag)
                    return;

                int x_val = (int)Math.Round(dtVal(Constantas.acid_spread_factor));

                for (int i = 0; i < x_val; i++)
                {
                    if (inBounds(x + rand, y) && (this[x + rand, y] == ParticleId.Empty || this[x + rand, y] == ParticleId.Fire))
                    {
                        Swap(x + rand, y, x, y);
                        flag = true;
                        x += rand;
                        continue;
                    }
                    break;
                }
                if (flag)
                    return;


                for (int i = 0; i < x_val; i++)
                {
                    if (inBounds(x - rand, y) && (this[x - rand, y] == ParticleId.Empty || this[x - rand, y] == ParticleId.Fire))
                    {
                        Swap(x - rand, y, x, y);
                        x -= rand;
                        continue;
                    }
                    break;
                }


            }

        }


        private static class Constantas
        {

            public const float gravity = 3f;


            public const uint water_spread_factor = 7;
            public const uint water_fall_speed = 5;

            public const uint sand_fall_speed = 5;

            public const uint smoke_floating_speed = 2;

            public const uint fire_spread_factor = 10; //(number 1 - 100)
            public const uint fire_smoke_generation_factor = 15; //(number 1 - 100) 


            public const uint oil_spread_factor = 7;
            public const uint oil_fall_speed = 5;

            public const uint acid_spread_factor = 3;
            public const uint acid_fall_speed = 8;
            public const uint acid_acidity_factor = 4; //(number 1 - 100)

            public static readonly float[] empty_color = { 0.1f, 0.1f, 0.1f, 1.0f };
            public static readonly float[] sand_color = { 0.76f, 0.698f, 0.502f, 1.0f };
            public static readonly float[] wood_color = { 0.285f, 0.199f, 0.136f, 1.0f };
            public static readonly float[] water_color = { 0.3f, 0.3f, 0.6f, 0.7f };
            public static readonly float[] fire_color = { 0.886f, 0.345f, 0.133f, 1.0f };
            public static readonly float[] oil_color = { 0.459f, 0.498f, 0.40f, 0.7f };
            public static readonly float[] metal_color = { 0.67f, 0.67f, 0.67f, 1.0f };
            public static readonly float[] smoke_color = { 0.23f, 0.23f, 0.22f, 0.7f };
            public static readonly float[] steam_color = { 0.91f, 0.91f, 0.91f, 0.5f };
            public static readonly float[] sparkle_color = { 0.8f, 0.8f, 0.2f, 1.0f };
            public static readonly float[] acid_color = { 0.651f, 0.9f, 0.145f, 0.8f };

        }

        private class UI
        {
            private GameWorld worldreference;
            public int draw_radius { set; get; }
            public bool VisibleInterface { set; get; }
            public int pause { set; get; }

            public FileDropCallback drop_callback;

            private float euclidian_dist_sqr(float[] first, float[] second)
            {
                return (first[0] - second[0]) * (first[0] - second[0]) +
                    (first[1] - second[1]) * (first[1] - second[1])
                    + (first[2] - second[2]) * (first[2] - second[2]);
            }
            private GameWorld.ParticleId ClosestParticle(Color color)
            {
                float[] rgba = { (float)color.R / 255f, (float)color.G / 255f, (float)color.B / 255f, (float)color.A / 255f };


                GameWorld.ParticleId closest = GameWorld.ParticleId.Empty;

                float currentdist = euclidian_dist_sqr(rgba, Constantas.empty_color);

                if (euclidian_dist_sqr(rgba, Constantas.sand_color) < currentdist)
                {
                    currentdist = euclidian_dist_sqr(rgba, Constantas.sand_color);
                    closest = GameWorld.ParticleId.Sand;
                }

                if (euclidian_dist_sqr(rgba, Constantas.water_color) < currentdist)
                {
                    currentdist = euclidian_dist_sqr(rgba, Constantas.water_color);
                    closest = GameWorld.ParticleId.Water;
                }

                if (euclidian_dist_sqr(rgba, Constantas.fire_color) < currentdist)
                {
                    currentdist = euclidian_dist_sqr(rgba, Constantas.fire_color);
                    closest = GameWorld.ParticleId.Fire;
                }

                if (euclidian_dist_sqr(rgba, Constantas.metal_color) < currentdist)
                {
                    currentdist = euclidian_dist_sqr(rgba, Constantas.metal_color);
                    closest = GameWorld.ParticleId.Metal;
                }

                if (euclidian_dist_sqr(rgba, Constantas.oil_color) < currentdist)
                {
                    currentdist = euclidian_dist_sqr(rgba, Constantas.oil_color);
                    closest = GameWorld.ParticleId.Oil;
                }

                if (euclidian_dist_sqr(rgba, Constantas.wood_color) < currentdist)
                {
                    currentdist = euclidian_dist_sqr(rgba, Constantas.wood_color);
                    closest = GameWorld.ParticleId.Wood;
                }

                if (euclidian_dist_sqr(rgba, Constantas.smoke_color) < currentdist)
                {
                    currentdist = euclidian_dist_sqr(rgba, Constantas.smoke_color);
                    closest = GameWorld.ParticleId.Smoke;
                }

                if (euclidian_dist_sqr(rgba, Constantas.steam_color) < currentdist)
                {
                    currentdist = euclidian_dist_sqr(rgba, Constantas.steam_color);
                    closest = GameWorld.ParticleId.Steam;
                }

                if (euclidian_dist_sqr(rgba, Constantas.sparkle_color) < currentdist)
                {
                    currentdist = euclidian_dist_sqr(rgba, Constantas.sparkle_color);
                    closest = GameWorld.ParticleId.Sparkle;
                }

                if (euclidian_dist_sqr(rgba, Constantas.acid_color) < currentdist)
                {
                    currentdist = euclidian_dist_sqr(rgba, Constantas.acid_color);
                    closest = GameWorld.ParticleId.Acid;
                }

                return closest;
            }
            private void DropCallback(int count, IntPtr charptr)
            {
                IntPtr[] pIntPtrArray = new IntPtr[count];

                Marshal.Copy(charptr, pIntPtrArray, 0, count);

                string path = Marshal.PtrToStringAnsi(pIntPtrArray[0]);

                Console.WriteLine(path);
                string format = path.Substring(path.Length - 4);
                if (!(format == ".png" || format == ".jpg"))
                {
                    Console.WriteLine("Wrong Format");
                    return;
                }
                Bitmap bitmap = new Bitmap(path);

                for (int x = 0; x < bitmap.Width && x < worldreference.getSize.X; x++)
                {
                    for (int y = 0; y < bitmap.Height && y < worldreference.getSize.Y; y++)
                    {
                        Color pixel_color = bitmap.GetPixel(x, y);
                        worldreference[x, y] = ClosestParticle(pixel_color);
                    }
                }
            }



            private GameWorld.ParticleId current = GameWorld.ParticleId.Sand;
            private float[] currentcolor = Constantas.sand_color;
            private void drawFilledCircle(int xc, int yc, int r = 5)
            {
                for (int y = -r; y <= r; y++)
                    for (int x = -r; x <= r; x++)
                        if (x * x + y * y <= r * r)
                        {
                            worldreference[xc + x, yc + y] = current;
                        }
            }
            private void ProcessInput()
            {
                if (Glfw.GetKey(DisplayManager.Window, Keys.Alpha1) == InputState.Press)
                {
                    current = GameWorld.ParticleId.Sand;
                    currentcolor = Constantas.sand_color;
                }
                if (Glfw.GetKey(DisplayManager.Window, Keys.Alpha2) == InputState.Press)
                {
                    current = GameWorld.ParticleId.Water;
                    currentcolor = Constantas.water_color;
                }
                if (Glfw.GetKey(DisplayManager.Window, Keys.Alpha3) == InputState.Press)
                {
                    current = GameWorld.ParticleId.Metal;
                    currentcolor = Constantas.metal_color;
                }
                if (Glfw.GetKey(DisplayManager.Window, Keys.Alpha4) == InputState.Press)
                {
                    current = GameWorld.ParticleId.Sparkle;
                    currentcolor = Constantas.sparkle_color;
                }
                if (Glfw.GetKey(DisplayManager.Window, Keys.Alpha5) == InputState.Press)
                {
                    current = GameWorld.ParticleId.Smoke;
                    currentcolor = Constantas.smoke_color;
                }

                if (Glfw.GetKey(DisplayManager.Window, Keys.Alpha6) == InputState.Press)
                {
                    current = GameWorld.ParticleId.Wood;
                    currentcolor = Constantas.wood_color;
                }

                if (Glfw.GetKey(DisplayManager.Window, Keys.Alpha7) == InputState.Press)
                {
                    current = GameWorld.ParticleId.Oil;
                    currentcolor = Constantas.oil_color;
                }
                if (Glfw.GetKey(DisplayManager.Window, Keys.Alpha8) == InputState.Press)
                {
                    current = GameWorld.ParticleId.Fire;
                    currentcolor = Constantas.fire_color;
                }
                if (Glfw.GetKey(DisplayManager.Window, Keys.Alpha9) == InputState.Press)
                {
                    current = GameWorld.ParticleId.Acid;
                    currentcolor = Constantas.acid_color;
                }

                if (Glfw.GetKey(DisplayManager.Window, Keys.R) == InputState.Press)
                {
                    for (int i = 0; i < worldreference.getSize.X; i++)
                        for (int j = 0; j < worldreference.getSize.Y; j++)
                            worldreference[i, j] = GameWorld.ParticleId.Empty;
                }

                if (Glfw.GetKey(DisplayManager.Window, Keys.Equal) == InputState.Press)
                {
                    draw_radius++;
                }

                if (Glfw.GetKey(DisplayManager.Window, Keys.Minus) == InputState.Press)
                {
                    if (draw_radius == 0)
                        return;
                    draw_radius--;
                }

                if (Glfw.GetKey(DisplayManager.Window, Keys.H) == InputState.Press)
                {
                    
                        VisibleInterface = false;
                }
                if (Glfw.GetKey(DisplayManager.Window, Keys.J) == InputState.Press)
                {

                    VisibleInterface = true;
                }

                if (Glfw.GetKey(DisplayManager.Window, Keys.C) == InputState.Press)
                {
                    pause = -1;
                }
                if (Glfw.GetKey(DisplayManager.Window, Keys.V) == InputState.Press)
                {
                    pause = 1;
                }

                if ((Glfw.GetMouseButton(DisplayManager.Window, MouseButton.Button1)) == InputState.Press)
                {
                    Glfw.GetCursorPosition(DisplayManager.Window, out double x, out double y);
                    drawFilledCircle((int)(x / ((double)DisplayManager.WindowSize.X / (double)worldreference.getSize.X)),
                               (int)(y / ((double)DisplayManager.WindowSize.Y / (double)worldreference.getSize.Y)), draw_radius);
                }
            }


            public UI(GameWorld wrldreference)
            {
                worldreference = wrldreference;
                draw_radius = 3;
                VisibleInterface = true;
                pause = 1;
                drop_callback = (_, count, charptr) => DropCallback(count, charptr);
            }
           
            private bool inSquare(float x, float y, float x1, float y1, float x2, float y2)
            {
                return (x >= x1 && x <= x2 && y <= y1 && y >= y2);
            }

            private void drawCircle(float[] color, int xc, int yc, int x, int y)
            {
                float gridHBegin = (float)worldreference.getSize.Y * 1.0f / (float)worldreference.getSize.Y - 2.0f / (float)worldreference.getSize.Y;
                float gridWBegin = -(float)worldreference.getSize.X * 1.0f / (float)worldreference.getSize.X;

                Vector2 position = new Vector2(gridWBegin + (float)(xc + x) * 2.0f / (float)worldreference.getSize.X, gridHBegin - (float)(yc + y) * 2.0f / (float)worldreference.getSize.Y);
                Vector2 size = new Vector2(2.0f / (float)worldreference.getSize.X, 2.0f / (float)worldreference.getSize.Y);
                Renderer.DrawQuad(position, size, new Vector4(color[0], color[1], color[2], color[3]));

                position = new Vector2(gridWBegin + (float)(xc + x) * 2.0f / (float)worldreference.getSize.X, gridHBegin - (float)(yc - y) * 2.0f / (float)worldreference.getSize.Y);
                Renderer.DrawQuad(position, size, new Vector4(color[0], color[1], color[2], color[3]));

                position = new Vector2(gridWBegin + (float)(xc - x) * 2.0f / (float)worldreference.getSize.X, gridHBegin - (float)(yc - y) * 2.0f / (float)worldreference.getSize.Y);
                Renderer.DrawQuad(position, size, new Vector4(color[0], color[1], color[2], color[3]));

                position = new Vector2(gridWBegin + (float)(xc - x) * 2.0f / (float)worldreference.getSize.X, gridHBegin - (float)(yc + y) * 2.0f / (float)worldreference.getSize.Y);
                Renderer.DrawQuad(position, size, new Vector4(color[0], color[1], color[2], color[3]));

                position = new Vector2(gridWBegin + (float)(xc + y) * 2.0f / (float)worldreference.getSize.X, gridHBegin - (float)(yc + x) * 2.0f / (float)worldreference.getSize.Y);
                Renderer.DrawQuad(position, size, new Vector4(color[0], color[1], color[2], color[3]));

                position = new Vector2(gridWBegin + (float)(xc + y) * 2.0f / (float)worldreference.getSize.X, gridHBegin - (float)(yc - x) * 2.0f / (float)worldreference.getSize.Y);
                Renderer.DrawQuad(position, size, new Vector4(color[0], color[1], color[2], color[3]));

                position = new Vector2(gridWBegin + (float)(xc - y) * 2.0f / (float)worldreference.getSize.X, gridHBegin - (float)(yc - x) * 2.0f / (float)worldreference.getSize.Y);
                Renderer.DrawQuad(position, size, new Vector4(color[0], color[1], color[2], color[3]));

                position = new Vector2(gridWBegin + (float)(xc - y) * 2.0f / (float)worldreference.getSize.X, gridHBegin - (float)(yc + x) * 2.0f / (float)worldreference.getSize.Y);
                Renderer.DrawQuad(position, size, new Vector4(color[0], color[1], color[2], color[3]));
            }

            
            private void circleBres(float[] color, int xc, int yc, int r)
            {
                int x = 0, y = r;
                int d = 3 - 2 * r;
                drawCircle(color, xc, yc, x, y);
                while (y >= x)
                {            
                    x++;
                    if (d > 0)
                    {
                        y--;
                        d = d + 4 * (x - y) + 10;
                    }
                    else
                        d = d + 4 * x + 6;
                    drawCircle(color, xc, yc, x, y);
                }
               
            }

            public void Render()
            {
                if (VisibleInterface)
                {
                    Glfw.GetCursorPosition(DisplayManager.Window, out double x, out double y);

                    Renderer.BeginBatch();

                    float[] white = { 1.0f, 1.0f, 1.0f, 1.0f };
                    circleBres(white, (int)(x / ((double)DisplayManager.WindowSize.X / worldreference.getSize.X)),
                        (int)(y / ((double)DisplayManager.WindowSize.Y / worldreference.getSize.Y)), draw_radius);
                    Renderer.EndBatch();

                    Renderer.Flush();

                    Renderer.BeginBatch();


                    Renderer.DrawQuad(new Vector2(0.82f, 0.86f), new Vector2(0.06f, 0.06f),
                        new Vector4(currentcolor[0], currentcolor[1], currentcolor[2], currentcolor[3]), 0.1f);

                   
                    Renderer.EndBatch();
                    Renderer.Flush();


                }
            }

            public void Update()
            {
                ProcessInput();
            }


        }

}
}
