using System;

namespace TheGodfatherGM.Data
{
    public class Caption
    {
        public int Id { get; set; }

        public DateTime Time { get; set; }
        public string Sector { get; set; }
        public int GangAttack { get; set; }
        public int FragsAttack { get; set; }
        public int GangDefend { get; set; }
        public int FragsDefend { get; set; }
    }
}
