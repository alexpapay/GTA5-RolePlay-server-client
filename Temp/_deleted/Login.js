var loginBrowser = null;
var width = 380;
var height = 550;
var charSelectMenu = null;

charSelectMenu = API.createMenu("Выберите персонажа", 0, 0, 6);
charSelectMenu.ResetKey(menuControl.Back);

charSelectMenu.OnItemSelect.connect(function(sender, item, index)
{
    API.triggerServerEvent("account_selected", index + 1);
    API.sendNotification("Вы выбрали персонажа по имени ~b~" + item.Text + "~w~.");
    charSelectMenu.Visible = false;
});


API.onServerEventTrigger.connect(function(eventName, args)
{
    if (eventName === "account_charlist")
    {
        var characterNum = args[0];

        charSelectMenu.Clear();

        for (var i = 0; i < characterNum; i++)
        {
            var charname = args[i + 1];
            var charItem = API.createMenuItem(charname,
                "Вы должны выбрать каким персонажем вы бы хотели играть.");
            charSelectMenu.AddItem(charItem);
        }

        charSelectMenu.Visible = true;
        API.sendNotification("~r~Пожалуйства выберите своего персонажа");
    }
});


API.onUpdate.connect(function(sender, args)
{
    API.drawMenu(charSelectMenu);
});
