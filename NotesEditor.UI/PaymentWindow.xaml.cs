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
using NoteEditor.Domain;

namespace NoteEditor.UI
{
    /// <summary>
    /// Логика взаимодействия для PaymentWindow.xaml
    /// </summary>
    public partial class PaymentWindow : Window
    {
        User _currentUser {  get; set; }
        public PaymentWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void PayButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser.Password == PasswordBox.Password)
            {
                _currentUser.IsVip = true;
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show(
                    "Неверный пароль. Пожалуйста, попробуйте снова.",
                    "Ошибка оплаты",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }

        }
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(c => char.IsDigit(c) || c == '/');
        }
    }
}
