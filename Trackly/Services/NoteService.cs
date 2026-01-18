using Microsoft.EntityFrameworkCore;
using Trackly.Models;

namespace Trackly.Services
{
    public class NoteService
    {
        private readonly AppDbContext _context;

        public NoteService(AppDbContext context)
        {
            _context = context;
        }

        public List<NoteModel> GetNotesForMonth(int userId, int year, int month)
        {
            var monthStart = new DateOnly(year, month, 1);
            var monthEndExclusive = monthStart.AddMonths(1);

            return _context.Notes
                .AsNoTracking()
                .Where(n => n.UserId == userId && n.Date >= monthStart && n.Date < monthEndExclusive)
                .OrderByDescending(n => n.Date)
                .ToList();
        }

        public NoteModel? GetNoteByDate(int userId, DateOnly date)
        {
            return _context.Notes
                .AsNoTracking()
                .FirstOrDefault(n => n.UserId == userId && n.Date == date);
        }

        public void InsertNote(int userId, NoteModel note)
        {
            note.UserId = userId;
            _context.Notes.Add(note);
            _context.SaveChanges();
        }

        public void UpdateNote(int userId, NoteModel updatedNote)
        {
            var note = _context.Notes
                .FirstOrDefault(n => n.Id == updatedNote.Id && n.UserId == userId);

            if (note == null)
                return;

            note.Date = updatedNote.Date;
            note.Text = updatedNote.Text;

            _context.SaveChanges();
        }


        public bool DeleteNote(int userId, int noteId)
        {
            var note = _context.Notes.FirstOrDefault(n => n.Id == noteId && n.UserId == userId);
            if (note == null) return false;

            _context.Notes.Remove(note);
            _context.SaveChanges();
            return true;
        }

        public bool DeleteNoteByDate(int userId, DateOnly date)
        {
            var note = _context.Notes.FirstOrDefault(n => n.UserId == userId && n.Date == date);
            if (note == null) return false;

            _context.Notes.Remove(note);
            _context.SaveChanges();
            return true;
        }
    }
}
