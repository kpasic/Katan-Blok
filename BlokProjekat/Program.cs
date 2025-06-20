using Catan;
using static CNetworking.NetworkUtils;

namespace ClientApp
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            PlayerTypeRegistry.RegisterPlayerType<HumanPlayer>();
            //PlayerTypeRegistry.RegisterPlayerType<NetworkPlayer>();
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}