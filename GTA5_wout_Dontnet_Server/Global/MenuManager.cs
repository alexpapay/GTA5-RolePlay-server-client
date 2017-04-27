using GTANetworkServer;
using GTANetworkShared;
using TheGodfatherGM.Server.Characters;
using TheGodfatherGM.Server.Jobs;
using System.Collections.Generic;
using TheGodfatherGM.Server.DBManager;
using TheGodfatherGM.Data;
using System.Linq;
using TheGodfatherGM.Server.Vehicles;
using System.Security.Cryptography;
using System.Text;
using System;
using TheGodfatherGM.Data.Enums;

namespace TheGodfatherGM.Server.Menu
{
    public class MenuManager : Script
    {
        public MenuManager()
        {
            API.onClientEventTrigger += onClientEventTrigger;
            API.onClientEventTrigger += onCreateEventTrigger;
        }

        // Login & Registration
        private void onCreateEventTrigger(Client player, string eventName, object[] args)
        {
            if (eventName == "enter_login")            
                API.shared.sendChatMessageToPlayer(player, string.Format("~g~Введите свой логин в формате Имя_Фамилия"));
            if (eventName == "enter_pwd")            
                API.shared.sendChatMessageToPlayer(player, string.Format("~g~Введите пароль"));

            if (eventName == "create_char")
            {
                if (CharacterController.NameValidityCheck(player, args[0].ToString()))
                {
                    MD5 md5 = new MD5CryptoServiceProvider();
                    string pass = args[1].ToString();
                    byte[] checkSum = md5.ComputeHash(Encoding.UTF8.GetBytes(pass));
                    string result = BitConverter.ToString(checkSum).Replace("-", String.Empty);
                    CharacterController newCharacterController = new CharacterController(player, args[0].ToString(), result);
                }
                else
                {
                    API.shared.triggerClientEvent(player, "create_char_menu", 0);
                    return;
                }
            }
            if (eventName == "login_char")
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                string pass = args[0].ToString();
                byte[] checkSum = md5.ComputeHash(Encoding.UTF8.GetBytes(pass));
                string result = BitConverter.ToString(checkSum).Replace("-", String.Empty);
                Character character = ContextFactory.Instance.Character.FirstOrDefault(x => x.SocialClub == player.socialClubName);

                if (character.AccountId == result)
                {
                    CharacterController.SelectCharacter(player);
                    API.shared.triggerClientEvent(player, "reset_menu");
                }                    
                else
                {
                    API.shared.sendChatMessageToPlayer(player, string.Format("~r~Вы ввели неверный пароль!"));
                    API.shared.triggerClientEvent(player, "login_char_menu", 0);
                    return;
                }
                ContextFactory.Instance.SaveChanges();
            }
        }

        private void onClientEventTrigger(Client player, string eventName, object[] args)
        {
            VehicleController vehicleController = EntityManager.GetVehicle(player.vehicle);
            CharacterController characterController = player.getData("CHARACTER");
            if (characterController == null) return;
            Character character = characterController.Character;            
            var FormatName = character.Name.Replace("_", " ");

            if (eventName == "playerlist_pings")
            {
                var players = API.getAllPlayers();
                var list = new List<string>();
                foreach (var ply in players)
                {
                    Character characterToList = ContextFactory.Instance.Character.FirstOrDefault(x => x.SocialClub == ply.socialClubName);
                    var dic = new Dictionary<string, object>();
                    dic["userName"] = characterToList.Name.ToString();
                    dic["userID"] = characterToList.OID;
                    dic["socialClubName"] = ply.socialClubName;
                    dic["ping"] = ply.ping;
                    list.Add(API.toJson(dic));
                }
                API.triggerClientEvent(player, "playerlist_pings", list);
            }
            if (eventName == "menu_handler_select_item")
            {
                int callback = (int)args[0];

                if (callback == 0) // Character Menu
                {
                    if ((int)args[1] == (int)args[2] - 1) CharacterController.CreateCharacter(player);
                    else CharacterController.SelectCharacter(player);
                }
                else if (callback == 1) // Vehicle Menu
                {
                    List<int> VehicleIDs = player.getData("VSTORAGE");

                    int vehID = VehicleIDs[(int)args[1]];
                    VehicleController _VehicleController = EntityManager.GetVehicle(vehID);
                    if (_VehicleController == null) VehicleController.LoadVehicle(player, vehID);
                    else _VehicleController.UnloadVehicle(character);
                    player.resetData("VSTORAGE");
                }
                else if (callback == 2)
                {
                    
                }
            }
            if (eventName == "change_clothes")
            {
                int slot = (int)args[0];
                int drawable = (int)args[1];
                int texture = (int)args[2];
                API.setPlayerClothes(player, slot, drawable, texture);
            }
            if (eventName == "change_accessory")
            {
                int slot = (int)args[0];
                int drawable = (int)args[1];
                int texture = (int)args[2];
                API.setPlayerAccessory(player, slot, drawable, texture);
            }

            // CAR MENU
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

                if (VehicleController.CheckAccess(characterController))
                {
                    if ((int)args[0] == 1)
                    {
                        API.setVehicleLocked(VehicleController.Vehicle, false);
                        ChatController.sendProxMessage(player, 15.0f, "~#C2A2DA~", FormatName + " открыл водительскую дверь.");
                    }
                    else
                    {
                        API.setVehicleLocked(VehicleController.Vehicle, true);
                        ChatController.sendProxMessage(player, 15.0f, "~#C2A2DA~", FormatName + " закрыл водительскую дверь.");
                    }
                }
                else API.sendNotificationToPlayer(player, "~r~ERROR: ~w~Вы не можете открыть данный транспорт!");
            }
            if (eventName == "engine_on")
            {
                VehicleController VehicleController = null;

                if (player.isInVehicle) VehicleController = EntityManager.GetVehicle(player.vehicle);
                else VehicleController = EntityManager.GetVehicleControllers().Find(x => x.Vehicle.position.DistanceTo(player.position) < 2.0f);

                if (character.DriverLicense == 0 && vehicleController.VehicleData.Model != Data.Models.RentModels.ScooterModel)
                {
                    API.sendNotificationToPlayer(player, "У вас нет прав на управление данным транспортом.");
                    return;
                }
                if (!vehicleController.CheckAccess(characterController) || 
                    vehicleController.VehicleData.RentTime == 0)
                {
                    API.sendNotificationToPlayer(player, "Вы не можете использовать данный транспорт.");
                    return;
                }
                else
                {
                    if (vehicleController.VehicleData.Fuel != 0)
                    {
                        vehicleController.Vehicle.engineStatus = true;
                        ChatController.sendProxMessage(player, 15.0f, "~#C2A2DA~", FormatName + " вставил ключ в зажигание и запустил мотор.");
                    }
                    else API.sendNotificationToPlayer(player, "~r~В данном транспорте закончился бензин!");
                }                    
            }
            if (eventName == "engine_off")
            {
                if (!vehicleController.CheckAccess(characterController))
                {
                    API.sendNotificationToPlayer(player, "Вы не можете использовать данный транспорт.");
                    return;
                }
                else
                {
                    vehicleController.Vehicle.engineStatus = false;
                    ChatController.sendProxMessage(player, 15.0f, "~#C2A2DA~", FormatName + " повернул ключ зажигания и заглушил мотор.");
                }
            }
            if (eventName == "park_vehicle")
            {
                if (vehicleController.CheckAccess(characterController))
                {
                    vehicleController.ParkVehicle(player);
                }
                else API.sendNotificationToPlayer(player, "~r~ОШИБКА: ~w~Вы не можете парковать данный транспорт");
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

                if (VehicleController.CheckAccess(characterController))
                {
                    if ((int)args[0] == 1)
                    {
                        VehicleController.TriggerDoor(VehicleController.Vehicle, 4);
                        ChatController.sendProxMessage(player, 15.0f, "~#C2A2DA~", FormatName + " открыл/закрыл капот.");
                    }
                    else
                    {
                        VehicleController.TriggerDoor(VehicleController.Vehicle, 5);
                        ChatController.sendProxMessage(player, 15.0f, "~#C2A2DA~", FormatName + " открыл/закрыл багажник.");
                    }
                }
                else API.sendNotificationToPlayer(player, "~r~ERROR: ~w~Вы не можете открыть капот или багажник данного транспорта.");
            }

            // RENT MENU
            if (eventName == "rent_scooter")
            {
                // Delete 30$ from cash
                if (character == null) return;
                if (character.Cash - Data.Models.Prices.ScooterRentPrice < 0)
                {
                    API.shared.sendNotificationToPlayer(player, "У вас недостаточно средств для аренды!");
                    return;
                }
                else
                {
                    character.Cash -= Data.Models.Prices.ScooterRentPrice;
                    API.shared.triggerClientEvent(player, "update_money_display", character.Cash);
                    
                    Data.Vehicle VehicleData = new Data.Vehicle();
                    VehicleData.Character = character;
                    VehicleController VehicleController = new VehicleController(VehicleData, API.createVehicle(VehicleHash.Faggio, player.position, player.rotation, 0, 0, 0));

                    VehicleData.Model = Data.Models.RentModels.ScooterModel;
                    VehicleData.PosX = player.position.X;
                    VehicleData.PosY = player.position.Y;
                    VehicleData.PosZ = player.position.Z;
                    VehicleData.Rot = player.rotation.Z;
                    VehicleData.Color1 = 0;
                    VehicleData.Color2 = 0;
                    VehicleData.RentTime = Data.Models.Time.ScooterRentTime;
                    VehicleData.Fuel = 10;
                    VehicleData.Type = 1;
                    VehicleData.GroupId = character.ActiveGroupID;

                    ContextFactory.Instance.Vehicle.Add(VehicleData);
                    ContextFactory.Instance.SaveChanges();
                }
                ContextFactory.Instance.SaveChanges();
            }
            if (eventName == "rent_prolong")
            {
                if (character == null) return;
                int callback = (int)args[0];
                int vehicleModel = (int)args[1];

                var vehicle = ContextFactory.Instance.Vehicle
                        .Where(x => x.Model == vehicleModel)
                        .FirstOrDefault(x => x.CharacterId == character.Id);

                if (callback == 1)
                {
                    // Checking money for prolongate
                    if (vehicleModel == Data.Models.RentModels.ScooterModel)
                    {
                        if (character.Cash - Data.Models.Prices.ScooterRentPrice < 0)
                        {
                            API.shared.sendNotificationToPlayer(player, "У вас недостаточно средств для продления аренды!");                            
                        }
                    }   // Scooter
                    if (vehicleModel == Data.Models.RentModels.TaxiModel)
                    {
                        if (character.Cash - Data.Models.Prices.TaxiRentPrice < 0)
                        {
                                API.shared.sendNotificationToPlayer(player, "У вас недостаточно средств для продления аренды!");                      
                        }
                    }    // Taxi 
                    else
                    {
                        // Adding additional time for using and take money
                        if (vehicleModel == Data.Models.RentModels.ScooterModel)
                        {
                            vehicle.RentTime = Data.Models.Time.ScooterRentTime;
                            character.Cash -= Data.Models.Prices.ScooterRentPrice;
                        }   // Scooter
                        if (vehicleModel == Data.Models.RentModels.TaxiModel)
                        {
                            vehicle.RentTime = Data.Models.Time.TaxiRentTime;
                            character.Cash -= Data.Models.Prices.TaxiRentPrice;
                        }    // Taxi 
                        ContextFactory.Instance.SaveChanges();
                    }                    
                }
                if (callback == 0)
                {
                    if (vehicleModel == Data.Models.RentModels.ScooterModel)
                    {
                        VehicleController _VehicleController = EntityManager.GetVehicle(vehicle);
                        _VehicleController.UnloadVehicle(character);
                        ContextFactory.Instance.Vehicle.Remove(vehicle);
                    }  // Scooter deleting
                    if (vehicleModel == Data.Models.RentModels.TaxiModel) VehicleController.RespawnWorkVehicle(vehicle, vehicleModel, 126, 126);

                    ContextFactory.Instance.SaveChanges();
                }
            }

            // WORK MENU
            if (eventName == "work_unemployers")
            {
                character.JobId = 888;
                ContextFactory.Instance.SaveChanges();
            }
            if (eventName == "work_loader")
            {
                if (character == null) return;
                int callback = (int)args[0];
                int jobId = (int)args[1];
                var posX = (float)args[2];
                var posY = (float)args[3];
                var posZ = (float)args[4];

                if (callback == 1)
                {
                    character.JobId = jobId;
                    ContextFactory.Instance.SaveChanges();
                    API.shared.triggerClientEvent(player, "loader_one", posX, posY, posZ);
                    player.setData("SECOND_OK", null);
                    SpawnManager.SetPlayerSkinClothes(player, 1, characterController.Character, 1);
                }       

                if (callback == 0)
                {
                    character.JobId = 0;
                    ContextFactory.Instance.SaveChanges();
                    API.shared.triggerClientEvent(player, "loader_end");
                    API.shared.triggerClientEvent(player, "update_money_display", character.Cash);
                    SpawnManager.SetPlayerSkinClothes(player, 0, characterController.Character, 1);
                    player.resetData("FIRST_OK");
                    player.resetData("SECOND_OK");
                }
            }
            if (eventName == "work_taxi")
            {
                int callback = (int)args[0];

                if (callback == 1) // Начало работы 
                {
                    var playerVehicles = ContextFactory.Instance.Vehicle.Where(x => x.JobId == Data.Models.JobsIdNonDataBase.TaxiDriver);

                    bool isPlayerInTaxi = false;

                    foreach (var playerVehicle in playerVehicles)
                    {
                        if (playerVehicle.JobId == character.JobId) isPlayerInTaxi = true;                        
                    }

                    if (isPlayerInTaxi == true)
                    {
                        if (character.Cash - Data.Models.Prices.TaxiRentPrice < 0)
                        {
                            API.sendChatMessageToPlayer(player, "~r~Вы не можете работать таксистом!\nУ вас нет " + Data.Models.Prices.TaxiRentPrice + "$ на аренду авто!");
                        }
                        else
                        {
                            var taxiVehicles = ContextFactory.Instance.Vehicle.Where(x => x.JobId == Data.Models.JobsIdNonDataBase.TaxiDriver);
                            bool hasPlayerTaxi = false;

                            foreach (var taxi in taxiVehicles)
                                if (taxi.Character == character) hasPlayerTaxi = true;                            

                            if (hasPlayerTaxi == true)
                                API.sendChatMessageToPlayer(player, "~r~У вас уже есть арендованное такси для работы!");
                            else
                            {
                                vehicleController.VehicleData.Character = characterController.Character;
                                vehicleController.VehicleData.RentTime += Data.Models.Time.TaxiRentTime;
                                character.Cash -= Data.Models.Prices.TaxiRentPrice;
                                character.TempVar = character.JobId;
                                ContextFactory.Instance.SaveChanges();

                                API.shared.triggerClientEvent(player, "update_money_display", character.Cash);
                                API.setEntityData(player, "TAXI", true);
                                API.sendChatMessageToPlayer(player, "~g~Вы за рулем такси,~s~ждите вызовов клиентов\nиспользуйте кнопку 3 для принятия заявки.");
                            }
                        }
                    }
                    else API.sendChatMessageToPlayer(player, "~r~Вы не за рулем такси,~s~ сядьте в машину\nи после этого вы сможете принимать заявки");
                }
                if (callback == 2) // [Command("accept")] 
                {
                    if (API.getEntityData(player.handle, "TAXI"))
                    {
                        double id = API.random();
                        API.call("JobController", "Accepted", player, id);
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(player, "~r~You are not a job");
                    }
                }
                if (callback == 3) // [Command("done")]
                {
                    API.setEntityData(player, "TASK", 1.623482);
                    API.sendChatMessageToPlayer(player, "~g~Вы свободны для клиентов.");
                }
                if (callback == 4)
                {
                    if (character.Cash - Data.Models.Prices.TaxiRentPrice < 0)
                    {
                        if (character.DriverLicense == 0)
                        {
                            API.sendChatMessageToPlayer(player, "~r~Вы не можете работать таксистом! У вас нет водительских прав!");
                            return;
                        }
                        API.sendChatMessageToPlayer(player, "~r~Вы не можете работать таксистом!\nУ вас нет " + Data.Models.Prices.TaxiRentPrice + "$ на аренду авто!");
                    }
                    else
                    {
                        if (character.JobId != 777 && character.Level >= 2)
                        {
                            API.sendChatMessageToPlayer(player, "~g~Поздравляем! Вы устроились на работу таксистом!\nПроследуйте в ближайщий таксопарк для аренды такси.");
                            character.JobId = 777;
                            ContextFactory.Instance.SaveChanges();
                            API.shared.triggerClientEvent(player, "markonmap", -1024, -2728);
                        }
                        else if (character.JobId == 777 && character.Level == 2)
                            API.sendChatMessageToPlayer(player, "~r~Вы уже работаете таксистом.");
                        else /*(character.JobId != 777 && character.Level < 2)*/ API.sendChatMessageToPlayer(player, "~r~Вы не можете работать таксистом!");
                    }
                }
                if (callback == 0) // Уволиться с работы
                {
                    if (player.isInVehicle)
                    {
                        API.sendNotificationToPlayer(player, "Пожалуйста выйдите из машины перед увольнением.");
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(player, "~r~Вы уволились с работы таксиста.\n~s~Для получения пособия пройдите в мэрию");
                        character.JobId = 0; //character.TempVar;
                        character.TempVar = 0;

                        var playerTaxiVehicle = ContextFactory.Instance.Vehicle.FirstOrDefault(x => x.CharacterId.ToString() == character.Id.ToString());
                        playerTaxiVehicle.Character = null;
                        playerTaxiVehicle.RentTime = 0;
                        ContextFactory.Instance.SaveChanges();

                        VehicleController.RespawnWorkVehicle(playerTaxiVehicle, Data.Models.RentModels.TaxiModel, 126, 126);
                        
                    }                    
                }
            }
            if (eventName == "get_taxi")
            {
                API.call("JobController", "UseTaxis", player);
                API.sendChatMessageToPlayer(player, "~b~Вызов такси для вас!");
            }

            // GANG MENU
            if (eventName == "gang_menu")
            {                
                if (character == null) return;
                string propertyName = (string)args[0];
                int trigger = (int)args[1];

                string stockName = propertyName;
                if (propertyName == "Army2_gang") stockName = "Army2_stock";
                if (propertyName == "Army1_gang") stockName = "Army1_stock";
                var propertyData = ContextFactory.Instance.Property.FirstOrDefault(x => x.Name == stockName);
                var getGroup = ContextFactory.Instance.Group.FirstOrDefault(x => x.Id == propertyData.GroupId);
                var groupType = (GroupType)Enum.Parse(typeof(GroupType), getGroup.Type.ToString());

                // Steal by yourself
                if (trigger == 1)
                {
                    if (propertyData.Stock - 500 < 0)
                        API.sendChatMessageToPlayer(player, "~r~Вы не можете украсть с данного склада!\nНа складе нет материалов!");
                    else
                    {
                        if (character.Material == 500)
                            API.sendChatMessageToPlayer(player, "~r~Вы не можете украсть с данного склада!\nВы перегружены у вас уже: " + character.Material + " материалов");
                        else
                        {
                            propertyData.Stock -= 500;
                            character.Material = 500;
                            ContextFactory.Instance.SaveChanges();
                            API.sendChatMessageToPlayer(player, "~g~Вы украли 500 материалов со склада: " + EntityManager.GetDisplayName(groupType));
                        }
                    }
                }
                // Steal by gang and vagoon
                if (trigger == 2)
                {
                    if (propertyData.Stock - 1000 < 0)
                        API.sendChatMessageToPlayer(player, "~r~Вы не можете украсть с данного склада!\nНа складе нет материалов!");
                    else
                    {
                        if (character.Material == 1000)
                            API.sendChatMessageToPlayer(player, "~r~Вы не можете украсть с данного склада!\nВы перегружены у вас уже: " + character.Material + " материалов");
                        else
                        {
                            propertyData.Stock -= 1000;
                            character.Material = 1000;
                            ContextFactory.Instance.SaveChanges();
                            API.sendChatMessageToPlayer(player, "~g~Вы украли 1000 материалов со склада: " + EntityManager.GetDisplayName(groupType) + "\nЗагрузите в свой транспорт и берите новую порцию со склада.");
                        }
                    }
                }
                // Unload materials by gang from vagoon
                if (trigger == 3)
                {
                    VehicleController vehicleControllerLoad = null;

                    if (player.isInVehicle)
                        vehicleControllerLoad = EntityManager.GetVehicle(player.vehicle);
                    else API.sendNotificationToPlayer(player, "Вы не в транспорте!");
                    if (vehicleControllerLoad == null) return;
                    
                    if (vehicleControllerLoad.VehicleData.Material != 0)
                    {
                        propertyData.Stock += vehicleControllerLoad.VehicleData.Material;
                        
                        API.sendChatMessageToPlayer(player, "~g~Вы разгрузили " + vehicleControllerLoad.VehicleData.Material.ToString() + " материалов с машины.\nНа свой склад.");
                        vehicleControllerLoad.VehicleData.Material = 0;
                        ContextFactory.Instance.SaveChanges();
                    }
                    else API.sendChatMessageToPlayer(player, "~r~В вашей машине нет материалов!");
                }
                // Work with form
                if (trigger == 4)
                {
                    if (character == null) return;

                    if (character.ClothesTypes != 0)
                    {
                        SpawnManager.SetPlayerSkinClothes(player, character.ClothesTypes, character, 1);
                        character.ActiveClothes = character.ClothesTypes;
                        ContextFactory.Instance.SaveChanges();
                    }                        
                    else API.sendChatMessageToPlayer(player, "~r~У вас нет доступной формы!");
                }
                if (trigger == 5)
                {
                    if (character == null) return;
                    var cloth = 0;
                    switch (propertyName)
                    {
                        case "Ballas_main":         cloth = 131; break;
                        case "Vagos_main":          cloth = 141; break;
                        case "LaCostaNotsa_main":   cloth = 151; break;
                        case "GroveStreet_main":    cloth = 161; break;
                        case "TheRifa_main":        cloth = 171; break;
                    }
                    SpawnManager.SetPlayerSkinClothesToDb(player, cloth, character, 1);
                    character.ActiveClothes = cloth;
                    ContextFactory.Instance.SaveChanges();
                }
            }
            if (eventName == "gang_weapon")
            {
                if (character == null) return;
                int callback = (int)args[0];
                int cost = (int)args[1];

                if (character.Material - cost < 0)
                    API.sendChatMessageToPlayer(player, "~r~Вы не можете создать данное оружие!\nУ вас недостаточно материалов!");
                else
                {
                    switch (callback)
                    {
                        case 1: character.Material -= cost;
                            API.givePlayerWeapon(player, WeaponHash.APPistol, 150, true, true); break;
                        case 2: character.Material -= cost;
                            API.givePlayerWeapon(player, WeaponHash.SMG, 250, true, true); break;
                        case 3: character.Material -= cost;
                            API.givePlayerWeapon(player, WeaponHash.AdvancedRifle, 350, true, true); break;
                        case 4: character.Material -= cost;
                            API.givePlayerWeapon(player, WeaponHash.HeavySniper, 150, true, true); break;
                        case 5: character.Material -= cost;
                            API.givePlayerWeapon(player, WeaponHash.GrenadeLauncher, 150, true, true); break;
                        case 6: character.Material -= cost;
                            API.givePlayerWeapon(player, WeaponHash.Grenade, 150, true, true); break;
                    }
                }
                ContextFactory.Instance.SaveChanges();                
            }

            if (eventName == "load_unload_material")
            {
                int trigger = (int)args[0];

                // Load material into vagoon
                if (trigger == 1)
                {
                    VehicleController vehicleControllerLoad = null;

                    if (player.isInVehicle)
                        vehicleControllerLoad = EntityManager.GetVehicle(player.vehicle);
                    else
                        vehicleControllerLoad = EntityManager.GetVehicleControllers().Find(x => x.Vehicle.position.DistanceTo(player.position) < 3.0f);
                    if (vehicleControllerLoad == null)
                    {
                        API.sendNotificationToPlayer(player, "Вы находитесь далеко от транспорта!");
                        return;
                    }
                    if (vehicleControllerLoad.VehicleData.Material + character.Material <= 10000)
                    {
                        vehicleControllerLoad.VehicleData.Material += character.Material;
                        API.sendChatMessageToPlayer(player, "~g~Вы загрузили " + character.Material.ToString() + " материалов со склада.\nВ свой транспорт. Берите очередную новую порцию со склада.");
                        character.Material = 0;
                        ContextFactory.Instance.SaveChanges();
                    }
                    else API.sendChatMessageToPlayer(player, "~r~Вы не можете загрузить в эту машину больше!\nОна перегружена и в ней" + vehicleControllerLoad.VehicleData.Material.ToString() + " материалов.");
                }                
            }

            // ARMY TWO MENU
            if (eventName == "army_two_menu")
            {
                if (character == null) return;
                int trigger = (int)args[0];
                string propertyName = (string)args[1];
                string propertyDestName = (string)args[2];

                var propertyData = ContextFactory.Instance.Property.FirstOrDefault(x => x.Name == propertyName);
                if (propertyData == null) return;

                // Change clothes for officers
                if (trigger == 4)
                {
                    var type = 0;

                    if (propertyDestName == "Cloth_up" &&
                        character.ActiveGroupID >= 2103 &&
                        character.ActiveGroupID <= 2114) type = 3;
                    if (propertyDestName == "Cloth_up" &&
                        character.ActiveGroupID == 2115) type = 4;

                    if (propertyDestName == "Cloth_up" &&
                        character.ActiveGroupID >= 2003 &&
                        character.ActiveGroupID <= 2014) type = 3;
                    if (propertyDestName == "Cloth_up" &&
                        character.ActiveGroupID == 2015) type = 4;

                    if (propertyDestName == "Cloth_down") type = 101;
                    
                    if (CharacterController.IsCharacterArmyInAllOfficers(character))
                    {
                        SpawnManager.SetPlayerSkinClothes(player, type, characterController.Character, 1);
                        characterController.Character.ActiveClothes = type;
                    }                     
                }

                var propertyDestData = ContextFactory.Instance.Property.FirstOrDefault(x => x.Name == propertyDestName);
                if (propertyDestData == null) return;

                // Load in Army 2 Stock
                if (trigger == 1)
                {
                    VehicleController vehicleControllerLoad = null;

                    if (player.isInVehicle)
                        vehicleControllerLoad = EntityManager.GetVehicle(player.vehicle);
                    else API.sendNotificationToPlayer(player, "Вы не в транспорте!");
                    if (vehicleControllerLoad == null) return;

                    if (propertyData.Stock - 20000 > 0)
                    {
                        if (vehicleController.VehicleData.Material == 0)
                        {
                            propertyData.Stock -= 20000;
                            vehicleControllerLoad.VehicleData.Material += 20000;
                            API.sendChatMessageToPlayer(player, "~g~Вы загрузили " + vehicleControllerLoad.VehicleData.Material.ToString() + " материалов в машину.");
                            API.triggerClientEvent(player, "markonmap", propertyDestData.ExtPosX, propertyDestData.ExtPosY);
                        }
                        else API.sendChatMessageToPlayer(player, "~r~Ваша машина заполнена!");
                    }
                    else API.sendChatMessageToPlayer(player, "~r~На вашем складе недостаточно материалов!");
                    ContextFactory.Instance.SaveChanges();
                }                
                // Unload to Police/FBI/ARMY 1/ARMY 2
                if (trigger == 2)
                {
                    VehicleController vehicleControllerLoad = null;

                    if (player.isInVehicle)
                        vehicleControllerLoad = EntityManager.GetVehicle(player.vehicle);
                    else API.sendNotificationToPlayer(player, "Вы не в транспорте!");
                    if (vehicleControllerLoad == null) return;

                    if (vehicleControllerLoad.VehicleData.Material != 0)
                    {
                        propertyData.Stock += vehicleControllerLoad.VehicleData.Material;

                        API.sendChatMessageToPlayer(player, "~g~Вы разгрузили " + vehicleControllerLoad.VehicleData.Material.ToString() + " материалов с машины.\nНа склад: " + propertyData.Name);
                        vehicleControllerLoad.VehicleData.Material = 0;
                    }
                    else API.sendChatMessageToPlayer(player, "~r~В вашей машине нет материалов!");
                    ContextFactory.Instance.SaveChanges();
                }
                // Load in Army 1 Source
                if (trigger == 3)
                {
                    VehicleController vehicleControllerLoad = null;

                    if (player.isInVehicle)
                        vehicleControllerLoad = EntityManager.GetVehicle(player.vehicle);
                    else API.sendNotificationToPlayer(player, "Вы не в транспорте!");
                    if (vehicleControllerLoad == null) return;

                    if (vehicleController.VehicleData.Material == 0)
                    {
                        vehicleControllerLoad.VehicleData.Material += 100000;
                        API.sendChatMessageToPlayer(player, "~g~Вы загрузили " + vehicleControllerLoad.VehicleData.Material.ToString() + " материалов в машину.");
                        API.triggerClientEvent(player, "markonmap", propertyDestData.ExtPosX, propertyDestData.ExtPosY);
                    }
                    else API.sendChatMessageToPlayer(player, "~r~Ваша машина заполнена!");                    
                    ContextFactory.Instance.SaveChanges();
                }                
            }
            if (eventName == "army_two_weapon")
            {
                if (character == null) return;
                int callback = (int)args[0];
                int cost = (int)args[1];

                var propertyData = ContextFactory.Instance.Property.FirstOrDefault(x => x.Name == "Army2_stock");
                if (propertyData.Stock - cost < 0)
                {
                    API.sendChatMessageToPlayer(player, "~r~Вы не можете взять данное оружие!\nНа складе нет достаточно материалов!");
                }
                else
                {
                    switch (callback)
                    {
                        case 1:
                            propertyData.Stock -= cost;
                            API.givePlayerWeapon(player, WeaponHash.APPistol, 150, true, true); break;
                        case 2:
                            propertyData.Stock -= cost;
                            API.givePlayerWeapon(player, WeaponHash.SMG, 250, true, true); break;
                        case 3:
                            propertyData.Stock -= cost;
                            API.givePlayerWeapon(player, WeaponHash.AdvancedRifle, 350, true, true); break;
                        case 4:
                            propertyData.Stock -= cost;
                            API.givePlayerWeapon(player, WeaponHash.HeavySniper, 150, true, true); break;
                        case 5:
                            propertyData.Stock -= cost;
                            API.givePlayerWeapon(player, WeaponHash.GrenadeLauncher, 150, true, true); break;
                        case 6:
                            propertyData.Stock -= cost;
                            API.givePlayerWeapon(player, WeaponHash.Grenade, 150, true, true); break;
                    }
                }
                ContextFactory.Instance.SaveChanges();                  
            }
            if (eventName == "gang_ballas_add_to_group")
            {
                int callBack = (int)args[2];
                if (character == null) return;

                // Принятие в банду
                if (callBack == 1)
                {
                    try
                    {
                        int userID = (int)args[0];
                        int gangID = (int)args[1];

                        var targetCharacter = ContextFactory.Instance.Character.FirstOrDefault(x => x.OID == userID);
                        if (targetCharacter == null)
                        {
                            API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~Вы ввели неверный пользовательский ID.");
                            return;
                        }
                        targetCharacter.ActiveGroupID = gangID;
                        targetCharacter.JobId = 0;
                        ContextFactory.Instance.SaveChanges();

                        Client target = API.shared.getAllPlayers().FirstOrDefault(x => x.socialClubName == targetCharacter.SocialClub);
                        if (target == null) return;

                        targetCharacter.ActiveClothes = SpawnManager.SetFractionClothes(target, gangID, targetCharacter);
                        ContextFactory.Instance.SaveChanges();

                        var getGroup = ContextFactory.Instance.Group.FirstOrDefault(x => x.Id == gangID);
                        var groupType = (GroupType)Enum.Parse(typeof(GroupType), getGroup.Type.ToString());
                        var groupExtraType = (GroupExtraType)Enum.Parse(typeof(GroupExtraType), getGroup.ExtraType.ToString());
                       
                        target.sendChatMessage("~g~[СЕРВЕР]: ~w~Игрок " + FormatName + " принял вас в банду: " + EntityManager.GetDisplayName(groupType) + "\nНа должность: " + EntityManager.GetDisplayName(groupExtraType));
                        API.sendChatMessageToPlayer(player, "~g~[УСПЕШНО]: ~w~Вы перевели пользователя: " + targetCharacter.Name.ToString() + "\nВо фракцию: " + EntityManager.GetDisplayName(groupType) + "\nНа должность: " + EntityManager.GetDisplayName(groupExtraType));
                    }
                    catch (Exception e)
                    {
                        API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~Вы ввели неверный пользовательский ID.");
                        return;
                    }
                }
                // Выгнать из банды
                if (callBack == 2)
                {
                    try
                    {
                        int userID = (int)args[0];
                        int gangID = (int)args[1];

                        var targetCharacter = ContextFactory.Instance.Character.FirstOrDefault(x => x.OID == userID);
                        if (targetCharacter == null)
                        {
                            API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~Вы ввели неверный пользовательский ID.");
                            return;
                        }

                        if (CharacterController.IsCharacterInGang(targetCharacter) &&
                            !CharacterController.IsCharacterGangBoss(character))
                        {
                            targetCharacter.ActiveGroupID = 2;
                            ContextFactory.Instance.SaveChanges();

                            Client target = API.shared.getAllPlayers().FirstOrDefault(x => x.socialClubName == targetCharacter.SocialClub);
                            if (target == null) return;
                            targetCharacter.ActiveClothes = SpawnManager.SetFractionClothes(target, 0, targetCharacter);
                            ContextFactory.Instance.SaveChanges();

                            var getGroup = ContextFactory.Instance.Group.FirstOrDefault(x => x.Id == gangID);
                            var groupType = (GroupType)Enum.Parse(typeof(GroupType), getGroup.Type.ToString());
                            var groupExtraType = (GroupExtraType)Enum.Parse(typeof(GroupExtraType), getGroup.ExtraType.ToString());
                                                       
                            target.sendChatMessage("~g~[СЕРВЕР]: ~w~Игрок " + FormatName + " выгнал вас из банды: " + EntityManager.GetDisplayName(groupType) + "\nДля пособия по безработице - проследуйте в мэрию.");
                            API.sendChatMessageToPlayer(player, "~g~[УСПЕШНО]: ~w~Вы выгнали пользователя: " + targetCharacter.Name.ToString() + "\nИз фракции: " + EntityManager.GetDisplayName(groupType));
                        }                        
                        else
                        {
                            API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~Пользователь не состоит в вашей фракции\nИли вы пытаетесь выгнать сами себя!");
                            return;
                        }
                    }
                    catch (Exception e)
                    {
                        API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~Вы ввели неверный пользовательский ID.");
                        return;
                    }
                }

                // Поменять звание TODO
                if (callBack == 3)
                {
                    try
                    {
                        int userID = (int)args[0];
                        int rangID = (int)args[1];
                        int groupID = (int)args[3];

                        var targetCharacter = ContextFactory.Instance.Character.FirstOrDefault(x => x.OID == userID);
                        if (targetCharacter == null)
                        {
                            API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~Вы ввели неверный пользовательский ID.");
                            return;
                        }

                        int correctGroupId = 0;
                        if (groupID >= 2012 && groupID <= 2015)
                        {
                            correctGroupId = 2000;
                            goto x;
                        }
                        if (groupID >= 2112 && groupID <= 2115)
                        {
                            correctGroupId = 2100;
                            goto y;
                        }

                        x: if (rangID >= 1 && rangID <= 11 && groupID >= 2012 && groupID <= 2014)
                        {
                            targetCharacter.ActiveGroupID = correctGroupId + rangID;
                            ContextFactory.Instance.SaveChanges();

                            Client target = API.shared.getAllPlayers().FirstOrDefault(x => x.socialClubName == targetCharacter.SocialClub);
                            if (target == null) return;

                            var getGroup = ContextFactory.Instance.Group.FirstOrDefault(x => x.Id == targetCharacter.ActiveGroupID);
                            var groupType = (GroupType)Enum.Parse(typeof(GroupType), getGroup.Type.ToString());
                            var groupExtraType = (GroupExtraType)Enum.Parse(typeof(GroupExtraType), getGroup.ExtraType.ToString());

                            target.sendChatMessage("~g~[СЕРВЕР]: ~w~Игрок " + FormatName + " присвоил вам звание: " + EntityManager.GetDisplayName(groupExtraType));
                            API.sendChatMessageToPlayer(player, "~g~[УСПЕШНО]: ~w~Вы присвоили пользователю: " + targetCharacter.Name.ToString() + "\nВоинское звание: " + EntityManager.GetDisplayName(groupExtraType));
                        }
                        else if (rangID >= 1 && rangID <= 14 && groupID == 2015)
                        {
                            targetCharacter.ActiveGroupID = correctGroupId + rangID;
                            ContextFactory.Instance.SaveChanges();

                            Client target = API.shared.getAllPlayers().FirstOrDefault(x => x.socialClubName == targetCharacter.SocialClub);
                            if (target == null) return;

                            var getGroup = ContextFactory.Instance.Group.FirstOrDefault(x => x.Id == targetCharacter.ActiveGroupID);
                            var groupType = (GroupType)Enum.Parse(typeof(GroupType), getGroup.Type.ToString());
                            var groupExtraType = (GroupExtraType)Enum.Parse(typeof(GroupExtraType), getGroup.ExtraType.ToString());

                            target.sendChatMessage("~g~[СЕРВЕР]: ~w~Игрок " + FormatName + " присвоил вам звание: " + EntityManager.GetDisplayName(groupExtraType));
                            API.sendChatMessageToPlayer(player, "~g~[УСПЕШНО]: ~w~Вы присвоили пользователю: " + targetCharacter.Name.ToString() + "\nВоинское звание: " + EntityManager.GetDisplayName(groupExtraType));
                        }
                        else
                        {
                            API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~Пользователь не состоит в вашей фракции\nИли вам недопустимо присвоение данного звания!");
                            return;
                        }

                        y: if (rangID >= 1 && rangID <= 11 && groupID >= 2112 && groupID <= 2114)
                        {
                            targetCharacter.ActiveGroupID = correctGroupId + rangID;
                            ContextFactory.Instance.SaveChanges();

                            Client target = API.shared.getAllPlayers().FirstOrDefault(x => x.socialClubName == targetCharacter.SocialClub);
                            if (target == null) return;

                            var getGroup = ContextFactory.Instance.Group.FirstOrDefault(x => x.Id == targetCharacter.ActiveGroupID);
                            var groupType = (GroupType)Enum.Parse(typeof(GroupType), getGroup.Type.ToString());
                            var groupExtraType = (GroupExtraType)Enum.Parse(typeof(GroupExtraType), getGroup.ExtraType.ToString());

                            target.sendChatMessage("~g~[СЕРВЕР]: ~w~Игрок " + FormatName + " присвоил вам звание: " + EntityManager.GetDisplayName(groupExtraType));
                            API.sendChatMessageToPlayer(player, "~g~[УСПЕШНО]: ~w~Вы присвоили пользователю: " + targetCharacter.Name.ToString() + "\nВоинское звание: " + EntityManager.GetDisplayName(groupExtraType));
                        }
                        else if (rangID >= 1 && rangID <= 14 && groupID == 2115)
                        {
                            targetCharacter.ActiveGroupID = correctGroupId + rangID;
                            ContextFactory.Instance.SaveChanges();

                            Client target = API.shared.getAllPlayers().FirstOrDefault(x => x.socialClubName == targetCharacter.SocialClub);
                            if (target == null) return;

                            var getGroup = ContextFactory.Instance.Group.FirstOrDefault(x => x.Id == targetCharacter.ActiveGroupID);
                            var groupType = (GroupType)Enum.Parse(typeof(GroupType), getGroup.Type.ToString());
                            var groupExtraType = (GroupExtraType)Enum.Parse(typeof(GroupExtraType), getGroup.ExtraType.ToString());

                            target.sendChatMessage("~g~[СЕРВЕР]: ~w~Игрок " + FormatName + " присвоил вам звание: " + EntityManager.GetDisplayName(groupExtraType));
                            API.sendChatMessageToPlayer(player, "~g~[УСПЕШНО]: ~w~Вы присвоили пользователю: " + targetCharacter.Name.ToString() + "\nВоинское звание: " + EntityManager.GetDisplayName(groupExtraType));
                        }
                        else
                        {
                            API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~Пользователь не состоит в вашей фракции\nИли вам недопустимо присвоение данного звания!");
                            return;
                        }
                    }
                    catch (Exception e)
                    {
                        API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~Вы ввели неверный пользовательский ID.");
                        return;
                    }
                }
            }
            if (eventName == "national_guard_add_to_group")
            {
                int callBack = (int)args[2];
                if (character == null) return;

                // Принятие в армию
                if (callBack == 1)
                {
                    try
                    {
                        int userID = (int)args[0];
                        int armyID = (int)args[1];

                        var targetCharacter = ContextFactory.Instance.Character.FirstOrDefault(x => x.OID == userID);
                        if (targetCharacter == null)
                        {
                            API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~Вы ввели неверный пользовательский ID.");
                            return;
                        }
                        targetCharacter.ActiveGroupID = armyID;
                        targetCharacter.JobId = 0;
                        ContextFactory.Instance.SaveChanges();

                        Client target = API.shared.getAllPlayers().FirstOrDefault(x => x.socialClubName == targetCharacter.SocialClub);
                        if (target == null) return;
                        targetCharacter.ActiveClothes = SpawnManager.SetFractionClothes(target, armyID, targetCharacter);
                        ContextFactory.Instance.SaveChanges();

                        var getGroup = ContextFactory.Instance.Group.FirstOrDefault(x => x.Id == armyID);
                        var groupType = (GroupType)Enum.Parse(typeof(GroupType), getGroup.Type.ToString());
                        var groupExtraType = (GroupExtraType)Enum.Parse(typeof(GroupExtraType), getGroup.ExtraType.ToString());
                                                
                        target.sendChatMessage("~g~[СЕРВЕР]: ~w~Игрок " + FormatName + " принял вас в армию: " + EntityManager.GetDisplayName(groupType) + "\nНа звание: " + EntityManager.GetDisplayName(groupExtraType));
                        API.sendChatMessageToPlayer(player, "~g~[УСПЕШНО]: ~w~Вы перевели пользователя: " + targetCharacter.Name.ToString() + "\nВо фракцию: " + EntityManager.GetDisplayName(groupType) + "\nНа должность: " + EntityManager.GetDisplayName(groupExtraType));
                    }
                    catch (Exception e)
                    {
                        API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~Вы ввели неверный пользовательский ID.");
                        return;
                    }
                }
                // Выгнать из армии
                if (callBack == 2)
                {
                    try
                    {
                        int userID = (int)args[0];
                        int groupID = (int)args[1];

                        var targetCharacter = ContextFactory.Instance.Character.FirstOrDefault(x => x.OID == userID);
                        if (targetCharacter == null)
                        {
                            API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~Вы ввели неверный пользовательский ID.");
                            return;
                        }

                        if (CharacterController.IsCharacterInArmy(targetCharacter) && 
                            groupID != character.ActiveGroupID)
                        {
                            targetCharacter.ActiveGroupID = 2;
                            ContextFactory.Instance.SaveChanges();

                            Client target = API.shared.getAllPlayers().FirstOrDefault(x => x.socialClubName == targetCharacter.SocialClub);
                            if (target == null) return;
                            targetCharacter.ActiveClothes = SpawnManager.SetFractionClothes(target, 0, targetCharacter);
                            ContextFactory.Instance.SaveChanges();

                            var getGroup = ContextFactory.Instance.Group.FirstOrDefault(x => x.Id == groupID);
                            var groupType = (GroupType)Enum.Parse(typeof(GroupType), getGroup.Type.ToString());
                            var groupExtraType = (GroupExtraType)Enum.Parse(typeof(GroupExtraType), getGroup.ExtraType.ToString());

                            target.sendChatMessage("~g~[СЕРВЕР]: ~w~Игрок " + FormatName + " выгнал вас из фракции: " + EntityManager.GetDisplayName(groupType) + "\nДля пособия по безработице - проследуйте в мэрию.");
                            API.sendChatMessageToPlayer(player, "~g~[УСПЕШНО]: ~w~Вы выгнали пользователя: " + targetCharacter.Name.ToString() + "\nИз фракции: " + EntityManager.GetDisplayName(groupType));
                        }                        
                        else
                        {
                            API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~Пользователь не состоит в вашей фракции.\nЛибо вы пытаетесь выгнать сами себя!");
                            return;
                        }
                    }
                    catch (Exception e)
                    {
                        API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~Вы ввели неверный пользовательский ID.");
                        return;
                    }
                }
                // Поменять звание
                if (callBack == 3)
                {
                    try
                    {
                        int userID = (int)args[0];
                        int rangID = (int)args[1];
                        int groupID = (int)args[3];
                        int correctGroupId = 0;

                        var targetCharacter = ContextFactory.Instance.Character.FirstOrDefault(x => x.OID == userID);
                        if (targetCharacter == null)
                        {
                            API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~Вы ввели неверный пользовательский ID.");
                            return;
                        }

                        if (groupID >= 2012 && groupID <= 2015) correctGroupId = 2000;
                        if (groupID >= 2112 && groupID <= 2115) correctGroupId = 2100;

                        if (rangID >= 1 && rangID <= 11 && CharacterController.IsCharacterArmyHighOfficer(character))
                        {
                            targetCharacter.ActiveGroupID = correctGroupId + rangID;
                            ContextFactory.Instance.SaveChanges();

                            Client target = API.shared.getAllPlayers().FirstOrDefault(x => x.socialClubName == targetCharacter.SocialClub);
                            if (target == null) return;

                            if (CharacterController.IsCharacterArmyInAllOfficers(targetCharacter))
                            {
                                SpawnManager.SetPlayerSkinClothesToDb(target, 101, targetCharacter, 1);
                                SpawnManager.SetPlayerSkinClothes(target, 3, targetCharacter, 1);
                                targetCharacter.ActiveClothes = 3;
                                ContextFactory.Instance.SaveChanges();
                            }

                            var getGroup = ContextFactory.Instance.Group.FirstOrDefault(x => x.Id == targetCharacter.ActiveGroupID);
                            var groupType = (GroupType)Enum.Parse(typeof(GroupType), getGroup.Type.ToString());
                            var groupExtraType = (GroupExtraType)Enum.Parse(typeof(GroupExtraType), getGroup.ExtraType.ToString());

                            target.sendChatMessage("~g~[СЕРВЕР]: ~w~Игрок " + FormatName + " присвоил вам звание: " + EntityManager.GetDisplayName(groupExtraType));
                            API.sendChatMessageToPlayer(player, "~g~[УСПЕШНО]: ~w~Вы присвоили пользователю: " + targetCharacter.Name.ToString() + "\nВоинское звание: " + EntityManager.GetDisplayName(groupExtraType));
                        }
                        else if (rangID >= 1 && rangID <= 14 && CharacterController.IsCharacterArmyGeneral(character))
                        {
                            targetCharacter.ActiveGroupID = correctGroupId + rangID;
                            ContextFactory.Instance.SaveChanges();

                            Client target = API.shared.getAllPlayers().FirstOrDefault(x => x.socialClubName == targetCharacter.SocialClub);
                            if (target == null) return;

                            if (CharacterController.IsCharacterArmyInAllOfficers(targetCharacter))
                            {
                                SpawnManager.SetPlayerSkinClothesToDb(target, 101, targetCharacter, 1);
                                SpawnManager.SetPlayerSkinClothes(target, 3, targetCharacter, 1);
                                targetCharacter.ActiveClothes = 3;
                                ContextFactory.Instance.SaveChanges();
                            }

                            var getGroup = ContextFactory.Instance.Group.FirstOrDefault(x => x.Id == targetCharacter.ActiveGroupID);
                            var groupType = (GroupType)Enum.Parse(typeof(GroupType), getGroup.Type.ToString());
                            var groupExtraType = (GroupExtraType)Enum.Parse(typeof(GroupExtraType), getGroup.ExtraType.ToString());
                            
                            target.sendChatMessage("~g~[СЕРВЕР]: ~w~Игрок " + FormatName + " присвоил вам звание: " + EntityManager.GetDisplayName(groupExtraType));
                            API.sendChatMessageToPlayer(player, "~g~[УСПЕШНО]: ~w~Вы присвоили пользователю: " + targetCharacter.Name.ToString() + "\nВоинское звание: " + EntityManager.GetDisplayName(groupExtraType));
                        }
                        else
                        {
                            API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~Пользователь не состоит в вашей фракции\nИли вам недопустимо присвоение данного звания!");
                            return;
                        }
                    }
                    catch (Exception e)
                    {
                        API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~Вы ввели неверный пользовательский ID.");
                        return;
                    }
                }
            }
            if (eventName == "buy_driver_license")
            {
                if (character == null) return;
                if (character.Cash - Data.Models.Prices.DriverLicensePrice < 0)
                {
                    API.shared.sendNotificationToPlayer(player, "У вас нет " + Data.Models.Prices.DriverLicensePrice + "$ для покупки прав!");
                    return;
                }
                else
                {
                    character.Cash -= Data.Models.Prices.DriverLicensePrice;
                    character.DriverLicense = 1;
                    API.shared.triggerClientEvent(player, "update_money_display", character.Cash);
                    ContextFactory.Instance.SaveChanges();
                }
            }
            if (eventName == "got_driver_license")
            {
                if (character == null) return;
                try
                {
                    int userID = (int)args[0];
                    var targetCharacter = ContextFactory.Instance.Character.FirstOrDefault(x => x.OID == userID);
                    if (targetCharacter == null)
                    {
                        API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~Вы ввели неверный пользовательский ID.");
                        return;
                    }
                    targetCharacter.DriverLicense = 1;
                    ContextFactory.Instance.SaveChanges();

                    Client target = API.shared.getAllPlayers().FirstOrDefault(x => x.socialClubName == targetCharacter.SocialClub);
                    if (target == null) return;

                    target.sendChatMessage("~g~[СЕРВЕР]: ~w~Игрок " + FormatName + " выдал вам водительские права \nкатегории " + targetCharacter.DriverLicense.ToString());
                    API.sendChatMessageToPlayer(player, "~g~[УСПЕШНО]: ~w~Вы выдали права категории" + targetCharacter.DriverLicense.ToString() + "\nпользователю с ID: " + userID);
                }
                catch (Exception e)
                {
                    API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~Вы ввели неверный пользовательский ID.");
                    return;
                }
            }

            if (eventName == "admin_add_to_group")
            {
                if (character == null) return;
                try
                {
                    int userID = (int)args[0];
                    int groupID = (int)args[1];
                    var targetCharacter = ContextFactory.Instance.Character.FirstOrDefault(x => x.OID == userID);
                    if (targetCharacter == null)
                    {
                        API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~Вы ввели неверный пользовательский ID.");
                        return;
                    }
                    targetCharacter.ActiveGroupID = groupID;
                    ContextFactory.Instance.SaveChanges();

                    Client target = API.shared.getAllPlayers().FirstOrDefault(x => x.socialClubName == targetCharacter.SocialClub);
                    if (target == null) return;

                    var getGroup = ContextFactory.Instance.Group.FirstOrDefault(x => x.Id == groupID);
                    var groupType = (GroupType)Enum.Parse(typeof(GroupType), getGroup.Type.ToString());
                    var groupExtraType = (GroupExtraType)Enum.Parse(typeof(GroupExtraType), getGroup.ExtraType.ToString());

                    if (CharacterController.IsCharacterArmyGeneral(targetCharacter))
                    {
                        SpawnManager.SetPlayerSkinClothes(target, 4, targetCharacter, 1);
                        targetCharacter.ActiveClothes = 4;
                    }
                    if (CharacterController.IsCharacterGangBoss(targetCharacter))
                    {
                        SpawnManager.SetPlayerSkinClothes(target, 131, targetCharacter, 1);
                        targetCharacter.ActiveClothes = 131;
                    }
                    if (targetCharacter.ActiveGroupID == 101 || targetCharacter.ActiveGroupID == 101)
                    {
                        SpawnManager.SetPlayerSkinClothes(target, 10, targetCharacter, 1);
                        targetCharacter.ActiveClothes = 10;
                    }

                    target.sendChatMessage("~g~[СЕРВЕР]: ~w~Игрок " + FormatName + " перевел вас во фракцию: " + EntityManager.GetDisplayName(groupType) + "\nНа должность: " + EntityManager.GetDisplayName(groupExtraType));
                    API.sendChatMessageToPlayer(player, "~g~[УСПЕШНО]: ~w~Вы перевели пользователя: " + targetCharacter.Name.ToString() + "\nВо фракцию: " + EntityManager.GetDisplayName(groupType) + "\nНа должность: " + EntityManager.GetDisplayName(groupExtraType));
                }
                catch (Exception e)
                {
                    API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~Вы ввели неверный пользовательский ID.");
                    return;
                }
            }
            if (eventName == "admin_add_to_admin")
            {
                if (character == null) return;
                try
                {
                    int userID = (int)args[0];
                    int adminID = (int)args[1];
                    var targetCharacter = ContextFactory.Instance.Character.FirstOrDefault(x => x.OID == userID);
                    if (targetCharacter == null)
                    {
                        API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~Вы ввели неверный пользовательский ID.");
                        return;
                    }
                    targetCharacter.Admin = adminID;
                    ContextFactory.Instance.SaveChanges();

                    Client target = API.shared.getAllPlayers().FirstOrDefault(x => x.socialClubName == targetCharacter.SocialClub);
                    if (target == null) return;
                    
                    target.sendChatMessage("~g~[СЕРВЕР]: ~w~Игрок " + FormatName + " сделал Вас администратором уровня: " + adminID.ToString());
                    API.sendChatMessageToPlayer(player, "~g~[УСПЕШНО]: ~w~Вы сделали пользователя: " + targetCharacter.Name.ToString() + "\nАдминистратором уровня: " + adminID.ToString());
                }
                catch (Exception e)
                {
                    API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~Вы ввели неверный пользовательский ID.");
                    return;
                }
            }
            if (eventName == "admin_change user_level")
            {
                if (character == null) return;
                try
                {
                    int userID = (int)args[0];
                    int level = (int)args[1];
                    var targetCharacter = ContextFactory.Instance.Character.FirstOrDefault(x => x.OID == userID);
                    if (targetCharacter == null)
                    {
                        API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~Вы ввели неверный пользовательский ID.");
                        return;
                    }
                    targetCharacter.Level = level;
                    ContextFactory.Instance.SaveChanges();

                    Client target = API.shared.getAllPlayers().FirstOrDefault(x => x.socialClubName == targetCharacter.SocialClub);
                    if (target == null) return;

                    target.sendChatMessage("~g~[СЕРВЕР]: ~w~Игрок " + FormatName + " сделал вам уровень: " + level.ToString());
                    API.sendChatMessageToPlayer(player, "~g~[УСПЕШНО]: ~w~Вы сделали пользователю: " + targetCharacter.Name.ToString() + " уровень: " + level.ToString());
                }
                catch (Exception e)
                {
                    API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~Вы ввели неверный пользовательский ID.");
                    return;
                }
            }            
        }
    }
}
