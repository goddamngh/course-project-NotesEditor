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
    public class TextRepository : BaseRepository<Text>, ITextRepository
    {
        public TextRepository(NotesEditorDbContext context) : base(context) { }
        public override List<Text> GetAll()
        {
            return _dbSet
                .Include(n => n.Note)
                .ToList();
        }
    }
}
