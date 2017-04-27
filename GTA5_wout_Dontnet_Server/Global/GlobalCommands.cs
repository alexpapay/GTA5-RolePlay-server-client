using GTANetworkServer;
using TheGodfatherGM.Server.Characters;

namespace TheGodfatherGM.Server.Global
{
    public class GlobalCommands : Script
    {

        [Command("id", GreedyArg = true)]
        public void id(Client player, string IDName)
        {
            CharacterController account = EntityManager.GetUserAccount(player, IDName);
            if (account == null)
            {
                EntityManager.ListUserAccounts(player, IDName);
            }
            else
            {
                API.sendChatMessageToPlayer(player, "" + account.FormatName + " (ID: " + account.Character.Id + ") - (Level: " + account.Character.Level + ") - (Ping: " + API.getPlayerPing(player) + ")");
            }
        }
    }
}