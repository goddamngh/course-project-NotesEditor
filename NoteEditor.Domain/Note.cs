using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteEditor.Domain
{
    public class Note : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public Category? Category { get; set; }
        public User User { get; set; }
        public DateTime CreationDate { get; set; }
        public Note(User user)
        {
            Id = Guid.NewGuid();
            User = user;
            CreationDate = DateTime.Now;
        }
    }
}
