using Microsoft.Win32;
using NoteEditor.Data.InMemory;
using NoteEditor.Data.Interfaces;
using NoteEditor.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.WebRequestMethods;

namespace NoteEditor.UI
{
    /// <summary>
    /// Логика взаимодействия для NoteEditWindow.xaml
    /// </summary>
    public partial class NoteEditWindow : Window
    {
        private UIElement? _selectedElement;

        private readonly INoteRepository _noteRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITextRepository _textRepository;
        private readonly IPictureRepository _pictureRepository;

        private Note _currentNote;
        private readonly User _currentUser;
        private readonly bool _isNewNote = true;

        private readonly List<Text> _textComponents = new();
        private readonly List<Picture> _pictureComponents = new();



        //public List<object> Components { get; private set; }


        public NoteEditWindow(User user, INoteRepository noteRepository, ICategoryRepository categoryRepository,
            IPictureRepository pictureRepository, ITextRepository textRepository)
        {
            InitializeComponent();

            _noteRepository = noteRepository;
            _textRepository = textRepository;
            _pictureRepository = pictureRepository;
            _categoryRepository = categoryRepository;

            _currentUser = user;
            _currentNote = new Note(_currentUser);
            DataContext = _currentNote;


            UpdateCategoryUI();

        }
        public NoteEditWindow(Note note, INoteRepository noteRepository, ICategoryRepository categoryRepository,
            IPictureRepository pictureRepository, ITextRepository textRepository)
        {
            InitializeComponent();

            _noteRepository = noteRepository;
            _textRepository = textRepository;
            _pictureRepository = pictureRepository;
            _categoryRepository = categoryRepository;

            _currentNote = note;
            _isNewNote = false;
            DataContext = _currentNote;

            UpdateCategoryUI();


            LoadNoteContent();

        }
        /// <summary>
        /// метод для загрузки содержимого заметки из репозиториев и отображения его в окне редактирования
        /// </summary>
        private void LoadNoteContent()
        {
            _textComponents.AddRange(_textRepository.GetAll()
                                    .Where(t => t.Note.Id == _currentNote.Id)
                                    .ToList());

            _pictureComponents.AddRange(_pictureRepository.GetAll()
                                        .Where(p => p.Note.Id == _currentNote.Id)
                                        .ToList());

            var allComponents = new List<object>();
            allComponents.AddRange(_textComponents);
            allComponents.AddRange(_pictureComponents);

            var sortedComponents = allComponents
                .OrderBy(c => c is Text t ? t.Index : (c as Picture)?.Index)
                .ToList();

            //Components = sortedComponents;

            foreach (var component in sortedComponents)
            {
                if (component is Text text)
                {
                    ContentStackPanel.Children.Add(CreateTextBox(text));
                }
                else if (component is Picture picture)
                {
                    ContentStackPanel.Children.Add(CreateImageControl(picture));
                }
            }
        }


        private void AddTextButton_Click(object sender, RoutedEventArgs e)
        {
            var newIndex = ContentStackPanel.Children.Count;
            var text = new Text(_currentNote) { Index = newIndex };
            _textComponents.Add(text);
            ContentStackPanel.Children.Add(CreateTextBox(text));
        }

        private void AddPictureButton_Click(object sender, RoutedEventArgs e)
        {
            var newIndex = ContentStackPanel.Children.Count;

            string PicturePath = GetFilePathWithDialog();

            if (string.IsNullOrEmpty(PicturePath))
            {
                return;
            }

            var picture = new Picture(_currentNote, PicturePath) { Index = newIndex };
            _pictureComponents.Add(picture);

            var imageControl = CreateImageControl(picture);
            ContentStackPanel.Children.Add(imageControl);
        }
        /// <summary>
        /// метод для открытия диалога выбора файла и получения пути к выбранному изображению
        /// </summary>
        /// <returns></returns>
        private string GetFilePathWithDialog()
        {
            var openFileDialog = new OpenFileDialog();

            openFileDialog.Title = "Выберите изображение";
            openFileDialog.Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Все файлы|*.*";
            openFileDialog.Multiselect = false;
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            
            bool? result = openFileDialog.ShowDialog();

            return result == true ? openFileDialog.FileName : null;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            int index = 0;
            foreach (UIElement child in ContentStackPanel.Children)
            {
                if (child is TextBox textBox && textBox.Tag is Guid textId)
                {
                    var textComponent = _textComponents.FirstOrDefault(t => t.Id == textId);
                    if (textComponent != null)
                    {
                        textComponent.Content = textBox.Text;
                        textComponent.Index = index;
                    }
                    index++;
                }
                else if (child is Border border && border.Tag is Guid pictureId)
                {
                    var pictureComponent = _pictureComponents.FirstOrDefault(p => p.Id == pictureId);
                    if (pictureComponent != null)
                    {
                        pictureComponent.Index = index;
                    }
                    index++;
                }
            }

            foreach (var text in _textComponents)
            {
                if (_textRepository.GetAll().Any(t => t.Id == text.Id))
                {
                    _textRepository.Update(text);
                }
                else
                {
                    _textRepository.Add(text);
                }
            }

            foreach (var picture in _pictureComponents)
            {
                if (_pictureRepository.GetAll().Any(p => p.Id == picture.Id))
                {
                    _pictureRepository.Update(picture);
                }
                else
                {
                    _pictureRepository.Add(picture);
                }
            }

            if (NoteNameTextBox.Text == "")
                _currentNote.Name = null;
            else
                _currentNote.Name = NoteNameTextBox.Text;

            if (_isNewNote)
            {
                _noteRepository.Add(_currentNote);
            }
            else
            {
                _noteRepository.Update(_currentNote);
            }

            this.DialogResult = true;
            this.Close();
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Вы уверены, что хотите отменить изменения? Все несохраненные данные будут потеряны.",
                "Подтверждение отмены",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result == MessageBoxResult.Yes)
            {
                this.DialogResult = false;
                this.Close();
            }
        }

        /// <summary>
        /// метод для создания текстового поля на основе текстового компонента заметки
        /// </summary>
        /// <param name="textEntity"></param>
        /// <returns></returns>
        private TextBox CreateTextBox(Text textEntity)
        {
            var textBox = new TextBox
            {
                Text = textEntity.Content,
                AcceptsReturn = true,
                TextWrapping = TextWrapping.Wrap,
                MinHeight = MinHeight,
                Tag = textEntity.Id,
                Margin = new Thickness(0, 5, 0, 5),
                BorderBrush = SystemColors.ControlDarkBrush,
                BorderThickness = new Thickness(1)
            };

            textBox.GotFocus += (s, e) =>
            {
                _selectedElement = textBox;
                HighlightSelectedElement(textBox);
            };

            textBox.PreviewMouseDown += (s, e) =>
            {
                _selectedElement = textBox;
                HighlightSelectedElement(textBox);
            };

            return textBox;
        }

        /// <summary>
        /// метод для создания элемента управления изображением на основе компонента изображения заметки
        /// </summary>
        /// <param name="pictureEntity"></param>
        /// <returns></returns>
        private Border CreateImageControl(Picture pictureEntity)
        {
            var grid = new Grid()
            {
                Tag = pictureEntity.Id,
                Margin = new Thickness(5),
                Background = Brushes.Transparent
            };

            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            var changeImageButton = new Button
            {
                Content = "Изменить картинку",
                Tag = pictureEntity.Id,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 0, 5, 0)
            };
            changeImageButton.Click += ChangeImageButton_Click;

            Grid.SetColumn(changeImageButton, 0);
            grid.Children.Add(changeImageButton);

            var image = new Image
            {
                Stretch = Stretch.Uniform,
                MaxWidth = 700,
                MaxHeight = 600,
                Tag = pictureEntity.Id
            };

            LoadImageToImageControl(image, pictureEntity.Data);

            Grid.SetColumn(image, 1);
            grid.Children.Add(image);

            var border = new Border
            {
                BorderBrush = Brushes.Transparent,
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(3),
                Margin = new Thickness(5),
                Child = grid
            };

            border.PreviewMouseDown += (s, e) =>
            {
                _selectedElement = border;
                HighlightSelectedElement(border);
            };

            border.Tag = pictureEntity.Id;

            return border;
        }

        /// <summary>
        /// метод для загрузки изображения из массива байтов в элемент управления Image
        /// </summary>
        /// <param name="image"></param>
        /// <param name="imageData"></param>
        private void LoadImageToImageControl(Image image, byte[] imageData)
        {
            using (var stream = new MemoryStream(imageData))
            {
                var imageSource = new BitmapImage();
                imageSource.BeginInit();
                imageSource.StreamSource = stream;
                imageSource.CacheOption = BitmapCacheOption.OnLoad;
                imageSource.EndInit();
                imageSource.Freeze();

                image.Source = imageSource;
            }
        }

        private void ChangeImageButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Guid pictureId)
            {
                var picture = _pictureComponents.FirstOrDefault(p => p.Id == pictureId);
                if (picture == null) return;

                var openFileDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Title = "Выберите новое изображение",
                    Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Все файлы|*.*",
                    Multiselect = false,
                    CheckFileExists = true,
                    CheckPathExists = true
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        byte[] newImageData = System.IO.File.ReadAllBytes(openFileDialog.FileName);

                        picture.Data = newImageData;

                        var border = FindParentBorder(button);
                        if (border != null)
                        {
                            var grid = border.Child as Grid;
                            if (grid != null)
                            {
                                var image = grid.Children
                                    .OfType<Image>()
                                    .FirstOrDefault(i => i.Tag as Guid? == pictureId);

                                if (image != null)
                                {
                                    LoadImageToImageControl(image, newImageData);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        /// <summary>
        /// вспомогательный метод для поиска родительского элемента Border для заданного дочернего элемента
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        private Border FindParentBorder(DependencyObject child)
        {
            while (child != null && !(child is Border))
            {
                child = VisualTreeHelper.GetParent(child);
            }
            return child as Border;
        }

        private void ChoiceCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            var choiceCategoryWindow = new ChoiceCategoryWindow(_currentNote, _categoryRepository, _noteRepository);

            if (choiceCategoryWindow.ShowDialog() == true)
            {
                UpdateCategoryUI();
            }
        }

        private void UpdateCategoryUI()
        {
            if (_currentNote?.Category != null)
            {
                CategoryNameTextBox.Text = _currentNote.Category.Name;

                System.Drawing.Color drawingColor = _currentNote.Category.Color;

                System.Windows.Media.Color mediaColor = System.Windows.Media.Color.FromArgb(
                    drawingColor.A,
                    drawingColor.R,
                    drawingColor.G,
                    drawingColor.B);

                CategoryColorRectangle.Fill = new SolidColorBrush(mediaColor);
                CleanCategoryButton.IsEnabled = true;
            }
            else
            {
                CategoryNameTextBox.Text = "Не выбрана";
                CategoryColorRectangle.Fill = System.Windows.Media.Brushes.Transparent;
                CleanCategoryButton.IsEnabled = false;
            }
        }

        private void DeleteSelectedElButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedElement is TextBox textBox)
            {
                DeleteTextElement(textBox);
            }
            else if (_selectedElement is Border border)
            {
                DeletePictureElement(border);
            }

            _selectedElement = null;
            DeleteSelectedElButton.IsEnabled = false;
        }

        /// <summary>
        /// метод для удаления текстового элемента из заметки и интерфейса
        /// </summary>
        /// <param name="textBox"></param>
        private void DeleteTextElement(TextBox textBox)
        {
            if (textBox.Tag is Guid textId)
            {
                var result = MessageBox.Show(
                    "Вы уверены, что хотите удалить этот текстовый блок?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var text = _textComponents.FirstOrDefault(t => t.Id == textId);
                    if (text != null)
                    {
                        _textComponents.Remove(text);
                    }

                    ContentStackPanel.Children.Remove(textBox);

                    UpdateComponentIndices();

                    MessageBox.Show("Текстовый блок успешно удален.",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        /// <summary>
        /// метод для удаления элемента изображения из заметки и интерфейса
        /// </summary>
        /// <param name="border"></param>
        private void DeletePictureElement(Border border)
        {
            if (border.Tag is Guid pictureId)
            {
                var result = MessageBox.Show(
                    "Вы уверены, что хотите удалить это изображение?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var picture = _pictureComponents.FirstOrDefault(p => p.Id == pictureId);
                    if (picture != null)
                    {
                        _pictureComponents.Remove(picture);
                    }

                    ContentStackPanel.Children.Remove(border);

                    UpdateComponentIndices();

                    MessageBox.Show("Изображение успешно удалено.",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        /// <summary>
        /// метод для обновления индексов всех компонентов заметки после удаления элемента
        /// </summary>
        private void UpdateComponentIndices()
        {
            int index = 0;
            foreach (UIElement child in ContentStackPanel.Children)
            {
                if (child is TextBox textBox && textBox.Tag is Guid textId)
                {
                    var text = _textComponents.FirstOrDefault(t => t.Id == textId);
                    if (text != null)
                    {
                        text.Index = index;
                    }
                    index++;
                }
                else if (child is Border border && border.Tag is Guid pictureId)
                {
                    var picture = _pictureComponents.FirstOrDefault(p => p.Id == pictureId);
                    if (picture != null)
                    {
                        picture.Index = index;
                    }
                    index++;
                }
            }
        }

        /// <summary>
        /// метод для выделения выбранного элемента в интерфейсе
        /// </summary>
        /// <param name="element"></param>
        private void HighlightSelectedElement(UIElement element)
        {
            ClearAllHighlights();
            DeleteSelectedElButton.IsEnabled = true;
            if (element is TextBox textBox)
            {
                textBox.BorderBrush = Brushes.Blue;
                textBox.BorderThickness = new Thickness(2);
            }
            else if (element is Border border)
            {
                border.BorderBrush = Brushes.Blue;
            }
        }

        /// <summary>
        /// вспомогательный метод для снятия выделения со всех элементов в интерфейсе
        /// </summary>
        private void ClearAllHighlights()
        {
            foreach (UIElement child in ContentStackPanel.Children)
            {
                if (child is TextBox textBox)
                {
                    textBox.BorderBrush = SystemColors.ControlDarkBrush;
                    textBox.BorderThickness = new Thickness(1);
                }
                else if (child is Border border)
                {
                    border.BorderBrush = Brushes.Transparent;
                }
            }
        }

        private void CleanCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            _currentNote.Category = null;
            UpdateCategoryUI();
        }
    }
}
