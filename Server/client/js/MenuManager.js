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

        if (banner == null) mainWindow = API.createMenu(subtitle, 0, 0, 6);
        else mainWindow = API.createMenu(banner, subtitle, 0, 0, 6);
        menuPool.Add(mainWindow);

        var gps = API.createMenuItem("GPS", "");
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

                var meria = API.createMenuItem("Грузчик", "");
                meria.Activated.connect(function (menu, item) {
                    mainWindow3.Visible = false;
                    API.displaySubtitle("Метка на карте установлена", 5000);
                    waypoint = API.setWaypoint(-144, -948);
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

        var fuel = API.createMenuItem("Топливо", "Топлива в баке: ~g~" + args[4]);
        if (args[5] == true) mainWindow.AddItem(fuel);

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

        if (banner == null) mainWindow = API.createMenu(subtitle, 0, 0, 6);
        else mainWindow = API.createMenu(banner, subtitle, 0, 0, 6);
        menuPool.Add(mainWindow);

        var prolongate = API.createMenuItem("~g~Продлить~s~ прокат", "");
        prolongate.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
            API.triggerServerEvent("rent_prolong", 1);
        });
        mainWindow.AddItem(prolongate);

        var close = API.createMenuItem("~r~Не продлевать и закрыть", "");
        close.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
            API.triggerServerEvent("rent_prolong", 0);
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

    if (name == "work_loader_menu") {
        resetMainMenu();
        var callbackId = args[0];
        var banner = args[1];
        var subtitle = args[2];
        var propertyID = args[3];

        if (banner == null) mainWindow = API.createMenu(subtitle, 0, 0, 6);
        else mainWindow = API.createMenu(banner, subtitle, 0, 0, 6);
        menuPool.Add(mainWindow);

        var start = API.createMenuItem("~g~Начать~s~ работу", "");
        start.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
            API.triggerServerEvent("work_loader", 1, propertyID);
        });
        mainWindow.AddItem(start);

        var close = API.createMenuItem("~r~Закончить~s~ работу", "");
        close.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
            API.triggerServerEvent("work_loader", 0, propertyID);
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