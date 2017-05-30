using System;
using System.Linq;
using GTANetworkServer;
using GTANetworkShared;
using TheGodfatherGM.Data;
using TheGodfatherGM.Data.Models;
using System.Collections.Generic;
using TheGodfatherGM.Server.DBManager;
using TheGodfatherGM.Server.Characters;

namespace TheGodfatherGM.Server.Vehicles
{
    public class VehicleController : Script
    {
        public Data.Vehicle VehicleData;
        public GTANetworkServer.Vehicle Vehicle;
        public Groups.GroupController Group;

        private DateTime _oneSecond;
        private DateTime _oneMinute;
        private DateTime _hourAnnounce;
        private double _vehRpm;
        private double _currentFuel;

        public VehicleController()
        {
            API.onUpdate += VehicleFuelEvent;
            API.onUpdate += RentVehicleEvent;
            API.onUpdate += RespawnStaticVehicle;
            API.onVehicleDeath += OnVehicleExplode;
            API.onPlayerExitVehicle += OnPlayerExitVehicle;
            API.onPlayerEnterVehicle += OnPlayerEnterVehicle;
            API.onClientEventTrigger += OnClientEventTrigger;            
        }

        public VehicleController(Data.Vehicle vehicleData, GTANetworkServer.Vehicle vehicle)
        {
            VehicleData = vehicleData;
            Vehicle = vehicle;
            API.setVehicleEngineStatus(vehicle, false); // Engine is always off.

            if (vehicleData.JobId == JobsIdNonDataBase.BusDriver || vehicleData.Type == 1)
            {
                API.setVehicleLocked(vehicle, false);       // Driver door is opened for Buses.
            }
            else
            {                
                API.setVehicleDoorState(vehicle, 0, false); // Driver door is always closed.
                API.setVehicleLocked(vehicle, true);        // Driver door is always locked.
            }
            EntityManager.Add(this);
        }

        private void OnPlayerExitVehicle(Client player, NetHandle vehicle)
        {
            CharacterController characterController = player.getData("CHARACTER");
            if (characterController == null) return;

            var vehicleController = EntityManager.GetVehicle(vehicle);
            if (vehicleController != null)
            {
            }
            API.triggerClientEvent(player, "hide_vehicle_hud");
        }
        private void OnPlayerEnterVehicle(Client player, NetHandle vehicle)
        {
            CharacterController characterController = player.getData("CHARACTER");
            if (characterController == null) return;

            var vehicleController = EntityManager.GetVehicle(vehicle);
            if (vehicleController != null)
            {
                _currentFuel = vehicleController.VehicleData.Fuel;
                var characterGroup = ContextFactory.Instance.Group.FirstOrDefault(x => x.Id == characterController.Character.ActiveGroupID);
                if (characterGroup == null) return;

                Group vehicleGroup = null;
                if (vehicleController.VehicleData.GroupId != null)
                    vehicleGroup = ContextFactory.Instance.Group.FirstOrDefault(x => x.Id == vehicleController.VehicleData.GroupId);

                if (vehicleController.VehicleData.Model == RentModels.ScooterModel ||
                    vehicleController.VehicleData.Model == RentModels.TaxiModel)
                    API.sendNotificationToPlayer(player, "Вы сели в прокатный транспорт");

                else if(vehicleController.VehicleData.Character == characterController.Character)
                    API.sendNotificationToPlayer(player, "Вы сели в свой транспорт");

                else if (vehicleGroup != null)
                {
                    if (vehicleGroup.Type == characterGroup.Type)
                        API.sendNotificationToPlayer(player, "Вы сели в транспорт вашей организации");
                }                       

                else if (vehicleController.VehicleData.JobId == characterController.Character.JobId)
                    API.sendNotificationToPlayer(player, "Вы сели в транспорт вашей рабочей профессии");

                else
                {
                    if (vehicleController.VehicleData.Character == null)
                        API.sendNotificationToPlayer(player, "Данный транспорт не принадлежит никому");
                    else API.sendNotificationToPlayer(player, "Вы сели в чужой транспорт");                    
                }
            }
            API.triggerClientEvent(player, "show_vehicle_hud"); // TODO: use this for vehicle HUD
        }

        public void OnVehicleExplode(NetHandle vehicle)
        {
            /*
            * VehicleData VM = EntityManager.GetVehicle(vehicle);
            if (VM.respawnable)
            {
                API.sendChatMessageToAll("Vehicle respawning...");
                API.delay(10000, true, () => // 10 seconds between vehicle explosion and respawning
                {
                    API.sendChatMessageToAll("Vehicle respawned!...");
                    API.deleteEntity(vehicle);
                    VehicleData VM2 = new VehicleData(API.createVehicle((VehicleHash)VM.ModelHash, VM.vehPos, new Vector3(0, 0, VM.vehRotZ), VM.color1, VM.color1));
                    VM2.id = VM.id;
                });
            }
            */
        }

        public static void LoadVehicles()
        {
            foreach (var vehicle in ContextFactory.Instance.Vehicle.Where(x => x.Respawnable).ToList())
            {
                var vehicleController = new VehicleController(vehicle, API.shared.createVehicle((VehicleHash)vehicle.Model, new Vector3(vehicle.PosX, vehicle.PosY, vehicle.PosZ), new Vector3(0.0f, 0.0f, vehicle.Rot), vehicle.Color1, vehicle.Color2));
                if (vehicle.Group != null) // -1 is reserved for non-group job vehicles.
                {
                    vehicleController.Group = EntityManager.GetGroup(vehicle.Group.Id);
                }                
            }
            API.shared.consoleOutput("[GM] Загружено транспорта: " + ContextFactory.Instance.Vehicle.Count() +" ед.");
        }
        public static List<Data.Vehicle> GetVehicles(CharacterController account)
        {
            return ContextFactory.Instance.Vehicle.Where(x => x.Character == account.Character).ToList();
        }
        public static void LoadVehicle(Client player, int id)
        {
            var vm = ContextFactory.Instance.Vehicle.FirstOrDefault(x => x.Id == id);
            if (vm == null) return;
            var vehicle = new VehicleController(vm, API.shared.createVehicle((VehicleHash)vm.Model, new Vector3(vm.PosX, vm.PosY, vm.PosZ), new Vector3(0.0f, 0.0f, vm.Rot), vm.Color1, vm.Color2));
            API.shared.sendNotificationToPlayer(player, "You spawned your " + API.shared.getVehicleDisplayName((VehicleHash)vm.Model));
        }
        public static void UnloadVehicles(Character character)
        {
            var vehicles = EntityManager.GetVehicleControllers(character);
            foreach (var vehicle in vehicles)
            {
                if (vehicle != null)
                    vehicle.UnloadVehicle(character);
            }
        }        
        public static void TriggerDoor(GTANetworkServer.Vehicle vehicle, int doorId)
        {
            if (vehicle.isDoorOpen(doorId)) vehicle.closeDoor(doorId);
            else vehicle.openDoor(doorId);
        }
        
        public void UnloadVehicle(Character character)
        {
            //API.sendNotificationToPlayer(account.Client, "You stored your " + API.getVehicleDisplayName((VehicleHash)Vehicle.model));
            EntityManager.Remove(this);
            Vehicle.delete();
        }

        private static void RentVehicle(int vehicleModel)
        {            
            foreach (var vehicle in ContextFactory.Instance.Vehicle.Where(x => x.Model == vehicleModel).ToList())
            {
                if (vehicle.RentTime != 0)
                {                   
                    if (vehicle.Model == RentModels.TaxiModel || 
                        vehicle.Model == RentModels.ScooterModel)
                    {
                        if (vehicle.Character != null)
                        {
                            vehicle.RentTime = vehicle.RentTime - 1;
                            ContextFactory.Instance.SaveChanges();
                        }
                    }                    
                }
                else
                {
                    var player = API.shared.getAllPlayers().FirstOrDefault(x => x.socialClubName == vehicle.Character.SocialClub);
                    if (player == null) return;
                    var vehicleController = EntityManager.GetVehicle(vehicle);
                                        
                    if (player.isInVehicle == false)
                    {
                        if (vehicle.Model == RentModels.ScooterModel) ContextFactory.Instance.Vehicle.Remove(vehicle);
                        if (vehicle.Model == RentModels.TaxiModel) RespawnWorkVehicle(vehicle, RentModels.TaxiModel, 126, 126);
                        ContextFactory.Instance.SaveChanges();
                    }
                    if (player.isInVehicle)
                    {
                        vehicleController.Vehicle.engineStatus = false;
                        string banner;
                        string text;
                        switch (vehicle.Model)
                        {
                            case RentModels.ScooterModel:
                                banner = "Время проката мопеда вышло";
                                text = "Продлите ваш мопед на полчаса всего за 30$"; break;
                            case RentModels.TaxiModel:
                                banner = "Время проката такси вышло";
                                text = "Продлите ваше такси на час всего за 100$"; break;
                            default:
                                banner = "Время проката вышло";
                                text = "Продлите прокат или начнит сначала."; break;
                        }

                        API.shared.triggerClientEvent(player, "rent_finish_menu",
                            1, //0
                            banner,
                            text, vehicle.Model);
                    }
                }
            }
        }

        public static void RespawnWorkVehicle (Data.Vehicle vehicle, int vehicleModel, int vehicleCol1, int vehicleCol2)
        {
            var vehiclePosX = vehicle.PosX;
            var vehiclePosY = vehicle.PosY;
            var vehiclePosZ = vehicle.PosZ;
            var vehicleRotZ = vehicle.Rot;

            var vehicleController = EntityManager.GetVehicle(vehicle);
            vehicleController.Vehicle.delete();

            VehicleController newVehicle = new VehicleController(vehicle, 
                API.shared.createVehicle((VehicleHash)vehicle.Model, 
                new Vector3(vehiclePosX, vehiclePosY, vehiclePosZ), 
                new Vector3(0.0f, 0.0f, vehicleRotZ), vehicleCol1, vehicleCol2));

            if (vehicle.Model == RentModels.TaxiModel)
            {
                vehicle.Character = null;
                vehicle.RentTime = 0;
            }
            ContextFactory.Instance.SaveChanges();
        }
        private static void RespawnStaticJobVehicle(int jobId)
        {
            var allStaticVehicles = ContextFactory.Instance.Vehicle.Where(x => x.JobId == jobId);

            foreach (var vehicle in allStaticVehicles)
            {
                var vehicleController = EntityManager.GetVehicle(vehicle);
                var vehiclePostition = new Vector3(vehicle.PosX, vehicle.PosY, vehicle.PosZ);

                if (vehicleController.Vehicle.occupants.Length == 0 && 
                    vehicleController.Vehicle.position != vehiclePostition)
                {
                    vehicleController.Vehicle.delete();

                    vehicle.Fuel = FuelByType.GetFuel(vehicle.Model);
                    var newVehicle = new VehicleController(vehicle,
                            API.shared.createVehicle((VehicleHash)vehicle.Model,
                            new Vector3(vehicle.PosX, vehicle.PosY, vehicle.PosZ),
                            new Vector3(0.0f, 0.0f, vehicle.Rot), vehicle.Color1, vehicle.Color2));
                }
            }
            ContextFactory.Instance.SaveChanges();
        }
        private void RespawnStaticGroupVehicle(int groupId)
        {
            var allStaticVehicles = ContextFactory.Instance.Vehicle.Where(x => x.GroupId == groupId);

            foreach (var vehicle in allStaticVehicles)
            {
                var vehicleController = EntityManager.GetVehicle(vehicle);
                var vehiclePostition = new Vector3(vehicle.PosX, vehicle.PosY, vehicle.PosZ);

                if (vehicleController.Vehicle.occupants.Length == 0 &&
                    vehicleController.Vehicle.position != vehiclePostition)
                {
                    vehicleController.Vehicle.delete();

                    vehicle.Fuel = FuelByType.GetFuel(vehicle.Model);
                    var newVehicle = new VehicleController(vehicle,
                            API.shared.createVehicle((VehicleHash)vehicle.Model,
                            new Vector3(vehicle.PosX, vehicle.PosY, vehicle.PosZ),
                            new Vector3(0.0f, 0.0f, vehicle.Rot), vehicle.Color1, vehicle.Color2));
                }
            }
            ContextFactory.Instance.SaveChanges();
        }        

        public void ParkVehicle(Client player)
        {
            var vehicleController = EntityManager.GetVehicle(player.vehicle);
            if (vehicleController == null) return;
            CharacterController characterController = player.getData("CHARACTER");
            if (characterController == null) return;
            
            // Rent vehicles (scooter, taxi)
            if (vehicleController.VehicleData.Model == RentModels.ScooterModel /*||
                /*vehicleController.VehicleData.Model == Data.Models.RentModels.TaxiModel*/)      // Taxi
            {
                API.sendNotificationToPlayer(player, "~r~Вы не можете парковать данный транспорт!");
                return;
            }                

            if (vehicleController.VehicleData.CharacterId == characterController.Character.Id)
            {
                if (player.velocity != new Vector3(0.0f, 0.0f, 0.0f))
                {
                    API.sendNotificationToPlayer(player, "Вы не должны двигаться пока транспорт паркуется");
                    return;
                }

                var firstPos = player.vehicle.position;
                API.sendNotificationToPlayer(player, "Не двигайтесь пока ваш транспорт паркуется.");
                Global.Util.delay(2500, () =>
                {
                    if (player.vehicle != null)
                    {
                        if (firstPos.DistanceTo(player.vehicle.position) <= 5.0f)
                        {

                            var newPos = player.vehicle.position;// + new Vector3(0.0f, 0.0f, 0.5f);
                            var rot = player.vehicle.rotation.Z;
                            vehicleController.VehicleData.PosX = newPos.X;
                            vehicleController.VehicleData.PosY = newPos.Y;
                            vehicleController.VehicleData.PosZ = newPos.Z;
                            vehicleController.VehicleData.Rot = rot;

                            ContextFactory.Instance.SaveChanges();

                            API.sendNotificationToPlayer(player, "~g~[СЕРВЕР]: ~w~Ваш траспорт припаркован!");
                            API.sendNotificationToPlayer(player, "~y~[СЕРВЕР]: ~w~ Х= " + newPos.X + " Y= " + newPos.Y);
                        }
                        else API.sendNotificationToPlayer(player, "~y~Вы двигали транспорт пока парковались.");
                    }
                });
            }
            else API.sendNotificationToPlayer(player, "~r~Вы не можете парковать данный транспорт!");
        }
        public bool CheckAccess(CharacterController characterController)
        {
            var characterGroup = ContextFactory.Instance.Group.FirstOrDefault(x => x.Id == characterController.Character.ActiveGroupID);
            var vehicleGroup = ContextFactory.Instance.Group.FirstOrDefault(x => x.Id == VehicleData.GroupId);
            ContextFactory.Instance.SaveChanges();

            // Check for gangs for stealing
            if (VehicleData.GroupId == 2000 || VehicleData.GroupId == 2100)
                switch (characterController.Character.ActiveClothes)
                {
                    case 2: return true;
                    case 3: return true;
                    case 4: return true;
                    default: return false;
                }
            // Check for busDrivers:
            if (VehicleData.JobId == 888)
            {
                switch (characterController.Character.JobId)
                {
                    case JobsIdNonDataBase.BusDriver1: return true;
                    case JobsIdNonDataBase.BusDriver2: return true;
                    case JobsIdNonDataBase.BusDriver3: return true;
                    case JobsIdNonDataBase.BusDriver4: return true;
                    default: return false;
                }                
            }

            if (VehicleData.Model == RentModels.ScooterModel) return true;

            if (VehicleData.Character != null && 
                VehicleData.Character == characterController.Character) return true;

            if (VehicleData.JobId != null && 
                VehicleData.JobId == characterController.Character.JobId) return true;

            if (vehicleGroup != null && characterGroup != null && 
                vehicleGroup.Type == characterGroup.Type) return true;
            
            return false;
        }

        private void VehicleFuelEvent()
        {
            if (DateTime.Now.Subtract(_oneSecond).TotalMilliseconds >= 500)
            {
                try
                {
                    var allVehicles = API.getAllVehicles();
                    foreach (var vehicle in allVehicles)
                    {
                        if (API.getVehicleEngineStatus(vehicle))
                        {
                            var vehicleController = EntityManager.GetVehicle(vehicle);
                            
                            if (vehicleController != null)
                            {
                                _currentFuel = vehicleController.VehicleData.Fuel;
                                vehicleController.VehicleData.Fuel -= _vehRpm * FuelByType.GetConsumption(vehicleController.VehicleData.Model);

                                if (_currentFuel - vehicleController.VehicleData.Fuel > 0.2)
                                {
                                    _currentFuel = vehicleController.VehicleData.Fuel;
                                    ContextFactory.Instance.SaveChanges();
                                }
                                if (_currentFuel < 0)
                                {
                                    _currentFuel = 0.0;
                                    vehicleController.VehicleData.Fuel = 0.0;
                                    ContextFactory.Instance.SaveChanges();
                                    vehicleController.Vehicle.engineStatus = false;
                                    return;
                                }                                
                            }                                
                        }
                    }
                }
                catch (Exception)
                {
                    // ignored
                }

                _oneSecond = DateTime.Now;
            }            
        }
        private void RentVehicleEvent()
        {
            if (DateTime.Now.Subtract(_oneMinute).TotalMinutes >= 1)
            {
                // Прокат транспорта (каждую минуту вычитается 1 ед. RentTime):
                try
                {
                    RentVehicle(RentModels.ScooterModel);
                    RentVehicle(RentModels.TaxiModel);
                }
                catch (Exception)
                {
                    // ignored
                }

                _oneMinute = DateTime.Now;
            }
            
        }
        private void RespawnStaticVehicle()
        {
            if (DateTime.Now.Subtract(_hourAnnounce).TotalMinutes >= 60)
            {
                try
                {
                    RespawnStaticJobVehicle(JobsIdNonDataBase.BusDriver);

                    
                    foreach (var group in GroupsConst.GroupsIndexes)
                    {
                        RespawnStaticGroupVehicle(group);
                    }
                    API.shared.sendChatMessageToAll("~g~[СЕРВЕР]: ~s~Ежечаcный респавн статичного транспорта.");
                }
                catch (Exception)
                {
                    // ignored
                }

                _hourAnnounce = DateTime.Now;
            }
        }
        private void OnClientEventTrigger(Client player, string eventName, object[] args)
        {
            /*
            CharacterController characterController = player.getData("CHARACTER");
            if (characterController == null) return;
            Character character = characterController.Character;
            var FormatName = character.Name.Replace("_", " ");
            */
            if (eventName == "ask_fuel_in_car")
                if (player.isInVehicle) API.triggerClientEvent(player, "update_fuel_display", _currentFuel);

            if (eventName == "fuel_consumption") _vehRpm = Convert.ToDouble(args[0]); 
        }
    }
}
