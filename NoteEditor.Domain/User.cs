using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteEditor.Domain
{
    public class User : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsVip { get; set; }
        public DateTime RegistrationDate { get; set; } 

        public User()
        {
            Id = Guid.NewGuid();
            Username = string.Empty;
            Password = string.Empty;
            IsVip = false;
            RegistrationDate = DateTime.Now;
        }
    }
}
