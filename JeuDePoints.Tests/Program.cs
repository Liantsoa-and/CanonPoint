using JeuDePoints.Forms;
using JeuDePoints.Tests;

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