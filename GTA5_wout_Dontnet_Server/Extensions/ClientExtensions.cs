using GTANetworkServer;
using TheGodfatherGM.Server.Characters;

namespace TheGodfatherGM.Server.Extensions
{
    public static class ClientExtensions
    {
        public static CharacterController GetCharacterController(this Client client)
        {
            return client.getData("CHARACTER") as CharacterController;
        }
    }
}
