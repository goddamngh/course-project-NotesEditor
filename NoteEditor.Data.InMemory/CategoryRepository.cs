using NoteEditor.Data.Interfaces;
using NoteEditor.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteEditor.Data.InMemory
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository { }
}
