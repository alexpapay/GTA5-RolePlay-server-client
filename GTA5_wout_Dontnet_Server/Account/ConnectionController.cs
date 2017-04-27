using GTANetworkServer;
using GTANetworkShared;
using System;
using System.Linq;
using TheGodfatherGM.Server.DBManager;
using TheGodfatherGM.Data;
using TheGodfatherGM.Server.Characters;
using System.Collections.Generic;

namespace TheGodfatherGM.Server
{
    public class ConnectionController : Script
    {
        public static readonly Vector3 _startPos = new Vector3(3433.339f, 5177.579f, 39.79541f);
        public static readonly Vector3 _startCamPos = new Vector3(3476.85f, 5228.022f, 9.453369f);

        public ConnectionController()
        {
            API.onPlayerConnected += OnPlayerConnectedHandler;
            API.onPlayerFinishedDownload += OnPlayerFinishedDownloadHandler;
            API.onPlayerDisconnected += OnPlayerDisconnectedHandler;
            API.onPlayerDeath += OnPlayerDeath;
        }

        private void OnPlayerDeath(Client player, NetHandle entityKiller, int weapon)
        {
            Character targetCharacter = null;
            Client killer = API.getPlayerFromHandle(entityKiller);
            if (killer != null)
            {
                API.sendNotificationToAll(killer.name + " убил " + player.name);
                targetCharacter = ContextFactory.Instance.Character.FirstOrDefault(x => x.SocialClub == killer.socialClubName);
            }
            else
            {
                API.sendNotificationToAll(player.name + " умер...");
            }

            CharacterController characterController = player.getData("CHARACTER");            
            if (characterController == null) return;

            SpawnManager.SetPlayerWeapon(player, characterController.Character, 2);

            // Army change cloth after death
            if (CharacterController.IsCharacterInArmy(characterController))
            {
                SpawnManager.SetPlayerSkinClothesToDb(player, 101, characterController.Character, 1);

                if (targetCharacter != null && CharacterController.IsCharacterInGhetto(killer))
                {                    
                    if (CharacterController.IsCharacterArmySoldier(characterController))
                    {
                        targetCharacter.ClothesTypes = 2;
                        API.sendChatMessageToPlayer(killer, "~g~Вы забрали форму солдата!");
                    }                                           
                    if (CharacterController.IsCharacterArmyInAllOfficers(characterController))
                    {
                        targetCharacter.ClothesTypes = 3;
                        API.sendChatMessageToPlayer(killer, "~g~Вы забрали форму офицера!");
                    }                        
                    if (CharacterController.IsCharacterArmyGeneral(characterController))
                    {
                        targetCharacter.ClothesTypes = 4;
                        API.sendChatMessageToPlayer(killer, "~g~Вы забрали форму генерала!");
                    }                       
                    ContextFactory.Instance.SaveChanges();
                }
                else API.sendNotificationToAll("[DEBUG]: Ошибка передачи формы!");
            }            
            // Gang change clothes after death
            if (CharacterController.IsCharacterInGang(characterController) && 
                CharacterController.IsCharacterInActiveArmyCloth(characterController.Character))
            {
                characterController.Character.ClothesTypes = 0;
                ContextFactory.Instance.SaveChanges();
            }            
        }

        public void OnPlayerConnectedHandler(Client player)
        {
            Character characterToList = ContextFactory.Instance.Character.FirstOrDefault(x => x.SocialClub == player.socialClubName);
            if (characterToList == null) return;
            API.triggerClientEventForAll("playerlist_join", player.socialClubName, player.name, ColorForPlayer(player), characterToList.Id , characterToList.Name.ToString());
 
            if (IsAccountBanned(player))
            {
                player.kick("~r~Вы забанены на данном сервере.");
            }
        }
        public void OnPlayerFinishedDownloadHandler(Client player)
        {
            var players = API.getAllPlayers();
            var list = new List<string>();
            foreach (var ply in players)
            {
                var dic = new Dictionary<string, object>();
                dic["socialClubName"] = ply.socialClubName;
                dic["name"] = ply.name;
                dic["ping"] = ply.ping;
                dic["color"] = ColorForPlayer(ply);
                list.Add(API.toJson(dic));
            }
            API.triggerClientEvent(player, "playerlist", list);

            API.setEntityData(player, "DOWNLOAD_FINISHED", true);
            LoginMenu(player);
        }
        public void OnPlayerDisconnectedHandler(Client player, string reason)
        {
            API.triggerClientEventForAll("playerlist_leave", player.socialClubName);
            var character = ContextFactory.Instance.Character.FirstOrDefault(x => x.SocialClub == player.socialClubName);
            if (character == null) return;
            else
            {
                // Gang change clothes after disconnect
                if (CharacterController.IsCharacterInGang(character) && 
                    CharacterController.IsCharacterInActiveArmyCloth(character))
                    character.ClothesTypes = 0;
                
                character.LastLogoutDate = DateTime.Now;
                character.Online = false;
                character.OID = 0;
                SpawnManager.SetPlayerWeapon(player, character, 3);
                ContextFactory.Instance.SaveChanges();
            }            
        }

        public static void LogOut(Client player, Character character, int type = 0)
        {            
            character.Online = false;
            character.OID = 0;
            ContextFactory.Instance.SaveChanges();

            if (type != 0)
            {
                LoginMenu(player);
            }
            
            Global.CEFController.Close(player);
            player.resetData("CHARACTER");
            API.shared.resetEntityData(player, "DOWNLOAD_FINISHED");
        }
        public static void LoginMenu(Client player)
        {
            API.shared.triggerClientEvent(player, "interpolateCamera", 20000, _startCamPos, _startCamPos + new Vector3(0.0, -50.0, 50.0), new Vector3(0.0, 0.0, 180.0), new Vector3(0.0, 0.0, 95.0));
            player.position = _startPos;
            player.freeze(true);
            player.transparency = 0;
            PromptLoginScreen(player);
        }
        public static void PromptLoginScreen(Client player)
        {
            var character = ContextFactory.Instance.Character.FirstOrDefault(x => x.SocialClub == player.socialClubName);
            ContextFactory.Instance.SaveChanges();
            if (character == null) CharacterController.CreateCharacter(player);
            else
            {
                API.shared.triggerClientEvent(player, "login_char_menu", 0);
            }
        }

        public static bool IsAccountBanned(Client player)
        {
            var IPBanEntity = ContextFactory.Instance.Ban.FirstOrDefault(x => x.Active == true && x.Ip == player.address);
            if (IPBanEntity != null) return true;
            var SocialClubBanEntity = ContextFactory.Instance.Ban.FirstOrDefault(x => x.Active == true && x.IsSocialClubBanned == true && x.SocialClub == player.socialClubName);
            if (SocialClubBanEntity != null) return true;
            return false;
        }

        private string ColorForPlayer(Client player)
        {
            if (!API.isResourceRunning("colorednames"))
            {
                return "FFFFFF";
            }
            string ret = player.getData("PROFILE_color");
            if (ret == null)
            {
                return "FFFFFF";
            }
            return ret;
        }
    }
}