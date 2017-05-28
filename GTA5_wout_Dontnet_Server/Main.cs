﻿using GTANetworkServer;
using GTANetworkShared;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using TheGodfatherGM.Data;
using TheGodfatherGM.Server.DBManager;
using TheGodfatherGM.Server.Groups;
using TheGodfatherGM.Data.Enums;
using TheGodfatherGM.Data.Models;

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

            // TODO: do different method later
            API.createObject(552807189, new Vector3(-1397.168, 5815.977, 20.0), new Vector3(0.0f, 0.0f, 0.0f));
        }

        private DateTime m_lastTick = DateTime.Now;
        private DateTime MinuteAnnounce;
        private DateTime HourAnnounce;

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
                            var money = 0;
                            if (character.JobId == JobsIdNonDataBase.TaxiDriver && isTaxiVehicle != null) money += WorkPay.TaxiDriver; // TaxiDrivers
                            //if (character.JobId == JobsIdNonDataBase.BusDriver1) money += WorkPay.BusDriver1Pay; // BusDrivers
                            if (character.JobId == JobsIdNonDataBase.Unemployer) money += WorkPay.Unemployer; // Unemployers
                            money += PayDayMoney.GetPayDaYMoney(character.ActiveGroupID);
                            character.Cash += money;

                            try
                            {
                                Client currentPlayer = API.shared.getAllPlayers().FirstOrDefault(x => x.socialClubName == character.SocialClub);
                                API.shared.sendChatMessageToPlayer(currentPlayer, "~g~[СЕРВЕР]: ~w~ Вы получили деньги: " + money + "$.");
                                API.shared.triggerClientEvent(currentPlayer, "update_money_display", character.Cash);
                            }
                            catch (Exception e) { }
                        }
                    }
                    ContextFactory.Instance.SaveChanges();
                }
                catch (Exception e) { }

                // Капт сектора
                try
                {
                    var tick = 0;
                    var startCapting = ContextFactory.Instance.Caption.First(x => x.Id == 1);
                    if (startCapting.Sector != "0;0")
                    {
                        var sector = startCapting.Sector.Split(';');
                        tick += 1;
                        if (tick == 2)
                        {
                            var getAttackGroup = ContextFactory.Instance.Group.FirstOrDefault(x => x.Id == startCapting.GangAttack * 100);
                            var groupAttackType = (GroupType)Enum.Parse(typeof(GroupType), getAttackGroup.Type.ToString());
                            var getDefendGroup = ContextFactory.Instance.Group.FirstOrDefault(x => x.Id == startCapting.GangDefend * 100);
                            var groupDefendType = (GroupType)Enum.Parse(typeof(GroupType), getAttackGroup.Type.ToString());

                            if (startCapting.FragsAttack > startCapting.FragsDefend || 
                                (startCapting.FragsAttack == 0 && startCapting.FragsDefend == 0))
                            {                                
                                GroupController.SetGangSectorData(Convert.ToInt32(sector[0]),
                                    Convert.ToInt32(sector[1]), startCapting.GangAttack);
                                API.shared.sendChatMessageToAll("Банда: " + EntityManager.GetDisplayName(groupAttackType) + "захватила у банды: \n" + EntityManager.GetDisplayName(groupDefendType) + "сектор: " + startCapting.Sector);
                            }
                            else
                            {
                                GroupController.SetGangSectorData(Convert.ToInt32(sector[0]),
                                    Convert.ToInt32(sector[1]), startCapting.GangDefend);
                                API.shared.sendChatMessageToAll("Банда: " + EntityManager.GetDisplayName(groupDefendType) + "не смогла захватить у банды: \n" + EntityManager.GetDisplayName(groupAttackType) + "сектор: " + startCapting.Sector);
                            }

                            GroupController.SetDefaultCaption(1);
                        }
                    }
                    else tick = 0;
                }
                catch (Exception e) { }

                // Тестовая зона (ежеминутный тик)
                try
                {
                    
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
                        ContextFactory.Instance.SaveChanges();
                        numInc++;                        
                    }
                }
                catch (Exception e) { }

                // Начисление зарплаты в за каждый бизнес. Пока 10К за любую заправку.
                try
                {                    
                    var characters = ContextFactory.Instance.Character.Where(x => x.Online == true).ToList();
                    var test2 = characters.Count();
                    ContextFactory.Instance.SaveChanges();

                    foreach (var character in characters)
                    {
                        var jobs = ContextFactory.Instance.Job.Where(y => y.CharacterId == character.Id).ToList();

                        foreach (var job in jobs)
                        {
                            character.Cash += job.Cost;
                            ContextFactory.Instance.SaveChanges();

                            try
                            {
                                Client currentPlayer = API.shared.getAllPlayers().FirstOrDefault(x => x.socialClubName == character.SocialClub);
                                API.shared.sendChatMessageToPlayer(currentPlayer, "~g~[СЕРВЕР]: ~w~ Вы получили за " + job.Id + " бизнес: " + job.Cost + "$.");
                                API.shared.triggerClientEvent(currentPlayer, "update_money_display", character.Cash);
                            }
                            catch (Exception e) { }
                        }                        
                    }
                    /*foreach (var character in characters)
                    
                                character.Cash += WorkPay.AllGusStationOwner;
                                
                                
                            }
                        }
                    }*/
                }
                catch (Exception e) {}

                HourAnnounce = DateTime.Now;                
            }  

            // List of players
            if ((DateTime.Now - m_lastTick).TotalMilliseconds >= 500)
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
