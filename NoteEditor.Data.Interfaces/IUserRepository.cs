using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoteEditor.Domain;

namespace NoteEditor.Data.Interfaces
{
    public interface IUserRepository : IBaseRepository<User> { }
}
