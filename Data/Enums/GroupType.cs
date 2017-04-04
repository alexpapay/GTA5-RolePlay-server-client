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

        [Display(Name = "Военные")]
        [BlipType(60)]
        NationalGuard = 2,

        [Display(Name = "Медики")]
        [BlipType(61)]
        MedicalDepartment = 3,

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

        [Display(Name = "ФБР")]
        [BlipType(60)]
        FBI = 10,

        [Display(Name = "Правительство")]
        [BlipType(181)]
        Goverment = 11,

        [Display(Name = "Автошкола")]
        [BlipType(315)]
        AutoSchool = 12,

        [Display(Name = "Балласы")]
        [BlipType(106)]
        Ballas = 13,

        [Display(Name = "Вагосы")]
        [BlipType(315)]
        Vagos = 14,

        [Display(Name = "La Costa Notsa")]
        [BlipType(315)]
        LaCostaNotsa = 15,

        [Display(Name = "Русская мафия")]
        [BlipType(315)]
        RussianMafia = 16,

        [Display(Name = "Грув")]
        [BlipType(315)]
        Groove = 17,

        [Display(Name = "Зона 51")]
        [BlipType(315)]
        Zone51 = 18,

        [Display(Name = "LVPD")]
        [BlipType(315)]
        LVPD = 19
    }
}
