using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using TraderPerformanceComparer.Assets;

namespace TraderPerformanceComparer.Assets
{
    public class WinnerHighLightConvertor : IValueConverter
    {
        public SolidColorBrush GREEN = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3EAE2D"));
        Brush brush;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {


            WinnerHighlightStruct input = (WinnerHighlightStruct)value;
            if(input.participant1UserId==input.winneruserId)
            {
                return GREEN;
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
