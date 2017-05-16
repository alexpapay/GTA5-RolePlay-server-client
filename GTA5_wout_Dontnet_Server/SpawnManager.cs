using System.Linq;

using GTANetworkServer;
using GTANetworkShared;

using TheGodfatherGM.Data;
using TheGodfatherGM.Server.Characters;
using TheGodfatherGM.Server.DBManager;

namespace TheGodfatherGM.Server
{
    public class SpawnManager : Script
    {
        private static readonly Vector3 _newPlayerPosition = new Vector3(-1034.794, -2727.422, 13.75663); //
        private static readonly Vector3 _newPlayerRotation = new Vector3(0.0, 0.0, -34.4588);
        private static readonly Vector3 _firstPlayerPosition = new Vector3(-1042.2, -2772.6, 4.639); //
        private static readonly Vector3 _firstPlayerRotation = new Vector3(0.0, 0.0, 58.7041);
        private static readonly int _newPlayerDimension = 0;

        public SpawnManager()
        {
        }

        public static void SpawnCharacter(Client player, CharacterController characterController)
        {
            API.shared.triggerClientEvent(player, "destroyCamera"); 
            API.shared.resetPlayerNametagColor(player);

            if (characterController.Character.RegistrationStep == 0)
            {
                SetCharacterFace (player, characterController.Character);
                ClothesManager.SetPlayerSkinClothes(player, 0, characterController.Character, 0);
                WeaponManager.SetPlayerWeapon(player, characterController.Character, 0);
                API.shared.setEntityPosition(player, _firstPlayerPosition);
                API.shared.setEntityRotation(player, _firstPlayerRotation);
                characterController.Character.RegistrationStep = -1; // 'Tutorial Done'                               
            }
            else
            {
                SetCharacterFace(player, characterController.Character);
                ClothesManager.SetPlayerSkinClothes(player, 0, characterController.Character, 1);
                WeaponManager.SetPlayerWeapon(player, characterController.Character, 1);
                API.shared.setEntityPosition(player, _newPlayerPosition);
                API.shared.setEntityRotation(player, _newPlayerRotation);                
            }
            
            if (CharacterController.IsCharacterArmySoldier(characterController))
            {                
                Data.Property placeArmy = new Data.Property();

                if (characterController.Character.ActiveGroupID < 2003)
                     placeArmy = ContextFactory.Instance.Property.FirstOrDefault(x => x.GroupId == 2000);
                else placeArmy = ContextFactory.Instance.Property.FirstOrDefault(x => x.GroupId == 2100);

                if (placeArmy != null)
                {
                    API.shared.setEntityPosition(player, new Vector3(placeArmy.ExtPosX, placeArmy.ExtPosY, placeArmy.ExtPosZ));
                    API.shared.setEntityRotation(player, _newPlayerRotation);
                }      
            }
            if (CharacterController.IsCharacterInGang(characterController))
            {
                Data.Property placeGangs = new Data.Property();

                if (characterController.Character.ActiveGroupID > 1300 &&
                    characterController.Character.ActiveGroupID <= 1310)
                    placeGangs = ContextFactory.Instance.Property.FirstOrDefault(x => x.GroupId == 1300);
                if (characterController.Character.ActiveGroupID > 1400 &&
                    characterController.Character.ActiveGroupID <= 1410)
                    placeGangs = ContextFactory.Instance.Property.FirstOrDefault(x => x.GroupId == 1400);
                if (characterController.Character.ActiveGroupID > 1500 &&
                    characterController.Character.ActiveGroupID <= 1510)
                    placeGangs = ContextFactory.Instance.Property.FirstOrDefault(x => x.GroupId == 1500);
                if (characterController.Character.ActiveGroupID > 1600 &&
                    characterController.Character.ActiveGroupID <= 1610)
                    placeGangs = ContextFactory.Instance.Property.FirstOrDefault(x => x.GroupId == 1600);
                if (characterController.Character.ActiveGroupID > 1700 &&
                    characterController.Character.ActiveGroupID <= 1710)
                    placeGangs = ContextFactory.Instance.Property.FirstOrDefault(x => x.GroupId == 1700);

                if (placeGangs != null)
                {
                    API.shared.setEntityPosition(player, new Vector3(placeGangs.ExtPosX, placeGangs.ExtPosY, placeGangs.ExtPosZ));
                    API.shared.setEntityRotation(player, _newPlayerRotation);
                }
            }
            var userHouse = ContextFactory.Instance.Property.FirstOrDefault(x => x.CharacterId == characterController.Character.Id);
            if (userHouse != null)
            {
                API.shared.setEntityPosition(player, new Vector3(userHouse.ExtPosX, userHouse.ExtPosY, userHouse.ExtPosZ));
                API.shared.setEntityRotation(player, _newPlayerRotation);
            }
            ContextFactory.Instance.SaveChanges();
        }

        public static void SetCharacterFace (Client player, Character character)
        {
            var face = ContextFactory.Instance.Faces.First(x => x.CharacterId == character.Id);
            var pedHash = face.SEX == 1885233650 ? PedHash.FreemodeMale01 : PedHash.FreemodeFemale01;

            API.shared.setPlayerSkin(player, pedHash);

            CharacterController.InitializePedFace(player.handle);
            //API.shared.exported.gtaocharacter.initializePedFace(player.handle);

            API.shared.setEntitySyncedData(player, "GTAO_SHAPE_FIRST_ID", face.GTAO_SHAPE_FIRST_ID);
            API.shared.setEntitySyncedData(player, "GTAO_SHAPE_SECOND_ID", face.GTAO_SHAPE_SECOND_ID);
            API.shared.setEntitySyncedData(player, "GTAO_SKIN_FIRST_ID", face.GTAO_SKIN_FIRST_ID);
            API.shared.setPlayerClothes(player, 2, face.GTAO_HAIR, 0);
            API.shared.setEntitySyncedData(player, "GTAO_HAIR_COLOR", face.GTAO_HAIR_COLOR);
            API.shared.setEntitySyncedData(player, "GTAO_EYE_COLOR", face.GTAO_SHAPE_FIRST_ID);
            API.shared.setEntitySyncedData(player, "GTAO_EYEBROWS", face.GTAO_EYEBROWS);
            API.shared.setEntitySyncedData(player, "GTAO_EYEBROWS_COLOR", face.GTAO_EYEBROWS_COLOR);

            CharacterController.UpdatePlayerFace(player.handle);
            //API.shared.exported.gtaocharacter.updatePlayerFace(player.handle);
        }

        public static Vector3 GetSpawnPosition() { return _newPlayerPosition; }
        public static int GetSpawnDimension() { return _newPlayerDimension; }
    }
}