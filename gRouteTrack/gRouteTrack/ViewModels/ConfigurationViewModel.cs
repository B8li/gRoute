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
using gRouteTrack.Model;
namespace gRouteTrack.ViewModels
{
    public class ConfigurationViewModel:BaseNotify
    {
        public double DistanceInMeters
        {
            get
            {
                return gSystemSettings.DistanceInMeters; 
            }
            set
            {
                gSystemSettings.DistanceInMeters = value;
                RaisePropertyChanged("DistanceInMeters");
            }
        }

        public string UserName
        {
            get
            {
                return gSystemSettings.UserName;
            }
            set
            {
                gSystemSettings.UserName = value;
                RaisePropertyChanged("UserName");
            }
        }

        public string Password
        {
            get
            {
                return gSystemSettings.Password;
            }
            set
            {
                gSystemSettings.Password = value;
                RaisePropertyChanged("Password");
            }
        }
    }
}
