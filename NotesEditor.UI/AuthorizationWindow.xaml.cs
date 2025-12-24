using NoteEditor.Data.InMemory;
using NoteEditor.Data.Interfaces;
using NoteEditor.Domain;
using NotesEditor.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit.Primitives;

namespace NoteEditor.UI
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationWindow.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window
    {
        private readonly INoteRepository _noteRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IPictureRepository _pictureRepository;
        private readonly ITextRepository _textRepository;
        private readonly IUserRepository _userRepository;

        public AuthorizationWindow(INoteRepository noteRepository,
            ICategoryRepository categoryRepository,
            IPictureRepository pictureRepository,
            ITextRepository textRepository,
            IUserRepository userRepository)
        {
            InitializeComponent();
            _noteRepository = noteRepository;
            _categoryRepository = categoryRepository;
            _pictureRepository = pictureRepository;
            _textRepository = textRepository;
            _userRepository = userRepository;

        }

        private void CreateAccountButton_Click(object sender, RoutedEventArgs e)
        {
            string userName = UserNameTextBox.Text;
            string userPassword = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(userPassword))
            {
                MessageBox.Show(
                    "Пожалуйста, заполните все поля!",
                    "Пустые поля",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            var existingUser = _userRepository.GetAll()
                .FirstOrDefault(n => n.Username == userName);

            if (existingUser != null)
            {
                MessageBox.Show(
                    $"Пользователь с именем '{userName}' уже существует!\nПожалуйста, выберите другое имя.",
                    "Пользователь уже существует",
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation
                );
                return;
            }

            try
            {
                User user = new User(userName, userPassword);
                _userRepository.Add(user);

                MessageBox.Show(
                    $"Аккаунт '{userName}' успешно создан!",
                    "Успех",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );

            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при создании аккаунта:\n{ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UserNameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = _userRepository.GetAll()
                .FirstOrDefault(n => n.Username == username && n.Password == password);

            if (user == null)
            {
                MessageBox.Show("Неверный логин или пароль!", "Ошибка входа",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                PasswordBox.Clear();
                PasswordBox.Focus();
                return;
            }

            try
            {
                var mainWindow = new MainWindow(user, _userRepository, _noteRepository, _categoryRepository, _pictureRepository, _textRepository);
                if (mainWindow.ShowDialog() == false)
                {
                    this.Close();
                }
                else
                {
                    PasswordBox.Clear();
                    UserNameTextBox.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
