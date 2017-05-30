using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel;
using TheGodfatherGM.Data.Enums;

namespace TheGodfatherGM.Data
{
    public class Job
    {
        [Key]
        public int Id { get; set; }
        public JobType Type { get; set; }
        
        public int CharacterId { get; set; }
        public string OwnerName { get; set; }

        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }

        public int Level { get; set; }
        public int Money { get; set; }
        public int Cost { get; set; }

        public int MafiaRoofId { get; set; }
        public int MafiaRoofMoney { get; set; }

        public ICollection<Vehicle> Vehicles { get; set; }
    }
}