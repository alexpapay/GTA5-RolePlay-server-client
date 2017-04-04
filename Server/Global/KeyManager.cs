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
                    AccountController account = player.getData("ACCOUNT");
                    if (account == null) return;
                    var age = account.CharacterController.Character.LastLoginDate.ToString();
                    var level = account.CharacterController.Character.Level.ToString();
                    var job = account.CharacterController.Character.JobId.ToString();
                    var bank = account.CharacterController.Character.Bank.ToString();
                    var driverLicense = account.CharacterController.Character.DriverLicense == 1 ? "Да" : "Нет";

                    API.shared.triggerClientEvent(player, "character_menu",
                        2, // 0
                        "Ваша статистика",
                        "Ваше имя: " + account.CharacterController.FormatName,
                        age, level, job, bank, driverLicense);
                }

                // GET info about car
                else if ((int)args[0] == 10)
                {
                    VehicleController vehicleController = null;
                    bool inVehicleCheck;

                    if (player.isInVehicle)
                    {
                        vehicleController = EntityManager.GetVehicle(player.vehicle);
                        inVehicleCheck = true;
                    }
                    else
                    {
                        vehicleController = EntityManager.GetVehicleControllers().Find(x => x.Vehicle.position.DistanceTo(player.position) < 2.0f);
                        inVehicleCheck = false;
                    }                    

                    if (vehicleController == null)
                    {
                        API.sendNotificationToPlayer(player, "Вы находитесь далеко от транспорта.");
                        return;
                    }                    

                    AccountController account = player.getData("ACCOUNT");
                    if (account == null) return;                    
                    string FormatName = account.CharacterController.Character.Name.Replace("_", " ");

                    int engineStatus = 0;
                    if (API.getVehicleEngineStatus(vehicleController.Vehicle)) engineStatus = 1;

                    int driverDoorStatus = 1;
                    if (API.getVehicleLocked(vehicleController.Vehicle)) driverDoorStatus = 0;
                    var fuel = vehicleController.VehicleData.Fuel;

                    API.shared.triggerClientEvent(player, "vehicle_menu", 
                        2, // 0
                        "Меню транспорта", 
                        "Владелец: " + account.CharacterController.FormatName,
                        engineStatus,
                        fuel.ToString(), 
                        inVehicleCheck, 
                        driverDoorStatus);                    
                }
            }
        }
    }
}
