using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trackly.Models
{
    public class NoteModel
    {
        [Key] public int Id { get; set; }
        [ForeignKey(nameof(User))]public int UserId { get; set; }
        public UserModel User { get; set; } = null!;
        [Required] public DateOnly Date {  get; set; }
        [Required] public string Text { get; set; } = "";
    }
}
