using GTANetworkServer;
using GTANetworkShared;

using TheGodfatherGM.Data;
using TheGodfatherGM.Data.Enums;
using TheGodfatherGM.Server.Groups;
using TheGodfatherGM.Server.Vehicles;
using TheGodfatherGM.Server.DBManager;
using TheGodfatherGM.Server.Characters;

using System.Security.Cryptography;
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;

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
            if (eventName == "create_char")
            {
                if (CharacterController.NameValidityCheck(player, args[0].ToString()))
                {
                    // Password correct
                    MD5 md5 = new MD5CryptoServiceProvider();
                    string pass = args[1].ToString();
                    byte[] checkSum = md5.ComputeHash(Encoding.UTF8.GetBytes(pass));
                    string result = BitConverter.ToString(checkSum).Replace("-", String.Empty);
                    CharacterController newCharacterController = new CharacterController(player, args[0].ToString(), result, (int)args[2]);
                }
                else
                {
                    // Password is wrong
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
                    CharacterController.SelectCharacter(player, character);
                    API.shared.triggerClientEvent(player, "reset_menu");
                }                    
                else
                {
                    API.shared.sendChatMessageToPlayer(player, string.Format("~r~Вы ввели неверный пароль!"));
                    API.shared.triggerClientEvent(player, "login_char_menu", character.Language);
                    return;
                }
                ContextFactory.Instance.SaveChanges();
            }
            if (eventName == "change_face")
            {
                int value = (int)args[1];                

                switch (args[0])
                {
                    case "GTAO_SHAPE_FIRST_ID":
                        API.setEntitySyncedData(player, "GTAO_SHAPE_FIRST_ID", value);
                        CharacterController.UpdatePlayerFace(player.handle);
                        //API.exported.gtaocharacter.updatePlayerFace(player.handle);
                        break;
                    case "GTAO_SHAPE_SECOND_ID":
                        API.setEntitySyncedData(player, "GTAO_SHAPE_SECOND_ID", value);
                        CharacterController.UpdatePlayerFace(player.handle);
                        //API.exported.gtaocharacter.updatePlayerFace(player.handle); 
                        break;
                    case "GTAO_SKIN_FIRST_ID":
                        API.setEntitySyncedData(player, "GTAO_SKIN_FIRST_ID", value);
                        CharacterController.UpdatePlayerFace(player.handle);
                        //API.exported.gtaocharacter.updatePlayerFace(player.handle); 
                        break;
                    case "GTAO_HAIR":
                        API.setPlayerClothes(player, 2, value, 0); break;
                    case "GTAO_HAIR_COLOR":
                        API.setEntitySyncedData(player, "GTAO_HAIR_COLOR", value);
                        CharacterController.UpdatePlayerFace(player.handle);
                        //API.exported.gtaocharacter.updatePlayerFace(player.handle);
                        break;
                    case "GTAO_EYE_COLOR":
                        API.setEntitySyncedData(player, "GTAO_EYE_COLOR", value);
                        CharacterController.UpdatePlayerFace(player.handle);
                        //API.exported.gtaocharacter.updatePlayerFace(player.handle); 
                        break;
                    case "GTAO_EYEBROWS":
                        API.setEntitySyncedData(player, "GTAO_EYEBROWS", value);
                        CharacterController.UpdatePlayerFace(player.handle);
                        //API.exported.gtaocharacter.updatePlayerFace(player.handle);
                        break;
                    case "GTAO_EYEBROWS_COLOR":
                        API.setEntitySyncedData(player, "GTAO_EYEBROWS_COLOR", value);
                        CharacterController.UpdatePlayerFace(player.handle);
                        //API.exported.gtaocharacter.updatePlayerFace(player.handle); 
                        break;                    
                }
            }
            if (eventName == "custom_char")
            {
                var faceJson = JsonConvert.DeserializeObject<Faces>((string)args[0]);
                var character = ContextFactory.Instance.Character.First(x => x.SocialClub == player.socialClubName);

                Faces characterFace = new Faces();
                characterFace.CharacterId = character.Id;

                characterFace.SEX = faceJson.SEX;
                characterFace.GTAO_SHAPE_FIRST_ID = faceJson.GTAO_SHAPE_FIRST_ID;
                characterFace.GTAO_SHAPE_SECOND_ID = faceJson.GTAO_SHAPE_SECOND_ID;
                characterFace.GTAO_SKIN_FIRST_ID = faceJson.GTAO_SKIN_FIRST_ID;
                characterFace.GTAO_HAIR = faceJson.GTAO_HAIR;
                characterFace.GTAO_HAIR_COLOR = faceJson.GTAO_HAIR_COLOR;
                characterFace.GTAO_EYE_COLOR = faceJson.GTAO_EYE_COLOR;
                characterFace.GTAO_EYEBROWS = faceJson.GTAO_EYEBROWS;
                characterFace.GTAO_EYEBROWS_COLOR = faceJson.GTAO_EYEBROWS_COLOR;

                character.Model = faceJson.SEX;
                character.ModelName = faceJson.SEX.ToString();

                ContextFactory.Instance.Faces.Add(characterFace);
                ContextFactory.Instance.SaveChanges();

                CharacterController.SelectCharacter(player, character);
                API.shared.triggerClientEvent(player, "reset_menu");
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
                /*
                if (callback == 0) // Character Menu
                {
                    if ((int)args[1] == (int)args[2] - 1) CharacterController.CreateCharacter(player);
                    else CharacterController.SelectCharacter(player);
                }
                else */
                if (callback == 1) // Vehicle Menu
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
            if (eventName == "house_menu_buysell")
            {
                var propertyId = (int)args[0];
                var cost = (int)args[1];
                var trigger = (int)args[2];

                var buyedProperty = ContextFactory.Instance.Property.First(x => x.PropertyID == propertyId);
                if (buyedProperty == null) return;

                // Buy house
                if (trigger == 1)
                {
                    if (character.Cash - cost < 0)
                    {
                        API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~У вас недостаточно средств для покупки!");
                        return;
                    }
                    else
                    {
                        character.Cash -= cost;
                        buyedProperty.CharacterId = character.Id;

                        var blips = API.getAllBlips();
                        foreach (var blip in blips)
                        {
                            var blipPos = API.getBlipPosition(blip);
                            if (blipPos.X == buyedProperty.ExtPosX &&
                                blipPos.Y == buyedProperty.ExtPosY &&
                                blipPos.Z == buyedProperty.ExtPosZ)
                                API.setBlipColor(blip, 1);
                        }
                        var labels = API.getAllLabels();
                        foreach (var label in labels)
                        {
                            var labelText = API.getTextLabelText(label);
                            if (labelText.ToString().Contains("~g~Купить дом №" + buyedProperty.PropertyID))
                                API.setTextLabelText(label, "~g~Вход в дом №" + buyedProperty.PropertyID + ".\nВладелец: " + character.Name);
                        }
                        API.shared.triggerClientEvent(player, "update_money_display", character.Cash);
                        ContextFactory.Instance.SaveChanges();
                    }
                }
                if (trigger == 0)
                {
                    if (character.Id != buyedProperty.CharacterId)
                    {
                        API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~Вы не можете продать данный дом!");
                        return;
                    }
                    else
                    {
                        character.Cash += cost/2;
                        buyedProperty.CharacterId = null;

                        var list = API.getAllBlips();
                        foreach (var blip in list)
                        {
                            var blipPos = API.getBlipPosition(blip);
                            if (blipPos.X == buyedProperty.ExtPosX &&
                                blipPos.Y == buyedProperty.ExtPosY &&
                                blipPos.Z == buyedProperty.ExtPosZ)
                                API.setBlipColor(blip, 2);
                        }
                        var labels = API.getAllLabels();
                        foreach (var label in labels)
                        {
                            var labelText = API.getTextLabelText(label);
                            if (labelText.ToString().Contains("~g~Вход в дом №" + buyedProperty.PropertyID))
                                API.setTextLabelText(label, "~g~Купить дом №" + buyedProperty.PropertyID +".\nСтоимость: " + cost + "$");
                        }
                        API.shared.triggerClientEvent(player, "update_money_display", character.Cash);
                        ContextFactory.Instance.SaveChanges();
                    }
                }
            }
            if (eventName == "ask_user_posXY")
            {
                API.shared.triggerClientEvent(player, "send_user_posXY", (int)player.position.X, (int)player.position.Y);
            }
            if (eventName == "ask_user_sector")
            {
                API.shared.triggerClientEvent(player, "send_user_sector", CharacterController.InWhichSectorOfGhetto(player));
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
                    ClothesManager.SetPlayerSkinClothes(player, 1, characterController.Character, 1);
                }       

                if (callback == 0)
                {
                    character.JobId = 0;
                    ContextFactory.Instance.SaveChanges();
                    API.shared.triggerClientEvent(player, "loader_end");
                    API.shared.triggerClientEvent(player, "update_money_display", character.Cash);
                    ClothesManager.SetPlayerSkinClothes(player, 0, characterController.Character, 1);
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
                if (propertyName == "Army2_stock") stockName = "Army2_stock";
                if (propertyName == "Army1_stock") stockName = "Army1_stock";
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
                        ClothesManager.SetPlayerSkinClothes(player, character.ClothesTypes, character, 1);
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
                    ClothesManager.SetPlayerSkinClothesToDb(player, cloth, character, 1);
                    character.ActiveClothes = cloth;
                    ContextFactory.Instance.SaveChanges();
                }
                // Take metall
                if (trigger == 6)
                {
                    if (character == null) return;
                    character.TempVar = 0;
                    character.Cash += 500;
                    ContextFactory.Instance.SaveChanges();
                    API.shared.triggerClientEvent(player, "update_money_display", character.Cash);
                    API.sendChatMessageToPlayer(player, "~g~Вы успешно сдали металл и получили 500$");
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
            if (eventName == "gang_add_money")
            {
                var money = (int)args[0];
                var gangGroupBank = character.GroupType * 100;
                var gangBank = ContextFactory.Instance.Group.First(x => x.Id == gangGroupBank);
                if (character.Cash - money < 0)
                {
                    API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~У вас недостаточно средств для вклада!");
                    return;
                }
                else
                {
                    character.Cash -= money;
                    gangBank.MoneyBank += money;
                    API.shared.triggerClientEvent(player, "update_money_display", character.Cash);
                }
                ContextFactory.Instance.SaveChanges();
            }
            if (eventName == "gang_get_money")
            {
                var money = (int)args[0];
                var gangGroupBank = character.GroupType * 100;
                var gangBank = ContextFactory.Instance.Group.First(x => x.Id == gangGroupBank);
                if (gangBank.MoneyBank - money < 0)
                {
                    API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~В банке банды недостаточно средств для снятия!");
                    return;
                }
                else
                {
                    character.Cash += money;
                    gangBank.MoneyBank -= money;
                    API.shared.triggerClientEvent(player, "update_money_display", character.Cash);
                }
                ContextFactory.Instance.SaveChanges();
            }
            if (eventName == "gang_rob_house")
            {
                var targetCharacterId = (int)args[0];
                var targetCharacter = ContextFactory.Instance.Character.FirstOrDefault(x => x.Id == targetCharacterId);
                if (targetCharacter == null || targetCharacter.Online == false) return;
                else
                {
                    character.TempVar = 111;
                    ContextFactory.Instance.SaveChanges();
                    API.sendChatMessageToPlayer(player, "~g~Вы успешно украли металл и теперь можете сдать его.");
                }                
            }
            if (eventName == "gang_get_material")
            {
                var material = (int)args[0];
                var gangGroupProperty = character.GroupType * 100;
                var gangStockName = GroupController.GetGroupStockName(character);
                var gangStockProperty = ContextFactory.Instance.Property
                    .First(x => x.Name == gangStockName);
                if (gangStockProperty.Stock - material < 0)
                {
                    API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~В банке банды недостаточно материалов для снятия!");
                    return;
                }
                else
                {
                    character.Material += material;
                    gangStockProperty.Stock -= material;
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

                        Client target = API.shared.getAllPlayers().FirstOrDefault(x => x.socialClubName == targetCharacter.SocialClub);
                        if (target == null) return;
                        if (player.position.DistanceTo(target.position) < 3.0F)
                        {
                            API.sendChatMessageToPlayer(player, "~y~[ПРЕДУПРЕЖДЕНИЕ]: ~w~Вы находитесь далеко от пользователя!");
                            return;
                        }

                        var getGroup = ContextFactory.Instance.Group.FirstOrDefault(x => x.Id == gangID);
                        var groupType = (GroupType)Enum.Parse(typeof(GroupType), getGroup.Type.ToString());
                        var groupExtraType = (GroupExtraType)Enum.Parse(typeof(GroupExtraType), getGroup.ExtraType.ToString());

                        targetCharacter.ActiveGroupID = gangID;
                        targetCharacter.GroupType = (int)groupType;
                        targetCharacter.JobId = 0;
                        targetCharacter.ActiveClothes = ClothesManager.SetFractionClothes(target, gangID, targetCharacter);
                        ContextFactory.Instance.SaveChanges();

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
                            targetCharacter.GroupType = 100;
                            ContextFactory.Instance.SaveChanges();

                            Client target = API.shared.getAllPlayers().FirstOrDefault(x => x.socialClubName == targetCharacter.SocialClub);
                            if (target == null) return;
                            targetCharacter.ActiveClothes = ClothesManager.SetFractionClothes(target, 0, targetCharacter);
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
                // Поменять ранг
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

                        var getGroup = ContextFactory.Instance.Group.FirstOrDefault(x => x.Id == groupID);
                        var groupType = (GroupType)Enum.Parse(typeof(GroupType), getGroup.Type.ToString());
                        var groupExtraType = (GroupExtraType)Enum.Parse(typeof(GroupExtraType), getGroup.ExtraType.ToString());
                        var gangGroupId = (int)groupType * 100 + rangID;

                        if (gangGroupId >= groupID)
                        {
                            API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~Присваемый ранг выше вашего!");
                            return;
                        }
                        else
                        {
                            targetCharacter.ActiveGroupID = gangGroupId;
                            ContextFactory.Instance.SaveChanges();

                            Client target = API.shared.getAllPlayers().FirstOrDefault(x => x.socialClubName == targetCharacter.SocialClub);
                            if (target == null) return;
                            target.sendChatMessage("~g~[СЕРВЕР]: ~w~Игрок " + FormatName + " присвоил вам ранг: " + EntityManager.GetDisplayName(groupExtraType));
                            API.sendChatMessageToPlayer(player, "~g~[УСПЕШНО]: ~w~Вы присвоили пользователю: " + targetCharacter.Name.ToString() + "\nРанг в банде: " + EntityManager.GetDisplayName(groupExtraType));
                        }
                    }
                    catch (Exception e)
                    {
                        API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~Вы ввели неверный пользовательский ID.");
                        return;
                    }
                }
            }            

            // ARMY MENU
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
                        ClothesManager.SetPlayerSkinClothes(player, type, characterController.Character, 1);
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
            if (eventName == "army_add_to_group")
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

                        Client target = API.shared.getAllPlayers().FirstOrDefault(x => x.socialClubName == targetCharacter.SocialClub);
                        if (target == null) return;
                        if (player.position.DistanceTo(target.position) < 3.0F)
                        {
                            API.sendChatMessageToPlayer(player, "~y~[ПРЕДУПРЕЖДЕНИЕ]: ~w~Вы находитесь далеко от пользователя!");
                            return;
                        }

                        var getGroup = ContextFactory.Instance.Group.FirstOrDefault(x => x.Id == armyID);
                        var groupType = (GroupType)Enum.Parse(typeof(GroupType), getGroup.Type.ToString());
                        var groupExtraType = (GroupExtraType)Enum.Parse(typeof(GroupExtraType), getGroup.ExtraType.ToString());

                        targetCharacter.ActiveGroupID = armyID;
                        targetCharacter.GroupType = (int)groupType;
                        targetCharacter.JobId = 0;
                        targetCharacter.ActiveClothes = ClothesManager.SetFractionClothes(target, armyID, targetCharacter);
                        ContextFactory.Instance.SaveChanges();

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
                            Client target = API.shared.getAllPlayers().FirstOrDefault(x => x.socialClubName == targetCharacter.SocialClub);
                            if (target == null) return;
                            targetCharacter.ActiveClothes = ClothesManager.SetFractionClothes(target, 0, targetCharacter);
                            ContextFactory.Instance.SaveChanges();

                            var getGroup = ContextFactory.Instance.Group.FirstOrDefault(x => x.Id == groupID);
                            var groupType = (GroupType)Enum.Parse(typeof(GroupType), getGroup.Type.ToString());
                            var groupExtraType = (GroupExtraType)Enum.Parse(typeof(GroupExtraType), getGroup.ExtraType.ToString());

                            target.sendChatMessage("~g~[СЕРВЕР]: ~w~Игрок " + FormatName + " выгнал вас из фракции: " + EntityManager.GetDisplayName(groupType) + "\nДля пособия по безработице - проследуйте в мэрию.");
                            API.sendChatMessageToPlayer(player, "~g~[УСПЕШНО]: ~w~Вы выгнали пользователя: " + targetCharacter.Name.ToString() + "\nИз фракции: " + EntityManager.GetDisplayName(groupType));

                            targetCharacter.ActiveGroupID = 2;
                            targetCharacter.GroupType = 100;
                            ContextFactory.Instance.SaveChanges();
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

                        if (rangID >= 1 && rangID <= 11 && CharacterController.IsCharacterArmyHighOfficer(character))
                        {
                            targetCharacter.ActiveGroupID = character.GroupType * 100 + rangID;
                            ContextFactory.Instance.SaveChanges();

                            Client target = API.shared.getAllPlayers().FirstOrDefault(x => x.socialClubName == targetCharacter.SocialClub);
                            if (target == null) return;

                            if (CharacterController.IsCharacterArmyInAllOfficers(targetCharacter))
                            {
                                ClothesManager.SetPlayerSkinClothesToDb(target, 101, targetCharacter, 1);
                                ClothesManager.SetPlayerSkinClothes(target, 3, targetCharacter, 1);
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
                                ClothesManager.SetPlayerSkinClothesToDb(target, 101, targetCharacter, 1);
                                ClothesManager.SetPlayerSkinClothes(target, 3, targetCharacter, 1);
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

            if (eventName == "yes_no_menu")
            {
                var type = (string)args[0];
                var targetUserId = (int)args[1]; // OID of buyer
                var initUserId = (int)args[4];   // OID of seller

                if (type == "cloth")
                {
                    var cost = (int)args[3];

                    var targetCharacter = ContextFactory.Instance.Character.FirstOrDefault(x => x.OID == targetUserId);
                    if (targetCharacter == null)
                    {
                        API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~Вы ввели неверный пользовательский ID.");
                        return;
                    }
                    Client target = API.shared.getAllPlayers().FirstOrDefault(x => x.socialClubName == targetCharacter.SocialClub);
                    if (target == null) return;

                    if (player.position.DistanceTo(target.position) > 3.0F)
                    {
                        API.sendChatMessageToPlayer(player, "~y~[ПРЕД]: ~w~Вы находитесь далеко от пользователя!");
                        return;
                    }

                    API.shared.triggerClientEvent(target, "yes_no_menu_client",
                        "Вам предлагают купить военную форму за " + cost + "$",// 0
                        type,                                                       // 1
                        0,                                                          // 2
                        cost,                                                       // 3 
                        initUserId,                                                 // 4  
                        targetUserId);                                              // 5 
                }
                if (type == "weapon")
                {
                    var weapon = (string)args[2];
                    var cost = (int)args[3];

                    var targetCharacter = ContextFactory.Instance.Character.FirstOrDefault(x => x.OID == targetUserId);
                    if (targetCharacter == null)
                    {
                        API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~Вы ввели неверный пользовательский ID.");
                        return;
                    }
                    Client target = API.shared.getAllPlayers().FirstOrDefault(x => x.socialClubName == targetCharacter.SocialClub);
                    if (target == null) return;

                    if (player.position.DistanceTo(target.position) > 3.0F)
                    {
                        API.sendChatMessageToPlayer(player, "~y~[ПРЕД]: ~w~Вы находитесь далеко от пользователя!");
                        return;
                    }

                    API.shared.triggerClientEvent(target, "yes_no_menu_client",
                        "Вам предлагают " + weapon.ToString() + " за " + cost + "$",// 0
                        type,                                                       // 1
                        weapon,                                                     // 2
                        cost,                                                       // 3 
                        initUserId,                                                 // 4  
                        targetUserId);                                              // 5 
                }
                if (type == "gang_sector")
                {

                    var sector = (string)args[2];
                    var gangNum = (int)args[3];         // Gang Number for selling
                    var cost = (int)args[5];
                    var sellerGangType = (int)args[6];  // Seller Gang ID

                    var selectedSector = sector.Split('_');
                    var selectedSectorData = GroupController.GetGangSectorData(
                        Convert.ToInt32(selectedSector[0]), Convert.ToInt32(selectedSector[1]));

                    if (selectedSectorData == sellerGangType * 10 || selectedSectorData != sellerGangType)
                    {
                        API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~Вы не можете продать данный сектор!");
                        return;
                    }

                    var targetGangBoss = gangNum * 100 + 10;
                    var targetCharacter = ContextFactory.Instance.Character.FirstOrDefault(x => x.ActiveGroupID == targetGangBoss);
                    if (targetCharacter == null)
                    {
                        API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~Вы ввели неверный номер банды!");
                        return;
                    }
                    Client target = API.shared.getAllPlayers().FirstOrDefault(x => x.socialClubName == targetCharacter.SocialClub);
                    if (target == null) return;

                    API.shared.triggerClientEvent(target, "yes_no_menu_client",
                            "Вам предлагают сектор " + selectedSector[0].ToString() + " " + selectedSector[1].ToString() + " за " + cost + "$",// 0
                            type,                                                       // 1
                            selectedSector,                                             // 2
                            cost,                                                       // 3 
                            initUserId,                                                 // 4  
                            targetGangBoss);                                            // 5 
                }
            }

            if (eventName == "get_weapon")
            {
                if (character == null) return;
                int callback = (int)args[0];
                int cost = (int)args[1];

                WeaponTint weaponTint = new WeaponTint();
                if (CharacterController.IsCharacterInArmy(character)) weaponTint = WeaponTint.Army;
                if (CharacterController.IsCharacterInFBI(character) ||
                    CharacterController.IsCharacterInPolice(character)) weaponTint = WeaponTint.LSPD;
                if (CharacterController.IsCharacterInGang(character)) weaponTint = WeaponTint.Gold;

                var weaponData = ContextFactory.Instance.Weapon.FirstOrDefault(x => x.CharacterId == character.Id);
                var propertyData = ContextFactory.Instance.Property.FirstOrDefault(x => x.Name == "Army2_stock");
                if (propertyData.Stock - cost < 0)
                {
                    API.sendChatMessageToPlayer(player, "~r~Вы не можете получить данное оружие!\nНедостаточно материалов!");
                }
                else
                {
                    switch (callback)
                    {
                        case 1:
                            propertyData.Stock -= cost;
                            API.givePlayerWeapon(player, WeaponHash.Revolver, 50, true, true);
                            API.setPlayerWeaponTint(player, WeaponHash.Revolver, weaponTint);
                            weaponData.Revolver = 1; weaponData.RevolverPt = 50; break;
                        case 2:
                            propertyData.Stock -= cost;
                            API.givePlayerWeapon(player, WeaponHash.CarbineRifle, 250, true, true);
                            API.setPlayerWeaponTint(player, WeaponHash.CarbineRifle, weaponTint);
                            weaponData.CarbineRifle = 1; weaponData.CarbineRiflePt = 250; break;
                        case 3:
                            propertyData.Stock -= cost;
                            API.givePlayerWeapon(player, WeaponHash.SniperRifle, 50, true, true);
                            API.setPlayerWeaponTint(player, WeaponHash.SniperRifle, weaponTint);
                            weaponData.SniperRifle = 1; weaponData.SniperRiflePt = 50; break;
                        case 4:
                            propertyData.Stock -= cost;
                            API.givePlayerWeapon(player, WeaponHash.SmokeGrenade, 10, true, true);
                            API.setPlayerWeaponTint(player, WeaponHash.SmokeGrenade, weaponTint);
                            weaponData.SmokeGrenade = 1; weaponData.SmokeGrenadePt = 10; break;
                        case 5:
                            propertyData.Stock -= cost;
                            API.givePlayerWeapon(player, WeaponHash.FlareGun, 50, true, true);
                            API.setPlayerWeaponTint(player, WeaponHash.FlareGun, weaponTint);
                            weaponData.FlareGun = 1; weaponData.FlareGunPt = 50; break;
                        case 6:
                            propertyData.Stock -= cost;
                            API.givePlayerWeapon(player, WeaponHash.CompactRifle, 250, true, true);
                            API.setPlayerWeaponTint(player, WeaponHash.CompactRifle, weaponTint);
                            weaponData.CompactRifle = 1; weaponData.CompactRiflePt = 250; break;
                        case 7:
                            propertyData.Stock -= cost;
                            API.givePlayerWeapon(player, WeaponHash.PumpShotgun, 100, true, true);
                            API.setPlayerWeaponTint(player, WeaponHash.PumpShotgun, weaponTint);
                            weaponData.PumpShotgun = 1; weaponData.PumpShotgunPt = 100; break;
                        case 8:
                            propertyData.Stock -= cost;
                            API.givePlayerWeapon(player, WeaponHash.BZGas, 100, true, true);
                            API.setPlayerWeaponTint(player, WeaponHash.BZGas, weaponTint);
                            weaponData.BZGas = 1; weaponData.BZGasPt = 100; break;
                        case 9:
                            propertyData.Stock -= cost;
                            API.givePlayerWeapon(player, WeaponHash.Nightstick, 1, true, true);
                            API.setPlayerWeaponTint(player, WeaponHash.Nightstick, weaponTint);
                            weaponData.Nightstick = 1; break;
                        case 10:
                            propertyData.Stock -= cost;
                            API.givePlayerWeapon(player, WeaponHash.StunGun, 120, true, true);
                            API.setPlayerWeaponTint(player, WeaponHash.StunGun, weaponTint);
                            weaponData.StunGun = 1; weaponData.StunGunPt = 120; break;
                        case 11:
                            propertyData.Stock -= cost;
                            API.givePlayerWeapon(player, WeaponHash.HeavyPistol, 100, true, true);
                            API.setPlayerWeaponTint(player, WeaponHash.HeavyPistol, weaponTint);
                            weaponData.HeavyPistol = 1; weaponData.HeavyPistolPt = 100; break;
                        case 12:
                            propertyData.Stock -= cost;
                            API.givePlayerWeapon(player, WeaponHash.BullpupRifle, 200, true, true);
                            API.setPlayerWeaponTint(player, WeaponHash.BullpupRifle, weaponTint);
                            weaponData.BullpupRifle = 1; weaponData.BullpupRiflePt = 200; break;
                        case 13:
                            propertyData.Stock -= cost;
                            API.givePlayerWeapon(player, WeaponHash.HeavyShotgun, 100, true, true);
                            API.setPlayerWeaponTint(player, WeaponHash.HeavyShotgun, weaponTint);
                            weaponData.HeavyShotgun = 1; weaponData.HeavyShotgunPt = 100; break;
                    }
                }
                ContextFactory.Instance.SaveChanges();
            }
            
            if (eventName == "sell")
            {
                var targetUserId = (int)args[1];
                var cost = (int)args[3];
                var initUserId = (int)args[4];

                if (args[0].ToString() == "gang_sector")
                {
                    if (character == null) return;

                    var sectorRow = ((List<object>)args[2]).ElementAt(0);
                    var sectorCol = ((List<object>)args[2]).ElementAt(1);

                    var initCharacter = ContextFactory.Instance.Character.FirstOrDefault(x => x.OID == initUserId);
                    var targetCharacter = ContextFactory.Instance.Character.FirstOrDefault(x => x.ActiveGroupID == targetUserId);
                    if (targetCharacter == null)
                    {
                        API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~Продавцом был введен неверный пользовательский ID.");
                        return;
                    }
                    Client target = API.shared.getAllPlayers().FirstOrDefault(x => x.socialClubName == targetCharacter.SocialClub);
                    if (target == null) return;

                    if (targetCharacter.Cash - cost < 0)
                    {
                        API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~У вас недостаточно средств для покупки!");
                        return;
                    }
                    else
                    {
                        targetCharacter.Cash -= cost;
                        initCharacter.Cash += cost;
                        GroupController.SetGangSectorData(Convert.ToInt32(sectorRow), Convert.ToInt32(sectorCol), targetCharacter.GroupType);
                        API.shared.triggerClientEvent(target, "update_money_display", targetCharacter.Cash);
                        API.shared.triggerClientEvent(player, "update_money_display", initCharacter.Cash);
                        ContextFactory.Instance.SaveChanges();
                        API.sendChatMessageToPlayer(player, "~g~[СЕРВЕР]: ~w~Вы успешно продали сектор" + sectorRow + " " + sectorCol + " за " + cost + "$");
                        API.sendChatMessageToPlayer(target, "~g~[СЕРВЕР]: ~w~Вы успешно купили " + sectorRow + " " + sectorCol + " за " + cost + "$");
                    }
                }
                if (args[0].ToString() == "weapon")
                {
                    if (character == null) return;                    
                    var weaponName = (string)args[2];

                    var initCharacter = ContextFactory.Instance.Character.FirstOrDefault(x => x.OID == initUserId);
                    var sellPlayerWeapons = ContextFactory.Instance.Weapon.FirstOrDefault(x => x.CharacterId == initCharacter.Id);
                    if (sellPlayerWeapons == null)
                    {
                        API.sendChatMessageToPlayer(player, "~r~У вас нет оружия!");
                        return;
                    }                    
                    var targetCharacter = ContextFactory.Instance.Character.FirstOrDefault(x => x.OID == targetUserId);
                    var buyPlayerWeapons = ContextFactory.Instance.Weapon.FirstOrDefault(x => x.CharacterId == targetCharacter.Id);
                    if (targetCharacter == null || buyPlayerWeapons == null)
                    {
                        API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~Продавцом был введен неверный пользовательский ID.");
                        return;
                    }

                    Client target = API.shared.getAllPlayers().FirstOrDefault(x => x.socialClubName == targetCharacter.SocialClub);
                    if (target == null) return;

                    if (player.position.DistanceTo(target.position) > 3.0F)
                    {
                        API.sendChatMessageToPlayer(player, "~y~[ПРЕДУПРЕЖДЕНИЕ]: ~w~Вы находитесь далеко от продавца!");
                        return;
                    }
                    if (targetCharacter.Cash - cost < 0)
                    {
                        API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~У вас недостаточно средств для покупки!");
                        return;
                    }
                    else
                    {
                        targetCharacter.Cash -= cost;
                        initCharacter.Cash += cost;
                        int ammoAmount = API.getPlayerWeaponAmmo(player, WeaponManager.GetWeaponHash(weaponName));
                        WeaponManager.BuySellWeapon(target, player, sellPlayerWeapons, buyPlayerWeapons, weaponName, ammoAmount, initCharacter, targetCharacter);
                        API.shared.triggerClientEvent(target, "update_money_display", targetCharacter.Cash);
                        API.shared.triggerClientEvent(player, "update_money_display", initCharacter.Cash);
                        ContextFactory.Instance.SaveChanges();
                        API.sendChatMessageToPlayer(player, "~g~[СЕРВЕР]: ~w~Вы успешно продали " + weaponName + " за " + cost + "$");
                        API.sendChatMessageToPlayer(target, "~g~[СЕРВЕР]: ~w~Вы успешно купили " + weaponName + " за " + cost + "$");
                    }
                }
                if (args[0].ToString() == "cloth")
                {
                    if (character == null) return;

                    var initCharacter = ContextFactory.Instance.Character.FirstOrDefault(x => x.OID == initUserId);
                    
                    if (initCharacter.ClothesTypes <= 1 && initCharacter.ClothesTypes > 4)
                    {
                        API.sendChatMessageToPlayer(player, "~r~У вас нет формы!");
                        return;
                    }
                    var targetCharacter = ContextFactory.Instance.Character.FirstOrDefault(x => x.OID == targetUserId);
                    if (targetCharacter == null)
                    {
                        API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~Продавцом был введен неверный пользовательский ID.");
                        return;
                    }

                    Client target = API.shared.getAllPlayers().FirstOrDefault(x => x.socialClubName == targetCharacter.SocialClub);
                    if (target == null) return;

                    if (player.position.DistanceTo(target.position) > 3.0F)
                    {
                        API.sendChatMessageToPlayer(player, "~y~[ПРЕДУПРЕЖДЕНИЕ]: ~w~Вы находитесь далеко от продавца!");
                        return;
                    }
                    if (targetCharacter.Cash - cost < 0)
                    {
                        API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~У вас недостаточно средств для покупки!");
                        return;
                    }
                    else
                    {
                        targetCharacter.Cash -= cost;
                        initCharacter.Cash += cost;
                        targetCharacter.ClothesTypes = initCharacter.ClothesTypes;
                        initCharacter.ClothesTypes = 0;
                        ClothesManager.SetPlayerSkinClothesToDb(player, 100, initCharacter, 1);
                        API.shared.triggerClientEvent(target, "update_money_display", targetCharacter.Cash);
                        API.shared.triggerClientEvent(player, "update_money_display", initCharacter.Cash);
                        ContextFactory.Instance.SaveChanges();
                        API.sendChatMessageToPlayer(player, "~g~[СЕРВЕР]: ~w~Вы успешно продали свою военную форму!");
                        API.sendChatMessageToPlayer(target, "~g~[СЕРВЕР]: ~w~Вы успешно купили себе военную форму!");
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

                    Client target = API.shared.getAllPlayers().FirstOrDefault(x => x.socialClubName == targetCharacter.SocialClub);
                    if (target == null) return;
                    if (player.position.DistanceTo(target.position) < 3.0F)
                    {
                        API.sendChatMessageToPlayer(player, "~y~[ПРЕДУПРЕЖДЕНИЕ]: ~w~Вы находитесь далеко от пользователя!");
                        return;
                    }
                    targetCharacter.DriverLicense = 1;
                    ContextFactory.Instance.SaveChanges();

                    target.sendChatMessage("~g~[СЕРВЕР]: ~w~Игрок " + FormatName + " выдал вам водительские права \nкатегории " + targetCharacter.DriverLicense.ToString());
                    API.sendChatMessageToPlayer(player, "~g~[УСПЕШНО]: ~w~Вы выдали права категории" + targetCharacter.DriverLicense.ToString() + "\nпользователю с ID: " + userID);
                }
                catch (Exception e)
                {
                    API.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~Вы ввели неверный пользовательский ID.");
                    return;
                }
            }

            if (eventName == "send_chat_message")
            {
                var message = (string)args[0];
                ChatController.SendMessageInGroup(player, message);
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

                    Client target = API.shared.getAllPlayers().FirstOrDefault(x => x.socialClubName == targetCharacter.SocialClub);
                    if (target == null) return;

                    var getGroup = ContextFactory.Instance.Group.FirstOrDefault(x => x.Id == groupID);
                    var groupType = (GroupType)Enum.Parse(typeof(GroupType), getGroup.Type.ToString());
                    var groupExtraType = (GroupExtraType)Enum.Parse(typeof(GroupExtraType), getGroup.ExtraType.ToString());

                    targetCharacter.ActiveGroupID = groupID;
                    targetCharacter.GroupType = (int)groupType;
                    ContextFactory.Instance.SaveChanges();

                    if (CharacterController.IsCharacterArmyGeneral(targetCharacter))
                    {
                        ClothesManager.SetPlayerSkinClothesToDb(target, 4, targetCharacter, 1);
                        targetCharacter.ActiveClothes = 4;
                    }
                    if (CharacterController.IsCharacterGangBoss(targetCharacter))
                    {
                        ClothesManager.SetPlayerSkinClothesToDb(target, 131, targetCharacter, 1);
                        targetCharacter.ActiveClothes = 131;
                    }
                    if (targetCharacter.ActiveGroupID == 101 || targetCharacter.ActiveGroupID == 101)
                    {
                        ClothesManager.SetPlayerSkinClothesToDb(target, 10, targetCharacter, 1);
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
