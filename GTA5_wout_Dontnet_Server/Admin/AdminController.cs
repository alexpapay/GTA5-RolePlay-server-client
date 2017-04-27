using GTANetworkServer;
using TheGodfatherGM.Data;

namespace TheGodfatherGM.Server.Admin
{
    public class AdminController
    {
        
        public static bool AdminRankCheck(string cmd, Character character, Client player)
        {
            if (cmd == "makeadmin" && character.Admin > 4) return true;
            else if (cmd == "creategroup" && character.Admin > 4) return true;
            else if (cmd == "editgroup" && character.Admin > 4) return true;
            else if (cmd == "createproperty" && character.Admin > 4) return true;
            else if (cmd == "setproperty" && character.Admin > 4) return true;
            else if (cmd == "editjob" && character.Admin > 4) return true;
            else if (cmd == "setmoney" && character.Admin > 4) return true;
            else if (cmd == "createnpc" && character.Admin > 4) return true;
            else if (cmd == "givegun" && character.Admin > 1) return true;
            else if (cmd == "createvehicle" && character.Admin > 1) return true;
            else if (cmd == "goto" && character.Admin > 1) return true;
            else if (cmd == "switchgroup" && character.Admin > 1) return true;
            else if (cmd == "setskin" && character.Admin > 1) return true;
            else if (cmd == "sethealth" && character.Admin > 1) return true;
            else if (cmd == "setarmor" && character.Admin > 1) return true;


            API.shared.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~У вас нет доступа к данной команде!");
            return false;
        }

        public static string GetAdminRank(int Level)
        {
            switch (Level)
            {
                case 1: return "~g~Уровень 1 Админ~w~";
                case 2: return "~g~Уровень 2 Админ~w~";
                case 3: return "~g~Уровень 3 Админ~w~";
                case 4: return "~o~Старший Админ~w~";
                case 5: return "~r~Ведущий Админ~w~";
                case 6: return "~r~Управление~w~";
                case 9: return "~p~Root Доступ~w~";
                default: return "";
            }
        }
    }
}
