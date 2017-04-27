using GTANetworkServer;
using System.Linq;
using TheGodfatherGM.Data.Enums;
using TheGodfatherGM.Server.Characters;
using TheGodfatherGM.Server.DBManager;

namespace TheGodfatherGM.Server.Vehicles
{
    class Commands : Script
    {
        public Commands() { }

        [Command("car", "~y~USAGE: ~w~/car [engine/park/hood/trunk]")]
        public void car(Client player, string Choice)
        {
            CharacterController characterController = player.getData("CHARACTER");
            if (characterController == null) return;

            if (Choice == "engine")
            {
                VehicleController VehicleController = EntityManager.GetVehicle(player.vehicle);
                if (VehicleController == null || player.vehicleSeat != -1)
                {
                    API.sendChatMessageToPlayer(player, "~r~ERROR: ~w~¬ы не в транспорте или не на сидении водител€");
                    return;
                }

                if (!VehicleController.CheckAccess(characterController))
                {
                    API.sendNotificationToPlayer(player, "¬ы не можете использовать данный транспорт.");
                    return;
                }
                else
                {
                    var FormatName = characterController.Character.Name.Replace("_", " ");
                    if (API.getVehicleEngineStatus(VehicleController.Vehicle))
                    {
                        VehicleController.Vehicle.engineStatus = false;
                        ChatController.sendProxMessage(player, 15.0f, "~#C2A2DA~", FormatName + " turns the key in the ignition and the engine stops.");
                    }
                    else
                    {
                        VehicleController.Vehicle.engineStatus = true;
                        ChatController.sendProxMessage(player, 15.0f, "~#C2A2DA~", FormatName + " turns the key in the ignition and the engine starts.");
                    }
                }
            }
            else if(Choice == "park")
            {                
                VehicleController VehicleController = EntityManager.GetVehicle(player.vehicle);
                Data.Vehicle VM = VehicleController.VehicleData;
                if (VM == null || player.vehicleSeat != -1)
                {
                    API.sendNotificationToPlayer(player, "~r~ERROR: ~w~¬ы не в транспорте или не на сидении водител€.");
                    return;
                }

                if (VehicleController.CheckAccess(characterController))
                {
                    VehicleController.ParkVehicle(player);
                }
                else API.sendNotificationToPlayer(player, "~r~ERROR: ~w~¬ы не можете парковать данный транспорт");
            }

            else if (Choice == "hood" || Choice == "trunk")
            {
                VehicleController VehicleController = null;
                if (player.isInVehicle) VehicleController = EntityManager.GetVehicle(player.vehicle);
                else VehicleController = EntityManager.GetVehicleControllers().Find(x => x.Vehicle.position.DistanceTo(player.position) < 3.0f);

                if(VehicleController == null)
                {
                    API.sendNotificationToPlayer(player, "¬ы находитесь далеко от транспорта.");
                    return;
                }

                if (VehicleController.CheckAccess(characterController))
                {
                    if (Choice == "hood") VehicleController.TriggerDoor(VehicleController.Vehicle, 4);
                    else VehicleController.TriggerDoor(VehicleController.Vehicle, 5);
                }
                else API.sendNotificationToPlayer(player, "~r~ERROR: ~w~¬ы не можете парковать данный транспорт.");
            }
        }
    }
}