using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NoteEditor.Data.InMemory;
using NoteEditor.Data.Interfaces;
using NoteEditor.UI;
using NotesEditor.Data.SqlServer;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace NotesEditor.UI
{
    public partial class App : Application
    {
        private INoteRepository _noteRepository = null!;
        private ICategoryRepository _categoryRepository = null!;
        private IPictureRepository _pictureRepository = null!;
        private ITextRepository _textRepository = null!;
        private IUserRepository _userRepository = null!;

        private NotesEditorDbContext _dbContext = null!;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.database.json")
                    .Build();

                var factory = new NotesEditorDbContextFactory();
                _dbContext = factory.CreateDbContext(configuration);

                _dbContext.Database.Migrate();

                _noteRepository = new NoteEditor.Data.SqlServer.NoteRepository(_dbContext);
                _categoryRepository = new NoteEditor.Data.SqlServer.CategoryRepository(_dbContext);
                _pictureRepository = new NoteEditor.Data.SqlServer.PictureRepository(_dbContext);
                _textRepository = new NoteEditor.Data.SqlServer.TextRepository(_dbContext);
                _userRepository = new NoteEditor.Data.SqlServer.UserRepository(_dbContext);

                var mainWindow = new AuthorizationWindow(_noteRepository, _categoryRepository, _pictureRepository, _textRepository, _userRepository);
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка подключения к базе данных MS SQL Server.\n\n" +
                    $"Убедитесь, что служба SQL Server запущена и строка подключения в 'appsettings.database.json' верна.\n\n" +
                    $"Техническая информация: {ex.Message}",
                    "Критическая ошибка запуска",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Shutdown();
            }
        }
        protected override void OnExit(ExitEventArgs e)
        {
            _dbContext?.Dispose();
            base.OnExit(e);
        }
    }
}
