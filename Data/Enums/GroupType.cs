using TheGodfatherGM.Data.Attributes;
using System.ComponentModel.DataAnnotations;

namespace TheGodfatherGM.Data.Enums
{
    public enum GroupType
    {
        [Display(Name = "Бомжи")]
        [BlipType(496)]
        Homeless = 0,

        [Display(Name = "Полиция")]
        [BlipType(188)]
        PoliceDepartment = 1,

        [Display(Name = "МЧС")]
        [BlipType(188)]
        Emergency = 2,

        [Display(Name = "ФБР")]
        [BlipType(61)]
        FBI = 3,

        [Display(Name = "Азтэк")]
        CorrectionalFacility = 4,

        [Display(Name = "Авианосец")]
        HitmanAgency = 5,

        [Display(Name = "Таксисты")]
        [BlipType(198)]
        TaxiCabCompany = 6,

        [Display(Name = "Новости")]
        [BlipType(50)]
        NewsNetwork = 7,

        [Display(Name = "Якутза")]
        [BlipType(1)]
        CriminalOrganization = 8,

        [Display(Name = "Бизнес")]
        [BlipType(50)]
        Business = 9,

        [Display(Name = "")]
        [BlipType(60)]
        NULL = 10,

        [Display(Name = "Правительство")]
        [BlipType(181)]
        Goverment = 11,

        [Display(Name = "Автошкола")]
        [BlipType(315)]
        AutoSchool = 12,

        [Display(Name = "The Ballas Gang")]
        [BlipType(106)]
        Ballas = 13,

        [Display(Name = "Los Varios Azcas Gang")]
        [BlipType(315)]
        Vagos = 14,

        [Display(Name = "Los Santos Vagos Gang")]
        [BlipType(315)]
        LaCostaNotsa = 15,

        [Display(Name = "Grove Street Gang")]
        [BlipType(315)]
        GroveStreet = 16,

        [Display(Name = "The Rifa Gang")]
        [BlipType(315)]
        TheRifa = 17,

        [Display(Name = "Зона 51")]
        [BlipType(315)]
        Zone51 = 18,

        [Display(Name = "LVPD")]
        [BlipType(315)]
        LVPD = 19,

        [Display(Name = "Военные 1")]
        [BlipType(60)]
        Army1 = 20,

        [Display(Name = "Военные 2")]
        [BlipType(60)]
        Army2 = 21,

        [Display(Name = "Русская Мафия")]
        [BlipType(60)]
        MafiaRussian = 30,

        [Display(Name = "Безработный")]
        [BlipType(315)]
        Unemployed = 100
    }
}
