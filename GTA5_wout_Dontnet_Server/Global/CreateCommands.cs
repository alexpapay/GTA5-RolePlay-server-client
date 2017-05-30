using GTANetworkServer;
using GTANetworkShared;
using TheGodfatherGM.Data.Enums;
using TheGodfatherGM.Server.Admin;
using TheGodfatherGM.Server.DBManager;
using TheGodfatherGM.Server.Jobs;
using TheGodfatherGM.Server.Property;

namespace TheGodfatherGM.Server.Global
{
    public class CreateCommands : Script
    {
        [Command("createhome", "~y~Usage: ~w~/createhome  [cost]")]
        public void CreateHome(Client player, int cost)
        {
            if (!AdminController.AdminRankCheck(player, "createhome")) return;
            PropertyController.CreateHome(player, cost);
        }

        [Command("createjob", "~y~usage: ~w~/createjob [group] [characterId] [level] [cost] [type]")]
        public void Createjob(Client player, int characterId, int level, int cost, JobType type)
        {
            if (!AdminController.AdminRankCheck(player, "createjob")) return;

            var jobData = new Data.Job
            {
                CharacterId = characterId,
                Level = level,
                Cost = cost,
                Type = type,
                PosX = player.position.X,
                PosY = player.position.Y,
                PosZ = player.position.Z
            };
            var jobController = new JobController(jobData);
            ContextFactory.Instance.Job.Add(jobData);
            ContextFactory.Instance.SaveChanges();
            jobController.CreateWorldEntity();

            player.sendChatMessage("~g~[СЕРВЕР]: ~w~ Добавлена работа: " + jobController.Type());
        }

        [Command("createpickup", "~y~Usage: ~w~/createpickup [hash] [amount] [resptime]")]
        public void CreatePickup(Client player, PickupHash hash, int amount, uint resptime)
        {
            if (!AdminController.AdminRankCheck(player, "createpickup")) return;
            API.createPickup(hash, player.position, player.rotation, amount, resptime);
        }

        [Command("createproperty", "~y~Usage: ~w~/createproperty [player/group/null] [type] [ID/Part of Name] [cost]")]
        public void CreateProperty(Client player, string ownerType, PropertyType type, string idOrName, int cost)
        {
            if (!AdminController.AdminRankCheck(player, "createproperty")) return;
            PropertyController.CreateProperty(player, ownerType, type, idOrName, cost);
        }

        [Command("createvehicle", "~y~Usage: ~w~/createvehicle [characterId] [groupId] [Model] [Color1] [Color2]")]
        public void CreateVehicle(Client player, int characterId, int groupId, VehicleHash hash, int color1, int color2)
        {
            if (!AdminController.AdminRankCheck(player, "createvehicle")) return;

            var vehicleData = new Data.Vehicle
            {
                CharacterId = characterId == 0 ? (int?) null : characterId,
                GroupId = groupId == 0 ? (int?) null : groupId,
                Model = hash.GetHashCode(),
                PosX = player.position.X,
                PosY = player.position.Y,
                PosZ = player.position.Z,
                Rot = player.rotation.Z,
                Color1 = color1,
                Color2 = color2,
                Fuel = 50,
                FuelTank = 50,
                RentTime = 999,
                Respawnable = true
            };

            var vehicleController = new Vehicles.VehicleController(vehicleData, API.createVehicle(hash, player.position, player.rotation, color1, color2, 0));

            ContextFactory.Instance.Vehicle.Add(vehicleData);
            ContextFactory.Instance.SaveChanges();
        }

        [Command("editjob", "~y~usage: ~w~/editjob [id] [characterId] [level] [cost] [type]")]
        public void Editjob(Client player, int id, int characterId, int level, int cost, JobType type)
        {
            if (!AdminController.AdminRankCheck(player, "editjob")) return;

            var job = EntityManager.GetJob(id);
            if (job == null) API.sendNotificationToPlayer(player, "~r~[ОШИБКА]: ~w~Вы указали неверную работу!");
            else
            {
                job.JobData.CharacterId = characterId;
                job.JobData.Level = level;
                job.JobData.Cost = cost;
                job.JobData.Type = type;
                ContextFactory.Instance.SaveChanges();
                API.sendNotificationToPlayer(player, "Вы успешно изменили работу: " + id);
            }
        }
        
    }
}
