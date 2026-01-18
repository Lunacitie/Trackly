using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trackly.Models
{
    public class HabitModel
    {
        [Key]public int Id { get; set; }
        [Required]public string Name { get; set; }
        [Required, Range(0,31)]public int Goal { get; set; }
        [ForeignKey(nameof(User))]public int UserId { get; set; }
        public UserModel User { get; set; } = null!;
        public ICollection<HabitEntryModel> Habits { get; set; } = new List<HabitEntryModel>();
    }
}
