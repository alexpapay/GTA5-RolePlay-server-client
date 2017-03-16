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
            Vector3 buggiOneRot = new Vector3(0.0, 0.0, 62.19496);
            Vector3 scooterMarker = new Vector3(-989.4827, -2706.635, 12.7);
            //API.createVehicle(VehicleHash.Faggio2, buggiOnePos, buggiOneRot, 0, 0);
        } 
    }
}