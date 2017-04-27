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
        public static void SetPlayerWeapon(Client player, Character character, int check)
        {
            // First registration of character clothes
            if (check == 0)
            {
                Weapon weapon = new Weapon();
                weapon.CharacterId = character.Id;
                ContextFactory.Instance.Weapon.Add(weapon);
                ContextFactory.Instance.SaveChanges();
            }            

            var characterWeapons = ContextFactory.Instance.Weapon.First(x => x.CharacterId == character.Id);

            // Respawn with weapons from DB
            if (check == 1)
            {
                WeaponTint weaponTint = new WeaponTint();
                if (CharacterController.IsCharacterInArmy(character)) weaponTint = WeaponTint.Army;
                if (CharacterController.IsCharacterInFBI(character) ||
                    CharacterController.IsCharacterInPolice(character)) weaponTint = WeaponTint.LSPD;
                if (CharacterController.IsCharacterInGang(character)) weaponTint = WeaponTint.Gold;

                if (characterWeapons.Revolver != 0) GetWeapon(player, 1, characterWeapons.RevolverPt);
                API.shared.setPlayerWeaponTint(player, WeaponHash.Revolver, weaponTint);
                if (characterWeapons.CarbineRifle != 0) GetWeapon(player, 2, characterWeapons.CarbineRiflePt);
                API.shared.setPlayerWeaponTint(player, WeaponHash.CarbineRifle, weaponTint);
                if (characterWeapons.SniperRifle != 0) GetWeapon(player, 3, characterWeapons.SniperRiflePt);
                API.shared.setPlayerWeaponTint(player, WeaponHash.SniperRifle, weaponTint);
                if (characterWeapons.SmokeGrenade != 0) GetWeapon(player, 4, characterWeapons.SmokeGrenadePt);
                API.shared.setPlayerWeaponTint(player, WeaponHash.SmokeGrenade, weaponTint);
                if (characterWeapons.FlareGun != 0) GetWeapon(player, 5, characterWeapons.FlareGunPt);
                API.shared.setPlayerWeaponTint(player, WeaponHash.FlareGun, weaponTint);
                if (characterWeapons.CompactRifle != 0) GetWeapon(player, 6, characterWeapons.CompactRiflePt);
                API.shared.setPlayerWeaponTint(player, WeaponHash.CompactRifle, weaponTint);
                if (characterWeapons.PumpShotgun != 0) GetWeapon(player, 7, characterWeapons.PumpShotgunPt);
                API.shared.setPlayerWeaponTint(player, WeaponHash.PumpShotgun, weaponTint);
                if (characterWeapons.BZGas != 0) GetWeapon(player, 8, characterWeapons.BZGasPt);
                API.shared.setPlayerWeaponTint(player, WeaponHash.BZGas, weaponTint);
                if (characterWeapons.Nightstick != 0) GetWeapon(player, 9, 1);
                API.shared.setPlayerWeaponTint(player, WeaponHash.Nightstick, weaponTint);
                if (characterWeapons.StunGun != 0) GetWeapon(player, 10, characterWeapons.StunGunPt);
                API.shared.setPlayerWeaponTint(player, WeaponHash.StunGun, weaponTint);
                if (characterWeapons.HeavyPistol != 0) GetWeapon(player, 11, characterWeapons.HeavyPistolPt);
                API.shared.setPlayerWeaponTint(player, WeaponHash.HeavyPistol, weaponTint);
                if (characterWeapons.BullpupRifle != 0) GetWeapon(player, 12, characterWeapons.BullpupRiflePt);
                API.shared.setPlayerWeaponTint(player, WeaponHash.BullpupRifle, weaponTint);
                if (characterWeapons.HeavyShotgun != 0) GetWeapon(player, 13, characterWeapons.HeavyShotgunPt);
                API.shared.setPlayerWeaponTint(player, WeaponHash.HeavyShotgun, weaponTint);
            }
            // After players death
            if (check == 2)
            {
                characterWeapons.Revolver = 0; characterWeapons.RevolverPt = 0;
                characterWeapons.CarbineRifle = 0; characterWeapons.CarbineRiflePt = 0;
                characterWeapons.SniperRifle = 0; characterWeapons.SniperRiflePt = 0;
                characterWeapons.SmokeGrenade = 0; characterWeapons.SmokeGrenadePt = 0;
                characterWeapons.FlareGun = 0; characterWeapons.FlareGunPt = 0;
                characterWeapons.CompactRifle = 0; characterWeapons.CompactRiflePt = 0;
                characterWeapons.PumpShotgun = 0; characterWeapons.PumpShotgunPt = 0;
                characterWeapons.BZGas = 0; characterWeapons.BZGasPt = 0;
                characterWeapons.Nightstick = 0;
                characterWeapons.StunGun = 0; characterWeapons.StunGunPt = 0;
                characterWeapons.HeavyPistol = 0; characterWeapons.HeavyPistolPt = 0;
                characterWeapons.BullpupRifle = 0; characterWeapons.BullpupRiflePt = 0;
                characterWeapons.HeavyShotgun = 0; characterWeapons.HeavyShotgunPt = 0;                
            }
            // After players disconnect (bullet saving)
            if (check == 3)
            {
                characterWeapons.RevolverPt = API.shared.getPlayerWeaponAmmo(player, WeaponHash.Revolver);
                characterWeapons.CarbineRiflePt = API.shared.getPlayerWeaponAmmo(player, WeaponHash.CarbineRifle);
                characterWeapons.SniperRiflePt = API.shared.getPlayerWeaponAmmo(player, WeaponHash.SniperRifle);
                characterWeapons.SmokeGrenadePt = API.shared.getPlayerWeaponAmmo(player, WeaponHash.SmokeGrenade);
                characterWeapons.FlareGunPt = API.shared.getPlayerWeaponAmmo(player, WeaponHash.FlareGun);
                characterWeapons.CompactRiflePt = API.shared.getPlayerWeaponAmmo(player, WeaponHash.CompactRifle);
                characterWeapons.PumpShotgunPt = API.shared.getPlayerWeaponAmmo(player, WeaponHash.PumpShotgun);
                characterWeapons.BZGasPt = API.shared.getPlayerWeaponAmmo(player, WeaponHash.BZGas);
                characterWeapons.StunGunPt = API.shared.getPlayerWeaponAmmo(player, WeaponHash.StunGun);
                characterWeapons.HeavyPistolPt = API.shared.getPlayerWeaponAmmo(player, WeaponHash.HeavyPistol);
                characterWeapons.BullpupRiflePt = API.shared.getPlayerWeaponAmmo(player, WeaponHash.BullpupRifle);
                characterWeapons.HeavyShotgunPt = API.shared.getPlayerWeaponAmmo(player, WeaponHash.HeavyShotgun);
            }
            ContextFactory.Instance.SaveChanges();
        }
        public static void GetWeapon (Client player, int type, int bullets)
        {
            switch (type)
            {
                case 1: API.shared.givePlayerWeapon(player, WeaponHash.Revolver, bullets, true, true); break;
                case 2: API.shared.givePlayerWeapon(player, WeaponHash.CarbineRifle, bullets, true, true); break;
                case 3: API.shared.givePlayerWeapon(player, WeaponHash.SniperRifle, bullets, true, true); break;
                case 4: API.shared.givePlayerWeapon(player, WeaponHash.SmokeGrenade, bullets, true, true); break;
                case 5: API.shared.givePlayerWeapon(player, WeaponHash.FlareGun, bullets, true, true); break;
                case 6: API.shared.givePlayerWeapon(player, WeaponHash.CompactRifle, bullets, true, true); break;
                case 7: API.shared.givePlayerWeapon(player, WeaponHash.PumpShotgun, bullets, true, true); break;
                case 8: API.shared.givePlayerWeapon(player, WeaponHash.BZGas, bullets, true, true); break;
                case 9: API.shared.givePlayerWeapon(player, WeaponHash.Nightstick, 1, true, true); break;
                case 10: API.shared.givePlayerWeapon(player, WeaponHash.StunGun, bullets, true, true); break;
                case 11: API.shared.givePlayerWeapon(player, WeaponHash.HeavyPistol, bullets, true, true); break;
                case 12: API.shared.givePlayerWeapon(player, WeaponHash.BullpupRifle, bullets, true, true); break;
                case 13: API.shared.givePlayerWeapon(player, WeaponHash.HeavyShotgun, bullets, true, true); break;
            }
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
                SetPlayerWeapon(player, characterController.Character, 0);
                API.shared.setEntityPosition(player, _newPlayerPosition);
                API.shared.setEntityRotation(player, _newPlayerRotation);
                characterController.Character.RegistrationStep = -1; // 'Tutorial Done'
                characterController.Character.ModelName = API.shared.getEntityModel(player).ToString();
            }
            else
            {
                SetPlayerSkinClothes(player, 0, characterController.Character, 1);
                SetPlayerWeapon(player, characterController.Character, 1);
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
            ContextFactory.Instance.SaveChanges();
        }

        public static Vector3 GetSpawnPosition() { return _newPlayerPosition; }
        public static int GetSpawnDimension() { return _newPlayerDimension; }
    }
}