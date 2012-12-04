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
using System.ComponentModel;

namespace gRouteTrack.ViewModels
{
    /// <summary>
    /// Abstract class that implemts INotifyPropertyChanged interface.
    /// It's helper class for rasing PropertyChangedEventHandler event 
    /// </summary>
    public abstract class BaseNotify : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// This is helper method that wraps raising of PropertyChangedEventArgs event.
        /// </summary>
        /// <param name="propertyName">Name of the property for PropertyChangedEventArgs event</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            var eventHandler = PropertyChanged;

            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
