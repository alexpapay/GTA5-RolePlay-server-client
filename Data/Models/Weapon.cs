using System.ComponentModel.DataAnnotations;

namespace TheGodfatherGM.Data
{
    public class Weapon
    {
        [Key]
        public int Id { get; set; }

        public int CharacterId { get; set; }

        public int Revolver { get; set; }
        public int RevolverPt { get; set; }
        public int HeavyPistol { get; set; }
        public int HeavyPistolPt { get; set; }

        public int CarbineRifle { get; set; }
        public int CarbineRiflePt { get; set; }
        public int CompactRifle { get; set; }
        public int CompactRiflePt { get; set; }
        public int BullpupRifle { get; set; }
        public int BullpupRiflePt { get; set; }

        public int PumpShotgun { get; set; }
        public int PumpShotgunPt { get; set; }
        public int HeavyShotgun { get; set; }
        public int HeavyShotgunPt { get; set; }

        public int SniperRifle { get; set; }
        public int SniperRiflePt { get; set; }

        public int SmokeGrenade { get; set; }
        public int SmokeGrenadePt { get; set; } 
        public int BZGas { get; set; }
        public int BZGasPt { get; set; }
        public int StunGun { get; set; }
        public int StunGunPt { get; set; }
        public int FlareGun { get; set; }
        public int FlareGunPt { get; set; }        

        public int Nightstick { get; set; }
    }
}
