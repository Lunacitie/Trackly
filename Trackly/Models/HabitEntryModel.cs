using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trackly.Models
{
    public class HabitEntryModel
    {
        [Key] public int Id { get; set; }
        [ForeignKey(nameof(Habit))]public int HabitId { get; set; }
        public HabitModel Habit { get; set; } = null!;
        [Required] public DateOnly Date { get; set; }
        public bool IsDone { get; set; }
    }
}
