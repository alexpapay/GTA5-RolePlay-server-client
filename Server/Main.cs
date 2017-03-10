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
            CylinderColShape m_colShape = API.createCylinderColShape(new Vector3(-989.4827, -2706.635, 12.7), 0.5f, 1.0f);
            m_colShape.onEntityEnterColShape += M_colShape_onEntityEnterColShape;
            API.onEntityEnterColShape += OnEntityEnterColShapeHandler;

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

            Vector3 scooterMarker = new Vector3(-989.4827, -2706.635, 12.7);

            API.createVehicle(VehicleHash.Faggio2, buggiOnePos, buggiOneRot, 0, 0);
            API.createVehicle(VehicleHash.Faggio2, buggiTwoPos, buggiOneRot, 0, 0);
            API.createVehicle(VehicleHash.Faggio2, buggiThreePos, buggiOneRot, 0, 0);
            API.createVehicle(VehicleHash.Faggio2, buggiFourPos, buggiOneRot, 0, 0);

            API.createMarker(1, scooterMarker, new Vector3(), new Vector3(), new Vector3 (2, 2, 2), 255, 255, 255, 0, 0);
        }

        private void M_colShape_onEntityEnterColShape(ColShape shape, NetHandle entity)
        {
            var player = API.getPlayerFromHandle(entity);
            if (player == null)
            {
                return;
            }
            player.sendChatMessage("You stepped in the colshape!");
            API.triggerClientEvent(player, "BuyScooter");
        }

        private void OnEntityEnterColShapeHandler(ColShape shape, NetHandle entity)
        {
            var player = API.getPlayerFromHandle(entity);
            if (player == null)
            {
                return;
            }
            player.sendChatMessage("Нажмите Е для покупки скутера");
            API.createTextLabel("TEST!", new Vector3(-989.4827, -2706.635, 15.0), 1.0f, 2.0f);
            //API.triggerClientEvent(player, "BuyScooter");
            //
            //Code
        }
    }
}