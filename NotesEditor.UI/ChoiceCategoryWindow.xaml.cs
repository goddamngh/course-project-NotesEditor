using NoteEditor.Data.InMemory;
using NoteEditor.Data.Interfaces;
using NoteEditor.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NoteEditor.UI
{
    /// <summary>
    /// Логика взаимодействия для ChoiceCategoryWindow.xaml
    /// </summary>
    public partial class ChoiceCategoryWindow : Window
    {
        private Note _currentNote;
        private readonly ICategoryRepository _categoryRepository;
        private readonly INoteRepository _noteRepository;
        public List<Category> Items { get; set; }
        public Category? SelectedItem { get; set; }

        public ChoiceCategoryWindow(Note note, ICategoryRepository categoryRepository, INoteRepository noteRepository)
        {
            InitializeComponent();

            _currentNote = note;
            _categoryRepository = categoryRepository;
            _noteRepository = noteRepository;

            DataContext = this;
            UpdateMainList();
        }

        void UpdateMainList()
        {
            CategoryList.ItemsSource = null;

            var itemsToDisplay = _categoryRepository.GetAll()
                .Where(n => n.User.Id == _currentNote.User.Id)
                .ToList();

            Items = new List<Category>(itemsToDisplay.OrderByDescending(n => n.Name));

            CategoryList.ItemsSource = Items;
        }

        private void ChoiceButton_Click(object sender, RoutedEventArgs e)
        {
            _currentNote.Category = SelectedItem;
            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void AddNewCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            var createCategoryWindow = new CreateCategoryWindow(_categoryRepository, _currentNote.User);

            if (createCategoryWindow.ShowDialog() == true)
            {
                UpdateMainList();
            }
        }

        private void DeleteCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var note in _noteRepository.GetAll().Where(n => n.Category != null && n.Category.Id == SelectedItem.Id).ToList())
            {
                note.Category = null;
                _noteRepository.Update(note);
            }

            _categoryRepository.Delete((Category)CategoryList.SelectedItem);
            UpdateMainList();
        }

        private void CategoryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = CategoryList.SelectedItem as Category;
            SelectedItem = selected;

            bool isSelected = selected != null;

            ChangeCategoryButton.IsEnabled = isSelected;
            DeleteCategoryButton.IsEnabled = isSelected;
            ChoiceButton.IsEnabled = isSelected;
        }

        protected override void OnClosed(EventArgs e)
        {
            CategoryList.SelectionChanged -= CategoryList_SelectionChanged;
            base.OnClosed(e);
        }

        private void ChangeCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            var createCategoryWindow = new CreateCategoryWindow(_categoryRepository, SelectedItem);

            if (createCategoryWindow.ShowDialog() == true)
            {
                UpdateMainList();
            }
        }
    }
}
