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

var userSector = "0;0";
var gangMap = 0;
var playerX = 0;
var playerY = 0;
var gangsSectors = new Array();

var faceJson = { SEX: 1885233650, GTAO_SHAPE_FIRST_ID: 0, GTAO_SHAPE_SECOND_ID: 0, GTAO_SKIN_FIRST_ID: 0, GTAO_HAIR: 0, GTAO_HAIR_COLOR: 0, GTAO_EYE_COLOR: 0, GTAO_EYEBROWS: 0, GTAO_EYEBROWS_COLOR: 0 };

API.onResourceStart.connect(function () {

});

API.onServerEventTrigger.connect(function (name, args) {

    if (name == "character_menu")
    {
        resetMainMenu();
        var callbackId = args[0];
        var banner = args[1];
        var subtitle = args[2];
        var myAge = args[3];
        var myLevel = args[4];
        var myJob = args[5];
        var moneyInBank = args[6];
        var myDriverLicense = args[7];
        var myDebt = args[8];
        var myFraction = args[9];
        var myRangFraction = args[10];
        var myMaterials = args[11];
        var IsCharacterInMafia = args[12];
        var IsCharacterInGang = args[13];

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

        if (myDebt != 0) {
            var takeDebt = API.createMenuItem("~y~Вернуть~s~ долг", "~r~Мой долг:~s~ " + myDebt);
            takeDebt.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
                API.displaySubtitle("Введите сумму для возврата", 5000);
                var debtSize = parseInt(API.getUserInput("", 40));
                API.triggerServerEvent("take_debt", debtSize);
            });
            mainWindow.AddItem(takeDebt); 
        }        

        var age = API.createMenuItem("Ваш возраст:~s~ " + myAge, "");
        age.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
        });
        mainWindow.AddItem(age);

        var level = API.createMenuItem("Ваш уровень:~s~ " + myLevel, "");
        level.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
        });
        mainWindow.AddItem(level);

        var job = API.createMenuItem("Ваша работа:~s~ " + myJob, "");
        job.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
        });
        mainWindow.AddItem(job);   

        var fraction = API.createMenuItem("Ваша фракция:~s~ " + myFraction, "");
        fraction.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
        });
        mainWindow.AddItem(fraction);  

        var job = API.createMenuItem("Ваша должность:~s~ " + myRangFraction, "");
        job.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
        });
        mainWindow.AddItem(job); 

        var driverLicense = API.createMenuItem("Наличие прав:~s~ " + myDriverLicense, "");
        driverLicense.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
        });
        mainWindow.AddItem(driverLicense);  

        if (IsCharacterInGang == true) {
            var material = API.createMenuItem("Ваши материалы:~s~ " + myMaterials, "");
            material.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
            });
            mainWindow.AddItem(material); 
        }

        var bank = API.createMenuItem("Денег в банке:~s~ " + moneyInBank, "");
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
    if (name == "face_custom") {
        var banner = "Внешний вид";
        var subtitle = "Test";

        if (banner == null) mainWindow = API.createMenu(subtitle, 0, 0, 6);
        else mainWindow = API.createMenu(banner, subtitle, 0, 0, 6);
        menuPool.Add(mainWindow);

        var listGender = new List(String);
        listGender.Add("Mужской");
        listGender.Add("Женский");

        var gender_select = API.createListItem("Выберите пол", "Выберите пол персонажа: ", listGender, 0);
        mainWindow.AddItem(gender_select);
        gender_select.OnListChanged.connect(function (sender, new_index) { 
            if (new_index == 0) {
                faceJson["SEX"] = 1885233650;
                API.setPlayerSkin(1885233650);
            }
            if (new_index == 1) {
                faceJson["SEX"] = -1667301416;
                API.setPlayerSkin(-1667301416);
            }
        });              

        var list = new List(String);
        list = new List(String); for (var i = 0; i < 46; i++) { list.Add("" + i); }
        var face_form = API.createListItem("Форма лица", "", list, 0);
        face_form.OnListChanged.connect(function (sender, new_index) {
            faceJson["GTAO_SHAPE_FIRST_ID"] = new_index;
            API.triggerServerEvent("change_face", "GTAO_SHAPE_FIRST_ID", new_index); 
        });   
        mainWindow.AddItem(face_form);

        list = new List(String); for (var i = 0; i < 41; i++) { list.Add("" + i); }
        var face_addit = API.createListItem("Порезы, шрамы", "", list, 0);
        face_addit.OnListChanged.connect(function (sender, new_index) {
            faceJson["GTAO_SHAPE_SECOND_ID"] = new_index;
            API.triggerServerEvent("change_face", "GTAO_SHAPE_SECOND_ID", new_index);
        });
        mainWindow.AddItem(face_addit);

        list = new List(String); for (var i = 0; i < 12; i++) { list.Add("" + i); }
        var skin_color = API.createListItem("Цвет кожи", "", list, 0);
        skin_color.OnListChanged.connect(function (sender, new_index) {
            faceJson["GTAO_SKIN_FIRST_ID"] = new_index;
            API.triggerServerEvent("change_face", "GTAO_SKIN_FIRST_ID", new_index);
        });
        mainWindow.AddItem(skin_color);        

        list = new List(String); for (var i = 0; i < 37; i++) { list.Add("" + i); }
        var hairs = API.createListItem("Волосы", "", list, 0);
        hairs.OnListChanged.connect(function (sender, new_index) {
            faceJson["GTAO_HAIR"] = new_index;
            API.triggerServerEvent("change_face", "GTAO_HAIR", new_index);
        });
        mainWindow.AddItem(hairs);

        list = new List(String); for (var i = 0; i < 64; i++) { list.Add("" + i); }
        var hair_color = API.createListItem("Цвет волос", "", list, 0);
        hair_color.OnListChanged.connect(function (sender, new_index) {
            faceJson["GTAO_HAIR_COLOR"] = new_index;
            API.triggerServerEvent("change_face", "GTAO_HAIR_COLOR", new_index);
        });
        mainWindow.AddItem(hair_color);

        list = new List(String); for (var i = 0; i < 30; i++) { list.Add("" + i); }
        var list_item = API.createListItem("Цвет глаз", "", list, 0);
        list_item.OnListChanged.connect(function (sender, new_index) {
            faceJson["GTAO_EYE_COLOR"] = new_index;
            API.triggerServerEvent("change_face", "GTAO_EYE_COLOR", new_index);
        });
        mainWindow.AddItem(list_item);

        list = new List(String); for (var i = 0; i < 30; i++) { list.Add("" + i); }
        var list_item = API.createListItem("Форма бровей", "", list, 0);
        list_item.OnListChanged.connect(function (sender, new_index) {
            faceJson["GTAO_EYEBROWS"] = new_index;
            API.triggerServerEvent("change_face", "GTAO_EYEBROWS", new_index);
        });
        mainWindow.AddItem(list_item);

        list = new List(String); for (var i = 0; i < 46; i++) { list.Add("" + i); }
        var list_item = API.createListItem("Цвет бровей", "", list, 0);
        list_item.OnListChanged.connect(function (sender, new_index) {
            faceJson["GTAO_EYEBROWS_COLOR"] = new_index;
            API.triggerServerEvent("change_face", "GTAO_EYEBROWS_COLOR", new_index);
        });
        mainWindow.AddItem(list_item);

        var close = API.createMenuItem("~g~Закончить", "");
        close.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
            var jsonFaceSave = JSON.stringify(faceJson);
            API.triggerServerEvent("custom_char", jsonFaceSave);
        });
        mainWindow.AddItem(close);

        mainWindow.Visible = true;
        mainWindow.RefreshIndex();
    }
    if (name == "login_char_menu") {
        resetMainMenu();
        var language = args[0];
        var banner = "GTA: Role Play";
        var subtitle = "";
        var enterGame = "Enter the game";
        var closeText = "~r~Exit";

        switch (language) {
            case 1: enterGame = "~g~Enter ~s~the game"; closeText = "~r~Exit ~s~game"; break;
            case 2: enterGame = "~g~Войти ~s~в игру"; closeText = "~r~Выйти ~s~из игры"; break;
        }

        if (banner == null) mainWindow = API.createMenu(subtitle, 0, 0, 6);
        else mainWindow = API.createMenu(banner, subtitle, 0, 0, 6);
        menuPool.Add(mainWindow);

        var create = API.createMenuItem(enterGame, "");
        create.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;

            API.displaySubtitle(subtitle, 5000);
            var pwd = API.getUserInput("", 40);

            API.triggerServerEvent("login_char", pwd);
        });
        mainWindow.AddItem(create);

        var close = API.createMenuItem(closeText, "");
        close.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
            API.disconnect("Exit from start menu.");
        });
        mainWindow.AddItem(close);

        mainWindow.Visible = true;
        mainWindow.RefreshIndex();
    }
    if (name == "create_char_menu") {
        resetMainMenu();
        var callbackId = args[0];
        var banner = "GTA: Role Play";
        var subtitle = "";

        if (banner == null) mainWindow = API.createMenu(subtitle, 0, 0, 6);
        else mainWindow = API.createMenu(banner, subtitle, 0, 0, 6);
        menuPool.Add(mainWindow);

        var createNew = "Create a new character";
        var enterName = "Enter your Name_Surname";
        var enterPwd = "Enter your password";
        var backText = "Back";
        var languageNum = 0;

        var list = new List(String);
        list.Add("English");
        list.Add("Russian");
        var list_item = API.createListItem("Select language", "Select language from a list and press Enter.", list, 0);

        mainWindow.AddItem(list_item);

        list_item.OnListChanged.connect(function (sender, new_index) {
            if (new_index == 0) {
                enterName = "Enter your Name_Surname";
                enterPwd = "Enter your password";
                createNew = "Create a new character";
                backText = "~r~Back";
                languageNum = new_index;
            }
            if (new_index == 1) {
                enterName = "Введите ваше Имя_Фамилию";
                enterPwd = "Введите ваш пароль";
                createNew = "Создать нового персонажа";
                backText = "~r~Назад";
                languageNum = new_index;
            }
        });

        list_item.Activated.connect(function (menu, item) {

            mainWindow.Visible = false;
            mainWindow2 = API.createMenu(banner, subtitle, 0, 0, 6);
            menuPool2.Add(mainWindow2);
            mainWindow2.Visible = true;

            var create = API.createMenuItem(createNew, "");
            create.Activated.connect(function (menu, item) {
                mainWindow2.Visible = false;

                API.displaySubtitle(enterName, 5000);
                var name = API.getUserInput("", 40);

                API.displaySubtitle(enterPwd, 5000);
                var pwd = API.getUserInput("", 40);

                API.triggerServerEvent("create_char", name, pwd, languageNum + 1);
            });
            mainWindow2.AddItem(create);

            var back = API.createMenuItem(backText, "");
            back.Activated.connect(function (menu, item) {
                mainWindow2.Visible = false;
                mainWindow.Visible = true;
            });
            mainWindow2.AddItem(back);
            mainWindow2.RefreshIndex();
        });
        mainWindow.Visible = true;
        mainWindow.RefreshIndex();
    }
    if (name == "yes_no_menu_client") {        
        var banner = "Предложение";
        var subtitle = args[0];
        var type = args[1];        
        var initUserId = args[4];
        var targetUserId = args[5];

        if (banner == null) mainWindow = API.createMenu(subtitle, 0, 0, 6);
        else mainWindow = API.createMenu(banner, subtitle, 0, 0, 6);
        menuPool.Add(mainWindow);

        var parameter = args[2];
        var cost = args[3];

        var yes = API.createMenuItem("~g~Да", "");
        yes.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
            API.triggerServerEvent("sell", type, targetUserId, parameter, cost, initUserId);
        });
        mainWindow.AddItem(yes);

        var no = API.createMenuItem("~r~Нет", "");
        no.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
        });
        mainWindow.AddItem(no);

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
        var groupType = args[5];
        var groupExtraType = args[6];
        var material = args[7];
        var isInGang = args[8];
        var isGangBoss = args[9];
        var isHighOfficer = args[10];
        var isInGhetto = args[11];
        var isArmyGeneral = args[12];
        var weaponList = args[13];
        var initUserId = args[14];
        var stockMaterial = args[15];
        var moneyBank = args[16];
        var isCharacterIsHighRankInGang = args[17];
        var gangRank = args[18];
        var intGroupType = args[19];
        gangsSectors = args[20];
        var isSectorInYourGang = args[21];
        var groupStockName = args[22];
        var IsCharacterInMafia = args[23];

        if (banner == null) mainWindow = API.createMenu(subtitle, 0, 0, 6);
        else mainWindow = API.createMenu(banner, subtitle, 0, 0, 6);
        menuPool.Add(mainWindow);

        var groupChat = API.createMenuItem("~g~Написать~s~ в чат вашей группы", "");
        groupChat.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;

            API.displaySubtitle("Напишите сообщение", 5000);
            var message = API.getUserInput("", 350);

            API.triggerServerEvent("send_chat_message", message);
        });
        mainWindow.AddItem(groupChat);

        // Autoschool:
        if (intGroupType == 12) {
            var autoschool = API.createMenuItem("~g~Выдать права~s~ пользователю", "");
            autoschool.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
                API.displaySubtitle("Введите ID игрока", 5000);
                var userId = parseInt(API.getUserInput("", 40));
                API.triggerServerEvent("got_driver_license", userId);
            });
            mainWindow.AddItem(autoschool); 
        }

        // Army:
        if (intGroupType == 20 || intGroupType == 21) {

            if (isHighOfficer == true || isArmyGeneral == true) {

                var armyId = 0;

                var natioalGuard = API.createMenuItem("~g~Управление ~s~своей фракцией", "");
                natioalGuard.Activated.connect(function (menu, item) {
                    mainWindow.Visible = false;

                    mainWindow2 = API.createMenu("Моя фракция", "~s~" + groupType + " : " + groupExtraType, 0, 0, 6);
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

                            API.triggerServerEvent("army_add_to_group", userId, armyId, 1);
                        });
                        mainWindow2.AddItem(addToArmy);
                    }
                    if (isHighOfficer == true && isArmyGeneral == true) {

                        var higherRank = API.createMenuItem("~y~Изменить звание пользователя", "");
                        higherRank.Activated.connect(function (menu, item) {
                            mainWindow2.Visible = false;

                            API.displaySubtitle("Введите ID игрока", 5000);
                            var userId = parseInt(API.getUserInput("", 40));

                            API.displaySubtitle("Введите присваиваемое звание", 5000);
                            var rangId = parseInt(API.getUserInput("", 40));

                            API.triggerServerEvent("army_add_to_group", userId, rangId, 3, groupId);
                        });
                        mainWindow2.AddItem(higherRank);
                    }
                    if (groupId >= 2013 && groupId <= 2015 || groupId >= 2113 && groupId <= 2115) {
                        var goOutFrom = API.createMenuItem("~r~Выгнать пользователя из армии", "");
                        goOutFrom.Activated.connect(function (menu, item) {
                            mainWindow2.Visible = false;

                            API.displaySubtitle("Введите ID игрока", 5000);
                            var userId = parseInt(API.getUserInput("", 40));

                            API.triggerServerEvent("army_add_to_group", userId, groupId, 2);
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

            if (isInGhetto == true) {
                var sellCloth = API.createMenuItem("~g~Продать ~w~свою форму", "");
                sellCloth.Activated.connect(function (menu, item) {
                    mainWindow.Visible = false;

                    API.displaySubtitle("Введите ID игрока", 5000);
                    var userId = parseInt(API.getUserInput("", 40));

                    API.displaySubtitle("Введите стоимость формы", 5000);
                    var money = parseInt(API.getUserInput("", 40));

                    API.triggerServerEvent("yes_no_menu", "cloth", targetUserId, 0, cost, initUserId);
                });
                mainWindow.AddItem(sellCloth);
            }
        }            

        // Gangs:
        if (isInGang == true) {
            if (material != 0) {
                var materialLoad = API.createMenuItem("~g~Загрузить материалы~s~ в машину", "");
                materialLoad.Activated.connect(function (menu, item) {
                    mainWindow.Visible = false;
                    API.triggerServerEvent("load_unload_material", 1);
                });
                mainWindow.AddItem(materialLoad);
            }
        }
        if (isInGang == true) {

            if (gangRank >= 7 && gangRank <= 10 && isSectorInYourGang == false) {
                var caption = API.createMenuItem("~y~Захват~s~ данной территории", "");
                caption.Activated.connect(function (menu, item) {
                    mainWindow.Visible = false;
                    API.triggerServerEvent("gang_capting_sector");
                });
                mainWindow.AddItem(caption);
            }

            // Gang map:
            var listMap = new List(String);
            listMap.Add("Выкл");
            listMap.Add("Вкл");
            var list_map_item = API.createListItem("Карта банд", "Отображение карты банд (Вкл/Выкл)", listMap, 0);

            mainWindow.AddItem(list_map_item);

            list_map_item.OnListChanged.connect(function (sender, new_index) {
                if (new_index == 0) gangMap = 0;
                if (new_index == 1) gangMap = 1;
            });

            if (material != 0) {
                if (isInGhetto == true) {
                    var gangWeaponMenu = API.createMenuItem("~g~Создание ~s~оружия из материалов", "");
                    gangWeaponMenu.Activated.connect(function (menu, item) {
                        mainWindow.Visible = false;

                        mainWindow2 = API.createMenu("Моя фракция", "~s~" + groupType + " : " + groupExtraType, 0, 0, 6);
                        menuPool2.Add(mainWindow2);
                        mainWindow2.Visible = true;

                        var Revolver = API.createMenuItem("~s~ Создать Revolver : 100 мат.", "");
                        Revolver.Activated.connect(function (menu, item) {
                            API.triggerServerEvent("get_weapon", 1, 100, groupStockName);
                        });
                        mainWindow2.AddItem(Revolver);

                        var HeavyPistol = API.createMenuItem("~s~ Создать Heavy Pistol : 100 мат.", "");
                        HeavyPistol.Activated.connect(function (menu, item) {
                            API.triggerServerEvent("get_weapon", 11, 100, groupStockName);
                        });
                        mainWindow2.AddItem(HeavyPistol);

                        var CarbineRifle = API.createMenuItem("~s~ Создать Carbine Rifle : 250 мат.", "");
                        CarbineRifle.Activated.connect(function (menu, item) {
                            API.triggerServerEvent("get_weapon", 2, 250, groupStockName);
                        });
                        mainWindow2.AddItem(CarbineRifle);

                        var BullpupRifle = API.createMenuItem("~s~ Создать Bullpup Rifle : 250 мат.", "");
                        BullpupRifle.Activated.connect(function (menu, item) {
                            API.triggerServerEvent("get_weapon", 12, 250, groupStockName);
                        });
                        mainWindow2.AddItem(BullpupRifle);

                        var CompactRifle = API.createMenuItem("~s~ Создать Compact Rifle : 250 мат.", "");
                        CompactRifle.Activated.connect(function (menu, item) {
                            API.triggerServerEvent("get_weapon", 6, 250, groupStockName);
                        });
                        mainWindow2.AddItem(CompactRifle);

                        var HeavyShotgun = API.createMenuItem("~s~ Создать Heavy Shotgun : 200 мат.", "");
                        HeavyShotgun.Activated.connect(function (menu, item) {
                            API.triggerServerEvent("get_weapon", 7, 200, groupStockName);
                        });
                        mainWindow2.AddItem(HeavyShotgun);

                        var PumpShotgun = API.createMenuItem("~s~ Создать Pump Shotgun : 200 мат.", "");
                        PumpShotgun.Activated.connect(function (menu, item) {
                            API.triggerServerEvent("get_weapon", 7, 200, groupStockName);
                        });
                        mainWindow2.AddItem(PumpShotgun);

                        var SniperRifle = API.createMenuItem("~s~ Создать Sniper Rifle : 300 мат.", "");
                        SniperRifle.Activated.connect(function (menu, item) {
                            API.triggerServerEvent("get_weapon", 3, 300, groupStockName);
                        });
                        mainWindow2.AddItem(SniperRifle);

                        var SmokeGrenade = API.createMenuItem("~s~ Создать Smoke Grenade : 150 мат.", "");
                        SmokeGrenade.Activated.connect(function (menu, item) {
                            API.triggerServerEvent("get_weapon", 4, 150, groupStockName);
                        });
                        mainWindow2.AddItem(SmokeGrenade);

                        var FlareGun = API.createMenuItem("~s~ Создать Flare Gun : 100 мат.", "");
                        FlareGun.Activated.connect(function (menu, item) {
                            API.triggerServerEvent("get_weapon", 5, 100, groupStockName);
                        });
                        mainWindow2.AddItem(FlareGun);

                        var StunGun = API.createMenuItem("~s~ Создать Stun Gun : 150 мат.", "");
                        StunGun.Activated.connect(function (menu, item) {
                            API.triggerServerEvent("get_weapon", 10, 100, groupStockName);
                        });
                        mainWindow2.AddItem(StunGun);

                        var back = API.createMenuItem("~g~Назад", "");
                        back.Activated.connect(function (menu, item) {
                            mainWindow2.Visible = false;
                            mainWindow.Visible = true;
                        });
                        mainWindow2.AddItem(back);
                        mainWindow2.RefreshIndex();
                    });
                    mainWindow.AddItem(gangWeaponMenu);

                    var gangWeaponMenu = API.createMenuItem("~g~Продажа ~s~оружия игроку по ID", "");
                    gangWeaponMenu.Activated.connect(function (menu, item) {
                        mainWindow.Visible = false;

                        mainWindow2 = API.createMenu("Моя фракция", "~s~" + groupType + " : " + groupExtraType, 0, 0, 6);
                        menuPool2.Add(mainWindow2);
                        mainWindow2.Visible = true;

                        if (weaponList != "") {

                            var weapons = weaponList.split('-');
                            var list = new List(String);
                            var selectedWeapon = "";

                            for (var i = 0; i < weapons.length - 1; i++) {
                                list.Add(weapons[i]);
                            }
                            if (weapons.length > 0) {
                                selectedWeapon = list[0];
                            }

                            var list_item = API.createListItem("Оружие: ", "Выберите оружие", list, 0);
                            mainWindow2.AddItem(list_item);
                            list_item.OnListChanged.connect(function (sender, new_index) {
                                selectedWeapon = list[new_index];
                            });
                            list_item.Activated.connect(function (menu, item) {
                                mainWindow2.Visible = false;

                                var weaponName = weapons[i];
                                API.displaySubtitle("Введите ID игрока", 5000);
                                var targetUserId = parseInt(API.getUserInput("", 40));

                                API.displaySubtitle("Введите стоимость продажи", 5000);
                                var cost = parseInt(API.getUserInput("", 40));

                                API.triggerServerEvent("yes_no_menu", "weapon", targetUserId, selectedWeapon, cost, initUserId);
                            });
                        }

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
            }

            var gangAdd = API.createMenuItem("~g~Положить~s~ на счет банды.", "В банке денег:~s~ " + moneyBank + "$");
            gangAdd.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
                API.displaySubtitle("Введите сумму", 5000);
                var money = parseInt(API.getUserInput("", 40));
                API.triggerServerEvent("gang_add_money", money);
            });
            mainWindow.AddItem(gangAdd); 
        }
        if (isGangBoss == true) {

            var gangMenu = API.createMenuItem("~g~Управление ~s~своей фракцией", "");
            gangMenu.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;

                mainWindow2 = API.createMenu("Моя фракция", "~s~" + groupType + " : " + groupExtraType, 0, 0, 6);
                menuPool2.Add(mainWindow2);
                mainWindow2.Visible = true;

                if (gangRank == 9 || gangRank == 10) {
                    var gangAdd = API.createMenuItem("~g~Принять~s~ пользователя в банду", "");
                    gangAdd.Activated.connect(function (menu, item) {
                        mainWindow2.Visible = false;
                        API.displaySubtitle("Введите ID игрока", 5000);
                        var userId = parseInt(API.getUserInput("", 40));
                        API.triggerServerEvent("gang_add_to_group", userId, intGroupType * 100 + 1, 1);
                    });
                    mainWindow2.AddItem(gangAdd);
                }
                if (gangRank >= 7 && gangRank <= 10) {
                    var gangChange = API.createMenuItem("~y~Изменить~s~ ранг пользователя в банде", "Ваш ранг в банде: " + gangRank);
                    gangChange.Activated.connect(function (menu, item) {
                        mainWindow2.Visible = false;
                        API.displaySubtitle("Введите ID игрока", 5000);
                        var userId = parseInt(API.getUserInput("", 40));

                        API.displaySubtitle("Введите требуемый ранг", 5000);
                        var rangId = parseInt(API.getUserInput("", 40));
                        API.triggerServerEvent("gang_add_to_group", userId, rangId, 3, groupId);
                    });
                    mainWindow2.AddItem(gangChange);
                }
                if (gangRank >= 8 && gangRank <= 10) {
                    var gangDel = API.createMenuItem("~r~Выгнать~s~ пользователя из банды", "");
                    gangDel.Activated.connect(function (menu, item) {
                        mainWindow2.Visible = false;
                        API.displaySubtitle("Введите ID игрока", 5000);
                        var userId = parseInt(API.getUserInput("", 40));
                        API.triggerServerEvent("gang_add_to_group", userId, gangId, 2);
                    });
                    mainWindow2.AddItem(gangDel);

                    var sellMafia = API.createMenuItem("~r~Продать~s~ материалы Мафии", "");
                    sellMafia.Activated.connect(function (menu, item) {
                        mainWindow2.Visible = false;
                        API.displaySubtitle("Введите ID мафии", 5000);
                        var mafiaId = parseInt(API.getUserInput("", 40));
                        API.displaySubtitle("Введите количество материалов", 5000);
                        var material = parseInt(API.getUserInput("", 40));
                        API.displaySubtitle("Введите стоиомсть", 5000);
                        var cost = parseInt(API.getUserInput("", 40));
                        API.triggerServerEvent("yes_no_menu", "gang_material_mafia", 0, material, mafiaId, initUserId, cost);
                    });
                    mainWindow2.AddItem(sellMafia);
                }

                var gangGetMoney = API.createMenuItem("~y~Снять~s~ деньги со счета банды.", "Денег на счету: " + moneyBank + "$");
                gangGetMoney.Activated.connect(function (menu, item) {
                    mainWindow.Visible = false;
                    API.displaySubtitle("Введите сумму", 5000);
                    var money = parseInt(API.getUserInput("", 40));
                    API.triggerServerEvent("gang_get_money", money);
                });
                mainWindow2.AddItem(gangGetMoney);

                var sellSector = API.createMenuItem("~r~Продать~s~ территорию банды.", "");
                sellSector.Activated.connect(function (menu, item) {
                    mainWindow.Visible = false;
                    API.displaySubtitle("Введите номер квадрата в формате N_N", 5000);
                    var sector = API.getUserInput("", 40);
                    API.displaySubtitle("Введите номер банды", 5000);
                    var gangNum = parseInt(API.getUserInput("", 40));
                    API.displaySubtitle("Введите сумму", 5000);
                    var cost = parseInt(API.getUserInput("", 40));
                    API.triggerServerEvent("yes_no_menu", "gang_sector", 0, sector, gangNum, initUserId, cost, intGroupType);
                });
                mainWindow2.AddItem(sellSector);

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

        // Mafia:
        if (IsCharacterInMafia == true) {
            if (gangRank >= 1 && gangRank <= 10) {
                var mafiaDebt = API.createMenuItem("~g~Дать в долг~s~ пользователю", "");
                mafiaDebt.Activated.connect(function (menu, item) {
                    mainWindow.Visible = false;                    
                    API.displaySubtitle("Введите ID игрока", 5000);
                    var targetUserId = parseInt(API.getUserInput("", 40));
                    API.displaySubtitle("Введите сумму долга", 5000);
                    var cost = parseInt(API.getUserInput("", 40));
                    API.triggerServerEvent("yes_no_menu", "mafia_debt", targetUserId, 0, cost, initUserId);
                });
                mainWindow.AddItem(mafiaDebt);

                var mafiaRoof = API.createMenuItem("~g~Предложить~s~ крышу пользователю", "");
                mafiaRoof.Activated.connect(function (menu, item) {
                    mainWindow.Visible = false;
                    API.displaySubtitle("Введите ID игрока", 5000);
                    var targetUserId = parseInt(API.getUserInput("", 40));
                    API.displaySubtitle("Введите сумму за крышу", 5000);
                    var cost = parseInt(API.getUserInput("", 40));
                    API.triggerServerEvent("yes_no_menu", "mafia_roof", targetUserId, 0, cost, initUserId);
                });
                mainWindow.AddItem(mafiaRoof);

                var mafiaInfo = API.createMenuItem("~y~Узнать~s~ информацию о пользователе", "");
                mafiaInfo.Activated.connect(function (menu, item) {
                    mainWindow.Visible = false;
                    API.displaySubtitle("Введите ID игрока", 5000);
                    var targetUserId = parseInt(API.getUserInput("", 40));
                    API.triggerServerEvent("mafia_get_info", targetUserId);
                });
                mainWindow.AddItem(mafiaInfo);
            }
            if (gangRank >= 5 && gangRank <= 10) {

            }
            if (gangRank >= 6 && gangRank <= 10) {

            }
            if (gangRank >= 7 && gangRank <= 10) {
                var mafiaChange = API.createMenuItem("~y~Изменить~s~ ранг пользователя в мафии", "Ваш ранг в мафии: " + gangRank);
                mafiaChange.Activated.connect(function (menu, item) {
                    mainWindow.Visible = false;
                    API.displaySubtitle("Введите ID игрока", 5000);
                    var userId = parseInt(API.getUserInput("", 40));

                    API.displaySubtitle("Введите требуемый ранг", 5000);
                    var rangId = parseInt(API.getUserInput("", 40));
                    API.triggerServerEvent("gang_add_to_group", userId, rangId, 3, groupId);
                });
                mainWindow.AddItem(mafiaChange);
            }
            if (gangRank >= 8 && gangRank <= 10) {
                var mafiaDel = API.createMenuItem("~r~Выгнать~s~ пользователя из мафии", "");
                mafiaDel.Activated.connect(function (menu, item) {
                    mainWindow.Visible = false;
                    API.displaySubtitle("Введите ID игрока", 5000);
                    var userId = parseInt(API.getUserInput("", 40));
                    API.triggerServerEvent("gang_add_to_group", userId, gangId, 2);
                });
                mainWindow.AddItem(mafiaDel);
            }
            if (gangRank >= 9 && gangRank <= 10) {
                var mafiaAdd = API.createMenuItem("~g~Принять~s~ пользователя в мафию", "");
                mafiaAdd.Activated.connect(function (menu, item) {
                    mainWindow.Visible = false;
                    API.displaySubtitle("Введите ID игрока", 5000);
                    var userId = parseInt(API.getUserInput("", 40));
                    API.triggerServerEvent("gang_add_to_group", userId, intGroupType * 100 + 1, 1);
                });
                mainWindow.AddItem(mafiaAdd);
            }
            if (gangRank == 10) {
                var mafiaGetMoney = API.createMenuItem("~y~Снять~s~ деньги со счета мафии.", "Денег на счету: " + moneyBank + "$");
                mafiaGetMoney.Activated.connect(function (menu, item) {
                    mainWindow.Visible = false;
                    API.displaySubtitle("Введите сумму", 5000);
                    var money = parseInt(API.getUserInput("", 40));
                    API.triggerServerEvent("gang_get_money", money);
                });
                mainWindow.AddItem(mafiaGetMoney);
            }

            if (gangRank >= 8 && gangRank <= 10) {
                var matInfo = API.createMenuItem("На складе материалов:~s~ " + stockMaterial, "");
                mainWindow.AddItem(matInfo);
                var moneyInfo = API.createMenuItem("В банке фракции денег:~s~ " + moneyBank + "$", "");
                mainWindow.AddItem(moneyInfo);
            }
        }

        // Taxi Driver:
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
        // Admin menu:
        if (isAdmin == 10) {

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
        var fuelTank = args[4];
        var inVehicleNear = args[5];
        var driverDoorStatus = args[6];
        var materialCount = args[7];        

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
        if (inVehicleNear == true) mainWindow.AddItem(engine);        

        var park = API.createMenuItem("~y~Припарковать~s~ транспорт", "");
        park.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
            API.triggerServerEvent("park_vehicle");
        });
        if (inVehicleNear == true) mainWindow.AddItem(park);

        if (driverDoorStatus == 0) {
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

        var fuel = API.createMenuItem("Объем бака", "Объем бака: ~g~" + fuelTank);
        if (inVehicleNear == true) mainWindow.AddItem(fuel);

        var material = API.createMenuItem("Материал", "Материалов: ~g~" + materialCount);
        mainWindow.AddItem(material);

        var close = API.createMenuItem("~r~Закрыть", "");
        close.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
        });
        mainWindow.AddItem(close);

        mainWindow.Visible = true;
        mainWindow.RefreshIndex();
    }
    if (name == "house_menu") {
        var banner = "Жилой дом";
        var subtitle = "Стоимость дома: " + args[1] + "$";
        var callbackId = args[2];
        var action = args[3];        

        if (banner == null) mainWindow = API.createMenu(subtitle, 0, 0, 6);
        else mainWindow = API.createMenu(banner, subtitle, 0, 0, 6);
        menuPool.Add(mainWindow);

        if (action == 2) {
            var robHouse = API.createMenuItem("~y~Ограбить~s~ дом", "");
            robHouse.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
                API.triggerServerEvent("gang_rob_house", args[4]);
            });
            mainWindow.AddItem(robHouse);
        }

        if (action == 1) {
            var buyHouse = API.createMenuItem("~g~Купить дом", "");
            buyHouse.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
                API.triggerServerEvent("house_menu_buysell", args[0], args[1], 1);
            });
            mainWindow.AddItem(buyHouse);
        }        

        if (action == 0) {
            var sellHouse = API.createMenuItem("~g~Продать дом", "");
            sellHouse.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
                API.triggerServerEvent("house_menu_buysell", args[0], args[1], 0);
            });
            mainWindow.AddItem(sellHouse);
        }

        var exit = API.createMenuItem("~r~Выйти", "");
        exit.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
        });
        mainWindow.AddItem(exit);

        if (callbackId == 0) {
            exit.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
            });
        }
        else mainWindow.Visible = true;
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
        var propertyName = args[1];

        var banner = "";
        var subtitle = "";
        switch (propertyName) {
            case "Rent_scooter": banner = "Прокат мопеда"; subtitle = "Возьмите на полчаса мопед всего за 30$"; break;
        }  
        if (banner == null) mainWindow = API.createMenu(subtitle, 0, 0, 6);
        else mainWindow = API.createMenu(banner, subtitle, 0, 0, 6);
        menuPool.Add(mainWindow);
        
        var payAndGo = API.createMenuItem("Заплатить и поехать!", "");
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
        var propertyName = args[1];  
        var banner = "";
        var subtitle = "";

        switch (propertyName)
        {
            case "Gangs_metall":banner = "Прием металла"; subtitle = "Сдайте украденный металл!"; break;
            case "Army2_gang":  subtitle = "Украдите материалы у Армии 2"; break;
            case "Army1_gang":  subtitle = "Украдите материалы у Армии 1"; break;
            case "Army2_stock": subtitle = "Украдите материалы со склада Армии 2"; break;
            case "Army1_stock": subtitle = "Украдите материалы со склада Армии 1"; break;
            case "Ballas_stock":banner = "Банда Ballas"; subtitle = "Разгрузите украденные материалы."; break;
            case "Ballas_main": banner = "Банда Ballas"; subtitle = "Ваша база."; break;
            case "Azcas_stock": banner = "Банда Azcas"; subtitle = "Разгрузите украденные материалы."; break;
            case "Azcas_main":  banner = "Банда Azcas"; subtitle = "Ваша база."; break;
            case "Vagos_stock": banner = "Банда Vagos"; subtitle = "Разгрузите украденные материалы."; break;
            case "Vagos_main":  banner = "Банда Vagos"; subtitle = "Ваша база."; break;
            case "Grove_stock": banner = "Банда Grove"; subtitle = "Разгрузите украденные материалы."; break;
            case "Grove_main":  banner = "Банда Grove"; subtitle = "Ваша база."; break;
            case "Rifa_stock":  banner = "Банда Rifa"; subtitle = "Разгрузите украденные материалы."; break;
            case "Rifa_main":   banner = "Банда Rifa"; subtitle = "Ваша база."; break;
        }             

        if (banner == null) mainWindow = API.createMenu(subtitle, 0, 0, 6);
        else mainWindow = API.createMenu(banner, subtitle, 0, 0, 6);
        menuPool.Add(mainWindow);

        if (propertyName == "Gangs_metall") {
            var takeMaterial = API.createMenuItem("~s~Сдать материалы", "");
            takeMaterial.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
                API.triggerServerEvent("gang_menu", propertyName, 6);
            });
            mainWindow.AddItem(takeMaterial);
        }  
         
        if (propertyName == "Army2_stock" || propertyName == "Army1_stock") {
            var stealGroup = API.createMenuItem("~s~Украсть 1000 материалов", "");
            stealGroup.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
                API.triggerServerEvent("gang_menu", propertyName, 2);
            });
            mainWindow.AddItem(stealGroup);
        }  
        if (propertyName == "Ballas_stock" || propertyName == "Azcas_stock" ||
            propertyName == "Vagos_stock" || propertyName == "Grove_stock" || propertyName == "Rifa_stock") {
            var unloadMaterial = API.createMenuItem("~g~Разгрузить материалы", "");
            unloadMaterial.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
                API.triggerServerEvent("gang_menu", propertyName, 3);
            });
            mainWindow.AddItem(unloadMaterial);
        }  
        if (propertyName == "Ballas_main" || propertyName == "Azcas_main" ||
            propertyName == "Vagos_main" || propertyName == "Grove_main" || propertyName == "Rifa_main") {

            var clothesTypes = args[2]; 
            var stockMaterial = args[3];
            var gangRank = args[4];
            var moneyBank = args[5];
            var clothName = "";
            if (clothesTypes == 2) clothName = "солдата";
            if (clothesTypes == 3) clothName = "офицера";
            if (clothesTypes == 4) clothName = "генерала";
            if (clothesTypes == 10) clothName = "полицейского";            

            if (clothesTypes != 0) {
                var clothArmy = API.createMenuItem("~g~Одеть форму " + clothName, "");
                clothArmy.Activated.connect(function (menu, item) {
                    mainWindow.Visible = false;
                    API.triggerServerEvent("gang_menu", propertyName, 4);
                });
                mainWindow.AddItem(clothArmy);
            }   

            var clothGang = API.createMenuItem("~g~Одеть форму своей банды", "");
            clothGang.Activated.connect(function (menu, item) {
                mainWindow.Visible = false;
                API.triggerServerEvent("gang_menu", propertyName, 5);
            });
            mainWindow.AddItem(clothGang);

            if (gangRank >= 4 && gangRank <= 10) {
                var gangGetMaterial = API.createMenuItem("~g~Взять~s~ материалы со склада.", "На складе материалов:~s~ " + stockMaterial);
                gangGetMaterial.Activated.connect(function (menu, item) {
                    mainWindow.Visible = false;
                    API.displaySubtitle("Введите количество материалов", 5000);
                    var material = parseInt(API.getUserInput("", 40));
                    API.triggerServerEvent("gang_get_material", material);
                });
                mainWindow.AddItem(gangGetMaterial);

                var moneyInfo = API.createMenuItem("В банке фракции ~g~денег:~s~ " + moneyBank + "$", "");
                mainWindow.AddItem(moneyInfo);
            }
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

    if (name == "police_menu") {
        resetMainMenu();
        var callbackId = args[0];        
        var propertyName = args[1];
        var access = args[2];

        var banner = "";
        var subtitle = "";
        switch (propertyName)
        {
            case "Police_weapon": banner = "Полиция: оружие"; subtitle = "Выберите нужное вам оружие:"; break;
            case "Police_main": banner = "Полиция: главная"; subtitle = "Ваша база."; break;
            case "Police_stock": banner = "Полиция: склад"; subtitle = "Работа со складом:"; break;
        }
        
        if (banner == null) mainWindow = API.createMenu(subtitle, 0, 0, 6);
        else mainWindow = API.createMenu(banner, subtitle, 0, 0, 6);
        menuPool.Add(mainWindow); 

        if (propertyName == "Police_weapon") {
            var pistol = API.createMenuItem("~s~ Взять Revolver : 100 мат.", "");
            pistol.Activated.connect(function (menu, item) {
                API.triggerServerEvent("get_weapon", 1, 100);
            });
            mainWindow.AddItem(pistol);

            var CompactRifle = API.createMenuItem("~s~ Взять Compact Rifle : 250 мат.", "");
            CompactRifle.Activated.connect(function (menu, item) {
                API.triggerServerEvent("get_weapon", 6, 250);
            });
            mainWindow.AddItem(CompactRifle);

            var PumpShotgun = API.createMenuItem("~s~ Взять Pump Shotgun : 200 мат.", "");
            PumpShotgun.Activated.connect(function (menu, item) {
                API.triggerServerEvent("get_weapon", 7, 200);
            });
            mainWindow.AddItem(PumpShotgun);

            var BZGas = API.createMenuItem("~s~ Взять BZGas : 150 мат.", "");
            BZGas.Activated.connect(function (menu, item) {
                API.triggerServerEvent("get_weapon", 8, 150);
            });
            mainWindow.AddItem(BZGas);

            var Nightstick = API.createMenuItem("~s~ Взять Nightstick : 100 мат.", "");
            Nightstick.Activated.connect(function (menu, item) {
                API.triggerServerEvent("get_weapon", 9, 100);
            });
            mainWindow.AddItem(Nightstick);

            var StunGun = API.createMenuItem("~s~ Взять Stun Gun : 150 мат.", "");
            StunGun.Activated.connect(function (menu, item) {
                API.triggerServerEvent("get_weapon", 10, 100);
            });
            mainWindow.AddItem(StunGun);

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

    }
    if (name == "fbi_menu") {
        resetMainMenu();
        var callbackId = args[0];
        var propertyName = args[1];
        var access = args[2];

        var banner = "";
        var subtitle = "";
        switch (propertyName) {
            case "FBI_weapon": banner = "FBI: оружие"; subtitle = "Выберите нужное вам оружие:"; break;
            case "FBI_main": banner = "FBI: главная"; subtitle = "Ваша база."; break;
            case "FBI_stock": banner = "FBI: склад"; subtitle = "Работа со складом:"; break;
        }
        if (banner == null) mainWindow = API.createMenu(subtitle, 0, 0, 6);
        else mainWindow = API.createMenu(banner, subtitle, 0, 0, 6);
        menuPool.Add(mainWindow);

        if (propertyName == "FBI_weapon") {
            var HeavyPistol = API.createMenuItem("~s~ Взять Heavy Pistol : 100 мат.", "");
            HeavyPistol.Activated.connect(function (menu, item) {
                API.triggerServerEvent("get_weapon", 11, 100);
            });
            mainWindow.AddItem(HeavyPistol);

            var BullpupRifle = API.createMenuItem("~s~ Взять Bullpup Rifle : 250 мат.", "");
            BullpupRifle.Activated.connect(function (menu, item) {
                API.triggerServerEvent("get_weapon", 12, 250);
            });
            mainWindow.AddItem(BullpupRifle);

            var HeavyShotgun = API.createMenuItem("~s~ Взять Heavy Shotgun : 200 мат.", "");
            HeavyShotgun.Activated.connect(function (menu, item) {
                API.triggerServerEvent("get_weapon", 7, 200);
            });
            mainWindow.AddItem(HeavyShotgun);

            var BZGas = API.createMenuItem("~s~ Взять BZGas : 150 мат.", "");
            BZGas.Activated.connect(function (menu, item) {
                API.triggerServerEvent("get_weapon", 8, 150);
            });
            mainWindow.AddItem(BZGas);

            var SmokeGrenade = API.createMenuItem("~s~ Взять Smoke Grenade : 150 мат.", "");
            SmokeGrenade.Activated.connect(function (menu, item) {
                API.triggerServerEvent("get_weapon", 4, 150);
            });
            mainWindow.AddItem(SmokeGrenade);

            var Nightstick = API.createMenuItem("~s~ Взять Nightstick : 100 мат.", "");
            Nightstick.Activated.connect(function (menu, item) {
                API.triggerServerEvent("get_weapon", 9, 100);
            });
            mainWindow.AddItem(Nightstick);

            var StunGun = API.createMenuItem("~s~ Взять Stun Gun : 150 мат.", "");
            StunGun.Activated.connect(function (menu, item) {
                API.triggerServerEvent("get_weapon", 10, 100);
            });
            mainWindow.AddItem(StunGun);

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
    }
    if (name == "mafia_menu") {
        resetMainMenu();
        var callbackId = args[0];        
        var propertyName = args[1];        

        var banner = "";
        var subtitle = "";
        switch (propertyName)
        {
            case "RussianMafia_stock": banner = "Russian Mafia"; subtitle = "Выберите нужное вам оружие:"; break;
            case "MafiaLKN_stock": banner = "Mafia LKN"; subtitle = "Выберите нужное вам оружие:"; break;
            case "MafiaArmeny_stock": banner = "Mafia Armeny"; subtitle = "Выберите нужное вам оружие:"; break;
            case "RussianMafia_main": banner = "Russian Mafia"; subtitle = "Ваша база:"; break;
            case "MafiaLKN_main": banner = "Mafia LKN"; subtitle = "Ваша база:"; break;
            case "MafiaArmeny_main": banner = "Mafia Armeny"; subtitle = "Ваша база:"; break;
        }        
        if (banner == null) mainWindow = API.createMenu(subtitle, 0, 0, 6);
        else mainWindow = API.createMenu(banner, subtitle, 0, 0, 6);
        menuPool.Add(mainWindow);

        if (propertyName == "RussianMafia_stock" || propertyName == "MafiaLKN_stock" || propertyName == "MafiaArmeny_stock") {

            var mafiaRank = args[2];
            var IsCharacterInMafia = args[3];

            if (IsCharacterInMafia == true) {
                if (mafiaRank >= 3) {
                    var Revolver = API.createMenuItem("~s~ Взять Revolver : 100 мат.", "");
                    Revolver.Activated.connect(function (menu, item) {
                        API.triggerServerEvent("get_weapon", 1, 100, propertyName);
                    });
                    mainWindow.AddItem(Revolver);

                    var CarbineRifle = API.createMenuItem("~s~ Взять Carbine Rifle : 250 мат.", "");
                    CarbineRifle.Activated.connect(function (menu, item) {
                        API.triggerServerEvent("get_weapon", 2, 250, propertyName);
                    });
                    mainWindow.AddItem(CarbineRifle);

                    var SniperRifle = API.createMenuItem("~s~ Взять Sniper Rifle : 300 мат.", "");
                    SniperRifle.Activated.connect(function (menu, item) {
                        API.triggerServerEvent("get_weapon", 3, 300, propertyName);
                    });
                    mainWindow.AddItem(SniperRifle);

                    var SmokeGrenade = API.createMenuItem("~s~ Взять Smoke Grenade : 150 мат.", "");
                    SmokeGrenade.Activated.connect(function (menu, item) {
                        API.triggerServerEvent("get_weapon", 4, 150, propertyName);
                    });
                    mainWindow.AddItem(SmokeGrenade);

                    var FlareGun = API.createMenuItem("~s~ Взять Flare Gun : 100 мат.", "");
                    FlareGun.Activated.connect(function (menu, item) {
                        API.triggerServerEvent("get_weapon", 5, 100, propertyName);
                    });
                    mainWindow.AddItem(FlareGun);
                }    
            }            
        }     

        var close = API.createMenuItem("~r~Закрыть", "");
        close.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
        });
        mainWindow.AddItem(close);

        mainWindow.Visible = true;
        mainWindow.RefreshIndex();
    }
    if (name == "army_menu") {
        resetMainMenu();
        var callbackId = args[0];
        var propertyName = args[1];
        var characterGroup = args[2];

        var banner = "";
        var subtitle = "";
        switch (propertyName)
        {
            case "Army2_stock": banner = "Армия 2: склад"; subtitle = "Загрузка/разгрузка на складе Армии 2"; break;
            case "Army2_main": banner = "Армия 2: главная"; subtitle = "Ваша база:"; break;
            case "Army1_weapon": banner = "Армия 1: оружие"; subtitle = "Выберите нужное вам оружие:"; break;
            case "Army1_main": banner = "Армия 1: главная"; subtitle = "Ваша база:"; break;
            case "Army1_stock": banner = "Армия 1: склад"; subtitle = "Загрузка/разгрузка на складе Армии 1"; break;
            case "Army1_source": banner = "Материалы для армий"; subtitle = "Загружайте материалы с авианосца"; break;
            case "Army2_weapon": banner = "Армия 2: оружие"; subtitle = "Выберите нужное вам оружие:"; break;
            case "FBI_stock": banner = "FBI склад"; subtitle = "Разгрузка на складе FBI:"; break;
            case "Police_stock": banner = "Police склад"; subtitle = "Разгрузка на складе Police:"; break;
        }

        if (banner == null) mainWindow = API.createMenu(subtitle, 0, 0, 6);
        else mainWindow = API.createMenu(banner, subtitle, 0, 0, 6);
        menuPool.Add(mainWindow);   

        // Loading from carrier to Army 1 vehicle.
        // Unloading from Army 1 vehicle to Army 2 stock.
        if (characterGroup == 20) {        
            if (propertyName == "Army1_source") {
                var loadFromStock = API.createMenuItem("~g~Загрузить ~s~материалы", "");
                loadFromStock.Activated.connect(function (menu, item) {
                    mainWindow.Visible = false;
                    API.triggerServerEvent("armys_menu", 3);
                });
                mainWindow.AddItem(loadFromStock);
            }
        
            if (propertyName == "Army2_stock") {
                var loadToStock = API.createMenuItem("~g~Разгрузить ~s~материалы у ~y~Армии 2", "");
                loadToStock.Activated.connect(function (menu, item) {
                    mainWindow.Visible = false;
                    API.triggerServerEvent("armys_menu", 2, propertyName);
                });
                mainWindow.AddItem(loadToStock);
            }
        }
        // Loading from stock by Army 2 vehicle.
        // Unloading from Army 2 vehicle to stocks (Army 1, FBI, Poloce).
        if (characterGroup == 21) {            
            if (propertyName == "Army2_stock") {
                var loadForArmy = API.createMenuItem("~g~Загрузить ~s~материалы для ~y~Армии 1", "На складе " + args[3] + " материалов");
                loadForArmy.Activated.connect(function (menu, item) {
                    mainWindow.Visible = false;
                    API.triggerServerEvent("armys_menu", 1, propertyName, "Army1_stock");
                });
                mainWindow.AddItem(loadForArmy);

                var loadForPolice = API.createMenuItem("~g~Загрузить ~s~материалы для ~y~Полиции", "На складе " + args[3] + " материалов");
                loadForPolice.Activated.connect(function (menu, item) {
                    mainWindow.Visible = false;
                    API.triggerServerEvent("armys_menu", 1, propertyName, "Police_stock");
                });
                mainWindow.AddItem(loadForPolice);

                var loadForFBI = API.createMenuItem("~g~Загрузить ~s~материалы для ~y~FBI", "На складе " + args[3] + " материалов");
                loadForFBI.Activated.connect(function (menu, item) {
                    mainWindow.Visible = false;
                    API.triggerServerEvent("armys_menu", 1, propertyName, "FBI_stock");
                });
                mainWindow.AddItem(loadForFBI);
            }            
            if (propertyName == "Army1_stock") {
                var loadToStock = API.createMenuItem("~g~Разгрузить ~s~материалы в ~y~Армии 1", "");
                loadToStock.Activated.connect(function (menu, item) {
                    mainWindow.Visible = false;
                    API.triggerServerEvent("armys_menu", 2, propertyName);
                });
                mainWindow.AddItem(loadToStock);
            }
            if (propertyName == "Police_stock") {
                var unloadPolice = API.createMenuItem("~g~Разгрузить ~s~материалы в ~y~Полиции", "");
                unloadPolice.Activated.connect(function (menu, item) {
                    mainWindow.Visible = false;
                    API.triggerServerEvent("armys_menu", 2, propertyName);
                });
                mainWindow.AddItem(unloadPolice);
            }
            if (propertyName == "FBI_stock") {
                var unloadPolice = API.createMenuItem("~g~Разгрузить ~s~материалы в ~y~FBI", "");
                unloadPolice.Activated.connect(function (menu, item) {
                    mainWindow.Visible = false;
                    API.triggerServerEvent("armys_menu", 2, propertyName);
                });
                mainWindow.AddItem(unloadPolice);
            }
        }
        // Change clothes.
        if (characterGroup == 20 || characterGroup == 21) {
            if (propertyName == "Army2_main" || propertyName == "Army1_main") {
                var clothUp = API.createMenuItem("~g~Одеть ~s~военную форму", "");
                clothUp.Activated.connect(function (menu, item) {
                    mainWindow.Visible = false;
                    API.triggerServerEvent("armys_menu", 4, propertyName, "Cloth_up");
                });
                mainWindow.AddItem(clothUp);

                var clothDown = API.createMenuItem("~r~Снять ~s~военную форму", "");
                clothDown.Activated.connect(function (menu, item) {
                    mainWindow.Visible = false;
                    API.triggerServerEvent("armys_menu", 4, propertyName, "Cloth_down");
                });
                mainWindow.AddItem(clothDown);
            }
        }
        // Weapon creating.
        if (propertyName == "Army2_weapon" || propertyName == "Army1_weapon") {

            if (propertyName == "Army1_weapon") propertyName = "Army1_stock";
            else                                propertyName = "Army2_stock";

            var Revolver = API.createMenuItem("~s~ Взять Revolver : 100 мат.", "");
            Revolver.Activated.connect(function (menu, item) {
                API.triggerServerEvent("get_weapon", 1, 100, propertyName);
            });
            mainWindow.AddItem(Revolver);

            var CarbineRifle = API.createMenuItem("~s~ Взять Carbine Rifle : 250 мат.", "");
            CarbineRifle.Activated.connect(function (menu, item) {
                API.triggerServerEvent("get_weapon", 2, 250, propertyName);
            });
            mainWindow.AddItem(CarbineRifle);

            var SniperRifle = API.createMenuItem("~s~ Взять Sniper Rifle : 300 мат.", "");
            SniperRifle.Activated.connect(function (menu, item) {
                API.triggerServerEvent("get_weapon", 3, 300, propertyName);
            });
            mainWindow.AddItem(SniperRifle);

            var SmokeGrenade = API.createMenuItem("~s~ Взять Smoke Grenade : 150 мат.", "");
            SmokeGrenade.Activated.connect(function (menu, item) {                
                API.triggerServerEvent("get_weapon", 4, 150, propertyName);
            });
            mainWindow.AddItem(SmokeGrenade);

            var FlareGun = API.createMenuItem("~s~ Взять Flare Gun : 100 мат.", "");
            FlareGun.Activated.connect(function (menu, item) {
                API.triggerServerEvent("get_weapon", 5, 100, propertyName);
            });
            mainWindow.AddItem(FlareGun);
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
    if (name == "work_busdriver_menu") {
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

        var start = API.createMenuItem("~g~Первый~s~ маршрут", "Больница - Мэрия - Стройка - Аэропорт");
        start.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
            API.triggerServerEvent("work_busdriver", 1, posX, posY, posZ);
        });
        mainWindow.AddItem(start);        

        var close = API.createMenuItem("~r~Закончить~s~ работу", "Переодеться в свою одежду");
        close.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
            API.triggerServerEvent("work_busdriver", 0, jobId, posX, posY, posZ);
        });
        mainWindow.AddItem(close);

        if (callbackId == 0) {
            close.Activated.connect(function (menu, item) {
                API.triggerServerEvent("work_busdriver", 0);
                mainWindow.Visible = false;
            });
        }
        else mainWindow.Visible = true;
        mainWindow.RefreshIndex();
    }
    if (name == "work_gasstation_menu") {
        resetMainMenu();
        var callbackId = args[0];
        var banner = args[1];
        var subtitle = args[2];
        var jobId = args[3];
        var trigger = args[4];
        var cost = args[5];

        if (banner == null) mainWindow = API.createMenu(subtitle, 0, 0, 6);
        else mainWindow = API.createMenu(banner, subtitle, 0, 0, 6);
        menuPool.Add(mainWindow);

        switch (trigger) {
            case 1:
                var buyGasStation = API.createMenuItem("~g~Купить~s~ канистру", "");
                buyGasStation.Activated.connect(function (menu, item) {
                    mainWindow.Visible = false;
                    API.triggerServerEvent("work_gasstation", 1, jobId, "canister");
                });
                mainWindow.AddItem(buyGasStation);

                var buyPhone = API.createMenuItem("~g~Купить~s~ телефон", "");
                buyPhone.Activated.connect(function (menu, item) {
                    mainWindow.Visible = false;
                    API.triggerServerEvent("work_gasstation", 1, jobId, "phone");
                });
                mainWindow.AddItem(buyPhone);

                var buyFood = API.createMenuItem("~g~Купить~s~ еды", "");
                buyFood.Activated.connect(function (menu, item) {
                    mainWindow.Visible = false;
                    API.triggerServerEvent("work_gasstation", 1, jobId, "food");
                });
                mainWindow.AddItem(buyFood);

                if (args[6] == true) {
                    var money = args[7];
                    var getMoney = API.createMenuItem("~y~Снять~s~ кассу", "В кассе: " + money);
                    getMoney.Activated.connect(function (menu, item) {
                        mainWindow.Visible = false;
                        API.triggerServerEvent("work_gasstation", 1, jobId, "getmoney", money);
                    });
                    mainWindow.AddItem(getMoney);

                    var sellGasStation = API.createMenuItem("~r~Продать~s~ заправку", "Стоимость: " + cost);
                    sellGasStation.Activated.connect(function (menu, item) {
                        mainWindow.Visible = false;
                        API.triggerServerEvent("work_gasstation", 1, jobId, "sell");
                    });
                    mainWindow.AddItem(sellGasStation);
                }                
                break;

            case 2:
                var buyGasStation = API.createMenuItem("~g~Купить~s~ заправку", "Стоимость: " + cost);
                buyGasStation.Activated.connect(function (menu, item) {
                    mainWindow.Visible = false;
                    API.triggerServerEvent("work_gasstation", 0, jobId, cost);
                });
                mainWindow.AddItem(buyGasStation);
                break;
        }

        var close = API.createMenuItem("~r~Закрыть", "");
        close.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
        });
        mainWindow.AddItem(close);

        if (callbackId == 0) {
            close.Activated.connect(function (menu, item) {
                API.triggerServerEvent("work_busdriver", 0);
                mainWindow.Visible = false;
            });
        }
        else mainWindow.Visible = true;
        mainWindow.RefreshIndex();
    }
    if (name == "get_petrolium") {
        resetMainMenu();
        var callbackId = args[0];
        var jobId = args[1];

        mainWindow = API.createMenu("Заправить топливо", "У нас всего ~g~1$ за литр!", 0, 0, 6);
        menuPool.Add(mainWindow);

        var list = new List(String);
        list.Add("5");
        list.Add("10");
        list.Add("15");
        list.Add("20");
        var list_item = API.createListItem("Заправить литров: ", "", list, 0);

        var fuel = 5;
        list_item.OnListChanged.connect(function (sender, new_index) {
            switch (new_index) {
                case 0: fuel = 5; break;
                case 1: fuel = 10; break;
                case 2: fuel = 15; break;
                case 3: fuel = 20; break;
            }
        });
        list_item.Activated.connect(function (menu, item) {
            mainWindow.Visible = false;
            API.triggerServerEvent("get_petrolium", jobId, fuel);
        });
        mainWindow.AddItem(list_item);

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
        var userLevel = args[1];
        var propertyName = args[2];

        var banner = "";
        var subtitle = "";
        switch (propertyName) {
            case "Meria_main": banner = "Мэрия города"; subtitle = "Всегда рады помочь вам!"; break;
        }    

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

                var loader1 = API.createMenuItem("Грузчик 1", "Требуется уровень 0");
                loader1.Activated.connect(function (menu, item) {
                    mainWindow3.Visible = false;
                    API.displaySubtitle("Метка первой работы на карте установлена", 5000);
                    waypoint = API.setWaypoint(-144, -948);
                });
                mainWindow3.AddItem(loader1);

                var loader2 = API.createMenuItem("Грузчик 2", "Требуется уровень 0");
                loader2.Activated.connect(function (menu, item) {
                    mainWindow3.Visible = false;
                    API.displaySubtitle("Метка первой работы на карте установлена", 5000);
                    waypoint = API.setWaypoint(853, -2927);
                });
                mainWindow3.AddItem(loader2);

                var taxiDriver = API.createMenuItem("Таксист", "Требуется уровень 2");
                taxiDriver.Activated.connect(function (menu, item) {
                    mainWindow3.Visible = false;
                    API.triggerServerEvent("work_taxi", 4);                  
                });
                mainWindow3.AddItem(taxiDriver);

                var busDriver = API.createMenuItem("Водитель автобуса", "Требуется уровень 2");
                busDriver.Activated.connect(function (menu, item) {
                    mainWindow3.Visible = false;
                    API.displaySubtitle("Метка первой работы на карте установлена", 5000);
                    waypoint = API.setWaypoint(-831, -2351);
                });
                mainWindow3.AddItem(busDriver);

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
    if (name == "autoschool_menu") {
        resetMainMenu();
        var callbackId = args[0];
        var propertyName = args[1];

        var banner = "";
        var subtitle = "";
        switch (propertyName) {
            case "Autoschool_main": banner = "Автошкола"; subtitle = "Получите здесь свои права!"; break;
        }  
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

    if (name == "gang_map") {
        gangMap = 1;
    }
    if (name == "send_user_posXY") {
        playerX = args[0];
        playerY = args[1];
    }
    if (name == "send_user_sector") {
        userSector = args[0];
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

    if (gangMap != 0) {
        var x = 300;
        var y = 50;

        var zeroPointX = -468;
        var zeroPointY = -2262;

        var zeroMapPointX = x + 101;
        var zeroMapPointY = y + 925;

        API.triggerServerEvent("ask_user_posXY");
        API.triggerServerEvent("ask_user_sector");
        //API.sendChatMessage(userSector);

        API.dxDrawTexture("client/img/1.png", new Point(x, y), new Size(1148, 997), 0);

        var xGangs = 0;
        var yGangs = 0;
        var inc = 0;

        for (var i = 0; i < 13; i++) {
            for (var j = 0; j < 13; j++) {
                var color = gangsSectors[j + inc];
                switch (color) {
                    case 0:   API.drawRectangle(x + 108 + xGangs, y + 932 - 65 - yGangs, 65.0, 65.0, 255, 255, 255, 255); break;
                    case 13:  API.drawRectangle(x + 108 + xGangs, y + 932 - 65 - yGangs, 65.0, 65.0, 100, 0, 100, 255); break;
                    case 14:  API.drawRectangle(x + 108 + xGangs, y + 932 - 65 - yGangs, 65.0, 65.0, 9, 15, 70, 255); break;
                    case 15:  API.drawRectangle(x + 108 + xGangs, y + 932 - 65 - yGangs, 65.0, 65.0, 100, 100, 0, 255); break;
                    case 16:  API.drawRectangle(x + 108 + xGangs, y + 932 - 65 - yGangs, 65.0, 65.0, 0, 100, 0, 255); break;
                    case 17:  API.drawRectangle(x + 108 + xGangs, y + 932 - 65 - yGangs, 65.0, 65.0, 0, 100, 100, 255); break;
                    case 130: API.drawRectangle(x + 108 + xGangs, y + 932 - 65 - yGangs, 65.0, 65.0, 72, 0, 58, 255); break;
                    case 140: API.drawRectangle(x + 108 + xGangs, y + 932 - 65 - yGangs, 65.0, 65.0, 0, 0, 100, 255); break;
                    case 150: API.drawRectangle(x + 108 + xGangs, y + 932 - 65 - yGangs, 65.0, 65.0, 100, 80, 0, 255); break;
                    case 160: API.drawRectangle(x + 108 + xGangs, y + 932 - 65 - yGangs, 65.0, 65.0, 0, 80, 0, 255); break;
                    case 170: API.drawRectangle(x + 108 + xGangs, y + 932 - 65 - yGangs, 65.0, 65.0, 0, 80, 100, 255); break;
                }                
                xGangs = xGangs + 66;
            }
            inc = inc + 13;
            xGangs = 0;
            yGangs = yGangs + 66;
        }
        
        var playerMarkerX = zeroMapPointX - Math.floor((zeroPointX - playerX) / 2);
        var playerMarkerY = zeroMapPointY + Math.floor((zeroPointY - playerY) / 2);

        if (x < playerMarkerX && playerMarkerX < x + 1148)
            if (y < playerMarkerY && playerMarkerY < y + 997)
                API.dxDrawTexture("client/img/player.png", new Point(playerMarkerX, playerMarkerY), new Size(14, 14), 0);
    }    

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