using OOP_projects.GameEngine;
using OOP_projects.Games;

class Program
{
    static void Main()
    {
        Game game = new SandFallingSim(1000, 1000, 300, 300);
        game.Run();     
    }
}