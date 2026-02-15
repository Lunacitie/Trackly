using System.ComponentModel.DataAnnotations;

namespace Trackly.Models
{
    public class UserModel
    {
        [Key] public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public ICollection<UserHabitModel> UserHabits { get; set; } = new List<UserHabitModel>();
        public ICollection<NoteModel> Notes { get; set; } = new List<NoteModel>();
    }
}
