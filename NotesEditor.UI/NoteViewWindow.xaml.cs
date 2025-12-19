using NoteEditor.Data.Interfaces;
using NoteEditor.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NoteEditor.UI
{
    public partial class NoteViewWindow : Window
    {
        private readonly Note _currentNote;
        private readonly ITextRepository _textRepository;
        private readonly IPictureRepository _pictureRepository;

        public NoteViewWindow(Note note, ITextRepository textRepository, IPictureRepository pictureRepository, ICategoryRepository categoryRepository)
        {
            InitializeComponent();

            _currentNote = note;
            _textRepository = textRepository;
            _pictureRepository = pictureRepository;

            DataContext = _currentNote;

            LoadNoteContent();
        }
        private void LoadNoteContent()
        {
            ContentStackPanel.Children.Clear();

            var textComponents = _textRepository.GetAll()
                .Where(t => t.Note.Id == _currentNote.Id)
                .ToList();

            var pictureComponents = _pictureRepository.GetAll()
                .Where(p => p.Note.Id == _currentNote.Id)
                .ToList();

            var allComponents = new List<object>();
            allComponents.AddRange(textComponents);
            allComponents.AddRange(pictureComponents);

            var sortedComponents = allComponents
                .OrderBy(c => c is Text t ? t.Index : (c as Picture)?.Index)
                .ToList();

            foreach (var component in sortedComponents)
            {
                if (component is Text text)
                {
                    var textBlock = CreateTextBlock(text);
                    if (textBlock != null)
                        ContentStackPanel.Children.Add(textBlock);
                }
                else if (component is Picture picture)
                {
                    var image = CreateImageControl(picture);
                    if (image != null)
                        ContentStackPanel.Children.Add(image);
                }
            }
        }

        private TextBlock CreateTextBlock(Text text)
        {
            var textBlock = new TextBlock
            {
                Text = text.Content ?? string.Empty,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(15),
                HorizontalAlignment = HorizontalAlignment.Left
            };

            return textBlock;
        }

        private Image CreateImageControl(Picture picture)
        {
            if (picture.Data == null || picture.Data.Length == 0)
                return null;

            try
            {
                var image = new Image
                {
                    Stretch = Stretch.Uniform,
                    MaxWidth = 700,
                    Margin = new Thickness(15),
                };

                using (var stream = new MemoryStream(picture.Data))
                {
                    var imageSource = new BitmapImage();
                    imageSource.BeginInit();
                    imageSource.StreamSource = stream;
                    imageSource.CacheOption = BitmapCacheOption.OnLoad;
                    imageSource.EndInit();
                    imageSource.Freeze();
                    image.Source = imageSource;
                }

                return image;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}