using GTANetworkServer;
using TheGodfatherGM.Data;
using TheGodfatherGM.Server.Characters;

namespace TheGodfatherGM.Server.Admin
{
    public class AdminController
    {
        
        public static bool AdminRankCheck(Client player, string cmd)
        {
            CharacterController characterController = player.getData("CHARACTER");
            if (characterController == null) return false;

            switch (characterController.Character.Admin)
            {
                case 1:
                    break;

                case 5:
                    switch (cmd)
                    {
                        case "createhome":
                            return true;
                        case "createpickup":
                            return true;
                        case "createvehicle":
                            return true;
                    }
                    break;

                case 10:
                    switch (cmd)
                    {
                        case "createhome":
                            return true;
                        case "createjob":
                            return true;
                        case "createpickup":
                            return true;
                        case "createproperty":
                            return true;
                        case "createvehicle":
                            return true;
                        case "editjob":
                            return true;
                        case "invincible":
                            return true;
                        case "givemoney":
                            return true;
                    }
                    break;
            }
            if (characterController.Character.Admin != 0)
            {
                if (cmd == "gocoo") return true;
                if (cmd == "goto") return true;
                if (cmd == "sethealth") return true;
                if (cmd == "setarmor") return true;
            }
            API.shared.sendNotificationToPlayer(player, "~r~[ОШИБКА]: ~w~У вас нет доступа к данной команде!");
            return false;
        }

        public static string GetAdminRank(int level)
        {
            switch (level)
            {
                case 1: return "~g~Уровень 1 Админ~w~";
                case 5: return "~g~Уровень 5 Админ~w~";
                case 10: return "~g~Уровень 10 Админ~w~";
                default: return "";
            }
        }
    }
}
