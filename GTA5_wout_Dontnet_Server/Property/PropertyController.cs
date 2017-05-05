using TheGodfatherGM.Data.Attributes;
using TheGodfatherGM.Data.Enums;
using TheGodfatherGM.Data.Extensions;
using GTANetworkServer;
using GTANetworkShared;
using System.Linq;
using TheGodfatherGM.Server.DBManager;
using TheGodfatherGM.Server.Characters;
using System;

namespace TheGodfatherGM.Server.Property
{
    public class PropertyController : Script
    {
        public Data.Property PropertyData;

        private string ownername = "None";
        public Groups.GroupController GroupController { get; private set; }

        public Marker ExteriorMarker { get; private set; }
        public TextLabel ExteriorTextLabel { get; private set; }
        public ColShape ExteriorColShape { get; private set; }

        public Marker InteriorMarker { get; private set; }
        public TextLabel InteriorTextLabel { get; private set; }
        public ColShape InteteriorColShape { get; private set; }
        public Blip Blip { get; private set; }

        // Rent place fields
        public Marker RentPlaceMarker { get; private set; }
        public TextLabel RentPlaceTextLabel { get; private set; }
        public ColShape RentPlaceColshape { get; private set; }

        //WorkLoader place fileds
        public Marker WorkLoaderMarker { get; private set; }
        public TextLabel WorkLoaderTextLabel { get; private set; }
        public ColShape WorkLoaderColshape { get; private set; }

        //Autoschool place fileds
        public Marker AutoschoolMarker { get; private set; }
        public TextLabel AutoschoolTextLabel { get; private set; }
        public ColShape AutoschoolColshape { get; private set; }

        //Meria place fileds
        public Marker MeriaMarker { get; private set; }
        public TextLabel MeriaTextLabel { get; private set; }
        public ColShape MeriaColshape { get; private set; }

        //Armys place fileds
        public Marker ArmysMarker { get; private set; }
        public TextLabel ArmysTextLabel { get; private set; }
        public ColShape ArmysColshape { get; private set; }
        public ColShape ArmysSourceColshape { get; private set; }

        //ArmyOne place fileds
        public ColShape ArmyOneColshape { get; private set; }
        public ColShape ArmyOneSourceColshape { get; private set; }

        //ArmyTwo place fileds
        public ColShape ArmyTwoColshape { get; private set; }
        public ColShape ArmyTwoStockColshape { get; private set; }

        //Gangs place fileds
        public Marker GangsMarker { get; private set; }
        public TextLabel GangsTextLabel { get; private set; }
        public ColShape GangsColshape { get; private set; }

        //GangBallas place fileds
        public Marker GangBallasMarker { get; private set; }
        public TextLabel GangBallasTextLabel { get; private set; }
        public ColShape GangBallasColshape { get; private set; }

        //Police place fileds
        public Marker PoliceMarker { get; private set; }
        public TextLabel PoliceTextLabel { get; private set; }
        public ColShape PoliceColshape { get; private set; }

        public PropertyController() { }
        public PropertyController(Data.Property PropertyData)
        {
            this.PropertyData = PropertyData;
        }
        public static void LoadProperties()
        {
            foreach (var property in ContextFactory.Instance.Property.ToList())
            {
                PropertyController propertyController = new PropertyController(property);
                if (property.Group != null)
                {
                    propertyController.GroupController = EntityManager.GetGroup(property.Group.Id);
                    API.shared.consoleOutput("Загружен маркер номер " + property.PropertyID + " для фракции : " + propertyController.GroupController.Group.Name);
                    if (propertyController.GroupController != null)
                    {
                        string name = property.Group.Name;
                        if (name == null) propertyController.ownername = "None";
                        else propertyController.ownername = property.Name;

                        if (propertyController.GroupController.Group.Type == GroupType.Business) // If property's group is a business, initialize items.
                        {
                        }
                    }
                }
                else if (property.Character != null)
                {
                    string name = property.Character.Name;
                    if (name == null) propertyController.ownername = "None";
                    else propertyController.ownername = name.Replace("_", " ");
                }

                propertyController.CreateWorldEntity();
                EntityManager.Add(propertyController);

            }
            API.shared.consoleOutput("[GM] Загружено маркеров: " + ContextFactory.Instance.Property.Count() + " шт.");
        }

        /*
        [Display(Name = "Invalid")]             Invalid = 0        
        [Display(Name = "Полиция")]             Police = 1
        [Display(Name = "Дверь")]               Door = 2
        [Display(Name = "Строение")]            Building = 3
        [Display(Name = "Прокат")]              Rent = 4
        [Display(Name = "Автошкола")]           Autoschool = 6
        [Display(Name = "Мэрия")]               Meria = 11
        [Display(Name = "Балласы")]             GangBallas = 13
        [Display(Name = "Военные 1")]           ArmyOne = 20
        [Display(Name = "Военные 2")]           ArmyTwo = 21        
        [Display(Name = "Жилой дом")]           House = 100
        */
        public void CreateWorldEntity()
        {
            if (PropertyData.Type == PropertyType.Invalid)
            {                
                ExteriorMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                new Vector3(1f, 1f, 1f), 150, 255, 255, 0);
                var propertyName = "";
                switch (PropertyData.Name)
                {
                    case "Roof": propertyName = "Крыша"; break;
                }
                ExteriorTextLabel = API.createTextLabel("~g~Вход в: " + propertyName, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);

                InteriorMarker = API.shared.createMarker(1, new Vector3(PropertyData.IntPosX, PropertyData.IntPosY, PropertyData.IntPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                new Vector3(1f, 1f, 1f), 150, 255, 255, 0);
                InteriorTextLabel = API.createTextLabel("~w~Выход из: " + propertyName, new Vector3(PropertyData.IntPosX, PropertyData.IntPosY, PropertyData.IntPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
            }
            if (PropertyData.Type == PropertyType.Building)
            {                
                Blip = API.createBlip(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 0);
                Blip.sprite = GetBlip();
                Blip.name = PropertyData.Name;
            }
            if (PropertyData.Type == PropertyType.House)
            {
                var owner = ContextFactory.Instance.Character.FirstOrDefault(x => x.Id == PropertyData.CharacterId);
                
                ExteriorMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                    new Vector3(1f, 1f, 1f), 150, 255, 255, 50);
                
                if (owner == null)
                {
                    var cost = PropertyData.Stock;
                    ExteriorTextLabel = API.createTextLabel("~g~Купить дом №"+ PropertyData.PropertyID +".\nСтоимость: " + cost + "$", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                }
                else
                {                    
                    ExteriorTextLabel = API.createTextLabel("~g~Вход в дом №" + PropertyData.PropertyID + ".\nВладелец: " + owner.Name, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                }

                InteriorMarker = API.shared.createMarker(1, new Vector3(PropertyData.IntPosX, PropertyData.IntPosY, PropertyData.IntPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                new Vector3(1f, 1f, 1f), 150, 255, 255, 0);
                InteriorTextLabel = API.createTextLabel("~w~Выход из дома.", new Vector3(PropertyData.IntPosX, PropertyData.IntPosY, PropertyData.IntPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);

                Blip = API.createBlip(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 0);
                Blip.sprite = 40;
                Blip.color = PropertyData.CharacterId == null ? 2 : 1;
                Blip.name = "Жилой дом";
            }
            if (PropertyData.Type == PropertyType.Rent)
            {
                RentPlaceMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                    new Vector3(2f, 2f, 2f), 150, 0, 255, 0);
                RentPlaceTextLabel = API.createTextLabel("~w~Прокат мопеда.\nВсего 30$ за полчаса", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                Blip = API.createBlip(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 0);
                Blip.sprite = 512;
                Blip.name = "Прокат";
            }
            if (PropertyData.Type == PropertyType.Autoschool)
            {
                AutoschoolMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                    new Vector3(1f, 1f, 1f), 250, 25, 50, 200);
                AutoschoolTextLabel = API.createTextLabel("~w~Автошкола", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                Blip = API.createBlip(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 0);
                Blip.sprite = 76;
                Blip.name = "Автошкола";
            }
            if (PropertyData.Type == PropertyType.Meria)
            {
                MeriaMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                    new Vector3(1f, 1f, 1f), 250, 25, 50, 200);
                MeriaTextLabel = API.createTextLabel("~w~Мэрия", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                Blip = API.createBlip(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 0);
                Blip.sprite = 181;
                Blip.name = "Мэрия";
            }
            if (PropertyData.Type == PropertyType.ArmyOne)
            {
                if (PropertyData.Name == "Army1_main")
                {
                    ArmysMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(3f, 3f, 3f), 250, 25, 50, 200);
                    ArmysTextLabel = API.createTextLabel("~w~Армия 1\nГлавный маркер", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                    Blip = API.createBlip(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 0);
                    Blip.sprite = 60;
                    Blip.name = "Армия 1";
                }
                if (PropertyData.Name == "Army1_source")
                {
                    ArmysMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(15f, 15f, 15f), 250, 50, 90, 0);
                    ArmysTextLabel = API.createTextLabel("~w~Армия 1\nМатериалы", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                    Blip = API.createBlip(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 0);
                    Blip.sprite = 60;
                    Blip.name = "Армия 1 : Материалы";
                }
                if (PropertyData.Name == "Army1_stock")
                {
                    ArmysMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(3f, 3f, 3f), 250, 25, 50, 200);
                    ArmysTextLabel = API.createTextLabel("~w~Армия 1\nГлавный склад", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                    Blip = API.createBlip(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 0);
                    Blip.sprite = 60;
                    Blip.name = "Армия 1 : Главный склад";
                }
                if (PropertyData.Name == "Army1_weapon")
                {
                    ArmysMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(2f, 2f, 2f), 255, 255, 50, 200);
                    ArmysTextLabel = API.createTextLabel("~w~Армия 1\nОружие", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                    Blip = API.createBlip(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 0);
                    Blip.sprite = 150;
                    Blip.name = "Армия 1 : Оружие";
                }
                if (PropertyData.Name == "Army1_gang")
                {
                    ArmysMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(1f, 1f, 1f), 255, 255, 0, 0);
                    ArmysTextLabel = API.createTextLabel("~w~Армия 1\nЛичка бандита", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                }
            }
            if (PropertyData.Type == PropertyType.ArmyTwo)
            {
                if (PropertyData.Name == "Army2_main")
                {
                    ArmysMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(3f, 3f, 3f), 250, 25, 50, 200);
                    ArmysTextLabel = API.createTextLabel("~w~Армия 2\nГлавный маркер", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                    Blip = API.createBlip(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 0);
                    Blip.sprite = 60;
                    Blip.name = "Армия 2";
                }
                if (PropertyData.Name == "Army2_stock")
                {
                    ArmysMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(3f, 3f, 3f), 250, 25, 50, 200);
                    ArmysTextLabel = API.createTextLabel("~w~Армия 2\nГлавный склад", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                    Blip = API.createBlip(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 0);
                    Blip.sprite = 60;
                    Blip.name = "Армия 2 : Главный склад";
                }
                if (PropertyData.Name == "Army2_weapon")
                {
                    ArmysMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(2f, 2f, 2f), 255, 255, 50, 200);
                    ArmysTextLabel = API.createTextLabel("~w~Армия 2\nОружие", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                    Blip = API.createBlip(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 0);
                    Blip.sprite = 150;
                    Blip.name = "Армия 2 : Оружие";
                }
                if (PropertyData.Name == "Army2_gang")
                {
                    ArmysMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(1f, 1f, 1f), 255, 255, 0, 0);
                    ArmysTextLabel = API.createTextLabel("~w~Армия 2\nЛичка бандита", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                }
            }
            if (PropertyData.Type == PropertyType.GangBallas)
            {
                if (PropertyData.Name == "Ballas_main")
                {
                    GangsMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(2f, 2f, 2f), 250, 153, 0, 153);
                    GangsTextLabel = API.createTextLabel("~w~Балласы\nГлавная", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                    Blip = API.createBlip(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 0);
                    Blip.sprite = 106;
                    Blip.name = "Балласы : Главная";
                }
                if (PropertyData.Name == "Ballas_stock")
                {
                    GangsMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(3f, 3f, 3f), 250, 25, 50, 200);
                    GangsTextLabel = API.createTextLabel("~w~Балласы\nСклад", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                    Blip = API.createBlip(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 0);
                    Blip.sprite = 106;
                    Blip.name = "Балласы : Склад";
                }
                if (PropertyData.Name == "Ballas_weapon")
                {
                    GangsMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(2f, 2f, 2f), 250, 25, 50, 200);
                    GangsTextLabel = API.createTextLabel("~w~Балласы\nОружие", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                    Blip = API.createBlip(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 0);
                    Blip.sprite = 106;
                    Blip.name = "Балласы : Оружие";
                }
            }
            if (PropertyData.Type == PropertyType.Police)
            {
                if (PropertyData.Name == "Police_stock")
                {
                    PoliceMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(3f, 3f, 3f), 250, 0, 255, 255);
                    PoliceTextLabel = API.createTextLabel("~w~Полиция\nСклад", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                    Blip = API.createBlip(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 0);
                    Blip.sprite = 188;
                    Blip.name = "Полиция : Склад";
                }
            }
            CreateColShape();
        }

        public void CreateColShape()
        {
            ExteriorColShape = API.createCylinderColShape(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 2f, 3f);
            ExteriorColShape.onEntityEnterColShape += (shape, entity) =>
            {
                if (API.getPlayerFromHandle(entity) != null)
                {
                    CharacterController characterController = API.getPlayerFromHandle(entity).getData("CHARACTER");
                    if (PropertyData.Name == "House")
                    {
                        if (PropertyData.Enterable && PropertyData.CharacterId == characterController.Character.Id)
                        {
                            API.shared.sendNotificationToPlayer(API.getPlayerFromHandle(entity), "Вы можете зайти сюда.\nНажмите N для входа.");
                            API.getPlayerFromHandle(entity).setData("AT_PROPERTY", this);
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "house_menu", PropertyData.PropertyID, PropertyData.Stock, 1, 0);
                        }
                        else if (PropertyData.CharacterId == characterController.Character.Id)
                        {
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "house_menu", PropertyData.PropertyID, PropertyData.Stock, 1, 1);
                        }
                    }
                }                
            };
            ExteriorColShape.onEntityExitColShape += (shape, entity) =>
            {
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {
                    API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "house_menu", PropertyData.PropertyID, PropertyData.Stock, 0, 1);
                    if (PropertyData.Enterable) player.resetData("AT_PROPERTY");
                }
            };

            InteteriorColShape = API.createCylinderColShape(new Vector3(PropertyData.IntPosX, PropertyData.IntPosY, PropertyData.IntPosZ), 2f, 3f);
            InteteriorColShape.onEntityEnterColShape += (shape, entity) =>
            {
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {
                    if (PropertyData.Enterable)
                    {
                        API.shared.sendNotificationToPlayer(player, "Выйти отсюда.\nНажмите N для выхода.");
                        player.setData("AT_PROPERTY", this);
                    }
                }
            };
            InteteriorColShape.onEntityExitColShape += (shape, entity) =>
            {
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {
                    if (PropertyData.Enterable) player.resetData("AT_PROPERTY");
                }
            };

            RentPlaceColshape = API.createCylinderColShape(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 2f, 3f);
            RentPlaceColshape.onEntityEnterColShape += (shape, entity) =>
            {
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {
                    if (PropertyData.Type == PropertyType.Rent)
                    {                        
                        API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "scooter_rent_menu", 1, "Прокат мопеда", "Возьмите на полчаса мопед всего за 30$", false, "Оплатить и поехать");
                    }
                }
            };
            RentPlaceColshape.onEntityExitColShape += (shape, entity) =>
            {
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {
                    if (PropertyData.Type == PropertyType.Rent)
                    API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "scooter_rent_menu", 0, "Прокат мопеда", "Возьмите на час мопед всего за 30$", false, "Оплатить и поехать"); 
                }
            };
            
            AutoschoolColshape = API.createCylinderColShape(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 2f, 3f);
            AutoschoolColshape.onEntityEnterColShape += (shape, entity) =>
            {
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {
                    if (PropertyData.Type == PropertyType.Autoschool)
                    {
                        API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "autoschool_menu", 1, "Автошкола", "Получите здесь свои права!");
                    }
                }
            };
            AutoschoolColshape.onEntityExitColShape += (shape, entity) =>
            {
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {
                    if (PropertyData.Type == PropertyType.Autoschool)
                        API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "autoschool_menu", 0, "Автошкола", "Получите здесь свои права!");
                }
            };
            
            MeriaColshape = API.createCylinderColShape(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 2f, 3f);
            MeriaColshape.onEntityEnterColShape += (shape, entity) =>
            {
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {                    
                    if (PropertyData.Type == PropertyType.Meria)
                    {
                        CharacterController characterController = player.getData("CHARACTER");
                        API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "work_meria_menu", 1, "Мэрия города", "Всегда рады помочь вам!", characterController.Character.Level);
                    }
                }
            };
            MeriaColshape.onEntityExitColShape += (shape, entity) =>
            {
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {
                    if (PropertyData.Type == PropertyType.Meria)
                    {
                        CharacterController characterController = player.getData("CHARACTER");
                        API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "work_meria_menu", 0, "Мэрия города", "Всегда рады помочь вам!", characterController.Character.Level);
                    }                        
                }
            };

            // ARMY ONE:
            ArmyOneSourceColshape = API.createCylinderColShape(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 10f, 5f);
            ArmyOneSourceColshape.onEntityEnterColShape += (shape, entity) =>
            {
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {
                    if (PropertyData.Type == PropertyType.ArmyOne)
                    {
                        CharacterController characterController = player.getData("CHARACTER");
                        var numberOfArmy = ContextFactory.Instance.Group.FirstOrDefault(x => x.Id == characterController.Character.ActiveGroupID);
                        var groupType = (GroupType)Enum.Parse(typeof(GroupType), numberOfArmy.Type.ToString());
                                                
                        if (PropertyData.Name == "Army1_source" &&
                        characterController.Character.GroupType == 20 &&
                        player.isInVehicle)
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "army_menu", 1, "Армия 1", "Вежливые люди и тут!", PropertyData.Name, 1);
                        
                        ContextFactory.Instance.SaveChanges();
                    }
                }
            };
            ArmyOneSourceColshape.onEntityExitColShape += (shape, entity) =>
            {
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {
                    if (PropertyData.Type == PropertyType.ArmyOne)
                    {
                        CharacterController characterController = player.getData("CHARACTER");
                        API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "army_menu", 0, "Армия 2", "Вежливые люди тут!", PropertyData.Name, 0);
                        //API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "gang_menu", 0, "", "Воровство", PropertyData.Name);
                    }
                }
            };

            ArmyOneColshape = API.createCylinderColShape(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 3f, 3f);
            ArmyOneColshape.onEntityEnterColShape += (shape, entity) =>
            {
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {
                    if (PropertyData.Type == PropertyType.ArmyOne)
                    {
                        CharacterController characterController = player.getData("CHARACTER");
                        var numberOfArmy = ContextFactory.Instance.Group.FirstOrDefault(x => x.Id == characterController.Character.ActiveGroupID);
                        var groupType = (GroupType)Enum.Parse(typeof(GroupType), numberOfArmy.Type.ToString());
                                                
                        if (PropertyData.Name == "Army1_weapon" && characterController.Character.GroupType == 20)
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "army_menu", 1, "Армия 1", "Вежливые люди и тут!", PropertyData.Name, 1);

                        if (PropertyData.Name == "Army1_main" &&
                        characterController.Character.ActiveGroupID > 2002 &&
                        characterController.Character.ActiveGroupID <= 2015)
                        {
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "army_menu", 1, "Армия 1", "Вежливые люди и тут!", PropertyData.Name, 1);
                        }

                        if (PropertyData.Name == "Army1_stock" &&
                        characterController.Character.GroupType == 20 && player.isInVehicle)
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "army_menu", 1, "Армия 2", "Вежливые люди и тут!", PropertyData.Name, 2);
                        
                        // GANG STEaLING:
                        if (PropertyData.Name == "Army1_gang" && CharacterController.IsCharacterInGang(characterController))
                        {
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "gang_menu", 1, EntityManager.GetDisplayName(groupType), "Воровство", PropertyData.Name, 0);
                        }
                        if (PropertyData.Name == "Army1_stock" &&
                        CharacterController.IsCharacterInGang(characterController) &&
                        !player.isInVehicle)
                        {
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "gang_menu", 1, EntityManager.GetDisplayName(groupType), "Воровство", PropertyData.Name, 0);
                        }
                        ContextFactory.Instance.SaveChanges();
                    }
                }
            };
            ArmyOneColshape.onEntityExitColShape += (shape, entity) =>
            {
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {
                    if (PropertyData.Type == PropertyType.ArmyOne)
                    {
                        CharacterController characterController = player.getData("CHARACTER");
                        API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "army_menu", 0, "Армия 2", "Вежливые люди тут!", PropertyData.Name, 0);
                        API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "gang_menu", 0, "", "Воровство", PropertyData.Name);
                    }
                }
            };

            // ARMY TWO:
            ArmyTwoStockColshape = API.createCylinderColShape(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 10f, 5f);
            ArmyTwoStockColshape.onEntityEnterColShape += (shape, entity) =>
            {
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {
                    if (PropertyData.Type == PropertyType.ArmyTwo)
                    {
                        CharacterController characterController = player.getData("CHARACTER");
                        
                        if (PropertyData.Name == "Army2_stock" && characterController.Character.GroupType == 20 &&
                        player.isInVehicle)
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "army_menu", 1, "Армия 2", "Вежливые люди тут!", PropertyData.Name, 2);

                        ContextFactory.Instance.SaveChanges();
                    }
                }
            };
            ArmyTwoStockColshape.onEntityExitColShape += (shape, entity) =>
            {
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {
                    if (PropertyData.Type == PropertyType.ArmyTwo)
                    {
                        CharacterController characterController = player.getData("CHARACTER");
                        API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "army_menu", 0, "Армия 2", "Вежливые люди тут!", PropertyData.Name, 0);
                    }
                }
            };

            ArmyTwoColshape = API.createCylinderColShape(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 3f, 3f);
            ArmyTwoColshape.onEntityEnterColShape += (shape, entity) =>
            {
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {
                    if (PropertyData.Type == PropertyType.ArmyTwo)
                    {
                        CharacterController characterController = player.getData("CHARACTER");
                        var numberOfArmy = ContextFactory.Instance.Group.FirstOrDefault(x => x.Id == characterController.Character.ActiveGroupID);
                        var groupType = (GroupType)Enum.Parse(typeof(GroupType), numberOfArmy.Type.ToString());

                        if (PropertyData.Name == "Army2_stock" && characterController.Character.GroupType == 21 && 
                        player.isInVehicle)
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "army_menu", 1, "Армия 2", "Вежливые люди тут!", PropertyData.Name, 1);

                        if (PropertyData.Name == "Army2_stock" && characterController.Character.GroupType == 20 &&
                        player.isInVehicle)
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "army_menu", 1, "Армия 2", "Вежливые люди тут!", PropertyData.Name, 2);

                        if (PropertyData.Name == "Army2_weapon" && characterController.Character.GroupType == 21)
                        API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "army_menu", 1, "Армия 2", "Вежливые люди тут!", PropertyData.Name, 1);

                        if (PropertyData.Name == "Army2_main" &&
                        characterController.Character.ActiveGroupID >= 2103 &&
                        characterController.Character.ActiveGroupID <= 2115)                        
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "army_menu", 1, "Армия 2", "Вежливые люди тут!", PropertyData.Name, 1);
                        
                        // GANG STEaLING:
                        if (PropertyData.Name == "Army2_gang" && 
                        CharacterController.IsCharacterInGang(characterController) && !player.isInVehicle)
                        //API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "gang_menu", 1, EntityManager.GetDisplayName(groupType), "Воровство", PropertyData.Name, 0);
                        {
                            if (PropertyData.Stock - 500 < 0)
                                API.sendChatMessageToPlayer(player, "~r~Вы не можете украсть с данного склада!\nНа складе нет материалов!");
                            else
                            {
                                if (characterController.Character.Material >= 500)
                                    API.sendChatMessageToPlayer(player, "~r~Вы не можете украсть с данного склада!\nВы перегружены у вас уже: " + characterController.Character.Material + " материалов");
                                else
                                {
                                    PropertyData.Stock -= 500;
                                    characterController.Character.Material = 500;
                                    ContextFactory.Instance.SaveChanges();
                                    API.sendChatMessageToPlayer(player, "~g~Вы украли 500 материалов со склада: " + EntityManager.GetDisplayName(groupType));
                                }
                            }
                        }

                        if (PropertyData.Name == "Army2_stock" &&
                        CharacterController.IsCharacterInGang(characterController) && !player.isInVehicle)
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "gang_menu", 1, EntityManager.GetDisplayName(groupType), "Воровство", PropertyData.Name, 0);
                        
                        ContextFactory.Instance.SaveChanges();
                    }
                }
            };
            ArmyTwoColshape.onEntityExitColShape += (shape, entity) =>
            {
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {
                    if (PropertyData.Type == PropertyType.ArmyTwo)
                    {
                        CharacterController characterController = player.getData("CHARACTER");
                        API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "army_menu", 0, "Армия 2", "Вежливые люди тут!", PropertyData.Name, 0);
                        API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "gang_menu", 0, "", "Воровство", PropertyData.Name, 0);
                    }
                }
            };

            GangBallasColshape = API.createCylinderColShape(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 3f, 3f);
            GangBallasColshape.onEntityEnterColShape += (shape, entity) =>
            {
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {
                    if (PropertyData.Type == PropertyType.GangBallas)
                    {
                        CharacterController characterController = player.getData("CHARACTER");
                        
                        if (PropertyData.Name == "Ballas_stock" &&
                        CharacterController.IsCharacterInGang(characterController) && player.isInVehicle)
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "gang_menu", 1, "Балласы", "Круче нас только яйца!", PropertyData.Name, 1, 0);

                        if (PropertyData.Name == "Ballas_main" && CharacterController.IsCharacterInGang(characterController))
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "gang_menu", 1, "Балласы", "Круче нас только яйца!", PropertyData.Name, 1, characterController.Character.ClothesTypes);
                    }
                }
            };
            GangBallasColshape.onEntityExitColShape += (shape, entity) =>
            {
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {
                    if (PropertyData.Type == PropertyType.GangBallas)
                    {
                        CharacterController characterController = player.getData("CHARACTER");                        
                        API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "gang_menu", 0, "", "Воровство", PropertyData.Name, 0);
                    }
                }
            };

            PoliceColshape = API.createCylinderColShape(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 3f, 3f);
            PoliceColshape.onEntityEnterColShape += (shape, entity) =>
            {
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {
                    if (PropertyData.Type == PropertyType.Police)
                    {
                        CharacterController characterController = player.getData("CHARACTER");
                        var nameOfGang = ContextFactory.Instance.Group.FirstOrDefault(x => x.Id == characterController.Character.ActiveGroupID);
                        var groupType = (GroupType)Enum.Parse(typeof(GroupType), nameOfGang.Type.ToString());

                        if (PropertyData.Name == "Police_stock" &&
                        characterController.Character.ActiveGroupID >= 2101 &&
                        characterController.Character.ActiveGroupID <= 2115 &&
                        player.isInVehicle)
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), 
                                "army_2_menu",          
                                1,                      // 0
                                "Армия 2",              // 1
                                "Вежливые люди тут!",   // 2
                                PropertyData.Name,      // 3
                                1);                     // 4

                        ContextFactory.Instance.SaveChanges();
                    }
                }
            };
            PoliceColshape.onEntityExitColShape += (shape, entity) =>
            {
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {
                    if (PropertyData.Type == PropertyType.Police)
                    {
                        CharacterController characterController = player.getData("CHARACTER");
                        API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "army_2_menu", 0, "Армия 2", "Вежливые люди тут!", PropertyData.Name, 0);
                    }
                }
            };
        }

        public static void CreateProperty(Client player, string ownerType, PropertyType type, string Name, int cost)
        {
            Data.Property propertyData = new Data.Property();
            string ownerName;
            if (ownerType == "player")
            {
                var targetCharacter = ContextFactory.Instance.Character.FirstOrDefault(x => x.SocialClub == player.socialClubName);
                if (targetCharacter == null) return;
                propertyData.Character = targetCharacter;
                ownerName = targetCharacter.Name;
            }
            else if (ownerType == "group")
            {
                Groups.GroupController groupController = EntityManager.GetGroup(player, Name);
                if (groupController == null) return;

                propertyData.Group = groupController.Group;
                ownerName = groupController.Group.Name;
            }
            else if (ownerType == "null")
            {
                propertyData.GroupId = null;
                ownerName = null;
            }
            else
            {
                API.shared.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~Вы указали неверный тип (player/group");
                return;
            }

            PropertyController propertyController = new PropertyController(propertyData);

            if ((int)type == 100) propertyData.Name = "House";

            propertyData.Stock = cost;
            propertyData.Type = type;
            propertyData.ExtPosX = player.position.X;
            propertyData.ExtPosY = player.position.Y;
            propertyData.ExtPosZ = player.position.Z;
            propertyData.Enterable = false;
            propertyController.ownername = ownerName;

            ContextFactory.Instance.Property.Add(propertyData);
            ContextFactory.Instance.SaveChanges();
            propertyController.CreateWorldEntity();
            API.shared.sendChatMessageToPlayer(player, "~g~[СЕРВЕР]: ~w~ Добавлен:  " + propertyController.Type() + "\nпринадлежащий: " + ownerName);
        }
        public int GetBlip()
        {
            const int DefaultBlipId = 1;

            if (PropertyData.Character != null) return 40;
            if(GroupController.Group.ExtraType != 0)
            {
                return GroupController.Group.ExtraType.GetAttributeOfType<BlipTypeAttribute>()?.BlipId ?? DefaultBlipId;
            }
            return GroupController.Group.Type.GetAttributeOfType<BlipTypeAttribute>()?.BlipId ?? DefaultBlipId;
        }
        public string Type()
        {
            return PropertyData.Type.GetDisplayName();
        }
        public void PropertyDoor(Client player)
        {
            CharacterController characterController = player.getData("CHARACTER");
            if (characterController == null) return;
            if (player.isInVehicle) return;
           
            if (!PropertyData.Enterable)
            {
                API.shared.sendNotificationToPlayer(player, "~g~Server: ~w~Вы не можете сюда войти.");
                return;
            }

            if (player.getData("IN_PROP") == this)
            {
                if (PropertyData.IPL != null) API.shared.sendNativeToPlayer(player, Hash.REMOVE_IPL, PropertyData.IPL); // API.removeIpl(property.IPL);
                player.resetData("IN_PROP");
                player.dimension = 0;
                player.position = ExteriorMarker.position;
            }
            else
            {
                if (PropertyData.IPL != null) API.shared.sendNativeToPlayer(player, Hash.REQUEST_IPL, PropertyData.IPL);  // API.requestIpl(property.IPL);
                player.setData("IN_PROP", this);
                player.position = InteriorMarker.position;
                player.dimension = PropertyData.PropertyID;
            }
        }

        [Command]
        public void door(Client player)
        {
            PropertyDoor(player);
        }
    }
}
