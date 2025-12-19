using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteEditor.Domain
{
    public class Category : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Color Color { get; set; }
        public User User { get; set; }

        public Category(User user)
        {
            Id = Guid.NewGuid();
            Name = string.Empty;
            Color = Color.White;
            User = user;
        }
        public Category(Guid id, string name, Color color, User user)
        {
            Id = id;
            Name = name;
            Color = color;
            User = user;
        }
    }
}
