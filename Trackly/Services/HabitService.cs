using Microsoft.EntityFrameworkCore;
using Trackly.Models;

namespace Trackly.Services
{
    public class HabitService
    {
        private readonly AppDbContext _context;

        public HabitService(AppDbContext context)
        {
            _context = context;
        }

        public List<UserHabitModel> GetHabits(int userId)
        {
            return _context.UserHabits
                .AsNoTracking()
                .Include(uh => uh.Habit)
                .Where(uh => uh.UserId == userId && uh.IsActive)
                .OrderBy(uh => uh.Habit.Name)
                .ToList();
        }

        public UserHabitModel? GetHabit(int userId, int userHabitId)
        {
            return _context.UserHabits
                .AsNoTracking()
                .Include(uh => uh.Habit)
                .FirstOrDefault(uh => uh.Id == userHabitId && uh.UserId == userId);
        }

        public UserHabitModel CreateHabit(int userId, string habitName, int goal)
        {
            habitName = (habitName ?? "").Trim();
            if (habitName.Length == 0)
                throw new ArgumentException("Habit name is required.");

            var normalized = habitName.ToLower();

            var habit = _context.Habits
                .FirstOrDefault(h => h.IsSeed && h.Name.ToLower() == normalized);

            if (habit == null)
            {
                habit = new HabitModel
                {
                    Name = habitName,
                    DefaultGoal = null,
                    IsSeed = false
                };

                _context.Habits.Add(habit);
                _context.SaveChanges();
            }

            var existingUserHabit = _context.UserHabits
                .FirstOrDefault(uh =>
                    uh.UserId == userId &&
                    uh.HabitId == habit.Id);

            if (existingUserHabit != null)
            {
                existingUserHabit.Goal = goal;
                existingUserHabit.IsActive = true;
                _context.SaveChanges();

                _context.Entry(existingUserHabit)
                    .Reference(x => x.Habit)
                    .Load();

                return existingUserHabit;
            }

            var userHabit = new UserHabitModel
            {
                UserId = userId,
                HabitId = habit.Id,
                Goal = goal,
                IsActive = true
            };

            _context.UserHabits.Add(userHabit);
            _context.SaveChanges();

            _context.Entry(userHabit)
                .Reference(x => x.Habit)
                .Load();

            return userHabit;
        }

        public bool UpdateHabit(int userId, int userHabitId, int goal, string? newHabitName = null)
        {
            var userHabit = _context.UserHabits
                .Include(uh => uh.Habit)
                .FirstOrDefault(uh =>
                    uh.Id == userHabitId &&
                    uh.UserId == userId);

            if (userHabit == null) return false;

            userHabit.Goal = goal;

            if (!string.IsNullOrWhiteSpace(newHabitName) &&
                userHabit.Habit.IsSeed == false)
            {
                userHabit.Habit.Name = newHabitName.Trim();
            }

            _context.SaveChanges();
            return true;
        }

        public bool DeleteHabit(int userId, int userHabitId)
        {
            var userHabit = _context.UserHabits
                .Include(uh => uh.Habit)
                .FirstOrDefault(uh =>
                    uh.Id == userHabitId &&
                    uh.UserId == userId);

            if (userHabit == null) return false;

            var entries = _context.HabitEntries
                .Where(e => e.UserHabitId == userHabitId)
                .ToList();

            if (entries.Count > 0)
                _context.HabitEntries.RemoveRange(entries);

            _context.UserHabits.Remove(userHabit);

            if (!userHabit.Habit.IsSeed)
            {
                var stillUsed = _context.UserHabits
                    .Any(uh =>
                        uh.HabitId == userHabit.HabitId &&
                        uh.Id != userHabit.Id);

                if (!stillUsed)
                    _context.Habits.Remove(userHabit.Habit);
            }

            _context.SaveChanges();
            return true;
        }


        public List<HabitEntryModel> GetEntriesForMonth(int userId, int year, int month)
        {
            var monthStart = new DateOnly(year, month, 1);
            var monthEndExclusive = monthStart.AddMonths(1);

            var userHabitIds = _context.UserHabits
                .AsNoTracking()
                .Where(uh => uh.UserId == userId && uh.IsActive)
                .Select(uh => uh.Id)
                .ToList();

            if (userHabitIds.Count == 0)
                return new List<HabitEntryModel>();

            return _context.HabitEntries
                .AsNoTracking()
                .Where(e =>
                    userHabitIds.Contains(e.UserHabitId) &&
                    e.Date >= monthStart &&
                    e.Date < monthEndExclusive)
                .ToList();
        }

        public bool ToggleEntry(int userId, int userHabitId, DateOnly date)
        {
            var owns = _context.UserHabits
                .Any(uh => uh.Id == userHabitId && uh.UserId == userId);

            if (!owns) return false;

            var existing = _context.HabitEntries
                .FirstOrDefault(e =>
                    e.UserHabitId == userHabitId &&
                    e.Date == date);

            if (existing == null)
            {
                _context.HabitEntries.Add(new HabitEntryModel
                {
                    UserHabitId = userHabitId,
                    Date = date,
                    IsDone = true
                });
            }
            else
            {
                _context.HabitEntries.Remove(existing);
            }

            _context.SaveChanges();
            return true;
        }


        public void EnsureDefaultHabitsForUser(int userId)
        {
            var seedHabits = _context.Habits
                .AsNoTracking()
                .Where(h => h.IsSeed)
                .Select(h => new { h.Id, h.DefaultGoal })
                .ToList();

            if (seedHabits.Count == 0) return;

            var existingHabitIds = _context.UserHabits
                .AsNoTracking()
                .Where(uh => uh.UserId == userId)
                .Select(uh => uh.HabitId)
                .ToHashSet();

            var toAdd = seedHabits
                .Where(h => !existingHabitIds.Contains(h.Id))
                .Select(h => new UserHabitModel
                {
                    UserId = userId,
                    HabitId = h.Id,
                    Goal = h.DefaultGoal ?? 0,
                    IsActive = true
                })
                .ToList();

            if (toAdd.Count == 0) return;

            _context.UserHabits.AddRange(toAdd);
            _context.SaveChanges();
        }

        public Dictionary<int, HabitStatsModel> GetStatsForUserHabits(int userId, IEnumerable<int> userHabitIds)
        {
            var ids = userHabitIds.Distinct().ToList();
            if (ids.Count == 0) return new();

            var ownedIds = _context.UserHabits
                .AsNoTracking()
                .Where(uh => uh.UserId == userId && ids.Contains(uh.Id))
                .Select(uh => uh.Id)
                .ToHashSet();

            var today = DateOnly.FromDateTime(DateTime.Today);

            var done = _context.HabitEntries
                .AsNoTracking()
                .Where(e =>
                    ownedIds.Contains(e.UserHabitId) &&
                    e.IsDone &&
                    e.Date <= today)
                .Select(e => new { e.UserHabitId, e.Date })
                .Distinct()
                .ToList();

            var grouped = done
                .GroupBy(x => x.UserHabitId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.Date).OrderBy(d => d).ToList()
                );

            var result = new Dictionary<int, HabitStatsModel>();

            foreach (var id in ownedIds)
            {
                if (!grouped.TryGetValue(id, out var dates) || dates.Count == 0)
                {
                    result[id] = new HabitStatsModel();
                    continue;
                }

                int total = dates.Count;
                int longest = 1;
                int run = 1;

                for (int i = 1; i < dates.Count; i++)
                {
                    if (dates[i] == dates[i - 1].AddDays(1))
                    {
                        run++;
                        if (run > longest) longest = run;
                    }
                    else run = 1;
                }

                int current = 0;
                var set = new HashSet<DateOnly>(dates);
                var d0 = today;

                while (set.Contains(d0))
                {
                    current++;
                    d0 = d0.AddDays(-1);
                }

                result[id] = new HabitStatsModel
                {
                    TotalAccomplishments = total,
                    LongestStreak = longest,
                    CurrentStreak = current
                };
            }

            return result;
        }
    }
}
