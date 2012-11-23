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

namespace WinPhoneApp
{
    public class GPointEventArgs:EventArgs 
    {
        private GPoint _gPoint;
        public GPointEventArgs(GPoint point)
        {
            _gPoint = point;
        }

        public GPoint Point
        {
            get
            {
                return this._gPoint;
            }
        }

    }
}
