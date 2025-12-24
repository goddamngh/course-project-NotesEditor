using Microsoft.EntityFrameworkCore;
using NoteEditor.Data.Interfaces;
using NoteEditor.Domain;
using NotesEditor.Data.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteEditor.Data.SqlServer
{
    public class NoteRepository : BaseRepository<Note>, INoteRepository
    {
        public NoteRepository(NotesEditorDbContext context) : base(context) { }
        public override void Add(Note entity)
        {
            if (entity.User != null)
            {
                entity.UserId = entity.User.Id;
            }

            if (entity.Category != null)
            {
                entity.CategoryId = entity.Category.Id;
            }

            entity.User = null!;
            entity.Category = null!;

            _dbSet.Add(entity);
            _context.SaveChanges();
        }
        public List<Note> GetAll(NoteFilter filter)
        {
            var query = _dbSet
                .Include(n => n.Category)
                .Include(n => n.User)
                .AsQueryable();


            if (filter.UserId.HasValue)
            {
                query = query.Where(n => n.User.Id == filter.UserId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.SearchText))
            {
                query = query.Where(n => n.Name.Contains(filter.SearchText));
            }

            if (filter.CategoryId.HasValue)
            {
                query = query.Where(n => n.Category.Id == filter.CategoryId.Value);
            }

            return query.ToList();
        }
    }
}
