using System.ComponentModel.DataAnnotations;

namespace Trackly.Models
{
    public class UserModel
    {
        [Key] public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public ICollection<HabitModel> Habits { get; set; } = new List<HabitModel>();
        public ICollection<NoteModel> Notes { get; set; } = new List<NoteModel>();
    }
}
