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
    /// Логика взаимодействия для AccountWindow.xaml
    /// </summary>
    public partial class AccountWindow : Window
    {
        private readonly INoteRepository _noteRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IPictureRepository _pictureRepository;
        private readonly ITextRepository _textRepository;
        private readonly IUserRepository _userRepository;
        private User _currentUser {  get; set; }
        public AccountWindow(User user, IUserRepository userRepository, INoteRepository repository, ICategoryRepository categoryRepository,
            IPictureRepository pictureRepository, ITextRepository textRepository)
        {
            InitializeComponent();

            _currentUser = user;
            DataContext = _currentUser;
            _userRepository = userRepository;
            _noteRepository = repository;
            _categoryRepository = categoryRepository;
            _pictureRepository = pictureRepository;
            _textRepository = textRepository;

            UpdateIsVip();
        }

        private void BuyVipButton_Click(object sender, RoutedEventArgs e)
        {
            var paymentWindow = new PaymentWindow(_currentUser);

            if (paymentWindow.ShowDialog() == true)
            {
                UpdateIsVip();
                _userRepository.Update(_currentUser);
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void UpdateIsVip()
        {
            IsVipTextBlock.Text = _currentUser.IsVip ? "Есть" : "Нет";
            BuyVipButton.IsEnabled = !_currentUser.IsVip;
        }

        private void DeleteAccountButton_Click(object sender, RoutedEventArgs e)
        {
            _userRepository.Delete(_currentUser);

            _categoryRepository.GetAll()
                .Where(c => c.User.Id == _currentUser.Id)
                .ToList()
                .ForEach(c => _categoryRepository.Delete(c));

            var userNotes = _noteRepository.GetAll()
                .Where(c => c.User.Id == _currentUser.Id)
                .ToList();

            foreach (var note in userNotes)
            {
                var texts = _textRepository.GetAll()
                    .Where(t => t.Note.Id == note.Id)
                    .ToList();

                foreach (var text in texts)
                {
                    _textRepository.Delete(text);
                }

                var images = _pictureRepository.GetAll()
                    .Where(i => i.Note.Id == note.Id)
                    .ToList();

                foreach (var image in images)
                {
                    _pictureRepository.Delete(image);
                }

                _noteRepository.Delete(note);
            }

            this.DialogResult = false;
            this.Close();
        }
    }
}
