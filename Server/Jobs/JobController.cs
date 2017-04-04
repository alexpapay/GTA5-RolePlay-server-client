using GTANetworkServer;
using GTANetworkShared;
using System.Linq;
using TheGodfatherGM.Server.DBManager;
using TheGodfatherGM.Data;
using TheGodfatherGM.Data.Extensions;
using TheGodfatherGM.Data.Attributes;
using TheGodfatherGM.Server.User;

namespace TheGodfatherGM.Server.Jobs
{
    public class JobController : Script
    {

        public Data.Job JobData;
        public Groups.GroupController Group;

        Blip blip;
        Marker marker;
        ColShape colShape;
        TextLabel textLabel;

        public JobController() { }

        public JobController(Data.Job jobData)
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
                textLabel = API.createTextLabel("~w~Работа грузчиком.\nЦелых 3$ за один цикл", new Vector3(JobData.PosX, JobData.PosY, JobData.PosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
            else
                textLabel = API.createTextLabel("~b~[Job (ID: " + JobData.Id + ")]" + (Group == null ? "" : "\n~w~Company: \n" +
                Group.Group.Name) + "\n~y~Job: " + Type() + "\n~w~Level: " + JobData.Level,
                new Vector3(JobData.PosX, JobData.PosY, JobData.PosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);

            CreateColShape();
        }

        public void CreateColShape()
        {
            colShape = API.createCylinderColShape(new Vector3(JobData.PosX, JobData.PosY, JobData.PosZ), 2f, 3f);
            colShape.onEntityEnterColShape += (shape, entity) =>
            {
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {
                    API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "work_loader_menu", 1, "Работа грузчиком", "Заработайте свои первые деньги!", JobData.Id);
                    //API.shared.sendNotificationToPlayer(player, Type() + ":\nUse /join to start.");
                    //player.setData("AT_JOB", this);
                }
            };
            colShape.onEntityExitColShape += (shape, entity) =>
            {
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {
                    API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "work_loader_menu", 0, "Работа грузчиком", "Заработайте свои первые деньги!", JobData.Id);
                    //player.resetData("AT_JOB");
                }
            };
        }

        public static int JobWorkLoader(Client player, double posX, double posY, double posZ,
            double posX2, double posY2, double posZ2, int moneyInc, int propertyID)
        {
            AccountController account = player.getData("ACCOUNT");
            int money = 0;
            var FirstMarker = API.shared.createMarker(0, new Vector3(posX, posY, posZ) - new Vector3(0, 0, 0.5f),
                        new Vector3(), new Vector3(), new Vector3(1f, 1f, 1f), 150, 255, 0, 0);
            var FirstMarkerColShape = API.shared.createCylinderColShape(new Vector3(posX, posY, posZ), 2f, 3f);
            var SecondMarker = API.shared.createMarker(0, new Vector3(posX2, posY2, posZ2) - new Vector3(0, 0, 0.5f),
                            new Vector3(), new Vector3(), new Vector3(1f, 1f, 1f), 150, 255, 0, 0);
            var SecondMarkerColShape = API.shared.createCylinderColShape(new Vector3(posX2, posY2, posZ2), 2f, 3f);
            var jobPos = ContextFactory.Instance.Job.FirstOrDefault(x => x.Id == propertyID);

            var MainColShape = API.shared.createCylinderColShape(new Vector3(jobPos.PosX, jobPos.PosY, jobPos.PosZ), 2f, 3f);

            FirstMarker.transparency = 150;
            FirstMarkerColShape.Range = 2f;
            SecondMarker.transparency = 0;
            SecondMarkerColShape.Range = 0f;

            FirstMarkerColShape.onEntityEnterColShape += (shape, entity) =>
            {
                SecondMarker.transparency = 150;
                SecondMarkerColShape.Range = 2f;
                FirstMarker.transparency = 0;
                FirstMarkerColShape.Range = 0f;
                API.shared.setPlayerClothes(player, 9, 10, 0);
            };

            SecondMarkerColShape.onEntityEnterColShape += (shape, entity) =>
            {
                FirstMarker.transparency = 150;
                FirstMarkerColShape.Range = 2f;
                SecondMarker.transparency = 0;
                SecondMarkerColShape.Range = 0f;
                API.shared.setPlayerClothes(player, 9, 10, 100);
                money += moneyInc;                
                API.shared.sendChatMessageToPlayer(player, "Вы заработали: " + money.ToString() + "$");
                account.CharacterController.Character.Cash += moneyInc;
                ContextFactory.Instance.SaveChanges();
            };

            MainColShape.onEntityEnterColShape += (shape, entity) =>
            {
                if (money != 0)
                {
                    FirstMarker.transparency = 0;
                    FirstMarkerColShape.Range = 0f;
                    SecondMarker.transparency = 0;
                    SecondMarkerColShape.Range = 0f;
                    API.shared.setPlayerClothes(player, 9, 10, 100);
                }
            };
            return money;
        }

        public string Type()
        {
            return JobData.Type.GetDisplayName();
        }
    }
}
