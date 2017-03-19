var menuPool = null;
var draw = false;


API.onResourceStart.connect(function () {

});

API.onServerEventTrigger.connect(function (name, args) {

    if (name == "vehicle_menu") {
        var callbackId = args[0];
        var banner = args[1];
        var subtitle = args[2];
        var engineStatus = args[3];

        var menu = null;
        if (banner == null) menu = API.createMenu(subtitle, 0, 0, 6);
        else menu = API.createMenu(banner, subtitle, 0, 0, 6);
        
        if (engineStatus == 0) {
            var engine = API.createMenuItem("~g~Завести~s~ двигатель", "");
            engine.Activated.connect(function (menu, item) {
                menu.Visible = false;
                API.triggerServerEvent("engine_on");
            });
        }
        else {
            var engine = API.createMenuItem("~r~Заглушить~s~ двигатель", "");
            engine.Activated.connect(function (menu, item) {
                menu.Visible = false;
                API.triggerServerEvent("engine_off");
            });
        }
        menu.AddItem(engine);

        var fuel = API.createMenuItem("Топливо", "Топлива в баке: ~g~" + args[4]);
        menu.AddItem(fuel);

        var park = API.createMenuItem("~y~Припарковать~s~ транспорт", "");
        park.Activated.connect(function (menu, item) {
            menu.Visible = false;
            API.triggerServerEvent("park_vehicle");
        });
        menu.AddItem(park);

        var hood = API.createMenuItem("~r~Закрыть~s~ / ~g~Открыть~s~ капот", "");
        hood.Activated.connect(function (menu, item) {
            menu.Visible = false;
            API.triggerServerEvent("hood_trunk", 1);
        });
        menu.AddItem(hood);

        var trunk = API.createMenuItem("~r~Закрыть~s~ / ~g~Открыть~s~ багажник", "");
        trunk.Activated.connect(function (menu, item) {
            menu.Visible = false;
            API.triggerServerEvent("hood_trunk", 0);
        });
        menu.AddItem(trunk);

        var close = API.createMenuItem("~r~Закрыть", "");
        close.Activated.connect(function (menu, item) {
            menu.Visible = false;
        });
        menu.AddItem(close);

        API.onUpdate.connect(function () {
            API.drawMenu(menu);
        });

        menu.Visible = true;
        menu.RefreshIndex();
    }

    if (name == "scooter_rent_menu")
    {
        var callbackId = args[0];
        var banner = args[1];
        var subtitle = args[2];
        var noExit = args[3];

        var menu = null;
        if (banner == null) menu = API.createMenu(subtitle, 0, 0, 6);
        else menu = API.createMenu(banner, subtitle, 0, 0, 6);

        if (noExit)
        {
            menu.ResetKey(menuControl.Back);
        }

        var items = args[4];
        var payAndGo = API.createMenuItem(items, "");
        payAndGo.Activated.connect(function (menu, item) {
            menu.Visible = false;
            API.triggerServerEvent("rent_scooter", 0);
        });
        menu.AddItem(payAndGo);  

        var close = API.createMenuItem("~r~Закрыть", "");
        close.Activated.connect(function (menu, item) {
            menu.Visible = false;
        });
        menu.AddItem(close);   

        API.onUpdate.connect(function () {
            API.drawMenu(menu);
        });

        if (callbackId == 0)
        {
            close.Activated.connect(function (menu, item) {
                menu.Visible = false;
            });
        }
        else menu.Visible = true;
        menu.RefreshIndex();
    }

    if (name == "create_menu")
    {
        var callbackId = args[0];
        var banner = args[1];
        var subtitle = args[2];
        var noExit = args[3];

        var menu = null;
        if (banner == null) menu = API.createMenu(subtitle, 0, 0, 6);
        else menu = API.createMenu(banner, subtitle, 0, 0, 6);

        if (noExit)
        {
            menu.ResetKey(menuControl.Back);
        }

        var items = args[4];
        for (var i = 0; i < items.Count; i++)
        {
            var listItem = API.createMenuItem(items[i], "");
            menu.AddItem(listItem);
        }
        
        menu.RefreshIndex();

        menu.OnItemSelect.connect(function (sender, item, index)
        {
            API.triggerServerEvent("menu_handler_select_item", callbackId, index, items.Count);
            menu.Visible = false;
        });

        API.onUpdate.connect(function ()
        {
            API.drawMenu(menu);
        });

        menu.Visible = true;
    }
});


API.onUpdate.connect(function (sender, args)
{
    if (menuPool != null) menuPool.ProcessMenus();
});