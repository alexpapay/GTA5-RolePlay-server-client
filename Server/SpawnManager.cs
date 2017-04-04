using GTANetworkServer;
using GTANetworkShared;
using TheGodfatherGM.Server.Characters;
using TheGodfatherGM.Server.DBManager;
using TheGodfatherGM.Server.Property;
using TheGodfatherGM.Data;
using TheGodfatherGM.Data.Extensions;
using TheGodfatherGM.Data.Attributes;
using TheGodfatherGM.Server.User;
using System.Linq;

namespace TheGodfatherGM.Server
{
    public class SpawnManager : Script
    {
        private static readonly Vector3 _newPlayerPosition = new Vector3(-1034.794, -2727.422, 13.75663); //
        private static readonly Vector3 _newPlayerRotation = new Vector3(0.0, 0.0, -34.4588);
        private static readonly int _newPlayerDimension = 0;

        public SpawnManager()
        {
        }

        public static void SpawnCharacter(CharacterController character)
        {
            API.shared.triggerClientEvent(character.AccountController.Client, "destroyCamera");

            Client target = character.AccountController.Client;

            API.shared.setPlayerSkin(character.AccountController.Client, (PedHash)character.Character.Model);

            API.shared.resetPlayerNametagColor(target);
            API.shared.removeAllPlayerWeapons(target);

            if (character.Character.RegistrationStep == 0)
            {
                API.shared.setEntityPosition(target, _newPlayerPosition);
                API.shared.setEntityRotation(target, _newPlayerRotation);
                character.Character.RegistrationStep = -1; // 'Tutorial Done'
                character.Character.ModelName = API.shared.getEntityModel(character.AccountController.Client).ToString();
            }
            else
            {
                API.shared.setEntityPosition(target, _newPlayerPosition);
                API.shared.setEntityRotation(target, _newPlayerRotation);
                //API.shared.setEntityPosition(character.AccountController.Client, new Vector3(character.Character.PosX, character.Character.PosY, character.Character.PosZ));
                //API.shared.setEntityRotation(character.AccountController.Client, new Vector3(0.0f, 0.0f, character.Character.Rot));
                character.Character.ModelName = API.shared.getEntityModel(character.AccountController.Client).ToString();
            }

            var userHouse = ContextFactory.Instance.Property.FirstOrDefault(x => x.CharacterId == character.Character.Id);
            if (userHouse != null)
            {
                API.shared.setEntityPosition(target, new Vector3(userHouse.ExtPosX, userHouse.ExtPosY, userHouse.ExtPosZ));
                API.shared.setEntityRotation(target, _newPlayerRotation);
            }

            ContextFactory.Instance.SaveChanges();
        }

        public static Vector3 GetSpawnPosition() { return _newPlayerPosition; }
        public static int GetSpawnDimension() { return _newPlayerDimension; }
    }
}