using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoteEditor.Domain;
using System.Drawing;

namespace NotesEditor.Data.SqlServer
{
    public class NotesEditorDbContext : DbContext
    {
        public NotesEditorDbContext(DbContextOptions<NotesEditorDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Picture> Pictures { get; set; }
        public DbSet<Text> Texts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>()
                .Property(c => c.Color)
                .HasConversion(
                    c => ColorTranslator.ToHtml(c),
                    s => ColorTranslator.FromHtml(s)
                );
        }
    }
}