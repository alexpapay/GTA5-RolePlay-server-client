using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheGodfatherGM.Data
{
    public class GroupMember
    {
        [Key]
        public int Id { get; set; }
        
        public int? CharacterId { get; set; }

        [ForeignKey("CharacterId")]
        public Character Character { get; set; }

        public int GroupId { get; set; }

        [ForeignKey("GroupId")]
        public Group Group { get; set; }

        public bool Leader { get; set; }
    }
}