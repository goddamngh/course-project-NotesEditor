using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NoteEditor.Domain
{
    public class Picture : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public byte[] Data { get; set; }
        public int Index { get; set; }
        public Note Note { get; set; }
        public Picture(Note note)
        {
            Id = Guid.NewGuid();
            Data = File.ReadAllBytes("BaseImage.png");
            Index = 0;
            Note = note;
        }
    }
}
