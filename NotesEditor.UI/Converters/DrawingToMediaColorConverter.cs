using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace NoteEditor.UI.Converters
{
    public class DrawingToMediaColorConverter : IValueConverter
    {
        // System.Drawing.Color -> System.Windows.Media.Color (для отображения в UI)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is System.Drawing.Color drawingColor)
            {
                return System.Windows.Media.Color.FromArgb(
                    drawingColor.A,
                    drawingColor.R,
                    drawingColor.G,
                    drawingColor.B);
            }
            return Colors.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is System.Windows.Media.Color mediaColor)
            {
                return System.Drawing.Color.FromArgb(
                    mediaColor.A,
                    mediaColor.R,
                    mediaColor.G,
                    mediaColor.B);
            }
            return System.Drawing.Color.White;
        }
    }
}
