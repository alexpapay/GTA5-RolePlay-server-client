using System.ComponentModel.DataAnnotations;

namespace TheGodfatherGM.Data
{
    public class Vehicle
    {
        [Key]
        public int Id { get; set; }

        public int Model { get; set; }
        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }
        public float Rot { get; set; }
        public int Color1 { get; set; }
        public int Color2 { get; set; }
        public bool Respawnable { get; set; }

        public int? CharacterId { get; set; }
        public virtual Character Character { get; set; }
        public int? GroupId { get; set; }
        public virtual Group Group { get; set; }
        public int? JobId { get; set; }
        public virtual Job Job { get; set; }
        public int RentTime { get; set; }
        public int Fuel { get; set; }
        public int Type { get; set; }
        // 0 - обычная,  1 - прокатная,
        public int Material { get; set; }

        public Vehicle()
        {
        }
    }
}
