using GTANetworkServer;
using GTANetworkShared;
using TheGodfatherGM.Data;
using TheGodfatherGM.Server.Characters;
using TheGodfatherGM.Server.DBManager;
using TheGodfatherGM.Data.Localize;
using System.Linq;


namespace TheGodfatherGM.Server
{
    class WeaponManager : Script
    {
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

                if (characterWeapons.Revolver != 0) GiveWeapon(player, 1, characterWeapons.RevolverPt);
                API.shared.setPlayerWeaponTint(player, WeaponHash.Revolver, weaponTint);
                if (characterWeapons.CarbineRifle != 0) GiveWeapon(player, 2, characterWeapons.CarbineRiflePt);
                API.shared.setPlayerWeaponTint(player, WeaponHash.CarbineRifle, weaponTint);
                if (characterWeapons.SniperRifle != 0) GiveWeapon(player, 3, characterWeapons.SniperRiflePt);
                API.shared.setPlayerWeaponTint(player, WeaponHash.SniperRifle, weaponTint);
                if (characterWeapons.SmokeGrenade != 0) GiveWeapon(player, 4, characterWeapons.SmokeGrenadePt);
                API.shared.setPlayerWeaponTint(player, WeaponHash.SmokeGrenade, weaponTint);
                if (characterWeapons.FlareGun != 0) GiveWeapon(player, 5, characterWeapons.FlareGunPt);
                API.shared.setPlayerWeaponTint(player, WeaponHash.FlareGun, weaponTint);
                if (characterWeapons.CompactRifle != 0) GiveWeapon(player, 6, characterWeapons.CompactRiflePt);
                API.shared.setPlayerWeaponTint(player, WeaponHash.CompactRifle, weaponTint);
                if (characterWeapons.PumpShotgun != 0) GiveWeapon(player, 7, characterWeapons.PumpShotgunPt);
                API.shared.setPlayerWeaponTint(player, WeaponHash.PumpShotgun, weaponTint);
                if (characterWeapons.BZGas != 0) GiveWeapon(player, 8, characterWeapons.BZGasPt);
                API.shared.setPlayerWeaponTint(player, WeaponHash.BZGas, weaponTint);
                if (characterWeapons.Nightstick != 0) GiveWeapon(player, 9, 1);
                API.shared.setPlayerWeaponTint(player, WeaponHash.Nightstick, weaponTint);
                if (characterWeapons.StunGun != 0) GiveWeapon(player, 10, characterWeapons.StunGunPt);
                API.shared.setPlayerWeaponTint(player, WeaponHash.StunGun, weaponTint);
                if (characterWeapons.HeavyPistol != 0) GiveWeapon(player, 11, characterWeapons.HeavyPistolPt);
                API.shared.setPlayerWeaponTint(player, WeaponHash.HeavyPistol, weaponTint);
                if (characterWeapons.BullpupRifle != 0) GiveWeapon(player, 12, characterWeapons.BullpupRiflePt);
                API.shared.setPlayerWeaponTint(player, WeaponHash.BullpupRifle, weaponTint);
                if (characterWeapons.HeavyShotgun != 0) GiveWeapon(player, 13, characterWeapons.HeavyShotgunPt);
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
            // After players disconnect (bullet and weapons saving)
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

        public static void GiveWeapon(Client player, int type, int bullets)
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
        public static WeaponHash GetWeaponHash(string weaponName)
        {
            switch(weaponName)
            {
                case "Revolver": return WeaponHash.Revolver;
                case "CarbineRifle": return WeaponHash.CarbineRifle;
                case "SniperRifle": return WeaponHash.SniperRifle;
                case "SmokeGrenade": return WeaponHash.SmokeGrenade;
                case "FlareGun": return WeaponHash.FlareGun;
                case "CompactRifle": return WeaponHash.CompactRifle;
                case "PumpShotgun": return WeaponHash.PumpShotgun;
                case "BZGas": return WeaponHash.BZGas;
                case "Nightstick": return WeaponHash.Nightstick;
                case "StunGun": return WeaponHash.StunGun;
                case "HeavyPistol": return WeaponHash.HeavyPistol;
                case "BullpupRifle": return WeaponHash.BullpupRifle;
                case "HeavyShotgun": return WeaponHash.HeavyShotgun;
            }            
            return WeaponHash.Unarmed;
        }

        public static void BuySellWeapon(
            Client buyer, Client seller, 
            Weapon sellCharacter, Weapon buyCharacter, 
            string weapon, int ammo,
            Character initCharacter, Character targetCharacter)
        {
            switch (weapon)
            {
                case "Revolver": sellCharacter.Revolver = 0; buyCharacter.Revolver = 1;
                    sellCharacter.RevolverPt = 0; buyCharacter.RevolverPt = ammo; break;
                case "CarbineRifle": sellCharacter.CarbineRifle = 0; buyCharacter.CarbineRifle = 1;
                    sellCharacter.CarbineRiflePt = 0; buyCharacter.CarbineRiflePt = ammo; break;
                case "SniperRifle": sellCharacter.SniperRifle = 0; buyCharacter.SniperRifle = 1;
                    sellCharacter.SniperRiflePt = 0; buyCharacter.SniperRiflePt = ammo; break;
                case "SmokeGrenade": sellCharacter.SmokeGrenade = 0; buyCharacter.SmokeGrenade = 1;
                    sellCharacter.SmokeGrenadePt = 0; buyCharacter.SmokeGrenadePt = ammo; break;
                case "FlareGun": sellCharacter.FlareGun = 0; buyCharacter.FlareGun = 1;
                    sellCharacter.FlareGunPt = 0; buyCharacter.FlareGunPt = ammo; break;
                case "CompactRifle": sellCharacter.CompactRifle = 0; buyCharacter.CompactRifle = 1;
                    sellCharacter.CompactRiflePt = 0; buyCharacter.CompactRiflePt = ammo; break;
                case "PumpShotgun": sellCharacter.PumpShotgun = 0; buyCharacter.PumpShotgun = 1;
                    sellCharacter.PumpShotgunPt = 0; buyCharacter.PumpShotgunPt = ammo; break;
                case "StunGun": sellCharacter.StunGun = 0; buyCharacter.StunGun = 1;
                    sellCharacter.StunGunPt = 0; buyCharacter.StunGunPt = ammo; break;
                case "HeavyPistol": sellCharacter.HeavyPistol = 0; buyCharacter.HeavyPistol = 1;
                    sellCharacter.HeavyPistolPt = 0; buyCharacter.HeavyPistolPt = ammo; break;
                case "BullpupRifle": sellCharacter.BullpupRifle = 0; buyCharacter.BullpupRifle = 1;
                    sellCharacter.BullpupRiflePt = 0; buyCharacter.BullpupRiflePt = ammo; break;
                case "HeavyShotgun": sellCharacter.HeavyShotgun = 0; buyCharacter.HeavyShotgun = 1;
                    sellCharacter.HeavyShotgunPt = 0; buyCharacter.HeavyShotgunPt = ammo; break;
            }            
            ContextFactory.Instance.SaveChanges();

            API.shared.removePlayerWeapon(seller, GetWeaponHash(weapon));
            API.shared.givePlayerWeapon(buyer, GetWeaponHash(weapon), ammo, false, true);
            API.shared.sendNotificationToPlayer(buyer, Localize.Lang(initCharacter.Language, "you_bought_weapon") + weapon.ToString());
            API.shared.sendNotificationToPlayer(seller, Localize.Lang(targetCharacter.Language, "you_sold_weapon") + weapon.ToString());
        }
    }
}
