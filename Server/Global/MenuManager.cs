using GTANetworkServer;
using GTANetworkShared;
using TheGodfatherGM.Server.Characters;
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

            if (eventName == "engine_on")
            {                
                if (!vehicleController.CheckAccess(account))
                {
                    API.sendNotificationToPlayer(player, "Вы не можете использовать данный транспорт.");
                    return;
                }
                else
                {
                    vehicleController.Vehicle.engineStatus = true;
                    ChatController.sendProxMessage(player, 15.0f, "~#C2A2DA~", account.CharacterController.FormatName + " вставил ключ в зажигание и запустил мотор.");
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
                else VehicleController = EntityManager.GetVehicleControllers().Find(x => x.Vehicle.position.DistanceTo(player.position) < 3.0f);

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
                        ChatController.sendProxMessage(player, 15.0f, "~#C2A2DA~", account.CharacterController.FormatName + " заблокировал двери своего транспорта.");
                    }
                    else
                    {
                        VehicleController.TriggerDoor(VehicleController.Vehicle, 5);
                        ChatController.sendProxMessage(player, 15.0f, "~#C2A2DA~", account.CharacterController.FormatName + " разблокировал двери своего транспорта.");
                    }
                }
                else API.sendNotificationToPlayer(player, "~r~ERROR: ~w~Вы не можете открыть данный транспорт.");
            }
            
            if (eventName == "rent_scooter")
            {
                // Delete 30$ from cash
                if (account == null) return;
                if (account.CharacterController.Character.Cash - 30 < 0)
                {
                    API.shared.sendNotificationToPlayer(player, "У вас недостаточно средств для аренды мопеда!");
                    return;
                }
                else
                {
                    account.CharacterController.Character.Cash = account.CharacterController.Character.Cash - 30;
                    API.shared.triggerClientEvent(player, "update_money_display", account.CharacterController.Character.Cash);
                    // Create Faggio
                    //API.createVehicle(VehicleHash.Faggio, new Vector3(-989.4827, -2706.635, 12.7), new Vector3(0.0, 0.0, 0.0), 0, 0);
                    //Admin.Commands.createvehicle(player, "group", "Homeless", VehicleHash.Faggio, 0, 0);

                    Data.Vehicle VehicleData = new Data.Vehicle();
                    VehicleData.Character = account.CharacterController.Character;
                    Vehicles.VehicleController VehicleController = new Vehicles.VehicleController(VehicleData, API.createVehicle(VehicleHash.Faggio, player.position, player.rotation, 0, 0, 0));

                    VehicleData.Model = -1842748181;
                    VehicleData.PosX = player.position.X;
                    VehicleData.PosY = player.position.Y;
                    VehicleData.PosZ = player.position.Z;
                    VehicleData.Rot = player.rotation.Z;
                    VehicleData.Color1 = 0;
                    VehicleData.Color2 = 0;
                    VehicleData.Fuel = 3;

                    ContextFactory.Instance.Vehicle.Add(VehicleData);
                    ContextFactory.Instance.SaveChanges();
                }
                ContextFactory.Instance.SaveChanges();
                
                // set 1 hour for rent
            }
        }
    }
}
