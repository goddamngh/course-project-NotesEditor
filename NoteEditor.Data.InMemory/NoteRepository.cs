using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoteEditor.Data.Interfaces;
using NoteEditor.Domain;

namespace NoteEditor.Data.InMemory
{
    public class NoteRepository : BaseRepository<Note>, INoteRepository 
    {
        public List<Note> GetAll(NoteFilter filter)
        {
            var result = _entities.AsEnumerable();

            if (filter.UserId.HasValue)
            {
                result = result.Where(n => n.User.Id == filter.UserId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.SearchText))
            {
                result = result.Where(n => n.Name != null &&
                    n.Name.Contains(filter.SearchText, StringComparison.OrdinalIgnoreCase));
            }

            if (filter.CategoryId.HasValue)
            {
                result = result.Where(n => n.Category != null &&
                    n.Category.Id == filter.CategoryId.Value);
            }

            return result.ToList();
        }
    }
}
