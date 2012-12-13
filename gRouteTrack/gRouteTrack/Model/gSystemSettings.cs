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

namespace gRouteTrack.Model
{
    public class gSystemSettings
    {
        public static int ComparingPointSeconds = 5;
        public static DateTime DefaultDate = new DateTime(1900,1,1);
        public static double DistanceInMeters = 10;
        public enum  GLocationServiceStatus
        {
            NotStarted,
            Started,
            Paused,
            Stopped
        }

        public static string UserName = "";
        public static string Password = "";
    }
}
