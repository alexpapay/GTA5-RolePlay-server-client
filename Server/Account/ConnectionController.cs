using GTANetworkServer;
using GTANetworkShared;
using System;
using System.Linq;
using TheGodfatherGM.Server.DBManager;
using TheGodfatherGM.Server.User;
using TheGodfatherGM.Server.Characters;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;

namespace TheGodfatherGM.Server
{
    public class ConnectionController : Script
    {
        public static readonly Vector3 _startPos = new Vector3(3433.339f, 5177.579f, 39.79541f);
        public static readonly Vector3 _startCamPos = new Vector3(3476.85f, 5228.022f, 9.453369f);


        public ConnectionController()
        {
            API.onPlayerConnected += OnPlayerConnectedHandler;
            API.onPlayerFinishedDownload += onPlayerFinishedDownloadHandler;
            API.onPlayerDisconnected += onPlayerDisconnectedHandler;            
        }

        public void OnPlayerConnectedHandler(Client player)
        {
            if(AccountController.IsAccountBanned(player))
            {
                player.kick("~r~Вы забанены на данном сервере.");
            }
        }

        public void onPlayerFinishedDownloadHandler(Client player)
        {
            API.setEntityData(player, "DOWNLOAD_FINISHED", true);
            LoginMenu(player);
        }

        public void onPlayerDisconnectedHandler(Client player, string reason)
        {            
            AccountController account = player.getData("ACCOUNT");
            if (account == null) return;
            account.CharacterController.Character.Online = false;
            ContextFactory.Instance.SaveChanges();
            LogOut(account);            
        }

        public static void LoginMenu(Client player)
        {
            API.shared.triggerClientEvent(player, "interpolateCamera", 20000, _startCamPos, _startCamPos + new Vector3(0.0, -50.0, 50.0), new Vector3(0.0, 0.0, 180.0), new Vector3(0.0, 0.0, 95.0));
            player.position = _startPos;
            player.freeze(true);
            player.transparency = 0;
            PromptLoginScreenAsync(player);
        }

        public static void LogOut(AccountController account, int type = 0)
        {
            account.CharacterController.Character.LastLogoutDate = DateTime.Now;            
            account.Save();
            account.Account.Online = false;
            if (type != 0)
            {
                LoginMenu(account.Client);
            }

            account.Account.SessionID = null;
            account.CharacterController.Character.Online = false;
            ContextFactory.Instance.SaveChanges();
            //Vehicles.VehicleController.UnloadVehicles(account);
            account.Client.resetData("ACCOUNT");
        }

        // Инициализация окна регистрации или авторизации
        public static void PromptLoginScreenAsync(Client player)
        {

            //CharacterController.SelectCharacter(player, 1);
            
            string url = Global.GlobalVars.WebServerConnectionString + "Game/Login?socialclub=" + player.socialClubName + "&token=" + Global.Util.GenerateToken();
            API.shared.triggerClientEvent(player, "CEFController", url);
            API.shared.sendChatMessageToPlayer(player, "URL: " + url);
            /*
            AccountController account = player.getData("ACCOUNT");
            var test = ContextFactory.Instance.Character.FirstOrDefault(x => x.Name == player.socialClubName);

            if (test != null)
            {
                CharacterController.SelectCharacter(player, 0);
            }
            else LogOut(account);*/
        }        

        /*
        [Command]
        public static void Login(Client player, string name, string password)
        {
            if (API.shared.getEntityData(player, "ACCOUNT") != null)
            {
                API.shared.sendChatMessageToPlayer(player, "~r~SERVER: ~w~You're already logged in!");
                return;
            }

            var accountData = ContextFactory.Instance.Account.Where(x => x.UserName == name).FirstOrDefault();
            if (accountData != null && BCr.BCrypt.Verify(password, accountData.PasswordHash))
            {
                if (AccountController.IsAccountBanned(accountData))
                {
                    player.kick("~r~This account is banned.");
                }
                else new AccountController(accountData, player);
            }
            else
            {
                API.shared.sendChatMessageToPlayer(player, "~r~ERROR:~w~ Wrong password, or account doesnt exist!");
                PromptLoginScreen(player);
            }
        }
        */
    }
    }