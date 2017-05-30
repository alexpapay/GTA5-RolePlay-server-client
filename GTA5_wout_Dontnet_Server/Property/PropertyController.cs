using TheGodfatherGM.Data.Attributes;
using TheGodfatherGM.Data.Enums;
using TheGodfatherGM.Data.Extensions;
using GTANetworkServer;
using GTANetworkShared;
using System.Linq;
using TheGodfatherGM.Server.DBManager;
using TheGodfatherGM.Server.Characters;
using System;
using TheGodfatherGM.Data;

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
        public ColShape ArmysColshapes { get; private set; }
        public ColShape ArmysGangColshape { get; private set; }
        public ColShape ArmyOneSourceColshape { get; private set; }
        public ColShape ArmysStocksColshape { get; private set; }

        //Gangs place fileds
        public Marker GangsMarker           { get; private set; }
        public TextLabel GangsTextLabel     { get; private set; }
        public ColShape GangsMainColshape   { get; private set; } // 2f
        public ColShape GangsStockColshape  { get; private set; } // 3f

        //Mafia place fileds
        public Marker MafiaMarker { get; private set; }
        public TextLabel MafiaTextLabel { get; private set; }
        public ColShape MafiaMainColshape { get; private set; } // 2f
        public ColShape MafiaStockColshape { get; private set; } // 3f

        //Police place fileds
        public Marker PoliceMarker          { get; private set; }
        public TextLabel PoliceTextLabel    { get; private set; }
        public ColShape PoliceMainColshape      { get; private set; }
        public ColShape PoliceStockColshape { get; private set; }

        //FBI place fileds
        public Marker FbiMarker { get; private set; }
        public TextLabel FbiTextLabel { get; private set; }
        public ColShape FbiMainColshape { get; private set; }
        public ColShape FbiStockColshape { get; private set; }

        //Emergency place fileds
        public Marker EmergencyMarker { get; private set; }
        public TextLabel EmergencyTextLabel { get; private set; }
        public ColShape EmergencyColshape { get; private set; }

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
                if (PropertyData.Name == "Rent_scooter")
                {
                    RentPlaceMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(2f, 2f, 2f), 150, 0, 255, 0);
                    RentPlaceTextLabel = API.createTextLabel("~w~Прокат мопеда.\nВсего 30$ за полчаса", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                    Blip = API.createBlip(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 0);
                    Blip.sprite = 512;
                    Blip.name = "Прокат мопеда";
                }                
            }
            if (PropertyData.Type == PropertyType.Autoschool)
            {
                if (PropertyData.Name == "Autoschool_main")
                {
                    AutoschoolMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                    new Vector3(1f, 1f, 1f), 250, 25, 50, 200);
                    AutoschoolTextLabel = API.createTextLabel("~w~Автошкола:\nГлавная", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                    Blip = API.createBlip(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 0);
                    Blip.sprite = 76;
                    Blip.name = "Автошкола";
                }                
            }
            if (PropertyData.Type == PropertyType.Meria)
            {
                if (PropertyData.Name == "Meria_main")
                {
                    MeriaMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                    new Vector3(1f, 1f, 1f), 250, 25, 50, 200);
                    MeriaTextLabel = API.createTextLabel("~w~Мэрия:\nГлавная", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                    Blip = API.createBlip(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 0);
                    Blip.sprite = 419;
                    Blip.name = "Мэрия";
                }                
            }
            if (PropertyData.Type == PropertyType.ArmyOne)
            {
                if (PropertyData.Name == "Army1_main")
                {
                    ArmysMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(3f, 3f, 3f), 250, 25, 50, 200);
                    ArmysTextLabel = API.createTextLabel("~w~Армия 1\nГлавный маркер", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                }
                if (PropertyData.Name == "Army1_source")
                {
                    ArmysMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(15f, 15f, 15f), 250, 20, 20, 50);
                    ArmysTextLabel = API.createTextLabel("~w~Армия 1\nМатериалы", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                    Blip = API.createBlip(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 0);
                    Blip.sprite = 481;
                    Blip.name = "Армия 1 : Исходные материалы";
                }
                if (PropertyData.Name == "Army1_stock")
                {
                    ArmysMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(10f, 10f, 10f), 100, 0, 80, 0);
                    ArmysTextLabel = API.createTextLabel("~w~Армия 1\nГлавный склад", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                    Blip = API.createBlip(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 0);
                    Blip.sprite = 421;
                    Blip.name = "Армия 1 : Главный склад";
                }
                if (PropertyData.Name == "Army1_weapon")
                {
                    ArmysMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(2f, 2f, 2f), 125, 0, 100, 0);
                    ArmysTextLabel = API.createTextLabel("~w~Армия 1\nОружие", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                    Blip = API.createBlip(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 0);
                    Blip.sprite = 150;
                    Blip.name = "Армия 1 : Оружие";
                }
                if (PropertyData.Name == "Army1_gang")
                {
                    ArmysMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(1f, 1f, 1f), 60, 255, 0, 0);
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
                }
                if (PropertyData.Name == "Army2_stock")
                {
                    ArmysMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(10f, 10f, 10f), 100, 0, 80, 0);
                    ArmysTextLabel = API.createTextLabel("~w~Армия 2\nГлавный склад", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 1.5f), 15.0f, 1.5f);
                    Blip = API.createBlip(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 0);
                    Blip.sprite = 421;
                    Blip.name = "Армия 2 : Главный склад";
                }
                if (PropertyData.Name == "Army2_weapon")
                {
                    ArmysMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(2f, 2f, 2f), 125, 0, 100, 0);
                    ArmysTextLabel = API.createTextLabel("~w~Армия 2\nОружие", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                    Blip = API.createBlip(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 0);
                    Blip.sprite = 150;
                    Blip.name = "Армия 2 : Оружие";
                }
                if (PropertyData.Name == "Army2_gang")
                {
                    ArmysMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(1f, 1f, 1f), 60, 255, 0, 0);
                    ArmysTextLabel = API.createTextLabel("~w~Армия 2\nЛичка бандита", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                }
            }
            if (PropertyData.Type == PropertyType.Gangs)
            {                
                if (PropertyData.Name == "Gangs_metall")
                {
                    GangsMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(1f, 1f, 1f), 70, 0, 100, 153);
                    GangsTextLabel = API.createTextLabel("~w~Сдача металла", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                    Blip = API.createBlip(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 0);
                    Blip.sprite = 478;
                    Blip.name = "Сдача металла";
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
                    //Blip = API.createBlip(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 0);
                    //Blip.sprite = 106;
                    //Blip.name = "Балласы : Склад";
                }
            } // 13
            if (PropertyData.Type == PropertyType.GangAzcas)
            {
                if (PropertyData.Name == "Azcas_main")
                {
                    GangsMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(2f, 2f, 2f), 250, 9, 15, 70);
                    GangsTextLabel = API.createTextLabel("~w~Azcas\nГлавная", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                    Blip = API.createBlip(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 0);
                    Blip.sprite = 76;
                    Blip.name = "Azcas : Главная";
                }
                if (PropertyData.Name == "Azcas_stock")
                {
                    GangsMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(3f, 3f, 3f), 250, 25, 50, 200);
                    GangsTextLabel = API.createTextLabel("~w~Azcas\nСклад", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                }
            }  // 14
            if (PropertyData.Type == PropertyType.GangVagos)
            {
                if (PropertyData.Name == "Vagos_main")
                {
                    GangsMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(2f, 2f, 2f), 250, 100, 100, 0);
                    GangsTextLabel = API.createTextLabel("~w~Vagos\nГлавная", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                    Blip = API.createBlip(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 0);
                    Blip.sprite = 120;
                    Blip.name = "Vagos : Главная";
                }
                if (PropertyData.Name == "Vagos_stock")
                {
                    GangsMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(3f, 3f, 3f), 250, 25, 50, 200);
                    GangsTextLabel = API.createTextLabel("~w~Vagos\nСклад", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                }
            }  // 15
            if (PropertyData.Type == PropertyType.GangGrove)
            {
                if (PropertyData.Name == "Grove_main")
                {
                    GangsMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(2f, 2f, 2f), 250, 0, 80, 0);
                    GangsTextLabel = API.createTextLabel("~w~Grove\nГлавная", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                    Blip = API.createBlip(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 0);
                    Blip.sprite = 77;
                    Blip.name = "Grove : Главная";
                }
                if (PropertyData.Name == "Grove_stock")
                {
                    GangsMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(3f, 3f, 3f), 250, 25, 50, 200);
                    GangsTextLabel = API.createTextLabel("~w~Grove\nСклад", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                }
            }  // 16
            if (PropertyData.Type == PropertyType.GangRifa)
            {
                if (PropertyData.Name == "Rifa_main")
                {
                    GangsMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(2f, 2f, 2f), 250, 0, 100, 100);
                    GangsTextLabel = API.createTextLabel("~w~Rifa\nГлавная", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                    Blip = API.createBlip(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 0);
                    Blip.sprite = 88;
                    Blip.name = "Rifa : Главная";
                }
                if (PropertyData.Name == "Rifa_stock")
                {
                    GangsMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(3f, 3f, 3f), 250, 25, 50, 200);
                    GangsTextLabel = API.createTextLabel("~w~Rifa\nСклад", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                }
            }   // 17
            if (PropertyData.Type == PropertyType.RussianMafia)
            {
                if (PropertyData.Name == "RussianMafia_main")
                {
                    MafiaMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(2f, 2f, 2f), 250, 0, 100, 100);
                    MafiaTextLabel = API.createTextLabel("~w~Russian Mafia\nГлавная", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                    Blip = API.createBlip(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 0);
                    Blip.sprite = 78;
                    Blip.name = "Russian Mafia : Главная";
                }
                if (PropertyData.Name == "RussianMafia_stock")
                {
                    MafiaMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(3f, 3f, 3f), 250, 25, 50, 200);
                    MafiaTextLabel = API.createTextLabel("~w~Russian Mafia\nСклад", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                }
            }   // 30
            if (PropertyData.Type == PropertyType.MafiaLKN)
            {
                if (PropertyData.Name == "MafiaLKN_main")
                {
                    MafiaMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(2f, 2f, 2f), 250, 0, 100, 100);
                    MafiaTextLabel = API.createTextLabel("~w~Mafia LKN\nГлавная", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                    Blip = API.createBlip(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 0);
                    Blip.sprite = 78;
                    Blip.name = "Mafia LKN : Главная";
                }
                if (PropertyData.Name == "MafiaLKN_stock")
                {
                    MafiaMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(3f, 3f, 3f), 250, 25, 50, 200);
                    MafiaTextLabel = API.createTextLabel("~w~Mafia LKN\nСклад", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                }
            }       // 31
            if (PropertyData.Type == PropertyType.MafiaArmeny)
            {
                if (PropertyData.Name == "MafiaArmeny_main")
                {
                    MafiaMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(2f, 2f, 2f), 250, 0, 100, 100);
                    MafiaTextLabel = API.createTextLabel("~w~Mafia Armeny\nГлавная", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                    Blip = API.createBlip(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ));
                    Blip.sprite = 78;
                    Blip.name = "Mafia Armeny : Главная";
                }
                if (PropertyData.Name == "MafiaArmeny_stock")
                {
                    MafiaMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(3f, 3f, 3f), 250, 25, 50, 200);
                    MafiaTextLabel = API.createTextLabel("~w~Mafia Armeny\nСклад", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                }
            }    // 32
            if (PropertyData.Type == PropertyType.Police)
            {
                if (PropertyData.Name == "Police_stock")
                {
                    PoliceMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(3f, 3f, 3f), 250, 0, 255, 255);
                    PoliceTextLabel = API.createTextLabel("~w~Полиция\nСклад", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                    Blip = API.createBlip(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ));
                    Blip.sprite = 56;
                    Blip.name = "Полиция : Склад";
                }
                if (PropertyData.Name == "Police_main")
                {
                    PoliceMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(2f, 2f, 2f), 250, 0, 255, 255);
                    PoliceTextLabel = API.createTextLabel("~w~Полиция\nГлавная", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                }
                if (PropertyData.Name == "Police_weapon")
                {
                    PoliceMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(1f, 1f, 1f), 250, 0, 255, 255);
                    PoliceTextLabel = API.createTextLabel("~w~Полиция\nОружие", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                }
            }
            if (PropertyData.Type == PropertyType.FBI)
            {
                if (PropertyData.Name == "FBI_main")
                {
                    FbiMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(2f, 2f, 2f), 250, 0, 255, 255);
                    FbiTextLabel = API.createTextLabel("~w~FBI\nГлавная", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                    Blip = API.createBlip(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ));
                    Blip.sprite = 60;
                    Blip.name = "FBI : Главная";
                }
                if (PropertyData.Name == "FBI_weapon")
                {
                    FbiMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(1f, 1f, 1f), 250, 0, 255, 255);
                    FbiTextLabel = API.createTextLabel("~w~FBI\nОружие", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                }
                if (PropertyData.Name == "FBI_stock")
                {
                    FbiMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(3f, 3f, 3f), 250, 0, 255, 255);
                    FbiTextLabel = API.createTextLabel("~w~FBI\nСклад", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                }
            }
            if (PropertyData.Type == PropertyType.Emergency)
            {
                if (PropertyData.Name == "Emergency_main")
                {
                    EmergencyMarker = API.shared.createMarker(1, new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) - new Vector3(0, 0, 1f), new Vector3(), new Vector3(),
                                                        new Vector3(2f, 2f, 2f), 250, 255, 10, 10);
                    EmergencyTextLabel = API.createTextLabel("~w~Emergency\nГлавная", new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ) + new Vector3(0.0f, 0.0f, 0.5f), 15.0f, 0.5f);
                    Blip = API.createBlip(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ));
                    Blip.sprite = 51;
                    Blip.name = "Emergency : Главная";
                }
            }
            CreateColShape();
        }

        public void CreateColShape()
        {
            ExteriorColShape = API.createCylinderColShape(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 1f, 1f);
            ExteriorColShape.onEntityEnterColShape += (shape, entity) =>
            {
                if (API.getPlayerFromHandle(entity) != null)
                {
                    CharacterController characterController = API.getPlayerFromHandle(entity).getData("CHARACTER");
                    if (PropertyData.Name == "House")
                    {
                        // It`s your house
                        if (PropertyData.Enterable && PropertyData.CharacterId == characterController.Character.Id)
                        {
                            API.shared.sendNotificationToPlayer(API.getPlayerFromHandle(entity), "Вы можете зайти сюда.\nНажмите N для входа.");
                            API.getPlayerFromHandle(entity).setData("AT_PROPERTY", this);
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), 
                                "house_menu", PropertyData.PropertyID, PropertyData.Stock, 1, 0);
                        }
                        // House for sale
                        if (PropertyData.CharacterId == null)
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), 
                                "house_menu", PropertyData.PropertyID, PropertyData.Stock, 1, 1);
                        //  Steal from the house
                        if (PropertyData.CharacterId != null && 
                        CharacterController.IsCharacterInGang(characterController) &&
                        PropertyData.Enterable && PropertyData.CharacterId != characterController.Character.Id)
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), 
                                "house_menu", PropertyData.PropertyID, PropertyData.Stock, 1, 2, PropertyData.CharacterId);
                    }
                }                
            };
            ExteriorColShape.onEntityExitColShape += (shape, entity) =>
            {
                if (API.getPlayerFromHandle(entity) != null && PropertyData.Name == "House")
                {
                    API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), 
                        "house_menu", PropertyData.PropertyID, PropertyData.Stock, 0, 3, 0);
                    if (PropertyData.Enterable) API.getPlayerFromHandle(entity).resetData("AT_PROPERTY");
                }
            };

            InteteriorColShape = API.createCylinderColShape(new Vector3(PropertyData.IntPosX, PropertyData.IntPosY, PropertyData.IntPosZ), 1f, 1f);
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
                if ((player = API.getPlayerFromHandle(entity)) == null) return;
                if (PropertyData.Enterable) player.resetData("AT_PROPERTY");
            };

            RentPlaceColshape = API.createCylinderColShape(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 2f, 3f);
            RentPlaceColshape.onEntityEnterColShape += (shape, entity) =>
            {
                if (API.getPlayerFromHandle(entity) != null)
                {
                    if (PropertyData.Name == "Rent_scooter")                     
                        API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "scooter_rent_menu", 1, PropertyData.Name);
                }
            };
            RentPlaceColshape.onEntityExitColShape += (shape, entity) =>
            {
                if (API.getPlayerFromHandle(entity) == null) return;
                if (PropertyData.Name == "Rent_scooter")
                    API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "scooter_rent_menu", 0, PropertyData.Name);
            };
            
            AutoschoolColshape = API.createCylinderColShape(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 2f, 3f);
            AutoschoolColshape.onEntityEnterColShape += (shape, entity) =>
            {
                if (API.getPlayerFromHandle(entity) != null)
                {
                    if (PropertyData.Name == "Autoschool_main")
                        API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "autoschool_menu", 1, PropertyData.Name);
                }
            };
            AutoschoolColshape.onEntityExitColShape += (shape, entity) =>
            {
                if (API.getPlayerFromHandle(entity) == null) return;
                if (PropertyData.Name == "Autoschool_main")
                    API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "autoschool_menu", 0, PropertyData.Name);
            };
            
            MeriaColshape = API.createCylinderColShape(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 2f, 3f);
            MeriaColshape.onEntityEnterColShape += (shape, entity) =>
            {
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {                    
                    if (PropertyData.Name == "Meria_main")
                    {
                        CharacterController characterController = player.getData("CHARACTER");
                        API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "work_meria_menu", 1, characterController.Character.Level, PropertyData.Name);
                    }
                }
            };
            MeriaColshape.onEntityExitColShape += (shape, entity) =>
            {
                if (API.getPlayerFromHandle(entity) != null)
                {
                    if (PropertyData.Name == "Meria_main")
                        API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "work_meria_menu", 0, 0, PropertyData.Name);
                }
            };

            // ARMY Stocks both (10.0f):
            ArmysStocksColshape = API.createCylinderColShape(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 10f, 5f);
            ArmysStocksColshape.onEntityEnterColShape += (shape, entity) =>
            {
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {
                    CharacterController characterController = player.getData("CHARACTER");
                    Character character = characterController.Character;

                    if (PropertyData.Type == PropertyType.ArmyOne || PropertyData.Type == PropertyType.ArmyTwo)
                    {
                        if (PropertyData.Name == "Army1_stock" && character.GroupType == 21 && player.isInVehicle)
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "army_menu", 1, PropertyData.Name, character.GroupType, PropertyData.Stock);

                        if (PropertyData.Name == "Army2_stock" && character.GroupType == 20 && player.isInVehicle)
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "army_menu", 1, PropertyData.Name, character.GroupType, PropertyData.Stock);

                        if (PropertyData.Name == "Army2_stock" && character.GroupType == 21 && player.isInVehicle)
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "army_menu", 1, PropertyData.Name, character.GroupType, PropertyData.Stock);

                        if (PropertyData.Name == "Army1_stock" || PropertyData.Name == "Army2_stock")
                            if (CharacterController.IsCharacterInGang(characterController) && !player.isInVehicle)
                                API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "gang_menu", 1, PropertyData.Name);
                    }
                }
            };
            ArmysStocksColshape.onEntityExitColShape += (shape, entity) =>
            {
                if (API.getPlayerFromHandle(entity) != null)
                {
                    if (PropertyData.Name == "Army2_stock" || PropertyData.Name == "Army1_stock")
                        API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "army_menu", 0, PropertyData.Name, 0, 0);
                    if (PropertyData.Name == "Army1_stock" || PropertyData.Name == "Army2_stock")
                        API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "gang_menu", 0, PropertyData.Name);
                }
            };

            // ARMY ONE Source loading (10.0f):
            ArmyOneSourceColshape = API.createCylinderColShape(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 10f, 5f);
            ArmyOneSourceColshape.onEntityEnterColShape += (shape, entity) =>
            {
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {
                    if (PropertyData.Type == PropertyType.ArmyOne)
                    {
                        CharacterController characterController = player.getData("CHARACTER");
                        Character character = characterController.Character;

                        if (PropertyData.Name == "Army1_source" && character.GroupType == 20 && player.isInVehicle)
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "army_menu", 1, PropertyData.Name, character.GroupType);
                    } 
                }
            };
            ArmyOneSourceColshape.onEntityExitColShape += (shape, entity) =>
            {
                if (API.getPlayerFromHandle(entity) != null)
                {
                    if (PropertyData.Name == "Army1_source")
                        API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "army_menu", 0, PropertyData.Name, 0);                    
                }
            };

            // ARMYs all colshapes (2.0f):
            ArmysColshapes = API.createCylinderColShape(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 2f, 2f);
            ArmysColshapes.onEntityEnterColShape += (shape, entity) =>
            {
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {
                    if (PropertyData.Type == PropertyType.ArmyOne || PropertyData.Type == PropertyType.ArmyTwo)
                    {
                        CharacterController characterController = player.getData("CHARACTER");
                        Character character = characterController.Character;

                        if (PropertyData.Name == "Army1_main" && character.ActiveGroupID > 2002 && character.ActiveGroupID <= 2015)
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "army_menu", 1, PropertyData.Name, character.GroupType);
                        if (PropertyData.Name == "Army2_main" && character.ActiveGroupID > 2102 && character.ActiveGroupID <= 2115)
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "army_menu", 1, PropertyData.Name, character.GroupType);
                        
                        if (PropertyData.Name == "Army1_weapon" && character.GroupType == 20)
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "army_menu", 1, PropertyData.Name, character.GroupType);
                        if (PropertyData.Name == "Army2_weapon" && character.GroupType == 21)
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "army_menu", 1, PropertyData.Name, character.GroupType);
                    }
                }
            };
            ArmysColshapes.onEntityExitColShape += (shape, entity) =>
            {
                if (API.getPlayerFromHandle(entity) != null)
                {
                    if (PropertyData.Type == PropertyType.ArmyOne || PropertyData.Type == PropertyType.ArmyTwo)
                    {
                        if (PropertyData.Name == "Army1_main" || PropertyData.Name == "Army2_main")
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "army_menu", 0, PropertyData.Name, 0);
                        if (PropertyData.Name == "Army1_weapon" || PropertyData.Name == "Army2_weapon")
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "army_menu", 0, PropertyData.Name, 0);
                    }
                }
            };

            // Army One shapes:
            ArmysGangColshape = API.createCylinderColShape(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 1f, 2f);
            ArmysGangColshape.onEntityEnterColShape += (shape, entity) =>
            {
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {
                    CharacterController characterController = player.getData("CHARACTER");

                    // GANG STEaLING:
                    if (PropertyData.Name == "Army1_gang" || PropertyData.Name == "Army2_gang")
                        if (CharacterController.IsCharacterInGang(characterController))
                        {
                            Data.Property currentStock = new Data.Property();
                            switch (PropertyData.Name)
                            {
                                case "Army1_gang": currentStock = ContextFactory.Instance.Property.First(x => x.Name == "Army1_stock"); break;
                                case "Army2_gang": currentStock = ContextFactory.Instance.Property.First(x => x.Name == "Army2_stock"); break;
                            }
                            
                            if (currentStock.Stock - 500 < 0)
                                API.sendNotificationToPlayer(player, "~r~Вы не можете украсть! На данном складе нет материалов!");
                            else
                            {
                                if (characterController.Character.Material >= 500)
                                    API.sendNotificationToPlayer(player, "~r~Вы не можете украсть! Вы перегружены у вас: " + characterController.Character.Material + " материалов");
                                else
                                {
                                    currentStock.Stock -= 500;
                                    characterController.Character.Material = 500;
                                    ContextFactory.Instance.SaveChanges();
                                    API.sendNotificationToPlayer(player, "~g~Вы украли 500 материалов со склада!");
                                }
                            }
                        }                        
                }
            };

            // For all Gangs, MAIN (2f dimension):
            GangsMainColshape = API.createCylinderColShape(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 2f, 2f);
            GangsMainColshape.onEntityEnterColShape += (shape, entity) =>
            {
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {
                    CharacterController characterController = player.getData("CHARACTER");
                    var character = characterController.Character;
                    var gangRank = characterController.Character.ActiveGroupID - characterController.Character.GroupType * 100;
                    var moneyInGang = new Group();
                    var moneyBankGroup = characterController.Character.GroupType * 100;
                    try { moneyInGang = ContextFactory.Instance.Group.First(x => x.Id == moneyBankGroup); }
                    catch (Exception)
                    {
                        // ignored
                    }

                    switch (PropertyData.Type)
                    {
                        case PropertyType.Gangs:
                            if (PropertyData.Name == "Gangs_metall" &&
                                CharacterController.IsCharacterInGang(characterController) &&
                                characterController.Character.TempVar == 111)                                
                                    API.triggerClientEvent(API.getPlayerFromHandle(entity), "gang_menu", 1, 
                                        PropertyData.Name, characterController.Character.ClothesTypes, 0, gangRank, moneyInGang.MoneyBank);
                            else    API.sendNotificationToPlayer(API.getPlayerFromHandle(entity), "~r~У вас нет металла для сдачи!"); break;

                        case PropertyType.GangBallas:                            
                            if (PropertyData.Name == "Ballas_main" && character.GroupType == 13)
                                    API.triggerClientEvent(API.getPlayerFromHandle(entity), "gang_menu", 1, 
                                        PropertyData.Name, characterController.Character.ClothesTypes,
                                        ContextFactory.Instance.Property.First(x => x.Name == "Ballas_stock").Stock,
                                        gangRank, moneyInGang.MoneyBank);
                            break;
                        case PropertyType.GangAzcas:
                            if (PropertyData.Name == "Azcas_main" && character.GroupType == 14)
                                    API.triggerClientEvent(API.getPlayerFromHandle(entity), "gang_menu", 1, 
                                        PropertyData.Name, characterController.Character.ClothesTypes,
                                        ContextFactory.Instance.Property.First(x => x.Name == "Azcas_stock").Stock, 
                                        gangRank, moneyInGang.MoneyBank);
                            break;
                        case PropertyType.GangVagos:
                            if (PropertyData.Name == "Vagos_main" && character.GroupType == 15)
                                    API.triggerClientEvent(API.getPlayerFromHandle(entity), "gang_menu", 1, 
                                        PropertyData.Name, characterController.Character.ClothesTypes,
                                        ContextFactory.Instance.Property.First(x => x.Name == "Vagos_stock").Stock,
                                        gangRank, moneyInGang.MoneyBank);
                            break;
                        case PropertyType.GangGrove:
                            if (PropertyData.Name == "Grove_main" && character.GroupType == 16)
                                    API.triggerClientEvent(API.getPlayerFromHandle(entity), "gang_menu", 1, 
                                        PropertyData.Name, characterController.Character.ClothesTypes,
                                        ContextFactory.Instance.Property.First(x => x.Name == "Grove_stock").Stock,
                                        gangRank, moneyInGang.MoneyBank);
                            break;
                        case PropertyType.GangRifa:
                            if (PropertyData.Name == "Rifa_main" && character.GroupType == 17)
                                    API.triggerClientEvent(API.getPlayerFromHandle(entity), "gang_menu", 1, 
                                        PropertyData.Name, characterController.Character.ClothesTypes,
                                        ContextFactory.Instance.Property.First(x => x.Name == "Rifa_stock").Stock, 
                                        gangRank, moneyInGang.MoneyBank);
                            break;
                    }                    
                }
            };
            GangsMainColshape.onEntityExitColShape += (shape, entity) =>
            {
                if (API.getPlayerFromHandle(entity) != null)
                {
                    if (PropertyData.Name == "Gangs_metall" || PropertyData.Name == "Ballas_main" ||
                    PropertyData.Name == "Azcas_main" || PropertyData.Name == "Vagos_main" ||
                    PropertyData.Name == "Grove_main" || PropertyData.Name == "Rifa_main")
                        API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "gang_menu", 0, PropertyData.Name, 0, 0, 0, 0);
                }
            };

            // For all Gangs, STOCK (3f dimension):
            GangsStockColshape = API.createCylinderColShape(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 3f, 3f);
            GangsStockColshape.onEntityEnterColShape += (shape, entity) =>
            {
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {
                    CharacterController characterController = player.getData("CHARACTER");
                    Character character = characterController.Character;

                    switch (PropertyData.Type)
                    {
                        case PropertyType.GangBallas:
                            if (PropertyData.Name == "Ballas_stock" && character.GroupType == 13 && 
                                player.isInVehicle)
                                    API.triggerClientEvent(API.getPlayerFromHandle(entity), "gang_menu", 1, PropertyData.Name);
                            break;
                        case PropertyType.GangAzcas:
                            if (PropertyData.Name == "Azcas_stock" && character.GroupType == 14 && 
                                player.isInVehicle)
                                    API.triggerClientEvent(API.getPlayerFromHandle(entity), "gang_menu", 1, PropertyData.Name);
                            break;
                        case PropertyType.GangVagos:
                            if (PropertyData.Name == "Vagos_stock" && character.GroupType == 15 && 
                                player.isInVehicle)
                                    API.triggerClientEvent(API.getPlayerFromHandle(entity), "gang_menu", 1, PropertyData.Name);
                            break;
                        case PropertyType.GangGrove:
                            if (PropertyData.Name == "Grove_stock" && character.GroupType == 16 && 
                                player.isInVehicle)
                                    API.triggerClientEvent(API.getPlayerFromHandle(entity), "gang_menu", 1, PropertyData.Name);
                            break;
                        case PropertyType.GangRifa:
                            if (PropertyData.Name == "Rifa_stock" && character.GroupType == 17 && 
                                player.isInVehicle)
                                    API.triggerClientEvent(API.getPlayerFromHandle(entity), "gang_menu", 1, PropertyData.Name);
                            break;
                    }
                }
            };
            GangsStockColshape.onEntityExitColShape += (shape, entity) =>
            {
                if (API.getPlayerFromHandle(entity) != null)
                {
                    if (PropertyData.Name == "Ballas_stock" || PropertyData.Name == "Azcas_stock" ||
                    PropertyData.Name == "Vagos_stock" || PropertyData.Name == "Grove_stock" || PropertyData.Name == "Rifa_stock")
                        API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "gang_menu", 0, PropertyData.Name);
                }
            };

            // For all Mafias, MAIN (2f dimension):
            MafiaMainColshape = API.createCylinderColShape(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 2f, 2f);
            MafiaMainColshape.onEntityEnterColShape += (shape, entity) =>
            {
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {
                    CharacterController characterController = player.getData("CHARACTER");
                    Character character = characterController.Character;

                    switch (PropertyData.Type)
                    {                        
                        case PropertyType.RussianMafia:
                            if (PropertyData.Name == "RussianMafia_main" && character.GroupType == 30)
                                API.triggerClientEvent(API.getPlayerFromHandle(entity), "mafia_menu", 1, PropertyData.Name);
                            break;
                        case PropertyType.MafiaLKN:
                            if (PropertyData.Name == "MafiaLKN_main" && character.GroupType == 31)
                                API.triggerClientEvent(API.getPlayerFromHandle(entity), "mafia_menu", 1, PropertyData.Name);
                            break;
                        case PropertyType.MafiaArmeny:
                            if (PropertyData.Name == "MafiaArmeny_main" && character.GroupType == 32)
                                API.triggerClientEvent(API.getPlayerFromHandle(entity), "mafia_menu", 1, PropertyData.Name);
                            break;                        
                    }
                }
            };
            MafiaMainColshape.onEntityExitColShape += (shape, entity) =>
            {
                if (API.getPlayerFromHandle(entity) != null)
                {
                    if (PropertyData.Name == "RussianMafia_main" || PropertyData.Name == "MafiaLKN_main"
                    || PropertyData.Name == "MafiaArmeny_main")
                        API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "mafia_menu", 0, PropertyData.Name);
                }
            };

            // For all Mafias, MAIN (3f dimension):
            MafiaStockColshape = API.createCylinderColShape(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 3f, 3f);
            MafiaStockColshape.onEntityEnterColShape += (shape, entity) =>
            {
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {
                    CharacterController characterController = player.getData("CHARACTER");
                    Character character = characterController.Character;
                    var mafiaRank = Convert.ToInt32(characterController.Character.ActiveGroupID) - characterController.Character.GroupType * 100;

                    switch (PropertyData.Type)
                    {
                        case PropertyType.RussianMafia:
                            if (PropertyData.Name == "RussianMafia_stock" && character.GroupType == 30)
                                API.triggerClientEvent(API.getPlayerFromHandle(entity), "mafia_menu", 1, 
                                    PropertyData.Name, mafiaRank, CharacterController.IsCharacterInMafia(character));
                            break;
                        case PropertyType.MafiaLKN:
                            if (PropertyData.Name == "MafiaLKN_stock" && character.GroupType == 31)
                                API.triggerClientEvent(API.getPlayerFromHandle(entity), "mafia_menu", 1,
                                    PropertyData.Name, mafiaRank, CharacterController.IsCharacterInMafia(character));
                            break;
                        case PropertyType.MafiaArmeny:
                            if (PropertyData.Name == "MafiaArmeny_stock" && character.GroupType == 32)
                                API.triggerClientEvent(API.getPlayerFromHandle(entity), "mafia_menu", 1,
                                    PropertyData.Name, mafiaRank, CharacterController.IsCharacterInMafia(character));
                            break;
                    }
                }
            };
            MafiaStockColshape.onEntityExitColShape += (shape, entity) =>
            {
                if (API.getPlayerFromHandle(entity) != null)
                {
                    if (PropertyData.Name == "RussianMafia_stock" || PropertyData.Name == "MafiaLKN_stock"
                    || PropertyData.Name == "MafiaArmeny_stock")
                        API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "mafia_menu", 0, PropertyData.Name);
                }
            };

            // Police main (2f dimension)
            PoliceMainColshape = API.createCylinderColShape(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 3f, 3f);
            PoliceMainColshape.onEntityEnterColShape += (shape, entity) =>
            {
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {
                    if (PropertyData.Type == PropertyType.Police)
                    {
                        CharacterController characterController = player.getData("CHARACTER");

                        if (PropertyData.Name == "Police_main" && CharacterController.IsCharacterInPolice(characterController))
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "police_menu", 1, PropertyData.Name, 1);

                        if (PropertyData.Name == "Police_weapon" && CharacterController.IsCharacterInPolice(characterController))
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "police_menu", 1, PropertyData.Name, 1);
                    }
                }
            };
            PoliceMainColshape.onEntityExitColShape += (shape, entity) =>
            {
                if (API.getPlayerFromHandle(entity) != null)
                {
                    if (PropertyData.Name == "Police_main" || PropertyData.Name == "Police_weapon")
                        API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "police_menu", 0, PropertyData.Name, 0);
                }
            };

            // Police stock (3f dimension)
            PoliceStockColshape = API.createCylinderColShape(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 3f, 3f);
            PoliceStockColshape.onEntityEnterColShape += (shape, entity) =>
            {
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {
                    if (PropertyData.Type == PropertyType.Police)
                    {
                        CharacterController characterController = player.getData("CHARACTER");

                        if (PropertyData.Name == "Police_stock" &&
                        CharacterController.IsCharacterInArmy(characterController) && player.isInVehicle)
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "army_menu", 1, PropertyData.Name, characterController.Character.GroupType);
                    }
                }
            };
            PoliceStockColshape.onEntityExitColShape += (shape, entity) =>
            {
                if (API.getPlayerFromHandle(entity) != null)
                {
                    if (PropertyData.Name == "Police_stock")
                        API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "army_menu", 0, PropertyData.Name, 0);
                }
            };

            // FBI main (2f dimension)
            FbiMainColshape = API.createCylinderColShape(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 2f, 2f);
            FbiMainColshape.onEntityEnterColShape += (shape, entity) =>
            {
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {
                    if (PropertyData.Type == PropertyType.FBI)
                    {
                        CharacterController characterController = player.getData("CHARACTER");

                        if (PropertyData.Name == "FBI_main" && CharacterController.IsCharacterInFbi(characterController))
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "fbi_menu", 0, PropertyData.Name, 0);

                        if (PropertyData.Name == "FBI_weapon" && CharacterController.IsCharacterInFbi(characterController))
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "fbi_menu", 0, PropertyData.Name, 0);
                    }
                }
            };
            FbiMainColshape.onEntityExitColShape += (shape, entity) =>
            {
                if (API.getPlayerFromHandle(entity) != null)
                {
                    if (PropertyData.Name == "FBI_main" || PropertyData.Name == "FBI_weapon")
                        API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "fbi_menu", 0, PropertyData.Name, 0);
                }
            };

            // FBI stock (3f dimension)
            FbiStockColshape = API.createCylinderColShape(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 3f, 3f);
            FbiStockColshape.onEntityEnterColShape += (shape, entity) =>
            {
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {
                    if (PropertyData.Type == PropertyType.FBI)
                    {
                        CharacterController characterController = player.getData("CHARACTER");

                        if (PropertyData.Name == "FBI_stock" && 
                        CharacterController.IsCharacterInArmy(characterController) && player.isInVehicle)
                            API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "army_menu", 1, PropertyData.Name, characterController.Character.GroupType);                        
                    }
                }
            };
            FbiStockColshape.onEntityExitColShape += (shape, entity) =>
            {
                if (API.getPlayerFromHandle(entity) != null)
                {
                    if (PropertyData.Name == "FBI_stock")
                        API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "army_menu", 0, PropertyData.Name, 0);
                }
            };
            
            // Emergency
            EmergencyColshape = API.createCylinderColShape(new Vector3(PropertyData.ExtPosX, PropertyData.ExtPosY, PropertyData.ExtPosZ), 2f, 2f);
            EmergencyColshape.onEntityEnterColShape += (shape, entity) =>
            {
                Client player;
                if ((player = API.getPlayerFromHandle(entity)) != null)
                {
                    if (PropertyData.Type == PropertyType.Emergency)
                    {
                        CharacterController characterController = player.getData("CHARACTER");

                        if (PropertyData.Name == "Emergency_main" )
                            /*API.shared.triggerClientEvent(API.getPlayerFromHandle(entity),
                                "army_menu",
                                1,                           // 0
                                "FBI stock",                 // 1
                                "Разгрузка на складе FBI",   // 2
                                PropertyData.Name,           // 3
                                1);                          // 4*/

                        ContextFactory.Instance.SaveChanges();
                    }
                }
            };
            EmergencyColshape.onEntityExitColShape += (shape, entity) =>
            {
                if (API.getPlayerFromHandle(entity) != null)
                {
                    if (PropertyData.Name == "Emergency_main")
                    {
                        //API.shared.triggerClientEvent(API.getPlayerFromHandle(entity), "army_2_menu", 0, "Армия 2", "Вежливые люди тут!", PropertyData.Name, 0);
                    }
                }
            };
        }

        public static void CreateHome(Client player, int cost)
        {
            var propertyData = new Data.Property
            {
                CharacterId = null,
                GroupId = null
            };

            var propertyController = new PropertyController(propertyData);

            propertyData.Name = "House";

            propertyData.Stock = cost;
            propertyData.Type = PropertyType.House;
            propertyData.ExtPosX = player.position.X;
            propertyData.ExtPosY = player.position.Y;
            propertyData.ExtPosZ = player.position.Z;
            propertyData.Enterable = true;

            ContextFactory.Instance.Property.Add(propertyData);
            ContextFactory.Instance.SaveChanges();
            propertyController.CreateWorldEntity();
            API.shared.sendChatMessageToPlayer(player, "~g~[СЕРВЕР]: ~w~ Добавлен новый дом!");
        }
        public static void CreateProperty(Client player, string ownerType, PropertyType type, string name, int cost)
        {
            var propertyData = new Data.Property();
            string ownerName;
            switch (ownerType)
            {
                case "player":
                    var targetCharacter = ContextFactory.Instance.Character.FirstOrDefault(x => x.SocialClub == player.socialClubName);
                    if (targetCharacter == null) return;
                    propertyData.Character = targetCharacter;
                    ownerName = targetCharacter.Name;
                    break;
                case "group":
                    var groupController = EntityManager.GetGroup(player, name);
                    if (groupController == null) return;

                    propertyData.Group = groupController.Group;
                    ownerName = groupController.Group.Name;
                    break;
                case "null":
                    propertyData.GroupId = null;
                    ownerName = null;
                    break;
                default:
                    API.shared.sendChatMessageToPlayer(player, "~r~[ОШИБКА]: ~w~Вы указали неверный тип (player/group");
                    return;
            }

            var propertyController = new PropertyController(propertyData);

            if ((int)type == 100) propertyData.Name = "House";

            propertyData.Stock = cost;
            propertyData.Type = type;
            propertyData.ExtPosX = player.position.X;
            propertyData.ExtPosY = player.position.Y;
            propertyData.ExtPosZ = player.position.Z;
            propertyData.Enterable = true;
            propertyController.ownername = ownerName;

            ContextFactory.Instance.Property.Add(propertyData);
            ContextFactory.Instance.SaveChanges();
            propertyController.CreateWorldEntity();
            API.shared.sendChatMessageToPlayer(player, "~g~[СЕРВЕР]: ~w~ Добавлен:  " + propertyController.Type() + "\nпринадлежащий: " + ownerName);
        }
        public int GetBlip()
        {
            const int defaultBlipId = 1;

            if (PropertyData.Character != null) return 40;
            if(GroupController.Group.ExtraType != 0)
            {
                return GroupController.Group.ExtraType.GetAttributeOfType<BlipTypeAttribute>()?.BlipId ?? defaultBlipId;
            }
            return GroupController.Group.Type.GetAttributeOfType<BlipTypeAttribute>()?.BlipId ?? defaultBlipId;
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
        public void Door(Client player)
        {
            PropertyDoor(player);
        }
    }
}
