using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trackly.Models
{
    public class HabitModel
    {
        [Key]public int Id { get; set; }
        [Required]public string Name { get; set; }
        [Range(0,31)]public int? DefaultGoal { get; set; }
        public bool IsSeed { get; set; }
    }
}
