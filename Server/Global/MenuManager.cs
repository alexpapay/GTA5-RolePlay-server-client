using GTANetworkServer;
using GTANetworkShared;
using TheGodfatherGM.Server.Characters;
using TheGodfatherGM.Server.Jobs;
using System.Collections.Generic;
using TheGodfatherGM.Server.Property;
using TheGodfatherGM.Server.DBManager;
using TheGodfatherGM.Server.User;
using TheGodfatherGM.Server.Admin;
using TheGodfatherGM.Data;
using TheGodfatherGM.Data.Enums;
using System.Linq;
using TheGodfatherGM.Server.Vehicles;

namespace TheGodfatherGM.Server.Menu
{
    public class MenuManager : Script
    {   
        public MenuManager()
        {
            API.onClientEventTrigger += onClientEventTrigger;
        }

        private void onClientEventTrigger(Client player, string eventName, object[] args)
        {
            VehicleController vehicleController = EntityManager.GetVehicle(player.vehicle);
            AccountController account = player.getData("ACCOUNT");

            if (eventName == "menu_handler_select_item")
            {
                int callback = (int)args[0];

                if (callback == 0) // Character Menu
                {
                    if ((int)args[1] == (int)args[2] - 1) CharacterController.CreateCharacter(player);
                    else CharacterController.SelectCharacter(player, (int)args[1] + 1);
                }
                else if (callback == 1) // Vehicle Menu
                {
                    List<int> VehicleIDs = player.getData("VSTORAGE");

                    int vehID = VehicleIDs[(int)args[1]];
                    VehicleController _VehicleController = EntityManager.GetVehicle(vehID);
                    if (_VehicleController == null) VehicleController.LoadVehicle(account, vehID);
                    else _VehicleController.UnloadVehicle(account);
                    player.resetData("VSTORAGE");
                }
                else if (callback == 2)
                {
                    
                }
            }

            if (eventName == "driver_door")
            {
                VehicleController VehicleController = null;
                if (player.isInVehicle) VehicleController = EntityManager.GetVehicle(player.vehicle);
                else VehicleController = EntityManager.GetVehicleControllers().Find(x => x.Vehicle.position.DistanceTo(player.position) < 2.0f);

                if (VehicleController == null)
                {
                    API.sendNotificationToPlayer(player, "Вы находитесь далеко от транспорта.");
                    return;
                }

                if (VehicleController.CheckAccess(account))
                {
                    if ((int)args[0] == 1)
                    {
                        API.setVehicleLocked(VehicleController.Vehicle, false);
                        ChatController.sendProxMessage(player, 15.0f, "~#C2A2DA~", account.CharacterController.FormatName + " открыл водительскую дверь.");
                    }
                    else
                    {
                        API.setVehicleLocked(VehicleController.Vehicle, true);
                        ChatController.sendProxMessage(player, 15.0f, "~#C2A2DA~", account.CharacterController.FormatName + " закрыл водительскую дверь.");
                    }
                }
                else API.sendNotificationToPlayer(player, "~r~ERROR: ~w~Вы не можете открыть данный транспорт!");
            }

            if (eventName == "engine_on")
            {
                VehicleController VehicleController = null;
                if (player.isInVehicle) VehicleController = EntityManager.GetVehicle(player.vehicle);
                else VehicleController = EntityManager.GetVehicleControllers().Find(x => x.Vehicle.position.DistanceTo(player.position) < 2.0f);

                if (account.CharacterController.Character.DriverLicense == 0 && vehicleController.VehicleData.Type != 1)
                {
                    API.sendNotificationToPlayer(player, "У вас нет прав на управление данным транспортом.");
                    return;
                }
                if (!vehicleController.CheckAccess(account))
                {
                    API.sendNotificationToPlayer(player, "Вы не можете использовать данный транспорт.");
                    return;
                }
                else
                {
                    if (vehicleController.VehicleData.Fuel != 0)
                    {
                        vehicleController.Vehicle.engineStatus = true;
                        ChatController.sendProxMessage(player, 15.0f, "~#C2A2DA~", account.CharacterController.FormatName + " вставил ключ в зажигание и запустил мотор.");
                    }                    
                }                    
            }
            if (eventName == "engine_off")
            {
                if (!vehicleController.CheckAccess(account))
                {
                    API.sendNotificationToPlayer(player, "Вы не можете использовать данный транспорт.");
                    return;
                }
                else
                {
                    vehicleController.Vehicle.engineStatus = false;
                    ChatController.sendProxMessage(player, 15.0f, "~#C2A2DA~", account.CharacterController.FormatName + " повернул ключ зажигания и заглушил мотор.");
                }
            }

            if (eventName == "park_vehicle")
            {
                if (vehicleController.CheckAccess(account, account.CharacterController))
                {
                    vehicleController.ParkVehicle(player);
                }
                else API.sendNotificationToPlayer(player, "~r~ERROR: ~w~Вы не можете парковать данный транспорт");
            }

            if (eventName == "hood_trunk")
            {
                VehicleController VehicleController = null;
                if (player.isInVehicle) VehicleController = EntityManager.GetVehicle(player.vehicle);
                else VehicleController = EntityManager.GetVehicleControllers().Find(x => x.Vehicle.position.DistanceTo(player.position) < 2.0f);

                if (VehicleController == null)
                {
                    API.sendNotificationToPlayer(player, "Вы находитесь далеко от транспорта.");
                    return;
                }

                if (VehicleController.CheckAccess(account))
                {
                    if ((int)args[0] == 1)
                    {
                        VehicleController.TriggerDoor(VehicleController.Vehicle, 4);
                        ChatController.sendProxMessage(player, 15.0f, "~#C2A2DA~", account.CharacterController.FormatName + " открыл/закрыл капот.");
                    }
                    else
                    {
                        VehicleController.TriggerDoor(VehicleController.Vehicle, 5);
                        ChatController.sendProxMessage(player, 15.0f, "~#C2A2DA~", account.CharacterController.FormatName + " открыл/закрыл багажник.");
                    }
                }
                else API.sendNotificationToPlayer(player, "~r~ERROR: ~w~Вы не можете открыть капот или багажник данного транспорта.");
            }

            if (eventName == "rent_scooter")
            {
                // Delete 30$ from cash
                if (account == null) return;
                if (account.CharacterController.Character.Cash - 30 < 0)
                {
                    API.shared.sendNotificationToPlayer(player, "У вас недостаточно средств для аренды!");
                    return;
                }
                else
                {
                    account.CharacterController.Character.Cash -= 30;
                    API.shared.triggerClientEvent(player, "update_money_display", account.CharacterController.Character.Cash);
                    
                    Data.Vehicle VehicleData = new Data.Vehicle();
                    VehicleData.Character = account.CharacterController.Character;
                    VehicleController VehicleController = new VehicleController(VehicleData, API.createVehicle(VehicleHash.Faggio, player.position, player.rotation, 0, 0, 0));

                    VehicleData.Model = -1842748181;
                    VehicleData.PosX = player.position.X;
                    VehicleData.PosY = player.position.Y;
                    VehicleData.PosZ = player.position.Z;
                    VehicleData.Rot = player.rotation.Z;
                    VehicleData.Color1 = 0;
                    VehicleData.Color2 = 0;
                    VehicleData.Fuel = 30;
                    VehicleData.Type = 1;
                    VehicleData.GroupId = account.CharacterController.Character.ActiveGroupID;

                    ContextFactory.Instance.Vehicle.Add(VehicleData);
                    ContextFactory.Instance.SaveChanges();
                }
                ContextFactory.Instance.SaveChanges();
            }

            if (eventName == "rent_prolong")
            {
                if (account == null) return;
                int callback = (int)args[0];
                var vehicle = ContextFactory.Instance.Vehicle
                        .Where(x => x.Type == 1)
                        .FirstOrDefault(x => x.CharacterId == account.CharacterController.Character.Id);

                if (callback == 1)
                {
                    vehicle.Fuel = 30;
                    ContextFactory.Instance.SaveChanges();
                }

                if (callback == 0)
                {
                    VehicleController _VehicleController = EntityManager.GetVehicle(vehicle);
                    _VehicleController.UnloadVehicle(account);
                    ContextFactory.Instance.Vehicle.Remove(vehicle);
                    ContextFactory.Instance.SaveChanges();
                }

            }                       

            if (eventName == "work_loader")
            {                
                if (account == null) return;
                int callback = (int)args[0];
                int jobId = (int)args[1];

                if (callback == 1)
                {                    
                    if (jobId == 1) JobController.JobWorkLoader(player,
                                                          -1020.5, -2722.14, 13.8,
                                                          -1035.88, -2735.7, 13.8,
                                                          3, jobId);    // For Testing at start 
                                                                        // Coords for job marker: -1030.91 ; -2722.16 ; 13.7624

                    if (jobId == 2) JobController.JobWorkLoader(player,
                                                        -155.5, -959.14, 269.2,
                                                        -179.88, -1008.7, 254.1316,
                                                        5, jobId);
                }       

                if (callback == 0)
                {
                    API.shared.triggerClientEvent(player, "update_money_display", account.CharacterController.Character.Cash);
                    API.setPlayerClothes(player, 9, 10, 100);                    
                }
            }
        }
    }
}
