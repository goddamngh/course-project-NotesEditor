using NoteEditor.Data.InMemory;
using NoteEditor.Data.Interfaces;
using NoteEditor.Domain;
using NoteEditor.UI;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NotesEditor.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly INoteRepository _repository = new NoteRepository();
        private readonly ICategoryRepository _categoryRepository = new CategoryRepository();
        private readonly IPictureRepository _pictureRepository = new PictureRepository();
        private readonly ITextRepository _textRepository = new TextRepository();
        private readonly IUserRepository _userRepository = new UserRepository();

        public List<Note> Items { get; set; }
        public Note? SelectedItem { get; set; }
        public User _currentUser = new User { Id = Guid.NewGuid(), Username = "DemoUser" };
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            UpdateMainList();
            UpdateCategoryComboBox();
        }

        private void AddNoteButton_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new NoteEditWindow(_currentUser, _repository, _categoryRepository, _pictureRepository, _textRepository);

            if (editWindow.ShowDialog() == true)
            {
                UpdateMainList();
                UpdateCategoryComboBox();
            }
        }

        private void ChangeNoteButton_Click(object sender, RoutedEventArgs e)
        {

            var editWindow = new NoteEditWindow(SelectedItem, _repository, _categoryRepository, _pictureRepository, _textRepository);

            if (editWindow.ShowDialog() == true)
            {
                UpdateMainList();
                UpdateCategoryComboBox();
            }
        }

        private void DeleteNoteButton_Click(object sender, RoutedEventArgs e)
        {

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить заметку: \"{SelectedItem.Name}\"?\n\n" +
                $"ВНИМАНИЕ: Будут также удалены все текстовые блоки и изображения, связанные с этой заметкой.",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var relatedTexts = _textRepository.GetAll()
                        .Where(t => t.Note.Id == SelectedItem.Id)
                        .ToList();

                    foreach (var text in relatedTexts)
                    {
                        _textRepository.Delete(text);
                    }

                    var relatedPictures = _pictureRepository.GetAll()
                        .Where(p => p.Note.Id == SelectedItem.Id)
                        .ToList();

                    foreach (var picture in relatedPictures)
                    {
                        _pictureRepository.Delete(picture);
                    }

                    _repository.Delete(SelectedItem);

                    SelectedItem = null;
                    UpdateMainList();

                    MessageBox.Show(
                        $"Заметка успешно удалена.\n" +
                        $"Удалено: {relatedTexts.Count} текстовых блоков, {relatedPictures.Count} изображений.",
                        "Удаление завершено",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Произошла ошибка при удалении: {ex.Message}",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private void FilterNoteButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateMainList();
        }

        private void ShowNoteButton_Click(object sender, RoutedEventArgs e)
        {
            Window viewWindow = new NoteViewWindow(SelectedItem, _textRepository, _pictureRepository, _categoryRepository);
            viewWindow.Show();
        }

        void UpdateMainList()
        {
            MainList.ItemsSource = null;

            var filter = new NoteFilter
            {
                UserId = _currentUser.Id,
                SearchText = FindNoteTextBox.Text?.Trim(),
                CategoryId = GetSelectedCategoryId()
            };

            var filteredNotes = _repository.GetAll(filter);

            Items = new List<Note>(filteredNotes.OrderByDescending(n => n.CreationDate));
            MainList.ItemsSource = Items;



            var filterOnlyUser = new NoteFilter
            {
                UserId = _currentUser.Id
            };
            if (_currentUser.IsVip == false && _repository.GetAll(filterOnlyUser).Count() >= 10)
            {
                AddNoteButton.IsEnabled = false;
                MessageBox.Show(
                        "Вы достигли лимита бесплатных заметок (10 шт.)\n\n" +
                        "Для создания большего количества заметок необходимо оформить VIP-подписку.\n" +
                        "Оплатите подписку в разделе 'Аккаунт'.",
                        "Ограничение бесплатной версии",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
            }
            else
                AddNoteButton.IsEnabled = true;
        }

        private void AccountButton_Click(object sender, RoutedEventArgs e)
        {
            var accountWindow = new AccountWindow(_currentUser, _userRepository);

            if (accountWindow.ShowDialog() == false)
            {
                this.Close();
            }
            else
                UpdateMainList();
        }

        private void MainList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedItem = MainList.SelectedItem as Note;

            ShowNoteButton.IsEnabled = SelectedItem != null;
            ChangeNoteButton.IsEnabled = SelectedItem != null;
            DeleteNoteButton.IsEnabled = SelectedItem != null;
        }

        protected override void OnClosed(EventArgs e)
        {
            MainList.SelectionChanged -= MainList_SelectionChanged;
            base.OnClosed(e);
        }
        private void UpdateCategoryComboBox()
        {
            CategoryFilterComboBox.Items.Clear();

            var allItem = new
            {
                Name = "Все",
                Color = System.Drawing.Color.Transparent
            };

            CategoryFilterComboBox.Items.Add(allItem);

            var userCategories = _categoryRepository.GetAll()
                .Where(c => c.User == _currentUser)
                .ToList();

            foreach (var category in userCategories)
            {
                CategoryFilterComboBox.Items.Add(category);
            }

            CategoryFilterComboBox.SelectedIndex = 0;
        }
        private Guid? GetSelectedCategoryId()
        {
            if (CategoryFilterComboBox.SelectedIndex == 0)
                return null;

            if (CategoryFilterComboBox.SelectedItem is Category category)
                return category.Id;

            return null;
        }
    }
}