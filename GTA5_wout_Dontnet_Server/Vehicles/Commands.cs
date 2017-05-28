using GTANetworkServer;
using GTANetworkShared;
using System.Collections.Generic;
using TheGodfatherGM.Server.Characters;

namespace TheGodfatherGM.Server.Vehicles
{
    class Commands : Script
    {
        public Commands() { }

        [Command("vstorage")]
        public static void VehicleStorage(Client player)
        {
            CharacterController characterController = player.getData("CHARACTER");
            if (characterController == null) return;

            if (characterController.Character.Vehicle == null)
            {
                API.shared.sendChatMessageToPlayer(player, "You have no vehicles.");
                return;
            }
            else if (characterController.Character.Vehicle.Count == 0)
            {
                API.shared.sendChatMessageToPlayer(player, "You have no vehicles.");
            }
            else
            {
                List<string> VehicleNames = new List<string>();
                List<int> VehicleIDs = new List<int>();
                foreach (var VehicleData in characterController.Character.Vehicle)
                {
                    VehicleController _VehicleController = EntityManager.GetVehicle(VehicleData);
                    string isSpawned = (_VehicleController == null ? " (stored)" : " (spawned)");
                    VehicleNames.Add(API.shared.getVehicleDisplayName((VehicleHash)VehicleData.Model) + isSpawned);
                    VehicleIDs.Add(VehicleData.Id);
                }

                player.setData("VSTORAGE", VehicleIDs);
                API.shared.triggerClientEvent(player, "create_menu", 1, null, "Vehicles", false, VehicleNames.ToArray());
            }
        }
    }
}