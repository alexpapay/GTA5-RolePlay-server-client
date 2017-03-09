using GTANetworkServer;

using GTANetworkShared;

using System;
using System.Globalization;
using System.Threading.Tasks;

namespace TheGodfatherGM.Server
{
    class Main : Script
    {        
        public Main()
        {            
            API.onResourceStart += OnResourceStart;
            API.onResourceStop += OnResourceStop;
            OnStartInit();
        }

        private void OnResourceStart()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("en-GB");
            Console.BackgroundColor = ConsoleColor.Blue;
            API.consoleOutput(Global.GlobalVars.ServerName + " was started at " + DateTime.Now);
            Console.ResetColor();
        }

        private void OnResourceStop()
        {
            API.consoleOutput("Resetting active sessions...");
            Task DBTerminate = Task.Run(() =>
            {
                DatabaseManager.ResetSessions();
            });
            DBTerminate.Wait();
            API.consoleOutput(Global.GlobalVars.ServerName + " was stopped at " + DateTime.Now);        
        }

        // Инициализация объектов без записи в БД
        private void OnStartInit()
        {       
            Vector3 buggiOnePos = new Vector3   (-1019.0, -2716.0, 13.3);
            Vector3 buggiTwoPos = new Vector3   (-1018.0, -2714.0, 13.3);
            Vector3 buggiThreePos = new Vector3 (-1017.0, -2712.0, 13.3);
            Vector3 buggiFourPos = new Vector3  (-1016.0, -2710.0, 13.3);
            Vector3 buggiOneRot = new Vector3(0.0, 0.0, 62.19496);

            API.createVehicle(VehicleHash.Faggio2, buggiOnePos, buggiOneRot, 0, 0);
            API.createVehicle(VehicleHash.Faggio2, buggiTwoPos, buggiOneRot, 0, 0);
            API.createVehicle(VehicleHash.Faggio2, buggiThreePos, buggiOneRot, 0, 0);
            API.createVehicle(VehicleHash.Faggio2, buggiFourPos, buggiOneRot, 0, 0);            
        }
    }
}