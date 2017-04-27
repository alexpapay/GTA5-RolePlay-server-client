using System;
using System.Linq;
using GTANetworkServer;
using TheGodfatherGM.Data;
using TheGodfatherGM.Data.Enums;
using TheGodfatherGM.Server.Characters;
using TheGodfatherGM.Server.DBManager;
using TheGodfatherGM.Server.Property;
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
            CharacterController characterController = player.getData("CHARACTER");
            if (characterController == null) return;
            Character character = characterController.Character;
            string FormatName = character.Name.Replace("_", " ");

            if (eventName == "onKeyDown")
            {
                if ((int)args[0] == 2)
                {
                    PropertyController propertyController = player.getData("AT_PROPERTY");
                    if ((propertyController = player.getData("AT_PROPERTY")) != null)
                    {
                        propertyController.PropertyDoor(player);
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
                    if (player.isInVehicle) VehicleController.TriggerDoor(player.vehicle, 4);
                }
                else if ((int)args[0] == 6)
                {
                    if (player.isInVehicle) VehicleController.TriggerDoor(player.vehicle, 5);                    
                }
                else if ((int)args[0] == 8)
                {
                    CharacterController.StopAnimation(player);
                }
                // Get info about Player:
                else if ((int)args[0] == 9)
                {                    
                    if (character == null) return;
                    var job = "";
                    if (character.JobId == 0) job = "Бомж";
                    if (character.JobId == 777) job = "Таксист";
                    if (character.JobId == 888) job = "Безработный";
                    if (character.JobId == 1 || character.JobId == 2) job = "Грузчик";

                    var getGroup = ContextFactory.Instance.Group.FirstOrDefault(x => x.Id == character.ActiveGroupID);
                    var groupType = (GroupType)Enum.Parse(typeof(GroupType), getGroup.Type.ToString());
                    var groupExtraType = (GroupExtraType)Enum.Parse(typeof(GroupExtraType), getGroup.ExtraType.ToString());
                    
                    var driverLicense = character.DriverLicense == 1 ? "Да" : "Нет";                   

                    API.shared.triggerClientEvent(player, "character_menu",
                        2,                                                  // 0
                        "Ваша статистика",                                  // 1
                        "Ваше имя: " + FormatName,                          // 2
                        character.Age,                                      // 3
                        character.Level.ToString(),                         // 4
                        job,                                                // 5
                        character.Bank.ToString(),                          // 6
                        driverLicense,                                      // 7
                        character.ActiveGroupID,                            // 8
                        EntityManager.GetDisplayName(groupType),            // 9
                        EntityManager.GetDisplayName(groupExtraType),       // 10
                        character.Material);                                // 11
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
                    
                    int engineStatus = 0;
                    if (API.getVehicleEngineStatus(vehicleController.Vehicle)) engineStatus = 1;

                    int driverDoorStatus = 1;
                    if (API.getVehicleLocked(vehicleController.Vehicle)) driverDoorStatus = 0;
                    var fuel = vehicleController.VehicleData.Fuel;

                    API.shared.triggerClientEvent(player, "vehicle_menu", 
                        2,                                          // 0
                        "Меню транспорта",                          // 1
                        "Владелец: " + FormatName,                  // 2
                        engineStatus,                               // 3
                        fuel.ToString(),                            // 4
                        inVehicleCheck,                             // 5
                        driverDoorStatus,                           // 6
                        vehicleController.VehicleData.Material);    // 7                    
                }
                // Get info about work possibilities:
                else if ((int)args[0] == 12)
                {
                    if (character == null) return;
                    var getGroup = ContextFactory.Instance.Group.FirstOrDefault(x => x.Id == character.ActiveGroupID);
                    var groupType = (GroupType)Enum.Parse(typeof(GroupType), getGroup.Type.ToString());
                    var groupExtraType = (GroupExtraType)Enum.Parse(typeof(GroupExtraType), getGroup.ExtraType.ToString());

                    API.shared.triggerClientEvent(player, "workposs_menu",
                         1,                                                                                  // 0
                         character.ActiveGroupID,                                                            // 1
                         character.JobId,                                                                    // 2
                         character.TempVar,                                                                  // 3
                         character.Admin,                                                                    // 4
                         EntityManager.GetDisplayName(groupType),                                            // 5
                         EntityManager.GetDisplayName(groupExtraType),                                       // 6
                         character.Material,                                                                 // 7
                         CharacterController.IsCharacterInGang(characterController),                         // 8
                         CharacterController.IsCharacterGangBoss(characterController),                       // 9
                         CharacterController.IsCharacterArmyHighOfficer(characterController.Character),      // 10
                         CharacterController.IsCharacterInGhetto(player),                                    // 11
                         CharacterController.IsCharacterArmyGeneral(characterController));                   // 12
                }
            }
        }
    }
}
