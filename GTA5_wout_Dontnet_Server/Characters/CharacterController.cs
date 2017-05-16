using System;
using System.Linq;
using System.Text.RegularExpressions;

using GTANetworkServer;
using GTANetworkShared;

using TheGodfatherGM.Data;
using TheGodfatherGM.Server.Jobs;
using TheGodfatherGM.Server.DBManager;

namespace TheGodfatherGM.Server.Characters
{
    public class CharacterController
    {
        public Character Character = new Character();

        public string FormatName { get; private set; }
        public JobController job;
        public GroupMember ActiveGroup = new GroupMember();

        public CharacterController(Client player, Character characterData)
        {
            if (characterData != null)
            {
                Character = characterData;
                FormatName = Character.Name.Replace("_", " ");

                player.setData("CHARACTER", this);

                //LoadProperties(player);
                switch (Character.RegistrationStep)
                {
                    case 0:
                        API.shared.sendChatMessageToPlayer(player, string.Format("~w~Добро пожаловать {0} \nна ~g~{1}~w~. \nЭто Ваш первый визит. Наслаждайтесь игрой!", FormatName, Global.GlobalVars.ServerName));
                        break;
                    default:
                        API.shared.sendChatMessageToPlayer(player, string.Format("~w~Добро пожаловать {0}! \nВаше последнее подключение было: {1}", FormatName, Character.LastLoginDate));
                        break;
                }

                Character.LastLoginDate = DateTime.Now;
                Character.Online = true;

                // Dynamic ID
                var characters = ContextFactory.Instance.Character.
                Where(x => x.OID != 0)
                .OrderBy(x => x.OID);
                Character firstCharacter = new Character();

                if (characters == null) Character.OID = 1; // Alone on the server
                else
                {
                    if (characters.Count() > 2)
                    {
                        firstCharacter = characters.First();
                        foreach (var character in characters)
                        {
                            if (character.OID != firstCharacter.OID)
                            {
                                if (character.OID - firstCharacter.OID != 1)
                                {
                                    Character.OID = firstCharacter.OID + 1;
                                    goto OIDok;
                                }
                                else firstCharacter.OID = character.OID;
                            }
                        }
                        Character.OID = characters.Count() + 1;
                    }
                    else Character.OID = characters.Count() + 1; // Second player login
                }
                OIDok: ContextFactory.Instance.SaveChanges();
            }
            else
            {
                API.shared.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: characterData is null");
                return;
            }
        }

        public CharacterController(Client player, string name, string pwd, int language)
        {
            Character.AccountId = pwd;
            Character.GroupType = 0;
            Character.ActiveGroupID = 1;
            Character.Admin = 0;
            Character.Bank = 0;
            Character.Cash = 300;
            Character.JobId = 0;
            Character.Level = 0;
            Character.Model = PedHash.FreemodeMale01.GetHashCode();
            Character.ModelName = "FreemodeMale01";
            Character.Name = name;
            Character.Online = false;
            Character.PosX = -1034.794f;
            Character.PosY = -2727.422f;
            Character.PosZ = 13.75663f;
            Character.RegisterDate = DateTime.Now;
            Character.PlayMinutes = 0;
            Character.DriverLicense = 0;
            Character.TempVar = 0;
            Character.Material = 0;
            Character.SocialClub = player.socialClubName;
            Character.ClothesTypes = 0;
            Character.ActiveClothes = 101;
            Character.Language = language;
            ContextFactory.Instance.Character.Add(Character);
            ContextFactory.Instance.SaveChanges();
            API.shared.setPlayerSkin(player, PedHash.FreemodeMale01);

            InitializePedFace(player.handle);
            //API.shared.exported.gtaocharacter.initializePedFace(player.handle);

            API.shared.triggerClientEvent(player, "face_custom");
        }

        public void Save(Client player)
        {
            Character.PosX = player.position.X;
            Character.PosY = player.position.Y;
            Character.PosZ = player.position.Z;
            Character.Rot = player.rotation.Z;
            Character.Model = player.model.GetHashCode();
        }
        private void OnPlayerDeathHandler(Client player, NetHandle entityKiller, int weapon)
        {

            API.shared.sendNativeToPlayer(player, Hash._RESET_LOCALPLAYER_STATE, player);
            API.shared.sendNativeToPlayer(player, Hash.RESET_PLAYER_ARREST_STATE, player);

            API.shared.sendNativeToPlayer(player, Hash.IGNORE_NEXT_RESTART, true);
            API.shared.sendNativeToPlayer(player, Hash._DISABLE_AUTOMATIC_RESPAWN, true);

            API.shared.sendNativeToPlayer(player, Hash.SET_FADE_IN_AFTER_DEATH_ARREST, true);
            API.shared.sendNativeToPlayer(player, Hash.SET_FADE_OUT_AFTER_DEATH, false);
            API.shared.sendNativeToPlayer(player, Hash.NETWORK_REQUEST_CONTROL_OF_ENTITY, player);

            API.shared.sendNativeToPlayer(player, Hash.FREEZE_ENTITY_POSITION, player, false);
            API.shared.sendNativeToPlayer(player, Hash.NETWORK_RESURRECT_LOCAL_PLAYER, player.position.X, player.position.Y, player.position.Z, player.rotation.Z, false, false);
            API.shared.sendNativeToPlayer(player, Hash.RESURRECT_PED, player);

            API.shared.sendNativeToPlayer(player, Hash.SET_PED_TO_RAGDOLL, player, true);
        }

        public static void CreateCharacter(Client player)
        {
            API.shared.triggerClientEvent(player, "create_char_menu", 0);
        }
        public static void SelectCharacter(Client player, Character character)
        {
            CharacterController characterController = new CharacterController(player, character);
            SpawnManager.SpawnCharacter(player, characterController);

            API.shared.triggerClientEvent(player, "stopAudio");
            player.freeze(false);
            player.transparency = 255;
            API.shared.triggerClientEvent(player, "update_money_display", character.Cash);
            API.shared.triggerClientEvent(player, "CEF_DESTROY");
        }

        // TODO: А нужна ли вообще эта функция нам?
        public void LoadProperties(Client player)
        {
            if (Character.Property == null) return;
            foreach (Data.Property property in Character.Property)
            {
                API.shared.triggerClientEvent(player, "create_blip", new Vector3(property.ExtPosX, property.ExtPosY, property.ExtPosZ), 0, 0);
            }
        }

        public GroupMember GetGroupInfo(int GroupId)
        {
            return Character.GroupMember.FirstOrDefault(x => x.Group.Id == GroupId);
        }
        public void AddGroup(Data.Group group, bool leader)
        {
            GroupMember memberEntry = new GroupMember();
            memberEntry.Character = Character;
            memberEntry.Group = group;
            memberEntry.Leader = leader;
            Character.GroupMember.Add(memberEntry);
            ContextFactory.Instance.SaveChanges();
        }

        public void SetActiveGroup(Data.Group group)
        {
            if (group == null) return;

            GroupMember GroupInfo = GetGroupInfo(group.Id);
            if (GroupInfo != null)
            {
                ActiveGroup = new GroupMember();
                ActiveGroup.Group = group;
                ActiveGroup.Leader = GroupInfo.Leader;
                Character.ActiveGroupID = GroupInfo.Group.Id;
                API.shared.consoleOutput("Вы переведены в группу: " + ActiveGroup.Group.Name);
            }
            //API.shared.consoleOutput("Вы переведены в группу: " + group.Name + "На должность: " + Enum.GetName(typeof(Data.Enums.GroupExtraType), group.Id));
        }

        public string ListGroups()
        {
            string returnstring = "Группы: ";
            int count = 0;
            foreach (var group in Character.GroupMember)
            {
                if (group == null) returnstring += "None, ";
                else returnstring += "TODO";
                count++;
            }
            if (count == 0) returnstring += "None";
            return returnstring;
        }

        public static bool NameValidityCheck(Client player, string name)
        {
            if (!name.Contains("_") || name.Count(x => x == '_') > 1)
            {
                API.shared.sendChatMessageToPlayer(player, "~r~ОШИБКА: ~w~Вы должны ввести Имя и Фамилию.\nПожалуйста разделите ваше Имя и Фамилию\nсимволом '_'.");
                return false;
            }
            else if (ContextFactory.Instance.Character.FirstOrDefault(x => x.Name.ToLower() == name.ToLower()) != null)
            {
                API.shared.sendChatMessageToPlayer(player, "~r~ОШИБКА: ~w~Такое имя уже существует!");
                return false;
            }
            else if (Regex.IsMatch(name, @"^[a-zA-Z_]+$")) return true;
            API.shared.sendChatMessageToPlayer(player, "~r~ОШИБКА: ~w~Вы ввели неверное имя");
            return false;
        }

        public static void PlayScenario(Client player, string scenario)
        {
            player.playScenario(scenario);
            API.shared.triggerClientEvent(player, "animation_text");
        }
        public static void PlayAnimation(Client player, string animDict, string animName, int flag)
        {
            player.playAnimation(animDict, animName, flag);
            API.shared.triggerClientEvent(player, "animation_text");
        }
        public static void StopAnimation(Client player)
        {
            player.stopAnimation();
            API.shared.triggerClientEvent(player, "animation_text");
        }

        public static int IsCharacterInGangNum(CharacterController characterController)
        {
            if (characterController.Character.ActiveGroupID >= 1300 &&
                characterController.Character.ActiveGroupID <= 1310) return 13;
            if (characterController.Character.ActiveGroupID >= 1400 &&
                characterController.Character.ActiveGroupID <= 1410) return 14;
            if (characterController.Character.ActiveGroupID >= 1500 &&
                characterController.Character.ActiveGroupID <= 1510) return 15;
            if (characterController.Character.ActiveGroupID >= 1600 &&
                characterController.Character.ActiveGroupID <= 1610) return 16;
            if (characterController.Character.ActiveGroupID >= 1700 &&
                characterController.Character.ActiveGroupID <= 1710) return 17;
            return 0;
        }
        public static bool IsCharacterInGang(CharacterController characterController)
        {
            if (characterController.Character.ActiveGroupID >= 1300 &&
                characterController.Character.ActiveGroupID <= 1310) return true;
            if (characterController.Character.ActiveGroupID >= 1400 &&
                characterController.Character.ActiveGroupID <= 1410) return true;
            if (characterController.Character.ActiveGroupID >= 1500 &&
                characterController.Character.ActiveGroupID <= 1510) return true;
            if (characterController.Character.ActiveGroupID >= 1600 &&
                characterController.Character.ActiveGroupID <= 1610) return true;
            if (characterController.Character.ActiveGroupID >= 1700 &&
                characterController.Character.ActiveGroupID <= 1710) return true;
            return false;
        }
        public static bool IsCharacterInGang(Character character)
        {
            if (character.ActiveGroupID >= 1300 &&
                character.ActiveGroupID <= 1310) return true;
            if (character.ActiveGroupID >= 1400 &&
                character.ActiveGroupID <= 1410) return true;
            if (character.ActiveGroupID >= 1500 &&
                character.ActiveGroupID <= 1510) return true;
            if (character.ActiveGroupID >= 1600 &&
                character.ActiveGroupID <= 1610) return true;
            if (character.ActiveGroupID >= 1700 &&
                character.ActiveGroupID <= 1710) return true;
            return false;
        }
        public static bool IsCharacterHighRankInGang(Character character)
        {
            if (character.ActiveGroupID >= 1307 &&
                character.ActiveGroupID <= 1310) return true;
            if (character.ActiveGroupID >= 1407 &&
                character.ActiveGroupID <= 1410) return true;
            if (character.ActiveGroupID >= 1507 &&
                character.ActiveGroupID <= 1510) return true;
            if (character.ActiveGroupID >= 1607 &&
                character.ActiveGroupID <= 1610) return true;
            if (character.ActiveGroupID >= 1707 &&
                character.ActiveGroupID <= 1710) return true;
            return false;
        }
        public static bool IsCharacterGangBoss(CharacterController characterController)
        {
            if (characterController.Character.ActiveGroupID == 1310) return true;
            if (characterController.Character.ActiveGroupID == 1410) return true;
            if (characterController.Character.ActiveGroupID == 1510) return true;
            if (characterController.Character.ActiveGroupID == 1610) return true;
            if (characterController.Character.ActiveGroupID == 1710) return true;
            return false;
        }
        public static bool IsCharacterGangBoss(Character character)
        {
            if (character.ActiveGroupID == 1310) return true;
            if (character.ActiveGroupID == 1410) return true;
            if (character.ActiveGroupID == 1510) return true;
            if (character.ActiveGroupID == 1610) return true;
            if (character.ActiveGroupID == 1710) return true;
            return false;
        }

        public static bool IsCharacterInActiveArmyCloth(Character character)
        {
            if (character.ActiveClothes == 2) return true;
            if (character.ActiveClothes == 3) return true;
            if (character.ActiveClothes == 4) return true;
            return false;
        }
        public static bool IsCharacterInArmy(CharacterController characterController)
        {
            if (characterController.Character.ActiveGroupID >= 2000 &&
                characterController.Character.ActiveGroupID <= 2015) return true;
            if (characterController.Character.ActiveGroupID >= 2100 &&
                characterController.Character.ActiveGroupID <= 2115) return true;
            return false;
        }
        public static bool IsCharacterInArmy(Character character)
        {
            if (character.ActiveGroupID >= 2000 &&
                character.ActiveGroupID <= 2015) return true;
            if (character.ActiveGroupID >= 2100 &&
                character.ActiveGroupID <= 2115) return true;
            return false;
        }
        public static bool IsCharacterArmySoldier(CharacterController characterController)
        {
            if (characterController.Character.ActiveGroupID == 2001 || characterController.Character.ActiveGroupID == 2002) return true;
            if (characterController.Character.ActiveGroupID == 2101 || characterController.Character.ActiveGroupID == 2102) return true;
            return false;
        }
        public static bool IsCharacterArmyHighOfficer(Character character)
        {
            if (character.ActiveGroupID >= 2012 && character.ActiveGroupID <= 2014) return true;
            if (character.ActiveGroupID >= 2112 && character.ActiveGroupID <= 2114) return true;
            return false;
        }
        public static bool IsCharacterArmyInAllOfficers(CharacterController characterController)
        {
            if (characterController.Character.ActiveGroupID >= 2003 && characterController.Character.ActiveGroupID <= 2014) return true;
            if (characterController.Character.ActiveGroupID >= 2103 && characterController.Character.ActiveGroupID <= 2114) return true;
            return false;
        }
        public static bool IsCharacterArmyInAllOfficers(Character character)
        {
            if (character.ActiveGroupID >= 2003 && character.ActiveGroupID <= 2014) return true;
            if (character.ActiveGroupID >= 2103 && character.ActiveGroupID <= 2114) return true;
            return false;
        }
        public static bool IsCharacterArmyGeneral(CharacterController characterController)
        {
            if (characterController.Character.ActiveGroupID == 2015) return true;
            if (characterController.Character.ActiveGroupID == 2115) return true;
            return false;
        }
        public static bool IsCharacterArmyGeneral(Character character)
        {
            if (character.ActiveGroupID == 2015) return true;
            if (character.ActiveGroupID == 2115) return true;
            return false;
        }

        public static bool IsCharacterInPolice(CharacterController characterController)
        {
            if (characterController.Character.ActiveGroupID >= 100 &&
                characterController.Character.ActiveGroupID <= 114) return true;
            return false;
        }
        public static bool IsCharacterInPolice(Character character)
        {
            if (character.ActiveGroupID >= 100 &&
                character.ActiveGroupID <= 114) return true;
            return false;
        }
        public static bool IsCharacterInFBI(CharacterController characterController)
        {
            if (characterController.Character.ActiveGroupID >= 300 &&
                characterController.Character.ActiveGroupID <= 310) return true;
            return false;
        }
        public static bool IsCharacterInFBI(Character character)
        {
            if (character.ActiveGroupID >= 300 &&
                character.ActiveGroupID <= 310) return true;
            return false;
        }
       
        // Gangs sectors methods:
        public static bool IsCharacterInGhetto(Client player)
        {
            var x1 = -468.0; var y1 = -2262.0;
            var x3 = 1222.0; var y3 = -572.0;
            
            if (x1 < player.position.X && player.position.X < x3)
                if (y1 < player.position.Y && player.position.Y < y3)
                    return true;
            return false;
        }
        public static bool IsPlayerInYourGangSector(Client player)
        {
            var currentSector = InWhichSectorOfGhetto(player).Split(';');
            var currentSectorData = ContextFactory.Instance.GangSectors.First(
                x => x.IdRow == Convert.ToInt32(currentSector[0]));

            var sectorData = 0;
            switch(currentSector[1])
            {
                case "1": sectorData = currentSectorData.Col1; break;
                case "2": sectorData = currentSectorData.Col2; break;
                case "3": sectorData = currentSectorData.Col3; break;
                case "4": sectorData = currentSectorData.Col4; break;
                case "5": sectorData = currentSectorData.Col5; break;
                case "6": sectorData = currentSectorData.Col6; break;
                case "7": sectorData = currentSectorData.Col7; break;
                case "8": sectorData = currentSectorData.Col8; break;
                case "9": sectorData = currentSectorData.Col9; break;
                case "10": sectorData = currentSectorData.Col10; break;
                case "11": sectorData = currentSectorData.Col11; break;
                case "12": sectorData = currentSectorData.Col12; break;
                case "13": sectorData = currentSectorData.Col13; break;
            }

            CharacterController characterController = player.getData("CHARACTER");
            if (characterController == null) return false;
            Character character = characterController.Character;

            if (sectorData == character.GroupType || 
                sectorData == character.GroupType * 10) return true;

            return false;
        }
        public static string InWhichSectorOfGhetto(Client player)
        {
            // START points:
            var x1 = -468.0; var y1 = -2262.0;
            var x3 = -338.0; var y3 = -2132.0;

            for (var i = 1; i < 14; i++)
            {
                x1 = -468.0;
                x3 = -338.0;
                for (var j = 1; j < 14; j++)
                {
                    if (x1 < player.position.X && player.position.X < x3)
                        if (y1 < player.position.Y && player.position.Y < y3)
                            return i + ";" + j;
                    x1 += 130;
                    x3 += 130;
                }
                y1 += 130;
                y3 += 130;
            }
            return "0;0";
        }

        // Face methods:
        public static void InitializePedFace(NetHandle ent)
        {
            API.shared.setEntitySyncedData(ent, "GTAO_HAS_CHARACTER_DATA", true);

            API.shared.setEntitySyncedData(ent, "GTAO_SHAPE_FIRST_ID", 0);
            API.shared.setEntitySyncedData(ent, "GTAO_SHAPE_SECOND_ID", 0);
            API.shared.setEntitySyncedData(ent, "GTAO_SKIN_FIRST_ID", 0);
            API.shared.setEntitySyncedData(ent, "GTAO_SKIN_SECOND_ID", 0);
            API.shared.setEntitySyncedData(ent, "GTAO_SHAPE_MIX", 0f);
            API.shared.setEntitySyncedData(ent, "GTAO_SKIN_MIX", 0f);
            API.shared.setEntitySyncedData(ent, "GTAO_HAIR", 0);
            API.shared.setEntitySyncedData(ent, "GTAO_HAIR_COLOR", 0);
            API.shared.setEntitySyncedData(ent, "GTAO_HAIR_HIGHLIGHT_COLOR", 0);
            API.shared.setEntitySyncedData(ent, "GTAO_EYE_COLOR", 0);
            API.shared.setEntitySyncedData(ent, "GTAO_EYEBROWS", 0);

            //API.setEntitySyncedData(ent, "GTAO_MAKEUP", 0); // No lipstick by default. 
            //API.setEntitySyncedData(ent, "GTAO_LIPSTICK", 0); // No makeup by default.

            API.shared.setEntitySyncedData(ent, "GTAO_EYEBROWS_COLOR", 0);
            API.shared.setEntitySyncedData(ent, "GTAO_MAKEUP_COLOR", 0);
            API.shared.setEntitySyncedData(ent, "GTAO_LIPSTICK_COLOR", 0);
            API.shared.setEntitySyncedData(ent, "GTAO_EYEBROWS_COLOR2", 0);
            API.shared.setEntitySyncedData(ent, "GTAO_MAKEUP_COLOR2", 0);
            API.shared.setEntitySyncedData(ent, "GTAO_LIPSTICK_COLOR2", 0);

            var list = new float[21];

            for (var i = 0; i < 21; i++)
            {
                list[i] = 0f;
            }

            API.shared.setEntitySyncedData(ent, "GTAO_FACE_FEATURES_LIST", list);
        }
        public static void RemovePedFace(NetHandle ent)
        {
            API.shared.setEntitySyncedData(ent, "GTAO_HAS_CHARACTER_DATA", false);

            API.shared.resetEntitySyncedData(ent, "GTAO_SHAPE_FIRST_ID");
            API.shared.resetEntitySyncedData(ent, "GTAO_SHAPE_SECOND_ID");
            API.shared.resetEntitySyncedData(ent, "GTAO_SKIN_FIRST_ID");
            API.shared.resetEntitySyncedData(ent, "GTAO_SKIN_SECOND_ID");
            API.shared.resetEntitySyncedData(ent, "GTAO_SHAPE_MIX");
            API.shared.resetEntitySyncedData(ent, "GTAO_SKIN_MIX");
            API.shared.resetEntitySyncedData(ent, "GTAO_HAIR");
            API.shared.resetEntitySyncedData(ent, "GTAO_HAIR_COLOR");
            API.shared.resetEntitySyncedData(ent, "GTAO_HAIR_HIGHLIGHT_COLOR");
            API.shared.resetEntitySyncedData(ent, "GTAO_EYE_COLOR");
            API.shared.resetEntitySyncedData(ent, "GTAO_EYEBROWS");
            API.shared.resetEntitySyncedData(ent, "GTAO_MAKEUP");
            API.shared.resetEntitySyncedData(ent, "GTAO_LIPSTICK");
            API.shared.resetEntitySyncedData(ent, "GTAO_EYEBROWS_COLOR");
            API.shared.resetEntitySyncedData(ent, "GTAO_MAKEUP_COLOR");
            API.shared.resetEntitySyncedData(ent, "GTAO_LIPSTICK_COLOR");
            API.shared.resetEntitySyncedData(ent, "GTAO_EYEBROWS_COLOR2");
            API.shared.resetEntitySyncedData(ent, "GTAO_MAKEUP_COLOR2");
            API.shared.resetEntitySyncedData(ent, "GTAO_LIPSTICK_COLOR2");
            API.shared.resetEntitySyncedData(ent, "GTAO_FACE_FEATURES_LIST");
        }
        public static bool IsPlayerFaceValid(NetHandle ent)
        {
            if (!API.shared.hasEntitySyncedData(ent, "GTAO_SHAPE_FIRST_ID")) return false;
            if (!API.shared.hasEntitySyncedData(ent, "GTAO_SHAPE_SECOND_ID")) return false;
            if (!API.shared.hasEntitySyncedData(ent, "GTAO_SKIN_FIRST_ID")) return false;
            if (!API.shared.hasEntitySyncedData(ent, "GTAO_SKIN_SECOND_ID")) return false;
            if (!API.shared.hasEntitySyncedData(ent, "GTAO_SHAPE_MIX")) return false;
            if (!API.shared.hasEntitySyncedData(ent, "GTAO_SKIN_MIX")) return false;
            if (!API.shared.hasEntitySyncedData(ent, "GTAO_HAIR")) return false;
            if (!API.shared.hasEntitySyncedData(ent, "GTAO_HAIR_COLOR")) return false;
            if (!API.shared.hasEntitySyncedData(ent, "GTAO_HAIR_HIGHLIGHT_COLOR")) return false;
            if (!API.shared.hasEntitySyncedData(ent, "GTAO_EYE_COLOR")) return false;
            if (!API.shared.hasEntitySyncedData(ent, "GTAO_EYEBROWS")) return false;
            //if (!API.hasEntitySyncedData(ent, "GTAO_MAKEUP")) return false; // Player may have no makeup
            //if (!API.hasEntitySyncedData(ent, "GTAO_LIPSTICK")) return false; // Player may have no lipstick
            if (!API.shared.hasEntitySyncedData(ent, "GTAO_EYEBROWS_COLOR")) return false;
            if (!API.shared.hasEntitySyncedData(ent, "GTAO_MAKEUP_COLOR")) return false;
            if (!API.shared.hasEntitySyncedData(ent, "GTAO_LIPSTICK_COLOR")) return false;
            if (!API.shared.hasEntitySyncedData(ent, "GTAO_EYEBROWS_COLOR2")) return false;
            if (!API.shared.hasEntitySyncedData(ent, "GTAO_MAKEUP_COLOR2")) return false;
            if (!API.shared.hasEntitySyncedData(ent, "GTAO_LIPSTICK_COLOR2")) return false;
            if (!API.shared.hasEntitySyncedData(ent, "GTAO_FACE_FEATURES_LIST")) return false;

            return true;
        }
        public static void UpdatePlayerFace(NetHandle player)
        {
            API.shared.triggerClientEventForAll("UPDATE_CHARACTER", player);
        }
    }
}