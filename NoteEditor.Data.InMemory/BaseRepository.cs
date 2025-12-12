using NoteEditor.Data.Interfaces;
using NoteEditor.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteEditor.Data.InMemory
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class, IEntity<Guid>
    {
        protected readonly List<TEntity> _entities = new();
        public List<TEntity> GetAll()
        {
            return _entities.ToList();
        }

        public void Add(TEntity entity)
        {
            _entities.Add(entity);
        }

        public void Delete(TEntity entity)
        {
            _entities.RemoveAll(r => r.Id == entity.Id);
        }
        public void Update(TEntity entity)
        {
            var index = _entities.FindIndex(r => r.Id == entity.Id);
            if (index >= 0)
            {
                _entities[index] = entity;
            }
        }
    }
}
