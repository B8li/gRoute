using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;

namespace gRouteTrack
{
    public class FullTimeConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TimeSpan valueTimeSpan = (TimeSpan)value;

            return valueTimeSpan.ToString("hh") + " h" + Environment.NewLine + valueTimeSpan.ToString("mm") + " min" + Environment.NewLine + valueTimeSpan.ToString("ss") + " sec";
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
