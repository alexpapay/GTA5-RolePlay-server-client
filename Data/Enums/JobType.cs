using TheGodfatherGM.Data.Attributes;
using System.ComponentModel.DataAnnotations;

namespace TheGodfatherGM.Data.Enums
{
    public enum JobType
    {
        [Display(Name = "Gas Station")]
        [BlipType(361)]
        GasStation = 1,

        [Display(Name = "Lawyer")]
        [BlipType(408)]
        Lawyer = 2,

        [Display(Name = "Mechanic")]
        [BlipType(446)]
        Mechanic = 3,

        [Display(Name = "Bodyguard")]
        [BlipType(461)]
        Bodyguard = 4,

        [Display(Name = "Taxi Driver")]
        [BlipType(198)]
        TaxiDriver = 5,

        [Display(Name = "Trucker")]
        [BlipType(85)]
        Trucker = 6,

        [Display(Name = "Грузчик")]
        [BlipType(311)]
        Loader = 7,

        [Display(Name = "Водитель автобуса")]
        [BlipType(513)]
        BusDriver = 8
    }
}
