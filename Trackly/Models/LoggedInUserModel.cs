namespace Trackly.Models
{
    public class LoggedInUserModel
    {
        public int Id { get; set; }
        public UserModel? User { get; set; }
        public string? Token { get; set; }

        public void Clear()
        {
            Id = 0;
            User = null;
            Token = null;
        }
    }
}
