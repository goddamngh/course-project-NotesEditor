using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoteEditor.Data.Interfaces;
using NoteEditor.Domain;

namespace NoteEditor.Data.InMemory
{
    public class NoteRepository : BaseRepository<Note>, INoteRepository { }
}
