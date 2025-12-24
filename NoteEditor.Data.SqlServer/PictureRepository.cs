using Microsoft.EntityFrameworkCore;
using NoteEditor.Data.Interfaces;
using NotesEditor.Data.SqlServer;
using NoteEditor.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteEditor.Data.SqlServer
{
    public class PictureRepository : BaseRepository<Picture>, IPictureRepository
    {
        public PictureRepository(NotesEditorDbContext context) : base(context) { }
        public override List<Picture> GetAll()
        {
            return _dbSet
                .Include(n => n.Note)
                .ToList();
        }
    }
}
