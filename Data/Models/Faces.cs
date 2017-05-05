using System.ComponentModel.DataAnnotations;

namespace TheGodfatherGM.Data
{
    public class Faces
    {
        [Key]
        public int Id { get; set; }

        public int CharacterId { get; set; }

        public int SEX { get; set; }
        public int GTAO_SHAPE_FIRST_ID { get; set; }
        public int GTAO_SHAPE_SECOND_ID { get; set; }
        public int GTAO_SKIN_FIRST_ID { get; set; }
        public int GTAO_HAIR { get; set; }
        public int GTAO_HAIR_COLOR { get; set; }
        public int GTAO_EYE_COLOR { get; set; }
        public int GTAO_EYEBROWS { get; set; }
        public int GTAO_EYEBROWS_COLOR { get; set; }
    }
}
