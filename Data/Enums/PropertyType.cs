using TheGodfatherGM.Data.Attributes;
using System.ComponentModel.DataAnnotations;

namespace TheGodfatherGM.Data.Enums
{
    public enum PropertyType
    {
        [Display(Name = "Invalid")]
        Invalid = 0,

        [BlipType(188)]
        [Display(Name = "Полиция")]
        Police = 1,

        [Display(Name = "Дверь")]
        Door = 2,

        [BlipType(40)]
        [Display(Name = "Строение")]
        Building = 3,

        [BlipType(512)]
        [Display(Name = "Прокат")]
        Rent = 4,

        [BlipType(277)]
        [Display(Name = "Работа грузчиком")]
        WorkLoader = 5,

        [BlipType(76)]
        [Display(Name = "Автошкола")]
        Autoschool = 6,

        [BlipType(181)]
        [Display(Name = "Мэрия")]
        Meria = 11,

        [BlipType(106)]
        [Display(Name = "Балласы")]
        GangBallas = 13,

        [BlipType(76)]
        [Display(Name = "Azcas")]
        GangAzcas = 14,

        [BlipType(120)]
        [Display(Name = "Vagos")]
        GangVagos = 15,

        [BlipType(77)]
        [Display(Name = "Grove")]
        GangGrove = 16,

        [BlipType(88)]
        [Display(Name = "Rifa")]
        GangRifa = 17,

        [BlipType(60)]
        [Display(Name = "Военные 1")]
        ArmyOne = 20,

        [BlipType(60)]
        [Display(Name = "Военные 2")]
        ArmyTwo = 21,

        [BlipType(478)]
        [Display(Name = "Прием металла")]
        Gangs = 99,

        [BlipType(40)]
        [Display(Name = "Жилой дом")]
        House = 100
    }
}
