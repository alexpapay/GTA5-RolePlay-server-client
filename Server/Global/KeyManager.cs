﻿using GTANetworkServer;
using GTANetworkShared;
using TheGodfatherGM.Server.Characters;
using TheGodfatherGM.Server.Property;

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
            }
            if (eventName == "BuyScooter")
            {
                if ((int)args[0] == 77)
                {
                    API.createVehicle(VehicleHash.Faggio2, new Vector3(-989.4827, -2706.635, 13.3), new Vector3(0.0, 0.0, 62.19496), 0, 0);
                }
            }
        }
    }
}
