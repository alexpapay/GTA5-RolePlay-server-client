using System;
using System.Linq;
using System.Collections.Generic;

using GTANetworkServer;
using GTANetworkShared;

using TheGodfatherGM.Data;
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

        // Водитель автобуса :: метки и маркеры
        public Vector3 jobBus1Marker1 = new Vector3(307.9809, -1378.786, 31.47842);
        public Vector3 jobBus1Marker2 = new Vector3(115.8893, -938.0698, 29.32286);
        public Vector3 jobBus1Marker3 = new Vector3(-146.7599, -919.225, 28.87529);
        public Vector3 jobBus1Marker4 = new Vector3(-1032.782, -2723.743, 13.66907);
        public Vector3 jobBus1MarkerFin = new Vector3(-810.782, -2330.743, 14.66907);
        ColShape jobBus1Colshape1;
        ColShape jobBus1Colshape2;
        ColShape jobBus1Colshape3;
        ColShape jobBus1Colshape4;
        ColShape jobBus1ColshapeFin;

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
        public void CreateWorldEntity()
        {
            blip = API.createBlip(new Vector3(JobData.PosX, JobData.PosY, JobData.PosZ), 0, 0);
            API.setBlipSprite(blip, JobData.Type.GetAttributeOfType<BlipTypeAttribute>().BlipId);
            API.setBlipName(blip, (Group == null ? Type() : Group.Group.Name));



            switch (JobData.Type)
            {
                case Data.Enums.JobType.Loader:
                    textLabel = API.createTextLabel("~w~Работа грузчиком.\nЗаработок даже за один цикл", new Vector3(JobData.PosX, JobData.PosY, JobData.PosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                    marker = API.createMarker(1, new Vector3(JobData.PosX, JobData.PosY, JobData.PosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                 new Vector3(1f, 1f, 1f), 250, 25, 50, 200);
                    break;

                case Data.Enums.JobType.BusDriver:
                    textLabel = API.createTextLabel("~w~Работа водителем\nавтобуса", new Vector3(JobData.PosX, JobData.PosY, JobData.PosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                    marker = API.createMarker(1, new Vector3(JobData.PosX, JobData.PosY, JobData.PosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                 new Vector3(1.5f, 1.5f, 1.5f), 250, 25, 50, 200); break;

                case Data.Enums.JobType.GasStation:
                    if (JobData.CharacterId == 0)
                    {
                        textLabel = API.createTextLabel("~w~Бизнес: заправка\n(свободен)", new Vector3(JobData.PosX, JobData.PosY, JobData.PosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                        marker = API.createMarker(1, new Vector3(JobData.PosX, JobData.PosY, JobData.PosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                 new Vector3(1f, 1f, 1f), 250, 10, 250, 10);
                    }
                    else
                    {
                        //var character = ContextFactory.Instance.Character.First(x => x.Id == JobData.CharacterId);
                        //if (character == null) break;
                        textLabel = API.createTextLabel("~w~Бизнес: заправка\n", new Vector3(JobData.PosX, JobData.PosY, JobData.PosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                        marker = API.createMarker(1, new Vector3(JobData.PosX, JobData.PosY, JobData.PosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                 new Vector3(1f, 1f, 1f), 250, 10, 250, 10);
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
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {
                    switch (JobData.Type)
                    {
                        case Data.Enums.JobType.Loader:
                            Vector3 firstMarker = null;
                            if (JobData.Id == 1) firstMarker = job1Marker1;
                            if (JobData.Id == 2) firstMarker = job2Marker1;
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "work_loader_menu", 1,
                                "Работа грузчиком", "Заработайте свои первые деньги!", JobData.Id,
                                firstMarker.X, firstMarker.Y, firstMarker.Z); break;

                        case Data.Enums.JobType.BusDriver:
                            Vector3 firstBusMarker = null;
                            if (JobData.Id == 7) firstBusMarker = jobBus1Marker1;
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "work_busdriver_menu", 1,
                                "Водитель автобуса", "Перевозите пассажиров за деньги!", JobData.Id,
                                firstBusMarker.X, firstBusMarker.Y, firstBusMarker.Z); break;
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
                        case Data.Enums.JobType.Loader:
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "work_loader_menu", 0,
                                "Работа грузчиком", "Заработайте свои первые деньги!", JobData.Id, 0.0, 0.0, 0.0); break;
                        case Data.Enums.JobType.BusDriver:
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "work_busdriver_menu", 0,
                                "Водитель автобуса", "Перевозите пассажиров за деньги!", JobData.Id, 0.0, 0.0, 0.0); break;
                    }                    
                }
            };
        }
        public void CreateMarkersColShape()
        {  
            if (JobData.Id == 7)
            {
                jobBus1Colshape1 = API.shared.createCylinderColShape(jobBus1Marker1, 3f, 3f);
                jobBus1Colshape2 = API.shared.createCylinderColShape(jobBus1Marker2, 3f, 3f);
                jobBus1Colshape3 = API.shared.createCylinderColShape(jobBus1Marker3, 3f, 3f);
                jobBus1Colshape4 = API.shared.createCylinderColShape(jobBus1Marker4, 3f, 3f);
                jobBus1ColshapeFin = API.shared.createCylinderColShape(jobBus1MarkerFin, 3f, 3f);

                jobBus1Colshape1.onEntityEnterColShape += (shape, entity) =>
                {
                    var playersOcupation = API.getVehicleOccupants(entity);
                    Client player = API.getPlayerFromHandle(entity);
                    foreach (var playerInCar in playersOcupation)
                        if (playerInCar.vehicleSeat == -1) player = playerInCar;
                 
                    CharacterController characterController = player.getData("CHARACTER");
                    if (characterController == null) return;

                    if (characterController.Character.JobId == 7)
                    { 
                        if (player.hasData("FIRSTBUS_1") && player.isInVehicle)
                        {
                            API.triggerClientEvent(player, "bus_end");
                            API.triggerClientEvent(player, "bus_marker", jobBus1Marker2.X, jobBus1Marker2.Y, jobBus1Marker2.Z);
                            player.resetData("FIRSTBUS_1");
                            player.setData("SECONDBUS_1", null);
                            API.triggerClientEvent(player, "markonmap", jobBus1Marker2.X, jobBus1Marker2.Y);
                            API.sendNotificationToPlayer(player, "Первая остановка: Больница. Двигайтесь дальше.");
                        }
                    }
                };
                jobBus1Colshape2.onEntityEnterColShape += (shape, entity) =>
                {
                    var playersOcupation = API.getVehicleOccupants(entity);
                    Client player = API.getPlayerFromHandle(entity);
                    foreach (var playerInCar in playersOcupation)
                        if (playerInCar.vehicleSeat == -1) player = playerInCar;

                    CharacterController characterController = player.getData("CHARACTER");
                    if (characterController == null) return;

                    if (characterController.Character.JobId == 7)
                    {
                        if (player.hasData("SECONDBUS_1") && player.isInVehicle)
                        {
                            API.triggerClientEvent(player, "bus_end");
                            API.triggerClientEvent(player, "bus_marker", jobBus1Marker3.X, jobBus1Marker3.Y, jobBus1Marker3.Z);
                            player.resetData("SECONDBUS_1");
                            player.setData("THIRDBUS_1", null);
                            API.triggerClientEvent(player, "markonmap", jobBus1Marker3.X, jobBus1Marker3.Y);
                            API.sendNotificationToPlayer(player, "Вторая остановка: Мэрия. Двигайтесь дальше.");
                        }
                    }
                };
                jobBus1Colshape3.onEntityEnterColShape += (shape, entity) =>
                {
                    var playersOcupation = API.getVehicleOccupants(entity);
                    Client player = API.getPlayerFromHandle(entity);
                    foreach (var playerInCar in playersOcupation)
                        if (playerInCar.vehicleSeat == -1) player = playerInCar;

                    CharacterController characterController = player.getData("CHARACTER");
                    if (characterController == null) return;

                    if (characterController.Character.JobId == 7)
                    {
                        if (player.hasData("THIRDBUS_1") && player.isInVehicle)
                        {
                            API.triggerClientEvent(player, "bus_end");
                            API.triggerClientEvent(player, "bus_marker", jobBus1Marker4.X, jobBus1Marker4.Y, jobBus1Marker4.Z);
                            player.resetData("THIRDBUS_1");
                            player.setData("FOURTHBUS_1", null);
                            API.triggerClientEvent(player, "markonmap", jobBus1Marker4.X, jobBus1Marker4.Y);
                            API.sendNotificationToPlayer(player, "Третья остановка: Работа грузчиком. Двигайтесь дальше.");
                        }
                    }
                };
                jobBus1Colshape4.onEntityEnterColShape += (shape, entity) =>
                {
                    var playersOcupation = API.getVehicleOccupants(entity);
                    Client player = API.getPlayerFromHandle(entity);
                    foreach (var playerInCar in playersOcupation)
                        if (playerInCar.vehicleSeat == -1) player = playerInCar;

                    CharacterController characterController = player.getData("CHARACTER");
                    if (characterController == null) return;

                    if (characterController.Character.JobId == 7)
                    {
                        if (player.hasData("FOURTHBUS_1") && player.isInVehicle)
                        {
                            API.triggerClientEvent(player, "bus_end");
                            API.triggerClientEvent(player, "bus_marker", jobBus1Marker4.X, jobBus1Marker4.Y, jobBus1Marker4.Z);
                            player.resetData("FOURTHBUS_1");
                            player.setData("FIFTHBUS_1", null);
                            API.triggerClientEvent(player, "markonmap", jobBus1Marker4.X, jobBus1Marker4.Y);
                            API.sendNotificationToPlayer(player, "Конечная остановка: Аэропорт. Двигайтесь обратно на автовокзал.");
                        }
                    }
                };
                jobBus1ColshapeFin.onEntityEnterColShape += (shape, entity) =>
                {
                    var playersOcupation = API.getVehicleOccupants(entity);
                    Client player = API.getPlayerFromHandle(entity);
                    foreach (var playerInCar in playersOcupation)
                        if (playerInCar.vehicleSeat == -1) player = playerInCar;

                    CharacterController characterController = player.getData("CHARACTER");
                    if (characterController == null) return;

                    if (characterController.Character.JobId == 7)
                    {
                        if (player.hasData("FIFTHBUS_1") && player.isInVehicle)
                        {
                            characterController.Character.Cash += WorkPay.BusDriver1Pay;
                            ContextFactory.Instance.SaveChanges();
                            API.triggerClientEvent(player, "update_money_display", characterController.Character.Cash);

                            API.triggerClientEvent(player, "bus_end");
                            API.triggerClientEvent(player, "bus_marker", jobBus1MarkerFin.X, jobBus1MarkerFin.Y, jobBus1MarkerFin.Z);
                            player.resetData("FIFTHBUS_1");
                            API.triggerClientEvent(player, "markonmap", jobBus1MarkerFin.X, jobBus1MarkerFin.Y);
                            API.sendNotificationToPlayer(player, "Маршрут окончен: выберите новый на автовокзале. ~g~Ваш заработок: " + WorkPay.BusDriver1Pay + "$");
                        }
                    }
                };
            }
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
        }
        
        // Работа таксистом
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
