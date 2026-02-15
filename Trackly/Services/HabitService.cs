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

        public List<HabitModel> GetHabits(int userId)
        {
            return _context.Habits
                .AsNoTracking()
                .Where(h => h.UserId == userId)
                .OrderBy(h => h.Name)
                .ToList();
        }

        public HabitModel? GetHabit(int userId, int habitId)
        {
            return _context.Habits
                .AsNoTracking()
                .FirstOrDefault(h => h.Id == habitId && h.UserId == userId);
        }

        public HabitModel CreateHabit(int userId, HabitModel habit)
        {
            habit.Id = 0;
            habit.UserId = userId;

            var normalized = habitName.ToLower();

            var habit = _context.Habits.FirstOrDefault(h => h.IsSeed && h.Name.ToLower() == normalized);

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
                .FirstOrDefault(uh => uh.UserId == userId && uh.HabitId == habit.Id);

            if (existingUserHabit != null)
            {
                existingUserHabit.Goal = goal;
                existingUserHabit.IsActive = true;
                _context.SaveChanges();
                _context.Entry(existingUserHabit).Reference(x => x.Habit).Load();
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

            _context.Entry(userHabit).Reference(x => x.Habit).Load();
            return userHabit;
        }


        public bool UpdateHabit(int userId, int userHabitId, int goal, string? newHabitName = null)
        {
            var existing = _context.Habits.FirstOrDefault(h => h.Id == habit.Id && h.UserId == userId);
            if (existing == null) return false;

            existing.Name = habit.Name;
            existing.Goal = habit.Goal;

            _context.SaveChanges();
            return true;
        }

        public bool DeleteHabit(int userId, int habitId)
        {
            var habit = _context.Habits.FirstOrDefault(h => h.Id == habitId && h.UserId == userId);
            if (habit == null) return false;

            var entries = _context.HabitEntries.Where(e => e.HabitId == habitId).ToList();
            if (entries.Count > 0) _context.HabitEntries.RemoveRange(entries);

            _context.Habits.Remove(habit);
            _context.SaveChanges();
            return true;
        }

        public List<HabitEntryModel> GetEntriesForMonth(int userId, int year, int month)
        {
            var monthStart = new DateOnly(year, month, 1);
            var monthEndExclusive = monthStart.AddMonths(1);

            var habitIds = _context.Habits
                .AsNoTracking()
                .Where(h => h.UserId == userId)
                .Select(h => h.Id)
                .ToList();

            if (habitIds.Count == 0) return new List<HabitEntryModel>();

            return _context.HabitEntries
                .AsNoTracking()
                .Where(e => habitIds.Contains(e.HabitId) && e.Date >= monthStart && e.Date < monthEndExclusive)
                .ToList();
        }

        public bool ToggleEntry(int userId, int habitId, DateOnly date)
        {
            var ownsHabit = _context.Habits.Any(h => h.Id == habitId && h.UserId == userId);
            if (!ownsHabit) return false;

            var existing = _context.HabitEntries.FirstOrDefault(e => e.HabitId == habitId && e.Date == date);

            if (existing == null)
            {
                _context.HabitEntries.Add(new HabitEntryModel
                {
                    HabitId = habitId,
                    Date = date,
                    IsDone = true
                });
            }
            else _context.HabitEntries.Remove(existing);

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

            if (ownedIds.Count == 0) return new();

            var today = DateOnly.FromDateTime(DateTime.Today);

            var done = _context.HabitEntries
                .AsNoTracking()
                .Where(e => ownedIds.Contains(e.UserHabitId) && e.IsDone && e.Date <= today)
                .Select(e => new { e.UserHabitId, e.Date })
                .Distinct()
                .ToList();

            var grouped = done
                .GroupBy(x => x.UserHabitId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.Date).Distinct().OrderBy(d => d).ToList()
                );

            var result = new Dictionary<int, HabitStatsModel>();

            foreach (var id in ownedIds)
            {
                if (!grouped.TryGetValue(id, out var dates) || dates.Count == 0)
                {
                    result[id] = new HabitStatsModel();
                    continue;
                }

                var total = dates.Count;

                int longest = 1;
                int run = 1;

                for (int i = 1; i < dates.Count; i++)
                {
                    if (dates[i] == dates[i - 1].AddDays(1))
                    {
                        run++;
                        if (run > longest) longest = run;
                    }
                    else
                    {
                        run = 1;
                    }
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
