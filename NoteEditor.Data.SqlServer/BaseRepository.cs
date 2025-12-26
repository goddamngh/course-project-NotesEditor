using Microsoft.EntityFrameworkCore;
using NoteEditor.Data.Interfaces;
using NoteEditor.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotesEditor.Data.SqlServer
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class, IEntity<Guid>
    {
        protected readonly NotesEditorDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public BaseRepository(NotesEditorDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public virtual List<TEntity> GetAll()
        {
            var query = _context.Set<TEntity>().AsQueryable();
            return query.ToList();
        }

        public virtual void Add(TEntity entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
        }

        public void Update(TEntity entity)
        {
            var existing = _dbSet.Find(entity.Id);
            if (existing == null) return;

            _context.Entry(existing).CurrentValues.SetValues(entity);

            _context.SaveChanges();
        }

        public void Delete(TEntity entity)
        {
            var resultEntity = _dbSet.Find(entity.Id);
            if (resultEntity == null)
                return;

            _dbSet.Remove(resultEntity);

            _context.SaveChanges();
        }
    }
}
