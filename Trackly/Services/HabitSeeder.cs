using System.Text.Json;
using Trackly.Models;

namespace Trackly.Services
{
    public class HabitSeeder
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public HabitSeeder(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        private record HabitSeedItem(string Name, int? DefaultGoal, bool IsSeed);

        public void SeedHabitsFromJson()
        {
            var path = Path.Combine(_env.ContentRootPath, "Data", "seed-habits.json");
            if (!File.Exists(path)) return;

            var json = File.ReadAllText(path);

            var items = JsonSerializer.Deserialize<List<HabitSeedItem>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<HabitSeedItem>();

            foreach (var item in items)
            {
                var habitName = (item.Name ?? "").Trim();
                if (habitName.Length == 0) continue;

                var exists = _db.Habits.Any(h => h.Name.ToLower() == habitName.ToLower());
                if (exists) continue;

                _db.Habits.Add(new HabitModel
                {
                    Name = habitName,
                    DefaultGoal = item.DefaultGoal,
                    IsSeed = true
                });
            }

            _db.SaveChanges();
        }
    }
}
