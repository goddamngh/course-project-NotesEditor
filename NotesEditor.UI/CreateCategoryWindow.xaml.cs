using NoteEditor.Data.Interfaces;
using NoteEditor.Domain;
using System;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
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
    /// Логика взаимодействия для CreateCategoryWindow.xaml
    /// </summary>
    public partial class CreateCategoryWindow : Window
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly Category _currentCategory;
        private readonly User _currentUser;

        public CreateCategoryWindow(ICategoryRepository categoryRepository, User user)
        {
            InitializeComponent();
            _categoryRepository = categoryRepository;

            _currentUser = user;
            _currentCategory = new Category(_currentUser);

            this.DataContext = _currentCategory;

        }
        public CreateCategoryWindow(ICategoryRepository categoryRepository, Category category)
        {
            InitializeComponent();
            _categoryRepository = categoryRepository;

            _currentUser = category.User;
            _currentCategory = category;

            MainTextBlock.Text = "Редактирование категории";
            SaveButton.Content = "Сохранить изменения";

            this.DataContext = _currentCategory;

            SaveButton.IsEnabled = !string.IsNullOrWhiteSpace(_currentCategory.Name);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_currentCategory.Name))
            {
                MessageBox.Show("Введите название категории.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                CategoryNameTextBox.Focus();
                return;
            }

            bool isExisting = _categoryRepository.GetAll()
                    .Any(c => c.Id == _currentCategory.Id && c.User.Id == _currentUser.Id);

            if (isExisting)
            {
                _categoryRepository.Update(_currentCategory);
            }
            else
            {
                _categoryRepository.Add(_currentCategory);
            }

            this.DialogResult = true;
            this.Close();
        }
        private void CategoryNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SaveButton.IsEnabled = !string.IsNullOrWhiteSpace(CategoryNameTextBox.Text);
        }
    }
}
