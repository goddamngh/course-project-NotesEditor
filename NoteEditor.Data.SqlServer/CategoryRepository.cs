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
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(NotesEditorDbContext context) : base(context) { }
        public override List<Category> GetAll()
        {
            return _dbSet
                .Include(n => n.User)
                .ToList();
        }
    }
}
