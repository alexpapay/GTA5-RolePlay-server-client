using System.ComponentModel.DataAnnotations;
using TheGodfatherGM.Data.Attributes;

namespace TheGodfatherGM.Data.Enums
{
    public enum GroupExtraType
    {
        [Display(Name = "Вновь зарегистрированный")]
        [BlipType(496)]
        HomelessFirst = 0,

        [Display(Name = "Car Dealership")]
        [BlipType(225)]
        CarDealership = 1,

        [Display(Name = "Food Market")]
        [BlipType(11)]
        FoodMarket = 2,

        [Display(Name = "Gun Store")]
        [BlipType(110)]
        GunStore = 3,

        [Display(Name = "Gas Station")]
        [BlipType(361)]
        GasStation = 4,

        // 11 - Goverment
        [Display(Name = "Правительство: Секретарь")]
        [BlipType(181)]
        Secretary = 1101,

        [Display(Name = "Правительство: Адвокат")]
        [BlipType(181)]
        Lawyer = 1102,

        [Display(Name = "Правительство: Охранник")]
        [BlipType(181)]
        Security = 1103,

        [Display(Name = "Правительство: Начальник охраны")]
        [BlipType(181)]
        HeadOfSecurity = 1104,

        [Display(Name = "Правительство: Заместитель мэра")]
        [BlipType(181)]
        DeputyMayor = 1105,

        [Display(Name = "Правительство: Мэр")]
        [BlipType(181)]
        Mayor = 1106,

        // 12 - Auto School
        [Display(Name = "Автошкола: Стажер")]
        [BlipType(181)]
        Trainee = 1201,

        [Display(Name = "Автошкола: Консультант")]
        [BlipType(181)]
        Consultant = 1202,

        [Display(Name = "Автошкола: Экзаменатор")]
        [BlipType(181)]
        Examiner = 1203,

        [Display(Name = "Автошкола: Младший инструктор")]
        [BlipType(181)]
        JuniorInstructor = 1204,

        [Display(Name = "Автошкола: Инструктор")]
        [BlipType(181)]
        Instructor = 1205,

        [Display(Name = "Автошкола: Координатор")]
        [BlipType(181)]
        Coordinator = 1206,

        [Display(Name = "Автошкола: Младший Менеджер")]
        [BlipType(181)]
        JuniorManager = 1207,

        [Display(Name = "Автошкола: Старший  Менеджер")]
        [BlipType(181)]
        SeniorManager = 1208,

        [Display(Name = "Автошкола: Директор")]
        [BlipType(181)]
        Director = 1209,

        [Display(Name = "Автошкола: Управляющий")]
        [BlipType(181)]
        Manager = 1210,
    }
}
