using GTANetworkServer;
using System.Collections.Generic;
using System.Linq;
using GTANetworkShared;
using TheGodfatherGM.Server.DBManager;
using TheGodfatherGM.Server.Characters;
using TheGodfatherGM.Data;

namespace TheGodfatherGM.Server.Vehicles
{
    public class VehicleController : Script
    {
        public Data.Vehicle VehicleData;
        public GTANetworkServer.Vehicle Vehicle;
        public Groups.GroupController Group;

        public VehicleController()
        {
            API.onVehicleDeath += OnVehicleExplode;
            API.onPlayerEnterVehicle += OnPlayerEnterVehicle;
            API.onPlayerExitVehicle += OnPlayerExitVehicle;
        }

        public VehicleController(Data.Vehicle VehicleData, GTANetworkServer.Vehicle vehicle)
        {
            this.VehicleData = VehicleData;
            Vehicle = vehicle;
            API.setVehicleEngineStatus(vehicle, false); // Engine is always off.
            API.setVehicleDoorState(vehicle, 0, false); // Driver door is always closed.
            EntityManager.Add(this);
        }

        private void OnPlayerExitVehicle(Client player, NetHandle vehicle)
        {
            CharacterController characterController = player.getData("CHARACTER");
            if (characterController == null) return;

            VehicleController vehicleController = EntityManager.GetVehicle(vehicle);
            if (vehicleController != null)
            {
                /*
                if (vehicleController.VehicleData.JobId == characterController.Character.JobId)
                {
                    vehicleController.VehicleData.Character = null;
                    ContextFactory.Instance.SaveChanges();
                }
                */
            }
            API.triggerClientEvent(player, "hide_vehicle_hud");
        }
        private void OnPlayerEnterVehicle(Client player, NetHandle vehicle)
        {
            CharacterController characterController = player.getData("CHARACTER");
            if (characterController == null) return;

            VehicleController vehicleController = EntityManager.GetVehicle(vehicle);
            if (vehicleController != null)
            {
                var CharacterGroup = ContextFactory.Instance.Group.FirstOrDefault(x => x.Id == characterController.Character.ActiveGroupID);
                Group VehicleGroup = null;
                if (vehicleController.VehicleData.GroupId != null)
                    VehicleGroup = ContextFactory.Instance.Group.FirstOrDefault(x => x.Id == vehicleController.VehicleData.GroupId);

                if (vehicleController.VehicleData.Character == characterController.Character)
                    API.sendNotificationToPlayer(player, "Вы сели в свой транспорт");

                else if (VehicleGroup.Type == CharacterGroup.Type)
                    API.sendNotificationToPlayer(player, "Вы сели в транспорт вашей организации");

                else if (vehicleController.VehicleData.JobId == characterController.Character.JobId)
                    API.sendNotificationToPlayer(player, "Вы сели в транспорт вашей рабочей профессии");

                else
                {
                    API.sendNotificationToPlayer(player, "Вы сели в чужой транспорт");
                    if (VehicleGroup == null && vehicleController.VehicleData.Character == null)
                        API.sendNotificationToPlayer(player, "Данный транспорт не принадлежит никому");
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
            foreach (var vehicle in ContextFactory.Instance.Vehicle.Where(x => x.Respawnable == true).ToList())
            {
                VehicleController VehicleController = new VehicleController(vehicle, API.shared.createVehicle((VehicleHash)vehicle.Model, new Vector3(vehicle.PosX, vehicle.PosY, vehicle.PosZ), new Vector3(0.0f, 0.0f, vehicle.Rot), vehicle.Color1, vehicle.Color2));
                if (vehicle.Group != null) // -1 is reserved for non-group job vehicles.
                {
                    VehicleController.Group = EntityManager.GetGroup(vehicle.Group.Id);
                }
                API.shared.setVehicleLocked(VehicleController.Vehicle, true);
            }
            API.shared.consoleOutput("[GM] Загружено транспорта: " + ContextFactory.Instance.Vehicle.Count() +" ед.");
        }
        public static List<Data.Vehicle> GetVehicles(CharacterController account)
        {
            return ContextFactory.Instance.Vehicle.Where(x => x.Character == account.Character).ToList();
        }
        public static void LoadVehicle(Client player, int id)
        {
            Data.Vehicle VM = ContextFactory.Instance.Vehicle.Where(x => x.Id == id).FirstOrDefault();
            if (VM != null)
            {
                VehicleController _vehicle = new VehicleController(VM, API.shared.createVehicle((VehicleHash)VM.Model, new Vector3(VM.PosX, VM.PosY, VM.PosZ), new Vector3(0.0f, 0.0f, VM.Rot), VM.Color1, VM.Color2));
                API.shared.sendNotificationToPlayer(player, "You spawned your " + API.shared.getVehicleDisplayName((VehicleHash)VM.Model));
            }
        }
        public static void UnloadVehicles(Character character)
        {
            List<VehicleController> Vehicles = EntityManager.GetVehicleControllers(character);
            foreach (var vehicle in Vehicles)
            {
                if (vehicle != null)
                {
                    vehicle.UnloadVehicle(character);
                }
            }
        }        
        public static void TriggerDoor(GTANetworkServer.Vehicle vehicle, int DoorID)
        {
            if (vehicle.isDoorOpen(DoorID)) vehicle.closeDoor(DoorID);
            else vehicle.openDoor(DoorID);
        }

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
        public void UnloadVehicle(Character character)
        {
            //API.sendNotificationToPlayer(account.Client, "You stored your " + API.getVehicleDisplayName((VehicleHash)Vehicle.model));
            EntityManager.Remove(this);
            Vehicle.delete();
        }

        public static void RentVehicle(int vehicleModel)
        {            
            foreach (var vehicle in ContextFactory.Instance.Vehicle.Where(x => x.Model == vehicleModel).ToList())
            {
                if (vehicle.RentTime != 0)
                {                   
                    if (vehicle.Model == Data.Models.RentModels.TaxiModel || 
                        vehicle.Model == Data.Models.RentModels.ScooterModel)
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
                    var player = API.shared.getPlayerFromName(vehicle.Character.Name);
                    VehicleController VehicleController = EntityManager.GetVehicle(vehicle);
                                        
                    if (player == null || player.isInVehicle == false)
                    {
                        if (vehicle.Model == Data.Models.RentModels.ScooterModel) ContextFactory.Instance.Vehicle.Remove(vehicle);
                        if (vehicle.Model == Data.Models.RentModels.TaxiModel) RespawnWorkVehicle(vehicle, Data.Models.RentModels.TaxiModel, 126, 126);
                        ContextFactory.Instance.SaveChanges();
                    }
                    if (player.isInVehicle)
                    {
                        VehicleController.Vehicle.engineStatus = false;
                        string banner = "";
                        string text = "";

                        if (vehicle.Model == Data.Models.RentModels.ScooterModel)
                        {
                            banner = "Время проката мопеда вышло";
                            text = "Продлите ваш мопед на полчаса всего за 30$";
                        }  // Scooter
                        if (vehicle.Model == Data.Models.RentModels.TaxiModel)
                        {
                            banner = "Время проката такси вышло";
                            text = "Продлите ваше такси на час всего за 100$";
                        }   // Taxi
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

            VehicleController vehicleController = EntityManager.GetVehicle(vehicle);
            vehicleController.Vehicle.delete();

            VehicleController newVehicle = new VehicleController(vehicle, 
                API.shared.createVehicle((VehicleHash)vehicle.Model, 
                new Vector3(vehiclePosX, vehiclePosY, vehiclePosZ), 
                new Vector3(0.0f, 0.0f, vehicleRotZ), vehicleCol1, vehicleCol2));

            if (vehicle.Model == Data.Models.RentModels.TaxiModel)
            {
                vehicle.Character = null;
                vehicle.RentTime = 0;
            }
            ContextFactory.Instance.SaveChanges();
        }

        public void ParkVehicle(Client player)
        {
            VehicleController vehicleController = EntityManager.GetVehicle(player.vehicle);
            if (vehicleController == null) return;
            CharacterController characterController = player.getData("CHARACTER");
            if (characterController == null) return;

            var CharacterGroup = ContextFactory.Instance.Group.FirstOrDefault(x => x.Id == characterController.Character.ActiveGroupID);
            var VehicleGroup = ContextFactory.Instance.Group.FirstOrDefault(x => x.Id == VehicleData.GroupId);

            // Rent vehicles (scooter, taxi)
            if (vehicleController.VehicleData.Model == Data.Models.RentModels.ScooterModel ||
                vehicleController.VehicleData.Model == Data.Models.RentModels.TaxiModel)      // Taxi
            {
                API.sendNotificationToPlayer(player, "~r~Вы не можете парковать данный транспорт!");
                return;
            }                

            if (VehicleGroup.Type == CharacterGroup.Type ||
                vehicleController.VehicleData.Character == characterController.Character)
            {
                if (player.velocity != new Vector3(0.0f, 0.0f, 0.0f))
                {
                    API.sendNotificationToPlayer(player, "Вы не должны двигаться пока транспорт паркуется");
                    return;
                }

                Vector3 firstPos = player.vehicle.position;
                API.sendNotificationToPlayer(player, "Не двигайтесь пока ваш транспорт паркуется.");
                Global.Util.delay(2500, () =>
                {
                    if (player.vehicle != null)
                    {
                        if (firstPos.DistanceTo(player.vehicle.position) <= 5.0f)
                        {

                            Vector3 newPos = player.vehicle.position;// + new Vector3(0.0f, 0.0f, 0.5f);
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
            var CharacterGroup = ContextFactory.Instance.Group.FirstOrDefault(x => x.Id == characterController.Character.ActiveGroupID);
            var VehicleGroup = ContextFactory.Instance.Group.FirstOrDefault(x => x.Id == VehicleData.GroupId);
            ContextFactory.Instance.SaveChanges();

            // Check for gangs for stealing
            if (VehicleData.GroupId == 2000 || VehicleData.GroupId == 2100)            
                switch (characterController.Character.ActiveClothes)
                {
                   case 2: return true;
                   case 3: return true;
                   case 4: return true;
                }           

            if (VehicleData.Model == Data.Models.RentModels.ScooterModel) return true;

            if (VehicleData.Character != null && 
                VehicleData.Character == characterController.Character) return true;

            if (VehicleData.JobId != null && 
                VehicleData.JobId == characterController.Character.JobId) return true;

            if (VehicleGroup != null && CharacterGroup != null && 
                VehicleGroup.Type == CharacterGroup.Type) return true;
            
            return false;
        }
    }
}
