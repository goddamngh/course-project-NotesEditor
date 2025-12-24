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
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(NotesEditorDbContext context) : base(context) { }
    }
}
