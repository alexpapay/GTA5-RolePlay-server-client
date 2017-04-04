using TheGodfatherGM.Data.Attributes;
using System.ComponentModel.DataAnnotations;

namespace TheGodfatherGM.Data.Enums
{
    public enum PropertyType
    {
        [Display(Name = "Invalid")]
        Invalid = 0,

        [BlipType(40)]
        [Display(Name = "Жилой дом")]
        House = 1,

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
        WorkLoader = 5
    }
}
