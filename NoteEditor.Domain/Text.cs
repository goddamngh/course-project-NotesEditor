using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteEditor.Domain
{
    public class Text : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public int Index { get; set; }
        public Note Note { get; set; }
        public Text(Note note)
        {
            Id = Guid.NewGuid();
            Content = string.Empty;
            Index = 0;
            Note = note;
        }
    }
}
