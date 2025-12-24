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

        //конструктор для EF
        protected User() { }

        public User(string name, string password)
        {
            Id = Guid.NewGuid();
            Username = name;
            Password = password;
            IsVip = false;
            RegistrationDate = DateTime.Now;
        }
    }
}
