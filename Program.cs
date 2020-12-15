using System;

namespace gol
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            Gol game = new Gol();
            game.Run();
        }
    }
}
