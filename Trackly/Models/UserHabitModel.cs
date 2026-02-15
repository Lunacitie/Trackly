using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trackly.Models
{
    public class UserHabitModel
    {
        [Key] public int Id { get; set; }
        [ForeignKey(nameof(User))] public int UserId { get; set; }
        public UserModel User { get; set; } = null!;
        [ForeignKey(nameof(Habit))] public int HabitId { get; set; }
        public HabitModel Habit { get; set; } = null!;
        [Required, Range(0, 31)] public int Goal { get; set; }

        public bool IsActive { get; set; } = true;
        public ICollection<HabitEntryModel> Entries { get; set; } = new List<HabitEntryModel>();
    }
}
