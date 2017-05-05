using GTANetworkServer;
using GTANetworkShared;
using System.Linq;
using TheGodfatherGM.Server.DBManager;
using TheGodfatherGM.Data;
using TheGodfatherGM.Data.Extensions;
using TheGodfatherGM.Data.Attributes;
using TheGodfatherGM.Server.Characters;
using System.Collections.Generic;
using System;

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
        public void CreateWorldEntity()
        {
            blip = API.createBlip(new Vector3(JobData.PosX, JobData.PosY, JobData.PosZ), 0, 0);
            API.setBlipSprite(blip, JobData.Type.GetAttributeOfType<BlipTypeAttribute>().BlipId);
            API.setBlipName(blip, (Group == null ? Type() : Group.Group.Name));

            marker = API.createMarker(1, new Vector3(JobData.PosX, JobData.PosY, JobData.PosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
               new Vector3(1f, 1f, 1f), 250, 25, 50, 200);

            if (JobData.Type == Data.Enums.JobType.Loader) 
                textLabel = API.createTextLabel("~w~Работа грузчиком.\nЗаработок даже за один цикл", new Vector3(JobData.PosX, JobData.PosY, JobData.PosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
            else
                textLabel = API.createTextLabel("~b~[Job (ID: " + JobData.Id + ")]" + (Group == null ? "" : "\n~w~Company: \n" +
                Group.Group.Name) + "\n~y~Job: " + Type() + "\n~w~Level: " + JobData.Level,
                new Vector3(JobData.PosX, JobData.PosY, JobData.PosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);

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
                    Vector3 firstMarker = null;
                    if (JobData.Id == 1) firstMarker = job1Marker1;
                    if (JobData.Id == 2) firstMarker = job2Marker1;
                    API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "work_loader_menu", 1, 
                        "Работа грузчиком", "Заработайте свои первые деньги!", JobData.Id, 
                        firstMarker.X, firstMarker.Y, firstMarker.Z);                    
                }
            };
            colShape.onEntityExitColShape += (shape, entity) =>
            {
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {
                    API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "work_loader_menu", 0, 
                        "Работа грузчиком", "Заработайте свои первые деньги!", JobData.Id, 0.0, 0.0, 0.0);
                }
            };
        }
        public void CreateMarkersColShape()
        {  
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
                        if (API.getPlayerFromHandle(entity).hasData("SECOND_OK"))
                        {
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "loader_end");
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "loader_two", 
                                job1Marker2.X, job1Marker2.Y, job1Marker2.Z);
                            API.shared.setPlayerClothes(API.getPlayerFromHandle(entity), 5, 44, 0);
                            API.getPlayerFromHandle(entity).resetData("SECOND_OK");
                            API.getPlayerFromHandle(entity).setData("FIRST_OK", null);
                            Client player;
                            if ((player = API.getPlayerFromHandle(entity)) == null) return;
                            API.shared.triggerClientEvent(player, "markonmap", job1Marker2.X, job1Marker2.Y);
                        }
                    }
                };
                job1_2MarCol.onEntityEnterColShape += (shape, entity) =>
                {
                    CharacterController characterController = API.getPlayerFromHandle(entity).getData("CHARACTER");
                    if (characterController == null) return;
                    if (characterController.Character.JobId == 1)
                    {
                        if (API.getPlayerFromHandle(entity).hasData("FIRST_OK"))
                        {
                            API.shared.sendChatMessageToPlayer(API.getPlayerFromHandle(entity), "Вы заработали 5$");
                            characterController.Character.Cash += 5;
                            ContextFactory.Instance.SaveChanges();
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "update_money_display", characterController.Character.Cash);
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "loader_end");
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "loader_one", job1Marker1.X, job1Marker1.Y, job1Marker1.Z);
                            API.shared.setPlayerClothes(API.getPlayerFromHandle(entity), 5, 42, 0);
                            API.getPlayerFromHandle(entity).resetData("FIRST_OK");
                            API.getPlayerFromHandle(entity).setData("SECOND_OK", null);
                            Client player;
                            if ((player = API.getPlayerFromHandle(entity)) == null) return;
                            API.shared.triggerClientEvent(player, "markonmap", job1Marker1.X, job1Marker1.Y);
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
                    var box = API.createObject(371570974, API.getPlayerFromHandle(entity).position, API.getPlayerFromHandle(entity).rotation);

                    CharacterController characterController = API.getPlayerFromHandle(entity).getData("CHARACTER");
                    if (characterController == null) return;
                    if (characterController.Character.JobId == 2)
                    {
                        if (API.getPlayerFromHandle(entity).hasData("SECOND_OK"))
                        {
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "loader_end");
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "loader_two",
                                job2Marker2.X, job2Marker2.Y, job2Marker2.Z);

                            //API.attachEntityToEntity(box, API.getPlayerFromHandle(entity), "IK_Head", API.getPlayerFromHandle(entity).position, API.getPlayerFromHandle(entity).rotation);

                            API.shared.setPlayerClothes(API.getPlayerFromHandle(entity), 5, 44, 0);
                            API.getPlayerFromHandle(entity).resetData("SECOND_OK");
                            API.getPlayerFromHandle(entity).setData("FIRST_OK", null);
                            Client player;
                            if ((player = API.getPlayerFromHandle(entity)) == null) return;
                            API.shared.triggerClientEvent(player, "markonmap", job2Marker2.X, job2Marker2.Y);
                        }
                    }
                };
                job2_2MarCol.onEntityEnterColShape += (shape, entity) =>
                {
                    CharacterController characterController = API.getPlayerFromHandle(entity).getData("CHARACTER");
                    if (characterController == null) return;
                    if (characterController.Character.JobId == 2)
                    {
                        if (API.getPlayerFromHandle(entity).hasData("FIRST_OK"))
                        {
                            API.shared.sendChatMessageToPlayer(API.getPlayerFromHandle(entity), "Вы заработали 5$");
                            characterController.Character.Cash += 5;
                            ContextFactory.Instance.SaveChanges();
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "update_money_display", characterController.Character.Cash);
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "loader_end");
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "loader_one", 
                                job2Marker1.X, job2Marker1.Y, job2Marker1.Z);
                            API.shared.setPlayerClothes(API.getPlayerFromHandle(entity), 5, 42, 0);

                            //var box = API.createObject(0, API.getPlayerFromHandle(entity).position, API.getPlayerFromHandle(entity).rotation); ;

                            API.getPlayerFromHandle(entity).resetData("FIRST_OK");
                            API.getPlayerFromHandle(entity).setData("SECOND_OK", null);
                            Client player;
                            if ((player = API.getPlayerFromHandle(entity)) == null) return;
                            API.shared.triggerClientEvent(player, "markonmap", job2Marker1.X, job2Marker1.Y);
                        }
                    }
                };
            }
        }

        // Работа грузчиком
        public static void JobWorkLoader(Client player, int jobId, int trigger)
        {
            CharacterController characterController = player.getData("CHARACTER");
            if (characterController == null) return;           

            if (trigger == 1)
            {
                currentJobId = characterController.Character.JobId;
                characterController.Character.JobId = jobId;
            }
            if (trigger == 0)
            {
                characterController.Character.JobId = currentJobId;
            }
            ContextFactory.Instance.SaveChanges();
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
