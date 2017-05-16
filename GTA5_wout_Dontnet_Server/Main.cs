using GTANetworkServer;
using GTANetworkShared;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using TheGodfatherGM.Data;
using TheGodfatherGM.Server.DBManager;
using TheGodfatherGM.Server.Vehicles;
using TheGodfatherGM.Server.Groups;

namespace TheGodfatherGM.Server
{
    class Main : Script
    {
        public Main()
        {
            API.onResourceStart += OnResourceStart;
            API.onResourceStop += OnResourceStop;
            API.onUpdate += OnUpdateHandler;
            API.onPlayerWeaponSwitch += OnPlayerWeaponSwitchHandler;
        }

        private DateTime m_lastTick = DateTime.Now;
        private DateTime MinuteAnnounce;
        private DateTime OneSecond;
        private DateTime HourAnnounce;

        // Добавление минуты к пребыванию в игре после авторизации пользователя
        public void OnUpdateHandler()
        {
            if (DateTime.Now.Subtract(MinuteAnnounce).TotalMinutes >= 1)
            {                
                // Уровень пользователя
                try
                {
                    var characters = ContextFactory.Instance.Character.Where(x => x.Online == true);
                    foreach (var character in characters)
                    {
                        character.PlayMinutes++;
                        // Уровень за 4 часа пребывания в игре
                        if (character.PlayMinutes % 240 == 0)
                        {
                            character.Level = character.PlayMinutes / 240;
                            try
                            {
                                Client currentPlayer = API.shared.getAllPlayers().FirstOrDefault(x => x.socialClubName == character.SocialClub);
                                API.shared.sendChatMessageToPlayer(currentPlayer, "~g~[СЕРВЕР]: ~w~ Поздравляем! Теперь вы достигли " + character.Level + " уровня.");
                            }
                            catch (Exception e) { }
                        }
                        // Зарплата за час пребывания в игре
                        if (character.PlayMinutes % 60 == 0)
                        {
                            var isTaxiVehicle = ContextFactory.Instance.Vehicle.FirstOrDefault(x => x.CharacterId == character.Id);
                            if (character.JobId == 777 && isTaxiVehicle != null) character.Cash += 300; // TaxiDrivers
                            if (character.JobId == 888) character.Cash += 100; // Unemployers
                            character.Cash += Data.Models.PayDayMoney.GetPayDaYMoney(character.ActiveGroupID);

                            try
                            {
                                Client currentPlayer = API.shared.getAllPlayers().FirstOrDefault(x => x.socialClubName == character.SocialClub);
                                API.shared.sendChatMessageToPlayer(currentPlayer, "~g~[СЕРВЕР]: ~w~ Вы получили свою зарплату: " + Data.Models.PayDayMoney.GetPayDaYMoney(character.ActiveGroupID) + "$.");
                                API.shared.triggerClientEvent(currentPlayer, "update_money_display", character.Cash);
                            }
                            catch (Exception e) { }
                        }
                    }
                    ContextFactory.Instance.SaveChanges();
                }
                catch (Exception e) { }
                // Прокат транспорта (каждую минуту вычитается 1 ед. Fuel):
                try
                {
                    VehicleController.RentVehicle(Data.Models.RentModels.ScooterModel);
                    VehicleController.RentVehicle(Data.Models.RentModels.TaxiModel);
                }
                catch (Exception e) { }                

                MinuteAnnounce = DateTime.Now;
            }

            if (DateTime.Now.Subtract(HourAnnounce).TotalMinutes >= 60)
            {
                // Начисление зарплаты в банк банд каждый час по количеству квадратов.
                try
                {
                    var numInc = 0;
                    for (var i = 1300; i <= 1700; i += 100)
                    {
                        var currentGang = ContextFactory.Instance.Group.First(x => x.Id == i);
                        var numOfSectors = GroupController.GetCountOfGangsSectors();
                        var money = numOfSectors[numInc] * 50;
                        currentGang.MoneyBank += money;
                        numInc++;
                    }
                }
                catch (Exception e) { }

                HourAnnounce = DateTime.Now;                
            }
            
            if (DateTime.Now.Subtract(OneSecond).TotalSeconds >= 1)
            {
                //Client player = API.shared.getAllPlayers().FirstOrDefault(x => x.socialClubName == character.SocialClub);
                //API.shared.triggerClientEvent(player, "send_coord", player.position.X, player.position.Y);

                //OneSecond = DateTime.Now;
            }

            // List of players
            if ((DateTime.Now - m_lastTick).TotalMilliseconds >= 1000)
            {
                m_lastTick = DateTime.Now;

                var changedNames = new List<string>();
                var players = API.getAllPlayers();
                foreach (var player in players)
                {
                    string lastName = player.getData("playerlist_lastname");

                    if (lastName == null)
                    {
                        player.setData("playerlist_lastname", player.name);
                        continue;
                    }

                    if (lastName != player.name)
                    {
                        player.setData("playerlist_lastname", player.name);
                        Character characterToList = ContextFactory.Instance.Character.FirstOrDefault(x => x.SocialClub == player.socialClubName);
                        var dic = new Dictionary<string, object>();
                        dic["userName"] = characterToList.Name.ToString();
                        dic["userID"] = characterToList.Id;
                        dic["socialClubName"] = player.socialClubName;
                        dic["newName"] = player.name;
                        changedNames.Add(API.toJson(dic));
                    }
                }

                if (changedNames.Count > 0)
                {
                    API.triggerClientEventForAll("playerlist_changednames", changedNames);
                }
            }
        }

        private void OnPlayerWeaponSwitchHandler(Client player, WeaponHash oldWeapon)
        {
            var weapons = API.getPlayerWeapons(player);
            foreach (var weapon in weapons)
            {
                if (weapon == WeaponHash.Minigun) player.kick("This weapon are restricted!");
            }
        }

        private void OnResourceStart()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("en-GB");
            Console.BackgroundColor = ConsoleColor.Blue;
            API.consoleOutput(Global.GlobalVars.ServerName + " was started at " + DateTime.Now);
            Console.ResetColor();
        }

        private void OnResourceStop()
        {
            var characters = ContextFactory.Instance.Character.Where(x => x.Online == true);
            foreach (var character in characters) character.Online = false;
            ContextFactory.Instance.SaveChanges();

            API.consoleOutput("Сброс активных сессий...");
            Task DBTerminate = Task.Run(() =>
            {
                //DatabaseManager.ResetSessions();
            });
            DBTerminate.Wait();
            API.consoleOutput(Global.GlobalVars.ServerName + " был остановлен в " + DateTime.Now);
            //Console.ReadKey();
        }
    }
}
