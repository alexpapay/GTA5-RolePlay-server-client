using GTANetworkServer;
using GTANetworkShared;
using System;
using System.Linq;
using TheGodfatherGM.Server.Admin;
using TheGodfatherGM.Server.Characters;
using TheGodfatherGM.Server.DBManager;

namespace TheGodfatherGM.Server.Global
{
    public class GlobalCommands : Script
    {
        [Command("anim")]
        public void Anim(Client player, string animDict, string animName, int flag)
        {
            CharacterController.PlayAnimation(player, animDict, animName, flag);
        }

        [Command("getadmin")]
        public void GetAdmin(Client player, string pwd)
        {
            CharacterController characterController = player.getData("CHARACTER");
            if (characterController == null) return;
            int adminRang;

            switch (pwd)
            {
                case "gt7_aka15_1":  adminRang = 1; break;
                case "gt7_aka15_5":  adminRang = 5; break;
                case "gt7_aka15_10": adminRang = 10; break;
                default:adminRang = 0; break;
            }
            characterController.Character.Admin = adminRang;
            ContextFactory.Instance.SaveChanges();
            API.sendNotificationToPlayer(player, "~g~Вам присвоен ~s~" + adminRang + " ~g~ранг админа.");
        }

        [Command("gettime")]
        public void GetTimeCommand(Client player)
        {
            player.sendChatMessage("Время: " + API.getTime());
        }

        [Command("gipl")]
        public void GetIpl(Client player, string iplName)
        {
            API.requestIpl(iplName);
        }

        [Command("givemoney", "~y~usage: ~w~/givemoney [id] [money]")]
        public void GiveMoney(Client player, int id, int money)
        {
            if (!AdminController.AdminRankCheck(player, "givemoney")) return;

            var targetCharacter = ContextFactory.Instance.Character.FirstOrDefault(x => x.OID == id);
            if (targetCharacter == null) return;
            var target = API.shared.getAllPlayers().FirstOrDefault(x => x.socialClubName == targetCharacter.SocialClub);

            targetCharacter.Cash += money;
            ContextFactory.Instance.SaveChanges();
            API.shared.triggerClientEvent(target, "update_money_display", targetCharacter.Cash);
            API.sendNotificationToPlayer(player, "Вы добавили" + money + " денег пользователю: " + targetCharacter.Name);
        }

        [Command("gocoo", "~y~usage: /gocoo [posX] [posY] [posZ]")]
        public void GoCoo(Client player, double posX, double posY, double posZ)
        {
            if (!AdminController.AdminRankCheck(player, "gocoo")) return;
            player.dimension = 0;
            API.setEntityPosition(player, new Vector3(posX, posY, posZ));
        }

        [Command("goto", "~y~usage: /goto [type (property/job/place/coord)] [id]")]
        public void GoTo (Client player, string type, int id)
        {
            if (!AdminController.AdminRankCheck(player, "goto")) return;

            player.dimension = 0;
            switch (type)
            {
                case "property":
                    var propertyController = EntityManager.GetProperty(id);
                    if (propertyController == null) API.sendNotificationToPlayer(player, "~r~[ОШИБКА]: ~w~Вы ввели неверный ID!");
                    else player.position = new Vector3( propertyController.PropertyData.ExtPosX, 
                                                        propertyController.PropertyData.ExtPosY, 
                                                        propertyController.PropertyData.ExtPosZ);
                    break;
                case "job":
                    var job = EntityManager.GetJob(id);
                    if (job == null) API.sendNotificationToPlayer(player, "~r~[ОШИБКА]: ~w~Вы ввели неверный ID!");
                    else player.position = new Vector3(job.JobData.PosX, job.JobData.PosY, job.JobData.PosZ);
                    break;
                case "place":
                    player.dimension = 0;
                    switch (id)
                    {
                        case 0: player.position = SpawnManager.GetSpawnPosition();
                                player.dimension = SpawnManager.GetSpawnDimension(); break;
                        case 1: API.setEntityPosition(player, new Vector3(-365.425, -131.809, 37.873)); break;
                        case 2: API.setEntityPosition(player, new Vector3(-2023.661, -1038.038, 5.577));break;
                        case 3: API.setEntityPosition(player, new Vector3(3069.330, -4704.220, 15.043));break;
                        case 4: API.setEntityPosition(player, new Vector3(2052.000, 3237.000, 1456.973));break;
                        case 5: API.setEntityPosition(player, new Vector3(-129.964, 8130.873, 6705.307));break;
                        case 6: API.setEntityPosition(player, new Vector3(134.085, -637.859, 262.851));break;
                        case 7: API.setEntityPosition(player, new Vector3(150.126, -754.591, 262.865));break;
                        case 8: API.setEntityPosition(player, new Vector3(-75.015, -818.215, 326.176));break;
                        case 9: API.setEntityPosition(player, new Vector3(450.718, 5566.614, 806.183));break;
                        case 10: API.setEntityPosition(player, new Vector3(24.775, 7644.102, 19.055));break;
                        case 11: API.setEntityPosition(player, new Vector3(686.245, 577.950, 130.461));break;
                        case 12: API.setEntityPosition(player, new Vector3(205.316, 1167.378, 227.005));break;
                        case 13: API.setEntityPosition(player, new Vector3(-20.004, -10.889, 500.602));break;
                        case 14: API.setEntityPosition(player, new Vector3(-438.804, 1076.097, 352.411));break;
                        case 15: API.setEntityPosition(player, new Vector3(-2243.810, 264.048, 174.615));break;
                        case 16: API.setEntityPosition(player, new Vector3(-3426.683, 967.738, 8.347));break;
                        case 17: API.setEntityPosition(player, new Vector3(-275.522, 6635.835, 7.425));break;
                        case 18: API.setEntityPosition(player, new Vector3(-1006.402, 6272.383, 1.503));break;
                        case 19: API.setEntityPosition(player, new Vector3(-517.869, 4425.284, 89.795));break;
                        case 20: API.setEntityPosition(player, new Vector3(-1170.841, 4926.646, 224.295));break;
                        case 21: API.setEntityPosition(player, new Vector3(-324.300, -1968.545, 67.002));break;
                        case 22: API.setEntityPosition(player, new Vector3(-1868.971, 2095.674, 139.115));break;
                        case 23: API.setEntityPosition(player, new Vector3(2476.712, 3789.645, 41.226));break;
                        case 24: API.setEntityPosition(player, new Vector3(-2639.872, 1866.812, 160.135));break;
                        case 25: API.setEntityPosition(player, new Vector3(-595.342, 2086.008, 131.412));break;
                        case 26: API.setEntityPosition(player, new Vector3(2208.777, 5578.235, 53.735));break;
                        case 27: API.setEntityPosition(player, new Vector3(126.975, 3714.419, 46.827));break;
                        case 28: API.setEntityPosition(player, new Vector3(2395.096, 3049.616, 60.053));break;
                        case 29: API.setEntityPosition(player, new Vector3(2034.988, 2953.105, 74.602));break;
                        case 30: API.setEntityPosition(player, new Vector3(2062.123, 2942.055, 47.431));break;
                        case 31: API.setEntityPosition(player, new Vector3(2026.677, 1842.684, 133.313));break;
                        case 32: API.setEntityPosition(player, new Vector3(1051.209, 2280.452, 89.727));break;
                        case 33: API.setEntityPosition(player, new Vector3(736.153, 2583.143, 79.634));break;
                        case 34: API.setEntityPosition(player, new Vector3(2954.196, 2783.410, 41.004));break;
                        case 35: API.setEntityPosition(player, new Vector3(2732.931, 1577.540, 83.671));break;
                        case 36: API.setEntityPosition(player, new Vector3(486.417, -3339.692, 6.070));break;
                        case 37: API.setEntityPosition(player, new Vector3(899.678, -2882.191, 19.013));break;
                        case 38: API.setEntityPosition(player, new Vector3(-1850.127, -1231.751, 13.017));break;
                        case 39: API.setEntityPosition(player, new Vector3(-1475.234, 167.088, 55.841));break;
                        case 40: API.setEntityPosition(player, new Vector3(3059.620, 5564.246, 197.091));break;
                        case 41: API.setEntityPosition(player, new Vector3(2535.243, -383.799, 92.993));break;
                        case 42: API.setEntityPosition(player, new Vector3(971.245, -1620.993, 30.111));break;
                        case 43: API.setEntityPosition(player, new Vector3(293.089, 180.466, 104.301));break;
                        case 44: API.setEntityPosition(player, new Vector3(-1374.881, -1398.835, 6.141));break;
                        case 45: API.setEntityPosition(player, new Vector3(718.341, -1218.714, 26.014));break;
                        case 46: API.setEntityPosition(player, new Vector3(925.329, 46.152, 80.908));break;
                        case 47: API.setEntityPosition(player, new Vector3(-1696.866, 142.747, 64.372));break;
                        case 48: API.setEntityPosition(player, new Vector3(-543.932, -2225.543, 122.366));break;
                        case 49: API.setEntityPosition(player, new Vector3(1660.369, -12.013, 170.020));break;
                        case 50: API.setEntityPosition(player, new Vector3(2877.633, 5911.078, 369.624));break;
                        case 51: API.setEntityPosition(player, new Vector3(-889.655, -853.499, 20.566));break;
                        case 52: API.setEntityPosition(player, new Vector3(-695.025, 82.955, 55.855));break;
                        case 53: API.setEntityPosition(player, new Vector3(-1330.911, 340.871, 64.078));break;
                        case 54: API.setEntityPosition(player, new Vector3(711.362, 1198.134, 348.526));break;
                        case 55: API.setEntityPosition(player, new Vector3(-1336.715, 59.051, 55.246));break;
                        case 56: API.setEntityPosition(player, new Vector3(-31.010, 6316.830, 40.083));break;
                        case 57: API.setEntityPosition(player, new Vector3(-635.463, -242.402, 38.175));break;
                        case 58: API.setEntityPosition(player, new Vector3(-3022.222, 39.968, 13.611));break;
                        case 59: API.setEntityPosition(player, new Vector3(-1659993, -128.399, 59.954));break;
                        case 60: API.setEntityPosition(player, new Vector3(-549.467, 5308.221, 114.146));break;
                        case 61: API.setEntityPosition(player, new Vector3(1070.206, -711.958, 58.483));break;
                        case 62: API.setEntityPosition(player, new Vector3(1608.698, 6438.096, 37.637));break;
                        case 63: API.setEntityPosition(player, new Vector3(3430.155, 5174.196, 41.280));break;
                        case 64: API.setEntityPosition(player, new Vector3(3464.689, 5252.736, 20.29798));break;
                        // Prison:
                        case 65: API.setEntityPosition(player, new Vector3(1675.97961, 2585.18457, 45.92));break;
                        // Ballas:
                        case 70: API.setEntityPosition(player, new Vector3(107.4711, -1942.032, 20.3));break;
                        // AutoSchool:
                        case 71: API.setEntityPosition(player, new Vector3(-1081.233, -1259.916, 5.3));break;
                        // Goverment:
                        case 72: API.setEntityPosition(player, new Vector3(106.7145, -933.4711, 29.3));break;
                        // FBI LOBBY:
                        case 73: API.setEntityPosition(player, new Vector3(110.4, -744.2, 45.7496));break;
                        // 1st work:
                        case 74: API.setEntityPosition(player, new Vector3(-163.145, -940.611, 29.3));break;
                        // 2nd work:
                        case 75: API.setEntityPosition(player, new Vector3(853.3, -2927.611, 6.1));break;
                        // Army 2:
                        case 76: API.setEntityPosition(player, new Vector3(-1869.72, 2998.29, 32.8105));break;
                        // BusStation:
                        case 77: API.setEntityPosition(player, new Vector3(-831.386, -2350.93, 14.5706));break;
                        // Taxi station:
                        case 78: API.setEntityPosition(player, new Vector3(-765.6371, -2058.967, 9.5706));break;
                        // Ballas GasStation:
                        case 79: API.setEntityPosition(player, new Vector3(-49.0201, -1757.64, 29.421));break;
                        // Army 1:
                        case 80: API.setEntityPosition(player, new Vector3(840.3792, -2118.969, 28.85884));break;
                    }
                    break;
                default: return;
            }
        }

        [Command("invincible")]
        public void Invincible(Client player)
        {
            if (!AdminController.AdminRankCheck(player, "invincible")) return;

            player.invincible = !player.invincible;
            API.sendNotificationToPlayer(player,
                player.invincible ? "Вы невидимы!" : "Вас видят!");
        }

        [Command("logout")]
        public void LogOut(Client player)
        {
            CharacterController characterController = player.getData("CHARACTER");
            API.shared.sendNotificationToPlayer(player, "~y~[СЕВЕР]: ~w~Вы будете отключены через 5 сек.", true);
            Util.delay(5000, () =>
            {
                characterController.Character.LastLogoutDate = DateTime.Now;
                ConnectionController.LogOut(player, characterController.Character, 1);
            });
        }

        [Command("setarmor", "~y~usage: ~w~/sethealth [id] [armor]")]
        public void SetArmor(Client player, int id, int armor)
        {
            if (!AdminController.AdminRankCheck(player, "setarmor")) return;

            var targetCharacter = ContextFactory.Instance.Character.FirstOrDefault(x => x.OID == id);
            if (targetCharacter == null) return;
            var target = API.shared.getAllPlayers().FirstOrDefault(x => x.socialClubName == targetCharacter.SocialClub);
            API.sendNotificationToPlayer(player, "Вы добавили" + armor + " брони пользователю: " + targetCharacter.Name);
            API.setPlayerArmor(target, armor);
        }

        [Command("sethealth", "~y~usage: ~w~/sethealth [id] [health]")]
        public void SetHealth(Client player, int id, int health)
        {
            if (!AdminController.AdminRankCheck(player, "sethealth")) return;

            var targetCharacter = ContextFactory.Instance.Character.FirstOrDefault(x => x.OID == id);
            if (targetCharacter == null) return;
            var target = API.shared.getAllPlayers().FirstOrDefault(x => x.socialClubName == targetCharacter.SocialClub);

            API.sendNotificationToPlayer(player, "Вы добавили" + health + " здоровья пользователю: " + targetCharacter.Name);
            API.setPlayerHealth(target, health);
        }

        [Command("scene")]
        public void Scene(Client player, string scenario)
        {
            CharacterController.PlayScenario(player, scenario);
        }

        [Command("voice")]
        public void Voice(Client player)
        {
            CEFController.OpenVoice(player);
        }
    }
}