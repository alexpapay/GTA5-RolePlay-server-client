using System.ComponentModel.DataAnnotations;
using TheGodfatherGM.Data.Attributes;

namespace TheGodfatherGM.Data.Enums
{
    public enum GroupExtraType
    {
        [Display(Name = "Мигрант")]
        [BlipType(496)]
        HomelessFirst = 0,

        [Display(Name = "Безработный")]
        [BlipType(496)]
        UnemployedFirst = 1,

        [Display(Name = "Car Dealership")]
        [BlipType(225)]
        CarDealership = 10,

        [Display(Name = "Food Market")]
        [BlipType(11)]
        FoodMarket = 20,

        [Display(Name = "Gun Store")]
        [BlipType(110)]
        GunStore = 30,

        [Display(Name = "Gas Station")]
        [BlipType(361)]
        GasStation = 40,

        #region Police 1
        [Display(Name = "Полиция : База")]
        [BlipType(188)]
        PoliceBase = 100,

        [Display(Name = "Кадет")]
        [BlipType(188)]
        PoliceKadet = 101,

        [Display(Name = "Офицер")]
        [BlipType(188)]
        PoliceOfficer = 102,

        [Display(Name = "Мл. Сержант")]
        [BlipType(188)]
        PoliceSergJunior = 103,

        [Display(Name = "Сержант")]
        [BlipType(188)]
        PoliceSerguant = 104,

        [Display(Name = "Прапорщик")]
        [BlipType(188)]
        PolicePrapor = 105,

        [Display(Name = "Старший Прапорщик")]
        [BlipType(188)]
        PoliceSeniorPrapor = 106,

        [Display(Name = "Младший Лейтенант")]
        [BlipType(188)]
        PoliceJunLeytenant = 107,

        [Display(Name = "Лейтенант")]
        [BlipType(188)]
        PoliceLeytenant = 108,

        [Display(Name = "Старший Лейтенант")]
        [BlipType(188)]
        PoliceSenLeytenant = 109,

        [Display(Name = "Капитан")]
        [BlipType(188)]
        PoliceCaptain = 110,

        [Display(Name = "Майор")]
        [BlipType(188)]
        PoliceMajor = 111,

        [Display(Name = "Подполковник")]
        [BlipType(188)]
        PoliceJunPolkovnik = 112,

        [Display(Name = "Полковник")]
        [BlipType(188)]
        PolicePolkivnik = 113,

        [Display(Name = "Шериф")]
        [BlipType(188)]
        PoliceSheriff = 114,

        #endregion
        #region Emergency 2
        [Display(Name = "МЧС : База")]
        [BlipType(188)]
        EmergencyBase = 200,

        [Display(Name = "Интерн")]
        [BlipType(188)]
        EmergencyIntern = 201,

        [Display(Name = "Санитар")]
        [BlipType(188)]
        EmergencySanitar = 202,

        [Display(Name = "Мед. Брат")]
        [BlipType(188)]
        EmergencyMedBrother = 203,

        [Display(Name = "Спасатель")]
        [BlipType(188)]
        EmergencyRescue = 204,

        [Display(Name = "Нарколог")]
        [BlipType(188)]
        EmergencyNarco = 205,

        [Display(Name = "Доктор")]
        [BlipType(188)]
        EmergencyDoctor = 206,

        [Display(Name = "Психолог")]
        [BlipType(188)]
        EmergencyPsyho = 207,

        [Display(Name = "Хирург")]
        [BlipType(188)]
        EmergencyHirurg = 208,

        [Display(Name = "Зам. Главного Врача")]
        [BlipType(188)]
        EmergencyZamBoss = 209,

        [Display(Name = "Главный Врач")]
        [BlipType(188)]
        EmergencyBoss = 210,

        #endregion
        #region FBI 3
        [Display(Name = "ФБР : База")]
        [BlipType(188)]
        FBIBase = 300,

        [Display(Name = "Стажер")]
        [BlipType(188)]
        FBIStajor = 301,

        [Display(Name = "Дежурный")]
        [BlipType(188)]
        FBIDejurniy = 302,

        [Display(Name = "Младший Агент")]
        [BlipType(188)]
        FBIJunAgent = 303,

        [Display(Name = "Агент GNK")]
        [BlipType(188)]
        FBIAgentOfGNK = 304,

        [Display(Name = "Агент СID")]
        [BlipType(188)]
        FBIAgentOfCID = 305,

        [Display(Name = "Глава GNK")]
        [BlipType(188)]
        FBIBossOfGNK = 306,

        [Display(Name = "Глава CID")]
        [BlipType(188)]
        FBIBossOfCID = 307,

        [Display(Name = "Инспектор")]
        [BlipType(188)]
        FBIInspector = 308,

        [Display(Name = "Зам. Директора")]
        [BlipType(188)]
        FBIZamBoss = 309,

        [Display(Name = "Директор")]
        [BlipType(188)]
        FBIBoss = 310,

        #endregion
        #region Goverment 11
        [Display(Name = "Правительство:База")]
        [BlipType(181)]
        GovermentBase = 1100,

        [Display(Name = "Секретарь")]
        [BlipType(181)]
        Secretary = 1101,

        [Display(Name = "Адвокат")]
        [BlipType(181)]
        Lawyer = 1102,

        [Display(Name = "Охранник")]
        [BlipType(181)]
        Security = 1103,

        [Display(Name = "Начальник охраны")]
        [BlipType(181)]
        HeadOfSecurity = 1104,

        [Display(Name = "Заместитель мэра")]
        [BlipType(181)]
        DeputyMayor = 1105,

        [Display(Name = "Мэр")]
        [BlipType(181)]
        Mayor = 1106,
        #endregion 

        #region AutoSchool 12

        [Display(Name = "Автошкола:База")]
        [BlipType(181)]
        AutoSchoolBase = 1200,

        [Display(Name = "Стажер")]
        [BlipType(181)]
        Trainee = 1201,

        [Display(Name = "Консультант")]
        [BlipType(181)]
        Consultant = 1202,

        [Display(Name = "Экзаменатор")]
        [BlipType(181)]
        Examiner = 1203,

        [Display(Name = "Младший инструктор")]
        [BlipType(181)]
        JuniorInstructor = 1204,

        [Display(Name = "Инструктор")]
        [BlipType(181)]
        Instructor = 1205,

        [Display(Name = "Координатор")]
        [BlipType(181)]
        Coordinator = 1206,

        [Display(Name = "Младший Менеджер")]
        [BlipType(181)]
        JuniorManager = 1207,

        [Display(Name = "Старший  Менеджер")]
        [BlipType(181)]
        SeniorManager = 1208,

        [Display(Name = "Директор")]
        [BlipType(181)]
        Director = 1209,

        [Display(Name = "Управляющий")]
        [BlipType(181)]
        Manager = 1210,

        #endregion


        #region The Ballas Gang : 13
        [Display(Name = "Балласы:База")]
        [BlipType(106)]
        BallasBase = 1300,

        [Display(Name = "Baby")]
        [BlipType(106)]
        BallasBaby = 1301,

        [Display(Name = "Bastard")]
        [BlipType(106)]
        BallasBastard = 1302,

        [Display(Name = "Burglar")]
        [BlipType(106)]
        BallasBurglar = 1303,

        [Display(Name = "Little Nigga")]
        [BlipType(106)]
        BallasLittleNigga = 1304,

        [Display(Name = "Skilled")]
        [BlipType(106)]
        BallasSkilled = 1305,

        [Display(Name = "Gangstar")]
        [BlipType(106)]
        BallasGangstar = 1306,

        [Display(Name = "Destroyer")]
        [BlipType(106)]
        BallasDestroyer = 1307,

        [Display(Name = "Shooter")]
        [BlipType(106)]
        BallasShooter = 1308,

        [Display(Name = "Star")]
        [BlipType(106)]
        BallasStar = 1309,

        [Display(Name = "Big Daddy")]
        [BlipType(106)]
        BallasBigDaddy = 1310,

        #endregion
        #region Los Varios Azcas Gang : 14
        [Display(Name = "Балласы:База")]
        [BlipType(106)]
        LosVariosAzcasBase = 1400,

        [Display(Name = "Perro")]
        [BlipType(106)]
        LosVariosAzcasPerro = 1401,

        [Display(Name = "Amistoso")]
        [BlipType(106)]
        LosVariosAzcasAmistoso = 1402,

        [Display(Name = "Tirador")]
        [BlipType(106)]
        LosVariosAzcasTirador = 1403,

        [Display(Name = "Sabio")]
        [BlipType(106)]
        LosVariosAzcasSabio = 1404,

        [Display(Name = "Mirando")]
        [BlipType(106)]
        LosVariosAzcasMirando = 1405,

        [Display(Name = "Mejor")]
        [BlipType(106)]
        LosVariosAzcasMejor = 1406,

        [Display(Name = "Tesorero")]
        [BlipType(106)]
        LosVariosAzcasTesorero = 1407,

        [Display(Name = "Dedicado")]
        [BlipType(106)]
        LosVariosAzcasDedicado = 1408,

        [Display(Name = "Diputado")]
        [BlipType(106)]
        LosVariosAzcasDiputado = 1409,

        [Display(Name = "El Padre Supremo")]
        [BlipType(106)]
        LosVariosAzcasElPadreSupremo = 1410,

        #endregion 
        #region Los Santos Vagos Gang : 15
        [Display(Name = "Los Santos Vagos Gang:База")]
        [BlipType(106)]
        LosSantosVagosGangBase = 1500,

        [Display(Name = "Novato")]
        [BlipType(106)]
        LosSantosVagosGangNovato = 1501,

        [Display(Name = "Local")]
        [BlipType(106)]
        LosSantosVagosGangLocal = 1502,

        [Display(Name = "Verificado")]
        [BlipType(106)]
        LosSantosVagosGangVerificado = 1503,

        [Display(Name = "Buscando")]
        [BlipType(106)]
        LosSantosVagosGangBuscando = 1504,

        [Display(Name = "Autoridad")]
        [BlipType(106)]
        LosSantosVagosGangAutoridad = 1505,

        [Display(Name = "Guardian")]
        [BlipType(106)]
        LosSantosVagosGangGuardian = 1506,

        [Display(Name = "Asessino")]
        [BlipType(106)]
        LosSantosVagosGangAsessino = 1507,

        [Display(Name = "Elite")]
        [BlipType(106)]
        LosSantosVagosGangElite = 1508,

        [Display(Name = "La Leyenda")]
        [BlipType(106)]
        LosSantosVagosGangLaLeyenda = 1509,

        [Display(Name = "El Padre")]
        [BlipType(106)]
        LosSantosVagosGangElPadre = 1510,

        #endregion 
        #region Grove Street Gang : 16
        [Display(Name = "Grove Street Gang:База")]
        [BlipType(106)]
        GroveStreetGangBase = 1600,

        [Display(Name = "Freak")]
        [BlipType(106)]
        GroveStreetGangFreak = 1601,

        [Display(Name = "Dweller")]
        [BlipType(106)]
        GroveStreetGangDweller = 1602,

        [Display(Name = "Huckster")]
        [BlipType(106)]
        GroveStreetGangHuckster = 1603,

        [Display(Name = "Crazy")]
        [BlipType(106)]
        GroveStreetGangCrazy = 1604,

        [Display(Name = "True")]
        [BlipType(106)]
        GroveStreetGangTrue = 1605,

        [Display(Name = "O.G")]
        [BlipType(106)]
        GroveStreetGangOG = 1606,

        [Display(Name = "Goon")]
        [BlipType(106)]
        GroveStreetGangGoon = 1607,

        [Display(Name = "Big Bro")]
        [BlipType(106)]
        GroveStreetGangBigBro = 1608,

        [Display(Name = "Legend")]
        [BlipType(106)]
        GroveStreetGangLegend = 1609,

        [Display(Name = "Daddy")]
        [BlipType(106)]
        GroveStreetGangDaddy = 1610,

        #endregion 
        #region The Rifa Gang : 17
        [Display(Name = "The Rifa Gang:База")]
        [BlipType(106)]
        TheRifaGangBase = 1700,

        [Display(Name = "Ладрон")]
        [BlipType(106)]
        TheRifaGangLadron = 1701,

        [Display(Name = "Новато")]
        [BlipType(106)]
        TheRifaGangNovato = 1702,

        [Display(Name = "Амиго")]
        [BlipType(106)]
        TheRifaGangAmigo = 1703,

        [Display(Name = "Мачо")]
        [BlipType(106)]
        TheRifaGangMacho = 1704,

        [Display(Name = "Джуниор")]
        [BlipType(106)]
        TheRifaGangJunior = 1705,

        [Display(Name = "Эрмано")]
        [BlipType(106)]
        TheRifaGangErmano = 1706,

        [Display(Name = "Бандидо")]
        [BlipType(106)]
        TheRifaGangBandido = 1707,

        [Display(Name = "Ауторидад")]
        [BlipType(106)]
        TheRifaGangAutoridad = 1708,

        [Display(Name = "Аджунто")]
        [BlipType(106)]
        TheRifaGangAdjunto = 1709,

        [Display(Name = "Падре")]
        [BlipType(106)]
        TheRifaGangPadre = 1710,

        #endregion 

        #region National Guard 1        
        [Display(Name = "Военные:База")]
        [BlipType(60)]
        NationalGuardBase = 2000,

        [Display(Name = "Рядовой")]
        [BlipType(60)]
        Soldier = 2001,

        [Display(Name = "Ефрейтор")]
        [BlipType(60)]
        LanceCorporal = 2002,

        [Display(Name = "Младший Сержант")]
        [BlipType(60)]
        LanceSergeant = 2003,

        [Display(Name = "Сержант")]
        [BlipType(60)]
        Sergeant = 2004,

        [Display(Name = "Старший Сержант")]
        [BlipType(60)]
        StaffSergeant = 2005,

        [Display(Name = "Старшина")]
        [BlipType(60)]
        PettyOfficer = 2006,

        [Display(Name = "Прапорщик")]
        [BlipType(60)]
        Ensign = 2007,

        [Display(Name = "Младший Лейтенант")]
        [BlipType(60)]
        LieutenantJun = 2008,

        [Display(Name = "Лейтенант")]
        [BlipType(60)]
        Lieutenant = 2009,

        [Display(Name = "Старший Лейтенант")]
        [BlipType(60)]
        SeniorLieutenant = 2010,

        [Display(Name = "Капитан")]
        [BlipType(60)]
        Captain = 2011,

        [Display(Name = "Майор")]
        [BlipType(60)]
        Major = 2012,

        [Display(Name = "Подполковник")]
        [BlipType(60)]
        LieutenantColonel = 2013,

        [Display(Name = "Полковник")]
        [BlipType(60)]
        Colonel = 2014,

        [Display(Name = "Генерал")]
        [BlipType(60)]
        General = 2015,
        #endregion
        #region National Guard 2        
        [Display(Name = "Военные:База")]
        [BlipType(60)]
        NationalGuardBaseTwo = 2100,

        [Display(Name = "Рядовой")]
        [BlipType(60)]
        SoldierTwo = 2101,

        [Display(Name = "Ефрейтор")]
        [BlipType(60)]
        LanceCorporalTwo = 2102,

        [Display(Name = "Младший Сержант")]
        [BlipType(60)]
        LanceSergeantTwo = 2103,

        [Display(Name = "Сержант")]
        [BlipType(60)]
        SergeantTwo = 2104,

        [Display(Name = "Старший Сержант")]
        [BlipType(60)]
        StaffSergeantTwo = 2105,

        [Display(Name = "Старшина")]
        [BlipType(60)]
        PettyOfficerTwo = 2106,

        [Display(Name = "Прапорщик")]
        [BlipType(60)]
        EnsignTwo = 2107,

        [Display(Name = "Младший Лейтенант")]
        [BlipType(60)]
        LieutenantJunTwo = 2108,

        [Display(Name = "Лейтенант")]
        [BlipType(60)]
        LieutenantTwo = 2109,

        [Display(Name = "Старший Лейтенант")]
        [BlipType(60)]
        SeniorLieutenantTwo = 2110,

        [Display(Name = "Капитан")]
        [BlipType(60)]
        CaptainTwo = 2111,

        [Display(Name = "Майор")]
        [BlipType(60)]
        MajorTwo = 2112,

        [Display(Name = "Подполковник")]
        [BlipType(60)]
        LieutenantColonelTwo = 2113,

        [Display(Name = "Полковник")]
        [BlipType(60)]
        ColonelTwo = 2114,

        [Display(Name = "Генерал")]
        [BlipType(60)]
        GeneralTwo = 2115,
        #endregion

        #region Mafia Russian 30        
        [Display(Name = "Русская Мафия:База")]
        [BlipType(60)]
        MafiaRussianBase = 3000,

        [Display(Name = "1 Ранг")]
        [BlipType(60)]
        MafiaRussian1 = 3001,

        [Display(Name = "2 Ранг")]
        [BlipType(60)]
        MafiaRussian2 = 3002,

        [Display(Name = "3 Ранг")]
        [BlipType(60)]
        MafiaRussian3 = 3003,

        [Display(Name = "4 Ранг")]
        [BlipType(60)]
        MafiaRussian4 = 3004,

        [Display(Name = "5 Ранг")]
        [BlipType(60)]
        MafiaRussian5 = 3005,

        [Display(Name = "6 Ранг")]
        [BlipType(60)]
        MafiaRussian6 = 3006,

        [Display(Name = "7 Ранг")]
        [BlipType(60)]
        MafiaRussian7 = 3007,

        [Display(Name = "8 Ранг")]
        [BlipType(60)]
        MafiaRussian8 = 3008,

        [Display(Name = "9 Ранг")]
        [BlipType(60)]
        MafiaRussian9 = 3009,

        [Display(Name = "10 Ранг")]
        [BlipType(60)]
        MafiaRussian10 = 3010,
        #endregion
        #region Mafia LKN 31        
        [Display(Name = "LKN Мафия:База")]
        [BlipType(60)]
        MafiaLKNBase = 3100,

        [Display(Name = "1 Ранг")]
        [BlipType(60)]
        MafiaLKN1 = 3101,

        [Display(Name = "2 Ранг")]
        [BlipType(60)]
        MafiaLKN2 = 3102,

        [Display(Name = "3 Ранг")]
        [BlipType(60)]
        MafiaLKN3 = 3103,

        [Display(Name = "4 Ранг")]
        [BlipType(60)]
        MafiaLKN4 = 3104,

        [Display(Name = "5 Ранг")]
        [BlipType(60)]
        MafiaLKN5 = 3105,

        [Display(Name = "6 Ранг")]
        [BlipType(60)]
        MafiaLKN6 = 3106,

        [Display(Name = "7 Ранг")]
        [BlipType(60)]
        MafiaLKN7 = 3107,

        [Display(Name = "8 Ранг")]
        [BlipType(60)]
        MafiaLKN8 = 3108,

        [Display(Name = "9 Ранг")]
        [BlipType(60)]
        MafiaLKN9 = 3109,

        [Display(Name = "10 Ранг")]
        [BlipType(60)]
        MafiaLKN10 = 3110,
        #endregion
        #region Mafia Armeny 32        
        [Display(Name = "LKN Мафия:База")]
        [BlipType(60)]
        MafiaArmenyBase = 3200,

        [Display(Name = "1 Ранг")]
        [BlipType(60)]
        MafiaArmeny1 = 3201,

        [Display(Name = "2 Ранг")]
        [BlipType(60)]
        MafiaArmeny2 = 3202,

        [Display(Name = "3 Ранг")]
        [BlipType(60)]
        MafiaArmeny3 = 3203,

        [Display(Name = "4 Ранг")]
        [BlipType(60)]
        MafiaArmeny4 = 3204,

        [Display(Name = "5 Ранг")]
        [BlipType(60)]
        MafiaArmeny5 = 3205,

        [Display(Name = "6 Ранг")]
        [BlipType(60)]
        MafiaArmeny6 = 3206,

        [Display(Name = "7 Ранг")]
        [BlipType(60)]
        MafiaArmeny7 = 3207,

        [Display(Name = "8 Ранг")]
        [BlipType(60)]
        MafiaArmeny8 = 3208,

        [Display(Name = "9 Ранг")]
        [BlipType(60)]
        MafiaArmeny9 = 3209,

        [Display(Name = "10 Ранг")]
        [BlipType(60)]
        MafiaArmeny10 = 3210
        #endregion
    }
}
