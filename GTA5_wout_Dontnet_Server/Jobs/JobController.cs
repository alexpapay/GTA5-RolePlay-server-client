using System;
using System.Linq;
using System.Collections.Generic;

using GTANetworkServer;
using GTANetworkShared;

using TheGodfatherGM.Data;
using TheGodfatherGM.Data.Enums;
using TheGodfatherGM.Data.Models;
using TheGodfatherGM.Data.Extensions;
using TheGodfatherGM.Data.Attributes;
using TheGodfatherGM.Server.DBManager;
using TheGodfatherGM.Server.Characters;

namespace TheGodfatherGM.Server.Jobs
{
    public class JobController : Script
    {
        // Грузчик :: метки и маркеры
        public Vector3 job1Marker1 = new Vector3(895.07, -2968.57, 5.9);
        public Vector3 job1Marker2 = new Vector3(936.667, -2907.894, 5.9);
        ColShape job1_1MarCol;
        ColShape job1_2MarCol;

        public Vector3 job2Marker1 = new Vector3(-155.5, -959.14, 269.2);
        public Vector3 job2Marker2 = new Vector3(-179.88, -1008.7, 254.1316);
        ColShape job2_1MarCol;
        ColShape job2_2MarCol;

        

        public static int currentJobId;

        // Таксист :: переменные
        double senderxcoords, senderycoords;
        Client sen;

        public Job JobData;
        public Groups.GroupController Group;

        Blip blip;
        Marker marker;
        ColShape colShape;
        TextLabel textLabel;

        public JobController() { }
        public JobController(Job jobData)
        {
            JobData = jobData;
            EntityManager.Add(this);
        }
        public static void LoadJobs()
        {
            foreach (var job in ContextFactory.Instance.Job)
            {
                JobController jobController = new JobController(job);
                jobController.CreateWorldEntity();
            }
            API.shared.consoleOutput("[GM] Загружено работ : " + ContextFactory.Instance.Job.Count());
        }
        // Add new job below:
        public void CreateWorldEntity()
        {
            blip = API.createBlip(new Vector3(JobData.PosX, JobData.PosY, JobData.PosZ), 0, 0);
            API.setBlipSprite(blip, JobData.Type.GetAttributeOfType<BlipTypeAttribute>().BlipId);
            API.setBlipName(blip, (Group == null ? Type() : Group.Group.Name));

            switch (JobData.Type)
            {
                case JobType.Loader:
                    textLabel = API.createTextLabel("~w~Работа грузчиком.\nЗаработок даже за один цикл", new Vector3(JobData.PosX, JobData.PosY, JobData.PosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                    marker = API.createMarker(1, new Vector3(JobData.PosX, JobData.PosY, JobData.PosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                 new Vector3(1f, 1f, 1f), 250, 25, 50, 200);
                    break;

                case JobType.BusDriver:
                    textLabel = API.createTextLabel("~w~Работа водителем\nавтобуса", new Vector3(JobData.PosX, JobData.PosY, JobData.PosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                    marker = API.createMarker(1, new Vector3(JobData.PosX, JobData.PosY, JobData.PosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                 new Vector3(1.5f, 1.5f, 1.5f), 250, 25, 50, 200); break;
                case JobType.TaxiDriver:
                    textLabel = API.createTextLabel("~w~Работа таксистом", new Vector3(JobData.PosX, JobData.PosY, JobData.PosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                    marker = API.createMarker(1, new Vector3(JobData.PosX, JobData.PosY, JobData.PosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                 new Vector3(1.5f, 1.5f, 1.5f), 250, 100, 100, 0); break;
                case JobType.GasStation:
                    if (JobData.CharacterId == 0)
                    {
                        textLabel = API.createTextLabel("~w~Бизнес: заправка\n(свободен): " + JobData.Id, new Vector3(JobData.PosX, JobData.PosY, JobData.PosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                        marker = API.createMarker(1, new Vector3(JobData.PosX, JobData.PosY, JobData.PosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                 new Vector3(1f, 1f, 1f), 250, 10, 250, 10);                        
                        API.setBlipColor(blip, 1);
                    }
                    else
                    {                        
                        textLabel = API.createTextLabel("~w~Бизнес: заправка\nВладелец: " + JobData.OwnerName, new Vector3(JobData.PosX, JobData.PosY, JobData.PosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                        marker = API.createMarker(1, new Vector3(JobData.PosX, JobData.PosY, JobData.PosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                 new Vector3(1f, 1f, 1f), 250, 10, 250, 10);
                        API.setBlipColor(blip, 2);
                    }
                    break;
            }

            CreateColShape();
            CreateMarkersColShape();
        }
        public void CreateColShape()
        {
            colShape = API.createCylinderColShape(new Vector3(JobData.PosX, JobData.PosY, JobData.PosZ), 2f, 3f);
            colShape.onEntityEnterColShape += (shape, entity) =>
            {
                if (API.getPlayerFromHandle(entity) != null)
                {
                    switch (JobData.Type)
                    {
                        case JobType.Loader:
                            Vector3 firstMarker = null;
                            if (JobData.Id == 1) firstMarker = job1Marker1;
                            if (JobData.Id == 2) firstMarker = job2Marker1;
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "work_loader_menu", 1,
                                "Работа грузчиком", "Заработайте свои первые деньги!", JobData.Id,
                                firstMarker.X, firstMarker.Y, firstMarker.Z); break;

                        case JobType.BusDriver:
                            Vector3 firstBusMarker = null;
                            if (JobData.Id == 7) firstBusMarker = BusOne.Marker1;
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "work_busdriver_menu", 1,
                                "Водитель автобуса", "Перевозите пассажиров за деньги!", JobData.Id,
                                firstBusMarker.X, firstBusMarker.Y, firstBusMarker.Z); break;

                        case JobType.GasStation:
                            if (JobData.CharacterId != 0)
                            {                                
                                var owner = ContextFactory.Instance.Character.First(x => x.Id == JobData.CharacterId);
                                CharacterController character = API.getPlayerFromHandle(entity).getData("CHARACTER");
                                bool isOwner = owner.Id == character.Character.Id ? true : false;

                                API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "work_gasstation_menu", 1,
                                    "24/7 заправка", "Владелец: " + owner.Name, JobData.Id, 1, JobData.Cost, isOwner, JobData.Money); break;
                            }
                            else
                            {
                                API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "work_gasstation_menu", 1,
                                    "24/7 заправка", "Доступная для покупки!",  JobData.Id, 2, JobData.Cost); break;
                            }                            
                    }                                      
                }
            };
            colShape.onEntityExitColShape += (shape, entity) =>
            {
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {
                    switch (JobData.Type)
                    {
                        case JobType.Loader:
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "work_loader_menu", 0,
                                "Работа грузчиком", "Заработайте свои первые деньги!", JobData.Id, 0.0, 0.0, 0.0); break;
                        case JobType.BusDriver:
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "work_busdriver_menu", 0,
                                "Водитель автобуса", "Перевозите пассажиров за деньги!", JobData.Id, 0.0, 0.0, 0.0); break;
                        case JobType.GasStation:
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "work_gasstation_menu", 0,
                                    "", "", JobData.Id, 1, JobData.Cost, false, JobData.Money);
                            break;
                    }                    
                }
            };
        }
        public void CreateMarkersColShape()
        {  
            // Bus Driver
            if (JobData.Id == 7)
            {
                ColShape BusOneColShape1 = API.shared.createCylinderColShape(BusOne.Marker1, 3f, 3f);
                ColShape BusOneColShape2 = API.shared.createCylinderColShape(BusOne.Marker2, 3f, 3f);
                ColShape BusOneColShape3 = API.shared.createCylinderColShape(BusOne.Marker3, 3f, 3f);
                ColShape BusOneColShape4 = API.shared.createCylinderColShape(BusOne.Marker4, 3f, 3f);
                ColShape BusOneColShapeFin = API.shared.createCylinderColShape(BusOne.MarkerFin, 3f, 3f);

                BusOneColShape1.onEntityEnterColShape += (shape, entity) =>
                {
                    BusDriver(entity, "BUSONE_1", "BUSONE_2", BusOne.Marker2.X, BusOne.Marker2.Y, BusOne.Marker2.Z,
                        "Первая остановка: Аэропорт. Двигайтесь дальше.");
                };
                BusOneColShape2.onEntityEnterColShape += (shape, entity) =>
                {
                    BusDriver(entity, "BUSONE_2", "BUSONE_3", BusOne.Marker3.X, BusOne.Marker3.Y, BusOne.Marker3.Z,
                        "Вторая остановка: Больница. Двигайтесь дальше.");                    
                };
                BusOneColShape3.onEntityEnterColShape += (shape, entity) =>
                {
                    BusDriver(entity, "BUSONE_3", "BUSONE_4", BusOne.Marker4.X, BusOne.Marker4.Y, BusOne.Marker4.Z,
                        "Третья остановка: Мэрия. Двигайтесь дальше.");                    
                };
                BusOneColShape4.onEntityEnterColShape += (shape, entity) =>
                {
                    BusDriver(entity, "BUSONE_4", "BUSONE_5", BusOne.MarkerFin.X, BusOne.MarkerFin.Y, BusOne.MarkerFin.Z,
                        "Конечная остановка: стройка. Двигайтесь на станцию.");
                };
                BusOneColShapeFin.onEntityEnterColShape += (shape, entity) =>
                {
                    BusDriver(entity, "BUSONE_5", "BUSONE_5", 0.0f, 0.0f, 0.0f, "");
                };
            }
            // Loader 1st place
            if (JobData.Id == 1)
            {
                job1_1MarCol = API.shared.createCylinderColShape(job1Marker1, 2f, 3f);
                job1_2MarCol = API.shared.createCylinderColShape(job1Marker2, 2f, 3f);                

                job1_1MarCol.onEntityEnterColShape += (shape, entity) =>
                {                    
                    CharacterController characterController = API.getPlayerFromHandle(entity).getData("CHARACTER");
                    if (characterController == null) return;

                    if (characterController.Character.JobId == 1)
                    {
                        Client player = API.getPlayerFromHandle(entity);

                        if (player.hasData("SECOND_OK"))
                        {
                            API.triggerClientEvent(player, "loader_end");
                            API.triggerClientEvent(player, "loader_two", job1Marker2.X, job1Marker2.Y, job1Marker2.Z);
                            API.setPlayerClothes(player, 5, 44, 0);
                            player.resetData("SECOND_OK");
                            player.setData("FIRST_OK", null);
                            API.triggerClientEvent(player, "markonmap", job1Marker2.X, job1Marker2.Y);
                        }
                    }
                };
                job1_2MarCol.onEntityEnterColShape += (shape, entity) =>
                {
                    CharacterController characterController = API.getPlayerFromHandle(entity).getData("CHARACTER");
                    if (characterController == null) return;

                    if (characterController.Character.JobId == 1)
                    {
                        Client player = API.getPlayerFromHandle(entity);

                        if (player.hasData("FIRST_OK"))
                        {
                            characterController.Character.Cash += WorkPay.Loader1Pay;
                            ContextFactory.Instance.SaveChanges();
                            API.sendNotificationToPlayer(player, "Вы заработали: " + WorkPay.Loader1Pay + "$");
                            API.triggerClientEvent(player, "update_money_display", characterController.Character.Cash);

                            API.triggerClientEvent(player, "loader_end");
                            API.triggerClientEvent(player, "loader_one", job1Marker1.X, job1Marker1.Y, job1Marker1.Z);
                            API.setPlayerClothes(player, 5, 42, 0);
                            player.resetData("FIRST_OK");
                            player.setData("SECOND_OK", null);
                            API.triggerClientEvent(player, "markonmap", job1Marker1.X, job1Marker1.Y);
                        }
                    }                        
                };
            }
            // Loader 2nd place
            if (JobData.Id == 2)
            {                
                job2_1MarCol = API.shared.createCylinderColShape(job2Marker1, 2f, 3f);
                job2_2MarCol = API.shared.createCylinderColShape(job2Marker2, 2f, 3f);

                job2_1MarCol.onEntityEnterColShape += (shape, entity) =>
                {                    
                    CharacterController characterController = API.getPlayerFromHandle(entity).getData("CHARACTER");
                    if (characterController == null) return;

                    if (characterController.Character.JobId == 2)
                    {
                        Client player = API.getPlayerFromHandle(entity);

                        if (player.hasData("SECOND_OK"))
                        {                            
                            API.triggerClientEvent(player, "loader_end");
                            API.triggerClientEvent(player, "loader_two",
                                job2Marker2.X, job2Marker2.Y, job2Marker2.Z);

                            //API.attachEntityToEntity(box, API.getPlayerFromHandle(entity), "IK_Head", API.getPlayerFromHandle(entity).position, API.getPlayerFromHandle(entity).rotation);

                            API.setPlayerClothes(player, 5, 44, 0);
                            player.resetData("SECOND_OK");
                            player.setData("FIRST_OK", null);
                            API.triggerClientEvent(player, "markonmap", job2Marker2.X, job2Marker2.Y);
                        }
                    }
                };
                job2_2MarCol.onEntityEnterColShape += (shape, entity) =>
                {
                    CharacterController characterController = API.getPlayerFromHandle(entity).getData("CHARACTER");
                    if (characterController == null) return;

                    if (characterController.Character.JobId == 2)
                    {
                        Client player = API.getPlayerFromHandle(entity);

                        if (player.hasData("FIRST_OK"))
                        {                            
                            characterController.Character.Cash += WorkPay.Loader2Pay;
                            ContextFactory.Instance.SaveChanges();
                            API.sendNotificationToPlayer(player, "Вы заработали: " + WorkPay.Loader2Pay + "$");
                            API.triggerClientEvent(player, "update_money_display", characterController.Character.Cash);

                            API.triggerClientEvent(player, "loader_end");
                            API.triggerClientEvent(player, "loader_one", 
                                job2Marker1.X, job2Marker1.Y, job2Marker1.Z);
                            API.setPlayerClothes(player, 5, 42, 0);
                            player.resetData("FIRST_OK");
                            player.setData("SECOND_OK", null);
                            API.triggerClientEvent(player, "markonmap", job2Marker1.X, job2Marker1.Y);
                        }
                    }
                };
            }
            // GasStation_Id3 petrolium
            if (JobData.Id == 3)
            { 
                ColShape GasOneColShape1 = API.shared.createCylinderColShape(GasStation_Id3.Marker1, 2f, 3f);
                API.createMarker(1, GasStation_Id3.Marker1, new Vector3(), new Vector3(), new Vector3(1f, 1f, 1f), 250, 0, 250, 0);
                ColShape GasOneColShape2 = API.shared.createCylinderColShape(GasStation_Id3.Marker2, 2f, 3f);
                API.createMarker(1, GasStation_Id3.Marker2, new Vector3(), new Vector3(), new Vector3(1f, 1f, 1f), 250, 0, 250, 0);
                ColShape GasOneColShape3 = API.shared.createCylinderColShape(GasStation_Id3.Marker3, 2f, 3f);
                API.createMarker(1, GasStation_Id3.Marker3, new Vector3(), new Vector3(), new Vector3(1f, 1f, 1f), 250, 0, 250, 0);
                ColShape GasOneColShape4 = API.shared.createCylinderColShape(GasStation_Id3.Marker4, 2f, 3f);
                API.createMarker(1, GasStation_Id3.Marker4, new Vector3(), new Vector3(), new Vector3(1f, 1f, 1f), 250, 0, 250, 0);
                ColShape GasOneColShape5 = API.shared.createCylinderColShape(GasStation_Id3.Marker5, 2f, 3f);
                API.createMarker(1, GasStation_Id3.Marker5, new Vector3(), new Vector3(), new Vector3(1f, 1f, 1f), 250, 0, 250, 0);
                ColShape GasOneColShape6 = API.shared.createCylinderColShape(GasStation_Id3.Marker6, 2f, 3f);
                API.createMarker(1, GasStation_Id3.Marker6, new Vector3(), new Vector3(), new Vector3(1f, 1f, 1f), 250, 0, 250, 0);

                List<ColShape> GasOneColShapes = new List<ColShape>
                {GasOneColShape1, GasOneColShape2, GasOneColShape3, GasOneColShape4, GasOneColShape5, GasOneColShape6};

                foreach (var colshape in GasOneColShapes)
                {
                    colshape.onEntityEnterColShape += (shape, entity) =>
                    {
                        if (GetPlayerDriver(entity).isInVehicle && JobData.CharacterId != 0)
                            API.shared.triggerClientEvent(GetPlayerDriver(entity), "get_petrolium", 1, JobData.Id);
                        else API.sendNotificationToPlayer(GetPlayerDriver(entity), "~r~Заправка не работает!");
                    };
                    colshape.onEntityExitColShape += (shape, entity) =>
                    {
                        if (GetPlayerDriver(entity).isInVehicle)
                            API.shared.triggerClientEvent(GetPlayerDriver(entity), "get_petrolium", 0, JobData.Id);
                    };
                }                
            }
            // GasStation_Id4 petrolium
            if (JobData.Id == 4)
            {
                 ColShape GasOneColShape1 = API.shared.createCylinderColShape(GasStation_Id4.Marker1, 2f, 3f);
                 API.createMarker(1, GasStation_Id4.Marker1, new Vector3(), new Vector3(), new Vector3(1f, 1f, 1f), 250, 0, 250, 0);
                 ColShape GasOneColShape2 = API.shared.createCylinderColShape(GasStation_Id4.Marker2, 2f, 3f);
                 API.createMarker(1, GasStation_Id4.Marker2, new Vector3(), new Vector3(), new Vector3(1f, 1f, 1f), 250, 0, 250, 0);
                 ColShape GasOneColShape3 = API.shared.createCylinderColShape(GasStation_Id4.Marker3, 2f, 3f);
                 API.createMarker(1, GasStation_Id4.Marker3, new Vector3(), new Vector3(), new Vector3(1f, 1f, 1f), 250, 0, 250, 0);
                 ColShape GasOneColShape4 = API.shared.createCylinderColShape(GasStation_Id4.Marker4, 2f, 3f);
                 API.createMarker(1, GasStation_Id4.Marker4, new Vector3(), new Vector3(), new Vector3(1f, 1f, 1f), 250, 0, 250, 0);

                 List<ColShape> GasOneColShapes = new List<ColShape>
                 {GasOneColShape1, GasOneColShape2, GasOneColShape3, GasOneColShape4};

                 foreach (var colshape in GasOneColShapes)
                 {
                      colshape.onEntityEnterColShape += (shape, entity) =>
                      {
                          if (GetPlayerDriver(entity).isInVehicle && JobData.CharacterId != 0)
                              API.shared.triggerClientEvent(GetPlayerDriver(entity), "get_petrolium", 1, JobData.Id);
                          else API.sendNotificationToPlayer(GetPlayerDriver(entity), "~r~Заправка не работает!");
                      };
                      colshape.onEntityExitColShape += (shape, entity) =>
                      {
                          if (GetPlayerDriver(entity).isInVehicle)
                              API.shared.triggerClientEvent(GetPlayerDriver(entity), "get_petrolium", 0, JobData.Id);
                      };
                 }                
            }
        }

        private Client GetPlayerDriver (NetHandle entity)
        {
            var playersOcupation = API.getVehicleOccupants(entity);
            Client player = API.getPlayerFromHandle(entity);
            foreach (var playerInCar in playersOcupation)
                if (playerInCar.vehicleSeat == -1) player = playerInCar;
            return player;
        }
        
        // BusDriver methods:
        private void BusDriver (NetHandle entity, string startData, string endData,
            float nextX, float nextY, float nextZ, string textStop)
        {
            var playersOcupation = API.getVehicleOccupants(entity);
            Client player = API.getPlayerFromHandle(entity);
            foreach (var playerInCar in playersOcupation)
                if (playerInCar.vehicleSeat == -1) player = playerInCar;

            CharacterController characterController = player.getData("CHARACTER");
            if (characterController == null) return;

            if (characterController.Character.JobId == JobsIdNonDataBase.BusDriver1 ||
                characterController.Character.JobId == JobsIdNonDataBase.BusDriver2 ||
                characterController.Character.JobId == JobsIdNonDataBase.BusDriver3 ||
                characterController.Character.JobId == JobsIdNonDataBase.BusDriver4)
            {
                if (player.hasData(startData) && player.isInVehicle)
                {
                    if (startData == endData)
                    {
                        characterController.Character.Cash += WorkPay.BusDriver1Pay;
                        ContextFactory.Instance.SaveChanges();
                        API.triggerClientEvent(player, "update_money_display", characterController.Character.Cash);

                        API.triggerClientEvent(player, "bus_end");
                        API.sendNotificationToPlayer(player, "Маршрут окончен: выберите новый на автовокзале.");
                        API.sendNotificationToPlayer(player, "~g~Ваш заработок: " + WorkPay.BusDriver1Pay + "$");
                        player.resetData(endData);
                    }
                    else
                    {
                        API.triggerClientEvent(player, "bus_end");
                        API.triggerClientEvent(player, "bus_marker", nextX, nextY, nextZ);
                        player.resetData(startData);
                        player.setData(endData, null);
                        API.triggerClientEvent(player, "markonmap", nextX, nextY);
                        API.sendNotificationToPlayer(player, textStop);
                    }                    
                }
            }
        }

        public static void BusDriverStart (Client player, Character character, string startData, int traceNum,
            float firstPosX, float firstPosY, float firstPosZ)
        {
            if (character.Level < 2)
            {
                API.shared.sendNotificationToPlayer(player, "Вы не можете работать на этой работе. Она доступна со 2 уровня");
                return;
            }
            if (!player.hasData(startData))
            {
                switch (traceNum)
                {
                    case 1: character.JobId = JobsIdNonDataBase.BusDriver1; break;
                    case 2: character.JobId = JobsIdNonDataBase.BusDriver2; break;
                    case 3: character.JobId = JobsIdNonDataBase.BusDriver3; break;
                    case 4: character.JobId = JobsIdNonDataBase.BusDriver4; break;
                }
                ContextFactory.Instance.SaveChanges();
                API.shared.triggerClientEvent(player, "bus_marker", firstPosX, firstPosY, firstPosZ);
                API.shared.triggerClientEvent(player, "markonmap", firstPosX, firstPosY);
                player.setData(startData, null);
                ClothesManager.SetPlayerSkinClothes(player, 5, character, 1);
            }
            else API.shared.sendNotificationToPlayer(player, "Вы уже выбрали свой маршрут! Садитесь в автобус.");
        }

        // Taxi Working
        public void UseTaxis(Client player)
        {
            sen = player;
            senderxcoords = API.getEntityPosition(player.handle).X;
            senderycoords = API.getEntityPosition(player.handle).Y;
            List<Client> taxiPlayers = new List<Client>();
            foreach (var driver in API.getAllPlayers())
            {
                if (API.getEntityData(driver, "TAXI") != null && API.getEntityData(driver, "TASK") != 1.623482)
                {
                    API.sendPictureNotificationToPlayer(driver, player.name + " вызвал такси, вы заберете его?", "CHAR_TAXI", 0, 1, "Downtown Cab Co.", "Job");
                }
            }
        }
        static int i = 0;
        public void Accepted(Client driver, double d)
        {
            foreach (var driver2 in API.getAllPlayers())
            {
                if (API.getEntityData(driver2, "TASK") == d)
                {
                    API.sendChatMessageToPlayer(driver2, "~r~Данный заказ уже был взят");
                    i = 1;
                }
            }

            if (i == 0)
            {
                try
                {
                    API.sendChatMessageToPlayer(driver, "~g~Вы согласились с заказом, следуйте по маршруту!");
                    API.triggerClientEvent(driver, "markonmap", senderxcoords, senderycoords);
                    API.setEntityData(driver, "TASK", d);
                    API.sendPictureNotificationToPlayer(sen, driver.name + " выехал за вами, пожалуйста ждите его приезда!", "CHAR_TAXI", 0, 1, "Downtown Cab Co.", "Message");
                }
                catch (Exception e)
                {
                    API.sendNotificationToPlayer(driver, "У вас нет заявок. Ждите заказов.");
                }
            }
        }
        
        public string Type()
        {
            return JobData.Type.GetDisplayName();
        }
    }
}
