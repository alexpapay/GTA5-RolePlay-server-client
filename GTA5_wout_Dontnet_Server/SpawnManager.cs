using GTANetworkServer;
using GTANetworkShared;
using TheGodfatherGM.Server.Characters;
using TheGodfatherGM.Server.DBManager;
using System.Linq;
using TheGodfatherGM.Data;

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

        public static void SetPlayerSkinClothesToDb(Client player, int type, Character character, int check)
        {
            if (character == null) return;

            ClothesTypes typeClothes = null;
            Clothes playerClothes = null;
            
            playerClothes = ContextFactory.Instance.Clothes.FirstOrDefault(x => x.CharacterId == character.Id);
            typeClothes = ContextFactory.Instance.ClothesTypes.FirstOrDefault(x => x.Type == type);

            playerClothes.MaskSlot = typeClothes.MaskSlot; playerClothes.MaskDraw = typeClothes.MaskDraw;
            playerClothes.TorsoSlot = typeClothes.TorsoSlot; playerClothes.TorsoDraw = typeClothes.TorsoDraw;
            playerClothes.LegsSlot = typeClothes.LegsSlot; playerClothes.LegsDraw = typeClothes.LegsDraw;
            playerClothes.BagsSlot = typeClothes.BagsSlot; playerClothes.BagsDraw = typeClothes.BagsDraw;
            playerClothes.FeetSlot = typeClothes.FeetSlot; playerClothes.FeetDraw = typeClothes.FeetDraw;
            playerClothes.AccessSlot = typeClothes.AccessSlot; playerClothes.AccessDraw = typeClothes.AccessDraw;
            playerClothes.UndershirtSlot = typeClothes.UndershirtSlot; playerClothes.UndershirtDraw = typeClothes.UndershirtDraw;
            playerClothes.ArmorSlot = typeClothes.ArmorSlot; playerClothes.ArmorDraw = typeClothes.ArmorDraw;
            playerClothes.TopsSlot = typeClothes.TopsSlot; playerClothes.TopsDraw = typeClothes.TopsDraw;
            playerClothes.HatsSlot = typeClothes.HatsSlot; playerClothes.HatsDraw = typeClothes.HatsDraw;
            playerClothes.GlassesSlot = typeClothes.GlassesSlot; playerClothes.GlassesDraw = typeClothes.GlassesDraw;

            ContextFactory.Instance.SaveChanges();
            SetPlayerSkinClothes(player, type, character, check);
        }
        public static void SetPlayerSkinClothes(Client player, int type, Character character, int check)
        {
            Clothes playerClothes = null;
            ClothesTypes playerTypeClothes = null;

            if (type == 0)
                playerClothes = ContextFactory.Instance.Clothes.FirstOrDefault(x => x.CharacterId == character.Id);
            else
                playerTypeClothes = ContextFactory.Instance.ClothesTypes.FirstOrDefault(x => x.Type == type);

            // First registration of character clothes
            if (check == 0)
            {
                ClothesTypes typeClothes = new ClothesTypes();
                playerClothes = new Clothes();
                typeClothes = ContextFactory.Instance.ClothesTypes.FirstOrDefault(x => x.Type == 101);
                playerClothes.CharacterId = character.Id;
                playerClothes.MaskSlot = typeClothes.MaskSlot;
                playerClothes.MaskDraw = typeClothes.MaskDraw;
                playerClothes.TorsoSlot = typeClothes.TorsoSlot;
                playerClothes.TorsoDraw = typeClothes.TorsoDraw;
                playerClothes.LegsSlot = typeClothes.LegsSlot;
                playerClothes.LegsDraw = typeClothes.LegsDraw;
                playerClothes.BagsSlot = typeClothes.BagsSlot;
                playerClothes.BagsDraw = typeClothes.BagsDraw;
                playerClothes.FeetSlot = typeClothes.FeetSlot;
                playerClothes.FeetDraw = typeClothes.FeetDraw;
                playerClothes.AccessSlot = typeClothes.AccessSlot;
                playerClothes.AccessDraw = typeClothes.AccessDraw;
                playerClothes.UndershirtSlot = typeClothes.UndershirtSlot;
                playerClothes.UndershirtDraw = typeClothes.UndershirtDraw;
                playerClothes.ArmorSlot = typeClothes.ArmorSlot;
                playerClothes.ArmorDraw = typeClothes.ArmorDraw;
                playerClothes.TopsSlot = typeClothes.TopsSlot;
                playerClothes.TopsDraw = typeClothes.TopsDraw;
                playerClothes.HatsSlot = typeClothes.HatsSlot;
                playerClothes.HatsDraw = typeClothes.HatsDraw;
                playerClothes.GlassesSlot = typeClothes.GlassesSlot;
                playerClothes.GlassesDraw = typeClothes.GlassesDraw;

                ContextFactory.Instance.Clothes.Add(playerClothes);
                ContextFactory.Instance.SaveChanges();

                API.shared.setPlayerSkin(player, (PedHash)character.Model); // TODO: delete after face custom
                API.shared.setPlayerClothes(player, 0, 0, 0);
                API.shared.setPlayerClothes(player, 1, playerClothes.MaskSlot, playerClothes.MaskDraw);
                API.shared.setPlayerClothes(player, 3, playerClothes.TorsoSlot, playerClothes.TorsoDraw);
                API.shared.setPlayerClothes(player, 4, playerClothes.LegsSlot, playerClothes.LegsDraw);
                API.shared.setPlayerClothes(player, 5, playerClothes.BagsSlot, playerClothes.BagsDraw);
                API.shared.setPlayerClothes(player, 6, playerClothes.FeetSlot, playerClothes.FeetDraw);
                API.shared.setPlayerClothes(player, 7, playerClothes.AccessSlot, playerClothes.AccessDraw);
                API.shared.setPlayerClothes(player, 8, playerClothes.UndershirtSlot, playerClothes.UndershirtDraw);
                API.shared.setPlayerClothes(player, 9, playerClothes.ArmorSlot, playerClothes.ArmorDraw);
                API.shared.setPlayerClothes(player, 11, playerClothes.TopsSlot, playerClothes.TopsDraw);
                API.shared.setPlayerAccessory(player, 0, playerClothes.HatsSlot, playerClothes.HatsDraw);
                API.shared.setPlayerAccessory(player, 1, playerClothes.GlassesSlot, playerClothes.GlassesDraw);

            }

            if (check == 1 && type == 0)
            {
                API.shared.setPlayerSkin(player, (PedHash)character.Model); // TODO: delete after face custom
                API.shared.setPlayerClothes(player, 1, playerClothes.MaskSlot, playerClothes.MaskDraw);
                API.shared.setPlayerClothes(player, 3, playerClothes.TorsoSlot, playerClothes.TorsoDraw);
                API.shared.setPlayerClothes(player, 4, playerClothes.LegsSlot, playerClothes.LegsDraw);
                API.shared.setPlayerClothes(player, 5, playerClothes.BagsSlot, playerClothes.BagsDraw);
                API.shared.setPlayerClothes(player, 6, playerClothes.FeetSlot, playerClothes.FeetDraw);
                API.shared.setPlayerClothes(player, 7, playerClothes.AccessSlot, playerClothes.AccessDraw);
                API.shared.setPlayerClothes(player, 8, playerClothes.UndershirtSlot, playerClothes.UndershirtDraw);
                API.shared.setPlayerClothes(player, 9, playerClothes.ArmorSlot, playerClothes.ArmorDraw);
                API.shared.setPlayerClothes(player, 11, playerClothes.TopsSlot, playerClothes.TopsDraw);
                API.shared.setPlayerAccessory(player, 0, playerClothes.HatsSlot, playerClothes.HatsDraw);
                API.shared.setPlayerAccessory(player, 1, playerClothes.GlassesSlot, playerClothes.GlassesDraw);
            }
            if (check == 1 && type != 0)
            {
                API.shared.setPlayerSkin(player, (PedHash)character.Model); // TODO: delete after face custom
                API.shared.setPlayerClothes(player, 1, playerTypeClothes.MaskSlot, playerTypeClothes.MaskDraw);
                API.shared.setPlayerClothes(player, 3, playerTypeClothes.TorsoSlot, playerTypeClothes.TorsoDraw);
                API.shared.setPlayerClothes(player, 4, playerTypeClothes.LegsSlot, playerTypeClothes.LegsDraw);
                API.shared.setPlayerClothes(player, 5, playerTypeClothes.BagsSlot, playerTypeClothes.BagsDraw);
                API.shared.setPlayerClothes(player, 6, playerTypeClothes.FeetSlot, playerTypeClothes.FeetDraw);
                API.shared.setPlayerClothes(player, 7, playerTypeClothes.AccessSlot, playerTypeClothes.AccessDraw);
                API.shared.setPlayerClothes(player, 8, playerTypeClothes.UndershirtSlot, playerTypeClothes.UndershirtDraw);
                API.shared.setPlayerClothes(player, 9, playerTypeClothes.ArmorSlot, playerTypeClothes.ArmorDraw);
                API.shared.setPlayerClothes(player, 11, playerTypeClothes.TopsSlot, playerTypeClothes.TopsDraw);
                API.shared.setPlayerAccessory(player, 0, playerTypeClothes.HatsSlot, playerTypeClothes.HatsDraw);
                API.shared.setPlayerAccessory(player, 1, playerTypeClothes.GlassesSlot, playerTypeClothes.GlassesDraw);
            }                      
        }
        // TODO: add another clothes for different gangs!
        public static int SetFractionClothes(Client player, int fractionID, Character character)
        {
            switch(fractionID)
            {
                case 2: SetPlayerSkinClothesToDb(player, 2, character, 1); return 2;
                case 1301: SetPlayerSkinClothesToDb(player, 131, character, 1); return 131;
                case 1401: SetPlayerSkinClothesToDb(player, 141, character, 1); return 141;
                case 1501: SetPlayerSkinClothesToDb(player, 151, character, 1); return 151;
                case 1601: SetPlayerSkinClothesToDb(player, 161, character, 1); return 161;
                case 1701: SetPlayerSkinClothesToDb(player, 171, character, 1); return 171;                
            }
            return 101; // Default clothes
        }
        public static void SpawnCharacter(Client player, CharacterController characterController)
        {
            API.shared.triggerClientEvent(player, "destroyCamera");            

            API.shared.resetPlayerNametagColor(player);
            //API.shared.removeAllPlayerWeapons(player);

            if (characterController.Character.RegistrationStep == 0)
            {
                SetPlayerSkinClothes(player, 0, characterController.Character, 0);
                API.shared.setEntityPosition(player, _newPlayerPosition);
                API.shared.setEntityRotation(player, _newPlayerRotation);
                characterController.Character.RegistrationStep = -1; // 'Tutorial Done'
                characterController.Character.ModelName = API.shared.getEntityModel(player).ToString();
            }
            else
            {
                SetPlayerSkinClothes(player, 0, characterController.Character, 1);
                API.shared.setEntityPosition(player, _newPlayerPosition);
                API.shared.setEntityRotation(player, _newPlayerRotation);
                characterController.Character.ModelName = API.shared.getEntityModel(player).ToString();
            }

            var userHouse = ContextFactory.Instance.Property.FirstOrDefault(x => x.CharacterId == characterController.Character.Id);
            if (userHouse != null)
            {
                API.shared.setEntityPosition(player, new Vector3(userHouse.ExtPosX, userHouse.ExtPosY, userHouse.ExtPosZ));
                API.shared.setEntityRotation(player, _newPlayerRotation);
            }    

            if (characterController.Character.ActiveGroupID == 2001 || characterController.Character.ActiveGroupID == 2002)
            {
                var placeArmyOne = ContextFactory.Instance.Property.FirstOrDefault(x => x.GroupId == 2000);
                if (placeArmyOne != null)
                {
                    API.shared.setEntityPosition(player, new Vector3(placeArmyOne.ExtPosX, placeArmyOne.ExtPosY, placeArmyOne.ExtPosZ));
                    API.shared.setEntityRotation(player, _newPlayerRotation);
                }
            }

            if (characterController.Character.ActiveGroupID == 2101 || characterController.Character.ActiveGroupID == 2102)
            {
                var placeArmyTwo = ContextFactory.Instance.Property.FirstOrDefault(x => x.GroupId == 2100);
                if (placeArmyTwo != null)
                {
                    API.shared.setEntityPosition(player, new Vector3(placeArmyTwo.ExtPosX, placeArmyTwo.ExtPosY, placeArmyTwo.ExtPosZ));
                    API.shared.setEntityRotation(player, _newPlayerRotation);
                }                
            }

            ContextFactory.Instance.SaveChanges();
        }

        public static Vector3 GetSpawnPosition() { return _newPlayerPosition; }
        public static int GetSpawnDimension() { return _newPlayerDimension; }
    }
}