using Finance_Pro.ServerConnect;
using Finance_Pro.TheImitationGame;
using System.Configuration;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows;

namespace Finance_Pro
{

    public partial class App : Application
    {
        public static string clientKey = "DontDoThat";
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MySession.gI().setHandler(Controller.gI());
            MySession.gI().connect();
        }
    }
}
