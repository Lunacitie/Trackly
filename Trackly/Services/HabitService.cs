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

            _context.Habits.Add(habit);
            _context.SaveChanges();

            return habit;
        }

        public bool UpdateHabit(int userId, HabitModel habit)
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
    }
}
