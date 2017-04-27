var mainWindow = null;
var mainWindow2 = null;
var mainWindow3 = null;
var menuPool = API.getMenuPool();
var menuPool2 = API.getMenuPool();
var menuPool3 = API.getMenuPool();
var is_show_house_menu_out = false;
var is_show_house_menu_in = false;
var is_show_cloth_shop = false;
var marker = null;
var waypoint = null;
var mainBrowser = null;
var is_open_cef = false;
var text = null;
var pool = null;
var draw = false;
var pwdtext = null;


API.onResourceStart.connect(function () {

});

API.onServerEventTrigger.connect(function (name, args) {

    if (name == "character_menu")
    {
        resetMainMenu();
        var callbackId = args[0];
        var banner = args[1];
        var subtitle = args[2];
        var bank = args[6];
        var access = args[7];

        if (banner == null) mainWindow = API.createMenu(subtitle, 0, 0, 6);
        else mainWindow = API.createMenu(banner, subtitle, 0, 0, 6);
        menuPool.Add(mainWindow);

        var gps = API.createMenuItem("~g~GPS", "");
        gps.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;

            mainWindow2 = API.createMenu("GPS", "Меню вашего аккаунта", 0, 0, 6);
            menuPool2.Add(mainWindow2);

            mainWindow2.Visible = true;

            var work = API.createMenuItem("Работа", "");
            work.Activated.connect(function (menu, item) {
                mainWindow2.Visible = false;

                mainWindow3 = API.createMenu("Работа для бомжа", "", 0, 0, 6);
                menuPool3.Add(mainWindow3);

                mainWindow3.Visible = true;

                var loader1 = API.createMenuItem("Грузчик 1: Уровень 0 ", "");
                loader1.Activated.connect(function (menu, item) {
                    mainWindow3.Visible = false;
                    API.displaySubtitle("Метка на карте установлена", 5000);
                    waypoint = API.setWaypoint(-144, -948);
                });
                mainWindow3.AddItem(loader1);

                var loader2 = API.createMenuItem("Грузчик 2: Уровень 0", "");
                loader2.Activated.connect(function (menu, item) {
                    mainWindow3.Visible = false;
                    API.displaySubtitle("Метка первой работы на карте установлена", 5000);
                    waypoint = API.setWaypoint(853, -2927);
                });
                mainWindow3.AddItem(loader2);

                var back = API.createMenuItem("~g~Назад", "");
                back.Activated.connect(function (menu, item) {
                    mainWindow3.Visible = false;
                    mainWindow2.Visible = true;
                });
                mainWindow3.AddItem(back);

                var close = API.createMenuItem("~r~Закрыть", "");
                close.Activated.connect(function (menu, item) {
                    mainWindow3.Visible = false;
                });
                mainWindow3.AddItem(close);

                mainWindow3.RefreshIndex();
            });
            mainWindow2.AddItem(work);

            var gos = API.createMenuItem("Важные места", "");
            gos.Activated.connect(function (menu, item) {
                mainWindow2.Visible = false;

                mainWindow3 = API.createMenu("Важные места", "", 0, 0, 6);
                menuPool3.Add(mainWindow3);

                mainWindow3.Visible = true;

                var meria = API.createMenuItem("Здание правительства", "");
                meria.Activated.connect(function (menu, item) {
                    mainWindow3.Visible = false;
                    API.displaySubtitle("Метка на карте установлена", 5000);
                    waypoint = API.setWaypoint(105, -933);
                });
                mainWindow3.AddItem(meria);

                var meria = API.createMenuItem("Автошкола", "");
                meria.Activated.connect(function (menu, item) {
                    mainWindow3.Visible = false;
                    API.displaySubtitle("Метка на карте установлена", 5000);
                    waypoint = API.setWaypoint(-1082, -1260);
                });
                mainWindow3.AddItem(meria);

                var meria = API.createMenuItem("Балласы", "");
                meria.Activated.connect(function (menu, item) {
                    mainWindow3.Visible = false;
                    API.displaySubtitle("Метка на карте установлена", 5000);
                    waypoint = API.setWaypoint(107, -1942);
                });
                mainWindow3.AddItem(meria);

                var back = API.createMenuItem("~g~Назад", "");
                back.Activated.connect(function (menu, item) {
                    mainWindow3.Visible = false;
                    mainWindow2.Visible = true;
                });
                mainWindow3.AddItem(back);

                var close = API.createMenuItem("~r~Закрыть", "");
                close.Activated.connect(function (menu, item) {
                    mainWindow3.Visible = false;
                });
                mainWindow3.AddItem(close);

                mainWindow3.RefreshIndex();
            });
            mainWindow2.AddItem(gos);

            var back = API.createMenuItem("~g~Назад", "");
            back.Activated.connect(function (menu, item) {
                mainWindow2.Visible = false;
                mainWindow.Visible = true;
            });
            mainWindow2.AddItem(back);

            var close = API.createMenuItem("~r~Закрыть", "");
            close.Activated.connect(function (menu, item) {
                mainWindow2.Visible = false;
            });
            mainWindow2.AddItem(close);

            mainWindow2.RefreshIndex();
        });
        mainWindow.AddItem(gps);       

        var getTaxi = API.createMenuItem("~g~Вызвать~s~ такси", "");
        getTaxi.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
            API.triggerServerEvent("get_taxi");
        });
        mainWindow.AddItem(getTaxi); 

        var age = API.createMenuItem("Ваш возраст:~s~ " + args[3], "");
            age.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
        });
            mainWindow.AddItem(age);

        var level = API.createMenuItem("Ваш уровень:~s~ " + args[4], "");
            level.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
        });
            mainWindow.AddItem(level);

        var job = API.createMenuItem("Ваша работа:~s~ " + args[5], "");
            job.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
        });
            mainWindow.AddItem(job);   

            var fraction = API.createMenuItem("Ваша фракция:~s~ " + args[9], "");
            fraction.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
            });
            mainWindow.AddItem(fraction);  

            var job = API.createMenuItem("Ваша должность:~s~ " + args[10], "");
            job.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
            });
            mainWindow.AddItem(job); 

            var driverLicense = API.createMenuItem("Наличие прав:~s~ " + args[7], "");
            driverLicense.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
            });
            mainWindow.AddItem(driverLicense);  

            var bank = API.createMenuItem("Денег в банке:~s~ " + args[6], "");
            bank.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
        });
            mainWindow.AddItem(bank);  

            var material = API.createMenuItem("Ваши материалы:~s~ " + args[11], "");
            material.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
            });
            mainWindow.AddItem(material); 

        var close = API.createMenuItem("~r~Закрыть", "");
        close.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
        });
        mainWindow.AddItem(close);

        mainWindow.Visible = true;
        mainWindow.RefreshIndex();
    }

    if (name == "workposs_menu") {
        resetMainMenu();
        var callbackId = args[0];
        var banner = "Я могу";
        var subtitle = "Выберите вашу рабочую возможность";
        var groupId = args[1];
        var jobId = args[2];
        var tempVar = args[3];
        var isAdmin = args[4];
        var material = args[7];
        var isInGang = args[8];
        var isGangBoss = args[9];
        var isInGhetto = args[10];

        if (banner == null) mainWindow = API.createMenu(subtitle, 0, 0, 6);
        else mainWindow = API.createMenu(banner, subtitle, 0, 0, 6);
        menuPool.Add(mainWindow);

        if (groupId >= 1200 && groupId <= 1210) {
            var autoschool = API.createMenuItem("~g~Выдать права~s~ пользователю", "");
            autoschool.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
                API.displaySubtitle("Введите ID игрока", 5000);
                var userId = parseInt(API.getUserInput("", 40));
                API.triggerServerEvent("got_driver_license", userId);
            });
            mainWindow.AddItem(autoschool); 
        }
        if (groupId >= 2012 && groupId <= 2015 || groupId >= 2112 && groupId <= 2115) {

            var armyId = 0;

            var natioalGuard = API.createMenuItem("~g~Управление ~s~своей фракцией", "");
            natioalGuard.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;

                mainWindow2 = API.createMenu("Моя фракция", "~s~" + args[5] + " : " + args[6], 0, 0, 6);
                menuPool2.Add(mainWindow2);

                mainWindow2.Visible = true;

                if (groupId >= 2014 && groupId <= 2015 || groupId >= 2114 && groupId <= 2115) {

                    var addToArmy = API.createMenuItem("~g~Принять пользователя в армию", "");
                    addToArmy.Activated.connect(function (menu, item) {
                        mainWindow2.Visible = false;

                        API.displaySubtitle("Введите ID игрока", 5000);
                        var userId = parseInt(API.getUserInput("", 40));

                        if (groupId == 2014 || groupId == 2015) armyId = 2001;
                        if (groupId == 2114 || groupId == 2115) armyId = 2101;

                        API.triggerServerEvent("national_guard_add_to_group", userId, armyId, 1);
                    });
                    mainWindow2.AddItem(addToArmy);
                }

                if (groupId >= 2012 && groupId <= 2015 || groupId >= 2112 && groupId <= 2115) {

                    var higherRank = API.createMenuItem("~y~Изменить звание пользователя", "");
                    higherRank.Activated.connect(function (menu, item) {
                        mainWindow2.Visible = false;

                        API.displaySubtitle("Введите ID игрока", 5000);
                        var userId = parseInt(API.getUserInput("", 40));

                        API.displaySubtitle("Введите присваиваемое звание", 5000);
                        var rangId = parseInt(API.getUserInput("", 40));

                        API.triggerServerEvent("national_guard_add_to_group", userId, rangId, 3, groupId);
                    });
                    mainWindow2.AddItem(higherRank);
                }

                if (groupId >= 2013 && groupId <= 2015 || groupId >= 2113 && groupId <= 2115) {
                    var goOutFrom = API.createMenuItem("~r~Выгнать пользователя из армии", "");
                    goOutFrom.Activated.connect(function (menu, item) {
                        mainWindow2.Visible = false;

                        API.displaySubtitle("Введите ID игрока", 5000);
                        var userId = parseInt(API.getUserInput("", 40));

                        API.triggerServerEvent("national_guard_add_to_group", userId, groupId, 2);
                    });
                    mainWindow2.AddItem(goOutFrom);
                }

                var back = API.createMenuItem("~g~Назад", "");
                back.Activated.connect(function (menu, item) {
                    mainWindow2.Visible = false;
                    mainWindow.Visible = true;
                });
                mainWindow2.AddItem(back);

                mainWindow2.RefreshIndex();
            });
            mainWindow.AddItem(natioalGuard); 
        }

        if (isInGang == true && material != 0) {
            var materialLoad = API.createMenuItem("~g~Загрузить материалы~s~ в машину", "");
            materialLoad.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
                API.triggerServerEvent("load_unload_material", 1);
            });
            mainWindow.AddItem(materialLoad);             
        }
        if (isInGang == true && material != 0 && isInGhetto == true) {

            var gangWeaponMenu = API.createMenuItem("~g~Создание ~s~оружия из материалов", "");
            gangWeaponMenu.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;

                mainWindow2 = API.createMenu("Моя фракция", "~s~" + args[5] + " : " + args[6], 0, 0, 6);
                menuPool2.Add(mainWindow2);
                mainWindow2.Visible = true;

                    var pistol = API.createMenuItem("~s~ Создать APPistol : 50 мат.", "");
                    pistol.Activated.connect(function (menu, item) {
                        API.triggerServerEvent("army_two_weapon", 1, 50);
                    });
                    mainWindow2.AddItem(pistol);

                    var smg = API.createMenuItem("~s~ Создать SMG : 100 мат.", "");
                    smg.Activated.connect(function (menu, item) {
                        API.triggerServerEvent("army_two_weapon", 2, 100);
                    });
                    mainWindow2.AddItem(smg);

                    var AdvancedRifle = API.createMenuItem("~s~ Создать AdvancedRifle : 250 мат.", "");
                    AdvancedRifle.Activated.connect(function (menu, item) {
                        API.triggerServerEvent("army_two_weapon", 3, 250);
                    });
                    mainWindow2.AddItem(AdvancedRifle);

                    var HeavySniper = API.createMenuItem("~s~ Создать HeavySniper : 350 мат.", "");
                    HeavySniper.Activated.connect(function (menu, item) {
                        API.triggerServerEvent("army_two_weapon", 4, 350);
                    });
                    mainWindow2.AddItem(HeavySniper);

                    var GrenadeLauncher = API.createMenuItem("~s~ Создать GrenadeLauncher : 500 мат.", "");
                    GrenadeLauncher.Activated.connect(function (menu, item) {
                        API.triggerServerEvent("army_two_weapon", 5, 500);
                    });
                    mainWindow2.AddItem(GrenadeLauncher);

                    var Grenade = API.createMenuItem("~s~ Создать Grenade : 100 мат.", "");
                    Grenade.Activated.connect(function (menu, item) {
                        API.triggerServerEvent("army_two_weapon", 6, 50);
                    });
                    mainWindow2.AddItem(Grenade);

                    var back = API.createMenuItem("~g~Назад", "");
                    back.Activated.connect(function (menu, item) {
                        mainWindow2.Visible = false;
                        mainWindow.Visible = true;
                    });
                    mainWindow2.AddItem(back);
                    mainWindow2.RefreshIndex();
            });
            mainWindow.AddItem(gangWeaponMenu); 
        }
        if (isGangBoss == true) {

            var gangId = 0;

            if (groupId == 1310) gangId = 1301; if (groupId == 1410) gangId = 1401;
            if (groupId == 1510) gangId = 1501; if (groupId == 1610) gangId = 1601;
            if (groupId == 1710) gangId = 1701;

            var gangMenu = API.createMenuItem("~g~Управление ~s~своей фракцией", "");
            gangMenu.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;

                mainWindow2 = API.createMenu("Моя фракция", "~s~" + args[5] + " : " + args[6], 0, 0, 6);
                menuPool2.Add(mainWindow2);
                mainWindow2.Visible = true;

                var gangAdd = API.createMenuItem("~g~Принять~s~ пользователя в банду", "");
                gangAdd.Activated.connect(function (menu, item) {
                    mainWindow2.Visible = false;
                    API.displaySubtitle("Введите ID игрока", 5000);
                    var userId = parseInt(API.getUserInput("", 40));
                    API.triggerServerEvent("gang_ballas_add_to_group", userId, gangId, 1);
                });
                mainWindow2.AddItem(gangAdd);

                var gangDel = API.createMenuItem("~r~Выгнать~s~ пользователя из банды", "");
                gangDel.Activated.connect(function (menu, item) {
                    mainWindow2.Visible = false;
                    API.displaySubtitle("Введите ID игрока", 5000);
                    var userId = parseInt(API.getUserInput("", 40));
                    API.triggerServerEvent("gang_ballas_add_to_group", userId, gangId, 2);
                });
                mainWindow2.AddItem(gangDel);

                var back = API.createMenuItem("~g~Назад", "");
                back.Activated.connect(function (menu, item) {
                    mainWindow2.Visible = false;
                    mainWindow.Visible = true;
                });
                mainWindow2.AddItem(back);
                mainWindow2.RefreshIndex();
            });
            mainWindow.AddItem(gangMenu); 
        }
        

        if (jobId == 777) {
            var taxiStart = API.createMenuItem("~g~Начать работу~s~ таксистом ~g~за 100$ в час", "");
            taxiStart.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
                API.triggerServerEvent("work_taxi", 1);
            });
            if (tempVar == 0) {
                mainWindow.AddItem(taxiStart);
            }

            var taxiBusy = API.createMenuItem("~s~Взять заказ / ~r~Я занят", "");
            taxiBusy.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
                API.triggerServerEvent("work_taxi", 2);
            });
            mainWindow.AddItem(taxiBusy);

            var taxiFree = API.createMenuItem("~s~Заказ выполнен / ~g~Я свободен", "");
            taxiFree.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
                API.triggerServerEvent("work_taxi", 3);
            });
            mainWindow.AddItem(taxiFree);

            var taxiStop = API.createMenuItem("~r~Закончить работу~s~ таксистом", "");
            taxiStop.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
                API.triggerServerEvent("work_taxi", 0);
            });
            mainWindow.AddItem(taxiStop);
        }        
        if (isAdmin == 5) {

            var admin = API.createMenuItem("~g~Админка", "");
            admin.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;

                mainWindow2 = API.createMenu("Админка", "Меню администратора сервера", 0, 0, 6);
                menuPool2.Add(mainWindow2);

                mainWindow2.Visible = true;

                var addToGroup = API.createMenuItem("~s~Добавить пользователя во фракцию", "");
                addToGroup.Activated.connect(function (menu, item) {
                    mainWindow2.Visible = false;
                    
                    API.displaySubtitle("Введите ID игрока", 5000);
                    var userId = parseInt(API.getUserInput("", 40));

                    API.displaySubtitle("Введите ID группы", 5000);
                    var groupId = parseInt(API.getUserInput("", 40));

                    API.triggerServerEvent("admin_add_to_group", userId, groupId);
                });
                mainWindow2.AddItem(addToGroup);

                var addToAdmin = API.createMenuItem("~s~Добавить пользователя в админы", "");
                addToAdmin.Activated.connect(function (menu, item) {
                    mainWindow2.Visible = false;

                    API.displaySubtitle("Введите ID игрока", 5000);
                    var userId = parseInt(API.getUserInput("", 40));

                    API.displaySubtitle("Введите ID группы", 5000);
                    var groupId = parseInt(API.getUserInput("", 40));

                    API.triggerServerEvent("admin_add_to_admin", userId, groupId);
                });
                mainWindow2.AddItem(addToAdmin);

                var chngLevel = API.createMenuItem("~s~Сменить уровень пользователя", "");
                chngLevel.Activated.connect(function (menu, item) {
                    mainWindow2.Visible = false;

                    API.displaySubtitle("Введите ID игрока", 5000);
                    var userId = parseInt(API.getUserInput("", 40));

                    API.displaySubtitle("Введите желаемый уровень", 5000);
                    var level = parseInt(API.getUserInput("", 40));

                    API.triggerServerEvent("admin_change user_level", userId, level);
                });
                mainWindow2.AddItem(chngLevel);

                var back = API.createMenuItem("~g~Назад", "");
                back.Activated.connect(function (menu, item) {
                    mainWindow2.Visible = false;
                    mainWindow.Visible = true;
                });
                mainWindow2.AddItem(back);



                var list = new List(String);
                for (var i = 0; i < 251; i++) {
                    list.Add("" + i);
                }
                var itemSelect = 0;

                var clothes = API.createListItem("Маски", "", list, 0);
                clothes.OnListChanged.connect(function (menu, item) {
                    API.triggerServerEvent("change_clothes", 1, item, 0);
                    itemSelect = item;
                });
                mainWindow2.AddItem(clothes);

                var clothes = API.createListItem("Маски | Цвета", "", list, 0);
                clothes.OnListChanged.connect(function (menu, item) {
                    API.triggerServerEvent("change_clothes", 1, itemSelect, item);
                });
                mainWindow2.AddItem(clothes);

                var clothes = API.createListItem("Торс", "", list, 0);
                clothes.OnListChanged.connect(function (menu, item) {
                    API.triggerServerEvent("change_clothes", 3, item, 0);
                    itemSelect = item;
                });
                mainWindow2.AddItem(clothes);

                var clothes = API.createListItem("Торс | Цвета", "", list, 0);
                clothes.OnListChanged.connect(function (menu, item) {
                    API.triggerServerEvent("change_clothes", 3, itemSelect, item);
                });
                mainWindow2.AddItem(clothes);

                var clothes = API.createListItem("Брюки / шорты", "", list, 0);
                clothes.OnListChanged.connect(function (menu, item) {
                    API.triggerServerEvent("change_clothes", 4, item, 0);
                    itemSelect = item;
                });
                mainWindow2.AddItem(clothes);

                var clothes = API.createListItem("Брюки / шорты | Цвета", "", list, 0);
                clothes.OnListChanged.connect(function (menu, item) {
                    API.triggerServerEvent("change_clothes", 4, itemSelect, item);
                });
                mainWindow2.AddItem(clothes);

                var clothes = API.createListItem("Сумки и рюкзаки", "", list, 0);
                clothes.OnListChanged.connect(function (menu, item) {
                    API.triggerServerEvent("change_clothes", 5, item, 0);
                    itemSelect = item;
                });
                mainWindow2.AddItem(clothes);

                var clothes = API.createListItem("Сумки и рюкзаки | Цвета", "", list, 0);
                clothes.OnListChanged.connect(function (menu, item) {
                    API.triggerServerEvent("change_clothes", 5, itemSelect, item);
                });
                mainWindow2.AddItem(clothes);

                var clothes = API.createListItem("Обувь", "", list, 0);
                clothes.OnListChanged.connect(function (menu, item) { 
                    API.triggerServerEvent("change_clothes", 6, item, 0);
                    itemSelect = item;
                });
                mainWindow2.AddItem(clothes);

                var clothes = API.createListItem("Обувь | Цвета", "", list, 0);
                clothes.OnListChanged.connect(function (menu, item) {
                    API.triggerServerEvent("change_clothes", 6, itemSelect, item);
                });
                mainWindow2.AddItem(clothes);

                var clothes = API.createListItem("Аксессуары", "", list, 0);
                clothes.OnListChanged.connect(function (menu, item) {
                    API.triggerServerEvent("change_clothes", 7, item, 0);
                    itemSelect = item;
                });
                mainWindow2.AddItem(clothes);

                var clothes = API.createListItem("Аксессуары | Цвета", "", list, 0);
                clothes.OnListChanged.connect(function (menu, item) {
                    API.triggerServerEvent("change_clothes", 7, itemSelect, item);
                });
                mainWindow2.AddItem(clothes);

                var clothes2 = API.createListItem("Нижняя одежда", "", list, 0);
                clothes2.OnListChanged.connect(function (menu, item) {
                    API.triggerServerEvent("change_clothes", 8, item, 0);
                    itemSelect = item;
                });
                mainWindow2.AddItem(clothes2);

                var clothes2 = API.createListItem("Нижняя одежда | Цвета", "", list, 0);
                clothes2.OnListChanged.connect(function (menu, item) {
                    API.triggerServerEvent("change_clothes", 8, itemSelect, item);
                });
                mainWindow2.AddItem(clothes2);

                var clothes = API.createListItem("Верхняя одежда", "", list, 0);
                clothes.OnListChanged.connect(function (menu, item) {
                    API.triggerServerEvent("change_clothes", 11, item, 0);
                    itemSelect = item;
                });
                mainWindow2.AddItem(clothes);

                var clothes = API.createListItem("Верхняя одежда | Цвета", "", list, 0);
                clothes.OnListChanged.connect(function (menu, item) {
                    API.triggerServerEvent("change_clothes", 11, itemSelect, item);
                });
                mainWindow2.AddItem(clothes);

                var clothes3 = API.createListItem("Шляпы", "", list, 0);
                clothes3.OnListChanged.connect(function (menu, item) {
                    API.triggerServerEvent("change_accessory", 0, item, 0);
                    itemSelect = item;
                });
                mainWindow2.AddItem(clothes3);

                var clothes3 = API.createListItem("Шляпы | Цвета", "", list, 0);
                clothes3.OnListChanged.connect(function (menu, item) {
                    API.triggerServerEvent("change_accessory", 0, itemSelect, item);
                });
                mainWindow2.AddItem(clothes3);

                var clothes3 = API.createListItem("Очки", "", list, 0);
                clothes3.OnListChanged.connect(function (menu, item) {
                    API.triggerServerEvent("change_accessory", 1, item, 0);
                });
                mainWindow2.AddItem(clothes3);

                var close = API.createMenuItem("~r~Закрыть", "");
                close.Activated.connect(function (menu, item) {
                    mainWindow2.Visible = false;
                });
                mainWindow2.AddItem(close);

                mainWindow2.RefreshIndex();
            });
            mainWindow.AddItem(admin); 
        }

        var close = API.createMenuItem("~r~Закрыть", "");
        close.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
        });
        mainWindow.AddItem(close);

        mainWindow.Visible = true;
        mainWindow.RefreshIndex();
    }
    if (name == "vehicle_menu")
    {
        resetMainMenu();
        var callbackId = args[0];
        var banner = args[1];
        var subtitle = args[2];
        var engineStatus = args[3];
        
        if (banner == null) mainWindow = API.createMenu(subtitle, 0, 0, 6);
        else mainWindow = API.createMenu(banner, subtitle, 0, 0, 6);
        menuPool.Add(mainWindow);
        
        if (engineStatus == 0) {
            var engine = API.createMenuItem("~g~Завести~s~ двигатель", "");
            engine.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
                API.triggerServerEvent("engine_on");
            });
        }
        else {
            var engine = API.createMenuItem("~r~Заглушить~s~ двигатель", "");
            engine.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
                API.triggerServerEvent("engine_off");
            });
        }
        if (args[5] == true) mainWindow.AddItem(engine);        

        var park = API.createMenuItem("~y~Припарковать~s~ транспорт", "");
        park.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
            API.triggerServerEvent("park_vehicle");
        });
        if (args[5] == true) mainWindow.AddItem(park);

        if (args[6] == 0) {
            var driverDoor = API.createMenuItem("~g~Открыть~s~ водительскую дверь", "");
            driverDoor.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
                API.triggerServerEvent("driver_door", 1);
            });
        }
        else {
            var driverDoor = API.createMenuItem("~r~Закрыть~s~ водительскую дверь", "");
            driverDoor.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
                API.triggerServerEvent("driver_door", 0);
            });
        }
        mainWindow.AddItem(driverDoor);

        var hood = API.createMenuItem("~r~Закрыть~s~ / ~g~Открыть~s~ капот", "");
        hood.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
            API.triggerServerEvent("hood_trunk", 1);
        });
        mainWindow.AddItem(hood);

        var trunk = API.createMenuItem("~r~Закрыть~s~ / ~g~Открыть~s~ багажник", "");
        trunk.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
            API.triggerServerEvent("hood_trunk", 0);
        });
        mainWindow.AddItem(trunk);

        var fuel = API.createMenuItem("Топливо", "Топлива в баке: ~g~" + args[4]);
        if (args[5] == true) mainWindow.AddItem(fuel);

        var material = API.createMenuItem("Материал", "Материалов: ~g~" + args[7]);
        mainWindow.AddItem(material);

        var close = API.createMenuItem("~r~Закрыть", "");
        close.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
        });
        mainWindow.AddItem(close);

        mainWindow.Visible = true;
        mainWindow.RefreshIndex();
    }
    if (name == "login_char_menu") {
        resetMainMenu();
        var callbackId = args[0];
        var banner = "GTA: MY LIFE";
        var subtitle = "Введите ваш пароль";

        if (banner == null) mainWindow = API.createMenu(subtitle, 0, 0, 6);
        else mainWindow = API.createMenu(banner, subtitle, 0, 0, 6);
        menuPool.Add(mainWindow);

        var create = API.createMenuItem("~g~Войти в игру", "");
        create.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
            API.displaySubtitle("Введите Ваш пароль", 5000);
            API.triggerServerEvent("enter_pwd");
            var pwd = API.getUserInput("", 40);
            API.triggerServerEvent("login_char", pwd);
        });
        mainWindow.AddItem(create);

        var close = API.createMenuItem("~r~Закрыть", "");
        close.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
        });
        mainWindow.AddItem(close);  

        mainWindow.Visible = true;
        mainWindow.RefreshIndex();
    }
    if (name == "create_char_menu") {
        resetMainMenu();
        var callbackId = args[0];
        var banner = "GTA: MY LIFE";
        var subtitle = "Введите Имя_Фамилию (через _)";

        if (banner == null) mainWindow = API.createMenu(subtitle, 0, 0, 6);
        else mainWindow = API.createMenu(banner, subtitle, 0, 0, 6);
        menuPool.Add(mainWindow);

        var create = API.createMenuItem("~g~Создать нового персонажа", "");
        create.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
            API.displaySubtitle("Введите Ваше Имя_Фамилию", 5000);
            API.triggerServerEvent("enter_login");            
            var name = API.getUserInput("", 40);

            API.displaySubtitle("Введите Ваш пароль", 5000);
            API.triggerServerEvent("enter_pwd");
            var pwd = API.getUserInput("", 40);
            API.triggerServerEvent("create_char", name, pwd); 
        });
        mainWindow.AddItem(create);    

        var close = API.createMenuItem("~r~Закрыть", "");
        close.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
        });
        mainWindow.AddItem(close);  

        mainWindow.Visible = true;
        mainWindow.RefreshIndex();
    }
    if (name == "rent_finish_menu") {
        resetMainMenu();
        var callbackId = args[0];
        var banner = args[1];
        var subtitle = args[2];
        var vehicleModel = args[3];

        if (banner == null) mainWindow = API.createMenu(subtitle, 0, 0, 6);
        else mainWindow = API.createMenu(banner, subtitle, 0, 0, 6);
        menuPool.Add(mainWindow);

        var prolongate = API.createMenuItem("~g~Продлить~s~ прокат", "");
        prolongate.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
            API.triggerServerEvent("rent_prolong", 1, vehicleModel);
        });
        mainWindow.AddItem(prolongate);

        var close = API.createMenuItem("~r~Не продлевать и закрыть", "");
        close.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
            API.triggerServerEvent("rent_prolong", 0, vehicleModel);
        });
        mainWindow.AddItem(close);

        mainWindow.Visible = true;
        mainWindow.RefreshIndex();
    }
    if (name == "scooter_rent_menu")
    {
        resetMainMenu();
        var callbackId = args[0];
        var banner = args[1];
        var subtitle = args[2];
        var noExit = args[3];
        
        if (banner == null) mainWindow = API.createMenu(subtitle, 0, 0, 6);
        else mainWindow = API.createMenu(banner, subtitle, 0, 0, 6);
        menuPool.Add(mainWindow);

        if (noExit)
        {
            mainWindow.ResetKey(menuControl.Back);
        }

        var items = args[4];
        var payAndGo = API.createMenuItem(items, "");
        payAndGo.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
            API.triggerServerEvent("rent_scooter", 0);
        });
        mainWindow.AddItem(payAndGo);  

        var close = API.createMenuItem("~r~Закрыть", "");
        close.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
        });
        mainWindow.AddItem(close);   

        if (callbackId == 0)
        {
            close.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
            });
        }
        else mainWindow.Visible = true;
        mainWindow.RefreshIndex();
    }
    if (name == "gang_menu") {
        resetMainMenu();
        var callbackId = args[0];
        var banner = args[1];
        var subtitle = args[2];
        var propertyName = args[3];
        var cloth = args[4];

        if (propertyName == "Army2_gang") {
            subtitle = "Украдите материалы у Армии 2";
        }
        if (propertyName == "Army1_gang") {
            subtitle = "Украдите материалы у Армии 1";
        }
        if (propertyName == "Army2_stock") {
            subtitle = "Украдите материалы со склада Армии 2";
        }
        if (propertyName == "Army1_stock") {
            subtitle = "Украдите материалы со склада Армии 1";
        }
        if (propertyName == "Ballas_stock") {
            banner = "Банда Ballas";
            subtitle = "Разгрузите украденные материалы.";
        }
        if (propertyName == "Ballas_main") {
            banner = "Банда Ballas";
            subtitle = "Ваша база.";
        }
        if (banner == null) mainWindow = API.createMenu(subtitle, 0, 0, 6);
        else mainWindow = API.createMenu(banner, subtitle, 0, 0, 6);
        menuPool.Add(mainWindow);

        if (propertyName == "Army2_gang" || propertyName == "Army1_gang") {
            var steal = API.createMenuItem("~s~Украсть 500 материалов", "");
        steal.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
            API.triggerServerEvent("gang_menu", propertyName, 1);
        });
        mainWindow.AddItem(steal);
        }   

        if (propertyName == "Army2_stock" || propertyName == "Army1_gang") {
            var steal = API.createMenuItem("~s~Украсть 1000 материалов", "");
            steal.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
                API.triggerServerEvent("gang_menu", propertyName, 2);
            });
            mainWindow.AddItem(steal);
        }  

        if (propertyName == "Ballas_stock") {
            var unload = API.createMenuItem("~g~Разгрузить материалы", "");
            unload.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
                API.triggerServerEvent("gang_menu", propertyName, 3);
            });
            mainWindow.AddItem(unload);
        }  

        if (propertyName == "Ballas_main") {
            var clothName = "";
            if (cloth == 3) clothName = "солдата";
            if (cloth == 4) clothName = "офицера";
            if (cloth == 5) clothName = "генерала";
            if (cloth == 10) clothName = "полицейского";

            if (cloth != 0) {
                var unload = API.createMenuItem("~g~Одеть форму " + clothName, "");
                unload.Activated.connect(function (menu, item) {
                    mainWindow.Visible = false;
                    API.triggerServerEvent("gang_menu", propertyName, 4);
                });
                mainWindow.AddItem(unload);
            }   

            var unload = API.createMenuItem("~g~Одеть форму своей банды", "");
            unload.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
                API.triggerServerEvent("gang_menu", propertyName, 5);
            });
            mainWindow.AddItem(unload);
        }  

        if (propertyName == "Ballas_weapon") {
            var pistol = API.createMenuItem("~s~ Взять APPistol : 50 мат.", "");
            pistol.Activated.connect(function (menu, item) {
                API.triggerServerEvent("ballas_weapon", 1, 50);
            });
            mainWindow.AddItem(pistol);

            var smg = API.createMenuItem("~s~ Взять SMG : 100 мат.", "");
            smg.Activated.connect(function (menu, item) {
                API.triggerServerEvent("ballas_weapon", 2, 100);
            });
            mainWindow.AddItem(smg);

            var AdvancedRifle = API.createMenuItem("~s~ Взять AdvancedRifle : 250 мат.", "");
            AdvancedRifle.Activated.connect(function (menu, item) {
                API.triggerServerEvent("ballas_weapon", 3, 250);
            });
            mainWindow.AddItem(AdvancedRifle);

            var HeavySniper = API.createMenuItem("~s~ Взять HeavySniper : 350 мат.", "");
            HeavySniper.Activated.connect(function (menu, item) {
                API.triggerServerEvent("ballas_weapon", 4, 350);
            });
            mainWindow.AddItem(HeavySniper);

            var GrenadeLauncher = API.createMenuItem("~s~ Взять GrenadeLauncher : 500 мат.", "");
            GrenadeLauncher.Activated.connect(function (menu, item) {
                API.triggerServerEvent("ballas_weapon", 5, 500);
            });
            mainWindow.AddItem(GrenadeLauncher);

            var Grenade = API.createMenuItem("~s~ Взять Grenade : 100 мат.", "");
            Grenade.Activated.connect(function (menu, item) {
                API.triggerServerEvent("ballas_weapon", 6, 50);
            });
            mainWindow.AddItem(Grenade);
        }

        var close = API.createMenuItem("~r~Закрыть", "");
        close.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
        });
        mainWindow.AddItem(close);

        if (callbackId == 0) {
            close.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
            });
        }
        else mainWindow.Visible = true;
        mainWindow.RefreshIndex();
    }

    if (name == "army_menu") {
        resetMainMenu();
        var callbackId = args[0];
        var banner = args[1];
        var subtitle = args[2];
        var propertyName = args[3];
        var access = args[4];

        if (propertyName == "Army2_weapon"){
            banner = "Взять оружие";
            subtitle = "Выберите нужное вам оружие:";
        }
        if (banner == null) mainWindow = API.createMenu(subtitle, 0, 0, 6);
        else mainWindow = API.createMenu(banner, subtitle, 0, 0, 6);
        menuPool.Add(mainWindow);       

        if (propertyName == "Army1_source" && access == 1) {
            var loadFromStock = API.createMenuItem("~g~Загрузить ~s~материалы", "");
            loadFromStock.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
                API.triggerServerEvent("army_two_menu", 3, propertyName, "Army2_stock");
            });
            mainWindow.AddItem(loadFromStock);            
        }
        if (propertyName == "Army1_stock" && access == 2) {
            var loadToStock = API.createMenuItem("~g~Разгрузить ~s~материалы для ~y~Армии 1", "");
            loadToStock.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
                API.triggerServerEvent("army_two_menu", 2, propertyName, "Police_stock");
            });
            mainWindow.AddItem(loadToStock);
        }
        if (access == 1) {
            if (propertyName == "Army2_main" || propertyName == "Army1_main") {
                var cloth = API.createMenuItem("~g~Одеть ~s~военную форму", "");
                cloth.Activated.connect(function (menu, item) {
                    mainWindow.Visible = false;
                    API.triggerServerEvent("army_two_menu", 4, propertyName, "Cloth_up");
                });
                mainWindow.AddItem(cloth);
            }
        }
        if (access == 1) {
            if (propertyName == "Army2_main" || propertyName == "Army1_main") {
                var cloth = API.createMenuItem("~r~Снять ~s~военную форму", "");
                cloth.Activated.connect(function (menu, item) {
                    mainWindow.Visible = false;
                    API.triggerServerEvent("army_two_menu", 4, propertyName, "Cloth_down");
                });
                mainWindow.AddItem(cloth);
            }
        } 
        if (propertyName == "Army2_stock" && access == 1) {
            var loadFromStock = API.createMenuItem("~g~Загрузить ~s~материалы для ~y~Полиции", "");
            loadFromStock.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
                API.triggerServerEvent("army_two_menu", 1, propertyName, "Police_stock");
            });
            mainWindow.AddItem(loadFromStock);

            var loadFromStock = API.createMenuItem("~g~Загрузить ~s~материалы для ~y~ФБР", "");
            loadFromStock.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
                API.triggerServerEvent("army_two_menu", 1, propertyName, "FBI_stock");
            });
            mainWindow.AddItem(loadFromStock);

            var loadFromStock = API.createMenuItem("~g~Загрузить ~s~материалы для ~y~Армии 1", "");
            loadFromStock.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
                API.triggerServerEvent("army_two_menu", 1, propertyName, "Army1_stock");
            });
            mainWindow.AddItem(loadFromStock);
        }
        if (propertyName == "Army2_stock" && access == 2) {
            var loadToStock = API.createMenuItem("~g~Разгрузить ~s~материалы для ~y~Армии 2", "");
            loadToStock.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
                API.triggerServerEvent("army_two_menu", 2, propertyName, "Police_stock");
            });
            mainWindow.AddItem(loadToStock);
        }

        if (propertyName == "Police_stock" && access == 1) {
            var unloadPolice = API.createMenuItem("~g~Разгрузить ~s~материалы на склад", "");
            unloadPolice.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
                API.triggerServerEvent("army_two_menu", 2, propertyName, "Police_stock");
            });
            mainWindow.AddItem(unloadPolice);
        }

        if (propertyName == "Army2_weapon") {
            var pistol = API.createMenuItem("~s~ Взять APPistol : 50 мат.", "");
            pistol.Activated.connect(function (menu, item) {
                API.triggerServerEvent("army_two_weapon", 1, 50);
            });
            mainWindow.AddItem(pistol);

            var smg = API.createMenuItem("~s~ Взять SMG : 100 мат.", "");
            smg.Activated.connect(function (menu, item) {
                API.triggerServerEvent("army_two_weapon", 2, 100);
            });
            mainWindow.AddItem(smg);

            var AdvancedRifle = API.createMenuItem("~s~ Взять AdvancedRifle : 250 мат.", "");
            AdvancedRifle.Activated.connect(function (menu, item) {
                API.triggerServerEvent("army_two_weapon", 3, 250);
            });
            mainWindow.AddItem(AdvancedRifle);

            var HeavySniper = API.createMenuItem("~s~ Взять HeavySniper : 350 мат.", "");
            HeavySniper.Activated.connect(function (menu, item) {                
                API.triggerServerEvent("army_two_weapon", 4, 350);
            });
            mainWindow.AddItem(HeavySniper);

            var GrenadeLauncher = API.createMenuItem("~s~ Взять GrenadeLauncher : 500 мат.", "");
            GrenadeLauncher.Activated.connect(function (menu, item) {
                API.triggerServerEvent("army_two_weapon", 5, 500);
            });
            mainWindow.AddItem(GrenadeLauncher);

            var Grenade = API.createMenuItem("~s~ Взять Grenade : 100 мат.", "");
            Grenade.Activated.connect(function (menu, item) {
                API.triggerServerEvent("army_two_weapon", 6, 50);
            });
            mainWindow.AddItem(Grenade);
        }

        var close = API.createMenuItem("~r~Закрыть", "");
        close.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
        });
        mainWindow.AddItem(close);

        if (callbackId == 0) {
            close.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
            });
        }
        else mainWindow.Visible = true;
        mainWindow.RefreshIndex();
    }

    if (name == "work_meria_menu")
    {
        resetMainMenu();
        var callbackId = args[0];
        var banner = args[1];
        var subtitle = args[2];
        var userLevel = args[3];

        if (banner == null) mainWindow = API.createMenu(subtitle, 0, 0, 6);
        else mainWindow = API.createMenu(banner, subtitle, 0, 0, 6);
        menuPool.Add(mainWindow);

        var getWork = API.createMenuItem("Устроиться на работу", "");
        getWork.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;

            mainWindow2 = API.createMenu("Доступные работы", "Выбирайте работы по вашему уровню:", 0, 0, 6);
            menuPool2.Add(mainWindow2);

            mainWindow2.Visible = true;

            var work = API.createMenuItem("Простая работа", "");
            work.Activated.connect(function (menu, item) {
                mainWindow2.Visible = false;

                mainWindow3 = API.createMenu("Простая работа", "", 0, 0, 6);
                menuPool3.Add(mainWindow3);

                mainWindow3.Visible = true;

                var loader1 = API.createMenuItem("Грузчик 1: Уровень 0", "");
                loader1.Activated.connect(function (menu, item) {
                    mainWindow3.Visible = false;
                    API.displaySubtitle("Метка первой работы на карте установлена", 5000);
                    waypoint = API.setWaypoint(-144, -948);
                });
                mainWindow3.AddItem(loader1);

                var loader2 = API.createMenuItem("Грузчик 2: Уровень 0", "");
                loader2.Activated.connect(function (menu, item) {
                    mainWindow3.Visible = false;
                    API.displaySubtitle("Метка первой работы на карте установлена", 5000);
                    waypoint = API.setWaypoint(853, -2927);
                });
                mainWindow3.AddItem(loader2);

                var loader = API.createMenuItem("Таксист : Уровень 2", "");
                loader.Activated.connect(function (menu, item) {
                    mainWindow3.Visible = false;
                    API.triggerServerEvent("work_taxi", 4);                  
                });
                mainWindow3.AddItem(loader);

                var back = API.createMenuItem("~g~Назад", "");
                back.Activated.connect(function (menu, item) {
                    mainWindow3.Visible = false;
                    mainWindow2.Visible = true;
                });
                mainWindow3.AddItem(back);

                mainWindow3.RefreshIndex();
            });
            mainWindow2.AddItem(work);

            var back = API.createMenuItem("~g~Назад", "");
            back.Activated.connect(function (menu, item) {
                mainWindow2.Visible = false;
                mainWindow.Visible = true;
            });
            mainWindow2.AddItem(back);

            var close = API.createMenuItem("~r~Закрыть", "");
            close.Activated.connect(function (menu, item) {
                mainWindow2.Visible = false;
            });
            mainWindow2.AddItem(close);
            mainWindow2.RefreshIndex();
        });
        mainWindow.AddItem(getWork);  

        var unemplyers = API.createMenuItem("~g~Пособие~s~ по безработице", "");
        unemplyers.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
            API.triggerServerEvent("work_unemployers");
        });
        mainWindow.AddItem(unemplyers);

        var close = API.createMenuItem("~r~Закрыть", "");
        close.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
        });
        mainWindow.AddItem(close);

        if (callbackId == 0) {
            close.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
            });
        }
        else mainWindow.Visible = true;
        mainWindow.RefreshIndex();
    }
    if (name == "work_loader_menu") {
        resetMainMenu();
        var callbackId = args[0];
        var banner = args[1];
        var subtitle = args[2];
        var jobId = args[3];
        var posX = args[4];
        var posY = args[5];
        var posZ = args[6];

        if (banner == null) mainWindow = API.createMenu(subtitle, 0, 0, 6);
        else mainWindow = API.createMenu(banner, subtitle, 0, 0, 6);
        menuPool.Add(mainWindow);

        var start = API.createMenuItem("~g~Начать~s~ работу", "");
        start.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
            API.triggerServerEvent("work_loader", 1, jobId, posX, posY, posZ);
        });
        mainWindow.AddItem(start);

        var close = API.createMenuItem("~r~Закончить~s~ работу", "");
        close.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
            API.triggerServerEvent("work_loader", 0, jobId, posX, posY, posZ);
        });
        mainWindow.AddItem(close);

        if (callbackId == 0) {
            close.Activated.connect(function (menu, item) {
                API.triggerServerEvent("work_loader", 0);
                mainWindow.Visible = false;
            });
        }
        else mainWindow.Visible = true;
        mainWindow.RefreshIndex();
    }
    if (name == "autoschool_menu") {
        resetMainMenu();
        var callbackId = args[0];
        var banner = args[1];
        var subtitle = args[2];

        if (banner == null) mainWindow = API.createMenu(subtitle, 0, 0, 6);
        else mainWindow = API.createMenu(banner, subtitle, 0, 0, 6);
        menuPool.Add(mainWindow);

        var start = API.createMenuItem("~g~Купить~s~ права за 50$", "");
        start.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
            API.triggerServerEvent("buy_driver_license", 1);
        });
        mainWindow.AddItem(start);

        var close = API.createMenuItem("~r~Закрыть", "");
        close.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
        });
        mainWindow.AddItem(close);

        if (callbackId == 0) {
            close.Activated.connect(function (menu, item) {
                API.triggerServerEvent("buy_driver_license", 0);
                mainWindow.Visible = false;
            });
        }
        else mainWindow.Visible = true;
        mainWindow.RefreshIndex();
    }
    if (name == "create_menu")
    {
        resetMainMenu();
        var callbackId = args[0];
        var banner = args[1];
        var subtitle = args[2];
        var noExit = args[3];
        
        if (banner == null) mainWindow = API.createMenu(subtitle, 0, 0, 6);
        else mainWindow = API.createMenu(banner, subtitle, 0, 0, 6);
        menuPool.Add(mainWindow);

        if (noExit)
        {
            mainWindow.ResetKey(menuControl.Back);
        }

        var items = args[4];
        for (var i = 0; i < items.Count; i++)
        {
            var listItem = API.createMenuItem(items[i], "");
            mainWindow.AddItem(listItem);
        }
        
        mainWindow.RefreshIndex();

        mainWindow.OnItemSelect.connect(function (sender, item, index)
        {
            API.triggerServerEvent("menu_handler_select_item", callbackId, index, items.Count);
            mainWindow.Visible = false;
        });

        mainWindow.Visible = true;
    }
    if (name == "markonmap") {
        API.setWaypoint(args[0], args[1]);
    }
});

function resetMainMenu() {
    if (mainWindow != null)
        mainWindow.Visible = false;
    if (mainWindow2 != null)
        mainWindow2.Visible = false;
    if (mainWindow3 != null)
        mainWindow3.Visible = false;

    mainWindow = null;
    mainWindow2 = null;
    mainWindow3 = null;

    menuPool = null;
    menuPool2 = null;
    menuPool3 = null;

    menuPool = API.getMenuPool();
    menuPool2 = API.getMenuPool();
    menuPool3 = API.getMenuPool();
}
API.onUpdate.connect(function () {

    if (pool != null) {
        pool.ProcessMenus();
    }
    if (menuPool != null) {
        menuPool.ProcessMenus();
    }
    if (menuPool2 != null) {
        menuPool2.ProcessMenus();
    }
    if (menuPool3 != null) {
        menuPool3.ProcessMenus();
    }
});