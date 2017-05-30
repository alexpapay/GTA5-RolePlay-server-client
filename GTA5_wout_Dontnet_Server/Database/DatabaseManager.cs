using GTANetworkServer;
using System.Linq;
using TheGodfatherGM.Server.Characters;
using TheGodfatherGM.Server.DBManager;

namespace TheGodfatherGM.Server
{
    public class DatabaseManager : Script
    {
        public DatabaseManager()
        {
            API.onResourceStart += OnResourceStart;
        }

        private void OnResourceStart()
        {
            ContextFactory.SetConnectionParameters(API.getSetting<string>("database_server"), API.getSetting<string>("database_user"), API.getSetting<string>("database_password"), API.getSetting<string>("database_database"));
            EntityManager.Init();
        }
        public static bool DoesCharacterExist(string name)
        {
            return ContextFactory.Instance.Character.FirstOrDefault(x => x.Name == name) != null;
        }
        public static bool RegisterCharacter(Client player, string name, string pwd, int laguage)
        {
            if (DoesCharacterExist(name)) return false;
            new CharacterController(player, name, pwd, laguage);
            return true;
        }
    }
}