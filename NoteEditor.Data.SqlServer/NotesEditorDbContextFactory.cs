using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotesEditor.Data.SqlServer
{
    public class NotesEditorDbContextFactory : IDesignTimeDbContextFactory<NotesEditorDbContext>
    {
        public NotesEditorDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile("appsettings.database.json")
                                    .Build();
            return CreateDbContext(configuration);
        }
        public NotesEditorDbContext CreateDbContext(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var optionsBuilder = new DbContextOptionsBuilder<NotesEditorDbContext> ();
            optionsBuilder.UseSqlServer(connectionString);
            return new NotesEditorDbContext(optionsBuilder.Options);
        }
    }
}
