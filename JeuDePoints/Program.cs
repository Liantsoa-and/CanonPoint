using JeuDePoints.Forms;

namespace JeuDePoints
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new FormConfig());
        }
    }
}