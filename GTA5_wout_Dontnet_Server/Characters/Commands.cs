using GTANetworkServer;
using GTANetworkShared;
using System;

namespace TheGodfatherGM.Server.Characters
{
    public class Commands : Script
    {
        [Command("voice")]
        public void voice(Client player)
        {
            Global.CEFController.OpenVoice(player);
        }

        // Команда дать оружие Миниган
        [Command("gw")]
        public void GiveWeapon(Client player)
        {
            API.givePlayerWeapon(player, WeaponHash.Minigun, 500, true, true);
        }

        [Command("createcharacter")]
        public static void CreateCharacter(Client player, string name, string pwd, int language)
        {
            if (!CharacterController.NameValidityCheck(player, name))
            {
                // TODO: CharacterController.CreateCharacter(player);
                return;
            }

            bool SuccessID = DatabaseManager.RegisterCharacter(player, name, pwd, language);
            if (SuccessID)
            {
                API.shared.sendChatMessageToPlayer(player, "~g~Server: ~w~Character created!");
                //CharacterController.CharacterMenu(player);
            }
            else
            {
                API.shared.sendChatMessageToPlayer(player, "We couldn't create this character.");
                // TODO: CharacterController.CreateCharacter(player);
            }
        }

        [Command("gettime")]
        public void GetTimeCommand(Client player)
        {
            player.sendChatMessage("Time: " + API.getTime());
        }

        [Command("logout")]
        public void LogOut(Client player)
        {
            CharacterController characterController = player.getData("CHARACTER");
            API.shared.sendNotificationToPlayer(player, "~y~Server: ~w~You will be logged out in 5 seconds.", true);
            Global.Util.delay(5000, () =>
            {
                characterController.Character.LastLogoutDate = DateTime.Now;
                ConnectionController.LogOut(player, characterController.Character, 1);
            });
        }

        [Command("stats", Group = "Global Commands")]
        public void GetStatistics(Client player)
        {
            CharacterController characterController = player.getData("CHARACTER");
            if (characterController == null) return;

            API.sendChatMessageToPlayer(player, "___________ STATS __________");
            API.sendChatMessageToPlayer(player, string.Format("~h~Name:~h~ {0} ~h~Level:~h~ {1} ~h~Job:~h~ {2}\n",
                characterController.FormatName,
                characterController.Character.Level,
                (characterController.job == null ? "None" : characterController.job.Type())) +
                characterController.ListGroups());
        }
    }
}
