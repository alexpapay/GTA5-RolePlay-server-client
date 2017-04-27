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


            API.shared.sendChatMessageToPlayer(player, "~r~[������]: ~w~� ��� ��� ������� � ������ �������!");
            return false;
        }

        public static string GetAdminRank(int Level)
        {
            switch (Level)
            {
                case 1: return "~g~������� 1 �����~w~";
                case 2: return "~g~������� 2 �����~w~";
                case 3: return "~g~������� 3 �����~w~";
                case 4: return "~o~������� �����~w~";
                case 5: return "~r~������� �����~w~";
                case 6: return "~r~����������~w~";
                case 9: return "~p~Root ������~w~";
                default: return "";
            }
        }
    }
}
