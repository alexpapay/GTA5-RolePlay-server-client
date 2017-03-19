using GTANetworkServer;
using GTANetworkShared;
using System.Collections.Generic;
using TheGodfatherGM.Server.Characters;
using TheGodfatherGM.Server.Property;
using TheGodfatherGM.Server.User;
using TheGodfatherGM.Server.Vehicles;

namespace TheGodfatherGM.Server.Global
{
    class KeyManager : Script
    {
        public KeyManager()
        {
            API.onClientEventTrigger += OnClientEventTrigger;
        }

        private void OnClientEventTrigger(Client player, string eventName, object[] args)
        {
            if(eventName == "onKeyDown")
            {
                if ((int)args[0] == 2)
                {
                    PropertyController PropertController = player.getData("AT_PROPERTY");
                    if ((PropertController = player.getData("AT_PROPERTY")) != null)
                    {
                        PropertController.PropertyDoor(player);
                    }
                }
                else if((int)args[0] == 3)
                {
                    if(player.isInVehicle)
                    {
                        player.vehicle.specialLight = true;
                    }
                }
                else if ((int)args[0] == 4)
                {
                    if (player.isInVehicle)
                    {
                        player.vehicle.specialLight = true;
                    }
                }
                else if ((int)args[0] == 5)
                {
                    if (player.isInVehicle)
                    {
                        Vehicles.VehicleController.TriggerDoor(player.vehicle, 4);
                    }
                }
                else if ((int)args[0] == 6)
                {
                    if (player.isInVehicle)
                    {
                        Vehicles.VehicleController.TriggerDoor(player.vehicle, 5);
                    }
                }

                else if ((int)args[0] == 8)
                {
                    CharacterController.StopAnimation(player);
                }

                // Try to test on_key_down trigger with argument 9:
                else if ((int)args[0] == 9)
                {
                    //API.createVehicle(VehicleHash.Faggio2, new Vector3(-989.4827, -2706.635, 13.3), new Vector3(0.0, 0.0, 62.19496), 0, 0);
                }

                // GET info about car
                else if ((int)args[0] == 10)
                {
                    VehicleController vehicleController = EntityManager.GetVehicle(player.vehicle);

                    if (vehicleController == null || player.vehicleSeat != -1)
                    {
                        API.sendChatMessageToPlayer(player, "~r~Ошибка: ~w~Вы не в транспорте или не на сидении водителя");
                        return;
                    }
                    
                    AccountController account = player.getData("ACCOUNT");
                    if (account == null) return;                    
                    string FormatName = account.CharacterController.Character.Name.Replace("_", " ");

                    int engineStatus = 0;
                    if (API.getVehicleEngineStatus(vehicleController.Vehicle)) engineStatus = 1;
                    
                    API.shared.triggerClientEvent(player, "vehicle_menu", 
                        2, // 0
                        "Ваш транспорт", 
                        "Владелец: " + account.CharacterController.FormatName,
                        engineStatus,
                        vehicleController.VehicleData.Fuel.ToString());                    
                }
            }
        }
    }
}
