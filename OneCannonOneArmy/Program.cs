using System;
using System.Windows.Forms;

namespace OneCannonOneArmy
{
#if WINDOWS
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        static bool restart = true;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            while (restart)
            {
                restart = false;
                using (var game = new Game1())
                {
                    game.Run();
                    restart = game.Restart;
                }
            }
        }
    }
#endif
}